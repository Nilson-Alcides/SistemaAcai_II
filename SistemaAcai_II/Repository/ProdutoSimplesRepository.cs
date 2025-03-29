using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using MySql.Data.MySqlClient;
using System.Data;
using X.PagedList;
using static Org.BouncyCastle.Math.EC.ECCurve;
using X.PagedList.Extensions;
using static System.Net.Mime.MediaTypeNames;
using MySqlX.XDevAPI;
using System.Globalization;

namespace AppQuinta6.Repository 
{
    public class ProdutoSimplesRepository : IProdutoSimplesRepository
    {
        // Propriedade Privada para injetar a conexão com o banco de dados ;
        private readonly string _conexaoMySQL;
        IConfiguration _config;

        //Metodo construtor da classe ClienteRepository    
        public ProdutoSimplesRepository(IConfiguration conf)
        {
            // Injeção de dependencia do banco de dados
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }


        public IPagedList<ProdutoSimples> ObterTodosProdutoSimples(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<ProdutoSimples> ListProdSimp = new List<ProdutoSimples>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(" select * from ProdutoSimples ", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from ProdutoSimples where Descricao like '%"
                                          + pesquisa + "%' ", conexao);
                }
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListProdSimp.Add(
                        new ProdutoSimples
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Descricao = (string)(dr["Descricao"]),
                            PrecoUn = (decimal)(dr["PrecoUn"])


                        });
                }
                return ListProdSimp.ToPagedList<ProdutoSimples>(NumeroPagina, RegistroPorPagina);
            }
        }
        public IEnumerable<ProdutoSimples> ObterTodosProdutoSimples()
        {
            List<ProdutoSimples> ListProd = new List<ProdutoSimples>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand(" select * from ProdutoSimples ", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListProd.Add(
                        new ProdutoSimples
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Descricao = (string)(dr["Descricao"]),
                            PrecoUn = (decimal)(dr["PrecoUn"])

                        });
                }
                return ListProd;
            }
        }
        public void Cadastrar(ProdutoSimples produtoSimples)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("insert into ProdutoSimples(Descricao, PrecoUn)  " +
                                                   " values(@Descricao, @PrecoUn)", conexao);

                cmd.Parameters.Add("@Descricao", MySqlDbType.VarChar).Value = produtoSimples.Descricao;             
                cmd.Parameters.Add("@PrecoUn", MySqlDbType.Decimal).Value = Convert.ToDecimal(produtoSimples.PrecoUn, CultureInfo.InvariantCulture); 
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public ProdutoSimples ObterProdutoSimples(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from ProdutoSimples as t1 inner join categoria as t2 on t1.IdCat = t2.Id WHERE t1.Id=@Id", conexao);
                cmd.Parameters.AddWithValue("@Id", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                ProdutoSimples produtoSimples = new ProdutoSimples();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    produtoSimples.Id = Convert.ToInt32(dr["Id"]);
                    produtoSimples.Descricao = (string)(dr["Descricao"]);
                    produtoSimples.PrecoUn = (decimal)(dr["PrecoUn"]);

                }
                return produtoSimples;
            }
        }
        public void Atualizar(ProdutoSimples ProdutoSimples)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update ProdutoSimples set Descricao=@Descricao, PrecoUn=@PrecoUn " +
                                                  "   WHERE IdProd=@Id ", conexao);

                cmd.Parameters.Add("@Descricao", MySqlDbType.VarChar).Value = ProdutoSimples.Descricao;
                cmd.Parameters.Add("@PrecoUn", MySqlDbType.Decimal).Value = ProdutoSimples.PrecoUn;
                cmd.Parameters.Add("@Id", MySqlDbType.Int32).Value = ProdutoSimples.Id; // <- ESTA LINHA FALTAVA

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("delete from ProdutoSimples WHERE Id=@Id ", conexao);
                cmd.Parameters.AddWithValue("@Id", Id);
                int i = cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public ProdutoSimples ObterProduto(int id)
        {
            ProdutoSimples produto = null;

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                string query = "SELECT * FROM ProdutoSimples WHERE IdProd = @id";
                MySqlCommand cmd = new MySqlCommand(query, conexao);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        produto = new ProdutoSimples
                        {                            
                            Id= Convert.ToInt32(reader["IdProd"]),
                            Descricao = reader["Descricao"].ToString(),
                            PrecoUn = Convert.ToDecimal(reader["PrecoUn"])
                        };
                    }
                }
            }

            return produto;
        }

        public List<ProdutoSimples> ListarTodos()
        {
            var lista = new List<ProdutoSimples>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                string query = "SELECT * FROM ProdutoSimples";
                MySqlCommand cmd = new MySqlCommand(query, conexao);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ProdutoSimples
                        {
                            Id = Convert.ToInt32(reader["IdProd"]),
                            Descricao = reader["Descricao"].ToString(),
                            PrecoUn = Convert.ToDecimal(reader["PrecoUn"])
                        });
                    }
                }
            }

            return lista;
        }
        public IEnumerable<ProdutoSimples> BuscarPorNome(string pesquisa)
        {
            var produtos = new List<ProdutoSimples>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                string query;

                // Verifica se o usuário digitou um número (pesquisa por código)
                if (int.TryParse(pesquisa, out int codigo))
                {
                    query = "SELECT * FROM ProdutoSimples WHERE IdProd = @IdProd";
                }
                else
                {
                    query = "SELECT * FROM ProdutoSimples WHERE Descricao LIKE @Descricao";
                }

                using (var cmd = new MySqlCommand(query, conexao))
                {
                    if (int.TryParse(pesquisa, out int codigoPesquisa))
                    {
                        cmd.Parameters.AddWithValue("@IdProd", codigoPesquisa);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Descricao", "%" + pesquisa + "%");
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            produtos.Add(new ProdutoSimples
                            {
                                Id = Convert.ToInt32(reader["IdProd"]),
                                Descricao = reader["Descricao"].ToString(),
                                PrecoUn = Convert.ToDecimal(reader["PrecoUn"])
                            });
                        }
                    }
                }
            }

            return produtos;
        }
        public IEnumerable<ProdutoSimples> ObterTodosProdutos()
        {
            throw new NotImplementedException();
        }

        public IPagedList<ProdutoSimples> ObterTodosProdutos(int? pagina, string pesquisa)
        {
            throw new NotImplementedException();
        }
        //public IEnumerable<ProdutoSimples> BuscarPorNome(string nome)
        //{
        //    var produtos = new List<ProdutoSimples>();
        //    using (var conexao = new MySqlConnection(_conexaoMySQL))
        //    {
        //        conexao.Open();
        //        string query = "SELECT * FROM Produto WHERE Descricao LIKE @Nome";
        //        MySqlCommand cmd = new MySqlCommand(query, conexao);
        //        cmd.Parameters.AddWithValue("@Nome", "%" + nome + "%");

        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                produtos.Add(new ProdutoSimples
        //                {
        //                    Id = Convert.ToInt32(reader["Id"]),
        //                    Descricao = reader["Descricao"].ToString(),
        //                    PrecoUn = Convert.ToDecimal(reader["PrecoUn"])
        //                });
        //            }
        //        }
        //    }
        //    return produtos;
        //}
    }
}