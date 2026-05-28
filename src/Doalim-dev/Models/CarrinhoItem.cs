using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("CarrinhoItens")]
    public class CarrinhoItem
    {
        [Key]
        public int IdCarrinhoItem { get; set; }

        // Momento em que o item foi adicionado ao carrinho
        [Required]
        public DateTime DataAdicao { get; set; } = DateTime.UtcNow;

        // Expiração do carrinho: 30 minutos após a criação do primeiro item do grupo
        // Todos os itens de um mesmo beneficiário compartilham a mesma expiração
        [Required]
        public DateTime Expiracao { get; set; }

        // FK para o Beneficiario dono do carrinho
        [Required]
        public int IdBeneficiario { get; set; }

        [ForeignKey(nameof(IdBeneficiario))]
        public Beneficiario Beneficiario { get; set; } = null!;

        // FK para o Produto adicionado
        // O lote específico só é definido na finalização do pedido (FIFO)
        [Required]
        public int IdProduto { get; set; }

        [ForeignKey(nameof(IdProduto))]
        public Produto Produto { get; set; } = null!;
    }
}