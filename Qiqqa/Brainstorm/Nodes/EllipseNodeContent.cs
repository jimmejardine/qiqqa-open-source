using System;
using System.Globalization;
using Qiqqa.Brainstorm.Common.Searching;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class EllipseNodeContent : ISearchable
    {
        public string text;

        public EllipseNodeContent()
        {
        }

        public EllipseNodeContent(string text)
        {
            this.text = text;
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != text) && text.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
