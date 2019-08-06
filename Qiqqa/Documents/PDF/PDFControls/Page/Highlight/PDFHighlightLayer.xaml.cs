using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Highlight
{
    /// <summary>
    /// Interaction logic for PDFHighlightLayer.xaml
    /// </summary>
    public partial class PDFHighlightLayer : PageLayer
    {
        PDFRendererControlStats pdf_renderer_control_stats;
        int page;

        private DragAreaTracker drag_area_tracker;
        private TextSelectionManager text_selection_manager;
        bool toggled_deleting = false;

        private TextLayerSelectionMode text_layer_selection_mode;
        public int CurrentColourNumber { get; set; }

        public PDFHighlightLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            this.Background = Brushes.Transparent;

            this.Cursor = Cursors.Pen;

            drag_area_tracker = new DragAreaTracker(this, false);
            drag_area_tracker.OnDragStarted += drag_area_tracker_OnDragStarted;
            drag_area_tracker.OnDragInProgress += drag_area_tracker_OnDragInProgress;
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;

            text_selection_manager = new TextSelectionManager();

            this.SizeChanged += PDFHighlightLayer_SizeChanged;

            SetLeft(ObjHighlightRenderer, 0);
            SetTop(ObjHighlightRenderer, 0);

            this.text_layer_selection_mode = TextLayerSelectionMode.Sentence;
            this.CurrentColourNumber = 0;

            this.Loaded += PDFHighlightLayer_Loaded;
        }

        void PDFHighlightLayer_Loaded(object sender, RoutedEventArgs e)
        {
            ObjHighlightRenderer.RebuildVisual(pdf_renderer_control_stats.pdf_document, page);
        }

        public static bool IsLayerNeeded(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            return pdf_renderer_control_stats.pdf_document.Highlights.GetHighlightsForPage(page).Count > 0;
        }

        void PDFHighlightLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ObjHighlightRenderer.Width = this.ActualWidth;
            ObjHighlightRenderer.Height = this.ActualHeight;
        }

        internal override void Dispose()
        {
            Logging.Info("PDFHighlightLayer::Dispose()");

            pdf_renderer_control_stats = null;
            drag_area_tracker = null;
            text_selection_manager = null;
        }

        void drag_area_tracker_OnDragStarted(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_AddHighlight);

            WordList words = pdf_renderer_control_stats.pdf_document.PDFRenderer.GetOCRText(page);
            WordList selected_words = text_selection_manager.OnDragStarted(text_layer_selection_mode, words, this.ActualWidth, this.ActualHeight, button_left_pressed, button_right_pressed, mouse_down_point);

            // Decide if we are adding or removing highlights
            double mouse_down_left = mouse_down_point.X / this.ActualWidth;
            double mouse_down_top = mouse_down_point.Y / this.ActualHeight;

            // See if this click point is inside any current existing highlight
            toggled_deleting = false;
            foreach (PDFHighlight highlight in pdf_renderer_control_stats.pdf_document.Highlights.GetHighlightsForPage(page))
            {
                if (highlight.Contains(page, mouse_down_left, mouse_down_top))
                {
                    toggled_deleting = true;
                    break;
                }
            }
        }

        void drag_area_tracker_OnDragInProgress(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_move_point)
        {
            WordList selected_words = text_selection_manager.OnDragInProgress(button_left_pressed, button_right_pressed, mouse_down_point, mouse_move_point);            
            ProcessAndApplyHighlights(selected_words);
        }

        void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
        {
            WordList selected_words = text_selection_manager.OnDragInProgress(button_left_pressed, button_right_pressed, mouse_down_point, mouse_up_point);            
            ProcessAndApplyHighlights(selected_words);
        }

        private void ProcessAndApplyHighlights(WordList selected_words)
        {
            // Are we adding
            if (!toggled_deleting && 0 <= CurrentColourNumber)
            {
                foreach (Word word in selected_words)
                {
                    PDFHighlight pdf_highlight = new PDFHighlight(page, word, CurrentColourNumber);
                    pdf_renderer_control_stats.pdf_document.Highlights.AddUpdatedHighlight(pdf_highlight);
                }
            }

            // Or deleting?
            else
            {
                HashSet<PDFHighlight> highlight_list = pdf_renderer_control_stats.pdf_document.Highlights.GetHighlightsForPage(page);
                HashSet<PDFHighlight> highlights_to_delete = new HashSet<PDFHighlight>();
                foreach (Word word in selected_words)
                {
                    foreach (PDFHighlight pdf_highlight in highlight_list)
                    {
                        if (
                            false
                            || word.Contains(pdf_highlight.Left, pdf_highlight.Top)
                            || word.Contains(pdf_highlight.Left + pdf_highlight.Width, pdf_highlight.Top)
                            || word.Contains(pdf_highlight.Left + pdf_highlight.Width, pdf_highlight.Top + pdf_highlight.Height)
                            || word.Contains(pdf_highlight.Left, pdf_highlight.Top + pdf_highlight.Height)
                            )
                        {
                            highlights_to_delete.Add(pdf_highlight);
                        }
                    }
                }

                foreach (PDFHighlight pdf_highlight in highlights_to_delete)
                {
                    pdf_renderer_control_stats.pdf_document.Highlights.RemoveUpdatedHighlight(pdf_highlight);
                }
            }

            // Redraw
            {                
                ObjHighlightRenderer.RebuildVisual(pdf_renderer_control_stats.pdf_document, page);
            }
        }

        internal void RaiseHighlightChange(int colourNumber)
        {
            this.CurrentColourNumber = colourNumber;
        }
    }
}
