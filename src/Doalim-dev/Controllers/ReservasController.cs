using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class ReservasController : BaseController
    {
        public ReservasController(AppDbContext context) : base(context) { }

        // -----------------------------------------------------------------------------------------
        // GET: /Reservas/MinhasReservas
        // Exibe o histórico de reservas do beneficiário logado com filtros opcionais.
        // -----------------------------------------------------------------------------------------
        public async Task<IActionResult> MinhasReservas(MinhasReservasFiltroViewModel filtros)
        {
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Auth");

            var query = _context.Reservas
                .Include(r => r.Lote)
                    .ThenInclude(l => l.Produto)
                        .ThenInclude(p => p.Doador)
                            .ThenInclude(d => d.Usuario)
                                .ThenInclude(u => u.Endereco)
                .Where(r => r.IdBeneficiario == usuarioId)
                .AsQueryable();

            // Filtro por status
            if (!string.IsNullOrWhiteSpace(filtros.Status) &&
                Enum.TryParse<StatusReserva>(filtros.Status, out var statusEnum))
                query = query.Where(r => r.Status == statusEnum);

            // Filtro por categoria
            if (!string.IsNullOrWhiteSpace(filtros.Categoria))
                query = query.Where(r => r.Lote.Produto.CategoriaProduto == filtros.Categoria);

            // Filtro por nome do produto
            if (!string.IsNullOrWhiteSpace(filtros.NomeProduto))
                query = query.Where(r => r.Lote.Produto.NomeProduto.Contains(filtros.NomeProduto));

            // Filtro por nome do doador
            if (!string.IsNullOrWhiteSpace(filtros.NomeDoador))
                query = query.Where(r => r.Lote.Produto.Doador.Usuario.Nome.Contains(filtros.NomeDoador));

            // Filtro por validade do lote
            if (filtros.ValidadeInicio.HasValue)
            {
                var inicio = filtros.ValidadeInicio.Value.Date;
                query = query.Where(r => r.Lote.DataValidade >= inicio);
            }
            if (filtros.ValidadeFim.HasValue)
            {
                var fim = filtros.ValidadeFim.Value.Date.AddDays(1); // inclui o dia final
                query = query.Where(r => r.Lote.DataValidade < fim);
            }

            // Filtro por data de reserva
            if (filtros.DataReservaInicio.HasValue)
            {
                var inicio = filtros.DataReservaInicio.Value.Date;
                query = query.Where(r => r.DataReserva >= inicio);
            }
            if (filtros.DataReservaFim.HasValue)
            {
                var fim = filtros.DataReservaFim.Value.Date.AddDays(1); // inclui o dia final
                query = query.Where(r => r.DataReserva < fim);
            }

            // Categorias disponíveis para o select do filtro
            ViewBag.CategoriasDisponiveis = await _context.ValoresLookup
                .Where(v => v.Tipo == TipoLookup.Categoria && v.Ativo)
                .OrderBy(v => v.Nome)
                .Select(v => v.Nome)
                .ToListAsync();

            // Carrega para memória antes de projetar (FotoProduto usa Convert.ToBase64String)
            var reservasDb = await query
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            // Busca avaliacoes ja feitas pelo beneficiario para reservas Retiradas
            var idsRetiradas = reservasDb
                .Where(r => r.Status == StatusReserva.Retirada)
                .Select(r => r.IdReserva)
                .ToList();

            Dictionary<int, int> avaliacoesPorReserva = new();
            if (idsRetiradas.Any())
            {
                avaliacoesPorReserva = await _context.Avaliacoes
                    .Where(a => a.IdAvaliador == usuarioId
                             && a.IdReserva != null
                             && idsRetiradas.Contains(a.IdReserva.Value))
                    .ToDictionaryAsync(a => a.IdReserva!.Value, a => a.Nota);
            }

            var reservas = reservasDb.Select(r => new MinhasReservasViewModel
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
                    : ObterFotoProdutoDataUrl(r.Lote.Produto.FotoProduto),
                IdUsuarioDoador = r.Lote.Produto.IdDoador,
                NomeDoador = r.Lote.Produto.Doador.Usuario.Nome,
                TelefoneDoador = r.Lote.Produto.Doador.Usuario.Telefone,
                EnderecoDoador = r.Lote.Produto.Doador.Usuario.Endereco == null ? null
                    : $"{r.Lote.Produto.Doador.Usuario.Endereco.Logradouro}, {r.Lote.Produto.Doador.Usuario.Endereco.Numero}"
                    + (string.IsNullOrWhiteSpace(r.Lote.Produto.Doador.Usuario.Endereco.Complemento) ? "" : $" - {r.Lote.Produto.Doador.Usuario.Endereco.Complemento}")
                    + $" — {r.Lote.Produto.Doador.Usuario.Endereco.Bairro}, {r.Lote.Produto.Doador.Usuario.Endereco.Cidade}/{r.Lote.Produto.Doador.Usuario.Endereco.Estado}",
                MotivoRejeicao = r.MotivoRejeicao,
                PodeAvaliar = r.Status == StatusReserva.Retirada,
                JaAvaliou   = r.Status == StatusReserva.Retirada
                              && avaliacoesPorReserva.ContainsKey(r.IdReserva),
                NotaAvaliacao = (r.Status == StatusReserva.Retirada
                                 && avaliacoesPorReserva.TryGetValue(r.IdReserva, out var nota))
                                ? nota : (int?)null
            }).ToList();

            var viewModel = new MinhasReservasPageViewModel
            {
                Filtros  = filtros,
                Reservas = reservas
            };

            return View(viewModel);
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
            var usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0)
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
    }
}
