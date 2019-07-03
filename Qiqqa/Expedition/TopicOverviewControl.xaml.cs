using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Common;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;

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
            public Library library;
            public int topic;
        }

        public TopicOverviewControl()
        {
            InitializeComponent();

            this.DataContextChanged += TopicOverviewControl_DataContextChanged;
        }

        void TopicOverviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PopulateDetail(false);
        }

        void PopulateDetail(bool detailed_mode)
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
            ExpeditionDataSource eds = tod.library.ExpeditionManager.ExpeditionDataSource;
            LDAAnalysis lda_analysis = tod.library.ExpeditionManager.ExpeditionDataSource.LDAAnalysis;

            // First the terms header
            {
                string header = eds.GetDescriptionForTopic(tod.topic);
                ObjHeader.Header = header;
                ObjHeader.ToolTip = header;
                ObjHeader.HeaderBackground = new SolidColorBrush(eds.Colours[tod.topic]);
            }

            // Then the docs
            {
                int NUM_DOCS = detailed_mode ? 50 : 10;

                for (int d = 0; d < NUM_DOCS && d < eds.docs.Count; ++d)
                {
                    PDFDocument pdf_document = tod.library.GetDocumentByFingerprint(eds.docs[lda_analysis.DensityOfDocsInTopicsSorted[tod.topic][d].doc]);

                    string doc_percentage = String.Format("{0:N0}%", 100 * lda_analysis.DensityOfDocsInTopicsSorted[tod.topic][d].prob);

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

        void button_more_Click(object sender, RoutedEventArgs e)
        {
            PopulateDetail(true);
        }

        void button_brainstorm_Click(object sender, RoutedEventArgs e)
        {
            AugmentedButton button_brainstorm = sender as AugmentedButton;
            TopicOverviewData tod = button_brainstorm.Tag as TopicOverviewData;
            MainWindowServiceDispatcher.Instance.ExploreTopicInBrainstorm(tod.library, tod.topic);
        }

        void TopicDocumentPressed_MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;
            if (null != PDFDocumentSelected)
            {
                PDFDocumentSelected(tag.pdf_document);
            }            
            e.Handled = true;
        }
    }
}
