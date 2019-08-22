using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Qiqqa.Main;
using Utilities;

namespace Qiqqa.Common
{
    class RestoreDesktopManager
    {
        public static void SaveDesktop()
        {
            List<string> restore_settings = new List<string>();

            // Get the remembrances
            List<FrameworkElement> framework_elements = MainWindowServiceDispatcher.Instance.MainWindow.DockingManager.GetAllFrameworkElements();
            foreach (FrameworkElement framework_element in framework_elements)
            {
                {
                    LibraryControl library_control = framework_element as LibraryControl;
                    if (null != library_control)
                    {
                        Logging.Info("Remembering a library control " + library_control.Library.WebLibraryDetail.Id);
                        restore_settings.Add(String.Format("PDF_LIBRARY,{0}", library_control.Library.WebLibraryDetail.Id));
                    }
                }

                {
                    PDFReadingControl pdf_reading_control = framework_element as PDFReadingControl;
                    if (null != pdf_reading_control)
                    {
                        Logging.Info("Remembering a PDF reader " + pdf_reading_control.PDFRendererControlStats.pdf_document.Fingerprint);
                        restore_settings.Add(String.Format("PDF_DOCUMENT,{0},{1}", pdf_reading_control.PDFRendererControlStats.pdf_document.Library.WebLibraryDetail.Id, pdf_reading_control.PDFRendererControlStats.pdf_document.Fingerprint));
                    }
                }
            }

            // Store the remembrances
            File.WriteAllLines(Filename, restore_settings);
        }

        public static void RestoreDesktop()
        {
            try
            {
                // Get the remembrances
                if (File.Exists(Filename))
                {
                    string[] restore_settings = File.ReadAllLines(Filename);
                    foreach (string restore_setting in restore_settings)
                    {
                        try
                        {
                            if (false) { }

                            else if (restore_setting.StartsWith("PDF_LIBRARY"))
                            {
                                string[] parts = restore_setting.Split(',');
                                string library_id = parts[1];

                                Library library = WebLibraryManager.Instance.GetLibrary(library_id);
                                MainWindowServiceDispatcher.Instance.OpenLibrary(library);
                            }
                            else if (restore_setting.StartsWith("PDF_DOCUMENT"))
                            {
                                string[] parts = restore_setting.Split(',');
                                string library_id = parts[1];
                                string document_fingerprint = parts[2];

                                Library library = WebLibraryManager.Instance.GetLibrary(library_id);
                                PDFDocument pdf_document = library.GetDocumentByFingerprint(document_fingerprint);
                                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                            }
                        }

                        catch (Exception ex)
                        {
                            Logging.Warn(ex, "There was an problem restoring desktop with state {0}", restore_setting);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem restoring the saved desktop state.");
            }
        }

        private static string Filename
        {
            get
            {
                return ConfigurationManager.Instance.BaseDirectoryForUser + @"\Qiqqa.restore_desktop";
            }
        }
    }
}
