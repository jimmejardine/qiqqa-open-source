# Progress in Development :: Considering the way forward : Qiqqa library storage, database, DropBox (and *frenemies*), backups and backwards compatibility

## Hashing and caching PDFs

### The MD5 hash that isn't

Currently Qiqqa employs a (botched) MD5^[or was it SHA1 after all? Check the sourcecode. Writing off the top of your head isn't always... uh...] checksum to **uniquely identify** every document stored in the Qiqqa library.

While the hash is botched (see source code comment about dropped '0's in hex strings for bytes' representation) it's okay enough to cope with a decent library, but given that there **do exist** PDF files which exhibit a **hash collision** for MD5, I think it's time to consider migrating that bit too, if only for the pathological case where I wish to be able to store both those PDFs and have Qiqqa identify them as *different*, like Qiqqa *should* but currently does *not*!

So what's holding us back from migrating to a, say, SHA256 based hash scheme? 

While I certainly like the hash-based scheme ^[similar to git: the **content** determines the key, not the filename that once was: 
that's become metadata to keep with the file in the database] for storage on disk, which is, in my opinion, 
a **whole lot better than storing PDFs by title or filename or whatever**, as there collisions and weird Windows (and Linux!) PATH & NAME limitations ^[ `:` colon in a filename on Windows, anyone?! ... Right! And how about maybe some `|` pipe symbols as part of a bit of TeX math in a filename-slash-title, then? Ooooh, Linux accepts but I can `| cat /etc/passwd` now, can't I?!] will cause all kinds of havoc,
the MD5 hash has outlived its welcome by 2020 AD.

Unfortunately, that hash is quite pervasive in the software ^[no complaint there, it's how I would have done it too! It's just that we now must consider how to be/stay backwards compatible so that any old Qiqqa DB can be picked up and imported into new Qiqqa, even when it's 2050 AD on the kitchen calendar at that moment.] and migrating to SHA256 or similar implies
that the entire database and PDF store must be *processed* to relate the old MD5 hash to the then new SHA256 hash.

New documents would not get a MD5 hash, or it would not be considered unique anymore, anyway), but everyone would be named using SHA256 and the database table in SQlite would need to be changed to use that SHA256 as a unique key.

Hence the thought is, here and now, to keep the old SQLite database table as-is, in case you want to migrate *back* to an older version perhaps, but to copy/transform it into a new table, where everything is using the new SHA256 key. **Plus** you'ld need a lookup table where MD5 is mapped to SHA256 and vice versa. [^1]

[^1]: Hm, maybe we could combine that the 'document grouping' feature I want to add to Qiqqa so I can 'bundle' PDFs for chapters into books and such-like. MD5 hash collisions would just be another grouping/mapping type. There's also [^to be, not implemented yet!] the decrypting and cleaning up and PDF/A text embedding of existing PDFs, resulting in more PDFs with basically the same content, but a different internal *shape* and thus different hash key.  All these should not land in a single table as they clearly have slightly different structure and widely different semantics, but it all means the same thing: there's some database rework to be done!


## Backups to cloud storage

Currently Qiqqa copies the Sqlite DB to cloud using SQlite, which is not very smart as this can break the database due to potential collisions with other accessors[^2]

[^2]: you or other user accessing the same cloud storage spot and thus shared DB over network, if only for a short moment]: the idea there is to always **binary file copy** the database to cloud storage and only ever let Sqlite access the DB that sits in local *private* storage.

Multi-user access over cloud storage is a persistent problem as there's no solid file locking solution for such systems: not for basic networking and certainly not for cloud storage systems (such as Google Drive or DropBox, which have their own proprietary ways of 'syncing' files and none of them will be happy with *shared use* of such files while they 'sync').


## Backwards compatible?

Qiqqa, with a new hash system like SHA256, should at least be **forward compatible forever**: old Qiqqa libraries, even when untouched for a decade or more, **must** be readable and importable into modern Qiqqa, whatever it may be at that moment in time. 

The ability to produce old-skool Qiqqa library DBs however, is only a *nice to have* as far as I am concerned.


### And how about co-workers then? Running disparate Qiqqa versions?

Yes, that *is* a problem now that Qiqqa Cloud, etc. is gone, so I'll have to come up with something new. Current ideas of mine would only work in some offices and are certainly not suitable for international, distributed co-workers, so we'll have to come up with something better.

How about... using github as a share base? Using a git repository? 

Well, that sounds nice on paper, but given a library running at 50K+ articles and books and such, clocking in at over 40GB storage for Qiqqa DB + PDF storage cache + Lucene Index, we might want to reconsider that one, perhaps.

Torrent-based sharing schemes, maybe? How about a git repo for the metadata, document **hashes** and torrent spec files only -- that would give us git-like management over the library metadata and the PDF documents would follow as we've got their hashes and torrent specs on board, so as soon as the other party comes on-line again, we can continue syncing. Added benefit then would be that we don't have a huge git repo with yet another copy of every PDF revision in that huge set, so no GigaBytes but MegaBytes only for the git part. And the big stuff, the PDFs, get distributed across the network as co-workers join. ^[ as with all things distributed and DVCS and such, when those co-workers **leave** you always have a **security problem** as they will have a full copy of the last state of the database from the last day they were still part of the team. Add 'torrents' to that scenario and how are you keeping 'outsiders' from grabbing your private team stuff when they already have access to the torrent hashes?

The alternative there would be a (semi-)continuous backup and restore-equals-**merge** type of cloud storage sharing of binary files, including the metadata database itself: 'sync' would mean you check the cloud store, see if any of the stuff is newer/not-what-you-have and then **import/merge** that into your local copy. Second stage would then be to 'backup' your local copy into the cloud, updating everything that's not already identical to yours. `rsync` on steroids? That way, privacy / security management remains outside Qiqqa and wholly inside the realm of your cloud storage management (sharing, etc.), while the DVCS-style drawback remains: you cannot ever revoke a **local copy** if you fire someone from the team. The other troubling bit that remains there is the bit about the 'sync' itself: storing your local state to cloud **must not be assumed to succeed**: when another individual (or you, running Qiqqa on another machine at the same time!) happens to *push* at the same time, their copy, or at least part of it, **may win over yours**.
Hence a 'sync' process would go something like this:

1. import from cloud by **merging**: keep what you have, add what's new.
2. export to cloud
3. check cloud copy: if not identical to your local one, rinse & repeat from step 1 onwards until you're either okay or fed enough to quit doing it today.

Note that cloud storage is not transactional; the only guarantee that DropBox, Google Drive, etc. give ^[oh yeah? You've read it on SO and elsewhere, but do we have *proof*?] is that a single file copy will be transactional, hence when two or more nodes are writing the same file, only one will persist *and it will persist in its entirety*: files will never become fragments gathered from multiple simultaneous uploads.

Hence we might conclude that step 3 is sufficient to provide a safe and repeatable sharing function **iff step 1 acts as described** -- which it does not. At least not today!
We have currently no way to detect which metadata entry (local or cloud) is the more recent / more valid one and we always pick the cloud copy. ^[ check the source code. Anyway, even if this line is wrong, it's still trouble as the point is: Qiqqa cannot determine which entry **should** win, be it ours or theirs.]
Given that our step 2 **can fail**, while we do **not** have a reliable network-wide clock, there's always the risk of losing records during sync... unless we introduce some sort of record update versioning scheme, i.e. unless we more to a metadata store that has features similar to `git`: if we encounter any edit history that is not ours, we need to merge it. And then, in those rarest of cases, we of course hit the same snag as `git`: a merge collision. Which requires user activity to 'merge' correctly. So... should we simply go and use git for meta storage in the cloud sync space there at least, or do we code our own crude re-imagining of a git-like VCS there?
It's bothersome, either way. Still, better than git+torrents as that was a cool idea just there but fraught with security/privacy issues re *outsiders*.

---

More to come when it crosses my mind again.
