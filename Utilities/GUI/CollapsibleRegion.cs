using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Utilities.GUI
{
    [ContentProperty("Child")]
    public class CollapsibleRegion : DockPanel
    {
        class ButtonWithNubbin : Grid
        {
            public Button button;
            public Button nubbin;
            public Button nubbin2;

            public ButtonWithNubbin()
            {
                Theme.Initialize();

                button = new Button();
                button.Background = Brushes.Transparent;
                button.BorderBrush = Brushes.Transparent;
                this.Children.Add(button);

                nubbin = new Button();
                nubbin.Width = 50;
                nubbin.Height = 50;
                nubbin.HorizontalAlignment = HorizontalAlignment.Center;
                nubbin.VerticalAlignment = VerticalAlignment.Center;
                nubbin.Cursor = Cursors.Hand;
                nubbin.BorderBrush = Brushes.Transparent;
                nubbin.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                this.Children.Add(nubbin);

                nubbin2 = new Button();
                nubbin2.Width = 30;
                nubbin2.Height = 30;
                nubbin2.HorizontalAlignment = HorizontalAlignment.Center;
                nubbin2.VerticalAlignment = VerticalAlignment.Center;
                nubbin2.Cursor = Cursors.Hand;
                nubbin2.BorderBrush = Brushes.Transparent;
                nubbin2.Background = ThemeColours.Background_Brush_Blue_VeryVeryDark;
                this.Children.Add(nubbin2);
            }
        }

        static readonly int BORDER_THICKNESS = 5;

        ButtonWithNubbin BL, BR, BT, BB;
        Grid GridContent;

        bool mouse_is_down;
        Point last_down_point;
        Point last_move_point;

        double saved_width;
        double saved_height;

        public CollapsibleRegion()
        {
            // Create our child controls
            BL = new ButtonWithNubbin();
            BR = new ButtonWithNubbin();
            BT = new ButtonWithNubbin();
            BB = new ButtonWithNubbin();
            GridContent = new Grid();

            // Add them to our layout
            SetDock(BL, Dock.Left);
            SetDock(BR, Dock.Right);
            SetDock(BT, Dock.Top);
            SetDock(BB, Dock.Bottom);

            this.Children.Add(BL);
            this.Children.Add(BR);
            this.Children.Add(BT);
            this.Children.Add(BB);
            this.Children.Add(GridContent);

            BL.Visibility = BR.Visibility = BT.Visibility = BB.Visibility = Visibility.Collapsed;
            BT.Height = BB.Height = BORDER_THICKNESS;
            BL.Width = BR.Width = BORDER_THICKNESS;
            BL.Cursor = BR.Cursor = Cursors.SizeWE;
            BT.Cursor = BB.Cursor = Cursors.SizeNS;
            BL.ToolTip = BR.ToolTip = BT.ToolTip = BB.ToolTip = "Click to collapse or expand, or drag to resize.";

            SetupEvents(BL);
            SetupEvents(BR);
            SetupEvents(BT);
            SetupEvents(BB);
        }

        [Bindable(true)]
        public FrameworkElement Child
        {
            set
            {
                GridContent.Children.Clear();
                GridContent.Children.Add(value);
            }

            get
            {
                if (GridContent.Children.Count > 0)
                {
                    return (FrameworkElement)GridContent.Children[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public Dock VisibleEdge
        {
            set
            {
                BL.Visibility = BR.Visibility = BT.Visibility = BB.Visibility = Visibility.Collapsed;

                switch (value)
                {
                    case Dock.Left:
                        BL.Visibility = Visibility.Visible;
                        break;
                    case Dock.Right:
                        BR.Visibility = Visibility.Visible;
                        break;
                    case Dock.Top:
                        BT.Visibility = Visibility.Visible;
                        break;
                    case Dock.Bottom:
                        BB.Visibility = Visibility.Visible;
                        break;
                    default:
                        throw new Exception(String.Format("Unknown Dock value '{0}'", value));
                }
            }
        }

        void SetupEvents(ButtonWithNubbin b)
        {
            b.PreviewMouseDown += b_MouseDown;
            b.PreviewMouseMove += b_MouseMove;
            b.PreviewMouseUp += b_MouseUp;
        }

        void b_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouse_is_down = true;
            last_down_point = e.GetPosition(null);
        }

        void b_MouseMove(object sender, MouseEventArgs e)
        {
            Point current_move_point = e.GetPosition(null);

            if (mouse_is_down)
            {
                double resize_x, resize_y;
                DetermineResize(sender, out resize_x, out resize_y);

                double delta_x = resize_x * (current_move_point.X - last_move_point.X);
                double delta_y = resize_y * (current_move_point.Y - last_move_point.Y);

                if (0 != resize_x)
                {
                    Child.Width = Math.Max(0, Child.ActualWidth + delta_x);
                    Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    GridContent.InvalidateMeasure();
                }
                if (0 != resize_y)
                {
                    Child.Height = Math.Max(0, Child.ActualHeight + delta_y);
                    Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    GridContent.InvalidateMeasure();
                }
            }

            last_move_point = current_move_point;
        }

        void b_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouse_is_down = false;
            Point up_point = e.GetPosition(null);

            if (Distance(up_point, last_down_point) < 5)
            {
                double resize_x, resize_y;
                DetermineResize(sender, out resize_x, out resize_y);

                if (0 != resize_x)
                {
                    if (0 == Child.Width)
                    {
                        Child.Width = saved_width;
                        Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else
                    {
                        saved_width = Child.ActualWidth;
                        Child.Width = 0;
                        Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }

                if (0 != resize_y)
                {
                    if (0 == Child.Height)
                    {
                        Child.Height = saved_height;
                        Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    }
                    else
                    {
                        saved_height = Child.ActualHeight;
                        Child.Height = 0;
                        Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }

        public void ToggleCollapseRestore()
        {
            if (Child.ActualWidth == 0 || Child.ActualHeight == 0)
            {
                Restore();
            }
            else
            {
                Collapse();
            }
        }

        public void Collapse()
        {
            double resize_x, resize_y;
            DetermineResize(null, out resize_x, out resize_y);

            if (0 != resize_x)
            {
                if (Child.ActualWidth > 0)
                {
                    saved_width = Child.ActualWidth;
                    Child.Width = 0;
                    Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            if (0 != resize_y)
            {
                if (Child.ActualHeight > 0)
                {
                    saved_height = Child.ActualHeight;
                    Child.Height = 0;
                    Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        public void Restore()
        {
            double resize_x, resize_y;
            DetermineResize(null, out resize_x, out resize_y);

            if (0 != resize_x)
            {
                Child.Width = saved_width;
                Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
            }

            if (0 != resize_y)
            {
                Child.Height = saved_height;
                Child.Visibility = (0 == Child.Width || 0 == Child.Height) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        void DetermineResize(object sender, out double resize_x, out double resize_y)
        {
            resize_x = 0;
            resize_y = 0;

            if (null != sender)
            {
                if (sender == BL) resize_x = -1;
                if (sender == BR) resize_x = +1;
                if (sender == BT) resize_y = -1;
                if (sender == BB) resize_y = +1;
            }
            else
            {
                if (BL.Visibility == Visibility.Visible) resize_x = -1;
                if (BR.Visibility == Visibility.Visible) resize_x = +1;
                if (BT.Visibility == Visibility.Visible) resize_y = -1;
                if (BB.Visibility == Visibility.Visible) resize_y = +1;
            }
        }

        static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            CollapsibleRegion cr = new CollapsibleRegion();
            cr.VisibleEdge = Dock.Bottom;
            WrapPanel sp = new WrapPanel();
            sp.Children.Add(cr);
            ControlHostingWindow w = new ControlHostingWindow("", sp);
            w.Show();
        }
#endif

        #endregion
    }
}
