using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
    public partial class PDFTextSentenceLayer : PageLayer
    {
        PDFRendererControlStats pdf_renderer_control_stats;
        int page;

        private DragAreaTracker drag_area_tracker;
        private TextLayerSelectionMode text_layer_selection_mode;
        TextSelectionManager text_selection_manager;

        public PDFTextSentenceLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            this.Focusable = true;            
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Once);

            this.Background = Brushes.Transparent;
            this.Cursor = Cursors.IBeam;

            this.SizeChanged += PDFTextSentenceLayer_SizeChanged;

            drag_area_tracker = new DragAreaTracker(this, false);
            drag_area_tracker.OnDragStarted += drag_area_tracker_OnDragStarted;
            drag_area_tracker.OnDragInProgress += drag_area_tracker_OnDragInProgress;
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;

            text_selection_manager = new TextSelectionManager();

            this.PreviewMouseDown += PDFTextSentenceLayer_PreviewMouseDown;
            this.PreviewKeyDown += PDFTextLayer_PreviewKeyDown;
            this.RequestBringIntoView += PDFTextSentenceLayer_RequestBringIntoView;

            this.text_layer_selection_mode = TextLayerSelectionMode.Sentence;
        }

        void PDFTextSentenceLayer_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // We dont want this control to be scrolled into view when it gets keyboard focus...
            e.Handled = true;
        }

        void PDFTextSentenceLayer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
        }

        void PDFTextLayer_PreviewKeyDown(object sender, KeyEventArgs e)
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
        
        internal override void Dispose()
        {
            ClearChildren();
        }

        internal override void DeselectLayer()
        {
            ClearChildren();
        }

        void ClearChildren()
        {
            PDFTextItemPool.Instance.RecyclePDFTextItemsFromChildren(Children);
        }

        void drag_area_tracker_OnDragStarted(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point)
        {
            WordList words = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
            WordList selected_words = text_selection_manager.OnDragStarted(text_layer_selection_mode, words, this.ActualWidth, this.ActualHeight, button_left_pressed, button_right_pressed, mouse_down_point);
        }

        void drag_area_tracker_OnDragInProgress(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_move_point)
        {
            if (button_left_pressed)
            {
                WordList selected_words = text_selection_manager.OnDragInProgress(button_left_pressed, button_right_pressed, mouse_down_point, mouse_move_point);
                ReflectWordList(selected_words);
            }
        }

        void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
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

                this.pdf_renderer_control_stats.pdf_renderer_control.OnTextSelected(selected_text);
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

        void PDFTextSentenceLayer_SizeChanged(object sender, SizeChangedEventArgs e)
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

        void BuildWords()
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

        void ResizeTextItem(PDFTextItem pdf_text_item)
        {
            SetLeft(pdf_text_item, pdf_text_item.word.Left * ActualWidth);
            SetTop(pdf_text_item, pdf_text_item.word.Top * ActualHeight);
            pdf_text_item.Width = pdf_text_item.word.Width * ActualWidth;
            pdf_text_item.Height = pdf_text_item.word.Height * ActualHeight;
        }

        internal void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            this.text_layer_selection_mode = textLayerSelectionMode;
        }
    }
}
