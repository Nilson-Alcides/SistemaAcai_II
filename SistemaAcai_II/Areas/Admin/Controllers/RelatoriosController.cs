using Microsoft.AspNetCore.Mvc;
using SistemaAcai_II.Libraries.ExportarArquivo;
using SistemaAcai_II.Libraries.Filtro;
using SistemaAcai_II.Libraries.Login;
using SistemaAcai_II.Libraries.PedidoCompra;
using SistemaAcai_II.Repository.Contract;

namespace SistemaAcai_II.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ColaboradorAutorizacao]
    public class RelatoriosController : Controller
    {
        private readonly IComandaRepository _comandaRepository;
        private readonly ExportaArquivo _exportaArquivo;

        public RelatoriosController(IComandaRepository comandaRepository, ExportaArquivo exportaArquivo)
        {
            _comandaRepository = comandaRepository;
            _exportaArquivo = exportaArquivo;
        }       
       
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal, string export)
        {
            var comandas = _comandaRepository.ObterTodasComandasFechadas();

            if (dataInicial.HasValue && dataFinal.HasValue)
            {
                comandas = comandas
                    .Where(c => c.DataAbertura.Date >= dataInicial.Value.Date && c.DataAbertura.Date <= dataFinal.Value.Date)
                    .ToList();
            }

            // Exportar Excel
            if (export == "excel")
            {
                // Gere e retorne o arquivo Excel
                var excelFile = _exportaArquivo.GerarExcel((List<Models.Comanda>)comandas);
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "comandas.xlsx");
            }

            // Exportar PDF
            if (export == "pdf")
            {
                var pdfFile = _exportaArquivo.GerarPdf((List<Models.Comanda>)comandas);
                return File(pdfFile, "application/pdf", "comandas.pdf");
            }

            // Pesquisa normal
            ViewBag.DataInicial = dataInicial;
            ViewBag.DataFinal = dataFinal;
            return View(comandas);
        }
        public IActionResult ComandaHoje()
        {
            DateTime date = DateTime.Now;
            return View(_comandaRepository.BuscarComandasFechadasDoDia(date));
        }
        

    }
}
