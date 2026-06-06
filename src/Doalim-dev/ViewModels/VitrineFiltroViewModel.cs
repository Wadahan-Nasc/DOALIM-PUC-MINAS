namespace Doalim_dev.ViewModels
{
    public class VitrineFiltroViewModel
    {
        public int? QuantidadeMinima { get; set; }
        public string OrdemValidade { get; set; } = "asc";
        public string? NomeBusca { get; set; }
        public string? Categoria { get; set; }

        // Filtros de localidade
        // Por padrão ativo para beneficiários: filtra pela cidade do próprio usuário.
        // Quando desativado, o usuário pode digitar qualquer cidade (e opcionalmente bairro).
        public bool FiltrarPorCidade { get; set; } = true;
        public string? Cidade { get; set; }
        public string? Bairro { get; set; }
    }
}
