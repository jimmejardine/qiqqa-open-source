using System;
using System.Collections.Generic;
using System.Threading;
using Utilities.Shutdownable;

namespace Utilities.Misc
{
    /// <summary>
    /// Queues pending notifications, and fires out new notification events with minimal time intervals inbetween.
    /// </summary>
    public class NotificationManager
    {
        #region Notification Struct

        /// <summary>
        /// Contains all the information about a notification, only the main text is required, the rest can be null.
        /// </summary>
        public class Notification
        {
            public Notification(string text, string tooltip, NotificationType type, string image_name)
            {
                Text = text;
                ToolTip = tooltip;
                Type = type;
                ImageName = image_name;
            }

            /// <summary>
            /// Single button version.
            /// </summary>
            public Notification(string text, string tooltip, NotificationType type, string image_name, string button_text, WaitCallback stuff_to_do_when_button_pressed)
                : this(text, tooltip, type, image_name)
            {
                Button1 = new NotificationButton(button_text, stuff_to_do_when_button_pressed);
            }

            /// <summary>
            /// Two button version.
            /// </summary>
            public Notification(string text, string tooltip, NotificationType type, string image_name, string button_text_1, WaitCallback stuff_to_do_when_button_pressed_1, string button_text_2, WaitCallback stuff_to_do_when_button_pressed_2)
                : this(text, tooltip, type, image_name, button_text_1, stuff_to_do_when_button_pressed_1)
            {
                Button2 = new NotificationButton(button_text_2, stuff_to_do_when_button_pressed_2);
            }

            public string Text { get; set; }

            private string tooltip;
            public string ToolTip
            {
                get => (tooltip ?? Text);
                set => tooltip = value;
            }
            public NotificationType Type { get; set; }
            public string ImageName { get; set; }
            public NotificationButton Button1 { get; set; }
            public NotificationButton Button2 { get; set; }
        }

        /// <summary>
        /// The button for the notification.
        /// </summary>
        public class NotificationButton
        {
            public NotificationButton(string text, WaitCallback stuff_to_do_when_pressed)
            {
                Text = text;
                StuffToDoWhenPressed = stuff_to_do_when_pressed;
            }

            public string Text { get; set; }
            public WaitCallback StuffToDoWhenPressed { get; set; }
        }

        public enum NotificationType
        {
            Info,
            Warning,
            Error
        }

        #endregion

        private readonly Queue<Notification> pending_notifications = new Queue<Notification>(2);
        private readonly ReaderWriterLockSlim pending_notifications_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private Daemon daemon;

        private DateTime last_notification_fired = DateTime.MinValue;
        private const int DelayBetweenNotificationsSeconds = 60;
        public delegate void NewNotificationFiredDelegate(Notification notification);
        public event NewNotificationFiredDelegate NewNotificationFired;

        public static readonly NotificationManager Instance = new NotificationManager();
        private readonly AutoResetEvent auto_reset_event = new AutoResetEvent(false);

        private NotificationManager()
        {
            //  register for shutdown
            ShutdownableManager.Instance.Register(DoShutdown);

            //  kick off our thread
            daemon = new Daemon("NotificationManager");
            daemon.Start(StartDaemon);
        }

        public void AddPendingNotification(Notification notification)
        {
            pending_notifications_lock.EnterWriteLock();
            try
            {
                Logging.Debug特("Adding pending notification for: {0}", notification.Text);
                pending_notifications.Enqueue(notification);

                auto_reset_event.Set();
            }
            finally
            {
                pending_notifications_lock.ExitWriteLock();
            }
        }

        private void FireNewNotification(Notification notification)
        {
            Logging.Debug特("Informing notification listeners of new notification for: {0}", notification.Text);
            last_notification_fired = DateTime.Now;
            NewNotificationFiredDelegate dlgt = NewNotificationFired;
            if (dlgt != null)
            {
                SafeThreadPool.QueueUserWorkItem(delegate { NewNotificationFired(notification); });
            }
        }

        private void DoShutdown()
        {
            Logging.Info("Shutting down notifications ({0} pending)", pending_notifications.Count);

            // get out of the WaitOne() *fast*:
            auto_reset_event.Set();

            // signal thread kill:
            if (daemon != null)
            {
                daemon.Stop();
            }
            daemon = null;
        }

        /// <summary>
        /// Check for new notifications and process them one by one, with a minimum break between notifications.
        /// </summary>
        private void StartDaemon(object obj)
        {
            Daemon daemon = (Daemon)obj;

            Logging.Info("Starting Notifications daemon");

            while (daemon.StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                pending_notifications_lock.EnterUpgradeableReadLock();
                try
                {
                    //  process notifications while the notification bar is closed, and there has been a bit of a break since the last notification
                    if (pending_notifications.Count > 0 && (DateTime.Now - last_notification_fired).TotalSeconds > DelayBetweenNotificationsSeconds)
                    {
                        pending_notifications_lock.EnterWriteLock();
                        try
                        {
                            Notification notification = pending_notifications.Dequeue();
                            FireNewNotification(notification);
                        }
                        finally
                        {
                            pending_notifications_lock.ExitWriteLock();
                        }
                    }
                }
                finally
                {
                    pending_notifications_lock.ExitUpgradeableReadLock();
                }

                //  siesta - wait for a set event or timeout
                auto_reset_event.WaitOne(5000);
            }
        }
    }
}
