using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Hand
{
    /// <summary>
    /// Interaction logic for PDFTextLayer.xaml
    /// </summary>
    public partial class PDFHandLayer : PageLayer, IDisposable
    {
        private PDFRendererControlStats pdf_renderer_control_stats;
        private int page;
        private PDFRendererControl pdf_renderer_control;
        private bool mouse_pressed = false;
        private Point mouse_down_position;
        private Point mouse_last_position;
        private Point mouse_last_delta = new Point();

        public PDFHandLayer(PDFRendererControlStats pdf_renderer_control_stats, int page, PDFRendererControl pdf_renderer_control)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;
            this.pdf_renderer_control = pdf_renderer_control;

            InitializeComponent();

            Background = Brushes.Transparent;
            Cursor = Cursors.Hand;

            int start_page_offset = pdf_renderer_control_stats.StartPageOffset;
            if (0 != start_page_offset)
            {
                ObjPageNumberControl.SetPageNumber(String.Format("{2} ({0}/{1})", page, pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount, (page + start_page_offset - 1)));
            }
            else
            {
                ObjPageNumberControl.SetPageNumber(String.Format("{0}/{1}", page, pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount));
            }

            MouseDown += PDFHandLayer_MouseDown;
            MouseUp += PDFHandLayer_MouseUp;
            MouseMove += PDFHandLayer_MouseMove;

            //Unloaded += PDFHandLayer_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void PDFHandLayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private void PDFHandLayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouse_pressed = false;
            ReleaseMouseCapture();

            pdf_renderer_control.ScrollPageArea(new Point(0, 0), mouse_last_delta);

            e.Handled = true;
        }

        private void PDFHandLayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mouse_pressed = true;
            mouse_last_position = mouse_down_position = e.GetPosition(pdf_renderer_control);

            mouse_last_delta.X = 0;
            mouse_last_delta.Y = 0;

            CaptureMouse();
        }

        private void PDFHandLayer_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse_current_position = e.GetPosition(pdf_renderer_control);

            if (mouse_pressed)
            {
                if (mouse_last_position.X != mouse_current_position.X || mouse_last_position.Y != mouse_current_position.Y)
                {
                    mouse_last_delta.X = mouse_last_position.X - mouse_current_position.X;
                    mouse_last_delta.Y = mouse_last_position.Y - mouse_current_position.Y;

                    pdf_renderer_control.ScrollPageArea(mouse_last_delta, new Point(0, 0));
                }
            }

            mouse_last_position = mouse_current_position;
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFHandLayer()
        {
            Logging.Debug("~PDFHandLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFHandLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFHandLayer::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (0 == dispose_count)
                    {
                        WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                        foreach (var el in Children)
                        {
                            IDisposable node = el as IDisposable;
                            if (null != node)
                            {
                                node.Dispose();
                            }
                        }
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    Children.Clear();
                });

                WPFDoEvents.SafeExec(() =>
                {
                    MouseDown -= PDFHandLayer_MouseDown;
                    MouseUp -= PDFHandLayer_MouseUp;
                    MouseMove -= PDFHandLayer_MouseMove;

                    Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    // Clear the references for sanity's sake
                    pdf_renderer_control_stats = null;
                    pdf_renderer_control = null;
                });

                ++dispose_count;

                //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
            });
        }

        #endregion

    }
}
