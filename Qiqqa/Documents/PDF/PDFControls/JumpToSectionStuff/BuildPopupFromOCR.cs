using System;
using System.Globalization;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromOCR
    {
        static internal void BuildMenu(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            PDFDocument pdf_document = pdf_reading_control.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            WordList[] word_lists = new WordList[Math.Max(0, pdf_document.PageCount) + 1];
            bool missing_text = false;
            for (int page = 1; page <= pdf_document.PageCount; ++page)
            {
                WordList word_list = pdf_document.GetOCRText(page);
                if (null != word_list)
                {
                    word_lists[page] = word_list;
                }
                else
                {
                    missing_text = true;
                }
            }

            // TODO: this menu is hard-coded and fully geared towards English-only whitepapers. Make this more flexible.
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Abstract");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Keywords");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Introduction");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Review");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Literature");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Method");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Implementation");
            AddItemChild_Front(popup, pdf_reading_control, word_lists, "Experiment");

            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Result");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Results");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Discussion");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Evaluation");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Conclusion");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Conclusions");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Bibliography");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Reference");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "References");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Appendix");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Appendices");
            AddItemChild_Rear(popup, pdf_reading_control, word_lists, "Acknowledgements");

            // Add a notice if OCR is not complete
            if (missing_text)
            {
                // TODO: add a warning that OCR is not complete
            }
        }


        static private void AddItemChild_Largest(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control, WordList[] word_lists, string section)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            double largest_height = double.MinValue;
            int largest_page = 0;

            string section_lower = section.ToLower();

            PDFDocument pdf_document = pdf_reading_control.GetPDFDocument();
            ASSERT.Test(pdf_document != null);

            for (int page = 1; page <= pdf_document.PageCount; ++page)
            {
                WordList word_list = word_lists[page];
                if (null != word_list)
                {
                    for (int i = 0; i < word_list.Count; ++i)
                    {
                        Word word = word_list[i];
                        if (word.Text.ToLower().Contains(section_lower))
                        {
                            if (largest_height < word.Height)
                            {
                                largest_height = word.Height;
                                largest_page = page;
                            }
                        }
                    }
                }
            }

            if (0 != largest_page)
            {
                WPFDoEvents.InvokeInUIThread(() =>
                {
                    popup.Children.Add(new JumpToSectionItem(popup, pdf_reading_control, section, largest_page));
                });
            }
        }

        static private void AddItemChild_Front(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control, WordList[] word_lists, string section)
        {
            AddItemChild_Largest(popup, pdf_reading_control, word_lists, section);
        }

        static private void AddItemChild_Rear(JumpToSectionPopup popup, PDFReadingControl pdf_reading_control, WordList[] word_lists, string section)
        {
            AddItemChild_Largest(popup, pdf_reading_control, word_lists, section);
        }
    }
}
