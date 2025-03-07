using SistemaAcai_II.Libraries.Sessao;
using SistemaAcai_II.Models;
using Newtonsoft.Json;

namespace SistemaAcai_II.Libraries.Login
{
    public class LoginColaborador
    {
        // Criar uma chave para a sessão
        private string Key = "Login.Colaborador";
        private Sessao.Sessao _sessao;
        //Injetar sessão na classe LoginColaborador
        public LoginColaborador(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }
        //Converte o objeto Colaborador para Json ** Serializar **
        public void Login(Colaborador colaborador)
        {
            //Serializar
            string colaboradorJSONString = JsonConvert.SerializeObject(colaborador);

            _sessao.Cadastrar(Key, colaboradorJSONString);
        }
        //Reverter Json para objeto Colaborador ** Deserializar **
        public Colaborador GetColaborador()
        {
            //Deserializar
            if (_sessao.Existe(Key))
            {
                string colaboradorJSONString = _sessao.Consultar(Key);
                return JsonConvert.DeserializeObject<Colaborador>(colaboradorJSONString);
            }
            else
            {
                return null;
            }
        }
        //Remove a sessão e desloga o Colaborador
        public void Logout()
        {
            _sessao.RemoverTodos();
        }
    }
}
