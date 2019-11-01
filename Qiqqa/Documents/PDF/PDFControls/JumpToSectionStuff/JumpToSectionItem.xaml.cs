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
        private PDFRendererControl pdf_render_control;
        private PDFRendererControlStats pdf_renderer_control_stats;
        private string section;
        private int page;

        public JumpToSectionItem(JumpToSectionPopup jtsp, PDFReadingControl pdf_reading_control, PDFRendererControl pdf_render_control, PDFRendererControlStats pdf_renderer_control_stats, string section, int page)
        {
            InitializeComponent();

            this.jtsp = jtsp;
            this.pdf_reading_control = pdf_reading_control;
            this.pdf_render_control = pdf_render_control;
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            this.section = section;
            this.page = page;

            ButtonSection.Header = section;
            ButtonSection.Click += ButtonSection_Click;
        }

        private void ButtonSection_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_JumpToSection);

            PDFRendererPageControl prpc = pdf_render_control.GetPageControl(page);

            string search_query = '"' + section + '"';
            pdf_reading_control.SetSearchKeywords(search_query);
            pdf_render_control.SelectedPage = prpc;
            if (null != pdf_render_control.SelectedPage)
            {
                pdf_render_control.SelectedPage.BringIntoView();
            }
            jtsp.Close();
        }
    }
}
