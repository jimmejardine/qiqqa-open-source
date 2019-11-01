using System;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common
{
    internal class URLProtocolHandler
    {
        internal static void Go(string filename)
        {
            try
            {
                string parts_string = filename.Replace("qiqqa://", "");
                string[] parts = parts_string.Split('/');

                if (0 == parts.Length)
                {
                    throw new GenericException("Unknown qiqqa protocol request: {0}", filename);
                }

                // Is it an open url?
                if ("open" == parts[0])
                {
                    if (3 > parts.Length)
                    {
                        throw new GenericException("Too few parameters for qiqqa protocol open request: {0}", filename);
                    }

                    string library_id = parts[1];
                    string document_fingerprint = parts[2];

                    Library library = WebLibraryManager.Instance.GetLibrary(library_id);
                    if (null == library)
                    {
                        throw new GenericException("Unknown library for qiqqa protocol open request: {0}", filename);
                    }

                    PDFDocument pdf_document = library.GetDocumentByFingerprint(document_fingerprint);
                    if (null == pdf_document)
                    {
                        throw new GenericException("Unknown document for qiqqa protocol open request: {0} @ fingerprint {1}", filename, document_fingerprint);
                    }

                    MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
                }

                else
                {
                    throw new GenericException("Unknown qiqqa protocol request: {0}", filename);
                }
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex.Message);
            }
        }
    }
}
