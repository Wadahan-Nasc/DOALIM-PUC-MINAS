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
            // 1. Busca a doação no banco
            var doacao = await _context.Doacoes
                .FirstOrDefaultAsync(d => d.Id == doacaoId);

            // 2. Existe?
            if (doacao == null)
                return new ReservaResponseDto(false, "Doação não encontrada.");

            // 3. Ainda está disponível?
            if (doacao.Status != StatusDoacao.Disponivel)
                return new ReservaResponseDto(false, "Esta doação não está mais disponível.");

            // 4. Faz a reserva
            doacao.Status = StatusDoacao.Reservado;
            doacao.BeneficiarioId = beneficiarioId;
            doacao.DataReserva = DateTime.UtcNow;

            // 5. Salva no banco
            await _context.SaveChangesAsync();

            return new ReservaResponseDto(true, "Doação reservada com sucesso!");
        }

        public async Task<ReservaResponseDto> CancelarReservaAsync(int doacaoId, int beneficiarioId)
        {
            // Garante que a reserva pertence a este beneficiário
            var doacao = await _context.Doacoes
                .FirstOrDefaultAsync(d =>
                    d.Id == doacaoId &&
                    d.BeneficiarioId == beneficiarioId);

            if (doacao == null)
                return new ReservaResponseDto(false, "Reserva não encontrada.");

            if (doacao.Status != StatusDoacao.Reservado)
                return new ReservaResponseDto(false, "Não é possível cancelar esta reserva.");

            // Volta ao estado disponível
            doacao.Status = StatusDoacao.Disponivel;
            doacao.BeneficiarioId = null;
            doacao.DataReserva = null;

            await _context.SaveChangesAsync();

            return new ReservaResponseDto(true, "Reserva cancelada com sucesso.");
        }
    }
}
