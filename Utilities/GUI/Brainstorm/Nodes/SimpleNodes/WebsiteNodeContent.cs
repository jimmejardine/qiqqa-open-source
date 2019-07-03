using System;
using System.Globalization;
using System.Reflection;
using Utilities.GUI.Brainstorm.Common.Searching;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    [Obfuscation(Feature = "properties renaming")]
    public class WebsiteNodeContent : Searchable
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime LastVisited { get; set; }
        public int VisitedCount { get; set; }

        public bool MatchesKeyword(string keyword)
        {
            return
                null != Url && Url.ToLower(CultureInfo.CurrentCulture).Contains(keyword) ||
                null != Title && Title.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
