using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common
{
    class URLProtocolHandler
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

                if (false) { }

                // Is it an open url?
                else if ("open" == parts[0])
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
