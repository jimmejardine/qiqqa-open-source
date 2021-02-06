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
using Utilities.Images;
using Utilities.Misc;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Camera
{
    /// <summary>
    /// Interaction logic for PDFCameraLayer.xaml
    /// </summary>
    public partial class PDFCameraLayer : PageLayer, IDisposable
    {
        private PDFDocument pdf_document;
        private int page;
        private DragAreaTracker drag_area_tracker;

        public PDFCameraLayer(PDFDocument pdf_document, int page)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            this.pdf_document = pdf_document;
            this.page = page;

            InitializeComponent();

            Background = Brushes.Transparent;
            Cursor = Cursors.Cross;

            drag_area_tracker = new DragAreaTracker(this);
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;

            //Unloaded += PDFCameraLayer_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void PDFCameraLayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_Camera);

            double width_page = Math.Abs(mouse_up_point.X - mouse_down_point.X);
            double height_page = Math.Abs(mouse_up_point.Y - mouse_down_point.Y);
            if (3 <= width_page && 3 <= height_page)
            {
                DocPageInfo page_info = new DocPageInfo{
                    pdf_document = @pdf_document,
                    page = @page,
                    ActualHeight = @ActualHeight,
                    ActualWidth = @ActualWidth
                };

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    // GetSnappedImage() invokes the background renderer, hence run it in a background thread itself:
                    BitmapSource image = GetSnappedImage(page_info, mouse_up_point, mouse_down_point);
                    List<Word> words = GetSnappedWords(page_info, mouse_up_point, mouse_down_point);
                    string raw_text = SelectedWordsToFormattedTextConvertor.ConvertToParagraph(words);
                    string tabled_text = SelectedWordsToFormattedTextConvertor.ConvertToTable(words);

                    WPFDoEvents.InvokeAsyncInUIThread(() =>
                    {
                        CameraActionChooserDialog cacd = new CameraActionChooserDialog();
                        cacd.SetLovelyDetails(image, raw_text, tabled_text);
                        cacd.ShowDialog();
                    });
                });
            }
            else
            {
                Logging.Info("Region too small to screen grab");
            }
        }

        private class DocPageInfo
        {
            internal PDFDocument pdf_document;
            internal int page;
            internal double ActualWidth;
            internal double ActualHeight;
        }

        private static List<Word> GetSnappedWords(DocPageInfo page_info, Point mouse_up_point, Point mouse_down_point)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            double left = Math.Min(mouse_up_point.X, mouse_down_point.X) / page_info.ActualWidth;
            double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) / page_info.ActualHeight;
            double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) / page_info.ActualWidth;
            double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) / page_info.ActualHeight;

            List<Word> words_in_selection = new List<Word>();

            WordList word_list = page_info.pdf_document.PDFRenderer.GetOCRText(page_info.page);
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

        private static BitmapSource GetSnappedImage(DocPageInfo page_info, Point mouse_up_point, Point mouse_down_point)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            BitmapSource cropped_image_page = null;

            using (MemoryStream ms = new MemoryStream(page_info.pdf_document.PDFRenderer.GetPageByDPIAsImage(page_info.page, 150)))
            {
                PngBitmapDecoder decoder = new PngBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapSource image_page = decoder.Frames[0];
                if (null != image_page)
                {
                    double left = Math.Min(mouse_up_point.X, mouse_down_point.X) * image_page.PixelWidth / page_info.ActualWidth;
                    double top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) * image_page.PixelHeight / page_info.ActualHeight;
                    double width = Math.Abs(mouse_up_point.X - mouse_down_point.X) * image_page.PixelWidth / page_info.ActualWidth;
                    double height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) * image_page.PixelHeight / page_info.ActualHeight;

                    left = Math.Max(left, 0);
                    top = Math.Max(top, 0);
                    width = Math.Min(width, image_page.PixelWidth - left);
                    height = Math.Min(height, image_page.PixelHeight - top);

                    if (0 < width && 0 < height)
                    {
                        var cropped = new CroppedBitmap(image_page, new Int32Rect((int)left, (int)top, (int)width, (int)height));

                        // UPDATE HERE: CroppedBitmap to BitmapImage
                        // cropped_image_page = GetJpgImage(cropped.Source);
                        // or
                        //cropped_image_page = GetPngImage(cropped.Source);

                        using (MemoryStream mStream = new MemoryStream())
                        {
                            PngBitmapEncoder jEncoder = new PngBitmapEncoder();

                            jEncoder.Frames.Add(BitmapFrame.Create(cropped));  // the croppedBitmap is a CroppedBitmap object

                            // jEncoder.QualityLevel = 75;
                            jEncoder.Save(mStream);

                            cropped_image_page = BitmapImageTools.LoadFromStream(mStream);

                            // I can also get array of bytes that represent the cropped image by call this method : mStream.GetBuffer()

                            //cropped_image_page = BitmapImageTools.CropImageRegion(image_page, left, top, width, height);
                        }
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

            WPFDoEvents.InvokeInUIThread(() =>
            {
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
                });

                WPFDoEvents.SafeExec(() =>
                {
                    Children.Clear();
                });

                WPFDoEvents.SafeExec(() =>
                {
                    if (drag_area_tracker != null)
                    {
                        drag_area_tracker.OnDragComplete -= drag_area_tracker_OnDragComplete;
                    }

                    Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    // Clear the references for sanity's sake
                    pdf_document = null;
                    drag_area_tracker = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                ++dispose_count;

                //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
            });
        }

        #endregion

    }
}
