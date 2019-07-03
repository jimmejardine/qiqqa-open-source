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


//            TextLabelTitle.OnClick += new HyperlinkTextBlock.OnClickDelegate(TextLabelTitle_OnClick);
  //          TextLabelAuthors.OnClick += new HyperlinkTextBlock.OnClickDelegate(TextLabelAuthors_OnClick);
        }

        void TextLabelTitle_OnClick(object sender, MouseButtonEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.SearchWeb(pdf_document_bindable.Underlying.TitleCombined);
                e.Handled = true;
            }
        }

        void TextLabelAuthors_OnClick(object sender, MouseButtonEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null != pdf_document_bindable)
            {
                MainWindowServiceDispatcher.Instance.SearchWeb(pdf_document_bindable.Underlying.AuthorsCombined);
                e.Handled = true;
            }
        }

    }
}
