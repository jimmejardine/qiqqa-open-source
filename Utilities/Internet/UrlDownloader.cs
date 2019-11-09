using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Utilities.Internet
{
    public class UrlDownloader
    {
        // -------------------------------------------------------------------------------------

        public static MemoryStream DownloadWithBlocking(string url)
        {
            WebHeaderCollection header_collection;
            return DownloadWithBlocking(url, out header_collection);
        }

        public static MemoryStream DownloadWithBlocking(string url, out WebHeaderCollection header_collection)
        {
            return DownloadWithBlocking(url, null, out header_collection);
        }

        private class WebClientWithCompression : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest web_request = base.GetWebRequest(address);
                HttpWebRequest http_web_request = web_request as HttpWebRequest;
                if (null != http_web_request)
                {
                    http_web_request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    // Allow ALL protocols
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                    return http_web_request;
                }
                else
                {
                    Logging.Warn("Cannot use automatic compression on non HttpWebRequest requestors.");
                    return web_request;
                }
            }
        }

        public static MemoryStream DownloadWithBlocking(string url, IEnumerable<string> request_headers, out WebHeaderCollection header_collection)
        {
            using (WebClientWithCompression wc = new WebClientWithCompression())
            {
                DownloadProgressReporter dpr = new DownloadProgressReporter(wc);
                wc.Proxy = Configuration.Proxy;

                if (null != request_headers)
                {
                    foreach (string request_header in request_headers)
                    {
                        wc.Headers.Add(request_header);
                    }
                }

                // same headers as sent by modern Chrome.
                // Gentlemen, start your prayer wheels!
                wc.Headers.Add("Cache-Control", "no-cache");
                wc.Headers.Add("Pragma", "no-cache");
                wc.Headers.Add("User-agent", Configuration.WebUserAgent);

                byte[] data = wc.DownloadData(new Uri(url));
                header_collection = wc.ResponseHeaders;
                return new MemoryStream(data);
            }
        }

        private class DownloadProgressReporter
        {
            private object progress_lock = new object();
            private int progress_percentage = 0;

            public DownloadProgressReporter(WebClient wc)
            {
                wc.DownloadProgressChanged += wcb_DownloadProgressChanged;
                wc.DownloadDataCompleted += wcb_DownloadDataCompleted;
            }

            private void wcb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (progress_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    if (progress_percentage < e.ProgressPercentage)
                    {
                        Logging.Info("Downloaded {0} / {1} ({2}%)", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
                        progress_percentage = e.ProgressPercentage;
                    }
                }
            }

            private void wcb_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (progress_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    Logging.Info("Download complete");
                }
            }
        }

        // -------------------------------------------------------------------------------------

        public class DownloadAsyncTracker : IDisposable
        {
            private WebClientWithCompression wc;

            private object progress_lock = new object();

            public int ProgressPercentage { get; private set; } // integer [0,100]
            public bool DownloadComplete => DownloadDataCompletedEventArgs != null;
            public DownloadDataCompletedEventArgs DownloadDataCompletedEventArgs { get; set; }

            public DownloadAsyncTracker(string url)
            {
                // Init
                ProgressPercentage = 0;
                DownloadDataCompletedEventArgs = null;

                // Start the download
                wc = new WebClientWithCompression();
                wc.Proxy = Configuration.Proxy;

                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadDataCompleted += wc_DownloadDataCompleted;

                // same headers as sent by modern Chrome.
                // Gentlemen, start your prayer wheels!
                wc.Headers.Add("Cache-Control", "no-cache");
                wc.Headers.Add("Pragma", "no-cache");
                wc.Headers.Add("User-agent", Configuration.WebUserAgent);

                wc.DownloadDataAsync(new Uri(url));
            }

            internal void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (progress_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    // Limit the logging frequency
                    if (ProgressPercentage < e.ProgressPercentage)
                    {
                        Logging.Info("Downloaded {0} / {1} ({2}%)", e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
                        ProgressPercentage = e.ProgressPercentage;
                    }
                }
            }

            internal void wc_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (progress_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    Logging.Info("Download complete");
                    DownloadDataCompletedEventArgs = e;
                }

                CleanupAfterDownload();
            }

            public void Cancel()
            {
                wc.CancelAsync();

                //CleanupAfterDownload();
            }

            private void CleanupAfterDownload()
            {
                if (null != wc)
                {
                    wc.DownloadProgressChanged -= wc_DownloadProgressChanged;
                    wc.DownloadDataCompleted -= wc_DownloadDataCompleted;

                    wc.Dispose();
                }
                wc = null;
            }

            #region --- IDisposable ------------------------------------------------------------------------

            ~DownloadAsyncTracker()
            {
                Logging.Debug("~DownloadAsyncTracker()");
                Dispose(false);
            }

            public void Dispose()
            {
                Logging.Debug("Disposing DownloadAsyncTracker");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private int dispose_count = 0;
            protected virtual void Dispose(bool disposing)
            {
                Logging.Debug("DownloadAsyncTracker::Dispose({0}) @{1}", disposing, dispose_count);

                try
                {
                    if (dispose_count == 0)
                    {
                        // Get rid of managed resources
                        CleanupAfterDownload();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                }

                ++dispose_count;
            }

            #endregion
        };

        public static DownloadAsyncTracker DownloadWithNonBlocking(string url)
        {
            DownloadAsyncTracker tracker = new DownloadAsyncTracker(url);
            return tracker;
        }
    }
}
