namespace Doalim_dev.Models
{
    public class Doacao
    {
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public int Quantidade { get; set; }

        // Status atual — começa como Disponivel automaticamente
        public StatusDoacao Status { get; set; } = StatusDoacao.Disponivel;

        // Quem cadastrou a doação (Doador)
        public int DoadorId { get; set; }

        // Quem reservou — fica nulo até alguém reservar
        public int? BeneficiarioId { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        public DateTime? DataReserva { get; set; }
    }
}
