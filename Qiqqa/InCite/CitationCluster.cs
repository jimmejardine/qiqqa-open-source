using System;
using System.Collections.Generic;
using System.Text;

namespace Qiqqa.InCite
{
    public class CitationCluster
    {
        public static readonly string QIQQA_CLUSTER = "QIQQA_CLUSTER";
        private static readonly string SEPARATOR_CITATION = ".oOo.";
        private static readonly string[] SEPARATOR_CITATION_ARRAY = new string[] { SEPARATOR_CITATION };
        private static readonly string SEPARATOR_PARAMETER = ".xXx.";
        private static readonly string[] SEPARATOR_PARAMETER_ARRAY = new string[] { SEPARATOR_PARAMETER };
        private static readonly string SEPARATOR_HASH = ".wWw.";
        private static readonly string[] SEPARATOR_HASH_ARRAY = new string[] { SEPARATOR_HASH };

        public string cluster_id;
        public List<CitationItem> citation_items;
        public string rtf_hash;

        public CitationCluster(string source)
        {
            // Citation cluster is of the form
            //    [.wWw.rtf_hash.wWw.]QIQQA_CLUSTER.oOo.<cluster_id>[.oOo.<reference_key>.oOo.<reference_library_hint>[.xXx.param_key.xXx.param_value]*]*.oOo.

            // Split into the root and the hash
            string rtf_hash = null;

            string[] hash_items = source.Split(SEPARATOR_HASH_ARRAY, StringSplitOptions.None);
            if (1 == hash_items.Length)
            {
                rtf_hash = null;
                source = hash_items[0];
            }
            else
            {
                rtf_hash = hash_items[1];
                source = hash_items[2];
            }

            string[] source_items = source.Split(SEPARATOR_CITATION_ARRAY, StringSplitOptions.None);

            // Sanity check
            if (QIQQA_CLUSTER != source_items[0])
            {
                throw new Exception(String.Format("Cannot parse invalid citation cluster: {0}", source));
            }

            // Build the cluster
            cluster_id = source_items[1];
            citation_items = new List<CitationItem>();
            this.rtf_hash = rtf_hash;

            // Build the sub_items
            for (int i = 2; i < source_items.Length; i += 2)
            {
                if (source_items[i].Trim().StartsWith(@"\*"))
                {
                    break;
                }

                string reference_key = source_items[i + 0];
                string remainder = source_items[i + 1];
                string[] remainder_split = remainder.Split(SEPARATOR_PARAMETER_ARRAY, StringSplitOptions.None);
                string reference_library_hint = remainder_split[0];
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                for (int j = 1; j < remainder_split.Length; j += 2)
                {
                    parameters[remainder_split[j + 0]] = remainder_split[j + 1];
                }

                CitationItem ci = new CitationItem(reference_key, reference_library_hint, parameters);
                citation_items.Add(ci);
            }
        }

        public CitationCluster()
        {
            cluster_id = GetRandomClusterId();
            citation_items = new List<CitationItem>();
            rtf_hash = null;
        }

        public CitationCluster(CitationItem ci)
        {
            cluster_id = GetRandomClusterId();
            citation_items = new List<CitationItem>();
            citation_items.Add(ci);
            rtf_hash = null;
        }

        public string GetBibTeXKeySummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach (CitationItem ci in citation_items)
            {
                sb.Append(ci.reference_key);
                sb.Append(" ");
            }
            sb.Append(" ]");

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(SEPARATOR_HASH);
            sb.Append(rtf_hash);
            sb.Append(SEPARATOR_HASH);
            sb.Append(QIQQA_CLUSTER);
            sb.Append(SEPARATOR_CITATION);
            sb.Append(cluster_id);
            for (int i = 0; i < citation_items.Count; ++i)
            {
                sb.Append(SEPARATOR_CITATION);
                sb.Append(citation_items[i].reference_key);
                sb.Append(SEPARATOR_CITATION);
                sb.Append(citation_items[i].reference_library_hint);
                foreach (var pair in citation_items[i].parameters)
                {
                    sb.Append(SEPARATOR_PARAMETER);
                    sb.Append(pair.Key);
                    sb.Append(SEPARATOR_PARAMETER);
                    sb.Append(pair.Value);
                }
            }
            sb.Append(SEPARATOR_CITATION);

            return sb.ToString();
        }

        public static string GetRandomClusterId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        internal string ToCodedString()
        {
            return "MERGEFIELD " + ToString() + " \\* MERGEFORMAT";
        }
    }
}
