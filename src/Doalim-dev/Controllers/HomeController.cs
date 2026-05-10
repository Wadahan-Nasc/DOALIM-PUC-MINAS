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
            ViewBag.TotalUsuarios = await _context.Usuarios.CountAsync();
            ViewBag.DocumentosPendentes = await _context.Usuarios.CountAsync(u => u.StatusVerificacao != StatusVerificacao.Aprovado);
            ViewBag.ProdutosDisponiveis = await _context.Produtos.CountAsync(p => p.StatusProduto);
            ViewBag.TotalAdministradores = await _context.Usuarios.CountAsync(u => u.TipoUsuario == TipoUsuario.Admin);

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
