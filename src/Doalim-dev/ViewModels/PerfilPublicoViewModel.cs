using Doalim_dev.Models;

namespace Doalim_dev.ViewModels
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

        // Dados de avaliação do usuário logado (se já avaliou este perfil)
        public int? NotaDoLogado { get; set; }
        public bool JaAvaliou => NotaDoLogado.HasValue;
        public bool PodeAvaliar { get; set; }
    }
}