using System;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF
{
    internal class PDFDocumentFileLocations
    {
        public static readonly string METADATA = "metadata";
        public static readonly string ANNOTATIONS = "annotations";
        public static readonly string HIGHLIGHTS = "highlights";
        public static readonly string INKS = "inks";
        public static readonly string CITATIONS = "citations";


        internal static string DocumentBasePath(WebLibraryDetail web_library_detail, string fingerprint)
        {
            char folder_id = fingerprint[0];
            return Path.GetFullPath(Path.Combine(web_library_detail.LIBRARY_DOCUMENTS_BASE_PATH, String.Format(@"{0}", folder_id)));
        }

        /// <summary>
        /// Do not include the initial . in the filetype
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <param name="file_type"></param>
        /// <returns></returns>
        internal static string DocumentPath(WebLibraryDetail web_library_detail, string fingerprint, string file_type)
        {
            return Path.GetFullPath(Path.Combine(DocumentBasePath(web_library_detail, fingerprint), String.Format(@"{0}.{1}", fingerprint, file_type)));
        }
    }
}
