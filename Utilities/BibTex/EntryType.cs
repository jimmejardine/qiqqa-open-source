using System.Collections.Generic;

namespace Utilities.BibTex
{
    public class EntryType
    {
        public string type;
        public List<string> requireds;
        public List<string> optionals;

        public EntryType(string type, string[] requireds, string[] optionals)
        {
            this.type = type;
            this.requireds = new List<string>(requireds);
            this.optionals = new List<string>(optionals);
        }
    }
}
