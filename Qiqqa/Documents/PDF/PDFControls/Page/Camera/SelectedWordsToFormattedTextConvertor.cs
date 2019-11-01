using System;
using System.Collections.Generic;
using System.Text;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Camera
{
    internal class SelectedWordsToFormattedTextConvertor
    {
        private static List<List<Word>> CreateRows(List<Word> words)
        {
            List<List<Word>> words_in_rows = new List<List<Word>>();


            // Start the algo in the top-right corner, so the next word is guaranteed to be a new line...
            List<Word> current_row = new List<Word>();
            double last_left = 1;
            double last_top = 0;

            foreach (var word in words)
            {
                if (word.Left < last_left && word.Top > last_top)
                {
                    current_row = new List<Word>();
                    words_in_rows.Add(current_row);
                }

                current_row.Add(word);
                last_left = word.Left;
                last_top = word.Top;
            }

            return words_in_rows;
        }

        private static string CreateSimpleText(List<List<Word>> words_in_rows, char separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var words_in_row in words_in_rows)
            {
                foreach (var word in words_in_row)
                {
                    sb.Append(word.Text);
                    sb.Append(separator);
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        private static string CreateSpacedText(List<List<Word>> words_in_rows)
        {
            return CreateSimpleText(words_in_rows, ' ');
        }

        private static string CreateTabbedText(List<List<Word>> words_in_rows)
        {
            return CreateSimpleText(words_in_rows, '\t');
        }



        private static int GetCommonWordCountPerRow(List<List<Word>> words_in_rows, int start, int end)
        {
            int common_length = words_in_rows[start].Count;
            for (int i = start + 1; i < end; ++i)
            {
                if (common_length != words_in_rows[i].Count) return -1;
            }

            return common_length;
        }

        private static int GetMinimumWordCountPerRow(List<List<Word>> words_in_rows, int start, int end)
        {
            int minimum_length = int.MaxValue;
            for (int i = start + 1; i < end; ++i)
            {
                minimum_length = Math.Min(minimum_length, words_in_rows[i].Count);
            }

            return minimum_length;
        }

        private static void CramIntoExactNumberOfWords(List<Word> words_in_row, int desired_word_count)
        {
            Word BLANK_WORD = new Word();
            BLANK_WORD.Text = "";

            // If there were too few words in the row, add some
            if (words_in_row.Count < desired_word_count)
            {
                while (words_in_row.Count < desired_word_count)
                {
                    words_in_row.Insert(0, BLANK_WORD);
                }
            }

            // If there were too many words in the row, cull some from the front
            if (words_in_row.Count > desired_word_count)
            {
                StringBuilder culled_words = new StringBuilder();
                while (words_in_row.Count > desired_word_count)
                {
                    culled_words.Append(words_in_row[0].Text);
                    culled_words.Append(' ');

                    words_in_row.RemoveAt(0);
                }

                words_in_row[0].Text = culled_words + words_in_row[0].Text;
            }
        }



        internal static string ConvertToParagraph(List<Word> words)
        {
            List<List<Word>> words_in_rows = CreateRows(words);
            return CreateSpacedText(words_in_rows);
        }


        internal static string ConvertToTable(List<Word> words)
        {
            List<List<Word>> words_in_rows = CreateRows(words);

            // If there are 0 or 1 rows, we can't do much...
            if (2 > words_in_rows.Count)
            {
                return CreateSpacedText(words_in_rows);
            }

            // If all rows have the same number of words, we are DONE!  WISHFUL!
            if (true)
            {
                int common_word_count = GetCommonWordCountPerRow(words_in_rows, 0, words_in_rows.Count);
                if (-1 != common_word_count)
                {
                    return CreateTabbedText(words_in_rows);
                }
            }

            // Perhaps the first row is lacking a few headers...
            if (true)
            {
                int common_word_count_for_non_headers = GetCommonWordCountPerRow(words_in_rows, 1, words_in_rows.Count);

                // If all the "non header data" has the same length, then smash up the first row till it matches...
                if (-1 != common_word_count_for_non_headers)
                {
                    CramIntoExactNumberOfWords(words_in_rows[0], common_word_count_for_non_headers);
                    return CreateTabbedText(words_in_rows);
                }
            }

            // If we have a lot of numbers in these words, then lets try to create a table by jamming the first few words together in each row to make the same number of "columns"
            if (true)
            {
                double number_percentage = GetPercentageOfNumbers(words);
                if (number_percentage > 0.1)
                {
                    int minimum_length = GetMinimumWordCountPerRow(words_in_rows, 1, words_in_rows.Count);
                    foreach (var words_in_row in words_in_rows)
                    {
                        CramIntoExactNumberOfWords(words_in_row, minimum_length);
                    }
                    return CreateTabbedText(words_in_rows);
                }
            }

            // If we get here, we have given up on any table formatting, so just output the text
            if (true)
            {
                return CreateSpacedText(words_in_rows);
            }
        }

        private static double GetPercentageOfNumbers(List<Word> words)
        {
            double number_tally = 0;
            foreach (var word in words)
            {
                double result;
                if (Double.TryParse(word.Text, out result))
                {
                    ++number_tally;
                }
            }

            return number_tally / words.Count;
        }
    }
}
