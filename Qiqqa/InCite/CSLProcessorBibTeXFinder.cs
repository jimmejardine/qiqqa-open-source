using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities.BibTex.Parsing;

namespace Qiqqa.InCite
{
    internal class CSLProcessorBibTeXFinder
    {
        internal class MatchingBibTeXRecord
        {
            public PDFDocument pdf_document;
            public BibTexItem bibtex_item;
        }

        /**
         * Returns a map from citation reference_key -> PDFDocument
         */
        internal static Dictionary<string, MatchingBibTeXRecord> Find(List<CitationCluster> citation_clusters, Library primary_library)
        {
            Dictionary<string, MatchingBibTeXRecord> bitex_items = new Dictionary<string, MatchingBibTeXRecord>();

            foreach (CitationCluster citation_cluster in citation_clusters)
            {
                foreach (CitationItem citation_item in citation_cluster.citation_items)
                {
                    if (!bitex_items.ContainsKey(citation_item.reference_key))
                    {
                        MatchingBibTeXRecord bitex_item = LocateBibTexForCitationItem(citation_item.reference_key, primary_library);
                        bitex_items[citation_item.reference_key] = bitex_item;
                    }
                }
            }

            return bitex_items;
        }


        internal static MatchingBibTeXRecord LocateBibTexForCitationItem(string reference_key, Library primary_library)
        {
            // First try in the preferred library
            if (null != primary_library)
            {
                MatchingBibTeXRecord item = LocateBibTexForCitationItem_FOCUS(reference_key, primary_library);
                if (null != item) return item;
            }

            // Failing that, look in all libraries
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All)
            {
                // Don't repeat a search of the preferred library
                if (primary_library != web_library_detail.library)
                {
                    MatchingBibTeXRecord item = LocateBibTexForCitationItem_FOCUS(reference_key, web_library_detail.library);
                    if (null != item) return item;
                }
            }

            return null;
        }

        private static MatchingBibTeXRecord LocateBibTexForCitationItem_FOCUS(string reference_key, Library library)
        {
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                BibTexItem bibtex_item = pdf_document.BibTexItem;
                if (null != bibtex_item)
                {
                    if (bibtex_item.Key == reference_key)
                    {
                        return new MatchingBibTeXRecord { pdf_document = pdf_document, bibtex_item = bibtex_item };
                    }
                }
            }

            return null;
        }
    }
}
