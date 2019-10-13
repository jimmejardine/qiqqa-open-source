using System;
using System.Collections.Generic;
using System.IO;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Main.LoginStuff;
using Utilities;
using Utilities.Files;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.Synchronisation.PDFSync
{
    class SyncQueues_Intranet
    {
        internal static void DaemonPut(Library library, string fingerprint)
        {
            string filename_full = PDFDocumentFileLocations.DocumentPath(library, fingerprint, "pdf");
            string filename_short = Path.GetFileName(filename_full);
            string pdf_path = IntranetLibraryTools.GetLibraryPDFPath(library.WebLibraryDetail.IntranetPath, filename_short);
            DirectoryTools.CreateDirectory(Path.GetDirectoryName(pdf_path));

            Logging.Info("+Copying up {0}", fingerprint);
            File.Copy(filename_full, pdf_path);
            Logging.Info("-Copying up {0}", fingerprint);
        }


        internal static void DaemonGet(Library library, string fingerprint)
        {
            string filename_full = PDFDocumentFileLocations.DocumentPath(library, fingerprint, "pdf");
            string filename_short = Path.GetFileName(filename_full);
            string pdf_path = IntranetLibraryTools.GetLibraryPDFPath(library.WebLibraryDetail.IntranetPath, filename_short);
            DirectoryTools.CreateDirectory(Path.GetDirectoryName(filename_full));

            Logging.Info("+Copying down {0}", fingerprint);
            File.Copy(pdf_path, filename_full);
            Logging.Info("-Copying down {0}", fingerprint);

            // Write the audit
            if (true)
            {
                string audit_filename = IntranetLibraryTools.GetLibraryAuditFilename(library.WebLibraryDetail.IntranetPath);
                string audit_directory = Path.GetDirectoryName(audit_filename);

                if (Directory.Exists(audit_directory))
                {
                    string audit_data = String.Format(
                        "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\r\n"
                        , DateTime.UtcNow.ToString("yyyyMMdd.hhmmss")
                        , ConfigurationManager.Instance.ConfigurationRecord.Account_Username
                        , ConfigurationManager.Instance.ConfigurationRecord.Account_Nickname
                        , Environment.UserName
                        , filename_short
                        , pdf_path
                    );

                    try
                    {
                        File.AppendAllText(audit_filename, audit_data);
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "Unable to write intranet sync audit data.");
                    }
                }
            }
        }

        internal static void QueueUploadOfMissingPDFs(Library library, List<PDFDocument> pdf_documents)
        {
            // Get a list of all the documents in the Intranet library
            List<string> existing_pdfs_full = IntranetLibraryTools.GetListOfDocumentsInLibrary(library.WebLibraryDetail.IntranetPath);

            HashSet<string> existing_pdfs = new HashSet<string>();
            foreach (string existing_pdf in existing_pdfs_full)
            {
                existing_pdfs.Add(Path.GetFileName(existing_pdf));
            }

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                bool deleted = pdf_document.Deleted;

                // Try to upload all files that we have
                if (!deleted && pdf_document.DocumentExists)
                {
                    string filename_full = PDFDocumentFileLocations.DocumentPath(library, pdf_document.Fingerprint, "pdf");
                    string filename_short = Path.GetFileName(filename_full);

                    if (!existing_pdfs.Contains(filename_short))
                    {
                        SyncQueues.Instance.QueuePut(pdf_document.Fingerprint, library);
                    }
                }
            }
        }
    }
}
