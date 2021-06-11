using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls.Page
{
    /// <summary>
    /// Interaction logic for PDFRendererPageControl.xaml
    /// </summary>
    public partial class PDFRendererPageControl : Grid, IDisposable
    {
        internal const int BASIC_PAGE_WIDTH = 850;
        internal const int BASIC_PAGE_HEIGHT = 1100;

        private WeakReference<PDFRendererControl> pdf_renderer_control = null;

        private int page = 0;
        private bool add_bells_and_whistles;
        private double remembered_image_width = BASIC_PAGE_WIDTH;
        private double remembered_image_height = BASIC_PAGE_HEIGHT;
        private bool page_is_in_view = false;
        private Image ImagePage_HIDDEN_;

        private Image ImagePage_HIDDEN
        {
            get
            {
                if (null == ImagePage_HIDDEN_)
                {
                    ImagePage_HIDDEN_ = new Image();
                    ImagePage_HIDDEN.Stretch = Stretch.Uniform;

                    // THIS MUST BE IN PLACE SO THAT WE HAVE PIXEL PERFECT RENDERING
                    ImagePage_HIDDEN.SnapsToDevicePixels = true;
                    //RenderOptions.SetBitmapScalingMode(ImagePage_HIDDEN, BitmapScalingMode.NearestNeighbor);
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

        private class CurrentlyShowingImageClass
        {
            public BitmapSource Image;
            public int requested_height;
            public int requested_width;
        }

        private CurrentlyShowingImageClass _currently_showing_image = null;

        private CurrentlyShowingImageClass CurrentlyShowingImage
        {
            get => _currently_showing_image;

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

        public double RememberedImageWidth => remembered_image_width;
        public double RememberedImageHeight => remembered_image_height;

        public int Page => page;

        private PDFTextSentenceLayer CanvasTextSentence_;
        private PDFSearchLayer CanvasSearch_;
        private PDFAnnotationLayer CanvasAnnotation_;
        private PDFHighlightLayer CanvasHighlight_;
        private PDFCameraLayer CanvasCamera_;
        private PDFHandLayer CanvasHand_;
        private PDFInkLayer CanvasInk_;
        private List<PageLayer> page_layers;

        // Provide a cached copy of the PDF document fingerprint for Exception report logging,
        // when this instance has otherwise already been Disposed():
        private string documentFingerprint;

        public PDFRendererPageControl(PDFRendererControl pdf_renderer_control, int page, bool add_bells_and_whistles)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            Theme.Initialize();

            InitializeComponent();

            this.page = page;
            this.pdf_renderer_control = new WeakReference<PDFRendererControl>(pdf_renderer_control);
            this.add_bells_and_whistles = add_bells_and_whistles;

            PDFRendererControlStats pdf_renderer_control_stats = pdf_renderer_control.GetPDFRendererControlStats();

            // cache the document fingerprint for the occasion where the RefreshPage_*() methods invoked/dispatched
            // below happen to encounter a Disposed()-just-now state of affairs for this Control instance, where
            // an exception may be thrown and *reported*: that's where we need this Fingerprint copy to prevent
            // a second failure:
            ASSERT.Test(pdf_renderer_control_stats.pdf_document != null);
            documentFingerprint = pdf_renderer_control_stats.pdf_document?.Fingerprint;

            // Start with a reasonable size
            Background = Brushes.White;
            Height = remembered_image_height * pdf_renderer_control_stats.zoom_factor;
            Width = remembered_image_width * pdf_renderer_control_stats.zoom_factor;

            page_layers = new List<PageLayer>();

            // Try to trap the DAMNED cursor keys escape route
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.None);

            SetOperationMode(PDFRendererControl.OperationMode.Hand);

            MouseDown += PDFRendererPageControl_MouseDown;

            pdf_renderer_control_stats.pdf_document.OnPageTextAvailable += pdf_renderer_OnPageTextAvailable;

            if (add_bells_and_whistles)
            {
                DropShadowEffect dse = new DropShadowEffect();
                dse.Color = ThemeColours.Background_Color_Blue_Dark;
                Effect = dse;
            }

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

                PopulateNeededLayers();
            });

            //Unloaded += PDFRendererPageControl_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private PDFRendererControl GetPDFRendererControl()
        {
            if (pdf_renderer_control != null && pdf_renderer_control.TryGetTarget(out var control) && control != null)
            {
                return control;
            }
            return null;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void PDFRendererPageControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        #region --- Page layer on-demand creation -------------------

        private PDFTextSentenceLayer CanvasTextSentence
        {
            get
            {
                if (null == CanvasTextSentence_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);

                    if (pdf_renderer_control != null)
                    {
                        CanvasTextSentence_ = new PDFTextSentenceLayer(pdf_renderer_control, page);
                        page_layers.Add(CanvasTextSentence_);
                        KeyboardNavigation.SetDirectionalNavigation(CanvasTextSentence_, KeyboardNavigationMode.None);
                        ReflectContentChildren();
                    }
                }

                return CanvasTextSentence_;
            }
        }

        private PDFSearchLayer CanvasSearch
        {
            get
            {
                if (null == CanvasSearch_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);

                    if (pdf_renderer_control != null)
                    {
                        CanvasSearch_ = new PDFSearchLayer(pdf_renderer_control, page);
                        page_layers.Add(CanvasSearch_);
                        ReflectContentChildren();
                    }
                }

                return CanvasSearch_;
            }
        }

        private PDFAnnotationLayer CanvasAnnotation
        {
            get
            {
                if (null == CanvasAnnotation_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    //
                    // Such an exception's stacktrace looks like this:
                    //
                    //```
                    //at System.Windows.Input.InputManager..ctor()
                    //at System.Windows.Input.InputManager.GetCurrentInputManagerImpl()
                    //at System.Windows.Input.KeyboardNavigation..ctor()
                    //at System.Windows.FrameworkElement.FrameworkServices..ctor()
                    //at System.Windows.FrameworkElement.EnsureFrameworkServices()
                    //at System.Windows.FrameworkElement..ctor()
                    //at System.Windows.Controls.Panel..ctor()
                    //at System.Windows.Controls.Canvas..ctor()
                    //at Qiqqa.Documents.PDF.PDFControls.Page.Tools.PageLayer..ctor() in W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\Qiqqa\Documents\PDF\PDFControls\Page\Tools\PageLayer.cs:line 10
                    //at Qiqqa.Documents.PDF.PDFControls.Page.Annotation.PDFAnnotationLayer..ctor(PDFRendererControlStats pdf_renderer_control_stats, Int32 page) in W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\Qiqqa\Documents\PDF\PDFControls\Page\Annotation\PDFAnnotationLayer.xaml.cs:line 25
                    //at Qiqqa.Documents.PDF.PDFControls.Page.PDFRendererPageControl.get_CanvasAnnotation() in W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\Qiqqa\Documents\PDF\PDFControls\Page\PDFRendererPageControl.xaml.cs:line 225
                    //at Qiqqa.Documents.PDF.PDFControls.Page.PDFRendererPageControl.PopulateNeededLayers() in W:\Projects\sites\library.visyond.gov\80\lib\tooling\qiqqa\Qiqqa\Documents\PDF\PDFControls\Page\PDFRendererPageControl.xaml.cs:line 330
                    //...
                    //```
                    //
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
                    ASSERT.Test(pdf_document != null);

                    if (pdf_document != null)
                    {
                        page_layers.Add(CanvasAnnotation_ = new PDFAnnotationLayer(pdf_document, page));
                        KeyboardNavigation.SetDirectionalNavigation(CanvasAnnotation_, KeyboardNavigationMode.None);
                        ReflectContentChildren();
                    }
                }

                return CanvasAnnotation_;
            }
        }

        private PDFHighlightLayer CanvasHighlight
        {
            get
            {
                if (null == CanvasHighlight_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
                    ASSERT.Test(pdf_document != null);

                    if (pdf_document != null)
                    {
                        page_layers.Add(CanvasHighlight_ = new PDFHighlightLayer(pdf_document, page));
                        ReflectContentChildren();
                    }
                }

                return CanvasHighlight_;
            }
        }

        private PDFCameraLayer CanvasCamera
        {
            get
            {
                if (null == CanvasCamera_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
                    ASSERT.Test(pdf_document != null);

                    if (pdf_document != null)
                    {
                        page_layers.Add(CanvasCamera_ = new PDFCameraLayer(pdf_document, page));
                        ReflectContentChildren();
                    }
                }

                return CanvasCamera_;
            }
        }

        private PDFHandLayer CanvasHand
        {
            get
            {
                if (null == CanvasHand_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);

                    if (pdf_renderer_control != null)
                    {
                        CanvasHand_ = new PDFHandLayer(pdf_renderer_control, page);
                        page_layers.Add(CanvasHand_);
                        ReflectContentChildren();
                    }
                }

                return CanvasHand_;
            }
        }

        private PDFInkLayer CanvasInk
        {
            get
            {
                if (null == CanvasInk_)
                {
                    // WARNING: this UI thread check is necessary as I had quite a bit of trouble with this. Turns out
                    // code internal to PDFAnnotationLayer constructor uses classes which are derived off system classes
                    // (PageLayer, ...) which internally perform VerifyAccess() calls (the exception-throwing twin of
                    // CheckAccess()) and thus fail with a dramatic STA-only exception when the *constructor* wasn't invoked
                    // from the STA = UI thread to begin with!
                    WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
                    ASSERT.Test(pdf_document != null);

                    if (pdf_document != null)
                    {
                        page_layers.Add(CanvasInk_ = new PDFInkLayer(pdf_document, page));
                        ReflectContentChildren();
                    }
                }

                return CanvasInk_;
            }
        }

        #endregion

        private void PopulateNeededLayers()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
            ASSERT.Test(pdf_renderer_control != null);
            PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            if (pdf_document != null)
            {
                bool need_annots = PDFAnnotationLayer.IsLayerNeeded(pdf_document, page);
                bool need_inks = PDFInkLayer.IsLayerNeeded(pdf_document, page);
                bool need_highlights = PDFHighlightLayer.IsLayerNeeded(pdf_document, page);

                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    Stopwatch clk = Stopwatch.StartNew();
                    Logging.Info("+PopulateNeededLayers for document {0}", pdf_document.Fingerprint);

                    try
                    {
                        if (need_annots)
                        {
                            _ = CanvasAnnotation;
                        }
                        if (need_inks)
                        {
                            _ = CanvasInk;
                        }
                        if (need_highlights)
                        {
                            _ = CanvasHighlight;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "PopulateNeededLayers: Error occurred while fetching annotations, inks and highlights for document {0}", pdf_document.Fingerprint);
                    }

                    Logging.Info("-PopulateNeededLayers for document {1} (time spent: {0} ms)", clk.ElapsedMilliseconds, pdf_document.Fingerprint);
                });
            }
        }

        private void ReflectContentChildren()
        {
            Children.Clear();

            if (page_is_in_view)
            {
                // The image layer
                Children.Add(ImagePage_HIDDEN);

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

                    Children.Add(layer_curly);
                }

                // The functional layers
                foreach (PageLayer page_layer in page_layers)
                {
                    Children.Add(page_layer);
                }
            }
        }

        public void RotatePage()
        {
            RotateTransform rt = LayoutTransform as RotateTransform;
            if (null == rt)
            {
                rt = new RotateTransform(0);
                LayoutTransform = rt;
            }

            rt.Angle += 90;
        }

        public int PageNumber => page;

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

        private void PDFRendererPageControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            Keyboard.Focus(this);

            PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
            ASSERT.Test(pdf_renderer_control != null);

            if (pdf_renderer_control != null)
            {
                pdf_renderer_control.SelectedPage = this;
            }

            e.Handled = true;

#if false
            // Save this image to disk
            ImageSaver.SaveAsPng(@"C:\temp\aaa.png", (BitmapImage)this.ImagePage_HIDDEN.Source);
            Logging.Debug("SAVED PAGE {0}", page);
#endif
        }

        private void pdf_renderer_OnPageTextAvailable(int page_from, int page_to)
        {
            WPFDoEvents.SafeExec(() =>
            {
                if (page_from <= page && page_to >= page || page == 0)
                {
                    WPFDoEvents.InvokeAsyncInUIThread(OnPageTextAvailable_DISPATCHER, DispatcherPriority.Background);
                }
            });
        }

        private void OnPageTextAvailable_DISPATCHER()
        {
            Logging.Info("Page text is available for page {0}", page);

            foreach (PageLayer page_layer in page_layers)
            {
                page_layer.PageTextAvailable();
            }
        }

        public bool PageIsInView => page_is_in_view;

        #region Refresh Page

        internal void RefreshPage()
        {
            RefreshPage(null, 0, 0);
        }

        private void RefreshPage_ResizedImageCallback(BitmapSource requested_image_rescale, int requested_height, int requested_width)
        {
            RefreshPage(requested_image_rescale, requested_height, requested_width);
        }

        private class PendingRefreshWork
        {
            public BitmapSource requested_image_rescale;
            public int requested_height;
            public int requested_width;
        }

        private object pending_refresh_work_lock = new object();
        private bool pending_refresh_work_fast_running = false;
        private PendingRefreshWork pending_refresh_work_fast = null;
        private bool pending_refresh_work_slow_running = false;
        private PendingRefreshWork pending_refresh_work_slow = null;

        /// <summary>
        /// Queues the page for refresh, holding onto the most recent recommended_pretty_image_rescale.
        /// Any previous queued refresh is ignored.  If another refresh is busy running, then the most recently received request is queued.
        /// </summary>
        /// <param name="requested_image_rescale">The suggested image to use, if null then will be requested asynchronously.</param>
        private void RefreshPage(BitmapSource requested_image_rescale, int requested_height, int requested_width)
        {
            PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
            ASSERT.Test(pdf_renderer_control != null);
            PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            // prevent crashes by PDF page renders which call back late (when control has already expired)
            if (pdf_document == null)
                return;

            PendingRefreshWork pending_refresh_work = new PendingRefreshWork { requested_image_rescale = requested_image_rescale, requested_height = requested_height, requested_width = requested_width };

            bool call_fast = false;
            bool call_slow = false;

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pending_refresh_work_lock)
            {
                // l1_clk.LockPerfTimerStop();
                pending_refresh_work_fast = pending_refresh_work;
                if (!pending_refresh_work_fast_running)
                {
                    pending_refresh_work_fast_running = true;
                    call_fast = true;
                }

                pending_refresh_work_slow = pending_refresh_work;
                if (!pending_refresh_work_slow_running)
                {
                    pending_refresh_work_slow_running = true;
                    call_slow = true;
                }
            }

            if (call_fast)
            {
                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    RefreshPage_INTERNAL_FAST();
                });
            }
            else if (call_slow)
            {
                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    RefreshPage_INTERNAL_SLOW();
                }, DispatcherPriority.Background);
            }
        }

        private void RefreshPage_INTERNAL_FAST()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            while (true)
            {
                // Get the next piece of work
                PendingRefreshWork pending_refresh_work = null;

                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pending_refresh_work_lock)
                {
                    // l1_clk.LockPerfTimerStop();
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
                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFRendererControlStats pdf_renderer_control_stats = pdf_renderer_control?.GetPDFRendererControlStats();
                    ASSERT.Test(pdf_renderer_control_stats != null);

                    if (pdf_renderer_control_stats != null)
                    {
                        if (page_is_in_view)
                        {
                            int desired_rescaled_image_height = (int)Math.Round(remembered_image_height * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI);
                            int desired_rescaled_image_width = (int)Math.Round(remembered_image_width * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI);
                            if (null != CurrentlyShowingImage && (CurrentlyShowingImage.requested_height == desired_rescaled_image_height || CurrentlyShowingImage.requested_width == desired_rescaled_image_width))
                            {
                                ImagePage_HIDDEN.Stretch = Stretch.Uniform;  // TODO: WTF? With this hack (see previous commit for old value) the PDF render scales correctly on screen, finally)
                                //ImagePage_HIDDEN.Stretch = Stretch.None;
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
                        Height = (int)Math.Round(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                        Width = (int)Math.Round(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem while trying to FAST render the page image for page {0} of document {1}", page, documentFingerprint);
                }
            }
        }


        private void RefreshPage_INTERNAL_SLOW()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            while (true)
            {
                // Get the next piece of work
                PendingRefreshWork pending_refresh_work = null;

                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pending_refresh_work_lock)
                {
                    //l1_clk.LockPerfTimerStop();
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
                    PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                    ASSERT.Test(pdf_renderer_control != null);
                    PDFRendererControlStats pdf_renderer_control_stats = pdf_renderer_control?.GetPDFRendererControlStats();
                    ASSERT.Test(pdf_renderer_control_stats != null);

                    if (pdf_renderer_control_stats != null)
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
                            Height = (int)Math.Round(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                            Width = (int)Math.Round(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                            continue;
                        }

                        if (page_is_in_view)
                        {
                            // Work out the size of the image we would like to have
                            int desired_rescaled_image_height = (int)Math.Round(remembered_image_height * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI);
                            int desired_rescaled_image_width = (int)Math.Round(remembered_image_width * pdf_renderer_control_stats.zoom_factor * pdf_renderer_control_stats.DPI);

                            // Is the current image not good enough?  Then perhaps use a provided one
                            if (null == CurrentlyShowingImage || (CurrentlyShowingImage.requested_height != desired_rescaled_image_height && CurrentlyShowingImage.requested_width != desired_rescaled_image_width))
                            {
                                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                                lock (pending_refresh_work_lock)
                                {
                                    // l2_clk.LockPerfTimerStop();
                                    // Check if we want to use the supplied image
                                    if (null != pending_refresh_work.requested_image_rescale)
                                    {
                                        // Choose the closer image
                                        double discrepancy_existing_image = (null == CurrentlyShowingImage) ? Double.MaxValue : Math.Min(Math.Abs(CurrentlyShowingImage.requested_height - desired_rescaled_image_height), Math.Abs(CurrentlyShowingImage.requested_width - desired_rescaled_image_width));
                                        double discrepancy_supplied_image = (null == pending_refresh_work.requested_image_rescale) ? Double.MaxValue : Math.Min(Math.Abs(pending_refresh_work.requested_height - desired_rescaled_image_height), Math.Abs(pending_refresh_work.requested_width - desired_rescaled_image_width));

                                        // If the request image is better, use it
                                        if (discrepancy_supplied_image < discrepancy_existing_image)
                                        {
                                            CurrentlyShowingImage = new CurrentlyShowingImageClass
                                            {
                                                Image = pending_refresh_work.requested_image_rescale,
                                                requested_height = pending_refresh_work.requested_height,
                                                requested_width = pending_refresh_work.requested_width
                                            };
                                        }
                                    }
                                }
                            }

                            // If our current image is still not good enough, request one
                            if (null == CurrentlyShowingImage || (CurrentlyShowingImage.requested_height != desired_rescaled_image_height && CurrentlyShowingImage.requested_width != desired_rescaled_image_width))
                            {
                                pdf_renderer_control_stats.GetResizedPageImage(this, page, desired_rescaled_image_height, desired_rescaled_image_width, RefreshPage_ResizedImageCallback);
                            }

                            // Recalculate the aspect ratio
                            if (null != CurrentlyShowingImage)
                            {
                                if (CurrentlyShowingImage.requested_height == desired_rescaled_image_height || CurrentlyShowingImage.requested_width == desired_rescaled_image_width)
                                {
                                    ImagePage_HIDDEN.Stretch = Stretch.Uniform;  // TODO: WTF? With this hack (see previous commit for old value) the PDF render scales correctly on screen, finally)
                                }
                                else
                                {
                                    ImagePage_HIDDEN.Stretch = Stretch.Uniform;
                                }

                                remembered_image_height = BASIC_PAGE_HEIGHT;
                                remembered_image_width = BASIC_PAGE_HEIGHT * CurrentlyShowingImage.Image.Width / CurrentlyShowingImage.Image.Height;
                                pdf_renderer_control_stats.largest_page_image_width = Math.Max(pdf_renderer_control_stats.largest_page_image_width, remembered_image_width);
                                pdf_renderer_control_stats.largest_page_image_height = Math.Max(pdf_renderer_control_stats.largest_page_image_height, remembered_image_height);

                                Height = (int)Math.Round(remembered_image_height * pdf_renderer_control_stats.zoom_factor);
                                Width = (int)Math.Round(remembered_image_width * pdf_renderer_control_stats.zoom_factor);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem while trying to SLOW render the page image for page {0} of document {1}", page, documentFingerprint);
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
            // Do nothing if nothing has changed
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
            // Do nothing if nothing has changed
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
            CanvasInk.RaiseInkChange(inkCanvasEditingMode);
        }

        internal void RaiseInkChange(DrawingAttributes drawingAttributes)
        {
            CanvasInk.RaiseInkChange(drawingAttributes);
        }

        internal void RaiseHighlightChange(int colourNumber)
        {
            CanvasHighlight.RaiseHighlightChange(colourNumber);
        }

        internal void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            CanvasTextSentence.RaiseTextSelectModeChange(textLayerSelectionMode);
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

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        PDFRendererControl pdf_renderer_control = GetPDFRendererControl();
                        ASSERT.Test(pdf_renderer_control != null);
                        PDFDocument pdf_document = pdf_renderer_control?.GetPDFDocument();
                        ASSERT.Test(pdf_document != null);

                        if (pdf_document != null)
                        {
                            pdf_document.OnPageTextAvailable -= pdf_renderer_OnPageTextAvailable;
                        }
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        foreach (PageLayer page_layer in page_layers)
                        {
                            page_layer.Dispose();
                        }
                        page_layers.Clear();

                                // Also erase any pending RefreshPage work:
                                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                                lock (pending_refresh_work_lock)
                        {
                                    // l1_clk.LockPerfTimerStop();
                                    pending_refresh_work_fast = null;
                            pending_refresh_work_slow = null;
                        }

#if false               // These Dispose() calls have already been done above in the page_layers.Dispose() loop!
                            CanvasTextSentence_.Dispose();
                            CanvasSearch_.Dispose();
                            CanvasAnnotation_.Dispose();
                            CanvasHighlight_.Dispose();
                            CanvasCamera_.Dispose();
                            CanvasHand_.Dispose();
                            CanvasInk_.Dispose();
#endif
                            }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    page_layers = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    CurrentlyShowingImage = null;
                    ImagePage_HIDDEN = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    pdf_renderer_control = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    CanvasTextSentence_ = null;
                    CanvasSearch_ = null;
                    CanvasAnnotation_ = null;
                    CanvasHighlight_ = null;
                    CanvasCamera_ = null;
                    CanvasHand_ = null;
                    CanvasInk_ = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    // Clear the references for sanity's sake
                    DataContext = null;
                });

                ++dispose_count;
            });
        }

        #endregion

    }
}
