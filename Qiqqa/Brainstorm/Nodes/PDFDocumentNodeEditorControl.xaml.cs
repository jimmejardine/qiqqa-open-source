using System.Windows.Controls;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for PDFDocumentNodeEditorControl.xaml
    /// </summary>
    public partial class PDFDocumentNodeEditorControl : UserControl
    {
        public PDFDocumentNodeEditorControl(NodeControl node_control, PDFDocumentNodeContent pdf_document_node_content)
        {
            InitializeComponent();

            this.ObjDocumentMetadataControlsPanel.DataContext = pdf_document_node_content.PDFDocument.Bindable;
        }
    }
}
