using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SistemaAcai_II.Models;
using SistemaAcai_II.Repository;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Libraries.Filtro
{
    public class CaixaAutorizacaoAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var caixaRepository = serviceProvider.GetRequiredService<ICaixaRepository>();

            var caixaAberto = caixaRepository.BuscarCaixaAbertoHoje();

            if (caixaAberto == null || caixaAberto.Situacao == "F")
            {
                context.Result = new RedirectToActionResult("Index", "Caixa", null);
            }
        }      
    }
 }

