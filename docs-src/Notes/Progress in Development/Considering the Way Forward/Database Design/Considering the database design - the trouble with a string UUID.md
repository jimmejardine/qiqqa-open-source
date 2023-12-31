# Considering the database design :: the trouble with a string UUID

Presently (and in the future) Qiqqa identifies any document using a hash-based UUID. Up to now this is a SHA1B content-based hash; a new BLAKE3 content-based hash has been selected to replace this flawed UUID everywhere. 

Where needed/applicable, the 256 bit BLAKE3 hash will be encoded to *string* using a custom BASE58X-based encoding scheme, producing a *string* key length of 44 ASCII characters, all in the regular alphanumeric range, hence easy to parse and process in any system, including SQL databases.

We use SQLite, which can handle binary numeric data up to 64 bits, but, like almost all other SQL databases out there, it cannot process larger binary numeric values *directly* for *primary key* columns, hence the *string* variant should be used instead. This is what is currently done in Qiqqa, which stores all document-related records in a single table, keyed by the string-typed SHA1B UID.

## How important is key *size*, really?

Currently, all records use the SHA1B *string*-typed UID, which is variable length, but basically a hexdumped SHA1 at 160 bits, thus $160/8 \times 2 = 40 \text{ characters}$ wide, thanks to its hex encoding. 

> See also [[../Fingerprinting Documents/Fingerprinting - moving forward and away from b0rked SHA1]].

With the new BLAKE3+BASE58X-based scheme, that *string*-typed key would be a *fixed-sized* 44 characters wide. A 10% size increase.

> Using the BLAKE3 hash directly is not possible in an SQL database, as it's a 256 bit binary number. This takes up $256/8 = 32 \text{ bytes}$ in the machine, but SQL cannot 'talk' 32 byte binary for key / indexing columns as those cannot be BLOBs.
> 
> *However, SQLite **can** use BLOBs as primary keys!* See these references:
> - [Will I run into performance issues if I use a BLOB field as primary key in SQLite?](https://stackoverflow.com/questions/1562731/will-i-run-into-performance-issues-if-i-use-a-blob-field-as-primary-key-in-sqlit)
> - [Can I use anything other than BIGINT as Primary Key data type in SQLite?](https://stackoverflow.com/questions/3001093/can-i-use-anything-other-than-bigint-as-primary-key-data-type-in-sqlite)
> - [Using UUIDs in SQLite](https://stackoverflow.com/questions/17277735/using-uuids-in-sqlite)
> - [How to efficiently insert and fetch UUID](https://stackoverflow.com/questions/11337324/how-to-efficient-insert-and-fetch-uuid-in-core-data/11337522#11337522)
> - [Datatypes In SQLite](https://www.sqlite.org/datatype3.html)
> 
> Ergo: now that we have discovered that SQLite differs from our usual SQL databases in that it fully supports BLOBs as primary key field type, the question then gets raised: "**What's the use of BASE58X, after all? Won't `SELECT hex(blake3) FROM document_table` do very nicely for all involved, including future external use?**"
> 
> Erm... Good point. Further checks into the SQLite API show that I can feed those BLAKE3 hashes verbatim using query parameters and the appropriate APIs:
> - [Binding Values To Prepared Statements](http://www.sqlite.org/c3ref/bind_blob.html) -> `sqlite3_bind_blob()` and `sqlite3_bind_blob64()` 
> 
> This would mean the only reason to employ BASE58X is to keep transmission costs low (storage being covered by this ability of SQLite to store BLOBs as primary keys), but does this weigh against using the default `hex()` encode already offered by SQLite? (`select hex(blake3) from ...` vs. custom function registration requiring `select base58x(blake3) from ...`)

Given the note above, *not using any string encoding at all* is a viable option while we're using SQLite. This would mean our BLAKE3 hash can be used as a key of length $256/8 = 32$ bytes. Which is *even shorter* than the original (*string*-typed) SHA1B hash (at 40 characters long).

Do we mind when external users need a human-viewable string-typed version of the identifying hash: are we okay with a hexdump then ($256 / 8 * 2 = 64 \text{ characters}$) instead of our custom, shorter BASE58X scheme, which clocks in at only 40 characters?

**While I like BASE58X, it looks like it's time to *kill my darlings* today. Thanks to an unexpected feature of SQLite: direct BLOB-as-key support.**

Which leaves us the other part we came here to consider:


## Which is 'better': using our BLAKE3 UID as document key everywhere in the database, or do we `JOIN TABLE` with a mapping table while using fast 64-bit *integer* *rowids* instead?

Storage cost-wise, we can say this: iff we create the mapping table with a `INTEGER PRIMARY KEY` (which would act as a `rowid` alias in SQLite) and a separate index on the BLAKE3 UID BLOB column, then we would, at least theoretically, get the same query performance as when we were to use the UID as key in every table instead: SQLite would then first lookup the *rowid* and retrieve the record then, while, with an `INTEGER PRIMARY KEY` approach used for those table(s) *plus* the mapping table, we'ld have the same number of lookups: UID -> index -> rowid -> table access -> record row.

Of course, we'll only know when we create a performance test and check which one will be faster and by how much.

References:
- [INTEGER PRIMARY KEY vs rowid in SQLite](https://stackoverflow.com/questions/50686224/integer-primary-key-vs-rowid-in-sqlite)
- [Clustered Indexes and the WITHOUT ROWID Optimization](https://www.sqlite.org/withoutrowid.html)
- [CREATE TABLE :: # ROWIDs and the INTEGER PRIMARY KEY](https://www.sqlite.org/lang_createtable.html#rowid)
- [Using UUIDs in SQLite](https://stackoverflow.com/questions/17277735/using-uuids-in-sqlite)

At least *storage-cost-wise*, the latter approach (mapping table + INTEGER index) will consume less storage space when we assume our document will have several metadata records attached: than would be $1 \times 32 + n \times 8$ vs. classic $n \times 32$ for the key storage alone. However, keep in mind that the gains will be meager, compared to the overall size of a *library*, including all those stored PDF files. (Let alone OCR text extracts and FTS reverse index for Lucene/SOLR.)

Meanwhile, the queries might become a little more complicated when we need to start searching from the BLAKE3 UID, *and/or* the rest of our software would become more complex when we decide to use the `rowid` in there as well: it's handy to have a 64 bit integer value as a unique document identifier in your code (`uint64_t`) but there's one drawback:
that `rowid`-based "unique id" would be *very local*: it will only apply to the document on this machine!

Imagine we work on 2 machines and sync/merge a library: then we can be sure the rowids produced at both sites will not have known about one another just yet, so having added a few documents at both sites and only then syncing the library for the first time would cause a *rowid collision* if we were to carry those across machine boundaries *sans prejudice*: the same *rowid* will have been assigned to two different documents!

We *can* try to alleviate this issue by using a *64 bit folded version* of the BLAKE3 content hash, but that would degrade our uniqueness guarantees for that hash value to $2^{\frac{64}{2}} = 2^{32} \approx 4.0e9$ thanks to the ubiquitous *birthday paradox*. Better not go that route as it would be *just sufficient* to hide bugs in any collision resolution code we have to write then anyway.

Consider also the *sync target database*, i.e. the *library copy* at the remote storage location where we will be *syncing to*: already that library database (a `*.s3db`  in Qiqqa's case) *may* have been updated by another site/machine in between two sync actions from our machine, resulting in other documents already having been added there and certain *rowids* already having been assigned to those newcomers, **without us knowing about this**: sufficient cause for another case of *rowid collision*. 

Thus we conclude that we must, at least during any and every *sync operation*, always only refer to document data using the BLAKE3 UID; rowids are relegated to the machine *internal software*'s use (where having a `uint64_t` as an identifier can be a nice to have).

Summary: when we go the 64-bit *rowid* route, we can use those *only within a single **zone**,* where we identify these *zones*:
- all Qiqqa software running on a single machine, i.e. 'the database + UI software' the user regularly interacts with,
- the *sync destination path* for any library that the user synchronizes, even if that path is only a local drive or a USB stick or whatever: we do *not* have **permanent control** over that one as it will bee used to transport data back and forth between nodes and/or people.
- Ditto for any remote site where you or anyone else uses the software: each of those is to be considered a separate **zone** as well.

**Library synchronization** then becomes as slightly more complex task as next to document and metadata synchronization, that part of the software will also have to contend with crossing the *zone boundary* and thus be able to detect and resolve *rowid collisions*: the only viable way there is to treat *rowids* as *immutable in the zone* once they are assigned, hence any collision is to be resolved by mapping the incoming one to a new, unique *rowid*.

> That's the technical way of putting it when we said all data must be referenced against the original BLAKE3 unique content hash identifier during any synchronization operation.
 
Now *that* is a hint that during synchronization at least, we will be doing a lot of `INNER JOIN`s with the BLAKE3-to-rowid mapping table in both our own and the sync destination library database! 
This would make synchronization a slightly more *expensive* operation, but we expect sync actions to happen much less often than any other library/database action/query which happen while the user is perusing the library software.

What are the risks and can we speed this up?

As we do not intend to offer a 'real time' interaction/cooperation mode in Qiqqa, we will always only be accessing 2(two) *zones* at most: our own, local, zone and the *sync destination*. Thus a simple mapping can be created where we can list BLAKE3 UIDs next to our own *rowid* and *remote rowid*: a simple three-column lookup table that we might consider *caching* in our sync software for speed and simplified query usage.

As we treat a *synchronization action* as *atomic* (nobody can access the *sync dest library* while we are busy working on it), there's no risk in using a cache or other means to speed up the process: we could start by mixing the two zones' UID-to-rowid mapping tables and update that table both locally and remotely before we do anything else, thus producing a 100% complete mapping for all known documents in both zones. Then we would have all the rowids necessary in both zones to merge the databases and transfer document information from one zone to the other, using the appropriate rowids *for that zone*.

Doesn't this make things quite confusing for humans? The above means that a single document will have different rowid identifiers in every different database!
Yes, it might be confusing at first sight, but only tech-savvy users who wish to access those databases directly will be able to observe this. For them, there's the `inner join` with the BLAKE3-to-rowid mapping table in their local database to stabilize the results across zones.

By the way: this also means a **backup** also is *crossing a zone boundary*: we must treat a database recovered from a backup like any other *sync target*: only when we have *nothing* in our own database yet, do we accept the rowids from the backup. However, I feel it's safer to always assume we're restoring the backup onto a system that already has some (minimal) content stored (and thus *rowids allocated*), hence we would need to perform a *sync operation* to *recover the backup*.

> In actual practice, current Qiqqa always fills a fresh database with two sample documents, so that "we're not recovering into a pristine, empty target" stuff above wholly applies: it's a sync action, rather than a classic recovery/restore action.
> 
> It is only a recovery/restore action when we restore all the files as is into a completely empty library location. That can happen and is good to have, but keep in mind that "partial backups" and other such *slight alterations* to the idea of backup/restore activity all will imply they're *sync actions*, with a different label perhaps, but *sync actions* nevertheless.

