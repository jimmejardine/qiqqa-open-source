using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.InCite
{
    /// <summary>
    /// Interaction logic for UsedReferencesControl.xaml
    /// </summary>
    public partial class UsedCitationsControl : UserControl
    {
        private List<UsedCitation> latest_used_citations = new List<UsedCitation>();
        private List<MissingCitation> latest_missing_citations = new List<MissingCitation>();

        public UsedCitationsControl()
        {
            InitializeComponent();

            ObjUsedCitationsCatalog.SelectionChanged += ObjUsedCitationsCatalog_SelectionChanged;
        }

        public void Refresh(Library primary_library)
        {
            SafeThreadPool.QueueUserWorkItem(o => Refresh_BACKGROUND(primary_library));
        }

        private class UsedCitation
        {
            public CitationCluster citation_cluster;
            public CitationItem citation_item;
            public PDFDocument pdf_document;
        }

        private class MissingCitation
        {
            public CitationCluster citation_cluster;
            public CitationItem citation_item;
        }

        private void Refresh_BACKGROUND(Library primary_library)
        {
            StatusManager.Instance.UpdateStatus("UsedCitations", "Finding used citations...", 0, 1);

            try
            {

                // Get the citations from Word
                List<CitationCluster> citation_clusters = WordConnector.Instance.GetAllCitationClustersFromCurrentDocument();
                Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord> bitex_items = CSLProcessorBibTeXFinder.Find(citation_clusters, primary_library);

                // Catalogue the citations
                List<UsedCitation> used_citations = new List<UsedCitation>();
                List<MissingCitation> missing_citations = new List<MissingCitation>();
                foreach (CitationCluster citation_cluster in citation_clusters)
                {
                    foreach (CitationItem citation_item in citation_cluster.citation_items)
                    {
                        CSLProcessorBibTeXFinder.MatchingBibTeXRecord bibtex_record = bitex_items[citation_item.reference_key];
                        if (null != bibtex_record)
                        {
                            PDFDocument pdf_document = bibtex_record.pdf_document;
                            used_citations.Add(new UsedCitation { citation_cluster = citation_cluster, citation_item = citation_item, pdf_document = pdf_document });
                        }
                        else
                        {
                            missing_citations.Add(new MissingCitation { citation_cluster = citation_cluster, citation_item = citation_item });
                        }
                    }
                }

                // Sort them by author
                used_citations.Sort(delegate (UsedCitation p1, UsedCitation p2)
                {
                    return String.Compare(p1.pdf_document.AuthorsCombined, p2.pdf_document.AuthorsCombined);
                });

                Dispatcher.Invoke(new Action(() =>
                {
                    // Store them for the GUI
                    latest_used_citations = used_citations;
                    latest_missing_citations = missing_citations;

                    // First set the used citations
                    List<PDFDocument> pdf_documents = new List<PDFDocument>();
                    used_citations.ForEach(o => pdf_documents.Add(o.pdf_document));
                    ObjUsedCitationsCatalog.SetPDFDocuments(pdf_documents, null);

                    // Then set the missing citations
                    ObjMissingCitationsList.Children.Clear();
                    foreach (MissingCitation missing_citation in missing_citations)
                    {
                        TextBlock text_doc = new TextBlock();
                        text_doc.Text = missing_citation.citation_item.reference_key;
                        text_doc.Tag = missing_citation;
                        text_doc.MouseDown += text_doc_MouseDown;
                        ListFormattingTools.AddGlowingHoverEffect(text_doc);

                        ObjMissingCitationsList.Children.Add(text_doc);
                    }

                    // Only show the missing area if we have missing items...
                    ObjGridMissingCitations.Visibility = (0 < missing_citations.Count) ? Visibility.Visible : Visibility.Collapsed;
                }));

            }

            catch (Exception ex)
            {
                Logging.Error(ex, "There was an exception while looking for used citations.");
                MessageBoxes.Error("There was a problem while looking for used citations.  Are you sure Word is running?");
            }

            StatusManager.Instance.UpdateStatus("UsedCitations", "Found used citations");
        }

        private void ObjUsedCitationsCatalog_SelectionChanged(List<PDFDocument> selected_pdf_documents)
        {
            try
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_ClickUsedReference);

                if (0 < selected_pdf_documents.Count)
                {
                    foreach (UsedCitation used_citation in latest_used_citations)
                    {
                        if (used_citation.pdf_document == selected_pdf_documents[0])
                        {
                            WordConnector.Instance.FindCitationCluster(used_citation.citation_cluster);
                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Error locating used citation.");
                MessageBoxes.Error("There was a problem locating the used citation.  Is Word still running?");
            }
        }

        private void text_doc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FeatureTrackingManager.Instance.UseFeature(Features.InCite_ClickMissingReference);

                TextBlock text_doc = (TextBlock)sender;
                MissingCitation missing_citation = (MissingCitation)text_doc.Tag;

                // Jump to the citation in Word
                WordConnector.Instance.FindCitationCluster(missing_citation.citation_cluster);

                e.Handled = true;

            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Error locating missing citation.");
                MessageBoxes.Error("There was a problem locating the missing citation.  Is Word still running?");
            }
        }
    }
}
