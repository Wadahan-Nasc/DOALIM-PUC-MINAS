using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    // Passo 1: usuário informa o e-mail
    public class RecuperarSenhaViewModel
    {
        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;
    }

    // Passo 2: usuário define a nova senha usando o token recebido
    public class ResetSenhaViewModel
    {
        // Recebido via query string no link do e-mail — campo hidden na view
        [Required]
        public string Token { get; set; } = string.Empty;

        // Campo hidden na view, preenchido automaticamente
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar a nova senha!")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório confirmar a nova senha!")]
        [DataType(DataType.Password)]
        [Compare(nameof(NovaSenha), ErrorMessage = "As senhas não conferem.")]
        [Display(Name = "Confirmar Nova Senha")]
        public string ConfirmaSenha { get; set; } = string.Empty;
    }
}
