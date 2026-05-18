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

        public bool StatusLote { get; set; } = true; // Por padrão, o lote nasce ativo

        // Chave estrangeira
        public int IdProduto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public Produto Produto { get; set; }
    }
}