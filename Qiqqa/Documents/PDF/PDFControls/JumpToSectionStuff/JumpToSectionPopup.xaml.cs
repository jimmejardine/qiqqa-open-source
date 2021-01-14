using System.Windows;
using System.Windows.Controls;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    /// <summary>
    /// Interaction logic for JumpToSectionPopup.xaml
    /// </summary>
    public partial class JumpToSectionPopup : StackPanel
    {
        internal PDFReadingControl pdf_reading_control;
        internal PDFRendererControl pdf_render_control;
        internal PDFRendererControlStats pdf_renderer_control_stats;
        internal AugmentedPopup popup;

        public JumpToSectionPopup(PDFReadingControl pdf_reading_control, PDFRendererControl pdf_render_control, PDFRendererControlStats pdf_renderer_control_stats)
        {
            InitializeComponent();

            this.pdf_reading_control = pdf_reading_control;
            this.pdf_render_control = pdf_render_control;
            this.pdf_renderer_control_stats = pdf_renderer_control_stats;
            popup = new AugmentedPopup(this);

            // Add the bit explaining how to use bookmarks
            {
                TextBlock tb = new TextBlock();
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Bookmarks:";
                Children.Add(tb);
            }

            {
                {
                    MenuItem mi = new MenuItem();
                    mi.Header =
                        ""
                        + "You can create up to 9 bookmarks while reading a PDF by\n"
                        + "holding down CTRL+SHIFT and pressing a number from 1 to 9.";
                    Children.Add(mi);
                }
                {
                    MenuItem mi = new MenuItem();
                    mi.Header =
                        ""
                    + "You can later jump to a remembered bookmark by\n"
                    + "holding down CTRL and pressing a number from 1 to 9.";
                    Children.Add(mi);
                }
            }

            Children.Add(new AugmentedSpacer());

            // Then add the sections
            {
                TextBlock tb = new TextBlock();
                tb.FontWeight = FontWeights.Bold;
                tb.Text = "Sections:";
                Children.Add(tb);
            }

            // First try from the PDF
            {
                BuildPopupFromPDF build_popup_from_pdf = new BuildPopupFromPDF(this);
                build_popup_from_pdf.BuildMenu();
            }

            // If there are not enough bookmarks, go the OCR route
            if (pdf_renderer_control_stats.pdf_document.PDFRenderer.PageCount < 100)
            {
                BuildPopupFromOCR build_popup_from_ocr = new BuildPopupFromOCR(this);
                build_popup_from_ocr.BuildMenu();
            }

            this.Unloaded += JumpToSectionPopup_Unloaded;
        }

        private void JumpToSectionPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            pdf_reading_control = null;
            pdf_render_control = null;
            pdf_renderer_control_stats = null;
            popup = null;
        }

        public void Open()
        {
            popup.IsOpen = true;
        }

        public void Close()
        {
            popup.Close();
        }
    }
}
