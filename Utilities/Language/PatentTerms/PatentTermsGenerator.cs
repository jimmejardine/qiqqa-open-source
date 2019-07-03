using System;
using System.Collections.Generic;

namespace Utilities.Language.PatentTerms
{
    public class PatentTermsGenerator
    {
        private static readonly char[] SPACE_SPLITTER = new char[] { ' ' };

        public static List<string> ExtractCandidateTermsFromTitle(string title)
        {
            List<string> candidate_terms = new List<string>();

            string[] words = title.Split(SPACE_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            if (1 == words.Length && !String.IsNullOrEmpty(words[0]))
            {
                candidate_terms.Add(words[0]);
            }
            else
            {
                // Build up a series of terms from the component words.  
                // We require at least 2 words in a term...
                const int MAX_TERM_LENGTH = 6;
                for (int i = 0; i < words.Length; ++i)
                {
                    // Don't allow terms to start with a stop word
                    if (IsStopWord_Opening(words[i])) continue;

                    // Don't allow terms to start with a comma-terminated word
                    if (words[i].EndsWith(",")) continue;

                    string growing_term = words[i];

                    for (int j = i + 1; j < words.Length && j < i + MAX_TERM_LENGTH; ++j)
                    {
                        string word_j = words[j];

                        // We can't keep globbing after a comma, so if we have done, break out
                        bool must_terminate_after_this_word = false;
                        if (word_j.EndsWith(","))
                        {
                            must_terminate_after_this_word = true;
                            word_j = word_j.TrimEnd(',');
                        }

                        growing_term = growing_term + ' ' + word_j;

                        // We won't store any terms ending in a stopword
                        if (!IsStopWord_Closing(word_j))
                        {
                            candidate_terms.Add(growing_term);
                        }

                        if (must_terminate_after_this_word)
                        {
                            break;
                        }
                    }
                }
            }

            return candidate_terms;
        }


        private static bool IsStopWord(string word)
        {
            if ("&" == word) return true;
            if ("a" == word) return true;
            if ("be" == word) return true;
            if ("an" == word) return true;
            if ("at" == word) return true;
            if ("of" == word) return true;
            if ("in" == word) return true;
            if ("is" == word) return true;
            if ("as" == word) return true;
            if ("am" == word) return true;
            if ("or" == word) return true;
            if ("by" == word) return true;
            if ("to" == word) return true;
            if ("on" == word) return true;
            if ("via" == word) return true;
            if ("use" == word) return true;
            if ("are" == word) return true;
            if ("its" == word) return true;
            if ("can" == word) return true;
            if ("the" == word) return true;
            if ("one" == word) return true;
            if ("for" == word) return true;
            if ("and" == word) return true;
            if ("with" == word) return true;
            if ("used" == word) return true;
            if ("such" == word) return true;
            if ("from" == word) return true;
            if ("both" == word) return true;
            if ("same" == word) return true;
            if ("when" == word) return true;
            if ("into" == word) return true;
            if ("than" == word) return true;
            if ("then" == word) return true;
            if ("also" == word) return true;
            if ("that" == word) return true;
            if ("more" == word) return true;
            if ("less" == word) return true;
            if ("each" == word) return true;
            if ("made" == word) return true;
            if ("their" == word) return true;
            if ("apply" == word) return true;
            if ("among" == word) return true;
            if ("other" == word) return true;
            if ("along" == word) return true;
            if ("which" == word) return true;
            if ("after" == word) return true;
            if ("using" == word) return true;
            if ("before" == word) return true;
            if ("during" == word) return true;
            if ("and/or" == word) return true;
            if ("having" == word) return true;
            if ("within" == word) return true;
            if ("cannot" == word) return true;
            if ("common" == word) return true;
            if ("between" == word) return true;
            if ("without" == word) return true;
            if ("through" == word) return true;
            if ("another" == word) return true;
            if ("thereof" == word) return true;
            if ("thereto" == word) return true;
            if ("capable" == word) return true;
            if ("thereby" == word) return true;
            if ("causing" == word) return true;
            if ("therefor" == word) return true;
            if ("applying" == word) return true;
            if ("therefore" == word) return true;
            if ("including" == word) return true;
            if ("utilizing" == word) return true;
            if ("comprising" == word) return true;

            if ("top" == word) return true;
            if ("bottom" == word) return true;
            if ("back" == word) return true;
            if ("front" == word) return true;
            if ("center" == word) return true;
            if ("left" == word) return true;
            if ("right" == word) return true;

            return false;
        }

        private static bool IsStopWord_Opening(string word)
        {
            if ("0" == word) return true;
            if ("1" == word) return true;
            if ("2" == word) return true;
            if ("3" == word) return true;
            if ("4" == word) return true;
            if ("5" == word) return true;
            if ("6" == word) return true;
            if ("7" == word) return true;
            if ("8" == word) return true;
            if ("9" == word) return true;

            return IsStopWord(word);
        }

        private static bool IsStopWord_Closing(string word)
        {
            bool is_stop = IsStopWord(word);
            if (is_stop) return true;

            if ("over" == word) return true;
            if ("under" == word) return true;
            if ("named" == word) return true;
            if ("producing" == word) return true;
            if ("varying" == word) return true;

            if (word.EndsWith("'s")) return true;

            return false;

        }
    }
}
