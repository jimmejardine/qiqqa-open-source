using System.Windows;
using System.Windows.Controls;
using Qiqqa.Documents.PDF.PDFControls.Page;
using Qiqqa.UtilisationTracking;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    /// <summary>
    /// Interaction logic for JumpToSectionItem.xaml
    /// </summary>
    public partial class JumpToSectionItem : Grid
    {
        private JumpToSectionPopup jtsp;
        private PDFReadingControl pdf_reading_control;
        private string section;
        private int page;

        public JumpToSectionItem(JumpToSectionPopup jtsp, PDFReadingControl pdf_reading_control, string section, int page)
        {
            InitializeComponent();

            this.jtsp = jtsp;
            this.pdf_reading_control = pdf_reading_control;
            this.section = section;
            this.page = page;

            ButtonSection.Header = section;
            ButtonSection.Click += ButtonSection_Click;
        }

        private void ButtonSection_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_JumpToSection);

            PDFRendererControl pdf_renderer_control = pdf_reading_control.GetPDFRendererControl();

            PDFRendererPageControl prpc = pdf_renderer_control.GetPageControl(page);

            string search_query = '"' + section + '"';
            pdf_reading_control.SetSearchKeywords(search_query);
            pdf_renderer_control.SelectedPage = prpc;
            if (null != pdf_renderer_control.SelectedPage)
            {
                pdf_renderer_control.SelectedPage.BringIntoView();
            }
            jtsp.Close();
        }
    }
}
