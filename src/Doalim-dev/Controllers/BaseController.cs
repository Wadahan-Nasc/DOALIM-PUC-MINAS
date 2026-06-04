using Doalim_dev.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    // =====================================================================
    // BaseController — Classe base para todos os controllers da aplicação.
    // Centraliza métodos auxiliares compartilhados, evitando duplicação.
    // =====================================================================
    public abstract class BaseController : Controller
    {
        protected readonly AppDbContext _context;

        protected BaseController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------------------
        // Lê o ID do usuário logado a partir dos Claims da autenticação.
        // Retorna 0 se não estiver autenticado ou o claim for inválido.
        // -------------------------------------------------------------------------
        protected int ObterIdUsuarioLogado()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }

        protected async Task<bool> UsuarioComprovadoAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            return usuario != null && UsuarioRegras.TemComprovacaoAprovada(usuario);
        }

        protected async Task<bool> UsuarioPodeDoarAsync(int usuarioId)
        {
            if (!await _context.Doadores.AsNoTracking().AnyAsync(d => d.IdUsuario == usuarioId))
                return false;

            return await UsuarioComprovadoAsync(usuarioId);
        }

        protected async Task<bool> UsuarioPodeReservarAsync(int usuarioId)
        {
            if (!await _context.Beneficiarios.AsNoTracking().AnyAsync(b => b.IdUsuario == usuarioId))
                return false;

            return await UsuarioComprovadoAsync(usuarioId);
        }

        // -------------------------------------------------------------------------
        // Recalcula e persiste o StatusPedido com base no estado atual
        // de todas as suas Reservas filhas.
        //
        // Regras:
        //   Todas Retiradas           → Pedido.Retirado
        //   Todas Canceladas/Rejeitadas → Pedido.Cancelado
        //   Alguma Confirmada         → Pedido.Confirmado
        //   Demais casos              → Pedido.Pendente
        // -------------------------------------------------------------------------
        // -------------------------------------------------------------------------
        // Converte um byte[] de foto de produto em data URL, detectando PNG/JPEG/SVG
        // pelo magic bytes para definir o MIME type correto.
        // Disponível para ProdutosController, CarrinhoController e ReservasController.
        // -------------------------------------------------------------------------
        protected static string ObterFotoProdutoDataUrl(byte[]? fotoProduto)
        {
            if (fotoProduto == null || fotoProduto.Length == 0) return string.Empty;

            string mime;

            if (fotoProduto.Length >= 4
                && fotoProduto[0] == 0x89 && fotoProduto[1] == 0x50
                && fotoProduto[2] == 0x4E && fotoProduto[3] == 0x47)
            {
                mime = "image/png";
            }
            else if (fotoProduto.Length >= 2
                && fotoProduto[0] == 0xFF && fotoProduto[1] == 0xD8)
            {
                mime = "image/jpeg";
            }
            else
            {
                mime = "image/jpeg"; // fallback seguro para a maioria dos uploads
            }

            return $"data:{mime};base64,{Convert.ToBase64String(fotoProduto)}";
        }

        // -------------------------------------------------------------------------
        // Cria uma notificação para o usuário.
        // Se chaveDeduplicacao for informada, ignora silenciosamente duplicatas.
        // -------------------------------------------------------------------------
        protected async Task CriarNotificacaoAsync(
            int idUsuario,
            string titulo,
            string mensagem,
            TipoNotificacao tipo,
            string? url = null,
            string? chaveDeduplicacao = null)
        {
            // Evita duplicata quando a chave já existe para este usuário
            if (chaveDeduplicacao != null)
            {
                var jaExiste = await _context.Notificacoes
                    .AnyAsync(n => n.IdUsuario == idUsuario && n.ChaveDuplicacao == chaveDeduplicacao);
                if (jaExiste) return;
            }

            _context.Notificacoes.Add(new Notificacao
            {
                IdUsuario        = idUsuario,
                Titulo           = titulo,
                Mensagem         = mensagem,
                Tipo             = tipo,
                Url              = url,
                DataCriacao      = DateTime.UtcNow,
                Lida             = false,
                ChaveDuplicacao  = chaveDeduplicacao
            });
        }

        protected async Task AtualizarStatusPedidoAsync(int? idPedido)
        {
            if (idPedido == null) return;

            var pedido = await _context.Pedidos
                .Include(p => p.Reservas)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

            if (pedido == null) return;

            var statusReservas = pedido.Reservas.Select(r => r.Status).ToList();

            pedido.StatusPedido = statusReservas.All(s => s == StatusReserva.Retirada)
                      ? StatusPedido.Retirado
                      : statusReservas.All(s => s == StatusReserva.Cancelada || s == StatusReserva.Rejeitada)
                                 ? StatusPedido.Cancelado
                                 : statusReservas.Any(s => s == StatusReserva.Confirmada)
                                            ? StatusPedido.Confirmado
                                            : StatusPedido.Pendente;
        }
    }
}
