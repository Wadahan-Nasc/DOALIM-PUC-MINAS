using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Beneficiarios")]
    public class Beneficiario
    {
        [Key]
        public int IdUsuario { get; set; }

        public int CadastroUnico { get; set; }

        public bool Eong { get; set; }
              
        public int QuantidadeAlimentosRecebidos { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; }
    }
}
