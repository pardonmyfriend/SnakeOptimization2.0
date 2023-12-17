using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SnakeOptimization
{
    public class GeneratePDFReport : IGeneratePDFReport
    {
        public void GenerateReport(string path, string reportString)
        {
            try
            {
                

                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
                document.Open();
                Paragraph title = new Paragraph("Raport z dzialania algorytmu Snake Optimization");
                title.Font = FontFactory.GetFont("Arial", 32);
                document.Add(title);
                document.Add(new Paragraph($"Data: {DateTime.Now.ToShortDateString()}"));
                document.Add(new Paragraph(reportString));
                document.Close();

                Console.WriteLine($"Raport został zapisany do pliku: {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas generowania raportu PDF: {ex.Message}");
            }
        }
    }
}

