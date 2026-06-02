using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    /// <summary>
    /// Avaliação de 1 a 5 estrelas entre usuários que já interagiram.
    /// Cada reserva concluída (Retirada) permite uma avaliação por participante — o índice único em
    /// (IdAvaliador, IdReserva) garante isso.
    /// </summary>
    [Table("Avaliacoes")]
    public class Avaliacao
    {
        [Key]
        public int IdAvaliacao { get; set; }

        [Required]
        public int IdAvaliador { get; set; }

        [Required]
        public int IdAvaliado { get; set; }

        // Avaliação vinculada à reserva que originou a interação
        public int? IdReserva { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        public int Nota { get; set; }

        [MaxLength(500)]
        public string? Comentario { get; set; }

        public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(IdAvaliador))]
        public Usuario Avaliador { get; set; } = null!;

        [ForeignKey(nameof(IdAvaliado))]
        public Usuario Avaliado { get; set; } = null!;

        [ForeignKey(nameof(IdReserva))]
        public Reserva? Reserva { get; set; }
    }
}
