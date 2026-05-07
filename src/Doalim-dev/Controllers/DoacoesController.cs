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
            var query = _context.Doacoes
                .Include(a => a.Doador)
                    .ThenInclude(d => d.Usuario) // Inclui os dados do usuário do doador
                .Where(a => a.StatusProduto && a.DataValidade > DateTime.UtcNow && a.QuantidadeDisponivel > 0)
                .AsQueryable();

            // Filtro de quantidade mínima
            if (filtros.QuantidadeMinima.HasValue)
                query = query.Where(a => a.QuantidadeDisponivel >= filtros.QuantidadeMinima.Value);

            //Filtro de busca por nome
            if (!string.IsNullOrWhiteSpace(filtros.NomeBusca))
                query = query.Where(a => a.Nome.Contains(filtros.NomeBusca));

            //Filtro de ordenação por validade
            query = filtros.OrdemValidade == "desc"
                ? query.OrderByDescending(a => a.DataValidade)
                : query.OrderBy(a => a.DataValidade);

            var produtos = await query
                .Select(a => new VitrineDoacoesViewModel
                {
                    IdProduto = a.IdProduto,
                    Nome = a.Nome,
                    DataValidade = a.DataValidade,
                    Categoria = a.Categoria ?? "",
                    MarcaProduto = a.MarcaProduto ?? "",
                    TipoArmazenamento = a.TipoArmazenamento ?? "",
                    FotoProduto = a.FotoProduto ?? "",
                    QuantidadeDisponivel = a.QuantidadeDisponivel,
                    NomeDoador = a.Doador.Usuario.Nome
                })
                .ToListAsync();

            var viewModel = new VitrineCompletaViewModel
            {
                Filtros = filtros,
                Produtos = produtos
            };

            return View(viewModel);
        }
    }
}
