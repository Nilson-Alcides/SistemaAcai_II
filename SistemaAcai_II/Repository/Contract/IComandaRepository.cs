using SistemaAcai_II.Models;
using System.Collections.Generic;

namespace SistemaAcai_II.Repository.Contract
{
    public interface IComandaRepository
    {
        void Cadastrar(Comanda comanda);
        Comanda ObterComandaPorId(int id);        
        IEnumerable<Comanda> ObterTodasComandas();
        void AtualizarValor(Comanda comanda);
        void Atualizar(Comanda comanda);
        int BuscarUltimoIdComanda();
        void Excluir(int id);
    }
}
