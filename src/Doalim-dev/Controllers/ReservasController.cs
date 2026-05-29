using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------------------------------------
        // GET: /Reservas/MinhasReservas
        // Exibe o histórico de reservas do beneficiário logado, agrupadas por pedido.
        // -----------------------------------------------------------------------------------------
        public async Task<IActionResult> MinhasReservas()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var reservas = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                        .ThenInclude(p => p.Doador)
                            .ThenInclude(d => d.Usuario)
                .Where(r => r.IdBeneficiario == usuarioId)
                .OrderByDescending(r => r.DataReserva)
                .Select(r => new MinhasReservasViewModel
                {
                    IdReserva = r.IdReserva,
                    IdPedido = r.IdPedido ?? 0,
                    DataReserva = r.DataReserva,
                    StatusReserva = r.Status.ToString(),
                    QuantidadeReservada = r.QuantidadeReservada,
                    TokenConfirmacao = r.TokenConfirmacao,
                    DataRetiradaInicio = r.DataRetiradaInicio,
                    DataRetiradaFim = r.DataRetiradaFim,
                    NumeroLote = r.Lote.NumeroLote,
                    DataValidadeLote = r.Lote.DataValidade,
                    NomeProduto = r.Lote.Produto.NomeProduto,
                    MarcaProduto = r.Lote.Produto.MarcaProduto,
                    CategoriaProduto = r.Lote.Produto.CategoriaProduto,
                    UnidadeMedidaProduto = r.Lote.Produto.UnidadeMedida,
                    FotoProduto = r.Lote.Produto.FotoProduto == null
                        ? null
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(r.Lote.Produto.FotoProduto)}",
                    NomeDoador = r.Lote.Produto.Doador.Usuario.Nome,
                    TelefoneDoador = r.Lote.Produto.Doador.Usuario.Telefone
                })
                .ToListAsync();

            return View(reservas);
        }

        // -----------------------------------------------------------------------------------------
        // POST: /Reservas/Cancelar
        // Cancela uma reserva individual — disponível para status
        // Pendente e Confirmada. Devolve o lote para a vitrine
        // se ainda estiver dentro da validade.
        // -----------------------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int idReserva)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var reserva = await _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                .FirstOrDefaultAsync(r => r.IdReserva == idReserva
                                       && r.IdBeneficiario == usuarioId);

            if (reserva == null)
                return NotFound();

            // Valida se a reserva pode ser cancelada
            if (reserva.Status != StatusReserva.Pendente
             && reserva.Status != StatusReserva.Confirmada)
            {
                TempData["Erro"] = "Esta reserva não pode ser cancelada.";
                return RedirectToAction(nameof(MinhasReservas));
            }

            // Cancela a reserva
            reserva.Status = StatusReserva.Cancelada;
            reserva.DataEncerramento = DateTime.UtcNow;

            // Devolve a quantidade ao lote se ainda estiver dentro da validade
            if (reserva.Lote.DataValidade.Date >= DateTime.Today)
            {
                reserva.Lote.Quantidade += reserva.QuantidadeReservada;
                reserva.Lote.StatusLote = StatusLote.Disponivel;
                reserva.Lote.Produto.StatusProduto = true; // reativa o produto se estava inativo
            }

            // Atualiza o status do pedido
            await AtualizarStatusPedidoAsync(reserva.IdPedido);

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Reserva cancelada com sucesso.";
            return RedirectToAction(nameof(MinhasReservas));
        }


        // -----------------------------------------------------------------------------------------
        // MÉTODO AUXILIAR
        // Atualiza o status do Pedido com base no estado atual
        // de todas as suas reservas filhas.
        // -----------------------------------------------------------------------------------------
        private async Task AtualizarStatusPedidoAsync (int? idPedido)
        {
            if (idPedido == null) return;

            var pedido = await _context.Pedidos
                .Include(p => p.Reservas)
                .FirstOrDefaultAsync(p=> p.IdPedido == idPedido);

            if (pedido == null) return;

            var statusReservas = pedido.Reservas.Select(r=>r.Status).ToList();

            pedido.StatusPedido = statusReservas.All(s => s == StatusReserva.Retirada)
                      ? StatusPedido.Retirado
                      : statusReservas.All(s => s == StatusReserva.Cancelada || s == StatusReserva.Rejeitada)
                                 ? StatusPedido.Cancelado
                                 : statusReservas.Any(s => s == StatusReserva.Confirmada)
                                            ? StatusPedido.Confirmado
                                            : StatusPedido.Pendente;
        }

        // -----------------------------------------------------------------------------------------
        // MÉTODO AUXILIAR
        // Lê o ID do usuário logado a partir dos Claims.
        // -----------------------------------------------------------------------------------------
        private int? ObterUsuarioId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : null;
        }

    }
}
