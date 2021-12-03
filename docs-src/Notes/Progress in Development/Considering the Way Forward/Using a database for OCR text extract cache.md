# Using a database for OCR / *text extracts* cache

Qiqqa classically uses a filesystem directory tree (2 levels, segmented by the first byte of the document content hash (a.k.a. *Document ID*), where each (PDF) *document* results in two or more files:

- 1 tiny text file caching the *document page count*.
- 1 or more text extract files (text format, but proprietary: each '*word*' in the text is encoded as a serialized tuple: `(bbox coordate x0, y0, w, h, word_text)`, resulting in a rather high overhead for the ASCII-text serialized *bbox* (bounding box) coordinates, using 5-significant-digits per coordinate, taking up 8 (text) bytes per coordinate for a positioning accuracy that's a little less that 32bit IEEE *float* -- plus it costs additional CPU serialization and deserialization overhead on write/read.

  An example record for an `ocr` file:
  
  ```
  0.29902,0.21465,0.21569,0.03535:Association

  ```
  
    > One of the ideas here is to use *native binary* storage data to cut down on the storage overhead: 4 *float*s would store the same *bbox* coordinates at slightly higher precision in half the bytes.
    
	Also note that "*one or more files*" is more specifically: one(1) file per 20 document pages, **iff** the original document has *text content*, either as an additional *text layer* or as *the original text* itself. When Qiqqa decides it needs to OCR a page to get at its text content, each *single page* will produce its own additional `ocr` text extract cache file.
	
---

I think we can do better than this by using a database, which stores these text extracts per page (or per document). 

When storage cost is considered*an added challenge*, then we might be better off storing the text extracts on a per-document level only (leaving the per-page handling to upper-layer custom code) so we can *compress* the document text using any compression library of choice (e.g. `lz4`  or `zstd`).



## Using a database for this: SQLite or something else?

As this is an item that is expected to need to be highly performant, *and* is very easy and basic re expected content queries, we are considering using a NoSQL database system instead.

These *content queries* are currently used in the Qiqqa codebase:

- get page count for document `D`
- get all text content for document `D` (to feed to the FTS engine, keyword analysis and auto-suggestion, topic analysis, FTS search query highlighting, ...)
- get text for page `X` of document `D` (Qiqqa PDF Viewer text overlay)

plus, of course, the insert/update actions.

There are no complex `SELECT` conditions used anywhere, nor any hard or soft range queries, so a very basic NoSQL API would already suffice.

---

Expected use is 1 or few *writes* -- there'll only be *updates* when OCR page actions output text data that *overrides* the default text extract, or when the user explicitly commands a re-do of the text extraction process, which is rare.

Meanwhile, the `ocr` cache is expected to serve *a lot* of *read* requests, as the text extracts are used for multiple purposes in Qiqqa, not just for display. (topic extraction, auto-suggesting title, author, etc. metadata, paper abstract extraction, feeding the (Lucene) FTS engine, etc.) 
And this set of usages is only expected to increase. (document similarity analysis, user-scripted meta-analysis of documents' content, etc.)



## Which were considered?

We've looked at several NoSQL databases (including our old friend `hamsterdb`, on which I worked some years ago), investigated performance reports, bug reports, maintainability and *current support*, and while several candidates looked hopeful for quite a while, the final verdict is: it's going to be LMDB or a flavour thereof, at least. I.e. a COW B+-tree (Copy-On-Write), preferably transaction-safe, database. 
High-write-overhead, i.e. so-called *log/write-optimized* (a.k.a. Log Structured Storage) databases are right out as they will thrash our storage (including SSDs) while not performing any better on reads. Hence LevelDB and any in its family are not to be used.

LMDB is transaction-safe, can be '*repaired*' rather swiftly and easily after a crash, and seems well supported throughout.

The fact that LMDB is crash/recovery-safe in its design *also* allows us to consider it for other, tightly related, *text extracts* storage: I intend to fit Qiqqa with the ability to *correct/augment* any machine-produced text extract, making this data not just *machine-produced* any more! 

Once this new *user effort* is enabled, the *document text extracts* are another of **valuable primary data source** from that moment on. Which would mean we need to treat differently: current `ocr` cache files can be killed with extreme prejudice as the machinery will be able to regenerate all of them. **That will no longer be allowed when there's *additional added value* in there due to user actions.**
Implying we either need to store the user-produced text extracts elsewhere too, or *elevate* the `ocr` cache to become another *treasured source* of data. Which precludes killing cache files at our whim. And adding them to the Sync & Backup processes too, or at least their *human-edited content parts* to ensure we don't loose that info.





## Further reading

- https://blogs.kolabnow.com/2018/06/07/a-short-guide-to-lmdb
- http://www.lmdb.tech/bench/microbench/ ( + its previous revision at: http://www.lmdb.tech/bench/microbench/july/)
	- http://www.lmdb.tech/bench/hyperdex/	
	- http://highlandsun.com/hyc/malloc/
- https://wenxueliu.github.io/blog/06/24/2014/lmdb-discussion (**particularly note the feedback notes at the bottom; the whole article is very relevant for us!**)
- https://mozilla.github.io/firefox-browser-architecture/text/0017-lmdb-vs-leveldb.html
- https://medium.com/walmartglobaltech/https-medium-com-kharekartik-rocksdb-and-embedded-databases-1a0f8e6ea74f
- https://stackoverflow.com/questions/40039230/how-to-add-compression-support-to-lmdb
- https://github.com/yinqiwen/ardb -- a general wrapper for many NoSQL databases so we can compare CPU and I/O performance
- https://github.com/fastogt/fastonosql -- ditto as `ardb`: a general wrapper for many NoSQL databases so we can compare CPU and I/O performance
- https://github.com/LMDB/lmdb
- https://github.com/erthink/libmdbx -- "*One of the fastest embeddable key-value ACID database without WAL. `libmdbx` surpasses the legendary `LMDB` in terms of reliability, features and performance.*"
- SQLite port to use LMDB: https://github.com/LMDB/sqlightning
- Further LMDB wrappers for various languages:
	- https://github.com/CoreyKaylor/Lightning.NET -- .NET
	- https://github.com/Spreads/Spreads.LMDB -- .NET
	- https://github.com/kwaclaw/KdSoft.Lmdb -- .NET
	- https://github.com/Venemo/node-lmdb -- NodeJS
	- https://github.com/DoctorEvidence/lmdb-store -- NodeJS
	- https://github.com/ahupowerdns/lmdb-safe -- C++
	- https://github.com/drycpp/lmdbxx -- C++
	- https://github.com/Instand/lmdbxx -- C++ with Signals (C++17)
	- https://github.com/kriszyp/lmdbx-store
	- https://github.com/ray2501/tcl-lmdb -- Tcl
	- https://github.com/shmul/lightningmdb
	- https://github.com/ChristophePichaud/LMDBWindows
- https://github.com/lmdbjava/benchmarks --> https://github.com/lmdbjava/benchmarks/blob/master/results/20160710/README.md
- https://github.com/kellabyte/rewind -- command log vs. WAL for LMDB
- https://github.com/hoytech/quadrable#lmdb -- LMDB used with Merkle Trees
- https://github.com/LMDB/memcachedb -- `memcachedb` on top of LMDB
- 



    
	
