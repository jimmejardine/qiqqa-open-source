using System.Collections.Generic;
using HtmlAgilityPack;

namespace Utilities.Internet
{
    public class DownloadableFileGrabber
    {
        public static List<string> Grab(string html, string extension)
        {
            string extension_lower = extension.ToLower();

            List<string> results = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var a_nodes = doc.DocumentNode.SelectNodes("//a");

            if (null != a_nodes)
            {
                foreach (var a_node in a_nodes)
                {
                    var href_object = a_node.Attributes["href"];
                    if (null != href_object)
                    {
                        string href = href_object.Value;
                        if (null != href)
                        {
                            // Strip off everything after the #
                            int hash_position = href.IndexOf('#');
                            if (-1 != hash_position)
                            {
                                href = href.Substring(0, hash_position);
                            }

                            // Look for the required extension
                            if (href.ToLower().EndsWith(extension_lower))
                            {
                                //Logging.Info("We have {0}", href);
                                results.Add(href);
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
