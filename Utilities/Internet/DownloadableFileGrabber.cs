using System;
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
                            // Look for the required extension
                            Uri url = new Uri(href);
                            if (url.AbsolutePath.ToLower().EndsWith(extension_lower))
                            {
                                Logging.Info("Grabber/HREF: We have {0}", href);
                                results.Add(href);
                            }
                        }
                    }
                }
            }

            // Also cope with Adobe Acrobat reader pages which show a single PDF while using HTML like this:
            //
            // <html><head><meta content="width=device-width; height=device-height;" name="viewport"></head>
            // <body marginheight="0" marginwidth="0">
            //   <embed type="application/pdf" src="http://the.pdf" name="plugin" height="100%" width="100%">
            // </body></html>

            var embed_nodes = doc.DocumentNode.SelectNodes("//embed");

            if (null != embed_nodes)
            {
                foreach (var embed_node in embed_nodes)
                {
                    var type_object = embed_node.Attributes["type"];
                    var src_object = embed_node.Attributes["src"];
                    string type_value = type_object?.Value;

                    if (type_value.Contains("application/pdf"))
                    {
                        string href = src_object?.Value;

                        if (null != href)
                        {
                            Uri url = new Uri(href);
                            if (url.AbsolutePath.ToLower().EndsWith(extension_lower))
                            { 
                                Logging.Info("Grabber/EMBED: We have {0}", href);
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
