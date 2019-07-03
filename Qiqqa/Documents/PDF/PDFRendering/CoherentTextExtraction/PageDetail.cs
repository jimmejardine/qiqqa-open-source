using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFRendering.CoherentTextExtraction
{
    class PageDetail
    {
        public int page;
        public WordList word_list;

        public PageDetail(int page)
        {            
            this.page = page;
        }
    }
}
