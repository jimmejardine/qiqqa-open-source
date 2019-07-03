using System;
using System.Collections.Generic;
using System.ComponentModel;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF
{
    [Serializable]
    public class PDFHightlightList : ICloneable
    {
        Dictionary<int, HashSet<PDFHighlight>> highlights = new Dictionary<int, HashSet<PDFHighlight>>();

        public delegate void OnPDFHighlightListChangedDelegate();
        public event OnPDFHighlightListChangedDelegate OnPDFHighlightListChanged;

        private void RemoveHighlight_Internal(PDFHighlight highlight)
        {
            int count_before = highlights.Count;

            HashSet<PDFHighlight> highlights_for_page;
            if (highlights.TryGetValue(highlight.Page, out highlights_for_page))
            {
                highlights_for_page.Remove(highlight);
                if (0 == highlights_for_page.Count)
                {
                    highlights.Remove(highlight.Page);
                }
            }

            // If we find we have removed the last highlight, then let's add a null highlight so that the sync happens...
            // This is a hack, so if you can improve the sync problem (don't want to create a lot of null files upfront), then pls do...
            int count_after = highlights.Count;
            if (0 == count_after && 0 != count_before)
            {
                AddHighlight_Internal(new PDFHighlight(1, new Word { Text = "" }, 1));
            }
        }

        private void AddHighlight_Internal(PDFHighlight highlight)
        {
            HashSet<PDFHighlight> highlights_for_page;
            if (!highlights.TryGetValue(highlight.Page, out highlights_for_page))
            {
                highlights_for_page = new HashSet<PDFHighlight>();
                highlights[highlight.Page] = highlights_for_page;
            }

            highlights_for_page.Add(highlight);
        }

        public void AddUpdatedHighlight(PDFHighlight highlight)
        {
            AddHighlight_Internal(highlight);

            if (null != OnPDFHighlightListChanged)
            {
                OnPDFHighlightListChanged();
            }
        }

        public void RemoveUpdatedHighlight(PDFHighlight highlight)
        {
            RemoveHighlight_Internal(highlight);

            if (null != OnPDFHighlightListChanged)
            {
                OnPDFHighlightListChanged();
            }
        }

        void Bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (null != OnPDFHighlightListChanged)
            {
                OnPDFHighlightListChanged();
            }
        }

        public int Count
        {
            get
            {
                int total = 0;
                foreach (var highlights_for_page in highlights.Values)
                {
                    total += highlights_for_page.Count;
                }
                return total;
            }
        }

        public List<int> GetAffectedPages()
        {
            List<int> result = new List<int>(highlights.Keys);
            result.Sort();
            return result;
        }

        HashSet<PDFHighlight> EMPTY_HIGHLIGHTS_FOR_PAGE = new HashSet<PDFHighlight>();
        public HashSet<PDFHighlight> GetHighlightsForPage(int page)
        {
            HashSet<PDFHighlight> highlights_for_page;
            if (highlights.TryGetValue(page, out highlights_for_page))
            {
                return highlights_for_page;
            }
            else
            {
                return EMPTY_HIGHLIGHTS_FOR_PAGE;
            }
        }

        public HashSet<PDFHighlight> GetAllHighlights()
        {
            HashSet<PDFHighlight> results = new HashSet<PDFHighlight>();
            foreach (var highlights_for_page in highlights.Values)
            {
                results.UnionWith(highlights_for_page);
            }
            return results;
        }

        /// <summary>
        /// Deep clone, but does not copy the OnPDFHighlightListChanged subscribers.
        /// </summary>
        public object Clone()
        {
            var clone = new PDFHightlightList();
            foreach (var highlights_for_page in highlights.Values)
            {
                foreach (var highlight in highlights_for_page)
                {
                    clone.AddUpdatedHighlight((PDFHighlight)highlight.Clone());
                }
            }
            return clone;
        }
    }
}
