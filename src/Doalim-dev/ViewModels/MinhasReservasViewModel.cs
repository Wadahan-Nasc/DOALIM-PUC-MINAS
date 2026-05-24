namespace Doalim_dev.ViewModels
{
    public class MinhasReservasViewModel
    {
        public int IdReserva { get; set; }
        public DateTime DataReserva { get; set; }
        public string StatusReserva { get; set; }
        public int QuantidadeReservada { get; set; }

        // Dados do lote
        public string NumeroLote { get; set; }
        public DateTime DataValidadeLote { get; set; }

        // Dados do produto
        public string NomeProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string Categoria { get; set; }
        public string UnidadeMedida { get; set; }
        public string? FotoProduto { get; set; } // Base64 convertido no controller

        // Dados do doador
        public string NomeDoador { get; set; }
    }
}