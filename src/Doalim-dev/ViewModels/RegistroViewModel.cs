using System.ComponentModel.DataAnnotations;
using Doalim_dev.Models;

namespace Doalim_dev.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        [MaxLength(150, ErrorMessage = "Nome pode ter no máximo 150 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido. Ex: nome@dominio.com")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório criar uma senha!")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório confirmar a senha!")]
        [DataType(DataType.Password)]
        [Compare(nameof(Senha), ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o telefone!")]
        [MaxLength(20)]
        [RegularExpression(@"^\(\d{2}\)\s\d{4,5}-\d{4}$",
            ErrorMessage = "Telefone inválido. Use o formato (11) 99999-9999.")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o endereço!")]
        [Display(Name = "Endereço")]
        [MaxLength(300)]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório selecionar o tipo de usuário!")]
        [Display(Name = "Tipo de Usuário")]
        public TipoUsuario TipoUsuario { get; set; }

        // Obrigatório somente para PF — validado no controller
        [Display(Name = "CPF")]
        [MaxLength(14)]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$",
            ErrorMessage = "CPF inválido. Use o formato 000.000.000-00.")]
        public string? Cpf { get; set; }

        // Obrigatório somente para PJ — validado no controller
        [Display(Name = "CNPJ")]
        [MaxLength(18)]
        [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$",
            ErrorMessage = "CNPJ inválido. Use o formato 00.000.000/0001-00.")]
        public string? Cnpj { get; set; }

        // RF-002: Aceite obrigatório somente se TipoUsuario = DoadorPF ou DoadorPJ
        [Display(Name = "Li e aceito o Termo de Responsabilidade (Lei 14.016/2020)")]
        public bool AceitouTermo { get; set; }
    }
}
