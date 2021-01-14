using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.OCR;
#if SORAX_ANTIQUE
using Utilities.PDF.Sorax;
#endif
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class PDFRenderer
    {
        public const int TEXT_PAGES_PER_GROUP = 20;

        private string pdf_filename;
        private string pdf_user_password;
        private string pdf_owner_password;
        private string document_fingerprint;
        private PDFRendererFileLayer pdf_render_file_layer;
        private Dictionary<int, TypedWeakReference<WordList>> texts = new Dictionary<int, TypedWeakReference<WordList>>();
        private object texts_lock = new object();

        //List<int> pages_to_render = new List<int>();

        public delegate void OnPageTextAvailableDelegate(int page_from, int page_to);
        public event OnPageTextAvailableDelegate OnPageTextAvailable;

#if SORAX_ANTIQUE
        private SoraxPDFRenderer sorax_pdf_renderer;
#endif

        public PDFRenderer(string pdf_filename, string pdf_user_password, string pdf_owner_password)
            : this(null, pdf_filename, pdf_user_password, pdf_owner_password)
        {
        }

        public PDFRenderer(string precomputed_document_fingerprint, string pdf_filename, string pdf_user_password, string pdf_owner_password)
        {
            this.pdf_filename = pdf_filename;
            this.pdf_user_password = pdf_user_password;
            this.pdf_owner_password = pdf_owner_password;
            document_fingerprint = precomputed_document_fingerprint ?? StreamFingerprint.FromFile(this.pdf_filename);

            pdf_render_file_layer = new PDFRendererFileLayer(document_fingerprint, pdf_filename);
#if SORAX_ANTIQUE
            sorax_pdf_renderer = new SoraxPDFRenderer(pdf_filename, pdf_user_password, pdf_owner_password);
#endif
        }

        public string PDFFilename => pdf_filename;

        public string PDFUserPassword => pdf_user_password;

        public PDFRendererFileLayer PDFRendererFileLayer => pdf_render_file_layer;

        /// <summary>
        /// Returns 0 when
        /// - page count has not been calculated yet (pending action)
        /// - or PDF is damaged so badly that no page count can be determined.
        ///
        /// Otherwise returns the number of pages in the PDF document.
        /// </summary>
        public int PageCount => pdf_render_file_layer.PageCount;

        public string DocumentFingerprint => document_fingerprint;

        public override string ToString()
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (texts_lock)
            {
                //l1_clk.LockPerfTimerStop();
                return string.Format(
                    "{0}T/{1} - {2}",
                    texts.Count,
                    PageCount,
                    document_fingerprint
                );
            }
        }

        /// <summary>
        /// Page is 1 based
        /// </summary>
        /// <param name="page"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        internal byte[] GetPageByHeightAsImage(int page, double height)
        {
#if SORAX_ANTIQUE
            return sorax_pdf_renderer.GetPageByHeightAsImage(page, height);
#else
            return new byte[] { };
#endif
        }

        internal byte[] GetPageByDPIAsImage(int page, float dpi)
        {
#if SORAX_ANTIQUE
            return sorax_pdf_renderer.GetPageByDPIAsImage(page, dpi);
#else
            return new byte[] { };
#endif
        }

            public void CauseAllPDFPagesToBeOCRed()
        {
            // jobqueue this one too - saves us one PDF access + parse action inline when invoked in the UI thread by OpenDocument()
            int pgcount = PageCount;
            SafeThreadPool.QueueUserWorkItem(o =>
            {
                for (int i = pgcount; i >= 1; --i)
                {
                    GetOCRText(i);
                }
            });
        }

        /// <summary>
        /// Returns the OCR words on the page.  Null if the words are not yet available.
        /// The page will be queued for OCRing if they are not available...
        /// Page is 1 based...
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public WordList GetOCRText(int page, bool queue_for_ocr = true)
        {
            if (page > PageCount || page < 1)
            {
                // dump stacktrace with this one so we know who instigated this out-of-bounds request.
                //
                // Boundary issue was discovered during customer log file analysis (log files courtesy of Chris Hicks)
                try
                {
                    throw new ArgumentException($"INTERNAL ERROR: requesting page text for page {page} which lies outside the detected document page range 1..{PageCount} for PDF document {DocumentFingerprint}");
                }
                catch (Exception ex)
                {
                    Logging.Error(ex);
                }
            }

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (texts_lock)
            {
                //l1_clk.LockPerfTimerStop();

                // First check our cache
                {
                    TypedWeakReference<WordList> word_list_weak;
                    texts.TryGetValue(page, out word_list_weak);
                    if (null != word_list_weak)
                    {
                        WordList word_list = word_list_weak.TypedTarget;
                        if (null != word_list)
                        {
                            return word_list;
                        }
                    }
                }

                // Then check for an existing SINGLE file
                {
                    string filename = pdf_render_file_layer.MakeFilename_TextSingle(page);
                    try
                    {
                        if (File.Exists(filename))
                        {
                            // Get this ONE page
                            Dictionary<int, WordList> word_lists = WordList.ReadFromFile(filename, page);
                            WordList word_list = word_lists[page];
                            if (null == word_list)
                            {
                                throw new Exception(String.Format("No words on page {0} in OCR file {1}", page, filename));
                            }
                            texts[page] = new TypedWeakReference<WordList>(word_list);
                            return word_list;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was an error loading the OCR text for {0} page {1}.", document_fingerprint, page);
                        FileTools.Delete(filename);
                    }
                }

                // Then check for an existing GROUP file
                {
                    string filename = pdf_render_file_layer.MakeFilename_TextGroup(page);
                    try
                    {
                        if (File.Exists(filename))
                        {
                            Dictionary<int, WordList> word_lists = WordList.ReadFromFile(filename);
                            foreach (var pair in word_lists)
                            {
                                texts[pair.Key] = new TypedWeakReference<WordList>(pair.Value);
                            }

                            TypedWeakReference<WordList> word_list_weak;
                            texts.TryGetValue(page, out word_list_weak);
                            if (null != word_list_weak)
                            {
                                WordList word_list = word_list_weak.TypedTarget;
                                if (null != word_list)
                                {
                                    return word_list;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was an error loading the OCR text group for {0} page {1}.", document_fingerprint, page);
                        FileTools.Delete(filename);
                    }
                }
            }

            // If we get this far then the text was not available so queue extraction
            if (queue_for_ocr)
            {
                // If we have never tried the GROUP version before, queue for it
                string filename = pdf_render_file_layer.MakeFilename_TextGroup(page);
                PDFTextExtractor.Job job = new PDFTextExtractor.Job(this, page);

                if (!File.Exists(filename) && PDFTextExtractor.Instance.JobGroupHasNotFailedBefore(job))
                {
                    PDFTextExtractor.Instance.QueueJobGroup(job);
                }
                else
                {
                    PDFTextExtractor.Instance.QueueJobSingle(job);
                }
            }

            return null;
        }

        internal string GetFullOCRText(int page)
        {
            StringBuilder sb = new StringBuilder();

            WordList word_list = GetOCRText(page);
            if (null != word_list)
            {
                foreach (Word word in word_list)
                {
                    sb.Append(word.Text);
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        // Gets the full concatenated text for this document.
        // This is slow as it concatenates all the words from the OCR result...
        internal string GetFullOCRText()
        {
            StringBuilder sb = new StringBuilder();

            for (int page = 1; page <= PageCount; ++page)
            {
                WordList word_list = GetOCRText(page);
                if (null != word_list)
                {
                    foreach (Word word in word_list)
                    {
                        sb.Append(word.Text);
                        sb.Append(' ');
                    }
                }
            }

            return sb.ToString();
        }

        public void ClearOCRText()
        {
            Logging.Info("Clearing OCR for document " + document_fingerprint);

            // Delete the OCR files
            for (int page = 1; page <= PageCount; ++page)
            {
                // First the SINGLE file
                {
                    string filename = pdf_render_file_layer.MakeFilename_TextSingle(page);

                    if (File.Exists(filename))
                    {
                        try
                        {
                            File.Delete(filename);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "There was a problem while trying to delete the OCR file " + filename);
                        }
                    }
                }

                // Then the MULTI file
                {
                    string filename = pdf_render_file_layer.MakeFilename_TextGroup(page);

                    if (File.Exists(filename))
                    {
                        try
                        {
                            File.Delete(filename);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "There was a problem while trying to delete the OCR file " + filename);
                        }
                    }
                }
            }

            // Clear out the old texts
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (texts_lock)
            {
                //l1_clk.LockPerfTimerStop();
                texts.Clear();
            }
        }

        public void ForceOCRText(string language = "eng")
        {
            Logging.Info("Forcing OCR for document {0} in language {1}", document_fingerprint, language);

            // Clear out the old texts
            FlushCachedTexts();

            // To truly FORCE the OCR to run again, we have to nuke the old results stored on disk as well!
            ClearOCRText();

            // Queue all the pages for OCR
            for (int page = 1; page <= PageCount; ++page)
            {
                PDFTextExtractor.Job job = new PDFTextExtractor.Job(this, page);
                job.force_job = true;
                job.language = language;
                PDFTextExtractor.Instance.QueueJobSingle(job);
            }
        }

        internal void StorePageTextSingle(int page, string source_filename)
        {
            pdf_render_file_layer.StorePageTextSingle(page, source_filename);

            OnPageTextAvailable?.Invoke(page, page);
        }

        internal void StorePageTextGroup(int page, int TEXT_PAGES_PER_GROUP, string source_filename)
        {
            string filename = pdf_render_file_layer.StorePageTextGroup(page, source_filename);

            if (null != OnPageTextAvailable)
            {
                int page_range_start = ((page - 1) / TEXT_PAGES_PER_GROUP) * TEXT_PAGES_PER_GROUP + 1;
                int page_range_end = page_range_start + TEXT_PAGES_PER_GROUP - 1;
                page_range_end = Math.Min(page_range_end, PageCount);

                OnPageTextAvailable(page_range_start, page_range_end);
            }
        }

        internal void DumpImageCacheStats(out int pages_count, out int pages_bytes)
        {
            pages_count = 0;
            pages_bytes = 0;
        }

        public void FlushCachedPageRenderings()
        {
            Logging.Info("Flushing the cached page renderings for {0}", document_fingerprint);

#if SORAX_ANTIQUE
            sorax_pdf_renderer.Flush();
#endif        
        }

        public void FlushCachedTexts()
        {
            Logging.Info("Flushing the cached texts for {0}", document_fingerprint);

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (texts_lock)
            {
                //l1_clk.LockPerfTimerStop();
                texts.Clear();
            }
        }
    }
}
