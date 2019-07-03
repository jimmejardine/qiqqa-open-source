using System;
using System.Collections.Generic;
using System.IO;
using IWshRuntimeLibrary;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.Language;

namespace Qiqqa.Exporting
{
    class LibraryExporter_Directories
    {
        internal static void Export(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            Export_Directories_Titles(library, base_path, pdf_document_export_items);
            Export_Directories_Authors(library, base_path, pdf_document_export_items);
            Export_Directories_Tags(library, base_path, pdf_document_export_items);
            Export_Directories_AutoTags(library, base_path, pdf_document_export_items);
        }

        private static void Export_Directories_Titles(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            WshShell shell = new WshShell();

            string titles_base_path = base_path + @"titles\";
            Directory.CreateDirectory(titles_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {   
                    string filename = titles_base_path + FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk";
                    CreateShortcut(shell, item.filename, filename);                    
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }
        
        private static void Export_Directories_Authors(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            WshShell shell = new WshShell();

            string authors_base_path = base_path + @"authors\";
            Directory.CreateDirectory(authors_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    List<NameTools.Name> names = NameTools.SplitAuthors(item.pdf_document.AuthorsCombined, PDFDocument.UNKNOWN_AUTHORS);
                    foreach (var name in names)
                    {
                        string author_base_path = authors_base_path + name.LastName_Initials + @"\";
                        Directory.CreateDirectory(author_base_path);
                        string filename = author_base_path + FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk";
                        CreateShortcut(shell, item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_Directories_Tags(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            WshShell shell = new WshShell();

            string tags_base_path = base_path + @"tags\";
            Directory.CreateDirectory(tags_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    foreach (string tag in TagTools.ConvertTagBundleToTags(item.pdf_document.Tags))
                    {
                        string tag_base_path = tags_base_path + FileTools.MakeSafeFilename(tag) + @"\";
                        Directory.CreateDirectory(tag_base_path);
                        string filename = tag_base_path + FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk";
                        CreateShortcut(shell, item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void Export_Directories_AutoTags(Library library, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            WshShell shell = new WshShell();

            string tags_base_path = base_path + @"autotags\";
            Directory.CreateDirectory(tags_base_path);

            foreach (var item in pdf_document_export_items.Values)
            {
                try
                {
                    foreach (string tag in library.AITagManager.AITags.GetTagsWithDocument(item.pdf_document.Fingerprint))
                    {
                        string tag_base_path = tags_base_path + FileTools.MakeSafeFilename(tag) + @"\";
                        Directory.CreateDirectory(tag_base_path);
                        string filename = tag_base_path + FileTools.MakeSafeFilename(item.pdf_document.TitleCombined) + ".lnk";
                        CreateShortcut(shell, item.filename, filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error creating shortcut for " + item.filename);
                }
            }
        }

        private static void CreateShortcut(WshShell shell, string filename_target, string filename_shortcut)
        {
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(filename_shortcut);
            link.TargetPath = filename_target;
            link.Save();
        }
    }
}
