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
    public class FiliaisRepository : IFiliaisRepository
    {
        // Propriedade Privada para injetar a conexão com o banco de dados;
        private readonly string _conexaoMySQL;
        IConfiguration _config;

        //Metodo construtor da classe ClienteRepository    
        public FiliaisRepository(IConfiguration conf)
        {
            // Injeção de dependencia do banco de dados
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }
      
        public IEnumerable<Filiais> ObterTodosFiliais()
        {
            List<Filiais> filList = new List<Filiais>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Filiais", conexao);            

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    filList.Add(
                        new Filiais
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            RazaoSocial = (string)(dr["RazaoSocial"]),
                            NomeFantasia = (string)(dr["NomeFantasia"]),
                            Email = Convert.ToString(dr["Email"]),
                            CNPJ = Convert.ToString(dr["CNPJ"]),
                            Telefone = Convert.ToString(dr["Telefone"]),                            
                            Status = Convert.ToString(dr["Status"])
                        });
                }
                return filList;
            }
        }
        public void Cadastrar(Filiais filiais)
        {          
            string retorno;
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();

                    MySqlCommand cmd = new MySqlCommand("insert into Filiais(RazaoSocial, NomeFantasia, Email, CNPJ, Telefone) " +
                                    "values (@RazaoSocial, @NomeFantasia, @Email, @CNPJ, @Telefone); SELECT LAST_INSERT_ID();", conexao); // @: PARAMETRO
                    
                    cmd.Parameters.Add("@RazaoSocial", MySqlDbType.VarChar).Value = filiais.RazaoSocial;
                    cmd.Parameters.Add("@NomeFantasia", MySqlDbType.VarChar).Value = filiais.NomeFantasia;
                    cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = filiais.Email;                   
                    cmd.Parameters.Add("@CNPJ", MySqlDbType.VarChar).Value = filiais.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                    cmd.Parameters.Add("@Telefone", MySqlDbType.VarChar).Value = filiais.Telefone;                    
                   

                    retorno =  Convert.ToString(cmd.ExecuteScalar());

                    MySqlCommand cmd1 = new MySqlCommand("insert into Endereco(Idfilial, CEP, Estado, Cidade, Bairro, Endereco, Complemento, Numero) " +
                                                         " values (@Idfilial, @CEP, @Estado,@Cidade, @Bairro,@Logradouro, @Complemento, @Numero)", conexao); // @: PARAMETRO

                    cmd1.Parameters.Add("@Idfilial", MySqlDbType.VarChar).Value = retorno;                    
                    cmd1.Parameters.Add("@CEP", MySqlDbType.VarChar).Value = filiais.CEP.Replace(".", "").Replace("-", "");
                    cmd1.Parameters.Add("@Estado", MySqlDbType.VarChar).Value = filiais.Estado;
                    cmd1.Parameters.Add("@Cidade", MySqlDbType.VarChar).Value = filiais.Cidade;
                    cmd1.Parameters.Add("@Bairro", MySqlDbType.VarChar).Value = filiais.Bairro;
                    cmd1.Parameters.Add("@Logradouro", MySqlDbType.VarChar).Value = filiais.Logradouro;
                    cmd1.Parameters.Add("@Complemento", MySqlDbType.VarChar).Value = filiais.Complemento;
                    cmd1.Parameters.Add("@Numero", MySqlDbType.VarChar).Value = filiais.Numero;
                    cmd1.ExecuteNonQuery();
                    conexao.Close();
                }

            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro no banco em cadastro filiais" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro na aplicação em cadastro filiais" + ex.Message);
            }
        }
        public void Atualizar(Filiais filiais)
        {
            string Status = SituacaoConstant.Ativo;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Filiais set RazaoSocial=@RazaoSocial, NomeFantasia=@NomeFantasia, " +
                    " Telefone=@Telefone, Email=@Email, CNPJ=@CNPJ, Status=@Status WHERE Id=@Id ", conexao);

                cmd.Parameters.Add("@Id", MySqlDbType.VarChar).Value = filiais.Id;
                cmd.Parameters.Add("@RazaoSocial", MySqlDbType.VarChar).Value = filiais.RazaoSocial;
                cmd.Parameters.Add("@NomeFantasia", MySqlDbType.DateTime).Value = filiais.NomeFantasia;
                cmd.Parameters.Add("@Email", MySqlDbType.VarChar).Value = filiais.Email;
                cmd.Parameters.Add("@CNPJ", MySqlDbType.VarChar).Value = filiais.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                cmd.Parameters.Add("@Telefone", MySqlDbType.VarChar).Value = filiais.Telefone;
                cmd.Parameters.Add("@Status", MySqlDbType.VarChar).Value = Status;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("delete from Filiais WHERE Id=@Id ", conexao);
                cmd.Parameters.AddWithValue("@Id", Id);
                int i = cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public Filiais ObterFiliais(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Filiais WHERE Id=@Id ", conexao);
                cmd.Parameters.AddWithValue("@Id", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Filiais filiais = new Filiais();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    filiais.Id = (Int32)(dr["Id"]);
                    filiais.RazaoSocial = (string)(dr["Nome"]);                   
                    filiais.NomeFantasia = (string)(dr["Sexo"]);
                    filiais.Email = (string)(dr["Email"]);
                    filiais.CNPJ = (string)(dr["CNPJ"]);
                    filiais.Telefone = (string)(dr["Telefone"]);  
                    filiais.Status = (string)(dr["Status"]);
                }
                return filiais;
            }
        }

        public void Ativar(int Id)
        {
            string Status = SituacaoConstant.Ativo;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Filiais set Status=@Status WHERE Id=@Id ", conexao);

                cmd.Parameters.Add("@Id", MySqlDbType.VarChar).Value = Id;
                cmd.Parameters.Add("@Status", MySqlDbType.VarChar).Value = Status;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void Desativar(int Id)
        {
            string Status = SituacaoConstant.Desativado;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Filiais set Status=@Status WHERE Id=@Id ", conexao);

                cmd.Parameters.Add("@Id", MySqlDbType.VarChar).Value = Id;
                cmd.Parameters.Add("@Status", MySqlDbType.VarChar).Value = Status;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
            
        public IPagedList<Filiais> ObterTodosFiliais(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<Filiais> filList = new List<Filiais>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Filiais;", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from Filiais where RazaoSocial like '%" + pesquisa + "%' ", conexao);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {

                    filList.Add(
                        new Filiais
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            RazaoSocial = (string)(dr["RazaoSocial"]),
                            NomeFantasia = (string)(dr["NomeFantasia"]),
                            Email = Convert.ToString(dr["Email"]),
                            CNPJ = Convert.ToString(dr["CNPJ"]),
                            Telefone = Convert.ToString(dr["Telefone"]),
                            Status = Convert.ToString(dr["Status"])
                        });
                }
                return filList.ToPagedList<Filiais>(NumeroPagina, RegistroPorPagina);
            }
        }
    }
}
