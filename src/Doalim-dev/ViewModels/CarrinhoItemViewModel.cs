using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class CarrinhoItemViewModel
    {
        public int IdCarrinho { get; set; }

        //Dados do produto
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public string CategoriaProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string UnidadeMedidaProduto { get; set; }
        public string? FotoProduto { get; set; }


        // Dados do lote mais urgente
        public string NumeroLote { get; set; }
        public DateTime DataValidadeLote { get; set; }
        public int QuantidadeDisponivelLote { get; set; }

        // Dados do doador
        public string NomeDoador { get; set; }

        // Quantidade desejada pelo beneficiário
        public int QuantidadeDesejada { get; set; }

        // Controle de Expiração
        public DateTime DataExpiracao { get; set; }

        // Campos calculados preenchidos no controller (não armazenados no banco)
        public int MinutosRestantes => Math.Max(0, (int)(DataExpiracao - DateTime.UtcNow).TotalMinutes);
        public bool EstaExpirando => MinutosRestantes <= 5;
        public bool EstaExpirado => MinutosRestantes == 0;

        // Aviso de disponibilidade preenchido no controller ao exibir o carrinho
        public bool LoteDisponivel { get; set; } = true;
        public string? AvisoDisponibilidade { get; set; }
    }
}
