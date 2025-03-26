using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repositories.Contract;
using SistemaAcai_II.Repositories.Contracts;
using SistemaAcai_II.Repository;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class PagamentoController : Controller
    {
        private IFormasPagamentoRepository _formasPagamentoRepository;
        public PagamentoController(IFormasPagamentoRepository formasPagamentoRepository)
        {
            _formasPagamentoRepository = formasPagamentoRepository;
        }
        public IActionResult Index(int? pagina, string pesquisa)
        {
            return View(_formasPagamentoRepository.ObterTodasFormasPagamentos(pagina,pesquisa));
        }
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastrar(FormasPagamento formasPagamento)
        {
            if (ModelState.IsValid)
            {
                _formasPagamentoRepository.Cadastrar(formasPagamento);
                return RedirectToAction("Painel", "Home");
            }
            return View();
        }
        //[ValidateHttpReferer]
        public IActionResult Atualizar(int id)
        {
            Models.FormasPagamento formasPagamento = _formasPagamentoRepository.ObterFormasPagamentoPorId(id);
            return View(formasPagamento);
        }

        [HttpPost]
        public IActionResult Atualizar([FromForm] Models.FormasPagamento formasPagamento)
        {
            _formasPagamentoRepository.Atualizar(formasPagamento);
            return RedirectToAction(nameof(Index));
        }
        //    [ValidateHttpReferer]
        public IActionResult Excluir(int id)
        {
            _formasPagamentoRepository.Excluir(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
