using System.Collections.Generic;

namespace Utilities.PDF.Sorax
{
    internal class SoraxPDFRendererCache
    {
        private class SoraxPDFRendererCacheEntry
        {
            public int page;
            public double height;
            public byte[] image;
        }

        private static readonly int CACHE_SIZE = 3;
        private List<SoraxPDFRendererCacheEntry> cache_entries = new List<SoraxPDFRendererCacheEntry>();

        public void Put(int page, double height, byte[] image)
        {
            // Nothing to do if we have the Image, except perhaps store the latest version
            SoraxPDFRendererCacheEntry cache_entry = GetCacheEntry(page, height);
            if (null != cache_entry)
            {
                cache_entry.image = image;
            }
            else
            {
                // We have to bump someone from the cache...
                if (cache_entries.Count > CACHE_SIZE)
                {
                    cache_entries.RemoveAt(0);
                }

                // ...and add the new guy
                cache_entries.Add(new SoraxPDFRendererCacheEntry { page = page, height = height, image = image });
            }
        }

        public byte[] Get(int page, double height)
        {
            SoraxPDFRendererCacheEntry cache_entry = GetCacheEntry(page, height);
            if (null != cache_entry)
            {
                return cache_entry.image;
            }
            else
            {
                return null;
            }
        }

        private SoraxPDFRendererCacheEntry GetCacheEntry(int page, double height)
        {
            for (int i = 0; i < cache_entries.Count; ++i)
            {
                SoraxPDFRendererCacheEntry cache_entry = cache_entries[i];
                if (cache_entry.page == page && cache_entry.height == height)
                {
                    return cache_entry;
                }
            }

            return null;
        }

        internal void Flush()
        {
            cache_entries.Clear();
        }
    }
}
