using System;
using System.Collections.Generic;

namespace Utilities.Shutdownable
{
    public class ShutdownableManager
    {
        public static ShutdownableManager Instance = new ShutdownableManager();
        public delegate void ShutdownDelegate();

        List<ShutdownDelegate> shutdown_delegates = new List<ShutdownDelegate>();

        ShutdownableManager()
        {
            Logging.Info("Creating ShutdownableManager");
        }

        public void Register(ShutdownDelegate shutdown_delegate)
        {
            lock (shutdown_delegates)
            {
                Logging.Info("ShutdownableManager is registering {0}", shutdown_delegate.Target);
                shutdown_delegates.Add(shutdown_delegate);
            }
        }

        private bool is_being_shutting_down = false;
        private object is_being_shutting_down_lock = new object();

        public bool IsShuttingDown
        {
            get
            {
                lock (is_being_shutting_down_lock)
                {
                    return is_being_shutting_down;
                }
            }
            set
            {
                lock (is_being_shutting_down_lock)
                {
                    if (!is_being_shutting_down)
                    {
                        is_being_shutting_down = true;
                    }
                }
            }
        }

        public void Shutdown()
        {
            Logging.Info("ShutdownableManager is shutting down all shutdownables:");

            this.IsShuttingDown = true;

            while (true)
            {
                ShutdownDelegate shutdown_delegate = null;
                lock (shutdown_delegates)
                {
                    if (0 == shutdown_delegates.Count) break;
                    shutdown_delegate = shutdown_delegates[0];
                    shutdown_delegates.RemoveAt(0);
                }

                try
                {
                    Logging.Info("ShutdownableManager is shutting down {0}", shutdown_delegate.Target);
                    shutdown_delegate();
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem shutting down Shutdownable {0}", shutdown_delegate.Target);
                }
            }
        }
    }
}
