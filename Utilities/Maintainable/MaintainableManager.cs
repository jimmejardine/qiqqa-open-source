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
            internal int delay_before_repeat_milliseconds;
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
                    do_maintenance_delegate_wrapper?.daemon.Stop();
                }
            }

            // Then go and wait for all to really terminate.
            shutdown_cleanup_action = new WaitCallback(o =>
            {
                Logging.Debug("+Stopping MaintainableManager tasks (async wait callback)");

                if (!CleanupOnShutdown())
                {
                    // queue another cleanup task round to check again
                    SafeThreadPool.QueueUserWorkItem(shutdown_cleanup_action, skip_task_at_app_shutdown: false);
                }
            });
            SafeThreadPool.QueueUserWorkItem(shutdown_cleanup_action, skip_task_at_app_shutdown: false);
        }

        private WaitCallback shutdown_cleanup_action = null;

        private bool CleanupOnShutdown()
        {
            try
            {
                Logging.Debug("+Stopping MaintainableManager tasks (async)");

                Stopwatch shutdown_cleanup_clk = Stopwatch.StartNew();

                // foreach... loop here would get screwed up by inner code invoking the List.Remove() API. So we use this GetFirst() + counter loop instead. Good for threadsafe locking too.
                int cnt = GetItemCount();

                // quickly trigger shutdown for all tasks:
                for (int i = 0; i < cnt; i++)
                {
                    CleanupEntry(i, first_stage_only: true, timeout: 100);
                }

                // now wait for the tasks to terminate properly
                for (int i = 0; i < cnt; i++)
                {
                    CleanupEntry(i, first_stage_only: true, timeout: Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN / cnt - 100);
                }

                // check which threads have terminated already: if they have terminated all, then we're golden!
                cnt = GetPendingItemCount();
                if (cnt > 0)
                {
                    Logging.Info("Stopping MaintainableManager tasks (async): {0} threads (or less) are pending.", cnt);

                    // abort the threads if they're taking way too long:
                    if (shutdown_cleanup_clk.ElapsedMilliseconds >= Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN)
                    {
                        cnt = GetItemCount();
                        for (int i = 0; i < cnt; i++)
                        {
                            CleanupEntry(i, second_stage_only: true);
                        }
                    }
                    // else: run another round to see if the tasks did terminate? This will be handled by an outer caller anyway.

                    cnt = GetPendingItemCount();
                }

                return cnt == 0;
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "MaintainableManager: Cleanup during Shutdown");

                // when an error occurred, we've arrived in an unknown application state as far as the itemcount is concerned,
                // hence we flag this task as 'finished' then.
                return true;
            }
            finally
            {
#if false
                    lock (do_maintenance_delegate_wrappers_lock)
                    {
                        do_maintenance_delegate_wrappers.Clear();
                    }
#endif
            }
        }

        public bool CleanupEntry(int index, bool first_stage_only = false, bool second_stage_only = false, int timeout = 0)
        {
            try
            {
                DoMaintenanceDelegateWrapper w = GetEntry(index);
                if (w != null)
                {
                    if (first_stage_only || !second_stage_only)
                    {
                        Logging.Info("Waiting for Maintainable {0} to terminate.", w.maintainable_description);

                        if (w.daemon.Join(timeout > 0 ? timeout : first_stage_only ? 150 : 2000))
                        {
                            RemoveEntry(index);
                            return true;
                        }
                    }
                }
                else
                {
                    // NULL means we've hit the end of the list or an empty slot. Ignore.
                    return true;
                }

                if (!first_stage_only || second_stage_only)
                {
                    // abort the thread if it's taking way too long:
                    w = GetEntry(index);
                    if (w != null)
                    {
                        if (w.daemon.Join(100))
                        {
                            RemoveEntry(index);
                            return true;
                        }
                        else
                        {
                            Logging.Info("Timeout ({1} ms), hence ABORTing Maintainable thread {0}.", w.maintainable_description, Constants.MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN / 1000);

                            w.daemon.Abort();
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "MaintainableManager: Cleanup entry [{0}] failed.", index);

                return false;
            }
        }

        private int GetItemCount()
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                return do_maintenance_delegate_wrappers.Count;
            }
        }

        private int GetPendingItemCount()
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                int cnt = do_maintenance_delegate_wrappers.Count;
                int pending = 0;

                for (int i = 0; i < cnt; i++)
                {
                    if (null != do_maintenance_delegate_wrappers[i])
                    {
                        pending++;
                    }
                }

                return pending;
            }
        }

        private DoMaintenanceDelegateWrapper GetEntry(int i)
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                if (i >= 0 && i < do_maintenance_delegate_wrappers.Count)
                {
                    return do_maintenance_delegate_wrappers[i];
                }
                return null;
            }
        }

        private void RemoveEntry(int index)
        {
            lock (do_maintenance_delegate_wrappers_lock)
            {
                do_maintenance_delegate_wrappers[index] = null; // do NOT call .Remove(w) as we MUST keep the indexes for all remaining tasks intact.
            }
        }

        public int RegisterHeldOffTask(DoMaintenanceDelegate do_maintenance_delegate, int delay_before_start_milliseconds, int delay_before_repeat_milliseconds = -1, ThreadPriority thread_priority = ThreadPriority.BelowNormal, int hold_off_level = 0, string extra_descr = "")
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            int index;

            if (delay_before_repeat_milliseconds <= 0)
            {
                delay_before_repeat_milliseconds = delay_before_start_milliseconds;
            }

            lock (do_maintenance_delegate_wrappers_lock)
            {
                // l1_clk.LockPerfTimerStop();
                // Set up the wrapper
                DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper = new DoMaintenanceDelegateWrapper();
                do_maintenance_delegate_wrapper.maintainable_description = String.Format("{0}:{1}{2}", do_maintenance_delegate.Target, do_maintenance_delegate.Method.Name, extra_descr);
                do_maintenance_delegate_wrapper.target = new WeakReference(do_maintenance_delegate.Target);
                do_maintenance_delegate_wrapper.method_info = do_maintenance_delegate.Method;
                do_maintenance_delegate_wrapper.delay_before_start_milliseconds = delay_before_start_milliseconds;
                do_maintenance_delegate_wrapper.delay_before_repeat_milliseconds = delay_before_repeat_milliseconds;
                do_maintenance_delegate_wrapper.hold_off_level = hold_off_level;
                do_maintenance_delegate_wrapper.daemon = new Daemon("Maintainable:" + do_maintenance_delegate.Target.GetType().Name + "." + do_maintenance_delegate.Method.Name + extra_descr);

                // Add it to our list of trackers
                index = do_maintenance_delegate_wrappers.Count;
                do_maintenance_delegate_wrappers.Add(do_maintenance_delegate_wrapper);

                // Start the thread
                do_maintenance_delegate_wrapper.daemon.Start(DaemonThreadEntryPoint, do_maintenance_delegate_wrapper);
                do_maintenance_delegate_wrapper.daemon.Priority = thread_priority;
            }

            return index;
        }

        private void DaemonThreadEntryPoint(object wrapper)
        {
            DoMaintenanceDelegateWrapper do_maintenance_delegate_wrapper = (DoMaintenanceDelegateWrapper)wrapper;
            Daemon daemon = do_maintenance_delegate_wrapper.daemon;

            // first wait until the hold off signal is released
            while (daemon.StillRunning && !ShutdownableManager.Instance.IsShuttingDown)
            {
                if (!IsHoldOffPending(do_maintenance_delegate_wrapper.hold_off_level)) break;
                daemon.Sleep(3000);
            }

            // only sleep the extra delay time when there's still a chance we will be running the actual thread code.
            if (daemon.StillRunning && !ShutdownableManager.Instance.IsShuttingDown)
            {
                if (0 != do_maintenance_delegate_wrapper.delay_before_start_milliseconds)
                {
                    Logging.Info("+MaintainableManager is waiting some startup time ({1}ms) for {0}", do_maintenance_delegate_wrapper.maintainable_description, do_maintenance_delegate_wrapper.delay_before_start_milliseconds);
                    daemon.Sleep(do_maintenance_delegate_wrapper.delay_before_start_milliseconds);
                    Logging.Info("-MaintainableManager was waiting some startup time for {0}", do_maintenance_delegate_wrapper.maintainable_description);
                }
            }

            while (daemon.StillRunning && !ShutdownableManager.Instance.IsShuttingDown)
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

                daemon.Sleep(Math.Max(500, do_maintenance_delegate_wrapper.delay_before_repeat_milliseconds));
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
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (hold_off_lock)
            {
                // l1_clk.LockPerfTimerStop();
                hold_off--;
            }
        }
    }
}
