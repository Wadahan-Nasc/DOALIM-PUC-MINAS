namespace Doalim_dev.ViewModels
{
    /// <summary>
    /// ViewModel da pagina de gerenciamento de reservas do doador.
    /// Separa reservas por status para facilitar a renderizacao da view.
    /// </summary>
    public class GerenciarReservasPageViewModel
    {
        // Reservas aguardando aprovacao do doador
        public List<GerenciarReservaDoadorViewModel> Pendentes { get; set; } = new();

        // Reservas aprovadas aguardando retirada pelo beneficiario
        public List<GerenciarReservaDoadorViewModel> Confirmadas { get; set; } = new();

        // Reservas concluidas (Retirada) que o doador ainda nao avaliou
        public List<GerenciarReservaDoadorViewModel> PendentesAvaliacao { get; set; } = new();
    }
}
