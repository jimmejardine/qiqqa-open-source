using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Utilities.BibTex.Parsing
{
    /// <summary>
    /// Splits up a bibtex file into its component bits and calls a callback for each one
    /// </summary>
    public class BibTexLexer
    {
        string bibtex;

        // Named c for current_pos, but needs to be short because we will use it in a LOT of bibtex[c+i] expressions...
        readonly int MAX_C;
        int c;

        public BibTexLexer(string bibtex)
        {
            if (null == bibtex)
            {
                bibtex = "";
            }

            // Append and prepend some harmless whitespace at the end to help 
            // reduce the number of out-of-bounds exceptions in the parser/lexer
            // while keeping the code simple:
            bibtex = "\n" + bibtex + "\n\n\n";

            this.bibtex = bibtex;
            this.MAX_C = bibtex.Length - 3;
            this.c = 0;
        }

        public void Parse(BibTexLexerCallback callback)
        {
            ParseTopLevel(callback);
        }

        void ParseTopLevel(BibTexLexerCallback callback)
        {
            int prev_c = c;
            while (c < MAX_C)
            {
                // The `@keyword` must always be preceded by at least one whitespace
                // character so as to differentiate it from email addresses, etc.:
                // `joe@article` vs. ` @article`
                if ('@' != bibtex[c] || !Char.IsWhiteSpace(bibtex[c - 1]))
                {
                    ++c;
                }
                else
                {
                    // Hit a `@` BibTeX command start marker.

                    // Everything that we looped through thus far is either whitespace or comment or both:
                    string comment = bibtex.Substring(prev_c, c - prev_c).Trim();
                    if (!String.IsNullOrEmpty(comment))
                    {
                        callback.RaiseComment(comment);
                    }

                    // Get this @entry
                    ParseEntry(callback);

                    // Skip spaces between entries
                    ParseWhiteSpace();

                    prev_c = c;
                }
            }

            callback.RaiseFinished();
        }

        private void ParseEntry(BibTexLexerCallback callback)
        {
            // skip initial `@`
            ++c;

            // Get the entry name
            int entry_name_start = c;
            while (IsNameChar(bibtex[c]))
            {
                ++c;
            }
            int entry_name_end = c;

            string entry_name = bibtex.Substring(entry_name_start, entry_name_end - entry_name_start);

            // Check if it is a comment
            if (0 == "comment".CompareTo(entry_name.ToLower()))
            {
                ParseWhiteSpace();

                switch (bibtex[c])
                {
                    case '(':
                        callback.RaiseComment(ParseUntilDelim(callback, ')'));
                        return;

                    case '{':
                        callback.RaiseComment(ParseUntilDelim(callback, '}'));
                        return;

                    default:
                        // b0rked `@comment`: parse until EOL
                        callback.RaiseComment(ParseUntilDelim(callback, '\n'));
                        return;
                }
            }

            callback.RaiseEntryName(entry_name);

            // Check the length of the entry name
            if (entry_name_end == entry_name_start)
            {
                Exception(callback, "The BibTeX type is missing");
            }

            // Skip spaces between name and open brackets
            ParseWhiteSpace();

            // Now an entry is either { xxx } or ( xxx )
            switch (/* c < MAX_C ? */ bibtex[c])    // -- optimization
            {
                case '(':
                    ParseEntry_Delim(callback, '(', ')');
                    break;

                case '{':
                    ParseEntry_Delim(callback, '{', '}');
                    break;

                default:
                    Exception(callback, "Expecting a {0} or {1} to start a BibTeX reference", "(", "{{");
                    return;
            }
        }

        private void ParseEntry_Delim(BibTexLexerCallback callback, char delim_open, char delim_close)
        {
            // Check the beginning of this entry
            if (delim_open != bibtex[c])
            {
                Exception(callback, "Expecting a {0} to start a BibTeX reference", delim_open);
                return;
            }
            else
            {
                ++c;
            }

            // Parse whitespace
            ParseWhiteSpace();

            // Get the key
            ParseKey(callback);

            // Parse whitespace
            ParseWhiteSpace();

            // Then the comma after the key
            if (',' != bibtex[c])
            {
                Exception(callback, "Expecting a , after the BibTeX key");
                return;
            }
            else
            {
                ++c;
            }

            // Parse whitespace
            ParseWhiteSpace();

            ParseFields(callback, delim_close);

            // Check the end of this entry
            if (delim_close != bibtex[c])
            {
                Exception(callback, "Expecting a {0} to end this BibTeX reference", delim_close);
                return;
            }
            else
            {
                ++c;
            }
        }

        private void ParseKey(BibTexLexerCallback callback)
        {
            // Get the entry name
            int key_start = c;
            while (/* c < MAX_C && */ IsKeyChar(bibtex[c]))  // -- optimization
            {
                ++c;
            }
            int key_end = c;

            if (key_end == key_start)
            {
                callback.RaiseWarning("There is no key in this BibTeX reference");
            }
            
            string key = bibtex.Substring(key_start, key_end - key_start);

            callback.RaiseKey(key);
        }

        private List<string> field_values = new List<string>();

        private void ParseFields(BibTexLexerCallback callback, char delim_close)
        {
                            while (delim_close != bibtex[c])
                {
                    // Parse whitespace
                    ParseWhiteSpace();

                    // Get the name
                    if (ParseFieldName(callback))
                    {
                                            // Parse whitespace
                        ParseWhiteSpace();

                        // Get the equals
                        if ('=' != bibtex[c])
                        {
                            Exception(callback, "Expecting an = between the field name and the field value");
                            if (c >= MAX_C) break;  // abort as we've hit EOF prematurely
                            continue;
                        }
                        else
                        {
                            ++c;
                        }

                        field_values.Clear();
                        while (c < MAX_C)
                        {
                            // Parse whitespace
                            ParseWhiteSpace();

                            // Remember `c` as broken (and thus infinitely running) field values
                            // are a common problem in BibTeX records...
                            int value_start = c;

                            if (!ParseFieldValue(callback))
                            {
                                break;
                            }

                            // Parse whitespace
                            ParseWhiteSpace();

                            // Get an optional comma or `#` concatenation operator
                            switch (bibtex[c])
                            {
                                case ',':
                                    ++c;
                                    break;

                                case '#':
                                    // concatenation: keep adding to the field value!
                                    ++c;
                                    continue;

                                default:
                                    break;
                            }
                            break;
                        }

                        callback.RaiseFieldValue(field_values);
                        field_values.Clear();
                    }
                    else
                    {
                        if (c >= MAX_C)
                        {
                            Exception(callback, "Expecting a field name but nothing was acceptable until EOF: broken or truncated BibTeX?");
                            break;  // abort as we've hit EOF prematurely
                        }
                        else
                        {
                            Exception(callback, "Expecting a field name but nothing was acceptable, so moving onto next whitespace");
                            HuntForWhitespace();
                        }
                    }

                    // Parse whitespace
                    ParseWhiteSpace();
                            }
        }

        /// <summary>
        /// Returns true if it managed to find a field name
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private bool ParseFieldName(BibTexLexerCallback callback)
        {
            int field_name_start = c;
            while (IsNameChar(bibtex[c]))
            {
                ++c;
            }
            int field_name_end = c;

            if (field_name_end > field_name_start)
            {
                string field_name = bibtex.Substring(field_name_start, field_name_end - field_name_start);
                callback.RaiseFieldName(field_name);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ParseFieldValue(BibTexLexerCallback callback)
        {
            // Decide on the value type and parse it
            char ch = bibtex[c];
            if ('{' == ch)
            {
                return ParseFieldValue_Braces(callback);
            }
            else if ('"' == ch)
            {
                return ParseFieldValue_Quotes(callback);
            }
            else if (Char.IsLetterOrDigit(ch))
            {
                return ParseFieldValue_AlphaNumeric(callback);
            }
            else
            {
                Exception(callback, "A field value should start with '{0}' or '{1}' or digit", "(", "{{");
                return false;
            }
        }

        private bool ParseFieldValue_Quotes(BibTexLexerCallback callback)
        {
            // Skip our initial quote
            char quote = bibtex[c];

            ++c;

            int field_value_start = c;
            bool escape = false;
            while (c < MAX_C)
            {
                if ('\\' == bibtex[c] && '\\' == bibtex[c - 1] && escape)
                {
                    ++c;
                    escape = false;
                }
                else if ('\\' == bibtex[c])
                {
                    ++c;
                    escape = true;
                }
                else if (quote == bibtex[c] && ('\\' != bibtex[c - 1] || !escape))
                {
                    break;
                }
                else
                {
                    ++c;
                    escape = false;
                }
            }
            int field_value_end = c;
            string field_value = bibtex.Substring(field_value_start, field_value_end - field_value_start);

            // Check we have our final "
            if (quote != bibtex[c])
            {
                    Exception(callback, "Quotes field value should end with \\{0} instead of being terminated at EOF. Corrupted or truncated BibTeX?", quote);
                
                // see if we can backpedal to a first comma and take it from there, thus 'recovering' the damaged field value
                c = field_value_start;

                while (true)
                {
                    while (c < MAX_C && ',' != bibtex[c])
                    {
                        ++c;
                    }

                    if (c < MAX_C)
                    {
                        // Assuming we will parse a field name and an assignment next...
                        Regex re = new Regex(@"^\s*\w+\s*=\s*\S+", RegexOptions.Compiled);

                        if (re.IsMatch(bibtex.Substring(c + 1)))
                        {
                            // ... we now assume this comma ends this field
                            break;
                        }
                        ++c;
                    }
                    else
                    {
                        break;
                    }
                }

                if (c < MAX_C)
                {
                    field_value_end = c;
                    field_value = bibtex.Substring(field_value_start, field_value_end - field_value_start);

                    field_values.Add(field_value);
                }

                return false;
            }
            else
            {
                ++c;
            }

            field_values.Add(field_value);

            return true;
        }

        private bool ParseFieldValue_Braces(BibTexLexerCallback callback)
        {
            // Skip our initial {
            ++c;

            bool escape = false;
            int brace_depth = 0;
            int field_value_start = c;
            while (c < MAX_C)
            {
                if ('\\' == bibtex[c] && '\\' == bibtex[c - 1] && escape)
                {
                    ++c;
                    escape = false;
                }
                else if ('\\' == bibtex[c])
                {
                    ++c;
                    escape = true;
                }
                else if ('{' == bibtex[c] && ('\\' != bibtex[c - 1] || !escape))
                {
                    ++brace_depth;
                    ++c;
                }
                else if ('}' == bibtex[c] && ('\\' != bibtex[c - 1] || !escape))
                {
                    --brace_depth;

                    // Are we out of our delimiters yet?
                    if (0 <= brace_depth)
                    {
                        ++c;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    ++c;
                    escape = false;
                }
            }

            int field_value_end = c;
            string field_value = bibtex.Substring(field_value_start, field_value_end - field_value_start);

            // Check we have our final }
            if ('}' != bibtex[c])
            {
                Exception(callback, "Braces field value should end with {0}. Corrupted or truncated BibTeX?", "}}");

                // see if we can backpedal to a first comma and take it from there, thus 'recovering' the damaged field value
                c = field_value_start;

                while (true)
                {
                    while (c < MAX_C && ',' != bibtex[c])
                    {
                        ++c;
                    }

                    if (c < MAX_C)
                    {
                        // Assuming we will parse a field name and an assignment next...
                        Regex re = new Regex(@"^\s*\w+\s*=\s*\S+", RegexOptions.Compiled);

                        if (re.IsMatch(bibtex.Substring(c + 1)))
                        {
                            // ... we now assume this comma ends this field
                            break;
                        }
                        ++c;
                    }
                    else
                    {
                        break;
                    }
                }

                if (c < MAX_C)
                {
                    field_value_end = c;
                    field_value = bibtex.Substring(field_value_start, field_value_end - field_value_start);

                    field_values.Add(field_value);
                }
                return false;
            }
            else
            {
                ++c;
            }

            field_values.Add(field_value);

            return true;
        }

        private bool ParseFieldValue_AlphaNumeric(BibTexLexerCallback callback)
        {
            int field_value_start = c;
            while (c < MAX_C)
            {
                if (Char.IsLetterOrDigit(bibtex[c]))
                {
                    ++c;
                }
                else if ('.' == bibtex[c])
                {
                    ++c;
                }
                else if ('-' == bibtex[c])
                {
                    ++c;
                }
                else
                {
                    break;
                }
            }
            int field_value_end = c;
            string field_value = bibtex.Substring(field_value_start, field_value_end - field_value_start);

            if (c >= MAX_C)
            {
                // a field value MUST always sit inside a BibTeX record at least, hence we expect
                // at least one outer '}'. Therefore the field value cannot run up to EOF.
                Exception(callback, "Alphanumeric field value should not continue until EOF. Corrupted or truncated BibTeX?");
                return false;
            }

            field_values.Add(field_value);

            return true;
        }




        // ------------------------------------------------------------------------------------

        private void HuntForWhitespace()
        {
            while (c < MAX_C && !Char.IsWhiteSpace(bibtex[c]))
            {
                ++c;
            }
        }


        void ParseWhiteSpace()
        {
            while (c < MAX_C && Char.IsWhiteSpace(bibtex[c]))
            {
                ++c;
            }
        }

        private string ParseUntilDelim(BibTexLexerCallback callback, char delim)
        {
            int start = c;
            while (c < MAX_C && delim != bibtex[c])
            {
                ++c;
            }

            if (c >= MAX_C)
            {
                Exception(callback, "Could not find delim: {0}", delim);
            }

            return bibtex.Substring(start, c - start);
        }


        // ------------------------------------------------------------------------------------

        private static bool IsNameChar(char c)
        {
            if (Char.IsLetterOrDigit(c)) return true;
            if ('-' == c) return true;
            if ('_' == c) return true;
            return false;
        }

        private static bool IsKeyChar(char c)
        {
            if (Char.IsLetterOrDigit(c)) return true;
            if ('_' == c) return true;
            if ('.' == c) return true;
            if ('-' == c) return true;
            if ('+' == c) return true;
            if (':' == c) return true;
            if ('/' == c) return true;
            //if (' ' == c) return true;
            if ('?' == c) return true;
            return false;
        }

        public static string StripNonKeyChars(string key, string replacement)
        {
            StringBuilder sb = new StringBuilder();
            // Only replace first in a series of non-key chars
            // and then only IFF followed by at least one key-char
            // AND IFF preceded by at least one key-char!
            //
            // This prevents replacements at start or end of a 'key'.
            int replace_state = 0;              
            foreach (char c in key)
            {
                if (IsKeyChar(c))
                {
                    replace_state++;
                    if (0 == replace_state)
                    {
                        sb.Append(replacement);
                        replace_state = 1;
                    }
                    sb.Append(c);
                }
                else if (replace_state > 0)
                {
                    replace_state = -1;
                }
            }

            return sb.ToString();
        }

        // ------------------------------------------------------------------------------------

        private void Exception(BibTexLexerCallback callback, string p, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(p, args);
            sb.AppendLine();

            // Build up some context
            int run_from = Math.Max(0, c - 40);
            int run_to = Math.Min(MAX_C, c + 40);
            for (int i = run_from; i < run_to; ++i)
            {
                if (Char.IsWhiteSpace(bibtex[i]))
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(bibtex[i]);
                }
            }
            sb.AppendLine();
            for (int i = run_from; i < run_to; ++i)
            {
                if (i == c)
                {
                    sb.Append('^');
                }
                else
                {
                    sb.Append(' ');
                }

            }
            sb.AppendLine();

            string msg = sb.ToString();
            msg = msg.Trim();
            callback.RaiseException(msg);
        }
    }
}
