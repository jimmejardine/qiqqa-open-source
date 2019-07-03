using System.Threading;

namespace Utilities.Misc
{
    public class ThreadTools
    {
        public static Thread StartThread(WaitCallback callback)
        {
            Thread thread = new Thread(callback.Invoke);
            thread.Priority = ThreadPriority.Lowest;
            thread.Start(callback);
            return thread;
        }
    }
}
