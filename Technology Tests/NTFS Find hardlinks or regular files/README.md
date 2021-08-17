# NTFS Directory Scanner, a.k.a. *DirScanner*

## Purpose

We needed a tool, specifically for Windows/NTFS, which would scan a directory tree 

> (d'uh! why do your own?! Well... there's this:...)

but we *know* that our PDF directories are full of duplicates, which I took care of by running tools to have them converted to NTFS hardlinks so the dupes do NOT take up useless space on the SSD disk.

Works great, but now that the bulk tests for my MuPDF tooling (to be used in Qiqqa) are running full-out, the bulk test prep scripts encounter a few problems, including the *invisible* problem where we happen to test the same file multiple times, thanks to it existing in multiple places.

This DirScanner tool is **special** in that it has these non-trivial abilities:

- reports *all* NTFS/Windows File System attributes, not just the ubiquitous AHSR+D (Archive, Hidden, System, ReadOnly + Directory) from the DOS Days.
- detects and reports HardLinks in a UNIX "sense": the L+X attributes. 

  NOTE that Windows/NTFS considers *every* file a hardlink! We're talking multiple file paths which point at the same *file content*. In Windows, you can find those only by invoking other APIs and counting the number of "hardlinks" for a file: when there's more than 1 (i.e. 2 or more), *only then do we report hardlinks* (or may we call those "duplicate references"?) as you would expect on UNIX file systems.
- ability to filter on any attribute mix, **including the emulated UNIX-style hardlink L/X attributes**
- ability to scan multiple disjoint directory trees all together, producing one consolidated output.
- ability to write the file paths to file in UTF8, while file paths are checked against glaring Unicode issues.

This allows us to scan the directory trees (which also contain some *very nasty* named files) and get a list of **unique files** for us to process in the bulk test runner scripts, etc.

This tool also allows to get a list of "duplicates" which have been "hardlinked".

In that sense, this tool is far more powerful that MSYS' `find` tool (from UNIX) or native `DIR`, as none of those have the ability to uncover these hardlinked duplicates.

Meanwhile this tool is very helpful in that it will list the *first* of a set of hardlinked duplicates when you filter on the "hardlink" attribute (negated).
You are thus guaranteed to NEVER loose sight of a file, as DirScanner decides which is the 'first' and which are the 'hardlinked dupes' only after it has applied all your other filters. In other words: when you ask for the original file of a duplicate set, the first one we discover is **it**; the rest of the hardlinked set are remembered internally and discarded when requested. -- That would be the effect of `/r X` as in the commandline:

    ./DirScanner.exe -s -c -r X -o all_pdf_files.lst "*.pdf"
	
which is one of the commands executed by the bulk test prep script to collect all PDFs in that directory collective of mine and only spit out the *truely unique* ones. (Assuming that I ran the dedup tool on the entire collection and hardlinked all dupes, of course.)




> Yes, I know that's a mess. 
>
> It is a mess of directories which include all my old CRASHED commercial Qiqqa libraries, which still need to be recovered properly as Qiqqa still is not able to properly handle the onslaught of 30K+ unique PDFs -- now that I have this DirScanner working, it turns out that's the number. While the total file count in there numbers around 100K... So many dupes...




---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
