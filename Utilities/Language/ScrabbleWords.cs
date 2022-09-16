using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Utilities.Language
{
    public class ScrabbleWords
    {
        public static readonly ScrabbleWords Instance = new ScrabbleWords();
        private List<string> words = new List<string>();
        private ReadOnlyCollection<string> words_readonly = null;

        private ScrabbleWords()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Utilities.Language.ScrabbleWords.dat");

            using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (StreamReader sr = new StreamReader(compressed_stream))
                {
                    while (true)
                    {
                        string word = sr.ReadLine();
                        if (null == word)
                        {
                            break;
                        }

                        words.Add(word);
                    }
                }
            }

            words_readonly = words.AsReadOnly();
        }

        public ReadOnlyCollection<string> Words => words_readonly;

        public bool Contains(string word)
        {
            int min = 0;
            int max = words.Count - 1;

            if (words[min].Equals(word))
            {
                return true;
            }

            if (words[max].Equals(word))
            {
                return true;
            }

            while (min < max)
            {
                int mid = (max + min) / 2;
                string test_word = (string)words[mid];

                if (test_word == word)
                {
                    return true;
                }

                if (test_word.CompareTo(word) < 0)
                {
                    if (min < mid)
                    {
                        min = mid;
                    }
                    else
                    {
                        min = mid + 1;
                    }
                }
                else
                {
                    max = mid;
                }
            }

            return false;
        }


        #region --- MAINTENANCE - FOR THE GENERATION OF NEW WORD LISTS ---------------------------------------------------------------------------------------------------

        /// <summary>
        /// Use this method to generate the compressed wordslist that is embedded in the DLL
        /// </summary>
        public static void CreateWords()
        {
            Logging.Info("Creating words list");
            HashSet<string> words = new HashSet<string>();

            Logging.Info("Processing sowpods");
            LoadWords(words, @"C:\temp\sowpods.txt");

            Logging.Info("Processing twl");
            LoadWords(words, @"C:\temp\twl06.txt");

            List<string> words_list = new List<string>(words);
            words_list.Sort();

            Logging.Info("Saving");
            File.WriteAllLines(@"C:\temp\zzzRaw.dat", words_list.ToArray());
            {
                using (Stream stream = new FileStream(@"C:\temp\zzzRawZip.dat", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        using (StreamWriter sw = new StreamWriter(compressed_stream))
                        {
                            foreach (string word in words_list)
                            {
                                sw.WriteLine(word);
                            }
                        }
                    }
                }
            }

            Logging.Info("Loading");
            List<string> new_words = new List<string>();
            using (Stream stream = new FileStream(@"C:\temp\zzzRawZip.dat", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (StreamReader sr = new StreamReader(compressed_stream))
                    {
                        while (true)
                        {
                            string word = sr.ReadLine();
                            if (null == word)
                            {
                                break;
                            }

                            new_words.Add(word);
                        }
                    }
                }
            }


            Logging.Info("Old = {0} New = {1}", words_list.Count, new_words.Count);
            Logging.Info("Old = '{0}' New = '{1}'", words_list[777], new_words[777]);

        }

        private static void LoadWords(HashSet<string> words, string filename)
        {
            using (StreamReader sr = File.OpenText(filename))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (null == line)
                    {
                        break;
                    }

                    line = line.Trim();
                    line = line.ToLower();
                    words.Add(line);
                }
            }
        }

        #endregion
    }
}
