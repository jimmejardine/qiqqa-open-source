using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Utilities.Internet
{
    public class UrlDownloader
    {
        // -------------------------------------------------------------------------------------

        public static void DownloadWithBlocking(IWebProxy proxy, string url, out MemoryStream ms)
        {
            WebHeaderCollection header_collection;
            DownloadWithBlocking(proxy, url, out ms, out header_collection);
        }

        public static void DownloadWithBlocking(IWebProxy proxy, string url, out MemoryStream ms, out WebHeaderCollection header_collection)
        {
            DownloadWithBlocking(proxy, url, null, out ms, out header_collection);
        }

        class WebClientWithCompression : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {               
                WebRequest web_request = base.GetWebRequest(address);
                HttpWebRequest http_web_request = web_request as HttpWebRequest;
                if (null != http_web_request)
                {
                    http_web_request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    http_web_request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    return http_web_request;
                }
                else
                {
                    Logging.Warn("Cannot use automatic compression on non HttpWebRequest requestors.");
                    return web_request;
                }
            }
        }
        
        public static void DownloadWithBlocking(IWebProxy proxy, string url, IEnumerable<string> request_headers, out MemoryStream ms, out WebHeaderCollection header_collection)
        {
            using (WebClientWithCompression wc = new WebClientWithCompression())
            {
                DownloadProgressReporter dpr = new DownloadProgressReporter(wc);
                wc.Proxy = proxy;

                if (null != request_headers)
                {
                    foreach (string request_header in request_headers)
                    {
                        wc.Headers.Add(request_header);
                    }
                }

                byte[] data = wc.DownloadData(url);
                header_collection = wc.ResponseHeaders;
                ms = new MemoryStream(data);
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

            void wcb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
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

            void wcb_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
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

        public class DownloadAsyncTracker
        {
            private WebClientWithCompression wc;

            private object progress_lock = new object();

            public int ProgressPercentage { get; private set; } // integer [0,100]
            public bool DownloadComplete
            {
                get
                {
                    return this.DownloadDataCompletedEventArgs != null;
                }
            }
            public DownloadDataCompletedEventArgs DownloadDataCompletedEventArgs { get; set; }

            public DownloadAsyncTracker(IWebProxy proxy, string url)
            {
                // Init
                this.ProgressPercentage = 0;
                this.DownloadDataCompletedEventArgs = null;

                // Start the download
                wc = new WebClientWithCompression();
                wc.Proxy = proxy;
                wc.DownloadProgressChanged += this.wc_DownloadProgressChanged;
                wc.DownloadDataCompleted += this.wc_DownloadDataCompleted;
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
                    this.DownloadDataCompletedEventArgs = e;
                }
            }

            public void Cancel()
            {
                wc.CancelAsync();
            }
        };

        public static DownloadAsyncTracker DownloadWithNonBlocking(IWebProxy proxy, string url)
        {
            DownloadAsyncTracker tracker = new DownloadAsyncTracker(proxy, url);
            return tracker;
        }
    }
}



