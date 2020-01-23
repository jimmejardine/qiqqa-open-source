using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.FolderWatching
{
    public class FolderWatcher : IDisposable
    {
        private FolderWatcherManager folder_watcher_manager;
        private Library library;
        private HashSet<string> tags;

        // TODO
        //
        // Warning CA1001  Implement IDisposable on 'FolderWatcher' because it creates members 
        // of the following IDisposable types: 'FileSystemWatcher'. 

        private FileSystemWatcher file_system_watcher;
        private string configured_folder_to_watch;
        private string aspiring_folder_to_watch;
        private bool __folder_contents_has_changed;
        private object folder_contents_has_changed_lock = new object();

        private bool FolderContentsHaveChanged
        {
            get
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_contents_has_changed_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    return __folder_contents_has_changed;
                }
            }
            set
            {
                // can only SET the signal; TestAndReset is required to RESET the signalling boolean
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_contents_has_changed_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    __folder_contents_has_changed = true;
                }
            }
        }

        private bool TestAndReset_FolderContentsHaveChanged()
        {
            bool rv;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (folder_contents_has_changed_lock)
            {
                l1_clk.LockPerfTimerStop();
                rv = __folder_contents_has_changed;
                __folder_contents_has_changed = false;
            }

            return rv;
        }

        public FolderWatcher(FolderWatcherManager folder_watcher_manager, Library library, string folder_to_watch, string tags)
        {
            this.folder_watcher_manager = folder_watcher_manager;
            this.library = library;
            aspiring_folder_to_watch = folder_to_watch;
            this.tags = TagTools.ConvertTagBundleToTags(tags);
            configured_folder_to_watch = null;

            file_system_watcher = new FileSystemWatcher();
            file_system_watcher.IncludeSubdirectories = true;
            file_system_watcher.Filter = "*.pdf";
            file_system_watcher.Changed += file_system_watcher_Changed;
            file_system_watcher.Created += file_system_watcher_Created;
            configured_folder_to_watch = null;
            FolderContentsHaveChanged = false;

            file_system_watcher.Path = null;
            file_system_watcher.EnableRaisingEvents = false;
        }

        private void file_system_watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logging.Debug特("FolderWatcher file_system_watcher_Changed");
            FolderContentsHaveChanged = true;
        }

        private void file_system_watcher_Created(object sender, FileSystemEventArgs e)
        {
            Logging.Debug特("FolderWatcher file_system_watcher_Created");
            FolderContentsHaveChanged = true;
        }

        private void CheckIfFolderNameHasChanged()
        {
            // If they are both null, no worries - they are the same
            if (null == configured_folder_to_watch && null == aspiring_folder_to_watch)
            {
                file_system_watcher.EnableRaisingEvents = false;
                return;
            }

            // If they are both identical, no worries
            if (null != configured_folder_to_watch && null != aspiring_folder_to_watch && 0 == configured_folder_to_watch.CompareTo(aspiring_folder_to_watch))
            {
                return;
            }

            // If we get here, things have changed
            Logging.Info("FolderWatcher's folder has changed from '{0}' to '{1}'", configured_folder_to_watch, aspiring_folder_to_watch);
            file_system_watcher.EnableRaisingEvents = false;
            FolderContentsHaveChanged = true;
            try
            {
                file_system_watcher.Path = aspiring_folder_to_watch;
                configured_folder_to_watch = aspiring_folder_to_watch;
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Failure to set up the new Watch Folder: {0}.", aspiring_folder_to_watch);
                file_system_watcher.Path = null;
                configured_folder_to_watch = null;
                aspiring_folder_to_watch = null;
            }

            // Start watching if there is something to watch
            if (!String.IsNullOrEmpty(configured_folder_to_watch))
            {
                Logging.Info("FolderWatcher is watching '{0}'", configured_folder_to_watch);
                file_system_watcher.EnableRaisingEvents = true;
            }
            else
            {
                Logging.Info("FolderWatcher is disabled as there is nothing to watch");
            }
        }

        /// <summary>
        /// The daemon code calls this occasionally to poke it into action to do work
        /// </summary>
        /// <param name="daemon"></param>
        public void TaskDaemonEntryPoint(Daemon daemon)
        {
            // We don't want to start watching files until the library is loaded...
            if (!library.LibraryIsLoaded)
            {
                Logging.Info("Library is not yet loaded, so waiting before watching...");

                // Indicate that the library may still not have been changed...
                FolderContentsHaveChanged = true;
                return;
            }

            // Update our folder system watcher if necessary
            CheckIfFolderNameHasChanged();

            // If the current folder is blank, do nothing
            if (String.IsNullOrEmpty(configured_folder_to_watch))
            {
                return;
            }

            // If the folder does not exist, do nothing
            if (!Directory.Exists(configured_folder_to_watch))
            {
                return;
            }

            // If the folder or its contents has not changed since the last time, do nothing
            if (!FolderContentsHaveChanged)
            {
                return;
            }

            Logging.Debug("FolderWatcher BEGIN");

            // To recover from a fatal library failure and re-indexing attempt for very large libraries,
            // we're better off processing a limited number of source files as we'll be able to see 
            // *some* results more quickly and we'll have a working, though yet incomplete,
            // index in *reasonable time*.
            //
            // To reconstruct the entire index will take a *long* time. We grow the index and other meta
            // stores a bunch-of-files at a time and then repeat the entire maintenance process until
            // we'll be sure to have run out of files to process for sure...
            const int MAX_NUMBER_OF_PDF_FILES_TO_PROCESS = 5;
            const int MAX_SECONDS_PER_ITERATION = 5 * 1000;
            Stopwatch index_processing_clock = Stopwatch.StartNew();

            // Mark that we are now processing the folder
            while (TestAndReset_FolderContentsHaveChanged())
            {
                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs || Library.IsBusyRegeneratingTags)
                {
                    Logging.Debug特("FolderWatcher: Not daemon processing any library that is busy with adds...");
                    FolderContentsHaveChanged = true;
                    break;
                }

                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug特("FolderWatcher: Breaking out of outer processing loop due to daemon termination");
                    FolderContentsHaveChanged = true;
                    break;
                }

                if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                {
                    Logging.Debug特("FolderWatcher: Breaking out of outer processing loop due to DisableAllBackgroundTasks");
                    FolderContentsHaveChanged = true;
                    break;
                }

                int processing_file_count = 0;
                int processed_file_count = 0;
                int scanned_file_count = 0;
                int skipped_file_count = 0;

                // If we get this far then there might be some work to do in the folder...
                Stopwatch clk = Stopwatch.StartNew();
                IEnumerable<string> filenames_in_folder = Directory.EnumerateFiles(configured_folder_to_watch, "*.pdf", SearchOption.AllDirectories);
                Logging.Debug特("Directory.EnumerateFiles took {0} ms", clk.ElapsedMilliseconds);

                List<PDFDocument> pdf_documents_already_in_library = library.PDFDocuments;

                List<string> filenames_that_are_new = new List<string>();
                foreach (string filename in filenames_in_folder)
                {
                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Info("FolderWatcher: Breaking out of inner processing loop due to daemon termination");
                        break;
                    }

                    if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                    {
                        Logging.Info("FolderWatcher: Breaking out of inner processing loop due to DisableAllBackgroundTasks");
                        break;
                    }

                    scanned_file_count++;

                    if (index_processing_clock.ElapsedMilliseconds > MAX_SECONDS_PER_ITERATION)
                    {
                        Logging.Info("FolderWatcher: Taking a nap due to MAX_SECONDS_PER_ITERATION: {0} seconds consumed, {1} threads pending", index_processing_clock.ElapsedMilliseconds / 1E3, SafeThreadPool.QueuedThreadCount);

                        // Collect various 'pending' counts to help produce a stretched sleep/delay period
                        // in order to allow the other background tasks to keep up with the PDF series being
                        // fed into them by this task.
                        int thr_cnt = Math.Max(0, SafeThreadPool.QueuedThreadCount - 2);
                        int queued_cnt = Qiqqa.Documents.Common.DocumentQueuedStorer.Instance.PendingQueueCount;
                        int textify_count = 0;
                        int ocr_count = 0;
                        Qiqqa.Documents.PDF.PDFRendering.PDFTextExtractor.Instance.GetJobCounts(out textify_count, out ocr_count);

                        int duration = 1 * 1000 + thr_cnt * 250 + queued_cnt * 20 + textify_count * 50 + ocr_count * 500;

                        daemon.Sleep(Math.Min(60 * 1000, duration));
                        // As we have slept a while, it's quite unsure whether that file still exists. Skip it and 
                        // let the next round find it later on.
                        FolderContentsHaveChanged = true;
                        // reset:
                        index_processing_clock.Restart();
                        continue;
                    }

                    // If we already have this file in the "cache since we started", skip it
                    if (folder_watcher_manager.HaveProcessedFile(filename))
                    {
                        Logging.Debug特("FolderWatcher is skipping {0} as it has already been processed", filename);
                        skipped_file_count++;
                        continue;
                    }

                    // If we already have this file in the "pdf file locations", skip it
                    bool is_already_in_library = false;

                    // Check that the file is not still locked - if it is, mark that the folder is still "changed" and come back later.
                    //
                    // We do this at the same tim as calculating the file fingerprint as both actions require (costly) File I/O
                    // and can be folded together: if the fingerprint fails, that's 99.9% sure a failure in the File I/O, hence
                    // a locked or otherwise inaccessible file.
                    string fingerprint;
                    try
                    {
                        fingerprint = StreamFingerprint.FromFile(filename);
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "Watched folder contains file '{0}' which is locked, so coming back later...", filename);
                        FolderContentsHaveChanged = true;
                        continue;
                    }

                    foreach (PDFDocument pdf_document in pdf_documents_already_in_library)
                    {
                        // do NOT depend on the file staying the same; external activities may have replaced the PDF with another one!
                        //
                        // Hence we SHOULD check using file FINGERPRINT, even though that's a costly operation:
#if OLD          
                        if (pdf_document.DownloadLocation == filename)
                        {
                            is_already_in_library = true;
                            break;
                        }
#else
                        if (pdf_document.Fingerprint == fingerprint)
                        {
                            is_already_in_library = true;
                            break;
                        }
#endif
                    }

                    if (is_already_in_library)
                    {
                        // Add this file to the list of processed files...
                        folder_watcher_manager.RememberProcessedFile(filename);
                        skipped_file_count++;
                        continue;
                    }

                    // Do NOT count files which are already present in our library/DB, 
                    // despite the fact that those also *do* take time and effort to check
                    // in the code above.
                    //
                    // The issue here is that when we would import files A,B,C,D,E,F,G,H,I,J,K,
                    // we would do so in tiny batches, resulting in a rescan after each batch 
                    // where the already processed files will be included in the set, but must
                    // be filtered out as 'already in there' in the code above.
                    // Iff we had counted *all* files we inspect from the Watch Directory,
                    // we would never make it batch the first batch as then our count limit
                    // would trigger already for every round through here!
                    //
                    processing_file_count++;

                    Logging.Info("FolderWatcher is importing {0}", filename);
                    filenames_that_are_new.Add(filename);

                    if (processing_file_count >= MAX_NUMBER_OF_PDF_FILES_TO_PROCESS + processed_file_count)
                    {
                        Logging.Info("FolderWatcher: {0} of {1} files have been processed/inspected (total {2} scanned, {3} skipped, {4} ignored)", processed_file_count, processing_file_count, scanned_file_count, skipped_file_count, scanned_file_count - skipped_file_count - processing_file_count);
                        // process one little batch, before we add any more:
                        ProcessTheNewDocuments(filenames_that_are_new);

                        // reset 
                        filenames_that_are_new.Clear();

                        processed_file_count = processing_file_count;

                        // Relinquish control to the UI thread to make sure responsiveness remains tolerable at 100% CPU load.
                        Utilities.GUI.WPFDoEvents.WaitForUIThreadActivityDone();
                    }
                }

                Logging.Info("FolderWatcher: {0} of {1} files have been processed/inspected (total {2} scanned, {3} skipped, {4} ignored)", processed_file_count, processing_file_count, scanned_file_count, skipped_file_count, scanned_file_count - skipped_file_count - processing_file_count);
                // process the remainder: a last little batch:
                ProcessTheNewDocuments(filenames_that_are_new);

                Logging.Debug("FolderWatcher End-Of-Round");

                daemon.Sleep(3 * 1000);
            }

            Logging.Debug("FolderWatcher END");
        }

        private void ProcessTheNewDocuments(List<string> filenames_that_are_new)
        {
            if (0 == filenames_that_are_new.Count)
            {
                return;
            }

            if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Info("FolderWatcher: Breaking out due to daemon termination");
                return;
            }

            if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                Logging.Info("FolderWatcher: Breaking out due to DisableAllBackgroundTasks");
                return;
            }

            // Create the import records
            List<FilenameWithMetadataImport> filename_with_metadata_imports = new List<FilenameWithMetadataImport>();
            foreach (var filename in filenames_that_are_new)
            {
                filename_with_metadata_imports.Add(new FilenameWithMetadataImport
                {
                    filename = filename,
                    tags = new HashSet<string>(tags)
                });

                // TODO: refactor this: delay until the PDF has actually been processed completely!
                //
                // Add this file to the list of processed files...
                folder_watcher_manager.RememberProcessedFile(filename);
            }

            // Get the library to import all these new files
            ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_ASYNCHRONOUS(library, true, true, filename_with_metadata_imports.ToArray());

            // TODO: refactor the ImportingIntoLibrary class 
            //
            // HACK & QUICK PATCH until we have refactored this stuff:
            filenames_that_are_new.Clear();
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~FolderWatcher()
        {
            Logging.Debug("~FolderWatcher()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing FolderWatcher");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("FolderWatcher::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    file_system_watcher.EnableRaisingEvents = false;
                    file_system_watcher.Dispose();
                }

                file_system_watcher = null;

                folder_watcher_manager = null;
                //library.Dispose();
                library = null;
                tags.Clear();
                configured_folder_to_watch = null;
                aspiring_folder_to_watch = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;
        }

        #endregion
    }
}
