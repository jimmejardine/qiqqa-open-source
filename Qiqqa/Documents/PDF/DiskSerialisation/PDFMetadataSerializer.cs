using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    public static class PDFMetadataSerializer
    {
        internal static void WriteToDisk(PDFDocument pdf_document, bool force_flush_no_matter_what)
        {
            if (!force_flush_no_matter_what)
            {
                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            }

            // A little hack to make sure the legacies are updated...
            pdf_document.Tags = pdf_document.Tags;
            pdf_document.DateAddedToDatabase = pdf_document.DateAddedToDatabase;
            pdf_document.DateLastModified = pdf_document.DateLastModified;
            pdf_document.DateLastRead = pdf_document.DateLastRead;
            // A little hack to make sure the legacies are updated...

            string json = pdf_document.GetAttributesAsJSON();
            pdf_document.LibraryRef.Xlibrary.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.METADATA, json);
            Logging.Debug("Update metadata DB for PDF document {1}: JSON =\n{0}", json, pdf_document.Fingerprint);
        }


        public static DictionaryBasedObject ReadFromStream(byte[] data)
        {
            string json = null;
            char a;
            try
            {
                json = Encoding.UTF8.GetString(data);
                a = (json.Length > 0 ? json[0] : (char)0);
                if ((char)0 != a)
                {
                    Dictionary<string, object> attributes = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    return new DictionaryBasedObject(attributes);
                }
                else
                {
                    return ReadFromStream_BINARY(data);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Failed to ReadFromStream, second attempt is to read using old binary format.");

                try
                {
                    return ReadFromStream_BINARY(data);
                }
                catch (Exception ex2)
                {
                    Logging.Error(ex2, "Failed second attempt, reading using old binary format. Giving it a last chance at redemption, assuming it's raw BibTeX. RAW data:\n{0}", json);

                    // last chance at redemption for somewhat sane content:
                    if (!String.IsNullOrEmpty(json) && json.Length > 10)
                    {
                        // store it as raw bibTeX.
                        Dictionary<string, object> attributes = new Dictionary<string, object>();
                        attributes.Add("BibTex", json);

                        Logging.Warn("Last chance at redemption: Failing metadata record has been kept as RAW BibTeX record:\n{0}", json);

                        return new DictionaryBasedObject(attributes);
                    }
                }

                throw;
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
