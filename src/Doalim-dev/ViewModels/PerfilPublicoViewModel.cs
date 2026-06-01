namespace Doalim_dev.Models.ViewModels
{
    public class PerfilPublicoViewModel
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public byte[]? FotoPerfil { get; set; }
        public string? Bio { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public bool Verificado { get; set; }
        public DateTime MembroDesde { get; set; }
        public double? NotaMedia { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
}