using System.ComponentModel.DataAnnotations;

namespace Doalim_dev.ViewModels
{
    public class PedidoConfirmadoViewModel
    {
        // Dados do Pedido
        public int IdPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public int TotalReservas { get; set; }

        // Lista de reservas associadas ao pedido
        public List<ResumoReservaViewModel> Reservas { get; set; } = new();
    }

    public class ResumoReservaViewModel
    {
        public int IdReserva { get; set; }
        public string NomeProduto { get; set; }
        public string MarcaProduto { get; set; }
        public string CategoriaProduto { get; set; }
        public string UnidadeProduto { get; set; }
        public string NumeroLote { get; set; }
        public DateTime DataValidadeLote { get; set; }
        public int QuantidadeDesejada { get; set; }
        public string NomeDoador { get; set; }
        public string StatusReserva { get; set; }

        // Foto do produto em Base64 — preenchida após SaveChanges no controller
        public string? FotoProduto { get; set; }

        // Aviso de itens que não puderam ser reservados

        // O campo Sucesso permite exibir na tela de confirmação quais itens foram reservados com sucesso
        // e quais falharam por indisponibilidade
        // Sucesso = false seria apenas para casos de dois usuários finalizando simultaneamente
        public bool Sucesso { get; set; } = true;
        public string? MotivoFalha { get; set; }
    }
}
