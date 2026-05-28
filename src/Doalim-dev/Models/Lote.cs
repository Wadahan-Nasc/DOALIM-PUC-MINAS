using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Lotes")]
    public class Lote
    {
        [Key]
        public int IdLote { get; set; }

        [Required(ErrorMessage = "O número do lote é obrigatório.")]
        [Display(Name = "Número do Lote")]
        public string NumeroLote { get; set; }

        [Required(ErrorMessage = "A data de validade é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Validade")]
        public DateTime DataValidade { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Display(Name = "Status do Lote")]
        public StatusLote StatusLote { get; set; } = StatusLote.Disponivel; // Por padrão, o lote nasce disponível

        // Chave estrangeira
        public int IdProduto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public Produto Produto { get; set; }

        // Navegação inversa para Reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}