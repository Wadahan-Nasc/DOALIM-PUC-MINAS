using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table ("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        public string Nome { get; set; }

        [Required(ErrorMessage ="Obrigatório informar o CNPJ!")]
        [Display(Name = "CNPJ")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o CPF!")]
        [Display(Name = "CPF")]
        public string Cpf { get; set; }
              
        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o telefone!")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o endereço!")]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "Obrigatório adicionar a foto do perfil!")]
        [Display(Name ="Foto do Perfil")]
        public string FotoPerfil { get; set; }

        [Required(ErrorMessage = "Obrigatório enviar o arquivo de comprovação(CNH, Identidade, Cadastro único, Cartao CNPJ!")]
        [Display(Name = "Arquivo de Comprovação")]
        public string Arquivocomprovacao { get; set; }

        [Required(ErrorMessage = "Obrigatório selecionar o tipo de usuário!")]
        [Display(Name = "Tipo de Usuário")]
        public int TipoUsuario { get; set; }

        [Required(ErrorMessage = "Obrigatório criar senha!")]
        [Display(Name = "Senha")]
        public string SenhaHash { get; set; }

    }
}
