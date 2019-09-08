using System;
using System.Collections.Generic;

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
        static readonly string[] MAP = new string[]
        {
            // Conversion of double-backslash `\\` back to `\` must, by necessity of not 
            // causing corruption of the conversion process due to collision with other
            // backslash-escapes, happen as two-stage process, where we FIRST convert
            // double-backslash to a 'magic' Unicode NON-CHARACTER sequence and then,
            // when we've reached the end of the process, pick up from thre and perform
            // the second step.
            //
            // Since the forward conversion of 'literal' backslash to double backslash
            // doesn't suffer from the same collision risk, you'll note that the
            // forward and reverse conversion routines further below are coded ever so slightly
            // different. <evil_grin />
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
        };

        // re performance: https://stackoverflow.com/questions/301371/why-is-dictionary-preferred-over-hashtable-in-c
        static readonly Dictionary<string, string> MAP_U2T = new Dictionary<string, string>(/* MAP */);
        static readonly Dictionary<string, string> MAP_T2U = new Dictionary<string, string>(/* MAP */);

        static readonly int Size = MAP.Length;

        static BibTexCharacterMap()
        {
            for (int i = 0; i < MAP.Length; i += 2)
            {
                MAP_U2T.Add(MAP[i], MAP[i + 1]);
                MAP_T2U.Add(MAP[i], MAP[i + 1]);
            }
        }

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
            if (source.IndexOf('\\') >= 0)
            {
                // Guard for the DOUBLE \\
                const string BACKSLASH_GUARD = "\uFDD0\uFDD1"; // http://www.unicode.org/faq/private_use.html#nonchar3 and onwards
                source = source.Replace(@"\\", BACKSLASH_GUARD);

                // Do the million substitutions; skip the double-backslash conversion entry as that one's useless now
                int len = MAP.Length;
                for (int i = 2; i < len; i += 2)
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
            int len = MAP.Length;
            for (int i = 0; i < len; i += 2)
            {
                source = source.Replace(MAP[i], MAP[i + 1]);
            }

            return source;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		// see also http://diacritics.typo.cz/index.php?id=1
        const string TestStringS1 = @"Großherr Schneider müßt être fâché! \ možete tõmmata zdravím můj příteli jak se máš být lepší to těžká řeč áéíóúàèëïöüĳ ÁÉÍÓÚÀÈËÏÖÜĲ patiënt, reünie, coördinatie. Modern fonts rarely contain an Ĳ/ĳ with a double acute, so today it's usually represented as ÍJ/íj. hè, blèren. dấu nặng. ḃ, ċ, ḋ, ḟ, ġ, ṁ, ṗ, ṡ and ṫ. the Ŀ or ŀ (Ldot, ldot) characters. e is ɛ while é is [e:] ő is [ø:] and ű is [y:] dấu hỏ. Háček is a Czech word meaning little hook. This mark goes by other names as well. In Slovak it is called mäkčeň (i.e. “softener” or “palatalization mark”), in Slovenian strešica (“little roof”), in Croatian, Serbian and Bosnian kvačica (also “small hook”), and hattu (“hat”) in Fennic languages. Icelandic capital Ð appears the same as the South Slavic/Vietnamese Ð, but its lower case counterpart is different: ð. French â, ê, î, ô, û. In Slovak, it is used with ô. In Esperanto, with ĉ, ĝ, ĥ, ĵ, and ŝ. In Welsh with â, ê, î, ô, û, ŵ, and ŷ. quốc ngữ. An acute can be added to ư to produce ứ, and a breve can be added to ơ to produce ờ. Naming b bê bò and p pê phở is to avoid confusion in some dialects or some contexts, the same for s sờ mạnh (nặng) and x xờ nhẹ, i i ngắn and y y dài. Nguyễn, Đình-Hoà. In Polish, the ogonek is used with ą and ę for denoting nasal vowels. It also indicates nasality in a number of Native North American languages, such as Cayuga: ę and ǫ, and Chipewyan: ą, ę, ɛ̨, į, ǫ, ų. In Lithuanian, it lengthtens ą, ę, į and ų. somewhere between ö and ō. In Vietnamese, the tilde (or dấu ngã). including Pe̍h-ōe-jī and the Taiwanese Romanization System. It is used above vowels to indicate a specific tone (called tone 8) : a̍ e̍ i̍ o̍ o̍͘ u̍. It is also used in the Standardized orthography of Congo-Kinshasa to indicate mid tone in some languages like Ngbaka Gbaya : a̍ e̍ ɛ̍ i̍ o̍ ɔ̍ u̍. (or dấu sắc)";

        [TestMethod]
        public void Test_Conversion_To_And_From_BibTeX_Text()
        {
            string s1 = TestStringS1;
            string s2 = ASCIIToBibTex(s1);
            string s3 = BibTexToASCII(s2);

            ASSERT.AreEqual(s1, s3);
        }

        [TestMethod]
        public void Test_SPEED()
        {
            string s1 = TestStringS1;
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
            double time_a2b = i * 1.0E-3 * s1.Length / start.ElapsedMilliseconds;
            Logging.Info("ASCIIToBibTex can do {0:0.000}M operations per second per character", time_a2b);

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
            double time_b2a = i * 1.0E-3 * s1.Length / start.ElapsedMilliseconds;
            Logging.Info("BibTexToASCII can do {0:0.000}M operations per second per character", time_b2a);

            // dummy
            ASCIIToBibTex(s3);
        }
#endif

        #endregion
    }
}
