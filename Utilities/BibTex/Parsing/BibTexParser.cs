using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Utilities.BibTex.Parsing
{
    public class BibTexParser
    {
        private static Regex _latexEncodingsRemover = new Regex(@"\{\\textless\}(.*?)\{\\textgreater\}");

        public static BibTexItem ParseOne(string bibtex, bool suppress_error_logging)
        {
            if (String.IsNullOrWhiteSpace(bibtex)) return null;

            List<BibTexItem> items = Parse(bibtex).Items;

            if (1 < items.Count)
            {
                Logging.Warn("There is more than one BibTex record - using only the first...");
                if (!suppress_error_logging)
                {
                    items[0].Warnings.Add(String.Format("There is more than one BibTeX record - using only the first. RAW record: {0}", bibtex));
                }
            }

            if (0 < items.Count)
            {
                if (!suppress_error_logging)
                {
                    if (items[0].Exceptions.Count > 0)
                    {
                        string errors = items[0].GetExceptionsString();
                        Logging.Warn("{0}", errors);
                    }
                }

                return items[0];
            }

            return null;
        }

        public static BibTexParseResult Parse(string bibtex)
        {
            // First remove any latex encodings.
            if (!String.IsNullOrEmpty(bibtex))
            {
                bibtex = _latexEncodingsRemover.Replace(bibtex, String.Empty);
            }
            
            return BibTexAssembler.Parse(bibtex);
        }

        private BibTexParser()
        {
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            string filename = @"..\..\..\..\Utilities\BibTex\Parsing\Sample.bib";
            string bibtex = File.ReadAllText(filename);

            List<BibTexItem> items = Parse(bibtex).Items;

            for (int i = 0; i < 5; ++i)
            {
                Logging.Info(items[i].ToBibTex());
            }
        }
#endif

        #endregion
    }
}
