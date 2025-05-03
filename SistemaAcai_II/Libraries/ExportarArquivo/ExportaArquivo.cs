using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SistemaAcai_II.Models;



namespace SistemaAcai_II.Libraries.ExportarArquivo
{
    public class ExportaArquivo
    {
        public byte[] GerarExcel(List<Comanda> comandas)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Comandas");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Cliente";
                worksheet.Cell(1, 3).Value = "Data Abertura";
                worksheet.Cell(1, 4).Value = "Data Fechamento";
                worksheet.Cell(1, 5).Value = "Pagamento";
                worksheet.Cell(1, 6).Value = "Desconto";
                worksheet.Cell(1, 7).Value = "Valor Total";

                for (int i = 0; i < comandas.Count; i++)
                {
                    var c = comandas[i];
                    worksheet.Cell(i + 2, 1).Value = c.Id;
                    worksheet.Cell(i + 2, 2).Value = c.NomeCliente;
                    worksheet.Cell(i + 2, 3).Value = c.DataAbertura.ToString("dd/MM/yyyy");
                    worksheet.Cell(i + 2, 4).Value = c.DataFechamento?.ToString("dd/MM/yyyy");
                    worksheet.Cell(i + 2, 5).Value = c.RefFormasPagamento.Nome;
                    worksheet.Cell(i + 2, 6).Value = c.Desconto.Replace(".",",");
                    worksheet.Cell(i + 2, 7).Value = c.ValorTotal;
                }

                // Linha de total
                int linhaTotal = comandas.Count + 2;
                worksheet.Cell(linhaTotal, 1).Value = "TOTAL";
                worksheet.Range(linhaTotal, 1, linhaTotal, 6).Merge().Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.LightGray);

                worksheet.Cell(linhaTotal, 7).Value = comandas.Sum(c => c.ValorTotal);
                worksheet.Cell(linhaTotal, 7).Style.Font.SetBold(true);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GerarPdf(List<Comanda> comandas)
        {
            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var font = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var table = new PdfPTable(7)
                {
                    WidthPercentage = 100
                };

                // Corrigido: agora define as larguras para 7 colunas
                table.SetWidths(new float[] { 1f, 3f, 2f, 2f, 2f, 2f, 2f });

                // Cabeçalhos
                string[] headers = { "ID", "Cliente", "Data Abertura", "Data Fechamento", "Pagamento", "Desconto", "Valor Total" };

                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, font))
                    {
                        BackgroundColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                // Dados
                foreach (var c in comandas)
                {
                    table.AddCell(new Phrase(c.Id.ToString(), font));
                    table.AddCell(new Phrase(c.NomeCliente, font));
                    table.AddCell(new Phrase(c.DataAbertura.ToString("dd/MM/yyyy"), font));
                    table.AddCell(new Phrase(c.DataFechamento?.ToString("dd/MM/yyyy") ?? "", font));
                    table.AddCell(new Phrase(c.RefFormasPagamento?.Nome ?? "", font));
                    table.AddCell(new Phrase(c.Desconto, font));
                    table.AddCell(new Phrase(c.ValorTotal.ToString("C"), font));
                }

                // Total
                decimal totalGeral = comandas.Sum(c => c.ValorTotal);

                PdfPCell celulaTotal = new PdfPCell(new Phrase("TOTAL", font))
                {
                    Colspan = 6,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = BaseColor.LightGray
                };
                table.AddCell(celulaTotal);

                table.AddCell(new Phrase(totalGeral.ToString("C"), font));

                doc.Add(table);
                doc.Close();

                return stream.ToArray();
            }
        }
    }
}
