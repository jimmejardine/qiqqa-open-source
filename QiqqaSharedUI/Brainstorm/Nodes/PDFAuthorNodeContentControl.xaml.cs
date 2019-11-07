using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Brainstorm.SceneManager;
using Qiqqa.DocumentLibrary.SimilarAuthorsStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Brainstorm.Nodes
{
    /// <summary>
    /// Interaction logic for DocumentNodeContentControl.xaml
    /// </summary>
    public partial class PDFAuthorNodeContentControl : UserControl, IKeyPressableNodeContentControl
    {
        private NodeControl node_control;
        private PDFAuthorNodeContent pdf_author_node_content;

        public PDFAuthorNodeContentControl(NodeControl node_control, PDFAuthorNodeContent pdf_author_node_content)
        {
            this.node_control = node_control;
            this.pdf_author_node_content = pdf_author_node_content;
            DataContext = pdf_author_node_content;

            InitializeComponent();

            Focusable = true;

            ImageIcon.Source = Icons.GetAppIcon(Icons.BrainstormPDFAuthor);

            ImageIcon.Width = NodeThemes.image_width;
            TextBorder.CornerRadius = NodeThemes.corner_radius;
            TextBorder.Background = NodeThemes.background_brush;
        }

        public void ProcessKeyPress(KeyEventArgs e)
        {
            if (Key.D == e.Key)
            {
                ExpandDocuments();
                e.Handled = true;
            }
        }

        private void ExpandDocuments()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Brainstorm_ExploreLibrary_Author_Documents);

            List<PDFDocument> pdf_documents = SimilarAuthors.GetDocumentsBySameAuthorsSurnameAndInitial(pdf_author_node_content.Library, pdf_author_node_content.Surname, pdf_author_node_content.Initial);
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                PDFDocumentNodeContent content = new PDFDocumentNodeContent(pdf_document.Fingerprint, pdf_document.Library.WebLibraryDetail.Id);
                NodeControlAddingByKeyboard.AddChildToNodeControl(node_control, content, false);
            }
        }
    }
}
