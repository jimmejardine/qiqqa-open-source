using Qiqqa.Documents.PDF.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Language;
using Utilities.Strings;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    class BibTeXGoodnessOfFitEstimator
    {
        public static bool DoesBibTeXMatchDocument(string bibtex, PDFDocument pdf_document, out PDFSearchResultSet search_result_set)
        {
            try
            {
                if (!String.IsNullOrEmpty(bibtex))
                {
                    BibTexItem bibtex_item = BibTexParser.ParseOne(bibtex, true);

                    return DoesBibTeXMatchDocument(bibtex_item, pdf_document, out search_result_set);
                }
            }
            catch (Exception) { }

            search_result_set = new PDFSearchResultSet();
            return false;
        }

        public static bool DoesBibTeXMatchDocument(BibTexItem bibtex_item, PDFDocument pdf_document, out PDFSearchResultSet search_result_set)
        {
            try
            {
                string authors_string = BibTexTools.GetAuthor(bibtex_item);
                if (!String.IsNullOrEmpty(authors_string))
                {
                    List<NameTools.Name> names = NameTools.SplitAuthors(authors_string, PDFDocument.UNKNOWN_AUTHORS);
                    StringBuilder sb = new StringBuilder();
                    foreach (NameTools.Name name in names)
                    {
                        sb.AppendFormat("\"{0}\" ", name.last_name);
                    }

                    string names_search_string = sb.ToString();
                    if (!String.IsNullOrEmpty(names_search_string))
                    {
                        search_result_set = PDFSearcher.Search(pdf_document, 1, names_search_string, PDFSearcher.MATCH_CONTAINS);
                        if (0 < search_result_set.Count)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception) { }

            search_result_set = new PDFSearchResultSet();
            return false;
        }
    }
}
