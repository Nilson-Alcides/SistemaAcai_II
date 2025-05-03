using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SistemaAcai_II.Models;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/api/[controller]")]
    [ApiController]
    public class BalancaController : ControllerBase
    {
        private readonly ILogger<BalancaController> _logger;
        private readonly IMemoryCache _memoryCache;
        private const string ChavePeso = "peso_balcao";

        public BalancaController(ILogger<BalancaController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        public IActionResult ReceberPeso([FromBody] Peso modelo)
        {
            if (modelo == null || string.IsNullOrWhiteSpace(modelo.PesoAutomatico))
                return BadRequest("Peso não informado.");

            _logger.LogInformation($"Peso recebido via API: {modelo.PesoAutomatico}");

            _memoryCache.Set(ChavePeso, modelo.PesoAutomatico, TimeSpan.FromSeconds(30)); // expira após 10 segundos

            return Ok(new { status = "sucesso", pesoRecebido = modelo.PesoAutomatico });
        }

        [HttpGet]
        public IActionResult LerPeso()
        {
            var peso = _memoryCache.TryGetValue(ChavePeso, out string valorPeso) ? valorPeso : "0";
            return Ok(new { peso });
        }
    }
}
