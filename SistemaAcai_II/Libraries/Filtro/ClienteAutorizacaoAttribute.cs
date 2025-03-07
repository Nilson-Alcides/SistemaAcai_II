using SistemaAcai_II.Libraries.Login;
using SistemaAcai_II.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaAcai_II.Libraries.Filtro
{
    public class ClienteAutorizacaoAttribute : Attribute, IAuthorizationFilter
    {
        LoginCliente _loginCliente;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _loginCliente = (LoginCliente)context.HttpContext.RequestServices.GetService(typeof(LoginCliente));
            Cliente cliente = _loginCliente.GetCliente();
            if (cliente == null)
            {
                context.Result = new ContentResult() { Content = "Acesso negado." };
            } 
        }
    }
}
