using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.Strings
{
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


        /// <summary>
        /// Strange name for what this does, which is StripToLettersAndDigits...
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StripToASCII(string source)
        {
            return StripToLettersAndDigits(source);
        }

        private static HashSet<char> VOWELS = new HashSet<char> { 'A', 'E', 'I', 'O', 'U', 'a', 'e', 'i', 'o', 'u' };
        public static string StripVowels(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (!VOWELS.Contains(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        public static string StripToLettersAndDigits(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (Char.IsLetterOrDigit(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        public static List<String> Split_NotInDelims(string source, char delim_open, char delim_close, string split_chars)
        {
            return Split_NotInDelims(source, delim_open, delim_close, split_chars, StringSplitOptions.None);
        }

        public static List<String> Split_NotInDelims(string source, char delim_open, char delim_close, string split_chars, StringSplitOptions split_options, int max_items = 0)
        {
            List<String> result = new List<string>();

            if (!String.IsNullOrEmpty(source))
            {
                int depth = 0;
                int source_length = source.Length;
                int start_pos = 0;
                for (int i = 0; i < source_length; ++i)
                {
                    // What kind of delimiters are we using?  If they are the same, toggle depth, otherwise count depth.
                    if (delim_open == delim_close)
                    {
                        // Toggle the depth
                        if (delim_open == source[i])
                        {
                            depth = 1 - depth;
                        }
                    }
                    else
                    {
                        // Increase or decrease the depth
                        if (delim_open == source[i])
                        {
                            ++depth;
                        }
                        if (delim_close == source[i])
                        {
                            --depth;
                        }
                    }

                    if (0 == depth && split_chars.Contains(source.Substring(i, 1)))
                    {
                        if (i - start_pos > 0 || split_options == StringSplitOptions.None)
                        {
                            result.Add(source.Substring(start_pos, i - start_pos));
                            if (0 != max_items && max_items <= result.Count) break;
                        }

                        start_pos = i + 1;
                    }
                }

                // Add in the last item
                if (start_pos < source_length)
                {
                    if (source_length - start_pos > 0 || split_options == StringSplitOptions.None)
                    {
                        if (0 == max_items || max_items > result.Count)
                        {
                            result.Add(source.Substring(start_pos, source_length - start_pos));
                        }
                    }
                }
            }

            // Trim the start and end characters from each item
            for (int i = 0; i < result.Count; ++i)
            {
                result[i] = result[i].TrimStart(delim_open).TrimEnd(delim_close);
            }

            return result;
        }

        public static string PrettyPrint(params object[] vals)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var val in vals)
            {
                sb.AppendFormat("{0}\t", val);
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

        public static int getCharacterPositionOfRow(int row, string[] lines)
        {
            int position = 0;
            for (int i = 0; i < row; ++i)
            {
                position += lines[i].Length;
                position += 2;  //CRLF
            }

            return position;
        }

        public static int CharCount(string source, char key)
        {
            int total = 0;
            for (int i = 0; i < source.Length; ++i)
            {
                if (source[i] == key) ++total;
            }
            return total;
        }

        public static int CountStringOccurence(string source, string key)
        {
            string key_escaped = "\\b" + Regex.Escape(key) + "\\b";
            MatchCollection match_collection = Regex.Matches(source, key_escaped);
            return match_collection.Count;
        }

        /// <summary>
        /// Count the number of times KEY appears in the SOURCE string, but NOT as the substring of another word...
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int CountWordOccurence(string source, string key)
        {
            string key_escaped = @"\b" + Regex.Escape(key) + @"\b";
            MatchCollection match_collection = Regex.Matches(source, key_escaped);
            return match_collection.Count;
        }

        public static void ChangeLfToCrLf(ref String buf)
        {
            byte[] abuf = new byte[256000];
            int abuf_index = 0;
            ASCIIEncoding ASCII = new ASCIIEncoding();

            int crCount = buf.IndexOf("\r", 0);
            if (crCount == -1)
            {    // No carriage returns
                buf = buf.Replace("\x0a", "\x0d\x0a");
                return;
            }
            int bufsize = buf.Length;
            for (int i = 0; i < bufsize - 1; i++)
            {
                char ch = buf[i];
                char nextch = buf[i + 1];
                if (ch == '\n')
                {
                    if (nextch != '\r')
                    {
                        abuf[abuf_index++] = (byte)'\r';
                        abuf[abuf_index++] = (byte)'\n';
                    }
                    else abuf[abuf_index++] = (byte)'\n';
                }
                else abuf[abuf_index++] = (byte)ch;
            }
            buf = ASCII.GetString((byte[])abuf);
        }


        /// <summary>
        /// Finds the first occurrence of the target character that is not inside nested braces
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int IndexOf_NotInBraces(char value, string target)
        {
            return IndexOf_NotInBraces(value, target, 0);
        }

        /// <summary>
        /// Finds the first occurrence of the target character that is not inside nested braces, starting after the selected position.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="start_pos"></param>
        /// <returns></returns>

        public static int IndexOf_NotInBraces(char value, string target, int start_pos)
        {
            return IndexOf_NotInDelims(value, target, start_pos, '{', '}');
        }

        public static int IndexOf_NotInDelims(char value, string target, int start_pos, char delim_open, char delim_close)
        {
            int depth = 0;

            for (int i = start_pos; i < target.Length; ++i)
            {
                // Check our depth
                if (target[i] == delim_open)
                {
                    ++depth;
                }
                else if (target[i] == delim_close)
                {
                    --depth;
                }

                // Do we have a match
                if (0 == depth && target[i] == value)
                {
                    return i;
                }
            }

            // No match
            return -1;
        }

        public static int IndexOf_NotInDelim(char value, string target, int start_pos, char delim)
        {
            bool inside_delim = false;

            for (int i = start_pos; i < target.Length; ++i)
            {
                // Check our depth
                if (target[i] == delim)
                {
                    inside_delim = !inside_delim;
                }

                // Do we have a match
                if (!inside_delim && target[i] == value)
                {
                    return i;
                }
            }

            // No match
            return -1;
        }

        public static bool StartsWithCapital(string word)
        {
            if (String.IsNullOrEmpty(word)) return false;
            return word[0] >= 'A' && word[0] <= 'Z';
        }

        public static bool StartsWithLowerCase(string word)
        {
            if (String.IsNullOrEmpty(word)) return false;
            return word[0] >= 'a' && word[0] <= 'z';
        }

        public static double LewensteinSimilarity(string s, string t)
        {
            return 1 - LewensteinDistance(s, t) / (double)Math.Max(s.Length, t.Length);
        }

        /// <summary>
        /// Returns the lewenstein distance between the two strings.
        /// 0 if they are identical, up to a maximum of max(len(s),len(t))
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int LewensteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static bool HasSomeLowerCase(string title)
        {
            for (int i = title.Length - 1; i >= 0; --i)
            {
                if (Char.IsLower(title, i)) return true;
            }

            return false;
        }

        public static double LowerCasePercentage(string title)
        {
            int num_lower = 0;
            int num_total = 0;

            for (int i = title.Length - 1; i >= 0; --i)
            {
                if (Char.IsLetter(title, i))
                {
                    if (Char.IsLower(title, i))
                    {
                        ++num_lower;
                    }

                    ++num_total;
                }
            }

            return num_total > 0 ? num_lower / (double)num_total : 0;
        }


        public static bool HasSomeUpperCase(string title)
        {
            for (int i = 0; i < title.Length; ++i)
            {
                if (Char.IsUpper(title, i)) return true;
            }

            return false;
        }

        public static string StripCrLf(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            str = str.Replace("\r\n", " ");
            str = str.Replace("\r", " ");
            str = str.Replace("\n", " ");
            return str;
        }

        private static readonly Regex sWhitespace = new Regex(@"[\s\r\n]+");

        public static string StripWhitespace(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            return sWhitespace.Replace(str, String.Empty);
        }

        public static string ReplaceWhitespace(string str, string replacement)
        {
            if (String.IsNullOrEmpty(str)) return str;
            return sWhitespace.Replace(str, replacement);
        }

        public static string Sanitize(string str)
        {
            if (String.IsNullOrEmpty(str)) return str;
            str = str.Normalize().Trim();
            return ReplaceWhitespace(str, " ");
        }

        public static string StripHtmlTags(string str, string replacement)
        {
            if (String.IsNullOrEmpty(str)) return str;
            return Regex.Replace(str, @"<[^>]*>", replacement);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test_CountWordOccurence()
        {
            int string_count = CountStringOccurence("at father that cat 'at' threw an attack at.", "at");
            int word_count = CountWordOccurence("at father that cat 'at' threw an attack at.", "at");
        }
#endif

        #endregion

        public static string Reverse(string src)
        {
            char[] chars = src.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
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

        public static string GetFirstWord(string source)
        {
            if (String.IsNullOrEmpty(source)) return "";
            char[] WHITESPACE = { ' ', '\t', '\n' };
            string[] words = source.Split(WHITESPACE);

            foreach (string w in words)
            {
                bool is_word = true;
                foreach (char c in w)
                {
                    if (!Char.IsLetter(c))
                    {
                        is_word = false;
                        break;
                    }
                }
                if (is_word)
                {
                    return w;
                }
            }
            return "";
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
}
