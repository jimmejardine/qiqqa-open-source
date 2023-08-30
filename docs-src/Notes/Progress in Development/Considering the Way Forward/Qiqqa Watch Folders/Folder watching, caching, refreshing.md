# Folder watching, caching, refreshing

Short jot-down note for now: check out the ZIG programming language designer's blog again: re application rebuild action, i.e. the equivalent of running `make all`, he says something along these lines:

- run a filesystem `glob` to collect your file tree.
- DO NOT content-hash your source files (inputs) as that's too costly (disk access, transfer cost), but use everything `fstat` throws at you, while reckoning with:
- all filesystems have weird peculiarities, as do all OSes: `mtime` is not the "modified time" you would *think* it is; `ctime` is a farce as it really is another kind of `mtime`, but updates for different reasons, at least on Linuxes (AFAICT), `atime` is crap and smart admins have it disabled if it wasn't disabled at OS install already, `inode` number is a good *tell* to see if a file changed (if your OS+FS (`fuse` is crap!) delivers it through `fstat`: MS Windows doesn't do *inodes*, so...?)
- bottom line: hash your `fstat` output, f it changed since last time, assume the source file changed. For pedantics/*optimization*, you may consider doing content hashing when the `fstat` info hash signals a *highly probable* diff. Of course, all you were after was diff-signaling, so the content hashing is pretty useless ("*mosterd na de maaltijd*") from that perspective.
- *cave canem* with **everything** that comes out of the OS+FS combo re filesystem info; `fuse` and others also impact dependability negatively, so it is quite hairy, all.
- change triggers (= (registered) push notifications of FS change) may not "*bubble up*" the way you might like. classic rescan through `glob` seems the smart route, particularly when you have many watch folders.

