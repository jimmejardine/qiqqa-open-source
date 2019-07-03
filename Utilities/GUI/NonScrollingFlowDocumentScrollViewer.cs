using System.Windows.Controls;
using System.Windows.Input;

namespace Utilities.GUI
{
    /*
     * This HACKY class is needed to allow the FlowDocumentScrollViewer to not consume the mousewheel even when it's scrollbars are disabled.
     * NB: There is now a better way to do this: hunt for MouseWheelDisabler.DisableMouseWheelForControl(xxx);
     */
    public class NonScrollingFlowDocumentScrollViewer : FlowDocumentScrollViewer
    {
        public NonScrollingFlowDocumentScrollViewer()
        {
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // Don't handle the scroll...
        }
    }
}
