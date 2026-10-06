namespace Baltec.models
{
    public class Equipamento
    {
        public int Id { get; set; }

        public int FkCliente { get; set; }

        public string? ClienteNome { get; set; }

        public string Modelo { get; set; } = string.Empty;

        public string MarcaFabricante { get; set; } = string.Empty;

        public decimal CapacidadeMaximaKg { get; set; }

        public decimal DivisaoEscalaG { get; set; }

        public string NumeroSerie { get; set; } = string.Empty;

        public string SetorLocalizacao { get; set; } = string.Empty;

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }
    }
}