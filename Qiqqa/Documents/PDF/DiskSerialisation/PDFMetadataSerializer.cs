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
            // A little hack to make sure the legacies are updated...
            pdf_document.Tags = pdf_document.Tags;
            pdf_document.DateAddedToDatabase = pdf_document.DateAddedToDatabase;
            pdf_document.DateLastModified = pdf_document.DateLastModified;
            pdf_document.DateLastRead = pdf_document.DateLastRead;
            // A little hack to make sure the legacies are updated...

            string json = JsonConvert.SerializeObject(pdf_document.Dictionary.Attributes, Formatting.Indented);
            pdf_document.Library.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA, json);            
            Logging.Debug("Update metadata DB for PDF document {1}: JSON =\n{0}", json, pdf_document.Fingerprint);
        }


        public static DictionaryBasedObject ReadFromStream(byte[] data)
        {
            try
            {
                string json = Encoding.UTF8.GetString(data);
                char a = json[0];
                if (0 != a)
                {
                    Dictionary<string, object> attributes = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    return new DictionaryBasedObject(attributes);
                }
                else
                {
                    return ReadFromStream_BINARY(data);
                }
            }
            catch (Exception)
            {
                return ReadFromStream_BINARY(data);
            }
        }

        private static DictionaryBasedObject ReadFromStream_BINARY(byte[] data)
        {
            DictionaryBasedObject dbo = (DictionaryBasedObject)SerializeFile.LoadFromByteArray(data);
            FeatureTrackingManager.Instance.UseFeature(Features.Legacy_Metadata_Binary);
            return dbo;
        }

        private static DictionaryBasedObject ReadFromDisk_BINARY(string filename)
        {
            DictionaryBasedObject dbo = (DictionaryBasedObject)SerializeFile.LoadRedundant(filename);
            FeatureTrackingManager.Instance.UseFeature(Features.Legacy_Metadata_Binary);
            return dbo;
        }
    }
}
