using System;
using System.Collections.Generic;
using System.Text;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary
{
    public class LibraryDB
    {

#if SAMPLE_LOAD_CODE

        private string base_path;
        private string library_path;

        public LibraryDB(string base_path)
        {
            this.base_path = base_path;
            library_path = LibraryDB.GetLibraryDBPath(base_path);
        }

        internal static string GetLibraryDBPath(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"Qiqqa.library"));
        }

        internal static string GetLibraryDBTemplatePath()
        {
            return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"DocumentLibrary/Library.Template.s3db"));
        }

#endif
        
        public class LibraryItem
        {
            public string fingerprint;
            public string extension;
            public byte[] data;
            public string md5;

            public override string ToString()
            {
                return string.Format("{0}.{1}", fingerprint, extension);
            }

            internal string ToFileNameFormat()
            {
                return string.Format("{0}.{1}", fingerprint, extension);
            }
        }
    }
}
