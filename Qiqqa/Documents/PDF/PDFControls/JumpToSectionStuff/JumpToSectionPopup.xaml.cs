using System.Windows;
using System.Windows.Controls;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    /// <summary>
    /// Interaction logic for JumpToSectionPopup.xaml
    /// </summary>
    public partial class JumpToSectionPopup : StackPanel
    {
        internal PDFReadingControl pdf_reading_control;
        internal AugmentedPopup popup;

        public JumpToSectionPopup(PDFReadingControl pdf_reading_control)
        {
            InitializeComponent();

            this.pdf_reading_control = pdf_reading_control;
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

            // If there are not enough bookmarks, go the OCR route
            PDFDocument pdf_document = pdf_reading_control.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            // First try from the PDF
            if (pdf_document != null)
            {
                BuildPopupFromPDF build_popup_from_pdf = new BuildPopupFromPDF(this, pdf_document);
                build_popup_from_pdf.BuildMenu();
            }

            if (pdf_document != null && pdf_document.PDFRenderer.PageCount < 100)
            {
                BuildPopupFromOCR build_popup_from_ocr = new BuildPopupFromOCR(this, pdf_document);
                build_popup_from_ocr.BuildMenu();
            }

            //Unloaded += JumpToSectionPopup_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            CleanUp();
        }

        private void JumpToSectionPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }

        private void CleanUp()
        {
            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;

            pdf_reading_control = null;
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
