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

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
