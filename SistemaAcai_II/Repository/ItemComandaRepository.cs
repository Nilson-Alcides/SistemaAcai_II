using MySql.Data.MySqlClient;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using System.Data;
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
            List<ItemComanda> ListItemComanda = new List<ItemComanda>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(" SELECT * FROM ItemComanda as t1 " +
                    " INNER JOIN ProdutoSimples AS t2 ON t1.IdProd = t1.IdProd where IdComanda = @IdComanda ", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListItemComanda.Add(
                        new ItemComanda
                        {
                            Id = Convert.ToInt32(dr["IdItem"]),
                            Quantidade = Convert.ToInt32(dr["Quantidade"]),
                            Peso = Convert.ToInt32(dr["Peso"]),

                            RefProduto = new ProdutoSimples
                            {
                                Id = Convert.ToInt32(dr["IdProd"]),
                                Descricao = Convert.ToString(dr["Descricao"]),
                                PrecoUn = Convert.ToDecimal(dr["PrecoUn"])
                            },
                            RefComanda = new Comanda
                            {
                                Id = Convert.ToInt32(dr["IdComanda"]),
                                DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                                DataFechamento = Convert.ToDateTime(dr["DataFechamento"])
                            }
                    
                        });
                }
                return ListItemComanda;
            }
        }
        public ItemComanda ObterItensPorId(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * FROM ItemComanda as t1 " +
                    " INNER JOIN ProdutoSimples AS t2 ON t1.IdProd = t1.IdProd " +
                    " INNER JOIN comanda as t3 on t1.IdComanda = t3.IdComanda WHERE t1.IdComanda = @t1.IdComanda ", conexao);
                cmd.Parameters.AddWithValue("@t1.IdComanda", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                ItemComanda itemComanda = new ItemComanda();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    itemComanda.Id = Convert.ToInt32(dr["IdItem"]);
                    itemComanda.Quantidade = Convert.ToInt32(dr["Quantidade"]);
                    itemComanda.Peso = Convert.ToInt32(dr["Peso"]);
                   
                    itemComanda.RefProduto = new ProdutoSimples
                    {
                        Id = Convert.ToInt32(dr["IdProd"]),
                        Descricao = Convert.ToString(dr["Descricao"]),
                        PrecoUn = Convert.ToDecimal(dr["PrecoUn"])
                    };
                    itemComanda.RefComanda = new Comanda
                    {
                        Id = Convert.ToInt32(dr["IdComanda"]),
                        DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                        DataFechamento = Convert.ToDateTime(dr["DataFechamento"])
                    };                   
                }
                return itemComanda;
            }
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
                cmd.Parameters.Add("@Peso", MySqlDbType.Decimal).Value = Convert.ToDecimal(itemComanda.Peso, CultureInfo.InvariantCulture);
                //cmd.Parameters.Add("@Peso", MySqlDbType.Decimal).Value = Convert.ToDecimal(string.Format(CultureInfo.InvariantCulture, "{0:0.000}", itemComanda.Peso),
                //CultureInfo.InvariantCulture) ;
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
        public IEnumerable<ItemComanda> ObterItensPorComanda(int idComanda)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                var query = @"
                            SELECT * 
                            FROM ItemComanda AS t1
                            INNER JOIN ProdutoSimples AS t2 ON t1.IdProd = t2.IdProd
                            INNER JOIN Comanda AS t3 ON t1.IdComanda = t3.IdComanda
                            WHERE t1.IdComanda = @IdComanda";

                MySqlCommand cmd = new MySqlCommand(query, conexao);
                cmd.Parameters.AddWithValue("@IdComanda", idComanda);

                MySqlDataReader dr = cmd.ExecuteReader();
                var listaItens = new List<ItemComanda>();

                while (dr.Read())
                {
                    var itemComanda = new ItemComanda
                    {
                        Id = Convert.ToInt32(dr["IdItem"]),

                        Quantidade = dr["Quantidade"] != DBNull.Value ? Convert.ToInt32(dr["Quantidade"]) : 0,
                        Peso = dr["Peso"] != DBNull.Value ? Convert.ToDecimal(dr["Peso"]) : 0m, 

                        RefProduto = new ProdutoSimples
                        {
                            Id = Convert.ToInt32(dr["IdProd"]),
                            Descricao = Convert.ToString(dr["Descricao"]),
                            PrecoUn = Convert.ToDecimal(dr["PrecoUn"])
                        },
                        RefComanda = new Comanda
                        {
                            Id = Convert.ToInt32(dr["IdComanda"]),
                            DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                            DataFechamento = dr["DataFechamento"] != DBNull.Value ? Convert.ToDateTime(dr["DataFechamento"]) : (DateTime?)null
                        }
                    };

                    listaItens.Add(itemComanda);
                }

                return listaItens;
            }
        }
        
    }
}
