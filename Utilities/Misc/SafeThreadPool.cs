using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Shutdownable;

namespace Utilities.Misc
{
    public static class SafeThreadPool
    {
        public static event UnhandledExceptionEventHandler UnhandledException;

        private static int queued_thread_count = 0;
        private static int running_thread_count = 0;
        private static object queued_thread_count_lock = new object();
        private static object running_thread_count_lock = new object();

        public static int QueuedThreadCount
        {
            get
            {
                lock (queued_thread_count_lock)
                {
                    return queued_thread_count;
                }
            }
        }

        public static int RunningThreadCount
        {
            get
            {
                lock (running_thread_count_lock)
                {
                    return running_thread_count;
                }
            }
        }

        private static void IncrementRunningThreadCount()
        {
            lock (running_thread_count_lock)
            {
                running_thread_count++;
            }
        }

        private static void DecrementRunningThreadCount()
        {
            lock (running_thread_count_lock)
            {
                running_thread_count--;
            }
        }

        public static bool QueueAsyncUserWorkItem(Func<Task> callback, bool skip_task_at_app_shutdown = true)
        {
            lock (queued_thread_count_lock)
            {
                queued_thread_count++;
            }
#if TEST
            int workerThreads;
            int completionPortThreads;

            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            //Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}, CompletedWorkItemCount = {2}, PendingWorkItemCount = {3}, ThreadCount = {4}", workerThreads, completionPortThreads, ThreadPool.CompletedWorkItemCount, ThreadPool.PendingWorkItemCount, ThreadPool.ThreadCount);
            Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}, Queued Threads = {2}", workerThreads, completionPortThreads, QueuedThreadCount);
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MaxThreads = {0} | {1}", workerThreads, completionPortThreads);
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MinThreads = {0} | {1}", workerThreads, completionPortThreads);
#endif
            return ThreadPool.QueueUserWorkItem(o =>
            {
                AsyncUserWorkItem(callback, skip_task_at_app_shutdown);
            });
        }

        private static void AsyncUserWorkItem(Func<Task> callback, bool skip_at_app_shutdown)
        {
            IncrementRunningThreadCount();

            try
            {
                if (skip_at_app_shutdown && ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug特("SafeThreadPool::QueueUserWorkItem: Breaking out due to application termination");
                    return;
                }

                Task t = callback.Invoke();
                //var w = t.ConfigureAwait(false);
                //var a = w.GetAwaiter();
                //a.GetResult();
                t.Wait();
            }
            catch (Exception ex)
            {
                try
                {
                    Logging.Warn(ex, "There has been an exception on the SafeThreadPool context.");
                    if (null != UnhandledException)
                    {
                        UnhandledExceptionEventArgs ueea = new UnhandledExceptionEventArgs(ex, false);
                        UnhandledException(null, ueea);
                    }
                }
                catch (Exception ex2)
                {
                    Logging.Error(ex2, "There was an exception while trying to call back the SafeThreadPool exception callback!");
                }
            }
            finally
            {
                lock (queued_thread_count_lock)
                {
                    queued_thread_count--;
                }
                DecrementRunningThreadCount();
            }
        }

        public static bool QueueUserWorkItem(WaitCallback callback, bool skip_task_at_app_shutdown = true)
        {
            lock (queued_thread_count_lock)
            {
                queued_thread_count++;
            }
#if TEST
            int workerThreads;
            int completionPortThreads;

            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            //Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}, CompletedWorkItemCount = {2}, PendingWorkItemCount = {3}, ThreadCount = {4}", workerThreads, completionPortThreads, ThreadPool.CompletedWorkItemCount, ThreadPool.PendingWorkItemCount, ThreadPool.ThreadCount);
            Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}, Queued Threads = {2}", workerThreads, completionPortThreads, QueuedThreadCount);
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MaxThreads = {0} | {1}", workerThreads, completionPortThreads);
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MinThreads = {0} | {1}", workerThreads, completionPortThreads);
#endif
            return ThreadPool.QueueUserWorkItem(o =>
            {
                UserWorkItem(callback, skip_task_at_app_shutdown);
            });
        }

        private static void UserWorkItem(WaitCallback callback, bool skip_at_app_shutdown)
        {
            IncrementRunningThreadCount();

            try
            {
                if (skip_at_app_shutdown && ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug特("SafeThreadPool::QueueUserWorkItem: Breaking out due to application termination");
                    return;
                }

                callback.Invoke(null);
            }
            catch (Exception ex)
            {
                try
                {
                    Logging.Warn(ex, "There has been an exception on the SafeThreadPool context.");
                    if (null != UnhandledException)
                    {
                        UnhandledExceptionEventArgs ueea = new UnhandledExceptionEventArgs(ex, false);
                        UnhandledException(null, ueea);
                    }
                }
                catch (Exception ex2)
                {
                    Logging.Error(ex2, "There was an exception while trying to call back the SafeThreadPool exception callback!");
                }
            }
            finally
            {
                lock (queued_thread_count_lock)
                {
                    queued_thread_count--;
                }
                DecrementRunningThreadCount();
            }
        }

        public static void SetMaxActiveThreadCount(int count = 0)
        {
            if (0 == count)
            {
                // guestimate the number of *physical* processor cores, assuming HyperThreading is available.
                // see also: https://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c
                count = Math.Max(1, Environment.ProcessorCount / 2);
            }

            // heuristic: allow one CPU thread per core minus 2 (accounting for main thread and others), two I/O thread per core with a minimum of 1 and 3 respectively.
            //
            // Aber Oh-ho! It turns out that, for whatever obscure reason, Qiqqa UI/WPF? does not load correctly or completely when you allocate fewer than 4 threads!
#if false
            int min_cpu_threads = 0, min_io_threads = 0;
            ThreadPool.GetMinThreads(out min_cpu_threads, out min_io_threads);
            min_cpu_threads = Math.Min(Math.Max(4, count - 2), min_cpu_threads);
            min_io_threads = Math.Min(Math.Max(4, 2 * count), min_io_threads);
            ThreadPool.SetMinThreads(4, 4);
            ThreadPool.SetMaxThreads(min_cpu_threads, min_io_threads);
#endif
        }
    }
}
