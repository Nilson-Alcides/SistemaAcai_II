using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Models.Constants;
using SistemaAcai_II.Repositories.Contract;
using SistemaAcai_II.Repositories.Contracts;
using SistemaAcai_II.Repository;
using Microsoft.AspNetCore.Mvc;

namespace SistemaAcai_II.Areas.Colaborador.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao(ColaboradorTipoConstant.Gerente)]
    public class ColaboradorController : Controller
    {
        private IColaboradorRepository _colaboradorRepository;

        public ColaboradorController(IColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }
        //[ValidateHttpReferer]
        public IActionResult Index(int? pagina, string pesquisa)
        {
            return View(_colaboradorRepository.ObterTodosColaboradores(pagina, pesquisa));
        }
        [HttpGet]
        //[ValidateHttpReferer]
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastrar([FromForm] Models.Colaborador colaborador)
         {
            colaborador.Tipo = ColaboradorTipoConstant.Comum;
            if (ModelState.IsValid) {
                _colaboradorRepository.Cadastrar(colaborador);
                TempData["MSG_S"] = "Registro salvo com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpGet]
        //[ValidateHttpReferer]
        public IActionResult Atualizar(int id)
        {
            Models.Colaborador colaborador = _colaboradorRepository.ObterColaborador(id);
            return View(colaborador);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.Colaborador colaborador)
        {           
            if (ModelState.IsValid)
            {
                _colaboradorRepository.Atualizar(colaborador);

                TempData["MSG_S"] = "Registro salvo com sucesso!";

                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        //[ValidateHttpReferer]
        public IActionResult Excluir(int id)
        {
            _colaboradorRepository.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
        //[ValidateHttpReferer]
        public IActionResult Ativar(int id)
        {
            _colaboradorRepository.Ativar(id);
            return RedirectToAction(nameof(Index));
        }
        //  [ValidateHttpReferer]
        public IActionResult Desativar(int id)
        {
            _colaboradorRepository.Desativar(id);
            return RedirectToAction(nameof(Index));
        }
        //  [ValidateHttpReferer]
        public IActionResult Promover(int id)
        {
            _colaboradorRepository.Promover(id);
            return RedirectToAction(nameof(Index));
        }
        //  [ValidateHttpReferer]
        public IActionResult Rebaixar(int id)
        {
            _colaboradorRepository.Rebaixar(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
