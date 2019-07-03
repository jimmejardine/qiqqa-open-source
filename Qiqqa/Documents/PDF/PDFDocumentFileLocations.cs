using System;
using Qiqqa.DocumentLibrary;

namespace Qiqqa.Documents.PDF
{
    class PDFDocumentFileLocations
    {
        public static readonly string METADATA = "metadata";
        public static readonly string ANNOTATIONS = "annotations";
        public static readonly string HIGHLIGHTS = "highlights";
        public static readonly string INKS = "inks";
        public static readonly string CITATIONS = "citations";


        internal static string DocumentBasePath(Library library, string fingerprint)
        {
            char folder_id = fingerprint[0];
            return library.LIBRARY_DOCUMENTS_BASE_PATH + String.Format(@"{0}\", folder_id);
        }

        /// <summary>
        /// Do not include the initial . in the filetype
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <param name="file_type"></param>
        /// <returns></returns>
        internal static string DocumentPath(Library library, string fingerprint, string file_type)
        {
            return DocumentBasePath(library, fingerprint) + String.Format(@"{0}.{1}", fingerprint, file_type);
        }
    }
}
