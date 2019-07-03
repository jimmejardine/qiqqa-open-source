using Utilities.OCR;

namespace Qiqqa.Documents.PDF.Search
{
    public class PDFSearchResult
    {
        public string[] keywords;

        public int page;
        public int keyword_index;
        public Word[] words;
        public string context_sentence;

        public override string ToString()
        {
            return context_sentence;
        }
    }
}
