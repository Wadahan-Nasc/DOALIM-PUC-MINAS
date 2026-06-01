namespace Doalim_dev.ViewModels
{
    public class MinhasReservasViewModel
    {
        // Dados da reserva
        public int IdReserva { get; set; }
        public int IdPedido { get; set; }
        public DateTime DataReserva { get; set; }
        public string StatusReserva { get; set; }
        public int QuantidadeReservada { get; set; }

        // Token de confirmação da reserva
        // Exibido apenas quando Status = Confirmada (Reserva.cs)
        public string? TokenConfirmacao { get; set; }

        //Intervalo de retirada do pedido
        // É preenchido pelo doador durante aprovação da reserva
        public DateTime? DataRetiradaInicio { get; set; }
        public DateTime? DataRetiradaFim { get; set; }

        // Dados do lote reservado
        public string NumeroLote { get; set; }
        public DateTime DataValidadeLote { get; set; }

        // Dados do produto
        public string NomeProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string CategoriaProduto { get; set; }
        public string UnidadeMedidaProduto { get; set; }
        public string? FotoProduto { get; set; } // Base64 convertido no controller

        // Dados do doador
        public string NomeDoador { get; set; }
        public string TelefoneDoador { get; set; }

        // Motivo informado pelo doador ao rejeitar — exibido apenas quando Rejeitada
        public string? MotivoRejeicao { get; set; }

        //Campos Calculados
        public bool PodeSerCancelada =>
            StatusReserva == "Pendente" ||
            StatusReserva == "Confirmada";

        public bool ExibirToken =>
            StatusReserva == "Confirmada" && !string.IsNullOrEmpty(TokenConfirmacao);

        public bool RetiradaProxima =>
            DataRetiradaFim.HasValue && (DataRetiradaFim.Value.Date - DateTime.Today).TotalDays <= 2;
    }
}