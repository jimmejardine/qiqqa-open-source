using System;
using System.Collections.Generic;
using System.Diagnostics;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentConversionStuff;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Qiqqa.DocumentLibrary.DocumentLibraryIndex;
using Qiqqa.DocumentLibrary.FolderWatching;
using Qiqqa.DocumentLibrary.PasswordStuff;
using Qiqqa.DocumentLibrary.RecentlyReadStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Expedition;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary
{
    public class Library : IDisposable
    {
        public override string ToString()
        {
            throw new Exception("kaboom");
            //return String.Format("Library: {0}", WebLibraryDetail.Title);
        }

        /// <summary>
        /// Use this singleton instance ONLY for testing purposes!
        /// </summary>
        public static WebLibraryDetail GuestInstance => WebLibraryManager.Instance.Library_Guest;

        private LibraryDB library_db;
        public LibraryDB LibraryDB => library_db;

        private FolderWatcherManager folder_watcher_manager;
        public FolderWatcherManager FolderWatcherManager => folder_watcher_manager;

        private LibraryIndex library_index;
        public LibraryIndex LibraryIndex => library_index;

        private AITagManager ai_tag_manager;
        public AITagManager AITagManager => ai_tag_manager;

        private RecentlyReadManager recently_read_manager;
        public RecentlyReadManager RecentlyReadManager => recently_read_manager;

        private BlackWhiteListManager blackwhite_list_manager;
        public BlackWhiteListManager BlackWhiteListManager => blackwhite_list_manager;

        private PasswordManager password_manager;
        public PasswordManager PasswordManager => password_manager;

        private ExpeditionManager expedition_manager;
        public ExpeditionManager ExpeditionManager => expedition_manager;

        private Dictionary<string, PDFDocument> pdf_documents = new Dictionary<string, PDFDocument>();
        private object pdf_documents_lock = new object();

        public delegate void OnLibraryLoadedHandler(Library library);

        // Move this somewhere nice...
        public bool sync_in_progress = false;
        private bool library_is_loaded = false;
        private bool library_is_killed = false;
        private object library_is_loaded_lock = new object();

        public event OnLibraryLoadedHandler OnLibraryLoaded;

        public bool LibraryIsLoaded
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (library_is_loaded_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return library_is_loaded;
                }
            }
            private set
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (library_is_loaded_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    library_is_loaded = value;
                }
            }
        }

        /// <summary>
        /// Signals a forced `Dispose()` call was issued; any background processes involving this library should abort ASAP!
        /// </summary>
        public bool LibraryIsKilled
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (library_is_loaded_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return library_is_killed;
                }
            }
            private set
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (library_is_loaded_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    library_is_killed = value;
                }
            }
        }

        // This is GLOBAL to all libraries
        private static Stopwatch last_pdf_add_time = Stopwatch.StartNew();
        private static object last_pdf_add_time_lock = new object();

        public static bool IsBusyAddingPDFs
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (last_pdf_add_time_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    // heuristic; give OCR process et al some time to breathe
                    return (last_pdf_add_time.ElapsedMilliseconds < 3000);
                }
            }
        }

        private static int last_regen_counter = 0;
        private static object last_regen_time_lock = new object();

        public static bool IsBusyRegeneratingTags
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (last_regen_time_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return (last_regen_counter > 0);
                }
            }
            set
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (last_regen_time_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    if (value)
                    {
                        ++last_regen_counter;
                    }
                    else
                    {
                        --last_regen_counter;
                        if (last_regen_counter < 0)
                        {
                            throw new Exception("Internal error: IsBusyRegeneratingTags unmatched set call pairs!");
                        }
                    }
                }
            }
        }

        public Library(WebLibraryDetail web_library_detail)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            Logging.Info("Library basepath is at {0}", web_library_detail.LIBRARY_BASE_PATH);
            Logging.Info("Library document basepath is at {0}", web_library_detail.LIBRARY_DOCUMENTS_BASE_PATH);

            Directory.CreateDirectory(web_library_detail.LIBRARY_BASE_PATH);
            Directory.CreateDirectory(web_library_detail.LIBRARY_DOCUMENTS_BASE_PATH);

            library_db = new LibraryDB(web_library_detail);
            folder_watcher_manager = new FolderWatcherManager(web_library_detail);
            library_index = new LibraryIndex(web_library_detail);
            ai_tag_manager = new AITagManager(web_library_detail);
            recently_read_manager = new RecentlyReadManager(web_library_detail);
            blackwhite_list_manager = new BlackWhiteListManager(web_library_detail);
            password_manager = new PasswordManager(web_library_detail);
            expedition_manager = new ExpeditionManager(web_library_detail);

#if false
            // Start loading the documents in the background...
            //
            // NOTE/WARNING: do NOT spawn off another background thread
            // to execute the call below: you'll end up in a world of hurt
            // as a lot of stuff will arrive asynchronously at sites
            // in the Qiqqa code which expect this next piece of work *done*.!
            BuildFromDocumentRepository();
#endif
        }

        // NOTE: this function is executed ASYNCHRONOUSLY.
        //
        // Once completed, an event will be fired to
        // help the main application update any relevant views.
        public void BuildFromDocumentRepository(WebLibraryDetail web_library_detail)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                // short-circuit the work when this library instance has already been loaded via this call before:
                if (LibraryIsLoaded)
                {
                    return;
                }

                // abort work when this library instance has already been Dispose()d in the main UI thread:
                if (LibraryIsKilled)
                {
                    Logging.Info("Building the library has been SKIPPED/ABORTED as the library {0} has already been killed.", web_library_detail.Id);
                    return;
                }

                Stopwatch clk = Stopwatch.StartNew();
                long prev_clk = 0;
                long elapsed = 0;
                Logging.Debug特("+Build library {0} from repository", web_library_detail.Id);
                List<LibraryDB.LibraryItem> library_items = library_db.GetLibraryItems(PDFDocumentFileLocations.METADATA);
                /* const */
                int library_item_count = library_items.Count;

                elapsed = clk.ElapsedMilliseconds;
                Logging.Debug特(":Build library '{2}' from repository -- time spent: {0} ms on fetching {1} records from SQLite DB.", elapsed, library_item_count, web_library_detail.DescriptiveTitle);
                prev_clk = elapsed;

                // Get the annotations cache
                //Dictionary<string, byte[]> library_items_annotations_cache = library_db.GetLibraryItemsAsCache(PDFDocumentFileLocations.ANNOTATIONS);

                // abort work when this library instance has already been Dispose()d in the main UI thread:
                if (LibraryIsKilled)
                {
                    Logging.Info("Building the library has been SKIPPED/ABORTED as the library {0} has already been killed.", web_library_detail.Id);
                    return;
                }

                elapsed = clk.ElapsedMilliseconds;
                Logging.Debug特(":Build library '{2}' from repository -- time spent: {0} ms on fetching annotation cache for {1} records.", elapsed - prev_clk, library_item_count, web_library_detail.DescriptiveTitle);

                Logging.Info("Library '{2}': Loading {0} files from repository at {1}", library_item_count, web_library_detail.LIBRARY_DOCUMENTS_BASE_PATH, web_library_detail.DescriptiveTitle);

                long clk_bound = elapsed;
                /* const */
                int one_pct_point = library_item_count / 200; // one percent point is defined here as 0.5%
                int next_i_bound = one_pct_point;
                for (int i = 0; i < library_item_count; ++i)
                {
                    LibraryDB.LibraryItem library_item = library_items[i];

                    // Track progress of how long this is taking to load
                    elapsed = clk.ElapsedMilliseconds;
                    if (clk_bound <= elapsed || i >= next_i_bound)
                    {
                        StatusManager.Instance.UpdateStatus("LibraryInitialLoad", String.Format("Loading your library '{0}': {1} of {2} documents", web_library_detail.DescriptiveTitle, i, library_item_count), i, library_item_count);
                        Logging.Info("Library '{2}': Loaded {0}/{1} documents", i, library_item_count, web_library_detail.DescriptiveTitle);

                        // update the bound which triggered this status update:
                        while (clk_bound <= elapsed)
                        {
                            clk_bound += 1000;
                        }
                        if (i >= next_i_bound)
                        {
                            next_i_bound += one_pct_point;

                            // and don't trigger within a second from now on the time trigger either:
                            clk_bound = elapsed + 1000;
                        }
                    }

                    if (LibraryIsKilled)
                    {
                        // abort work when this library instance has already been Dispose()d in the main UI thread:
                        Logging.Info("Building the library has been SKIPPED/ABORTED as the library {0} has already been killed.", web_library_detail.Id);
                        break;
                    }

                    if (ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Info("Library '{0}': Breaking out of PDF/Documents loading loop due to application termination", web_library_detail.DescriptiveTitle);
                        break;
                    }

                    try
                    {
                        LoadDocumentFromMetadata(library_item, web_library_detail, false);
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "Library '{1}': There was a problem loading document {0}", library_item, web_library_detail.DescriptiveTitle);
                    }
                }

                //StatusManager.Instance.ClearStatus("LibraryInitialLoad");

                Logging.Debug特("-Build library '{2}' from repository -- time spent: {0} ms on {1} library records.", clk.ElapsedMilliseconds, library_item_count, web_library_detail.DescriptiveTitle);
            }
            catch (Exception ex)
            {
                if (LibraryIsKilled)
                {
                    Logging.Warn(ex, "There was a failure while building the *KILLED* document library instance for library {0} ({1})", web_library_detail.DescriptiveTitle, web_library_detail.Id);
                }
                else
                {
                    Logging.Error(ex, "There was a problem while building the document library {0} ({1})", web_library_detail.DescriptiveTitle, web_library_detail.Id);
                }
            }
            finally
            {
                LibraryIsLoaded = true;

                if (!LibraryIsKilled)
                {
                    // fire the event ASYNC
                    OnLibraryLoaded?.BeginInvoke(this, null, null);
                }
            }
        }

        private void LoadDocumentFromMetadata(LibraryDB.LibraryItem library_item, WebLibraryDetail web_library_detail, bool notify_changed_pdf_document)
        {
            if (library_item.data == null)
            {
                throw new Exception(String.Format("Skipping corrupted NULL record for ID {0}", library_item.ToString()));
            }

            PDFDocument pdf_document = PDFDocument.LoadFromMetaData(web_library_detail, library_item.fingerprint, library_item.data);

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                if (LibraryIsKilled)
                {
                    throw new Exception("Hacky backy internal error");
                }
                pdf_documents[pdf_document.Fingerprint] = pdf_document;
            }

            if (!pdf_document.Deleted)
            {
                TagManager.Instance.ProcessDocument(pdf_document);
                ReadingStageManager.Instance.ProcessDocument(pdf_document);
            }

            if (notify_changed_pdf_document)
            {
                SignalThatDocumentsHaveChanged(pdf_document);
            }
            else
            {
                SignalThatDocumentsHaveChanged(null);
            }
        }

        /// <summary>
        /// NB: Use ImportingIntoLibrary to add to the library.  Try not to call this directly!!
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="original_filename"></param>
        /// <param name="suggested_download_source"></param>
        /// <param name="bibtex"></param>
        /// <param name="tags"></param>
        /// <param name="suppressDialogs"></param>
        /// <returns></returns>
        public PDFDocument AddNewDocumentToLibrary_SYNCHRONOUS(string filename, WebLibraryDetail web_library_detail, string original_filename, string suggested_download_source, string bibtex, HashSet<string> tags, string comments, bool suppressDialogs, bool suppress_signal_that_docs_have_changed)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            if (!suppressDialogs)
            {
                StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("Adding {0} to library", filename));
            }

            PDFDocument pdf_document = AddNewDocumentToLibrary(filename, web_library_detail, original_filename, suggested_download_source, bibtex, tags, comments, suppressDialogs, suppress_signal_that_docs_have_changed);

            if (!suppressDialogs)
            {
                if (null != pdf_document)
                {
                    StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("Added {0} to your library", filename));
                }
                else
                {
                    StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("Could not add {0} to your library", filename));
                }
            }

            return pdf_document;
        }

        private PDFDocument AddNewDocumentToLibrary(string filename, WebLibraryDetail web_library_detail, string original_filename, string suggested_download_source, string bibtex, HashSet<string> tags, string comments, bool suppressDialogs, bool suppress_signal_that_docs_have_changed)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // Flag that someone is trying to add to the library.  This is used by the background processes to hold off while the library is busy being added to...
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (last_pdf_add_time_lock)
            {
                //l1_clk.LockPerfTimerStop();
                last_pdf_add_time.Restart();
            }

            FolderWatcher.global_watch_stats.Inc(0.1);

            if (String.IsNullOrEmpty(filename) || filename.EndsWith(".vanilla_reference"))
            {
                return AddVanillaReferenceDocumentToLibrary(bibtex, web_library_detail, tags, comments, suppressDialogs, suppress_signal_that_docs_have_changed);
            }

            bool is_a_document_we_can_cope_with = false;

            if (0 == Path.GetExtension(filename).ToLower().CompareTo(".pdf"))
            {
                is_a_document_we_can_cope_with = true;
            }
            else
            {
                if (DocumentConversion.CanConvert(filename))
                {
                    string filename_before_conversion = filename;
                    string filename_after_conversion = TempFile.GenerateTempFilename("pdf");
                    if (DocumentConversion.Convert(filename_before_conversion, filename_after_conversion))
                    {
                        is_a_document_we_can_cope_with = true;
                        filename = filename_after_conversion;
                    }
                }
            }

            if (!is_a_document_we_can_cope_with)
            {
                string extension = Path.GetExtension(filename);

                if (!suppressDialogs)
                {
                    MessageBoxes.Info("This document library does not support {0} files.  Free and Premium libraries only support PDF files.  Premium+ libraries can automatically convert DOC and DOCX files to PDF.\n\nYou can convert your DOC files to PDFs using the Conversion Tool available on the Start Page Tools menu.\n\nSkipping {1}.", extension, filename);
                }
                else
                {
                    StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("This document library does not support {0} files.", extension));
                }
                return null;
            }

            // If the PDF does not exist, can not clone
            if (!File.Exists(filename))
            {
                Logging.Info("Can not add non-existent file to library, so skipping: {0}", filename);
                return null;
            }

            string fingerprint = StreamFingerprint.FromFile(filename);

            PDFDocument pdf_document = GetDocumentByFingerprint(fingerprint);
            // Useful in logging for diagnosing if we're adding the same document again
            Logging.Info("Fingerprint: {0} - add to library: {1}", fingerprint, (null == pdf_document));
            if (null != pdf_document)
            {
                // Pdf reportedly exists in database.

                // Store the pdf in our location
                pdf_document.StoreAssociatedPDFInRepository(filename);

                // If the document was previously deleted in metadata, reinstate it
                if (pdf_document.Deleted)
                {
                    Logging.Info("The document {0} was deleted, so reinstating it.", fingerprint);
                    pdf_document.Deleted = false;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Deleted));
                }

                // Try to add some useful information from the download source if the metadata doesn't already have it
                if (!String.IsNullOrEmpty(suggested_download_source)
                    && (String.IsNullOrEmpty(pdf_document.DownloadLocation)
                    // or when the new source is a URL we also
                    // *upgrade* our source info by taking up the new URL
                    // as we than assume that a new URL is 'better' i.e. more 'fresh'
                    // than any existing URL or local source file path:
                    || suggested_download_source.StartsWith("http://")
                    || suggested_download_source.StartsWith("https://")
                    || suggested_download_source.StartsWith("ftp://")
                    || suggested_download_source.StartsWith("ftps://"))
                    // *and* the old and new source shouldn't be the same:
                    && suggested_download_source != pdf_document.DownloadLocation)
                {
                    Logging.Info("The document in the library had no download location or an older one, so inferring it from download: {0} --> {1}", pdf_document.DownloadLocation ?? "(NULL)", suggested_download_source);
                    pdf_document.DownloadLocation = suggested_download_source;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.DownloadLocation));
                }

                // TODO: *merge* the BibTeX!
                if (!String.IsNullOrEmpty(bibtex))
                {
                    pdf_document.BibTex = bibtex;
                    pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
                }

                // merge = add new tags to existing ones (if any)
                if (tags != null)
                {
                    foreach (string tag in tags)
                    {
                        pdf_document.AddTag(tag); // Notify changes called internally
                    }
                }

                // TODO: merge comments?
                //
                // If we already have comments, then append them to our existing comments (if they are not identical)
                if (!String.IsNullOrEmpty(comments))
                {
                    if (pdf_document.Comments != comments)
                    {
                        pdf_document.Comments = pdf_document.Comments + "\n\n---\n\n\n" + comments;
                        pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Comments));
                    }
                }
            }
            else
            {
                // Create a new document
                pdf_document = PDFDocument.CreateFromPDF(web_library_detail, filename, fingerprint);
                //pdf_document.OriginalFileName = original_filename;
                pdf_document.DownloadLocation = suggested_download_source;
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.DownloadLocation));
                pdf_document.BibTex = bibtex;
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
                if (tags != null)
                {
                    foreach (string tag in tags)
                    {
                        pdf_document.AddTag(tag);
                    }
                }

                pdf_document.Comments = comments;
                pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Comments));

                //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l2_clk.LockPerfTimerStop();
                    // Store in our database - note that we have the lock already
                    pdf_documents[pdf_document.Fingerprint] = pdf_document;
                }

                // Get OCR queued
                pdf_document.PDFRenderer.CauseAllPDFPagesToBeOCRed();
            }

            if (!suppress_signal_that_docs_have_changed)
            {
                SignalThatDocumentsHaveChanged(pdf_document);
            }

            FolderWatcher.global_watch_stats.Inc();

            return pdf_document;
        }

        private PDFDocument AddNewDocumentToLibrary(PDFDocument pdf_document_template, WebLibraryDetail web_library_detail, bool suppressDialogs, bool suppress_signal_that_docs_have_changed)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFDocument pdf_document = AddNewDocumentToLibrary(pdf_document_template.DocumentPath, web_library_detail, pdf_document_template.DownloadLocation, pdf_document_template.DownloadLocation, pdf_document_template.BibTex, null, null, suppressDialogs, suppress_signal_that_docs_have_changed);

            return pdf_document;
        }

        private static string GetBibTeXAfterKey(string bibtex)
        {
            if (null == bibtex) return bibtex;

            int comma_pos = bibtex.IndexOf(',');
            if (0 <= comma_pos)
            {
                return bibtex.Substring(comma_pos);
            }
            else
            {
                return bibtex;
            }
        }

        public PDFDocument AddVanillaReferenceDocumentToLibrary(string bibtex, WebLibraryDetail web_library_detail, HashSet<string> tags, string comments, bool suppressDialogs, bool suppress_signal_that_docs_have_changed)
        {
            string bibtex_after_key = GetBibTeXAfterKey(bibtex);

            // Check that we are not adding a duplicate
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (var pdf_document_existing in pdf_documents.Values)
                {
                    if (!String.IsNullOrEmpty(pdf_document_existing.BibTex))
                    {
                        // Identical BibTeX (after the key) will be treated as a duplicate
                        if (GetBibTeXAfterKey(pdf_document_existing.BibTex) == bibtex_after_key)
                        {
                            Logging.Info("Not importing duplicate vanilla reference with identical BibTeX to '{0}' ({1}).", pdf_document_existing.TitleCombined, pdf_document_existing.Fingerprint);
                            return pdf_document_existing;
                        }
                    }
                }
            }

            // Not a dupe, so create
            PDFDocument pdf_document = PDFDocument.CreateFromVanillaReference(web_library_detail);
            pdf_document.BibTex = bibtex;
            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.BibTex));
            pdf_document.Comments = comments;
            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Comments));

            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    pdf_document.AddTag(tag); // Notify changes called internally
                }
            }

            // Store in our database
            //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l2_clk.LockPerfTimerStop();
                pdf_documents[pdf_document.Fingerprint] = pdf_document;
            }

            if (!suppress_signal_that_docs_have_changed)
            {
                SignalThatDocumentsHaveChanged(pdf_document);
            }

            FolderWatcher.global_watch_stats.Inc();

            return pdf_document;
        }

        public PDFDocument CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS(PDFDocument existing_pdf_document, WebLibraryDetail web_library_detail, bool suppress_dialogs, bool suppress_signal_that_docs_have_changed)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("Copying {0} ({1}) into library", existing_pdf_document.TitleCombined, existing_pdf_document.Fingerprint));

            //  do a normal add (since stored separately)
            var new_pdf_document = AddNewDocumentToLibrary(existing_pdf_document, web_library_detail, suppress_dialogs, suppress_signal_that_docs_have_changed);

            // If we were not able to create the PDFDocument from an existing pdf file (i.e. it was a missing reference), then create one from scratch
            if (null == new_pdf_document)
            {
                new_pdf_document = PDFDocument.CreateFromPDF(web_library_detail, existing_pdf_document.DocumentPath, existing_pdf_document.Fingerprint);
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    pdf_documents[new_pdf_document.Fingerprint] = new_pdf_document;
                }
            }
            else
            {
                Logging.Warn("TODO: MERGE metadata for existing document and document which was copied/moved into this library. Document: {0}, {1}", new_pdf_document.Fingerprint, new_pdf_document.LibraryRef.ToString());
            }

            // clone the metadata and switch libraries
            new_pdf_document.CloneMetaData(existing_pdf_document);

            StatusManager.Instance.UpdateStatus("LibraryDocument", String.Format("Copied {0} into your library", existing_pdf_document.TitleCombined));

            return new_pdf_document;
        }

        public void DeleteDocument(PDFDocument pdf_document)
        {
            pdf_document.Deleted = true;
            pdf_document.Bindable.NotifyPropertyChanged(nameof(pdf_document.Deleted));

            SignalThatDocumentsHaveChanged(pdf_document);
        }

        /// <summary>
        /// Returns null if the document is not found
        /// </summary>
        /// <param name="fingerprint"></param>
        /// <returns></returns>
        public PDFDocument GetDocumentByFingerprint(string fingerprint)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                PDFDocument result = null;
                if (pdf_documents.TryGetValue(fingerprint, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        // Helper API
        public static List<string> GetDocumentsAsFingerprints(List<PDFDocument> pdf_documents)
        {
            List<string> rv = new List<string>();
            foreach(PDFDocument doc in pdf_documents)
            {
                rv.Add(doc.Fingerprint);
            }
            return rv;
        }

        public List<PDFDocument> GetDocumentByFingerprints(IEnumerable<string> fingerprints)
        {
            List<PDFDocument> pdf_documents_list = new List<PDFDocument>();

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (string fingerprint in fingerprints)
                {
                    PDFDocument pdf_document = null;
                    if (pdf_documents.TryGetValue(fingerprint, out pdf_document))
                    {
                        pdf_documents_list.Add(pdf_document);
                    }
                }
            }

            return pdf_documents_list;
        }

        internal List<PDFDocument> GetDocumentsByTag(string target_tag)
        {
            List<PDFDocument> pdf_documents_list = new List<PDFDocument>();

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (var pdf_document in pdf_documents.Values)
                {
                    foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
                    {
                        if (0 == tag.CompareTo(target_tag))
                        {
                            pdf_documents_list.Add(pdf_document);
                            break;
                        }
                    }
                }
            }

            return pdf_documents_list;
        }


        /// <summary>
        /// Performs thread unsafe check
        /// </summary>
        public bool DocumentExistsInLibraryWithFingerprint(string fingerprint)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                if (pdf_documents.ContainsKey(fingerprint))
                {
                    return !pdf_documents[fingerprint].Deleted;
                }

                return false;
            }
        }

        /// <summary>
        /// Performs thread unsafe check
        /// Ignores whitespace for (hopefully) less false-positives
        /// </summary>
        public bool DocumentExistsInLibraryWithBibTeX(string bibTeXId)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (var pdf in pdf_documents.Values)
                {
                    if (!pdf.Deleted)
                    {
                        if (String.Compare(pdf.BibTexKey, bibTeXId, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Returns a list of the PDF documents in the library.  This will include all the deleted documents too...
        /// You may mess with this list - it is yours and will not change...
        /// </summary>
        public List<PDFDocument> PDFDocuments_IncludingDeleted
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    List<PDFDocument> pdf_documents_list = new List<PDFDocument>(pdf_documents.Values);
                    return pdf_documents_list;
                }
            }
        }

        public int PDFDocuments_IncludingDeleted_Count
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return pdf_documents.Count;
                }
            }
        }


        /// <summary>
        /// Returns a list of the PDF documents in the library.  This will NOT include the deleted documents...
        /// You may mess with this list - it is yours and will not change...
        /// </summary>
        public List<PDFDocument> PDFDocuments
        {
            get
            {
                List<PDFDocument> pdf_documents_list = new List<PDFDocument>();

                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    foreach (var x in pdf_documents.Values)
                    {
                        if (!x.Deleted)
                        {
                            pdf_documents_list.Add(x);
                        }
                    }
                }
                return pdf_documents_list;
            }
        }

        /// <summary>
        /// Returns a list of the PDF documents in the library.  This will NOT include the deleted documents...
        /// You may mess with this list - it is yours and will not change...
        /// </summary>
        public List<PDFDocument> PDFDocumentsWithLocalFilePresent
        {
            get
            {
                List<PDFDocument> pdf_documents_list = new List<PDFDocument>();

                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    foreach (var x in pdf_documents.Values)
                    {
                        if (!x.Deleted && x.DocumentExists)
                        {
                            pdf_documents_list.Add(x);
                        }
                    }
                }
                return pdf_documents_list;
            }
        }

#if false
        internal HashSet<string> GetAllDocumentFingerprints()
        {
            HashSet<string> results = new HashSet<string>();

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (var pdf_document in pdf_documents.Values)
                {
                    results.Add(pdf_document.Fingerprint);
                }
            }
            return results;
        }
#else
        internal HashSet<string> GetAllDocumentFingerprints()
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                return new HashSet<string>(pdf_documents.Keys);
            }
        }
#endif

        /// <summary>
        /// Keyword search - but not on internal text
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        internal HashSet<string> GetDocumentFingerprintsWithKeyword(string keyword)
        {
            keyword = keyword.ToLower();

            HashSet<string> results = new HashSet<string>();

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_lock)
            {
                //l1_clk.LockPerfTimerStop();
                foreach (var pdf_document in pdf_documents.Values)
                {
                    bool document_matches = false;

                    if (!document_matches)
                    {
                        if
                        (
                            false
                            || (pdf_document.TitleCombined?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.AuthorsCombined?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.YearCombined?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.Comments?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.Publication?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.BibTex?.ToLower().Contains(keyword) ?? false)
                            || (pdf_document.Fingerprint?.ToLower().Contains(keyword) ?? false)
                        )
                        {
                            document_matches = true;
                        }
                    }

                    if (document_matches)
                    {
                        results.Add(pdf_document.Fingerprint);
                    }
                }
            }

            return results;
        }

        internal void NotifyLibraryThatDocumentHasChangedExternally(string fingerprint, WebLibraryDetail web_library_detail)
        {
            Logging.Info("Library has been notified that {0} has changed", fingerprint);
            try
            {
                var items = library_db.GetLibraryItems(PDFDocumentFileLocations.METADATA, new List<string>() { fingerprint });
                if (0 == items.Count)
                {
                    throw new Exception(String.Format("We were expecting one item matching {0}.{1} but found none.", fingerprint, PDFDocumentFileLocations.METADATA));
                }
                if (1 != items.Count)
                {
                    throw new Exception(String.Format("We were expecting only one item matching {0}.{1}", fingerprint, PDFDocumentFileLocations.METADATA));
                }

                LoadDocumentFromMetadata(items[0], web_library_detail, true);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Library '{0}': We were told that something had changed, but could not find it on looking...", web_library_detail.DescriptiveTitle);
            }
        }

        internal void NotifyLibraryThatDocumentListHasChangedExternally()
        {
            Logging.Info("Library has been notified that files have changed");

            SignalThatDocumentsHaveChanged(null);
        }

        #region --- Signaling that documents have been changed ------------------

        public class PDFDocumentEventArgs : EventArgs
        {
            public PDFDocumentEventArgs(PDFDocument pdf_document)
            {
                PDFDocument = pdf_document;
            }

            public PDFDocument PDFDocument { get; }

        }
        public event EventHandler<PDFDocumentEventArgs> OnDocumentsChanged;

        private DateTime last_documents_changed_time = DateTime.MinValue;
        private DateTime last_documents_changed_signal_time = DateTime.MinValue;
        private PDFDocument documents_changed_optional_changed_pdf_document = null;
        private object last_documents_changed_lock = new object();

        public void SignalThatDocumentsHaveChanged(PDFDocument optional_changed_pdf_document)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (last_documents_changed_lock)
            {
                //l1_clk.LockPerfTimerStop();
                last_documents_changed_time = DateTime.UtcNow;
                if (null == documents_changed_optional_changed_pdf_document || optional_changed_pdf_document == documents_changed_optional_changed_pdf_document)
                {
                    documents_changed_optional_changed_pdf_document = optional_changed_pdf_document;
                }
                else
                {
                    // multiple documents have changed since the observer(s) handled the previous signal...
                    documents_changed_optional_changed_pdf_document = null;
                }
            }
        }

        internal void CheckForSignalThatDocumentsHaveChanged()
        {
            if (LibraryIsKilled)
            {
                return;
            }

            PDFDocument local_documents_changed_optional_changed_pdf_document;
            DateTime now = DateTime.UtcNow;

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (last_documents_changed_lock)
            {
                //l1_clk.LockPerfTimerStop();
                // If no docs have changed, nothing to do
                if (last_documents_changed_signal_time >= last_documents_changed_time)
                {
                    return;
                }

                // Don't refresh more than once every few seconds in busy-adding times
                if (now.Subtract(last_documents_changed_time).TotalSeconds < 1 && now.Subtract(last_documents_changed_signal_time).TotalSeconds < 15)
                {
                    return;
                }

                // Don't refresh more than once a second in quiet times
                if (now.Subtract(last_documents_changed_signal_time).TotalSeconds < 1)
                {
                    return;
                }

                // Let's signal!
                local_documents_changed_optional_changed_pdf_document = documents_changed_optional_changed_pdf_document;
                documents_changed_optional_changed_pdf_document = null;
                last_documents_changed_signal_time = now;
            }

            try
            {
                OnDocumentsChanged?.Invoke(this, new PDFDocumentEventArgs(local_documents_changed_optional_changed_pdf_document));
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was an exception while notifying that documents have changed.");
            }
        }

        #endregion

        #region --- IDisposable ------------------------------------------------------------------------

        ~Library()
        {
            Logging.Debug("~Library()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing Library");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("Library::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.SafeExec(() =>
            {
                LibraryIsKilled = true;
            });

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources / get rid of cyclic references:

                    // Do we need to check that the library has finished being loaded?

                    // Switch off the living things
                    library_index?.Dispose();
                    folder_watcher_manager?.Dispose();
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // NULL the memory database

                    //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                    lock (pdf_documents_lock)
                    {
                        //l2_clk.LockPerfTimerStop();
                        pdf_documents.Clear();
                        pdf_documents = null;
                    }
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                // Clear the references for sanity's sake
                expedition_manager = null;
                password_manager = null;
                blackwhite_list_manager = null;
                recently_read_manager = null;
                ai_tag_manager = null;
                library_index = null;
                folder_watcher_manager = null;
                library_db = null;
            });

            ++dispose_count;
        }

        #endregion

    }
}
