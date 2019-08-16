using System;
using System.Threading;

namespace Utilities.Misc
{
    public class SafeThreadPool
    {
        public static event UnhandledExceptionEventHandler UnhandledException;

        public static bool QueueUserWorkItem(WaitCallback callback)
        {
#if DEBUG
            int workerThreads;
            int completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            //Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}, CompletedWorkItemCount = {2}, PendingWorkItemCount = {3}, ThreadCount = {4}", workerThreads, completionPortThreads, ThreadPool.CompletedWorkItemCount, ThreadPool.PendingWorkItemCount, ThreadPool.ThreadCount);
            Logging.Debug("QueueUserWorkItem: AvailableThreads = {0} | {1}", workerThreads, completionPortThreads);
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MaxThreads = {0} | {1}", workerThreads, completionPortThreads);
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            Logging.Debug("QueueUserWorkItem: MinThreads = {0} | {1}", workerThreads, completionPortThreads);
#endif
            return ThreadPool.QueueUserWorkItem(o => QueueUserWorkItem_THREAD(callback));
        }

        private SafeThreadPool() { }

        private static void QueueUserWorkItem_THREAD(WaitCallback callback)
        {
            try
            {
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
        }

        public static void SetMaxActiveThreadCount(int count = 0)
        {
            if (0 == count)
            {
                // determine the number of *logical* processor cores.
                // see also: https://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c
                count = Math.Max(2, Environment.ProcessorCount);
            }

            // heuristic: allow two CPU threads per core, two I/O thread per core with a minimum of 6
            ThreadPool.SetMaxThreads(2 * count, Math.Max(6, 2 * count));
            ThreadPool.SetMaxThreads(1, 2);
        }
    }
}
