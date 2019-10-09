using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Utilities.Shutdownable;

namespace Utilities.Maintainable
{
    public class MaintainableManager
    {
        public static MaintainableManager Instance = new MaintainableManager();
        public delegate void DoMaintenanceDelegate(Daemon daemon);

        class DoMaintenanceDelegateWrapper
        {
            internal string maintainable_description;
            internal WeakReference target;
            internal MethodInfo method_info;

            internal int delay_before_start_milliseconds;
            internal int hold_off_level;

            internal Daemon daemon;
        }

        List<DoMaintenanceDelegateWrapper> do_maintenance_delegate_wrappers = new List<DoMaintenanceDelegateWrapper>();
        object do_maintenance_delegate_wrappers_lock = new object();

        MaintainableManager()
        {
            Logging.Info("Creating MaintainableManager");
            ShutdownableManager.Instance.Register(Shutdown);
        }

        public void Shutdown()
        {
            Logging.Info("Stopping MaintainableManager");

            foreach (DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper in do_maintenance_delegate_wrappers)
            {
                do_maintenance_delegate_wrapper.daemon.Stop();
            }

            foreach (DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper in do_maintenance_delegate_wrappers)
            {
                while (!do_maintenance_delegate_wrapper.daemon.Join(1000))
                {
                    Logging.Info("Waiting for Maintainable {0} to terminate.", do_maintenance_delegate_wrapper.maintainable_description);
                    double memsize1 = GC.GetTotalMemory(false);
                    GC.Collect();
                    double memsize2 = GC.GetTotalMemory(true);
                    Logging.Info("While Waiting to terminate, GC collect => memory {0:0.000}K -> {1:0.000}K.", memsize1 / 1E3, memsize2 / 1E3);
                }
            }
        }

        public void RegisterHeldOffTask(DoMaintenanceDelegate do_maintenance_delegate, int delay_before_start_milliseconds, ThreadPriority thread_priority, int hold_off_level = 0)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (do_maintenance_delegate_wrappers_lock)
            {
                l1_clk.LockPerfTimerStop();
                // Set up the wrapper
                DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper = new DoMaintenanceDelegateWrapper();
                do_maintenance_delegate_wrapper.maintainable_description = String.Format("{0}:{1}", do_maintenance_delegate.Target, do_maintenance_delegate.Method.Name);
                do_maintenance_delegate_wrapper.target = new WeakReference(do_maintenance_delegate.Target);
                do_maintenance_delegate_wrapper.method_info = do_maintenance_delegate.Method;
                do_maintenance_delegate_wrapper.delay_before_start_milliseconds = delay_before_start_milliseconds;
                do_maintenance_delegate_wrapper.hold_off_level = hold_off_level;
                do_maintenance_delegate_wrapper.daemon = new Daemon("Maintainable:" + do_maintenance_delegate.Target.GetType().Name + "." + do_maintenance_delegate.Method.Name);
                
                // Add it to our list of trackers
                do_maintenance_delegate_wrappers.Add(do_maintenance_delegate_wrapper);

                // Start the thread
                do_maintenance_delegate_wrapper.daemon.Start(DaemonThreadEntryPoint, do_maintenance_delegate_wrapper);
                do_maintenance_delegate_wrapper.daemon.Priority = thread_priority;
            }
        }

        void DaemonThreadEntryPoint(object wrapper)
        {
            DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper = (DoMaintenanceDelegateWrapper)wrapper;
            Daemon daemon = do_maintenance_delegate_wrapper.daemon;

            // first wait until the hold off signal is released
            while (daemon.StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                if (!IsHoldOffPending(do_maintenance_delegate_wrapper.hold_off_level)) break;
                daemon.Sleep(1000);
            }

            // only sleep the extra delay time when there's still a chance we will be running the actual thread code.
            if (daemon.StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                if (0 != do_maintenance_delegate_wrapper.delay_before_start_milliseconds)
                {
                    Logging.Info("+MaintainableManager is waiting some startup time for {0}", do_maintenance_delegate_wrapper.maintainable_description);
                    daemon.Sleep(do_maintenance_delegate_wrapper.delay_before_start_milliseconds);
                    Logging.Info("-MaintainableManager is waiting some startup time for {0}", do_maintenance_delegate_wrapper.maintainable_description);
                }
            }

            while (daemon.StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                try
                {
                    object target = do_maintenance_delegate_wrapper.target.Target;
                    if (null != target)
                    {
                        do_maintenance_delegate_wrapper.method_info.Invoke(target, new object[] { daemon } );
                        target = null;
                    }
                    else
                    {
                        Logging.Info("Target maintainable ({0}) has been garbage collected, so closing down Maintainable thread.", do_maintenance_delegate_wrapper.maintainable_description);
                        daemon.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Maintainable {0} has thrown an unhandled exception.", do_maintenance_delegate_wrapper.maintainable_description);
                }

                daemon.Sleep();
            }
        }

        private int hold_off = 3;
        private object hold_off_lock = new object();

        public bool IsHoldOffPending(int level)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (hold_off_lock)
            {
                //l1_clk.LockPerfTimerStop();
                return (hold_off > level);
            }
        }
        public void BumpHoldOffPendingLevel()
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (hold_off_lock)
            {
                l1_clk.LockPerfTimerStop();
                hold_off--;
            }
        }
    }
}
