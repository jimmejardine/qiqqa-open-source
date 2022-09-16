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
using Utilities.GUI;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Camera
{
    /// <summary>
    /// Interaction logic for PDFCameraLayer.xaml
    /// </summary>
    public partial class PDFCameraLayer : PageLayer, IDisposable
    {
        private PDFRendererControlStats pdf_renderer_control_stats;
        private int page;
        private DragAreaTracker drag_area_tracker;

        public PDFCameraLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            Background = Brushes.Transparent;
            Cursor = Cursors.Cross;

            drag_area_tracker = new DragAreaTracker(this);
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;
        }

        private void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
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
            double left = Math.Min(mouse_up_point.X, mouse_down_point.X) / ActualWidth;
            double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) / ActualHeight;
            double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) / ActualWidth;
            double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) / ActualHeight;

            List<Word> words_in_selection = new List<Word>();

            WordList word_list = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
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

            using (MemoryStream ms = new MemoryStream(pdf_renderer_control_stats.pdf_document.PDFRenderer.GetPageByDPIAsImage(page, 150)))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapSource image_page = decoder.Frames[0];
                if (null != image_page)
                {
                    double left = Math.Min(mouse_up_point.X, mouse_down_point.X) * image_page.PixelWidth / ActualWidth;
                    double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) * image_page.PixelHeight / ActualHeight;
                    double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) * image_page.PixelWidth / ActualWidth;
                    double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) * image_page.PixelHeight / ActualHeight;

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

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFCameraLayer()
        {
            Logging.Debug("~PDFCameraLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFCameraLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFCameraLayer::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                foreach (var el in Children)
                {
                    IDisposable node = el as IDisposable;
                    if (null != node)
                    {
                        node.Dispose();
                    }
                }
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                Children.Clear();
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                if (drag_area_tracker != null)
                {
                    drag_area_tracker.OnDragComplete -= drag_area_tracker_OnDragComplete;
                }
            }, must_exec_in_UI_thread: true);

            WPFDoEvents.SafeExec(() =>
            {
                // Clear the references for sanity's sake
                pdf_renderer_control_stats = null;
                drag_area_tracker = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                DataContext = null;
            });

            ++dispose_count;

            //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
        }

        #endregion

    }
}
