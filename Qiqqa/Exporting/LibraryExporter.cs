using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Exporting
{
    public static class LibraryExporter
    {
        public static void Export(Library library, List<PDFDocument> pdf_documents)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_Export);

            // Get the directory
            string initial_directory = null;
            if (null == initial_directory) initial_directory = Path.GetDirectoryName(ConfigurationManager.Instance.ConfigurationRecord.System_LastLibraryExportFolder);
            if (null == initial_directory) initial_directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            using (FolderBrowserDialog dlg = new FolderBrowserDialog
            {
                Description = "Please select the folder to which you wish to export your entire Qiqqa library.",
                SelectedPath = initial_directory,
                ShowNewFolderButton = true
            })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Remember the filename for next time
                    string base_path = dlg.SelectedPath;
                    ConfigurationManager.Instance.ConfigurationRecord.System_LastLibraryExportFolder = base_path;
                    ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.System_LastLibraryExportFolder);

                    SafeThreadPool.QueueUserWorkItem(o => Export(library, pdf_documents, base_path));
                }
            }
        }

        public static void Export(Library library, List<PDFDocument> pdf_documents, string base_path)
        {
            try
            {
                int MAX_STEPS = 8;

                // Build the directory structure
                StatusManager.Instance.ClearCancelled("LibraryExport");

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Starting library export", 0, MAX_STEPS, true);
                Directory.CreateDirectory(base_path);
                Dictionary<string, PDFDocumentExportItem> pdf_document_export_items = Export_Docs(library, pdf_documents, base_path);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting BibTeX", 1, MAX_STEPS, true);
                LibraryExporter_BibTeX.Export(library, pdf_documents, base_path, pdf_document_export_items, true);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting html", 2, MAX_STEPS, true);
                LibraryExporter_HTML.Export(library, base_path, pdf_document_export_items);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting tabs", 3, MAX_STEPS, true);
                LibraryExporter_Tabs.Export(library, pdf_documents, base_path, pdf_document_export_items);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting BibTeX tabs", 4, MAX_STEPS, true);
                LibraryExporter_BibTeXTabs.Export(library, pdf_documents, base_path, pdf_document_export_items);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting directories", 5, MAX_STEPS, true);
                LibraryExporter_Directories.Export(library, base_path, pdf_document_export_items);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting PDF content", 6, MAX_STEPS, true);
                LibraryExporter_PDFs.Export(library, base_path, pdf_document_export_items);

                if (StatusManager.Instance.IsCancelled("LibraryExport")) return;
                StatusManager.Instance.UpdateStatus("LibraryExport", "Exporting PDF text", 7, MAX_STEPS, true);
                LibraryExporter_PDFText.Export(library, base_path, pdf_document_export_items);

                StatusManager.Instance.UpdateStatus("LibraryExport", "Finished library export");
                Process.Start(base_path);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while exporting your library.");
            }
        }

        private static Dictionary<string, PDFDocumentExportItem> Export_Docs(Library library, List<PDFDocument> pdf_documents, string base_path)
        {
            // Where the original docs go
            string doc_base_path_original = Path.GetFullPath(Path.Combine(base_path, @"docs_original"));
            Directory.CreateDirectory(doc_base_path_original);

            // Where the modified docs go
            string doc_base_path = Path.GetFullPath(Path.Combine(base_path, @"docs"));
            Directory.CreateDirectory(doc_base_path);

            Dictionary<string, PDFDocumentExportItem> pdf_document_export_items = new Dictionary<string, PDFDocumentExportItem>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                try
                {
                    if (File.Exists(pdf_document.DocumentPath))
                    {
                        // The original docs
                        string filename_original = Path.GetFullPath(Path.Combine(doc_base_path_original, ExportingTools.MakeExportFilename(pdf_document)));
                        File.Copy(pdf_document.DocumentPath, filename_original, true);

                        // The modified docs
                        string filename = Path.GetFullPath(Path.Combine(doc_base_path, ExportingTools.MakeExportFilename(pdf_document)));
                        File.Copy(pdf_document.DocumentPath, filename, true);

                        // And the ledger entry
                        PDFDocumentExportItem item = new PDFDocumentExportItem();
                        item.pdf_document = pdf_document;
                        item.filename = filename;
                        pdf_document_export_items[item.pdf_document.Fingerprint] = item;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error copying file from {0}", pdf_document.DocumentPath);
                }
            }

            return pdf_document_export_items;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test1()
        {
            LibraryExporter_PDFs.Test();
        }
        
        public static void Test()
        {
            // Get the gust library
            Library library = Library.GuestInstance;
            Thread.Sleep(500);

            string base_path = @"C:\temp\qiqqalibexport";
            Export(library, library.PDFDocuments, base_path);
        }
#endif

        #endregion
    }
}
