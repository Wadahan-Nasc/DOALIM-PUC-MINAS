namespace Doalim_dev.ViewModels
{
    public class VitrineCompletaViewModel
    {
        public VitrineFiltroViewModel Filtros { get; set; } = new();
        public IEnumerable<VitrineDoacoesViewModel> Produtos { get; set; } = new List<VitrineDoacoesViewModel>();
    }
}
