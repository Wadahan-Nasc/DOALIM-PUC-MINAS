namespace Doalim_dev.ViewModels
{
    public class MinhasReservasPageViewModel
    {
        public MinhasReservasFiltroViewModel Filtros { get; set; } = new();
        public List<MinhasReservasViewModel> Reservas { get; set; } = new();

    }
}
