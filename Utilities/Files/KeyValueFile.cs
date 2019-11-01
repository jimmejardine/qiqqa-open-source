using System;
using System.Collections;
using Utilities.Collections;

namespace Utilities.Files
{
    public class KeyValueFile
    {
        public static char SPLIT_CHARACTER = '=';

        public static Hashtable loadKeyValueFile(string filename)
        {
            Hashtable results = new Hashtable();

            // Load the file
            String[] descriptions_lines = TextFile.LoadTextFile(filename);

            // Process each line
            foreach (String next_line in descriptions_lines)
            {
                String[] components = Splitter.splitStringAtFirstChar(next_line, SPLIT_CHARACTER);

                // Ignore any line that does not have the right number of components
                if (2 != components.Length)
                {
                    Console.WriteLine("Warning: ignoring bad line {0}", next_line);
                    continue;
                }

                // Store the result
                String key_string = components[0];
                String value_string = components[1];
                results[key_string] = value_string;
            }

            return results;
        }

        public static void saveKeyValueFile(string filename, Hashtable keyvalues)
        {
            // Build up our lines array
            int lines_pos = 0;
            String[] lines = new String[keyvalues.Count];
            foreach (DictionaryEntry entry in keyvalues)
            {
                lines[lines_pos] = ((String)entry.Key) + SPLIT_CHARACTER + ((String)entry.Value);
                ++lines_pos;
            }

            TextFile.SaveTextFile(filename, lines);
        }

        public static void saveKeyValueFileIgnoringExceptions(string filename, Hashtable hashtable)
        {
            try
            {
                saveKeyValueFile(filename, hashtable);
            }

            catch (Exception e)
            {
                e.ToString();  // Suppress the warning by doing nothing special
            }
        }


        public static Hashtable loadKeyValueFileIgnoringExceptions(string filename)
        {
            try
            {
                return loadKeyValueFile(filename);
            }

            catch (Exception e)
            {
                e.ToString();  // Suppress the warning by doing nothing special
                return new Hashtable();
            }
        }
    }
}
