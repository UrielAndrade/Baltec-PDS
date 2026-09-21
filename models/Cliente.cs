namespace Baltec.models

{
    public class Cliente
    {
        public int Id { get; set; }

        public string RazaoSocial { get; set; } = string.Empty;

        public string? NomeFantasia { get; set; }

        public string CnpjCpf { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        public string? Email { get; set; }

        public string? Endereco { get; set; }

        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; }
    }
}