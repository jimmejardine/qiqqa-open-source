using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class WebsiteNodeContent : ISearchable
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime LastVisited { get; set; }
        public int VisitedCount { get; set; }

        public bool MatchesKeyword(string keyword)
        {
            return
                null != Url && Url.ToLower().Contains(keyword) ||
                null != Title && Title.ToLower().Contains(keyword);
        }
    }
}
