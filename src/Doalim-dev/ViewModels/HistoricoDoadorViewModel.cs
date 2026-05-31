namespace Doalim_dev.ViewModels
{
    public class HistoricoDoadorViewModel
    {
        public int IdReserva { get; set; }
        public int IdPedido { get; set; }

        // Produto
        public string NomeProduto { get; set; } = "";
        public string MarcaProduto { get; set; } = "";
        public string CategoriaProduto { get; set; } = "";
        public string UnidadeMedidaProduto { get; set; } = "";
        public string? FotoProduto { get; set; }

        // Lote
        public string NumeroLote { get; set; } = "";
        public DateTime DataValidadeLote { get; set; }

        // Reserva
        public int QuantidadeReservada { get; set; }
        public string StatusReserva { get; set; } = "";
        public DateTime DataReserva { get; set; }
        public DateTime? DataEncerramento { get; set; }
        public string? MotivoRejeicao { get; set; }

        // Beneficiário
        public string NomeBeneficiario { get; set; } = "";
        public bool EhOng { get; set; }
    }
}
