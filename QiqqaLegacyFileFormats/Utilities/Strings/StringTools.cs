using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;




namespace QiqqaLegacyFileFormats          // namespace Utilities.Strings
{

#if SAMPLE_LOAD_CODE

    public static class StringTools
    {
        private static readonly string[] SPLIT_SENTENCE = new string[] { ". " };

        public static string[] SpitIntoSentences(string paragraph)
        {
            return paragraph.Split(SPLIT_SENTENCE, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string TrimStart(string source, string victim)
        {
            if (source.StartsWith(victim))
            {
                return source.Substring(victim.Length);
            }
            else
            {
                return source;
            }
        }

        private static readonly char[] SPLIT_TAB = new char[] { '\t' };
        public static string[] SplitAtTabs(string line)
        {
            return line.Split(SPLIT_TAB);
        }

        public static string TrimEnd(string source, string victim)
        {
            if (source.EndsWith(victim))
            {
                return source.Substring(0, source.Length - victim.Length);
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Returns the concatenated list of strings separated by separator
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="separator"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static string ConcatenateStrings(IEnumerable<string> strings, string separator = ",", int from = 0, int to_exclusive = int.MaxValue)
        {
            if (null == strings)
            {
                return "";
            }

            // If there are zero, one or multiple items
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int j = 0;
            foreach (string s in strings)
            {
                if (i < from) continue;
                if (i >= to_exclusive) break;
                if (j > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(s);
                i++;
                j++;
            }

            return sb.ToString();
        }

        public static string StringFromByteArray(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte d in data)
            {
                if (0 != d)
                {
                    sb.Append((char)d);
                }
            }

            return sb.ToString();
        }



        public static StringArray splitAtNewline(string source)
        {
            StringArray result = new StringArray();
            int source_length = source.Length;
            int start_pos = 0;
            for (int i = 0; i < source_length; ++i)
            {
                // If we find a \r or \n, chop out the string
                if ('\r' == source[i])
                {
                    result.Add(source.Substring(start_pos, i - start_pos));
                    start_pos = i + 1;

                    // If \r is followed by \n ignore the \n
                    if (start_pos < source_length && '\n' == source[start_pos])
                    {
                        ++i;
                        ++start_pos;
                    }
                }

                else if ('\n' == source[i])
                {
                    result.Add(source.Substring(start_pos, i - start_pos));
                    start_pos = i + 1;
                }
            }

            // Add in the last item
            if (start_pos < source_length)
            {
                result.Add(source.Substring(start_pos, source_length - start_pos));
            }

            return result;
        }

        public static string StripAfterFirstInstanceOfChar(string str, char chr)
        {
            int blank_pos = str.IndexOf(chr);
            if (-1 != blank_pos)
            {
                str = str.Substring(0, blank_pos);
            }

            return str;
        }

        public static string StripAfterFirstInstanceOfString(string str, string chr, bool include_string)
        {
            int blank_pos = str.IndexOf(chr);
            if (-1 != blank_pos)
            {
                str = str.Substring(0, blank_pos + (include_string ? chr.Length : 0));
            }

            return str;
        }

        private static readonly Regex multi_ws_re = new Regex(@"[\s\r\n]+");

        /// <summary>
        /// Cleans up all whitespace in the given string. 
        /// Double spaces, tabs, etc. are converted to single spaces.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimAndClean(string str)
        {
            if (null == str) return str;
            str = multi_ws_re.Replace(str, " ");
            return str.Trim();
        }

        public static string TrimToLengthWithEllipsis(string str, int max_length = 66)
        {
            if (null == str) return str;
            str = TrimAndClean(str);
            int len = str.Length;
            max_length = Math.Max(5, max_length);  // sane lower limit?

            if (len > max_length)
            {
                // heuristic: when there's a word boundary within last 20% slots, take that one instead: 
                int range_to_check = Math.Max(2, max_length / 5);
                // account for the Unicode ellipsis character
                int pos = str.LastIndexOf(' ', max_length - 1, range_to_check);
                if (pos > 0 && pos >= (2 * max_length) / 3)    // heuristic: if the last space is too far back (2/3rd of output width), ignore it
                {
                    // soft cut: cut just past the space = word boundary
                    str = str.Substring(0, pos + 1);
                }
                else
                {
                    // hard cut
                    str = str.Substring(0, max_length - 1);
                }
                str += "…";
            }
            return str;
        }

        public static string PagesSetAsString(HashSet<int> pageSet)
        {
            var lst = pageSet.ToArray();
            Array.Sort(lst);
            List<string> pageRanges = new List<string>();

            // now that we've a sorted list of pages, determine the continuous ranges in that set:
            if (lst.Count() > 0)
            {
                int startPage = lst[0];
                int endPage = lst[0];
                for (var i = 1; i < lst.Count(); i++)
                {
                    int page = lst[i];
                    if (endPage + 1 == page)
                    {
                        endPage++;
                        continue;
                    }
                    else
                    {
                        // continuity has been disrupted. Store the range and begin a fresh one:
                        pageRanges.Add(startPage == endPage ? $"{startPage}" : $"{startPage}-{endPage}");
                        startPage = endPage = page;
                    }
                }
                // To finish up, push the last range we were working on when we hit the end of the list:
                pageRanges.Add(startPage == endPage ? $"{startPage}" : $"{startPage}-{endPage}");

                return String.Join(",", pageRanges);
            }

            // nothing --> empty string
            return "";
        }
    }

#endif

}


