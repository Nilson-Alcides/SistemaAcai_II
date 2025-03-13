using SistemaAcai_II.Models;
using SistemaAcai_II.Models.Contants;
using SistemaAcai_II.Repositories.Contract;
using MySql.Data.MySqlClient;
using System.Data;
using X.PagedList;
using X.PagedList.Extensions;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SistemaAcai_II.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        // Propriedade Privada para injetar a conexão com o banco de dados;
        private readonly string _conexaoMySQL;
        IConfiguration _config;

        //Metodo construtor da classe ClienteRepository    
        public ClienteRepository(IConfiguration conf)
        {
            // Injeção de dependencia do banco de dados
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }

        //Logim Cliente
        public Cliente Login(string Email, string Senha)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from cliente where Email = @Email  and Senha = @Senha", conexao);

                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = Email;
                cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = Senha;

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Cliente cliente = new Cliente();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    cliente.Id = Convert.ToInt32(dr["IdCli"]);
                    cliente.Nome = Convert.ToString(dr["Nome"]);
                    cliente.Nascimento = Convert.ToDateTime(dr["Nascimento"]);

                    cliente.Sexo = Convert.ToString(dr["Sexo"]);
                    cliente.CPF = Convert.ToString(dr["CPF"]);
                    cliente.Telefone = Convert.ToString(dr["Telefone"]);
                    cliente.Situacao = Convert.ToString(dr["Situacao"]);

                    cliente.Email = Convert.ToString(dr["Email"]);
                    cliente.Senha = Convert.ToString(dr["Senha"]);
                }
                return cliente;
            }
        }
        public IEnumerable<Cliente> ObterTodosClientes()
        {
            List<Cliente> cliList = new List<Cliente>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM CLIENTE", conexao);
               // MySqlCommand cmd = new MySqlCommand("CALL  proc_SelecionarCliente();", conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    cliList.Add(
                        new Cliente
                        {
                            Id = Convert.ToInt32(dr["IdCli"]),
                            Nome = (string)(dr["Nome"]),
                            Nascimento = Convert.ToDateTime(dr["Nascimento"]),
                            Sexo = Convert.ToString(dr["Sexo"]),
                            CPF = Convert.ToString(dr["CPF"]),
                            Telefone = Convert.ToString(dr["Telefone"]),
                            Email = Convert.ToString(dr["Email"]),
                          //  Senha = Convert.ToString(dr["Senha"]),
                            Situacao = Convert.ToString(dr["Situacao"])
                        });
                }
                return cliList;
            }
        }
        public void Cadastrar(Cliente cliente)
        {
            string Situacao = SituacaoConstant.Ativo;
            string retorno;
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();

                    MySqlCommand cmd = new MySqlCommand("insert into Cliente(Nome, Nascimento, Sexo, CPF, Telefone, Email, Senha, Situacao) " +
                                    "values (@Nome, @Nascimento, @Sexo, @CPF, @Telefone, @Email, @Senha, @Situacao); SELECT LAST_INSERT_ID();", conexao); // @: PARAMETRO

                    

                    // MySqlCommand cmd = new MySqlCommand("CALL (@Nome, @Nascimento, @Sexo, @CPF, @Telefone, @Email, @Senha, @Situacao) " , conexao); // @: PARAMETRO

                    cmd.Parameters.Add("@Nome", MySqlDbType.VarChar).Value = cliente.Nome;
                    cmd.Parameters.Add("@Nascimento", MySqlDbType.DateTime).Value = cliente.Nascimento.ToString("yyyy/MM/dd");
                    cmd.Parameters.Add("@Sexo", MySqlDbType.VarChar).Value = cliente.Sexo;
                    cmd.Parameters.Add("@CPF", MySqlDbType.VarChar).Value = cliente.CPF.Replace(".", "").Replace("-", "");
                    cmd.Parameters.Add("@Telefone", MySqlDbType.VarChar).Value = cliente.Telefone;
                    cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = cliente.Email;
                    cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = cliente.Senha;
                    cmd.Parameters.Add("@Situacao", MySqlDbType.VarChar).Value = Situacao;

                    retorno =  Convert.ToString(cmd.ExecuteScalar());

                    MySqlCommand cmd1 = new MySqlCommand("insert into Endereco(IdCli, CEP, Estado, Cidade, Bairro, Endereco, Complemento, Numero) " +
                                                         " values (@IdCli, @CEP, @Estado,@Cidade, @Bairro,@Logradouro, @Complemento, @Numero)", conexao); // @: PARAMETRO

                    cmd1.Parameters.Add("@IdCli", MySqlDbType.VarChar).Value = retorno;
                    cmd1.Parameters.Add("@CEP", MySqlDbType.VarChar).Value = cliente.CEP.Replace(".", "").Replace("-","");
                    cmd1.Parameters.Add("@Estado", MySqlDbType.VarChar).Value = cliente.Estado;
                    cmd1.Parameters.Add("@Cidade", MySqlDbType.VarChar).Value = cliente.Cidade;
                    cmd1.Parameters.Add("@Bairro", MySqlDbType.VarChar).Value = cliente.Bairro;
                    cmd1.Parameters.Add("@Logradouro", MySqlDbType.VarChar).Value = cliente.Logradouro;
                    cmd1.Parameters.Add("@Complemento", MySqlDbType.VarChar).Value = cliente.Complemento;
                    cmd1.Parameters.Add("@Numero", MySqlDbType.VarChar).Value = cliente.Numero;
                    cmd1.ExecuteNonQuery();
                    conexao.Close();
                }

            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro no banco em cadastro cliente" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro na aplicação em cadastro cliente" + ex.Message);
            }
        }
        public void Atualizar(Cliente cliente)
        {
            string Situacao = SituacaoConstant.Ativo;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Cliente set Nome=@Nome, Nascimento=@Nascimento, Sexo=@Sexo,  CPF=@CPF, " +
                    " Telefone=@Telefone, Email=@Email, Senha=@Senha, Situacao=@Situacao WHERE IdCli=@IdCli ", conexao);

                cmd.Parameters.Add("@IdCli", MySqlDbType.VarChar).Value = cliente.Id;  
                cmd.Parameters.Add("@Nascimento", MySqlDbType.DateTime).Value = cliente.Nascimento.ToString("yyyy/MM/dd");
                cmd.Parameters.Add("@Sexo", MySqlDbType.VarChar).Value = cliente.Sexo;
                cmd.Parameters.Add("@CPF", MySqlDbType.VarChar).Value = cliente.CPF;
                cmd.Parameters.Add("@Telefone", MySqlDbType.VarChar).Value = cliente.Telefone;
                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = cliente.Email;
                cmd.Parameters.Add("@Senha", MySqlDbType.VarChar).Value = cliente.Senha;
                cmd.Parameters.Add("@Situacao", MySqlDbType.VarChar).Value = Situacao;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("delete from Cliente WHERE IdCli=@IdCli ", conexao);
                cmd.Parameters.AddWithValue("@IdCli", Id);
                int i = cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public Cliente ObterCliente(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Cliente WHERE IdCli=@IdCli ", conexao);
                cmd.Parameters.AddWithValue("@IdCli", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Cliente cliente = new Cliente();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    cliente.Id = (Int32)(dr["IdCli"]);
                    cliente.Nome = (string)(dr["Nome"]);
                    cliente.Nascimento = (DateTime)(dr["Nascimento"]);
                    cliente.Sexo = (string)(dr["Sexo"]);
                    cliente.CPF = (string)(dr["CPF"]);
                    cliente.Telefone = (string)(dr["Telefone"]);
                    cliente.Email = (string)(dr["Email"]);
                    cliente.Senha = (string)(dr["Senha"]);
                    cliente.Situacao = (string)(dr["Situacao"]);

                }
                return cliente;
            }
        }

        public void Ativar(int Id)
        {
            string Situacao = SituacaoConstant.Ativo;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Cliente set Situacao=@Situacao WHERE IdCli=@IdCli ", conexao);

                cmd.Parameters.Add("@IdCli", MySqlDbType.VarChar).Value = Id;
                cmd.Parameters.Add("@Situacao", MySqlDbType.VarChar).Value = Situacao;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Desativar(int Id)
        {
            string Situacao = SituacaoConstant.Desativado;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Cliente set Situacao=@Situacao WHERE IdCli=@IdCli ", conexao);

                cmd.Parameters.Add("@IdCli", MySqlDbType.VarChar).Value = Id;
                cmd.Parameters.Add("@Situacao", MySqlDbType.VarChar).Value = Situacao;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        //Busca cpf cadastrado
        public Cliente BuscaCpfCliente(string CPF)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select CPF from Cliente WHERE CPF=@CPF ", conexao);
                cmd.Parameters.AddWithValue("@CPF", CPF);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Cliente cliente = new Cliente();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    cliente.CPF = (string)(dr["CPF"]);

                }
                return cliente;
            }
        }
        public Cliente BuscaEmailCliente(string email)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select Email from Cliente WHERE Email=@Email ", conexao);
                cmd.Parameters.AddWithValue("@Email", email);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Cliente cliente = new Cliente();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    cliente.Email = (string)(dr["Email"]);

                }
                return cliente;
            }
        }

        public IPagedList<Cliente> ObterTodosClientes(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<Cliente> ListCli = new List<Cliente>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from CLIENTE;", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from CLIENTE where Nome like '%" + pesquisa + "%' ", conexao);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListCli.Add(
                       new Cliente
                       {
                           Id = Convert.ToInt32(dr["IdCli"]),
                           Nome = (string)(dr["Nome"]),
                           Nascimento = Convert.ToDateTime(dr["Nascimento"]),
                           Sexo = Convert.ToString(dr["Sexo"]),
                           CPF = Convert.ToString(dr["CPF"]),
                           Telefone = Convert.ToString(dr["Telefone"]),
                           Email = Convert.ToString(dr["Email"]),
                           // Senha = Convert.ToString(dr["Senha"]),
                           Situacao = Convert.ToString(dr["Situacao"])
                       });
                }
                return ListCli.ToPagedList<Cliente>(NumeroPagina, RegistroPorPagina);
            }
        }
    }
}
