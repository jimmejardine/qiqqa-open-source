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
