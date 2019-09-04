using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Utilities.Internet
{
    public class DownloadableFileGrabber
    {
        public static List<string> Grab(Uri base_uri, string html, string extension)
        {
            string extension_lower = "." + extension.ToLower();

            // use a hashset so we don't add duplicate entries as one PDF *can* be
            // referenced multiple times on the same page: in such a case, we only
            // want it mentioned *once* in the result set.
            HashSet<string> results = new HashSet<string>();

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
                            Uri url;
                            try
                            {
                                // handle both relative and absolute URIs in one go
                                url = new Uri(base_uri, href);
                                if (url.AbsolutePath.ToLower().EndsWith(extension_lower))
                                {
                                    Logging.Info("Grabber/HREF: We have {0}", href);
                                    results.Add(href);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Grabber/HREF: Failed to interpret {0} as a URI: skipping this entry", href);
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
                            Uri url;
                            try
                            {
                                // handle both relative and absolute URIs in one go
                                url = new Uri(base_uri, href);
                                // WARNING: a PDF URI does *not* have to include a PDF extension!
                                // Case in point:
                                //   https://pubs.acs.org/doi/pdf/10.1021/ed1010618?rand=zf7t0csx
                                // is an example of such a URI: this URI references a PDF but DOES NOT
                                // contain the string ".pdf" itself!
                                //
                                //if (url.AbsolutePath.ToLower().EndsWith(extension_lower))
                                {
                                    Logging.Info("Grabber/HREF: We have {0}", href);
                                    results.Add(href);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "Grabber/HREF: Failed to interpret {0} as a URI: skipping this entry", href);
                            }
                        }
                    }
                }
            }

            return new List<string>(results);
        }
    }
}
