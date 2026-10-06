using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class FinanceiroDAO : BaseDAO
{
    public FinanceiroDAO(DatabaseConnection db) : base(db)
    {
    }

    private async Task EnsureTableCreatedAsync(MySqlConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS tipo_transacao_financeira (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome VARCHAR(50) NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS status_transacao_financeira (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome VARCHAR(50) NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS transacao_financeira (
                id INT AUTO_INCREMENT PRIMARY KEY,
                fk_tipo_transacao INT NOT NULL,
                descricao VARCHAR(255) NOT NULL,
                valor DECIMAL(18,2) NOT NULL,
                fk_status INT NOT NULL,
                data_vencimento DATE NOT NULL,
                data_pagamento DATE NULL,
                fk_cliente INT NULL,
                fk_fornecedor INT NULL,
                fk_ordem_servico INT NULL,
                data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            );

            INSERT IGNORE INTO tipo_transacao_financeira (id, nome) VALUES (1, 'Receita'), (2, 'Despesa');
            INSERT IGNORE INTO status_transacao_financeira (id, nome) VALUES (1, 'Pendente'), (2, 'Pago'), (3, 'Atrasado'), (4, 'Cancelado');
        ";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<TransacaoFinanceira>> ListarTodasAsync()
    {
        var lista = new List<TransacaoFinanceira>();

        using var conn = await OpenConnectionAsync();
        await EnsureTableCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT t.*, 
                   tt.nome AS TipoTransacaoNome,
                   st.nome AS StatusNome,
                   c.razao_social AS ClienteNome,
                   f.razao_social AS FornecedorNome,
                   os.numero_os AS NumeroOS
            FROM transacao_financeira t
            LEFT JOIN tipo_transacao_financeira tt ON t.fk_tipo_transacao = tt.id
            LEFT JOIN status_transacao_financeira st ON t.fk_status = st.id
            LEFT JOIN cliente c ON t.fk_cliente = c.id
            LEFT JOIN fornecedor f ON t.fk_fornecedor = f.id
            LEFT JOIN ordem_servico os ON t.fk_ordem_servico = os.id
            ORDER BY t.data_vencimento DESC, t.id DESC";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(MapearTransacao(reader));
        }

        return lista;
    }

    public async Task<bool> InserirAsync(TransacaoFinanceira transacao)
    {
        if (string.IsNullOrEmpty(transacao.TipoTransacaoNome))
        {
            transacao.TipoTransacaoNome = transacao.FkTipoTransacao == 2 ? "Despesa" : "Receita";
        }
        if (string.IsNullOrEmpty(transacao.StatusNome))
        {
            transacao.StatusNome = transacao.FkStatus == 2 ? "Pago" : (transacao.FkStatus == 3 ? "Atrasado" : "Pendente");
        }

        using var conn = await OpenConnectionAsync();
        await EnsureTableCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO transacao_financeira 
            (fk_tipo_transacao, descricao, valor, fk_status, data_vencimento, data_pagamento, fk_cliente, fk_fornecedor, fk_ordem_servico, data_cadastro)
            VALUES 
            (@tipo, @descricao, @valor, @status, @vencimento, @pagamento, @cliente, @fornecedor, @os, @cadastro);
            SELECT LAST_INSERT_ID();";

        cmd.Parameters.AddWithValue("@tipo", transacao.FkTipoTransacao);
        cmd.Parameters.AddWithValue("@descricao", transacao.Descricao);
        cmd.Parameters.AddWithValue("@valor", transacao.Valor);
        cmd.Parameters.AddWithValue("@status", transacao.FkStatus);
        cmd.Parameters.AddWithValue("@vencimento", transacao.DataVencimento.Date);
        cmd.Parameters.AddWithValue("@pagamento", (object?)transacao.DataPagamento?.Date ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@cliente", (object?)transacao.FkCliente ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@fornecedor", (object?)transacao.FkFornecedor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@os", (object?)transacao.FkOrdemServico ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@cadastro", transacao.DataCadastro == default ? DateTime.Now : transacao.DataCadastro);

        var novoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        transacao.Id = novoId;
        return novoId > 0;
    }

    public async Task<bool> DarBaixaPagamentoAsync(int idTransacao, DateTime dataPagamento)
    {
        using var conn = await OpenConnectionAsync();
        await EnsureTableCreatedAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE transacao_financeira 
            SET fk_status = 2, data_pagamento = @pagamento 
            WHERE id = @id";

        cmd.Parameters.AddWithValue("@id", idTransacao);
        cmd.Parameters.AddWithValue("@pagamento", dataPagamento.Date);

        var linhas = await cmd.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    public async Task<decimal> ObterFaturamentoMesAtualAsync()
    {
        var todas = await ListarTodasAsync();
        var hoje = DateTime.Today;
        return todas
            .Where(t => t.FkTipoTransacao == 1 && t.FkStatus == 2 && t.DataPagamento.HasValue && 
                        t.DataPagamento.Value.Month == hoje.Month && t.DataPagamento.Value.Year == hoje.Year)
            .Sum(t => t.Valor);
    }

    public async Task<decimal> ObterTotalReceitasMesAsync()
    {
        var todas = await ListarTodasAsync();
        var hoje = DateTime.Today;
        return todas
            .Where(t => t.FkTipoTransacao == 1 && t.DataVencimento.Month == hoje.Month && t.DataVencimento.Year == hoje.Year)
            .Sum(t => t.Valor);
    }

    public async Task<decimal> ObterTotalDespesasMesAsync()
    {
        var todas = await ListarTodasAsync();
        var hoje = DateTime.Today;
        return todas
            .Where(t => t.FkTipoTransacao == 2 && t.DataVencimento.Month == hoje.Month && t.DataVencimento.Year == hoje.Year)
            .Sum(t => t.Valor);
    }

    public async Task<decimal> ObterSaldoGeralAsync()
    {
        var todas = await ListarTodasAsync();
        var receitas = todas.Where(t => t.FkTipoTransacao == 1 && t.FkStatus == 2).Sum(t => t.Valor);
        var despesas = todas.Where(t => t.FkTipoTransacao == 2 && t.FkStatus == 2).Sum(t => t.Valor);
        return receitas - despesas;
    }

    private static TransacaoFinanceira MapearTransacao(MySqlDataReader reader)
    {
        return new TransacaoFinanceira
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            FkTipoTransacao = reader.GetInt32(reader.GetOrdinal("fk_tipo_transacao")),
            TipoTransacaoNome = HasColumn(reader, "TipoTransacaoNome") && !reader.IsDBNull(reader.GetOrdinal("TipoTransacaoNome"))
                ? reader.GetString(reader.GetOrdinal("TipoTransacaoNome"))
                : (reader.GetInt32(reader.GetOrdinal("fk_tipo_transacao")) == 2 ? "Despesa" : "Receita"),
            Descricao = reader.GetString(reader.GetOrdinal("descricao")),
            Valor = reader.GetDecimal(reader.GetOrdinal("valor")),
            FkStatus = reader.GetInt32(reader.GetOrdinal("fk_status")),
            StatusNome = HasColumn(reader, "StatusNome") && !reader.IsDBNull(reader.GetOrdinal("StatusNome"))
                ? reader.GetString(reader.GetOrdinal("StatusNome"))
                : (reader.GetInt32(reader.GetOrdinal("fk_status")) == 2 ? "Pago" : "Pendente"),
            DataVencimento = reader.GetDateTime(reader.GetOrdinal("data_vencimento")),
            DataPagamento = reader.IsDBNull(reader.GetOrdinal("data_pagamento")) ? null : reader.GetDateTime(reader.GetOrdinal("data_pagamento")),
            FkCliente = reader.IsDBNull(reader.GetOrdinal("fk_cliente")) ? null : reader.GetInt32(reader.GetOrdinal("fk_cliente")),
            ClienteNome = HasColumn(reader, "ClienteNome") && !reader.IsDBNull(reader.GetOrdinal("ClienteNome")) ? reader.GetString(reader.GetOrdinal("ClienteNome")) : null,
            FkFornecedor = reader.IsDBNull(reader.GetOrdinal("fk_fornecedor")) ? null : reader.GetInt32(reader.GetOrdinal("fk_fornecedor")),
            FornecedorNome = HasColumn(reader, "FornecedorNome") && !reader.IsDBNull(reader.GetOrdinal("FornecedorNome")) ? reader.GetString(reader.GetOrdinal("FornecedorNome")) : null,
            FkOrdemServico = reader.IsDBNull(reader.GetOrdinal("fk_ordem_servico")) ? null : reader.GetInt32(reader.GetOrdinal("fk_ordem_servico")),
            NumeroOS = HasColumn(reader, "NumeroOS") && !reader.IsDBNull(reader.GetOrdinal("NumeroOS")) ? reader.GetString(reader.GetOrdinal("NumeroOS")) : null,
            DataCadastro = reader.GetDateTime(reader.GetOrdinal("data_cadastro"))
        };
    }

    private static bool HasColumn(MySqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
