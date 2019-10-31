using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Utilities.Internet;

namespace Utilities.Internet.GoogleScholar
{
    public class GoogleScholarScraper
    {
        // the google scholar search request has 2 parameters: the search criteria (text) and the max number of results (number) 
        public const string Url_GoogleScholarQuery = @"http://scholar.google.com/scholar?q={0}&num={1}";

        public static string GenerateQueryUrl(string query, int num_items)
        {
            string url = String.Format(Url_GoogleScholarQuery, Uri.EscapeDataString(query), num_items);

            return url;
        }

        public static List<GoogleScholarScrapePaper> ScrapeHtml(string html, string url)
        {
            List<GoogleScholarScrapePaper> gssps = new List<GoogleScholarScrapePaper>();

            try
            {                
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                ScrapeDoc(doc, url, gssps);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem parsing the GoogleScholar html");
            }

            return gssps;
        }

        public static List<GoogleScholarScrapePaper> ScrapeUrl(string url)
        {
            List<GoogleScholarScrapePaper> gssps = new List<GoogleScholarScrapePaper>();

            try
            {
                WebHeaderCollection header_collection = new WebHeaderCollection();

                using (MemoryStream ms = UrlDownloader.DownloadWithBlocking(url, out header_collection))
                { 
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(ms, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false);

                    ScrapeDoc(doc, url, gssps);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem parsing the GoogleScholar url {0}", url);
            }

            return gssps;
        }

        private static HtmlNodeCollection GetElementsWithClass(HtmlDocument doc, string className, string className2 = null, string childQuery = "")
        {
            HtmlNodeCollection lst = GetElementsWithClass(doc.DocumentNode, className, className2, childQuery);

            return lst;
        }

        private static HtmlNodeCollection GetElementsWithClass(HtmlNode baseNode, string className, string className2 = null, string childQuery = "")
        {
            string query2 = (String.IsNullOrEmpty(className2) ? "" : $" or contains(concat(' ',normalize-space(@class), ' '),' {className2} ')");
            string query = $"//*[contains(concat(' ',normalize-space(@class), ' '),' {className} '){query2}]{childQuery}";
            HtmlNodeCollection lst = baseNode.SelectNodes(query);

            return lst;
        }

        /// <summary>
        /// Parses the HTML document for the relevant information.  
        /// The url is that from where the document was originally downloaded - it is needed to reconstruct some of the relative links.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static void ScrapeDoc(HtmlDocument doc, string url, List<GoogleScholarScrapePaper> gssps)
        {
            HtmlNodeCollection NoAltElements_outer = GetElementsWithClass(doc, "gs_r");

            if (null != NoAltElements_outer)
            {
                foreach (HtmlNode element in NoAltElements_outer)
                {
                    GoogleScholarScrapePaper gssp = new GoogleScholarScrapePaper();

                    string element_html = element.OuterHtml;

                    HtmlDocument item_doc = new HtmlDocument();
                    item_doc.LoadHtml(element_html);

                    HtmlNode NoAltElements = GetElementsWithClass(item_doc, "gs_r")[0];

                    var sel = GetElementsWithClass(item_doc, "gs_r");


                    sel = GetElementsWithClass(item_doc, "gs_rt");

                    var title_node = GetElementsWithClass(NoAltElements, "gs_rt");
                    if (null != title_node)
                    {
                        string title_raw = WebUtility.HtmlDecode(title_node[0].InnerText);

                        Match match = Regex.Match(title_raw, @"\[(.*)\] (.*)", RegexOptions.Singleline);
                        if (Match.Empty != match)
                        {
                            gssp.type = match.Groups[1].Value;
                            gssp.title = match.Groups[2].Value;
                        }
                        else
                        {
                            gssp.type = "";
                            gssp.title = title_raw;
                        }
                    }
                    else
                    {
                        Logging.Error("ScrapeDoc: unexpected structure of the Google Scholar search page snippet. Report this at https://github.com/jimmejardine/qiqqa-open-source/issues/ as it seems Google Scholar has changed its HTML output significantly. HTML:\n{0}", element_html);
                    }

                    {
                        var source_url_node = title_node[0].SelectNodes("a");
                        if (null != source_url_node)
                        {
                            gssp.source_url = WebUtility.HtmlDecode(source_url_node[0].Attributes["href"].Value);
                        }
                    }

                    {
                        var authors_node = GetElementsWithClass(NoAltElements, "gs_a");
                        if (null != authors_node)
                        {
                            gssp.authors = WebUtility.HtmlDecode(authors_node[0].InnerHtml);
                        }
                    }

                    // Pull out the abstract
                    {
                        var abstract_node = GetElementsWithClass(NoAltElements, "gs_rs");
                        if (null != abstract_node)
                        {
                            gssp.abstract_html = WebUtility.HtmlDecode(abstract_node[0].InnerText);
                        }
                    }

                    // Pull out the potential downloads
                    {
                        var downloads_node = GetElementsWithClass(NoAltElements, "gs_ggsd");  // was 'gs_md_wp gs_ttss' before.
                        if (null != downloads_node)
                        {
                            var source_url_node = downloads_node[0].SelectNodes(".//a");
                            if (null != source_url_node)
                            {
                                foreach (var child_node in source_url_node)
                                {
                                    if (null != child_node.Attributes["href"])
                                    {
                                        string download_url = child_node.Attributes["href"].Value;
                                        gssp.download_urls.Add(download_url);
                                        Logging.Info("ScrapeDoc(URL: {0}): Downloadable from {1}", url, download_url);
                                    }
                                }
                            }
                        }
                    }

                    var see_also_nodes = GetElementsWithClass(NoAltElements, "gs_fl", null, "/a");
                    GetUrlForRelatedList(url, "?cites=", "Cited by", see_also_nodes, out gssp.cited_by_header, out gssp.cited_by_url);
                    GetUrlForRelatedList(url, "?q=related:", "Related", see_also_nodes, out gssp.related_articles_header, out gssp.related_articles_url);
                    GetUrlForRelatedList(url, "scholar.bib?q=info:", "Import into BibTeX", see_also_nodes, out gssp.bibtex_header, out gssp.bibtex_url);

                    gssps.Add(gssp);
                }
            }
        }

        private static void GetUrlForRelatedList(string base_url, string href_query_part, string related_list_header, HtmlNodeCollection nodes, out string header, out string url)
        {
            header = null;
            url = null;

            if (null != nodes)
            {
                foreach (var node in nodes)
                {
                    try
                    {
                        //if (node.InnerText.StartsWith(related_list_header))
                        if (null != node.Attributes["href"] && node.Attributes["href"].Value.Contains(href_query_part))
                        {
                            header = node.InnerText;
                            url = node.Attributes["href"].Value;

                            // Make the relative url an absolute one
                            {
                                Uri base_uri = new Uri(base_url);
                                Uri absolute_uri;
                                Uri.TryCreate(base_uri, url, out absolute_uri);
                                url = absolute_uri.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "Trouble while trying to convert {2} URL {0} (BASE: {1})", url, base_url, related_list_header);
                    }
                }
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            //GoogleScholarScraper gs_scraper = new GoogleScholarScraper("latent dirichlet allocation", 100, true);
            string url = GenerateQueryUrl("dirichlet", 100);
            List<GoogleScholarScrapePaper> gssps = ScrapeUrl(null, url);
            foreach (var gs_paper in gssps)
            {
#if false
                bool have_a_match = false;
                bool have_a_close_match = false;

                foreach (ACLPaperMetadata metadata in ACLPaperMetadatas.GetACLPaperMetadatas().Values)
                {                    
                    string acl_title = metadata.title.ToLower();
                    string gs_title = gs_paper.title.ToLower();
                    double max_length = Math.Max(acl_title.Length, gs_title.Length);
                    int lewenstein = StringTools.LewensteinDistance(acl_title, gs_title);
                    if (lewenstein / max_length < 0.2)
                    {
                        have_a_close_match = true;
                    }

                    if (acl_title.Contains(gs_title) || gs_title.Contains(acl_title))
                    {
                        have_a_match = true;
                    }
                }

                Logging.Info((have_a_match ? "X" : ".") + (have_a_close_match ? "X" : "."));

                if (have_a_match != have_a_close_match)
                {
                    int aaaaaaaaa = 3;
                }
#endif

                Logging.Info(gs_paper.ToString());
            }
        }
#endif

#endregion
    }
}
