using SistemaAcai_II.Libraries.ExportarArquivo;
using SistemaAcai_II.Models;
using System.Net.Mail;

namespace SistemaAcai_II.Libraries.Email
{
    public class GerenciarEmail
    {
        private SmtpClient _smtp;
        private IConfiguration _configuration;
        private readonly ExportaArquivo _exportaArquivo;
        public GerenciarEmail(SmtpClient smtp, IConfiguration configuration, ExportaArquivo exportaArquivo )
        {
            _smtp = smtp;
            _configuration = configuration;
            _exportaArquivo = exportaArquivo;
        }
        public void EnviarComandaPorEmail(Comanda comanda)
        {
            // Gera o PDF da comanda
            byte[] pdfBytes = _exportaArquivo.GerarPdf(new List<Comanda> { comanda });

            // Corpo do e-mail
            string corpoMsg = $@"
        <h2>Resumo da Comanda - LojaAçaí</h2>
        <p><strong>ID:</strong> {comanda.Id}</p>
        <p><strong>Cliente:</strong> {comanda.NomeCliente}</p>
        <p><strong>Data Abertura:</strong> {comanda.DataAbertura:dd/MM/yyyy HH:mm}</p>
        <p><strong>Data Fechamento:</strong> {comanda.DataFechamento:dd/MM/yyyy HH:mm}</p>
        <p><strong>Valor Total:</strong> {comanda.ValorTotal:C}</p>
        <hr />
        <p><em>E-mail enviado automaticamente pelo sistema LojaAçaí.</em></p>
    ";

            // Monta a mensagem
            var mensagem = new MailMessage
            {
                From = new MailAddress(_configuration.GetValue<string>("Email:Username")),
                Subject = $"Fechamento de Comanda #{comanda.Id} - Cliente: {comanda.NomeCliente}",
                Body = corpoMsg,
                IsBodyHtml = true
            };

            mensagem.To.Add("nilson.alcides@gmail.com");

            // Anexa o PDF
            mensagem.Attachments.Add(new Attachment(
                new MemoryStream(pdfBytes),
                $"comanda_{comanda.Id}.pdf",
                "application/pdf"
            ));

            // Envia
            _smtp.Send(mensagem);
        }
    }
}
