using System;
using System.Collections.Generic;
using System.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.SimilarAuthorsStuff;
using Qiqqa.Documents.PDF.InfoBarStuff.PDFDocumentTagCloudStuff;
using Utilities;
using Utilities.Collections;
using Utilities.Internet.GoogleScholar;
using Utilities.Language;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls
{
    internal class PDFRendererControlInterestingAnalysis
    {
        public static void DoInterestingAnalysis(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            pdf_reading_control.OnlineDatabaseLookupControl.PDFDocument = pdf_renderer_control_stats.pdf_document;

            Thread.Sleep(1000);

            SafeThreadPool.QueueUserWorkItem(o => DoInterestingAnalysis_DuplicatesAndCitations(pdf_reading_control, pdf_renderer_control, pdf_renderer_control_stats));
            // Only bother Google Scholar with a query when we want to:
            if (ConfigurationManager.IsEnabled(nameof(DoInterestingAnalysis_GoogleScholar)))
            {
                SafeThreadPool.QueueUserWorkItem(o => DoInterestingAnalysis_GoogleScholar(pdf_reading_control, pdf_renderer_control, pdf_renderer_control_stats));
            }
            SafeThreadPool.QueueUserWorkItem(o => DoInterestingAnalysis_TagCloud(pdf_reading_control, pdf_renderer_control, pdf_renderer_control_stats));
            SafeThreadPool.QueueUserWorkItem(o => DoInterestingAnalysis_SimilarAuthors(pdf_reading_control, pdf_renderer_control, pdf_renderer_control_stats));
        }

        private static void DoInterestingAnalysis_DuplicatesAndCitations(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            try
            {
                pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                {
                    pdf_reading_control.DuplicateDetectionControl.SetPDFDocument(pdf_renderer_control_stats.pdf_document);
                }
                ));
                pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                {
                    pdf_reading_control.CitationsControl.SetPDFDocument(pdf_renderer_control_stats.pdf_document);
                }
                ));
                pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                {
                    pdf_reading_control.LinkedDocumentsControl.SetPDFDocument(pdf_renderer_control_stats.pdf_document);
                }
                ));
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem with the citations analysis for document {0}", pdf_renderer_control_stats.pdf_document.Fingerprint);
            }
        }

        private static void DoInterestingAnalysis_GoogleScholar(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            bool attempt_scrape = Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.GoogleScholar_DoExtraBackgroundQueries;

            if (attempt_scrape)
            {
                Logging.Info("You are accessing Google Scholar in the background as you have the 'Send extra background queries to Google Scholar' Configuration option ticked. Be aware that this can lead to a quick denial of service response by Google in the form of a 40x HTTP Error or RECAPTCHA page instead of the search response you seek! Also refer to GitHub issues #225 & #113.");

                // Get the GoogleScholar similar documents
                try
                {
                    string title = pdf_renderer_control_stats.pdf_document.TitleCombined;
                    if (Constants.TITLE_UNKNOWN != title)
                    {
                        GoogleScholarScrapePaperSet gssp_set = GoogleScholarScrapePaperSet.GenerateFromQuery(title, 10);

                        pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                        {
                            pdf_reading_control.SimilarDocsControl.SpecifyPaperSet(gssp_set);
                        }
                        ));
                    }
                    else
                    {
                        Logging.Info("We don't have a title, so skipping GoogleScholar similar documents");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem getting the GoogleScholar similar documents for document {0}", pdf_renderer_control_stats.pdf_document.Fingerprint);
                }
            }
        }

        private static void DoInterestingAnalysis_TagCloud(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            // Populate the tag cloud
            try
            {
                List<TagCloudEntry> tag_cloud_entries = PDFDocumentTagCloudBuilder.BuildTagCloud(pdf_renderer_control_stats.pdf_document.Library, pdf_renderer_control_stats.pdf_document);

                pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                {
                    pdf_reading_control.TagCloud.SpecifyEntries(tag_cloud_entries);
                }
                ));
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem creating the tag cloud for document {0}", pdf_renderer_control_stats.pdf_document.Fingerprint);
            }
        }

        private static void DoInterestingAnalysis_SimilarAuthors(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            // Populate the similar authors
            try
            {
                List<NameTools.Name> authors = SimilarAuthors.GetAuthorsForPDFDocument(pdf_renderer_control_stats.pdf_document);
                MultiMap<string, PDFDocument> authors_documents = SimilarAuthors.GetDocumentsBySameAuthors(pdf_renderer_control_stats.pdf_document.Library, pdf_renderer_control_stats.pdf_document, authors);

                pdf_renderer_control.Dispatcher.Invoke(new Action(() =>
                {
                    pdf_reading_control.SimilarAuthorsControl.SetItems(authors_documents);
                }
                ));
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem creating the tag cloud for document {0}", pdf_renderer_control_stats.pdf_document.Fingerprint);
            }
        }
    }
}
