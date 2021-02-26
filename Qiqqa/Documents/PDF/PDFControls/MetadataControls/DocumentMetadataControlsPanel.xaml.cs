using System.Windows;
using System.Windows.Controls;
using Utilities.GUI;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for DocumentMetadataControlsPanel.xaml
    /// </summary>
    public partial class DocumentMetadataControlsPanel : UserControl
    {
        public event PDFRendererControl.SelectedPageChangedDelegate SelectedPageChanged;

        public DocumentMetadataControlsPanel()
        {
            Theme.Initialize();

            InitializeComponent();

            //Unloaded += DocumentMetadataControlsPanel_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            ObjTabs.Children.Clear();
            ObjTabs.AddContent("Properties", "Properties", null, false, false, TabMetadata);
            ObjTabs.AddContent("Annotations", "Annotations", null, false, false, TabUserData);
            ObjTabs.AddContent("Preview", "Preview", null, false, false, TabPreview);

            HyperlinkRestore.Click += HyperlinkRestore_Click;

            DataContextChanged += DocumentMetadataControlsPanel_DataContextChanged;

            ReevaluateDataContext();
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                CleanUp();
            });
        }

        // WARNING: https://docs.microsoft.com/en-us/dotnet/api/system.windows.frameworkelement.unloaded?view=net-5.0
        // Which says:
        //
        // Note that the Unloaded event is not raised after an application begins shutting down.
        // Application shutdown occurs when the condition defined by the ShutdownMode property occurs.
        // If you place cleanup code within a handler for the Unloaded event, such as for a Window
        // or a UserControl, it may not be called as expected.
        private void DocumentMetadataControlsPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }

        private void CleanUp()
        {
            // TODO: ditch the pdf renders in the GridPreview children list...
            //DataContextChanged -= DocumentMetadataControlsPanel_DataContextChanged;
            DataContext = null;

            ObjTabs.Children.Clear();
            GridPreview.Children.Clear();

            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
        }

        private void HyperlinkRestore_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                pdf_document_bindable.Underlying.Deleted = false;
                pdf_document_bindable.NotifyPropertyChanged(nameof(pdf_document_bindable.Underlying.Deleted));
            }

            e.Handled = true;
        }

        private void DocumentMetadataControlsPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                ReevaluateDataContext();
            });
        }

        private void ReevaluateDataContext()
        {
            bool have_pdf_to_render = false;
            bool have_deleted_pdf = false;

            GridPreview.Children.Clear();

            if (null != DataContext)
            {
                AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
                if (null != pdf_document_bindable)
                {
                    if (pdf_document_bindable.Underlying.DocumentExists)
                    {
                        PDFRendererControl pdf_renderer_control = new PDFRendererControl(pdf_document_bindable.Underlying, remember_last_read_page: false, PDFRendererControl.ZoomType.Zoom1Up);
                        GridPreview.Children.Add(pdf_renderer_control);
                        pdf_renderer_control.SelectedPageChanged += pdf_renderer_control_SelectedPageChanged;
                        have_pdf_to_render = true;
                    }

                    have_deleted_pdf = pdf_document_bindable.Underlying.Deleted;
                }

                TxtNullDataContext.Visibility = Visibility.Collapsed;
                ObjDocumentInfo.Visibility = Visibility.Visible;
                if (!have_pdf_to_render)
                {
                    ObjAbstract.Expand();
                }
                IsEnabled = true;
            }
            else
            {
                TxtNullDataContext.Visibility = Visibility.Visible;
                ObjDocumentInfo.Visibility = Visibility.Collapsed;
                IsEnabled = false;
            }

#if true
            if (have_deleted_pdf)
            {
                DocumentIsDeletedNode.Visibility = Visibility.Visible;
            }
            else
            {
                DocumentIsDeletedNode.Visibility = Visibility.Collapsed;
            }
#endif
        }

        private void pdf_renderer_control_SelectedPageChanged(int page)
        {
            WPFDoEvents.SafeExec(() =>
            {
                SelectedPageChanged?.Invoke(page);
            });
        }
    }
}
