using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using icons;
using Qiqqa.Documents.PDF.PDFControls.Page.Annotation;
using Qiqqa.Documents.PDF.PDFControls.Page.Camera;
using Qiqqa.Documents.PDF.PDFControls.Page.Hand;
using Qiqqa.Documents.PDF.PDFControls.Page.Highlight;
using Qiqqa.Documents.PDF.PDFControls.Page.Ink;
using Qiqqa.Documents.PDF.PDFControls.Page.Search;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.Documents.PDF.Search;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Shaders.Negative;
using Utilities.Images;

namespace Qiqqa.Documents.PDF.PDFControls.Page
{
    /// <summary>
    /// Interaction logic for PDFRendererPageControl.xaml
    /// </summary>
    public partial class PDFRendererPageControl : Grid, IDisposable
    {
        internal const int BASIC_PAGE_WIDTH = 850;
        internal const int BASIC_PAGE_HEIGHT = 1100;

        PDFRendererControl pdf_renderer_control = null;
        PDFRendererControlStats pdf_renderer_control_stats = null;
        int page = 0;
        bool add_bells_and_whistles;

        double remembered_image_width = BASIC_PAGE_WIDTH;
        double remembered_image_height = BASIC_PAGE_HEIGHT;

        bool page_is_in_view = false;

        Image ImagePage_HIDDEN_;
        Image ImagePage_HIDDEN
        {
            get
            {
                if (null == ImagePage_HIDDEN_)
                {
                    ImagePage_HIDDEN_ = new Image();
                    ImagePage_HIDDEN.Stretch = Stretch.None;

                    // THIS MUST BE IN PLACE OS THAT WE HAVE PIXEL PERFECT RENDERING
                    ImagePage_HIDDEN.SnapsToDevicePixels = true;
                    RenderOptions.SetBitmapScalingMode(ImagePage_HIDDEN, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(ImagePage_HIDDEN, EdgeMode.Aliased);
                }

                return ImagePage_HIDDEN_;
            }

            set
            {
                if (null == value)
                {
                    ImagePage_HIDDEN_ = null;
                }
            }
        }

        class CurrentlyShowingImageClass
        {
            public BitmapSource Image;
            public double requested_height;
        }

        CurrentlyShowingImageClass _currently_showing_image = null;
        CurrentlyShowingImageClass CurrentlyShowingImage
        {
            get
            {
                return _currently_showing_image;
            }

            set
            {
                _currently_showing_image = value;
                if (null != _currently_showing_image)
                {
                    ImagePage_HIDDEN.Source = _currently_showing_image.Image;
                }
                else
                {
                    ImagePage_HIDDEN.Source = null;
                }
            }
        }

        public double RememberedImageWidth
        {
            get
            {
                return remembered_image_width;
            }
        }
        public double RememberedImageHeight
        {
            get
            {
                return remembered_image_height;
            }
        }

        public int Page
        {
            get
            {
                return page;
            }
        }

        PDFTextSentenceLayer CanvasTextSentence_;
        PDFSearchLayer CanvasSearch_;
        PDFAnnotationLayer CanvasAnnotation_;
        PDFHighlightLayer CanvasHighlight_;
        PDFCameraLayer CanvasCamera_;
        PDFHandLayer CanvasHand_;
        PDFInkLayer CanvasInk_;
        List<PageLayer> page_layers;

        // Provide a cached copy of the PDF document fingerprint for Exception report logging,
        // when this instance has otherwise already been Disposed():
        internal string documentFingerprint = String.Empty;

        public PDFRendererPageControl(int page, PDFRendererControl pdf_renderer_control, PDFRendererControlStats pdf_renderer_control_stats, bool add_bells_and_whistles)
        {
            Theme.Initialize();

            InitializeComponent();

            this.page = page;
            this.pdf_renderer_control = pdf_renderer_control;
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.add_bells_and_whistles = add_bells_and_whistles;

            // Start with a reasonable size
            this.Background = Brushes.White;
            this.Height = remembered_image_height * pdf_renderer_control_stats.zoom_factor;
            this.Width = remembered_image_width * pdf_renderer_control_stats.zoom_factor;

            page_layers = new List<PageLayer>();

            // Try to trap the DAMNED cursor keys escape route
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);

            SetOperationMode(PDFRendererControl.OperationMode.Hand);

            this.MouseDown += PDFRendererPageControl_MouseDown;

            this.pdf_renderer_control_stats.pdf_document.PDFRenderer.OnPageTextAvailable += pdf_renderer_OnPageTextAvailable;

            if (add_bells_and_whistles)
            {
                DropShadowEffect dse = new DropShadowEffect();
                dse.Color = ThemeColours.Background_Color_Blue_Dark;
                this.Effect = dse;
            }

            PopulateNeededLayers();
        }

        #region --- Page layer on-demand creation -------------------

        PDFTextSentenceLayer CanvasTextSentence
        {
            get
            {
                if (null == CanvasTextSentence_)
                {
                    page_layers.Add(CanvasTextSentence_ = new PDFTextSentenceLayer(pdf_renderer_control_stats, page));
                    KeyboardNavigation.SetDirectionalNavigation(CanvasTextSentence_, KeyboardNavigationMode.None);
                    ReflectContentChildren();
                }

                return CanvasTextSentence_;
            }
        }
        PDFSearchLayer CanvasSearch
        {
            get
            {
                if (null == CanvasSearch_)
                {
                    page_layers.Add(CanvasSearch_ = new PDFSearchLayer(pdf_renderer_control_stats, page));
                    ReflectContentChildren();
                }

                return CanvasSearch_;
            }
        }

        PDFAnnotationLayer CanvasAnnotation
        {
            get
            {
                if (null == CanvasAnnotation_)
                {
                    page_layers.Add(CanvasAnnotation_ = new PDFAnnotationLayer(pdf_renderer_control_stats, page));
                    KeyboardNavigation.SetDirectionalNavigation(CanvasAnnotation_, KeyboardNavigationMode.None);
                    ReflectContentChildren();
                }

                return CanvasAnnotation_;
            }
        }

        PDFHighlightLayer CanvasHighlight
        {
            get
            {
                if (null == CanvasHighlight_)
                {
                    page_layers.Add(CanvasHighlight_ = new PDFHighlightLayer(pdf_renderer_control_stats, page));
                    ReflectContentChildren();
                }

                return CanvasHighlight_;
            }
        }

        PDFCameraLayer CanvasCamera
        {
            get
            {
                if (null == CanvasCamera_)
                {
                    page_layers.Add(CanvasCamera_ = new PDFCameraLayer(pdf_renderer_control_stats, page));
                    ReflectContentChildren();
                }

                return CanvasCamera_;
            }
        }

        PDFHandLayer CanvasHand
        {
            get
            {
                if (null == CanvasHand_)
                {
                    page_layers.Add(CanvasHand_ = new PDFHandLayer(pdf_renderer_control_stats, page, pdf_renderer_control));
                    ReflectContentChildren();
                }

                return CanvasHand_;
            }
        }

        PDFInkLayer CanvasInk
        {
            get
            {
                if (null == CanvasInk_)
                {
                    page_layers.Add(CanvasInk_ = new PDFInkLayer(pdf_renderer_control_stats, page));
                    ReflectContentChildren();
                }

                return CanvasInk_;
            }
        }

        #endregion

        private void PopulateNeededLayers()
        {
            if (PDFAnnotationLayer.IsLayerNeeded(pdf_renderer_control_stats, page))
            {
                var a = CanvasAnnotation;
            }
            if (PDFInkLayer.IsLayerNeeded(pdf_renderer_control_stats, page))
            {
                var a = CanvasInk;
            }
            if (PDFHighlightLayer.IsLayerNeeded(pdf_renderer_control_stats, page))
            {
                var a = CanvasHighlight;
            }
        }

        private void ReflectContentChildren()
        {
            Children.Clear();

            if (page_is_in_view)
            {
                // The image layer
                this.Children.Add(ImagePage_HIDDEN);

                // Make the curly layer
                if (add_bells_and_whistles)
                {
                    Grid layer_curly = new Grid();

                    layer_curly.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    layer_curly.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
                    layer_curly.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    layer_curly.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    layer_curly.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(21, GridUnitType.Star) });
                    layer_curly.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    {
                        Image image_tl = new Image();
                        image_tl.Source = Icons.GetAppIcon(Icons.PageCurlTL);
                        image_tl.Stretch = Stretch.Uniform;
                        image_tl.HorizontalAlignment = HorizontalAlignment.Left;
                        image_tl.VerticalAlignment = VerticalAlignment.Top;
                        SetColumn(image_tl, 0);
                        SetRow(image_tl, 0);
                        layer_curly.Children.Add(image_tl);
                    }
                    {
                        Image image_tl = new Image();
                        image_tl.Source = Icons.GetAppIcon(Icons.PageCurlBR);
                        image_tl.Stretch = Stretch.Uniform;
                        image_tl.HorizontalAlignment = HorizontalAlignment.Right;
                        image_tl.VerticalAlignment = VerticalAlignment.Bottom;
                        SetColumn(image_tl, 2);
                        SetRow(image_tl, 2);
                        layer_curly.Children.Add(image_tl);
                    }

                    this.Children.Add(layer_curly);
                }

                // The functional layers
                foreach (PageLayer page_layer in page_layers)
                {
                    this.Children.Add(page_layer);
                }
            }
        }

        public void RotatePage()
        {
            RotateTransform rt = this.LayoutTransform as RotateTransform;
            if (null == rt)
            {
                rt = new RotateTransform(0);
                this.LayoutTransform = rt;
            }

            rt.Angle += 90;
        }

        public int PageNumber
        {
            get
            {
                return page;
            }
        }

        public void SelectPage()
        {
            foreach (PageLayer page_layer in page_layers)
            {
                page_layer.SelectPage();
            }
        }

        public void DeselectPage()
        {
            foreach (PageLayer page_layer in page_layers)
            {
                page_layer.DeselectPage();
            }
        }

        void PDFRendererPageControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            pdf_renderer_control.SelectedPage = this;
            e.Handled = true;

#if false
            // Save this image to disk
            ImageSaver.SaveAsPng(@"C:\temp\aaa.png", (BitmapImage)this.ImagePage_HIDDEN.Source);
            Logging.Debug("SAVED PAGE {0}", page);
#endif
        }

        void pdf_renderer_OnPageTextAvailable(int page_from, int page_to)
        {
            if (page_from <= this.page && page_to >= this.page || page == 0)
            {
                Dispatcher.BeginInvoke(new Action(OnPageTextAvailable_DISPATCHER), DispatcherPriority.Background);
            }
        }

        private void OnPageTextAvailable_DISPATCHER()
        {
            Logging.Info("Page text is available for page {0}", page);

            foreach (PageLayer page_layer in page_layers)
            {
                page_layer.PageTextAvailable();
            }
        }

        public bool PageIsInView
        {
            get
            {
                return page_is_in_view;
            }
        }

        #region Refresh Page

        internal void RefreshPage()
        {
            RefreshPage(null, 0);
        }

        private void RefreshPage_ResizedImageCallback(BitmapSource requested_image_rescale, double requested_height)
        {
            RefreshPage(requested_image_rescale, requested_height);
        }

        class PendingRefreshWork
        {
            public BitmapSource requested_image_rescale;
            public double requested_height;
        }

        object pending_refresh_work_lock = new object();
        bool pending_refresh_work_fast_running = false;
        PendingRefreshWork pending_refresh_work_fast = null;
        bool pending_refresh_work_slow_running = false;
        PendingRefreshWork pending_refresh_work_slow = null;

        /// <summary>
        /// Queues the page for refresh, holding onto the most recent recommended_pretty_image_rescale.
        /// Any previous queued refresh is ignored.  If another refresh is busy running, then the most recently received request is queued.
        /// </summary>
        /// <param name="requested_image_rescale">The suggested image to use, if null then will be requested asynchronously.</param>
        private void RefreshPage(BitmapSource requested_image_rescale, double requested_height)
        {
            PendingRefreshWork pending_refresh_work = new PendingRefreshWork { requested_image_rescale = requested_image_rescale, requested_height = requested_height };

            // cache the document fingerprint for the occasion where the RefreshPage_*() methods invoked/dispatched
            // below happen to encounter a Disposed()-just-now state of affairs for this Control instance, where
            // an exception may be thrown and *reported*: that's where we need this Fingerprint copy to prevent
            // a second failure:
            documentFingerprint = pdf_renderer_control_stats.pdf_document.Fingerprint;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pending_refresh_work_lock)
            {
                l1_clk.LockPerfTimerStop();
                pending_refresh_work_fast = pending_refresh_work;
                if (!pending_refresh_work_fast_running)
                {
                    pending_refresh_work_fast_running = true;
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => RefreshPage_INTERNAL_FAST()));
                }

                pending_refresh_work_slow = pending_refresh_work;
                if (!pending_refresh_work_slow_running)
                {
                    pending_refresh_work_slow_running = true;
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() => RefreshPage_INTERNAL_SLOW()));
                }
            }
        }

        private void RefreshPage_INTERNAL_FAST()
        {
            while (true)
            {
                // Get the next piece of work
                PendingRefreshWork pending_refresh_work = null;

                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pending_refresh_work_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    pending_refresh_work = pending_refresh_work_fast;
                    pending_refresh_work_fast = null;

                    // If there is nothing to do, then return
                    if (null == pending_refresh_work)
                    {
                        pending_refresh_work_fast_running = false;
                        return;
                    }
                }

                // Do the work
                try
                {
                    if (page_is_in_view)
                    {
                        double desired_rescaled_image_height = remembered_image_height * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI;
                        if (null != CurrentlyShowingImage && CurrentlyShowingImage.requested_height == desired_rescaled_image_height)
                        {
                            ImagePage_HIDDEN.Stretch = Stretch.None;
                        }
                        else
                        {
                            ImagePage_HIDDEN.Stretch = Stretch.Uniform;
                        }
                    }

                    //
                    // WARNING: we MAY be executing this bit of code while the control
                    // has just been Dispose()d! 
                    //
                    // When that happens, we're okay with FAILURE here...
                    Height = (int)(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                    Width = (int)(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem while trying to FAST render the page image for page {0} of document {1}", page, documentFingerprint);
                }
            }
        }


        private void RefreshPage_INTERNAL_SLOW()
        {
            while (true)
            {
                // Get the next piece of work
                PendingRefreshWork pending_refresh_work = null;

                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pending_refresh_work_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    pending_refresh_work = pending_refresh_work_slow;
                    pending_refresh_work_slow = null;

                    // If there is nothing to do, then return
                    if (null == pending_refresh_work)
                    {
                        pending_refresh_work_slow_running = false;
                        return;
                    }
                }

                // Do the work
                try
                {
                    // Invert colours if required
                    if (page_is_in_view)
                    {
                        InvertColours(pdf_renderer_control_stats.are_colours_inverted);
                    }

                    // Page is not in view, be nice to memory and clean up
                    //
                    // WARNING: we MAY be executing this bit of code while the control
                    // has just been Dispose()d! Hence the extra control_Stats check!
                    if (!page_is_in_view && null != pdf_renderer_control_stats)
                    {
                        CurrentlyShowingImage = null;
                        Height = (int)(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                        Width = (int)(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                        continue;
                    }

                    if (page_is_in_view)
                    {
                        // Work out the size of the image we would like to have
                        double desired_rescaled_image_height = remembered_image_height * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI;

                        // Is the current image not good enough?  Then perhaps use a provided one
                        if (null == CurrentlyShowingImage || CurrentlyShowingImage.requested_height != desired_rescaled_image_height)
                        {
                            Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                            lock (pending_refresh_work_lock)
                            {
                                l2_clk.LockPerfTimerStop();
                                // Check if we want to use the supplied image
                                if (null != pending_refresh_work.requested_image_rescale)
                                {
                                    // Choose the closer image
                                    double discrepancy_existing_image = (null == CurrentlyShowingImage) ? Double.MaxValue : Math.Abs(CurrentlyShowingImage.requested_height - desired_rescaled_image_height);
                                    double discrepancy_supplied_image = (null == pending_refresh_work.requested_image_rescale) ? Double.MaxValue : Math.Abs(pending_refresh_work.requested_height - desired_rescaled_image_height);

                                    // If the request image is better, use it
                                    if (discrepancy_supplied_image < discrepancy_existing_image)
                                    {
                                        CurrentlyShowingImage = new CurrentlyShowingImageClass { Image = pending_refresh_work.requested_image_rescale, requested_height = pending_refresh_work.requested_height };
                                    }
                                }
                            }
                        }

                        // If our current image is still not good enough, request one
                        if (null == CurrentlyShowingImage || CurrentlyShowingImage.requested_height != desired_rescaled_image_height)
                        {
                            pdf_renderer_control_stats.GetResizedPageImage(this, page, desired_rescaled_image_height, RefreshPage_ResizedImageCallback);
                        }

                        // Recalculate the aspect ratio
                        if (null != CurrentlyShowingImage)
                        {
                            if (CurrentlyShowingImage.requested_height == desired_rescaled_image_height)
                            {
                                ImagePage_HIDDEN.Stretch = Stretch.None;
                            }
                            else
                            {
                                ImagePage_HIDDEN.Stretch = Stretch.Uniform;
                            }

                            remembered_image_height = BASIC_PAGE_HEIGHT;
                            remembered_image_width = BASIC_PAGE_HEIGHT * CurrentlyShowingImage.Image.Width / CurrentlyShowingImage.Image.Height;
                            pdf_renderer_control_stats.largest_page_image_width = Math.Max(pdf_renderer_control_stats.largest_page_image_width, remembered_image_width);
                            pdf_renderer_control_stats.largest_page_image_height = Math.Max(pdf_renderer_control_stats.largest_page_image_height, remembered_image_height);

                            Height = (int)(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                            Width = (int)(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem while trying to SLOW render the page image for page {0} of document {1}", page, Logging.Error(ex, "There was a problem while trying to SLOW render the page image for page {0} of document {1}", page, documentFingerprint));
                }
            }
        }

        /// <summary>
        /// Sets whether the image colours are inverted.
        /// </summary>
        private void InvertColours(bool invert)
        {
            if (invert)
            {
                if (ImagePage_HIDDEN.Effect == null) ImagePage_HIDDEN.Effect = new NegativeEffect();
            }
            else
            {
                if (ImagePage_HIDDEN.Effect != null) ImagePage_HIDDEN.Effect = null;
            }
        }

        #endregion

        internal void SetOperationMode(PDFRendererControl.OperationMode operation_mode)
        {
            PageLayer selected_page_layer = null;

            switch (operation_mode)
            {
                case PDFRendererControl.OperationMode.Hand:
                    selected_page_layer = CanvasHand;
                    break;
                case PDFRendererControl.OperationMode.Annotation:
                    selected_page_layer = CanvasAnnotation;
                    break;
                case PDFRendererControl.OperationMode.Highlighter:
                    selected_page_layer = CanvasHighlight;
                    break;
                case PDFRendererControl.OperationMode.Camera:
                    selected_page_layer = CanvasCamera;
                    break;
                case PDFRendererControl.OperationMode.Ink:
                    selected_page_layer = CanvasInk;
                    break;
                case PDFRendererControl.OperationMode.TextSentenceSelect:
                    selected_page_layer = CanvasTextSentence;
                    break;
                default:
                    Logging.Warn("Unknown operation mode {0}", operation_mode);
                    break;
            }

            // Deselect all layers
            foreach (PageLayer page_layer in page_layers)
            {
                SetZIndex(page_layer, 0);
                page_layer.DeselectLayer();
            }

            // Make the new layer selected and the topmost
            foreach (PageLayer page_layer in page_layers)
            {
                if (selected_page_layer == page_layer)
                {
                    SetZIndex(page_layer, 1);
                    page_layer.SelectLayer();
                }
            }
        }

        internal void SetSearchKeywords(PDFSearchResultSet search_result_set)
        {
            CanvasSearch.SetSearchKeywords(search_result_set);
        }

        internal PDFSearchResult SetCurrentSearchPosition(PDFSearchResult previous_search_result_placeholder)
        {
            return CanvasSearch.SetCurrentSearchPosition(previous_search_result_placeholder);
        }

        internal PDFSearchResult SetNextSearchPosition(PDFSearchResult previous_search_result_placeholder)
        {
            return CanvasSearch.SetNextSearchPosition(previous_search_result_placeholder);
        }

        internal void SetPageNotInView()
        {
            // Do nothing if nothign has changed
            if (!page_is_in_view)
            {
                return;
            }

            page_is_in_view = false;

            ImagePage_HIDDEN = null;
            ReflectContentChildren();
            RefreshPage();
        }

        internal void SetPageInView()
        {
            // Do nothing if nothign has changed
            if (page_is_in_view)
            {
                return;
            }

            page_is_in_view = true;

            ReflectContentChildren();
            RefreshPage();
        }



        internal void RaiseInkChange(InkCanvasEditingMode inkCanvasEditingMode)
        {
            this.CanvasInk.RaiseInkChange(inkCanvasEditingMode);
        }

        internal void RaiseInkChange(DrawingAttributes drawingAttributes)
        {
            this.CanvasInk.RaiseInkChange(drawingAttributes);
        }

        internal void RaiseHighlightChange(int colourNumber)
        {
            this.CanvasHighlight.RaiseHighlightChange(colourNumber);
        }

        internal void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            this.CanvasTextSentence.RaiseTextSelectModeChange(textLayerSelectionMode);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFRendererPageControl()
        {
            Logging.Debug("~PDFRendererPageControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFRendererPageControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFRendererPageControl::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                pdf_renderer_control_stats.pdf_document.PDFRenderer.OnPageTextAvailable -= pdf_renderer_OnPageTextAvailable;

                foreach (PageLayer page_layer in page_layers)
                {
                    page_layer.Dispose();
                }
                page_layers.Clear();

                // Also erase any pending RefreshPage work:
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pending_refresh_work_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    pending_refresh_work_fast = null;
                    pending_refresh_work_slow = null;
                }
            }

            page_layers.Clear();

            CurrentlyShowingImage = null;
            ImagePage_HIDDEN = null;

            //pdf_renderer_control.Dispose();
            pdf_renderer_control = null;
            pdf_renderer_control_stats = null;

            CanvasTextSentence_ = null;
            CanvasSearch_ = null;
            CanvasAnnotation_ = null;
            CanvasHighlight_ = null;
            CanvasCamera_ = null;
            CanvasHand_ = null;
            CanvasInk_ = null;

            // Clear the references for sanity's sake
            this.DataContext = null;

            ++dispose_count;
        }

        #endregion

    }
}
