using System.Windows.Controls;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentEditor.xaml
    /// </summary>
    public partial class PDFAuthorNodeContentEditor : UserControl
    {
        NodeControl node_control;
        PDFAuthorNodeContent pdf_author_node_content;

        public PDFAuthorNodeContentEditor(NodeControl node_control, PDFAuthorNodeContent pdf_author_node_content)
        {
            this.node_control = node_control;
            this.pdf_author_node_content = pdf_author_node_content;
            this.DataContext = pdf_author_node_content;

            InitializeComponent();
        }
    }
}
