using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Doadores")]
    public class Doador
    {
        [Key]
        public int IdUsuario { get; set; }

        public string QtdAlimentosDoados { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; }

        public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();
    }
}
