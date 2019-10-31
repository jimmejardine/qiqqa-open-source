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
        PDFRendererControlStats pdf_renderer_control_stats;
        int page;

        public PDFInkLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);

            this.SizeChanged += PDFInkLayer_SizeChanged;

            ObjInkCanvas.StrokeCollected += ObjInkCanvas_StrokeCollected;
            ObjInkCanvas.StrokeErased += ObjInkCanvas_StrokeErased;
            ObjInkCanvas.SelectionMoved += ObjInkCanvas_SelectionMoved;
            ObjInkCanvas.SelectionResized += ObjInkCanvas_SelectionResized;

            ObjInkCanvas.RequestBringIntoView += ObjInkCanvas_RequestBringIntoView;

            RebuildInks(page, pdf_renderer_control_stats.pdf_document.Inks);

            RaiseInkChange(InkCanvasEditingMode.Ink);
        }

        public static bool IsLayerNeeded(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            StrokeCollection stroke_collection = pdf_renderer_control_stats.pdf_document.Inks.GetInkStrokeCollection(page);
            return (null != stroke_collection);
        }

        void ObjInkCanvas_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // Don't automatically bring into view when clicked - it draws a long ugly pen line...
            e.Handled = true;
        }

        private void RebuildInks(int page, PDFInkList pdf_ink_list)
        {
            StrokeCollection stroke_collection = pdf_ink_list.GetInkStrokeCollection(page);
            if (null != stroke_collection)
            {
                this.ObjInkCanvas.Strokes = stroke_collection;
            }
        }

        bool HaveStrokes()
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

        void ObjInkCanvas_SelectionResized(object sender, EventArgs e)
        {
            InkChanged();
        }

        void ObjInkCanvas_SelectionMoved(object sender, EventArgs e)
        {
            InkChanged();
        }

        void ObjInkCanvas_StrokeErased(object sender, RoutedEventArgs e)
        {
            InkChanged();
        }

        void ObjInkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            InkChanged();
        }

        void InkChanged()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_InkChanged);

            using (MemoryStream ms = new MemoryStream())
            {
                ObjInkCanvas.Strokes.Save(ms, true);
                byte[] ink_blob = ms.ToArray();
                pdf_renderer_control_stats.pdf_document.Inks.AddPageInkBlob(page, ink_blob);
            }
        }

        void PDFInkLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ObjBaseGrid.Width = this.ActualWidth;
            ObjBaseGrid.Height = this.ActualHeight;
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

            if (0 == dispose_count)
            {
                WPFDoEvents.InvokeInUIThread(() =>
                {
                    ObjInkCanvas.StrokeCollected -= ObjInkCanvas_StrokeCollected;
                    ObjInkCanvas.StrokeErased -= ObjInkCanvas_StrokeErased;
                    ObjInkCanvas.SelectionMoved -= ObjInkCanvas_SelectionMoved;
                    ObjInkCanvas.SelectionResized -= ObjInkCanvas_SelectionResized;

                    ObjInkCanvas.RequestBringIntoView -= ObjInkCanvas_RequestBringIntoView;

                    try
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
                    catch (Exception ex)
                    {
                        Logging.Error(ex);
                    }

                    try
                    {
                        Children.Clear();
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex);
                    }
                }, this.Dispatcher);
            }

            // Clear the references for sanity's sake
            pdf_renderer_control_stats = null;

            this.DataContext = null;

            ++dispose_count;

            //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
        }

        #endregion

    }
}
