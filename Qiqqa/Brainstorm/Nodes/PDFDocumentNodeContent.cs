using System;
using Qiqqa.Brainstorm.Common;
using Qiqqa.Brainstorm.Common.Searching;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;

namespace Qiqqa.Brainstorm.Nodes
{
    [Serializable]
    public class PDFDocumentNodeContent : ISearchable, IRecurrentNodeContent
    {
        string document_fingerprint;
        string library_fingerprint;

        [NonSerialized]
        PDFDocument pdf_document;

        public PDFDocumentNodeContent(string document_fingerprint, string library_fingerprint)
        {
            this.document_fingerprint = document_fingerprint;
            this.library_fingerprint = library_fingerprint;
        }

        public string Fingerprint
        {
            get
            {
                return document_fingerprint;
            }
        }

        public string LibraryFingerprint
        {
            get
            {
                return library_fingerprint;
            }
        }

        public PDFDocument PDFDocument
        {
            get
            {
                if (null == pdf_document)
                {
                    pdf_document = WebLibraryDocumentLocator.LocateFirstPDFDocument(library_fingerprint, document_fingerprint);
                }

                return pdf_document;
            }
        }


        public bool MatchesKeyword(string keyword)
        {
            return
                false
                || (null != PDFDocument.TitleCombined) && PDFDocument.TitleCombined.ToLower().Contains(keyword)
                || (null != PDFDocument.AuthorsCombined) && PDFDocument.AuthorsCombined.ToLower().Contains(keyword)
                || (null != PDFDocument.Comments) && PDFDocument.Comments.ToLower().Contains(keyword)
                || (null != PDFDocument.Publication) && PDFDocument.Publication.ToLower().Contains(keyword)
                ;
        }

        public override bool Equals(object obj)
        {
            PDFDocumentNodeContent other = obj as PDFDocumentNodeContent;
            if (null == other) return false;

            if (this.document_fingerprint != other.document_fingerprint) return false;
            if (this.library_fingerprint != other.library_fingerprint) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 37 + this.document_fingerprint.GetHashCode();
            hash = hash * 37 + this.library_fingerprint.GetHashCode();
            return hash;
        }
    }
}
