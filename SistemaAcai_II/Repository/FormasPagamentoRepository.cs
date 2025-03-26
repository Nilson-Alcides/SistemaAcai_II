using MySql.Data.MySqlClient;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using System.Data;
using X.PagedList.Extensions;
using X.PagedList;

namespace SistemaAcai_II.Repository
{
    public class FormasPagamentoRepository : IFormasPagamentoRepository
    {
        // Propriedade Privada para injetar a conexão com o banco de dados ;
        private readonly string _conexaoMySQL;
        IConfiguration _config;

        //Metodo construtor da classe ColaboradorRepository    
        public FormasPagamentoRepository(IConfiguration conf)
        {
            // Injeção de dependencia do banco de dados
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }

        public void Cadastrar(FormasPagamento formasPagamento)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into FormaPagamento(Nome) values (@Nome )", conexao); // @: PARAMETRO

                cmd.Parameters.Add("@Nome", MySqlDbType.VarChar).Value = formasPagamento.Nome;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public IEnumerable<FormasPagamento> ObterTodasFormasPagamentos()
        {
            List<FormasPagamento> pagList = new List<FormasPagamento>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM FormaPagamento", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    pagList.Add(
                        new FormasPagamento
                        {
                            Id = Convert.ToInt32(dr["IdColab"]),
                            Nome = (string)(dr["Nome"])
                        });
                }
                return pagList;
            }
        }
        public IPagedList<FormasPagamento> ObterTodasFormasPagamentos(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<FormasPagamento> pagList = new List<FormasPagamento>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from FormaPagamento;", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from FormaPagamento where nome like '%" + pesquisa + "%' ", conexao);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {

                    pagList.Add(
                        new FormasPagamento
                        {
                            Id = Convert.ToInt32(dr["IdForma"]),
                            Nome = (string)(dr["Nome"])

                        });
                }
                return pagList.ToPagedList<FormasPagamento>(NumeroPagina, RegistroPorPagina);
            }

        }
        public FormasPagamento ObterFormasPagamentoPorId(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from FormaPagamento WHERE IdForma=@IdForma ", conexao);
                cmd.Parameters.AddWithValue("@IdForma", id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                FormasPagamento formasPagamento = new FormasPagamento();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    formasPagamento.Id = (Int32)(dr["IdForma"]);
                    formasPagamento.Nome = (string)(dr["Nome"]);
                }
                return formasPagamento;
            }
        }
        public void Atualizar(FormasPagamento formasPagamento)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update FormaPagamento set Nome=@Nome " +
                                                    " WHERE IdForma=@IdForma ", conexao);

                cmd.Parameters.Add("@IdForma", MySqlDbType.VarChar).Value = formasPagamento.Id;
                cmd.Parameters.Add("@Nome", MySqlDbType.VarChar).Value = formasPagamento.Nome;

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("delete from FormaPagamento WHERE IdForma=@IdForma ", conexao);
                cmd.Parameters.AddWithValue("@IdForma", id);
                int i = cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        
    }
}
