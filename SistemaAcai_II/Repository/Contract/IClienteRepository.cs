using SistemaAcai_II.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;


namespace SistemaAcai_II.Repositories.Contract
{
    public interface IClienteRepository
    {
        // Logim Cliente
        Cliente Login(string Email, string Senha);
        
        //CRUD
        void Cadastrar(Cliente cliente);
        void Atualizar(Cliente cliente);

        void Ativar(int id);
        void Desativar(int id);

        void Excluir(int Id);
        Cliente ObterCliente(int Id);

        Cliente BuscaCpfCliente(string CPF);

        Cliente BuscaEmailCliente(string email);

        IEnumerable<Cliente> ObterTodosClientes();
        IPagedList<Cliente> ObterTodosClientes(int? pagina, string pesquisa);
    }
}
