using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Libraries.Login;
using SistemaAcai_II.Repositories.Contracts;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Areas.Colaborador.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        private IColaboradorRepository _repositoryColaborador;
        private LoginColaborador _loginColaborador;
        private readonly IComandaRepository _comandaRepository;

        public HomeController(IColaboradorRepository repositoryColaborador, LoginColaborador loginColaborador,
            IComandaRepository comandaRepository)
        {
            _repositoryColaborador = repositoryColaborador;
            _loginColaborador = loginColaborador;
            _comandaRepository = comandaRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index([FromForm] Models.Colaborador colaborador)
        {
            Models.Colaborador colaboradorDB = _repositoryColaborador.Login(colaborador.Email, colaborador.Senha);

            if (colaboradorDB.Email != null && colaboradorDB.Senha != null)
            {
                _loginColaborador.Login(colaboradorDB);

                return new RedirectResult(Url.Action(nameof(Painel)));
            }
            else
            {
                ViewData["MSG_E"] = "Usuário não encontrado, verifique o e-mail e senha digitado!";
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        // [ValidateHttpReferer]
        public IActionResult Login([FromForm] Models.Colaborador colaborador)
        {
            Models.Colaborador colaboradorDB = _repositoryColaborador.Login(colaborador.Email, colaborador.Senha);

            if (colaboradorDB.Email != null && colaboradorDB.Senha != null)
            {
                _loginColaborador.Login(colaboradorDB);

                return new RedirectResult(Url.Action(nameof(Painel)));
            }
            else
            {
                ViewData["MSG_E"] = "Usuário não encontrado, verifique o e-mail e senha digitado!";
                return View();
            }
        }
        [ColaboradorAutorizacao]
        public IActionResult Painel()
        {
           return View(_comandaRepository.ObterTodasComandasFechadas());
        }
        [ColaboradorAutorizacao]
        //  [ValidateHttpReferer]
        public IActionResult Logout()
        {
            _loginColaborador.Logout();
            return RedirectToAction("Login", "Home");
        }
    }

}
