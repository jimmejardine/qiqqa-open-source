using System.Collections.Generic;

#if !HAS_MUPDF_PAGE_RENDERER
namespace Utilities.PDF.Sorax
{
    internal class SoraxPDFRendererCache
    {
        private class CacheEntry
        {
            public string filepath;
            public double height;
            //public long last_use_time_tick;     // when entry was requested for the last time
            public int page;
            public byte[] image;
        }

        // one major cache slot per file stores all rendered pages at highest resolution (height) only
        private class FileCacheHashSlot
        {
            public Dictionary<int, LinkedListNode<CacheEntry>> pages = new Dictionary<int, LinkedListNode<CacheEntry>>();
        }

        private static readonly int CACHE_SIZE = 153;
        //private static long current_use_time_tick = 1;  // counter representing "current access time" in the cache.

        private Dictionary<string, FileCacheHashSlot> cache_entries = new Dictionary<string, FileCacheHashSlot>();
        // We keep track of 'most recent usage' in the timeline double-linked-list below.
        //
        // This structure has the added benefit of allowing us to update cache entries to be updated swiftly
        // while they are accessed (updating their 'last used' "timestamp") and quickly added and removed from 
        // the cache as they arrive and expire.
        private LinkedList<CacheEntry> cache_timeline = new LinkedList<CacheEntry>();

        public void Put(string filename, int page, double height, byte[] image)
        {
            // Nothing to do if we have the Image, except perhaps store the latest version
            CacheEntry cache_entry = GetCacheEntry(filename, page, height);
            if (null != cache_entry)
            {
                cache_entry.height = height;
                cache_entry.image = image;
            }
            else
            {
                // We have to bump someone from the cache...
                //
                // cache_timeline.Count is O(1): https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.linkedlist-1?view=netframework-4.8
                // --> "Because the list also maintains an internal count, getting the Count property is an O(1) operation."
                if (cache_timeline.Count > CACHE_SIZE)
                {
                    // get the oldest slot and discard it.
                    // remove it from the "timeline", i.e. the age monitor.
                    LinkedListNode<CacheEntry> oldest_entry = cache_timeline.Last;
                    cache_timeline.RemoveLast();

                    // also remove it from the file->page->image cache
                    CacheEntry oldest = oldest_entry.Value;
                    Dictionary<int, LinkedListNode<CacheEntry>> pagecache = cache_entries[oldest.filepath].pages;
                    pagecache.Remove(oldest.page);
                    if (pagecache.Count == 0)
                    {
                        // drop the entire file slot:
                        cache_entries.Remove(oldest.filepath);
                    }
                }

                // ...and add the new guy
                FileCacheHashSlot fileslot = null;
                if (!cache_entries.TryGetValue(filename, out fileslot))
                {
                    // we're adding the first page for this file, hence we must create a new file slot first:
                    fileslot = new FileCacheHashSlot();
                    cache_entries.Add(filename, fileslot);
                }

                // there MAY be a page slot already present for SMALLER height/image rez: REPLACE that one if it exists:
                LinkedListNode<CacheEntry> pageslot = null;
                if (fileslot.pages.TryGetValue(page, out pageslot))
                {
                    CacheEntry data = pageslot.Value;
                    data.height = height;
                    data.image = image;

                    // and move the 'last used time' forward:
                    cache_timeline.Remove(pageslot);    // cost: O(1), due to linked list!
                    cache_timeline.AddFirst(pageslot);  // plus: O(1), due to linked list!
                }
                else
                {
                    CacheEntry data = new CacheEntry()
                    {
                        //last_use_time_tick = current_use_time_tick,
                        filepath = filename,
                        page = page,
                        height = height,
                        image = image
                    };
                    // add to front of timeline (front is most recent):
                    pageslot = cache_timeline.AddFirst(data);

                    // inject it into the page hash table for this file
                    fileslot.pages.Add(page, pageslot);
                }

                // and move the current time forward
                //current_use_time_tick++;
            }
        }

        public byte[] Get(string filename, int page, double height)
        {
            CacheEntry cache_entry = GetCacheEntry(filename, page, height);
            if (null != cache_entry)
            {
                return cache_entry.image;
            }
            else
            {
                return null;
            }
        }

        private CacheEntry GetCacheEntry(string filename, int page, double height)
        {
            FileCacheHashSlot fileslot = null;
            if (!cache_entries.TryGetValue(filename, out fileslot))
            {
                return null;
            }
            LinkedListNode<CacheEntry> pageslot = null;
            if (!fileslot.pages.TryGetValue(page, out pageslot))
            {
                return null;
            }
            else
            {
                // there's an existing entry already: check if it's suitable before we rejoice
                CacheEntry data = pageslot.Value;
                if (data.height < height)
                {
                    return null;
                }

                // move the existing entry to the front of the timeline to signal its 'recent usage' now.
                //
                // GET is usage! (As is PUT.)
                //data.last_use_time_tick = current_use_time_tick;

                cache_timeline.Remove(pageslot);    // cost: O(1), due to linked list!
                cache_timeline.AddFirst(pageslot);  // plus: O(1), due to linked list!

                // and move the current time forward
                //current_use_time_tick++;

                return data;
            }
        }

        internal void Flush()
        {
            cache_timeline.Clear();
            cache_entries.Clear();
        }
    }
}
#endif
