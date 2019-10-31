using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Collections;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFRendering.CoherentTextExtraction
{
    public static class PDFCoherentTextExtractor
    {
        public class ExtractionResult
        {
            public enum ResultType
            {
                OCR_NOT_COMPLETE,
                EXCEPTION,
                SUCCESS
            }

            public ResultType result_type;
            public Exception exception;
            public List<string> words;

            public ExtractionResult(ResultType result_type)
            {
                this.result_type = result_type;
            }

            public ExtractionResult(ResultType result_type, Exception ex)
            {
                this.result_type = result_type;
                this.exception = ex;
            }

            public ExtractionResult(ResultType result_type, List<string> words)
            {
                this.result_type = result_type;
                this.words = words;
            }
        }


        public static ExtractionResult ExtractText(PDFRenderer pdf_renderer)
        {
            Logging.Info("Doing text extraction for {0}", pdf_renderer.ToString());

            try
            {
                int page_count = pdf_renderer.PageCount;
                PageDetail[] page_details = new PageDetail[page_count];

                for (int i = 0; i < page_count; ++i)
                {
                    page_details[i] = new PageDetail(i + 1);
                }

                bool all_pages_already_with_ocr = true;
                foreach (PageDetail page_detail in page_details)
                {
                    WordList word_list = pdf_renderer.GetOCRText(page_detail.page);

                    if (null != word_list)
                    {
                        Logging.Debug特("Page {0} has OCR available ({1})", page_detail.page, pdf_renderer.DocumentFingerprint);
                    }
                    else
                    {
                        Logging.Debug特("Page {0} has not had OCR done ({1})", page_detail.page, pdf_renderer.DocumentFingerprint);
                        all_pages_already_with_ocr = false;
                    }
                }

                if (!all_pages_already_with_ocr)
                {
                    Logging.Info("Not all pages are ready with OCR");
                    return new ExtractionResult(ExtractionResult.ResultType.OCR_NOT_COMPLETE);
                }

                // All pages OCR are complete, so load the words lists
                foreach (PageDetail page_detail in page_details)
                {
                    page_detail.word_list = pdf_renderer.GetOCRText(page_detail.page);
                }

                // Order the words on each page in a manner that makes sense of multiple columns
                List<Word> words_ordered = new List<Word>();
                foreach (PageDetail page_detail in page_details)
                {
                    WordList words_ordered_for_page = ColumnWordOrderer.ReorderWords(page_detail.word_list);
                    words_ordered.AddRange(words_ordered_for_page);
                }

                // Concatenate the words
                List<string> words = new List<string>();
                foreach (Word word in words_ordered)
                {
                    words.Add(word.Text);
                }                

                // Kill some of the line-wrapping hyphenation
                for (int i = words.Count - 2; i >= 0; --i)
                {
                    if (words[i].EndsWith("-"))
                    {
                        words[i] = words[i].Substring(0, words[i].Length - 1) + words[i + 1];
                        words.RemoveAt(i + 1);
                    }
                }                
                
                // Return the words
                Logging.Info("Successfully extracted {0} words: {1}", words.Count, ArrayFormatter.listElements(words));                 
                return new ExtractionResult(ExtractionResult.ResultType.SUCCESS, words);
            }

            catch (Exception ex)
            {
                Logging.Warn(ex, "There was an exception while extracting coherent text");
                return new ExtractionResult(ExtractionResult.ResultType.EXCEPTION, ex);
            }
        }
    }
}
