using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

#if TEST
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using static QiqqaTestHelpers.MiscTestHelpers;
using Newtonsoft.Json;
using Utilities;

using Utilities.Internet.GoogleScholar;
#endif


#if !TEST

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

                ScrapeDoc(doc, url, ref gssps);
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

                    ScrapeDoc(doc, url, ref gssps);
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
        protected static void ScrapeDoc(HtmlDocument doc, string url, ref List<GoogleScholarScrapePaper> gssps)
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

#if false
                    var sel = GetElementsWithClass(item_doc, "gs_r");
                    sel = GetElementsWithClass(item_doc, "gs_rt");
#endif

                    var title_node = GetElementsWithClass(NoAltElements, "gs_rt");
                    if (null != title_node)
                    {
                        string title_raw = WebUtility.HtmlDecode(title_node[0].InnerText);

                        // Anno 2020, Google Scholar has the type duplicated in *two* spans: we only extract the first of those.
                        Match match = Regex.Match(title_raw, @"\[(.*?)(\][^\]]+)?\] (.*)", RegexOptions.Singleline);
                        if (Match.Empty != match)
                        {
                            gssp.type = match.Groups[1].Value;
                            gssp.title = match.Groups[3].Value;
                        }
                        else
                        {
                            gssp.type = "";
                            gssp.title = title_raw;
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
                        if (see_also_nodes != null)
                        {
                            GetUrlForRelatedList(url, "?cites=", "Cited by", see_also_nodes, out gssp.cited_by_header, out gssp.cited_by_url);
                            GetUrlForRelatedList(url, "?q=related:", "Related", see_also_nodes, out gssp.related_articles_header, out gssp.related_articles_url);
                            GetUrlForRelatedList(url, "scholar.bib?q=info:", "Import into BibTeX", see_also_nodes, out gssp.bibtex_header, out gssp.bibtex_url);
                        }

                        gssps.Add(gssp);
                    }
                    else
                    {
                        Logging.Error("ScrapeDoc: unexpected structure of the Google Scholar search page snippet. Report this at https://github.com/jimmejardine/qiqqa-open-source/issues/ as it seems Google Scholar has changed its HTML output significantly. HTML:\n{0}", element_html);
                    }
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
    }
}


#else

#region --- Test ------------------------------------------------------------------------

namespace QiqqaUnitTester
{
    // Note https://stackoverflow.com/questions/10375090/accessing-protected-members-of-another-class

    [TestClass]
    public class GoogleScholarScraperTester : GoogleScholarScraper   // HACK: inherit so we can access protected members
    {
        // https://stackoverflow.com/questions/29003215/newtonsoft-json-serialization-returns-empty-json-object
        private class Result
        {
            public List<List<string>> lines_set;
        }

        [DataRow("Utilities/Internet/GoogleScholarScraper/TestFiles/Sample0001.html")]
        [DataTestMethod]
        public void Basic_Test(string sample_filepath)
        {
            string path = GetNormalizedPathToAnyTestDataTestFile(sample_filepath);
            ASSERT.FileExists(path);

            string sample_text = GetTestFileContent(path);

            // extract URL from sample file:
            string url = null;
            string docHtml = null;

            Match match = Regex.Match(sample_text, @"<!--(.*?)-->(.*)", RegexOptions.Singleline); // counter-intuitive flag: https://stackoverflow.com/questions/159118/how-do-i-match-any-character-across-multiple-lines-in-a-regular-expression
            if (Match.Empty != match)
            {
                url = match.Groups[1].Value.Trim();
                docHtml = match.Groups[2].Value;
            }

            List<GoogleScholarScrapePaper> gssps = new List<GoogleScholarScrapePaper>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(docHtml);
            //doc.Load(ms, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false);

            ScrapeDoc(doc, url, ref gssps);

            ASSERT.IsGreaterOrEqual(gssps.Count, 1);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(gssps, Newtonsoft.Json.Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, sample_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [TestMethod]
        public void Test_Live_Scholar_Search()
        {
            //GoogleScholarScraper gs_scraper = new GoogleScholarScraper("latent dirichlet allocation", 100, true);
            string url = GenerateQueryUrl("dirichlet", 100);
            List<GoogleScholarScrapePaper> gssps = ScrapeUrl(url);

            ASSERT.IsGreaterOrEqual(gssps.Count, 1);

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
    }
}

#endregion

#endif

