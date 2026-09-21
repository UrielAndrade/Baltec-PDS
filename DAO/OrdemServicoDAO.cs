using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class OrdemServicoDAO : BaseDAO
{
    // Armazenamento em memória para garantir funcionamento contínuo e demonstração mesmo quando o MySQL local estiver offline
    private static readonly List<OrdemServico> _fallbackOS = new();
    private static readonly List<TipoServico> _fallbackTipos = new();
    private static readonly List<StatusOS> _fallbackStatus = new();
    private static readonly List<Cliente> _fallbackClientes = new();
    private static readonly List<Equipamento> _fallbackEquipamentos = new();
    private static readonly List<Usuario> _fallbackTecnicos = new();
    private static readonly object _lock = new();
    private static bool _dadosIniciaisCarregados = false;

    public OrdemServicoDAO(DatabaseConnection db) : base(db)
    {
        InicializarDadosDemonstracao();
    }

    private static void InicializarDadosDemonstracao()
    {
        lock (_lock)
        {
            if (_dadosIniciaisCarregados) return;

            _fallbackStatus.AddRange(new[]
            {
                new StatusOS { Id = 1, Nome = "Pendente" },
                new StatusOS { Id = 2, Nome = "Em Andamento" },
                new StatusOS { Id = 3, Nome = "Aguardando Peça" },
                new StatusOS { Id = 4, Nome = "Concluído" },
                new StatusOS { Id = 5, Nome = "Cancelado" }
            });

            _fallbackTipos.AddRange(new[]
            {
                new TipoServico { Id = 1, Nome = "Calibração de Balança Analítica" },
                new TipoServico { Id = 2, Nome = "Manutenção Preventiva" },
                new TipoServico { Id = 3, Nome = "Manutenção Corretiva" },
                new TipoServico { Id = 4, Nome = "Aferição de Padrões" }
            });

            _fallbackClientes.AddRange(new[]
            {
                new Cliente { Id = 1, RazaoSocial = "Empresa Alpha Ltda", CnpjCpf = "12.345.678/0001-90", Telefone = "(11) 98888-1111", Email = "contato@alpha.com.br", Cidade = "São Paulo", Estado = "SP", Ativo = true },
                new Cliente { Id = 2, RazaoSocial = "Indústrias Beta S/A", CnpjCpf = "98.765.432/0001-10", Telefone = "(19) 97777-2222", Email = "operacoes@beta.com.br", Cidade = "Campinas", Estado = "SP", Ativo = true },
                new Cliente { Id = 3, RazaoSocial = "Frigorífico Sigma Carnes", CnpjCpf = "45.123.789/0001-55", Telefone = "(16) 96666-3333", Email = "manutencao@sigma.com", Cidade = "Ribeirão Preto", Estado = "SP", Ativo = true },
                new Cliente { Id = 4, RazaoSocial = "Metalúrgica Delta Inox", CnpjCpf = "33.654.987/0001-22", Telefone = "(11) 95555-4444", Email = "compras@deltainox.com", Cidade = "Guarulhos", Estado = "SP", Ativo = true }
            });

            _fallbackEquipamentos.AddRange(new[]
            {
                new Equipamento { Id = 1, FkCliente = 1, ClienteNome = "Empresa Alpha Ltda", Modelo = "Toledo 2090", MarcaFabricante = "Toledo do Brasil", NumeroSerie = "TOL-994821", CapacidadeMaximaKg = 300, SetorLocalizacao = "Expedição", Ativo = true },
                new Equipamento { Id = 2, FkCliente = 1, ClienteNome = "Empresa Alpha Ltda", Modelo = "Balança Analítica AS 220.R2", MarcaFabricante = "Radwag", NumeroSerie = "RDW-112344", CapacidadeMaximaKg = 0.220m, SetorLocalizacao = "Laboratório de Controle", Ativo = true },
                new Equipamento { Id = 3, FkCliente = 2, ClienteNome = "Indústrias Beta S/A", Modelo = "Filizola Platinum 30kg", MarcaFabricante = "Filizola", NumeroSerie = "FLZ-883912", CapacidadeMaximaKg = 30, SetorLocalizacao = "Linha de Montagem 02", Ativo = true },
                new Equipamento { Id = 4, FkCliente = 2, ClienteNome = "Indústrias Beta S/A", Modelo = "Balança de Piso 2000kg", MarcaFabricante = "Toledo", NumeroSerie = "TOL-556102", CapacidadeMaximaKg = 2000, SetorLocalizacao = "Almoxarifado Central", Ativo = true },
                new Equipamento { Id = 5, FkCliente = 3, ClienteNome = "Frigorífico Sigma Carnes", Modelo = "Balança Rodoviária 80t", MarcaFabricante = "Baltec Precision", NumeroSerie = "BLT-774901", CapacidadeMaximaKg = 80000, SetorLocalizacao = "Portaria Principal", Ativo = true },
                new Equipamento { Id = 6, FkCliente = 4, ClienteNome = "Metalúrgica Delta Inox", Modelo = "Balança Contadora Toledo 9094", MarcaFabricante = "Toledo", NumeroSerie = "TOL-332110", CapacidadeMaximaKg = 15, SetorLocalizacao = "Usinagem", Ativo = true }
            });

            _fallbackTecnicos.AddRange(new[]
            {
                new Usuario { Id = 1, NomeCompleto = "Roberto Ramos - Metrologista", Email = "roberto@baltec.com.br", CargoNome = "Técnico de Calibração", Ativo = true },
                new Usuario { Id = 2, NomeCompleto = "Carlos Eduardo - Técnico Especialista", Email = "carlos@baltec.com.br", CargoNome = "Técnico de Calibração", Ativo = true },
                new Usuario { Id = 3, NomeCompleto = "Eng. Marcos Oliveira", Email = "marcos@baltec.com.br", CargoNome = "Engenheiro", Ativo = true }
            });

            _fallbackOS.AddRange(new[]
            {
                new OrdemServico
                {
                    Id = 1,
                    NumeroOS = "OS-2026-0001",
                    FkCliente = 1,
                    ClienteNome = "Empresa Alpha Ltda",
                    FkEquipamento = 1,
                    EquipamentoModelo = "Toledo 2090",
                    EquipamentoSerie = "TOL-994821",
                    FkTipoServico = 2,
                    TipoServicoNome = "Manutenção Preventiva",
                    FkTecnico = 1,
                    TecnicoNome = "Roberto Ramos - Metrologista",
                    DescricaoProblema = "Limpeza preventiva nas células de carga e verificação do cabeamento de comunicação.",
                    FkStatus = 4,
                    StatusNome = "Concluído",
                    DataAbertura = DateTime.Today.AddDays(-10),
                    DataConclusao = DateTime.Today.AddDays(-8)
                },
                new OrdemServico
                {
                    Id = 2,
                    NumeroOS = "OS-2026-0002",
                    FkCliente = 2,
                    ClienteNome = "Indústrias Beta S/A",
                    FkEquipamento = 3,
                    EquipamentoModelo = "Filizola Platinum 30kg",
                    EquipamentoSerie = "FLZ-883912",
                    FkTipoServico = 3,
                    TipoServicoNome = "Manutenção Corretiva",
                    FkTecnico = 2,
                    TecnicoNome = "Carlos Eduardo - Técnico Especialista",
                    DescricaoProblema = "Balança apresentando erro de zero e leitura instável durante pesagens de 10kg.",
                    FkStatus = 2,
                    StatusNome = "Em Andamento",
                    DataAbertura = DateTime.Today.AddDays(-3),
                    DataConclusao = null
                },
                new OrdemServico
                {
                    Id = 3,
                    NumeroOS = "OS-2026-0003",
                    FkCliente = 3,
                    ClienteNome = "Frigorífico Sigma Carnes",
                    FkEquipamento = 5,
                    EquipamentoModelo = "Balança Rodoviária 80t",
                    EquipamentoSerie = "BLT-774901",
                    FkTipoServico = 1,
                    TipoServicoNome = "Calibração de Balança Analítica",
                    FkTecnico = 1,
                    TecnicoNome = "Roberto Ramos - Metrologista",
                    DescricaoProblema = "Aferição periódica de conformidade ISO/IEC 17025 e verificação de excentricidade.",
                    FkStatus = 1,
                    StatusNome = "Pendente",
                    DataAbertura = DateTime.Today.AddDays(-1),
                    DataConclusao = null
                },
                new OrdemServico
                {
                    Id = 4,
                    NumeroOS = "OS-2026-0004",
                    FkCliente = 4,
                    ClienteNome = "Metalúrgica Delta Inox",
                    FkEquipamento = 6,
                    EquipamentoModelo = "Balança Contadora Toledo 9094",
                    EquipamentoSerie = "TOL-332110",
                    FkTipoServico = 4,
                    TipoServicoNome = "Aferição de Padrões",
                    FkTecnico = 3,
                    TecnicoNome = "Eng. Marcos Oliveira",
                    DescricaoProblema = "Necessidade de substituição do conector de alimentação e checagem de linearidade.",
                    FkStatus = 3,
                    StatusNome = "Aguardando Peça",
                    DataAbertura = DateTime.Today.AddDays(-2),
                    DataConclusao = null
                }
            });

            _dadosIniciaisCarregados = true;
        }
    }

    private async Task EnsureTablesCreatedAsync(MySqlConnection conn)
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
    /// Lista todas as ordens de serviço, com filtro opcional por ID de status.
    /// </summary>
    public async Task<List<OrdemServico>> ListarTodasAsync(int? statusFiltro = null)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

            var lista = new List<OrdemServico>();
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

            if (lista.Any())
            {
                return lista;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Conexão com o banco falhou ou tabela vazia: {ex.Message}. Utilizando dados locais.");
        }

        // Fallback em memória
        lock (_lock)
        {
            var consulta = _fallbackOS.AsEnumerable();
            if (statusFiltro.HasValue && statusFiltro.Value > 0)
            {
                consulta = consulta.Where(os => os.FkStatus == statusFiltro.Value);
            }
            return consulta.OrderByDescending(os => os.Id).Select(ClonarOS).ToList();
        }
    }

    /// <summary>
    /// Busca uma ordem de serviço pelo ID.
    /// </summary>
    public async Task<OrdemServico?> BuscarPorIdAsync(int id)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

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
            Console.WriteLine($"[OrdemServicoDAO] Erro ao buscar OS {id} no banco: {ex.Message}. Verificando fallback.");
        }

        lock (_lock)
        {
            var encontrado = _fallbackOS.FirstOrDefault(os => os.Id == id);
            return encontrado != null ? ClonarOS(encontrado) : null;
        }
    }

    /// <summary>
    /// Gera o próximo código identificador de Ordem de Serviço (Ex: "OS-2026-0001").
    /// </summary>
    public async Task<string> GerarProximoNumeroOSAsync()
    {
        int anoAtual = DateTime.Now.Year;
        string prefixo = $"OS-{anoAtual}-";

        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

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
                string parteSequencial = ultimoNumero.Replace(prefixo, "");
                if (int.TryParse(parteSequencial, out int seq))
                {
                    return $"{prefixo}{(seq + 1):D4}";
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao gerar número via banco: {ex.Message}. Calculando via memória.");
        }

        lock (_lock)
        {
            int maiorSeq = 0;
            foreach (var os in _fallbackOS)
            {
                if (os.NumeroOS != null && os.NumeroOS.StartsWith(prefixo))
                {
                    string parte = os.NumeroOS.Replace(prefixo, "");
                    if (int.TryParse(parte, out int seq) && seq > maiorSeq)
                    {
                        maiorSeq = seq;
                    }
                }
            }
            return $"{prefixo}{(maiorSeq + 1):D4}";
        }
    }

    /// <summary>
    /// Insere uma nova Ordem de Serviço.
    /// </summary>
    public async Task<bool> InserirAsync(OrdemServico os)
    {
        if (string.IsNullOrWhiteSpace(os.NumeroOS))
        {
            os.NumeroOS = await GerarProximoNumeroOSAsync();
        }

        if (os.DataAbertura == default)
        {
            os.DataAbertura = DateTime.Now;
        }

        if (os.FkStatus <= 0)
        {
            os.FkStatus = 1; // 1 = Pendente
        }

        // Preenche nomes descritivos caso não preenchidos
        PreencherNomesDescritivos(os);

        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Falha ao inserir no banco: {ex.Message}. Inserindo no armazenamento local.");
        }

        // Mantém sempre atualizado na lista local
        lock (_lock)
        {
            if (os.Id <= 0)
            {
                os.Id = _fallbackOS.Any() ? _fallbackOS.Max(x => x.Id) + 1 : 1;
            }

            var existente = _fallbackOS.FirstOrDefault(x => x.Id == os.Id);
            if (existente == null)
            {
                _fallbackOS.Insert(0, ClonarOS(os));
            }
        }

        return true;
    }

    /// <summary>
    /// Atualiza o status de uma Ordem de Serviço e opcionalmente sua data de conclusão.
    /// </summary>
    public async Task<bool> AtualizarStatusAsync(int idOS, int novoStatus, DateTime? dataConclusao = null)
    {
        // Se o status for concluído (id 4) e data de conclusão não foi especificada, define data atual
        if (novoStatus == 4 && !dataConclusao.HasValue)
        {
            dataConclusao = DateTime.Now;
        }

        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

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
            if (linhas > 0)
            {
                AtualizarStatusLocal(idOS, novoStatus, dataConclusao);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao atualizar status no banco: {ex.Message}. Atualizando na memória.");
        }

        AtualizarStatusLocal(idOS, novoStatus, dataConclusao);
        return true;
    }

    private void AtualizarStatusLocal(int idOS, int novoStatus, DateTime? dataConclusao)
    {
        lock (_lock)
        {
            var item = _fallbackOS.FirstOrDefault(x => x.Id == idOS);
            if (item != null)
            {
                item.FkStatus = novoStatus;
                var statusObj = _fallbackStatus.FirstOrDefault(s => s.Id == novoStatus);
                item.StatusNome = statusObj?.Nome ?? $"Status {novoStatus}";
                item.DataConclusao = dataConclusao;
            }
        }
    }

    /// <summary>
    /// Lista os tipos de serviços cadastrados.
    /// </summary>
    public async Task<List<TipoServico>> ListarTiposServicoAsync()
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

            var lista = new List<TipoServico>();
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

            if (lista.Any())
            {
                return lista;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao listar tipos de serviço do banco: {ex.Message}. Usando lista padrão.");
        }

        lock (_lock)
        {
            return _fallbackTipos.Select(t => new TipoServico { Id = t.Id, Nome = t.Nome }).ToList();
        }
    }

    /// <summary>
    /// Lista os status possíveis para uma Ordem de Serviço.
    /// </summary>
    public async Task<List<StatusOS>> ListarStatusPossiveisAsync()
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            await EnsureTablesCreatedAsync(conn);

            var lista = new List<StatusOS>();
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

            if (lista.Any())
            {
                return lista;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdemServicoDAO] Erro ao listar status do banco: {ex.Message}. Usando lista padrão.");
        }

        lock (_lock)
        {
            return _fallbackStatus.Select(s => new StatusOS { Id = s.Id, Nome = s.Nome }).ToList();
        }
    }

    /// <summary>
    /// Método auxiliar para carregar clientes para o seletor dinâmico da tela de OS.
    /// </summary>
    public async Task<List<Cliente>> ListarClientesAuxAsync()
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            var lista = new List<Cliente>();
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

            if (lista.Any()) return lista;
        }
        catch
        {
            // Ignora e utiliza fallback
        }

        lock (_lock)
        {
            return _fallbackClientes.ToList();
        }
    }

    /// <summary>
    /// Método auxiliar para carregar equipamentos vinculados a um cliente (ou todos).
    /// </summary>
    public async Task<List<Equipamento>> ListarEquipamentosAuxAsync(int? clienteId = null)
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            var lista = new List<Equipamento>();
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

            if (lista.Any()) return lista;
        }
        catch
        {
            // Ignora e utiliza fallback
        }

        lock (_lock)
        {
            var consulta = _fallbackEquipamentos.AsEnumerable();
            if (clienteId.HasValue && clienteId.Value > 0)
            {
                consulta = consulta.Where(e => e.FkCliente == clienteId.Value);
            }
            return consulta.ToList();
        }
    }

    /// <summary>
    /// Método auxiliar para listar técnicos disponíveis no sistema.
    /// </summary>
    public async Task<List<Usuario>> ListarTecnicosAuxAsync()
    {
        try
        {
            using var conn = await OpenConnectionAsync();
            var lista = new List<Usuario>();
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

            if (lista.Any()) return lista;
        }
        catch
        {
            // Ignora e utiliza fallback
        }

        lock (_lock)
        {
            return _fallbackTecnicos.ToList();
        }
    }

    private void PreencherNomesDescritivos(OrdemServico os)
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(os.ClienteNome) && os.FkCliente > 0)
            {
                var cli = _fallbackClientes.FirstOrDefault(c => c.Id == os.FkCliente);
                os.ClienteNome = cli?.RazaoSocial;
            }

            if ((string.IsNullOrEmpty(os.EquipamentoModelo) || string.IsNullOrEmpty(os.EquipamentoSerie)) && os.FkEquipamento > 0)
            {
                var eq = _fallbackEquipamentos.FirstOrDefault(e => e.Id == os.FkEquipamento);
                if (eq != null)
                {
                    os.EquipamentoModelo = eq.Modelo;
                    os.EquipamentoSerie = eq.NumeroSerie;
                }
            }

            if (string.IsNullOrEmpty(os.TipoServicoNome) && os.FkTipoServico > 0)
            {
                var tipo = _fallbackTipos.FirstOrDefault(t => t.Id == os.FkTipoServico);
                os.TipoServicoNome = tipo?.Nome;
            }

            if (string.IsNullOrEmpty(os.TecnicoNome) && os.FkTecnico > 0)
            {
                var tec = _fallbackTecnicos.FirstOrDefault(u => u.Id == os.FkTecnico);
                os.TecnicoNome = tec?.NomeCompleto;
            }

            if (string.IsNullOrEmpty(os.StatusNome) && os.FkStatus > 0)
            {
                var st = _fallbackStatus.FirstOrDefault(s => s.Id == os.FkStatus);
                os.StatusNome = st?.Nome ?? "Pendente";
            }
        }
    }

    private static OrdemServico ClonarOS(OrdemServico o)
    {
        return new OrdemServico
        {
            Id = o.Id,
            NumeroOS = o.NumeroOS,
            FkCliente = o.FkCliente,
            ClienteNome = o.ClienteNome,
            FkEquipamento = o.FkEquipamento,
            EquipamentoModelo = o.EquipamentoModelo,
            EquipamentoSerie = o.EquipamentoSerie,
            FkTipoServico = o.FkTipoServico,
            TipoServicoNome = o.TipoServicoNome,
            FkTecnico = o.FkTecnico,
            TecnicoNome = o.TecnicoNome,
            DescricaoProblema = o.DescricaoProblema,
            FkStatus = o.FkStatus,
            StatusNome = o.StatusNome,
            DataAbertura = o.DataAbertura,
            DataConclusao = o.DataConclusao
        };
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
