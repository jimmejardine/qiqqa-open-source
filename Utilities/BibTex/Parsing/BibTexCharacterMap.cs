using System;
using System.Linq;

namespace Utilities.BibTex.Parsing
{
    /// <summary>
    /// Use this class to translate between the weird bibtex control codes to ASCII.
    /// The test harness suggests that this is quick (1.5M a second), so add as many translation lookups as you like...
    /// </summary>
    public class BibTexCharacterMap
    {
        static string[] MAP = new string[]
        {
             @"\", @"\\",

             "à", @"\`a",
             "è", @"\`e",
             "ì", @"\`i",
             "ò", @"\`o",
             "ù", @"\`u",
             "À", @"\`A",
             "È", @"\`E",
             "Ì", @"\`I",
             "Ò", @"\`O",
             "Ù", @"\`U",

             "á", @"\'a",
             "é", @"\'e",
             "í", @"\'i",
             "ó", @"\'o",
             "ú", @"\'u",
             "Á", @"\'A",
             "É", @"\'E",
             "Í", @"\'I",
             "Ó", @"\'O",
             "Ú", @"\'U",
             "Ý", @"\'Y",
            
             "â", @"\^a",
             "ê", @"\^e",            
             "î", @"\^i",
             "ô", @"\^o",
             "û", @"\^u",
             "Â", @"\^A",
             "Ê", @"\^E",
             "Î", @"\^I",
             "Ô", @"\^O",
             "Û", @"\^U",

             "ã", @"\~a",
             "õ", @"\~o",
             "Ã", @"\~A",
             "Õ", @"\~O",
            
             "ä", @"\""a",
             "ë", @"\""e",
             "ï", @"\""i",
             "ӧ", @"\""o",
             "ü", @"\""u",
             "ÿ", @"\""y",
             "Ä", @"\""A",
             "Ë", @"\""E",
             "Ï", @"\""I",
             "Ö", @"\""O",
             "Ü", @"\""U",

             "æ", @"\ae",
             "Æ", @"\AE",
             "ß", @"\ss",
             "å", @"\aa",
             "Å", @"\AA",
             "ø", @"\o",
             "Ç", @"\cC",
             "ç", @"\cc",

             "ñ", @"\~n",
             "Ñ", @"\~N",

             "XXX", "XXX"
        };

        public static string BibTexToASCII(string source)
        {
            if (null == source)
            {
                return source;
            }

            // Get rid of any curlies
            source = source.Replace("{", "");
            source = source.Replace("}", "");

            // Check if there are any escape characters
            if (source.Contains('\\'))
            {
                // Guard for the DOUBLE \\
                string BACKSLASH_GUARD = "__THE_BIGGEST_BACKSLASH_HACK__";
                source = source.Replace(@"\\", BACKSLASH_GUARD);

                // Do the million substitutions
                for (int i = 0; i < MAP.Length; i += 2)
                {
                    source = source.Replace(MAP[i + 1], MAP[i]);
                }

                source = source.Replace(BACKSLASH_GUARD, @"\");
            }

            return source;
        }

        public static string ASCIIToBibTex(string source)
        {
            if (null == source)
            {
                return source;
            }

            // Do the million substitutions
            for (int i = 0; i < MAP.Length; i += 2)
            {
                source = source.Replace(MAP[i], MAP[i+1]);
            }

            return source;
        }

        public static void Test()
        {
            Test_REVERSE();
            Test_SPEED();
        }

        public static void Test_SPEED()
        {
            string s1 = "Großherr Schneider müßt être fâché!";
            string s2 = ASCIIToBibTex(s1);
            string s3 = BibTexToASCII(s2);

            int NUM = 100000;
            DateTime start = DateTime.UtcNow;

            start = DateTime.UtcNow;
            for (int i = 0; i < NUM; ++i)
            {
                s2 = ASCIIToBibTex(s1);
            }
            double time_a2b = NUM * 1000.0 / DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            Logging.Info("To can do {0} per second", time_a2b);

            start = DateTime.UtcNow;
            for (int i = 0; i < NUM; ++i)
            {
                s3 = BibTexToASCII(s2);
            }
            double time_b2a = NUM * 1000.0 / DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            Logging.Info("To can do {0} per second", time_b2a);
        }

        public static void Test_REVERSE()
        {
            string s1 = "Großherr Schneider müßt être fâché!";
            string s2 = ASCIIToBibTex(s1);
            string s3 = BibTexToASCII(s2);

            if (0 != String.Compare(s1, s3))
            {
                Logging.Error("STRANGE");
            }
        }
    }
}
