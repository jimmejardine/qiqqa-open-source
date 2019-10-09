using System;
using Utilities.Mathematics;

namespace Utilities.OCR
{
    public class ColumnWordOrderer
    {
        public static WordList ReorderWords(WordList original_word_list)
        {
            Average inter_word_gap_average = new Average();
            Average word_length_average = new Average();
            double last_happy_rightmost_text = Double.MaxValue;

            WordList words_ordered = new WordList();
            WordList words_still_to_process = new WordList();
            WordList words_deferred_till_next_time = new WordList();

            words_still_to_process.AddRange(original_word_list);

            while (words_still_to_process.Count > 0)
            {
                inter_word_gap_average.Reset();
                last_happy_rightmost_text = Double.MaxValue;                

                foreach (Word word in words_still_to_process)
                {
                    if (word.Text.Length < 20)
                    {
                        word_length_average.Add(word.Width);
                    }

                    if (inter_word_gap_average.Count > 3 && word.Left > last_happy_rightmost_text + 3 * inter_word_gap_average.Current)
                    {
                        words_deferred_till_next_time.Add(word);
                    }
                    else if (word_length_average.Count > 1 && word.Left > last_happy_rightmost_text + 2 * word_length_average.Current)
                    {
                        words_deferred_till_next_time.Add(word);
                    }
                    else
                    {
                        words_ordered.Add(word);

                        if (word.Left < last_happy_rightmost_text)
                        {
                            inter_word_gap_average.Reset();
                        }
                        else
                        {
                            inter_word_gap_average.Add(word.Left - last_happy_rightmost_text);
                        }

                        last_happy_rightmost_text = word.Left + word.Width;
                    }
                }

                words_still_to_process.Clear();
                Swap<WordList>.swap(ref words_still_to_process, ref words_deferred_till_next_time);

                if (words_still_to_process.Count > 0)
                {
                    Logging.Debug特("We have a multiple column situation with {0} words outstanding", words_still_to_process.Count);
                }
            }

            return words_ordered;
        }
    }
}
