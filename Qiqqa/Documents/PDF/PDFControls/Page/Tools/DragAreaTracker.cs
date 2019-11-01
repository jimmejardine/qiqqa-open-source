using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class DragAreaTracker
    {
        private Canvas canvas;
        private bool visible;
        private Point mouse_down_point;
        private bool button_left_pressed = false;
        private bool button_right_pressed = false;
        private DragAreaControl current_annotation = null;

        public delegate void OnDragStartedDelegate(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point);
        public event OnDragStartedDelegate OnDragStarted;
        public delegate void OnDragInProgressDelegate(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_move_point);
        public event OnDragInProgressDelegate OnDragInProgress;
        public delegate void OnDragCompleteDelegate(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point);
        public event OnDragCompleteDelegate OnDragComplete;

        public DragAreaTracker(Canvas canvas) :
            this(canvas, true)
        {
        }

        public DragAreaTracker(Canvas canvas, bool visible)
        {
            this.canvas = canvas;
            this.visible = visible;

            canvas.MouseDown += DragAreaTracker_MouseDown;
            canvas.MouseMove += DragAreaTracker_MouseMove;
            canvas.MouseUp += DragAreaTracker_MouseUp;
        }

        private void DragAreaTracker_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouse_down_point = e.GetPosition(canvas);
            button_left_pressed = e.LeftButton == MouseButtonState.Pressed;
            button_right_pressed = e.RightButton == MouseButtonState.Pressed;

            if (null == current_annotation)
            {
                canvas.CaptureMouse();

                current_annotation = new DragAreaControl(visible);

                Canvas.SetLeft(current_annotation, mouse_down_point.X);
                Canvas.SetTop(current_annotation, mouse_down_point.Y);
                current_annotation.Width = 0;
                current_annotation.Height = 0;

                canvas.Children.Add(current_annotation);

                OnDragStarted?.Invoke(button_left_pressed, button_right_pressed, mouse_down_point);

                OnDragInProgress?.Invoke(button_left_pressed, button_right_pressed, mouse_down_point, mouse_down_point);
            }
        }

        private void DragAreaTracker_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != current_annotation)
            {
                Point mouse_move_point = e.GetPosition(canvas);
                Canvas.SetLeft(current_annotation, Math.Min(mouse_move_point.X, mouse_down_point.X));
                Canvas.SetTop(current_annotation, Math.Min(mouse_move_point.Y, mouse_down_point.Y));
                current_annotation.Width = Math.Abs(mouse_move_point.X - mouse_down_point.X);
                current_annotation.Height = Math.Abs(mouse_move_point.Y - mouse_down_point.Y);

                OnDragInProgress?.Invoke(button_left_pressed, button_right_pressed, mouse_down_point, mouse_move_point);
            }
        }

        private void DragAreaTracker_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (null != current_annotation)
            {
                canvas.Children.Remove(current_annotation);
                current_annotation = null;
                canvas.ReleaseMouseCapture();

                Point mouse_up_point = e.GetPosition(canvas);
                OnDragComplete?.Invoke(button_left_pressed, button_right_pressed, mouse_down_point, mouse_up_point);
            }
        }
    }
}
