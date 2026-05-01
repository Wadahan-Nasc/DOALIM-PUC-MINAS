using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table ("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage ="Obrigatório informar o CNPJ!")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o CPF!")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o telefone!")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o endereço!")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Obrigatório adicionar a foto do perfil!")]
        public string FotoPerfil { get; set; }

        [Required(ErrorMessage = "Obrigatório enviar o arquivo de comprovação(CNH, Identidade, Cadastro único, Cartao CNPJ!")]
        public string Arquivocomprovacao { get; set; }

        [Required(ErrorMessage = "Obrigatório selecionar o tipo de usuário!")]
        public int TipoUsuario { get; set; }

        [Required(ErrorMessage = "Obrigatório criar senha!")]
        public string SenhaHash { get; set; }

    }
}
