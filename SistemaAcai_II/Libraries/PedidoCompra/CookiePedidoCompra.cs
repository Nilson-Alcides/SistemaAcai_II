using Newtonsoft.Json;
using SistemaAcai_II.Models;

namespace SistemaAcai_II.Libraries.PedidoCompra
{
    public class CookiePedidoCompra
    {
        //criar uma chave
        private string Key = "Carrinho.Compras";
        private Cookie.Cookie _cookie;

        public CookiePedidoCompra(Cookie.Cookie cookie)
        {
            _cookie = cookie;
        }
        /*
         * CRUD - Cadastrar, Read, Update, Delete
         * Adicionar Item, Remover Item, Alterar Quantidade
         */

        //Salvar
        public void Salvar(List<ProdutoSimples> Lista)
        {
            string Valor = JsonConvert.SerializeObject(Lista);
            _cookie.Cadastrar(Key, Valor);
        }
        //Consulta
        public List<ProdutoSimples> Consultar()
        {
            if (_cookie.Existe(Key))
            {
                string valor = _cookie.Consultar(Key);
                return JsonConvert.DeserializeObject<List<ProdutoSimples>>(valor);
            }
            else
            {
                return new List<ProdutoSimples>();
            }
        }
        //Cadastrar
        public void Cadastrar(ProdutoSimples item)
        {
            List<ProdutoSimples> Lista;
            if (_cookie.Existe(Key))
            {
                Lista = Consultar();
                var ItemLocalizado = Lista.SingleOrDefault(a => a.Id == item.Id);

                if (ItemLocalizado == null)
                {
                    Lista.Add(item);
                }
                else
                {
                    ItemLocalizado.Peso += item.Peso;
                }
            }
            else
            {
                Lista = new List<ProdutoSimples>();
                Lista.Add(item);
            }
            // Criar o metrodo salvar
            Salvar(Lista);
        }
        //Atualiza 
        public void Atualizar(ProdutoSimples item)
        {
            var Lista = Consultar();
            var ItemLocalizado = Lista.SingleOrDefault(a => a.Id == item.Id);

            if (ItemLocalizado != null)
            {
                ItemLocalizado.Peso = item.Peso + 1;
                Salvar(Lista);
            }
        }
        // remove item
        public void Remover(ProdutoSimples item)
        {
            var Lista = Consultar();
            var ItemLocalizado = Lista.FirstOrDefault(a => a.Id == item.Id);

            if (ItemLocalizado != null)
            {
                Lista.Remove(ItemLocalizado);
                Salvar(Lista);
            }
        }
        // Verifica se existe
        public bool Existe(string Key)
        {
            if (_cookie.Existe(Key))
            {
                return false;
            }

            return true;
        }
        // Remove todos itens do carrinho
        public void RemoverTodos()
        {
            _cookie.Remover(Key);
        }

    }
}
