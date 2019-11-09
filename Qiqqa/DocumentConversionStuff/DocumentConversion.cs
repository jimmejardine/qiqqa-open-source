using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocToPDFConverter;
using Syncfusion.Pdf;
using Syncfusion.XPS;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentConversionStuff
{
    internal class DocumentConversion
    {
        private delegate bool ConvertDelegate(string filename, string pdf_filename);

        private static Dictionary<string, ConvertDelegate> convertors = new Dictionary<string, ConvertDelegate>()
        {
            {".doc", ConvertorDOC },
            {".docx", ConvertorDOC },
            {".xps", ConvertorXPS },
//            {".rtf", ConvertorRTF },
        };


        public static bool CanConvert(string filename)
        {
            string extension = Path.GetExtension(filename).ToLower();
            return convertors.ContainsKey(extension);
        }

        public static bool Convert(string filename, string pdf_filename)
        {
            // Check that we know how to convert this!
            if (!CanConvert(filename))
            {
                return false;
            }

            // Call the appropriate convertor
            string extension = Path.GetExtension(filename).ToLower();
            return convertors[extension](filename, pdf_filename);
        }

        private static bool ConvertorDOC(string filename, string pdf_filename)
        {
            using (WordDocument word_document = new WordDocument(filename))
            {
                using (DocToPDFConverter converter = new DocToPDFConverter())
                {
                    PdfDocument pdf_document = converter.ConvertToPDF(word_document);

                    pdf_document.Save(pdf_filename);
                    pdf_document.Close(true);
                }
            }
            return true;
        }

        /*
         * NEED A CLIENTPROFILE VERSION
         * 
        private static bool ConvertorXLS(string filename, string pdf_filename)
        {
            ExcelEngine engine = new ExcelEngine();
            IApplication application = engine.Excel;
            IWorkbook book = application.Workbooks.Open(filename);
            
            ExcelToPdfConverterSettings settings = new ExcelToPdfConverterSettings();
            settings.TemplateDocument = new PdfDocument();
            settings.LayoutOptions = LayoutOptions.NoScaling;
            settings.DisplayGridLines = GridLinesDisplayStyle.Invisible;

            ExcelToPdfConverter converter = new ExcelToPdfConverter(book);
            PdfDocument pdf_document = pdf_document = converter.Convert(settings);

            pdf_document.Save(pdf_filename);
            pdf_document.Close(true);
            return true;
        }
        */


        private static bool ConvertorXPS(string filename, string pdf_filename)
        {
            XPSToPdfConverter converter = new XPSToPdfConverter();
            PdfDocument pdf_document = converter.Convert(filename);

            pdf_document.Save(pdf_filename);
            pdf_document.Close(true);
            return true;
        }

        private static bool ConvertorRTF(string filename, string pdf_filename)
        {
            return false;
        }
    }
}
