using System;
using System.Collections.Generic;
using Utilities.Shutdownable;

namespace Utilities.Misc
{
    public class StatusManager
    {
        public static StatusManager Instance = new StatusManager();

        public delegate void OnStatusEntryUpdateDelegate(StatusEntry status_entry);
        public event OnStatusEntryUpdateDelegate OnStatusEntryUpdate;

        public class StatusMessage
        {
            public bool cancellable;
            public string message;
            public DateTime timestamp;

            public StatusMessage(string message, bool cancellable)
            {
                this.message = message;
                this.cancellable = cancellable;
                timestamp = DateTime.UtcNow;
            }
        }

        public class StatusEntry
        {
            public string key;

            public DateTime LastUpdated => last_status_message?.timestamp ?? DateTime.MinValue;

            public long current_update_number;
            public long total_update_count;

            protected StatusMessage last_status_message = null;

            public string LastStatusMessage => last_status_message?.message;

            public bool LastStatusMessageCancellable => last_status_message?.cancellable ?? false;

            // Produce a decent Perunage value for any progress status, even when 
            // `total_update_count` is ZERO.
            public double UpdatePercentage => Mathematics.Perunage.Calc(current_update_number, total_update_count);

            public void InsertStatusMessage(StatusMessage msg)
            {
                last_status_message = msg;
            }
        }

        private Dictionary<string, StatusEntry> status_entries = new Dictionary<string, StatusEntry>();

        // do note https://stackoverflow.com/questions/29557718/correct-way-to-lock-the-dictionary-object
        // https://stackoverflow.com/questions/410270/can-you-lock-on-a-generic-dictionary
        // https://stackoverflow.com/questions/16984598/can-i-use-dictionary-elements-as-lock-objects
        // generic advice at https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement#remarks
        private object status_entries_lock = new object();

        private StatusManager()
        {
            ShutdownableManager.Instance.Register(Shutdown);
        }

        private bool still_running = true;

        private void Shutdown()
        {
            Logging.Info("StatusManager is signalling shutdown");
            // canceling all statuses which can be canceled:
            SetAllCancelled();
            still_running = false;
        }

        public void ClearStatus(string key)
        {
            UpdateStatus(key, "", 0, 0);
        }

        public void UpdateStatus(string key, string message, long current_update_number = 0, long total_update_count = 0, bool cancellable = false)
        {
            // Don't log 'ClearStatus' messages: Clear ~ UpdateStatus("",0,0)
            if (current_update_number != 0 || total_update_count != 0 || !String.IsNullOrEmpty(message))
            {
                Logging.Debug特("{0}:{1} ({2}/{3})", key, message, current_update_number, total_update_count);
            }

            // Do log the statuses (above) but stop updating the UI when we're shutting down:
            if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                StatusEntry status_entry;

                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (status_entries_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    if (!status_entries.TryGetValue(key, out status_entry))
                    {
                        status_entry = new StatusEntry();
                        status_entry.key = key;
                        status_entries[key] = status_entry;
                    }

                    //status_entry.last_updated = DateTime.UtcNow;
                    status_entry.current_update_number = current_update_number;
                    status_entry.total_update_count = total_update_count;
                    status_entry.InsertStatusMessage(new StatusMessage(message, cancellable));
                }

                if (null != OnStatusEntryUpdate)
                {
                    OnStatusEntryUpdate(status_entry);
                }
            }
            else
            {
                Logging.Debug("statusManager: application is still running = {0}", still_running);
            }
        }

        private HashSet<string> cancelled_items = new HashSet<string>();
        private object cancelled_items_lock = new object();

        public bool IsCancelled(string key)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (cancelled_items_lock)
            {
                l1_clk.LockPerfTimerStop();
                return cancelled_items.Contains(key);
            }
        }

        public void SetCancelled(string key)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (cancelled_items_lock)
            {
                l1_clk.LockPerfTimerStop();
                cancelled_items.Add(key);
            }
        }

        public void ClearCancelled(string key)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (cancelled_items_lock)
            {
                l1_clk.LockPerfTimerStop();
                cancelled_items.Remove(key);
            }
        }

        public void SetAllCancelled()
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (status_entries_lock)
            {
                l1_clk.LockPerfTimerStop();
                Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (cancelled_items_lock)
                {
                    l2_clk.LockPerfTimerStop();
                    foreach (string key in status_entries.Keys)
                    {
                        cancelled_items.Add(key);
                    }
                }
            }
        }
    }
}
