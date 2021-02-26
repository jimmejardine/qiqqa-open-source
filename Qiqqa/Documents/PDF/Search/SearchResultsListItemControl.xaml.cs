using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.Search
{
    /// <summary>
    /// Interaction logic for SearchResultsListItemControl.xaml
    /// </summary>
    public partial class SearchResultsListItemControl : Grid
    {
        public SearchResultsListItemControl()
        {
            InitializeComponent();

            DataContextChanged += SearchResultsListItemControl_DataContextChanged;
        }

        private void SearchResultsListItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                TextContext.Inlines.Clear();

                // Check that we know what this DataContext is
                PDFSearchResult search_result = DataContext as PDFSearchResult;
                if (null == search_result)
                {
                    return;
                }

                string current_context_sentence = search_result.context_sentence;
                string current_context_sentence_lower = current_context_sentence.ToLower();

                while (!String.IsNullOrEmpty(current_context_sentence))
                {
                    // Look for the next keyword
                    int next_k = -1;
                    int next_keyword_pos = Int32.MaxValue;
                    for (int k = 0; k < search_result.keywords.Length; ++k)
                    {
                        int pos = current_context_sentence_lower.IndexOf(search_result.keywords[k]);
                        if (-1 < pos && pos < next_keyword_pos)
                        {
                            next_k = k;
                            next_keyword_pos = pos;
                        }
                    }

                    // If we have a keyword
                    if (-1 != next_k)
                    {
                        // Add a span of everything up to the keyword
                        {
                            Run run = new Run(current_context_sentence.Substring(0, next_keyword_pos));
                            TextContext.Inlines.Add(run);
                        }

                        {
                            Run run = new Run(current_context_sentence.Substring(next_keyword_pos, search_result.keywords[next_k].Length));
                            run.FontWeight = FontWeights.Bold;
                            run.Foreground = TextSearchBrushes.Instance.GetBrushPair(next_k).BorderBrush;
                            TextContext.Inlines.Add(run);
                        }

                        current_context_sentence = current_context_sentence.Substring(next_keyword_pos + search_result.keywords[next_k].Length);
                        current_context_sentence_lower = current_context_sentence.ToLower();
                    }

                    else
                    {
                        Run run = new Run(current_context_sentence);
                        TextContext.Inlines.Add(run);

                        current_context_sentence = "";
                        current_context_sentence_lower = "";
                    }
                }
            });
        }
    }
}
