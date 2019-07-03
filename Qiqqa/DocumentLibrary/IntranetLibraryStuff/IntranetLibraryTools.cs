using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    class IntranetLibraryTools
    {
        internal static string GetLibraryDetailPath(string base_path)
        {
            return base_path + @"\Qiqqa.LibraryDetail.txt";
        }

        internal static string GetLibraryMetadataPath(string base_path)
        {
            return base_path + @"\Qiqqa.LibraryMetadata.s3db";
        }

        internal static string GetLibraryDocumentsPath(string base_path)
        {
            return base_path + @"\documents\";
        }

        internal static string GetLibraryPDFPath(string base_path, string filename_short)
        {
            string documents_path = GetLibraryDocumentsPath(base_path);
            string specific_documents_path = documents_path + filename_short.Substring(0, 1).ToUpper() + @"\";
            string document_path = specific_documents_path + filename_short;

            return document_path;
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
            return base_path + @"_audit\" + DateTime.UtcNow.ToString("yyyyMMdd") + ".txt";
        }
    }
}
