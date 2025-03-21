using SistemaAcai_II.Models;
using X.PagedList;

namespace SistemaAcai_II.Repository.Contract
{
    public interface IProdutoSimplesRepository
    {
        //CRUD
        void Cadastrar(ProdutoSimples produtoSimples);
        void Atualizar(ProdutoSimples produtoSimples);
        ProdutoSimples ObterProduto(int Id);
        List<ProdutoSimples> ListarTodos();
       
         
        IEnumerable<ProdutoSimples> ObterTodosProdutos();
        IPagedList<ProdutoSimples> ObterTodosProdutos(int? pagina, string pesquisa);
        IEnumerable<ProdutoSimples> BuscarPorNome(string nome);
        void Excluir(int Id);
    }
}


