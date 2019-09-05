using System;
using System.Collections.Generic;
using System.Text;

namespace Qiqqa.Common.TagManagement
{
    class TagTools
    {
        public static HashSet<string> ConvertTagBundleToTags(string tag_bundle)
        {
            if (String.IsNullOrEmpty(tag_bundle))
            {
                return new HashSet<string>();
            }

            HashSet<string> list = new HashSet<string>(tag_bundle.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries));
            return list;
        }

        internal static string ConvertTagListToTagBundle(ICollection<string> tag_list)
        {
            if (0 == tag_list.Count)
            {
                return "";
            }
            else
            {
                List<string> tag_list_sorted = new List<string>(tag_list);
                tag_list_sorted.Sort();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < tag_list_sorted.Count; ++i)                
                {
                    string tag = tag_list_sorted[i];
                    tag = tag.Trim();                    

                    if (i > 0)
                    {
                        sb.Append(';');
                    }
                    sb.Append(tag);
                }

                return sb.ToString();
            }
        }
    }
}
