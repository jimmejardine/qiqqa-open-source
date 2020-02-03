using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Utilities.Files;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.Internet
{
    public class ScrapeCache
    {
        private string base_directory;
        private int throttle_ms;
        private Encoding encoding;
        private string userAgent;

        private Stopwatch last_scrape_time = Stopwatch.StartNew();

        public ScrapeCache(string base_directory, string user_agent, int throttle_ms, Encoding encoding = null)
        {
            this.base_directory = base_directory;
            this.throttle_ms = throttle_ms;
            this.encoding = encoding;
            userAgent = user_agent;
        }

        public IEnumerable<string> GetAllContentFilenames()
        {
            return Directory.GetFiles(base_directory, "*", SearchOption.AllDirectories);
        }

        public string ScrapeURL(string url, bool force_download = false, Dictionary<string, string> additional_headers = null)
        {
            string filename = ScrapeURLToFile(url, force_download, additional_headers);
            return ReadFile(filename);
        }

        public string ScrapeURLToFile(string url, bool force_download = false, Dictionary<string, string> additional_headers = null)
        {
            string cache_key = StreamMD5.FromText(url);
            string directory = Path.GetFullPath(Path.Combine(base_directory, cache_key.Substring(0, 2)));
            string filename = Path.GetFullPath(Path.Combine(directory, cache_key));

            if (!File.Exists(filename) || force_download)
            {
                Utilities.Files.DirectoryTools.CreateDirectory(directory);

                // Crude throttle
                while (last_scrape_time.ElapsedMilliseconds < throttle_ms)
                {
                    Thread.Sleep(50);
                }
                last_scrape_time.Restart();

                Logging.Info("Downloading from {0}", url);
                using (WebClient client = new WebClient())
                {
                    if (null != additional_headers)
                    {
                        foreach (var pair in additional_headers)
                        {
                            client.Headers.Add(pair.Key, pair.Value);
                        }
                    }
                    if (!String.IsNullOrEmpty(userAgent))
                    {
                        client.Headers.Add("User-agent", userAgent);
                    }

                    string temp_filename = filename + ".tmp";
                    try
                    {
                        client.DownloadFile(url, temp_filename);
                    }
                    catch (WebException ex)
                    {
                        File.WriteAllText(temp_filename, ex.ToString());
                    }

                    File.Delete(filename);
                    File.Move(temp_filename, filename);
                }

                string filename_manifest = Path.GetFullPath(Path.Combine(base_directory, @"manifest.txt"));
                string manifest_line = String.Format("{0}\t{1}", cache_key, url);
                using (StreamWriter sw = File.AppendText(filename_manifest))
                {
                    sw.WriteLine(manifest_line);
                }
            }

            return filename;
        }

        public string ReadFile(string filename)
        {
            if (null == encoding)
            {
                return File.ReadAllText(filename);
            }
            else
            {
                return File.ReadAllText(filename, encoding);
            }
        }
    }
}
