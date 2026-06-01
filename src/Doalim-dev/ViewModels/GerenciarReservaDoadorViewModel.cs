using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class GerenciarReservaDoadorViewModel
    {
        // Dados da reserva
        public int IdReserva { get; set; }
        public int IdPedido { get; set; }
        public DateTime DataReserva { get; set; }
        public string StatusReserva { get; set; }
        public int QuantidadeReservada { get; set; }

        // Dados do lote
        public string NumeroLote { get; set; }
        public DateTime DataValidadeLote { get; set; }

        // Dados do produto
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string CategoriaProduto { get; set; }
        public string UnidadeMedidaProduto { get; set; }
        public string? FotoProduto { get; set; } // Base64 convertido no controller

        // Dados do beneficiário
        public string NomeBeneficiario { get; set; }
        public string TelefoneBeneficiario { get; set; }
        public bool EhOng { get; set; }

        // Campos preenchidos pelo doador ao aprovar
        public DateTime? DataRetiradaInicio { get; set; }
        public DateTime? DataRetiradaFim { get; set; }

        // Token de confirmação — exibido ao doador após aprovação
        public string? TokenConfirmacao { get; set; }

        //Campos calculados
        public bool PodeAprovar =>
            StatusReserva == "Pendente";

        public bool PodeRejeitar =>
            StatusReserva == "Pendente";

        public bool PodeConfirmarEntrega =>
            StatusReserva == "Confirmada" &&
            !string.IsNullOrEmpty(TokenConfirmacao);

        public bool RetiradaProxima =>
            DataRetiradaFim.HasValue &&
            (DataRetiradaFim.Value - DateTime.Today).TotalDays <= 2;

        public bool LoteProximoVencimento =>
            (DataValidadeLote - DateTime.Today).TotalDays <= 7;
    }

    public class AprovarReservaViewModel
    {
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Informe a data de início da retirada.")]
        public DateTime DataRetiradaInicio { get; set; }

        [Required(ErrorMessage = "Informe a data fim da retirada.")]
        public DateTime DataRetiradaFim { get; set; }
    }
}