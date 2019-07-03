using System;
using System.Collections.Generic;

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
                this.timestamp = DateTime.UtcNow;
            }
        }

        public class StatusEntry
        {
            public string key;
            public DateTime last_updated;

            public long current_update_number;
            public long total_update_count;

            public List<StatusMessage> status_messages = new List<StatusMessage>();
            
            
            public string LastStatusMessage
            {
                get
                {
                    if (0 < status_messages.Count)
                    {
                        return status_messages[0].message;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public bool LastStatusMessageCancellable
            {
                get
                {
                    if (0 < status_messages.Count)
                    {
                        return status_messages[0].cancellable;
                    }
                    else
                    {
                        return false;
                    }
                }
            }            
        }


        Dictionary<string, StatusEntry> status_entries = new Dictionary<string, StatusEntry>();
        
        private StatusManager()
        {
        }

        public void ClearStatus(string key)
        {
            UpdateStatus(key, "", 0, 0);
        }

        public void UpdateStatus(string key, string message)
        {
            UpdateStatus(key, message, 0, 0);
        }

        public void UpdateStatusBusy(string key, string message)
        {
            UpdateStatus(key, message, 1, 2);
        }

        public void UpdateStatusBusy(string key, string message, long current_update_number, long total_update_count)
        {
            UpdateStatusBusy(key, message, current_update_number, total_update_count, false);
        }

        public void UpdateStatusBusy(string key, string message, long current_update_number, long total_update_count, bool cancellable)
        {
            UpdateStatus(key, message, current_update_number, total_update_count, cancellable);
        }


        public void UpdateStatus(string key, string message, long current_update_number, long total_update_count)
        {
            UpdateStatus(key, message, current_update_number, total_update_count, false);
        }

        public void UpdateStatus(string key, string message, double percentage_complete, bool cancellable)
        {
            UpdateStatus(key, message, (int)(percentage_complete * 100), 100, cancellable);
        }

        public void UpdateStatus(string key, string message, long current_update_number, long total_update_count, bool cancellable)
        {
            Logging.Info("{0}:{1} ({2}/{3})", key, message, current_update_number, total_update_count);

            StatusEntry status_entry;

            lock (status_entries)
            {                
                if (!status_entries.TryGetValue(key, out status_entry))
                {
                    status_entry = new StatusEntry();
                    status_entry.key = key;                    
                    status_entries[key] = status_entry;
                }

                status_entry.last_updated = DateTime.UtcNow;
                status_entry.current_update_number = current_update_number;
                status_entry.total_update_count = total_update_count;
                status_entry.status_messages.Insert(0, new StatusMessage(message, cancellable));
                while (status_entry.status_messages.Count > 100)
                {
                    status_entry.status_messages.RemoveAt(status_entry.status_messages.Count - 1);
                }
            }

            if (null != OnStatusEntryUpdate)
            {
                OnStatusEntryUpdate(status_entry);
            }
        }

        HashSet<string> cancelled_items = new HashSet<string>();
        
        public bool IsCancelled(string key)
        {
            lock (cancelled_items)
            {
                return cancelled_items.Contains(key);
            }
        }

        public void SetCancelled(string key)
        {
            lock (cancelled_items)
            {
                cancelled_items.Add(key);
            }
        }

        public void ClearCancelled(string key)
        {
            lock (cancelled_items)
            {
                cancelled_items.Remove(key);
            }
        }
    }
}
