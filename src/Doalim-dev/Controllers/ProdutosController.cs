using Doalim_dev.Models;
using Doalim_dev.Services;
using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Doalim_dev.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IReservaService _reservaService;

        public ProdutosController(AppDbContext context, IReservaService reservaService)
        {
            _context = context;
            _reservaService = reservaService;
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
        [Authorize]
        public IActionResult Create() => View();

        // CRIAR (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto, IFormFile arquivoFoto)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == usuarioId);
            if (!usuarioExiste)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Erro"] = "Sua sessão estava vinculada a um usuário que não existe mais. Faça login novamente.";
                return RedirectToAction("Login", "Auth");
            }

            var ehDoador = User.IsInRole(TipoUsuario.DoadorPF.ToString())
                || User.IsInRole(TipoUsuario.DoadorPJ.ToString());

            if (!ehDoador)
                ModelState.AddModelError(string.Empty, "Apenas usuários doadores podem cadastrar produtos.");

            produto.IdDoador = usuarioId;

            var doador = await _context.Doadores.FindAsync(usuarioId);
            if (doador == null && ehDoador)
            {
                doador = new Doador
                {
                    IdUsuario = usuarioId,
                    QtdAlimentosDoados = "0"
                };
                _context.Doadores.Add(doador);
            }

            ModelState.Remove(nameof(Produto.IdDoador));
            ModelState.Remove(nameof(Produto.Doador));

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
            var usuarioLogado = User.Identity?.IsAuthenticated == true;
            var usuarioBeneficiario = UsuarioEhBeneficiario();
            var usuarioAprovado = false;

            if (usuarioLogado)
            {
                var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(usuarioIdClaim, out var usuarioId))
                {
                    usuarioAprovado = await _context.Usuarios
                        .AsNoTracking()
                        .AnyAsync(u => u.IdUsuario == usuarioId && u.StatusVerificacao == StatusVerificacao.Aprovado);
                }
            }

            ViewBag.UsuarioLogado = usuarioLogado;
            ViewBag.UsuarioBeneficiario = usuarioBeneficiario;
            ViewBag.UsuarioAprovado = usuarioAprovado;
            ViewBag.PodeReservar = usuarioLogado && usuarioBeneficiario && usuarioAprovado;

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
                    FotoProduto = ObterFotoProdutoDataUrl(a.FotoProduto),
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int id)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            if (usuario == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["Erro"] = "Sua sessão estava vinculada a um usuário que não existe mais. Faça login novamente.";
                return RedirectToAction("Login", "Auth");
            }

            if (usuario.StatusVerificacao != StatusVerificacao.Aprovado)
            {
                TempData["Erro"] = "Sua conta precisa ser aprovada pelo administrador antes de reservar alimentos.";
                return RedirectToAction(nameof(Vitrine));
            }

            if (!UsuarioEhBeneficiario())
            {
                TempData["Erro"] = "Apenas usuários beneficiários podem reservar alimentos.";
                return RedirectToAction(nameof(Vitrine));
            }

            var resultado = await _reservaService.ReservarDoacaoAsync(id, usuarioId);

            TempData[resultado.Sucesso ? "Sucesso" : "Erro"] = resultado.Mensagem;

            if (resultado.Sucesso)
                return RedirectToAction(nameof(MinhasReservas));

            return RedirectToAction(nameof(Vitrine));
        }

        [Authorize]
        public async Task<IActionResult> MinhasReservas()
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            if (!UsuarioEhBeneficiario())
            {
                TempData["Erro"] = "Apenas usuários beneficiários têm reservas.";
                return RedirectToAction(nameof(Vitrine));
            }

            var reservas = await _context.Produtos
                .Include(p => p.Doador)
                    .ThenInclude(d => d.Usuario)
                .Where(p => p.IdBeneficiario == usuarioId)
                .OrderByDescending(p => p.DataReserva)
                .ToListAsync();

            return View(reservas);
        }

        private bool UsuarioEhBeneficiario()
        {
            return User.IsInRole(TipoUsuario.BeneficiarioPF.ToString())
                || User.IsInRole(TipoUsuario.BeneficiarioPJ.ToString());
        }

        private static string ObterFotoProdutoDataUrl(byte[]? fotoProduto)
        {
            if (fotoProduto == null || fotoProduto.Length == 0)
                return string.Empty;

            var mimeType = "image/jpeg";

            if (fotoProduto.Length >= 8
                && fotoProduto[0] == 0x89
                && fotoProduto[1] == 0x50
                && fotoProduto[2] == 0x4E
                && fotoProduto[3] == 0x47)
            {
                mimeType = "image/png";
            }
            else if (fotoProduto.Length >= 4
                && fotoProduto[0] == 0x3C
                && (fotoProduto[1] == 0x73 || fotoProduto[1] == 0x53 || fotoProduto[1] == 0x3F))
            {
                mimeType = "image/svg+xml";
            }

            return $"data:{mimeType};base64,{Convert.ToBase64String(fotoProduto)}";
        }
    }
}
