using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Utilities.Files;

namespace Utilities.Internet
{
    public class ScrapeCache
    {
        private string base_directory;
        private int throttle_ms;
        private Encoding encoding;
        
        private DateTime last_scrape_time = DateTime.MinValue;
        
        public ScrapeCache(string base_directory, int throttle_ms, Encoding encoding = null)
        {
            this.base_directory = base_directory;
            this.throttle_ms = throttle_ms;
            this.encoding=encoding;
        }

        public IEnumerable<string> GetAllContentFilenames()
        {
            return Directory.GetFiles(base_directory,"*", System.IO.SearchOption.AllDirectories);
        }

        public string ScrapeURL(string url, bool force_download = false, Dictionary<string, string> additional_headers = null)
        {
            string filename = ScrapeURLToFile(url, force_download, additional_headers);
            return ReadFile(filename);
        }

        public string ScrapeURLToFile(string url, bool force_download = false, Dictionary<string,string> additional_headers = null)
        {
            string cache_key = StreamMD5.FromText(url);
            string directory = base_directory + @"\" + cache_key.Substring(0, 2) + @"\";
            string filename = directory + cache_key;

            if (!File.Exists(filename) || force_download)
            {
                Utilities.Files.DirectoryTools.CreateDirectory(directory);

                // Crude throttle
                if (true)
                {
                    while (DateTime.UtcNow.Subtract(last_scrape_time).TotalMilliseconds < throttle_ms)
                    {
                        Thread.Sleep(50);
                    }
                    last_scrape_time = DateTime.UtcNow;
                }

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

                string filename_manifest = base_directory + @"\" + "manifest.txt";
                string manifest_line = String.Format("{0}\t{1}",cache_key, url);
                using (System.IO.StreamWriter sw = File.AppendText(filename_manifest))
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
