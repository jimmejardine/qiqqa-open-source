using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities.Collections;

namespace Qiqqa.DocumentLibrary.SimilarAuthorsStuff
{
    /// <summary>
    /// Interaction logic for SimilarAuthorsControl.xaml
    /// </summary>
    public partial class SimilarAuthorsControl : UserControl
    {
        public SimilarAuthorsControl()
        {
            InitializeComponent();
        }

        public void SetItems(MultiMap<string, PDFDocument> items)
        {
            // Clear our items
            PanelItems.Children.Clear();

            bool alternator = false;

            foreach (string author in items.Keys)
            {
                TextBlock text_author = new TextBlock();
                text_author.ToolTip = text_author.Text = author;

                text_author.FontWeight = FontWeights.Bold;
                PanelItems.Children.Add(text_author);

                List<PDFDocument> pdf_documents_sorted = new List<PDFDocument>(items.Get(author).OrderBy(x => x.YearCombined));
                foreach (PDFDocument pdf_document in pdf_documents_sorted)
                {
                    TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, Features.SimilarAuthor_OpenDoc);
                    PanelItems.Children.Add(text_doc);
                }
            }

            if (0 == PanelItems.Children.Count)
            {
                TextBlock text_doc = new TextBlock();
                text_doc.Text = "None in this library.";
                PanelItems.Children.Add(text_doc);
            }
        }
    }
}
