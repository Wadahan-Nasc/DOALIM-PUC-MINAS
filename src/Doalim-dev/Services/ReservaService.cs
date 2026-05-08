using Doalim_dev.DTOs;
using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Services
{
    public class ReservaService : IReservaService
    {
        private readonly AppDbContext _context;

        public ReservaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReservaResponseDto> ReservarDoacaoAsync(int doacaoId, int beneficiarioId)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.IdProduto == doacaoId);

            if (produto == null)
                return new ReservaResponseDto(false, "Doação não encontrada.");

            if (!produto.StatusProduto || produto.IdBeneficiario.HasValue || produto.Quantidade <= 0)
                return new ReservaResponseDto(false, "Esta doação não está mais disponível.");

            if (produto.IdDoador == beneficiarioId)
                return new ReservaResponseDto(false, "Você não pode reservar a própria doação.");

            produto.StatusProduto = false;
            produto.IdBeneficiario = beneficiarioId;
            produto.DataReserva = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ReservaResponseDto(true, "Doação reservada com sucesso!");
        }

        public async Task<ReservaResponseDto> CancelarReservaAsync(int doacaoId, int beneficiarioId)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p =>
                    p.IdProduto == doacaoId &&
                    p.IdBeneficiario == beneficiarioId);

            if (produto == null)
                return new ReservaResponseDto(false, "Reserva não encontrada.");

            if (produto.StatusProduto)
                return new ReservaResponseDto(false, "Não é possível cancelar esta reserva.");

            produto.StatusProduto = true;
            produto.IdBeneficiario = null;
            produto.DataReserva = null;

            await _context.SaveChangesAsync();

            return new ReservaResponseDto(true, "Reserva cancelada com sucesso.");
        }
    }
}
