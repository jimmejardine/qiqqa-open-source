using System;
using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF.CitationManagerStuff.CitationFinding;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.CitationManagerStuff
{
    public static class CitationFinder
    {

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;

            int total_found = 0;
            
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                total_found += FindCitations(pdf_document);
            }

            Logging.Info("Found {0} in total!!!!!!!!!!!", total_found);
        }
#endif

        #endregion

        public static int FindCitations(Library library)
        {
            int total_found = 0;

            List<PDFDocument> pdf_documents = library.PDFDocuments;

            StatusManager.Instance.ClearCancelled("CitationLibraryFinder");
            for (int i = 0; i < pdf_documents.Count; ++i)
            {
                try
                {
                    if (StatusManager.Instance.IsCancelled("CitationLibraryFinder"))
                    {
                        break;
                    }

                    PDFDocument pdf_document = pdf_documents[i];
                    StatusManager.Instance.UpdateStatusBusy("CitationLibraryFinder", String.Format("Looking for new citations in your library ({0} found so far)", total_found), i, pdf_documents.Count, true);
                    total_found += FindCitations(pdf_document);
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem during citation finding while processing one of the library documents.");
                }
            }

            StatusManager.Instance.UpdateStatus("CitationLibraryFinder", String.Format("Found {0} new citations", total_found));

            return total_found;
        }

        internal static int FindCitations(PDFDocument pdf_document)
        {
            int total_found = 0;
            total_found += BruteForceCitationFinder.FindCitations(pdf_document);
            total_found += OmnipatentsCitationFinder.FindCitations(pdf_document);
            return total_found;
        }
    }
}
