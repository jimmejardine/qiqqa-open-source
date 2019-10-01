using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Collections;
using Utilities.Language;

namespace Qiqqa.Exporting
{
    class LibraryExporter_HTML
    {
        internal static void Export(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<html>\n");
            html.AppendFormat("  <head>\n");
            html.AppendFormat("    <title>Qiqqa Library Export</title>\n");
            html.AppendFormat("  </head>\n");
            html.AppendFormat("<body>\n");
            html.AppendFormat(String.Format("<a href=\"{0}\"><image align=\"right\" src=\"Qiqqa.png\" border=\"0\"/></a>\n", WebsiteAccess.Url_QiqqaLibraryExportTrackReference));
            html.AppendFormat("<h1>Qiqqa Library Export</h1>\n");

            Export_HTML_Titles(html, library, base_path, pdf_document_export_items);
            Export_HTML_Authors(html, library, base_path, pdf_document_export_items);
            Export_HTML_Tags(html, library, base_path, pdf_document_export_items);
            Export_HTML_AutoTags(html, library, base_path, pdf_document_export_items);

            html.AppendFormat("</body>\n");
            html.AppendFormat("</html>\n");


            string image_filename = base_path + @"Qiqqa.png";
            BitmapImage image = Icons.GetAppIcon(Icons.Qiqqa);
            using (FileStream filestream = new FileStream(image_filename, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(filestream);
            }
            
            string filename = base_path + @"Qiqqa.html";
            File.WriteAllText(filename, html.ToString());
        }

        private static void DumpSortedItems(StringBuilder html, List<PDFDocumentExportItem> items)
        {
            items.Sort(delegate(PDFDocumentExportItem a, PDFDocumentExportItem b) { return String.Compare(a.pdf_document.TitleCombined, b.pdf_document.TitleCombined); });

            foreach (var item in items)
            {
                try
                {
                    html.AppendFormat("<li>");

                    html.AppendFormat(
                        "<a href=\"file://{0}\">{1}</a>\n",
                        WebUtility.HtmlEncode(item.filename),
                        WebUtility.HtmlEncode(item.pdf_document.TitleCombined)
                        );
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_HTML_Titles(StringBuilder html, Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            html.AppendFormat("<p/>\n");
            html.AppendFormat("<h2>{0}</h2>\n", "Titles");
            html.AppendFormat("<ul>\n");

            List<PDFDocumentExportItem> items = new List<PDFDocumentExportItem>(pdf_document_export_items.Values);
            
            DumpSortedItems(html, items);

            html.AppendFormat("</ul>\n");
        }

        private static void Export_HTML_XXX(StringBuilder html, string header, MultiMap<string, PDFDocumentExportItem> items_sliced)
        {
            html.AppendFormat("<p/>\n");
            html.AppendFormat("<h2>{0}</h2>\n", header);
            html.AppendFormat("<ul>\n");

            List<string> items_keys = new List<string>(items_sliced.Keys);
            items_keys.Sort();
            foreach (var item_key in items_keys)
            {
                var item_value = items_sliced[item_key];

                html.AppendFormat("<li>{0}\n", WebUtility.HtmlEncode(item_key));
                html.AppendFormat("<ul>\n");

                List<PDFDocumentExportItem> items = new List<PDFDocumentExportItem>(item_value);

                DumpSortedItems(html, items);

                html.AppendFormat("</ul>\n");
            }

            html.AppendFormat("</ul>\n");
        }

        private static void Export_HTML_Authors(StringBuilder html, Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            MultiMap<string, PDFDocumentExportItem> items_sliced = new MultiMap<string, PDFDocumentExportItem>(false);

            foreach (var item in pdf_document_export_items.Values)
            {
                List<NameTools.Name> names = NameTools.SplitAuthors(item.pdf_document.AuthorsCombined);
                foreach (var name in names)
                {
                    items_sliced.Add(name.LastName_Initials, item);
                }
            }

            Export_HTML_XXX(html, "Authors", items_sliced);
        }

        private static void Export_HTML_Tags(StringBuilder html, Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            MultiMap<string, PDFDocumentExportItem> items_sliced = new MultiMap<string, PDFDocumentExportItem>(false);

            foreach (var item in pdf_document_export_items.Values)
            {
                foreach (string tag in TagTools.ConvertTagBundleToTags(item.pdf_document.Tags))
                {
                    items_sliced.Add(tag, item);
                }
            }

            Export_HTML_XXX(html, "Tags", items_sliced);
        }

        private static void Export_HTML_AutoTags(StringBuilder html, Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            MultiMap<string, PDFDocumentExportItem> items_sliced = new MultiMap<string, PDFDocumentExportItem>(false);

            foreach (var item in pdf_document_export_items.Values)
            {
                foreach (string tag in library.AITagManager.AITags.GetTagsWithDocument(item.pdf_document.Fingerprint))
                {
                    items_sliced.Add(tag, item);
                }
            }

            Export_HTML_XXX(html, "AutoTags", items_sliced);
        }
    }
}
