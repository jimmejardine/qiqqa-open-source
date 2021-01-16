using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities.Collections;
using Utilities.Strings;

namespace Utilities.Language.Buzzwords
{
    // Generates NGrams for AutoTags from any given input (generally that would be user-supplied or *inferred* document titles).
    public class BuzzwordGenerator
    {
        // words are assumed to be any set of 'word' characters, plus the hyphen, e.g.
        // 'anti-pattern' would thus be considered a single word.
        private static readonly Regex TRIM_CHARACTERS_RE = new Regex(@"[^\w_-]+");

        public static CountingDictionary<NGram> GenerateBuzzwords(IEnumerable<string> titles, List<string> words_blacklist, List<string> words_whitelist, bool perform_scrabble_filtration, bool skip_numbers = false, bool skip_acronyms = false)
        {
            List<string> titles_unique = RemoveDuplicates(titles);
            List<string> titles_lower = ToLowerCase(titles);
            titles_lower = RemoveDuplicates(titles_lower);
            CountingDictionary<NGram> repeated_ngrams = GenerateRepeatedNGrams(titles_lower, perform_scrabble_filtration, skip_numbers);

            // Combine the lists
            if (!skip_acronyms)
            {
                CountingDictionary<NGram> acronyms = GenerateAcronyms(titles_unique);

                foreach (var pair in acronyms)
                {
                    NGram ngram = new NGram(pair.Key.n, pair.Key.text, pair.Key.is_acronym);

                    if (!repeated_ngrams.ContainsKey(ngram))
                    {
                        repeated_ngrams.TallyN(ngram, pair.Value);
                    }
                    else
                    {
                        Logging.Info("Already there");
                    }
                }
            }

            // Add / remove the black/whitelists
            foreach (string word in words_whitelist)
            {
                NGram ngram = new NGram(1, word, false);
                repeated_ngrams.TallyOne(ngram);
            }
            foreach (string word in words_blacklist)
            {
                NGram ngram = new NGram(1, word, false);
                repeated_ngrams.Remove(ngram);
            }

            return repeated_ngrams;
        }

        private static CountingDictionary<NGram> GenerateAcronyms(List<string> titles)
        {
            CountingDictionary<NGram> acronyms = new CountingDictionary<NGram>();

            List<string> potential_acronyms = new List<string>();

            foreach (string title in titles)
            {
                potential_acronyms.Clear();

                // Ignore strings that are ALL upper case
                if (!StringTools.HasSomeLowerCase(title))
                {
                    continue;
                }

                // Ignore strings where the are not enough lowercase letters
                if (StringTools.LowerCasePercentage(title) < .50)
                {
                    continue;
                }

                string[] words = TRIM_CHARACTERS_RE.Split(title);
                foreach (string word in words)
                {
                    // Ignore single letter words
                    if (word.Length < 2)
                    {
                        continue;
                    }

                    // Ignore any words with a lowercase letter
                    if (StringTools.HasSomeLowerCase(word))
                    {
                        continue;
                    }

                    if (!StringTools.HasSomeUpperCase(word))
                    {
                        continue;
                    }

                    potential_acronyms.Add(word);
                }

                // IF too many of the words in the sentence are acronyms, this is a no-go
                if (potential_acronyms.Count > 0.3 * words.Length)
                {
                    continue;
                }

                potential_acronyms.ForEach(potential_acronym => acronyms.TallyOne(new NGram(1, potential_acronym, true)));
            }

            return acronyms;
        }

        private static CountingDictionary<NGram> GenerateRepeatedNGrams(List<string> titles, bool perform_scrabble_filtration, bool skip_numbers)
        {
            Logging.Info("Building the ngram dictionary");
            CountingDictionary<NGram> repetitions = new CountingDictionary<NGram>();
            foreach (string title in titles)
            {
                // Record each ngram present in the title
                List<NGram> ngrams = GetNGrams(title, skip_numbers);
                foreach (NGram ngram in ngrams)
                {
                    repetitions.TallyOne(ngram);
                }
            }

            Logging.Info("Built the raw ngram dictionary with {0} entries", repetitions.Count);

            repetitions = FilterInfrequent(repetitions);
            repetitions = FilterEnglishUniGrams(repetitions, perform_scrabble_filtration);
            repetitions = FilterStoppedNGrams(repetitions);
            repetitions = FilterSmallestUniAndBiGrams(repetitions);
            repetitions = FilterSingleLetterUnigrams(repetitions);
            repetitions = FilterSubNGrams(repetitions);
            repetitions = FilterNumbers(repetitions);

            Logging.Info("Final ngram dictionary has {0} entries", repetitions.Count);

            return repetitions;
        }

        private static List<string> ToLowerCase(IEnumerable<string> source)
        {
            List<string> results = new List<string>();
            foreach (string s in source)
            {
                results.Add(s.ToLower());
            }
            return results;
        }

        private static List<string> RemoveDuplicates(IEnumerable<string> source)
        {
            HashSet<string> results = new HashSet<string>(source);
            return new List<string>(results);
        }

        private static CountingDictionary<NGram> FilterSmallestUniAndBiGrams(CountingDictionary<NGram> repetitions)
        {
            CountingDictionary<NGram> repetitions1 = new CountingDictionary<NGram>();

            // Add in all the 3+ grams
            foreach (var pair in repetitions)
            {
                if (pair.Key.n > 2)
                {
                    repetitions1[pair.Key] = pair.Value;
                }
            }

            // Now for 1 and 2 grams, take only the top 25%
            repetitions1.AddRange(GetTopPercentageOfNGrams(1, repetitions));
            repetitions1.AddRange(GetTopPercentageOfNGrams(2, repetitions));

            return repetitions1;
        }

        private static CountingDictionary<NGram> GetTopPercentageOfNGrams(int n, CountingDictionary<NGram> repetitions)
        {
            List<int> counts = new List<int>();
            foreach (var pair in repetitions)
            {
                if (pair.Key.n == n)
                {
                    counts.Add(pair.Value);
                }
            }

            CountingDictionary<NGram> results = new CountingDictionary<NGram>();
            if (counts.Count > 0)
            {
                counts.Sort();
                int threshold = counts[(int)(0.75 * counts.Count)];
                foreach (var pair in repetitions)
                {
                    if (pair.Key.n == n && pair.Value > threshold)
                    {
                        results[pair.Key] = pair.Value;
                    }
                }
            }

            return results;
        }


        private static CountingDictionary<NGram> FilterInfrequent(CountingDictionary<NGram> repetitions)
        {
            CountingDictionary<NGram> repetitions1 = new CountingDictionary<NGram>();
            foreach (var pair in repetitions)
            {
                if (pair.Value > 1)
                {
                    repetitions1[pair.Key] = pair.Value;
                }
            }

            return repetitions1;
        }

        private static bool SplitStringAtSpacesAndMarkBadSubstrings_IsAcceptable(char source_char)
        {
            if (Char.IsLetterOrDigit(source_char)) return true;
            if ('-' == source_char) return true;
            return false;
        }

        private static List<string> SplitStringAtSpacesAndMarkBadSubstrings(string source)
        {
            List<string> results = new List<string>();

            if (!String.IsNullOrEmpty(source))
            {
                {
                    int i = 0;
                    int j = 0;
                    bool do_mark_bad = true;

                    while (i < source.Length)
                    {
                        // Are we still scanning happily?
                        if (j < source.Length && SplitStringAtSpacesAndMarkBadSubstrings_IsAcceptable(source[j]))
                        {
                            ++j;
                            continue;
                        }

                        // We are not happily scanning - is there something to extract?
                        if (i < j)
                        {
                            string substring = source.Substring(i, j - i);
                            results.Add(substring);
                            do_mark_bad = true;   // reset the 'bad word' marker
                            i = j;
                        }

                        // Process all the dirty characters until we reach the next nice character
                        while (i < source.Length && !SplitStringAtSpacesAndMarkBadSubstrings_IsAcceptable(source[i]))
                        {
                            // Skip over spaces, but record odd characters, unless we have already recorded them after the last good word
                            if (' ' == source[i] || !do_mark_bad)
                            {
                                ++i;
                                continue;
                            }
                            else
                            {
                                results.Add(null);
                                ++i;
                                continue;
                            }
                        }

                        // Start afresh from this good character
                        j = i;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the word n-grams from the string.  They are null if there are not enough words.
        /// </summary>
        /// <param name="source_string"></param>
        /// <returns></returns>
        private static List<NGram> GetNGrams(string source_string, bool skip_numbers)
        {
            List<string> words = SplitStringAtSpacesAndMarkBadSubstrings(source_string);

            List<NGram> ngrams = new List<NGram>();
            for (int i = 0; i < words.Count; ++i)
            {
                // Don't start ngram with a dud character
                if (String.IsNullOrEmpty(words[i]))
                {
                    continue;
                }

                StringBuilder sb = new StringBuilder();
                for (int j = i; j < words.Count; ++j)
                {
                    // Don't extend n-gram across dud character
                    if (String.IsNullOrEmpty(words[j]))
                    {
                        break;
                    }

                    // Don-t allow just - as the word
                    if ("-" == words[j])
                    {
                        break;
                    }

                    if (skip_numbers)
                    {
                        double dummy;
                        if (Double.TryParse(words[j], out dummy))
                        {
                            break;
                        }
                    }

                    // Don't allow n-grams longer than XXX words
                    int MAX_NGRAM_LENGTH = 4;
                    if (j >= i + MAX_NGRAM_LENGTH)
                    {
                        break;
                    }

                    // Add this word to the growing n-gram
                    if (0 != sb.Length)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(words[j]);

                    // Store this new n-gram
                    string ngram_word = sb.ToString();
                    ngrams.Add(new NGram(j - i + 1, ngram_word, false));
                }
            }

            return ngrams;
        }

        private static CountingDictionary<NGram> FilterStoppedNGrams(CountingDictionary<NGram> source_ngrams)
        {
            List<string> stop_words_both = new List<string>();
            List<string> stop_words_head = new List<string>();
            List<string> stop_words_tail = new List<string>();
            foreach (string stop_word in Stopwords.Instance.Words)
            {
                stop_words_both.Add(' ' + stop_word + ' ');
                stop_words_head.Add(stop_word + ' ');
                stop_words_tail.Add(' ' + stop_word);
            }

            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();
            foreach (var pair in source_ngrams)
            {
                bool is_bad = false;

                if (!is_bad)
                {
                    foreach (string stop_word in stop_words_head)
                    {
                        if (pair.Key.text.StartsWith(stop_word))
                        {
                            is_bad = true;
                            break;
                        }
                    }
                }
                if (!is_bad)
                {
                    foreach (string stop_word in stop_words_tail)
                    {
                        if (pair.Key.text.EndsWith(stop_word))
                        {
                            is_bad = true;
                            break;
                        }
                    }
                }
                if (!is_bad)
                {
                    foreach (string stop_word in stop_words_both)
                    {
                        if (pair.Key.text.Contains(stop_word))
                        {
                            is_bad = true;
                            break;
                        }
                    }
                }

                if (!is_bad)
                {
                    ngrams[pair.Key] = pair.Value;
                }
                else
                {
                    // Logging.Info("Dropping stopped ngram {0}", ngram.text);
                }
            }

            return ngrams;
        }

        private static readonly List<string> UNIGRAM_STOP_WORDS = new List<string>(new string[]
        {
            "", "a", "i", "ii", "iii", "iv", "v", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"
        });

        private static CountingDictionary<NGram> FilterEnglishUniGrams(CountingDictionary<NGram> source_ngrams, bool perform_scrabble_filtration)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();

            foreach (var pair in source_ngrams)
            {
                bool is_bad = false;

                if (perform_scrabble_filtration)
                {
                    if (1 == pair.Key.n)
                    {
                        is_bad = ScrabbleWords.Instance.Contains(pair.Key.text.ToLower());
                    }
                }

                if (UNIGRAM_STOP_WORDS.Contains(pair.Key.text))
                {
                    is_bad = true;
                }


                if (Stopwords.Instance.IsStopWord(pair.Key.text))
                {
                    is_bad = true;
                }

                if (!is_bad)
                {
                    ngrams[pair.Key] = pair.Value;
                }
            }

            return ngrams;
        }

        private static CountingDictionary<NGram> FilterSingleLetterUnigrams(CountingDictionary<NGram> source_ngrams)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();

            foreach (var pair in source_ngrams)
            {
                bool is_bad = false;

                if (1 == pair.Key.n)
                {
                    is_bad = (pair.Key.text.Length < 2);
                }

                if (!is_bad)
                {
                    ngrams[pair.Key] = pair.Value;
                }
            }

            return ngrams;
        }

        private static CountingDictionary<NGram> FilterNumbers(CountingDictionary<NGram> source_ngrams)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();

            foreach (var pair in source_ngrams)
            {
                bool is_bad = false;

                double dummy;
                if (Double.TryParse(pair.Key.text, out dummy))
                {
                    is_bad = true;
                }

                if (!is_bad)
                {
                    ngrams[pair.Key] = pair.Value;
                }
            }

            return ngrams;
        }

        private static CountingDictionary<NGram> FilterSubNGrams(CountingDictionary<NGram> source_ngrams)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();
            object ngrams_lock = new object();

            Parallel.ForEach(source_ngrams, ngram_sub =>
            //foreach (var ngram_sub in source_ngrams)
            {
                bool is_bad = false;

                string text_sub = " " + ngram_sub.Key.text + " ";

                foreach (var ngram_sup in source_ngrams)
                {
                    if (ngram_sub.Key == ngram_sup.Key)
                    {
                        continue;
                    }

                    string text_sup = " " + ngram_sup.Key.text + " ";

                    if (text_sup.Contains(text_sub))
                    {
                        if (ngram_sup.Value / (double)ngram_sub.Value > 0.65)
                        {
                            // Logging.Info("Dropping sub-ngram '{0}' as it is subsumed by '{1}'", ngram_sub.Key.text, ngram_sup.Key.text);
                            is_bad = true;
                            break;
                        }
                    }
                }

                if (!is_bad)
                {
                    // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                    lock (ngrams_lock)
                    {
                        // l1_clk.LockPerfTimerStop();
                        ngrams[ngram_sub.Key] = ngram_sub.Value;
                    }
                }
            }
            );

            return ngrams;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            List<string> splits = SplitStringAtSpacesAndMarkBadSubstrings("This is a test. And's then there was madder-schein.  There (2010) you have it {although not quite} me.");
        }
#endif

        #endregion
    }
}
