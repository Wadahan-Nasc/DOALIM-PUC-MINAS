using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        [Required]
        public StatusPedido StatusPedido { get; set; } = StatusPedido.Pendente;

        // FK para o Beneficiario que criou o pedido
        [Required]
        public int IdBeneficiario { get; set; }

        [ForeignKey(nameof(IdBeneficiario))]
        public Beneficiario Beneficiario { get; set; } = null!;

        // Reservas agrupadas neste pedido
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}