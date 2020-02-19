using System;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFDocumentNodeContent : ISearchable, IRecurrentNodeContent
    {
        private string document_fingerprint;
        private string library_fingerprint;

        [NonSerialized]
        private PDFDocument pdf_document;

        public PDFDocumentNodeContent(string document_fingerprint, string library_fingerprint)
        {
            this.document_fingerprint = document_fingerprint;
            this.library_fingerprint = library_fingerprint;
        }

        public string Fingerprint => document_fingerprint;

        public string LibraryFingerprint => library_fingerprint;

        public PDFDocument PDFDocument
        {
            get
            {
                if (null == pdf_document)
                {
#if SAMPLE_LOAD_CODE
                    pdf_document = WebLibraryDocumentLocator.LocateFirstPDFDocument(library_fingerprint, document_fingerprint);
#endif
                }

                return pdf_document;
            }
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + document_fingerprint.GetHashCode();
            hash = hash * 37 + library_fingerprint.GetHashCode();
            return hash;
        }
    }
}
