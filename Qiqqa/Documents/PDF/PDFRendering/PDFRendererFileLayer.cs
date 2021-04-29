using System;
using System.IO;
using System.ServiceModel.Security;
using System.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF
{
    public partial class PDFDocument
    {
        public static readonly string BASE_PATH_DEFAULT = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, @"ocr"));

        public static void PrepOCRBaseDirectory()
        {
            Directory.CreateDirectory(BASE_PATH_DEFAULT);
        }

        private int num_pages = PDFTools.PAGECOUNT_PENDING;        // signal: -1 and below: pending/error; 0: empty document (rare/weird!); > 0: number of actual pages in document
        private bool heuristic_retry_pagecount_at_startup_done = false;

        /// <summary>
        /// This is an approximate response: it takes a *fast* shortcut to check if the given
        /// PDF has been OCR'd in the past.
        ///
        /// The emphasis here is on NOT triggering a new OCR action! Just taking a peek, *quickly*.
        ///
        /// The cost: one(1) I/O per check.
        /// 
        /// Implementation Note: do NOT check if DocumentExists: our pagecount cache check is sufficient and one I/O per check.
        /// </summary>
        public bool HasOCRdata()
        {
            // BasePath:
            string base_path = ConfigurationManager.Instance.ConfigurationRecord.System_OverrideDirectoryForOCRs;
            if (String.IsNullOrEmpty(base_path))
            {
                base_path = BASE_PATH_DEFAULT;
            }
            // string cached_count_filename = MakeFilename_PageCount(Fingerprint);
            // return MakeFilenameWith2LevelIndirection("pagecount", "0", "txt");
            string indirection_characters = Fingerprint.Substring(0, 2).ToUpper();
            string cached_count_filename = Path.GetFullPath(Path.Combine(base_path, indirection_characters, String.Format("{0}.{1}.{2}.{3}", Fingerprint, @"pagecount", @"0", @"txt")));

            return File.Exists(cached_count_filename);
        }

        private string BasePath
        {
            get
            {
                string folder_override = ConfigurationManager.Instance.ConfigurationRecord.System_OverrideDirectoryForOCRs;
                if (!String.IsNullOrEmpty(folder_override))
                {
                    return Path.GetFullPath(folder_override);
                }
                else
                {
                    return BASE_PATH_DEFAULT;
                }
            }
        }

        private string MakeFilename(string file_type, object token, string extension)
        {
            return Path.GetFullPath(Path.Combine(BasePath, String.Format("{0}.{1}.{2}.{3}", Fingerprint, file_type, token, extension)));
        }

        private string MakeFilenameWith2LevelIndirection(string file_type, object token, string extension)
        {
            string indirection_characters = Fingerprint.Substring(0, 2).ToUpper();
            return Path.GetFullPath(Path.Combine(BasePath, indirection_characters, String.Format("{0}.{1}.{2}.{3}", Fingerprint, file_type, token, extension)));
        }

        internal string MakeFilename_TextSingle(int page_number)
        {
            return MakeFilenameWith2LevelIndirection("text", page_number, "txt");
        }

        internal string MakeFilename_TextGroup(int page)
        {
            int page_range_start = ((page - 1) / TEXT_PAGES_PER_GROUP) * TEXT_PAGES_PER_GROUP + 1;
            int page_range_end = page_range_start + TEXT_PAGES_PER_GROUP - 1;
            string page_range = string.Format("{0:000}_to_{1:000}", page_range_start, page_range_end);
            return MakeFilenameWith2LevelIndirection(@"textgroup", page_range, @"txt");
        }

        private string MakeFilename_PageCount()
        {
            return MakeFilenameWith2LevelIndirection(@"pagecount", @"0", @"txt");
        }

        internal void StorePageTextSingle(int page, string source_filename)
        {
            string filename = MakeFilename_TextSingle(page);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.Copy(source_filename, filename, true);
            File.Delete(source_filename);

            OnPageTextAvailable?.Invoke(page, page);
        }

        internal void StorePageTextGroup(int page, int TEXT_PAGES_PER_GROUP, string source_filename)
        {
            string filename = MakeFilename_TextGroup(page);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.Copy(source_filename, filename, true);
            File.Delete(source_filename);

            if (null != OnPageTextAvailable)
            {
                int page_range_start = ((page - 1) / TEXT_PAGES_PER_GROUP) * TEXT_PAGES_PER_GROUP + 1;
                int page_range_end = page_range_start + TEXT_PAGES_PER_GROUP - 1;
                page_range_end = Math.Min(page_range_end, PageCount);

                OnPageTextAvailable?.Invoke(page_range_start, page_range_end);
            }
        }

        /// <summary>
        /// Return value meaning:
        /// * -3 and below: failed permanently (2 + #attempts to diagnose)
        /// * -2: failed permanently due to absent PDF file: cannot determine page count when the file does not exist!
        /// * -1: not yet determined
        /// * 0: empty document (rare, possibly even weird! Who stores an empty document in a library?)
        /// * 1 and above: number of actual pages in document
        /// </summary>
        public int PageCount
        {
            get
            {
                if (num_pages == PDFTools.PAGECOUNT_PENDING)
                {
                    WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

                    num_pages = CountPDFPages();
                }
                return num_pages;
            }
        }

        private int CountPDFPages()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            string cached_count_filename = MakeFilename_PageCount();
            int num;

            // Try the cached version
            try
            {
                if (File.Exists(cached_count_filename))
                {
                    num = Convert.ToInt32(File.ReadAllText(cached_count_filename));
                    if (num > 0)
                    {
                        return num;
                    }
                    // NOTE: all fringe cases, that is CORRUPTED and EMPTY documents, will be re-analyzed this way on every run!
                    // This prevents odd page numbers from creeping into the persisted cache.
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem loading the cached page count.");
            }

            num = PDFTools.PAGECOUNT_PENDING;

            if (!heuristic_retry_pagecount_at_startup_done)
            {
                heuristic_retry_pagecount_at_startup_done = true;

                // Nuke the cache file, iff it exists. When we get here, it contained undesirable data anyway.
                FileTools.Delete(cached_count_filename);

                // If we get here, either the pagecount-file doesn't exist, or there was an exception
                Logging.Debug特("Calculating PDF page count for file {0}", DocumentPath);

                if (!DocumentExists)
                {
                    num = PDFTools.PAGECOUNT_DOCUMENT_DOES_NOT_EXIST;

                    SavePageCountToCache(num);
                }
                else
                {
                    // PDFTools.CountPDFPages() is a very costly call: we DEFER that one until later and return 
                    // a signal we're waiting instead.
                    SafeThreadPool.QueueUserWorkItem(() =>
                    {
                        WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

                        int num = PDFTools.CountPDFPages(DocumentPath, PDFPassword);

                        Logging.Info("The result is {1} for using calculated PDF page count for file {0}", DocumentPath, num);

                        // Special case: when the CountPDFPages() API does not deliver a sane, that is positive, page count,
                        // then we've got a PDF file issue on our hands, very probably a damaged PDF.
                        //
                        // Damaged PDF files SHOULD NOT burden us forever, hence we introduce the heuristic of Retry At Restart:
                        // all "suspect" page counts are used as is for now, but when the application is restarted at any time
                        // later, those documents will be inspected once again.
                        //
                        // we DO NOT count I/O errors around the cache file.

                        SavePageCountToCache(num);
                    });
                }
            }

            return num;
        }

        private void SavePageCountToCache(int num)
        {
            if (num != PDFTools.PAGECOUNT_PENDING)
            {
                string cached_count_filename = MakeFilename_PageCount();

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(cached_count_filename));
                    string outputText = Convert.ToString(num, 10);
                    File.WriteAllText(cached_count_filename, outputText);

                    // Open the file and read back what was written: we don't do a file lock for this one, but reckon all threads who
                    // do this, will produce the same number = file content.
                    //
                    // Besides, we don't know if WriteAllText() is atomic; it probably is not.
                    // Hence we read back to verify written content and if there's anything off about it, we
                    // blow it out of the water and retry later, possibly.
                    string readText = File.ReadAllText(cached_count_filename);
                    if (readText != outputText)
                    {
                        throw new IOException("CountPDFPages: cache content as read back from cache file does not match written data. Cache file is untrustworthy.");
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem using calculated PDF page count for file {0} / {1}", DocumentPath, cached_count_filename);

                    FileTools.Delete(cached_count_filename);

                    // we know the *calculated* pagecount is A-okay, so we can pass that on.
                    // It's just the file I/O for caching the value that may have gone awry,
                    // so we should retry that the next time we (re)calculate the pagecount number
                    // as it is a heavy operation!
                    //
                    //num = 0;
                }
            }
        }
    }
}
