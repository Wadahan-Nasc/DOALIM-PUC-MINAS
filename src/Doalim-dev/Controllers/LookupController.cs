using Doalim_dev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LookupController : Controller
    {
        private readonly AppDbContext _context;

        public LookupController(AppDbContext context)
        {
            _context = context;
        }

        // -------------------------------------------------------------------------
        // GET: /Lookup
        // Exibe todos os valores de domínio agrupados por tipo.
        // -------------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var valores = await _context.ValoresLookup
                .OrderBy(v => v.Tipo)
                .ThenBy(v => v.Nome)
                .ToListAsync();

            return View(valores);
        }

        // -------------------------------------------------------------------------
        // POST: /Lookup/Adicionar
        // Adiciona um novo valor a um tipo de domínio.
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adicionar(TipoLookup tipo, string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                TempData["Erro"] = "Informe o nome do valor.";
                return RedirectToAction(nameof(Index));
            }

            var nomeNormalizado = nome.Trim();

            var jaExiste = await _context.ValoresLookup
                .AnyAsync(v => v.Tipo == tipo && v.Nome == nomeNormalizado);

            if (jaExiste)
            {
                TempData["Erro"] = $"O valor \"{nomeNormalizado}\" já existe para {DescricaoTipo(tipo)}.";
                return RedirectToAction(nameof(Index));
            }

            _context.ValoresLookup.Add(new ValorLookup
            {
                Tipo = tipo,
                Nome = nomeNormalizado,
                Ativo = true
            });

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Valor \"{nomeNormalizado}\" adicionado em {DescricaoTipo(tipo)}.";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------------
        // POST: /Lookup/ToggleAtivo
        // Ativa ou desativa um valor de domínio existente.
        // Valores de seed (IdValor <= 16) podem ser desativados mas não excluídos.
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAtivo(int id)
        {
            var valor = await _context.ValoresLookup.FindAsync(id);
            if (valor == null)
                return NotFound();

            valor.Ativo = !valor.Ativo;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"\"{valor.Nome}\" {(valor.Ativo ? "ativado" : "desativado")} com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------------
        // POST: /Lookup/Excluir
        // Remove permanentemente um valor adicionado pelo admin (não pertence ao seed).
        // -------------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var valor = await _context.ValoresLookup.FindAsync(id);
            if (valor == null)
                return NotFound();

            // Protege os valores que fazem parte do seed inicial do sistema
            if (valor.EhValorPadrao)
            {
                TempData["Erro"] = "Valores padrão do sistema não podem ser excluídos. Use o botão de desativar.";
                return RedirectToAction(nameof(Index));
            }

            _context.ValoresLookup.Remove(valor);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"\"{valor.Nome}\" excluído com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private static string DescricaoTipo(TipoLookup tipo) => tipo switch
        {
            TipoLookup.Categoria         => "Categoria",
            TipoLookup.TipoArmazenamento => "Tipo de Armazenamento",
            TipoLookup.UnidadeMedida     => "Unidade de Medida",
            _                            => tipo.ToString()
        };
    }
}
