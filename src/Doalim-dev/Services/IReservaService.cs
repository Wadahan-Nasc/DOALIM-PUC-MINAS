using Doalim_dev.DTOs;

namespace Doalim_dev.Services
{
    public interface IReservaService
    {
        Task<ReservaResponseDto> ReservarDoacaoAsync(int doacaoId, int beneficiarioId);
        Task<ReservaResponseDto> CancelarReservaAsync(int doacaoId, int beneficiarioId);
    }
}
