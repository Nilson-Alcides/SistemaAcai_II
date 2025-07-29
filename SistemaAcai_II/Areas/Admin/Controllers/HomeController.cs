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
        [HttpGet]
        public JsonResult ObterDadosGrafico()
        {
            var comandas = _comandaRepository.ObterTodasComandasFechadas();

            var hoje = DateTime.Today;
            var amanha = hoje.AddDays(1);
            var inicioSemana = hoje.AddDays(-(int)(hoje.DayOfWeek == DayOfWeek.Sunday ? 6 : ((int)hoje.DayOfWeek - 1)));

            var dadosPorDia = comandas
                .Where(c => c.DataFechamento != null &&
                            c.DataFechamento.Value >= inicioSemana &&
                            c.DataFechamento.Value < amanha) // inclui o dia atual inteiro
                .GroupBy(c => (int)c.DataFechamento.Value.DayOfWeek)
                .Select(g => new
                {
                    Dia = g.Key,
                    Total = g.Sum(c => c.ValorTotal)
                })
                .ToList();

            decimal[] totais = new decimal[7]; // índice: 0=Dom, ..., 6=Sáb
            foreach (var item in dadosPorDia)
            {
                totais[item.Dia] = item.Total;
            }

            var labels = new[] { "Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb" };

            return Json(new { labels = labels, valores = totais });
        }



        //    [HttpGet]
        //    public JsonResult ObterDadosGrafico()
        //    {
        //        var comandas = _comandaRepository.ObterTodasComandasFechadas();

        //        // Somente comandas com DataFechamento válida
        //        var dadosPorDia = comandas
        //            .Where(c => c.DataFechamento != null)
        //            .GroupBy(c => (int)c.DataFechamento.Value.DayOfWeek)
        //            .Select(g => new
        //            {
        //                Dia = g.Key,
        //                Total = g.Sum(c => c.ValorTotal)
        //            })
        //            .ToList();

        //        // Cria um array com os 7 dias da semana: [Dom, Seg, Ter, Qua, Qui, Sex, Sáb]
        //        decimal[] totais = new decimal[7];
        //        foreach (var item in dadosPorDia)
        //        {
        //            totais[item.Dia] = item.Total;
        //        }

        //        return Json(totais);
        //    }
    }

}
