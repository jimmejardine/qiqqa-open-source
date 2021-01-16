using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Qiqqa.Documents.PDF.ThreadUnsafe;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    internal class PDFAnnotationSerializer
    {
        internal static void WriteToDisk(PDFDocument_ThreadUnsafe pdf_document, bool force_flush_no_matter_what)
        {
            if (!force_flush_no_matter_what)
            {
                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            }

            string json = pdf_document.GetAnnotationsAsJSON();
            if (!String.IsNullOrEmpty(json))
            {
                pdf_document.LibraryRef.Xlibrary.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.ANNOTATIONS, json);
            }
        }

        internal static void ReadFromDisk(PDFDocument_ThreadUnsafe pdf_document)
        {
            byte[] annotations_data = null;

            // Try to load the annotations from file if they exist
            var items = pdf_document.LibraryRef.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.ANNOTATIONS, new List<string>() { pdf_document.Fingerprint });
            ASSERT.Test(items.Count < 2);
            if (0 < items.Count)
            {
                annotations_data = items[0].data;
            }

            // If we actually have some annotations, load them
            if (null != annotations_data)
            {
                List<DictionaryBasedObject> annotation_dictionaries = null;
                try
                {
                    annotation_dictionaries = ReadFromStream_JSON(annotations_data);
                }
                catch (Exception)
                {
                    annotation_dictionaries = ReadFromStream_BINARY(annotations_data);
                }

                if (null != annotation_dictionaries)
                {
                    foreach (DictionaryBasedObject annotation_dictionary in annotation_dictionaries)
                    {
                        PDFAnnotation pdf_annotation = new PDFAnnotation(annotation_dictionary, false);
                        pdf_document.AddUpdatedAnnotation(pdf_annotation);
                    }
                }
            }
        }

        private static List<DictionaryBasedObject> ReadFromStream_JSON(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            List<Dictionary<string, object>> attributes_list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

            List<DictionaryBasedObject> annotation_dictionaries = new List<DictionaryBasedObject>();
            foreach (Dictionary<string, object> attributes in attributes_list)
            {
                annotation_dictionaries.Add(new DictionaryBasedObject(attributes));
            }

            return annotation_dictionaries;
        }

        private static List<DictionaryBasedObject> ReadFromStream_BINARY(byte[] data)
        {
            List<DictionaryBasedObject> annotation_dictionaries = (List<DictionaryBasedObject>)SerializeFile.LoadFromByteArray(data);
            return annotation_dictionaries;
        }
    }
}
