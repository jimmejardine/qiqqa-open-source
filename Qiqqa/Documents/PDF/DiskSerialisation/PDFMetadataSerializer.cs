using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    public class PDFMetadataSerializer
    {
        internal static void WriteToDisk(PDFDocument pdf_document)
        {
            string json = JsonConvert.SerializeObject(pdf_document, Formatting.Indented);
            pdf_document.Library.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA, json);            
            //Logging.Debug(json);
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
                Logging.Error(ex, "Failed to import PDFDocumnt metadata -- is this some old format which hasn't been converted yet? Data: {0}", json);
                return null;
            }
        }
    }
}
