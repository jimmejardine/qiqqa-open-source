using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF
{
    internal class LockObject : object
    {
    }

    public class PDFDocument
    {
        private LockObject access_lock;

        private PDFDocument_ThreadUnsafe doc;

        public string GetAttributesAsJSON()
        {
            lock (access_lock)
            {
                return doc.GetAttributesAsJSON();
            }
        }

        //...
    }
}
