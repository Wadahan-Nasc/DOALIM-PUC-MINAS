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

            return View(CriarReservaViewModel(produto));
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

            var produto = await _context.Produtos
                .Include(p => p.Lotes)
                .FirstOrDefaultAsync(p => p.IdProduto == viewModel.IdProduto);

            if (produto == null || !produto.StatusProduto)
                return NotFound();

            if (produto.IdDoador == usuarioId)
            {
                TempData["Erro"] = "Você não pode reservar a própria doação.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            var hoje = DateTime.Today;
            var quantidadeDisponivel = produto.Lotes
                .Where(l => l.StatusLote && l.DataValidade.Date >= hoje)
                .Sum(l => l.Quantidade);

            if (viewModel.QuantidadeReservada > quantidadeDisponivel)
            {
                ModelState.AddModelError("QuantidadeReservada",
                    $"Quantidade indisponível. Máximo permitido: {quantidadeDisponivel}.");
                return await RetornarViewReservaAsync(viewModel);
            }

            var reserva = new Reserva
            {
                IdProduto = viewModel.IdProduto,
                IdBeneficiario = usuarioId,
                QuantidadeReservada = viewModel.QuantidadeReservada,
                Status = StatusReserva.Pendente,
                DataReserva = DateTime.UtcNow
            };

            DeduzirQuantidadeLotes(produto, viewModel.QuantidadeReservada);

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
                .Include(r => r.Produto)
                    .ThenInclude(p => p.Doador)
                        .ThenInclude(d => d.Usuario)
                .Include(r => r.Produto.Lotes)   // necessário para obter a menor validade
                .Where(r => r.IdBeneficiario == usuarioId)
                .OrderByDescending(r => r.DataReserva)
                .Select(r => new MinhasReservasViewModel
                {
                    IdReserva = r.IdReserva,
                    DataReserva = r.DataReserva,
                    StatusReserva = r.Status.ToString(),
                    QuantidadeReservada = r.QuantidadeReservada,
                    NomeProduto = r.Produto.NomeProduto,
                    MarcaProduto = r.Produto.MarcaProduto,
                    Categoria = r.Produto.CategoriaProduto,
                    UnidadeMedida = r.Produto.UnidadeMedida,
                    // Exibe a data de validade mais próxima entre os lotes ainda ativos
                    DataValidade = r.Produto.Lotes
                        .Where(l => l.StatusLote && l.DataValidade.Date >= DateTime.Today && l.Quantidade > 0)
                        .OrderBy(l => l.DataValidade)
                        .Select(l => (DateTime?)l.DataValidade)
                        .FirstOrDefault(),
                    FotoProduto = r.Produto.FotoProduto == null
                        ? null
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(r.Produto.FotoProduto)}",
                    NomeDoador = r.Produto.Doador.Usuario.Nome
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
            preenchido.QuantidadeReservada = viewModel.QuantidadeReservada;
            return View("Reservar", preenchido);
        }

        private ReservaViewModel CriarReservaViewModel(Produto produto)
        {
            var hoje = DateTime.Today;

            var lotesAtivos = produto.Lotes
                .Where(l => l.StatusLote && l.DataValidade.Date >= hoje && l.Quantidade > 0)
                .OrderBy(l => l.DataValidade)
                .ToList();

            return new ReservaViewModel
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                MarcaProduto = produto.MarcaProduto,
                Categoria = produto.CategoriaProduto,
                UnidadeMedida = produto.UnidadeMedida,
                QuantidadeDisponivel = lotesAtivos.Sum(l => l.Quantidade),
                QuantidadePessoaFisica = produto.QuantidadePessoaFisica,
                QuantidadePessoaJuridica = produto.QuantidadePessoaJuridica,
                DataValidade = lotesAtivos.FirstOrDefault()?.DataValidade ?? DateTime.MinValue,
                FotoProduto = produto.FotoProduto
            };
        }

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
    }
}
