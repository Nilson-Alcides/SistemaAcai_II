using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProdutoController : Controller
    {
        private readonly IProdutoSimplesRepository _produtoRepository;

        // Injetamos o repositório pelo construtor
        public ProdutoController(IProdutoSimplesRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Ação para listar todos os produtos
        public IActionResult Index()
        {
            var lista = _produtoRepository.ListarTodos();
            return View(lista);
        }

        // GET - Exibe o formulário para cadastrar um novo produto
        public IActionResult Cadastrar()
        {
            return View();
        }

        // POST - Recebe os dados do formulário e salva no banco
        [HttpPost]
        public IActionResult Cadastrar(ProdutoSimples produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            _produtoRepository.Cadastrar(produto);
            return RedirectToAction("Index");
        }

        // GET - Exibe formulário com dados preenchidos para edição
        public IActionResult Atualizar(int id)
        {
            var produto = _produtoRepository.ObterPorId(id);
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // POST - Atualiza os dados do produto
        [HttpPost]
        public IActionResult Atualizar(ProdutoSimples produto)
        {
            if (!ModelState.IsValid)
                return View(produto);

            _produtoRepository.Atualizar(produto);
            return RedirectToAction("Index");
        }
    }
}
