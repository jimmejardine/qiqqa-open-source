using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;
using Utilities.GUI;
using Utilities.Images;
using Utilities.PDF.MuPDF;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace QiqqaOCR
{
    /// <summary>
    /// Interaction logic for RegionExtractorTestWindow.xaml
    /// </summary>
    public partial class RegionExtractorTestWindow : UserControl
    {
        PDFRegionLocator region_locator;
        BitmapSource bitmap_source;

        public RegionExtractorTestWindow()
        {
            InitializeComponent();

            this.ObjImage.Stretch = Stretch.Fill;
            this.SizeChanged += RegionExtractorTestWindow_SizeChanged;

            ObjButtonGO.Click += ObjButtonGO_Click;
        }

        void ObjButtonGO_Click(object sender, RoutedEventArgs e)
        {
            int pdf_number = Convert.ToInt32(ObjTextDoc.Text);
            int page_number = Convert.ToInt32(ObjTextPage.Text);

            string pdf_filename = String.Format(@"C:\temp\{0}.pdf", pdf_number);

            Logging.Info("+Rendering page");
            MemoryStream ms = MuPDFRenderer.RenderPDFPage(pdf_filename, page_number, 200, null, ProcessPriorityClass.Normal);
            BitmapSource bitmap_image = BitmapImageTools.LoadFromBytes(ms.ToArray());
            Bitmap bitmap = new Bitmap(ms);
            Logging.Info("-Rendering page");
            
            this.Image = bitmap_image;

            Logging.Info("+Finding regions");
            this.region_locator = new PDFRegionLocator(bitmap);
            Logging.Info("-Finding regions");

            Recalc();
        }

        void RegionExtractorTestWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Recalc();
        }

        public PDFRegionLocator PDFRegionLocator
        {
            set
            {
                this.region_locator = value;
                Recalc();
            }
        }

        public BitmapSource Image
        {
            set
            {
                this.bitmap_source = value;
                this.ObjImage.Source = this.bitmap_source;
                Recalc();
            }
        }

        private void Recalc()
        {
            ObjCanvas.Children.Clear();

            if (null == region_locator || null == bitmap_source || Double.IsNaN(ObjImage.ActualHeight))
            {
                return;
            }

            
            // The test strip
            if (true)
            {
                Rectangle rect = new Rectangle();
                rect.Fill = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Black, 128));
                rect.Width = region_locator.width_x * ObjImage.ActualWidth / bitmap_source.PixelWidth;
                rect.Height = ObjImage.ActualHeight;
                Canvas.SetTop(rect, 0);
                Canvas.SetLeft(rect, (bitmap_source.PixelWidth / 2.0 - region_locator.width_x / 2.0) / bitmap_source.PixelWidth * ObjImage.ActualWidth);
                ObjCanvas.Children.Add(rect);
            }

            // The grid
            if (true)
            {
                for (int y = 0; y < bitmap_source.PixelHeight; y += 100)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Black, 128));
                    rect.Width = ObjImage.ActualWidth;
                    rect.Height = 1;
                    Canvas.SetTop(rect, ObjImage.ActualHeight * y / bitmap_source.PixelHeight);
                    Canvas.SetLeft(rect, 0);
                    ObjCanvas.Children.Add(rect);
                }
            }


            PDFRegionLocator.Region last_region = region_locator.regions[0];
            foreach (PDFRegionLocator.Region region in region_locator.regions)
            {
                Rectangle rect = new Rectangle();
                rect.Fill = ChooseBrush(last_region);
                rect.Width = ObjImage.ActualWidth;
                rect.Height = ObjImage.ActualHeight * (region.y - last_region.y) / bitmap_source.PixelHeight;
                Canvas.SetTop(rect, ObjImage.ActualHeight * (last_region.y) / bitmap_source.PixelHeight);
                Canvas.SetLeft(rect, 0);
                ObjCanvas.Children.Add(rect);

                last_region = region;
            }
        }

        private static Brush ChooseBrush(PDFRegionLocator.Region region)
        {
            switch (region.state)
            {
                case PDFRegionLocator.SegmentState.TOP:
                    //return new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Yellow, 64));
                    return Brushes.Transparent;
                case PDFRegionLocator.SegmentState.BOTTOM:
                    //return new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Orange, 64));
                    return Brushes.Transparent;
                case PDFRegionLocator.SegmentState.BLANKS:
                    //return new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Green, 64));
                    return Brushes.Transparent;
                case PDFRegionLocator.SegmentState.PIXELS:
                    return new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Red, 64));
                default:
                    throw new NotImplementedException();
            }            
        }
    }
}
