using System;
using System.Collections.Generic;
using System.Text;
using Qiqqa.DocumentLibrary.DocumentLibraryIndex;
using Qiqqa.DocumentLibrary.SimilarAuthorsStuff;
using Utilities;
using Utilities.Language;
using Utilities.Language.TextIndexing;
using Utilities.Misc;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.CitationManagerStuff.CitationFinding
{
    /**
     * This CitationFinder looks at the authors of this doc.  
     * It then finds all docs in the index with those author names.  
     * It then searches those docs for the title of this doc.
     */
    internal class BruteForceCitationFinder
    {
        private static string GenerateTextOnlyTitle(string source)
        {
            if (null == source)
            {
                return null;
            }

            source = source.ToLower();

            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (Char.IsLetter(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static int FindCitations(PDFDocument pdf_document)
        {
            int total_found = 0;

            string target_title = GenerateTextOnlyTitle(pdf_document.TitleCombined);

            List<NameTools.Name> names = SimilarAuthors.GetAuthorsForPDFDocument(pdf_document);
            if (0 < names.Count)
            {
                // Look for all other docs that mention this author
                string author_query = null;
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < 3 && i < names.Count; ++i)
                    {
                        sb.AppendFormat("+{0} ", names[i].last_name);
                    }
                    author_query = sb.ToString();
                }

                StatusManager.Instance.UpdateStatusBusy("CitationDocumentFinder", String.Format("Looking for new citations in \"{0}\" by \"{1}\"", pdf_document.TitleCombined, author_query));

                List<IndexPageResult> index_page_results_with_author = LibrarySearcher.FindAllPagesMatchingQuery(pdf_document.Library, author_query);
                Logging.Info("  **** We have {0} documents matching {1}", index_page_results_with_author.Count, author_query);
                foreach (var index_page_result in index_page_results_with_author)
                {
                    try
                    {
                        string fingerprint = index_page_result.fingerprint;

                        // Check that the other one exists
                        PDFDocument pdf_document_other = pdf_document.Library.GetDocumentByFingerprint(fingerprint);
                        if (null == pdf_document_other || !pdf_document_other.DocumentExists)
                        {
                            continue;
                        }

                        // Let's not work on the same document
                        if (pdf_document.Fingerprint == pdf_document_other.Fingerprint)
                        {
                            continue;
                        }

                        // Lets not do work that has already been done before...
                        {
                            bool already_found = true;
                            already_found = already_found && pdf_document.PDFDocumentCitationManager.ContainsInboundCitation(pdf_document_other.Fingerprint);
                            already_found = already_found && pdf_document_other.PDFDocumentCitationManager.ContainsOutboundCitation(pdf_document.Fingerprint);
                            if (already_found)
                            {
                                Logging.Info("Skipping check for citation from {0} to {1} because we know it already.", pdf_document_other.Fingerprint, pdf_document.Fingerprint);
                                continue;
                            }
                        }

                        // Now search each page for the title of the paper
                        foreach (PageResult page_result in index_page_result.page_results)
                        {
                            // Don't process the metadata "page"
                            if (0 == page_result.page)
                            {
                                continue;
                            }

                            WordList word_list_page = pdf_document_other.PDFRenderer.GetOCRText(page_result.page);
                            if (null != word_list_page)
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (var word in word_list_page)
                                {
                                    sb.Append(word.Text);
                                }
                                string text_to_search_for_title = GenerateTextOnlyTitle(sb.ToString());

                                // If we have a match, record it!
                                if (text_to_search_for_title.Contains(target_title))
                                {
                                    pdf_document.PDFDocumentCitationManager.AddInboundCitation(pdf_document_other.Fingerprint);
                                    pdf_document_other.PDFDocumentCitationManager.AddOutboundCitation(pdf_document.Fingerprint);
                                    ++total_found;

                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was a problem during citation finding while processing one of the matching author documents.");
                    }
                }
            }

            StatusManager.Instance.UpdateStatus("CitationDocumentFinder", String.Format("Found {0} new citations of '{1}'", total_found, pdf_document.TitleCombined));

            return total_found;
        }


    }
}
