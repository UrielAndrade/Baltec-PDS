namespace Baltec.models;

public class TransacaoFinanceira
{
    public int Id { get; set; }
    public int FkTipoTransacao { get; set; } // 1 = Receita, 2 = Despesa
    public string? TipoTransacaoNome { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int FkStatus { get; set; } // 1 = Pendente, 2 = Pago, 3 = Atrasado, 4 = Cancelado
    public string? StatusNome { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public int? FkCliente { get; set; }
    public string? ClienteNome { get; set; }
    public int? FkFornecedor { get; set; }
    public string? FornecedorNome { get; set; }
    public int? FkOrdemServico { get; set; }
    public string? NumeroOS { get; set; }
    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
