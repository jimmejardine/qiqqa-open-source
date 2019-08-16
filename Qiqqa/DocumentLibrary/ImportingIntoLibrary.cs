using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Strings;

namespace Qiqqa.DocumentLibrary
{
    public class ImportingIntoLibrary
    {
        static ImportingIntoLibrary()
        {
            // This dodgy global hack allows SSL failures on the server to pass (i.e. if they have a dodgy certificate)
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
        
        static readonly string LIBRARY_DOWNLOAD = "LibraryDownload";

        // make sure all threads use the same report file and the alert box is only shown once
        internal static string problematic_import_documents_filename = null;
        internal static int problematic_import_documents_alert_showing = 0;
        internal static object problematic_import_documents_lock = new object();

        #region --- Add filenames ---------------------------------------------------------------------------------------------------------------------------

        public class FilenameWithMetadataImport
        {
            public string filename;
            public string bibtex;
            public string notes;
            public List<string> tags = new List<string>();

            public override string ToString()
            {
                return String.Format(
                    "---\r\n{0}\r\n{1}\r\n{2}\r\n{3}\r\n---"
                    ,filename
                    ,bibtex
                    ,notes
                    ,StringTools.ConcatenateStrings(tags, ';')
                );
            }
        }

        public static void AddNewPDFDocumentsToLibrary_ASYNCHRONOUS(Library library, bool suppress_notifications, bool suppress_signal_that_docs_have_changed, params string[] filenames)
        {
            SafeThreadPool.QueueUserWorkItem(o => AddNewPDFDocumentsToLibrary_SYNCHRONOUS(library, suppress_notifications, suppress_signal_that_docs_have_changed, filenames));
        }

        public static PDFDocument AddNewPDFDocumentsToLibrary_SYNCHRONOUS(Library library, bool suppress_notifications, bool suppress_signal_that_docs_have_changed, params string[] filenames)
        {
            FilenameWithMetadataImport[] filename_with_metadata_imports = new FilenameWithMetadataImport[filenames.Length];
            for (int i = 0; i < filenames.Length; ++i)
            {
                filename_with_metadata_imports[i] = new FilenameWithMetadataImport();
                filename_with_metadata_imports[i].filename = filenames[i];
            }

            return AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, suppress_notifications, suppress_signal_that_docs_have_changed, filename_with_metadata_imports);
        }

        public static void AddNewPDFDocumentsToLibraryWithMetadata_ASYNCHRONOUS(Library library, bool suppress_notifications, bool suppress_signal_that_docs_have_changed, FilenameWithMetadataImport[] filename_with_metadata_imports)
        {
            SafeThreadPool.QueueUserWorkItem(o => AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, suppress_notifications, suppress_signal_that_docs_have_changed, filename_with_metadata_imports));
        }

        public static PDFDocument AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(Library library, bool suppress_notifications, bool suppress_signal_that_docs_have_changed, FilenameWithMetadataImport[] filename_with_metadata_imports)
        {
            Stopwatch clk = new Stopwatch();
            clk.Start();

            // Notify if there is just a single doc
            suppress_notifications = suppress_notifications || (filename_with_metadata_imports.Length > 1);

            StatusManager.Instance.ClearCancelled("BulkLibraryDocument");

            PDFDocument last_added_pdf_document = null;
            
            int successful_additions = 0;
            for (int i = 0; i < filename_with_metadata_imports.Length; ++i)
            {
                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug("ImportingIntoLibrary: Breaking out of outer processing loop due to application termination");
                    break;
                }

                if (StatusManager.Instance.IsCancelled("BulkLibraryDocument"))
                {
                    Logging.Warn("User chose to stop bulk adding documents to the library");
                    break;
                }
                StatusManager.Instance.UpdateStatus("BulkLibraryDocument", String.Format("Adding document {0} of {1} to your library", i, filename_with_metadata_imports.Length), i, filename_with_metadata_imports.Length, true);

                FilenameWithMetadataImport filename_with_metadata_import = filename_with_metadata_imports[i];

                try
                {
                    string filename = filename_with_metadata_import.filename;
                    string bibtex = filename_with_metadata_import.bibtex;

                    // Although the outside world may allow us to be signalling, we will not do it unless we are the n-100th doc or the last doc
                    bool local_suppress_signal_that_docs_have_changed = suppress_signal_that_docs_have_changed;
                    if (!local_suppress_signal_that_docs_have_changed)
                    {
                        if ((i != filename_with_metadata_imports.Length - 1) && (0 != i % 100))
                        {
                            local_suppress_signal_that_docs_have_changed = true;
                        }
                    }

                    PDFDocument pdf_document = library.AddNewDocumentToLibrary_SYNCHRONOUS(filename, filename, filename, bibtex, filename_with_metadata_import.tags, filename_with_metadata_import.notes, suppress_notifications, local_suppress_signal_that_docs_have_changed);
                    if (null != pdf_document)
                    {
                        ++successful_additions;
                    }
                    last_added_pdf_document = pdf_document;
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem adding a document to the library:\n{0}", filename_with_metadata_import);

                    // if the problem report file doesn't exist yet, we have to create it:
                    lock (problematic_import_documents_lock)
                    {
                        if (null == problematic_import_documents_filename)
                        {
                            problematic_import_documents_filename = TempFile.GenerateTempFilename("qiqqa-import-problem-report.txt");
                        }
                    }

                    // then always append the entire report chunk at once as multiple threads MAY
                    // be appending to the report simultaneously!
                    lock (problematic_import_documents_lock)
                    {
                        File.AppendAllText(
                            problematic_import_documents_filename,
                            "The following files caused problems while being imported into Qiqqa:\r\n\r\n"
                        );

#if DEBUG
                        File.AppendAllText(
                            problematic_import_documents_filename,
                            String.Format(
                                "----------\r\n{0}\r\n{1}\r\n{2}\r\n----------\r\n"
                                , ex.Message
                                , ex.StackTrace
                                , filename_with_metadata_import
                            )
                        );
#else
                        File.AppendAllText(
                            problematic_import_documents_filename,
                            String.Format(
                                "----------\r\n{0}\r\n{1}\r\n----------\r\n"
                                , ex.Message
                                , filename_with_metadata_import
                            )
                        );
#endif
                    }
                }
            }

            if (filename_with_metadata_imports.Length > 0)
            {
                StatusManager.Instance.UpdateStatus("BulkLibraryDocument", String.Format("Added {0} of {1} document(s) to your library", successful_additions, filename_with_metadata_imports.Length));
            }
            else
            {
                StatusManager.Instance.ClearStatus("BulkLibraryDocument");
            }

            // If there have been some import problems, report them to the user.
            // However, we should not wait for the user response!
            lock (problematic_import_documents_lock)
            {
                if (null != problematic_import_documents_filename)
                {
                    problematic_import_documents_alert_showing++;

                    // only show a single alert, not a plethora of 'em!
                    if (1 == problematic_import_documents_alert_showing)
                    {
                        // once the user has ack'ed or nack'ed the message, that handler
                        // will RESET the 'showing' counter and the party can start all over again!
                        SafeThreadPool.QueueUserWorkItem(o => AlertUserAboutProblematicImports());
                    }
                }
            }

            Logging.Debug("AddNewPDFDocumentsToLibraryFromFolder_SYNCHRONOUS: time spent: {0} ms", clk.ElapsedMilliseconds);

            return last_added_pdf_document;
        }

#endregion

        internal static void AlertUserAboutProblematicImports()
        {
            bool do_view = MessageBoxes.AskErrorQuestion("There were problems with some of the documents you were trying to add to Qiqqa.  Do you want to see the problem details?", true);

            // do NOT spend a long time inside the lock!
            // hence we null the report file reference so that other threads can
            // create another report file and continue work while the user takes
            // a slow look at the old one...
            //
            // In short: take `Process.Start(...)` *outside* the lock!
            string report_filename = null;

            lock (problematic_import_documents_lock)
            {
                if (do_view)
                {
                    report_filename = problematic_import_documents_filename;
                }
                else
                {
                    File.Delete(problematic_import_documents_filename);
                }
                // reset:
                problematic_import_documents_filename = null;
                problematic_import_documents_alert_showing = 0;
            }

            if (null != report_filename)
            {
                Process.Start(report_filename);
            }
        }

#region --- Add from folder ----------------------------------------------------------------------------------------------------------------------- 

        public static void AddNewPDFDocumentsToLibraryFromFolder_SYNCHRONOUS(Library library, string root_folder, bool recurse_subfolders, bool import_tags_from_subfolder_names, bool suppress_notifications, bool suppress_signal_that_docs_have_changed)
        {
            //  build up the files list
            var file_list = new List<FilenameWithMetadataImport>();
            BuildFileListFromFolder(file_list, root_folder, null, recurse_subfolders, import_tags_from_subfolder_names);

            //  now import into the library
            Logging.Info(
                "About to import {0} from folder {1} [recurse_subfolders={2}][import_tags_from_subfolder_names={3}]",
                file_list.Count, root_folder, recurse_subfolders, import_tags_from_subfolder_names);
            AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(library, suppress_notifications, suppress_signal_that_docs_have_changed, file_list.ToArray());
        }

        public static void AddNewPDFDocumentsToLibraryFromFolder_ASYNCHRONOUS(Library library, string root_folder, bool recurse_subfolders, bool import_tags_from_subfolder_names, bool suppress_notifications, bool suppress_signal_that_docs_have_changed)
        {
            SafeThreadPool.QueueUserWorkItem(o => AddNewPDFDocumentsToLibraryFromFolder_SYNCHRONOUS(library, root_folder, recurse_subfolders, import_tags_from_subfolder_names, suppress_notifications, suppress_signal_that_docs_have_changed));
        }

        /// <summary>
        /// Build up the list of <code>FilenameWithMetadataImport</code>'s, including tags.  Recurse with all subfolders.
        /// </summary>
        private static void BuildFileListFromFolder(List<FilenameWithMetadataImport> file_list, string folder, List<string> tags, bool recurse_subfolders, bool import_tags_from_subfolder_names)
        {
            try
            {
                //  do for this folder
                foreach (var filename in Directory.GetFiles(folder, "*.pdf"))
                {
                    var filename_with_metadata_import = new FilenameWithMetadataImport
                                                            {
                                                                filename = filename,
                                                                tags = tags
                                                            };
                    file_list.Add(filename_with_metadata_import);

                    Logging.Debug("Registering file import {0} with tags {1}", filename, StringTools.ConcatenateStrings(tags));
                }

                //  onto the subfolders (if required)
                if (!recurse_subfolders) return;

                if (tags == null) tags = new List<string>();
                foreach (var subfolder in Directory.GetDirectories(folder))
                {
                    //  build up the new tags list (if required)
                    var subfolder_tags = new List<string>(tags);
                    if (import_tags_from_subfolder_names)
                    {
                        var directory_info = new DirectoryInfo(subfolder);
                        subfolder_tags.Add(directory_info.Name);
                    }

                    //  recurse
                    BuildFileListFromFolder(file_list, subfolder, subfolder_tags, true, import_tags_from_subfolder_names);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Unable to process folder {0} while importing files", folder);
            }
        }

#endregion

#region --- Add from internet ---------------------------------------------------------------------------------------------------------------------------

        public static void AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(Library library, object download_url)
        {
            SafeThreadPool.QueueUserWorkItem(o => AddNewDocumentToLibraryFromInternet_SYNCHRONOUS(library, download_url));
        }
        
        public static void AddNewDocumentToLibraryFromInternet_SYNCHRONOUS(Library library, object download_url_obj)
        {
            string download_url = (string)download_url_obj;

            StatusManager.Instance.UpdateStatus(LIBRARY_DOWNLOAD, String.Format("Downloading {0}", download_url));

            try
            {
                HttpWebRequest web_request = (HttpWebRequest)HttpWebRequest.Create(download_url);
                web_request.Proxy = ConfigurationManager.Instance.Proxy;
                web_request.Method = "GET";
                web_request.AllowAutoRedirect = true;

                using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
                {
                    if (HttpStatusCode.Redirect == web_response.StatusCode)
                    {
                        string redirect_url = web_response.Headers["Location"];
                    }
                    else
                    {
                        Stream response_stream = web_response.GetResponseStream();
                        string content_type = web_response.GetResponseHeader("Content-Type");
                        // See also: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Disposition
                        string content_disposition = web_response.GetResponseHeader("Content-Disposition");
                        string original_filename = null;
                        try
                        {
                            ContentDisposition contentDisposition = new ContentDisposition(content_disposition);
                            original_filename = contentDisposition.FileName;
                            //StringDictionary parameters = contentDisposition.Parameters;
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "AddNewDocumentToLibraryFromInternet: no Content-Disposition header received from {1}?\n  Headers:\n{0}", web_response.Headers, download_url);

                            // fallback: derive the filename from the URL:
                            original_filename = web_response.ResponseUri.LocalPath;
                        }
                        // extract the type from the Content-Type header value:
                        {
                            ContentType ct = new ContentType(content_type);
                            content_type = ct.MediaType.ToLower(CultureInfo.CurrentCulture);
                        }
                        
                        bool is_acceptable_content_type = false;
                        if (content_type.EndsWith("pdf")) is_acceptable_content_type = true;
                        if (content_type.StartsWith("application/octet-stream")) is_acceptable_content_type = true;

                        if (is_acceptable_content_type)
                        {
                            string filename = TempFile.GenerateTempFilename("pdf");
                            using (FileStream fs = File.OpenWrite(filename))
                            {
                                int total_bytes = StreamToFile.CopyStreamToStream(response_stream, fs);
                                Logging.Info("Saved {0} bytes to {1}", total_bytes, filename);
                                //fs.Close();    -- autoclosed by `using` statement
                            }

                            library.AddNewDocumentToLibrary_SYNCHRONOUS(filename, original_filename, download_url, null, null, null, false, false);
                            File.Delete(filename);
                        }
                        else
                        {
                            if (content_type.EndsWith("html"))
                            {
                                using (StreamReader sr = new StreamReader(response_stream))
                                {
                                    string html = sr.ReadToEnd();
                                    Logging.Warn("Got this HTML instead of a PDF: {0}", html);
                                }
                            }

                            MessageBoxes.Info("The document library supports only PDF files at the moment.  You are trying to download something of type {0}.", content_type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem adding the downloaded PDF to the library.");
            }

            StatusManager.Instance.UpdateStatus(LIBRARY_DOWNLOAD, String.Format("Downloaded {0}", download_url));
        }

#endregion

#region --- Add from another library ---------------------------------------------------------------------------------------------------------------------------

        public static void ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(PDFDocument existing_pdf_document, Library library, bool suppress_signal_that_docs_have_changed)
        {
            SafeThreadPool.QueueUserWorkItem(o => ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(existing_pdf_document, library, suppress_signal_that_docs_have_changed));
        }

        public static void ClonePDFDocumentsFromOtherLibrary_ASYNCHRONOUS(List<PDFDocument> existing_pdf_document, Library library)
        {
            SafeThreadPool.QueueUserWorkItem(o => ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(existing_pdf_document, library));
        }

        /// <summary>
        /// Creates a new <code>PDFDocument</code> in the given library, and creates a copy of all the metadata.
        /// </summary>
        public static void ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(List<PDFDocument> existing_pdf_documents, Library library)
        {
            for (int i = 0; i < existing_pdf_documents.Count; ++i)            
            {
                StatusManager.Instance.UpdateStatus("BulkLibraryDocument", String.Format("Adding document {0} of {1} to your library", i, existing_pdf_documents.Count), i, existing_pdf_documents.Count);

                PDFDocument existing_pdf_document = existing_pdf_documents[i];

                // Signal only the last doc
                bool suppress_signal_that_docs_have_changed = true;
                if (i == existing_pdf_documents.Count - 1) suppress_signal_that_docs_have_changed = false;

                ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(existing_pdf_document, library, suppress_signal_that_docs_have_changed);
            }

            library.NotifyLibraryThatDocumentListHasChangedExternally();

            StatusManager.Instance.UpdateStatus("BulkLibraryDocument", String.Format("Added {0} document(s) to your library", existing_pdf_documents.Count));
        }

        /// <summary>
        /// Creates a new <code>PDFDocument</code> in the given library, and creates a copy of all the metadata.
        /// </summary>
        public static PDFDocument ClonePDFDocumentsFromOtherLibrary_SYNCHRONOUS(PDFDocument existing_pdf_document, Library library, bool suppress_signal_that_docs_have_changed)
        {
            try
            {
                if (existing_pdf_document.Library == library)
                {
                    Logging.Debug("Trying to clone a pdf doc back into its own library, ignoring");
                    return null;
                }

                return library.CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS(existing_pdf_document, false, suppress_signal_that_docs_have_changed);
            }
            catch (Exception e)
            {
                Logging.Error(e, "Problem cloning PDF {0} from library {1} to library {2}",
                              existing_pdf_document.TitleCombined,
                              existing_pdf_document.Library.WebLibraryDetail.Title,
                              library.WebLibraryDetail.Title);
                return null;
            }
        }

#endregion
    }
}
