using SistemaAcai_II.Models;

namespace SistemaAcai_II.Repository.Contract
{
    public interface ICaixaRepository
    {
        //CRUD
        void AbrirCadastrar(Caixa caixa);
        Caixa BuscarCaixaAbertoHoje();
        void FecharCaixa(Caixa caixa);
    }
}
