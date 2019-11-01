using System;
using System.Collections.Generic;
using Utilities.Language.TextIndexing;

namespace Qiqqa.DocumentLibrary.DocumentLibraryIndex
{
    public static class LibrarySearcher
    {
        private static readonly List<IndexResult> EMPTY_LIST = new List<IndexResult>();
        private static readonly List<IndexPageResult> EMPTY_PAGE_LIST = new List<IndexPageResult>();

        public static List<IndexResult> FindAllFingerprintsMatchingQuery(Library library, string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return EMPTY_LIST;
            }

            return library.LibraryIndex.GetFingerprintsForQuery(query);
        }

        internal static List<IndexPageResult> FindAllPagesMatchingQuery(Library library, string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return EMPTY_PAGE_LIST;
            }

            return library.LibraryIndex.GetPagesForQuery(query);
        }
    }
}
