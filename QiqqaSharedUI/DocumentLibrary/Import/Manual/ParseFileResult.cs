using System.Collections.Generic;

namespace Qiqqa.DocumentLibrary.Import.Manual
{
    public class ParseFileResult
    {
        public List<BibTeXEntry> Entries { get; private set; }
        public int EntriesWithoutFileField { get; private set; }

        public ParseFileResult(List<BibTeXEntry> entries, int entriesWithoutValieFiles)
        {
            Entries = entries;
            EntriesWithoutFileField = entriesWithoutValieFiles;
        }
    }
}
