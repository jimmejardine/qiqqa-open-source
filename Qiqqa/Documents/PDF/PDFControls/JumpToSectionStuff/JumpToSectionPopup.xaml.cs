using System;
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
        internal AugmentedPopup popup;

        public JumpToSectionPopup()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            InitializeComponent();

            popup = new AugmentedPopup(this);

            //Unloaded += JumpToSectionPopup_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        public void BuildSectionList(PDFReadingControl pdf_reading_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            WPFDoEvents.InvokeInUIThread(() =>
            {
                Children.Clear();

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
            });

            // If there are not enough bookmarks, go the OCR route
            PDFDocument pdf_document = pdf_reading_control.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            // First try from the PDF
            if (pdf_document != null)
            {
                BuildPopupFromPDF.BuildMenu(this, pdf_reading_control);
            }

            int menu_item_count = 0;
            WPFDoEvents.InvokeInUIThread(() =>
            {
                menu_item_count = Children.Count;
            });

                // Then go and infer a set of chapters from the OCR results.
            if (pdf_document != null /* && pdf_document.PDFRenderer.PageCount < 100  -- plenty thesis papers out there with more than 100 pages... removed this arbitrary heuristic */
                && menu_item_count <= 1)
            {
                BuildPopupFromOCR.BuildMenu(this, pdf_reading_control);
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                CleanUp();
            });
        }

        private void JumpToSectionPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                CleanUp();
            });
        }

        private void CleanUp()
        {
            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;

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
