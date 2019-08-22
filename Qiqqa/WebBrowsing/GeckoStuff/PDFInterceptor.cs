using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Gecko;
using Gecko.Net;
using Gecko.Observers;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.WebBrowsing.GeckoStuff
{
    class PDFInterceptor : BaseHttpRequestResponseObserver
    {
        public static PDFInterceptor Instance = new PDFInterceptor();

        static bool have_notified_about_installing_acrobat = false;

        private PDFDocument potential_attachment_pdf_document = null;

        // information obtained from Gecko:
        private string document_source_filename = null;
        private string document_source_url = null;
        
        private PDFInterceptor() : base()
        {
        }

        public PDFDocument PotentialAttachmentPDFDocument
        {
            set
            {
                potential_attachment_pdf_document = value;
            }
        }

        protected override void Response(HttpChannel channel)
        {
#if DEBUG
            {
                string abs_uri = channel.Uri.AbsoluteUri;
                string disp_hdr = "(not specified)";
                try
                {
                    disp_hdr = channel.ContentDispositionHeader;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Gecko ContentDispositionHeader b0rk at {0}", abs_uri);
                }
                string mimetype = channel.ContentType;
                uint rvc = channel.ResponseStatus;
                var hdrs = channel.GetResponseHeadersDict();
                string hdrs_str = "";
                foreach (var key in hdrs)
                {
                    hdrs_str += "\n" + key.Key;
                    var v = key.Value;
                    hdrs_str += ":";
                    bool first = true;
                    foreach (var elv in v)
                    {
                        if (first)
                            first = false;
                        else
                            hdrs_str += ";";
                        hdrs_str += elv;
                    }
                }
                Logging.Info("PDFInterceptor::Response URI: {0} ; disposition: {1}; mime: {2}; status: {3}; headers:\n{4}", abs_uri, disp_hdr, mimetype, rvc, hdrs_str);
            }
#endif
            if (channel.ContentType.Contains("application/pdf"))
            {
                try
                {
                    // this is taken from the headers sent by the HTTP/FTP server
                    document_source_filename = channel.ContentDispositionFilename;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Gecko ContentDispositionFilename b0rk at {0}", channel.Uri.AbsoluteUri);
                }
                document_source_url = channel.Uri.AbsoluteUri;

                StreamListenerTee stream_listener_tee = new StreamListenerTee();  // <-- will be Dispose()d once response/content has been received
                stream_listener_tee.Completed += streamListener_Completed;
                TraceableChannel tc = channel.CastToTraceableChannel();
                tc.SetNewListener(stream_listener_tee);
            }
        }

        void streamListener_Completed(object sender, EventArgs e)
        {
            try
            {
                StreamListenerTee stream_listener_tee = (StreamListenerTee)sender;

                byte[] captured_data = stream_listener_tee.GetCapturedData();

                // stream_listener_tee.Dispose();  -- suggested by Microsoft Code Analysis Report, but with this active, some PDFs won't make it into the library!?!?

                if (0 == captured_data.Length)
                {
                    if (!have_notified_about_installing_acrobat)
                    {
                        have_notified_about_installing_acrobat = true;

                        NotificationManager.Instance.AddPendingNotification(
                            new NotificationManager.Notification(
                                "We notice that your PDF files are not loading in your browser.  Please install Acrobat Reader for Qiqqa to be able to automatically add PDFs to your libraries.",
                                "We notice that your PDF files are not loading in your browser.  Please install Acrobat Reader for Qiqqa to be able to automatically add PDFs to your libraries.",
                                NotificationManager.NotificationType.Info,
                                Icons.DocumentTypePdf,
                                "Download",
                                DownloadAndInstallAcrobatReader
                            ));
                    }

                    Logging.Error("We seem to have been notified about a zero-length PDF - URL: {0}, FILE: {1}", document_source_url, document_source_filename);
                    return;
                }

                string temp_pdf_filename = TempFile.GenerateTempFilename("pdf");
                File.WriteAllBytes(temp_pdf_filename, captured_data);

                PDFDocument pdf_document = Library.GuestInstance.AddNewDocumentToLibrary_SYNCHRONOUS(new FilenameWithMetadataImport
                {
                    filename = temp_pdf_filename,
                    original_filename = document_source_filename,
                    suggested_download_source_uri = document_source_url
                }, true);
                File.Delete(temp_pdf_filename);

                Application.Current.Dispatcher.Invoke
                (
                    new Action(() =>
                    {
                        PDFReadingControl pdf_reading_control = MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                        pdf_reading_control.EnableGuestMoveNotification(potential_attachment_pdf_document);
                    }),
                    DispatcherPriority.Background
                );
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while intercepting the download of a PDF - URL: {0}, FILE: {1}", document_source_url, document_source_filename);
            }
        }

        private void DownloadAndInstallAcrobatReader(object obj)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(() => MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_AdobeAcrobatDownload, true))
            );
        }
    }
}
