using System;
using System.Drawing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities.Images;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace Utilities.GUI
{
    public class FrameworkElementPrinter
    {
        public static void Print(FrameworkElement objectToPrint, string print_job_name)
        {
            double original_width = objectToPrint.ActualWidth;
            double original_height = objectToPrint.ActualHeight;

            PrintDialog printDialog = new PrintDialog();
            if (true == printDialog.ShowDialog())
            {
                PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

                // Convert the UI control into a bitmap
                double DPI = 200;
                double dpiScale = DPI / 96.0;
                RenderTargetBitmap bmp = new RenderTargetBitmap(
                    Convert.ToInt32(objectToPrint.ActualWidth * dpiScale),
                    Convert.ToInt32(objectToPrint.ActualHeight * dpiScale),
                    DPI, DPI, PixelFormats.Pbgra32
                );
                bmp.Render(objectToPrint);
                bmp.Freeze();


                // Create the document
                FixedDocument document = new FixedDocument();
                document.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);


                // Break the bitmap down into pages
                int pageWidth = Convert.ToInt32(capabilities.PageImageableArea.ExtentWidth * dpiScale);
                int pageHeight = Convert.ToInt32(capabilities.PageImageableArea.ExtentHeight * dpiScale);
                {
                    FixedPage printDocumentPage = new FixedPage();
                    printDocumentPage.Width = pageWidth;
                    printDocumentPage.Height = pageHeight;

                    // Create a new bitmap for the contents of this page
                    Image pageImage = new Image();
                    pageImage.Source = bmp;
                    pageImage.VerticalAlignment = VerticalAlignment.Center;
                    pageImage.HorizontalAlignment = HorizontalAlignment.Center;

                    // Place the bitmap on the page
                    printDocumentPage.Children.Add(pageImage);

                    PageContent pageContent = new PageContent();
                    ((IAddChild)pageContent).AddChild(printDocumentPage);

                    pageImage.Width = capabilities.PageImageableArea.ExtentWidth;
                    pageImage.Height = capabilities.PageImageableArea.ExtentHeight;

                    FixedPage.SetLeft(pageImage, capabilities.PageImageableArea.OriginWidth);
                    FixedPage.SetTop(pageImage, capabilities.PageImageableArea.OriginHeight);

                    document.Pages.Add(pageContent);
                }

                printDialog.PrintDocument(document.DocumentPaginator, print_job_name);
            }
        }


        public static void PrintMultipage(FrameworkElement objectToPrint, string print_job_name)
        {
            double original_width = objectToPrint.ActualWidth;
            double original_height = objectToPrint.ActualHeight;

            PrintDialog printDialog = new PrintDialog();
            if (true == printDialog.ShowDialog())
            {
                PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

                double DPI = 200;
                double dpiScale = DPI / 96.0;

                FixedDocument document = new FixedDocument();
                document.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                // Convert the UI control into a bitmap at 300 dpi
                RenderTargetBitmap bmp = new RenderTargetBitmap(
                    Convert.ToInt32(objectToPrint.ActualWidth * dpiScale),
                    Convert.ToInt32(objectToPrint.ActualHeight * dpiScale),
                    DPI, DPI, PixelFormats.Pbgra32
                );
                bmp.Render(objectToPrint);
                bmp.Freeze();

                // Convert the RenderTargetBitmap into a bitmap we can more readily use
                Bitmap bmp2 = BitmapImageTools.ConvertBitmapSourceToBitmap(bmp);
                bmp = null;

                // Break the bitmap down into pages
                int pageWidth = Convert.ToInt32(capabilities.PageImageableArea.ExtentWidth * dpiScale);
                int pageHeight = Convert.ToInt32(capabilities.PageImageableArea.ExtentHeight * dpiScale);

                int bmp_width = bmp2.Width;
                int bmp_height = (int)(pageHeight * bmp2.Width / pageWidth);

                int pageBreak = 0;
                int previousPageBreak = 0;
                while (true)
                {
                    pageBreak += bmp_height;

                    // We can't read out more than the image
                    if (pageBreak > bmp2.Height) pageBreak = bmp2.Height;

                    PageContent pageContent = generatePageContent(
                        bmp2,
                        previousPageBreak, pageBreak,
                        document.DocumentPaginator.PageSize.Width,
                        document.DocumentPaginator.PageSize.Height,
                        capabilities
                    );
                    document.Pages.Add(pageContent);

                    previousPageBreak = pageBreak;

                    // Are we done?
                    if (pageBreak >= bmp2.Height)
                    {
                        break;
                    }
                }

                printDialog.PrintDocument(document.DocumentPaginator, print_job_name);
            }
        }

        private static PageContent generatePageContent(
            Bitmap bitmap,
            int top, int bottom,
            double pageWidth, double PageHeight,
            PrintCapabilities capabilities
            )
        {
            FixedPage printDocumentPage = new FixedPage();
            printDocumentPage.Width = pageWidth;
            printDocumentPage.Height = PageHeight;

            int newImageHeight = bottom - top;

            Bitmap bmpPage = BitmapImageTools.CropBitmapRegion(bitmap, 0, top, bitmap.Width, newImageHeight);
            BitmapSource bmpSource = BitmapImageTools.FromBitmap(bmpPage);

            // Create a new bitmap for the contents of this page
            Image pageImage = new Image();
            pageImage.Source = bmpSource;
            pageImage.VerticalAlignment = VerticalAlignment.Center;

            // Place the bitmap on the page
            printDocumentPage.Children.Add(pageImage);

            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(printDocumentPage);

            pageImage.Width = capabilities.PageImageableArea.ExtentWidth;
            pageImage.Height = capabilities.PageImageableArea.ExtentHeight;

            FixedPage.SetLeft(pageImage, capabilities.PageImageableArea.OriginWidth);
            FixedPage.SetTop(pageImage, capabilities.PageImageableArea.OriginHeight);

            return pageContent;
        }
    }
}
