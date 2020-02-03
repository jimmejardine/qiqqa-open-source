using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.WebBrowsing
{
    [Serializable]
    public class WebSearcher
    {
        public string key;
        public string title;
        public string url_template;
        public PopulateUrlTemplateDelegate populate_url_template;

        public WebSearcher(string key, string title, string url_template, PopulateUrlTemplateDelegate populate_url_template)
        {
            this.key = key;
            this.title = title;
            this.url_template = url_template;
            this.populate_url_template = populate_url_template;
        }

        public string VisibleTitle => title;

        public string VisibleUrl => url_template;


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
