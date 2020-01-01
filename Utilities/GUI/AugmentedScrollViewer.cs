using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Utilities.GUI
{
    public class AugmentedScrollViewer : ScrollViewer
    {
        private const double INITIAL_ARROW_SCROLL_SIZE = 20;
        private const double ARROW_SCROLL_SIZE_DELTA = 2;

        // the current scroll speed accelleration
        private double arrow_scroll_size = 10;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                SmoothScroll(new Point(0, 0), new Point(0, -arrow_scroll_size));
                // accellerate scrolling while the user keeps the key depressed
                arrow_scroll_size += ARROW_SCROLL_SIZE_DELTA;
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                SmoothScroll(new Point(0, 0), new Point(0, arrow_scroll_size));
                // accellerate scrolling while the user keeps the key depressed
                arrow_scroll_size += ARROW_SCROLL_SIZE_DELTA;
                e.Handled = true;
            }
            else if (e.Key == Key.Home)
            {
                ScrollToVerticalOffset(0);
                e.Handled = true;
            }
            else if (e.Key == Key.End)
            {
                ScrollToVerticalOffset(Double.PositiveInfinity);
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // when the user lifts the key, reset the scroll speed to the default speed
            arrow_scroll_size = INITIAL_ARROW_SCROLL_SIZE;
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
                ScrollToHorizontalOffset(HorizontalOffset + delta.X);
            }
            if (delta.Y != 0)
            {
                ScrollToVerticalOffset(VerticalOffset + delta.Y);
            }
        }

        public void SmoothScroll(Point delta, Point gamma)
        {
            Scroll(delta);
            BackgroundQueueRollAfterDrag(gamma);
        }

        private object thread_lock = new object();
        private Point current_scroll_gamma = new Point();
        private bool is_someone_scrolling = false;

        private void BackgroundQueueRollAfterDrag(Point gamma)
        {
            current_scroll_gamma.X = gamma.X;
            current_scroll_gamma.Y = gamma.Y;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (thread_lock)
            {
                l1_clk.LockPerfTimerStop();
                if (!is_someone_scrolling)
                {
                    is_someone_scrolling = true;
                    WPFDoEvents.InvokeAsyncInUIThread(() => RollAfterDrag(), DispatcherPriority.Background);
                }
            }
        }

        private void RollAfterDrag()
        {
            const double ROLL_THRESHOLD = 1;
            const double ROLL_FALLOFF = 1.05;

            current_scroll_gamma.X /= ROLL_FALLOFF;
            current_scroll_gamma.Y /= ROLL_FALLOFF;

            Scroll(current_scroll_gamma);

            // Check if we should keep going
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (thread_lock)
            {
                l1_clk.LockPerfTimerStop();
                if (Math.Abs(current_scroll_gamma.X) < ROLL_THRESHOLD && Math.Abs(current_scroll_gamma.Y) < ROLL_THRESHOLD)
                {
                    is_someone_scrolling = false;
                }
                else
                {
                    WPFDoEvents.InvokeAsyncInUIThread(() => RollAfterDrag(), DispatcherPriority.Background);
                }
            }
        }
    }
}
