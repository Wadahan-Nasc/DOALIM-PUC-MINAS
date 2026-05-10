using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class ReservaViewModel
    {
        public int IdProduto { get; set; }

        // Dados do produto — preenchidos pelo controller
        public string NomeProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string Categoria { get; set; }
        public string UnidadeMedida { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public int QuantidadePessoaFisica { get; set; }
        public int QuantidadePessoaJuridica { get; set; }
        public DateTime DataValidade { get; set; }
        public byte[]? FotoProduto { get; set; }

        // Input do usuário
        [Required(ErrorMessage = "Informe a quantidade desejada.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int QuantidadeReservada { get; set; }
    }
}