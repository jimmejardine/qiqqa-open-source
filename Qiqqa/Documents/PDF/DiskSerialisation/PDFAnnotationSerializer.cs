using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    internal class PDFAnnotationSerializer
    {
        internal static void WriteToDisk(PDFDocument pdf_document, bool force_flush_no_matter_what)
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

        internal static Dictionary<string, PDFAnnotationList> BulkReadFromDisk(WebLibraryDetail web_library_detail)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // Every document which has any annotations, may have many. So its one fingerprint, many annotation records.
            // We do not (yet) bother about potential fringe cases where annotations end up in a corrupted database in duplicate:
            // we KNOW that PDFAnnotationList code can manage that as it does compare incoming annotations
            // to already existing ones (used by the GUI annotation editor, of course :-) )
            // Hence we choose a PDFAnnotationList for the annotations.
            Dictionary<string, PDFAnnotationList> rv = new Dictionary<string, PDFAnnotationList>();
        
            // Try to load the annotations from file if they exist
            Dictionary<string, byte[]> library_items_annotations_cache = web_library_detail.Xlibrary.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.ANNOTATIONS);

            foreach (var item in library_items_annotations_cache)
            {
                string fingerprint = item.Key;
                byte[] annotations_data = item.Value;

                List<DictionaryBasedObject> annotation_dictionaries;
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

                        if (pdf_annotation.DocumentFingerprint != fingerprint)
                        {
                            Logging.Error($"Corrupted ANNOTATIONS record in database? Fingerprints do not match: key: '{fingerprint}' record-ref: '{pdf_annotation.DocumentFingerprint}'");

                            // We DO NOT know which one of those fingerprints is TRUE. We DO NOT want to loose the data, so we assign to BOTH.
                            // This happens when we first clone the annotation and modify that fingerprint to store the SECOND record,
                            // after which we follow the regular code path further below to store the FIRST (= ORIGINAL) record.
                            PDFAnnotationList lst = null;
                            _ = rv.TryGetValue(fingerprint, out lst);
                            if (lst == null)
                            {
                                lst = new PDFAnnotationList();
                                rv.Add(fingerprint, lst);
                            }
                            var dupdict = (DictionaryBasedObject)annotation_dictionary.Clone();

                            // HACK: override the fingerprint in the decoded record: this duplicates code from `PDFAnnotation.DocumentFingerprint`
                            dupdict["DocumentFingerprint"] = fingerprint;

                            PDFAnnotation dup = new PDFAnnotation(annotation_dictionary, true);
                            lst.__AddUpdatedAnnotation(dup);
                        }

                        {
                            PDFAnnotationList lst = null;
                            _ = rv.TryGetValue(pdf_annotation.DocumentFingerprint, out lst);
                            if (lst == null)
                            {
                                lst = new PDFAnnotationList();
                                rv.Add(pdf_annotation.DocumentFingerprint, lst);
                            }
                            lst.__AddUpdatedAnnotation(pdf_annotation);
                        }
                    }
                }
            }

            return rv;
        }

        internal static void ReadFromDisk(PDFDocument pdf_document)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

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
