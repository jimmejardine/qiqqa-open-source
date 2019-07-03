using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Brainstorm.SceneManager;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentControl.xaml
    /// </summary>
    public partial class PDFTagNodeContentControl : UserControl, IKeyPressableNodeContentControl
    {
        NodeControl node_control;
        PDFTagNodeContent pdf_tag_node_content;

        public PDFTagNodeContentControl(NodeControl node_control, PDFTagNodeContent pdf_tag_node_content)
        {
            this.node_control = node_control;
            this.pdf_tag_node_content = pdf_tag_node_content;
            this.DataContext = pdf_tag_node_content;

            InitializeComponent();

            this.Focusable = true;

            this.ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormPDFTag);

            ImageIcon.Width = NodeThemes.image_width;
            TextBorder.CornerRadius = NodeThemes.corner_radius;
            TextBorder.Background = NodeThemes.background_brush;
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            if (false) { }
            else if (Key.D == e.Key)
            {
                ExpandDocuments();
                e.Handled = true;
            }
        }

        public void ExpandDocuments()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Tag_Documents);

            List<PDFDocument> pdf_documents = pdf_tag_node_content.Library.GetDocumentsByTag(pdf_tag_node_content.Tag);
            
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.Library.WebLibraryDetail.Id);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
            }            
        }
    }
}
