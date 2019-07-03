using System.Windows.Threading;

namespace Utilities.GUI
{
    /// <summary>
    /// A nasty little class that does almost the equivalent of DoEvents in former lifetimes.  
    /// Try to avoid using it.  Obviously you will have a good reason to do so (e.g WPF printing blocks the GUI, grrrrrrrrrrrrrr!!!).
    /// </summary>
    public class WPFDoEvents
    {
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
    }
}
