namespace Doalim_dev.ViewModels
{
    /// <summary>
    /// ViewModel da página de gerenciamento de reservas do doador.
    /// Separa reservas por status para facilitar a renderização da view.
    /// </summary>
    public class GerenciarReservasPageViewModel
    {
        // Reservas aguardando aprovação do doador
        public List<GerenciarReservaDoadorViewModel> Pendentes { get; set; } = new();

        // Reservas aprovadas aguardando retirada pelo beneficiário
        public List<GerenciarReservaDoadorViewModel> Confirmadas { get; set; } = new();

        // Reservas concluídas (Retirada) com avaliação pendente do doador
        public List<GerenciarReservaDoadorViewModel> Retiradas { get; set; } = new();

        // ── Filtros ativos ────────────────────────────────────────────────
        public string? FiltroNomeBeneficiario { get; set; }
        public DateTime? FiltroDataInicio { get; set; }
        public DateTime? FiltroDataFim { get; set; }
        public string? FiltroStatus { get; set; }

        public bool TemFiltroAtivo =>
            !string.IsNullOrWhiteSpace(FiltroNomeBeneficiario)
            || FiltroDataInicio.HasValue
            || FiltroDataFim.HasValue
            || !string.IsNullOrWhiteSpace(FiltroStatus);
    }
}
