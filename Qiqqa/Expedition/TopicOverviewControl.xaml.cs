using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Common;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Misc;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for TopicOverviewControl.xaml
    /// </summary>
    public partial class TopicOverviewControl : UserControl
    {
        public delegate void PDFDocumentSelectedDelegate(PDFDocument pdf_document);
        public event PDFDocumentSelectedDelegate PDFDocumentSelected;

        public class TopicOverviewData
        {
            public WebLibraryDetail web_library_detail;
            public int topic;
        }

        public TopicOverviewControl()
        {
            InitializeComponent();

            DataContextChanged += TopicOverviewControl_DataContextChanged;
        }

        private void TopicOverviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                PopulateDetail(false);
            });
        }

        private void PopulateDetail(bool detailed_mode)
        {
            // Clear the old
            ObjHeader.Header = null;
            ObjHeader.ToolTip = null;
            ObjPapers.Children.Clear();

            // Try to get the context
            TopicOverviewData tod = DataContext as TopicOverviewData;
            if (null == tod)
            {
                return;
            }

            // Quick refs
            ExpeditionDataSource eds = tod.web_library_detail.Xlibrary?.ExpeditionManager?.ExpeditionDataSource;

            if (null != eds)
            {
                LDAAnalysis lda_analysis = eds.LDAAnalysis;

                // First the terms header
                {
                    string header = eds.GetDescriptionForTopic(tod.topic);
                    ObjHeader.Header = header;
                    ObjHeader.ToolTip = header;
                    ObjHeader.HeaderBackground = new SolidColorBrush(eds.Colours[tod.topic]);
                }

                // Then the docs
                {
                    int NUM_DOCS = Math.Min(detailed_mode ? 50 : 10, lda_analysis.NUM_DOCS);

                    ASSERT.Test(tod.topic >= 0);
                    ASSERT.Test(tod.topic < lda_analysis.NUM_TOPICS);

                    for (int d = 0; d < NUM_DOCS && d < eds.docs.Count; ++d)
                    {
                        DocProbability[] docs = lda_analysis.DensityOfDocsInTopicsSorted[tod.topic];
                        ASSERT.Test(docs != null);
                        ASSERT.Test(docs.Length == lda_analysis.NUM_DOCS);
                        DocProbability lda_elem = docs[d];
                        ASSERT.Test(lda_elem != null);

                        PDFDocument pdf_document = tod.web_library_detail.Xlibrary.GetDocumentByFingerprint(eds.docs[lda_elem.doc]);

                        string doc_percentage = String.Format("{0:N0}%", 100 * lda_elem.prob);

                        bool alternator = false;
                        TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, Features.Expedition_TopicDocument, TopicDocumentPressed_MouseButtonEventHandler, doc_percentage + " - ");
                        ObjPapers.Children.Add(text_doc);
                    }

                    // The MORE button
                    if (!detailed_mode && NUM_DOCS < eds.docs.Count)
                    {
                        AugmentedButton button_more = new AugmentedButton();
                        button_more.Caption = "Show me more";
                        button_more.Click += button_more_Click;
                        ObjPapers.Children.Add(button_more);
                    }

                    // The BRAINSTORM button
                    {
                        AugmentedButton button_brainstorm = new AugmentedButton();
                        button_brainstorm.Caption = "Show me in Brainstorm";
                        button_brainstorm.Click += button_brainstorm_Click;
                        button_brainstorm.Tag = tod;
                        ObjPapers.Children.Add(button_brainstorm);
                    }
                }
            }
        }

        private void button_more_Click(object sender, RoutedEventArgs e)
        {
            PopulateDetail(true);
        }

        private void button_brainstorm_Click(object sender, RoutedEventArgs e)
        {
            AugmentedButton button_brainstorm = sender as AugmentedButton;
            TopicOverviewData tod = button_brainstorm.Tag as TopicOverviewData;
            MainWindowServiceDispatcher.Instance.ExploreTopicInBrainstorm(tod.web_library_detail, tod.topic);
        }

        private void TopicDocumentPressed_MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;
            PDFDocumentSelected?.Invoke(tag.pdf_document);
            e.Handled = true;
        }
    }
}
