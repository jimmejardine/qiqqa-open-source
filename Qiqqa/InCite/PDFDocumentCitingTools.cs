using System;
using System.Collections.Generic;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.InCite
{
    internal class PDFDocumentCitingTools
    {
        internal static void RefreshBibliography()
        {
            try
            {
                string last_style_filename = GetLastStyleFilename();
                if (null != last_style_filename)
                {
                    CSLProcessor.RefreshDocument(WordConnector.Instance, last_style_filename, null);
                }
            }

            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while refreshing bibliography.");
                MessageBoxes.Error("There has been a problem while trying to add the citation.  Please check that Microsoft Word is running.\n\nIf the problem persists, perhaps open InCite from the Start Page as it may offer more details about the problem.");
            }
        }

        public static void CitePDFDocument(PDFDocument pdf_document, bool separate_author_and_date)
        {
            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            pdf_documents.Add(pdf_document);
            CitePDFDocuments(pdf_documents, separate_author_and_date);
        }


        public static void CitePDFDocuments(List<PDFDocument> pdf_documents, bool separate_author_and_date)
        {
            try
            {
                foreach (var pdf_document in pdf_documents)
                {
                    if (String.IsNullOrEmpty(pdf_document.BibTex))
                    {
                        MessageBoxes.Warn("One or more of your documents have no associated BibTeX information.  Please add some or use the BibTeX Sniffer to locate it on the Internet.");
                        return;
                    }
                }

                CitationCluster citation_cluster = GenerateCitationClusterFromPDFDocuments(pdf_documents);
                citation_cluster.citation_items[0].SeparateAuthorsAndDate(separate_author_and_date);

                if (null != citation_cluster)
                {
                    WordConnector.Instance.WaitForAtLeastOneIteration();
                    WordConnector.Instance.AppendCitation(citation_cluster);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while citing PDFDocument.");
                MessageBoxes.Error("There has been a problem while trying to add the citation.  Please check that Microsoft Word is running.\n\nIf the problem persists, perhaps open InCite from the Start Page as it may offer more details about the problem.");
            }
        }

        public static CitationCluster GenerateCitationClusterFromPDFDocuments(List<PDFDocument> selected_pdf_documents)
        {
            foreach (PDFDocument pdf_document in selected_pdf_documents)
            {
                pdf_document.DateLastCited = DateTime.UtcNow;
            }

            // Check if any of these documents have duplicated BibTeX keys...
            foreach (PDFDocument pdf_document in selected_pdf_documents)
            {
                string key = pdf_document.BibTexKey;
                if (!String.IsNullOrEmpty(key))
                {
                    foreach (PDFDocument pdf_document_other in pdf_document.Library.PDFDocuments)
                    {
                        if (pdf_document_other != pdf_document)
                        {
                            if (!String.IsNullOrEmpty(pdf_document_other.BibTex) && pdf_document_other.BibTex.Contains(key))
                            {
                                if (pdf_document_other.BibTexKey == key)
                                {
                                    MessageBoxes.Warn(String.Format("There are several documents in your library with the same BibTeX key '{0}'.  Unless they are the same PDF, you should give them different keys or else InCite will just pick the first matching document.", key));
                                }
                            }
                        }
                    }
                }
            }

            // Check if any of these documents have no BibTeX key...
            foreach (PDFDocument pdf_document in selected_pdf_documents)
            {
                string key = pdf_document.BibTexKey;
                if (String.IsNullOrEmpty(key))
                {
                    MessageBoxes.Warn(String.Format("Some of your documents (e.g. with title, '{0}') have no BibTeX key.  Without a unique BibTeX key, Qiqqa has no way of referencing this document from Word, and they will show up in Word as either [ ] or a blank when you wave the magic wand.  Please add any missing keys and try again!", pdf_document.TitleCombined));
                }
            }

            if (0 < selected_pdf_documents.Count)
            {
                CitationCluster cc = new CitationCluster();
                foreach (PDFDocument selected_pdf_document in selected_pdf_documents)
                {
                    string reference_key = selected_pdf_document.BibTexKey;
                    if (null != reference_key)
                    {
                        string reference_library = selected_pdf_document.Library.WebLibraryDetail.Id;
                        CitationItem ci = new CitationItem(reference_key, reference_library);
                        cc.citation_items.Add(ci);
                    }
                    else
                    {
                        MessageBoxes.Warn("Could not add document '{0}' because it doesn't have valid BibTeX or a valid BibTeX key.", selected_pdf_document.TitleCombined);
                    }
                }

                return cc;
            }

            MessageBoxes.Warn("You have not selected any document(s) to cite.");
            return null;
        }

        private static string GetLastStyleFilename()
        {
            string style_filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_LastStyleFile;
            string new_style_filename = FindValidStyleFilename(style_filename);
            if (0 != String.Compare(new_style_filename, style_filename))
            {
                Logging.Warn("Using suggested CSL file {0} instead of missing file {1}", new_style_filename, style_filename);
            }

            if (!String.IsNullOrEmpty(new_style_filename))
            {
                return new_style_filename;
            }
            else
            {
                if (MessageBoxes.AskQuestion("You need to please select a CSL style file for your citations.\nDo you want to open InCite to choose one now?"))
                {
                    MainWindowServiceDispatcher.Instance.OpenInCite();
                }

                return null;
            }
        }

        internal static void CiteSnippetPDFDocument(bool suppress_messages, PDFDocument pdf_document, CSLProcessorOutputConsumer.BibliographyReadyDelegate brd = null)
        {
            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            pdf_documents.Add(pdf_document);
            CiteSnippetPDFDocuments(suppress_messages, pdf_documents, brd);
        }

        internal static void CiteSnippetPDFDocuments(bool suppress_messages, List<PDFDocument> pdf_documents, CSLProcessorOutputConsumer.BibliographyReadyDelegate brd = null)
        {
            foreach (var pdf_document in pdf_documents)
            {
                pdf_document.DateLastCited = DateTime.UtcNow;
            }

            string last_style_filename = GetLastStyleFilename();
            if (null != last_style_filename)
            {
                CSLProcessor.GenerateRtfCitationSnippet(suppress_messages, pdf_documents, last_style_filename, null, brd);
            }
        }

        private static readonly Lazy<string> __BASE_STYLE_DIRECTORY = new Lazy<string>(() => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"InCite/styles")));
            internal static string BASE_STYLE_DIRECTORY => __BASE_STYLE_DIRECTORY.Value;

        private static readonly Lazy<string> __BASE_REGISTRY_FIXES_DIRECTORY = new Lazy<string>(() => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"InCite/registry_fixes")));
            internal static string BASE_REGISTRY_FIXES_DIRECTORY => __BASE_REGISTRY_FIXES_DIRECTORY.Value;

        internal static string FindValidStyleFilename(string style_filename)
        {
            // If this filename no longer exists, check if we can find the same filename in the default directory
            if (File.Exists(style_filename))
            {
                return style_filename;
            }
            else
            {
                string new_style_filename = Path.GetFullPath(Path.Combine(BASE_STYLE_DIRECTORY, Path.GetFileName(style_filename)));
                if (File.Exists(new_style_filename))
                {
                    return new_style_filename;
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
