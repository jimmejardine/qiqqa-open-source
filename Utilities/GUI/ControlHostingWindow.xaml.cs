using System;
using System.ComponentModel;
using System.Windows;

namespace Utilities.GUI
{
    /// <summary>
    /// Interaction logic for ControlHostingWindow.xaml
    /// </summary>
    public partial class ControlHostingWindow : Window
    {
        FrameworkElement control;

        public interface WindowOpenedCapable
       {
           void OnWindowOpened();
        }
        public interface WindowClosedCapable
        {
            void OnWindowClosed();
        }

        public ControlHostingWindow(string title, FrameworkElement control)
            : this(title, control, true)
        {            
        }

        public ControlHostingWindow(string title, FrameworkElement control, bool size_to_content)
        {
            if (size_to_content)
            {
                this.SizeToContent = SizeToContent.WidthAndHeight;
            }

            this.Title = title;
            this.control = control;

            InitializeComponent();

            this.GridContent.Children.Add(control);

            this.Loaded += ControlHostingWindow_Loaded;
            this.Closed += ControlHostingWindow_Closed;
        }

        public FrameworkElement InternalControl
        {
            get
            {
                return control;
            }
        }

        void ControlHostingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowOpenedCapable woc = InternalControl as WindowOpenedCapable;
            if (null != woc) woc.OnWindowOpened();
        }

        void ControlHostingWindow_Closed(object sender, EventArgs e)
        {
            {
                WindowClosedCapable wcc = InternalControl as WindowClosedCapable;
                if (null != wcc)
                {
                    wcc.OnWindowClosed();
                }
            }

            {
                IDisposable disposable = InternalControl as IDisposable;
                this.GridContent.Children.Clear();

                if (null != disposable)
                {
                    disposable.Dispose();
                }
                this.control = null;
            }
        }

        public static void CloseHostedControl(FrameworkElement control)
        {
            ControlHostingWindow chw = GUITools.GetParentControl<ControlHostingWindow>(control);
            if (null != chw)
            {
                chw.Close();
            }
            else
            {
                Logging.Warn("ControlHostingWindow is not a parent of {0}", control.ToString());
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this calss Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

        }
    }
}
