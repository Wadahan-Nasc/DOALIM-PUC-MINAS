using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.Models
{
    public class Beneficiario
    {
        public int IdUsuario { get; set; }
               
        public int CadastroUnico { get; set; }

        public bool Eong { get; set; }
              
        public int QuantidadeAlimentosRecebidos { get; set; }
    }
}
