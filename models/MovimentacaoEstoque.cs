namespace Baltec.models
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }

        public int FkComponente { get; set; }

        public string? ComponenteNome { get; set; }

        public int FkTipoMovimentacao { get; set; }

        public string? TipoMovimentacaoNome { get; set; }

        public string MotivoMovimentacao { get; set; } = "";

        public int Quantidade { get; set; }

        public decimal CustoUnitario { get; set; }

        public int? FkFornecedor { get; set; }

        public int? FkOrdemServico { get; set; }

        public int FkUsuarioResponsavel { get; set; }

        public DateTime DataMovimentacao { get; set; }
    }
}