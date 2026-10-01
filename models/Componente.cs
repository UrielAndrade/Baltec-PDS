using System.ComponentModel.DataAnnotations;

namespace Baltec.models
{
    public class Componente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do componente é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome do componente deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O código do item é obrigatório.")]
        [StringLength(50, ErrorMessage = "O código do item deve ter no máximo 50 caracteres.")]
        public string CodigoItem { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
        public int FkCategoria { get; set; }

        public string? CategoriaNome { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
        public int QuantidadeEstoque { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "O preço não pode ser negativo.")]
        public decimal PrecoUnitario { get; set; }

        public DateTime DataCadastro { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
