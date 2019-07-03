using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Utilities.Internet.GoogleScholar
{
    public class GoogleScholarScraper
    {
        public static string GenerateQueryUrl(string query, int num_items)
        {
            string url =
                "http://scholar.google.com/scholar?"
                + "q=" + Uri.EscapeDataString(query)
                + "&num=" + num_items
                ;

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

        public static List<GoogleScholarScrapePaper> ScrapeUrl(IWebProxy proxy, string url)
        {
            List<GoogleScholarScrapePaper> gssps = new List<GoogleScholarScrapePaper>();

            MemoryStream ms = new MemoryStream();
            try
            {
                WebHeaderCollection header_collection = new WebHeaderCollection();
                UrlDownloader.DownloadWithBlocking(proxy, url, out ms, out header_collection);

                HtmlDocument doc = new HtmlDocument();
                doc.Load(ms);

                ScrapeDoc(doc, url, gssps);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem parsing the GoogleScholar url");
            }
            finally
            {
                if (ms != null) ms.Dispose();
            }

            return gssps;
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
            HtmlNodeCollection NoAltElements_outer = doc.DocumentNode.SelectNodes("//*[@class='gs_r']");

            if (null != NoAltElements_outer)
            {
                foreach (HtmlNode element in NoAltElements_outer)
                {
                    GoogleScholarScrapePaper gssp = new GoogleScholarScrapePaper();

                    string element_html = element.OuterHtml;

                    HtmlDocument item_doc = new HtmlDocument();
                    item_doc.LoadHtml(element_html);

                    HtmlNode NoAltElements = item_doc.DocumentNode.SelectNodes("//*[@class='gs_r']")[0];

                    var title_node = NoAltElements.SelectNodes("//*[@class='gs_rt']");
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

                    {
                        var source_url_node = title_node[0].SelectNodes("a");
                        if (null != source_url_node)
                        {
                            gssp.source_url = WebUtility.HtmlDecode(source_url_node[0].Attributes["href"].Value);
                        }
                    }

                    {
                        var authors_node = NoAltElements.SelectNodes("//*[@class='gs_a']");
                        if (null != authors_node)
                        {
                            gssp.authors = WebUtility.HtmlDecode(authors_node[0].InnerHtml);
                        }
                    }

                    // Pull out the abstract
                    {
                        var abstract_node = NoAltElements.SelectNodes("//*[@class='gs_rs']");
                        if (null != abstract_node)
                        {
                            gssp.abstract_html = WebUtility.HtmlDecode(abstract_node[0].InnerText);
                        }
                    }

                    // Pull out the potential downloads
                    {
                        var downloads_node = NoAltElements.SelectNodes("//*[@class='gs_md_wp gs_ttss']");
                        if (null != downloads_node)
                        {
                            foreach (var child_node in downloads_node[0].ChildNodes)
                            {
                                if ("a" == child_node.Name)
                                {
                                    string download_url = child_node.Attributes["href"].Value;
                                    gssp.download_urls.Add(download_url);
                                    Logging.Info("Downloadable from {0}", download_url);
                                }
                            }
                        }
                    }

                    var see_also_nodes = NoAltElements.SelectNodes("//*[@class='gs_fl']/a");
                    GetUrlForRelatedList(url, "Cited by", see_also_nodes, out gssp.cited_by_header, out gssp.cited_by_url);
                    GetUrlForRelatedList(url, "Related", see_also_nodes, out gssp.related_articles_header, out gssp.related_articles_url);
                    GetUrlForRelatedList(url, "Import into BibTeX", see_also_nodes, out gssp.bibtex_header, out gssp.bibtex_url);

                    gssps.Add(gssp);
                }
            }
        }

        private static void GetUrlForRelatedList(string base_url, string related_list_header, HtmlNodeCollection nodes, out string header, out string url)
        {
            header = null;
            url = null;

            if (null != nodes)
            {
                foreach (var node in nodes)
                {
                    try
                    {
                        if (node.InnerText.StartsWith(related_list_header))
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
                        Logging.Warn(ex, "Trouble while trying to convert URL");
                    }
                }
            }
        }
        
        public static void Test()
        {
            //GoogleScholarScraper gs_scraper = new GoogleScholarScraper("latent dirichlet allocation", 100, true);
            string url = GenerateQueryUrl("dirichlet", 100);
            List<GoogleScholarScrapePaper> gssps = ScrapeUrl(null, url);
            foreach (var gs_paper in gssps)
            {
                /*
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
                 * */

                Logging.Info(gs_paper.ToString());
                
            }
        }
    }
}
