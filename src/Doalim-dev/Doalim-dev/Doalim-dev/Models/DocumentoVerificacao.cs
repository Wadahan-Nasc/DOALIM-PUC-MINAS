using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.Models
{
    public class DocumentoVerificacao
    {

        public int IdUsuario { get; set; }

        public string IdDocumento { get; set; }

        public string TipoDocumento { get; set; }
                
        public string Arquivo { get; set; }

        public DateTime DataEnvio { get; set; }

        public int StatusValidacao { get; set; }
    }
}
