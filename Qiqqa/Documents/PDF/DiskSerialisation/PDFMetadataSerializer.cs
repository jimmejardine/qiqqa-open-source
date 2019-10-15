using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Qiqqa.UtilisationTracking;
using Qiqqa.Documents.PDF.ThreadUnsafe;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    public class PDFMetadataSerializer
    {
        internal static void WriteToDisk(PDFDocument_ThreadUnsafe pdf_document)
        {
            string json = pdf_document.GetAttributesAsJSON();
            pdf_document.Library.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA, json);            
            Logging.Debug("Update metadata DB for PDF document {1}: JSON =\n{0}", json, pdf_document.Fingerprint);
        }

        public static PDFDocument ReadFromStream(DocumentLibrary.Library library, byte[] data)
        {
            // TODO: make sure the legacies are updated...
            string json = "(null)";
            try
            {
                json = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject<PDFDocument>(json);
            }
            catch (Exception ex)
            {
                string msg = String.Format("Failed to import PDFDocument metadata -- is this some old format which hasn't been converted yet? Data: {0}", json);
                var ex2 = new Exception(msg, ex);
                Logging.Error(ex, msg);
                throw ex2;
            }
        }
    }
}
