using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Doalim_dev.Models;

namespace Doalim_dev.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAGEM
        public async Task<IActionResult> Index()
        {
            return View(await _context.Produtos.ToListAsync());
        }

        // DETALHES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var produto = await _context.Produtos.FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // CRIAR (GET)
        public IActionResult Create() => View();

        // CRIAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile arquivoFoto)
        {
            if (arquivoFoto != null && arquivoFoto.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await arquivoFoto.CopyToAsync(ms);
                    produto.FotoProduto = ms.ToArray();
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // EDITAR (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto, IFormFile arquivoFoto)
        {
            if (id != produto.IdProduto) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (arquivoFoto != null && arquivoFoto.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await arquivoFoto.CopyToAsync(ms);
                            produto.FotoProduto = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Se não enviou foto nova, busca a foto antiga para não apagar
                        var original = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProduto == id);
                        produto.FotoProduto = original?.FotoProduto;
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Produtos.Any(e => e.IdProduto == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // DELETAR (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var produto = await _context.Produtos.FirstOrDefaultAsync(m => m.IdProduto == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // DELETAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}