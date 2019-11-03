using System;
using System.Windows;
using System.Windows.Input;

namespace Utilities.GUI
{
    public class DragDropHelper
    {
        private FrameworkElement drag_origination_element;
        private GetDragDataDelegate get_drag_data_delegate;
        private Point dragStart;
        private bool drag_in_operation;

        public delegate object GetDragDataDelegate();

        public DragDropHelper(FrameworkElement drag_origination_element, GetDragDataDelegate get_drag_data_delegate)
        {
            this.drag_origination_element = drag_origination_element;
            this.get_drag_data_delegate = get_drag_data_delegate;

            drag_origination_element.PreviewMouseLeftButtonDown += dragElement_MouseLeftButtonDown;
            drag_origination_element.PreviewMouseMove += dragElement_MouseMove;
        }

        private void dragElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(null);
            drag_in_operation = false;
        }

        private void dragElement_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPos = e.GetPosition(null);

            if (
                !drag_in_operation &&
                e.LeftButton == MouseButtonState.Pressed &&
                ((Math.Abs(currentPos.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs(currentPos.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance))
                )
            {
                drag_in_operation = true;

                object drag_data = get_drag_data_delegate();
                DragDropEffects de = DragDrop.DoDragDrop(drag_origination_element, drag_data, DragDropEffects.Copy);
            }
        }
    }
}
