using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;

namespace SistemaAcai_II.Areas.Colaborador.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class ClienteController : Controller
    {
        private IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public IActionResult Index(int? pagina, string pesquisa)
        {
            return View(_clienteRepository.ObterTodosClientes(pagina, pesquisa));
        }
        [HttpPost]
        public IActionResult Index(int draw, int start, int length, string search)
        {
            var clientes = _clienteRepository.ObterTodosClientes();
            return Json(new { data = clientes });
        }
        public IActionResult Cadastrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Cadastrar(Cliente cliente)
        {
            if(ModelState.IsValid){
                _clienteRepository.Cadastrar(cliente);
                return RedirectToAction("Painel", "Home");
            }
            return View();
        }
      
        //[ValidateHttpReferer]
        public IActionResult Ativar(int id)
        {
            _clienteRepository.Ativar(id);
            return RedirectToAction(nameof(Index));
        }       
      //  [ValidateHttpReferer]
        public IActionResult Desativar(int id)
        {
            _clienteRepository.Desativar(id);
            return RedirectToAction(nameof(Index));
        } 
        public IActionResult Detalhes(int id)
        {   
            return View(_clienteRepository.ObterCliente(id));
        }
        [HttpPost]
        public IActionResult Detalhes(Cliente cliente)
        {
            return View();
        }
    }
}

