namespace Baltec.models;

public class Usuario
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Email { get; set; } = string.Empty;
    public int FkCargo { get; set; }
    public string? CargoNome { get; set; }
    public string SenhaHash { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
}
