namespace Baltec.models;

public class OrdemServicoPeca
{
    public int Id { get; set; }

    public int? FkOrdemServicoPrincipal { get; set; }

    public string? NumeroOS { get; set; }

    public int FkComponente { get; set; }

    public string? ComponenteNome { get; set; }

    public int Quantidade { get; set; }

    public int FkUrgencia { get; set; }

    public string? UrgenciaNome { get; set; }

    public string Observacoes { get; set; } = string.Empty;

    public int FkStatus { get; set; }

    public string? StatusNome { get; set; }

    public DateTime DataSolicitacao { get; set; }

    public DateTime? DataResolucao { get; set; }
}