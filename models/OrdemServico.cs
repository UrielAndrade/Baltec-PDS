namespace Baltec.models;

public class OrdemServico
{
    public int Id { get; set; }
    public string NumeroOS { get; set; } = string.Empty;
    public int FkCliente { get; set; }
    public string? ClienteNome { get; set; }
    public int FkEquipamento { get; set; }
    public string? EquipamentoModelo { get; set; }
    public string? EquipamentoSerie { get; set; }
    public int FkTipoServico { get; set; }
    public string? TipoServicoNome { get; set; }
    public int FkTecnico { get; set; }
    public string? TecnicoNome { get; set; }
    public string DescricaoProblema { get; set; } = string.Empty;
    public int FkStatus { get; set; }
    public string? StatusNome { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataConclusao { get; set; }
}
