using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class OrdemServicoPecaDAO
{
    private readonly DatabaseConnection _database;

    public OrdemServicoPecaDAO(DatabaseConnection database)
    {
        _database = database;
    }

    // Lista todas as solicitações de peças
    public async Task<List<OrdemServicoPeca>> ListarTodasAsync()
    {
        var lista = new List<OrdemServicoPeca>();

        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT
                osp.id,
                osp.fk_ordem_servico_principal,
                os.numero_os,
                osp.fk_componente,
                c.nome AS componente_nome,
                osp.quantidade,
                osp.fk_urgencia,
                gu.nome AS urgencia_nome,
                osp.observacoes,
                osp.fk_status,
                s.nome AS status_nome,
                osp.data_solicitacao,
                osp.data_resolucao
            FROM ordem_servico_peca osp
            LEFT JOIN ordem_servico os
                ON os.id = osp.fk_ordem_servico_principal
            INNER JOIN componente c
                ON c.id = osp.fk_componente
            INNER JOIN grau_urgencia gu
                ON gu.id = osp.fk_urgencia
            INNER JOIN status_os s
                ON s.id = osp.fk_status
            ORDER BY osp.data_solicitacao DESC;";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    // Lista solicitações de uma OS específica
    public async Task<List<OrdemServicoPeca>> ListarPorOSAsync(int idOS)
    {
        var lista = new List<OrdemServicoPeca>();

        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT
                osp.id,
                osp.fk_ordem_servico_principal,
                os.numero_os,
                osp.fk_componente,
                c.nome AS componente_nome,
                osp.quantidade,
                osp.fk_urgencia,
                gu.nome AS urgencia_nome,
                osp.observacoes,
                osp.fk_status,
                s.nome AS status_nome,
                osp.data_solicitacao,
                osp.data_resolucao
            FROM ordem_servico_peca osp
            LEFT JOIN ordem_servico os
                ON os.id = osp.fk_ordem_servico_principal
            INNER JOIN componente c
                ON c.id = osp.fk_componente
            INNER JOIN grau_urgencia gu
                ON gu.id = osp.fk_urgencia
            INNER JOIN status_os s
                ON s.id = osp.fk_status
            WHERE osp.fk_ordem_servico_principal = @idOS
            ORDER BY osp.data_solicitacao DESC;";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idOS", idOS);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(Mapear(reader));
        }

        return lista;
    }

    // Cria uma nova solicitação
    public async Task<bool> SolicitarPecaAsync(OrdemServicoPeca solicitacao)
    {
        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
            INSERT INTO ordem_servico_peca
            (
                fk_ordem_servico_principal,
                fk_componente,
                quantidade,
                fk_urgencia,
                observacoes,
                fk_status
            )
            VALUES
            (
                @fkOS,
                @componente,
                @quantidade,
                @urgencia,
                @observacoes,
                1
            );";

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@fkOS",
            solicitacao.FkOrdemServicoPrincipal.HasValue
                ? solicitacao.FkOrdemServicoPrincipal.Value
                : DBNull.Value);

        command.Parameters.AddWithValue(
            "@componente",
            solicitacao.FkComponente);

        command.Parameters.AddWithValue(
            "@quantidade",
            solicitacao.Quantidade);

        command.Parameters.AddWithValue(
            "@urgencia",
            solicitacao.FkUrgencia);

        command.Parameters.AddWithValue(
            "@observacoes",
            solicitacao.Observacoes);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Aprova a solicitação:
    // 1. verifica estoque
    // 2. baixa estoque
    // 3. registra movimentação
    // 4. coloca solicitação como "Em Andamento"
    public async Task<bool> AprovarEBaixarEstoqueAsync(
        int idSolicitacao,
        int idUsuarioAprovador)
    {
        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            const string sqlSolicitacao = @"
                SELECT
                    fk_componente,
                    quantidade,
                    fk_ordem_servico_principal,
                    fk_status
                FROM ordem_servico_peca
                WHERE id = @id
                FOR UPDATE;";

            using var commandSolicitacao =
                new MySqlCommand(sqlSolicitacao, connection, transaction);

            commandSolicitacao.Parameters.AddWithValue("@id", idSolicitacao);

            using var reader = await commandSolicitacao.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                await transaction.RollbackAsync();
                return false;
            }

            int idComponente = reader.GetInt32("fk_componente");
            int quantidade = reader.GetInt32("quantidade");
            int statusAtual = reader.GetInt32("fk_status");

            int? idOS = reader.IsDBNull(reader.GetOrdinal("fk_ordem_servico_principal"))
                ? null
                : reader.GetInt32("fk_ordem_servico_principal");

            await reader.CloseAsync();

            // Só permite aprovar uma solicitação pendente
            if (statusAtual != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            // Consulta estoque
            const string sqlEstoque = @"
                SELECT
                    quantidade_estoque,
                    preco_unitario
                FROM componente
                WHERE id = @idComponente
                FOR UPDATE;";

            using var commandEstoque =
                new MySqlCommand(sqlEstoque, connection, transaction);

            commandEstoque.Parameters.AddWithValue("@idComponente", idComponente);

            using var estoqueReader = await commandEstoque.ExecuteReaderAsync();

            if (!await estoqueReader.ReadAsync())
            {
                await estoqueReader.CloseAsync();
                await transaction.RollbackAsync();
                return false;
            }

            int estoqueAtual = estoqueReader.GetInt32("quantidade_estoque");
            decimal precoUnitario = estoqueReader.GetDecimal("preco_unitario");

            await estoqueReader.CloseAsync();

            // Não possui quantidade suficiente
            if (estoqueAtual < quantidade)
            {
                await transaction.RollbackAsync();
                return false;
            }

            // Baixa o estoque
            const string sqlBaixa = @"
                UPDATE componente
                SET quantidade_estoque = quantidade_estoque - @quantidade
                WHERE id = @idComponente;";

            using var commandBaixa =
                new MySqlCommand(sqlBaixa, connection, transaction);

            commandBaixa.Parameters.AddWithValue("@quantidade", quantidade);
            commandBaixa.Parameters.AddWithValue("@idComponente", idComponente);

            await commandBaixa.ExecuteNonQueryAsync();

            // Registra saída de estoque
            const string sqlMovimentacao = @"
                INSERT INTO movimentacao_estoque
                (
                    fk_componente,
                    fk_tipo_movimentacao,
                    motivo_movimentacao,
                    quantidade,
                    custo_unitario,
                    fk_ordem_servico,
                    fk_usuario_responsavel
                )
                VALUES
                (
                    @componente,
                    2,
                    'Saída por Ordem de Serviço',
                    @quantidade,
                    @custo,
                    @os,
                    @usuario
                );";

            using var commandMovimentacao =
                new MySqlCommand(sqlMovimentacao, connection, transaction);

            commandMovimentacao.Parameters.AddWithValue("@componente", idComponente);
            commandMovimentacao.Parameters.AddWithValue("@quantidade", quantidade);
            commandMovimentacao.Parameters.AddWithValue("@custo", precoUnitario);
            commandMovimentacao.Parameters.AddWithValue(
                "@os",
                idOS.HasValue ? idOS.Value : DBNull.Value);
            commandMovimentacao.Parameters.AddWithValue(
                "@usuario",
                idUsuarioAprovador);

            await commandMovimentacao.ExecuteNonQueryAsync();

            // "Em Andamento" = status 2
            const string sqlStatus = @"
                UPDATE ordem_servico_peca
                SET
                    fk_status = 2,
                    data_resolucao = NULL
                WHERE id = @id;";

            using var commandStatus =
                new MySqlCommand(sqlStatus, connection, transaction);

            commandStatus.Parameters.AddWithValue("@id", idSolicitacao);

            await commandStatus.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Rejeita/cancela a solicitação
    public async Task<bool> RejeitarSolicitacaoAsync(
        int idSolicitacao,
        string motivo)
    {
        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
            UPDATE ordem_servico_peca
            SET
                fk_status = 5,
                observacoes = CONCAT(
                    observacoes,
                    '\nMotivo da rejeição: ',
                    @motivo
                ),
                data_resolucao = CURRENT_TIMESTAMP
            WHERE id = @id
              AND fk_status = 1;";

        using var command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", idSolicitacao);
        command.Parameters.AddWithValue("@motivo", motivo);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    // Lista os graus de urgência
    public async Task<List<GrauUrgencia>> ListarGrausUrgenciaAsync()
    {
        var lista = new List<GrauUrgencia>();

        using var connection = _database.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
            SELECT id, nome
            FROM grau_urgencia
            ORDER BY id;";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            lista.Add(new GrauUrgencia
            {
                Id = reader.GetInt32("id"),
                Nome = reader.GetString("nome")
            });
        }

        return lista;
    }

    private static OrdemServicoPeca Mapear(MySqlDataReader reader)
    {
        return new OrdemServicoPeca
        {
            Id = reader.GetInt32("id"),

            FkOrdemServicoPrincipal =
                reader.IsDBNull(reader.GetOrdinal("fk_ordem_servico_principal"))
                    ? null
                    : reader.GetInt32("fk_ordem_servico_principal"),

            NumeroOS =
                reader.IsDBNull(reader.GetOrdinal("numero_os"))
                    ? null
                    : reader.GetString("numero_os"),

            FkComponente = reader.GetInt32("fk_componente"),

            ComponenteNome = reader.GetString("componente_nome"),

            Quantidade = reader.GetInt32("quantidade"),

            FkUrgencia = reader.GetInt32("fk_urgencia"),

            UrgenciaNome = reader.GetString("urgencia_nome"),

            Observacoes = reader.GetString("observacoes"),

            FkStatus = reader.GetInt32("fk_status"),

            StatusNome = reader.GetString("status_nome"),

            DataSolicitacao = reader.GetDateTime("data_solicitacao"),

            DataResolucao =
                reader.IsDBNull(reader.GetOrdinal("data_resolucao"))
                    ? null
                    : reader.GetDateTime("data_resolucao")
        };
    }
}
