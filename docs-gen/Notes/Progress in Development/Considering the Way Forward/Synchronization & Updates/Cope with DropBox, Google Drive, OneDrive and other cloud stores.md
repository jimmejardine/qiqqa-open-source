# Cope with DropBox, Google Drive, OneDrive and other cloud stores

Those are known to cause havoc when writing databases (SQLite) to them on the fly.

I wonder how this can happen, as it's a regular noise I hear on the Internet, but I can only imagine easy fault modes when you're accessing the same file from multiple locations: *multiple writers*.

However, SQLite and others have pages about warning folks against placing such databases/files on cloud-backed drives, for corruption could ensue.

Does this mean the driver software doesn't cope well with partial/fragmentary/non-linear file I/O? If so, then we *will find out* (sadly enough) when I introduce the sync protocol I've been considering.
