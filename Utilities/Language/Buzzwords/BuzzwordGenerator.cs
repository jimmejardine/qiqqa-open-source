using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Utilities.Collections;
using Utilities.Strings;

namespace Utilities.Language.Buzzwords
{
    public class BuzzwordGenerator
    {
        static readonly char[] TRIM_CHARACTERS = new char[]
        {
            ' ','\'', ',', ':', ':', '.', '(', ')', '[', ']', '{', '}', '\'', '"'
        };

        public static readonly List<string> EMPTY_LIST = new List<string>();

        public static CountingDictionary<NGram> GenerateBuzzwords(IEnumerable<string> titles, bool perform_scrabble_filtration)
        {
            return GenerateBuzzwords(titles, EMPTY_LIST, EMPTY_LIST, perform_scrabble_filtration);
        }

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
                
                string[] words = title.Split(TRIM_CHARACTERS);
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
                results.Add(s.ToLower(CultureInfo.CurrentCulture));
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
                if (false) { }
                else if (pair.Value > 1)
                {
                    repetitions1[pair.Key] = pair.Value;
                }
            }

            return repetitions1;
        }

        static bool SplitStringAtSpacesAndMarkBadSubstrings_IsAcceptable(char source_char)
        {
            if (Char.IsLetterOrDigit(source_char)) return true;
            if ('-' == source_char) return true;
            return false;
        }

        static List<string> SplitStringAtSpacesAndMarkBadSubstrings(string source)
        {
            List<string> results = new List<string>();

            if (!String.IsNullOrEmpty(source))
            {
                {
                    int i = 0;
                    int j = 0;
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
                            i = j;
                        }

                        // Process all the dirty characters until we reach the next nice character
                        while (i < source.Length && !SplitStringAtSpacesAndMarkBadSubstrings_IsAcceptable(source[i]))
                        {
                            // Skip over spaces, but record odd characters
                            if (' ' == source[i])
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

                // Now remove consecutive nulls from the list
                {
                    for (int i = 0; i < results.Count - 1; ++i)
                    {
                        while (null == results[i] && i < results.Count - 1 && null == results[i + 1])
                        {
                            results.RemoveAt(i);
                        }
                    }
                }
            }

            return results;
        }
        
        /// <summary>
        /// Gets the first n word n-grams from the string.  They are null if there are not enough words.
        /// </summary>
        /// <param name="source_string"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        static List<NGram> GetNGrams(string source_string, bool skip_numbers)
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
        
        static List<NGram> GetNGrams_OLD(string source_string)
        {
            source_string = source_string.ToLower();

            List<int> space_positions = new List<int>();
            space_positions.Add(-1);
            for (int i = 0; i < source_string.Length; ++i)
            {
                if (' ' == source_string[i])
                {
                    space_positions.Add(i);
                }
            }
            space_positions.Add(source_string.Length);

            List<NGram> ngrams = new List<NGram>();
            for (int i = 0; i < space_positions.Count; ++i)
            {
                for (int j = i + 1; j < space_positions.Count; ++j)
                {
                    string ngram = source_string.Substring(space_positions[i] + 1, space_positions[j] - space_positions[i] - 1);
                    ngram = ngram.Trim(TRIM_CHARACTERS);
                    ngrams.Add(new NGram(j - i, ngram, false));
                }
            }

            return ngrams;
        }

        static CountingDictionary<NGram> FilterStoppedNGrams(CountingDictionary<NGram> source_ngrams)
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

        static readonly List<string> UNIGRAM_STOP_WORDS = new List<string>(new string[]
        {
            "", "a", "i", "ii", "iii", "iv", "v", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"
        });
        
        static CountingDictionary<NGram> FilterEnglishUniGrams(CountingDictionary<NGram> source_ngrams, bool perform_scrabble_filtration)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();

            foreach (var pair in source_ngrams)
            {
                bool is_bad = false;

                if (perform_scrabble_filtration)
                {
                    if (1 == pair.Key.n)
                    {
                        is_bad = ScrabbleWords.Instance.Contains(pair.Key.text.ToLower(CultureInfo.CurrentCulture));
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

        static CountingDictionary<NGram> FilterNumbers(CountingDictionary<NGram> source_ngrams)
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
        
        static CountingDictionary<NGram> FilterSubNGrams(CountingDictionary<NGram> source_ngrams)
        {
            CountingDictionary<NGram> ngrams = new CountingDictionary<NGram>();

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
                        if (ngram_sup.Value / (double) ngram_sub.Value > 0.65)
                        {
                            // Logging.Info("Dropping sub-ngram '{0}' as it is subsumed by '{1}'", ngram_sub.Key.text, ngram_sup.Key.text);
                            is_bad = true;
                            break;
                        }
                    }
                }

                if (!is_bad)
                {
                    lock (ngrams)
                    {
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
