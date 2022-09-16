using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for ExpeditionPaperSimilarsControl.xaml
    /// </summary>
    public partial class ExpeditionPaperSimilarsControl : UserControl
    {
        public delegate void PDFDocumentSelectedDelegate(PDFDocument pdf_document);
        public event PDFDocumentSelectedDelegate PDFDocumentSelected;

        private int _NumberOfRelevantPapersToDisplay = 50;
        public int NumberOfRelevantPapersToDisplay
        {
            get => _NumberOfRelevantPapersToDisplay;
            set => _NumberOfRelevantPapersToDisplay = value;
        }

        private bool _ShowRelevancePercentage = true;
        public bool ShowRelevancePercentage
        {
            get => _ShowRelevancePercentage;
            set => _ShowRelevancePercentage = value;
        }

        public ExpeditionPaperSimilarsControl()
        {
            InitializeComponent();

            DataContextChanged += ExpeditionPaperSimilarsControl_DataContextChanged;
        }

        private void ExpeditionPaperSimilarsControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                // Clear the old
                ObjPapers.Children.Clear();
                TxtPleaseRunExpedition.Visibility = Visibility.Visible;

                AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
                if (null == pdf_document_bindable)
                {
                    return;
                }

                ASSERT.Test(this.IsHitTestVisible);

                PDFDocument pdf_document = pdf_document_bindable.Underlying;

                SafeThreadPool.QueueUserWorkItem(() =>
                {
                    List<ExpeditionPaperSuggestions.Result> results = ExpeditionPaperSuggestions.GetRelevantOthers(pdf_document, NumberOfRelevantPapersToDisplay);

                    WPFDoEvents.InvokeAsyncInUIThread(() =>
                    {
                        ASSERT.Test(this.IsHitTestVisible);

                        foreach (ExpeditionPaperSuggestions.Result result in results)
                        {
                            // Do we have specific event handling logic?
                            MouseButtonEventHandler mouse_down_event_handler = null;
                            if (null != PDFDocumentSelected)
                            {
                                mouse_down_event_handler = DocumentDocumentPressed_MouseButtonEventHandler;
                            }

                            string doc_percentage = String.Format("{0:N0}%", 100 * result.relevance);

                            bool alternator = false;
                            TextBlock text_doc =
                                ShowRelevancePercentage
                                    ? ListFormattingTools.GetDocumentTextBlock(result.pdf_document, ref alternator, Features.Expedition_TopicDocument, mouse_down_event_handler, doc_percentage + " - ")
                                    : ListFormattingTools.GetDocumentTextBlock(result.pdf_document, ref alternator, Features.Expedition_TopicDocument, mouse_down_event_handler, null);
                            ObjPapers.Children.Add(text_doc);
                        }

                        TxtPleaseRunExpedition.Visibility = Visibility.Collapsed;
                    });
                });
            });
        }

        private void DocumentDocumentPressed_MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_block = (TextBlock)sender;
            ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;
            PDFDocumentSelected?.Invoke(tag.pdf_document);
            e.Handled = true;
        }
    }
}
