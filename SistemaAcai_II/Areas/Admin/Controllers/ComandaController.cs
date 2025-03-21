﻿using SistemaAcai_II.Libraries.PedidoCompra;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using SistemaAcai_II.Libraries.Filtro;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaAcai_II.Libraries.Login;

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
        public IActionResult AdicionarItem(int id, decimal peso)
        {
            if (peso <= 0)
            {
                TempData["Erro"] = "O peso deve ser maior que zero.";
                return RedirectToAction(nameof(Vendas));
            }

            var produto = _produtoRepository.ObterProduto(id);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado.";
                return RedirectToAction(nameof(Vendas));
            }

            var itensCarrinho = _cookiePedidoCompra.Consultar();
            var itemExistente = itensCarrinho.FirstOrDefault(p => p.Id == id);

            if (itemExistente != null)
            {
                itemExistente.peso += peso;
            }
            else
            {
                var novoItem = new ProdutoSimples
                {
                    Id = id,
                    Descricao = produto.Descricao,
                    peso = peso,
                    PrecoUn = produto.PrecoUn
                };
                _cookiePedidoCompra.Cadastrar(novoItem);
            }

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

                ItemComanda novoItem = new ItemComanda
                {
                    RefComanda = new Comanda { Id = novaComanda.Id },
                    RefProduto = new ProdutoSimples { Id = item.Id },
                    Peso = item.peso,
                    Subtotal = subtotal
                };

                totalComanda += subtotal;

                _itensComandaRepository.Cadastrar(novoItem);
            }

            novaComanda.ValorTotal = totalComanda;
            _comandaRepository.AtualizarValor(novaComanda);

            _cookiePedidoCompra.RemoverTodos();

            return RedirectToAction("ComandasAbertas");
        }
              
        public IActionResult ComandasAbertas()
        {
            return  new ContentResult() { Content = "Pagiina Localizada Comandas abertas." };
        }





    }
}
