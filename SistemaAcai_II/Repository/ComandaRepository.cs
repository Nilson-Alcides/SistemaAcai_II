using SistemaAcai_II.Models;
using SistemaAcai_II.Repository.Contract;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using SistemaAcai_II.Models.Contants;
using System.Globalization;
using static Org.BouncyCastle.Math.EC.ECCurve;
using X.PagedList.Extensions;
using X.PagedList;

namespace SistemaAcai_II.Repository
{
    public class ComandaRepository : IComandaRepository
    {
        IConfiguration _config;
        private readonly string _conexaoMySQL;

        public ComandaRepository(IConfiguration conf)
        {
            _conexaoMySQL = conf.GetConnectionString("ConexaoMySQL");
            _config = conf;
        }

        public void Cadastrar(Comanda comanda)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();               
                MySqlCommand cmd = new MySqlCommand("INSERT INTO Comanda (IdColab, NomeCliente) " +
                                                    " VALUES (@IdColab, @NomeCliente)", conexao);
                cmd.Parameters.AddWithValue("@IdColab", comanda.RefColaborador.Id);
                cmd.Parameters.AddWithValue("@NomeCliente", comanda.NomeCliente);

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        
        public Comanda ObterComandaPorId(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("select * from Comanda as t1 " +
                    " inner join colaborador as t2 on t1.IdColab = t2.IdColab WHERE IdComanda=@IdComanda ", conexao);
                cmd.Parameters.AddWithValue("@IdComanda", Id);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;

                Comanda comanda = new Comanda();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dr.Read())
                {
                    comanda.Id = (Int32)(dr["IdComanda"]);
                    
                    comanda.RefColaborador = new Colaborador
                    {
                        Id = Convert.ToInt32(dr["IdColab"]),
                        Nome = Convert.ToString(dr["Nome"]),
                        Email = Convert.ToString(dr["Email"]),
                        Tipo = Convert.ToString(dr["Tipo"])
                    };
                    comanda.NomeCliente = (string)(dr["NomeCliente"]);
                    comanda.DataAbertura = Convert.ToDateTime(dr["DataAbertura"]);
                    comanda.DataFechamento = Convert.ToDateTime(dr["DataFechamento"]);
                    comanda.ValorTotal = Convert.ToDecimal(dr["ValorTotal"]);
                    comanda.Status = (string)(dr["Situacao"]);
                }
                return comanda;
            }           
        }
        public IEnumerable<Comanda> ObterTodasComandas()
        {
            List<Comanda> ListComanda = new List<Comanda>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Comanda", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListComanda.Add(
                        new Comanda
                        {
                            Id = (Int32)(dr["IdComanda"]),
                            RefColaborador = new Colaborador
                            {
                                Id = Convert.ToInt32(dr["IdColab"]),
                                Nome = (string)(dr["Nome"]),
                                Email = (string)(dr["Email"]),
                                Tipo = (string)(dr["Tipo"])
                            },
                            NomeCliente = (string)(dr["NomeCliente"]),
                            DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                            DataFechamento = Convert.ToDateTime(dr["Senha"]),
                            Status = (string)(dr["Situacao"])
                        });
                }
                return ListComanda;
            }
        }
        public IPagedList<Comanda> ObterTodasComandas(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<Comanda> ListComanda = new List<Comanda>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Comanda WHERE SITUACAO = 'A' ORDER BY IdComanda DESC;;", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from Comanda where SITUACAO = 'A' AND IdComanda like '%" + pesquisa + "%' ORDER BY IdComanda DESC", conexao);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListComanda.Add(
                        new Comanda
                        {
                            Id = (Int32)(dr["IdComanda"]),                          
                            NomeCliente = Convert.ToString(dr["NomeCliente"]),
                            DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                            ValorTotal = Convert.ToDecimal(dr["ValorTotal"])                            
                        });
                }
                return ListComanda.ToPagedList<Comanda>(NumeroPagina, RegistroPorPagina);
            }
        }
        public IPagedList<Comanda> ObterTodasComandasFechadas(int? pagina, string pesquisa)
        {
            int RegistroPorPagina = _config.GetValue<int>("RegistroPorPagina");

            int NumeroPagina = pagina ?? 1;
            List<Comanda> ListComanda = new List<Comanda>();
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Comanda WHERE SITUACAO = 'F' ORDER BY IdComanda DESC;;", conexao);

                if (!string.IsNullOrEmpty(pesquisa))
                {
                    cmd = new MySqlCommand("select * from Comanda where SITUACAO = 'F' AND IdComanda like '%" + pesquisa + "%' ORDER BY IdComanda DESC", conexao);
                }

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ListComanda.Add(
                        new Comanda
                        {
                            Id = (Int32)(dr["IdComanda"]),
                            NomeCliente = Convert.ToString(dr["NomeCliente"]),
                            DataAbertura = Convert.ToDateTime(dr["DataAbertura"]),
                            DataFechamento = Convert.ToDateTime(dr["DataFechamento"]),
                            ValorTotal = Convert.ToDecimal(dr["ValorTotal"])
                        });
                }
                return ListComanda.ToPagedList<Comanda>(NumeroPagina, RegistroPorPagina);
            }
        }

        public void Ativar(int Id)
        {
            string Status = "Ativa";
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("update Filiais set StatusFiliais=@StatusFiliais WHERE Idfilial=@Idfilial ", conexao);

                cmd.Parameters.Add("@Idfilial", MySqlDbType.VarChar).Value = Id;
                cmd.Parameters.Add("@StatusFiliais", MySqlDbType.VarChar).Value = Status;
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }
        public void AtualizarValor(Comanda comanda)
        {           
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                    MySqlCommand cmd = new MySqlCommand("UPDATE Comanda SET ValorTotal = @ValorTotal " +
                        " WHERE IdComanda = @IdComanda", conexao);
               
                cmd.Parameters.Add("@ValorTotal", MySqlDbType.Decimal).Value = Convert.ToDecimal(string.Format(CultureInfo.InvariantCulture, "{0:0.000}", comanda.ValorTotal),
                                   CultureInfo.InvariantCulture);                                
                cmd.Parameters.AddWithValue("@IdComanda", MySqlDbType.VarChar).Value = comanda.Id;                
                cmd.ExecuteNonQuery();                
                conexao.Close();
            }            
        }
        public void AtualizarValorComDesconto(Comanda comanda)
         {
            
            string Situacao = SituacaoConstant.Fechada;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("UPDATE Comanda SET Desconto = @Desconto, " +
                    " ValorTotal = @ValorTotal, Situacao = @Situacao, IdForma = @IdForma WHERE IdComanda = @IdComanda", conexao);
                
                cmd.Parameters.Add("@Desconto", MySqlDbType.VarChar).Value = comanda.Desconto;                
                cmd.Parameters.Add("@ValorTotal", MySqlDbType.Decimal).Value = Convert.ToDecimal(string.Format(CultureInfo.InvariantCulture, "{0:0.000}", comanda.ValorTotal),
                                   CultureInfo.InvariantCulture);
                cmd.Parameters.AddWithValue("@IdForma", MySqlDbType.Int32).Value = comanda.RefFormasPagamento.Id;
                cmd.Parameters.Add("@Situacao", MySqlDbType.VarChar).Value = Situacao;
                cmd.Parameters.AddWithValue("@IdComanda", MySqlDbType.Int32).Value = comanda.Id;
                cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }
        public void Atualizar(Comanda comanda)
        {
            string Situacao = SituacaoConstant.Fechada;
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();                
                    MySqlCommand cmd = new MySqlCommand("UPDATE Comanda SET DataFechamento = @DataFechamento, Situacao = @Situacao WHERE IdComanda = @IdComanda", conexao);
                    cmd.Parameters.AddWithValue("@DataFechamento", comanda.DataFechamento);
                    cmd.Parameters.AddWithValue("@Situacao", Situacao);
                    cmd.Parameters.AddWithValue("@IdComanda", comanda.Id);
                    cmd.ExecuteNonQuery();
                conexao.Close();
            }
        }

        public void Excluir(int id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                string query = "DELETE FROM Comanda WHERE IdComanda = @IdComanda";
                MySqlCommand cmd = new MySqlCommand(query, conexao);
                cmd.Parameters.AddWithValue("@IdComanda", id);
                cmd.ExecuteNonQuery();
            }
        }

        public int BuscarUltimoIdComanda()
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conexao);

                // Obtém o último ID gerado
                int ultimoId = Convert.ToInt32(cmd.ExecuteScalar());

                return ultimoId;
            }
        } 

    }
}
