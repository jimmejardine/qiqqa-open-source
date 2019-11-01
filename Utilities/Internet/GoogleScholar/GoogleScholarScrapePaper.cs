using System.Collections.Generic;
using System.Text;

namespace Utilities.Internet.GoogleScholar
{
    public class GoogleScholarScrapePaper
    {
        public string type;
        public string title;
        public string source_url;
        public string authors;
        public string abstract_html;
        public string cited_by_header, cited_by_url;
        public string related_articles_header, related_articles_url;
        public string bibtex_header, bibtex_url;
        public List<string> download_urls = new List<string>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\n");
            sb.AppendFormat("Type:          {0}\n", type);
            sb.AppendFormat("Title:         {0}\n", title);
            sb.AppendFormat("Authors:       {0}\n", authors);
            sb.AppendFormat("Cite count:    {0}\n", cited_by_header);
            sb.AppendFormat("Abstract:      {0}\n", abstract_html);
            sb.AppendFormat("Download URLs: {0}\n", download_urls.Count);

            return sb.ToString();
        }
    }
}
