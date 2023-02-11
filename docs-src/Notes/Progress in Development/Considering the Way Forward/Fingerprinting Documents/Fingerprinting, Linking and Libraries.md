# Fingerprinting, Linking and Libraries

Store fingerprints in binary byte mode instead of string, at least for the new SHA256 hashes. That saves space and makes comparisons, etc just a bit faster too. Store as hex text in database, etc. as humans will want to access those machine parts independently from Qiqqa.

Allow documents to link to others: "duplicates" are an obvious reason. But there's more:

As we want to decrypt / *unlock* PDFs using `qpdf` and make then carry text layers, when they don't already (OCR! Also *forced OCR* for those PDFs [that](https://owl.purdue.edu/owl/general_writing/grammar/that_vs_which.html) produce invalid text / mangled text on extract: some PDFs are protected against text extraction that way.)

So we recognize the following types of links:

- the document itself: "raw" / **original**
- **Aliases**
  + decrypted / **unlocked** (`qpdf` output of the same PDF) – note Update section at bottom of document!
  + **textified** (OCR applied and stored as text layer – hOCR format or otherwise; school's still out on that one today (2021/03/29))
  + **post-processed**:
    + **cover pages removed / cleaned up** – this is only *one* area where customizable/user-specified/AI-based processes would be needed as this stuff comes in a wide variety of shapes and forms. We've even observed *tail pages* with commercial content / adverts mixed in with the PDF.
    + **page scans / images cleaned up / righted** (for skewed and slightly rotated and other rougher scans)
    + **augmented / edited / fixed-unbroken** (where missing pages have been added or other errors in the original PDF have been fixed. Also addresses those PDFs which have gone through *repair* processes.)
    + **edited / reviewed** by other parties, e.g. magazine editors (while a manuscript may go through several iterations, each one reviewed, etc., this would then be SET/ARRAY of links, but I suggest we can get away with linking them one by one, so as a *linked list*. If a PDF is then removed, the linked list should be maintained by linking the previous with the next. Incidentally, *this* is where it clearly became apparent that *linking back to the original* is a smart idea - either that or you'ld have to use a double-linked list, otherwise you'ld need a $\mathrm{O}(N)$ costly scan of your library to find the original in order to patch the linked list on document delete! Referring back to the original is a smart thing anyway, because you may not land on the original at first, but find an alias/copy of it and then may want to visit the original...)
  + **vetted** (the final edition, flagged as such by the Qiqqa user: the cleaned up, to-be-used-from-here-on-out edition of the original PDF)
- **Similes**
  + **near-duplicates**: same content, different print / venue / publisher? (this would be a variable number of links: MANY)
  + **reprint** (when the same book or paper is reprinted later with minor or no changes. Sounds like another type of *edition* to me, though - see the next item)
  + **edition / revision** (not exactly a duplicate, but it's handy to have a quick way to point at a retraction, updated paper with partial new info, new edition of a book, etc.) (also a list of links: there's books out there with multiple editions and check arXiv to quickly find there's plenty papers that come in v1, v2, v3, ... versions. )
  + **annotated** (well, if you wanted to store your or others' annotations in the PDF itself. This would of course be another SET of links. Not much difference with *editions*, *revisions* or *edited* versions: all of them are *almost identical* in that the differences in the content will be *minimal*. Of course, this calls for a *comparison tool*: how about applying the nice .NET test system [ApprovalTests.NET](https://github.com/GerHobbelt/ApprovalTests.Net) and use or rip that one for the tek to fire up [Beyond Compare](https://scootersoftware.com/) or [whatever](http://kdiff3.sourceforge.net/) [external](https://meldmerge.org/) two-way/three-way comparison tool we might favor? _Good thought._ While I strongly favor [Beyond Compare](https://scootersoftware.com/) (we go back 20+ years, the two of us :-) ), [Open Source Blend et al should get mentioned here too](https://www.git-tower.com/blog/diff-tools-windows). KDiff3 is still around but suffers from inactivity, alas.
  
# Sync?

How should we go about **syncing** libraries? Qiqqa does that already, but what are our thoughts on what *should* happen vs what is actually done today?

Should we allow the guest library to be *shareable* at all? Currently, when you do, and *join* it from another computer your Guest lib there gets hooked up, even if you rename the bugger, and you *might* not want that to happen.

Should we have a Guest lib at all? How about we all start with a unique lib? Ah! That would cause trouble:

1. Qiqqa would then need to scan the base dir (it does that *anyway*) and the first lib it finds which includes a known_web_libraries file will drive the naming of the others: if you did a repair-and-care or made a mess of your base dir some other way (e.g. you're *me* and you have several copies of older *crashed* commercial Qiqqa Guest libs -- renamed! -- in there), then you will be unsure which lib+known_web_libraries combo will drive your named list of libraries *today* and **that is a problem**. Hence having a fixed Guest lib on every machine is a boon that way. Might want to tolerate having it renamed though...
2. While this "unique names from the start" idea sounds great on paper, the *second problem* is for folks using multiple computers: today, they can easily join up their guest libraries hassle-free, as the names will match. If they wish to join/sync their machines with this no-Guest idea, they'll have to either *delete* their initial Guest lib on every node (except the first one) as the sync/join will not link that lib, but hook them up to a *second* lib, so all machines except the first one will show two(2) libraries after join/sync -- until they have deleted the local (empty) guest lib.
  
   Now thoughtful minds might argue that this is **not a problem, but a benefit instead**, as it'll ensure you and anyone else will never inadvertently join/sync your _Guest_ libraries ever again. _Hmmmm._
  
3. How is Qiqqa to know that it has created its first demo library with those two documents?

   Or do we just mark a library as 'initial/starter'? Or do we provide the user with an up-front choice, whether they want to create a demo/guest lib or not? (Answer: they wouldn't want one created on their second/third/etc. machines if they plan to sync the boxes next.) How do we register in Qiqqa that the guest lib has been created? (Answer: simple marker file in the base directory root? That's enough to flag this thing has happened somewhere in the past. So... we should provide the user with the option to create a demo library? No/Yes, that's very close to offering to create an empty *unsynced* library anyway: just add the tick box "add demo content" or not and we're golden.)
  
4. How would Qiqqa cope with a roll-back? Answer: well, an older Qiqqa would not recognize this for it is and create a Guest library anyway. Ah well, we can always go and delete that one when we roll forward again. Still *binary compatible* that way. With a little side note for the situation when you roll back.
 



—

## \[Update 2021/03/29]

`qpdf` is NOT needed any more to "unlock" certain sets of PDFs as it turns out that the MuPDF library and (augmented) tools that come with that one completely cover our current need: 
`qpdf -decrypt` is not needed anymore for another reason as well: the `mutool` utilities, including `mudraw` DO NOT suffer from the render issues that the commercial SORAX library had with many PDFs, so a "decrypt" preprocess operation is not needed any longer to guaranteee a high quality screen render output and full text access for text extraction.

### Re: "types of links"

While the list above is nice as a reminder, it misses one important point: when you discover a paper or other publication PDF, you DO NOT know yet if this is the "original" version. Even quite a while later, when you may have run into several *revisions* and/or versions as published by multiple magazines, your own *definition* of what you consider the *root* document might be different from others' and isn't always a clear-cut case of *this* one or *that* one. Hence the "original" document isn't so much an item you can tag as such, but rather is a *result*, a *consequence*, of the other PDFs being tagged as *similes* and *aliases*: this creates a graph – directed or undirected, depending on your worldview ("*Weltanschauung*") – which MAY result in the discovery of a unique *root node*, which would then become the *original* document.

Anyway, the point here is that *original* is not something you can tag a document with *easily*: when other versions enter your library, the notion of what *is* the "original document" MAY shift.

