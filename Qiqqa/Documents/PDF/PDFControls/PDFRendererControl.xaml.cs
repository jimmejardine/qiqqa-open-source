using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFControls.BookmarkStuff;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Qiqqa.Documents.PDF.PDFControls.Page;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;
using Qiqqa.Documents.PDF.Search;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.Documents.PDF.PDFControls
{
    /// <summary>
    /// Interaction logic for PDFRendererControl.xaml
    /// </summary>
    public partial class PDFRendererControl : UserControl, IDisposable
    {
        public enum ZoomType
        {
            Other,
            Zoom1Up,
            Zoom2Up,
            ZoomNUp,
            ZoomWholeUp
        }

        public enum OperationMode
        {
            Hand,
            TextSentenceSelect,
            Annotation,
            Highlighter,
            Camera,
            Ink
        }

        private PDFRendererControlStats pdf_renderer_control_stats = null;
        private readonly bool remember_last_read_page;
        private ZoomType zoom_type = ZoomType.Zoom1Up;
        private double last_reasonable_offset_ratio = 0;
        public delegate void ZoomTypeChangedDelegate(ZoomType zoom_type);
        public event ZoomTypeChangedDelegate ZoomTypeChanged;

        private OperationMode operation_mode = OperationMode.Hand;
        public delegate void OperationModeChangedDelegate(OperationMode operation_mode);
        public event OperationModeChangedDelegate OperationModeChanged;

        //private DateTime control_creation_time = DateTime.UtcNow;

        public PDFRendererControl(PDFDocument pdf_document, bool remember_last_read_page) :
            this(pdf_document, remember_last_read_page, ZoomType.Other)
        {
        }

        public PDFRendererControl(PDFDocument pdf_document, bool remember_last_read_page, ZoomType force_zoom_type)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            Theme.Initialize();

            InitializeComponent();

            //Unloaded += PDFRendererControl_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            pdf_renderer_control_stats = new PDFRendererControlStats(pdf_document);
            this.remember_last_read_page = remember_last_read_page;

            ObjPagesPanel.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            PageRenderArea.SizeChanged += PDFRendererControl_SizeChanged;
            KeyUp += PDFRendererControl_KeyUp;
            KeyDown += PDFRendererControl_KeyDown;
            TextInput += PDFRendererControl_TextInput;
            PreviewMouseWheel += PDFRendererControl_MouseWheel;

            ScrollPages.PreviewMouseDown += ScrollPages_PreviewMouseDown;
            ScrollPages.ScrollChanged += ScrollPages_ScrollChanged;

            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);
            KeyboardNavigation.SetDirectionalNavigation(ScrollPages, KeyboardNavigationMode.None);
            KeyboardNavigation.SetDirectionalNavigation(ObjPagesPanel, KeyboardNavigationMode.None);

            // Set the initial zoom
            ZoomType zoom_type = ZoomType.Other;
            switch (ConfigurationManager.Instance.ConfigurationRecord.GUI_LastPagesUp)
            {
                case "1":
                    zoom_type = ZoomType.Zoom1Up;
                    break;

                case "2":
                    zoom_type = ZoomType.Zoom2Up;
                    break;

                case "N":
                    zoom_type = ZoomType.ZoomNUp;
                    break;

                case "W":
                    zoom_type = ZoomType.ZoomWholeUp;
                    break;

                default:
                    zoom_type = ZoomType.Zoom1Up;
                    break;
            }

            // Is the zoom type forced? (e.g. by the metadata panel or the sniffer)
            if (ZoomType.Other != force_zoom_type) zoom_type = force_zoom_type;

            PageZoom(zoom_type);

            var doc = pdf_renderer_control_stats.pdf_document;

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                // Add the child pages
                bool add_bells_and_whistles = (doc.PageCount > 0 && doc.PageCount < 50);

                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    Logging.Info("+Creating child page controls");
                    for (int page = 1; page <= doc.PageCount; ++page)
                    {
                        PDFRendererPageControl page_control = new PDFRendererPageControl(this, page, add_bells_and_whistles);
                        ObjPagesPanel.Children.Add(page_control);
                    }
                    Logging.Info("-Creating child page controls");
                });
            });

            Logging.Info("+Setting initial viewport");
            ReconsiderOperationMode(OperationMode.Hand);

            SetSearchKeywords();  // Eventually this should move into the ReconsiderOperationMode
            ScrollPages.Focus();

            Logging.Info("-Setting initial viewport");
        }

        public PDFRendererControlStats GetPDFRendererControlStats()
        {
            return pdf_renderer_control_stats;
        }

        public PDFDocument GetPDFDocument()
        {
            return pdf_renderer_control_stats.pdf_document;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        // WARNING: https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.unloaded?view=net-5.0
        // Which says:
        //
        // Note that the Unloaded event is not raised after an application begins shutting down.
        // Application shutdown occurs when the condition defined by the ShutdownMode property occurs.
        // If you place cleanup code within a handler for the Unloaded event, such as for a Window
        // or a UserControl, it may not be called as expected.
        private void PDFRendererControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private void PDFRendererControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                IncrementalZoom(Math.Sign(e.Delta));
                e.Handled = true;
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                SidewaysScroll(-e.Delta);
                e.Handled = true;
            }
        }

        private void ScrollPages_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ScrollPages.Focus();
        }

        public void SelectPage(int page_number)
        {
            MoveSelectedPageAbsolute(page_number);
        }

        private DateTime first_scroll_timestamp = DateTime.MaxValue;
        private DateTime selected_page_first_offscreen_timestamp = DateTime.MaxValue;

        private void ScrollPages_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                // Weirdly, ScrollChanged is a bubbling event - not a callback on the very object
                // So you can receive a scroll event from ANY of your children?!!?!!!!!
                // Inside the reading page, there are scrollbars in the annotation popups, which cause this to fire.  So ignore those...
                if (e.Source != ScrollPages)
                {
                    return;
                }

                if (DateTime.MaxValue == first_scroll_timestamp)
                {
                    first_scroll_timestamp = DateTime.UtcNow;

                    if (remember_last_read_page)
                    {
                        if (0 < pdf_renderer_control_stats.pdf_document.PageLastRead)
                        {
                            Logging.Debug("********************************** Restoring page to page {0}", pdf_renderer_control_stats.pdf_document.PageLastRead);
                            PDFRendererPageControl page_control = (PDFRendererPageControl)ObjPagesPanel.Children[pdf_renderer_control_stats.pdf_document.PageLastRead - 1];
                            ASSERT.Test(page_control != null);

                            page_control.BringIntoView();
                        }
                    }
                }

                // Lets see which pages are in view
                PDFRendererPageControl first_page_in_view = null;
                List<PDFRendererPageControl> pages_in_view = new List<PDFRendererPageControl>();
                List<PDFRendererPageControl> pages_not_in_view = new List<PDFRendererPageControl>();
                foreach (PDFRendererPageControl page in ObjPagesPanel.Children.OfType<PDFRendererPageControl>().Reverse())
                {
                    GeneralTransform childTransform = page.TransformToAncestor(ScrollPages);
                    Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), page.RenderSize));
                    Rect result = Rect.Intersect(new Rect(new Point(0, 0), ScrollPages.RenderSize), rectangle);

                    if (result != Rect.Empty)
                    {
                        if (null == first_page_in_view)
                        {
                            first_page_in_view = page;
                        }

                        pages_in_view.Add(page);
                    }
                    else
                    {
                        pages_not_in_view.Add(page);
                    }
                }

                // Check if the selected page has gone off screen.  If so, select the next page.
                if (null != SelectedPage)
                {
                    if (!pages_in_view.Contains(SelectedPage))
                    {
                        // IF this is the first time the selected page has gone off screen, record the moment
                        if (DateTime.MaxValue == selected_page_first_offscreen_timestamp)
                        {
                            selected_page_first_offscreen_timestamp = DateTime.UtcNow;
                        }

                        // We wait for a few moments after it has gone off the screen...2 is arbitrary, but large enough that we can zoom without changing the selected page before the zoom gets time to move the selected page back onto the screen...
                        if (DateTime.UtcNow.Subtract(selected_page_first_offscreen_timestamp).TotalSeconds > 1)
                        {
                            if (null != first_page_in_view)
                            {
                                SelectedPage = first_page_in_view;
                                selected_page_first_offscreen_timestamp = DateTime.MaxValue;
                            }
                        }
                    }
                    else
                    {
                        selected_page_first_offscreen_timestamp = DateTime.MaxValue;
                    }
                }

                // Clear down the pages NOT in view
                foreach (PDFRendererPageControl page in pages_not_in_view)
                {
                    page.SetPageNotInView();
                }

                // Notify the new pages that are in view
                foreach (PDFRendererPageControl page in pages_in_view)
                {
                    page.SetPageInView();
                }

                // If the page has been resized or rescaled, try keep the scrollbars in the same place...
                if (0 != e.ExtentHeightChange)
                {
                    double prev_extent_height = e.ExtentHeight - e.ExtentHeightChange;
                    double vertical_offset_ratio = e.VerticalOffset / prev_extent_height;
                    double new_vertical_offset = vertical_offset_ratio * e.ExtentHeight;

                    if (!Double.IsNaN(new_vertical_offset))
                    {
                        //Logging.Info("Forcing vertical offset from {0} to {1}", e.VerticalOffset, new_vertical_offset);
                        ScrollPages.ScrollToVerticalOffset(new_vertical_offset);
                        return;
                    }
                }

                // Store the last seen page - but not right at the start
                if (DateTime.UtcNow.Subtract(first_scroll_timestamp).TotalSeconds > 1)
                {
                    if (remember_last_read_page)
                    {
                        if (0 < pages_in_view.Count)
                        {
                            PDFRendererPageControl page = pages_in_view[0];

                            // Set the last read page
                            pdf_renderer_control_stats.pdf_document.PageLastRead = page.Page;

                            // Don't notify this now as it causes many writes of the metadata to be done, which is slow for large highlightlists
                            //pdf_renderer_control_stats.pdf_document.Bindable.NotifyPropertyChanged(() => pdf_renderer_control_stats.pdf_document.PageLastRead);
                        }
                    }
                }
            });
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFRendererControl()
        {
            Logging.Debug("~PDFRendererControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFRendererControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFRendererControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        if (!ShutdownableManager.Instance.IsShuttingDown)
                        {
                            pdf_renderer_control_stats?.pdf_document.QueueToStorage();
                        }
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        // Get rid of managed resources
                        List<PDFRendererPageControl> children = new List<PDFRendererPageControl>();
                        foreach (PDFRendererPageControl child in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
                        {
                            children.Add(child);
                        }

                        ObjPagesPanel.Children.Clear();

                        foreach (PDFRendererPageControl child in children)
                        {
                            WPFDoEvents.SafeExec(() =>
                            {
                                child.Dispose();
                            });
                        }
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        pdf_renderer_control_stats?.pdf_document.FlushCachedPageRenderings();
                    }

                    Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    pdf_renderer_control_stats = null;
                });

                ++dispose_count;
            });
        }

        #endregion

        private void PDFRendererControl_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void PDFRendererControl_KeyDown(object sender, KeyEventArgs e)
        {
        }

        internal void PDFRendererControl_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                switch (e.Key)
                {
                    case Key.P:
                        if (ZoomType.Zoom1Up == zoom_type)
                        {
                            PageZoom(ZoomType.Zoom2Up);
                        }
                        else
                        {
                            PageZoom(ZoomType.Zoom1Up);
                        }

                        e.Handled = true;
                        break;

                    case Key.M:
                        ReconsiderOperationMode(OperationMode.Hand);
                        e.Handled = true;

                        break;

                    case Key.A:
                        ReconsiderOperationMode(OperationMode.Annotation);
                        e.Handled = true;
                        break;

                    case Key.H:
                        ReconsiderOperationMode(OperationMode.Highlighter);
                        e.Handled = true;
                        break;

                    case Key.S:
                        ReconsiderOperationMode(OperationMode.TextSentenceSelect);
                        e.Handled = true;
                        break;

                    case Key.I:
                        ReconsiderOperationMode(OperationMode.Ink);
                        e.Handled = true;
                        break;

                    case Key.R:
                        ReconsiderOperationMode(OperationMode.Camera);
                        e.Handled = true;
                        break;

                    case Key.B:
                        GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
                        sniffer.Show(pdf_renderer_control_stats.pdf_document);
                        e.Handled = true;
                        break;

                    case Key.Add:
                    case Key.OemPlus:
                        IncrementalZoom(+1);
                        e.Handled = true;
                        break;

                    case Key.Subtract:
                    case Key.OemMinus:
                        IncrementalZoom(-1);
                        e.Handled = true;
                        break;

                    default:
                        if (Key.D1 <= e.Key && Key.D9 >= e.Key)
                        {
                            if (KeyboardTools.IsShiftDown())
                            {
                                int bookmark_number = BookmarkManager.KeyToBookmarkNumber(e.Key);
                                BookmarkManager.SetDocumentBookmark(pdf_renderer_control_stats.pdf_document, bookmark_number, ScrollPages.VerticalOffset / ScrollPages.ScrollableHeight);
                                StatusManager.Instance.UpdateStatus("Bookmarks", "Set bookmark " + bookmark_number);

                                e.Handled = true;
                            }

                            else
                            {
                                int bookmark_number = BookmarkManager.KeyToBookmarkNumber(e.Key);
                                double vertical_offset = BookmarkManager.GetDocumentBookmark(pdf_renderer_control_stats.pdf_document, bookmark_number);
                                ScrollPages.ScrollToVerticalOffset(vertical_offset * ScrollPages.ScrollableHeight);
                                StatusManager.Instance.UpdateStatus("Bookmarks", "Jumped to bookmark " + bookmark_number);

                                e.Handled = true;
                            }
                        }
                        break;
                }
            }
        }

        #region --- Mouse operation mode --------------------------------------------------------------------------------------------------------

        public void ReconsiderOperationMode(OperationMode operation_mode)
        {
            this.operation_mode = operation_mode;

            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.SetOperationMode(operation_mode);
            }

            OperationModeChanged?.Invoke(operation_mode);
        }

        private PDFSearchResultSet previous_search_result_set = null;
        private PDFSearchResult previous_search_result_placeholder = null;

        public void FlashSelectedSearchItem(PDFSearchResult search_result)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.DocumentSearch_GoToSearchResultLocation);
            if (null != search_result)
            {
                PDFRendererPageControl page_control = (PDFRendererPageControl)ObjPagesPanel.Children[search_result.page - 1];
                ASSERT.Test(page_control != null);

                previous_search_result_placeholder = page_control.SetCurrentSearchPosition(search_result);
            }
        }

        public void SetSearchKeywords()
        {
            PDFSearchResultSet search_result_set = new PDFSearchResultSet();
            SetSearchKeywords(search_result_set);
        }

        public void SetSearchKeywords(PDFSearchResultSet search_result_set)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            // If we have a new seat of search results
            if (previous_search_result_set != search_result_set)
            {
                previous_search_result_set = search_result_set;
                previous_search_result_placeholder = null;

                foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
                {
                    ASSERT.Test(page_control != null);

                    page_control.SetSearchKeywords(search_result_set);
                }
            }

            // Now find and flash the next (or first) placeholder
            {
                // If we are searching again, go from the placeholder
                if (null != previous_search_result_placeholder)
                {
                    for (int page = previous_search_result_placeholder.page; page <= ObjPagesPanel.Children.Count; ++page)
                    {
                        PDFRendererPageControl page_control = (PDFRendererPageControl)ObjPagesPanel.Children[page - 1];
                        ASSERT.Test(page_control != null);

                        previous_search_result_placeholder = page_control?.SetNextSearchPosition(previous_search_result_placeholder);

                        // If it managed to find a successor search position, stick with that
                        if (null != previous_search_result_placeholder)
                        {
                            break;
                        }
                    }
                }

                // If the placeholder hasn't found a successor, start from scratch
                if (null == previous_search_result_placeholder)
                {
                    for (int page = 1; page <= ObjPagesPanel.Children.Count; ++page)
                    {
                        PDFRendererPageControl page_control = (PDFRendererPageControl)ObjPagesPanel.Children[page - 1];
                        ASSERT.Test(page_control != null);

                        previous_search_result_placeholder = page_control?.SetNextSearchPosition(previous_search_result_placeholder);

                        // If it managed to find a successor search position, stick with that
                        if (null != previous_search_result_placeholder)
                        {
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region --- Page navigation --------------------------------------------------------------------------------------------------------

        public PDFRendererPageControl GetPageControl(int page)
        {
            List<PDFRendererPageControl> child_pages = new List<PDFRendererPageControl>(ObjPagesPanel.Children.OfType<PDFRendererPageControl>());

            if (child_pages.Count > 0 && page > 0 && page - 1 < child_pages.Count)
            {
                return (PDFRendererPageControl)child_pages[page - 1];
            }
            else
            {
                return null;
            }
        }

        private PDFRendererPageControl selected_page = null;
        public delegate void SelectedPageChangedDelegate(int page);
        public event SelectedPageChangedDelegate SelectedPageChanged;

        /// <summary>
        /// This is the page that the user has last selected by clicking on it.
        /// </summary>
        public PDFRendererPageControl SelectedPage
        {
            get => selected_page;

            set
            {
                if (value != selected_page)
                {
                    if (null != selected_page)
                    {
                        selected_page.DeselectPage();
                        //selected_page.Dispose();       // wrong idea of mine [GHo]: adding this to "ensure it Dispose()s early" is causing null ref crashes elsewhere in the app. The darn thing stays hairy. :-(
                    }

                    selected_page = value;

                    if (null != selected_page)
                    {
                        selected_page.SelectPage();

                        SelectedPageChanged?.Invoke(selected_page.Page);
                    }
                }
            }
        }

        public void MoveSelectedPageDelta(int direction)
        {
            if (null == pdf_renderer_control_stats)
            {
                return;
            }

            PDFRendererPageControl selected_page = SelectedPage;
            if (null == selected_page)
            {
                MoveSelectedPageAbsolute(1);
                return;
            }

            int page_number = selected_page.PageNumber;
            page_number = page_number + direction;

            // cycle around when jumping to page #N:
            int modulo = Math.Max(1, pdf_renderer_control_stats.pdf_document.PageCount);
            page_number = (page_number - 1) + 2 * modulo;     // guaranteed to end up with a positive number here...
            page_number %= modulo;
            page_number++;            // and make it 1-based again after the MODULO math.

            MoveSelectedPageAbsolute(page_number);
        }

        public void MoveSelectedPageAbsolute(int page)
        {
            Logging.Debug("Moving to page {0}", page);

            // Check that we are not moving to the same page
            {
                PDFRendererPageControl selected_page = SelectedPage;
                if (null != selected_page)
                {
                    if (selected_page.PageNumber == page)
                    {
                        if (!selected_page.PageIsInView)
                        {
                            ScrollPages.ScrollToTop();
                            selected_page.BringIntoView();
                        }

                        return;
                    }
                }
            }

            // Get the new page
            {
                PDFRendererPageControl selected_page = GetPageControl(page);
                if (null != selected_page)
                {
                    if (!selected_page.PageIsInView)
                    {
                        ScrollPages.ScrollToTop();
                        selected_page.BringIntoView();
                    }

                    SelectedPage = selected_page;
                }
            }
        }

        #endregion

        private double last_size_change_height = double.MinValue, last_size_change_width = double.MinValue;

        private void PDFRendererControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                ReconsiderZoom();
                last_size_change_height = e.NewSize.Height;
                last_size_change_width = e.NewSize.Width;
            });
        }

        private void ButtonedZoom(double page_count_abreast)
        {
            if (Double.IsNaN(ObjPagesPanel.ActualWidth))
            {
                return;
            }
            if (0 == ObjPagesPanel.Children.Count)
            {
                return;
            }

            double space_on_page = ObjPagesPanel.ActualWidth;
            space_on_page -= 8 * page_count_abreast;    // The margins
            space_on_page -= 5;     // Some vicious infinite loop in WPF if it matches exactly the width of the container.  So fudge it a little. :-(

            double zoom_factor = space_on_page / pdf_renderer_control_stats.largest_page_image_width / page_count_abreast;
            ZoomAbsolute(zoom_factor);
        }

        private void ZoomFullPage()
        {
            if (Double.IsNaN(ObjPagesPanel.ActualWidth))
            {
                return;
            }
            if (Double.IsNaN(ObjPagesPanel.ActualHeight))
            {
                return;
            }
            if (0 == ObjPagesPanel.Children.Count)
            {
                return;
            }

            PDFRendererPageControl page_control = SelectedPage ?? (PDFRendererPageControl)ObjPagesPanel.Children[0];
            ASSERT.Test(page_control != null);

            double zoom_factor_width = ObjPagesPanel.ActualWidth / page_control.RememberedImageWidth;
            double zoom_factor_height = ObjPagesPanel.ActualHeight / page_control.RememberedImageHeight;
            double zoom_factor = Math.Min(zoom_factor_width, zoom_factor_height);
            ZoomAbsolute(zoom_factor);
        }

        public void PageZoom(ZoomType zoomType)
        {
            zoom_type = zoomType;

            // Store the favourite zoom
            if (remember_last_read_page)
            {
                switch (zoomType)
                {
                    case ZoomType.Zoom1Up:
                        ConfigurationManager.Instance.ConfigurationRecord.GUI_LastPagesUp = "1";
                        break;

                    case ZoomType.Zoom2Up:
                        ConfigurationManager.Instance.ConfigurationRecord.GUI_LastPagesUp = "2";
                        break;

                    case ZoomType.ZoomNUp:
                        ConfigurationManager.Instance.ConfigurationRecord.GUI_LastPagesUp = "N";
                        break;

                    case ZoomType.ZoomWholeUp:
                        ConfigurationManager.Instance.ConfigurationRecord.GUI_LastPagesUp = "W";
                        break;

                    default:
                        break;
                }
            }

            ReconsiderZoom();
        }

        private void ReconsiderZoom()
        {
            // Find the offset position before zoom
            double offset_ratio = ScrollPages.VerticalOffset / ScrollPages.ScrollableHeight;
            if (!Double.IsNaN(offset_ratio))
            {
                last_reasonable_offset_ratio = offset_ratio;
            }

            switch (zoom_type)
            {
                case ZoomType.Zoom1Up:
                    ButtonedZoom(1);
                    break;

                case ZoomType.Zoom2Up:
                    ButtonedZoom(2);
                    break;

                case ZoomType.ZoomNUp:
                    ButtonedZoom(10);
                    break;

                case ZoomType.ZoomWholeUp:
                    ZoomFullPage();
                    break;

                default:
                    break;
            }

            ZoomTypeChanged?.Invoke(zoom_type);
        }

        internal void ToggleZoom()
        {
            if (zoom_type == ZoomType.ZoomNUp)
            {
                zoom_type = ZoomType.ZoomWholeUp;
            }
            else
            {
                zoom_type = ZoomType.ZoomNUp;
            }

            ReconsiderZoom();
        }

        public void SidewaysScroll(double direction)
        {
            ScrollPages.Scroll(new Point(direction, 0));
        }

        public void IncrementalZoom(double direction)
        {
            zoom_type = ZoomType.Other;
            Zoom(direction);
            ReconsiderZoom();
        }

        public void ScrollPageArea(Point delta, Point gamma)
        {
            ScrollPages.SmoothScroll(delta, gamma);
        }

        private void RefreshPages()
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>().Reverse())
            {
                ASSERT.Test(page_control != null);

                page_control.RefreshPage();
            }
        }

        public void ZoomAbsolute(double zoom)
        {
            if (null == pdf_renderer_control_stats)
            {
                return;
            }

            pdf_renderer_control_stats.zoom_factor = zoom;
            pdf_renderer_control_stats.zoom_factor = Math.Min(pdf_renderer_control_stats.zoom_factor, 3);
            pdf_renderer_control_stats.zoom_factor = Math.Max(pdf_renderer_control_stats.zoom_factor, 0.1);
            RefreshPages();
        }

        public void Zoom(double zoom_delta)
        {
            if (null == pdf_renderer_control_stats)
            {
                return;
            }

            double zoom = Math.Exp(Math.Log(pdf_renderer_control_stats.zoom_factor) + zoom_delta / 10.0);
            ZoomAbsolute(zoom);
        }

        public void InvertColours(bool invert)
        {
            if (null == pdf_renderer_control_stats)
            {
                return;
            }

            pdf_renderer_control_stats.are_colours_inverted = invert;
            RefreshPages();
        }

        internal void RaiseHighlightChange(int colourNumber)
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.RaiseHighlightChange(colourNumber);
            }
        }

        internal void RaiseInkChange(InkCanvasEditingMode inkCanvasEditingMode)
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.RaiseInkChange(inkCanvasEditingMode);
            }
        }

        internal void RaiseInkChange(DrawingAttributes drawingAttributes)
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.RaiseInkChange(drawingAttributes);
            }
        }

        internal void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.RaiseTextSelectModeChange(textLayerSelectionMode);
            }
        }

        public delegate void TextSelectedDelegate(string selected_text);
        public event TextSelectedDelegate TextSelected;

        /// <summary>
        /// This gets called by child pages if some text has been selected on those pages.
        /// </summary>
        /// <param name="selected_text"></param>
        internal void OnTextSelected(string selected_text)
        {
            TextSelected?.Invoke(selected_text);
        }

        internal void RotatePage()
        {
            if (null != SelectedPage)
            {
                SelectedPage.RotatePage();
            }
        }

        internal void RotateAllPages()
        {
            foreach (PDFRendererPageControl page_control in ObjPagesPanel.Children.OfType<PDFRendererPageControl>())
            {
                ASSERT.Test(page_control != null);

                page_control.RotatePage();
            }
        }
    }
}
