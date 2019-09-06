using System;
using System.Linq;

#if TEST
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using static QiqqaTestHelpers.MiscTestHelpers;
using Utilities;
#endif

#if TEST
namespace QiqqaSystemTester
#else
namespace Utilities.BibTex.Parsing
#endif
{
#if TEST
    [TestClass]
#endif
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
                source = source.Replace(MAP[i], MAP[i + 1]);
            }

            return source;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        [TestMethod]
        public void Test_Conversion_To_And_From_BibTeX_Text()
        {
            string s1 = "Großherr Schneider müßt être fâché!";
            string s2 = ASCIIToBibTex(s1);
            string s3 = BibTexToASCII(s2);

            ASSERT.AreEqual(s1, s3);
        }

        [TestMethod]
        public void Test_SPEED()
        {
            string s1 = "Großherr Schneider müßt être fâché!";
            string s2 = ASCIIToBibTex(s1);
            string s3 = BibTexToASCII(s2);

            const int NUM = 1000000;
            const int CHUNK = 10000;
            Stopwatch start = Stopwatch.StartNew();
            int i;

            for (i = 0; i < NUM; ++i)
            {
                if (i % CHUNK == CHUNK - 1)
                {
                    // don't run for more than 2 seconds
                    if (start.ElapsedMilliseconds >= 2000)
                    {
                        break;
                    }
                }
                s2 = ASCIIToBibTex(s1);
            }
            double time_a2b = i * 1.0 / start.ElapsedMilliseconds;
            Logging.Info("ASCIIToBibTex can do {0:0.000}K operations per second", time_a2b);

            start = Stopwatch.StartNew();
            for (i = 0; i < NUM; ++i)
            {
                if (i % CHUNK == CHUNK - 1)
                {
                    // don't run for more than 2 seconds
                    if (start.ElapsedMilliseconds >= 2000)
                    {
                        break;
                    }
                }
                s3 = BibTexToASCII(s2);
            }
            double time_b2a = i * 1.0 / start.ElapsedMilliseconds;
            Logging.Info("BibTexToASCII can do {0:0.000}K operations per second", time_b2a);

            // dummy
            ASCIIToBibTex(s3);
        }
#endif

        #endregion
    }
}
