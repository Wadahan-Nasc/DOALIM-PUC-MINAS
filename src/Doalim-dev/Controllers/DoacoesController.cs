using Doalim_dev.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Doalim_dev.Controllers
{
    public class DoacoesController : Controller
    {
        public IActionResult Vitrine(VitrineFiltroViewModel filtros)
        {
            return RedirectToAction("Vitrine", "Produtos", filtros);
        }
    }
}
