using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Qiqqa.Documents.PDF
{
    [Serializable]
    public class PDFAnnotationList : IEnumerable<PDFAnnotation>, ICloneable
    {
        private readonly List<PDFAnnotation> annotations;

        public PDFAnnotationList()
        {
            annotations = new List<PDFAnnotation>();
        }

        private PDFAnnotationList(List<PDFAnnotation> annotations)
        {
            this.annotations = annotations;
        }

        public void __AddUpdatedAnnotation(PDFAnnotation annotation)
        {
            if (!annotations.Contains(annotation))
            {
                annotations.Add(annotation);
            }
        }

        public int Count => annotations.Count;

        public IEnumerator<PDFAnnotation> GetEnumerator()
        {
            return annotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return annotations.GetEnumerator();
        }

        /// <summary>
        /// Deep clone, but does not copy the OnPDFAnnotationListChanged subscribers.
        /// </summary>
        public object Clone()
        {
            var clone = new PDFAnnotationList();
            foreach (var annotation in annotations)
            {
                clone.__AddUpdatedAnnotation((PDFAnnotation)annotation.Clone());
            }
            return clone;
        }
    }
}
