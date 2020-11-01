using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Alphaleonis.Win32.Filesystem;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.FolderWatching
{
    // Warning CA1001  Implement IDisposable on 'FolderWatcher' because it creates members
    // of the following IDisposable types: 'FileSystemWatcher'.

    public class FolderWatcher : IDisposable
    {
        private TypedWeakReference<FolderWatcherManager> folder_watcher_manager;

        private TypedWeakReference<WebLibraryDetail> web_library_detail;
        public WebLibraryDetail LibraryRef => web_library_detail?.TypedTarget;

        private readonly HashSet<string> tags;

        private FileSystemWatcher file_system_watcher;
        private string configured_folder_to_watch;
        private string aspiring_folder_to_watch;
        private bool __folder_contents_has_changed;
        private object folder_contents_has_changed_lock = new object();

        private bool FolderContentsHaveChanged
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_contents_has_changed_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return __folder_contents_has_changed;
                }
            }
            set
            {
                // can only SET the signal; TestAndReset is required to RESET the signaling boolean
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_contents_has_changed_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    __folder_contents_has_changed = true;
                }
            }
        }

        private bool TestAndReset_FolderContentsHaveChanged()
        {
            bool rv;

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (folder_contents_has_changed_lock)
            {
                //l1_clk.LockPerfTimerStop();
                rv = __folder_contents_has_changed;
                __folder_contents_has_changed = false;
            }

            return rv;
        }

        public FolderWatcher(FolderWatcherManager _folder_watcher_manager, WebLibraryDetail _library, string folder_to_watch, string _tags)
        {
            folder_watcher_manager = new TypedWeakReference<FolderWatcherManager>(_folder_watcher_manager);
            web_library_detail = new TypedWeakReference<WebLibraryDetail>(_library);
            aspiring_folder_to_watch = folder_to_watch;
            tags = TagTools.ConvertTagBundleToTags(_tags);
            configured_folder_to_watch = null;

            watch_stats = new WatchStatistics();

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

        public const int MAX_SECONDS_PER_ITERATION = 5 * 1000;

        internal class WatchStatistics
        {
            public int processing_file_count;
            public int processed_file_count;
            public int scanned_file_count;
            public int skipped_file_count;

            // Store the *hashes* of the files we have processed during this run.
            //
            // This helps filter out duplicates on import as the same PDF may be stored
            // in multiple subdirs or filenames in the watched directory tree.
            public Dictionary<string, string> file_hashes_added;

            public Stopwatch index_processing_clock;

            public Daemon daemon;

            public WatchStatistics()
            {
                Reset(null);
            }

            public void Reset(Daemon d)
            {
                processing_file_count = 0;
                processed_file_count = 0;
                scanned_file_count = 0;
                skipped_file_count = 0;

                file_hashes_added = new Dictionary<string, string>();

                index_processing_clock = Stopwatch.StartNew();

                daemon = d;
            }
        }

        private WatchStatistics watch_stats;

        /// <summary>
        /// The daemon code calls this occasionally to poke it into action to do work
        /// </summary>
        /// <param name="daemon"></param>
        public void ExecuteBackgroundProcess(Daemon daemon)
        {
            // We don't want to start watching files until the library is loaded...
            if (!(LibraryRef?.Xlibrary.LibraryIsLoaded ?? false))
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
                Logging.Info("Watched folder {0} does not exist: watching this directory has been disabled.", configured_folder_to_watch);
                return;
            }

            // If the folder or its contents has not changed since the last time, do nothing
            if (!FolderContentsHaveChanged)
            {
                return;
            }

            if (!ConfigurationManager.IsEnabled(nameof(FolderWatcher)))
            {
                Logging.Info("Watched folder {0} will not be watched/scanned due to Developer Override setting {1}=false", configured_folder_to_watch, nameof(FolderWatcher));
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

                if (ShutdownableManager.Instance.IsShuttingDown)
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

                if (LibraryRef == null || folder_watcher_manager?.TypedTarget == null)
                {
                    Logging.Debug特("FolderWatcher: Breaking out of outer processing loop due to disposed library and/or watch manager");
                    FolderContentsHaveChanged = true;
                    break;
                }

                // reset counters for logging/reporting:
                watch_stats.Reset(daemon);

                // If we get this far then there might be some work to do in the folder...
                Stopwatch clk = Stopwatch.StartNew();

                //
                // Summary:
                //     [AlphaFS] Specifies a set of custom filters to be used with enumeration methods
                //     of Alphaleonis.Win32.Filesystem.Directory, e.g., Alphaleonis.Win32.Filesystem.Directory.EnumerateDirectories(System.String),
                //     Alphaleonis.Win32.Filesystem.Directory.EnumerateFiles(System.String), or Alphaleonis.Win32.Filesystem.Directory.EnumerateFileSystemEntries(System.String).
                //
                // Remarks:
                //     Alphaleonis.Win32.Filesystem.DirectoryEnumerationFilters allows scenarios in
                //     which files/directories being enumerated by the methods of Alphaleonis.Win32.Filesystem.Directory
                //     class are accepted only if they match the search pattern, attributes (see Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.SkipReparsePoints),
                //     and optionally also the custom criteria tested in the method whose delegate is
                //     specified in Alphaleonis.Win32.Filesystem.DirectoryEnumerationFilters.InclusionFilter.
                //     These criteria could be, e.g., file size exceeding some threshold, pathname matches
                //     a complex regular expression, etc. If the enumeration process is set to be recursive
                //     (see Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Recursive) and
                //     Alphaleonis.Win32.Filesystem.DirectoryEnumerationFilters.RecursionFilter is specified,
                //     the directory is traversed recursively only if it matches the custom criteria
                //     in Alphaleonis.Win32.Filesystem.DirectoryEnumerationFilters.RecursionFilter method.
                //     This allows, for example, custom handling of junctions and symbolic links, e.g.,
                //     detection of cycles. If any error occurs during the enumeration and the enumeration
                //     process is not set to ignore errors (see Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.ContinueOnException),
                //     an exception is thrown unless the error is handled (filtered out) by the method
                //     specified in Alphaleonis.Win32.Filesystem.DirectoryEnumerationFilters.ErrorFilter
                //     (if specified). The method may, for example, consume the error by reporting it
                //     in a log, so that the enumeration continues as in the case of Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.ContinueOnException
                //     option but the user will be informed about errors.
                //
                DirectoryEnumerationFilters filter = new DirectoryEnumerationFilters();
                filter.ErrorFilter = DecideIfErrorDuringDirScan;
                filter.InclusionFilter = DecideIfIncludeDuringDirScan;
                filter.RecursionFilter = DecideIfRecurseDuringDirScan;
                // Note: don't use the CancellationToken, just throw an exception in the InclusionFilter when it's time to abort the scan.
                //filter.CancellationToken = null;

                IEnumerable<string> filenames_in_folder = Directory.EnumerateFiles(configured_folder_to_watch,
                    DirectoryEnumerationOptions.Files |
                    DirectoryEnumerationOptions.BasicSearch |
                    //DirectoryEnumerationOptions.ContinueOnException |
                    DirectoryEnumerationOptions.LargeCache |
                    DirectoryEnumerationOptions.Recursive,
                    filter);
                // SearchOption.AllDirectories);
                Logging.Debug特("Directory.EnumerateFiles took {0} ms", clk.ElapsedMilliseconds);

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

                List<string> filenames_that_are_new = new List<string>();
                foreach (string filename in filenames_in_folder)
                {
                    Logging.Info("FolderWatcher: {0} of {1} files have been processed/inspected (total {2} scanned, {3} skipped, {4} ignored)", watch_stats.processed_file_count, watch_stats.processing_file_count, watch_stats.scanned_file_count, watch_stats.skipped_file_count, watch_stats.scanned_file_count - watch_stats.skipped_file_count - watch_stats.processing_file_count);

                    try
                    {
                        // check the file once again: it MAY have disappeared while we were slowly scanning the remainder of the dirtree.
                        FileSystemEntryInfo info = File.GetFileSystemEntryInfo(filename);

                        watch_stats.processing_file_count++;

                        Logging.Info("FolderWatcher is importing {0}", filename);
                        filenames_that_are_new.Add(filename);
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "Folder Watcher: skipping file {0} due to file I/O error {1}", filename, ex.Message);
                    }
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

#if false
                    // delay until the PDF has actually been processed completely!
                    //
                    // Add this file to the list of processed files...
                    folder_watcher_manager.RememberProcessedFile(filename);
#endif
                }

                // Get the library to import all these new files
                if (filename_with_metadata_imports.Count > 0)
                {
                    ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_SYNCHRONOUS(LibraryRef, true, true, filename_with_metadata_imports.ToArray());

                    // TODO: refactor the ImportingIntoLibrary class
                }

                watch_stats.processed_file_count = watch_stats.processing_file_count;

                Logging.Info("FolderWatcher: {0} of {1} files have been processed/inspected (total {2} scanned, {3} skipped, {4} ignored)", watch_stats.processed_file_count, watch_stats.processing_file_count, watch_stats.scanned_file_count, watch_stats.skipped_file_count, watch_stats.scanned_file_count - watch_stats.skipped_file_count - watch_stats.processing_file_count);

                Logging.Debug("FolderWatcher End-Of-Round");
            }

            Logging.Debug("FolderWatcher END");
        }

        //
        // Summary:
        //     [AlphaFS] Represents the method that will handle an error raised during retrieving
        //     file system entries.
        //
        // Returns:
        //     true, if the error has been fully handled and the caller may proceed, The error
        //     code. The error message. The faulty path being processed. false otherwise, in
        //     which case the caller will throw the corresponding exception.
        internal bool DecideIfErrorDuringDirScan(int errorCode, string errorMessage, string pathProcessed)
        {
            Logging.Error("FolderWatcher scanning error: 0x{0:X8}: {1} :: path: {2}", errorCode, errorMessage, pathProcessed);

            return true; // == DirectoryEnumerationOptions.ContinueOnException
        }

        //
        // Summary:
        //     Represents the method that defines a set of criteria and determines whether the
        //     specified object meets those criteria.
        //
        // Parameters:
        //   obj:
        //     The object to compare against the criteria defined within the method represented
        //     by this delegate.
        //
        // Type parameters:
        //   T:
        //     The type of the object to compare.
        //
        // Returns:
        //     true if obj meets the criteria defined within the method represented by this
        //     delegate; otherwise, false.
        internal bool DecideIfIncludeDuringDirScan(FileSystemEntryInfo obj)
        {
            bool isRegularFile = !(obj.IsDevice || obj.IsDirectory || obj.IsMountPoint || /* obj.IsReparsePoint (hardlink!) || */ obj.IsOffline || obj.IsSystem || obj.IsTemporary);
            Logging.Debug("FolderWatcher: testing {1} '{0}' for inclusion in the Qiqqa library.", obj.FullPath, isRegularFile ? "regular File" : obj.IsDirectory ? "directory" : "node");

            if (ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Info("FolderWatcher: Breaking out of inner processing loop due to daemon termination");
                throw new OperationCanceledException("FolderWatcher: Breaking out of inner processing loop due to daemon termination");
            }

            if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                Logging.Info("FolderWatcher: Breaking out of inner processing loop due to DisableAllBackgroundTasks");
                throw new OperationCanceledException("FolderWatcher: Breaking out of inner processing loop due to DisableAllBackgroundTasks");
            }

            if (LibraryRef == null || folder_watcher_manager?.TypedTarget == null)
            {
                Logging.Info("FolderWatcher: Breaking out of inner processing loop due to disposed library and/or watch manager");
                throw new OperationCanceledException("FolderWatcher: Breaking out of inner processing loop due to disposed library and/or watch manager");
            }

            bool have_we_slept = false;

            if (watch_stats.index_processing_clock.ElapsedMilliseconds > MAX_SECONDS_PER_ITERATION)
            {
                Logging.Info("FolderWatcher: Taking a nap due to MAX_SECONDS_PER_ITERATION: {0} seconds consumed, {1} threads pending", watch_stats.index_processing_clock.ElapsedMilliseconds / 1E3, SafeThreadPool.QueuedThreadCount);

                // Collect various 'pending' counts to help produce a stretched sleep/delay period
                // in order to allow the other background tasks to keep up with the PDF series being
                // fed into them by this task.
                int thr_cnt = Math.Max(0, SafeThreadPool.QueuedThreadCount - 2);
                int queued_cnt = Qiqqa.Documents.Common.DocumentQueuedStorer.Instance.PendingQueueCount;
                Qiqqa.Documents.PDF.PDFRendering.PDFTextExtractor.Instance.GetJobCounts(out var textify_count, out var ocr_count);

                int duration = 1 * 1000 + thr_cnt * 250 + queued_cnt * 20 + textify_count * 50 + ocr_count * 500;

                watch_stats.daemon.Sleep(Math.Min(60 * 1000, duration));

                // Relinquish control to the UI thread to make sure responsiveness remains tolerable at 100% CPU load.
                WPFDoEvents.WaitForUIThreadActivityDone();

                // reset:
                watch_stats.index_processing_clock.Restart();

                have_we_slept = true;
            }

            // only include *.pdf files. Use a `while` loop to allow easy `break` statements to abort the inclusion filter logic below:
            while (isRegularFile && obj.Extension.ToLower() == ".pdf")
            {
                // check if the given file isn't already present in the library:

                watch_stats.scanned_file_count++;

                // If we already have this file in the "cache since we started", skip it
                if (folder_watcher_manager.TypedTarget.HaveProcessedFile(obj.FullPath))
                {
                    Logging.Debug("FolderWatcher is skipping {0} as it has already been processed", obj.FullPath);
                    watch_stats.skipped_file_count++;
                    break;
                }

                if (have_we_slept)
                {
                    // As we have slept a while, it's quite unsure whether that file still exists.
                    // Include it only when it still exists and otherwise be sure to retrigger a scan to follow up
                    // any other directory changes.
                    if (!File.Exists(obj.FullPath))
                    {
                        Logging.Info("FolderWatcher is skipping {0} as it has disappeared while we were sleeping", obj.FullPath);
                        FolderContentsHaveChanged = true;
                        break;
                    }
                }

                // ignore zero-length and tiny sized files as those sure are buggy/illegal PDFs:
                //
                // https://stackoverflow.com/questions/17279712/what-is-the-smallest-possible-valid-pdf
                if (obj.FileSize <= 66)
                {
                    Logging.Warn("FolderWatcher is skipping {0} as it is too small to be a valid PDF file @ {1} bytes", obj.FullPath, obj.FileSize);
                    break;
                }

                // Check that the file is not still locked - if it is, mark that the folder is still "changed" and come back later.
                //
                // We do this at the same time as calculating the file fingerprint as both actions require (costly) File I/O
                // and can be folded together: if the fingerprint fails, that's 99.9% sure a failure in the File I/O, hence
                // a locked or otherwise inaccessible file.
                string fingerprint;
                try
                {
                    fingerprint = StreamFingerprint.FromFile(obj.FullPath);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Watched folder contains file '{0}' which is locked, so coming back later...", obj.FullPath);
                    FolderContentsHaveChanged = true;
                    break;
                }

                // check if the PDF is already known:
                PDFDocument doc = LibraryRef.Xlibrary.GetDocumentByFingerprint(fingerprint);

                if (doc != null)
                {
                    // Add this file to the list of processed files...
                    Logging.Info("FolderWatcher is skipping {0} as it already exists in the library as fingerprint {1}, title: {2}", obj.FullPath, fingerprint, doc.TitleCombined);
                    folder_watcher_manager.TypedTarget.RememberProcessedFile(obj.FullPath);
                    watch_stats.skipped_file_count++;
                    break;
                }

                if (watch_stats.file_hashes_added.TryGetValue(fingerprint, out var dupe_file_path))
                {
                    Logging.Info("FolderWatcher is skipping {0} as it has already been included in the import set as file {1} which has the same fingerprint {2}", obj.FullPath, dupe_file_path, fingerprint);
                    watch_stats.skipped_file_count++;
                    break;
                }

                watch_stats.file_hashes_added.Add(fingerprint, obj.FullPath);

                return true;
            }

            return false;
        }

        //
        // Summary:
        //     Represents the method that defines a set of criteria and determines whether the
        //     specified object meets those criteria.
        //
        // Parameters:
        //   obj:
        //     The object to compare against the criteria defined within the method represented
        //     by this delegate.
        //
        // Type parameters:
        //   T:
        //     The type of the object to compare.
        //
        // Returns:
        //     true if obj meets the criteria defined within the method represented by this
        //     delegate; otherwise, false.
        internal bool DecideIfRecurseDuringDirScan(FileSystemEntryInfo obj)
        {
            Logging.Debug("FolderWatcher: scanning directory: {0}", obj.FullPath);

            return true;
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

            WPFDoEvents.SafeExec(() =>
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    file_system_watcher.EnableRaisingEvents = false;
                    file_system_watcher.Dispose();
                }

                file_system_watcher = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                folder_watcher_manager = null;
                //library.TypedTarget.Dispose();
                web_library_detail = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                tags.Clear();
            });

            WPFDoEvents.SafeExec(() =>
            {
                configured_folder_to_watch = null;
                aspiring_folder_to_watch = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                watch_stats.Reset(null);
            });

            ++dispose_count;
        }

        #endregion
    }
}
