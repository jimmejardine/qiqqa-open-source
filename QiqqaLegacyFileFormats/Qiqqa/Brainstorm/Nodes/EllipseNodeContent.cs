using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
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
            return (null != text) && text.ToLower().Contains(keyword);
        }
    }
}
