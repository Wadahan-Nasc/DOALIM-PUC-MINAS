using Doalim_dev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doalim_dev.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {

        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        private int ObterIdUsuarioLogado()
        {

            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (int.TryParse(claim, out int id)) return id;
            return 0;
        }

        public async Task<IActionResult> Index(string busca, string categoria, string marca, string numeroLote, DateTime? dataInicio, DateTime? dataFim, bool apenasAlertas, string statusFiltro = "ativos")
        {
            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            var ehDoador = User.IsInRole(TipoUsuario.DoadorPF.ToString()) || User.IsInRole(TipoUsuario.DoadorPJ.ToString());
            if (!ehDoador)
            {
                TempData["ErroSeguranca"] = "Acesso negado: Apenas Doadores possuem um painel de gerenciamento.";
                return RedirectToAction("Index", "Home");
            }

            // Auto-Inativação
            var produtosDoDoador = await _context.Produtos.Include(p => p.Lotes).Where(p => p.IdDoador == usuarioId).ToListAsync();
            bool precisaSalvarDB = false;

            foreach (var p in produtosDoDoador)
            {
                bool produtoTemLoteValidoEAtivo = false;

                if (p.Lotes != null && p.Lotes.Any())
                {
                    foreach (var lote in p.Lotes)
                    {
                        if (lote.DataValidade.Date < DateTime.Today && lote.StatusLote == true)
                        {
                            lote.StatusLote = false;
                            precisaSalvarDB = true;
                        }

                        if (lote.StatusLote == true && lote.DataValidade.Date >= DateTime.Today)
                        {
                            produtoTemLoteValidoEAtivo = true;
                        }
                    }

                    if (!produtoTemLoteValidoEAtivo && p.StatusProduto == true)
                    {
                        p.StatusProduto = false;
                        precisaSalvarDB = true;
                    }
                }
            }

            if (precisaSalvarDB) await _context.SaveChangesAsync();

            var query = _context.Produtos.Include(p => p.Lotes).Where(p => p.IdDoador == usuarioId).AsQueryable();

            // Filtros
            if (statusFiltro == "ativos") query = query.Where(p => p.StatusProduto == true);
            else if (statusFiltro == "inativos") query = query.Where(p => p.StatusProduto == false);

            if (!string.IsNullOrWhiteSpace(busca)) query = query.Where(p => p.NomeProduto.Contains(busca));
            if (!string.IsNullOrWhiteSpace(categoria)) query = query.Where(p => p.CategoriaProduto.Contains(categoria));
            if (!string.IsNullOrWhiteSpace(marca)) query = query.Where(p => p.MarcaProduto.Contains(marca));

            if (!string.IsNullOrWhiteSpace(numeroLote)) query = query.Where(p => p.Lotes.Any(l => l.NumeroLote.Contains(numeroLote)));
            if (dataInicio.HasValue) query = query.Where(p => p.Lotes.Any(l => l.DataValidade >= dataInicio.Value));
            if (dataFim.HasValue) query = query.Where(p => p.Lotes.Any(l => l.DataValidade <= dataFim.Value));

            if (apenasAlertas)
            {
                var limiteAlerta = DateTime.Today.AddDays(1);
                query = query.Where(p => p.Lotes.Any(l => l.StatusLote == true && l.DataValidade.Date <= limiteAlerta));
            }

            // Ordenação: ativos para cima e inativos para baixo e depois organiza em ordem alfabética
            query = query.OrderByDescending(p => p.StatusProduto).ThenBy(p => p.NomeProduto);

            var produtos = await query.ToListAsync();

            ViewBag.Busca = busca;
            ViewBag.Categoria = categoria;
            ViewBag.Marca = marca;
            ViewBag.NumeroLote = numeroLote;
            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
            ViewBag.ApenasAlertas = apenasAlertas;
            ViewBag.StatusFiltro = statusFiltro;

            return View(produtos);
        }


        // GET
        public IActionResult Create()
        {
            var ehDoador = User.IsInRole(TipoUsuario.DoadorPF.ToString()) || User.IsInRole(TipoUsuario.DoadorPJ.ToString());
            if (!ehDoador)
            {
                TempData["ErroSeguranca"] = "Acesso negado: Apenas Doadores podem cadastrar produtos.";
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile arquivoFoto, string[] NumeroLote, DateTime[] DataValidadeLote, int[] QuantidadeLote, bool[] StatusLote)
        {

            int usuarioId = ObterIdUsuarioLogado();
            if (usuarioId == 0) return RedirectToAction("Login", "Auth");

            produto.IdDoador = usuarioId;

            if (!string.IsNullOrWhiteSpace(produto.CodigoBarras))
            {
                bool codigoJaExiste = await _context.Produtos.AnyAsync(p => p.IdDoador == usuarioId && p.CodigoBarras == produto.CodigoBarras);
                if (codigoJaExiste)
                {
                    ModelState.AddModelError("CodigoBarras", "Você já possui um produto com este Código de Barras no seu estoque.");
                }
            }

            ModelState.Remove(nameof(Produto.IdDoador));
            ModelState.Remove(nameof(Produto.Doador));

            // Imagem byte[] = varbinary(MAX)
            if (arquivoFoto != null && arquivoFoto.Length > 0)
            {
                using var ms = new MemoryStream();
                await arquivoFoto.CopyToAsync(ms); 
                produto.FotoProduto = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                bool produtoTemLoteValido = false;

                _context.Add(produto);
                await _context.SaveChangesAsync();

                if (NumeroLote != null && NumeroLote.Length > 0)
                {
                    for (int i = 0; i < NumeroLote.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(NumeroLote[i]))
                        {
                            bool statusAtualLote = (StatusLote != null && StatusLote.Length > i) ? StatusLote[i] : true;

                            if (DataValidadeLote[i].Date < DateTime.Today) statusAtualLote = false;

                            if (statusAtualLote) produtoTemLoteValido = true;

                            var lote = new Lote
                            {
                                NumeroLote = NumeroLote[i],
                                DataValidade = DataValidadeLote[i],
                                Quantidade = QuantidadeLote[i],
                                StatusLote = statusAtualLote,
                                IdProduto = produto.IdProduto
                            };
                            _context.Add(lote);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // Regra de négocio
                if (!produtoTemLoteValido && produto.StatusProduto)
                {
                    produto.StatusProduto = false;
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }


        // Redirecionamento feito
        public IActionResult Details(int? id) => RedirectToAction(nameof(Edit), new { id = id });
        public IActionResult Delete(int? id) => RedirectToAction(nameof(Edit), new { id = id });

        // GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            int usuarioId = ObterIdUsuarioLogado();

            var produto = await _context.Produtos.Include(p => p.Lotes).FirstOrDefaultAsync(p => p.IdProduto == id);
            if (produto == null) return NotFound();

            if (produto.IdDoador != usuarioId)
            {
                TempData["ErroSeguranca"] = "Acesso Negado: Este produto não pertence à sua conta.";
                return RedirectToAction(nameof(Index));
            }

            return View(produto);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto, IFormFile arquivoFoto,
            int[] IdLote, string[] NumeroLote, DateTime[] DataValidade, int[] Quantidade, bool[] StatusLote, int[] LotesExcluidos)
        {
            if (id != produto.IdProduto) return NotFound();

            int usuarioId = ObterIdUsuarioLogado();

            var produtoOriginal = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProduto == id);
            if (produtoOriginal == null || produtoOriginal.IdDoador != usuarioId)
            {
                TempData["ErroSeguranca"] = "Tentativa de Fraude identificada. Operação cancelada.";
                return RedirectToAction(nameof(Index));
            }

            produto.IdDoador = usuarioId;

            ModelState.Remove(nameof(Produto.Doador));
            ModelState.Remove(nameof(Produto.IdDoador));


            if (!string.IsNullOrWhiteSpace(produto.CodigoBarras))
            {
                bool codigoJaExiste = await _context.Produtos.AnyAsync(p => p.IdDoador == usuarioId && p.CodigoBarras == produto.CodigoBarras && p.IdProduto != id);
                if (codigoJaExiste) ModelState.AddModelError("CodigoBarras", "Você já possui OUTRO produto usando este mesmo Código.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (arquivoFoto != null && arquivoFoto.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await arquivoFoto.CopyToAsync(ms);
                        produto.FotoProduto = ms.ToArray();
                    }
                    else
                    {
                        produto.FotoProduto = produtoOriginal.FotoProduto;
                    }

                    _context.Update(produto);
                    await _context.SaveChangesAsync();


                    if (LotesExcluidos != null && LotesExcluidos.Length > 0)
                    {
                        foreach (var loteId in LotesExcluidos)
                        {
                            var lRemover = await _context.Lotes.FindAsync(loteId);
                            if (lRemover != null) _context.Lotes.Remove(lRemover);
                        }
                    }

                    bool produtoTemLoteValido = false;


                    if (NumeroLote != null)
                    {
                        for (int i = 0; i < NumeroLote.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(NumeroLote[i])) continue;

                            bool statusAtualLote = (StatusLote != null && StatusLote.Length > i) ? StatusLote[i] : true;
                            if (DataValidade[i].Date < DateTime.Today) statusAtualLote = false;

                            if (statusAtualLote) produtoTemLoteValido = true;

                            if (IdLote != null && i < IdLote.Length && IdLote[i] > 0)
                            {
                                var loteExist = await _context.Lotes.FindAsync(IdLote[i]);
                                if (loteExist != null)
                                {
                                    loteExist.NumeroLote = NumeroLote[i];
                                    loteExist.DataValidade = DataValidade[i];
                                    loteExist.Quantidade = Quantidade[i];
                                    loteExist.StatusLote = statusAtualLote;
                                    _context.Update(loteExist);
                                }
                            }

                            else
                            {
                                var novoLote = new Lote
                                {
                                    NumeroLote = NumeroLote[i],
                                    DataValidade = DataValidade[i],
                                    Quantidade = Quantidade[i],
                                    StatusLote = statusAtualLote,
                                    IdProduto = produto.IdProduto
                                };
                                _context.Add(novoLote);
                            }
                        }
                    }

                    if (!produtoTemLoteValido && produto.StatusProduto)
                    {
                        produto.StatusProduto = false;
                        _context.Update(produto);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.IdProduto)) return NotFound();
                    throw;
                }
            }

            produto.Lotes = await _context.Lotes.Where(l => l.IdProduto == id).ToListAsync();
            return View(produto);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.Include(p => p.Lotes).FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto != null && produto.IdDoador == ObterIdUsuarioLogado())
            {

                if (produto.Lotes != null && produto.Lotes.Any()) _context.Lotes.RemoveRange(produto.Lotes);

                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id) => _context.Produtos.Any(e => e.IdProduto == id);
    }
}