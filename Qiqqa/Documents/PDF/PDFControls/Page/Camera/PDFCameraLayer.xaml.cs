using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Camera
{
    /// <summary>
    /// Interaction logic for PDFCameraLayer.xaml
    /// </summary>
    public partial class PDFCameraLayer : PageLayer
    {
        PDFRendererControlStats pdf_renderer_control_stats;
        int page;

        DragAreaTracker drag_area_tracker;

        public PDFCameraLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            this.Background = Brushes.Transparent;
            this.Cursor = Cursors.Cross;

            drag_area_tracker = new DragAreaTracker(this);
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;
        }

        void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)        
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_Camera);

            double width_page = Math.Abs(mouse_up_point.X - mouse_down_point.X);
            double height_page = Math.Abs(mouse_up_point.Y - mouse_down_point.Y);
            if (3 <= width_page && 3 <= height_page)
            {   
                CroppedBitmap image = GetSnappedImage(mouse_up_point, mouse_down_point);
                List<Word> words = GetSnappedWords(mouse_up_point, mouse_down_point);
                string raw_text = SelectedWordsToFormattedTextConvertor.ConvertToParagraph(words);
                string tabled_text = SelectedWordsToFormattedTextConvertor.ConvertToTable(words);
                

                CameraActionChooserDialog cacd = new CameraActionChooserDialog();
                cacd.SetLovelyDetails(image, raw_text, tabled_text);
                cacd.ShowDialog();
            }
            else
            {
                Logging.Info("Region too small to screen grab");
            }
        }

        private List<Word> GetSnappedWords(Point mouse_up_point, Point mouse_down_point)
        {
            double left = Math.Min(mouse_up_point.X, mouse_down_point.X) / this.ActualWidth;
            double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) / this.ActualHeight;
            double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) / this.ActualWidth;
            double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) / this.ActualHeight;

            List<Word> words_in_selection = new List<Word>();
            
            WordList word_list = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(this.page);
            if (null != word_list)
            {
                foreach (var word in word_list)
                {
                    if (word.IsContained(left, top, width, height))
                    {
                        words_in_selection.Add(word);
                    }
                }
            }

            return words_in_selection;
        }

        private CroppedBitmap GetSnappedImage(Point mouse_up_point, Point mouse_down_point)
        {
            CroppedBitmap cropped_image_page = null;

            PngBitmapDecoder decoder = new PngBitmapDecoder(new MemoryStream(pdf_renderer_control_stats.pdf_document.PDFRenderer.GetPageByDPIAsImage(page, 150)), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            BitmapSource image_page = decoder.Frames[0];
            if (null != image_page)
            {
                double left = Math.Min(mouse_up_point.X, mouse_down_point.X) * image_page.PixelWidth / this.ActualWidth;
                double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) * image_page.PixelHeight / this.ActualHeight;
                double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) * image_page.PixelWidth / this.ActualWidth;
                double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) * image_page.PixelHeight / this.ActualHeight;

                left = Math.Max(left, 0);
                top = Math.Max(top, 0);
                width = Math.Min(width, image_page.PixelWidth - left);
                height = Math.Min(height, image_page.PixelHeight - top);

                if (0 < width && 0 < height)
                {
                    cropped_image_page = new CroppedBitmap(image_page, new Int32Rect((int)left, (int)top, (int)width, (int)height));
                }
            }

            return cropped_image_page;
        }
    }
}
