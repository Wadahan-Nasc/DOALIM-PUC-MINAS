using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Doacoes")]
    public class Doacao
    {
        [Key]
        public int IdProduto { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório!")]
        [MaxLength(40, ErrorMessage = "O campo Nome deve ter no máximo 40 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        public bool StatusProduto { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Obrigatório informar a validade do produto!")]
        public DateTime DataValidade { get; set; }

        [MaxLength(15, ErrorMessage = "O campo Categoria deve ter no máximo 15 caracteres.")]
        public string Categoria { get; set; } = string.Empty;

        [MaxLength(15, ErrorMessage = "O campo Tipo de Armazenamento deve ter no máximo 15 caracteres.")]
        public string TipoArmazenamento { get; set; } = string.Empty;

        [MaxLength(15, ErrorMessage = "O campo Marca do Produto deve ter no máximo 15 caracteres.")]
        public string MarcaProduto { get; set; } = string.Empty;

        public string FotoProduto { get; set; }

        [Required]
        public int QuantidadeDisponivel { get; set; }

        // FK para Doador
        [Required]
        public int IdDoador { get; set; }

        [ForeignKey(nameof(IdDoador))]
        public Doador Doador { get; set; }


    }
}
