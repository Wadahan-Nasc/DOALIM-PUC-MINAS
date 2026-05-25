using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    public class DocumentoVerificacao
    {
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
        public int IdUsuario { get; set; }

        [Key]
        public int IdDocumento { get; set; }

        public string TipoDocumento { get; set; }

        public byte[] Arquivo { get; set; }

        public DateTime DataEnvio { get; set; }

        public int StatusValidacao { get; set; }
    }
}
