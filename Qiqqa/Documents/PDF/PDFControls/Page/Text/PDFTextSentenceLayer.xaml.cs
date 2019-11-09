using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Text
{
    /// <summary>
    /// Interaction logic for PDFTextLayer.xaml
    /// </summary>
    public partial class PDFTextSentenceLayer : PageLayer, IDisposable
    {
        private PDFRendererControlStats pdf_renderer_control_stats;
        private int page;

        private DragAreaTracker drag_area_tracker;
        private TextLayerSelectionMode text_layer_selection_mode;
        private TextSelectionManager text_selection_manager;

        public PDFTextSentenceLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            Focusable = true;
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Once);

            Background = Brushes.Transparent;
            Cursor = Cursors.IBeam;

            SizeChanged += PDFTextSentenceLayer_SizeChanged;

            drag_area_tracker = new DragAreaTracker(this, false);
            drag_area_tracker.OnDragStarted += drag_area_tracker_OnDragStarted;
            drag_area_tracker.OnDragInProgress += drag_area_tracker_OnDragInProgress;
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;

            text_selection_manager = new TextSelectionManager();

            PreviewMouseDown += PDFTextSentenceLayer_PreviewMouseDown;
            PreviewKeyDown += PDFTextLayer_PreviewKeyDown;
            RequestBringIntoView += PDFTextSentenceLayer_RequestBringIntoView;

            text_layer_selection_mode = TextLayerSelectionMode.Sentence;
        }

        private void PDFTextSentenceLayer_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // We don't want this control to be scrolled into view when it gets keyboard focus...
            e.Handled = true;
        }

        private void PDFTextSentenceLayer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
        }

        private void PDFTextLayer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && KeyboardTools.IsCTRLDown())
            {
                Logging.Info("Copying text");

                FeatureTrackingManager.Instance.UseFeature(Features.Document_CopyText);

                try
                {
                    string selected_text = text_selection_manager.GetLastSelectedWordsString();
                    ClipboardTools.SetText(selected_text);
                }

                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem copying text to the clipboard.");
                }

                e.Handled = true;
            }
        }

        internal override void DeselectLayer()
        {
            ClearChildren();
        }

        private void ClearChildren()
        {
            PDFTextItemPool.Instance.RecyclePDFTextItemsFromChildren(Children);
        }

        private void drag_area_tracker_OnDragStarted(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point)
        {
            WordList words = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
            WordList selected_words = text_selection_manager.OnDragStarted(text_layer_selection_mode, words, ActualWidth, ActualHeight, button_left_pressed, button_right_pressed, mouse_down_point);
        }

        private void drag_area_tracker_OnDragInProgress(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_move_point)
        {
            if (button_left_pressed)
            {
                WordList selected_words = text_selection_manager.OnDragInProgress(button_left_pressed, button_right_pressed, mouse_down_point, mouse_move_point);
                ReflectWordList(selected_words);
            }
        }

        private void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
        {
            if (button_left_pressed)
            {
                WordList selected_words = text_selection_manager.OnDragComplete(button_left_pressed, button_right_pressed, mouse_down_point, mouse_up_point);
                ReflectWordList(selected_words);
            }

            string selected_text = text_selection_manager.GetLastSelectedWordsString();
            if (selected_text.Length > 0)
            {
                if (button_right_pressed)
                {
                    PDFTextSelectPopup popup = new PDFTextSelectPopup(selected_text, pdf_renderer_control_stats.pdf_document);
                    popup.Open();
                }

                pdf_renderer_control_stats.pdf_renderer_control.OnTextSelected(selected_text);
            }
        }

        private void ReflectWordList(WordList words)
        {
            ClearChildren();
            foreach (Word word in words)
            {
                PDFTextItem pdf_text_item = PDFTextItemPool.Instance.GetPDFTextItem(word);
                pdf_text_item.SetHighlightedAppearance(true);
                ResizeTextItem(pdf_text_item);
                Children.Add(pdf_text_item);
            }
        }

        private void PDFTextSentenceLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (PDFTextItem pdf_text_item in Children.OfType<PDFTextItem>())
            {
                ResizeTextItem(pdf_text_item);
            }
        }

        internal override void DeselectPage()
        {
            ClearChildren();
        }

        internal override void SelectPage()
        {
            BuildWords();
        }

        internal override void PageTextAvailable()
        {
            BuildWords();
        }

        private void BuildWords()
        {
            ClearChildren();

            WordList words = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
            if (null == words)
            {
                Children.Add(new OCRNotAvailableControl());
            }
            else
            {
                foreach (var t in Children.OfType<OCRNotAvailableControl>())
                {
                    Children.Remove(t);
                    break;
                }
            }
        }

        private void ResizeTextItem(PDFTextItem pdf_text_item)
        {
            SetLeft(pdf_text_item, pdf_text_item.word.Left * ActualWidth);
            SetTop(pdf_text_item, pdf_text_item.word.Top * ActualHeight);
            pdf_text_item.Width = pdf_text_item.word.Width * ActualWidth;
            pdf_text_item.Height = pdf_text_item.word.Height * ActualHeight;
        }

        internal void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            text_layer_selection_mode = textLayerSelectionMode;
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFTextSentenceLayer()
        {
            Logging.Debug("~PDFTextSentenceLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFTextSentenceLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFTextSentenceLayer::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (null != drag_area_tracker)
                {
                    WPFDoEvents.InvokeInUIThread(() =>
                    {
                        try
                        {
                            foreach (var el in Children)
                            {
                                IDisposable node = el as IDisposable;
                                if (null != node)
                                {
                                    node.Dispose();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex);
                        }

                        try
                        {
                            ClearChildren();
                            Children.Clear();
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex);
                        }

                        drag_area_tracker.OnDragStarted -= drag_area_tracker_OnDragStarted;
                        drag_area_tracker.OnDragInProgress -= drag_area_tracker_OnDragInProgress;
                        drag_area_tracker.OnDragComplete -= drag_area_tracker_OnDragComplete;
                    }, Dispatcher);
                }

                // Clear the references for sanity's sake
                pdf_renderer_control_stats = null;
                drag_area_tracker = null;
                text_selection_manager = null;

                DataContext = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;

            //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
        }

        #endregion

    }
}
