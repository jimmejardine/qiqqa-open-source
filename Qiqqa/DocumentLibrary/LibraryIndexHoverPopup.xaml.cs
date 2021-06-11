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
using Utilities.Misc;
using Image = System.Drawing.Image;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Qiqqa.Common.Configuration;
using icons;

namespace Qiqqa.DocumentLibrary
{
    /// <summary>
    /// Interaction logic for LibraryIndexHoverPopup.xaml
    /// </summary>
    public partial class LibraryIndexHoverPopup : UserControl, IDisposable
    {
        private PDFDocument pdf_document = null;
        private int page;
        private PDFAnnotation specific_pdf_annotation = null;

        public LibraryIndexHoverPopup()
        {
            Theme.Initialize();

            InitializeComponent();

            double max_height = Math.Min(SystemParameters.PrimaryScreenHeight / 2, 600);

            ImageThumbnail.Height = max_height;

            ObjTitleBorder.Background = ThemeColours.Background_Brush_Blue_DarkToLight;
        }

        ~LibraryIndexHoverPopup()
        {
            Logging.Debug("~LibraryIndexHoverPopup()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing LibraryIndexHoverPopup");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("LibraryIndexHoverPopup::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    ImageThumbnail.Visibility = Visibility.Collapsed;
                    ImageThumbnail.Source = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    pdf_document = null;
                    specific_pdf_annotation = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                ++dispose_count;
            });
        }

        public void SetPopupContent(PDFDocument pdf_document, int page, PDFAnnotation specific_pdf_annotation = null)
        {
            DataContext = null;
            ObjThemeSwatch.Background = ThemeBrushes.GetBrushForDocument(pdf_document);
            ImageThumbnail.Source = null;

            // Set the new listener
            this.pdf_document = pdf_document;
            this.page = page;
            this.specific_pdf_annotation = specific_pdf_annotation;
            if (null != pdf_document)
            {
                DataContext = pdf_document.Bindable;
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

            // fake it while we test other parts of the UI and can dearly do without the shenanigans of the PDF page rendering system:
            //
            bool allow = ConfigurationManager.IsEnabled("RenderPDFPagesForSidePanels");

            if (!allow)
            {
                ImageThumbnail.Source = Backgrounds.GetBackground(Backgrounds.PageRenderingPending_1);
            }

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                try
                {
                    if (pdf_document?.DocumentExists ?? false)
                    {
                        const double IMAGE_PERCENTAGE = 0.5;
                        BitmapSource image_page = null;

                        using (MemoryStream ms = new MemoryStream(pdf_document.GetPageByHeightAsImage(page, (int)Math.Round(ImageThumbnail.Height / IMAGE_PERCENTAGE), (int)Math.Round(ImageThumbnail.Width / IMAGE_PERCENTAGE))))
                        {
                            using (Image image = Image.FromStream(ms))
                            {
                                PDFOverlayRenderer.RenderAnnotations(image, pdf_document, page, specific_pdf_annotation);
                                PDFOverlayRenderer.RenderHighlights(image, pdf_document, page);
                                PDFOverlayRenderer.RenderInks(image, pdf_document, page);

                                image_page = BitmapImageTools.CreateBitmapSourceFromImage(image);
                                ASSERT.Test(image_page.IsFrozen);
                            }
                        }

                        WPFDoEvents.InvokeAsyncInUIThread(() =>
                        {
                            ImageThumbnail.Source = image_page;

                            if (null != ImageThumbnail.Source)
                            {
                                ImageThumbnail.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageThumbnail.Visibility = Visibility.Collapsed;
                            }
                        });
                    }
                    else
                    {
                        string abstract_text = pdf_document?.Abstract;
                        if (null != abstract_text)
                        {
                            WPFDoEvents.InvokeAsyncInUIThread(() =>
                            {
                                TxtAbstract.Text = abstract_text;
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem showing the PDF thumbnail");
                }
            });
        }
    }
}
