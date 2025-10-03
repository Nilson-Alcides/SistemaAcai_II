using MySql.Data.MySqlClient;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Repository
{
    public class CaixaRepository : ICaixaRepository
    {
        IConfiguration _config;
        private readonly string _conexaoMySQL;

        public CaixaRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }
        public void AbrirCadastrar(Caixa caixa)
        {
          
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand(@"INSERT INTO Caixa (StatusEmail, ValorInicial)
                                                   VALUES (@StatusEmail, @ValorInicial)", conexao);                
                cmd.Parameters.AddWithValue("@StatusEmail", caixa.StatusEmail ?? "N");
                cmd.Parameters.AddWithValue("@ValorInicial", caixa.ValorInicial);

                cmd.ExecuteNonQuery();
            }
        }
        public Caixa BuscarCaixaAbertoHoje()
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            var query = @"SELECT * FROM Caixa WHERE Situacao = 'A' 
                         AND DATE(DataAbertura) = CURDATE() LIMIT 1";

            using var cmd = new MySqlCommand(query, conexao);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Caixa
                {
                    Id = reader.GetInt32("IdCaixa"),
                    DataAbertura = reader.GetDateTime("DataAbertura"),
                    Situacao = reader.GetString("Situacao"),
                    ValorInicial = reader.GetDecimal("ValorInicial"),
                    StatusEmail = reader.GetString("StatusEmail")
                };
            }

            return null;
        }
        public void FecharCaixa(Caixa caixa)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            var query = @"UPDATE Caixa SET Situacao = @Situacao, DataFechamento = @DataFechamento, 
                      StatusEmail = @StatusEmail WHERE IdCaixa = @IdCaixa";

            using var cmd = new MySqlCommand(query, conexao);
            cmd.Parameters.AddWithValue("@Situacao", caixa.Situacao);
            cmd.Parameters.AddWithValue("@DataFechamento", caixa.DataFechamento);
            cmd.Parameters.AddWithValue("@StatusEmail", caixa.StatusEmail);
            cmd.Parameters.AddWithValue("@IdCaixa", caixa.Id);

            cmd.ExecuteNonQuery();
        }
        public void FecharCaixaAntigos(Caixa caixa)
        {
            using var conexao = new MySqlConnection(_conexaoMySQL);
            conexao.Open();

            // Desativa o modo seguro
            using (var cmdSafeOff = new MySqlCommand("SET SQL_SAFE_UPDATES = 0;", conexao))
            {
                cmdSafeOff.ExecuteNonQuery();
            } 

            var query = @" UPDATE Caixa SET situacao = 'F' WHERE situacao = 'A' AND DATE(DataAbertura) != CURDATE(); ";

            using var cmd = new MySqlCommand(query, conexao);
           // cmd.Parameters.AddWithValue("@Situacao", caixa.Situacao);          

            cmd.ExecuteNonQuery();

            // (Opcional) Reativa o modo seguro
            using (var cmdSafeOn = new MySqlCommand("SET SQL_SAFE_UPDATES = 1;", conexao))
            {
                cmdSafeOn.ExecuteNonQuery();
            }
        }
    }
}
