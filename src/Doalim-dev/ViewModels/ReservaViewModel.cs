using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class LoteDisponivelViewModel
    {
        public int IdLote { get; set; }
        public string NumeroLote { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public DateTime DataValidade { get; set; }
    }

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
        // Data de validade do lote mais distante de vencer entre os lotes ativos
        public DateTime DataValidadeProduto { get; set; }
        public byte[]? FotoProduto { get; set; }

        //Lista de lotes disponíveis para reserva — preenchida pelo controller
        public List<LoteDisponivelViewModel> LotesDisponiveis { get; set; } = new ();

        // Input do usuário

        [Required(ErrorMessage ="Selecione um lote.")]
        public int IdLoteSelecionado { get; set; }

        [Required(ErrorMessage = "Informe a quantidade desejada.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int QuantidadeReservada { get; set; }
    }
}