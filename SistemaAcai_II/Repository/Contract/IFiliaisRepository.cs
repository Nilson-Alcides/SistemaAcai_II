using SistemaAcai_II.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;


namespace SistemaAcai_II.Repositories.Contract
{
    public interface IFiliaisRepository
    {
        //CRUD
        void Cadastrar(Filiais filiais);
        void Atualizar(Filiais filiais);

        void Ativar(int id);
        void Desativar(int id);

        void Excluir(int Id);
        Filiais ObterFiliais(int Id);
        IEnumerable<Filiais> ObterTodosFiliais();
        IPagedList<Filiais> ObterTodosFiliais(int? pagina, string pesquisa);
    }
}
