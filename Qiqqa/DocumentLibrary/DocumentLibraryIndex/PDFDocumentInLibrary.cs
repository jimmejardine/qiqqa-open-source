using System;
using System.Collections.Generic;

namespace Qiqqa.DocumentLibraryIndex // DON'T CHANGE THIS NAMESPACE AS IT IS USED IN THE SERIALIZATION INFORMATION...
{
    [Serializable]
    internal class PDFDocumentInLibrary
    {
        public string fingerprint;

        public int total_pages;

        /// <summary>
        /// This will be null before any pages are processed and after all pages have been processed.  Otherwise it is a set of the pages that have been processed so far.
        /// </summary>
        public HashSet<int> pages_already_indexed;

        /// <summary>
        /// True if all pages have been processed
        /// </summary>
        public bool finished_indexing;

        /// <summary>
        /// Timestamp of when this was last indexed
        /// </summary>
        public DateTime last_indexed;

        /// <summary>
        /// Has the metadata been indexed - if no, then it will be reindexed...
        /// </summary>
        public bool metadata_already_indexed;
    }
}
