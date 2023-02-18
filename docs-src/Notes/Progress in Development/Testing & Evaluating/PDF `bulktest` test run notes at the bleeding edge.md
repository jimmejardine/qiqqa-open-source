# PDF `bulktest` test run notes at the bleeding edge

This is about the multiple test runs covering the `evil-base` PDF corpus I've been collecting over the years.
This is about the things we observe when applying our tools (at the bleeding edge of development) to existing PDFs of all sorts and the (grave) bugs that pop up most unexpectedly.

This is a copy of the lump sum notes (logbook) of these test runs' *odd observations*.

-------------------------------

## `bulktest` Logbook

> This content is also available at the `evil-base` corpus repo, when all is well, but as of Q1 2022AD `bulktest`  does not just have `evil-base` but an entire 6TB HDD for itself. There we have collected:
> - all our (**b0rked!**) Commercial Qiqqa crash-dumped libraries, 
> - the entire gamut of PDFs, HTMLs, etc. that we have *ever* parked in our Qiqqa Watch Folder -- most of that stuff still needs to be re-imported but current Qiqqa (.NET) is not up to the task as the *search index* will *b0rk* (old Lucene.NET), the import/export tools (antique `mupdf` et al) will *b0rk*, etc.etc.: *b0rk* *b0rk* *B0RK*!
> - the `evil-base` corpus itself, and
> - any (PDF) corpuses referenced by it (via `git submodule`)
>
> Of course, the HDD carries most of these as direct backup copies/dumps since pre-dawn so there's lots of duplication too, which we resolved by a couple of long-running [*Duplicate & Same File Searcher 6.0.3*](https://malich.ru/duplicate_searcher.aspx#download) sessions, where the duplicates were then replaced by NTFS hardlinks to save space on disk.
>
> > Now the datafile list generating tool (based on `DirScanner`) must be made smart enough to skip those duplicates by checking those hardlinks, but that's not very easy as *every file* in NTFS is a hardlink: there's no master/slave there, just a link counter incrementing, similar to *inodes*, so you can't say "this is the master file and *that * is the duplicate", alas.
> > 

(The logbook was started quite a while ago, probably already in 2020.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------

`bulktest` and `mutool_ex` observations, crashes and misbehavings while we pump a *large* number of PDF metric tonnage and publication page URLs through the dev/test system, 2020AD -- 2023AD:

- [[notes 001-of-N]]
- [[notes 002-of-N]]
- [[notes 003-of-N]]
- [[notes 004-of-N]]
- [[notes 005-of-N]]
- [[notes 006-of-N]]
- [[notes 007-of-N]]
- [[notes 008-of-N]]
- [[notes 009-of-N]]
- [[notes 010-of-N]]
- [[notes 011-of-N]]
- [[notes 012-of-N]]
- [[notes 013-of-N]]
- [[notes 014-of-N]]
- [[notes 015-of-N]]
- [[notes 016-of-N]]
- ...
