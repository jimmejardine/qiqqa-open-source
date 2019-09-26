using System;
using System.Text;
using Utilities.BibTex.Parsing;
using Utilities.Random;

namespace Utilities.BibTex
{
    public class BibTexTools
    {
        // ----------------------------------------------------------------------------------------

        public static string GenerateRandomBibTeXKey(string seed = null)
        {
            if (String.IsNullOrEmpty(seed))
            {
                seed = "QIQQA";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(seed + "-");
            for (int i = 0; i < 5; ++i)
            {                
                sb.Append(Convert.ToChar((byte)(65 + RandomAugmented.Instance.NextIntExclusive(26))));
            }
            return sb.ToString();
        }

        public static string GetEmptyArticleBibTeXTemplate()
        {
            string key = GenerateRandomBibTeXKey();

            return
                "@article{" + GenerateRandomBibTeXKey() + "," +
                " author = {}," +
                " title = {}," +
                " year = {}," +
                " publisher = {}" +
                "}"
                ;
        }
    }
}
