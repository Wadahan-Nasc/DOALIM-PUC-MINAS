using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o nome!")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        // CPF — obrigatório apenas para PF (validado no controller/ViewModel)
        [Display(Name = "CPF")]
        [MaxLength(14)]
        public string? Cpf { get; set; }

        // CNPJ — obrigatório apenas para PJ (validado no controller/ViewModel)
        [Display(Name = "CNPJ")]
        [MaxLength(18)]
        public string? Cnpj { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o e-mail!")]
        [Display(Name = "E-mail")]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Obrigatório informar o telefone!")]
        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Display(Name = "Foto do Perfil")]
        public byte[]? FotoPerfil { get; set; }

        [Display(Name = "Bio")]
        [MaxLength(300)]
        public string? Bio { get; set; }

        [Display(Name = "Arquivo de Comprovação")]
        public byte[]? ArquivoComprovacao { get; set; }

        // Armazena o hash BCrypt — NUNCA a senha em texto puro
        [Required]
        [Display(Name = "Senha")]
        public string SenhaHash { get; set; } = string.Empty;

        // Tipo de perfil usando enum (salvo como int no banco)
        [Required(ErrorMessage = "Obrigatório selecionar o tipo de usuário!")]
        [Display(Name = "Tipo de Usuário")]
        public TipoUsuario TipoUsuario { get; set; }

        // Status de verificação pelo Admin (RF-008)
        [Display(Name = "Status de Verificação")]
        public StatusVerificacao StatusVerificacao { get; set; } = StatusVerificacao.NaoAplicavel;

        // Soft delete — Admin pode suspender sem excluir
        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        // Campos para recuperação de senha (RF-001)
        [MaxLength(200)]
        public string? TokenRecuperacao { get; set; }

        public DateTime? TokenExpiracao { get; set; }

        // Navegação para termos aceitos (RF-002)
        public ICollection<TermoAceitacao> TermosAceitados { get; set; } = new List<TermoAceitacao>();

        // Navegação para endereço (1:1)
        public Endereco? Endereco { get; set; }
    }
}
