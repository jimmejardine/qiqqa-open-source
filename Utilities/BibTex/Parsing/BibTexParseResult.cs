using System.Collections.Generic;

namespace Utilities.BibTex.Parsing
{
    public class BibTexParseResult
    {
        public BibTexParseResult(List<BibTexItem> items, List<string> comments)
        {
            Items = items;
            Comments = comments;
        }

        public List<BibTexItem> Items { get; private set; } 
        public List<string> Comments { get; private set; } 
    }
}
