using System;
using System.Globalization;
using Utilities.GUI.Brainstorm.Common.Searching;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    public class EllipseNodeContent : Searchable
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
