namespace Doalim_dev.ViewModels
{
    public class HistoricoDoadorPageViewModel
    {
        public HistoricoDoadorFiltroViewModel Filtros { get; set; } = new();
        public List<HistoricoDoadorViewModel> Itens { get; set; } = new();

        // Totalizadores calculados a partir dos itens retornados (já filtrados)
        public int TotalConcluidas => Itens.Count(i => i.StatusReserva == "Retirada");
        public int TotalUnidadesDoadas => Itens.Where(i => i.StatusReserva == "Retirada").Sum(i => i.QuantidadeReservada);
        public int TotalRejeitadas => Itens.Count(i => i.StatusReserva == "Rejeitada");
    }
}
