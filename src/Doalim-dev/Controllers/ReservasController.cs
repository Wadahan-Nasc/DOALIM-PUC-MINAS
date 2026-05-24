using System.Security.Claims;
using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: /Reservas/Reservar/5
        public async Task<IActionResult> Reservar(int id)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            if (!await UsuarioPodeReservarAsync(usuarioId))
            {
                TempData["Erro"] = "Apenas beneficiários aprovados podem realizar reservas.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            var produto = await _context.Produtos
                .Include(p => p.Lotes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null || !produto.StatusProduto)
                return NotFound();

            if (produto.IdDoador == usuarioId)
            {
                TempData["Erro"] = "Você não pode reservar a própria doação.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            var viewModel = CriarReservaViewModel(produto);
            if (viewModel == null)
            {
                TempData["Erro"] = "Não há lotes disponíveis para este produto.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            return View(viewModel);
        }

        // POST: /Reservas/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(ReservaViewModel viewModel)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            if (!await UsuarioPodeReservarAsync(usuarioId))
            {
                TempData["Erro"] = "Apenas beneficiários aprovados podem realizar reservas.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            if (!ModelState.IsValid)
                return await RetornarViewReservaAsync(viewModel);

            // Busca o lote mais urgente (com menor data de validade) - FIFO definido pelo sistema
            var lote = await _context.Lotes
                .Include(l => l.Produto)
                .Where(l => l.IdProduto == viewModel.IdProduto
                            && l.StatusLote
                            && l.DataValidade.Date >= DateTime.Today
                            && l.Quantidade > 0)
                .OrderBy(l => l.DataValidade)
                .FirstOrDefaultAsync();

            if (lote == null)
            {
                TempData["Erro"] = "Não há lotes disponíveis para este produto.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            if (lote.Produto.IdDoador == usuarioId)
            {
                TempData["Erro"] = "Você não pode reservar a própria doação.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            // Validade qtd contra lote mais urgente
            if (viewModel.QuantidadeReservada > lote.Quantidade)
            {
                ModelState.AddModelError("QuantidadeReservada",
                    $"Quantidade indisponível neste lote. Máximo permitido: {lote.Quantidade}.");
                return await RetornarViewReservaAsync(viewModel);
            }

            // Cria a reserva vinculada ao lote mais urgente
            var reserva = new Reserva
            {
                IdLote = lote.IdLote,
                IdBeneficiario = usuarioId,
                QuantidadeReservada = viewModel.QuantidadeReservada,
                Status = StatusReserva.Pendente,
                DataReserva = DateTime.UtcNow
            };

            // Deduz a quantidade do lote
            lote.Quantidade -= viewModel.QuantidadeReservada;

            // Desativa o lote zerado
            if (lote.Quantidade == 0)
            {
                lote.StatusLote = false;
            }

            // Busca se ainda há lotes ativos
            var aindaTemLotes = await _context.Lotes
                .AnyAsync(l => l.IdProduto == viewModel.IdProduto
                               && l.StatusLote
                               && l.DataValidade.Date >= DateTime.Today
                               && l.Quantidade > 0
                               && l.IdLote != lote.IdLote);

            // Se não houver mais lotes ativos, desativa o produto
            if (aindaTemLotes && lote.Quantidade == 0)
            {
                lote.StatusLote = false;
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Reserva realizada com sucesso! Seu código é #{reserva.IdReserva}.";
            return RedirectToAction("Vitrine", "Produtos");
        }

        // GET: /Reservas/MinhasReservas
        public async Task<IActionResult> MinhasReservas()
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
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
                    DataReserva = r.DataReserva,
                    StatusReserva = r.Status.ToString(),
                    QuantidadeReservada = r.QuantidadeReservada,
                    NumeroLote = r.Lote.NumeroLote,
                    DataValidadeLote = r.Lote.DataValidade,
                    NomeProduto = r.Lote.Produto.NomeProduto,
                    MarcaProduto = r.Lote.Produto.MarcaProduto,
                    Categoria = r.Lote.Produto.CategoriaProduto,
                    UnidadeMedida = r.Lote.Produto.UnidadeMedida,
                    FotoProduto = r.Lote.Produto.FotoProduto == null
                        ? null
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(r.Lote.Produto.FotoProduto)}",
                    NomeDoador = r.Lote.Produto.Doador.Usuario.Nome
                })
                .ToListAsync();

            return View(reservas);
        }

        // --- Métodos auxiliares ---

        private async Task<bool> UsuarioPodeReservarAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            if (usuario == null || usuario.StatusVerificacao != StatusVerificacao.Aprovado)
                return false;

            return await _context.Beneficiarios.AnyAsync(b => b.IdUsuario == usuarioId);
        }

        private async Task<IActionResult> RetornarViewReservaAsync(ReservaViewModel viewModel)
        {
            var produto = await _context.Produtos
                .Include(p => p.Lotes)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProduto == viewModel.IdProduto);

            if (produto == null)
                return NotFound();

            var preenchido = CriarReservaViewModel(produto);

            if (preenchido == null)
                return NotFound();

            preenchido.QuantidadeReservada = viewModel.QuantidadeReservada;
            return View("Reservar", preenchido);
        }

        private ReservaViewModel? CriarReservaViewModel(Produto produto)
        {
            var hoje = DateTime.Today;

            // Seleciona o lote mais urgente (com menor data de validade) que esteja ativo e com quantidade disponível - FIFO
            var loteMaisUrgente = produto.Lotes
                .Where(l => l.StatusLote && l.DataValidade.Date >= hoje && l.Quantidade > 0)
                .OrderBy(l => l.DataValidade)
                .FirstOrDefault();

            // Se não houver lotes disponíveis, retorna null para indicar que a reserva não pode ser feita
            if (loteMaisUrgente == null)
                return null;

            return new ReservaViewModel
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                MarcaProduto = produto.MarcaProduto,
                Categoria = produto.CategoriaProduto,
                UnidadeMedida = produto.UnidadeMedida,
                QuantidadePessoaFisica = produto.QuantidadePessoaFisica,
                QuantidadePessoaJuridica = produto.QuantidadePessoaJuridica,
                IdLoteMaisUrgente = loteMaisUrgente.IdLote,
                NumeroLote = loteMaisUrgente.NumeroLote,
                DataValidadeLote = loteMaisUrgente.DataValidade,
                QuantidadeDisponivelNoLote = loteMaisUrgente.Quantidade,
                FotoProduto = produto.FotoProduto
            };
        }

        /* APAGAR DEPOIS DE TESTAR - MANTIDO PARA REFERÊNCIA DE LÓGICA DE DEDUÇÃO DE QUANTIDADE DOS LOTES
        
        /// <summary>
        /// Deduz a quantidade reservada dos lotes ativos do produto,
        /// começando pelos mais próximos do vencimento (FIFO).
        /// Remove lotes que ficam zerados e atualiza StatusProduto se necessário.
        /// </summary>
        private void DeduzirQuantidadeLotes(Produto produto, int quantidade)
        {
            var hoje = DateTime.Today;
            var lotes = produto.Lotes
                .Where(l => l.StatusLote && l.DataValidade.Date >= hoje && l.Quantidade > 0)
                .OrderBy(l => l.DataValidade)
                .ToList();

            foreach (var lote in lotes)
            {
                if (quantidade <= 0)
                    break;

                if (lote.Quantidade <= quantidade)
                {
                    quantidade -= lote.Quantidade;
                    lote.Quantidade = 0;
                    lote.StatusLote = false;
                    _context.Lotes.Remove(lote);
                }
                else
                {
                    lote.Quantidade -= quantidade;
                    quantidade = 0;
                }
            }

            // Se não restarem lotes ativos, desativa o produto
            var aindaTemLotes = produto.Lotes
                .Any(l => l.StatusLote && l.DataValidade.Date >= hoje && l.Quantidade > 0);

            if (!aindaTemLotes)
                produto.StatusProduto = false;
        }
        */
    }
}
