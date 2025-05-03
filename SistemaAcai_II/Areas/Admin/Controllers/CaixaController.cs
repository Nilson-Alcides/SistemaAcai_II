using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.Filtro;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class CaixaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
