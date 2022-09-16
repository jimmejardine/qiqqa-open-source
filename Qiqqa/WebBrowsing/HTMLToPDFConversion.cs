using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.MessageBoxControls;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.ProcessTools;

namespace Qiqqa.WebBrowsing
{
    internal class HTMLToPDFConversion
    {
        internal static void GrabWebPage(string title, string url)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Web_ExportToPDF);

            GrabWebPage_REMOTE(title, url, true);
        }

        internal static void GrabWebPage_REMOTE(string title, string url, bool may_try_again_on_exception)
        {
            StatusManager.Instance.UpdateStatus("HTMLToPDF", "Converting HTML to PDF");

            // Strip off the trailing # cos Web2PDF hates is
            if (url.Contains('#'))
            {
                string old_url = url;
                url = url.Substring(0, url.IndexOf('#'));
                Logging.Info("Stripping the # off the original, from '{0}' to '{1}'", old_url, url);
            }

            string filename = Path.GetTempFileName() + ".pdf";

            try
            {
                // Spawn the conversion process
                {
                    string user_override_global = "";
                    string user_override_page = "";

                    string process_parameters = String.Format(
                        "{0} {1} --footer-right \"Page [page] of [topage]\" {2} --footer-left \"{3}\" --header-left \"{4}\" --header-right \"Created using www.qiqqa.com\" \"{5}\""
                        , user_override_global
                        , url
                        , user_override_page
                        , url.Replace('"', '\'')
                        , title.Replace('"', '\'')
                        , filename
                    );

                    // STDOUT/STDERR
                    using (Process process = ProcessSpawning.SpawnChildProcess(ConfigurationManager.Instance.ProgramHTMLToPDF, process_parameters, ProcessPriorityClass.Normal))
                    {
                        using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                        {
                            process.WaitForExit();

                            Logging.Info("HTMLToPDF:\n{0}", process_output_reader.GetOutputsDumpString());
                        }
                    }
                }

                StatusManager.Instance.UpdateStatus("HTMLToPDF", "Converting HTML to PDF: adding to library");
                PDFDocument pdf_document = Library.GuestInstance.Xlibrary.AddNewDocumentToLibrary_SYNCHRONOUS(filename, Library.GuestInstance, url, url, null, null, null, true, true);
                pdf_document.Title = title;
                pdf_document.Year = Convert.ToString(DateTime.Now.Year);
                pdf_document.DownloadLocation = url;

                WPFDoEvents.InvokeInUIThread(() =>
                    {
                        PDFReadingControl pdf_reading_control = MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                        pdf_reading_control.EnableGuestMoveNotification();
                    },
                    priority: DispatcherPriority.Background
                );
            }
            catch (Exception ex)
            {
                // Give it a 2nd try...
                if (may_try_again_on_exception)
                {
                    GrabWebPage_REMOTE(title, url, false);
                }
                else
                {
                    throw new UsefulTextException("Problem converting HTML page to PDF.  Please try again later.", String.Format("There has been a problem converting this web page '{0}' with title '{1}' to a PDF.", url, title), ex);
                }
            }
            finally
            {
                FileTools.Delete(filename);
            }

            StatusManager.Instance.UpdateStatus("HTMLToPDF", "Converting HTML to PDF: done");
        }
    }
}
