using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using icons;
using Qiqqa.UtilisationTracking;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Utilities;
using Utilities.Files;
using Utilities.GUI.Wizard;

namespace Qiqqa.AnnotationsReportBuilding
{
    /// <summary>
    /// Interaction logic for ReportViewerControl.xaml
    /// </summary>
    public partial class ReportViewerControl : UserControl, IDisposable
    {
        private AsyncAnnotationReportBuilder.AnnotationReport annotation_report;

        public ReportViewerControl(AsyncAnnotationReportBuilder.AnnotationReport annotation_report)
        {
            InitializeComponent();

            WizardDPs.SetPointOfInterest(this, "LibraryAnnotationReportViewer");

            ButtonPrint.Icon = Icons.GetAppIcon(Icons.Printer);
            ButtonPrint.ToolTip = "Print this report";
            ButtonPrint.Click += ButtonPrint_Click;

            ButtonToWord.Icon = Icons.GetAppIcon(Icons.AnnotationReportExportToWord);
            ButtonToWord.ToolTip = "Export to Word";
            ButtonToWord.Click += ButtonToWord_Click;

            ButtonToPDF.Icon = Icons.GetAppIcon(Icons.AnnotationReportExportToPDF);
            ButtonToPDF.ToolTip = "Export to PDF";
            ButtonToPDF.Click += ButtonToPDF_Click;

#if INCLUDE_UNUSED
            ButtonCollapseClickOptions.Caption = "Collapse";
            ButtonCollapseClickOptions.Click += ButtonCollapseClickOptions_Click;
            ButtonExpandClickOptions.Caption = "Expand";
            ButtonExpandClickOptions.Click += ButtonExpandClickOptions_Click;
#endif

            this.annotation_report = annotation_report;
            this.ObjDocumentViewer.Document = annotation_report.flow_document;
        }

        // Warning CA1811	'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
        // Warning CA1811	'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
#if INCLUDE_UNUSED
        void ButtonExpandClickOptions_Click(object sender, RoutedEventArgs e)
        {
            annotation_report.ExpandClickOptions();
        }

        void ButtonCollapseClickOptions_Click(object sender, RoutedEventArgs e)
        {
            annotation_report.CollapseClickOptions();
        }
#endif

        ~ReportViewerControl()
        {
            Logging.Info("~ReportViewerControl()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing ReportViewerControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                this.ObjDocumentViewer.Document.Blocks.Clear();
                this.ObjDocumentViewer.Document = null;

                this.annotation_report = null;
            }

            // Get rid of unmanaged resources 
        }

        string SaveToRTF()
        {
            FlowDocument flow_document = ObjDocumentViewer.Document;
            TextRange text_range = new TextRange(flow_document.ContentStart, flow_document.ContentEnd);

            string filename = TempFile.GenerateTempFilename("rtf");
            using (FileStream fs = File.OpenWrite(filename))
            {
                text_range.Save(fs, DataFormats.Rtf);
            }

            return filename;
        }
        
        void ButtonToWord_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.AnnotationReport_ToWord);

            annotation_report.CollapseClickOptions();
            string filename = SaveToRTF();
            annotation_report.ExpandClickOptions();
            Process.Start(filename);
        }

        void ButtonToPDF_Click(object sender, RoutedEventArgs e)
        {
            PdfDocument doc = null;
            string filename_pdf = TempFile.GenerateTempFilename("pdf");
                                  
            try
            {
                doc = new PdfDocument();

                PdfPage page = doc.Pages.Add();
                SizeF bounds = page.GetClientSize();

                string filename_rtf = SaveToRTF();
                string text = File.ReadAllText(filename_rtf);

                PdfMetafile metafile = (PdfMetafile)PdfImage.FromRtf(text, bounds.Width, PdfImageType.Metafile);
                PdfMetafileLayoutFormat format = new PdfMetafileLayoutFormat();

                // Allow the text to flow multiple pages without any breaks.
                format.SplitTextLines = true;
                format.SplitImages = true;

                // Draw the image.
                metafile.Draw(page, 0, 0, format);

                doc.Save(filename_pdf);
            }
            finally
            {
                // Warning CA2000  call System.IDisposable.Dispose on object 'doc' before all references to it are out of scope.
                doc?.Dispose();
            }

            Process.Start(filename_pdf);
        }

        void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.AnnotationReport_Print);
            annotation_report.CollapseClickOptions();
            ObjDocumentViewer.Print();
            annotation_report.ExpandClickOptions();
        }
    }
}
