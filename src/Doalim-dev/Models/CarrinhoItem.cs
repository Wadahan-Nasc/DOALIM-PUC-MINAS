using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("CarrinhoItens")]
    public class CarrinhoItem
    {
        [Key]
        public int IdCarrinhoItem { get; set; }

        // Quantidade do produto adicionada ao carrinho
        // Ou seja, qtd que o beneficiário deseja reservar, mais ainda não foi efetivada pela reserva;
        [Required]
        public int QuantidadeDesejada { get; set; }

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

/*
Como o pedido é um agrupador de itens, o carrinho é um agrupador de itens do carrinho;
Ou seja, o carrinho não é uma entidade única, mas sim um agrupamento de itens do carrinho;
É uma simplificação do modelo que evita a necessidade de uma entidade "Carrinho" separada, mas que ainda é funcional;
O IdCarrinhoItem é a PK de cada linha individual, não do carrinho como um todo;
E cada item do carrinho tem um IdBeneficiario que indica a qual beneficiário ele pertence;

Exemplo:
CarrinhoItens do IdBeneficiario = 42:
├── CarrinhoItem #1 → Arroz Camil, qtd 2
├── CarrinhoItem #2 → Feijão Preto, qtd 1
└── CarrinhoItem #3 → Macarrão, qtd 3

*/