using System;

namespace Qiqqa.Documents.PDF.InfoBarStuff.PDFDocumentTagCloudStuff
{
    public class TagCloudEntry
    {
        public string word;
        public int word_count;
        public int document_count;

        public double importance;

        public bool selected;

        public override string ToString()
        {

            return String.Format("{0} {1} = {2} / {3}", word, importance, word_count, document_count);
        }
    }
}
