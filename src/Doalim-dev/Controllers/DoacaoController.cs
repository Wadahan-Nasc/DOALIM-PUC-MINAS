using Doalim_dev.DTOs;
using Doalim_dev.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoacaoController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public DoacaoController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        // POST /api/doacao/5/reservar
        [HttpPost("{doacaoId}/reservar")]
        public async Task<IActionResult> Reservar(int doacaoId)
        {
            var beneficiarioId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var resultado = await _reservaService
                .ReservarDoacaoAsync(doacaoId, beneficiarioId);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // DELETE /api/doacao/5/reservar
        [HttpDelete("{doacaoId}/reservar")]
        public async Task<IActionResult> CancelarReserva(int doacaoId)
        {
            var beneficiarioId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var resultado = await _reservaService
                .CancelarReservaAsync(doacaoId, beneficiarioId);

            if (!resultado.Sucesso)
                return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}