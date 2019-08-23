using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Files;

namespace Qiqqa.DocumentLibrary.FolderWatching
{
    public class FolderWatcherManager
    {
        Library library;

        class FolderWatcherRecord
        {
            public string path;
            public string tags;
            public FolderWatcher folder_watcher;
        }
        Dictionary<string, FolderWatcherRecord> folder_watcher_records = new Dictionary<string, FolderWatcherRecord>();
        object folder_watcher_records_lock = new object();

        HashSet<string> filenames_processed = new HashSet<string>();
        // lock for all Filename_Store File I/O and filenames_processed HashSet:
        object filenames_processed_lock = new object();

        public FolderWatcherManager(Library library)
        {
            this.library = library;

            // Load any pre-existing watched filenames
            bool file_exists;

            lock (filenames_processed_lock)
            {
                file_exists = File.Exists(Filename_Store);
            }
            if (file_exists)
            {
                Logging.Info("Loading memory of files that we watched previously.");

                lock (filenames_processed_lock)
                {
                    foreach (string filename in File.ReadAllLines(Filename_Store))
                    {
                        filenames_processed.Add(filename);
                    }
                }
            }

            Utilities.Maintainable.MaintainableManager.Instance.RegisterHeldOffTask(TaskDaemonEntryPoint, 30 * 1000, System.Threading.ThreadPriority.BelowNormal);
        }

        private int dispose_count = 0;
        internal void Dispose()
        {
            Logging.Debug("FolderWatcherManager::Dispose() @{0}", ++dispose_count);

            lock (folder_watcher_records_lock)
            {
                // Dispose of all the folder watchers
                foreach (var folder_watcher_record in folder_watcher_records)
                {
                    folder_watcher_record.Value.folder_watcher.Dispose();
                }
                folder_watcher_records.Clear();

                //library.Dispose();
                library = null;
            }

            lock (filenames_processed_lock)
            {
                filenames_processed.Clear();
            }
        }

        public string Filename_Store
        {
            get
            {
                return library.LIBRARY_BASE_PATH + "Qiqqa.folder_watcher";
            }
        }

        internal void ResetHistory()
        {
            lock (filenames_processed_lock)
            {
                FileTools.Delete(Filename_Store);
                filenames_processed.Clear();
            }
        }
        
        internal bool HaveProcessedFile(string filename)
        {
            lock (filenames_processed_lock)
            {
                return filenames_processed.Contains(filename);
            }
        }
        
        // NOTE: this method will be called from various threads.
        internal void RememberProcessedFile(string filename)
        {
            lock (filenames_processed_lock)
            {
                File.AppendAllText(Filename_Store, filename + "\n");
                filenames_processed.Add(filename);
            }
        }

        internal void TaskDaemonEntryPoint(Utilities.Daemon daemon)
        {
            Dictionary<string, FolderWatcherRecord> folder_watchset = new Dictionary<string, FolderWatcherRecord>();

            lock (folder_watcher_records_lock)
            {
                // Get the new list of folders to watch
                string folders_to_watch_batch = library.WebLibraryDetail.FolderToWatch;
                HashSet<string> folders_to_watch = new HashSet<string>();
                if (null != folders_to_watch_batch)
                {
                    foreach (var folder_to_watch_batch in folders_to_watch_batch.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        folders_to_watch.Add(folder_to_watch_batch);
                    }
                }

                // Kill off any unwanted folders
                HashSet<string> folders_to_watch_deleted = new HashSet<string>(folder_watcher_records.Keys);
                foreach (var folder_to_watch in folders_to_watch)
                {
                    folders_to_watch_deleted.Remove(folder_to_watch);
                }
                foreach (var folder_to_watch_deleted in folders_to_watch_deleted)
                {
                    Logging.Info("Deleting FolderWatcher for '{0}'", folder_to_watch_deleted);

                    folder_watcher_records[folder_to_watch_deleted].folder_watcher.Dispose();
                    folder_watcher_records.Remove(folder_to_watch_deleted);
                }

                // Create any new folders
                foreach (var folder_to_watch in folders_to_watch)
                {
                    if (!folder_watcher_records.ContainsKey(folder_to_watch))
                    {
                        Logging.Info("Creating FolderWatcher for '{0}'", folder_to_watch);
                        string[] parts = folder_to_watch.Split(new char[] { ';' }, 2);

                        FolderWatcherRecord fwr = new FolderWatcherRecord();
                        fwr.path = parts[0];
                        fwr.tags = (1 < parts.Length) ? parts[1] : null;
                        fwr.folder_watcher = new FolderWatcher(this, library, fwr.path, fwr.tags);

                        folder_watcher_records[folder_to_watch] = fwr;
                    }
                }

                // Copy the list to local dict:
                foreach (var folder_watcher_record in folder_watcher_records)
                {
                    folder_watchset.Add(folder_watcher_record.Key, folder_watcher_record.Value);
                }
            }

            // Do the actual folder processing
            foreach (var folder_watcher_record in folder_watchset)
            {
                try
                {
                    folder_watcher_record.Value.folder_watcher.TaskDaemonEntryPoint(daemon);
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was an exception while processing the watched folder '{0}'.", folder_watcher_record.Value.path);
                }
            }
        }
    }
}
