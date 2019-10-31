using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.Documents.PDF.Search;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Animation;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Search
{
    /// <summary>
    /// Interaction logic for PDFSearchLayer.xaml
    /// </summary>
    public partial class PDFSearchLayer : PageLayer, IDisposable
    {
        PDFRendererControlStats pdf_renderer_control_stats;
        int page;

        PDFSearchResultSet search_result_set = null;

        public PDFSearchLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            Background = Brushes.Transparent;

            this.SizeChanged += PDFSearchLayer_SizeChanged;
        }

        void PDFSearchLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (PDFTextItem pdf_text_item in Children.OfType<PDFTextItem>())
            {
                ResizeTextItem(pdf_text_item);
            }
        }

        void ResizeTextItem(PDFTextItem pdf_text_item)
        {
            SetLeft(pdf_text_item, pdf_text_item.word.Left * ActualWidth);
            SetTop(pdf_text_item, pdf_text_item.word.Top * ActualHeight);
            pdf_text_item.Width = pdf_text_item.word.Width * ActualWidth;
            pdf_text_item.Height = pdf_text_item.word.Height * ActualHeight;
        }

        internal void SetSearchKeywords(PDFSearchResultSet search_result_set)
        {
            this.search_result_set = search_result_set;
            DoSearch();
        }

        internal override void PageTextAvailable()
        {
            DoSearch();
        }

        private void DoSearch()
        {
            PDFTextItemPool.Instance.RecyclePDFTextItemsFromChildren(Children);
            Children.Clear();

            if (null == search_result_set)
            {
                return;
            }

            foreach (PDFSearchResult search_result in search_result_set[this.page])
            {
                foreach (Word word in search_result.words)
                {
                    PDFTextItem pdf_text_item = PDFTextItemPool.Instance.GetPDFTextItem(word);
                    pdf_text_item.Tag = search_result;
                    pdf_text_item.SetAppearance(TextSearchBrushes.Instance.GetBrushPair(search_result.keyword_index));
                    ResizeTextItem(pdf_text_item);
                    Children.Add(pdf_text_item);
                }
            }
        }

        internal PDFSearchResult SetCurrentSearchPosition(PDFSearchResult previous_search_result_placeholder)
        {
            foreach (PDFTextItem pdf_text_item in Children.OfType<PDFTextItem>())
            {
                PDFSearchResult search_result_placeholder = pdf_text_item.Tag as PDFSearchResult;

                // If there was no previous search location, we use the first we find
                // If the last text item was the match position, use this next one                
                if (previous_search_result_placeholder == search_result_placeholder)
                {
                    pdf_renderer_control_stats.pdf_renderer_control.SelectPage(this.page);
                    pdf_text_item.BringIntoView();
                    pdf_text_item.Opacity = 0;
                    Animations.Fade(pdf_text_item, 0.1, 1);
                    return search_result_placeholder;
                }
            }

            return null;
        }

        internal PDFSearchResult SetNextSearchPosition(PDFSearchResult previous_search_result_placeholder)
        {
            bool have_found_last_search_item = false;
            foreach (PDFTextItem pdf_text_item in Children.OfType<PDFTextItem>())
            {
                PDFSearchResult search_result_placeholder = pdf_text_item.Tag as PDFSearchResult;

                // If there was no previous search location, we use the first we find
                // If the last text item was the match position, use this next one
                if (null == previous_search_result_placeholder || have_found_last_search_item && previous_search_result_placeholder != search_result_placeholder)
                {
                    pdf_renderer_control_stats.pdf_renderer_control.SelectPage(this.page);
                    pdf_text_item.BringIntoView();
                    pdf_text_item.Opacity = 0;
                    Animations.Fade(pdf_text_item, 0.1, 1);
                    return search_result_placeholder;
                }

                // If we have just found the last match, flag that the next one is the successor match
                if (previous_search_result_placeholder == search_result_placeholder)
                {
                    have_found_last_search_item = true;
                }
            }

            // We have not managed to find a search position if we get this far
            return null;
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFSearchLayer()
        {
            Logging.Debug("~PDFSearchLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFSearchLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFSearchLayer::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                WPFDoEvents.InvokeInUIThread(() =>
                {
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

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
                        Children.Clear();
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex);
                    }
                }, this.Dispatcher);
            }

            // Clear the references for sanity's sake
            pdf_renderer_control_stats = null;
            search_result_set = null;

            this.DataContext = null;

            ++dispose_count;

            //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
        }

        #endregion

    }
}
