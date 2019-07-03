using System;
using System.Collections.Generic;

namespace Utilities.Language.TextIndexing
{
    public class IndexPageResult
    {
        public class PageResult
        {
            public int page;
            public double score;

            public override string ToString()
            {
                return String.Format("{0} - {1:0.00}", page, score);
            }
        }

        public string fingerprint;
        public double score;
        public List<PageResult> page_results = new List<PageResult>();

        public override string ToString()
        {
            return String.Format("{0} - {1:0.00} ({2})", fingerprint, score, page_results.Count);
        }
    }
}
