using System;
using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Files;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    class PDFInkSerializer
    {
        internal static void ReadFromDisk(PDFDocument pdf_document, PDFInkList inks, Dictionary<string, byte[]> library_items_inks_cache)
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

        internal static void WriteToDisk(PDFDocument pdf_document)
        {
            Dictionary<int, byte[]> page_ink_blobs = new Dictionary<int, byte[]>();
            foreach (var pair in pdf_document.Inks.PageInkBlobs)
            {
                page_ink_blobs.Add(pair.Key, pair.Value);
            }

            // We only write to disk if we have at least one page of blobbies to write...
            if (page_ink_blobs.Count > 0)
            {
                byte[] data = SerializeFile.ProtoSaveToByteArray<Dictionary<int, byte[]>>(page_ink_blobs);
                pdf_document.Library.LibraryDB.PutBlob(pdf_document.Fingerprint, PDFDocumentFileLocations.INKS, data);
            }
        }
    }
}
