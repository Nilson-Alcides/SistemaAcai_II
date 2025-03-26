using SistemaAcai_II.Models;
using X.PagedList;

namespace SistemaAcai_II.Repository.Contract
{
    public interface IFormasPagamentoRepository
    {

        void Cadastrar(FormasPagamento formasPagamento);
        FormasPagamento ObterFormasPagamentoPorId(int id);
        IEnumerable<FormasPagamento> ObterTodasFormasPagamentos();       
        IPagedList<FormasPagamento> ObterTodasFormasPagamentos(int? pagina, string pesquisa);
        void Atualizar(FormasPagamento formasPagamento);        
        void Excluir(int id);
    }
}
