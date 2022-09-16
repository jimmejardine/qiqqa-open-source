using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.Language
{
    public class Stopwords
    {
        public static readonly Stopwords Instance = new Stopwords();
        private HashSet<string> words = new HashSet<string>();

        private Stopwords()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("Utilities.Language.Stopwords.txt");

            using (StreamReader sr = new StreamReader(stream))
            {
                while (true)
                {
                    string word = sr.ReadLine();
                    if (null == word) break;

                    words.Add(word);
                }
            }
        }

        public bool IsStopWord(string word)
        {
            return words.Contains(word);
        }

        public HashSet<string> Words => words;
    }
}
