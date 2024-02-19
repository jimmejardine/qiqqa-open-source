# Old single-table NoSQL-like vs. new multi-table approach :: what's speaking against going Boyce-Codd on this one?

The original Qiqqa database layout was simple, rather *crude*: all data is stored in a JSON record, which is hash-integrity checked and stored in a single database row: key/value lookup and all you'd get is a JSON field value, that itself represents an entire JSON file, which *includes* the whole document BibTeX record as one of its JSON-encoded fields. It's a bit nasty, really.

While I haven't coded it yet, I'm pondering the database design which should follow this one, where I hope to provide a better service for these audiences:

- the Qiqqa software itself
	- not just faster query processing (Qiqqa originally cached *everything* so that will be hard to beat), but, more importantly,
	- the ability to cope with *large* and *huge* libraries without *Out-Of-Memory* failures thanks to the inability-by-design to discard cached data: Qiqqa is quite instable when libraries near or pass through the 5-20K documents 'ceiling' and with the new design I **must** facilitate my own libraries at least, which clock in at between 40K and 100K, depending on which one you're looking at (real vs. "evil base" test lib)
	- easily and quickly identifying various metadata parts, which we:
		- show on screen (Qiqqa UI, now with much less caching and thus lower memory pressure)
		- feed into our FTS (Full Text Search) solution, e.g. SOLR, when we want to update or augment a search record in there.
- users who wish to perform meta-research and query/access the Qiqqa-collected and managed data through scripting or other means, i.e. folks who like to dig into their Qiqqa library databases and do some Big Data research on that, say? Jolly good!

  These users would want the database to be easy to query and specific data to be easy to obtain (decoding a JSON record and then digging through an obnoxious, vary-formatted, BibTeX record inside that one is not for the faint of heart).

- Also note that I expect these users to want to access our FTS (Full Text Search) solution without Qiqqa sitting in between, e.g. when they have custom search queries that Qiqqa is not (yet?) geared to facilitate: I expect power users to find creative ways to use our collected text data and metadata, which should all be searchable via our FTS solution (SOLR?) - such a subsystem is expected to spit out document ids (*that's one place where BLAKE3+BASE58X comes in handy: SOLR doesn't do BLOBs like SQLite does: [[Considering the database design - the trouble with a string UUID]]*) which a user can then feed into other Qiqqa subsystems to produce the documents and/or database metadata records involved, to be used in any way they fancy.

## Storing every metadata *field*+*value* as a separate record?

While that would be pretty close to [Boyce-Codd NF](https://en.wikipedia.org/wiki/Boyce%E2%80%93Codd_normal_form) ([3NF](https://en.wikipedia.org/wiki/Database_normalization)), it also means we'll be expecting a *lot* of metadata records per document: say an average BibTeX record carries 5-10 fields, plus the surrounding Qiqqa-specific JSON record carrying another 20 (which will increase as we intend to improve the metadata handling), we're looking at a rough 30 records per document, *easily*. 

Then there's plenty folks who *annotate* their documents and those annotations are another type of metadata, which is currently dumped in separate records already, but still gathered together, so 1 record per document at the moment, but were we to split those up for easier external access, then we can expect 10s of records more.
Add tags and rankings (currently hiding in the large JSON record blob, mostly) and you are looking at *what*? 100+ records per document?

How does that perform when we want to fetch it all, or at least a large chunk of it, for multiple documents? Versus an everything-dumped-into-one-ugly-field? 

We *would* be looking at a far more critical SQLite database performance, while our FTS/SOLR system would benefit form at least being fed those fields separately: that way users can search for hits in specific fields and thus dig through their collections in a more precise and useful fashion. Meanwhile, our database doesn't necessarily have to have each line stored separately...

Storage-wise, there's not a lot of difference: say we have 200 fields per document, then the original approach is 1 record per document, now 200. Assuming we go for the *rowid* indexing approach (see [[Considering the database design - the trouble with a string UUID]]), then the added cost would be SQLite storage overhead (about 1 byte per column IIRC) plus *rowid* (64 bit integer), hence an extra $200 \times 1 + 8 = 208 \text{ bytes}$ extra storage space per document required. *but that's not counting the increased index space plus the possibly reduced performance as it'll have to filter through several records per document*: 3NF or not, we already **know** we'll have to cope with custom BibTeX records/fields, hence a fixed field layout (where every metadata field known to man has its pre-assigned place in the schema) is out of the question, so we'll have *at least* 1 table with a *compound key* `(document, metadata_fieldname)` and a suitable *search index* added for querying that one.

Hence it *might* be beneficial to stick closer to the "*flattened table*" approach already in use and just get rid of the JSON+BibTeX mess in there: either make it all UTF8 JSON or some other easy-to-parse format, we can feed our data fields to SOLR as separate entities still, while keeping them together in a single (or only few) metadata records in the SQLite database, just to keep our indexes performant under pressure.

The trouble with that approach, though, is that nobody can easily search the database for matches against a particular metadata field, f.e. `title`, any more. All such searches are then, by necessity, relegated to the FTS/SOLR system, which might not be the optimal choice for searches like that?

> XML?! I don't like it, as a format, but it's an option... It saves on 'escapes' in string content, but then goes and bloats the entire thing with `<tag>...</tag>` repetition cruft.
> 
> While we *ho and hum* about it, we realize that we've been looking at the venerated `bibutils` package already and were intent on repurposing that one for our needs: when we go that route, the decision has already been made for us pretty much as the *common denominator format* used by `bibutils` internally is the [US Library sanctioned MODS format](https://www.loc.gov/standards/mods/userguide/index.html), which is... XML based.
> 
> Of course, we could be a dork and turn that one into our own JSON-or-whatever-based format, just out of spite for XML overhead (*oooh! oooh! BXML! Binary Encoded XML! Yay!!!1!*), but I keep thinking the `bibutils`'s MODS format is a nice one covering it all, including any custom fields and other special sauce we may encounter along the way. (US Library doesn't recognize `@datasheet` , nor does LaTeX or BibTeX, but **me, myself & I** certainly *do*! Those are an important part of the document collection over here, as are **application notes**!)
> 
> Cost increase estimate? At 200 fields, with field names averaging, say, 20 characters, the XML-based overhead will kick in at an *extra* $(2+2+20) \times n = 24 \times 200 = 4800 \approx 5 \text{ kByte}$ overhead per document. That's becoming a bit steep to my tastes (while I *utterly ignore* the 16TB storage disk that's inbound to these premises this week) as that would run us up an additional ${5\text{K overhead}} \times {100\text{K documents}} \approx 500 \text{MByte}$ overhead cost. **Whut?!**
> 
> Hold on! Let's get this into perspective: at a 100K document count, the *documents* themselves would clock in at, say:
> - 400KByte per document *on average*; 
> - add OCR text extract, now in a weird text+coordinates format, but intended for those evergreen hOCR/HTML pastures, *adding that same amount* once more for text, layout/position coordinates info and figures/images, etc. sounds about right, 
> - plus maybe an *abstract*, a review or summary, all at a grand 1K 
> - and the rest of the collected/extracted metadata and annotations + tagging besides, themselves clocking in at said 200 fields, each with an averaged content length of, say, 42 characters, perhaps?
> That would give us a per-document cost of *non-origin data* (everything but the document itself) of ${400K} + {1K} + 200 \times 42 \approx {409K}$ *per document*, hence a grand total of $({400K} + {409K}) \times {100K} \approx {81\text{GByte}}$ storage cost for the document+(meta)data itself, *sans overhead*?  
>
> > Maybe a tad clearer would be to round it up a bit and state the storage cost estimate as $N \times (2 \times D + {10K})$, where $D$ is the average document size and $N$ the number of documents. For $D = {400K}$ and $N = {100K}$, that would then be ${100K} \times (2 \times {400K} + {10K}) = {82 \text{GByte}}$. Check.
> 
> Thus, using the rough estimate above, the MODS XML overhead would clock in at less than 1% of the total cost.
> 
> Of course, when we (correctly) disregard the document + hOCR content (the $2 \times D$), the number gets rather more bleak, thanks to our estimated average MODS field name of 20 characters vs. content average length of only 42: the *extra* overhead, thanks to the XML format, would then be near 25% of the total. (1 fieldname: necessary; content itself is about the size of 2 average field names. Then the XML closing tag *repeats* the field name, utterly useless. That's $\frac 1 4$ of the total as **extra cost** due to using XML rather than JSON or something similar and non-repetitive.)
> 
> When observed from *that* angle, using MODS as-is for storage isn't a very bright idea. Besides, most folks out there are used to munching JSON these days, so why not store it in JSON format while keeping it otherwise the same?
> 
> > Meanwhile, we can ponder about using *solid archive' style compression systems* if this amount of storage is really going to bother us.
> > 
> > **Info Datum**: while I am backing up my current raw document bin (which includes a metric ton of duplicates and is otherwise also *not cleaned up at all* -- I want my tools to do that for me, as that activity doesn't scale when done manually), the backup tool currently reports a disk space usage of about 800GByte for the documents alone! 
> > 
> > Keep in mind, though, that a very pedantic duplicates removal tool has already dialed that number down to less than 500GByte *actual on-disk storage cost*, thanks to a zillion hardlinks to *exact duplicates* -- this dump bin includes all my old Commercial Qiqqa crashed library copies, which still need to be integrated into a new library: Qiqqa has **not** been able to do this for me without fatally crashing, thanks to hitting the .NET 1.5GB RAM limit, thanks to a 32bit application and the shitlist of issues (see github) which prevent me from easily changing that with the current codebase. Fancy a bloodcurdling scream, anyone? I've got one ready for you; all I need is to remind myself of this situation...
> > 
> > Anyway, that Info Datum implies that my document size estimate is either way too optimistic / small, or-r-r-r-r-r, we are looking at far more than 100K 'unique' documents here.
> > 
> > Remember the "*near-identical PDFs cluttering the library*" section in [[../../../Qiqqa Internals/SHA1B - Qiqqa Fingerprint 1.0 Classic]]? There are several reasons this dump bin has far more than 100K 'unique' documents, thanks to multiple download actions and crashed library recovery attempts, combined with several rather unsatisfactory attempts at improving OCR output, meanwhile 'deprotecting' obnoxious PDFs (I'm looking at you, On-Semi!), plus *possibly* a late realization that even though the previous factors are all very significant, there also *might* be a chance that the actual library hiding in that bin has grown at tad by now, say, another 25% since I last measured this in 0 B.C. (the Year Zero Before COVID) - 25% sounds quite reasonable given what I've been doing lately and the fact that this lib has been growing at a steady pace since at least 2015, with plenty of stuff already collected before then! This is about 30 years worth of professional living, after all.)


## Some thoughts on (database) speed

If we were to store each metadata field separately (for easier *targeted* search queries), the indexes would also grow notably. Considering that, using the FTS/SOLR system to help with these types of searches is perhaps not such a bad idea.

Meanwhile, having a non-unique field index on the metadata fields table, would *theoretically* perform only slightly worse if we are to load that table with a load of fields per document: SQLite indexes are [B+ tree](https://en.wikipedia.org/wiki/B%2B_tree) based, hence a search takes $O(log_b(n))$ 'operations' (B-tree node visits ~ disk section visits): with the number of fields estimated, I expect (with a 4K sector size) that number to be near $b$, hence only adding $O(+1)$ to the cost above, which is not negligible, but possibly acceptable / tolerable.

> Note that the FTS/SOLR system would face similar $O(...)$ odds, so it's not a clear-cut case yet -- future practice will tell us what we could have considered and decided better here.


### Maybe a bit of duplication isn't so bad...

Another idea is to store the metadata *twice*. Heck, it's already *encoded once more* in the FTS/SOLR system to facilitate "google style" searching there, and at an estimated cost of ${ 10K } / D = 10 / 400 = 1 / 40 \approx { 3{ \% } }$ of $D$, *doubling* that cost isn't high impact!

What about we store those metadata fields in one table, all fields a separate records, thus producing a huge search table with a compound index on `(document, fieldname)`, while the "*I want it all*" folks among us can query the second table where this data is stored in a single record, just like before, only now cleaned up and easier to parse: all the stuff together in a single field, JSON-formatted [MODS](https://www.loc.gov/standards/mods/userguide/index.html), ready for quick and easy use? I think that might be good and is relatively easy to do.

> #### Two questions remain
> 
> 1. **What about backwards compatibility? Aren't we already dumping that data in a single record for that purpose?**
>   
>   Yes, we are. But that table will contain those pesky old-style, hard-to-parse JSON+BibTeX records, which are only good for when you wish to revert to an old Qiqqa version for some reason. That's not something you want to visit upon your (power-)users to process as a query result. So, *yes*, in fact we are then **triplicating our metadata** instead of merely *duplicating* it. Ah well, what's an extra $10K$ per document, eh?
>    
> 2. **In case of a corruption, which of them will be leading?**
> 
>   Good question. While SQLite advertises the fact that their database cannot corrupt (easily), there's the **application crashes** that would bother me more: either we then go and do all this updating as [SQLite transactions](https://www.sqlite.org/lang_transaction.html), or we hedge our bets and pick the latest record(s): remember the old format already contained an (unused!) last-updated `datetime` timestamp for every metadata record! 
>   Allow me some hand-waving here. No bright sparkly ideas yet; *we get there when we get there*, I guess.


## References
- [Standards  |  Librarians and Archivists  |  Library of Congress (loc.gov)](https://www.loc.gov/librarians/standards)
 
