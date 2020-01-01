using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.GUI.Wizard;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Annotation
{
    /// <summary>
    /// Interaction logic for PDFAnnotationLayer.xaml
    /// </summary>
    public partial class PDFAnnotationLayer : PageLayer, IDisposable
    {
        private PDFRendererControlStats pdf_renderer_control_stats;
        private int page;
        private DragAreaTracker drag_area_tracker = null;

        public PDFAnnotationLayer(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.page = page;

            InitializeComponent();

            // Wizard
            if (1 == page)
            {
                WizardDPs.SetPointOfInterest(this, "PDFReadingAnnotationLayer");
            }

            Background = Brushes.Transparent;
            Cursor = Cursors.Cross;

            SizeChanged += PDFAnnotationLayer_SizeChanged;

            drag_area_tracker = new DragAreaTracker(this);
            drag_area_tracker.OnDragComplete += drag_area_tracker_OnDragComplete;

            // Add all the already existing annotations
            foreach (PDFAnnotation pdf_annotation in pdf_renderer_control_stats.pdf_document.GetAnnotations())
            {
                if (pdf_annotation.Page == this.page)
                {
                    if (!pdf_annotation.Deleted)
                    {
                        Logging.Info("Loading annotation on page {0}", page);
                        PDFAnnotationItem pdf_annotation_item = new PDFAnnotationItem(this, pdf_annotation, pdf_renderer_control_stats);
                        pdf_annotation_item.ResizeToPage(ActualWidth, ActualHeight);
                        Children.Add(pdf_annotation_item);
                    }
                    else
                    {
                        Logging.Info("Not loading deleted annotation on page {0}", page);
                    }
                }
            }
        }

        public static bool IsLayerNeeded(PDFRendererControlStats pdf_renderer_control_stats, int page)
        {
            foreach (PDFAnnotation pdf_annotation in pdf_renderer_control_stats.pdf_document.GetAnnotations())
            {
                if (pdf_annotation.Page == page)
                {
                    return true;
                }
            }

            return false;
        }

        private void drag_area_tracker_OnDragComplete(bool button_left_pressed, bool button_right_pressed, Point mouse_down_point, Point mouse_up_point)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_AddAnnotation);

            int MINIMUM_DRAG_SIZE_TO_CREATE_ANNOTATION = 20;
            if (Math.Abs(mouse_up_point.X - mouse_down_point.X) < MINIMUM_DRAG_SIZE_TO_CREATE_ANNOTATION ||
                Math.Abs(mouse_up_point.Y - mouse_down_point.Y) < MINIMUM_DRAG_SIZE_TO_CREATE_ANNOTATION)
            {
                Logging.Info("Drag area too small to create annotation");
                return;
            }

            PDFAnnotation pdf_annotation = new PDFAnnotation(pdf_renderer_control_stats.pdf_document.PDFRenderer.DocumentFingerprint, page, PDFAnnotationEditorControl.LastAnnotationColor, ConfigurationManager.Instance.ConfigurationRecord.Account_Nickname);
            pdf_annotation.Left = Math.Min(mouse_up_point.X, mouse_down_point.X) / ActualWidth;
            pdf_annotation.Top = Math.Min(mouse_up_point.Y, mouse_down_point.Y) / ActualHeight;
            pdf_annotation.Width = Math.Abs(mouse_up_point.X - mouse_down_point.X) / ActualWidth;
            pdf_annotation.Height = Math.Abs(mouse_up_point.Y - mouse_down_point.Y) / ActualHeight;

            pdf_renderer_control_stats.pdf_document.GetAnnotations().AddUpdatedAnnotation(pdf_annotation);

            PDFAnnotationItem pdf_annotation_item = new PDFAnnotationItem(this, pdf_annotation, pdf_renderer_control_stats);
            pdf_annotation_item.ResizeToPage(ActualWidth, ActualHeight);
            Children.Add(pdf_annotation_item);
        }

        private void PDFAnnotationLayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (PDFAnnotationItem pdf_annotation_item in Children.OfType<PDFAnnotationItem>())
            {
                pdf_annotation_item.ResizeToPage(ActualWidth, ActualHeight);
            }
        }

        internal override void SelectPage()
        {
        }

        internal override void DeselectPage()
        {
        }

        internal override void PageTextAvailable()
        {
        }

        internal void DeletePDFAnnotationItem(PDFAnnotationItem pdf_annotation_item)
        {
            Children.Remove(pdf_annotation_item);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFAnnotationLayer()
        {
            Logging.Debug("~PDFAnnotationLayer()");
            Dispose(false);
        }

        public override void Dispose()
        {
            Logging.Debug("Disposing PDFAnnotationLayer");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFAnnotationLayer::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (null != drag_area_tracker)
                {
                    WPFDoEvents.InvokeInUIThread(() =>
                    {
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

                        WizardDPs.ClearPointOfInterest(this);

                        drag_area_tracker.OnDragComplete -= drag_area_tracker_OnDragComplete;
                    });
                }

                // Clear the references for sanity's sake
                pdf_renderer_control_stats = null;
                drag_area_tracker = null;

                DataContext = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;

            //base.Dispose(disposing);     // parent only throws an exception (intentionally), so depart from best practices and don't call base.Dispose(bool)
        }

        #endregion

    }
}
