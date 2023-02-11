using System;
using System.Collections.Generic;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.Language;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Exporting
{
    internal class LibraryExporter_Directories
    {
        internal static void Export(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            Export_Directories_Titles(web_library_detail, base_path, pdf_document_export_items);
            Export_Directories_Authors(web_library_detail, base_path, pdf_document_export_items);
            Export_Directories_Tags(web_library_detail, base_path, pdf_document_export_items);
            Export_Directories_AutoTags(web_library_detail, base_path, pdf_document_export_items);
        }

        private static void Export_Directories_Titles(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            string titles_base_path = Path.GetFullPath(Path.Combine(base_path, @"titles"));
            Directory.CreateDirectory(titles_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    string filename = Path.GetFullPath(Path.Combine(titles_base_path, FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk"));
                    Logging.Error("CreateShortcut: {0} --> {1}", item.filename, filename);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_Directories_Authors(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            string authors_base_path = Path.GetFullPath(Path.Combine(base_path, @"authors"));
            Directory.CreateDirectory(authors_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    List<NameTools.Name> names = NameTools.SplitAuthors(item.pdf_document.AuthorsCombined);
                    foreach (var name in names)
                    {
                        string author_base_path = Path.GetFullPath(Path.Combine(authors_base_path, name.LastName_Initials));
                        Directory.CreateDirectory(author_base_path);
                        string filename = Path.GetFullPath(Path.Combine(author_base_path, FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk"));
                        Logging.Error("CreateShortcut: {0} --> {1}", item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_Directories_Tags(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            string tags_base_path = Path.GetFullPath(Path.Combine(base_path, @"tags"));
            Directory.CreateDirectory(tags_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    foreach (string tag in TagTools.ConvertTagBundleToTags(item.pdf_document.Tags))
                    {
                        string tag_base_path = Path.GetFullPath(Path.Combine(tags_base_path, FileTools.MakeSafeFilename(tag)));
                        Directory.CreateDirectory(tag_base_path);
                        string filename = Path.GetFullPath(Path.Combine(tag_base_path, FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk"));
                        Logging.Error("CreateShortcut: {0} --> {1}", item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_Directories_AutoTags(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            string tags_base_path = Path.GetFullPath(Path.Combine(base_path, @"autotags"));
            Directory.CreateDirectory(tags_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    foreach (string tag in web_library_detail.Xlibrary.AITagManager.AITags.GetTagsWithDocument(item.pdf_document.Fingerprint))
                    {
                        string tag_base_path = Path.GetFullPath(Path.Combine(tags_base_path, FileTools.MakeSafeFilename(tag)));
                        Directory.CreateDirectory(tag_base_path);
                        string filename = Path.GetFullPath(Path.Combine(tag_base_path, FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk"));
                        Logging.Error("CreateShortcut: {0} --> {1}", item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for {0}", item.filename);
                }
            }
        }

#if WSH_SHORTCUT_ANTIQUE
        private static void CreateShortcut(WshShell shell, string filename_target, string filename_shortcut)
        {
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(filename_shortcut);
            link.TargetPath = filename_target;
            link.Save();
        }
#endif
    }
}
