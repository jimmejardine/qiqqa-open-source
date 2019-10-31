using System;
using System.Threading;

namespace Utilities.Misc
{
    public class SafeThreadPool
    {
        public static event UnhandledExceptionEventHandler UnhandledException;

        private static int queued_thread_count = 0;
        private static object queued_thread_count_lock = new object();

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
            if (skip_task_at_app_shutdown)
            {
                return ThreadPool.QueueUserWorkItem(o => QueueUserWorkItem_THREAD(callback));
            }
            else
            {
                return ThreadPool.QueueUserWorkItem(o => QueueUserWorkItem_THREAD_NoSkip(callback));
            }
        }

        private SafeThreadPool() { }

        private static void QueueUserWorkItem_THREAD_NoSkip(WaitCallback callback)
        {
            UserWorkItem(callback, skip_at_app_shutdown: false);
        }
        private static void QueueUserWorkItem_THREAD(WaitCallback callback)
        {
            UserWorkItem(callback, skip_at_app_shutdown: true);
        }

        private static void UserWorkItem(WaitCallback callback, bool skip_at_app_shutdown)
        {
            try
            {
                if (!skip_at_app_shutdown && Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
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
            }
        }

        public static void SetMaxActiveThreadCount(int count = 0)
        {
            if (0 == count)
            {
                // determine the number of *logical* processor cores.
                // see also: https://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c
                count = Environment.ProcessorCount;
            }

            // heuristic: allow one CPU thread per core minus 2 (accounting for main thread and others), two I/O thread per core with a minimum of 2 and 6 respectively
            int min_cpu_threads = 0, min_io_threads = 0;
#if false
            ThreadPool.GetMinThreads(out min_cpu_threads, out min_io_threads);
#endif
            min_cpu_threads = Math.Max(Math.Max(2, count - 2), min_cpu_threads);
            min_io_threads = Math.Max(Math.Max(6, 2 * count), min_io_threads);
            ThreadPool.SetMaxThreads(min_cpu_threads, min_io_threads);
        }
    }
}
