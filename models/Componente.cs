namespace Baltec.models
{
    public class Componente
    {
        public int Id { get; set; }

        public string Nome { get; set; } = "";

        public string CodigoItem { get; set; } = "";

        public int FkCategoria { get; set; }

        public string? CategoriaNome { get; set; }

        public int QuantidadeEstoque { get; set; }

        public decimal PrecoUnitario { get; set; }

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }
    }
}