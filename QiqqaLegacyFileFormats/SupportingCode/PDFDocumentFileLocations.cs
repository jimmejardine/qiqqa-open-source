using System;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF
{
    internal class PDFDocumentFileLocations
    {
        public static readonly string METADATA = "metadata";
        public static readonly string ANNOTATIONS = "annotations";
        public static readonly string HIGHLIGHTS = "highlights";
        public static readonly string INKS = "inks";
        public static readonly string CITATIONS = "citations";
    }
}
