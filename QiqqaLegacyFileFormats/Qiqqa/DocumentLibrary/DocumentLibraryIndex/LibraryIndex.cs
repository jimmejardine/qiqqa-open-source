using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaLegacyFileFormats          // namespace Qiqqa.DocumentLibrary.DocumentLibraryIndex
{

#if SAMPLE_LOAD_CODE

    public class LibraryIndex : IDisposable
    {
        private const int LIBRARY_SCAN_PERIOD_SECONDS = 60;
        private const int DOCUMENT_INDEX_RETRY_PERIOD_SECONDS = 60;
        private const int MAX_MILLISECONDS_PER_ITERATION = 15 * 1000;

        private TypedWeakReference<Library> library;
        public Library Library => library?.TypedTarget;

        private LuceneIndex word_index_manager = null;
        private object word_index_manager_lock = new object();
        private Stopwatch time_of_last_library_scan = Stopwatch.StartNew();
        private Dictionary<string, PDFDocumentInLibrary> pdf_documents_in_library = null;
        private object pdf_documents_in_library_lock = new object();

        private bool libraryIndex_is_loaded = false;
        private object libraryIndex_is_loaded_lock = new object();        // fast lock
        private object libraryIndexInit_is_pending_lock = new object();   // slow lock; wraps the entire Init phase to ensure no two threads run it in parallel

        public bool LibraryIndexIsLoaded
        {
            get
            {
                lock (libraryIndex_is_loaded_lock)
                {
                    return libraryIndex_is_loaded;
                }
            }
            private set
            {
                lock (libraryIndex_is_loaded_lock)
                {
                    libraryIndex_is_loaded = value;
                }
            }
        }

        public LibraryIndex(WebLibraryDetail web_library_detail)
        {
            this.library = new TypedWeakReference<Library>(library);

            // postpone INIT phase...
        }

        private void Init(WebLibraryDetail web_library_detail)
        {
            // have we been here before?
            if (LibraryIndexIsLoaded)
            {
                return;
            }

            // Utilities.LockPerfTimer l5_clk = Utilities.LockPerfChecker.Start();
            lock (libraryIndexInit_is_pending_lock)
            {
                // l5_clk.LockPerfTimerStop();

                //Utilities.LockPerfTimer l4_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    lock (word_index_manager_lock)
                    {
                        //l4_clk.LockPerfTimerStop();
                        if (null != pdf_documents_in_library && null != word_index_manager)
                        {
                            Logging.Warn("LibraryIndex has already been initialized.");
                            return;
                        }
                    }
                }

                Logging.Info("Try to load a historical progress file: {0}", web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST);
                try
                {
                    if (File.Exists(web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST))
                    {
                        Stopwatch clk = Stopwatch.StartNew();
                        Logging.Info("+Loading historical progress file: {0}", web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST);
                        //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                        lock (pdf_documents_in_library_lock)
                        {
                            //l1_clk.LockPerfTimerStop();
                            pdf_documents_in_library = (Dictionary<string, PDFDocumentInLibrary>)SerializeFile.LoadSafely(web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST);
                        }
                        Logging.Info("-Loaded historical progress file: {0} (time spent: {1} ms)", web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST, clk.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "FAILED to load historical progress file \"{0}\". Will start indexing afresh.", web_library_detail.FILENAME_DOCUMENT_PROGRESS_LIST);
                    //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                    lock (pdf_documents_in_library_lock)
                    {
                        //l2_clk.LockPerfTimerStop();
                        pdf_documents_in_library = null;
                    }
                }

                // If there was no historical progress file, start afresh
                //Utilities.LockPerfTimer l3_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l3_clk.LockPerfTimerStop();
                    if (null == pdf_documents_in_library)
                    {
                        Logging.Warn("Could not find any indexing progress, so starting from scratch.");
                        pdf_documents_in_library = new Dictionary<string, PDFDocumentInLibrary>();
                    }
                }

                // Utilities.LockPerfTimer l6_clk = Utilities.LockPerfChecker.Start();
                lock (word_index_manager_lock)
                {
                    // l6_clk.LockPerfTimerStop();
                    word_index_manager = new LuceneIndex(web_library_detail.LIBRARY_INDEX_BASE_PATH);
                    word_index_manager.WriteMasterList();
                }

                LibraryIndexIsLoaded = true;
            }
        }

        private string FILENAME_DOCUMENT_PROGRESS_LIST => Path.GetFullPath(Path.Combine(Library.LIBRARY_INDEX_BASE_PATH, @"DocumentProgressList.dat"));
    }

#endif

}
