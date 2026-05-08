namespace Doalim_dev.DTOs
{
    public class ReservaResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }

        public ReservaResponseDto(bool sucesso, string mensagem)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }
    }
}
