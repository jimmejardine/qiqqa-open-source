using System;
using Qiqqa.Documents.PDF;
using Utilities;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    public static class WebLibraryDocumentLocator
    {
        public static PDFDocument LocateFirstPDFDocument(string library_fingerprint, string document_fingerprint)
        {
            // First attempt to find it in the specified library
            if (null != library_fingerprint)
            {
                Library library = WebLibraryManager.Instance.GetLibrary(library_fingerprint);
                if (null != library)
                {
                    PDFDocument pdf_document = library.GetDocumentByFingerprint(document_fingerprint);
                    if (null != pdf_document)
                    {
                        return pdf_document;
                    }
                    else
                    {
                        Logging.Warn("WbLibraryDocumentLocator: Cannot find document anymore for fingerprint {0}", document_fingerprint);
                    }
                }
            }

            // If we couldn't find it, then try in all the other libraries...
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries)
            {
                PDFDocument pdf_document = web_library_detail.library.GetDocumentByFingerprint(document_fingerprint);
                if (null != pdf_document)
                {
                    return pdf_document;
                }
                else
                {
                    Logging.Warn("WbLibraryDocumentLocator: Cannot find document anymore for fingerprint {0}", document_fingerprint);
                }
            }

            return null;
        }

        public static bool LocateFirstPDFDocumentWithAnnotation(string library_fingerprint, string document_fingerprint, Guid annotation_guid, out PDFDocument out_pdf_document, out PDFAnnotation out_pdf_annotation)
        {
            // First attempt to find it in the specified library
            if (null != library_fingerprint)
            {
                Library library = WebLibraryManager.Instance.GetLibrary(library_fingerprint);
                if (null != library)
                {
                    PDFDocument pdf_document = library.GetDocumentByFingerprint(document_fingerprint);
                    if (null != pdf_document)
                    {
                        PDFAnnotation pdf_annotation = pdf_document.GetAnnotationByGuid(annotation_guid);
                        if (null != pdf_annotation)
                        {
                            out_pdf_document = pdf_document;
                            out_pdf_annotation = pdf_annotation;
                            return true;
                        }
                    }
                }
            }

            // If we couldn't find it, then try in all the other libraries...
            foreach (WebLibraryDetail web_library_detail in WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries)
            {
                PDFDocument pdf_document = web_library_detail.library.GetDocumentByFingerprint(document_fingerprint);
                if (null != pdf_document)
                {
                    PDFAnnotation pdf_annotation = pdf_document.GetAnnotationByGuid(annotation_guid);
                    if (null != pdf_annotation)
                    {
                        out_pdf_document = pdf_document;
                        out_pdf_annotation = pdf_annotation;
                        return true;
                    }
                }
            }

            // Out of luck!
            out_pdf_document = null;
            out_pdf_annotation = null;
            return false;
        }
    }
}
