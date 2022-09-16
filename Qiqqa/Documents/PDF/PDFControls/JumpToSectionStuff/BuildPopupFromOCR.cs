using System.Globalization;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.JumpToSectionStuff
{
    internal class BuildPopupFromOCR
    {
        private JumpToSectionPopup popup;
        private PDFDocument pdf_document;

        internal BuildPopupFromOCR(JumpToSectionPopup popup, PDFDocument pdf_document)
        {
            this.popup = popup;
            this.pdf_document = pdf_document;
        }

        internal void BuildMenu()
        {
            if (null == pdf_document)
            {
                Logging.Info("No document has been selected, so no jump to can be built");
                return;
            }

            WordList[] word_lists = new WordList[pdf_document.PDFRenderer.PageCount + 1];
            bool missing_text = false;
            for (int page = 1; page <= pdf_document.PDFRenderer.PageCount; ++page)
            {
                WordList word_list = pdf_document.PDFRenderer.GetOCRText(page);
                if (null != word_list)
                {
                    word_lists[page] = word_list;
                }
                else
                {
                    missing_text = true;
                }
            }

            AddItemChild_Front(word_lists, "Abstract");
            AddItemChild_Front(word_lists, "Keywords");
            AddItemChild_Front(word_lists, "Introduction");
            AddItemChild_Front(word_lists, "Review");
            AddItemChild_Front(word_lists, "Literature");
            AddItemChild_Front(word_lists, "Method");
            AddItemChild_Front(word_lists, "Implementation");
            AddItemChild_Front(word_lists, "Experiment");

            AddItemChild_Rear(word_lists, "Result");
            AddItemChild_Rear(word_lists, "Results");
            AddItemChild_Rear(word_lists, "Discussion");
            AddItemChild_Rear(word_lists, "Evaluation");
            AddItemChild_Rear(word_lists, "Conclusion");
            AddItemChild_Rear(word_lists, "Conclusions");
            AddItemChild_Rear(word_lists, "Bibliography");
            AddItemChild_Rear(word_lists, "Reference");
            AddItemChild_Rear(word_lists, "References");
            AddItemChild_Rear(word_lists, "Appendix");
            AddItemChild_Rear(word_lists, "Appendices");
            AddItemChild_Rear(word_lists, "Acknowledgements");

            // Add a notice if OCR is not complete
            if (missing_text)
            {
                // add a warning that OCR is not complete
            }
        }


        private void AddItemChild_Largest(WordList[] word_lists, string section)
        {
            double largest_height = double.MinValue;
            int largest_page = 0;

            string section_lower = section.ToLower();

            for (int page = 1; page <= pdf_document.PDFRenderer.PageCount; ++page)
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
                popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, section, largest_page));
            }
        }

        private void AddItemChild_Front(WordList[] word_lists, string section)
        {
            AddItemChild_Largest(word_lists, section);
        }

        private void AddItemChild_Rear(WordList[] word_lists, string section)
        {
            AddItemChild_Largest(word_lists, section);
        }

        private void AddItemChild_Front_OLD(WordList[] word_lists, string section)
        {
            string section_lower = section.ToLower();

            for (int page = 1; page <= pdf_document.PDFRenderer.PageCount; ++page)
            {
                WordList word_list = word_lists[page];
                if (null != word_list)
                {
                    for (int i = 0; i < word_list.Count; ++i)
                    {
                        Word word = word_list[i];
                        if (word.Text.ToLower().Contains(section_lower))
                        {
                            popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, section, page));
                            return;
                        }
                    }
                }
            }
        }

        private void AddItemChild_Rear_OLD(WordList[] word_lists, string section)
        {
            string section_lower = section.ToLower();

            for (int page = pdf_document.PDFRenderer.PageCount; page >= 1; --page)
            {
                WordList word_list = word_lists[page];
                if (null != word_list)
                {
                    for (int i = word_list.Count - 1; i >= 0; --i)
                    {
                        Word word = word_list[i];
                        if (word.Text.ToLower().Contains(section_lower))
                        {
                            popup.Children.Add(new JumpToSectionItem(popup, popup.pdf_reading_control, section, page));
                            return;
                        }
                    }
                }
            }
        }
    }
}
