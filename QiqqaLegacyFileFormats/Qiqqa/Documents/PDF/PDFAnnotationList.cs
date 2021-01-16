using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF
{
    [Serializable]
    public class PDFAnnotationList // : IEnumerable<PDFAnnotation>
    {
        private readonly List<PDFAnnotation> annotations;

        // public delegate void OnPDFAnnotationListChangedDelegate();
        // // TODO: has a cyclic link in the GC to PDFDocument due to PDFDocument registering on this change event:
        // // PDFDocument -> Annotations -> OnPDFAnnotationListChanged -> PDFDocument
        // public event OnPDFAnnotationListChangedDelegate OnPDFAnnotationListChanged;

        public PDFAnnotationList()
        {
            annotations = new List<PDFAnnotation>();
        }

        private PDFAnnotationList(List<PDFAnnotation> annotations)
        {
            this.annotations = annotations;
        }

        public int Count => annotations.Count;
    }
}
