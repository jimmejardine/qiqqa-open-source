using System.Text;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    class PDFAbstractExtraction
    {
        public static readonly string CANT_LOCATE = "Can't locate abstract!  You can select the abstract while reading the PDF, then right click, and choose 'Set as Abstract override.'";

        public static string GetAbstractForDocument(PDFDocument pdf_document)
        {
            // Try on the first two pages (sometimes there is a couvert page)
            for (int page = 1; page <= 3; ++page)
            {
                string result = GetAbstractForDocument(pdf_document, page);
                if (CANT_LOCATE != result) return result;
            }

            return CANT_LOCATE;
        }

        private static string GetAbstractForDocument(PDFDocument pdf_document, int page)
        {
            if (pdf_document.DocumentExists)
            {
                // Get the OCR
                WordList word_list = pdf_document.PDFRenderer.GetOCRText(page);
                if (null == word_list) return CANT_LOCATE;

                // First find all the locations
                int abstract_start = -1;
                int intro_start = -1;
                int keywords_start = -1;
                int one_start = -1;

                if (true)
                {
                    for (int i = 0; i < word_list.Count; ++i)
                    {
                        string test_word = word_list[i].Text;
                        string test_word_lower = test_word.ToLower();


                        if (-1 == abstract_start && test_word_lower.Contains("abstract") && 'A' == test_word[0])
                        {
                            abstract_start = i;
                        }

                        if (-1 == intro_start && test_word_lower.Contains("introduction") && 'I' == test_word[0])
                        {
                            intro_start = i;
                        }

                        if (-1 == keywords_start && test_word_lower.Contains("keywords") && 'K' == test_word[0])
                        {
                            keywords_start = i;
                        }

                        if (-1 == one_start)
                        {
                            if
                            (
                                false
                                || "1" == test_word_lower
                                || "1." == test_word_lower
                                || "1)" == test_word_lower
                                || "(1)" == test_word_lower
                            )
                            {
                                one_start = i;
                            }
                        }
                    }

                    // Check that the word just before "introduction" isn't the section number 1
                    if (0 < intro_start && word_list[intro_start - 1].Text.ToLower().Contains("1"))
                    {
                        --intro_start;
                    }
                }

                // Look for ABSTRACT and INTRODUCTION and take everthing in between...
                if (true)
                {
                    if (-1 != intro_start && -1 != abstract_start && abstract_start < intro_start)
                    {
                        return BuildFromRange(word_list, abstract_start + 1, intro_start);
                    }
                }

                // If we don't have the word ABSTRACT, but we do have the word INTRODUCTION, run with that...
                if (true)
                {
                    if (-1 != intro_start)
                    {
                        return BuildFromRange(word_list, 0, intro_start);
                    }
                }

                // Look for ABSTRACT and KEYWORDS and take everthing in between...
                if (true)
                {
                    if (-1 != keywords_start && -1 != abstract_start && abstract_start < keywords_start)
                    {
                        return BuildFromRange(word_list, abstract_start + 1, keywords_start);
                    }
                }

                // If we don't have the word ABSTRACT, but we do have the number 1 appearing somewhere into the page (15 words)
                if (true)
                {
                    if (-1 != one_start && one_start > 15)
                    {
                        return BuildFromRange(word_list, 0, one_start);
                    }
                }

                // If we do have the word ABSTRACT, just run with it!
                if (true)
                {
                    if (-1 != abstract_start)
                    {
                        return BuildFromRange(word_list, abstract_start + 1, word_list.Count);
                    }
                }
            }

            // If we get here we have failed...
            return CANT_LOCATE;
        }

        private static string BuildFromRange(WordList word_list, int start_inclusive, int finish_exclusive)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = start_inclusive; i < finish_exclusive; ++i)
            {
                sb.Append(word_list[i].Text);
                sb.Append(" ");
            }

            // Post processing of strings
            string interim = sb.ToString();
            interim = interim.Replace("- ", "");  // Kill hyphenated line wraps

            return interim;
        }
    }
}
