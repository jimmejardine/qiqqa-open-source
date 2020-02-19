using System;
using System.Globalization;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFAnnotationNodeContent : ISearchable
    {
        // NB: Do not reorder these cos it breaks serialization!
        internal string pdf_document_fingerprint;
        internal Guid pdf_annotation_guid;
        internal string library_fingerprint;

        public PDFAnnotationNodeContent(string library_fingerprint, string pdf_document_fingerprint, Guid pdf_annotation_guid)
        {
            this.library_fingerprint = library_fingerprint;
            this.pdf_document_fingerprint = pdf_document_fingerprint;
            this.pdf_annotation_guid = pdf_annotation_guid;
        }
    }
}
