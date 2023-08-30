# Using SQLite for the OCR and page render caches

Is it a viable option to use SQLite for these as well?

## No. 1: the work queue

As we would like to have a flexible, adaptive priority queue for the task queue, doing that one as an (indexed) database table seems like a smart idea: our tasks are relatively costly (render a page, extract text from a document, OCR a page (or set of pages), etc.) thus we don't fear the extra overhead of using a (pre-compiled) SQL statement or two to push and pop work items of our queue.

## No. 2: the page render output cache

When we task the PDF page render process to produce one or more pages for viewing (or OCR processing!), it can benefit us to cache these page images. (Less so with the OCR process requesting a page: after all, we expect that process to complete and never ask for that same page again as it would be *done*!)

Given that the page images are expected to be relatively large (several 100's of kilobytes a piece), even when WebP encoded, it might be smart to store these directly in the file-system and only store the cache info + file reference in the SQL table itself.

As these page images will be valid *beyond a single application run* it MAY be useful to make this a persistent cache, which can be re-used the next time the application is run.

> If we do that, keep in mind that actual *timestamps* are less useful as an indicator of service life of a cached image: a cached image is still useful when a user didn't run the application for an entire *week*, for example! Thus we should use a *render counter* as our *clock*: the further away the stored counter value is from the current value, the *older* the cached image is. LRU then means that we also update its counter=timer value to the latest when we fetch it from the cache instead of re-rendering it.

## No. 3: OCR text extracts cache (+ manual corrections)

This is a *long living cache*: it is so with the current Qiqqa (where it lives as a huge set of files in the `/ocr/` directory tree) and it should with any future update: the OCR / text extract job is a very costly one and the output is useful and stable for a long period of time.

Having a persistent cache for this data also enables us to add another feature on top of this: *versioning* for *content updates*: a user can then choose to **update / correct** the text extracts, either manually or through external means, and have those updates registered and stored in the persistent cache.

> Any such updates should also be forwarded to the FTS engine: when the document text is altered (for whatever reason, be it error corrections, augmentation or otherwise) this should be reflected in the FTS index so that future search activity by the user will produce matches against the latest texts.

Currently, OCR / text extracts are stored in single page and 20-page files and are several kilobytes large, also thanks to the relatively expensive ASCII text representation of the word bboxes used in the current format. When we store these numbers as either integers or IEEE 754 *floats* (32-bit floating point values), we will have both plenty precision and a much lower storage cost per word in our store. 

Still it would be a pending question whether to store this data as a BLOB inside the SQLite table or merely store a reference to the cache file in the database instead.


## Alternative systems

### Using a (modified & augmented) in-memory priority queue for No.1

While this would certainly work, the mix of both near-real-time viewing and batch processing tasks feeding into the queue is not a good fit with the classic priority queue algorithm. Also note that, particularly with large libraries such as mine, the pending tasks' list can grow *huge* and I've observed the .NET queue performance degrading severely.

Another alternative which has been considered here was to implement the custom (dynamic) priority queue as an in-memory B+-tree.

We may still decide to go with that last option instead of yet another SQLite table/database.
However, the argument for using SQLite here is not about raw performance but about reachability: the task list can then be queried (and adjusted) from external user scripting, once that feature becomes available. This can unlock modes of use that would not be possible otherwise, when we would use a (hidden) in-memory B+-tree queue.


## Using LMDB or hamsterdb (now *upscaledb*) for the key-value stores/caches

While those systems have been tested heavily and are pretty reliable, the argument for using SQLite once more is not about achieving the ultimate in raw performance but rather:
- easier *reachability* of these data sets for external scripts (the same argument as in the previous ("No.1") section)
- using a system that's vetted to survive severe system crashes, even when lots of updates/inserts occur. LMDB and upscaledb have been tested very thoroughly and are used in many critical systems out there, but their documentation does not provide the same level of trust as does SQLite, where quite a few blog articles, next to the manual itself, discuss this very subject. 
 
  > This would be particularly important for the OCR/text-extract "cache" as recreating that one is a very heavy burden on the entire system. 
  > 
  > Once the "manual update of extracted text / hOCR layer" feature is available, the cost of re-creating this table and content will include (a lot of) human labor as well, so ruggedness of the data store should trump raw performance in the scoring for selection for use in the code. SQLite is the only one of the options which has clear public documentation listing these claims and under what conditions those guarantees are available to us -- f.e. it's good for performance to switch the WAL to be in-memory and thus non-persistent across an application hard abort or system disruption; while using such a *tweak* (pragma) might be nice for the work priority queue and rendered page image caches, it *certainly* would be very wrong to apply that same performance tweak to the OCR/text extract cache table!
  > 
  > While I like upscaledb/hamsterdb a lot (and find the lmdb variants very intriguing, while I haven't used them yet for stuff like this with lots of writes happening over time), the arguments for reachability and ruggedness are winning. The additional concern here is that, even when SQLite would be a decade *slower* than LMDB or hamsterdb, this "mediocre" performance would probably go unnoticed against the costs of the tasks themselves: regular usage and various experiments with Qiqqa to date have shown that the task prioritizer/scheduler is a very critical component for the UX (including "freezing the machine due to all threads being loaded with work") while the data storage part went unnoticed in terms of cost, so I expect not having to go to extremes there to make it "fast": there's more to be gained in optimizing the work tasks themselves and the scheduler thereof: that is another reason why I prefer to use SQLite for No.1 now: it gives me options to easily adjust the scheduler mechanism when I want to/have to when performance is at a premium when we process/import large Qiqqa libraries, for example.
  


## References

- https://stackoverflow.com/questions/1711631/improve-insert-per-second-performance-of-sqlite?rq=1 -- the entire content should be read again; there's pretty important and very useful stuff in there!
- https://stackoverflow.com/questions/60674800/using-sqlite-as-a-file-cache
- 

