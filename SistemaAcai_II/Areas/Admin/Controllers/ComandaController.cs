using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Libraries.Login;
using SistemaAcai_II.Libraries.PedidoCompra;
using SistemaAcai_II.Models;
using SistemaAcai_II.Models.Constants;
using SistemaAcai_II.Repository;
using SistemaAcai_II.Repository.Contract;
using SistemaAcai_II.Services;
using System;
using System.Collections.Generic;
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
        private readonly IScaleService _scale;


        //public ComandaController(IComandaRepository comandaRepository, CookiePedidoCompra cookiePedidoCompra,
        //    IProdutoSimplesRepository produtoRepository, LoginColaborador loginColaborador, IItensComandaRepository itensComandaRepository, IFormasPagamentoRepository formasPagamentoRepository)
        //{
        //    _comandaRepository = comandaRepository;
        //    _cookiePedidoCompra = cookiePedidoCompra;
        //    _produtoRepository = produtoRepository;
        //    _loginColaborador = loginColaborador;
        //    _itensComandaRepository = itensComandaRepository;
        //    _formasPagamentoRepository = formasPagamentoRepository; 
        //}
        public ComandaController(IComandaRepository comandaRepository, CookiePedidoCompra cookiePedidoCompra, IProdutoSimplesRepository produtoRepository, LoginColaborador loginColaborador, IItensComandaRepository itensComandaRepository, IFormasPagamentoRepository formasPagamentoRepository, IScaleService scale)
        {
            _comandaRepository = comandaRepository;
            _cookiePedidoCompra = cookiePedidoCompra;
            _produtoRepository = produtoRepository;
            _loginColaborador = loginColaborador;
            _itensComandaRepository = itensComandaRepository;
            _formasPagamentoRepository = formasPagamentoRepository;
            _scale = scale; // opcional
        }

        private (int? codigo, int? quantidade) TryParseCodigoQuantidade(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo)) return (null, null);

            var parts = termo.Split(new[] { '*', 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .ToArray();

            // Só um bloco → pode ser só o código
            if (parts.Length == 1)
            {
                if (int.TryParse(parts[0], out var cod))
                    return (cod, null);
                return (null, null);
            }

            // Dois blocos → tentar identificar quem é código
            int? aNum = int.TryParse(parts[0], out var a) ? a : (int?)null; // primeiro
            int? bNum = int.TryParse(parts[1], out var b) ? b : (int?)null; // segundo

            bool aExiste = aNum.HasValue && _produtoRepository.ObterProduto(aNum.Value) != null;
            bool bExiste = bNum.HasValue && _produtoRepository.ObterProduto(bNum.Value) != null;

            // REGRA PRINCIPAL: prioriza formato código*quantidade
            if (aExiste)
            {
                int qtd = 1;
                _ = int.TryParse(parts[1], out qtd);
                if (qtd <= 0) qtd = 1;
                return (aNum, qtd);
            }

            // Se o primeiro não é código mas o segundo é → quantidade*código
            if (bExiste)
            {
                int qtd = 1;
                _ = int.TryParse(parts[0], out qtd);
                if (qtd <= 0) qtd = 1;
                return (bNum, qtd);
            }

            // Fallback (quando nenhum existe como produto):
            // Heurística leve: maior parece código, menor parece quantidade
            if (aNum.HasValue && bNum.HasValue)
            {
                if (aNum.Value >= bNum.Value)
                    return (aNum, bNum);
                else
                    return (bNum, aNum);
            }

            return (null, null);
        }


        public IActionResult Vendas(string termo)
        {            
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");

            int quantidadeDigitada = 1;
            string termoBusca = termo;

            int? codigo, qtd;
            (codigo, qtd) = TryParseCodigoQuantidade(termo ?? string.Empty);

            // Se veio quantidade informada no termo, guarde
            if (qtd.HasValue && qtd.Value > 0) quantidadeDigitada = qtd.Value;

            // Se veio só código, mantenha o "termoBusca" como código para buscar
            if (codigo.HasValue) termoBusca = codigo.Value.ToString();

            // Busca (por nome ou código, como já faz)
            var produtos = string.IsNullOrWhiteSpace(termoBusca)
                ? new List<ProdutoSimples>()
                : _produtoRepository.BuscarPorNome(termoBusca);

            // Aplica a quantidade só para produtos por Unidade
            foreach (var produto in produtos)
            {
                if (produto.TipoMedidaEnum == SistemaAcai_II.Models.Constants.TipoMedida.Unidade)
                    produto.Quantidade = quantidadeDigitada;
                else
                    produto.Quantidade = 0; // Kg usa balança
            }

            var itensCarrinho = _cookiePedidoCompra.Consultar();
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
            if (pesoRcebido != null)
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
            if (peso <= 0 && quantidade <= 0)
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
            //Atualiza itens da comanda
            //if (itemExistente != null)
            //{
            //    if (itemExistente.peso > 0) 
            //    {
            //        // Atualiza o peso do item existente
            //        itemExistente.peso += peso;
            //    }
            //    else if(itemExistente.Quantidade > 0)
            //    {
            //        itemExistente.Quantidade += quantidade;
            //    }
            //}
            //else
            //{
            //    // Adiciona um novo item à lista
            //    var novoItem = new ProdutoSimples
            //    {
            //        Id = id,
            //        Descricao = produto.Descricao,
            //        peso = peso,
            //        Quantidade = quantidade,
            //        PrecoUn = produto.PrecoUn
            //    };
            //    itensCarrinho.Add(novoItem);
            //}
            // Adiciona um novo item à lista
            var novoItem = new ProdutoSimples
            {
                Id = id,
                IdItensGuid = Guid.NewGuid(),
                Descricao = produto.Descricao,
                Peso = peso,
                Quantidade = quantidade,
                PrecoUn = produto.PrecoUn
            };
            itensCarrinho.Add(novoItem);

            // Salva a lista atualizada no cookie
            _cookiePedidoCompra.Salvar(itensCarrinho);

            // Redireciona para a página de vendas
            return RedirectToAction(nameof(Vendas));
        }

        //[HttpPost]
        //public IActionResult AdicionarItemNovo(int id, string? pesoRecebido, int? quantidade, int? comandaId /*, Guid IdGuid - não usado */)
        //{
        //    // Converte peso
        //    decimal peso = 0;
        //    if (!string.IsNullOrWhiteSpace(pesoRecebido))
        //    {
        //        try
        //        {
        //            peso = Convert.ToDecimal(pesoRecebido.Replace(".", ","), CultureInfo.CurrentCulture);
        //        }
        //        catch
        //        {
        //            return BadRequest("Peso inválido. Use números no formato 0,200.");
        //        }
        //    }

        //    // Validação correta: se peso <= 0 E (quantidade não informada OU <= 0) => erro
        //    if (peso <= 0m && (!quantidade.HasValue || quantidade.Value <= 0))
        //        return BadRequest("Informe um peso (>0) para itens por Kg ou uma quantidade (>0) para itens por Unidade.");

        //    var produto = _produtoRepository.ObterProduto(id);
        //    if (produto == null)
        //        return NotFound("Produto não encontrado.");

        //    // Adiciona ao cookie (sem persistir no banco aqui)
        //    var itensCarrinho = _cookiePedidoCompra.Consultar();

        //    var novoItem = new ProdutoSimples
        //    {
        //        Id = id,
        //        IdItensGuid = Guid.NewGuid(),
        //        Descricao = produto.Descricao,
        //        Peso = peso,
        //        Quantidade = quantidade,
        //        PrecoUn = produto.PrecoUn
        //    };
        //    itensCarrinho.Add(novoItem);
        //    _cookiePedidoCompra.Salvar(itensCarrinho);

        //    // Retorno no formato que a view espera
        //    var itemRetorno = new
        //    {
        //        idItensGuid = novoItem.IdItensGuid,
        //        quantidade = novoItem.Quantidade,
        //        peso = novoItem.Peso,
        //        refProduto = new
        //        {
        //            id = novoItem.Id,
        //            descricao = novoItem.Descricao,
        //            precoUn = novoItem.PrecoUn.ToString("N2", new CultureInfo("pt-BR"))
        //        }
        //    };

        //    return Ok(itemRetorno);
        //}

        [HttpPost]
        public IActionResult AdicionarItemNovo(int id, string? pesoRecebido, int? quantidade, int? comandaId)
        {
            // valida comanda
            if (comandaId is null || comandaId <= 0)
                return BadRequest("Comanda inválida.");

            var comanda = _comandaRepository.ObterComandaPorId(comandaId.Value);
            if (comanda == null)
                return NotFound("Comanda não encontrada.");

            // parse peso (pt-BR)
            decimal peso = 0;
            if (!string.IsNullOrWhiteSpace(pesoRecebido))
            {
                if (!decimal.TryParse(pesoRecebido.Replace(".", ","), NumberStyles.Any, new CultureInfo("pt-BR"), out peso))
                    return BadRequest("Peso inválido. Use números no formato 0,200.");
            }

            // regra: precisa de peso > 0 (para Kg) ou quantidade > 0 (para Unidade)
            if (peso <= 0m && (!quantidade.HasValue || quantidade.Value <= 0))
                return BadRequest("Informe um peso (>0) para itens por Kg ou uma quantidade (>0) para itens por Unidade.");

            var produto = _produtoRepository.ObterProduto(id);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            // define quantidade/peso e subtotal
            var qtd = (peso > 0m) ? 0 : Math.Max(1, quantidade ?? 0);
            var subtotal = (peso > 0m)
                ? (peso * produto.PrecoUn)
                : (decimal)qtd * produto.PrecoUn;

            // cria item de comanda e persiste no BANCO
            var guid = Guid.NewGuid();
            var novoItem = new ItemComanda
            {
                IdItensGuid = guid,
                RefComanda = new Comanda { Id = comandaId.Value },
                RefProduto = new ProdutoSimples { Id = produto.Id },
                Peso = (peso > 0m) ? peso : (decimal?)null,
                Quantidade = (peso > 0m) ? 0 : qtd,
                Subtotal = subtotal
            };
           // _cookiePedidoCompra.Salvar(novoItem);
            _itensComandaRepository.Cadastrar(novoItem);

            // retorna o JSON que a view espera
            return Ok(new
            {
                idItensGuid = novoItem.IdItensGuid.ToString(),
                quantidade = novoItem.Quantidade,
                peso = novoItem.Peso ?? 0,
                refProduto = new
                {
                    id = produto.Id,                 
                    descricao = produto.Descricao,
                    precoUn = produto.PrecoUn.ToString("N2", new CultureInfo("pt-BR"))
                }
            });
        }




        //[HttpDelete]
        //public IActionResult RemoverItemAjax(Guid id /*, int? comandaId */)
        //{
        //    // Hoje você remove do COOKIE pelo Guid
        //    _cookiePedidoCompra.Remover(new ProdutoSimples { IdItensGuid = id });
        //    return Ok();
        //}
        [HttpDelete]
        public IActionResult RemoverItemAjax(Guid id, int comandaId)
        {
            var item = _itensComandaRepository
                .ObterItensPorComanda(comandaId)
                .FirstOrDefault(i => i.RefProduto != null && i.RefProduto.IdItensGuid == id);

            if (item == null) return NotFound("Item não encontrado.");

            // ⬇️ Ajuste o nome da propriedade conforme seu modelo: Id, IdItem, IdItemComanda, etc.
            _itensComandaRepository.Excluir(item.Id);

            return Ok();
        }



        [HttpGet]
        public IActionResult ObterProdutoPorId(int id)
        {
            var produto = _produtoRepository.ObterProduto(id);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            return Json(new
            {
                id = produto.Id,
                tipoMedida = produto.TipoMedida,                 // "Unidade" / "Kg" (string do banco)
                tipoMedidaEnum = produto.TipoMedidaEnum.ToString() // "Unidade" / "Kg" (enum)
            });
        }
       
        public IActionResult RemoverItemEditado(Guid Id, int idComanda)
        {
            _itensComandaRepository.Excluir(new ItemComanda() { IdItensGuid = Id });

            return RedirectToAction(nameof(EditarComanda), new { id = idComanda });
        }


        public IActionResult RemoverItem(Guid Id)
        {
            _cookiePedidoCompra.Remover(new ProdutoSimples() { IdItensGuid = Id });
            return RedirectToAction(nameof(Vendas));
        }

        DateTime data;
        [ColaboradorAutorizacao]
        [TypeFilter(typeof(CaixaAutorizacaoAttribute))]
        public IActionResult SalvarComanda(Comanda comanda)
        {
            if (comanda.NomeCliente == null)
            {
                comanda.NomeCliente = "Não informado";
            }

            if (comanda.RefFormasPagamento.Id != null)
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
                RefFormasPagamento = new FormasPagamento { Id = comanda.RefFormasPagamento.Id },
                Desconto = comanda.Desconto
            };

            // Salva a comanda e pega o último ID gerado
            _comandaRepository.Cadastrar(novaComanda);
            novaComanda.Id = _comandaRepository.BuscarUltimoIdComanda();

            decimal totalComanda = 0;

            foreach (var item in carrinho)
            {
                decimal subtotal = item.Peso * item.PrecoUn;
                if (item.Peso == 0)
                {
                    subtotal = Convert.ToDecimal(item.Quantidade * item.PrecoUn);
                }

                ItemComanda novoItem = new ItemComanda
                {
                    RefComanda = new Comanda { Id = novaComanda.Id },
                    RefProduto = new ProdutoSimples { Id = item.Id },
                    Peso = item.Peso,
                    Quantidade = item.Quantidade,
                    Subtotal = subtotal
                };

                totalComanda += subtotal;

                _itensComandaRepository.Cadastrar(novoItem);
            }
            novaComanda.ValorTotal = totalComanda;
            if (comanda.Desconto != null)
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
        public IActionResult FecharComanda(int id)
        {
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
            Comanda comanda = _comandaRepository.ObterComandaPorId(id);
            return View(comanda);
        }

        [HttpPost]
        public IActionResult FecharComanda(Comanda comanda)
        {
            if (comanda.NomeCliente == null)
            {
                comanda.NomeCliente = "Não informado";
            }

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
        public IActionResult FecharComandaEditada(int id)
        {
            _cookiePedidoCompra.RemoverTodos();
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");
            Comanda comanda = _comandaRepository.ObterComandaPorId(id);
            return View(comanda);
        }

        [HttpPost]
        public IActionResult FecharComandaEditada(VendasViewModel model, string itensJson)
        {
            var listPagamentos = _formasPagamentoRepository.ObterTodasFormasPagamentos();
            ViewBag.FormaPagamento = new SelectList(listPagamentos, "Id", "Nome");

            // Validação inicial
            if (string.IsNullOrEmpty(itensJson))
            {
                ModelState.AddModelError("", "Nenhum item foi enviado.");
                return View("EditarComanda", model);
            }

            try
            {
                // Desserializar o JSON para List<ItemComanda>
                var itens = JsonConvert.DeserializeObject<List<ItemComanda>>(itensJson);
                if (itens == null || !itens.Any())
                {
                    ModelState.AddModelError("", "A lista de itens está vazia ou inválida.");
                    return View("EditarComanda", model);
                }

                // Mapear itens para ProdutoSimples
                var produtosSimples = new List<ProdutoSimples>();
                foreach (var item in itens)
                {
                    if (item.Id <= 0) // Validar se o Id é válido
                    {
                        ModelState.AddModelError("", $"ID de produto inválido: {item.Id}");
                        continue;
                    }

                    var produto = _produtoRepository.ObterProduto(item.Id);
                    if (produto == null)
                    {
                        ModelState.AddModelError("", $"Produto com ID {item.Id} não encontrado.");
                        continue;
                    }

                    produtosSimples.Add(new ProdutoSimples
                    {
                        Id = produto.Id,
                        Descricao = produto.Descricao,
                        PrecoUn = produto.PrecoUn,
                        Quantidade = item.Quantidade ?? 0,
                        Peso = item.Peso ?? 0
                    });
                }

                // Atribuir à ViewModel
                model.ItensCarrinho = produtosSimples;

                // Validar se há itens válidos
                if (!model.ItensCarrinho.Any())
                {
                    ModelState.AddModelError("", "Nenhum produto válido foi adicionado ao carrinho.");
                    return View("EditarComanda", model);
                }

                // Calcular o total
                decimal total = 0;
                foreach (var item in model.ItensCarrinho)
                {
                    if (item.Peso > 0)
                        total += item.Peso * item.PrecoUn;
                    else
                        total += Convert.ToDecimal(item.Quantidade * item.PrecoUn);
                     model.Comanda.ValorTotal = total;
                }

                // Atualizar a comanda
                if (model.Comanda.Desconto != null)
                {
                    decimal desconto = Convert.ToDecimal(model.Comanda.Desconto.Replace(".", ","));
                    if (desconto > 0)
                    {
                        total = (total - desconto);
                        model.Comanda.ValorTotal = total;
                    }
                }               
                
                _comandaRepository.AtualizarComandaEItens(model.Comanda, itens);

                return RedirectToAction("Index");
            }
            catch (JsonException ex)
            {
                // Tratar erro de desserialização
                ModelState.AddModelError("", $"Erro ao processar os itens: {ex.Message}");
                return View("EditarComanda", model);
            }
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

        public IActionResult EditarComanda(int id, string termo)
        {
            // Formas de pagamento
            ViewBag.FormaPagamento = new SelectList(
                _formasPagamentoRepository.ObterTodasFormasPagamentos(),
                "Id",
                "Nome"
            );

            // Busca com a MESMA regra do Vendas (codigo*qtd etc.)
            int quantidadeDigitada = 1;
            string termoBusca = termo;

            int? codigo, qtd;
            (codigo, qtd) = TryParseCodigoQuantidade(termo ?? string.Empty);

            if (qtd.HasValue && qtd.Value > 0) quantidadeDigitada = qtd.Value;
            if (codigo.HasValue) termoBusca = codigo.Value.ToString();

            var produtos = string.IsNullOrWhiteSpace(termoBusca)
                ? new List<ProdutoSimples>()
                : _produtoRepository.BuscarPorNome(termoBusca);

            // Pré-preenche quantidade para itens por Unidade (igual Vendas)
            foreach (var produto in produtos)
            {
                if (produto.TipoMedidaEnum == SistemaAcai_II.Models.Constants.TipoMedida.Unidade)
                    produto.Quantidade = quantidadeDigitada;
                else
                    produto.Quantidade = 0; // Kg usa balança
            }

            // Carrega comanda e itens já existentes no banco
            var comanda = _comandaRepository.ObterComandaPorId(id);
            if (comanda == null) return NotFound();

            var itensComandaBanco = _itensComandaRepository.ObterItensPorComanda(id);

            var viewModel = new VendasViewModel
            {
                Comanda = comanda,
                Produtos = produtos,                // <<<<<<<<<< AQUI: produtos da busca
                ItensComanda = itensComandaBanco    // itens já existentes
            };

            ViewBag.QuantidadeDigitada = quantidadeDigitada;
            ViewBag.Termo = termo;

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult EditarComanda(Comanda comanda)
        {
            // Atualiza comanda no banco
            _comandaRepository.Atualizar(comanda);

            return RedirectToAction("FecharPedido", new { id = comanda.Id });
        }
    }
}
