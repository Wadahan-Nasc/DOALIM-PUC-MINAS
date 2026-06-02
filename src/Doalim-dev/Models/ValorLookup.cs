using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    /// <summary>
    /// Tipo fixo de domínio. Os valores dentro de cada tipo podem ser
    /// adicionados ou desativados pelo administrador em tempo de execução.
    /// </summary>
    public enum TipoLookup
    {
        Categoria = 0,
        TipoArmazenamento = 1,
        UnidadeMedida = 2
    }

    /// <summary>
    /// Tabela de domínio extensível: armazena os valores possíveis para
    /// Categoria, TipoArmazenamento e UnidadeMedida de produtos.
    /// Valores padrão populados via seed (HasData).
    /// O administrador pode adicionar novos valores ou desativar os existentes.
    /// </summary>
    [Table("ValoresLookup")]
    public class ValorLookup
    {
        [Key]
        public int IdValor { get; set; }

        [Required]
        public TipoLookup Tipo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = "";

        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Indica que este valor faz parte do seed inicial do sistema.
        /// Valores padrão não podem ser excluídos — apenas desativados.
        /// </summary>
        public bool EhValorPadrao { get; set; } = false;
    }
}
