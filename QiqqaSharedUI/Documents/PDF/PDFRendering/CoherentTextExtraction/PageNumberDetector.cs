#if false

using System;
using System.Collections.Generic;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFRendering.CoherentTextExtraction
{
    class PageNumberDetector
    {
        //const double HEADER_FOOTER_SIZE = 0.1;
        //const double HEADER_END = HEADER_FOOTER_SIZE;
        //const double FOOTER_BEGIN = 1.0 - HEADER_FOOTER_SIZE;
        
        PageDetail[] page_details;

        List<Word> words_whole_page_numbers = new List<Word>();
        List<Word> words_dirty_page_numbers = new List<Word>();

        public PageNumberDetector(PageDetail[] page_details)
        {
            this.page_details = page_details;

            foreach (PageDetail page_detail in page_details)
            {
                if (0 < page_detail.word_list.Count)
                {
                    // The first word
                    {
                        Word word = page_detail.word_list[0];
                        if (IsNumber(word.Text))
                        {
                            words_whole_page_numbers.Add(word);
                        }
                    }

                    // The last word
                    {
                        Word word = page_detail.word_list[page_detail.word_list.Count - 1];

                        if (IsNumber(word.Text))
                        {
                            words_whole_page_numbers.Add(word);
                        }
                        else if (EndsWithNumber(word.Text))
                        {
                            words_dirty_page_numbers.Add(word);
                        }
                    }
                }
            }
        }

        public bool IsPageNumber(Word word)
        {
            return words_whole_page_numbers.Contains(word);
        }

        public bool IsDirtyPageNumber(Word word)
        {
            return words_dirty_page_numbers.Contains(word);
        }

        public string GetWordCleansedOfDirtyPageNumber(Word word)
        {
            int no_number_pos = word.Text.Length-1;
            while (no_number_pos > 0)
            {
                if (!Char.IsNumber(word.Text, no_number_pos))
                {
                    break;
                }
                --no_number_pos;
            }

            return word.Text.Substring(0, no_number_pos);
        }

        static bool EndsWithNumber(string word)
        {
            return Char.IsNumber(word, word.Length-1);
        }

        static bool IsNumber(string word)
        {
            int page_number;
            return Int32.TryParse(word, out page_number);
        }
    }
}

#endif
