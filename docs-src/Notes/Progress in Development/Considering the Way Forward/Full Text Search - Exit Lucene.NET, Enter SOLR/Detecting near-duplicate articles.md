# Detecting *near*-duplicate articles

The inspiration came form a set of articles discussing SOLR aspects, focusing on any **pre-processing of the data being indexed** to help improve search quality.

## Caveat

Most limitations listed at the [inspiration source](https://cwiki.apache.org/confluence/display/solr/TextProfileSignature) apply to this idea too. 

## Building up to it... our challenge / problem we want to solve

However, we're *not interested in deduplicating documents*: we've our Qiqqa document content hash for that!

What we are looking for is quickly and easily find *near*-duplicate documents where these near-duplicates will exhibit one or more of these, ah, 'features':

- comes with different *cover pages*, thanks to coming from different sources, where each of them has added their own banners and what-not to the base text.
- comes with different (more or less verbose) *overlays on each page* (arXiv being one like that, adding version and timestamp info to the pages; other sources may add more verbose 'small print' like "This paper has been downloaded from foo-bar.zilch.com and yada yada yada budweiser yuck." -- you get the drift.)
- different revisions of the same paper: preprint vs. final or (*arXiv*!) initial version 1 vs. revision v2 or v3: near-duplicates of a different kind, as this is relevant change for the *content* itself, as it's *authored content edits* that cause the difference here.
- has completely reformatted the text, e.g. author's own *preprint version* vs. magazine copy layed out in the publication's style.
- user-edited revisions of a paper[^thanks to Kenelm for pointing out, once again, that Qiqqa users will want to *edit* their documents (rather: *annotate* the documents and/or comment on them otherwise) and save that activity as yet another PDF *revision* to track -- this is very valuable data to the user and thus should be kept safe & tracked], where the *editing* has involved any or all of the following actions:
	- correcting typos and other language mistakes in the OCR/text layer of the PDF (i.e. *correcting & improving the quality of the embedded text layer*)
	- annotating the paper (for various purposes, *all of them* requiring Qiqqa to offer the ability to *search* those annotations at any later date![^Currently (v83), Qiqqa does not yet support this functionality.])
	- augmenting / altering the document in any other way (either in content or technological access: adding/removing/changing content pages, changing PDF 'security' settings such as *print enabled*, etc.etc.).

	  I consider OCRing a document as yet another *augmentation action*, hence both original document and the OCRed revision should be stored & tracked by Qiqqa.

## The idea

*Text Distance metrics* are a no-go area anyway because that would mean quite a bit of calculus and text-processing when you have a large library, say upwards of 10K documents, some of them *entire books* with quite significant page counts (I've several which run into the 1100's there).

So I've been pondering if I could come up with some sort of mechanism that's similar to *image hashes* for image library de-duplication, where the basic idea is to take a image, create a fixed-sized *thumbnail* and produce a *hash* from that such that near-duplicates will match the hash value and thus *nearness calculations* reduce to simple, **fast**, equivalence checks. 

As with the image hashes, where the really hard part is coming up with a hash which permits *similarity* to the degree of accepting 'edits' like changes in color profile, **clipping** part of the image area, etc. and still have a hash that's *sufficiently unique* to be useful in a large collection, yet still be able to help find image *near duplicates*. So I've been thinking of rendering page images to PNG or such and use these thumbnail+similarity hashes as a means to dig up *similar pages*: have enough of those matching between documents and you've yourself a *near duplicate document*.

Trouble being that this is pretty intensive to prep as it means I'll have to PDF-render *every page in every document* to get me a similarity index of questionable quality... that's **lots of CPU cycles** burned on a possibly-maybe...

## *Page Similarity Hashes*, anyone?

Let's, for a second, ignore the last two types of near-duplicate: the *totally-different-layout-in-magazine-X* variant and the *authored-revision*. We'll revisit those later, for those don't add or paste *irrelevant pages* or inject *irrelevant text on existing pages*.

Now ask yourself: what if we create a *content hash per page*? Sounds good. For a paper, datasheet, etc. with a changed/added/removed banner page (or couple of banner pages), this would then result in a set of hashes (numbers) which match. At least most of them match and thus we can do *fast* searches for all the documents that also have *page hash H<sub>x</sub>* in their similarity hash set.

That solves the banner pages problem: if we have more than X% (80? 90?) matches in both our page similarity hash sets, we have ourselves a near-duplicate, right?

How about arXiv-style overlays pasted on top of my pages?

### Enter SOLR's [`TextProfileSignature`](https://cwiki.apache.org/confluence/display/solr/TextProfileSignature)

The key take-aways from that one are:

- performs *quantization on word count*, thus tolerating *minor edits* to a very high degree. (When we're right on a quantization boundary (i.e. rounding edge, say score 5.498 and the edit adds another copy of that word: oops, score now at 5.504))
- ignores single occurrences of words: this would take care of some pesky OCR artifacts -- combine that with a reasonable tolerance for differences in word frequency, thanks to the *quantization* bit above, a slightly less successful OCR copy of the same page would get me the same (quantized) score and thus a potential *match*. Yay!
- ignoring single occurrences would also result in those arXiv timestamps, etc. getting discarded entirely. Yay!
- re *word frequency quantization* (*brilliant*!): single occurrence counts as 1, anything that scores even slightly higher get's quantized as `2` so 'a few more occurrences' are identified as such vs. 'single occurrence'.
- the hash is constructed from the thus-filtered word set and their quantized frequency counts across the document. *Presto*.

The idea here is: let's do that, but **per page**! That way, all my banner pages and the odd tail page don't pollute my similarity hash. I'll end up with a set of hashes instead of just one, but that's *copeable*: it's slightly harder than a single hash to match against *at speed*, but I have a few other problems yet unsolved that might benefit from a *hash set* rather than a *single hash*... lest I mix that with something like a Bloom filter idea and thus arrive at a single number to check. However, that would kill the *equality check only* match process again, so I'm still kinda stuck there. *Hash set* it is, therefor. **One similarity hash per page.**

Combine that with another idea I got from somewhere else, where spell-checking your SOLR *input* is discussed: anything you index there is pre-preprocessed by a spell-checker and a character filter, where the latter strips off any non-alphanumerics before uploading the document into SOLR: that's the gist of it, though that one is more about applying AI to make this filter/preprocess considerable more *adaptable* and thus more lenient towards your various inputs (think Chinese texts, OCR'd documents with varying degree of OCR *success*, etc. and still ending up with a reasonably useful *document extract* for FTS indexing while not cluttering the FTS engine with a lot of crap/noise -- another *great* idea, BTW.)

So the idea here is to *similarity-hash* the *filtered text content*, which is inherently much faster than rendering each page to a full-fledged *image* and thumbnail-similarity-hashing that one!

Check out that [`TextProfileSignature`](https://cwiki.apache.org/confluence/display/solr/TextProfileSignature) link: the *cutoff* and *multiplier* listed there as configurable parameters are indeed the useful bits one would need to tweak and tune here.

### Added thoughts / ideas

Also I would apply a *logarithm* (or similar type of weight scaling) to that word frequency as that means I'll be more tolerant to more stopwords in my near-duplicate edit making the grade by ending up in the quantization slot, which should take care of my "This page has been downloaded from bla-di-bla-di-bla" overlayed (near-)duplicate documents.


### That takes care of... (and what did we miss?)

That takes care of
- banner pages of any kind: that is now turned into counting the number of matching vs. *different* page hashes and hash set size (different page counts, remember! Thanks to those pesky banner pages.) Which is quickly simplified to a *if it matches enough pages of mine it's a match, a forget the rest* rule.
- document revisions are matched, as long as the edits are small enough. *Whoopsie* when the edits causes the content to shift across the page boundary though! (More convoluted idea for that one next: add more hashes, based off other boundaries in the content... See further below.)
- any overlays, as long as they're *insignificant enough*, are caught. This takes care of aXiv stamps and a few others out there. The logarithmic weighting should help to kill the more verbose small-print overlays. *One can hope.*
- *part of a larger publication* is now also flagged as a near-duplicate: if a chapter (PDF) is discovered to match enough pages with a book (PDF) elsewhere in my collection, the book will pop up in the duplicate set. Which is a nice thought as that would help with the LNCS series (Springer) and other published *bundles*.

#### Leaving us...?

What this will not catch are revisions where a bunch of content is removed, rewritten or added, resulting in a completely *shifted* page layout. Or the *different-magazine-layout* items, that match almost word-for-word but look entirely different when rendered on-screen.

So the quick&dirty there would be to apply another *boundary heuristic*: if we can detect *chapter* or *paragraph* boundaries (i.e. significantly bigger vertical distance between reams of text in a page!), we could, theoretically, split at those points and create another set of similarity hashes, thus enabling us to find *plagiate* and re-designed pages. Yet the hash set would both be pretty large per document and of quite lower quality as the word count per hash would be much reduced.

> Oh, another take-away from that SOLR filter is this: when "all words are unique", they are not discarded, thus taking care of short texts which would otherwise cause false positives due to only the stopwords making it into the content hash.
> 

We *might* get away with reducing such a large hash set by subsampling or bundling-per-N, but then we're right smack in the 'hopefully that's still good enough' statistical game danger zone, if I'm any judge. As they say: "this is in need of further research."






## References

The way we got there:
- https://www.solr-start.com/ -->
- https://www.solr-start.com/info/update-request-processors/ -->
- https://solr.apache.org/guide/6_6/de-duplication.html -->
- https://cwiki.apache.org/confluence/display/solr/TextProfileSignature


## Postscript 1

Oh, before I forget to mention to my later self: these hashes can also be nicely stored and query-matched in SQLite: no need to bother SOLR/Lucene with these if we don't want to. So we have another option there, when we go hunt for *maximum performance*.

> Because I want to open up Qiqqa at the component level, such a move would make it slightly harder for others to dig through the SOLR database by themselves, though. 
> 
> **If** I deem that important enough for this type of search, that is. SQLite is easily accessible as well, anyway, so *power* users can cope if they fancy creating their own *special purpose job* on top of our data, hm?


## Postscript 2

The *Document Similarity Hash* is probably not good enough for a more *precise* near-duplicate/A-is-revision-of-B detection and reporting/clustering process.

That's *clustering* as in: bundling all near-duplicates and presenting them as a single (compound) entry in searches and other reports, unless the user specifically asks for the individual items in such a *we-are-all-revisions-of-one-another* cluster (or does the name **group** work better for you?)

There's a lot of research about detecting 'plagiarism' which might serve us there: if my *search for papers* has been thorough, then document similarity is generally attacked using some of the same toolkit that's used for *categorization* and *keyword extraction*: LDA, TF-IDF and the newer offspring of those: L-LDA, PLDA, etc. 

Thus I get the feeling that we *might* be able to produce a very rough pre-filter using some sort of *document similarity hash* (think: Bloomfilters) based on the extracted and *segmented/tokenized content* first. Low storage costs, should be fast and is, I hope, a reasonably good pre-filter to help us reduce the number of documents we need to subject to additional processes (such as LDA/SDA) to find out whether those 'candidates' are indeed revisions. (Incidentally, such processes would also spit out keywords/keyphrases, so maybe we should *cache* or *hash* those auto-keyword extraction activities to help speed up future document similarity searches? E.g. hash all n-gram/segment tokens into a limited width table with overlaps, thus producing a Bloom hash. We will need to also do this per *detected significant text chunk* (**not** per page!) as we otherwise would have a very hard time detecting near-duplicates of the *is-part-of-book-or-bigger-publication* and *is-formatted-very-differently-in-this-other-magazine* kinds: there the document will match only part of bigger pub or only most of the 'text chunks' -- assuming our title, chapter, figure and content paragraph detection logic is up to par for this. "*That is another active area of research* however," as the saying goes...)



## Postscript 3

Also note [[../Fingerprinting Documents/Fingerprinting - moving forward and away from b0rked SHA1|Fingerprinting - moving forward and away from b0rked SHA1]], section "*Re SHA1B collisions*" and onwards.