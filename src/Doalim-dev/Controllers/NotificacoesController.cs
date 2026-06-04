using Doalim_dev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class NotificacoesController : BaseController
    {
        public NotificacoesController(AppDbContext context) : base(context) { }

        // -----------------------------------------------------------------------
        // GET /Notificacoes/MinhasJson
        // Retorna as últimas notificações do usuário logado em JSON.
        // Também verifica e cria automaticamente:
        //   - Lembretes de retirada (reservas Confirmadas com DataRetiradaFim ≤ 2 dias)
        //   - Doações expiradas (lotes vencidos com reservas Pendentes/Confirmadas)
        // -----------------------------------------------------------------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MinhasJson()
        {
            if (User.Identity?.IsAuthenticated != true)
                return Json(new { notificacoes = Array.Empty<object>(), totalNaoLidas = 0 });

            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClaim, out var idUsuario))
                return Json(new { notificacoes = Array.Empty<object>(), totalNaoLidas = 0 });

            // ── Gera lembretes de retirada (ChaveDuplicacao evita duplicatas) ──
            if (User.IsInRole("BeneficiarioPF") || User.IsInRole("BeneficiarioPJ"))
            {
                var retiradaProxima = await _context.Reservas
                    .AsNoTracking()
                    .Where(r => r.IdBeneficiario == idUsuario
                             && r.Status == StatusReserva.Confirmada
                             && r.DataRetiradaFim.HasValue
                             && r.DataRetiradaFim.Value.Date <= DateTime.Today.AddDays(2)
                             && r.DataRetiradaFim.Value.Date >= DateTime.Today)
                    .Select(r => new { r.IdReserva, r.IdPedido, r.DataRetiradaFim })
                    .Distinct()
                    .ToListAsync();

                foreach (var r in retiradaProxima)
                {
                    var chave = $"lembrete-retirada-{r.IdPedido}";
                    await CriarNotificacaoAsync(
                        idUsuario          : idUsuario,
                        titulo             : "⏰ Retirada próxima!",
                        mensagem           : $"Pedido #{r.IdPedido}: prazo de retirada até {r.DataRetiradaFim!.Value:dd/MM/yyyy}. Não perca!",
                        tipo               : TipoNotificacao.LembreteRetirada,
                        url                : "/Reservas/MinhasReservas",
                        chaveDeduplicacao  : chave);
                }
            }

            // ── Gera alertas de doação expirada (lotes vencidos com reservas) ──
            if (User.IsInRole("DoadorPF") || User.IsInRole("DoadorPJ"))
            {
                var lotesExpirados = await _context.Reservas
                    .AsNoTracking()
                    .Where(r => r.Lote.Produto.IdDoador == idUsuario
                             && (r.Status == StatusReserva.Pendente || r.Status == StatusReserva.Confirmada)
                             && r.Lote.DataValidade.Date < DateTime.Today)
                    .Select(r => new { r.IdPedido, r.Lote.IdLote, r.Lote.NumeroLote })
                    .Distinct()
                    .ToListAsync();

                foreach (var l in lotesExpirados)
                {
                    var chave = $"expirado-lote-{l.IdLote}";
                    await CriarNotificacaoAsync(
                        idUsuario          : idUsuario,
                        titulo             : "⚠️ Doação expirada",
                        mensagem           : $"Lote {l.NumeroLote} do pedido #{l.IdPedido} venceu. Verifique as reservas associadas.",
                        tipo               : TipoNotificacao.DoacaoExpirada,
                        url                : "/Produtos/GerenciarReservas",
                        chaveDeduplicacao  : chave);
                }
            }

            // Salva eventuais notificações novas geradas acima
            if (_context.ChangeTracker.HasChanges())
                await _context.SaveChangesAsync();

            // ── Retorna as últimas 20 notificações ──
            var lista = await _context.Notificacoes
                .AsNoTracking()
                .Where(n => n.IdUsuario == idUsuario)
                .OrderByDescending(n => n.DataCriacao)
                .Take(20)
                .Select(n => new
                {
                    n.IdNotificacao,
                    n.Titulo,
                    n.Mensagem,
                    n.Url,
                    n.Lida,
                    n.Tipo,
                    DataCriacao = n.DataCriacao.ToString("dd/MM/yyyy HH:mm")
                })
                .ToListAsync();

            var totalNaoLidas = await _context.Notificacoes
                .CountAsync(n => n.IdUsuario == idUsuario && !n.Lida);

            return Json(new { notificacoes = lista, totalNaoLidas });
        }

        // -----------------------------------------------------------------------
        // POST /Notificacoes/MarcarLida/{id}
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarLida(int id)
        {
            var idUsuario = ObterIdUsuarioLogado();
            var notif = await _context.Notificacoes
                .FirstOrDefaultAsync(n => n.IdNotificacao == id && n.IdUsuario == idUsuario);

            if (notif != null)
            {
                notif.Lida = true;
                await _context.SaveChangesAsync();
            }

            return Json(new { ok = true });
        }

        // -----------------------------------------------------------------------
        // POST /Notificacoes/MarcarTodasLidas
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarTodasLidas()
        {
            var idUsuario = ObterIdUsuarioLogado();
            var pendentes = await _context.Notificacoes
                .Where(n => n.IdUsuario == idUsuario && !n.Lida)
                .ToListAsync();

            foreach (var n in pendentes)
                n.Lida = true;

            await _context.SaveChangesAsync();

            return Json(new { ok = true, marcadas = pendentes.Count });
        }
    }
}
