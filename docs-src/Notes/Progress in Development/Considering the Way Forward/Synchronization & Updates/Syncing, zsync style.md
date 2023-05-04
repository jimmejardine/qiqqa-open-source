# Syncing, `zsync` style

I've looked at a lot of stuff, including `rsync` & `unison`. All the *smart stuff* requires dedicated software running at both sides of the fence. I don't want that: the cheapest 'cloud storage' solutions are plain file and/or static web page access only:

- plain file access only:
	- DropBox
	- Google Drive
	- Microsoft OneDrive
	- Mega
	- ... other *upload services* that offer direct file access.
- static web pages
	- GitHub Pages (which are git/github backed, so you have an additional limitation: 50MB per file *max*)

 More expensive are:
 - Azure
 - AWS
 - ...

Alternatives which folks can set up / buy relatively easy, under their own control:
- NAS
- USB drive / memory stick (which *may* be transported to other machines on a regular basis)
- ...

so it is opportune to come up with a sync solution that caters to the lowest common denominator with regards to network access features:
- *either* plain file I/O (when using cloud storage, based on locally installed dedicated software drivers),
- *or* static web pages / web server (Github Pages: *read only*, i.e. no upload possible),
- *or* using the simplest possible web access tools to download/upload files to the store's website. I'm thinking: `curl`?

## zsync instead of rsync/unison/...: why?

`zsync` has the smart idea to keep all the work & intelligence client-side, contrary to `rsync` et al, who require server-side effort. Since we want to use *cheap servers*, the amount of work done there, or the complexity of setting it up there, or getting the dedicated software running there in the first place: it is all highly undesirable.

Assuming we only have the most rudimentary HTTP download/upload facility available (HTTP GET/POST only?), `rsync`-like behaviour is accomplished by `zsync` via their approach of using `.zsync` hash files alongside the actual files: a previous `zsync` session should have produced these `.zsync` files, so the current session can download these and thus act like a *fake* remote `rsync` server produced a set of rolling hashes for the given file, for the client to compare against, after which it can own-sync (download) only the changed parts via `Range:` HTTP headers when downloading the data file.

The `zsync` documentation is much less loud/clear about the *up-sync*/*upload* side of this game, though I assume they expect the web server to support `Range:` headers for POST/PUT file-overwriting uploads as well.

## Critique & Qiqqa?

1. I like the way of thinking that `zsync` did. Though using MD4 as the 'costly hash' is out of style, it's the principle of the thing that matters here: we can easily use BLAKE3 or similar or the same purpose today.
2. I'm of two minds where it comes to that 'rolling hash', which is, by necessity, Adler-based, or could use another fast hash that can be incrementally produced at cost $O(N)$ while moving a sliding window over a file of size $N$: while this works great for lots of small-ish files, such as source code, text, etc., I wonder if we benefit a lot from that approach still when moving large *binary files* across the interface? 
    My *guess* is that the rolling hash approach of `zsync` (Adler first, then MD4 for candidate matching blocks) is too costly compared to the transmission cost/speed benefits we may observe. Perhaps we can get away with another means of segmenting those files and discovering *localized* changes -- so we can reduce our transmission to only sending those, a la `rsync`/`zsync`.
3. How about **bittorrent** as a transfer protocol? Are there other established peer-to-peer data transfer protocols that haven't got a 'bad vibe' in some circles yet? **bittorrent** could be useful, but I image not everybody is keen on having that one punch a port hole through their firewall (PnP mode; DHT?); 
    **websockets**-based peer-to-peer chat protocols? 
    With a fallback all the way down to **email** support: the oldest known method for *collaborative file/data exchange*: **except** email is notorious as a spam/trojan vector so this might not be *appreciated* by the company IT/admins. ;-) Besides, these days, email services such as gmail have become *very* restrictive re supported email attachment formats (ZIP and a few other ubiquitous formats, no password protection allowed at all, etc.) and *they* advise to use Google Drive for any data, then send a link via email instead, so sync and/or collaborate via email-only is out of the picture: that was doable 20+ years ago. Not today.
   `irc`, anyone? It's still around and could be done securely... *I guess*?
 4. There's also `rclone` which is advertised as "`rsync`   for cloud storage" and supports a *huge* list of cloud/storage providers, including DropBox, Google Drive and MS OneDrive. Written in Go, so given our chosen dev policies, to be considered as an *external tool* only. 
     Might be smart to use that one, invoking it via some `exec` commands, for anything that we don't want to support ourselves, i.e. all cloud storage via `rclone`, while we take care of sync to NAS/local-drive/USB-drive ourselves. 
     AFAICT, `rclone` has the same drawback as `rsync` and friends: no verify-after-copy. `rsync` has a `--verify` option, but that is pre-copy only: it is meant to verify the *checksums database* against already present target files to assist the decision whether to copy a file or not. We, on the other hand, are looking for a tool that supports verify-after-write: thanks to our struggles with our own (flaky?) hardware -- after losing *weeks* we finally decided to throw out the new hardware (2y old, on the wrong side of the warranty expiry date) that has been giving us nightmares from december to april and grudgingly order alternative hardware (motherboard). Meanwhile, experience with (MS Windows based) `teracopy` has once more proven the usefulness of verify-after-write when you really care about your data: three copy faults have been detected while copying 60TB+ data around as we attempted to (re)organize our large data sets stored on (USB/SATA) HDDs (DoP 2010--2023).
5. Experience with `rsync`: bloody slow compared to `cp -n -r -v` for locally mounted storage (USB & SATA), particularly when copying a large number of small files, e.g. source code trees or the classic-Qiqqa OCR cache directory (`/ocr/`).
6. While I've been reading up on `rsync` and `rclone`, I haven't tested the latter on my own Google Drive or OneDrive storage. From hat I gather from the internet fora, Google Drive isn't exactly known for its speed. That's not overly important to me, but I was looking for more folks experiencing and technically analyzing copy/clone/update issues around cloud storage as reported in the SQLite FAQ, where one is strongly discouraged to run this over a cloud storage backing base: all I could find was a few people complaining about DropBox doing something *weird* with large files when those are updated by the user. Alas, links to the source articles are lost, thanks to my flaky hardware: the bugger started to randomly wipe parts of my data disks a few weeks ago (no virus/trojan, probably something culminating from a historical less-than-100%-success-rate track record of MS Windows Updates, combined with instable hardware.)
   
