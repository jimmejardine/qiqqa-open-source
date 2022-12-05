using System;
using System.Collections.Generic;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.FolderWatching
{
    public class FolderWatcherManager
    {
        private TypedWeakReference<WebLibraryDetail> web_library_detail;
        public WebLibraryDetail LibraryRef => web_library_detail?.TypedTarget;

        private class FolderWatcherRecord
        {
            public string path;
            public string tags;
            public FolderWatcher folder_watcher;
        }

        private Dictionary<string, FolderWatcherRecord> folder_watcher_records = new Dictionary<string, FolderWatcherRecord>();
        private object folder_watcher_records_lock = new object();
        // TODO: Remove the HashSet: > 1MByte for 7000 entries
        private HashSet<string> filenames_processed = new HashSet<string>();
        private int managed_thread_index = -1;

        // lock for all Filename_Store File I/O and filenames_processed HashSet:
        private object filenames_processed_lock = new object();

        public FolderWatcherManager(WebLibraryDetail _library)
        {
            web_library_detail = new TypedWeakReference<WebLibraryDetail>(_library);

            // Load any pre-existing watched filenames
            bool file_exists;

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (filenames_processed_lock)
            {
                // l1_clk.LockPerfTimerStop();
                file_exists = File.Exists(Filename_Store);
            }
            if (file_exists)
            {
                Logging.Info("Loading memory of files that we watched previously.");

                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (filenames_processed_lock)
                {
                    // l2_clk.LockPerfTimerStop();
                    foreach (string filename in File.ReadAllLines(Filename_Store))
                    {
                        filenames_processed.Add(filename);
                    }
                }
            }

            if (ConfigurationManager.IsEnabled(nameof(FolderWatcher)))
            {
                managed_thread_index = Utilities.Maintainable.MaintainableManager.Instance.RegisterHeldOffTask(TaskDaemonEntryPoint, 10 * 1000, extra_descr: $".Lib({LibraryRef})");
            }
        }

#if DIAG
        private int dispose_count = 0;
#endif
        public void Dispose()
        {
#if DIAG
            Logging.Debug("FolderWatcherManager::Dispose() @{0}", ++dispose_count);
#endif

            WPFDoEvents.SafeExec(() =>
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_watcher_records_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    // Dispose of all the folder watchers
                    foreach (var folder_watcher_record in folder_watcher_records)
                    {
                        folder_watcher_record.Value.folder_watcher.Dispose();
                    }
                    folder_watcher_records.Clear();
                }
            });

            WPFDoEvents.SafeExec(() =>
            {
                Utilities.Maintainable.MaintainableManager.Instance.CleanupEntry(managed_thread_index);
            });
            managed_thread_index = -1;

            WPFDoEvents.SafeExec(() =>
            {
                //Library.Dispose();
                web_library_detail = null;
            });

            WPFDoEvents.SafeExec(() =>
            {
                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (filenames_processed_lock)
                {
                    // l2_clk.LockPerfTimerStop();
                    filenames_processed.Clear();
                }
            });
        }

        public int GetBackgroundTaskIndex()
        {
            return managed_thread_index;
        }

        public string Filename_Store => Path.GetFullPath(Path.Combine(LibraryRef.LIBRARY_BASE_PATH, @"Qiqqa.folder_watcher"));

        internal void ResetHistory()
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (filenames_processed_lock)
            {
                // l1_clk.LockPerfTimerStop();
                FileTools.Delete(Filename_Store);
                filenames_processed.Clear();
            }
        }

        internal bool HaveProcessedFile(string filename)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (filenames_processed_lock)
            {
                // l1_clk.LockPerfTimerStop();
                return filenames_processed.Contains(filename);
            }
        }

        // NOTE: this method will be called from various threads.
        internal void RememberProcessedFile(string filename)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (filenames_processed_lock)
            {
                // l1_clk.LockPerfTimerStop();
                File.AppendAllText(Filename_Store, filename + "\n");
                filenames_processed.Add(filename);
            }
        }

        internal void TaskDaemonEntryPoint(Utilities.Daemon daemon)
        {
            try
            {
                if (ConfigurationManager.IsEnabled(nameof(FolderWatcher)))
                {
                    Logging.Debug特("FolderWatcherTask for library {0} SKIPPED: disabled by advanced settings.", LibraryRef);
                }

                Logging.Debug特("FolderWatcherTask for library {0} START", LibraryRef);

                Dictionary<string, FolderWatcherRecord> folder_watchset = new Dictionary<string, FolderWatcherRecord>();

                // Get the new list of folders to watch
                string folders_to_watch_batch = LibraryRef?.FolderToWatch;
                HashSet<string> folders_to_watch = new HashSet<string>();
                if (null != folders_to_watch_batch)
                {
                    foreach (var folder_to_watch_batch in folders_to_watch_batch.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        folders_to_watch.Add(folder_to_watch_batch);
                    }
                }
                else
                {
                    return;
                }

                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (folder_watcher_records_lock)
                {
                    //l1_clk.LockPerfTimerStop();
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
                            fwr.folder_watcher = new FolderWatcher(this, LibraryRef, fwr.path, fwr.tags);

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
                        folder_watcher_record.Value.folder_watcher.ExecuteBackgroundProcess(daemon);
                    }
                    catch (Exception ex)
                    {
                        Logging.Warn(ex, "There was an exception while processing the watched folder '{0}'.", folder_watcher_record.Value.path);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Terminating the Folder Watch background thread due to an otherwise unhandled exception.");
            }
        }

        public void TriggerExec()
        {
            SafeThreadPool.QueueAsyncUserWorkItem(async () =>
            {
                ConfigurationManager.GetDeveloperSettingsReference()[nameof(FolderWatcher)] = true;
                Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks = false;
                ASSERT.Test(ConfigurationManager.IsEnabled(nameof(FolderWatcher)));

                ResetHistory();

                TaskDaemonEntryPoint(null);
            });
        }
    }
}
