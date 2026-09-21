using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class EquipamentoOption
{
    public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
}

public class TecnicoOption
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}

public class OrdemServicoOption
{
    public int Id { get; set; }
    public string NumeroOS { get; set; } = string.Empty;
}

public class CertificadoCalibracaoDAO : BaseDAO
{
    public CertificadoCalibracaoDAO(DatabaseConnection db) : base(db)
    {
    }

    private async Task EnsureTableCreatedAsync(MySqlConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS certificado_calibracao (
                id INT AUTO_INCREMENT PRIMARY KEY,
                numero_certificado VARCHAR(50) NOT NULL UNIQUE,
                fk_ordem_servico INT NULL,
                fk_equipamento INT NOT NULL,
                data_calibracao DATE NOT NULL,
                data_proxima_calibracao DATE NOT NULL,
                temperatura_ambiente DECIMAL(5,2) NOT NULL,
                umidade_relativa DECIMAL(5,2) NOT NULL,
                fk_tecnico_responsavel INT NOT NULL,
                data_emissao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
            );";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<CertificadoCalibracao>> ListarTodosAsync()
    {
        var lista = new List<CertificadoCalibracao>();
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTableCreatedAsync(conn);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.*, 
                       e.modelo AS EquipamentoModelo, 
                       e.numero_serie AS EquipamentoSerie,
                       cl.razao_social AS ClienteNome,
                       os.numero_os AS NumeroOS,
                       u.nome_completo AS TecnicoNome
                FROM certificado_calibracao c
                LEFT JOIN equipamento e ON c.fk_equipamento = e.id
                LEFT JOIN cliente cl ON e.fk_cliente = cl.id
                LEFT JOIN ordem_servico os ON c.fk_ordem_servico = os.id
                LEFT JOIN usuario u ON c.fk_tecnico_responsavel = u.id
                ORDER BY c.id DESC";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearCertificado(reader));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao listar certificados: {ex.Message}");
        }
        return lista;
    }

    public async Task<CertificadoCalibracao?> BuscarPorNumeroAsync(string numeroCertificado)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTableCreatedAsync(conn);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.*, 
                       e.modelo AS EquipamentoModelo, 
                       e.numero_serie AS EquipamentoSerie,
                       cl.razao_social AS ClienteNome,
                       os.numero_os AS NumeroOS,
                       u.nome_completo AS TecnicoNome
                FROM certificado_calibracao c
                LEFT JOIN equipamento e ON c.fk_equipamento = e.id
                LEFT JOIN cliente cl ON e.fk_cliente = cl.id
                LEFT JOIN ordem_servico os ON c.fk_ordem_servico = os.id
                LEFT JOIN usuario u ON c.fk_tecnico_responsavel = u.id
                WHERE c.numero_certificado = @numeroCertificado";
            cmd.Parameters.AddWithValue("@numeroCertificado", numeroCertificado);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapearCertificado(reader);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao buscar certificado: {ex.Message}");
        }
        return null;
    }

    public async Task<string> GerarProximoNumeroCertificadoAsync()
    {
        var anoAtual = DateTime.Now.Year;
        var prefixo = $"CERT-{anoAtual}-";
        int proximoNumero = 1;

        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTableCreatedAsync(conn);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT numero_certificado 
                FROM certificado_calibracao 
                WHERE numero_certificado LIKE @prefixo 
                ORDER BY id DESC LIMIT 1";
            cmd.Parameters.AddWithValue("@prefixo", $"{prefixo}%");

            var maxCertObj = await cmd.ExecuteScalarAsync();
            if (maxCertObj != null && maxCertObj != DBNull.Value)
            {
                var maxCert = maxCertObj.ToString();
                if (!string.IsNullOrEmpty(maxCert))
                {
                    var partes = maxCert.Split('-');
                    if (partes.Length >= 3 && int.TryParse(partes[2], out int seq))
                    {
                        proximoNumero = seq + 1;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao gerar próximo número: {ex.Message}");
        }

        return $"{prefixo}{proximoNumero:D4}";
    }

    public async Task<bool> InserirAsync(CertificadoCalibracao cert)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTableCreatedAsync(conn);

            if (string.IsNullOrWhiteSpace(cert.NumeroCertificado))
            {
                cert.NumeroCertificado = await GerarProximoNumeroCertificadoAsync();
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO certificado_calibracao (
                    numero_certificado, fk_ordem_servico, fk_equipamento, 
                    data_calibracao, data_proxima_calibracao, temperatura_ambiente, 
                    umidade_relativa, fk_tecnico_responsavel, data_emissao
                ) VALUES (
                    @numeroCertificado, @fkOrdemServico, @fkEquipamento, 
                    @dataCalibracao, @dataProximaCalibracao, @temperaturaAmbiente, 
                    @umidadeRelativa, @fkTecnicoResponsavel, @dataEmissao
                )";

            cmd.Parameters.AddWithValue("@numeroCertificado", cert.NumeroCertificado);
            cmd.Parameters.AddWithValue("@fkOrdemServico", cert.FkOrdemServico.HasValue && cert.FkOrdemServico.Value > 0 ? cert.FkOrdemServico.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@fkEquipamento", cert.FkEquipamento);
            cmd.Parameters.AddWithValue("@dataCalibracao", cert.DataCalibracao);
            cmd.Parameters.AddWithValue("@dataProximaCalibracao", cert.DataProximaCalibracao);
            cmd.Parameters.AddWithValue("@temperaturaAmbiente", cert.TemperaturaAmbiente);
            cmd.Parameters.AddWithValue("@umidadeRelativa", cert.UmidadeRelativa);
            cmd.Parameters.AddWithValue("@fkTecnicoResponsavel", cert.FkTecnicoResponsavel);
            cmd.Parameters.AddWithValue("@dataEmissao", cert.DataEmissao == default ? DateTime.Now : cert.DataEmissao);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao inserir certificado: {ex.Message}");
            return false;
        }
    }

    public async Task<List<CertificadoCalibracao>> ListarProximosVencimentosAsync(int diasLimite = 30)
    {
        var lista = new List<CertificadoCalibracao>();
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTableCreatedAsync(conn);

            var dataHoje = DateTime.Today;
            var dataLimite = dataHoje.AddDays(diasLimite);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT c.*, 
                       e.modelo AS EquipamentoModelo, 
                       e.numero_serie AS EquipamentoSerie,
                       cl.razao_social AS ClienteNome,
                       os.numero_os AS NumeroOS,
                       u.nome_completo AS TecnicoNome
                FROM certificado_calibracao c
                LEFT JOIN equipamento e ON c.fk_equipamento = e.id
                LEFT JOIN cliente cl ON e.fk_cliente = cl.id
                LEFT JOIN ordem_servico os ON c.fk_ordem_servico = os.id
                LEFT JOIN usuario u ON c.fk_tecnico_responsavel = u.id
                WHERE c.data_proxima_calibracao BETWEEN @dataHoje AND @dataLimite
                ORDER BY c.data_proxima_calibracao ASC";

            cmd.Parameters.AddWithValue("@dataHoje", dataHoje);
            cmd.Parameters.AddWithValue("@dataLimite", dataLimite);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapearCertificado(reader));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao listar próximos vencimentos: {ex.Message}");
        }
        return lista;
    }

    public async Task<List<EquipamentoOption>> ListarEquipamentosAuxAsync()
    {
        var lista = new List<EquipamentoOption>();
        try
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT e.id, e.modelo, e.numero_serie, cl.razao_social AS cliente_nome 
                FROM equipamento e 
                LEFT JOIN cliente cl ON e.fk_cliente = cl.id 
                WHERE e.ativo = 1 ORDER BY e.modelo";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new EquipamentoOption
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Modelo = reader.GetString(reader.GetOrdinal("modelo")),
                    NumeroSerie = reader.GetString(reader.GetOrdinal("numero_serie")),
                    ClienteNome = reader.IsDBNull(reader.GetOrdinal("cliente_nome")) ? "Cliente Padrão" : reader.GetString(reader.GetOrdinal("cliente_nome"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao carregar equipamentos: {ex.Message}");
        }
        return lista;
    }

    public async Task<List<TecnicoOption>> ListarTecnicosAuxAsync()
    {
        var lista = new List<TecnicoOption>();
        try
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT u.id, u.nome_completo 
                FROM usuario u 
                WHERE u.ativo = 1 
                ORDER BY u.nome_completo";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new TecnicoOption
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Nome = reader.GetString(reader.GetOrdinal("nome_completo"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao carregar técnicos: {ex.Message}");
        }
        return lista;
    }

    public async Task<List<OrdemServicoOption>> ListarOrdensServicoAuxAsync()
    {
        var lista = new List<OrdemServicoOption>();
        try
        {
            using var conn = await OpenConnectionAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, numero_os 
                FROM ordem_servico 
                ORDER BY id DESC";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrdemServicoOption
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    NumeroOS = reader.GetString(reader.GetOrdinal("numero_os"))
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CertificadoCalibracaoDAO] Erro ao carregar OS: {ex.Message}");
        }
        return lista;
    }

    private static CertificadoCalibracao MapearCertificado(MySqlDataReader reader)
    {
        return new CertificadoCalibracao
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            NumeroCertificado = reader.GetString(reader.GetOrdinal("numero_certificado")),
            FkOrdemServico = reader.IsDBNull(reader.GetOrdinal("fk_ordem_servico")) ? null : reader.GetInt32(reader.GetOrdinal("fk_ordem_servico")),
            NumeroOS = HasColumn(reader, "NumeroOS") && !reader.IsDBNull(reader.GetOrdinal("NumeroOS")) ? reader.GetString(reader.GetOrdinal("NumeroOS")) : null,
            FkEquipamento = reader.GetInt32(reader.GetOrdinal("fk_equipamento")),
            EquipamentoModelo = HasColumn(reader, "EquipamentoModelo") && !reader.IsDBNull(reader.GetOrdinal("EquipamentoModelo")) ? reader.GetString(reader.GetOrdinal("EquipamentoModelo")) : null,
            EquipamentoSerie = HasColumn(reader, "EquipamentoSerie") && !reader.IsDBNull(reader.GetOrdinal("EquipamentoSerie")) ? reader.GetString(reader.GetOrdinal("EquipamentoSerie")) : null,
            ClienteNome = HasColumn(reader, "ClienteNome") && !reader.IsDBNull(reader.GetOrdinal("ClienteNome")) ? reader.GetString(reader.GetOrdinal("ClienteNome")) : null,
            DataCalibracao = reader.GetDateTime(reader.GetOrdinal("data_calibracao")),
            DataProximaCalibracao = reader.GetDateTime(reader.GetOrdinal("data_proxima_calibracao")),
            TemperaturaAmbiente = reader.GetDecimal(reader.GetOrdinal("temperatura_ambiente")),
            UmidadeRelativa = reader.GetDecimal(reader.GetOrdinal("umidade_relativa")),
            FkTecnicoResponsavel = reader.GetInt32(reader.GetOrdinal("fk_tecnico_responsavel")),
            TecnicoNome = HasColumn(reader, "TecnicoNome") && !reader.IsDBNull(reader.GetOrdinal("TecnicoNome")) ? reader.GetString(reader.GetOrdinal("TecnicoNome")) : null,
            DataEmissao = reader.GetDateTime(reader.GetOrdinal("data_emissao"))
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
