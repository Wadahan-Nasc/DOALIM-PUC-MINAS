using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Enderecos")]
    public class Endereco
    {
        [Key]
        public int IdEndereco { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(9)]
        public string Cep { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Logradouro { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Numero { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [Required]
        [MaxLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required]
        [MaxLength(2)]
        public string Estado { get; set; } = string.Empty;

        // Navegação inversa
        public Usuario? Usuario { get; set; }
    }
}