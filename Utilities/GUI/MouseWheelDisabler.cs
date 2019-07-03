using System.Windows;
using System.Windows.Input;

namespace Utilities.GUI
{
    public class MouseWheelDisabler
    {
        public static void DisableMouseWheelForControl(FrameworkElement control)
        {
            control.PreviewMouseWheel += control_PreviewMouseWheel;
        }

        static void control_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;

                var parent = GUITools.GetParentControl<UIElement>((FrameworkElement)sender);
                parent.RaiseEvent(eventArg);
            }
        }
    }
}
