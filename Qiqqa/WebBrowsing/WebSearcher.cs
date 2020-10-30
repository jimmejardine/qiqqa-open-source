using System;

namespace Qiqqa.WebBrowsing
{
    [Serializable]
    public class WebSearcher
    {
        public string key;
        public string title
        {
            get;
            set;
        }
        public string url_template
        {
            get;
            set;
        }
        public string StartUri
        {
            get;
            set;
        }
        private PopulateUrlTemplateDelegate populate_url_template;

        public WebSearcher(string key, string title, string url_template, string start_uri, PopulateUrlTemplateDelegate populate_url_template)
        {
            this.key = key;
            this.title = title;
            this.url_template = url_template;
            this.StartUri = start_uri;
            this.populate_url_template = populate_url_template;
        }

        public string VisibleTitle => title;            // used by the XAML bindings in WebSearcherPreferenceControl

        public string VisibleUrl => url_template;       // used by the XAML bindings in WebSearcherPreferenceControl

        public Uri Populate(string search_terms)
        {
            return populate_url_template(url_template, search_terms);
        }

        #region --- Url templates ----------------------------------------------------------------------------------------------------------------

        public delegate Uri PopulateUrlTemplateDelegate(string url_template, string keywords);

        public static Uri PopulateUrlTemplateDelegate_UrlEncode(string url_template, string keywords)
        {
            string keywords_translated = Uri.EscapeDataString(keywords);
            return new Uri(String.Format(url_template, keywords_translated));
        }

        public static Uri PopulateUrlTemplateDelegate_WikiEncode(string url_template, string keywords)
        {
            string keywords_translated = keywords.Replace(' ', '_');
            return new Uri(String.Format(url_template, keywords_translated));
        }

        #endregion
    }
}
