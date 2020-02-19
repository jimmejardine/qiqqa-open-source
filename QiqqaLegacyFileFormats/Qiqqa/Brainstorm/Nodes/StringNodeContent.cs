using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class StringNodeContent : ISearchable
    {
        [NonSerialized]
        public static string DEFAULT_NODE_CONTENT = "Type or double click to edit...";

        private string text;
        public string Text
        {
            get => text;
            set => text = value;
        }

        public StringNodeContent()
        {
        }

        public StringNodeContent(string text)
        {
            this.text = text;
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != text) && text.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
