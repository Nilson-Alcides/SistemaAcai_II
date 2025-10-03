using SistemaAcai_II.Models;

namespace SistemaAcai_II.Repository.Contract
{
    public interface IItensComandaRepository
    {
        //CRUD
        IEnumerable<ItemComanda> ObterTodosItens();
        IEnumerable<ItemComanda> ObterItensPorComanda(int Id);

        void Cadastrar(ItemComanda itemComanda);

        void Atualizar(ItemComanda itemComanda);

        ItemComanda ObterItensPorId(int Id);
        

        void Excluir(int id);
        void Excluir(ItemComanda itemComanda);
    }
}
