using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls;
using Utilities;
using Utilities.GUI;
using Utilities.Mathematics.Topics.LDAStuff;
using Utilities.Reflection;

namespace Qiqqa.Expedition
{
    /// <summary>
    /// Interaction logic for TopicOverviewControl.xaml
    /// </summary>
    public partial class DocumentOverviewControl : UserControl
    {
        public class TopicOverviewData
        {
            public int topic;

            public Library library;
            public ExpeditionDataSource eds;

            public LDASampler lda_sampler;
            public LDAAnalysis lda_analysis;
        }

        public DocumentOverviewControl()
        {
            InitializeComponent();

            //Unloaded += DocumentOverviewControl_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            DataContextChanged += TopicOverviewControl_DataContextChanged;

            TxtTitle.Cursor = Cursors.Hand;
            TxtTitle.MouseLeftButtonUp += TxtTitle_MouseLeftButtonUp;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            CleanUp();
        }

        // WARNING: https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.unloaded?view=net-5.0
        // Which says:
        //
        // Note that the Unloaded event is not raised after an application begins shutting down. 
        // Application shutdown occurs when the condition defined by the ShutdownMode property occurs. 
        // If you place cleanup code within a handler for the Unloaded event, such as for a Window 
        // or a UserControl, it may not be called as expected.
        private void DocumentOverviewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }

        private void CleanUp()
        {
            // TODO: discard the pdf renderer
            DataContext = null;

            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
        }

        private void TxtTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                PDFDocument pdf_document = pdf_document_bindable.Underlying;
                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
            }

            e.Handled = true;
        }

        private void TopicOverviewControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Clear the old
            ObjPDFRendererControlPlaceholder.Children.Clear();

            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            PDFDocument pdf_document = pdf_document_bindable.Underlying;

            if (null == pdf_document.LibraryRef.Xlibrary.ExpeditionManager.ExpeditionDataSource)
            {
                return;
            }

            ExpeditionDataSource eds = pdf_document.LibraryRef.Xlibrary.ExpeditionManager.ExpeditionDataSource;
            LDAAnalysis lda_analysis = eds.LDAAnalysis;

            try
            {
                if (!pdf_document.LibraryRef.Xlibrary.ExpeditionManager.ExpeditionDataSource.docs_index.ContainsKey(pdf_document.Fingerprint))
                {
                    MessageBoxes.Warn("Expedition doesn't have any information about this paper.  Please Refresh your Expedition.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem with Expedition for document {0}", pdf_document.Fingerprint);
            }

            if (pdf_document_bindable.Underlying.DocumentExists)
            {
                ObjPDFRendererControlPlaceholderBorder.Visibility = Visibility.Visible;
                ObjPDFRendererControlPlaceholderRow.Height = new GridLength(1, GridUnitType.Star);

                PDFRendererControl pdf_renderer_control = new PDFRendererControl(pdf_document_bindable.Underlying, false, PDFRendererControl.ZoomType.Zoom1Up);
                ObjPDFRendererControlPlaceholder.Children.Add(pdf_renderer_control);
            }
            else
            {
                ObjPDFRendererControlPlaceholderBorder.Visibility = Visibility.Collapsed;
                ObjPDFRendererControlPlaceholderRow.Height = new GridLength(0, GridUnitType.Pixel);
            }
        }
    }
}
