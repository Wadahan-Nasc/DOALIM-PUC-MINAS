using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    public enum StatusReserva
    {
        Pendente = 0,
        Confirmada = 1,
        Retirada = 2,
        Cancelada = 3,
        Rejeitada = 4 // Essencial para diferenciar reservas canceladas pelo beneficiário e rejeitadas pelo doador;
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

        // Quantidade efetivamente reservada do produto
        [Required]
        public int QuantidadeReservada { get; set; }

        // Token gerado pelo sistema quando o doador aprova a reserva.
        // Informado presencialmente pelo beneficiário ao retirar o produto.
        public string? TokenConfirmacao { get; set; }

        // Intervalo de retirada definido pelo doador ao aprovar a reserva.
        // Ambas as datas devem ser anteriores ao vencimento do lote.
        public DateTime? DataRetiradaInicio { get; set; }

        public DateTime? DataRetiradaFim { get; set; }

        // Data em que a reserva se encerrou (retirada ou cancelada)
        // Importante para históricos
        public DateTime? DataEncerramento { get; set; }

        // FK para o Pedido agrupador desta reserva
        [Required]
        public int? IdPedido { get; set; }

        [ForeignKey(nameof(IdPedido))]
        public Pedido Pedido { get; set; }

        // FK para o Lote reservado
        // Produto fica implícito através do Lote.IdProduto
        [Required]
        public int IdLote { get; set; }

        [ForeignKey(nameof(IdLote))]
        public Lote Lote { get; set; } = null!;

        // FK para o Beneficiario que fez a reserva
        [Required]
        public int IdBeneficiario { get; set; }

        [ForeignKey(nameof(IdBeneficiario))]
        public Beneficiario Beneficiario { get; set; } = null!;
    }
}