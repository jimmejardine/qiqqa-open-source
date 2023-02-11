using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.WebBrowsing.EZProxy
{
    public class Proxy
    {
        public string url { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

#if SAMPLE_LOAD_CODE

    public class Proxies
    {
        public static Proxies Instance = new Proxies();

        private Proxies()
        {
        }

        private List<Proxy> proxies;
        public List<Proxy> GetProxies()
        {
            if (null == proxies)
            {
                string proxies_json_filename = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"WebBrowsing/EZProxy/proxies.json"));
                Logging.Info("Loading EZProxy proxy information from {0}", proxies_json_filename);
                string proxies_json = File.ReadAllText(proxies_json_filename);
                proxies = JsonConvert.DeserializeObject<List<Proxy>>(proxies_json);
                Logging.Info("Loaded {0} EZProxy proxies", proxies.Count);

                proxies.Sort(
                    delegate (Proxy a, Proxy b)
                    {
                        return String.Compare(a.name, b.name);
                    }
                );
            }

            return proxies;
        }
    }

#endif

}
