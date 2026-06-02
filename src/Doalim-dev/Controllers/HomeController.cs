using System.Diagnostics;
using Doalim_dev.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var agora = DateTime.UtcNow;

            // DbContext não é thread-safe: queries executadas sequencialmente
            ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();

            ViewBag.ProdutosDisponiveis = await _context.Produtos
                .CountAsync(p => p.StatusProduto
                    && p.Lotes.Any(l => l.DataValidade > agora && l.Quantidade > 0));

            ViewBag.AlimentosDisponiveis = await _context.Produtos
                .Where(p => p.StatusProduto)
                .SelectMany(p => p.Lotes)
                .Where(l => l.DataValidade > agora && l.Quantidade > 0)
                .SumAsync(l => (int?)l.Quantidade) ?? 0;

            ViewBag.DoacoesReservadas = await _context.Reservas.CountAsync();

            ViewBag.TotalDoadores = await _context.Usuarios
                .CountAsync(u => u.TipoUsuario == TipoUsuario.DoadorPF
                              || u.TipoUsuario == TipoUsuario.DoadorPJ);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}