using System.Collections.Generic;

namespace Qiqqa.WebBrowsing
{
    class WebSearchers
    {
        public static readonly string SCHOLAR_KEY = "GOOGLE_SCHOLAR";
        public static readonly string PUBMED_KEY = "PUBMED";
        public static readonly string PUBMEDXML_KEY = "PUBMEDXML";
        public static readonly string ARXIV_KEY = "ARXIV";

        public static readonly List<WebSearcher> WEB_SEARCHERS = new List<WebSearcher>
            (
                new WebSearcher[]
                {
                    new WebSearcher("GOOGLE_US", "Google.com", "http://www.google.com/#q={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher("GOOGLE_UK", "Google.co.uk", "http://www.google.co.uk/#q={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher(SCHOLAR_KEY, "Google Scholar", "http://scholar.google.com/scholar?q={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher(PUBMED_KEY, "PubMed", "http://www.ncbi.nlm.nih.gov/pubmed?term={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher(PUBMEDXML_KEY, "PubMedXML", "http://www.ncbi.nlm.nih.gov/pubmed?report=xml&term={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher(ARXIV_KEY, "arXiv", "http://search.arxiv.org:8081/?query={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher("MSACADEMIC", "Microsoft Academic", "http://academic.research.microsoft.com/Search?query={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher("JSTOR", "JSTOR", "http://www.jstor.org/action/doBasicSearch?Query={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),                    
                    new WebSearcher("IEEEXPLORE", "IEEE Xplore", "http://ieeexplore.ieee.org/search/searchresult.jsp?queryText={0}", WebSearcher.PopulateUrlTemplateDelegate_UrlEncode),
                    new WebSearcher("WIKIPEDIA", "Wikipedia", "http://en.wikipedia.org/wiki/{0}", WebSearcher.PopulateUrlTemplateDelegate_WikiEncode),
                }
            );

        internal static WebSearcher FindWebSearcher(string requested_web_searcher)
        {
            foreach (var web_searcher in WEB_SEARCHERS)
            {
                if (0 == web_searcher.key.CompareTo(requested_web_searcher))
                {
                    return web_searcher;
                }
            }

            return null;
        }
    }
}