//using Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.Word;
//using _Application = Microsoft.Office.Interop.Word._Application;

//using Microsoft.Office.Interop.Word;
using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Pdf;
namespace oms
{
    public static class PdfLibrary
    {
        public static string ExportExcelToPdf(string fileName)
        {
            string fileName2 = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdf");

            using (Workbook wb = new Workbook())
            {
                wb.LoadDocument(fileName);

                using (FileStream pdfFileStream = new FileStream(fileName2, FileMode.Create))
                {
                    wb.ExportToPdf(pdfFileStream);
                }
            }

            return fileName2;
        }

        public static string ExportWordToPdf(string fileName)
        {
            string fileName2 = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdf");

            using (RichEditDocumentServer wb = new RichEditDocumentServer())
            {
                wb.LoadDocument(fileName);

                using (FileStream pdfFileStream = new FileStream(fileName2, FileMode.Create))
                {
                    wb.ExportToPdf(pdfFileStream);
                }
            }

            return fileName2;
        }

        public static string ExportImageToPdf(string fileName)
        {
            string fileName2 = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdf");

            using (RichEditDocumentServer wb = new RichEditDocumentServer())
            {
                DocumentImage docImage = wb.Document.Images.Append(DocumentImageSource.FromFile(fileName));
                wb.Document.Sections[0].Page.Width = docImage.Size.Width + wb.Document.Sections[0].Margins.Right + wb.Document.Sections[0].Margins.Left;
                wb.Document.Sections[0].Page.Height = docImage.Size.Height + wb.Document.Sections[0].Margins.Top + wb.Document.Sections[0].Margins.Bottom;

                using (FileStream fs = new FileStream(fileName2, FileMode.OpenOrCreate))
                {
                    wb.ExportToPdf(fs);
                }
            }

            return fileName2;
        }

        public static string ExportHtmlToPdf(string fileName)
        {
            return ExportWordToPdf(fileName);
        }

        public static string CombineMultiplePdf(string[] arrFiles, string destFolder)
        {
            string fileName = Path.Combine(destFolder, "combined.pdf");

            using (PdfDocumentProcessor pdfDocumentProcessor = new PdfDocumentProcessor())
            {
                if(File.Exists(fileName))
                    pdfDocumentProcessor.LoadDocument(fileName);
                else
                    pdfDocumentProcessor.CreateEmptyDocument(fileName);

                foreach (string file in arrFiles)
                {
                    pdfDocumentProcessor.AppendDocument(file);
                }
            }

            return fileName;
        }
    }
}
