using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repositories.Contract;
using SistemaAcai_II.Repositories.Contracts;
using SistemaAcai_II.Repository;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class FiliaisController : Controller
    {
        private IFiliaisRepository _filiaisRepository;
        public FiliaisController(IFiliaisRepository filiaisRepository)
        {
            _filiaisRepository = filiaisRepository;
        }

        public IActionResult Index(int? pagina, string pesquisa)
        {
            return View(_filiaisRepository.ObterTodosFiliais(pagina, pesquisa));
        }
        [HttpPost]
        public IActionResult Index(int draw, int start, int length, string search)
        {
            var filiais = _filiaisRepository.ObterTodosFiliais();
            return Json(new { data = filiais });
        }
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastrar(Filiais filiais)
        {
            if (ModelState.IsValid)
            {
                _filiaisRepository.Cadastrar(filiais);
                return RedirectToAction("Painel", "Home");
            }
            return View();
        }
        //[ValidateHttpReferer]
        public IActionResult Ativar(int id)
        {
            _filiaisRepository.Ativar(id);
            return RedirectToAction(nameof(Index));
        }
        //  [ValidateHttpReferer]
        public IActionResult Desativar(int id)
        {
            _filiaisRepository.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
        //[ValidateHttpReferer]
        public IActionResult Atualizar(int id)
        {
            Models.Filiais filiais = _filiaisRepository.ObterFiliais(id);
            return View(filiais);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Filiais filiais)
        {
            
                _filiaisRepository.Atualizar(filiais);

                TempData["MSG_S"] = "Registro salvo com sucesso!";

                return RedirectToAction(nameof(Index));
        }
        public IActionResult Detalhes(int id)
        {
            Models.Filiais filiais = _filiaisRepository.ObterFiliaisDetalhes(id);
            return View(filiais);
        }
        [HttpPost]
        public IActionResult Detalhes(Filiais filiais)
        {
            return View();
        }

    }
}
