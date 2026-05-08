namespace Doalim_dev.ViewModels
{
    public class VitrineDoacoesViewModel
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public DateTime DataValidade { get; set; }
        public string Categoria { get; set; }
        public string MarcaProduto { get; set; }
        public string TipoArmazenamento { get; set; }
        public string FotoProduto { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string NomeDoador { get; set; }
    }
}
