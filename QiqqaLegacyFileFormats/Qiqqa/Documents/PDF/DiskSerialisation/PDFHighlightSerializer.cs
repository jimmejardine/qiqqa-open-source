using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF.DiskSerialisation
{

#if SAMPLE_LOAD_CODE

    internal class PDFHighlightSerializer
    {
        internal static void ReadFromStream(PDFDocument_ThreadUnsafe pdf_document, PDFHightlightList highlights, Dictionary<string, byte[]> /* can be null */ library_items_highlights_cache)
        {
            byte[] highlights_data = null;

            if (null != library_items_highlights_cache)
            {
                library_items_highlights_cache.TryGetValue(pdf_document.Fingerprint, out highlights_data);
            }
            else
            {
                List<LibraryDB.LibraryItem> library_items = pdf_document.Library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.HIGHLIGHTS);
                if (0 < library_items.Count)
                {
                    highlights_data = library_items[0].data;
                }
            }

            if (null != highlights_data)
            {
                try
                {
                    List<PDFHighlight> highlights_list = null;

                    // First try normal
                    try
                    {
                        highlights_list = ReadFromStream_JSON(highlights_data);
                    }
                    catch (Exception)
                    {
                        highlights_list = ReadFromStream_PROTOBUF(highlights_data);
                    }

                    if (null != highlights_list)
                    {
                        foreach (PDFHighlight highlight in highlights_list)
                        {
                            highlights.AddUpdatedHighlight(highlight);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem loading the Highlights for document {0}", pdf_document.Fingerprint);
                }
            }
        }

        private static List<PDFHighlight> ReadFromStream_JSON(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            List<PDFHighlight> highlights_list = JsonConvert.DeserializeObject<List<PDFHighlight>>(json);
            return highlights_list;
        }

        private static List<PDFHighlight> ReadFromStream_PROTOBUF(byte[] data)
        {
            List<PDFHighlight> highlights_list = SerializeFile.ProtoLoadFromByteArray<List<PDFHighlight>>(data);
            return highlights_list;
        }

        internal static void WriteToDisk(PDFDocument_ThreadUnsafe pdf_document)
        {
            string json = pdf_document.GetHighlightsAsJSON();
            if (!String.IsNullOrEmpty(json))
            {
                pdf_document.Library.LibraryDB.PutString(pdf_document.Fingerprint, PDFDocumentFileLocations.HIGHLIGHTS, json);
            }
        }
    }

#endif

}
