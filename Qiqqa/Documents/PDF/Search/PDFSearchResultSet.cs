using System;
using System.Collections.Generic;

namespace Qiqqa.Documents.PDF.Search
{
    public class PDFSearchResultSet
    {
        private Dictionary<int, List<PDFSearchResult>> search_results = new Dictionary<int, List<PDFSearchResult>>();

        private static readonly List<PDFSearchResult> EMPTY_LIST = new List<PDFSearchResult>();

        public List<PDFSearchResult> this[int index]
        {
            get
            {
                List<PDFSearchResult> results = null;
                search_results.TryGetValue(index, out results);
                return results ?? EMPTY_LIST;
            }

            set
            {
                count_cached = Int32.MinValue;
                search_results[index] = value;
            }
        }
        
        int count_cached = Int32.MinValue;
        public int Count
        {
            get
            {
                if (Int32.MinValue == count_cached)
                {
                    count_cached = 0;
                    foreach (var list in search_results.Values)
                    {
                        count_cached += list.Count;
                    }
                }

                return count_cached;
            }
        }

        public List<PDFSearchResult> AsList()
        {
            List<PDFSearchResult> results = new List<PDFSearchResult>();

            for (int page = 1; page <= search_results.Count; ++page)
            {
                results.AddRange(search_results[page]);
            }
            
            return results;
        }
    }
}
