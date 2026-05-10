using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doalim_dev.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int IdProduto { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatório.")]
        [Display(Name = "Nome do Produto")]
        public string NomeProduto { get; set; }

        [Required(ErrorMessage = "O código de barras é obrigatório.")]
        [Display(Name = "Código de Barras")]
        public string CodigoBarras { get; set; }

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [Display(Name = "Marca")]
        public string MarcaProduto { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Display(Name = "Categoria")]
        public string CategoriaProduto { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Display(Name = "Quantidade disponível")]
        public int Quantidade { get; set; }

        [Display(Name = "Tipo de Armazenamento")]
        public string TipoArmazenamento { get; set; }

        [Required(ErrorMessage = "A unidade de medida é obrigatória.")]
        [Display(Name = "Unidade de Medida")]
        public string UnidadeMedida { get; set; }

        [Required(ErrorMessage = "A data de validade é obrigatória.")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de Validade")]
        public DateTime DataValidade { get; set; }

        [Display(Name = "Foto do Produto")]
        public byte[]? FotoProduto { get; set; }

        [Display(Name = "Quantidade p/ Pessoa Fisíca")]
        public int QuantidadePessoaFisica { get; set; }

        [Display(Name = "Quantidade p/ Pessoa Jurídica")]
        public int QuantidadePessoaJuridica { get; set; }

        [Display(Name = "Ativo")]
        public bool StatusProduto { get; set; } = true;

        [Display(Name = "Data de Cadastro")]
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        [Required]
        public int IdDoador { get; set; }

        [ForeignKey(nameof(IdDoador))]
        public Doador Doador { get; set; }
    }
}
