using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Utilities.BibTex.Parsing
{
    public class BibTexParser
    {
        public static BibTexItem ParseOne(string bibtex, bool suppress_error_logging)
        {
            if (String.IsNullOrWhiteSpace(bibtex))
            {
                return null;
            }

            var rv = Parse(bibtex);
            List<BibTexItem> items = rv.Items;

            if (1 < items.Count)
            {
                // TODO: look into the library entry/entries where I have multiple bibtex records dumped in a single slot.
                // That was surely some metadata merge attempt pending way back then... (Not a code issue; this is a DB issue)
                Logging.Warn("There is more than one BibTex record - using only the first... RAW record: {0}", bibtex);
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
            return BibTexAssembler.Parse(bibtex);
        }

        private BibTexParser()
        {
        }
    }
}
