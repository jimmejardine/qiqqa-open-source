using System;
using System.Collections.Generic;
using System.IO;
using Qiqqa.Common.GeneralTaskDaemonStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Qiqqa.Common.TagManagement;

namespace Qiqqa.DocumentLibrary.FolderWatching
{
    public class FolderWatcher
    {
        FolderWatcherManager folder_watcher_manager;
        Library library;
        HashSet<string> tags;
        FileSystemWatcher file_system_watcher;        
        string previous_folder_to_watch;
        string folder_to_watch;        
        bool folder_contents_has_changed;

        public FolderWatcher(FolderWatcherManager folder_watcher_manager, Library library, string folder_to_watch, string tags)
        {
            this.folder_watcher_manager = folder_watcher_manager;            
            this.library = library;
            this.folder_to_watch = folder_to_watch;
            this.tags = TagTools.ConvertTagBundleToTags(tags);
            this.previous_folder_to_watch = null;

            file_system_watcher = new FileSystemWatcher();
            file_system_watcher.IncludeSubdirectories = true;
            file_system_watcher.Filter = "*.pdf";
            file_system_watcher.Changed += file_system_watcher_Changed;
            file_system_watcher.Created += file_system_watcher_Created;
            previous_folder_to_watch = null;
            folder_contents_has_changed = false;

            file_system_watcher.Path = null;
            file_system_watcher.EnableRaisingEvents = false;
        }

        internal void Dispose()
        {
            if (null != file_system_watcher)
            {
                file_system_watcher.EnableRaisingEvents = false;
                file_system_watcher.Dispose();
                file_system_watcher = null;
            }
        }

        void file_system_watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logging.Info("FolderWatcher file_system_watcher_Changed");
            folder_contents_has_changed = true;
        }

        void file_system_watcher_Created(object sender, FileSystemEventArgs e)
        {
            Logging.Info("FolderWatcher file_system_watcher_Created");
            folder_contents_has_changed = true;
        }

        private void CheckIfFolderNameHasChanged()
        {
            // If they are both null, no worries - they are the same
            if (null == previous_folder_to_watch && null == folder_to_watch)
            {
                file_system_watcher.EnableRaisingEvents = false;
                return;
            }

            // If they are both identical, no worries
            if (null != previous_folder_to_watch && null != folder_to_watch && 0 == previous_folder_to_watch.CompareTo(folder_to_watch))
            {                
                return;
            }

            // If we get here, things have changed
            Logging.Info("FolderWatcher's folder has changed from '{0}' to '{1}'", previous_folder_to_watch, folder_to_watch);
            file_system_watcher.EnableRaisingEvents = false;
            folder_contents_has_changed = true;
            previous_folder_to_watch = folder_to_watch;
            file_system_watcher.Path = folder_to_watch;

            // Start watching if there is something to watch
            if (!String.IsNullOrEmpty(folder_to_watch))
            {
                Logging.Info("FolderWatcher is watching '{0}'", folder_to_watch);
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
                folder_contents_has_changed = true;
                return;
            }

            // Update our fole system watcher if necessary
            CheckIfFolderNameHasChanged();

            // If the current folder is blank, do nothing
            if (String.IsNullOrEmpty(folder_to_watch))
            {
                return;
            }

            // If the folder does not exist, do nothing
            if (!Directory.Exists(folder_to_watch))
            {
                return;
            }


            // If the folder or its contents has not changed since the last time, do nothing
            if (!folder_contents_has_changed)
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
            const int MAX_NUMBER_OF_PDF_FILES_TO_PROCESS = 10;
            const int MAX_SECONDS_PER_ITERATION = 15;
            DateTime index_processing_start_time = DateTime.UtcNow;

            while (folder_contents_has_changed)
            {
                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs)
                {
                    Logging.Info("FolderWatcher: Not daemon processing a library that is busy with adds...");
                    break;
                }

                if (!daemon.StillRunning)
                {
                    Logging.Debug("FolderWatcher: Breaking out of outer processing loop due to daemon termination");
                    break;
                }

                // Mark that we are now processing the folder
                folder_contents_has_changed = false;

                int processed_file_count = 0;

                // If we get this far then there might be some work to do in the folder...
                string[] filenames_in_folder = Directory.GetFiles(previous_folder_to_watch, "*.pdf", SearchOption.AllDirectories);

                List<PDFDocument> pdf_documents_already_in_library = library.PDFDocuments;

                List<string> filenames_that_are_new = new List<string>();
                foreach (string filename in filenames_in_folder)
                {
                    if (DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds > MAX_SECONDS_PER_ITERATION)
                    {
                        Logging.Info("FolderWatcher: Breaking out of inner processing loop due to MAX_SECONDS_PER_ITERATION: {0} seconds consumed", DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds);
                        folder_contents_has_changed = true;
                        break;
                    }

                    // If we already have this file in the "cache since we started", skip it
                    if (folder_watcher_manager.HaveProcessedFile(filename))
                    {
                        Logging.Debug("FolderWatcher is skipping {0} as it has already been processed", filename);
                        continue;
                    }

                    // If we already have this file in the "pdf file locations", skip it
                    bool is_already_in_library = false;
                    foreach (PDFDocument pdf_document in pdf_documents_already_in_library)
                    {
                        if (pdf_document.DownloadLocation == filename)
                        {
                            is_already_in_library = true;
                            break;
                        }
                    }

                    if (is_already_in_library)
                    {
                        // Add this file to the list of processed files...
                        folder_watcher_manager.RememberProcessedFile(filename);

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
                    processed_file_count++;

                    if (processed_file_count > MAX_NUMBER_OF_PDF_FILES_TO_PROCESS)
                    {
                        Logging.Info("FolderWatcher: {0} files have been processed/inspected", processed_file_count);
                        folder_contents_has_changed = true;
                        break;
                    }

                    // Check that the file is not still locked - if it is, mark that the folder is still "changed" and come back later..
                    if (IsFileLocked(filename))
                    {
                        Logging.Info("Watched folder contains file '{0}' which is locked, so coming back later...", filename);
                        folder_contents_has_changed = true;
                        continue;
                    }

                    Logging.Info("FolderWatcher is importing {0}", filename);
                    filenames_that_are_new.Add(filename);

                    // Add this file to the list of processed files...
                    folder_watcher_manager.RememberProcessedFile(filename);
                }

                // Create the import records
                List<ImportingIntoLibrary.FilenameWithMetadataImport> filename_with_metadata_imports = new List<ImportingIntoLibrary.FilenameWithMetadataImport>();
                foreach (var filename in filenames_that_are_new)
                {
                    filename_with_metadata_imports.Add(new ImportingIntoLibrary.FilenameWithMetadataImport { filename = filename, tags = new List<string>(this.tags) });
                }

                // Get the library to import all these new files
                ImportingIntoLibrary.AddNewPDFDocumentsToLibraryWithMetadata_ASYNCHRONOUS(library, true, true, filename_with_metadata_imports.ToArray());

                Logging.Debug("FolderWatcher End-Of-Round");

                daemon.Sleep(3 * 1000);
            }

            Logging.Debug("FolderWatcher END");
        }

        bool IsFileLocked(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}
