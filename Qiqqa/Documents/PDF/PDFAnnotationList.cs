using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Qiqqa.Documents.PDF
{
    [Serializable]
    public class PDFAnnotationList : IEnumerable<PDFAnnotation>, ICloneable
    {
        readonly List<PDFAnnotation> annotations;

        public delegate void OnPDFAnnotationListChangedDelegate();
        public event OnPDFAnnotationListChangedDelegate OnPDFAnnotationListChanged;

        public PDFAnnotationList()
        {
            this.annotations = new List<PDFAnnotation>();
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

            if (null != OnPDFAnnotationListChanged)
            {
                OnPDFAnnotationListChanged();
            }
        }

        void Bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (null != OnPDFAnnotationListChanged)
            {
                OnPDFAnnotationListChanged();
            }
        }

        public int Count
        {
            get
            {
                return annotations.Count;
            }
        }

        public IEnumerator<PDFAnnotation> GetEnumerator()
        {
            return annotations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return annotations.GetEnumerator();
        }

        /// <summary>
        /// Deep clone, clears out the OnPDFAnnotationListChanged subscribers.
        /// </summary>
        public object Clone()
        {
            var clone = new PDFAnnotationList();
            foreach (var annotation in annotations)
            {
                clone.AddUpdatedAnnotation((PDFAnnotation) annotation.Clone());
            }
            return clone;
        }
    }
}
