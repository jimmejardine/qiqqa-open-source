using System;
using System.Collections.Generic;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.Language.TextIndexing;

namespace Qiqqa.DocumentLibrary.DocumentLibraryIndex
{
    public static class LibrarySearcher
    {
        private static readonly List<IndexResult> EMPTY_LIST = new List<IndexResult>();
        private static readonly List<IndexPageResult> EMPTY_PAGE_LIST = new List<IndexPageResult>();

        public static List<IndexResult> FindAllFingerprintsMatchingQuery(WebLibraryDetail web_library_detail, string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return EMPTY_LIST;
            }

            return web_library_detail.Xlibrary.LibraryIndex.GetFingerprintsForQuery(query);
        }

        internal static List<IndexPageResult> FindAllPagesMatchingQuery(WebLibraryDetail web_library_detail, string query)
        {
            if (String.IsNullOrEmpty(query))
            {
                return EMPTY_PAGE_LIST;
            }

            return web_library_detail.Xlibrary.LibraryIndex.GetPagesForQuery(query);
        }
    }
}
