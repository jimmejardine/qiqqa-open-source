using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.BibTex
{
    public class BibTeXActionComments
    {
        public static readonly string SKIP = "@comment { BIBTEX_SKIP }";
        public static readonly string AUTO_BIBTEXSEARCH = "@comment { BIBTEX_AUTO - BIBTEXSEARCH }";
        public static readonly string AUTO_GS = "@comment { BIBTEX_AUTO - GS }";
        public static readonly string USER_VETTED = "@comment { BIBTEX_USER_VETTED }";
        public static readonly string MANUAL_EDIT = "@comment { BIBTEX_MANUAL_EDIT }";
    }
}
