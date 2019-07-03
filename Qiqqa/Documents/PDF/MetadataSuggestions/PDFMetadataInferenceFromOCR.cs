using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    public class PDFMetadataInferenceFromOCR
    {
        internal static bool NeedsProcessing(PDFDocument pdf_document)
        {
            if (!pdf_document.DocumentExists) return false;
            if (!String.IsNullOrEmpty(pdf_document.TitleSuggested)) return false;
            if (pdf_document.AutoSuggested_OCRFrontPage) return false;

            Logging.Info("{0} requires PDFMetadataInferenceFromOCR", pdf_document.Fingerprint);
            return true;
        }

        internal static bool InferTitleFromOCR(PDFDocument pdf_document, bool allow_retry = false)
        {
            if (!pdf_document.DocumentExists) return false;
            if (pdf_document.AutoSuggested_OCRFrontPage && !allow_retry) return false;
            if (!String.IsNullOrEmpty(pdf_document.TitleSuggested) && !allow_retry) return false;

            // Only look for metadata if the OCR is ready
            WordList word_list = pdf_document.PDFRenderer.GetOCRText(1);
            if (null != word_list)
            {
                pdf_document.AutoSuggested_OCRFrontPage = true;
                pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AutoSuggested_OCRFrontPage);

                // Try get the title from the OCR
                string title = InferTitleFromWordList(word_list);
                if (IsReasonableTitleOrAuthor(title))
                {
                    Logging.Info("Auto-found in OCR metadata title '{0}'", title);
                    pdf_document.TitleSuggested = title;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.TitleSuggested);
                    return true;
                }
            }
            else
            {
                Logging.Info("While autosuggesting title, OCR is still not ready for {0}", pdf_document.Fingerprint);
            }

            return false;
        }

        class SentenceHeight
        {
            public string sentence;
            public double height;

            public override string ToString()
            {
                return String.Format("{0:0.00000}: {1}", height, sentence);
            }
        }

        private static string InferTitleFromWordList(WordList words)
        {
            if (0 == words.Count) return null;

            int word_offset = 0;

            int NUM_LINES_TO_SCAN = 20;
            List<SentenceHeight> sentence_heights = new List<SentenceHeight>();            
            for (int i = 0; i < NUM_LINES_TO_SCAN; ++i)
            {
                string sentence;
                double average_sentence_height;
                GetSentence(words, ref word_offset, out sentence, out average_sentence_height);
                sentence_heights.Add(new SentenceHeight { sentence = sentence, height = average_sentence_height } );                
            }

            // Trash shitty sentences
            for (int i = 0; i < sentence_heights.Count; ++i)
            {
                if ((null == sentence_heights[i].sentence) || (sentence_heights[i].sentence.Length < 5))
                {
                    sentence_heights.RemoveAt(i);                    
                    --i;
                    continue;
                }
            }

            // Look for the tallest sentence
            int biggest_sentence_pos = 0;
            for (int i = 0; i < sentence_heights.Count; ++i)
            {
                if (sentence_heights[i].height > sentence_heights[biggest_sentence_pos].height)
                {
                    biggest_sentence_pos = i;
                }
            }

            double SIZE_ERROR_MARGIN = 0.9;

            // Go back a little if the previous sentence is almost as big as the biggest sentence
            int previous_almost_biggest_sentence_pos = biggest_sentence_pos;
            while (previous_almost_biggest_sentence_pos > 0)
            {
                if (sentence_heights[previous_almost_biggest_sentence_pos - 1].height > sentence_heights[biggest_sentence_pos].height * SIZE_ERROR_MARGIN)
                {
                    previous_almost_biggest_sentence_pos = previous_almost_biggest_sentence_pos - 1;
                }
                else
                {
                    break;
                }
            }

            // Use all big sentences after the first big sentence
            StringBuilder final_sentence = new StringBuilder();
            for (int i = previous_almost_biggest_sentence_pos; i < sentence_heights.Count; ++i)
            {
                if (sentence_heights[i].height >= SIZE_ERROR_MARGIN * sentence_heights[biggest_sentence_pos].height)
                {
                    final_sentence.Append(sentence_heights[i].sentence);
                    final_sentence.Append(" ");
                }
                else
                {
                    break;
                }
            }

            return final_sentence.ToString().TrimEnd();
        }

        private static void GetSentence(WordList words, ref int word_offset, out string sentence, out double average_sentence_height)
        {
            StringBuilder sb = new StringBuilder();
            double last_left = 0;
            double height_num = 0;
            double height_den = 0;

            if (word_offset < words.Count)
            {
                last_left = words[word_offset].Left;
            }

            for (; word_offset < words.Count; ++word_offset)
            {
                Word word = words[word_offset];
                if (word.Left >= last_left)
                {
                    sb.Append(word.Text);
                    sb.Append(" ");

                    last_left = word.Left;
                    height_num += word.Text.Length * word.Height;
                    height_den += word.Text.Length;
                }
                else
                {
                    break;
                }
            }

            if (height_den > 0)
            {
                sentence = sb.ToString();
                average_sentence_height = height_num / height_den;
            }
            else
            {
                sentence = null;
                average_sentence_height = 0;
            }
        }

        public static bool IsReasonableYear(int year)
        {
            return (year >= 1900) && (year <= DateTime.UtcNow.Year + 2);
        }

        public static bool IsReasonableTitleOrAuthor(string source)
        {
            if (null == source) return false;

            string source_lower = source.Trim().ToLower();

            if (String.IsNullOrEmpty(source_lower))
            {
                return false;
            }

            if ("unknown" == source_lower)
            {
                return false;
            }

            if (source_lower.EndsWith(".pdf")) return false;
            if (source_lower.EndsWith(".ps")) return false;
            if (source_lower.EndsWith(".doc")) return false;
            if (source_lower.EndsWith(".docx")) return false;
            if (source_lower.EndsWith(".tex")) return false;
            if (source_lower.EndsWith(".dvi")) return false;
            if (source_lower.EndsWith(".wpd")) return false;
            if (source_lower.EndsWith(".rtf")) return false;

            if (200 < source.Length)
            {
                return false;
            }

            // Count the chars
            int numerator = 0;
            int denominator = 0;
            foreach (char c in source)
            {
                if (Char.IsLetter(c)) ++numerator;
                ++denominator;
            }

            // If more than enough are letters, use it!
            double ratio = numerator / (double)denominator;
            return ratio > 0.75;
        }
    }
}
