using System.Collections.Generic;
using Utilities.Collections;
using Utilities.OCR;

namespace QiqqaOCR
{
    internal class WordListCredibility
    {
        public static WordListCredibility Instance = new WordListCredibility();
        private const int REASONABLE_WORD_LIST_LENGTH = 10;
        private HashSet<string> COMMON_WORDS;

        private WordListCredibility()
        {
            COMMON_WORDS = new HashSet<string>();

            // Such a eurocentric bastard am I...

            // english
            COMMON_WORDS.Add("the");
            COMMON_WORDS.Add("an");
            COMMON_WORDS.Add("and");
            COMMON_WORDS.Add("or");
            COMMON_WORDS.Add("but");

            // german
            COMMON_WORDS.Add("der");
            COMMON_WORDS.Add("die");
            COMMON_WORDS.Add("das");
            COMMON_WORDS.Add("ein");
            COMMON_WORDS.Add("eine");
            COMMON_WORDS.Add("und");
            COMMON_WORDS.Add("oder");
            COMMON_WORDS.Add("aber");

            // french
            COMMON_WORDS.Add("le");
            COMMON_WORDS.Add("la");
            COMMON_WORDS.Add("un");
            COMMON_WORDS.Add("une");
            COMMON_WORDS.Add("et");
            COMMON_WORDS.Add("ou");
            COMMON_WORDS.Add("mais");

            // spanish
            COMMON_WORDS.Add("el");
            COMMON_WORDS.Add("la");
            COMMON_WORDS.Add("las");
            COMMON_WORDS.Add("uno");
            COMMON_WORDS.Add("una");
            COMMON_WORDS.Add("i");
            COMMON_WORDS.Add("o");
            COMMON_WORDS.Add("pero");

            // italian
            COMMON_WORDS.Add("il");
            COMMON_WORDS.Add("la");
            COMMON_WORDS.Add("les");
            COMMON_WORDS.Add("uno");
            COMMON_WORDS.Add("una");
            COMMON_WORDS.Add("e");
            COMMON_WORDS.Add("o");
            COMMON_WORDS.Add("ma");

            // dutch
            COMMON_WORDS.Add("het");
            COMMON_WORDS.Add("een");
            COMMON_WORDS.Add("en");
            COMMON_WORDS.Add("of");
            COMMON_WORDS.Add("maar");
        }

        public bool IsACredibleWordList(WordList word_list)
        {
            if (null == word_list) return false;
            if (word_list.Count < REASONABLE_WORD_LIST_LENGTH) return false;
            if (HasSufficientCommonWords(word_list)) return true;
            if (HasSufficientRepeatedWords(word_list)) return true;

            return false;
        }

        internal bool IsACredibleWordList(Dictionary<int, WordList> word_lists)
        {
            // Look for at least one credible item in the list
            foreach (var pair in word_lists)
            {
                if (IsACredibleWordList(pair.Value))
                {
                    return true;
                }
            }

            // Nothing was credible
            return false;
        }


        private bool HasSufficientCommonWords(WordList word_list)
        {
            int COMMON_WORD_THRESHOLD = 2;

            HashSet<string> common_words_present = new HashSet<string>();

            foreach (var word in word_list)
            {
                string word_lower = word.Text.ToLower();
                if (COMMON_WORDS.Contains(word_lower))
                {
                    common_words_present.Add(word_lower);
                    if (common_words_present.Count >= COMMON_WORD_THRESHOLD)
                    {
                        break;
                    }
                }
            }

            return common_words_present.Count >= COMMON_WORD_THRESHOLD;
        }

        // Warning CA1822  The 'this' parameter(or 'Me' in Visual Basic) of 'WordListCredibility.HasSufficientRepeatedWords(WordList)' 
        // is never used.
        // Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, 
        // if appropriate.
        private static bool HasSufficientRepeatedWords(WordList word_list)
        {
            HashSet<string> viable_words = new HashSet<string>();

            CountingDictionary<string> word_counts = new CountingDictionary<string>();
            foreach (var word in word_list)
            {
                // Don't count single characters
                if (null == word.Text)
                {
                    continue;
                }
                // Don't count single characters
                if (word.Text.Length < 2)
                {
                    continue;
                }
                // Catch the series of ???????? that mupdf spits out
                if (word.Text.Trim('?').Length < 2)
                {
                    continue;
                }

                // Count the number of times we have seen this word
                string word_lower = word.Text.ToLower();
                word_counts.TallyOne(word_lower);

                // If we have seem the same words more than a few times, we like the list!
                if (word_counts[word_lower] > 3)
                {
                    viable_words.Add(word_lower);
                    if (viable_words.Count > 3)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
