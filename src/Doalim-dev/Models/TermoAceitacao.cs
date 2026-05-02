using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("TermosAceitacao")]
    public class TermoAceitacao
    {
        [Key]
        public int Id { get; set; }

        // FK para Usuario
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        // Timestamp exato do aceite — evidência jurídica (Lei 14.016/2020)
        [Required]
        public DateTime DataAceite { get; set; } = DateTime.UtcNow;

        // Versão do termo para controle futuro
        [Required]
        [MaxLength(20)]
        public string VersaoTermo { get; set; } = "v1.0-2026";

        // IP de origem para conformidade com LGPD
        [MaxLength(45)]
        public string? IpOrigem { get; set; }
    }
}
