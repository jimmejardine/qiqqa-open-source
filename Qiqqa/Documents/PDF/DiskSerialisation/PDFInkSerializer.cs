using System;
using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF.ThreadUnsafe;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.DiskSerialisation
{
    internal class PDFInkSerializer
    {
        internal static void ReadFromDisk(PDFDocument_ThreadUnsafe pdf_document, PDFInkList inks)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                byte[] inks_data = null;
                List<LibraryDB.LibraryItem> library_items = pdf_document.LibraryRef.Xlibrary.LibraryDB.GetLibraryItems(PDFDocumentFileLocations.INKS, new List<string>() { pdf_document.Fingerprint });
                ASSERT.Test(library_items.Count < 2);
                if (0 < library_items.Count)
                {
                    inks_data = library_items[0].data;
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

        internal static void WriteToDisk(PDFDocument_ThreadUnsafe pdf_document, bool force_flush_no_matter_what)
        {
            if (!force_flush_no_matter_what)
            {
                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            }

            byte[] data = pdf_document.GetInksAsJSON();

            if (null != data)
            {
                pdf_document.LibraryRef.Xlibrary.LibraryDB.PutBlob(pdf_document.Fingerprint, PDFDocumentFileLocations.INKS, data);
            }
        }
    }
}
