using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Files;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    internal class IntranetLibraryTools
    {
        internal static string GetLibraryDetailPath(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"Qiqqa.LibraryDetail.txt"));
        }

        internal static string GetLibraryMetadataPath(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"Qiqqa.LibraryMetadata.s3db"));
        }

        internal static string GetLibraryDocumentsPath(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"documents"));
        }

        internal static string GetLibraryPDFPath(string base_path, string filename_short)
        {
            return Path.GetFullPath(Path.Combine(GetLibraryDocumentsPath(base_path), filename_short.Substring(0, 1).ToUpper(), filename_short));
        }

        internal static List<string> GetListOfDocumentsInLibrary(string base_path)
        {
            Logging.Info("Getting list of documents in Intranet Library {0}", base_path);
            string documents_path = GetLibraryDocumentsPath(base_path);
            List<string> results = DirectoryTools.GetSubFiles(documents_path, "pdf");
            Logging.Info("Got list of documents ({0}) in Intranet Library {1}", results.Count, base_path);
            return results;
        }

        internal static string GetLibraryAuditFilename(string base_path)
        {
            return Path.GetFullPath(Path.Combine(base_path, @"_audit", DateTime.UtcNow.ToString("yyyyMMdd") + @".txt"));
        }
    }
}
