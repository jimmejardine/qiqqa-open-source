using System.Collections.Generic;

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
#pragma warning disable CS0414 // The field 'paper_set_source' is assigned a value which is never used
        private PaperSetSource paper_set_source;
#pragma warning restore CS0414  // The field 'paper_set_source' is assigned a value which is never used
        private string search_query;
        private GoogleScholarScrapePaper related_gssp;

        // The generated results
        private string url;
        private List<GoogleScholarScrapePaper> gssps;

        public string Url => url;

        public static GoogleScholarScrapePaperSet GenerateFromQuery(string search_query, int num_items)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.Query;
            gssp_set.search_query = search_query;
            gssp_set.related_gssp = null;
            gssp_set.url = GoogleScholarScraper.GenerateQueryUrl(gssp_set.search_query, num_items);
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(gssp_set.url);

            return gssp_set;
        }

        public static GoogleScholarScrapePaperSet GenerateFromRelatedPapers(GoogleScholarScrapePaper gssp)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.RelatedPapers;
            gssp_set.search_query = null;
            gssp_set.related_gssp = gssp;
            gssp_set.url = gssp.related_articles_url;
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(gssp_set.url);

            return gssp_set;
        }

        public static GoogleScholarScrapePaperSet GenerateFromCitedBy(GoogleScholarScrapePaper gssp)
        {
            GoogleScholarScrapePaperSet gssp_set = new GoogleScholarScrapePaperSet();

            gssp_set.paper_set_source = PaperSetSource.CitedBy;
            gssp_set.search_query = null;
            gssp_set.related_gssp = gssp;
            gssp_set.url = gssp.cited_by_url;
            gssp_set.gssps = GoogleScholarScraper.ScrapeUrl(gssp_set.url);

            return gssp_set;
        }

        public List<GoogleScholarScrapePaper> Papers => gssps;
    }
}
