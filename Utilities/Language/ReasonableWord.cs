using System;

namespace Utilities.Language
{
    public class ReasonableWord
    {
        private static readonly char[] keyword_polish_characters = new char[]
        {
            '"', '\'', '.', ':', ';', '!', '?', ',', '[', ']', '{', '}', '(', ')'
        };

        /// <summary>
        /// Returns a polished keyword if it is reasonable to start with (e.g. [james-22] becomes james-22).
        /// Returns null if the keyword is too bad (e.g. dfsdf£$%&^$).
        /// </summary>
        /// <param name="source_keyword"></param>
        /// <returns></returns>
        public static string MakeReasonableWord(string source_keyword)
        {
            source_keyword = source_keyword.Trim(keyword_polish_characters);
            source_keyword = source_keyword.ToLower();

            // We dont like loooong keywords (generally they are noise in the source data)
            if (source_keyword.Length > 20)
            {
                return null;
            }

            // Only accepts keywords that are letters, numbers, dash, quote, underscore (aka "the programmers' space")
            bool has_at_least_one_useful_char = false;
            for (int i = 0; i < source_keyword.Length; ++i)
            {
                if (
                    true
                    && !(Char.IsLetterOrDigit(source_keyword, i))
                    && !(Char.IsNumber(source_keyword, i))
                    && !(source_keyword[i] == '_')
                    && !(source_keyword[i] == '-')
                    && !(source_keyword[i] == '.')
                    && !(source_keyword[i] == ' ')
                    && !(source_keyword[i] == '\'')
                    )
                {
                    return null;
                }
                
                if (!has_at_least_one_useful_char)
                {
                    if (Char.IsLetter(source_keyword, i) || Char.IsNumber(source_keyword, i))
                    {
                        has_at_least_one_useful_char = true;
                    }
                }
            }

            // There must be at least one character in the word (we dont want long sequences of dashes, etc)
            if (!has_at_least_one_useful_char)
            {
                return null;
            }

            return source_keyword;
        }
    }
}
