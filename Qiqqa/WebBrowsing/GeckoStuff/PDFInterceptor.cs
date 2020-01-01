using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using Gecko.Net;
using Gecko.Observers;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.WebBrowsing.GeckoStuff
{
    internal class PDFInterceptor : BaseHttpRequestResponseObserver
    {
        public static PDFInterceptor Instance = new PDFInterceptor();
        private static bool have_notified_about_installing_acrobat = false;

        private PDFDocument potential_attachment_pdf_document = null;

        // information obtained from Gecko:
        private string document_source_filename = null;
        private string document_source_url = null;

        private PDFInterceptor() : base()
        {
        }

        public PDFDocument PotentialAttachmentPDFDocument
        {
            set => potential_attachment_pdf_document = value;
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
            // is this a 302/30x Response Code (Forwarded)?
            // if so, then grab the forward reference URI and go grab that one.
            if (channel.ResponseStatus == (uint)HttpStatusCode.MovedPermanently
                || channel.ResponseStatus == (uint)HttpStatusCode.Moved
                || channel.ResponseStatus == (uint)HttpStatusCode.Redirect
                || channel.ResponseStatus == (uint)HttpStatusCode.Found
                || channel.ResponseStatus == (uint)HttpStatusCode.SeeOther
                || channel.ResponseStatus == (uint)HttpStatusCode.RedirectKeepVerb
                || channel.ResponseStatus == (uint)HttpStatusCode.TemporaryRedirect
                || channel.ResponseStatus == 308)
            {
                string fwd_uri_str = channel.GetResponseHeader("Location");
                Uri fwd_uri = new Uri(channel.Uri, fwd_uri_str);
                // fetch the PDF!
                ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(Library.GuestInstance, fwd_uri.AbsoluteUri);
            }
            else if (channel.ContentType.Contains("application/pdf"))
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

#pragma warning disable CA2000 // Dispose objects before losing scope
                StreamListenerTee stream_listener_tee = new StreamListenerTee();  // <-- will be Dispose()d once response/content has been received
#pragma warning restore CA2000 // Dispose objects before losing scope
                stream_listener_tee.Completed += streamListener_Completed;
                TraceableChannel tc = channel.CastToTraceableChannel();
                tc.SetNewListener(stream_listener_tee);
            }
        }

        private void streamListener_Completed(object sender, EventArgs e)
        {
            StreamListenerTee stream_listener_tee = null;

            try
            {
                stream_listener_tee = (StreamListenerTee)sender;

                // WARNING: `captured_data` is a reference. Delay Dispose() call until data has actually been written to disk!
                byte[] captured_data = stream_listener_tee.GetCapturedData();

                if (0 == captured_data.Length)
                {
                    if (!have_notified_about_installing_acrobat)
                    {
                        have_notified_about_installing_acrobat = true;

                        NotificationManager.Instance.AddPendingNotification(
                            new NotificationManager.Notification(
                                "We notice that your PDF files are not loading in your browser.  Please install Acrobat Reader for Qiqqa to be able to automatically add PDFs to your libraries.",
                                null,
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

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                PDFDocument pdf_document = Library.GuestInstance.AddNewDocumentToLibrary_SYNCHRONOUS(new FilenameWithMetadataImport
                {
                    Filename = temp_pdf_filename,
                    OriginalFilename = document_source_filename,
                    SuggestedDownloadSourceURI = document_source_url
                }, true);
                    File.Delete(temp_pdf_filename);

                    WPFDoEvents.InvokeInUIThread(() =>
                        {
                            PDFReadingControl pdf_reading_control = MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                            pdf_reading_control.EnableGuestMoveNotification(potential_attachment_pdf_document);
                        },
                        priority: DispatcherPriority.Background
                    );
                });
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while intercepting the download of a PDF - URL: {0}, FILE: {1}", document_source_url, document_source_filename);
            }
            finally
            {
                // suggested by Microsoft Code Analysis Report, but with this done earlier than right here *after* the file WRITE action, some PDFs won't make it into the library!!
                stream_listener_tee.Dispose();
            }
        }

        private void DownloadAndInstallAcrobatReader(object obj)
        {
            WPFDoEvents.InvokeAsyncInUIThread(() => MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_AdobeAcrobatDownload, true));
        }
    }
}
