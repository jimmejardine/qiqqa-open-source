using System;
using Utilities;
using Utilities.BibTex.Parsing;

namespace Qiqqa.Documents.PDF.CitationManagerStuff.CitationFinding
{
    /**
     * This CitationFinder relies on the "patent_id" and "cites" field of the bibtex record.
     * It assumes that the "cites" field is a semicolon delimited list of patent IDs
     * It then searches for this docs patent_id in all other's cites...
     */
    class OmnipatentsCitationFinder
    {
        public static int FindCitations(PDFDocument pdf_document)
        {
            int total_found = 0;

            BibTexItem bibtex_item = pdf_document.BibTexItem;
            if (null != bibtex_item)
            {
                string patent_id = bibtex_item["patent_id"];
                if (!String.IsNullOrEmpty(patent_id))
                {
                    foreach (PDFDocument pdf_document_other in pdf_document.Library.PDFDocuments)
                    {
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

                        BibTexItem bibtex_item_other = pdf_document_other.BibTexItem;
                        if (null != bibtex_item_other)
                        {
                            string cites_other = bibtex_item_other["cites"];
                            if (!String.IsNullOrEmpty(cites_other))
                            {
                                if (cites_other.Contains(patent_id))
                                {
                                    pdf_document.PDFDocumentCitationManager.AddInboundCitation(pdf_document_other.Fingerprint);
                                    pdf_document_other.PDFDocumentCitationManager.AddOutboundCitation(pdf_document.Fingerprint);
                                    ++total_found;
                                }
                            }
                        }
                    }
                }
            }

            return total_found;
        }
    }
}
