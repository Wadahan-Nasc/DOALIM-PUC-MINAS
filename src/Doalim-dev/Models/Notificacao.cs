using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Notificacoes")]
    public class Notificacao
    {
        [Key]
        public int IdNotificacao { get; set; }

        public int IdUsuario { get; set; }

        [Required, MaxLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Mensagem { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Url { get; set; }

        public TipoNotificacao Tipo { get; set; }

        public bool Lida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Campo para evitar duplicação de notificações geradas automaticamente
        // (ex: "lembrete-reserva-42", "expirado-lote-7")
        [MaxLength(100)]
        public string? ChaveDuplicacao { get; set; }

        // Navegação
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;
    }
}
