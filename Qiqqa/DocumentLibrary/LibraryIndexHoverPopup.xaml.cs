using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.Documents.PDF.PDFRendering;
using Qiqqa.Expedition;
using Utilities;
using Utilities.GUI;
using Utilities.Images;
using Image = System.Drawing.Image;

namespace Qiqqa.DocumentLibrary
{
    /// <summary>
    /// Interaction logic for LibraryIndexHoverPopup.xaml
    /// </summary>
    public partial class LibraryIndexHoverPopup : UserControl, IDisposable
    {
        PDFDocument pdf_document = null;
        int page;
        PDFAnnotation specific_pdf_annotation;

        public LibraryIndexHoverPopup()
        {
            InitializeComponent();

            double max_height = Math.Min(SystemParameters.PrimaryScreenHeight / 2, 600);

            ImageThumbnail.Height = max_height;

            ObjTitleBorder.Background = ThemeColours.Background_Brush_Blue_DarkToLight;
        }

        ~LibraryIndexHoverPopup()
        {            
            Dispose(false);            
        }

        public void Dispose()
        {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                this.pdf_document = null;
            }

            // Get rid of unmanaged resources 
        }

        public void SetPopupContent(PDFDocument pdf_document, int page)
        {
            SetPopupContent(pdf_document, page, null);
        }

        public void SetPopupContent(PDFDocument pdf_document, int page, PDFAnnotation specific_pdf_annotation)
        {
            this.DataContext = null;
            ObjThemeSwatch.Background = ThemeBrushes.GetBrushForDocument(pdf_document);
            ImageThumbnail.Source = null;

            // Set the new listener
            this.pdf_document = pdf_document;
            this.page = page;
            this.specific_pdf_annotation = specific_pdf_annotation;
            if (null != pdf_document)
            {
                this.DataContext = pdf_document.Bindable;
                DisplayThumbnail();
            }
        }

        private void DisplayThumbnail()
        {
            ImageThumbnail.Source = null;
            TxtAbstract.Text = "";

            if (null == pdf_document)
            {
                return;
            }
            
            try
            {
                if (pdf_document.DocumentExists)
                {
                    double IMAGE_PERCENTAGE = 0.5;

                    Bitmap image = (Bitmap)Image.FromStream(new MemoryStream(pdf_document.PDFRenderer.GetPageByHeightAsImage(this.page, ImageThumbnail.Height / IMAGE_PERCENTAGE)));
                    PDFOverlayRenderer.RenderAnnotations(image, pdf_document, page, specific_pdf_annotation);
                    PDFOverlayRenderer.RenderHighlights(image, pdf_document, page);
                    PDFOverlayRenderer.RenderInks(image, pdf_document, page);

                    image = image.Clone(new RectangleF { Width = image.Width, Height = (int)Math.Round(image.Height * IMAGE_PERCENTAGE) }, image.PixelFormat);
                    BitmapSource image_page = BitmapImageTools.CreateBitmapSourceFromImage(image);
                    ImageThumbnail.Source = image_page;
                }
                else
                {
                    string abstract_text = pdf_document.Abstract;
                    if (PDFAbstractExtraction.CANT_LOCATE != abstract_text)
                    {
                        TxtAbstract.Text = abstract_text;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem showing the PDF thumbnail");
            }

            if (null != ImageThumbnail.Source)
            {
                this.ImageThumbnail.Visibility = Visibility.Visible;
            }
            else
            {
                this.ImageThumbnail.Visibility = Visibility.Collapsed;
            }
        }
    }
}
