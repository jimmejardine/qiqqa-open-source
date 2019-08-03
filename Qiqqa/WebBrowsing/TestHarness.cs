using System.Threading;
using Utilities.GUI;

namespace Qiqqa.WebBrowsing
{
#if TEST
    public class TestHarness
    {
        static WebBrowserHostControl WBC = null;

        public static void Test()
        {
            WebBrowserHostControl wbc = new WebBrowserHostControl();
            ControlHostingWindow chw = new ControlHostingWindow("Web browser", wbc);
            chw.Show();

            WBC = wbc;
            Thread thread = new Thread(ThreadEntry);
            thread.Start();
            thread.IsBackground = true;

        }

        static void ThreadEntry()
        {
            while (true)
            {
                Thread.Sleep(50);
            }
        }
    }
#endif
}
