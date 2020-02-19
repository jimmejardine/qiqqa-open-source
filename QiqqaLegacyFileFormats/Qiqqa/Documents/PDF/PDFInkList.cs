using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Ink;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Documents.PDF
{
    /// <summary>
    /// Holds all the ink paths that have been added to each page of the PDF document.
    /// </summary>
    public class PDFInkList
    {
        private Dictionary<int, byte[]> page_ink_blobs = new Dictionary<int, byte[]>();

        /// <summary>
        /// TODO: NOT threadsafe - should clean this up...
        /// </summary>
        public Dictionary<int, byte[]> PageInkBlobs => page_ink_blobs;

        public List<int> GetAffectedPages()
        {
            List<int> result = new List<int>(page_ink_blobs.Keys);
            result.Sort();
            return result;
        }

        public StrokeCollection GetInkStrokeCollection(int page)
        {
            byte[] ink_blob = null;
            if (page_ink_blobs.TryGetValue(page, out ink_blob))
            {
                using (MemoryStream ms_ink_blob = new MemoryStream(ink_blob))
                {
                    StrokeCollection stroke_collection = new StrokeCollection(ms_ink_blob);
                    return stroke_collection;
                }
            }

            return null;
        }

        // Deserialize an INK record from the DB:
        internal void AddPageInkBlob(int page, byte[] page_ink_blob)
        {
            page_ink_blobs[page] = page_ink_blob;
        }
    }
}
