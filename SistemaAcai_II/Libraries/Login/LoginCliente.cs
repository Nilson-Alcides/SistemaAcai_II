using SistemaAcai_II.Models;
using SistemaAcai_II.Libraries.Sessao;
using Newtonsoft.Json;

namespace SistemaAcai_II.Libraries.Login
{
    public class LoginCliente
    {
        private string Key = "Login.Cliente";
        private Sessao.Sessao _sessao;
        public LoginCliente(Sessao.Sessao sessao)
        {
            _sessao = sessao;
        }
        //Converte o objeto cliente para Json ** Serializar **
        public void Login(Cliente cliente)
        {
            // Serializar
            string clienteJSONString = JsonConvert.SerializeObject(cliente);

            _sessao.Cadastrar(Key, clienteJSONString);
        }
        //Reverter Json para objeto cliente ** Deserializar **
        public Cliente GetCliente()
        {
            // Deserializar
            if (_sessao.Existe(Key))
            {
                string clienteJSONString = _sessao.Consultar(Key);
                return JsonConvert.DeserializeObject<Cliente>(clienteJSONString);
            }
            else
            {
                return null;
            }
        }
        //Remove a sessão e desloga o Cliente
        public void Logout()
        {
            _sessao.RemoverTodos();
        }
    }
}
