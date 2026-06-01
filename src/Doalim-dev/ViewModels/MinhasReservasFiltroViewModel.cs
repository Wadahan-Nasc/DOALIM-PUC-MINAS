namespace Doalim_dev.ViewModels
{
    public class MinhasReservasFiltroViewModel
    {
        public string? Status { get; set; }
        public string? Categoria { get; set; }
        public string? NomeProduto { get; set; }
        public string? NomeDoador { get; set; }
        public DateTime? ValidadeInicio { get; set; }
        public DateTime? ValidadeFim { get; set; }
        public DateTime? DataReservaInicio { get; set; }
        public DateTime? DataReservaFim { get; set; }

        public bool TemFiltroAtivo =>
            !string.IsNullOrWhiteSpace(Status) ||
            !string.IsNullOrWhiteSpace(Categoria) ||
            !string.IsNullOrWhiteSpace(NomeProduto) ||
            !string.IsNullOrWhiteSpace(NomeDoador) ||
            ValidadeInicio.HasValue || ValidadeFim.HasValue ||
            DataReservaInicio.HasValue || DataReservaFim.HasValue;
    }
}
