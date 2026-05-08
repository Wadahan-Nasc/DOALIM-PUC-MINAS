using Doalim_dev.Models;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        // VITRINE
        public async Task<IActionResult> Vitrine(VitrineFiltroViewModel filtros)
        {
            var query = _context.Produtos
                .Include(a => a.Doador)
                    .ThenInclude(d => d.Usuario) // Inclui os dados do usuário do doador
                .Where(a => a.StatusProduto && a.DataValidade > DateTime.UtcNow && a.Quantidade > 0)
                .AsQueryable();

            // Filtro de quantidade mínima
            if (filtros.QuantidadeMinima.HasValue)
                query = query.Where(a => a.Quantidade >= filtros.QuantidadeMinima.Value);

            //Filtro de busca por nome
            if (!string.IsNullOrWhiteSpace(filtros.NomeBusca))
                query = query.Where(a => a.NomeProduto.Contains(filtros.NomeBusca));

            //Filtro de ordenação por validade
            query = filtros.OrdemValidade == "desc"
                ? query.OrderByDescending(a => a.DataValidade)
                : query.OrderBy(a => a.DataValidade);

            var produtos = await query.ToListAsync();

            var produtosViewModel = produtos
                .Select(a => new VitrineDoacoesViewModel
                {
                    IdProduto = a.IdProduto,
                    Nome = a.NomeProduto,
                    DataValidade = a.DataValidade,
                    Categoria = a.CategoriaProduto ?? "",
                    MarcaProduto = a.MarcaProduto ?? "",
                    TipoArmazenamento = a.TipoArmazenamento ?? "",
                    FotoProduto = a.FotoProduto == null || a.FotoProduto.Length == 0
                        ? ""
                        : $"data:image/jpeg;base64,{Convert.ToBase64String(a.FotoProduto)}",
                    QuantidadeDisponivel = a.Quantidade,
                    NomeDoador = a.Doador.Usuario.Nome
                })
                .ToList();

            var viewModel = new VitrineCompletaViewModel
            {
                Filtros = filtros,
                Produtos = produtosViewModel
            };

            return View(viewModel);
        }
    }
}
