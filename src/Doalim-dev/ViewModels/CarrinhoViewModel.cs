using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class CarrinhoViewModel
    {
        public List<CarrinhoItemViewModel> Itens { get; set; } = new();

        // Controle de expiração usando o menor prazo de expiração dos itens do carrinho
        public DateTime DataExpiracao =>
            Itens.Any() ? Itens.Min(i =>i.DataExpiracao) : DateTime.UtcNow;

        // Campos calculados
        public int TotalItens => Itens.Count;
        public int LimiteItens => 15;
        public bool LimiteAtingido => TotalItens >= LimiteItens;
        public int ItensRestantes => LimiteItens - TotalItens;

        public int MinutosRestantes => Math.Max(0, (int)(DataExpiracao - DateTime.UtcNow).TotalMinutes);
        public int SegundosRestantes => Math.Max(0, (int)(DataExpiracao - DateTime.UtcNow).TotalSeconds);
        public bool EstaExpirando => MinutosRestantes <= 5; // Alerta para expiração iminente
        public bool EstaExpirado => DateTime.UtcNow >= DataExpiracao;

        // Controla se há itens indisponíveis bloqueando a finalização da compra
        public bool PossuiItensIndisponiveis => Itens.Any(i => !i.LoteDisponivel);

        // Mensagem de status geral do carrinho
        public string? MensagemStatus { get; set; }
    }
}

/*
Todos os itens são adicionados com a mesma DataExpiracao 
Assim, o timer do carrinho começa na primeira adição e vale para todos os itens
*/