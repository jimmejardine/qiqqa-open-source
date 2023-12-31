# Fingerprinting :: moving forward and away from b0rked SHA1

Okay, it's a known fact that the SHA1-based fingerprinting used in Qiqqa to identify PDF documents is flawed: that's a bug that exists since the inception of the software. The SHA1 binary hash was encoded as HEX, but any byte value with a zero(0) for the most significant nibble would have that nibble silently discarded.

This results in a couple of things, none of them major, but enough for me to consider moving:

- the SHA1 hash is thus corrupted as-used in Qiqqa: the nibble sequence `0A BC 0D` in a hash is indistinguishable from `AB 0C 0D` and `0A 0B CD` as all encode as `ABCD`. Thus the number of 'bits' in the hash are reduced quite a bit - not enough to cause immediate disaster, but there's more risk at collisions as the hash is not used the way it should.
- if I want to store both SHA1-colliding PDFs as separate documents, I can't. That's not important if I only store a few research papers, but when you are stretching the original goals of Qiqqa to suit your needs, you're in trouble.
 
## Qiqqa = *SHA1B* fingerprint hash

Given the b0rked SHA1 encoding in Qiqqa (a.k.a. *SHA1B*), you can expect more than 1 collision out there as the number of hash bits is close to *halved* due to the encode b0rk, going from 160bits to ~80 bits and that's getting close to "hmmm, not feeling all that safe anymore" chances: when looking at the [Birthday Paradox Probability Table](https://en.wikipedia.org/wiki/Birthday_problem#Probability_table) I'm aiming for a fingerprint that can virtually *guarantee* uniqueness of fingerprint per document, even in [*adverse circumstances*](https://alf.nu/SHA1). In that table, I'm therefor only interested in the $p <= 10^-18$ column as I'm risk averse like that. (More info about [SHAttered](https://shattered.io/) and [related](https://people.csail.mit.edu/yiqun/SHA1AttackProceedingVersion.pdf) [material](https://www.theregister.com/2017/02/23/google_first_sha1_collision) [in](MD5_SHA1_transcript-collisions.pdf) [these](SHA1_Attack_Proceeding_Version.pdf) [here](Collisions_of_SHA_0_and_Reduced_SHA_1.pdf) [links](Collisions_of_SHA_0_and_Reduced_SHA_1-34940036.pdf).)

## So first question then is: can we grab ourselves another hash, which does better?

Sure we can. Plenty of them!


## But how about fingerprinting (i.e. *hashing*) *performance*?

Bigger hashes tend to get slower. but then I ran into this one: ["SHA512 faster than SHA256"](https://crypto.stackexchange.com/questions/26336/sha-512-faster-than-sha-256) and that got me looking for more.

There's also the article [Too Much Crypto by Aumassen](https://eprint.iacr.org/2019/1492.pdf), which addresses the uniqueness ("collision") challenge vs. performance.

Long story short: [BLAKE3](https://github.com/BLAKE3-team/BLAKE3) looks like a very promising hash, which is both better than SHA1 (so I'll be able to store *both* those SHA1-colliding PDF documents, which would *fail* if we were to merely *fix* the current SHA1 fingerprint encoding in Qiqqa) *plus* it's expected to be *faster* :yay:.


## Okay, that's the easy bit then. How do we remain backwards compatible or migrating forward?

Except for the colliding PDFs, the easy answer to that one would be to maintain a separate 1:1 lookup table in SQLite where old SHA1-B0RK fingerprints are mapped to new BLAKE3 hashes.

It would double the fingerprinting work as long as we want to stay backwards compatible as then we'd have to calculate both BLAKE3 and SHA1-B0RK, but that's manageable. (text extraction tasks, etc. are more costly, so let's try to see this *relatively*: $\operatorname{fingerprinting}(D) * 2$ is not going to kill the app.


## Any fears about "cracking the hash" in the future? Other doubts & worries?

Not much. I'm not using the hash for pure cryptographic means, so a few odds and ends are not going to kill us. The fact that there exist real PDFs out there which collide on MD5 and SHA1 have made those hashes obsolete, but otherwise I believe we'll be doing fine with a fast BLAKE3 for the foreseeable future, even when some future research deems it cryptographically 'reduced-strength'.

Again, do realize we're not doing *crypto*/*security* here: the fingerprinting is also not employed in a *legal* setting where I need to unambiguously, beyond the shadow of doubt, *prove* that my fingerprint $F$ identifies document $D$.
The fact that I'm paranoid enough already (perfectionist?) means I'll tend to pick a fingerprint which has great expectations that way anyway and so far, BLAKE3 is looking great.

All this of course *assumes* I don't screw up the BLAKE3-based fingerprint (BLAKE3-Base58)

### Totally unrelated but still *related* is another matter: *near-identical* PDFs cluttering the library

As you asked about my doubt & worries, here's one: I discovered last week that https://www.researchgate.net/ does something nasty (from *my* perspective): it kind-of-*personalizes* your PDF download by editing its cover page: it brags about the number of downloads and the day you downloaded the PDF, so downloading the same document *twice* (say, via Qiqqa sniffer) will get you two(2) **different fingerprints**, thus the document downloads will be filed as unique documents in Qiqqa!

Sites which encode the date when you downloaded the PDF into the PDF itself (watermark/overlay) cause the same problem: multiple downloads cannot easily be identified as duplicates through comparing their hashes!

That's therefor another **source of duplicates** for documents and increases the need for Qiqqa to incorporate better duplicate-detection logic (including 'probable cause of duplication' type identification, so you can observe & set *why* two documents are considered *duplicates*:

- watermark: time/otherwise-edited unique copy watermarks a la https://www.researchgate.net/
- website cover sheet: xArchiv and various universities or other sources may present the same content, but with different cover sheets. Again https://www.researchgate.net/ vs others.
- prepub vs. print: prepublication version vs print version. 
- journal, i.e. *which* print version: sometimes articles are published in multiple journals, each with their own formatting (and *editing*, sometimes). I'd say this type also includes "personal copies" as published by ohne of the authors.
- updates: xArchiv and a few others keep track of the *revisions*: v1, v2, v3, and so on of the published paper
- anniversary re-issues / reprints: some papers are reprinted after X years. Happens rarely, but they do exist. It happens more often when you include *books* in your library. *Editions* of books would, as far as I'm concerned, fall under the 'revisions/updates' type.
- amalgam/extract: sometimes papers are published in two versions: a short and a long one. Some papers are also *incorporated* into other papers, not just referenced, but re-used/quoted almost entirely, while other content is added. This goes beyond a mere 'update'.
- *plagiaat*: we now get into the more shady corners: some papers are copied or rephrased by others to make the publishing count ends meet. Plagiarism.
- *retractions*: that's a special kind of update, not really a *duplicate* per se, but more an *overriding* relationship. Remember our beautiful doctor who invented the pro-vegan argument of more aggression in meat-eating people while waiting for his train. Then the bugger went on to write a book about his life, earning yet more kudos. (I'm proud to be Dutch!)

and when we veer a little off from true duplicates that way, there's also:

- reviews of: reviews of a book or paper, either itself a paper or just a memo published in a journal or otherwise.
- meta-reviews: *Meta-reviews* are those where the "current state of art" is discussed and multiple papers are reviewed together to report on where we are right now as a group.
- answers to: memos and letters to the editor in answer to paper X or memo Y, that sort of thing mostly.
- replication reports (where B attempted to replicate A. Sometimes successful, sometimes not. Cold fusion by loose wiring, anyone?)
- bundling of (I have several PDFs which happen to contain multiple papers: that would be a *book* itself, but having that paper X in there would make it a duplicate of that paper (amalgam or otherwise)

Now I hope some day we'll be able to identify most, if not all, of those "duplicates" and "other relations" automatically...

Meanwhile, let's keep our fingerprint nicely unique and bemoan, yet accept/live with, the ways [researchgate](https://www.researchgate.net/) et al are thwarting our efforts (mostly *unintentionally*).


> Please also note the remarks and considerations mentioned in [[../Text Processing - Full Text Search, Categorization-Clustering, etc/Full Text Search - Enter SOLR/Detecting near-duplicate articles|Detecting near-duplicate articles]].


----

**\[Edit May 2023\]: 

### Re SHA1B collisions:

Anyway, SHA1 is known-broken (See https://shattered.io/), and real-world collision examples exist (for example two different PDFs which hash the same: https://shattered.io/)

There's also https://github.com/corkami/collisions to consider, so I'ld say MD5 is definitely *out*. (See also: https://github.com/pauldreik/rdfind/issues/126)



### Re "*nearly duplicates*" due to cover pages, reformatting, etc.

We've seen various styles of *cover pages* come through our pipeline over the years:
- OnSemi datasheets with fixed additional cover page preceding the datasheet pages or possibly an older cover page from a previous manufacturer company,
- some Taiwanese universities plonking a kind of sign-off + title + author cover sheet in front of published material,
- [ResearchGate](https://www.researchgate.net/) has their own title + author style cover page, with lots of white space,
- some datasheet aggregation and electronic component (re)seller sites prefixing every document they deliver with an odd-format banner advertising their own services,
- more or less fancy cover sheets for *PhD theses*, which are mandatory part of the original document (often including sign-off by the supporting faculty member, etc.)
- ...



#### Do not just use page *image* similarity but also consider *text similarity*:

My original idea was to collect a few samples of these, print these un-helpful pages at low resolution (or scale them down) before feeding them into a visual recognition AI training program so we can detect such cover pages with *hopefully* reasonably high accuracy and act accordingly -- some of these contain useful metadata, so it isn't always only a clip-and-ditch effort we need.
While this is still a workable idea IMO, I hadn't thought about a second/alternative approach: most of these cover pages carry text (readable with or without OCR effort) that's identical for all instances of a given cover sheet, so we also CAN check for *text similarities*, i.e. trawl our content database and check for recurring phrases and/or paragraphs of text in the initial pages to help detect cover pages with *possibly* high accuracy.



#### Idea: FTS text search with additional custom n-grams?

To assist with this endeavour, it MAY be handy to not only chop text into words and syllables for trigram/n-gram based FTS (Full Text Search), but it MAY also be useful to produce additional n-grams representing entire *reams of text*, e.g. *lines* (as displayed) and/or *paragraphs*, i.e. one gram token per line and one gram token per detected paragraph of text. These are not supposed to load/complicate the FTS index very much as we're facing an incoming stream of word-level n-grams in the thousands per document: 
- assuming, say, a paper of 12 pages, 60 lines per page and 10 words per line,
- we're then looking at an in-flow of $12 \times 60 \times 10 = 7200$ words, so that's *at least* 7200 n-grams to feed into the index per document for regular FTS.
- meanwhile, the same document will have $12 \times 60 = 720$ lines, so we'll be injecting an additional 10% of n-grams hashes for this,
- while, assuming the average paragraph spanning 2.5 lines, we'll also be producing an additional $\frac {12 \times 60} {2.5} = 288$ paragraph n-grams with that: another 4% additional n-gram inflow into the FTS index.
Hence a first rough estimate would grow the FTS index by ~15%, which would seem acceptable, right?

Then, when we're hunting for phrases & paragraphs that might indicate being part of a cover page/sheet, we would run a query to find which line- and/or paragraph-level n-grams have a high occurrence rate... 🤔 pondering this hypothetical query, in order to prevent overwhelming noise output produced by high occurrence rates reported for  other n-gram types, e.g. n-grams that happen to represent stopwords or similar oft-used vocabulary, it becomes obvious that we either:
- encode extra attributes with these n-grams to be able to *identify* them as line-level or paragraph-level, rather than regular word/syllable-level, or
- store these n-grams in a separate FTS inverted index, so that we aren't bothered by other n-gram types polluting our "*who's occurring in a lot of documents?*" query results.

I haven't tested this yet, but *gut feeling* says to set up a separate inverted index as these n-grams serve a particular purpose and I don't see them being useful or reused in conjunction with those regular FTS word-level n-grams...

Now a cover sheet can be detected by having quite a few lines/paragraphs of text in common with several other documents; of course, sponsoring statements, etc. are common occurrences in the footer of first pages of many papers, so we'll have to research this and find a decent lower limit to discern between such phenomena vs. actual cover sheets, but it's a plan.

When we also encode *position on the page* into those n-grams, or otherwise add this intel as attributes or some such, we can then add positional analysis & filtering to the reported set and thus add another dimension to cover sheet discovery. It doesn't have to be *very precise* -- in fact, we might benefit from a somewhat rough approach as not all cover page instances are positioned exactly alike -- so we might already benefit when we divide the page into $R \times C$ *sectors* and then encode the *start* and possibly also the *end* coordinate of a given line or paragraph-level n-gram in sectors: that way we will only get *very probable* candidates in the top-N query output.

> Extra thought: it might be opportune to use the same approach for document similarity and/or *plagiaat* detection: a large set of shared n-gram hashes indicates some portion of the content is at least *potentially* identical...

Which leaves the question: do we have a *fast* way to find the number of documents any n-gram shows up in? Does the FTS index also store a table/index such that we can:

```
SELECT *
FROM FTS-ngram-counts
WHERE document-count > 1
ORDER BY document-count
```

or something along such lines?


---------------


## References

- https://crypto.stackexchange.com/questions/26336/sha-512-faster-than-sha-256
- https://en.wikipedia.org/wiki/Birthday_attack
- https://eprint.iacr.org/2019/1492.pdf
- https://en.wikipedia.org/wiki/Birthday_problem#Probability_table
- https://stackoverflow.com/questions/62664761/probability-of-hash-collision
- https://en.wikipedia.org/wiki/BLAKE_(hash_function)
- https://shattered.io/
- https://github.com/corkami/collisions

 



-------------

## Quick update

Both classical Qiqqa hash fingerprinting (the b0rky SHA1 algo) and a *new* one for future use, based on a fast BLAKE3 hash and Base58 encoding to string for generic use as filename, database key, etc., has been coded in my MuPDF-based toolkit for Qiqqa: https://github.com/GerHobbelt/mupdf/commit/e440b55474b288f9ff5127ee3bf35c67909ec858

Commit Message:

added two `mutool` utilities for Qiqqa:
- [`mutool qiqqa_fingerprint0`](https://github.com/GerHobbelt/mupdf/blob/master/source/tools/pdffingerprint0.cpp), which calculates the classic Qiqqa SHA1-b0rked fingerprint hash for any given (PDF) file
- [`mutool qiqqa_fingerprint1`](https://github.com/GerHobbelt/mupdf/blob/master/source/tools/pdffingerprint1.c), which calculates the *new* Qiqqa fingerprint, based on BLAKE3 and "tightening" by printing it as a Base58X rather then HEX fingerprint string. (I call this Base58X because it takes the tables off [Base58](https://tools.ietf.org/id/draft-msporny-base58-01.html) from the original bitcoin author Satoshi Nakamoto but then goes and does something completely different with it as the original bitcoin code would treat the hash-to-encode as one _BigInt_, which he then converted to Base58, but which takes quite a few divide and modulo ops, which I don't want to spend on that optimum result, so *instead* I look for where number base 58 and number base 2 get very close and that happens to be at bit/power 41. 
 
   This idea is very similar to what the folks of Base85/Ascii85 argued for: $(85^N \simeq 2^{32} \land 85^N \succ 2^{32})$, so their argument is that number base 85 (or higher) is very handy to use to encode 32-bit integer streams; *I* have looked at Base85 and Base91 and many others and I don't like them for the same reasons that are listed in [the Base58 header file by Satoshi Nakamoto](https://github.com/trottier/original-bitcoin/blob/master/src/base58.h): things get icky quickly – while having a slightly higher "encoding space efficiency" – as channels such as web (URLs, eMail, etc.), file systems (file names are pretty restricted outside modern UNIX file systems!), source code / JSON data files, etc., etc. all add their own quirks to such encodings, resulting in *variable length*, more-or-less-riddled-with-escapes-or-potential-faults-if-you-don't, encoded strings.

Base58 has the advantage of remaining a "selectable word" with nothing to get any interface medium's nickers in a twist either.

Besides, consider the relative gains (we're looking at *stringified* hashes as we'll be storing these in databases as *unique record keys* and other non-BLOB-allowing fields and thus do string-comparison based lookups and duplicate checks via $\textit{fingerprint} \stackrel{?}{=} \textit{record.hash}$ string compare operations):



#### String Encoding of a BLAKE3 full size hash: output size

| encoding     | calculus                   | # output chars        |
|--------------|----------------------------|-----------------------|
| HEX:         | $32 * 2 = 64$              | 64 chars              |
| Base64:      | $32 * 1.33 = 42.7$         | 43 chars              |
| Base85:      | $32 * (1/0.80) = 40$       | 40 chars              |
| Base91:      | $32 * (1/(1-0.23)) = 41.6$ | 42 chars (worst case) |
| Base91:      | $32 * (1/(1-0.14)) = 37.2$ | 38 chars (best case)  |
| Base58:      | $32 * (1/0.73) = 43.8$     | 44 chars              |
| **Base58X**: | $32 * 7 * 8 / 41 = 43.7$   | 44 chars too!         |

Notes on these datums:

- Base85 is 80% efficient according to Wikipedia: https://en.wikipedia.org/wiki/Binary-to-text_encoding
- Base91 efficiency numbers are from the source: http://base91.sourceforge.net/
- Base85 and Base91 output sizes (including the "*worst case*" numbers listed above!) DO NOT account for mandatory escaping of some of the output characters for various mediums (URL/Web, File Systems, etc.); these escapes can increase the output size cost to *twice*, possibly even *thrice* the listed size above (`%XX` URL encoding for some chars!) in rare circumstances, over which we have NO CONTROL: we can't tweak the hashes to "go around" these worst case scenarios lest we 'downgrade' to a Base58 approximation.
- Base58 (Bitcoin / Nakamoto) efficiency number according to Wikipedia: https://en.wikipedia.org/wiki/Binary-to-text_encoding
- Base58X: my own approach takes $58^7$ for every 41 bits, hence takes 7 output ASCII characters per 41 bits, hence a 32 *byte* hash takes $32 * 8 / 41$ chunks of 7 Blake58X output characters *each*, hence the total output size is $(32 * 8 / 41) * 7 \equiv 32 * 7 * 8 / 41 \simeq 43.7$ Base58X *characters*.
- Base58X: same output size as Base58 while I'll have far fewer divide and modulo ops than bitcoin Base58 (Nakamoto) code as I don't treat the entire hash as one *BigInt*, but work in an intermediate 41-bit number base system instead.
- However, note that as Nakamoto's approach to Base58 is treating the entire byte series to encode as one *BigInt* and my own approach of 7 chars per 41 bits -- which is *less space efficient* as I add gaps in my value space that way -- **should be worse than Nakamoto's**, this does *not* explain how my own growth factor of $7 * 8 /41 = 1.3658536$ is *less than* the Nakamoto factor mentioned at Wikipedia: $(1 / 0.73) = 1.3698630$?! 
  
  Consider a value $n$, then it would take up ${}^{g}log(n) = \frac {log(n)} {log(g)}$ *digits* in base $g$. Thus a *32 byte* number (max value: $256^{32}$) would cost $\frac {log(256^{32})} {log(58)}$ *digits* in Base58: $\left \lceil \frac {log(256^{32})} {log(58)} \right \rceil \equiv \left \lceil 32 \times \frac {log(256)} {log(58)} \right \rceil = \lceil 43.701 \rceil = 44$ Base58 *digits*. That's what Nakamoto's approach should produce.
  Meanwhile, I'ld chop the 32 byte value ($32*8 = 256$ *bits*) up into $\left \lceil \frac {256}  {41} \right \rceil = \lceil 6.24 \rceil = 7$ chunks, encoding each into 7 characters, thus producing $7 \times 7 = 49$ Base58X *digits*. *Of course*, the last chunk is a *partial* one, so a smart encoder could indeed limit the cost to $\left \lceil 32 \times \frac {7 \times 8}  {41} \right \rceil = \lceil 43.7 \rceil = 44$ Base58X *digits*.
  
  But my concern are the growth factors. Let's have a look:

  | encoding     | calculus & growth factor                   | 
  |--------------|----------------------------|-----------------------|
  | HEX:         | $log(256) / log(16) = 2$, which is expected |
  | Base64:      | $log(256) / log(64) = 1.3333333$, as expected. |
  | Base85:      | $log(256) / log(85) = 1.24816852$ vs. Wikipedia's $(1/0.80) = 1.25$|
  | Base91:      | $log(256) / log(91) = 1.22929509$ vs. listed $(1/(1-0.23)) = 1.2987012987$ **ugh!**  |
  | Base91:      | $log(256) / log(91) = 1.22929509$ vs. listed $(1/(1-0.14)) = 1.16279069767$ **ugh!** |
  | Base58:      | $log(256) / log(58) = 1.3656582373$  vs. listed $(1/0.73) = 1.369863013699$ |
  | **Base58X**: | $7 * 8 / 41 = 1.365853658537$       |

  and there we see that Base58X is, **as expected from theory**, indeed slightly worse than Base58, in the 5th significant digit of the growth factor. *Worse, if only very little*.
  
  And there we also see that the listed growth factors in the references simply carry too few significant digits to be *decent* here: Base58 *must* be better than *Base58X* and now you can see that is indeed true.
  
  Does this change our numbers and conclusions?
  
  *No.* The encoded 'numbers' (BLAKE3 hashes) are simply too small to make this issue matter already, but the bit of logarithm and *number base* applied theory was dearly needed to explain the observed error and *my temporary confusion*: because the numbers didn't match expected reality, I went looking for the bug, possibly in my original write-up of this, but it turns out I was correct all along, just using flawed reference numbers. *Bug found. Bug fixed.*
  
  
  Anyway, these growth numbers tell us what we were curious about initially:
  
  - Our Base58X approach is 1.431‱ more costly than Nakamoto's Base58 in space, but saves a bundle on division and multiplication operations, thus winning big in CPU load costs. 2‱ is 2 per *10000*, BTW.
  - Industry-ubiquitous Base64, the trivial go-to for converting binary to text, results in a 33.3% space increase. 
   
    > This is relevant for database storage of these hashes: SQLite **does not support BIGINT, not really** as can be gleaned from section 3 ("Type Affinity") in https://www.sqlite.org/datatype3.html:
    > 
    > > A column with NUMERIC affinity may contain values using all five storage classes. When text data is inserted into a NUMERIC column, the storage class of the text is converted to INTEGER or REAL (in order of preference) if the text is a well-formed integer or real literal, respectively. **If the TEXT value is a well-formed integer literal that is too large to fit in a 64-bit signed integer, it is converted to REAL. For conversions between TEXT and REAL storage classes, only the first 15 significant decimal digits of the number are preserved.** If the TEXT value is not a well-formed integer or real literal, then the value is stored as TEXT. For the purposes of this paragraph, hexadecimal integer literals are not considered well-formed and are stored as TEXT. \[...]
    >
    >  (Emphasis in the quoted text is mine.)
    
  - Using Base58X instead of industry-ubiquitous, yet bothersome, Base64 (due to the character set in the latter which may need additional escaping in several communication protocols out there, e.g. HTTP GET/POST/REST), results in an additional 2.44% space cost increase vs. Base64, which is negligible -- while we do not have any obnoxious character anywhere in our Base58X charset!
    Meanwhile our computational encoding/decoding cost is slightly higher as we need to do a bit of judicious bit-shifting to arrive at those 41-bit chunks, plus a few more math operations to properly encode the digits in the chunk (vs. Base64 lookup table or similar means). Again, considered acceptable. Hence neglected.
	
  - Incidentally, given https://www.sqlite.org/datatype3.html (referenced above) and general SQL database experience through the years, we cannot have nice *pure binary data* for indexed / *unique key* fields: `BLOB` is generally not accepted as a unique key type in databases; the 37% size increase to a *whopping* 44 characters per *key* (document hash) does not warrant very unconventional shenanigans with our SQLite database, so we'll be storing those hashes in Base58X encoded form, alas.
   
    Were we to use a NoSQL (e.g. LMDB) database instead, then it would potentially make sense to store the hashes in their original, *pure binary* form, clocking in at 32 bytes a-piece.




Anyway, all this fun is more suitable food for a blog article than a commit message   :-D



- https://crypto.bi/base58/
- https://en.wikipedia.org/wiki/Binary-to-text_encoding (and related pages on Wikipedia)
- https://crypto.stackexchange.com/questions/57580/purpose-of-folding-a-digest-in-half et al (I've been considering using a truncated or *folded* BLAKE3 hash to make the fingerprint string a little shorter, but decided against it in the end as it is not so important any more: the Base58X-encoded fingerprint strings clock in at 44 characters each, which is a $44 / (20 * 2) \Rightarrow 10\% \textit{ increase}$ in fingerprint hash string size compared to the original Qiqqa SHA1B (*B* for *B0rk*), while encoding $256 / 160 = 60\%$ more hash bits. 
 
  **Nitpicking**: Yes, SHA1B is variable length, but that's not under user or application control, merely an artifact of certain PDF data hashing results. The shortest SHA1B fingerprint in my collection is 36 characters, and that's *rare*: 5 items in over 20K documents. The *theoretical* minimum size of a SHA1B encoded hash would be 1 nibble per byte, hence 20 ASCII characters. The chance of getting one of those is $1 : 2^{(20 * 4)} \equiv 1 : 2^{80}$ which is *pretty rare* indeed. ;-)
- https://github.com/nakov/Practical-Cryptography-for-Developers-Book 


## Performance of the new vs. old hash: CPU load

Using the new [`mutool qiqqa_fingerprint0`](https://github.com/GerHobbelt/mupdf/blob/master/source/tools/pdffingerprint0.cpp) and [`mutool qiqqa_fingerprint1`](https://github.com/GerHobbelt/mupdf/blob/master/source/tools/pdffingerprint1.c) tools to calculate the hashes of a subset of the bulktest suite (~ 2K documents) the verdicts are: v2 (BLAKE3+B58X) is about as fast or even *up to 4 times faster* in execution time than SHA1B.

Of course one can argue this was not tested with the .NET version of the SHA1B code, but I expect that to be on par or even worse then the code I used for this, which is through use of the [Crypto++](https://github.com/weidai11/cryptopp) library, which is pretty performant and highly optimized generally.

Anyway, the take-away of this is that the new hash is *better* or at least *competitive* with the original Qiqqa hash, everything else being equal, and such has been my goal.

> Note: As the timings are all in the sub-second range, often only a couple of milliseconds, these are thus clearly within or near the noise margin of the timing measurement code, which has a millisecond *granularity* (Windows background tasks not related to the tests add an undetermined extra noise layer).
> 
> A quick check/sampling of the performance data for a large set of PDFs shows a performance factor [range](https://en.wikipedia.org/wiki/Range_(statistics)) of 0.9 .. 4 times *faster* than SHA1B (eyeballed mean speed factor somewhere around 1.5 to 2), using the same machinery (C code, optimized "Release Build" binaries used for running the performance benchmark).


## Post Scriptum

Note the comment section at the top of the `xxx` source file, where we investigate the output size for any type of Base56 .. Base64 encoding: the conclusion is that Base58 (and Base58X) are the best option for short output (44 character 'digits' per 256 bits of binary input) when the 'output is a alphanumeric word' criterion is to be upheld: Base64 (and Base63) can deliver outputs that are 1 character shorter than this (43 'digits'), but do require the use of some non-alphanumeric characters, e.g. punctuation. This would then make such codes harder to parse in many scripting languages and would be affected by the 'encoding in URL and other foreign environments' as discussed at the start of this article.

In that comment section, Base64 is the only potential contender, for it can be calculated faster than Base58(X), thanks to cheap bit-shifting replacing costly divide and modulo operations; none of the others are 'better' in any sense (same output size, same or worse calculus cost).

### Use in an SQL database

We *are* considering using shortened/folded 64-bit binary-numeric hash values as identifying primary keys in the new database layout, based on the consideration that the BASE58X Unique Identifier is a 44-long string and we will have quite a bit of data rows referencing that UID, e.g. annotation records, bibliography line records, etc.etc. all will be referencing the document they belong to via its UID and we can expect to run many MERGE queries: for that reason alone it might be beneficial to keep the linked index **numeric** for faster table merge and other query processing. Further thoughts on this can be read at [[../Database Design/Considering the database design - the trouble with a string UUID]].


