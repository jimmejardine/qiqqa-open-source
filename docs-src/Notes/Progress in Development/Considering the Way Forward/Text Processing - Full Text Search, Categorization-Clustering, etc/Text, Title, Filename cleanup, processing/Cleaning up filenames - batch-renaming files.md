# Cleaning up filenames / (batch-)renaming files

Not just for Qiqqa imports, but sometimes we grab files from someplace (e.g. newsgroups or other "webby" places) where the original filename is damaged or crappy in other ways.

This would be another task for a `vcopy` like tool, where we have felt an need for these types of cleanup/processing:

## Filenames with any mix of `_` underscores, `-` dashes, ` ` spaces and other separators (& *mangled*)

Particularly noticeable in movie titles and similar stuff:

- non-ASCII, i.e. àççèñted letters which were *HTML escape* encoded (XY&#247WC) and subsequently get mangled → XY_#247_W#C
- filenames which were originally titles with `'` and `"` quotes in them, which get mangled/busted down to `_` or a similar "separator/replacement character".
- braced/bracketed parts which get glued together because, well, I don't know. E.g. `Documentary.about.Syria(BBC)(2024)[DVD][720p24][x265][TV][remux]`
- ditto, but now without the brackets to help identify the "words" in there: yet another spot where automated word identification & separation logic would be pretty darn useful, outside the obvious *Chinese Texts* arena: `DocumentaryaboutSyriaBBC2024DVD720p24x265TVremux`. *Good luck with those...*
- stuff that came from less, ah, attentive, sources where the title/filename is severely mangled. Newsgroups sometimes are able to produce filenames (also thanks to the tools used) which duplicate or *triplicate* the actual title, bunched up together with arbitrary `Re` and other interjections characteristic to these particular sources (newsgroups, torrents, websites such Archive.org, where some uploads which are still accessible are a horrible mess, really).

Bottom line: how do we wish to deal with *crap*? Which cleanup facilities are useful when bulk processing, i.e. when you more than just a couple files with the same kind of mangle pattern? *Can* we provide tooling to simplify this, speed this up, make this a largely *off-line*[^offline] process?


## Filenames with any mix of `_` underscores, `-` dashes, ` ` spaces and other separators (which are titles, more or less)

Then there's the more "*regular*" stuff, e.g. electronics books and whitepapers where the original whitespaces got converted to other characters, such as `_`, '.' or '-'. 
PLUS perhaps a bit of crapped up recombination of the authors' names, where the *order of appearance* of title vs. author is the least of your worries: `A.Doyle.-.Ooh,Sherlock.epub` vs. `Doyle,Arthur,Ooh,Sherlock.epub` vs. `Ooh,Sherlock,Doyle,Arthur.epub`, just for an example.

I seem to recall some AI/NLP work 're' Named Entity Recognition and some other possibly relevant stuff here, but who knows what works.

*One thing's for sure, though*": pulling this stuff through the `calibre` tool sounded like a great approach, *initially*, but I'm reverting back from that position as I've seen way too many screwups in both author name processing and title vs. author separation.

> If you like headaches, a great corpus to test your approach(es) against is simply grabbing a couple of years' worth off Usenet and see if your tools can produce a largely successful set of rename/adjust/correction operations to produce legible title + author names, which are consistent enough to be rated reasonable search database imports: author names with author names, irrespective of where they occurred in the filename/subject title, ditto for the titles (stripped of miscellaneous cruft, of course!), you know... *the works*.
> 
> Haven't seen a very successful approach to that one, yet.



## Filenames which are, frankly, *crap* and need to be *sourced* from elsewhere

How many times have I ended up with a published paper named `download.pdf` or `000001234WZXY4BUTT-K2.PDF` and their ilk?

*Too many times*, that's how many.
*Title extraction* (and *Abstract extraction* alongside) is *very* useful for this one too, not just for what everyone else expects (field-oriented FTS index building).

And don't get me started on some electronic components' manufacturers: the most glaring example is On-Semi, which has been purchasing other companies left & right and then bungled up their *datasheet* PDFs in all sorts of ways, depending on the year of acquisition and possibly *adjusted* for the *aspect of Saturn* or whatever: once you start to feel there's a pattern (consistency! yay!) there, it all changes. 
Add to that some of the craziest All Your Base Belong To Us legalese "cover sheet" pages (yep, *multiple, consecutive ones* in some cases) and this whole "extract title, make that the new filename, thank you" process isn't "simple" any more.

Older (declassified) US military and various government documents also wish to join your title/rename nightmares, but that's largely because the Mil stuff I grabbed is generally image-scanned from antiquated, not-so-well-maintained microfiche B&W equipment or what-have -you, so there is that added OCR challenge right there, as well. Nothing remains easy for long, I guess.

> 🕵️ Sounds like we're looking at two ever so slightly different problems:
> 1. how to *clean up / interpret / reformat* filenames that *are*, and
> 2. how to *obtain a proper filename from elsewhere*, e.g. from interpreted *document content*.
> 
> 🤔 Maybe the tool for this *rename* action (`vcopy`?) should only bother with challenge No. 1, *plus* accept "alternative source input" as one of the, ah, *channels* through which to obtain "cleaned up/proper" target filenames to apply. This would leave the *title extraction* and related challenges to *other* tools, which could provide their results to this *rename* utility to peruse. 
> This should keep things sort of manageable, me thinks...


### *Special mention!*

Oh! Before I leave, may I *again* direct your attention towards MS Windows Downloads? These *usually* (not always! this depends on user-controllable settings, for one!) come with ADS info streams on the NTFS storage devices, where you can obtain metadata, such as the origin URL where the downloaded file originated from; this MAY help with obtaining decent titles, authors and abstracts and thus is *very useful* for our librarian purposes here!


An alternative approach, **though pretty nasty itself**, is to grab and scan the browsers' History databases and seek the matching download/page access entry for download file X. Not a Sure Thing™️ either as various websites really make this in a doctoral thesis sized challenge, but on systems where ADS / `Internet Zone data` is not available (UNIX!) this seems like a pretty decent source of "source information"; once you have a Source URL, the Game Of Chance continues by firing off a post-fact `cUrl`  page fetch, prayer wheels running, in hopes of receiving something HTML-y **and usable** for grabbing title, authors, DOI and Abstract off for the metadata index we are to fill.

Third, there's decoding the document at hand and detecting any DOI, ISBN or other useful "identifier" (including our *guestimate* for title & author!) and dropping these into search queries fired at generic librarian/index sites, in search of more metadata/information about the document.
Of course, this becomes a moot point once we get to my *electronics datasheets* and other document-y stuff that one would want to add to their library: not even every academic paper out there gets assigned a DOI or similar identifier, so it is as the saying goes: YMMV. ***Your mileage May Vary.***[^1]




















[^offline]:  here "*off-line*" is used meaning: without the user having to be present, i.e. no "on-line" vetting, adjusting, editing activity is required for a (mostly) successful process here. Think *batches* you run and inspect and correct *post fact*. "*on-line*" then, of course, means you, the user, will need to be present and monitor/act/decide immediately before the process continues to process the remainder of the batch. 🤔 *Incidentally*, it would behoove tools like ours to behave similar to Windows Explorer Copy activity in this: run each item in the batch that does **not** need specific attention as you are, while stocking up the need-attention ones until the very end of the run where all "do we want these overwritten/renamed/whatever" user interaction happens all at once, so you can be assured *most* of the stuff has been done once you hit the *blocking* waiting-for-user-decisions action: while you deal with these, you "know" the entire batch will quickly clear as this is just the left overs you're taking care of right now.

[^1]:  YMMV... okay. But *at least we should have tried to get that metadata*, eh?! And not just accept the first byte that pops up and us its google, which way too many peeps in my environment consider equivalent to *canonical*. 🤢🤮



