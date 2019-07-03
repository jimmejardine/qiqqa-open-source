using System;
using System.Globalization;
using Qiqqa.Brainstorm.Common.Searching;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities.Reflection;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFAnnotationNodeContent : Searchable
    {
        // NB: Do not reorder these cos it breaks serialization!
        internal string pdf_document_fingerprint;
        internal Guid pdf_annotation_guid;
        internal string library_fingerprint;

        [NonSerialized]
        AugmentedBindable<PDFAnnotation> pdf_annotation_bindable;
        [NonSerialized]
        AugmentedBindable<PDFDocument> pdf_document_bindable;

        public PDFAnnotationNodeContent(string library_fingerprint, string pdf_document_fingerprint, Guid pdf_annotation_guid)
        {
            this.library_fingerprint = library_fingerprint;
            this.pdf_document_fingerprint = pdf_document_fingerprint;
            this.pdf_annotation_guid = pdf_annotation_guid;
        }

        private void PrepareBindables()
        {
            if (null == pdf_annotation_bindable || null == pdf_document_bindable)
            {
                PDFDocument out_pdf_document;
                PDFAnnotation out_pdf_annotation;
                if (WebLibraryDocumentLocator.LocateFirstPDFDocumentWithAnnotation(library_fingerprint, pdf_document_fingerprint, pdf_annotation_guid, out out_pdf_document, out out_pdf_annotation))
                {
                    pdf_annotation_bindable = out_pdf_annotation.Bindable;
                    pdf_document_bindable = out_pdf_document.Bindable;
                }
            }
        }
        
        public AugmentedBindable<PDFAnnotation> PDFAnnotation
        {
            get
            {
                PrepareBindables();
                return pdf_annotation_bindable;
            }
        }

        public AugmentedBindable<PDFDocument> PDFDocument
        {
            get
            {
                PrepareBindables();
                return pdf_document_bindable;
            }
        }

        public bool MatchesKeyword(string keyword)
        {
            return (null != PDFAnnotation.Underlying.Text) && PDFAnnotation.Underlying.Text.ToLower(CultureInfo.CurrentCulture).Contains(keyword);
        }
    }
}
