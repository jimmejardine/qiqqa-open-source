using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using Qiqqa.Documents.PDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities;
using Utilities.PDF;

namespace Qiqqa.DocumentLibrary.VisualGalleryStuff
{
    /// <summary>
    /// Interaction logic for VisualGalleryControl.xaml
    /// </summary>
    public partial class VisualGalleryControl : UserControl
    {
        public VisualGalleryControl()
        {
            InitializeComponent();
        }


        private static void ExportImage(PdfDictionary image, ref int count_jpeg, ref int count_other)
        {
            string filter = image.Elements.GetName("/Filter");
            switch (filter)
            {
                case "/DCTDecode":
                    ExportJpegImage(image, ref count_jpeg);
                    break;

                case "/FlateDecode":
                    ExportAsPngImage(image, ref count_other);
                    break;
            }
        }


        static int FILECOUNT = 0;
        static void ExportJpegImage(PdfDictionary image, ref int count_jpeg)
        {
            ++count_jpeg;

            if (true)
            {
                byte[] stream = image.Stream.Value;
                using (System.IO.FileStream fs = new System.IO.FileStream(String.Format(@"c:\temp\aaa\Image{0}.jpeg", ++FILECOUNT), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                    {
                        bw.Write(stream);
                    }
                }
            }
        }

        static void ExportAsPngImage(PdfDictionary image, ref int count_other)
        {
            ++count_other;
            byte[] stream = image.Stream.Value;
            int width = image.Elements.GetInteger(PdfImage.Keys.Width);
            int height = image.Elements.GetInteger(PdfImage.Keys.Height);
            int bitsPerComponent = image.Elements.GetInteger(PdfImage.Keys.BitsPerComponent);

            //if (false)
            //{
            //    using (System.IO.FileStream fs = new System.IO.FileStream(String.Format(@"c:\temp\aaa\Image{0}.png", ++FILECOUNT), System.IO.FileMode.Create, System.IO.FileAccess.Write))
            //    {
            //        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
            //        {
            //            bw.Write(stream);
            //        }
            //    }
            //}

        }

        public static void Test(Library library)
        {
            int total_count_jpeg=0;
            int total_count_other=0;

            foreach (PDFDocument pdf_document in library.PDFDocumentsWithLocalFilePresent)
            {
                try
                {
                    using (PdfDocument document = PdfReader.Open(pdf_document.DocumentPath, PdfDocumentOpenMode.ReadOnly))
                    {
                        int count_jpeg = 0;
                        int count_other = 0;

                        foreach (PdfPage page in document.Pages)
                        {
                            PdfDictionary resources = page.Elements.GetDictionary("/Resources");
                            if (resources != null)
                            {
                                // Get external objects dictionary
                                PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
                                if (xObjects != null)
                                {
                                    ICollection<PdfItem> items = xObjects.Elements.Values;
                                    // Iterate references to external objects
                                    foreach (PdfItem item in items)
                                    {
                                        PdfReference reference = item as PdfReference;
                                        if (reference != null)
                                        {
                                            PdfDictionary xObject = reference.Value as PdfDictionary;
                                            // Is external object an image?
                                            if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                            {
                                                ExportImage(xObject, ref count_jpeg, ref count_other);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        total_count_jpeg += count_jpeg;
                        total_count_other += count_other;

                        Logging.Info("We are at {0}/{1} images - total {2}/{3} - {4}", count_jpeg, count_other, total_count_jpeg, total_count_other, pdf_document.DocumentPath);
                    }

                    
                }

                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem extracting images from {0}", pdf_document.DocumentPath);
                }
            }
        }
    }
}

