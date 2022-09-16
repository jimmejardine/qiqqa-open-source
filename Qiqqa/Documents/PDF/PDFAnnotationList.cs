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

        public delegate void OnPDFAnnotationListChangedDelegate();
        // TODO: has a cyclic link in the GC to PDFDocument due to PDFDocument registering on this change event:
        // PDFDocument -> Annotations -> OnPDFAnnotationListChanged -> PDFDocument
        public event OnPDFAnnotationListChangedDelegate OnPDFAnnotationListChanged;

        public PDFAnnotationList()
        {
            annotations = new List<PDFAnnotation>();
        }

        private PDFAnnotationList(List<PDFAnnotation> annotations)
        {
            this.annotations = annotations;
        }

        public void AddUpdatedAnnotation(PDFAnnotation annotation)
        {
            if (!annotations.Contains(annotation))
            {
                annotations.Add(annotation);
                annotation.Bindable.PropertyChanged += Bindable_PropertyChanged;
            }

            OnPDFAnnotationListChanged?.Invoke();
        }

        private void Bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPDFAnnotationListChanged?.Invoke();
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
                clone.AddUpdatedAnnotation((PDFAnnotation)annotation.Clone());
            }
            return clone;
        }
    }
}
