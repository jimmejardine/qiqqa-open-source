using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Utilities.Collections;

namespace Utilities.Maps.Geocode
{
    public class PostcodeOutcodes
    {
        public static readonly PostcodeOutcodes Instance = new PostcodeOutcodes();

        public Dictionary<string, string> PostcodeMap = new Dictionary<string, string>();
        public Dictionary<string, string> RegionMap = new Dictionary<string, string>();
        public Dictionary<string, string> SubRegionMap = new Dictionary<string, string>();

        private PostcodeOutcodes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Utilities.Maps.Geocode.postcode-outcodes.csv.gz");

            using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (StreamReader sr = new StreamReader(compressed_stream))
                {
                    bool first_line = true;

                    AveragingDictionary<string> lats_region = new AveragingDictionary<string>();
                    AveragingDictionary<string> lons_region = new AveragingDictionary<string>();
                    AveragingDictionary<string> lats_sub_region = new AveragingDictionary<string>();
                    AveragingDictionary<string> lons_sub_region = new AveragingDictionary<string>();

                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (null == line) break;

                        if (first_line)
                        {
                            first_line = false;
                            continue;
                        }

                        // Parse the line...
                        string[] parts = line.Split(',');

                        string postcode = parts[1];
                        string region = PostcodeToRegion(postcode);
                        string sub_region = PostcodeToSubRegion(postcode);
                        string lat = parts[2];
                        string lon = parts[3];
                        string latlon = lat + ',' + lon;

                        PostcodeMap[postcode] = latlon;

                        double lat_num = Double.Parse(lat);
                        double lon_num = Double.Parse(lon);
                        lats_region.Add(region, lat_num);
                        lons_region.Add(region, lon_num);
                        lats_sub_region.Add(sub_region, lat_num);
                        lons_sub_region.Add(sub_region, lon_num);
                    }

                    // Populate the averages
                    {
                        foreach (var region in lats_region.Keys)
                        {
                            RegionMap[region] = lats_region[region].Current + "," + lons_region[region].Current;
                        }

                        foreach (var sub_region in lats_sub_region.Keys)
                        {
                            SubRegionMap[sub_region] = lats_sub_region[sub_region].Current + "," + lons_sub_region[sub_region].Current;
                        }
                    }
                }
            }
        }

        public string GetRegionLatLon(string postcode)
        {
            string region = PostcodeToRegion(postcode);
            if (null == region) return "Unknown";
            if (!RegionMap.ContainsKey(region)) return "Unknown";
            return RegionMap[region];
        }

        public static string PostcodeToRegion(string postcode)
        {
            string region = "";
            foreach (char c in postcode)
            {
                if (Char.IsLetter(c))
                {
                    region = region + c;
                }
                else
                {
                    break;
                }
            }

            return region;
        }

        public static string PostcodeToSubRegion(string postcode)
        {
            string region = "";
            foreach (char c in postcode)
            {
                if (' ' != c)
                {
                    region = region + c;
                }
                else
                {
                    break;
                }
            }

            return region;
        }
    }
}
