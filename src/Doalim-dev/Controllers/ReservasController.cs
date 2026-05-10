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
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null || !produto.StatusProduto)
                return NotFound();

            var viewModel = new ReservaViewModel
            {
                IdProduto = produto.IdProduto,
                NomeProduto = produto.NomeProduto,
                MarcaProduto = produto.MarcaProduto,
                Categoria = produto.CategoriaProduto,
                UnidadeMedida = produto.UnidadeMedida,
                QuantidadeDisponivel = produto.Quantidade,
                QuantidadePessoaFisica = produto.QuantidadePessoaFisica,
                QuantidadePessoaJuridica = produto.QuantidadePessoaJuridica,
                DataValidade = produto.DataValidade,
                FotoProduto = produto.FotoProduto
            };

            return View(viewModel);
        }

        // POST: /Reservas/Confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(ReservaViewModel viewModel)
        {
            // 1. Lê o ID do usuário logado
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            // 2. Verifica se é um Beneficiario
            var beneficiario = await _context.Beneficiarios.FindAsync(usuarioId);
            if (beneficiario == null)
            {
                TempData["Erro"] = "Apenas beneficiários podem realizar reservas.";
                return RedirectToAction("Vitrine", "Produtos");
            }

            if (!ModelState.IsValid)
                return View("Reservar", viewModel);

            // 3. Busca e valida o produto
            var produto = await _context.Produtos.FindAsync(viewModel.IdProduto);
            if (produto == null || !produto.StatusProduto)
                return NotFound();

            // 4. Valida quantidade solicitada
            if (viewModel.QuantidadeReservada > produto.Quantidade)
            {
                ModelState.AddModelError("QuantidadeReservada",
                    $"Quantidade indisponível. Máximo permitido: {produto.Quantidade}.");
                return View("Reservar", viewModel);
            }

            // 5. Cria a reserva
            var reserva = new Reserva
            {
                IdProduto = viewModel.IdProduto,
                IdBeneficiario = usuarioId,
                QuantidadeReservada = viewModel.QuantidadeReservada,
                Status = StatusReserva.Pendente,
                DataReserva = DateTime.UtcNow
            };

            // 6. Desconta a quantidade do produto
            produto.Quantidade -= viewModel.QuantidadeReservada;
            if (produto.Quantidade == 0)
                produto.StatusProduto = false;

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
                    DataValidade = r.Produto.DataValidade,
                    FotoProduto = r.Produto.FotoProduto == null
                        ? null
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(r.Produto.FotoProduto)}",
                    NomeDoador = r.Produto.Doador.Usuario.Nome
                })
                .ToListAsync();

            return View(reservas);
        }
    }
}