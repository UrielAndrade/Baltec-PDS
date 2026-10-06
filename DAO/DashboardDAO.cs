using Baltec.configs;
using Baltec.models;

namespace Baltec.DAO;

public class DashboardDAO : BaseDAO
{
    private readonly OrdemServicoDAO _osDAO;
    private readonly CertificadoCalibracaoDAO _certDAO;
    private readonly EquipamentoDAO _equipDAO;
    private readonly FinanceiroDAO _finDAO;

    public DashboardDAO(
        DatabaseConnection db,
        OrdemServicoDAO osDAO,
        CertificadoCalibracaoDAO certDAO,
        EquipamentoDAO equipDAO,
        FinanceiroDAO finDAO) : base(db)
    {
        _osDAO = osDAO;
        _certDAO = certDAO;
        _equipDAO = equipDAO;
        _finDAO = finDAO;
    }

    public async Task<DashboardDTO> ObterMetricasGeraisAsync()
    {
        var dto = new DashboardDTO();

        // 1. Ordens de Serviço
        var ordens = new List<OrdemServico>();
        try
        {
            ordens = await _osDAO.ListarTodasAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardDAO] Aviso ao carregar OS: {ex.Message}");
        }

        // 2. Certificados de Calibração
        var certificados = new List<CertificadoCalibracao>();
        try
        {
            certificados = await _certDAO.ListarTodosAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardDAO] Aviso ao carregar Certificados: {ex.Message}");
        }

        // 3. Equipamentos (Balanças)
        var equipamentos = new List<Equipamento>();
        try
        {
            equipamentos = await _equipDAO.ListarTodosComClienteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardDAO] Aviso ao carregar Equipamentos: {ex.Message}");
        }

        // 4. Financeiro
        decimal faturamento = 0;
        var transacoes = new List<TransacaoFinanceira>();
        try
        {
            faturamento = await _finDAO.ObterFaturamentoMesAtualAsync();
            transacoes = await _finDAO.ListarTodasAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardDAO] Aviso ao carregar Faturamento: {ex.Message}");
        }

        // Totalizadores estritamente do banco de dados
        dto.TotalBalancas = equipamentos.Count;
        dto.TotalOS = ordens.Count;
        dto.OSPendentes = ordens.Count(o => o.FkStatus == 1 || o.StatusNome == "Pendente");
        dto.TarefasConcluidas = ordens.Count(o => o.FkStatus == 4 || o.StatusNome == "Concluído");
        dto.TotalCertificados = certificados.Count;
        dto.FaturamentoMensal = faturamento;

        // Gráfico 1: Ordens de Serviço por Mês (Jan a Dez do ano atual)
        var hoje = DateTime.Today;
        var valoresMensais = new int[12];
        var osAnoAtual = ordens.Where(o => o.DataAbertura.Year == hoje.Year).ToList();

        for (int m = 1; m <= 12; m++)
        {
            valoresMensais[m - 1] = osAnoAtual.Count(o => o.DataAbertura.Month == m);
        }
        dto.GraficoOSMensalValores = valoresMensais.ToList();

        // Gráfico 2: Demandas por Categoria / Tipo de Serviço reais do banco
        if (ordens.Any())
        {
            var agrupados = ordens
                .GroupBy(o => string.IsNullOrWhiteSpace(o.TipoServicoNome) ? "Manutenção Geral" : o.TipoServicoNome)
                .OrderByDescending(g => g.Count())
                .Take(4)
                .ToList();

            dto.GraficoCategoriasNomes = agrupados.Select(g => g.Key).ToList();
            dto.GraficoCategoriasQuantidades = agrupados.Select(g => g.Count()).ToList();
        }
        else
        {
            dto.GraficoCategoriasNomes = new List<string>();
            dto.GraficoCategoriasQuantidades = new List<int>();
        }

        // Últimas OS reais
        dto.UltimasOS = ordens
            .OrderByDescending(o => o.DataAbertura)
            .ThenByDescending(o => o.Id)
            .Take(5)
            .ToList();

        // Atividades Recentes estritamente a partir dos dados do banco
        dto.AtividadesRecentes = ConsolidarAtividades(ordens, certificados, equipamentos, transacoes, 5);

        return dto;
    }

    public async Task<List<OrdemServico>> ObterUltimasOSAsync(int limite = 5)
    {
        try
        {
            var ordens = await _osDAO.ListarTodasAsync();
            return ordens
                .OrderByDescending(o => o.DataAbertura)
                .ThenByDescending(o => o.Id)
                .Take(limite)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DashboardDAO] Erro ao obter últimas OS: {ex.Message}");
            return new List<OrdemServico>();
        }
    }

    private static List<AtividadeRecenteDTO> ConsolidarAtividades(
        List<OrdemServico> ordens,
        List<CertificadoCalibracao> certificados,
        List<Equipamento> equipamentos,
        List<TransacaoFinanceira> transacoes,
        int limite = 5)
    {
        var atividades = new List<AtividadeRecenteDTO>();

        // 1. Última transação financeira
        var ultimaFin = transacoes.OrderByDescending(t => t.DataCadastro).FirstOrDefault();
        if (ultimaFin != null)
        {
            atividades.Add(new AtividadeRecenteDTO
            {
                Tempo = ultimaFin.DataCadastro.ToString("dd/MM HH:mm"),
                Descricao = $"{ultimaFin.TipoTransacaoNome}: {ultimaFin.Descricao} ({ultimaFin.Valor:C2})",
                Tag = "Financeiro",
                TagClass = "primary"
            });
        }

        // 2. Último certificado emitido
        var ultimoCert = certificados.OrderByDescending(c => c.DataEmissao).FirstOrDefault();
        if (ultimoCert != null)
        {
            atividades.Add(new AtividadeRecenteDTO
            {
                Tempo = ultimoCert.DataEmissao.ToString("dd/MM HH:mm"),
                Descricao = $"Certificado #{ultimoCert.NumeroCertificado} emitido para {ultimoCert.ClienteNome ?? "Cliente"}.",
                Tag = "Certificado",
                TagClass = "warning"
            });
        }

        // 3. Última OS criada
        var ultimaOS = ordens.OrderByDescending(o => o.DataAbertura).FirstOrDefault();
        if (ultimaOS != null)
        {
            atividades.Add(new AtividadeRecenteDTO
            {
                Tempo = ultimaOS.DataAbertura.ToString("dd/MM HH:mm"),
                Descricao = $"Ordem de Serviço #{ultimaOS.NumeroOS} gerada para {ultimaOS.ClienteNome ?? "Cliente"}.",
                Tag = "Serviço",
                TagClass = "info"
            });
        }

        // 4. Última balança cadastrada
        var ultimoEquip = equipamentos.OrderByDescending(e => e.Id).FirstOrDefault();
        if (ultimoEquip != null)
        {
            atividades.Add(new AtividadeRecenteDTO
            {
                Tempo = ultimoEquip.DataCadastro.ToString("dd/MM HH:mm"),
                Descricao = $"Balança {ultimoEquip.Modelo} ({ultimoEquip.MarcaFabricante}) cadastrada com sucesso.",
                Tag = "Cadastro",
                TagClass = "success"
            });
        }

        return atividades.Take(limite).ToList();
    }
}
