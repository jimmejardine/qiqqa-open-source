using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for QuickMetadataControl.xaml
    /// </summary>
    public partial class QuickMetadataControl : UserControl
    {
        public QuickMetadataControl()
        {
            InitializeComponent();

            //ObjTitle.OnClick += new ObjTitle.OnClickDelegate(TextLabelTitle_OnClick);
            //ObjAuthors.OnClick += new ObjAuthors.OnClickDelegate(TextLabelAuthors_OnClick);
        }

        private void TextLabelTitle_OnClick(object sender, MouseButtonEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.SearchWeb(pdf_document_bindable.Underlying.TitleCombined);
                e.Handled = true;
            }
        }

        private void TextLabelAuthors_OnClick(object sender, MouseButtonEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.SearchWeb(pdf_document_bindable.Underlying.AuthorsCombined);
                e.Handled = true;
            }
        }
    }
}
