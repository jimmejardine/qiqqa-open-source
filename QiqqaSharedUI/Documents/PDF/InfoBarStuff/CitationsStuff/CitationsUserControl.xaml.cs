using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Documents.PDF.InfoBarStuff.CitationsStuff
{
    /// <summary>
    /// Interaction logic for CitationsUserControl.xaml
    /// </summary>
    public partial class CitationsUserControl : UserControl
    {
        private PDFDocument pdf_document;

        public CitationsUserControl()
        {
            InitializeComponent();

            ImageRefresh.Source = Icons.GetAppIcon(Icons.Refresh);
            ImageRefresh.Cursor = Cursors.Hand;
            ImageRefresh.ToolTip = "Generate all the cross references from the papers that cite this paper.\nRemember that you need to have filled in your BibTeX records (or at least your paper titles and authors) for this to work well.";
            ImageRefresh.MouseUp += ImageRefresh_MouseUp;
        }

        internal static void PopulatePanelWithCitations(StackPanel panel, Library library, PDFDocument pdf_document_parent, List<Citation> citations, Feature feature, string prefix = "", bool should_add_none_indicator = true)
        {
            string fingerprint_parent = pdf_document_parent.Fingerprint;

            panel.Children.Clear();

            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            foreach (var citation in citations)
            {
                string fingerprint = null;
                if (0 != fingerprint_parent.CompareTo(citation.fingerprint_inbound)) fingerprint = citation.fingerprint_inbound;
                if (0 != fingerprint_parent.CompareTo(citation.fingerprint_outbound)) fingerprint = citation.fingerprint_outbound;

                if (null == fingerprint)
                {
                    continue;
                }

                PDFDocument pdf_document = library.GetDocumentByFingerprint(fingerprint);

                if (null == pdf_document)
                {
                    continue;
                }

                if (pdf_document.Deleted)
                {
                    continue;
                }

                pdf_documents.Add(pdf_document);
            }

            List<PDFDocument> pdf_documents_sorted = new List<PDFDocument>(pdf_documents.OrderBy(x => x.YearCombined));
            bool alternator = false;
            foreach (PDFDocument pdf_document in pdf_documents_sorted)
            {
                TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, Features.Citations_OpenDoc, null, prefix);
                panel.Children.Add(text_doc);
            }


            // If the panel is empty, put NONE
            if (should_add_none_indicator)
            {
                if (0 == panel.Children.Count)
                {
                    TextBlock text_doc = new TextBlock();
                    text_doc.Text = "(none)";
                    panel.Children.Add(text_doc);
                }
            }
        }

        public void SetPDFDocument(PDFDocument doc)
        {
            pdf_document = doc;
            RepopulatePanels();
        }

        private void RepopulatePanels()
        {
            PopulatePanelWithCitations(DocsPanel_Outbound, pdf_document.Library, pdf_document, pdf_document.PDFDocumentCitationManager.GetOutboundCitations(), Features.Citations_OpenDoc);
            PopulatePanelWithCitations(DocsPanel_Inbound, pdf_document.Library, pdf_document, pdf_document.PDFDocumentCitationManager.GetInboundCitations(), Features.Citations_OpenDoc);
        }

        public void ImageRefresh_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_FindAllCitations);
            CitationFinder.FindCitations(pdf_document);
            RepopulatePanels();
        }
    }
}
