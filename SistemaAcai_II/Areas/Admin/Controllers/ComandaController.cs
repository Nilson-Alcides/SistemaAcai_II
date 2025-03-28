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
    public class ComandaController : Controller
    {
        private readonly IComandaRepository _comandaRepository;
        private readonly CookiePedidoCompra _cookiePedidoCompra;
        private readonly IProdutoSimplesRepository _produtoRepository;
        private readonly IItensComandaRepository _itensComandaRepository;
        private LoginColaborador _loginColaborador;
       

        public ComandaController(IComandaRepository comandaRepository, CookiePedidoCompra cookiePedidoCompra,
            IProdutoSimplesRepository produtoRepository, LoginColaborador loginColaborador, IItensComandaRepository itensComandaRepository)
        {
            _comandaRepository = comandaRepository;
            _cookiePedidoCompra = cookiePedidoCompra;
            _produtoRepository = produtoRepository;
            _loginColaborador = loginColaborador;
            _itensComandaRepository = itensComandaRepository;
        }
        public IActionResult Vendas(string termo)
        {
            // Busca produtos caso tenha um termo de pesquisa
            var produtos = string.IsNullOrWhiteSpace(termo) ? new List<ProdutoSimples>() : _produtoRepository.BuscarPorNome(termo);

            // Obtém os itens do carrinho
            var itensCarrinho = _cookiePedidoCompra.Consultar();

            // Passa os dados para a View
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
        public IActionResult RemoverItem(int id)
        {
            _cookiePedidoCompra.Remover(new ProdutoSimples() { Id = id });
            return RedirectToAction(nameof(Vendas));
        }
       
        DateTime data;
        [ColaboradorAutorizacao]
        public IActionResult SalvarComanda(Comanda comanda)
        {
            if (comanda == null)
            {
                return BadRequest("Comanda não pode ser nula.");
            }

            List<ProdutoSimples> carrinho = _cookiePedidoCompra.Consultar();

            Comanda novaComanda = new Comanda
            {
                DataAbertura = DateTime.Now,
                NomeCliente = comanda.NomeCliente,
                RefColaborador = new Colaborador { Id = _loginColaborador.GetColaborador().Id }
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
            _comandaRepository.AtualizarValor(novaComanda);

            _cookiePedidoCompra.RemoverTodos();

            return RedirectToAction("Index");
        }
       
        public IActionResult Index(int? pagina, string pesquisa) 
        {
            return View(_comandaRepository.ObterTodasComandas(pagina, pesquisa));
        }

        public IActionResult ComandasAbertas()
        {
            return  new ContentResult() { Content = "Pagiina Localizada Comandas abertas." };
        }

    }
}
