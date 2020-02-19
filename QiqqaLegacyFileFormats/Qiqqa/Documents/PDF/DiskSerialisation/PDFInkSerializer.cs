using System;
using System.Collections.Generic;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF.DiskSerialisation
{

#if SAMPLE_LOAD_CODE

    internal class PDFInkSerializer
    {
        internal static void ReadFromDisk(PDFDocument_ThreadUnsafe pdf_document, PDFInkList inks, Dictionary<string, byte[]> library_items_inks_cache)
        {
            try
            {
                byte[] inks_data = null;
                if (null != library_items_inks_cache)
                {
                    library_items_inks_cache.TryGetValue(pdf_document.Fingerprint, out inks_data);
                }
                else
                {
                    List<LibraryDB.LibraryItem> library_items = pdf_document.Library.LibraryDB.GetLibraryItems(pdf_document.Fingerprint, PDFDocumentFileLocations.INKS);
                    if (0 < library_items.Count)
                    {
                        inks_data = library_items[0].data;
                    }
                }


                if (null != inks_data)
                {
                    Dictionary<int, byte[]> page_ink_blobs = SerializeFile.ProtoLoadFromByteArray<Dictionary<int, byte[]>>(inks_data);

                    if (null != page_ink_blobs)
                    {
                        foreach (var pair in page_ink_blobs)
                        {
                            inks.AddPageInkBlob(pair.Key, pair.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading the Inks for document {0}", pdf_document.Fingerprint);
            }
        }

        internal static void WriteToDisk(PDFDocument_ThreadUnsafe pdf_document)
        {
            byte[] data = pdf_document.GetInksAsJSON();

            if (null != data)
            {
                pdf_document.Library.LibraryDB.PutBlob(pdf_document.Fingerprint, PDFDocumentFileLocations.INKS, data);
            }
        }
    }

#endif

}
