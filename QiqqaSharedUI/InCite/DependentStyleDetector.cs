using System;
using System.IO;
using System.Windows;
using System.Xml;
using Qiqqa.Common;
using Utilities;
using Utilities.GUI;
using Utilities.Internet;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.InCite
{
    internal class DependentStyleDetector
    {
        internal static bool IsDependentStyle(string style_xml_filename, out string parent_filename, out string parent_url)
        {
            string style_xml = File.ReadAllText(style_xml_filename);

            try
            {
                // Lets parse the XML and look for the info/link node
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.LoadXml(style_xml);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml_doc.NameTable);
                nsmgr.AddNamespace("csl", "http://purl.org/net/xbiblio/csl");

                XmlNode xml_node = xml_doc.SelectSingleNode("/csl:style/csl:info/csl:link", nsmgr);
                if (null != xml_node)
                {
                    string relative_type = xml_node.Attributes["rel"].InnerText;
                    if ("independent-parent" == relative_type)
                    {
                        parent_url = xml_node.Attributes["href"].InnerText;

                        // Get the rightmost "folder name" from the url - that is the name of the parent style
                        string[] url_parts = parent_url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        parent_filename = url_parts[url_parts.Length - 1] + ".csl";

                        // This is a dependent style
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There is a problem with the XML while checking if it is a dependent style");
            }

            // Thisis not a dependent style, so keep calm and carry on
            parent_filename = null;
            parent_url = null;
            return false;
        }

        internal static string GetRootStyleFilename(string style_xml_filename)
        {
            string parent_filename;
            string parent_url;
            if (IsDependentStyle(style_xml_filename, out parent_filename, out parent_url))
            {
                // Check that we have the dependent style - if we don't prompt to download it
                string full_parent_filename = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(style_xml_filename), parent_filename));
                if (!File.Exists(full_parent_filename))
                {
                    string message = String.Format(
                        "Can't find parent style for this dependent style" +
                        "\n\n" +
                        "Your style depends on a parent style named {0}, which needs to be saved in the same directory.\n\n" +
                        "It appears to be available from {1}.\n" +
                        "Shall we try to download it automatically?  If you choose NO, Qiqqa will open the website for you so you can download it manually.",
                        parent_filename, parent_url
                        );

                    if (MessageBoxes.AskQuestion(message))
                    {
                        try
                        {
                            using (MemoryStream ms = UrlDownloader.DownloadWithBlocking(parent_url))
                            {
                                File.WriteAllBytes(full_parent_filename, ms.ToArray());
                            }
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Logging.Error(ex, "You don't seem to have permission to write the new style to the directory '{0}'.\nPlease copy the original style file '{1}' to a folder where you can write (perhaps alongside your Word document), and try again.", full_parent_filename, style_xml_filename);
                            MessageBoxes.Warn("You don't seem to have permission to write the new style to the directory '{0}'.\nPlease copy the original style file '{1}' to a folder where you can write (perhaps alongside your Word document), and try again.", full_parent_filename, style_xml_filename);
                        }
                    }
                    else
                    {
                        MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(parent_url, true);
                    }
                }

                // Check again if the parent file exists, and if it does, recurse the dependency check
                if (File.Exists(full_parent_filename))
                {
                    return GetRootStyleFilename(full_parent_filename);
                }
                else
                {
                    // We need the parent style, but haven't managed to download it, so return nothing...
                    return null;
                }
            }
            else // Not a dependent style, so use this filename
            {
                return style_xml_filename;
            }
        }
    }
}


/*

<style xmlns="http://purl.org/net/xbiblio/csl" class="in-text" version="1.0">
<info>
    <title>
    Academic Medicine (Formerly Journal of Medical Education)
    </title>
    <id>http://www.zotero.org/styles/academic-medicine</id>
    <link href="http://www.zotero.org/styles/vancouver" rel="independent-parent"/>
    <category citation-format="numeric"/>
    <category field="medicine"/>
    <updated>2011-05-18T00:31:02+00:00</updated>
</info>
</style>

 */
