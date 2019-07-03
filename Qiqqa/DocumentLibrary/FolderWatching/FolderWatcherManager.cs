using Qiqqa.Common.GeneralTaskDaemonStuff;
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

        HashSet<string> filenames_processed = new HashSet<string>();

        public FolderWatcherManager(Library library)
        {
            this.library = library;

            // Load any pre-existing watched filenames
            if (File.Exists(Filename_Store))
            {
                Logging.Info("Loading memory of files that we watched previously.");
                foreach (string filename in File.ReadAllLines(Filename_Store))
                {
                    filenames_processed.Add(filename);
                }
            }

            GeneralTaskDaemon.Instance.AddGeneralTask(TaskDaemonEntryPoint);
        }

        internal void Dispose()
        {
            GeneralTaskDaemon.Instance.RemoveGeneralTask(TaskDaemonEntryPoint);

            // Dispose of all the folder watchers
            foreach (var folder_watcher_record in folder_watcher_records)
            {
                folder_watcher_record.Value.folder_watcher.Dispose();
            }
            folder_watcher_records.Clear();
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
            FileTools.Delete(Filename_Store);
            filenames_processed.Clear();
        }
        
        internal bool HaveProcessedFile(string filename)
        {
            return filenames_processed.Contains(filename);
        }
        
        internal void RememberProcessedFile(string filename)
        {
            File.AppendAllText(Filename_Store, filename + "\n");
            filenames_processed.Add(filename);
        }

        internal void TaskDaemonEntryPoint(Utilities.Daemon daemon)
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
            {
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
            }

            // Create any new folders
            {
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
            }

            // Do the actual folder processing
            foreach (var folder_watcher_record in folder_watcher_records)
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
