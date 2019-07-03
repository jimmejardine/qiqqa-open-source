using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class PDFTextItemPool
    {
        public static PDFTextItemPool Instance = new PDFTextItemPool();

        Queue<PDFTextItem> pool = new Queue<PDFTextItem>();
        
        private PDFTextItemPool()
        {
        }

        public PDFTextItem GetPDFTextItem(Word word)
        {
            if (pool.Count > 0)
            {
                PDFTextItem pdf_text_item = pool.Dequeue();
                pdf_text_item.Word = word;
                return pdf_text_item;

            }
            else
            {
                return new PDFTextItem(word);
            }
        }

        public void RecyclePDFTextItem(PDFTextItem pdf_text_item)
        {
            pdf_text_item.Word = null;
            pool.Enqueue(pdf_text_item);
        }

        public void RecyclePDFTextItemsFromChildren(UIElementCollection children)
        {
            List<PDFTextItem> children_to_kill = new List<PDFTextItem>(children.OfType<PDFTextItem>());
            foreach (PDFTextItem pdf_text_item in children_to_kill)
            {
                children.Remove(pdf_text_item);
                RecyclePDFTextItem(pdf_text_item);                
            }
        }
    }
}
