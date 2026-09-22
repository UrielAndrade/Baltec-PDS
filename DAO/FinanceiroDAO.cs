using Baltec.configs;
using Baltec.models;
using MySqlConnector;

namespace Baltec.DAO;

public class FinanceiroDAO : BaseDAO
{
    private static readonly List<TransacaoFinanceira> _fallbackTransacoes = new();
    private static readonly object _lock = new();
    private static bool _dadosIniciaisCarregados = false;

    public FinanceiroDAO(DatabaseConnection db) : base(db)
    {
        InicializarDadosDemonstracao();
    }

    private static void InicializarDadosDemonstracao()
    {
        lock (_lock)
        {
            if (_dadosIniciaisCarregados) return;

            var hoje = DateTime.Today;
            var mesAtual = hoje.Month;
            var anoAtual = hoje.Year;

            _fallbackTransacoes.AddRange(new[]
            {
                new TransacaoFinanceira
                {
                    Id = 1,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Manutenção e Calibração de Balança Toledo - Alpha",
                    Valor = 4500.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 10),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 10),
                    FkCliente = 1,
                    ClienteNome = "Empresa Alpha Ltda",
                    FkOrdemServico = 1,
                    NumeroOS = "OS-2026-001",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 5)
                },
                new TransacaoFinanceira
                {
                    Id = 2,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Calibração Rastreada RBC - Indústrias Beta",
                    Valor = 8200.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 14),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 14),
                    FkCliente = 2,
                    ClienteNome = "Indústrias Beta S/A",
                    FkOrdemServico = 2,
                    NumeroOS = "OS-2026-002",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 8)
                },
                new TransacaoFinanceira
                {
                    Id = 3,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Aferição de Balança Rodoviária 60t - Frigorífico Sigma",
                    Valor = 12800.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 16),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 16),
                    FkCliente = 3,
                    ClienteNome = "Frigorífico Sigma Carnes",
                    FkOrdemServico = 3,
                    NumeroOS = "OS-2026-003",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 11)
                },
                new TransacaoFinanceira
                {
                    Id = 4,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Contrato Mensal de Manutenção Preventiva - Metalúrgica Delta",
                    Valor = 6500.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 18),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 18),
                    FkCliente = 4,
                    ClienteNome = "Metalúrgica Delta Inox",
                    FkOrdemServico = 4,
                    NumeroOS = "OS-2026-004",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 12)
                },
                new TransacaoFinanceira
                {
                    Id = 5,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Calibração e Ajuste de Balança Analítica AS 220",
                    Valor = 3800.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 20),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 20),
                    FkCliente = 1,
                    ClienteNome = "Empresa Alpha Ltda",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 15)
                },
                new TransacaoFinanceira
                {
                    Id = 6,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Emissão de Laudo Técnico Metrológico e Conformidade",
                    Valor = 6200.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 21),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 21),
                    FkCliente = 2,
                    ClienteNome = "Indústrias Beta S/A",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 16)
                },
                new TransacaoFinanceira
                {
                    Id = 7,
                    FkTipoTransacao = 1,
                    TipoTransacaoNome = "Receita",
                    Descricao = "Calibração em Lote de Balanças de Linha de Produção",
                    Valor = 5400.00m,
                    FkStatus = 1,
                    StatusNome = "Pendente",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 28),
                    DataPagamento = null,
                    FkCliente = 3,
                    ClienteNome = "Frigorífico Sigma Carnes",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 20)
                },
                new TransacaoFinanceira
                {
                    Id = 8,
                    FkTipoTransacao = 2,
                    TipoTransacaoNome = "Despesa",
                    Descricao = "Aquisição de Células de Carga de Alta Precisão (500kg)",
                    Valor = 3200.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 8),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 8),
                    FornecedorNome = "Sensortec Componentes Industriais",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 2)
                },
                new TransacaoFinanceira
                {
                    Id = 9,
                    FkTipoTransacao = 2,
                    TipoTransacaoNome = "Despesa",
                    Descricao = "Calibração Externa de Pesos-Padrão RBC / Inmetro",
                    Valor = 1850.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 12),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 12),
                    FornecedorNome = "Labmet Metrologia Acreditada",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 5)
                },
                new TransacaoFinanceira
                {
                    Id = 10,
                    FkTipoTransacao = 2,
                    TipoTransacaoNome = "Despesa",
                    Descricao = "Consumo de Energia Elétrica e Climatização Laboratório",
                    Valor = 1450.00m,
                    FkStatus = 2,
                    StatusNome = "Pago",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 15),
                    DataPagamento = new DateTime(anoAtual, mesAtual, 15),
                    FornecedorNome = "Distribuidora de Energia",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 1)
                },
                new TransacaoFinanceira
                {
                    Id = 11,
                    FkTipoTransacao = 2,
                    TipoTransacaoNome = "Despesa",
                    Descricao = "Reposição de Cabos Blindados e Displays de Reposição",
                    Valor = 2100.00m,
                    FkStatus = 1,
                    StatusNome = "Pendente",
                    DataVencimento = new DateTime(anoAtual, mesAtual, 29),
                    DataPagamento = null,
                    FornecedorNome = "Electro Peças e Componentes Ltda",
                    DataCadastro = new DateTime(anoAtual, mesAtual, 18)
                }
            });

            _dadosIniciaisCarregados = true;
        }
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
        try
        {
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
            var lista = new List<TransacaoFinanceira>();

            while (await reader.ReadAsync())
            {
                lista.Add(MapearTransacao(reader));
            }

            if (lista.Any())
            {
                return lista;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FinanceiroDAO] Erro ao listar do MySQL ({ex.Message}). Retornando dados em memória.");
        }

        lock (_lock)
        {
            return _fallbackTransacoes.OrderByDescending(t => t.DataVencimento).ThenByDescending(t => t.Id).ToList();
        }
    }

    public async Task<bool> InserirAsync(TransacaoFinanceira transacao)
    {
        // Garante nomes consistentes
        if (string.IsNullOrEmpty(transacao.TipoTransacaoNome))
        {
            transacao.TipoTransacaoNome = transacao.FkTipoTransacao == 2 ? "Despesa" : "Receita";
        }
        if (string.IsNullOrEmpty(transacao.StatusNome))
        {
            transacao.StatusNome = transacao.FkStatus == 2 ? "Pago" : (transacao.FkStatus == 3 ? "Atrasado" : "Pendente");
        }

        try
        {
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
            cmd.Parameters.AddWithValue("@cadastro", transacao.DataCadastro);

            var novoId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            transacao.Id = novoId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FinanceiroDAO] Erro ao inserir no MySQL ({ex.Message}). Gravando em memória.");
            lock (_lock)
            {
                transacao.Id = _fallbackTransacoes.Any() ? _fallbackTransacoes.Max(t => t.Id) + 1 : 1;
            }
        }

        lock (_lock)
        {
            _fallbackTransacoes.Add(transacao);
        }

        return true;
    }

    public async Task<bool> DarBaixaPagamentoAsync(int idTransacao, DateTime dataPagamento)
    {
        try
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

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FinanceiroDAO] Erro ao dar baixa no MySQL ({ex.Message}). Atualizando em memória.");
        }

        lock (_lock)
        {
            var item = _fallbackTransacoes.FirstOrDefault(t => t.Id == idTransacao);
            if (item != null)
            {
                item.FkStatus = 2;
                item.StatusNome = "Pago";
                item.DataPagamento = dataPagamento;
            }
        }

        return true;
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
