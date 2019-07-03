using System;
using System.Globalization;
using Utilities.GUI.Brainstorm.Common.Searching;
using Utilities.Reflection;

namespace Utilities.GUI.Brainstorm.Nodes.SimpleNodes
{
    [Serializable]
    public class StringNodeContent : Searchable
    {
        [NonSerialized]
        public static string DEFAULT_NODE_CONTENT = "Type or double click to edit...";

        private string text;
        public string Text
        {
            get {  return text; }
            set { text = value; }
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

        [NonSerialized]
        AugmentedBindable<StringNodeContent> bindable = null;
        public AugmentedBindable<StringNodeContent> Bindable
        {
            get
            {
                if (null == bindable)
                {
                    bindable = new AugmentedBindable<StringNodeContent>(this);
                }

                return bindable;
            }
        }

    }
}
