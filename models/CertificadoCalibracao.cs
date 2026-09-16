namespace Baltec.models;

public class CertificadoCalibracao
{
    public int Id { get; set; }
    public string NumeroCertificado { get; set; } = string.Empty;
    public int? FkOrdemServico { get; set; }
    public string? NumeroOS { get; set; }
    public int FkEquipamento { get; set; }
    public string? EquipamentoModelo { get; set; }
    public string? EquipamentoSerie { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime DataCalibracao { get; set; }
    public DateTime DataProximaCalibracao { get; set; }
    public decimal TemperaturaAmbiente { get; set; }
    public decimal UmidadeRelativa { get; set; }
    public int FkTecnicoResponsavel { get; set; }
    public string? TecnicoNome { get; set; }
    public DateTime DataEmissao { get; set; }
}
