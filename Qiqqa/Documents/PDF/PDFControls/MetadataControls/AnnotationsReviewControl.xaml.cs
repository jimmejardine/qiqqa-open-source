using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.AnnotationsReportBuilding;
using Qiqqa.Common;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for AnnotationsReviewControl.xaml
    /// </summary>
    public partial class AnnotationsReviewControl : UserControl
    {
        private NonScrollingFlowDocumentScrollViewer ObjDocumentViewer;

        public AnnotationsReviewControl()
        {
            InitializeComponent();

            ObjDocumentViewer = new NonScrollingFlowDocumentScrollViewer();
            ObjDocumentViewer.ToolTip = "Any annotations or highlights that you have made in your document are shown here.\nYou can also copy these and paste them into your word processor or email editor.";
            ObjFlowDocumentHolder.Children.Add(ObjDocumentViewer);

            ObjTooManyAnnotationsButton.CaptionDock = Dock.Bottom;
            ObjTooManyAnnotationsButton.Caption = "There are too many annotations to show automatically.  Click here to show them.";
            ObjTooManyAnnotationsButton.Click += ObjTooManyAnnotationsButton_Click;

            ObjRefreshButton.CaptionDock = Dock.Bottom;
            ObjRefreshButton.Caption = "Refresh";
            ObjRefreshButton.Click += ObjRefreshButton_Click;

            ObjPopupButton.CaptionDock = Dock.Bottom;
            ObjPopupButton.Caption = "Popup";
            ObjPopupButton.Click += ObjPopupButton_Click;

            DataContextChanged += AnnotationsReviewControl_DataContextChanged;
        }

        private void ObjTooManyAnnotationsButton_Click(object sender, RoutedEventArgs e)
        {
            ObjTooManyAnnotationsButton.Visibility = Visibility.Collapsed;

            PDFDocument pdf_document = (PDFDocument)ObjTooManyAnnotationsButton.Tag;

            SafeThreadPool.QueueUserWorkItem(o =>
            {
                PopulateWithAnnotationReport(pdf_document);
            });
        }

        private void ObjPopupButton_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.GenerateAnnotationReport(pdf_document_bindable.Underlying.LibraryRef, new List<PDFDocument>() { pdf_document_bindable.Underlying });
            }
        }

        private void ObjRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Rebuild();
        }

        private void AnnotationsReviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Rebuild();
        }

        private void Rebuild()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            ObjDocumentViewer.Document = null;
            ObjTooManyAnnotationsButton.Visibility = Visibility.Collapsed;

            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                PDFDocument pdf_document = pdf_document_bindable.Underlying;

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    // TODO: [GHo] what are these 'heuristic' conditions good for?!?!
                    if (pdf_document.GetAnnotations().Count > 50 || pdf_document.Highlights.Count > 1000)
                    {
                        WPFDoEvents.InvokeAsyncInUIThread(() =>
                        {
                            ObjTooManyAnnotationsButton.Visibility = Visibility.Visible;
                            ObjTooManyAnnotationsButton.Tag = pdf_document;
                        });
                    }
                    else
                    {
                        PopulateWithAnnotationReport(pdf_document);
                    }
                });
            }
        }

        private void PopulateWithAnnotationReport(PDFDocument pdf_document)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            pdf_documents.Add(pdf_document);

            AnnotationReportOptions annotation_report_options = new AnnotationReportOptions();
            annotation_report_options.ObeySuppressedImages = true;
            annotation_report_options.ObeySuppressedText = true;
            annotation_report_options.SuppressAllImages = false;
            annotation_report_options.SuppressAllText = true;
            annotation_report_options.SuppressPDFDocumentHeader = true;
            annotation_report_options.SuppressPDFAnnotationTags = true;
            annotation_report_options.InitialRenderDelayMilliseconds = 1000;

            AsyncAnnotationReportBuilder.BuildReport(pdf_document.LibraryRef, pdf_documents, annotation_report_options, delegate (AsyncAnnotationReportBuilder.AnnotationReport annotation_report)
            {
                ObjDocumentViewer.Document = annotation_report.flow_document;
            });
        }
    }
}
