namespace Baltec.models;

public class DashboardDTO
{
    public int TarefasConcluidas { get; set; }
    public int TotalBalancas { get; set; }
    public int TotalOS { get; set; }
    public int OSPendentes { get; set; }
    public int TotalCertificados { get; set; }
    public decimal FaturamentoMensal { get; set; }
    public decimal MetaFaturamentoMensal { get; set; } = 40000m;
    public List<int> GraficoOSMensalValores { get; set; } = new();
    public List<string> GraficoCategoriasNomes { get; set; } = new();
    public List<int> GraficoCategoriasQuantidades { get; set; } = new();
    public List<AtividadeRecenteDTO> AtividadesRecentes { get; set; } = new();
    public List<OrdemServico> UltimasOS { get; set; } = new();
}

public class AtividadeRecenteDTO
{
    public string Tempo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string TagClass { get; set; } = "info"; // success, info, warning, primary
}
