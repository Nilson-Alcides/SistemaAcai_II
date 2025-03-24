using MySql.Data.MySqlClient;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using System.Globalization;

namespace SistemaAcai_II.Repository
{
    public class ItemComandaRepository : IItensComandaRepository
    {
        private readonly string _conexaoMySQL;

        public ItemComandaRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
        }
        public IEnumerable<ItemComanda> ObterTodosItens()
        {
            throw new NotImplementedException();
        }
        public ItemComanda ObterItensPorId(int Id)
        {
            throw new NotImplementedException();
        }
        public void Cadastrar(ItemComanda itemComanda)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into ItemComanda(IdComanda, IdProd, Peso, Quantidade, Subtotal) " +
                                                    " values(@IdComanda, @IdProd, @Peso, @Quantidade,@Subtotal)", conexao);

                cmd.Parameters.Add("@IdComanda", MySqlDbType.VarChar).Value = itemComanda.RefComanda.Id;
                cmd.Parameters.Add("@IdProd", MySqlDbType.VarChar).Value = itemComanda.RefProduto.Id;
                //cmd.Parameters.Add("@Peso", MySqlDbType.Decimal).Value = Convert.ToDecimal(itemComanda.Peso, CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Peso", MySqlDbType.Decimal).Value = Convert.ToDecimal(string.Format(CultureInfo.InvariantCulture, "{0:0.000}", itemComanda.Peso),
                CultureInfo.InvariantCulture) ;
                cmd.Parameters.Add("@Quantidade", MySqlDbType.VarChar).Value = itemComanda.Quantidade;
                cmd.Parameters.Add("@Subtotal", MySqlDbType.Decimal).Value = Convert.ToDecimal(itemComanda.Subtotal, CultureInfo.InvariantCulture);
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void Atualizar(ItemComanda itemComanda)
        {
            throw new NotImplementedException();
        }
        public void Excluir(int Id)
        {
            throw new NotImplementedException();
        }

       
        
    }
}
