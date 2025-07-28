using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.Email;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class CaixaController : Controller
    {
        private readonly ICaixaRepository _caixaRepository;
        private readonly IComandaRepository _comandaRepository;
        private readonly GerenciarEmail _gerenciarEmail;

        public CaixaController(ICaixaRepository caixaRepository, IComandaRepository comandaRepository, GerenciarEmail gerenciarEmail)
        {
            _caixaRepository = caixaRepository;
            _comandaRepository = comandaRepository;
            _gerenciarEmail = gerenciarEmail;
        }
        public IActionResult Index()
        {
            var caixaAberto = _caixaRepository.BuscarCaixaAbertoHoje();
            ViewBag.CaixaAberto = caixaAberto;

           //ViewBag.CaixaAberto = caixaAberto != null;
            return View();
        }

        [HttpPost]
        public IActionResult Abrir( Caixa caixa)
        {
            caixa.StatusEmail = "N";            

            _caixaRepository.AbrirCadastrar(caixa);
            TempData["Mensagem"] = "Caixa aberto com sucesso!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Fechar()
        {
            
            DateTime date = DateTime.Now;
            
            
            // Buscar caixa aberto de hoje
            var caixa = _caixaRepository.BuscarCaixaAbertoHoje();

            if (caixa == null)
            {
                TempData["MensagemErro"] = "Nenhum caixa aberto encontrado para hoje.";
                return RedirectToAction("Index");
            }

            // Atualiza dados do fechamento
            caixa.DataFechamento = DateTime.Now;
            caixa.Situacao = "F";
            caixa.StatusEmail = "S";

            _caixaRepository.FecharCaixaAntigos(caixa);
            // Salvar alteração
            _caixaRepository.FecharCaixa(caixa);

            // Buscar comandas fechadas do dia
            var comandas = _comandaRepository.BuscarComandasFechadasDoDia(date);

            // Enviar resumo por email
            _gerenciarEmail.EnviarResumoComandasDia(comandas, caixa);

            TempData["Mensagem"] = "Caixa fechado e e-mail enviado com sucesso.";
            return RedirectToAction("Index");
        }
        
    }
}
