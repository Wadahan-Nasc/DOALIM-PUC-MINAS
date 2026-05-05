using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    public class DoacoesController : Controller
    {
        private readonly AppDbContext _context;

        public DoacoesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Doacoes
        public async Task<IActionResult> Vitrine()
        {
            var doacoes = await _context.Doacoes
                .Include(a => a.Doador)
                .ThenInclude(d => d.Usuario) // Inclui os dados do usuário do doador
                .Where(a => a.StatusProduto && a.DataValidade > DateTime.UtcNow && a.QuantidadeDisponivel > 0)
                .OrderByDescending(a => a.DataCadastro)
                .Select(a => new VitrineDoacoesViewModel
                {
                    IdProduto = a.IdProduto,
                    Nome = a.Nome,
                    Categoria = a.Categoria,
                    MarcaProduto = a.MarcaProduto,
                    TipoArmazenamento = a.TipoArmazenamento,
                    DataValidade = a.DataValidade,
                    QuantidadeDisponivel = a.QuantidadeDisponivel,
                    NomeDoador = a.Doador.Usuario.Nome,
                    FotoProduto = a.FotoProduto
                })
                .ToListAsync();
            return View(doacoes);
        }
    }
}
