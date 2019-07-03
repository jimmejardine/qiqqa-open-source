using System.Collections.Generic;
using System.Net;

namespace Utilities.Internet.GoogleScholar
{
    public class GoogleScholarScrapePaperSet
    {
        public enum PaperSetSource
        {
            Query,
            RelatedPapers,
            CitedBy
        }

        // The inputs to the query - each is optional
        PaperSetSource paper_set_source;
        string search_query;
        GoogleScholarScrapePaper related_gssp;

        // The generated results
        string url;
        List<GoogleScholarScrapePaper> gssps;

        public string Url
        {
            get
            {
                return url;
            }
        }

        public static GoogleScholarScrapePaperSet GenerateFromQuery(IWebProxy proxy, string search_query, int num_items)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.Query;
            gssp_set.search_query = search_query;
            gssp_set.related_gssp = null;
            gssp_set.url = GoogleScholarScraper.GenerateQueryUrl(gssp_set.search_query, num_items);
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(proxy, gssp_set.url);

            return gssp_set;
        }

        public static GoogleScholarScrapePaperSet GenerateFromRelatedPapers(IWebProxy proxy, GoogleScholarScrapePaper gssp)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.RelatedPapers;
            gssp_set.search_query = null;
            gssp_set.related_gssp = gssp;
            gssp_set.url = gssp.related_articles_url;
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(proxy, gssp_set.url);

            return gssp_set;
        }

        public static GoogleScholarScrapePaperSet GenerateFromCitedBy(IWebProxy proxy, GoogleScholarScrapePaper gssp)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.CitedBy;
            gssp_set.search_query = null;
            gssp_set.related_gssp = gssp;
            gssp_set.url = gssp.cited_by_url;
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(proxy, gssp_set.url);

            return gssp_set;
        }

        public List<GoogleScholarScrapePaper> Papers
        {
            get
            {
                return gssps;
            }
        }
    }
}
