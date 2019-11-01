using System;
using System.Collections.Generic;
using System.Text;
using Qiqqa.Documents.PDF.Search;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using Utilities.Language;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    internal class BibTeXGoodnessOfFitEstimator
    {
        public static bool DoesBibTeXMatchDocument(BibTexItem bibtex_item, PDFDocument pdf_document, out PDFSearchResultSet search_result_set)
        {
            if (bibtex_item != null)
            {
                try
                {
                    string authors_string = bibtex_item.GetAuthor();
                    if (!String.IsNullOrEmpty(authors_string))
                    {
                        List<NameTools.Name> names = NameTools.SplitAuthors(authors_string);
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
                catch (Exception ex)
                {
                    Logging.Error(ex);
                }
            }

            search_result_set = new PDFSearchResultSet();
            return false;
        }
    }
}
