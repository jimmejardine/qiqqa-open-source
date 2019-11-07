using System.Windows;
using System.Windows.Controls;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for CommentsControl.xaml
    /// </summary>
    public partial class CommentsControl : UserControl
    {
        public CommentsControl()
        {
            InitializeComponent();

            ButtonComments.Caption = "Popup";
            ButtonComments.Click += ButtonComments_Click;

            ObjCommentsBox.ToolTip = "This space lets you store any comments you may have about this reference.";
        }

        private void ButtonComments_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            MetadataCommentEditorControl editor = new MetadataCommentEditorControl();
            editor.Show(pdf_document_bindable);
        }
    }
}
