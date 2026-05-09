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

            return View("~/Views/Produtos/Vitrine.cshtml", viewModel);
        }
    }
}
