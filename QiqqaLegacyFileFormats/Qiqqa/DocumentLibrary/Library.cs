using System;
using System.Collections.Generic;
using System.Diagnostics;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary
{

#if SAMPLE_LOAD_CODE

    public class Library
    {
        private LibraryDB library_db;
        public LibraryDB LibraryDB => library_db;

        private Dictionary<string, PDFDocument> pdf_documents = new Dictionary<string, PDFDocument>();
        private object pdf_documents_lock = new object();

        public Library(WebLibraryDetail _web_library_detail)
        {
            Logging.Info("Library basepath is at {0}", LIBRARY_BASE_PATH);
            Logging.Info("Library document basepath is at {0}", LIBRARY_DOCUMENTS_BASE_PATH);

            Directory.CreateDirectory(LIBRARY_BASE_PATH);
            Directory.CreateDirectory(LIBRARY_DOCUMENTS_BASE_PATH);

            library_db = new LibraryDB(LIBRARY_BASE_PATH);
        }

#region --- File locations ------------------------------------------------------------------------------------

        public static string GetLibraryBasePathForId(string id)
        {
            return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, id));
        }

        public string LIBRARY_BASE_PATH => GetLibraryBasePathForId("...WebLibraryDetail.Id");

        public string LIBRARY_DOCUMENTS_BASE_PATH
        {
            get
            {
                //...

                    return Path.GetFullPath(Path.Combine(LIBRARY_BASE_PATH, @"documents"));
            }
        }

        public string LIBRARY_INDEX_BASE_PATH => Path.GetFullPath(Path.Combine(LIBRARY_BASE_PATH, @"index"));

#endregion
    }

#endif

}
