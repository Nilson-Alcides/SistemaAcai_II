using SistemaAcai_II.Models;
using System.Collections.Generic;
using X.PagedList;

namespace SistemaAcai_II.Repository.Contract
{
    public interface IComandaRepository
    {
        void Cadastrar(Comanda comanda);
        Comanda ObterComandaPorId(int id);        
        IEnumerable<Comanda> ObterTodasComandas();
        IEnumerable<Comanda> ObterTodasComandasFechadas();
        IEnumerable<Comanda> ObterTodasComandasFechadasProData(DateTime dateInicial, DateTime dateFinal);
        IPagedList<Comanda> ObterTodasComandas(int? pagina, string pesquisa);
        IPagedList<Comanda> ObterTodasComandasFechadas(int? pagina, string pesquisa);
        void AtualizarValor(Comanda comanda);
        void AtualizarValorComDesconto(Comanda comanda);
        void Atualizar(Comanda comanda);
        int BuscarUltimoIdComanda();
        void Excluir(int id);
        List<Comanda> BuscarComandasFechadasDoDia(DateTime dateInicial);
    }
}
