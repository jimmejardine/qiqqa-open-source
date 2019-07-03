using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Utilities.GUI
{
    public class AugmentedScrollViewer : ScrollViewer
    {
        static readonly double INITIAL_ARROW_SCROLL_SIZE = 20;
        static readonly double ARROW_SCROLL_SIZE_DELTA = 2;

        double ARROW_SCROLL_SIZE = 10;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (false) { }

            else if (e.Key == Key.Up)
            {
                this.SmoothScroll(new Point(0, 0), new Point(0, -ARROW_SCROLL_SIZE));
                ARROW_SCROLL_SIZE += ARROW_SCROLL_SIZE_DELTA;
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                this.SmoothScroll(new Point(0, 0), new Point(0, ARROW_SCROLL_SIZE));
                ARROW_SCROLL_SIZE += ARROW_SCROLL_SIZE_DELTA;
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                this.ScrollToVerticalOffset(0);
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                this.ScrollToVerticalOffset(Double.PositiveInfinity);
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            ARROW_SCROLL_SIZE = INITIAL_ARROW_SCROLL_SIZE;
            base.OnKeyUp(e);
        }

        public void ScrollVertical(double delta)
        {
            Scroll(new Point(0, delta));
        }

        public void Scroll(Point delta)
        {
            if (delta.X != 0)
            {
                this.ScrollToHorizontalOffset(this.HorizontalOffset + delta.X);
            }
            if (delta.Y != 0)
            {
                this.ScrollToVerticalOffset(this.VerticalOffset + delta.Y);
            }
        }

        public void SmoothScroll(Point delta, Point gamma)
        {
            Scroll(delta);
            BackgroundQueueRollAfterDrag(gamma);
        }

        object thread_lock = new object();
        Point current_scroll_gamma = new Point();
        bool is_someone_scrolling = false;

        private void BackgroundQueueRollAfterDrag(Point gamma)
        {
            current_scroll_gamma.X = gamma.X;
            current_scroll_gamma.Y = gamma.Y;

            lock (thread_lock)
            {
                if (!is_someone_scrolling)
                {
                    is_someone_scrolling = true;
                    Dispatcher.BeginInvoke(new Action(() => RollAfterDrag()), DispatcherPriority.Background);
                }
            }
        }

        private void RollAfterDrag()
        {
            double ROLL_THRESHOLD = 1;
            double ROLL_FALLOFF = 1.05;

            current_scroll_gamma.X /= ROLL_FALLOFF;
            current_scroll_gamma.Y /= ROLL_FALLOFF;

            Scroll(current_scroll_gamma);

            // Check if we should keep going
            lock (thread_lock)
            {
                if (Math.Abs(current_scroll_gamma.X) < ROLL_THRESHOLD && Math.Abs(current_scroll_gamma.Y) < ROLL_THRESHOLD)
                {
                    is_someone_scrolling = false;                    
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => RollAfterDrag()), DispatcherPriority.Background);
                }
            }
        }
    }
}
