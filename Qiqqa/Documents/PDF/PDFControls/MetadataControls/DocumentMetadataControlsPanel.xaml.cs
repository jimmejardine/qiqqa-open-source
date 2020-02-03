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

            ObjTabs.Children.Clear();
            ObjTabs.AddContent("Properties", "Properties", null, false, false, TabMetadata);
            ObjTabs.AddContent("Annotations", "Annotations", null, false, false, TabUserData);
            ObjTabs.AddContent("Preview", "Preview", null, false, false, TabPreview);

            HyperlinkRestore.Click += HyperlinkRestore_Click;

            DataContextChanged += DocumentMetadataControlsPanel_DataContextChanged;

            ObjTweetBar.Visibility = Visibility.Visible;

            ReevaluateDataContext();
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
            ReevaluateDataContext();
        }

        private void ReevaluateDataContext()
        {
            bool have_pdf_to_render = false;

            GridPreview.Children.Clear();

            if (null != DataContext)
            {
                AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
                if (null != pdf_document_bindable)
                {
                    if (pdf_document_bindable.Underlying.DocumentExists)
                    {
                        PDFRendererControl pdf_renderer_control = new PDFRendererControl(pdf_document_bindable.Underlying, false, PDFRendererControl.ZoomType.Zoom1Up);
                        GridPreview.Children.Add(pdf_renderer_control);
                        pdf_renderer_control.SelectedPageChanged += pdf_renderer_control_SelectedPageChanged;
                        have_pdf_to_render = true;
                    }
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
        }

        private void pdf_renderer_control_SelectedPageChanged(int page)
        {
            SelectedPageChanged?.Invoke(page);
        }
    }
}
