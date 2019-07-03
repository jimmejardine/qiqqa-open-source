using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Qiqqa.Documents.PDF.PDFRendering.CoherentTextExtraction;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class Tests
    {
        static readonly string TEST_PDF_FILENAME_LOCAL2 = @"C:\temp\2.pdf";
        static readonly string TEST_PDF_FILENAME_LOCAL5 = @"C:\temp\5.pdf";
        static readonly string TEST_PDF_FILENAME_LOCAL8 = @"C:\temp\8.pdf";

        static readonly string TEST_PDF_FILENAME = TEST_PDF_FILENAME_LOCAL2;

        public static void TestPDFRenderer()
        {
            PDFRenderer renderer = new PDFRenderer(TEST_PDF_FILENAME, null, null);

            while (true)
            {
                for (int i = 1; i < 10; ++i)
                {
                    Logging.Info("Asking for page {0}", i);
                    Image image = Image.FromStream(new MemoryStream(renderer.GetPageByDPIAsImage(i, 72)));
                    Logging.Info("Image is {0}", image);

                    Thread.Sleep(100);
                }
            }
        }

        static void PageReady(int page)
        {
            Logging.Info("Page {0} is ready", page);
        }

        static void PageTextReady(int page_from, int page_to)
        {
            Logging.Info("Page {0} thru {1} text is ready", page_from, page_to);
        }

        public static void TestTextExtraction()
        {
            PDFRenderer renderer = new PDFRenderer(TEST_PDF_FILENAME, null, null);
            renderer.OnPageTextAvailable += PageTextReady;

            while (true)
            {
                renderer.FlushCachedTexts();

                for (int i = 0; i < 8; ++i)
                {
                    try
                    {
                        string filename = renderer.PDFRendererFileLayer.MakeFilename_TextSingle(i);
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    WordList text = renderer.GetOCRText(i);
                }

                Thread.Sleep(100);
            }
        }

        public static void TestCoherentTextExtractor()
        {
            {
                PDFRenderer pdf_renderer = new PDFRenderer(TEST_PDF_FILENAME_LOCAL2, null, null);
             //   PDFCoherentTextExtractor.ExtractText(pdf_renderer);
            }
            {
                PDFRenderer pdf_renderer = new PDFRenderer(TEST_PDF_FILENAME_LOCAL5, null, null);
             //   PDFCoherentTextExtractor.ExtractText(pdf_renderer);
            }
            {
                PDFRenderer pdf_renderer = new PDFRenderer(TEST_PDF_FILENAME_LOCAL8, null, null);
                PDFCoherentTextExtractor.ExtractText(pdf_renderer);
            }

        }

    }
}
