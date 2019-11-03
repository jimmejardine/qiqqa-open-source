using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Utilities.Maintainable
{
    public class MaintainableManager
    {
        public static MaintainableManager Instance = new MaintainableManager();
        public delegate void DoMaintenanceDelegate(Daemon daemon);

        private class DoMaintenanceDelegateWrapper
        {
            internal string maintainable_description;
            internal WeakReference target;
            internal MethodInfo method_info;

            internal int delay_before_start_milliseconds;
            internal int hold_off_level;

            internal Daemon daemon;
        }

        private List<DoMaintenanceDelegateWrapper> do_maintenance_delegate_wrappers = new List<DoMaintenanceDelegateWrapper>();
        private object do_maintenance_delegate_wrappers_lock = new object();

        private MaintainableManager()
        {
            Logging.Info("Creating MaintainableManager");
            ShutdownableManager.Instance.Register(Shutdown);
        }

        public void Shutdown()
        {
            Logging.Info("Stopping MaintainableManager");

            // signal everyone it's STOP time ASAP:
            lock (do_maintenance_delegate_wrappers_lock)
            {
                foreach (DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper in do_maintenance_delegate_wrappers)
                {
                    do_maintenance_delegate_wrapper.daemon.Stop();
                }
            }

            // Then go and wait for all to really terminate.
            shutdown_cleanup_action = new WaitCallback(o =>
            {
                Logging.Info("+Stopping MaintainableManager tasks (async)");

                if (!CleanupOnShutdown())
                {
                    // queue another cleanup task round to check again
                    SafeThreadPool.QueueUserWorkItem(shutdown_cleanup_action, skip_task_at_app_shutdown: false);
                }
            });
            SafeThreadPool.QueueUserWorkItem(shutdown_cleanup_action, skip_task_at_app_shutdown: false);
        }

        private Stopwatch shutdown_cleanup_clk = null;
        private WaitCallback shutdown_cleanup_action = null;

        private bool CleanupOnShutdown()
        {
            try
            {
                Logging.Info("+Stopping MaintainableManager tasks (async)");

                if (null == shutdown_cleanup_clk)
                {
                    shutdown_cleanup_clk = Stopwatch.StartNew();
                }

                // foreach... loop here would get screwed up by inner code invoking the List.Remove() API. So we use this GetFirst() + counter loop instead. Good for threadsafe locking too.
                DoMaintenanceDelegateWrapper w;
                int cnt = GetItemCount();

                for (int i = 0; i < cnt; i++)
                {
                    w = GetEntry(i);
                    if (w != null)
                    {
                        Logging.Info("Waiting for Maintainable {0} to terminate.", w.maintainable_description);

                        if (w.daemon.Join(100))
                        {
                            RemoveEntry(w);
                            // Play nasty: we know this item was at index [i], hence there's a new item now at [i]
                            // OR we're gonna hit the end of the list. Either way, we're good to go:
                            i--;
                            continue;
                        }
                    }
                    else
                    {
                        // NULL means we've hit the end of the list. Break out of the loop.

#if TEST
                            double memsize1 = GC.GetTotalMemory(false);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                            GC.Collect();
                            double memsize2 = GC.GetTotalMemory(true);
                            Logging.Info("While Waiting to terminate, GC collect => memory {0:0.000}K -> {1:0.000}K.", memsize1 / 1E3, memsize2 / 1E3);
#endif

                        break;
                    }
                }

                cnt = GetItemCount();
                Logging.Info("Stopping MaintainableManager tasks (async): {0} threads are pending.", cnt);
                if (cnt == 0)
                {
                    return true;
                }

                // abort the threads if they're taking way too long:
                if (shutdown_cleanup_clk.ElapsedMilliseconds >= Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN)
                {
                    for (int i = 0; i < cnt; i++)
                    {
                        w = GetEntry(i);
                        if (w != null)
                        {
                            Logging.Info("Timeout ({1} sec), hence ABORTing Maintainable thread {0}.", w.maintainable_description, Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN / 1000);

                            w.daemon.Abort();
                        }
                    }

                    lock (do_maintenance_delegate_wrappers_lock)
                    {
                        do_maintenance_delegate_wrappers.Clear();
                    }

                    return true;
                }
                else
                {
                    // run another round to see if the tasks did terminate:
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "MaintainableManager: Cleanup during Shutdown");

                // when an error occurred, we've arrived in an unknown application state as far as the itemcount is concerned,
                // hence we flag this task as 'finished' then.
                return true;
            }
        }

        private int GetItemCount()
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                return do_maintenance_delegate_wrappers.Count;
            }
        }

        private DoMaintenanceDelegateWrapper GetEntry(int i)
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                if (i < 0 || i >= do_maintenance_delegate_wrappers.Count)
                {
                    return do_maintenance_delegate_wrappers[i];
                }
                return null;
            }
        }

        private void RemoveEntry(DoMaintenanceDelegateWrapper w)
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                do_maintenance_delegate_wrappers.Remove(w);
            }
        }

        public void RegisterHeldOffTask(DoMaintenanceDelegate do_maintenance_delegate, int delay_before_start_milliseconds, ThreadPriority thread_priority, int hold_off_level = 0, string extra_descr = "")
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (do_maintenance_delegate_wrappers_lock)
            {
                l1_clk.LockPerfTimerStop();
                // Set up the wrapper
                DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper = new DoMaintenanceDelegateWrapper();
                do_maintenance_delegate_wrapper.maintainable_description = String.Format("{0}:{1}{2}", do_maintenance_delegate.Target, do_maintenance_delegate.Method.Name, extra_descr);
                do_maintenance_delegate_wrapper.target = new WeakReference(do_maintenance_delegate.Target);
                do_maintenance_delegate_wrapper.method_info = do_maintenance_delegate.Method;
                do_maintenance_delegate_wrapper.delay_before_start_milliseconds = delay_before_start_milliseconds;
                do_maintenance_delegate_wrapper.hold_off_level = hold_off_level;
                do_maintenance_delegate_wrapper.daemon = new Daemon("Maintainable:" + do_maintenance_delegate.Target.GetType().Name + "." + do_maintenance_delegate.Method.Name + extra_descr);

                // Add it to our list of trackers
                do_maintenance_delegate_wrappers.Add(do_maintenance_delegate_wrapper);

                // Start the thread
                do_maintenance_delegate_wrapper.daemon.Start(DaemonThreadEntryPoint, do_maintenance_delegate_wrapper);
                do_maintenance_delegate_wrapper.daemon.Priority = thread_priority;
            }
        }

        private void DaemonThreadEntryPoint(object wrapper)
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
                    Logging.Info("+MaintainableManager is waiting some startup time ({1}ms) for {0}", do_maintenance_delegate_wrapper.maintainable_description, do_maintenance_delegate_wrapper.delay_before_start_milliseconds);
                    daemon.Sleep(do_maintenance_delegate_wrapper.delay_before_start_milliseconds);
                    Logging.Info("-MaintainableManager was waiting some startup time for {0}", do_maintenance_delegate_wrapper.maintainable_description);
                }
            }

            while (daemon.StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                try
                {
                    object target = do_maintenance_delegate_wrapper.target.Target;
                    if (null != target)
                    {
                        do_maintenance_delegate_wrapper.method_info.Invoke(target, new object[] { daemon });
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
