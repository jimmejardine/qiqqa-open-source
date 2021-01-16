using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Images;
using Image = System.Drawing.Image;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentControl.xaml
    /// </summary>
    public partial class PDFAnnotationNodeContentControl : UserControl, IKeyPressableNodeContentControl, IDisposable
    {
        private PDFAnnotationNodeContent pdf_annotation_node_content;

        //
        // Warning CA1001  Implement IDisposable on 'PDFAnnotationNodeContentControl' because it creates 
        // members of the following IDisposable types: 'LibraryIndexHoverPopup'. 
        // If 'PDFAnnotationNodeContentControl' has previously shipped, adding new members that implement 
        // IDisposable to this type is considered a breaking change to existing consumers.
        //
        // Note from GHO: that object is already managed through the sequence of tooltip_open and tooltip_close 
        // handlers below and is currently not considered a memory leak risk for https://github.com/jimmejardine/qiqqa-open-source/issues/19
        // and there-abouts.

        private LibraryIndexHoverPopup library_index_hover_popup = null;

        public PDFAnnotationNodeContentControl(NodeControl node_control, PDFAnnotationNodeContent pdf_annotation_node_content)
        {
            this.pdf_annotation_node_content = pdf_annotation_node_content;
            DataContext = pdf_annotation_node_content.PDFAnnotation;

            InitializeComponent();

            Focusable = true;

            ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormPDFAnnotation);
            RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            ImageIcon.Width = NodeThemes.image_width;
            TextBorder.CornerRadius = NodeThemes.corner_radius;
            TextBorder.Background = NodeThemes.background_brush;

            MouseDoubleClick += PDFAnnotationNodeContentControl_MouseDoubleClick;
            ToolTip = "";
            ToolTipClosing += PDFDocumentNodeContentControl_ToolTipClosing;
            ToolTipOpening += PDFDocumentNodeContentControl_ToolTipOpening;
        }

        private void PDFAnnotationNodeContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PDFDocument out_pdf_document;
            PDFAnnotation out_pdf_annotation;
            if (WebLibraryDocumentLocator.LocateFirstPDFDocumentWithAnnotation(pdf_annotation_node_content.library_fingerprint, pdf_annotation_node_content.pdf_document_fingerprint, pdf_annotation_node_content.pdf_annotation_guid, out out_pdf_document, out out_pdf_annotation))
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(out_pdf_document, page: out_pdf_annotation.Page);
            }
        }

        private void PDFDocumentNodeContentControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            try
            {
                if (null == library_index_hover_popup)
                {
                    library_index_hover_popup = new LibraryIndexHoverPopup();

                    PDFDocument out_pdf_document;
                    PDFAnnotation out_pdf_annotation;
                    if (WebLibraryDocumentLocator.LocateFirstPDFDocumentWithAnnotation(pdf_annotation_node_content.library_fingerprint, pdf_annotation_node_content.pdf_document_fingerprint, pdf_annotation_node_content.pdf_annotation_guid, out out_pdf_document, out out_pdf_annotation))
                    {
                        library_index_hover_popup.SetPopupContent(out_pdf_document, out_pdf_annotation.Page, out_pdf_annotation);
                        ToolTip = library_index_hover_popup;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception while displaying document preview popup");
            }
        }

        private void PDFDocumentNodeContentControl_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            ToolTip = "";

            library_index_hover_popup?.Dispose();
            library_index_hover_popup = null;
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            if (Key.P == e.Key)
            {
                PDFAnnotation pdf_annotation = pdf_annotation_node_content.PDFAnnotation.Underlying;
                PDFDocument pdf_document = pdf_annotation_node_content.PDFDocument.Underlying;

                int target_resolution = 150;
                int actual_resolution = target_resolution;
                double resolution_rescale_factor = 1;

                Image annotation_image = PDFAnnotationToImageRenderer.RenderAnnotation(pdf_document, pdf_annotation, actual_resolution);
                BitmapSource cropped_image_page = BitmapImageTools.FromImage(annotation_image, (int)(annotation_image.Width * resolution_rescale_factor), (int)(annotation_image.Height * resolution_rescale_factor));

                ImageIcon.Source = cropped_image_page;
                ImageIcon.Width = cropped_image_page.Width / 2;
                TextText.MaxWidth = ImageIcon.Width;

                e.Handled = true;
            }
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFAnnotationNodeContentControl()
        {
            Logging.Debug("~PDFAnnotationNodeContentControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFAnnotationNodeContentControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFAnnotationNodeContentControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        library_index_hover_popup?.Dispose();

                        ToolTipClosing -= PDFDocumentNodeContentControl_ToolTipClosing;
                        ToolTipOpening -= PDFDocumentNodeContentControl_ToolTipOpening;
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    // Clear the references for sanity's sake
                    pdf_annotation_node_content = null;
                    library_index_hover_popup = null;
                });

                ++dispose_count;
            });
        }

        #endregion

    }
}

