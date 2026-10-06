using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class OrdemServicoDAO : BaseDAO
{
    public OrdemServicoDAO(DatabaseConnection db) : base(db)
    {
    }

    private async Task CriarTabelasAsync(MySqlConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS tipo_servico (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome VARCHAR(100) NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS status_os (
                id INT AUTO_INCREMENT PRIMARY KEY,
                nome VARCHAR(50) NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS ordem_servico (
                id INT AUTO_INCREMENT PRIMARY KEY,
                numero_os VARCHAR(50) NOT NULL UNIQUE,
                fk_cliente INT NOT NULL,
                fk_equipamento INT NOT NULL,
                fk_tipo_servico INT NOT NULL,
                fk_tecnico INT NOT NULL,
                descricao_problema TEXT NOT NULL,
                fk_status INT NOT NULL,
                data_abertura DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                data_conclusao DATETIME NULL
            );";
        await cmd.ExecuteNonQueryAsync();

        // Popular tipo_servico caso vazio
        using var cmdTipoCount = conn.CreateCommand();
        cmdTipoCount.CommandText = "SELECT COUNT(1) FROM tipo_servico";
        var countTipos = Convert.ToInt32(await cmdTipoCount.ExecuteScalarAsync());
        if (countTipos == 0)
        {
            using var cmdInsertTipos = conn.CreateCommand();
            cmdInsertTipos.CommandText = @"
                INSERT INTO tipo_servico (nome) VALUES 
                ('Calibração de Balança Analítica'),
                ('Manutenção Preventiva'),
                ('Manutenção Corretiva'),
                ('Aferição de Padrões');";
            await cmdInsertTipos.ExecuteNonQueryAsync();
        }

        // Popular status_os caso vazio
        using var cmdStatusCount = conn.CreateCommand();
        cmdStatusCount.CommandText = "SELECT COUNT(1) FROM status_os";
        var countStatus = Convert.ToInt32(await cmdStatusCount.ExecuteScalarAsync());
        if (countStatus == 0)
        {
            using var cmdInsertStatus = conn.CreateCommand();
            cmdInsertStatus.CommandText = @"
                INSERT INTO status_os (nome) VALUES 
                ('Pendente'),
                ('Em Andamento'),
                ('Aguardando Peça'),
                ('Concluído'),
                ('Cancelado');";
            await cmdInsertStatus.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// Lista todas as ordens de serviço diretamente do banco de dados, com filtro opcional por ID de status.
    /// </summary>
    public async Task<List<OrdemServico>> ListarTodasAsync(int? statusFiltro = null)
    {
        var lista = new List<OrdemServico>();
        try
        {
            using var conn = await OpenConnectionAsync();
            await CriarTabelasAsync(conn);

            using var cmd = conn.CreateCommand();

            string sql = @"
                SELECT 
                    os.id,
                    os.numero_os,
                    os.fk_cliente,
                    c.razao_social AS ClienteNome,
                    os.fk_equipamento,
                    e.modelo AS EquipamentoModelo,
                    e.numero_serie AS EquipamentoSerie,
                    os.fk_tipo_servico,
                    ts.nome AS TipoServicoNome,
                    os.fk_tecnico,
                    u.nome_completo AS TecnicoNome,
                    os.descricao_problema,
                    os.fk_status,
                    s.nome AS StatusNome,
                    os.data_abertura,
                    os.data_conclusao
                FROM ordem_servico os
                LEFT JOIN cliente c ON os.fk_cliente = c.id
                LEFT JOIN equipamento e ON os.fk_equipamento = e.id
                LEFT JOIN tipo_servico ts ON os.fk_tipo_servico = ts.id
                LEFT JOIN usuario u ON os.fk_tecnico = u.id
                LEFT JOIN status_os s ON os.fk_status = s.id";

            if (statusFiltro.HasValue && statusFiltro.Value > 0)
            {
                sql += " WHERE os.fk_status = @statusFiltro";
                cmd.Parameters.AddWithValue("@statusFiltro", statusFiltro.Value);
            }

            sql += " ORDER BY os.id DESC;";
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearOrdemServico(reader));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao listar do banco: {ex.Message}");
            throw;
        }

        return lista;
    }

    /// <summary>
    /// Busca uma ordem de serviço pelo ID diretamente no banco de dados.
    /// </summary>
    public async Task<OrdemServico?> BuscarPorIdAsync(int id)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await CriarTabelasAsync(conn);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    os.id,
                    os.numero_os,
                    os.fk_cliente,
                    c.razao_social AS ClienteNome,
                    os.fk_equipamento,
                    e.modelo AS EquipamentoModelo,
                    e.numero_serie AS EquipamentoSerie,
                    os.fk_tipo_servico,
                    ts.nome AS TipoServicoNome,
                    os.fk_tecnico,
                    u.nome_completo AS TecnicoNome,
                    os.descricao_problema,
                    os.fk_status,
                    s.nome AS StatusNome,
                    os.data_abertura,
                    os.data_conclusao
                FROM ordem_servico os
                LEFT JOIN cliente c ON os.fk_cliente = c.id
                LEFT JOIN equipamento e ON os.fk_equipamento = e.id
                LEFT JOIN tipo_servico ts ON os.fk_tipo_servico = ts.id
                LEFT JOIN usuario u ON os.fk_tecnico = u.id
                LEFT JOIN status_os s ON os.fk_status = s.id
                WHERE os.id = @id
                LIMIT 1;";

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapearOrdemServico(reader);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao buscar OS {id} no banco: {ex.Message}");
            throw;
        }

        return null;
    }

    /// <summary>
    /// Gera o próximo código identificador de Ordem de Serviço (Ex: ""OS-2026-0001"") a partir do banco.
    /// </summary>
    public async Task<string> GerarProximoNumeroOSAsync()
    {
        int anoAtual = DateTime.Now.Year;
        string prefixo = $"OS-{anoAtual}-";

        try
        {
            using var conn = await OpenConnectionAsync();
            await CriarTabelasAsync(conn);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT numero_os 
                FROM ordem_servico 
                WHERE numero_os LIKE @prefixo 
                ORDER BY id DESC 
                LIMIT 1;";

            cmd.Parameters.AddWithValue("@prefixo", $"{prefixo}%");

            var resultado = await cmd.ExecuteScalarAsync();
            if (resultado != null && resultado != DBNull.Value)
            {
                string ultimoNumero = resultado.ToString() ?? "";
                string parteNumerica = ultimoNumero.Replace(prefixo, "");
                if (int.TryParse(parteNumerica, out int sequencial))
                {
                    return $"{prefixo}{(sequencial + 1):D4}";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao gerar próximo número no banco: {ex.Message}");
        }

        return $"{prefixo}0001";
    }

    /// <summary>
    /// Cadastra uma nova ordem de serviço no banco de dados.
    /// </summary>
    public async Task<bool> InserirAsync(OrdemServico os)
    {
        using var conn = await OpenConnectionAsync();
        await CriarTabelasAsync(conn);

        if (string.IsNullOrWhiteSpace(os.NumeroOS))
        {
            os.NumeroOS = await GerarProximoNumeroOSAsync();
        }

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO ordem_servico (
                numero_os,
                fk_cliente,
                fk_equipamento,
                fk_tipo_servico,
                fk_tecnico,
                descricao_problema,
                fk_status,
                data_abertura,
                data_conclusao
            ) VALUES (
                @numero_os,
                @fk_cliente,
                @fk_equipamento,
                @fk_tipo_servico,
                @fk_tecnico,
                @descricao_problema,
                @fk_status,
                @data_abertura,
                @data_conclusao
            );";

        cmd.Parameters.AddWithValue("@numero_os", os.NumeroOS);
        cmd.Parameters.AddWithValue("@fk_cliente", os.FkCliente);
        cmd.Parameters.AddWithValue("@fk_equipamento", os.FkEquipamento);
        cmd.Parameters.AddWithValue("@fk_tipo_servico", os.FkTipoServico);
        cmd.Parameters.AddWithValue("@fk_tecnico", os.FkTecnico);
        cmd.Parameters.AddWithValue("@descricao_problema", os.DescricaoProblema ?? string.Empty);
        cmd.Parameters.AddWithValue("@fk_status", os.FkStatus);
        cmd.Parameters.AddWithValue("@data_abertura", os.DataAbertura);
        cmd.Parameters.AddWithValue("@data_conclusao", (object?)os.DataConclusao ?? DBNull.Value);

        var linhasAfetadas = await cmd.ExecuteNonQueryAsync();
        if (linhasAfetadas > 0)
        {
            os.Id = (int)cmd.LastInsertedId;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Atualiza o status de uma Ordem de Serviço e opcionalmente sua data de conclusão no banco de dados.
    /// </summary>
    public async Task<bool> AtualizarStatusAsync(int idOS, int novoStatus, DateTime? dataConclusao = null)
    {
        if (novoStatus == 4 && !dataConclusao.HasValue)
        {
            dataConclusao = DateTime.Now;
        }

        using var conn = await OpenConnectionAsync();
        await CriarTabelasAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE ordem_servico 
            SET fk_status = @novoStatus,
                data_conclusao = @dataConclusao
            WHERE id = @idOS;";

        cmd.Parameters.AddWithValue("@novoStatus", novoStatus);
        cmd.Parameters.AddWithValue("@dataConclusao", (object?)dataConclusao ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@idOS", idOS);

        var linhas = await cmd.ExecuteNonQueryAsync();
        return linhas > 0;
    }

    /// <summary>
    /// Lista os tipos de serviços cadastrados no banco de dados.
    /// </summary>
    public async Task<List<TipoServico>> ListarTiposServicoAsync()
    {
        var lista = new List<TipoServico>();
        using var conn = await OpenConnectionAsync();
        await CriarTabelasAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome FROM tipo_servico ORDER BY id;";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new TipoServico
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome"))
            });
        }

        return lista;
    }

    /// <summary>
    /// Lista os status possíveis para uma Ordem de Serviço a partir do banco de dados.
    /// </summary>
    public async Task<List<StatusOS>> ListarStatusPossiveisAsync()
    {
        var lista = new List<StatusOS>();
        using var conn = await OpenConnectionAsync();
        await CriarTabelasAsync(conn);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, nome FROM status_os ORDER BY id;";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new StatusOS
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome"))
            });
        }

        return lista;
    }

    /// <summary>
    /// Método auxiliar para carregar clientes para o seletor dinâmico da tela de OS.
    /// </summary>
    public async Task<List<Cliente>> ListarClientesAuxAsync()
    {
        var lista = new List<Cliente>();
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, razao_social, nome_fantasia, cnpj_cpf, telefone, email, cidade, estado FROM cliente WHERE ativo = 1 ORDER BY razao_social;";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Cliente
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                RazaoSocial = reader.GetString(reader.GetOrdinal("razao_social")),
                NomeFantasia = reader.IsDBNull(reader.GetOrdinal("nome_fantasia")) ? null : reader.GetString(reader.GetOrdinal("nome_fantasia")),
                CnpjCpf = reader.GetString(reader.GetOrdinal("cnpj_cpf")),
                Telefone = reader.IsDBNull(reader.GetOrdinal("telefone")) ? null : reader.GetString(reader.GetOrdinal("telefone")),
                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                Cidade = reader.IsDBNull(reader.GetOrdinal("cidade")) ? null : reader.GetString(reader.GetOrdinal("cidade")),
                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                Ativo = true
            });
        }

        return lista;
    }

    /// <summary>
    /// Método auxiliar para carregar equipamentos vinculados a um cliente (ou todos) diretamente do banco.
    /// </summary>
    public async Task<List<Equipamento>> ListarEquipamentosAuxAsync(int? clienteId = null)
    {
        var lista = new List<Equipamento>();
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();

        string sql = @"
            SELECT e.id, e.fk_cliente, c.razao_social AS cliente_nome, e.modelo, e.marca_fabricante, 
                   e.capacidade_maxima_kg, e.divisao_escala_g, e.numero_serie, e.setor_localizacao
            FROM equipamento e
            LEFT JOIN cliente c ON e.fk_cliente = c.id
            WHERE e.ativo = 1";

        if (clienteId.HasValue && clienteId.Value > 0)
        {
            sql += " AND e.fk_cliente = @clienteId";
            cmd.Parameters.AddWithValue("@clienteId", clienteId.Value);
        }

        sql += " ORDER BY e.modelo;";
        cmd.CommandText = sql;

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Equipamento
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FkCliente = reader.GetInt32(reader.GetOrdinal("fk_cliente")),
                ClienteNome = reader.IsDBNull(reader.GetOrdinal("cliente_nome")) ? null : reader.GetString(reader.GetOrdinal("cliente_nome")),
                Modelo = reader.GetString(reader.GetOrdinal("modelo")),
                MarcaFabricante = reader.GetString(reader.GetOrdinal("marca_fabricante")),
                CapacidadeMaximaKg = reader.GetDecimal(reader.GetOrdinal("capacidade_maxima_kg")),
                DivisaoEscalaG = reader.GetDecimal(reader.GetOrdinal("divisao_escala_g")),
                NumeroSerie = reader.GetString(reader.GetOrdinal("numero_serie")),
                SetorLocalizacao = reader.GetString(reader.GetOrdinal("setor_localizacao")),
                Ativo = true
            });
        }

        return lista;
    }

    /// <summary>
    /// Método auxiliar para listar técnicos disponíveis no sistema diretamente do banco de dados.
    /// </summary>
    public async Task<List<Usuario>> ListarTecnicosAuxAsync()
    {
        var lista = new List<Usuario>();
        using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT u.id, u.nome_completo, u.email, c.nome AS CargoNome
            FROM usuario u
            LEFT JOIN cargo c ON u.fk_cargo = c.id
            WHERE u.ativo = 1
            ORDER BY u.nome_completo;";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            lista.Add(new Usuario
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                NomeCompleto = reader.GetString(reader.GetOrdinal("nome_completo")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                CargoNome = reader.IsDBNull(reader.GetOrdinal("CargoNome")) ? "Técnico" : reader.GetString(reader.GetOrdinal("CargoNome")),
                Ativo = true
            });
        }

        return lista;
    }

    private static OrdemServico MapearOrdemServico(MySqlDataReader reader)
    {
        return new OrdemServico
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            NumeroOS = reader.GetString(reader.GetOrdinal("numero_os")),
            FkCliente = reader.GetInt32(reader.GetOrdinal("fk_cliente")),
            ClienteNome = HasColumn(reader, "ClienteNome") && !reader.IsDBNull(reader.GetOrdinal("ClienteNome")) ? reader.GetString(reader.GetOrdinal("ClienteNome")) : null,
            FkEquipamento = reader.GetInt32(reader.GetOrdinal("fk_equipamento")),
            EquipamentoModelo = HasColumn(reader, "EquipamentoModelo") && !reader.IsDBNull(reader.GetOrdinal("EquipamentoModelo")) ? reader.GetString(reader.GetOrdinal("EquipamentoModelo")) : null,
            EquipamentoSerie = HasColumn(reader, "EquipamentoSerie") && !reader.IsDBNull(reader.GetOrdinal("EquipamentoSerie")) ? reader.GetString(reader.GetOrdinal("EquipamentoSerie")) : null,
            FkTipoServico = reader.GetInt32(reader.GetOrdinal("fk_tipo_servico")),
            TipoServicoNome = HasColumn(reader, "TipoServicoNome") && !reader.IsDBNull(reader.GetOrdinal("TipoServicoNome")) ? reader.GetString(reader.GetOrdinal("TipoServicoNome")) : null,
            FkTecnico = reader.GetInt32(reader.GetOrdinal("fk_tecnico")),
            TecnicoNome = HasColumn(reader, "TecnicoNome") && !reader.IsDBNull(reader.GetOrdinal("TecnicoNome")) ? reader.GetString(reader.GetOrdinal("TecnicoNome")) : null,
            DescricaoProblema = reader.GetString(reader.GetOrdinal("descricao_problema")),
            FkStatus = reader.GetInt32(reader.GetOrdinal("fk_status")),
            StatusNome = HasColumn(reader, "StatusNome") && !reader.IsDBNull(reader.GetOrdinal("StatusNome")) ? reader.GetString(reader.GetOrdinal("StatusNome")) : null,
            DataAbertura = reader.GetDateTime(reader.GetOrdinal("data_abertura")),
            DataConclusao = reader.IsDBNull(reader.GetOrdinal("data_conclusao")) ? null : reader.GetDateTime(reader.GetOrdinal("data_conclusao"))
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
