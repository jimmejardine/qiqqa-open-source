using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Animation;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Ink
{
    /// <summary>
    /// Interaction logic for InkLayer.xaml
    /// </summary>
    public partial class PDFInkLayer : PageLayer, IDisposable
    {
        private PDFDocument pdf_document;
        private int page;

        public PDFInkLayer(PDFDocument pdf_document, int page)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            this.pdf_document = pdf_document;
            this.page = page;

            InitializeComponent();

            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);

            SizeChanged += PDFInkLayer_SizeChanged;

            ObjInkCanvas.StrokeCollected += ObjInkCanvas_StrokeCollected;
            ObjInkCanvas.StrokeErased += ObjInkCanvas_StrokeErased;
            ObjInkCanvas.SelectionMoved += ObjInkCanvas_SelectionMoved;
            ObjInkCanvas.SelectionResized += ObjInkCanvas_SelectionResized;

            ObjInkCanvas.RequestBringIntoView += ObjInkCanvas_RequestBringIntoView;

            RebuildInks(page, pdf_document.Inks);

            RaiseInkChange(InkCanvasEditingMode.Ink);

            //Unloaded += PDFInkLayer_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void PDFInkLayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        public static bool IsLayerNeeded(PDFDocument pdf_document, int page)
        {
            StrokeCollection stroke_collection = pdf_document.Inks.GetInkStrokeCollection(page);
            return (null != stroke_collection);
        }

        private void ObjInkCanvas_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // Don't automatically bring into view when clicked - it draws a long ugly pen line...
            e.Handled = true;
        }

        private void RebuildInks(int page, PDFInkList pdf_ink_list)
        {
            StrokeCollection stroke_collection = pdf_ink_list.GetInkStrokeCollection(page);
            if (null != stroke_collection)
            {
                ObjInkCanvas.Strokes = stroke_collection;
            }
        }

        private bool HaveStrokes()
        {
            return ObjInkCanvas.Strokes.Count > 0;
        }

        internal override void SelectLayer()
        {
            if (HaveStrokes())
            {
                Animations.Fade(this, ConfigurationManager.Instance.ConfigurationRecord.GUI_InkScreenTransparency, 1);
            }
        }
        internal override void DeselectLayer()
        {
            if (HaveStrokes())
            {
                Animations.Fade(this, 1, ConfigurationManager.Instance.ConfigurationRecord.GUI_InkScreenTransparency);
            }
        }

        private void ObjInkCanvas_SelectionResized(object sender, EventArgs e)
        {
            InkChanged();
        }

        private void ObjInkCanvas_SelectionMoved(object sender, EventArgs e)
        {
            InkChanged();
        }

        private void ObjInkCanvas_StrokeErased(object sender, RoutedEventArgs e)
        {
            InkChanged();
        }

        private void ObjInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            InkChanged();
        }

        private void InkChanged()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_InkChanged);

            using (MemoryStream ms = new MemoryStream())
            {
                ObjInkCanvas.Strokes.Save(ms, true);
                byte[] ink_blob = ms.ToArray();
                pdf_document.AddPageInkBlob(page, ink_blob);
            }
        }

        private void PDFInkLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ObjBaseGrid.Width = ActualWidth;
            ObjBaseGrid.Height = ActualHeight;
        }

        internal void RaiseInkChange(InkCanvasEditingMode inkCanvasEditingMode)
        {
            ObjInkCanvas.EditingMode = inkCanvasEditingMode;

            if (inkCanvasEditingMode == InkCanvasEditingMode.Ink)
            {
                ObjInkCanvas.Cursor = Cursors.Pen;
                ObjInkCanvas.UseCustomCursor = true;
            }
            else
            {
                ObjInkCanvas.UseCustomCursor = false;
            }
        }

        internal void RaiseInkChange(DrawingAttributes drawingAttributes)
        {
            ObjInkCanvas.DefaultDrawingAttributes = drawingAttributes;
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFInkLayer()
        {
            Logging.Debug("~PDFInkLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFInkLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFInkLayer::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (0 == dispose_count)
                    {
                        if (null != ObjInkCanvas)
                        {
                            ObjInkCanvas.StrokeCollected -= ObjInkCanvas_StrokeCollected;
                            ObjInkCanvas.StrokeErased -= ObjInkCanvas_StrokeErased;
                            ObjInkCanvas.SelectionMoved -= ObjInkCanvas_SelectionMoved;
                            ObjInkCanvas.SelectionResized -= ObjInkCanvas_SelectionResized;

                            ObjInkCanvas.RequestBringIntoView -= ObjInkCanvas_RequestBringIntoView;
                        }
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
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
                    // Clear the references for sanity's sake
                    pdf_document = null;

                    Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                ++dispose_count;

                //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
            });
        }

        #endregion

    }
}
