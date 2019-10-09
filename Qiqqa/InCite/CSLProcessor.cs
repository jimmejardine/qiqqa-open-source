using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.InCite
{
    public class CSLProcessor
    {
        private static readonly string BASE_PATH = ConfigurationManager.Instance.TempDirectoryForQiqqa + @"InCite\";

        public class BrowserThreadPassThru
        {
            public WordConnector word_connector;
            public string style_file;
            public bool is_note_format;
        }

        # region --- Snippets --------------------------------------------------------------------------------------------------------------

        public static void GenerateRtfCitationSnippet(bool suppress_messages, List<PDFDocument> pdf_documents, string style_file, Library primary_library, CSLProcessorOutputConsumer.BibliographyReadyDelegate brd = null)
        {
            if (null == brd) brd = GenerateRtfCitationSnippet_OnBibliographyReady;

            // Build up all the files we need to run the CSL processor
            EnsureWorkingDirectoryIsPukka_WithCode();
            bool is_note_format;
            EnsureWorkingDirectoryIsPukka_WithStyle(style_file, out is_note_format);
            string citations_javascript = EnsureWorkingDirectoryIsPukka_WithCitations_Snippet(suppress_messages, pdf_documents, primary_library);
            CSLProcessorOutputConsumer csl_poc = new CSLProcessorOutputConsumer(BASE_PATH, citations_javascript, brd, null);
        }

        static void GenerateRtfCitationSnippet_OnBibliographyReady(CSLProcessorOutputConsumer ip)
        {
            if (ip.success)
            {
                string snippet_rtf = ip.GetRtf();

                // Set the clipbaord
                ClipboardTools.SetRtf(snippet_rtf);
                StatusManager.Instance.UpdateStatus("InCiteCitationSnippet", String.Format("{0} citation(s) copied to the clipboard.", ip.bibliography.Count));
            }
            
            else
            {
                StatusManager.Instance.UpdateStatus("InCiteCitationSnippet", String.Format("Error generating citation(s)."));
            }
        }

        #endregion

        # region --- CSLEditor --------------------------------------------------------------------------------------------------------------

        public static void GenerateCSLEditorCitations(string style_file_filename, string prepared_citation_javascript, CSLProcessorOutputConsumer.BibliographyReadyDelegate brd)
        {
            // Build up all the files we need to run the CSL processor
            EnsureWorkingDirectoryIsPukka_WithCode();
            bool is_note_format;
            EnsureWorkingDirectoryIsPukka_WithStyle(style_file_filename, out is_note_format);
            string citations_javascript = EnsureWorkingDirectoryIsPukka_WithCitations_CSLEditor(prepared_citation_javascript);
            CSLProcessorOutputConsumer csl_poc = new CSLProcessorOutputConsumer(BASE_PATH, citations_javascript, brd, null);
        }

        #endregion

        public static void RefreshDocument(WordConnector word_connector, string style_file, Library primary_library)
        {
            // Build up all the files we need to run the CSL processor
            StatusManager.Instance.UpdateStatus("InCite", "Building InCite code scripts", 1, 5);
            EnsureWorkingDirectoryIsPukka_WithCode();
            bool is_note_format;
            StatusManager.Instance.UpdateStatus("InCite", "Building InCite style scripts", 2, 5);
            EnsureWorkingDirectoryIsPukka_WithStyle(style_file, out is_note_format);
            StatusManager.Instance.UpdateStatus("InCite", "Building InCite citations", 3, 5);
            string citations_javascript = EnsureWorkingDirectoryIsPukka_WithCitations_Word(word_connector, primary_library);

            // Run the CSL processor - it will pick up from OnBibliographyReady()
            BrowserThreadPassThru passthru = new BrowserThreadPassThru { word_connector = word_connector, style_file = style_file, is_note_format = is_note_format };
            CSLProcessorOutputConsumer csl_poc = new CSLProcessorOutputConsumer(BASE_PATH, citations_javascript, RefreshDocument_OnBibliographyReady, passthru);
        }

        static void RefreshDocument_OnBibliographyReady(CSLProcessorOutputConsumer ip)
        {
            BrowserThreadPassThru passthru = (BrowserThreadPassThru)ip.user_argument;

            try
            {
                Thread thread = new Thread(passthru.word_connector.RepopulateFromCSLProcessor);
                //thread.IsBackground = true;
                //thread.Priority = ThreadPriority.Lowest;
                thread.Name = "CSLProcessor:Refresh";
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(ip);
            }
            catch (Exception ex)
            {
                SafeThreadPool.QueueUserWorkItem(o => 
                    {
                        throw new Exception(String.Format("Exception while generating bibliography with style '{0}'", passthru.style_file), ex);
                    }
                );                
            }
        }

        public static string CITATION_RESOURCES_SUBDIRECTORY
        {
            get
            {
                string citation_resources_subdirectory = ConfigurationManager.Instance.StartupDirectoryForQiqqa + @"InCite\resources";
                return citation_resources_subdirectory;
            }
        }

        private static void EnsureWorkingDirectoryIsPukka_WithCode()
        {
            // Renew the directory            
            DirectoryTools.CreateDirectory(BASE_PATH);

            // Copy in the important files
            string citation_resources_subdirectory = CITATION_RESOURCES_SUBDIRECTORY;
            List<string> citation_resources = DirectoryTools.GetSubFiles(citation_resources_subdirectory, "*");
            foreach (string citation_resource in citation_resources)
            {
                if (citation_resource.Contains("default_abbreviations")) continue;

                string source_filename = Path.GetFileName(citation_resource);
                string dest_filename = BASE_PATH + source_filename;
                File.Copy(citation_resource, dest_filename, true);
            }
        }

        private static void AppendCopyright(StringBuilder sb)
        {
            sb.AppendLine("// ------------------------------------------------------------------");
            sb.AppendLine("// --- © Copyright 2010-2016 Quantisle Ltd.  All Rights Reserved. ---");
            sb.AppendLine("// ------------------------------------------------------------------");
            sb.AppendLine();
        }

        private static void EnsureWorkingDirectoryIsPukka_WithStyle(string style_file_filename, out bool is_note_format)
        {
            // Load the style file
            string style = File.ReadAllText(style_file_filename);

            // Is this a NOTE format style?
            is_note_format = style.ToLower().Contains("class=\"note\"");

            // Remove everything up to the first <style> element
            int start_pos = style.IndexOf("<style");
            if (0 <= start_pos)
            {
                style = style.Substring(start_pos);
            }
            else
            {
                Logging.Warn("There is no <style> element in the file...?");
            }

            // Escape all the quotes
            style = style.Replace("\"", "\\\"");

            // Remove all teh linefeeds
            style = style.Replace("\n", "");
            style = style.Replace("\r", "");

            // Wrap in javascript
            StringBuilder sb = new StringBuilder();
            AppendCopyright(sb);
            sb.AppendFormat("var CITATION_STYLE = \"{0}\";", style);
            sb.AppendLine();

            string style_filename = BASE_PATH + "load_csl.js";
            File.WriteAllText(style_filename, sb.ToString());
        }

        private static string EnsureWorkingDirectoryIsPukka_WithCitations_Word(WordConnector word_connector, Library primary_library)
        {
            // Get all the citations from the word doc
            List<CitationCluster> citation_clusters = word_connector.GetAllCitationClustersFromCurrentDocument();
            if (0 == citation_clusters.Count)
            {
                MessageBoxes.Info("Qiqqa InCite could not find any citations in your Word document.  Before waving the magic wand, you need to have added some citation to the current active Word document.");
            }
            return EnsureWorkingDirectoryIsPukka_WithCitations_Common(citation_clusters, primary_library);
        }

        private static string EnsureWorkingDirectoryIsPukka_WithCitations_Snippet(bool suppress_messages, List<PDFDocument> pdf_documents, Library primary_library)
        {
            List<PDFDocument> skipped_pdf_documents = new List<PDFDocument>();

            // Create the citation clusters
            List<CitationCluster> citation_clusters = new List<CitationCluster>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                string bibtex_key = pdf_document.BibTex.Key;
                if (!String.IsNullOrEmpty(bibtex_key))
                {
                    CitationCluster cc = new CitationCluster();
                    CitationItem ci = new CitationItem(bibtex_key, pdf_document.Library.WebLibraryDetail.Id);
                    cc.citation_items.Add(ci);
                    citation_clusters.Add(cc);
                }
                else
                {
                    skipped_pdf_documents.Add(pdf_document);
                }
            }

            // Warn if we had to skip any citations
            if (!suppress_messages)
            {
                if (0 != skipped_pdf_documents.Count)
                {
                    MessageBoxes.Warn("We have had to skip {0} document(s) because they had no associated BibTeX record.", skipped_pdf_documents.Count);
                }
            }

            return EnsureWorkingDirectoryIsPukka_WithCitations_Common(citation_clusters, primary_library);
        }

        private static string EnsureWorkingDirectoryIsPukka_WithCitations_CSLEditor(string citation_javascript)
        {
            return EnsureWorkingDirectoryIsPukka_WithCitations_Common_FileWrite(citation_javascript);
        }

        private static string EnsureWorkingDirectoryIsPukka_WithCitations_Common(List<CitationCluster> citation_clusters, Library primary_library)
        {
            // Get all the equivalent bibtexes from the libraries
            Dictionary<string, CSLProcessorBibTeXFinder.MatchingBibTeXRecord> bitex_items = CSLProcessorBibTeXFinder.Find(citation_clusters, primary_library);

            // Get the uses of these citations
            string citation_uses = CSLProcessorTranslator_CitationClustersToJavaScript.Translate(citation_clusters);

            // Get the abbreviations (if any)
            Dictionary<string, string> abbreviations = CSLProcessorTranslator_AbbreviationsManager.GetAbbreviations();

            // Translate each bibtex into the corresponding csl javascript record
            string citation_init = CSLProcessorTranslator_BibTeXToJavaScript.Translate_INIT(bitex_items);
            string citation_database = CSLProcessorTranslator_BibTeXToJavaScript.Translate_DATABASE(bitex_items, abbreviations);

            // Write
            {
                StringBuilder sb = new StringBuilder();

                AppendCopyright(sb);
                sb.AppendLine(citation_uses);
                sb.AppendLine();

                AppendCopyright(sb);
                sb.AppendLine(citation_init);
                sb.AppendLine();

                AppendCopyright(sb);
                sb.AppendLine(citation_database);
                sb.AppendLine();

                AppendCopyright(sb);

                string citation_javascript = sb.ToString();
                Logging.Info(citation_javascript);

                return EnsureWorkingDirectoryIsPukka_WithCitations_Common_FileWrite(citation_javascript);
            }
        }

        private static string EnsureWorkingDirectoryIsPukka_WithCitations_Common_FileWrite(string citation_javascript)
        {
            string citation_filename = BASE_PATH + "load_citations.js";
            File.WriteAllText(citation_filename, citation_javascript);
            return citation_javascript;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;            
            Thread.Sleep(1000);

            CSLProcessor csl = new CSLProcessor();
            //string style_filename = @"c:\temp\vancouver.csl";
            string style_filename = @"c:\temp\harvard1.csl";
            RefreshDocument(WordConnector.Instance, style_filename, null);
        }
#endif

        #endregion
    }
}
