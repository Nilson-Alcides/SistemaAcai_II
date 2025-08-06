using SistemaAcai_II.Libraries.PedidoCompra;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SistemaAcai_II.Libraries.Filtro;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAcai_II.Libraries.Login;
using System.Globalization;

namespace SistemaAcai_II.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    [TypeFilter(typeof(CaixaAutorizacaoAttribute))]
    public class ComandaController : Controller
    {
        private readonly IComandaRepository _comandaRepository;
        private readonly CookiePedidoCompra _cookiePedidoCompra;
        private readonly IProdutoSimplesRepository _produtoRepository;
        private readonly IItensComandaRepository _itensComandaRepository;
        private readonly IFormasPagamentoRepository _formasPagamentoRepository;
        private LoginColaborador _loginColaborador;
       

        public ComandaController(IComandaRepository comandaRepository, CookiePedidoCompra cookiePedidoCompra,
            IProdutoSimplesRepository produtoRepository, LoginColaborador loginColaborador, IItensComandaRepository itensComandaRepository, IFormasPagamentoRepository formasPagamentoRepository)
        {
            _comandaRepository = comandaRepository;
            _cookiePedidoCompra = cookiePedidoCompra;
            _produtoRepository = produtoRepository;
            _loginColaborador = loginColaborador;
            _itensComandaRepository = itensComandaRepository;
            _formasPagamentoRepository = formasPagamentoRepository; 
        }


        public IActionResult Vendas(string termo)
        {
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");

            int quantidadeDigitada = 1;
            string termoBusca = termo;

            // Verifica se o termo está no formato "quantidade*código"
            if (!string.IsNullOrWhiteSpace(termo) && termo.Contains("*"))
            {
                var partes = termo.Split('*');
                if (partes.Length == 2)
                {
                    // Agora a primeira parte é a quantidade e a segunda é o código
                    if (int.TryParse(partes[0], out int qtd))
                    {
                        quantidadeDigitada = qtd;
                    }

                    termoBusca = partes[1]; // agora a segunda parte é o código
                }
            }

            // Realiza a busca por nome ou código
            var produtos = string.IsNullOrWhiteSpace(termoBusca)
                ? new List<ProdutoSimples>()
                : _produtoRepository.BuscarPorNome(termoBusca);

            // Aplica a quantidade apenas se o produto for por unidade
            foreach (var produto in produtos)
            {
                if (produto.TipoMedida?.ToLower() == "unidade")
                {
                    produto.Quantidade = quantidadeDigitada;
                }
                else
                {
                    // Deixa como está para produtos por quilo
                    produto.Quantidade = 0; // ou null, conforme sua lógica de balança
                }
            }

            var itensCarrinho = _cookiePedidoCompra.Consultar();

            // ViewBag com quantidade pré-preenchida
            ViewBag.QuantidadeDigitada = quantidadeDigitada;

            var model = new VendasViewModel
            {
                Produtos = produtos,
                ItensCarrinho = itensCarrinho
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult AdicionarItem(int id, string? pesoRcebido, int? quantidade)
        {

            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
           
            // Converte o peso recebido para decimal, considerando separadores de decimal.
            decimal peso = 0;            
            if(pesoRcebido != null)
            {
                try
                {
                    peso = Convert.ToDecimal(pesoRcebido.Replace(".", ","), CultureInfo.CurrentCulture);

                }
                catch
                {
                    TempData["Erro"] = "Peso inválido. Use números no formato 0,200.";
                    return RedirectToAction(nameof(Vendas));
                }
            }
            // Valida se o peso é maior que zero
            if (peso <= 0 && quantidade <=0)
            {
                TempData["Erro"] = "O peso deve ser maior que zero.";
                return RedirectToAction(nameof(Vendas));
            }

            // Obtém o produto do repositório
            var produto = _produtoRepository.ObterProduto(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado.";
                return RedirectToAction(nameof(Vendas));
            }

            // Recupera a lista de itens do cookie
            var itensCarrinho = _cookiePedidoCompra.Consultar();

            // Verifica se o item já existe na lista
            var itemExistente = itensCarrinho.FirstOrDefault(p => p.Id == id);

            if (itemExistente != null)
            {
                if (itemExistente.peso > 0) 
                {
                    // Atualiza o peso do item existente
                    itemExistente.peso += peso;
                }
                else if(itemExistente.Quantidade > 0)
                {
                    itemExistente.Quantidade += quantidade;
                }
            }
            else
            {
                // Adiciona um novo item à lista
                var novoItem = new ProdutoSimples
                {
                    Id = id,
                    Descricao = produto.Descricao,
                    peso = peso,
                    Quantidade = quantidade,
                    PrecoUn = produto.PrecoUn
                };
                itensCarrinho.Add(novoItem);
            }

            // Salva a lista atualizada no cookie
            _cookiePedidoCompra.Salvar(itensCarrinho);

            // Redireciona para a página de vendas
            return RedirectToAction(nameof(Vendas));
        }
        [HttpGet]
        public IActionResult ObterProdutoPorId(int id)
        {
            var produto = _produtoRepository.ObterProduto(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            // Retorna apenas os dados necessários em formato JSON
            return Json(new { id = produto.Id, tipoMedida = produto.TipoMedida });
        }



        public IActionResult RemoverItem(int id)
        {
            _cookiePedidoCompra.Remover(new ProdutoSimples() { Id = id });
            return RedirectToAction(nameof(Vendas));
        }
       
        DateTime data;
        [ColaboradorAutorizacao]
        [TypeFilter(typeof(CaixaAutorizacaoAttribute))]
        public IActionResult SalvarComanda(Comanda comanda)
        {
            if(comanda.RefFormasPagamento.Id != null)
            {
                var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
                ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
            }            

            if (comanda == null)
            {
                return BadRequest("Comanda não pode ser nula.");
            }

            List<ProdutoSimples> carrinho = _cookiePedidoCompra.Consultar();

            Comanda novaComanda = new Comanda
            {
                DataAbertura = DateTime.Now,
                NomeCliente = comanda.NomeCliente,
                RefColaborador = new Colaborador { Id = _loginColaborador.GetColaborador().Id },
                RefFormasPagamento = new FormasPagamento { Id = comanda .RefFormasPagamento.Id},
                Desconto = comanda.Desconto
            };

            // Salva a comanda e pega o último ID gerado
            _comandaRepository.Cadastrar(novaComanda);
            novaComanda.Id = _comandaRepository.BuscarUltimoIdComanda();

            decimal totalComanda = 0;

            foreach (var item in carrinho)
            {
                decimal subtotal = item.peso * item.PrecoUn;
                if (item.peso == 0)
                {
                    subtotal = Convert.ToDecimal( item.Quantidade * item.PrecoUn);
                }

                ItemComanda novoItem = new ItemComanda
                {
                    RefComanda = new Comanda { Id = novaComanda.Id },
                    RefProduto = new ProdutoSimples { Id = item.Id },
                    Peso = item.peso,
                    Quantidade =  item.Quantidade,
                    Subtotal = subtotal
                };

                totalComanda += subtotal;

                _itensComandaRepository.Cadastrar(novoItem);
            }
            novaComanda.ValorTotal = totalComanda;
            if(comanda.Desconto != null)
            {
                decimal desconto = Convert.ToDecimal(comanda.Desconto.Replace(".", ","));
                if (desconto > 0)
                {
                    novaComanda.ValorTotal = (totalComanda - desconto);
                }
            }

            //if(comanda.RefFormasPagamento.Id == 0) 
            if (comanda.RefFormasPagamento != null && comanda.RefFormasPagamento.Id > 0)
            {
                _comandaRepository.AtualizarValorComDesconto(novaComanda);
            }
            else
            {
                _comandaRepository.AtualizarValor(novaComanda);
               
            }
            _cookiePedidoCompra.RemoverTodos();

            return RedirectToAction("Vendas");
        }
       
        public IActionResult Index(int? pagina, string pesquisa) 
        {
            return View(_comandaRepository.ObterTodasComandas(pagina, pesquisa));
        }
        public IActionResult limpaConada()
        {
            _cookiePedidoCompra.RemoverTodos();
            return RedirectToAction(nameof(Vendas));
        }
        [HttpGet]
        public IActionResult ComandasFechada(int? pagina, string pesquisa)
        {
            return View(_comandaRepository.ObterTodasComandasFechadas(pagina, pesquisa));
        }
        [HttpGet]
        public IActionResult FechadaComanda(int id)
        {
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
            Comanda comanda = _comandaRepository.ObterComandaPorId(id);
            return View(comanda);            
        }

        [HttpPost]
        public IActionResult FechadaComanda(Comanda comanda)
        {

            if (comanda.RefFormasPagamento.Id != null)
            {
                var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
                ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
            }
            if (comanda.Desconto != null)
            {
                decimal desconto = Convert.ToDecimal(comanda.Desconto.Replace(".", ","));
                if (desconto > 0)
                {
                    comanda.ValorTotal = (comanda.ValorTotal - desconto);
                }
            }
            _comandaRepository.AtualizarValorComDesconto(comanda);

            return RedirectToAction(nameof(Index));
        }
        //Cancelar Comanda
        [HttpGet]
        public IActionResult CancelarComanda(int id)
        {
            Comanda comanda = _comandaRepository.ObterComandaPorId(id);
            
               _comandaRepository.Cancelar(comanda);
            
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult ItensComanda(int id)
        {
            ViewBag.NunComanda = id;
            var itens = _itensComandaRepository.ObterItensPorComanda(id);
            return View(itens);
        }
       
        //controler que acessa o peso através de um serviço no windows que salva o peso no arquivo peso.txt - necessario instalar serviço no windows
        [HttpGet]
        public JsonResult LerPeso()
        {
           string caminhoPeso = @"C:\balanca\peso.txt";  


            if (System.IO.File.Exists(caminhoPeso))
            {
                var peso = System.IO.File.ReadAllText(caminhoPeso).Trim();
                return Json(new { peso });
            }

            return Json(new { peso = "0" });
        }
    }
}
