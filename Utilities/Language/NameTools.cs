using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Strings;

namespace Utilities.Language
{
    public class NameTools
    {
        public class Name
        {
            public string last_name;
            public string first_names;

            public string Initials
            {
                get
                {
                    if (String.IsNullOrEmpty(first_names))
                    {
                        return "";
                    }

                    StringBuilder sb = new StringBuilder();
                    
                    // Extract all the upper case letters
                    foreach (char c in first_names)
                    {
                        if (Char.IsUpper(c))
                        {
                            sb.Append(c);
                        }
                    }

                    // If we got no initials, use the first letter
                    if (0 == sb.Length)
                    {
                        sb.Append(Char.ToUpper(first_names[0]));
                    }


                    return sb.ToString();
                }
            }

            /// <summary>
            /// Returns 
            ///     Jardine JG
            /// </summary>
            public string LastName_Initials
            {
                get
                {
                    return (last_name + " " + Initials).Trim();
                }
            }

            public static readonly Name UNKNOWN_NAME = new Name { last_name = "UNKNOWN", first_names = "" };
        }

        public static readonly string UNKNOWN_AUTHORS = "(unknown authors)";


        /// <summary>
        /// Splits up a string of authors into the component authors
        /// Accepts:
        ///   XYZ and YYY and ZZZ
        ///   XYZ; YYY; ZZZ
        ///   XYZ & YYY & ZZZ
        /// </summary>
        /// <param name="authors_combined"></param>
        /// <returns></returns>
        public static string[] SplitAuthors_LEGACY(string authors_combined)
        {
            // First get rid of all newlines and lfs
            authors_combined = authors_combined.Replace('\r', ' ');
            authors_combined = authors_combined.Replace('\n', ' ');

            string[] authors = authors_combined.Split(new string[] { " and ", " AND ", " And ", "&", ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < authors.Length; ++i)
            {
                authors[i] = authors[i].Trim();
                // ***TODO Some of these could be blank...
            }

            return authors;
        }

        /// <summary>
        /// Attempts to split a full name string into firstname, surname.
        /// Quite flakey at the moment...
        /// </summary>
        /// <param name="full_name"></param>
        /// <param name="first_name"></param>
        /// <param name="last_name"></param>
        public static void SplitName_LEGACY(string full_name, out string first_name, out string last_name)
        {
            // Format: James Jardine or JG Jardine or James van Jardine or J.G. Jardine
            if (full_name.IndexOfAny(new char[] { ',' }) == -1)
            {
                string[] nameSplit = full_name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (0 == nameSplit.Length)
                {
                    first_name = "";
                    last_name = "";
                }
                else if (1 == nameSplit.Length)
                {
                    first_name = "";
                    last_name = nameSplit[0].Trim();
                }
                else
                {
                    // If we get here, we have many names.  Lets assume that all first names start with a capital letter.  If we meet a lower case letter, then we assume that is where the surname starts...
                    int start_of_surname = nameSplit.Length-1;
                    while (true)
                    {
                        int next_test_position = start_of_surname - 1;
                        if (next_test_position >= 0 && Char.IsLower(nameSplit[next_test_position][0]))
                        {
                            start_of_surname = next_test_position;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    first_name = StringTools.ConcatenateStrings(nameSplit, ' ', 0, start_of_surname).Trim();
                    last_name = StringTools.ConcatenateStrings(nameSplit, ' ', start_of_surname).Trim();
                }
            }

            // Format: Jardine, James G.
            else
            {
                string[] nameSplit = full_name.Split(',');
                first_name = StringTools.ConcatenateStrings(nameSplit, ' ', 1).Trim();
                last_name = nameSplit[0].Trim();
            }
        }

        public static readonly List<Name> UNKNOWN_NAMES = new List<Name>(new Name[] { Name.UNKNOWN_NAME });
        public static List<Name> SplitAuthors(string authors_combined)
        {
            if (authors_combined == UNKNOWN_AUTHORS)
            {
                return UNKNOWN_NAMES;
            }

            List<Name> names = new List<Name>();

            string[] authors_split = SplitAuthors_LEGACY(authors_combined);
            foreach (string author_split in authors_split)
            {
                Name name = SplitName(author_split);
                names.Add(name);
            }

            return names;
        }

        public static Name SplitName(string full_name)
        {
            Name name = new Name();
            SplitName_LEGACY(full_name, out name.first_names, out name.last_name);
            return name;
        }
    }
}
