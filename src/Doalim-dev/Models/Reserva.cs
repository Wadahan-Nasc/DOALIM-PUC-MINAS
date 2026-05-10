using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    public enum StatusReserva
    {
        Pendente = 0,
        Confirmada = 1,
        Retirada = 2,
        Cancelada = 3
    }

    [Table("Reservas")]
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public DateTime DataReserva { get; set; } = DateTime.UtcNow;

        [Required]
        public StatusReserva Status { get; set; } = StatusReserva.Pendente;

        [Required]
        public int QuantidadeReservada { get; set; }

        // FK para o Produto reservado
        [Required]
        public int IdProduto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public Produto Produto { get; set; } = null!;

        // FK para o Beneficiario que fez a reserva
        [Required]
        public int IdBeneficiario { get; set; }

        [ForeignKey(nameof(IdBeneficiario))]
        public Beneficiario Beneficiario { get; set; } = null!;
    }
}