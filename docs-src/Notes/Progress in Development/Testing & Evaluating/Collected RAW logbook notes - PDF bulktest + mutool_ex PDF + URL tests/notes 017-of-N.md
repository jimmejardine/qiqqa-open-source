# vcopy + sqlite + sync + rclone + rsync + ... tests: logbook notes


## Test run notes at the bleeding edge

**Big Caveat: these notes were valid at the time of writing, but MAY be obsolete or even contradicting current behaviour at any later moment, sometimes even *seconds* away from the original event.**

This is about the things we observe when applying our tools at the bleeding edge of development. This is the lump sum notes (logbook) of these test runs' *odd observations*.

**The Table Of Contents / Overview Index is at [[../PDF bulktest test run notes at the bleeding edge]].**

-------------------------------

(This logbook section was started in 2023.)

*Here goes -- lower is later ==> updates are appended at the bottom.*

-------------------------------

### Item ♯00001 - Linux `cp` and Unicode / long filename surprises

When copying NTFS to BTRFS on Linux Mint, I got these error messages from `cp`:  DID NOT expect these. Apparently he Linux FUSE NTFS driver also suffers from MS Windows' long filename/path syndrome, but here I don't know how to fix that. On MS Windows you can get around it by using universal paths, like `\\.\drive\absolute-path\filename`, but there's no such thing on Linuxes. 🤔

```
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/!5/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/!QIQQA-pdf-watch-dir/2018-10-09/Phased Locked Loop - PLL/74HCT9046A Philips Semiconductor PDFs ����������� ������������ 74HCT9046A �������� ������� 74HCT9046A.pdf Philips Semiconductor PDFs datasheets datasheet data sheets 74HCT9046A Philips Semiconductor PDFs.html': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
cp: cannot stat '/media/ger/16TB-linux001/USB_H1/Sopkonijn/Qiqqa-exports-dir/docs_original/UNKNOWN - 增益值-信号极性以及更新速率的选择可用串行输入口由软件来配臵-该器件还包括自校准和系统校准选项-以消除器件本身或系统的增益和偏移误差- CMOS 结构确保器件具有极低功耗-掉电模式减少等待时的功耗至.pdf': File name too long
```

No idea how we're going to resolve this in our sync tool (named `vcopy`).



### Item ♯00002


#### Tests to set up & run (performance checks):

- SQLite performance test with 100K+ documents:
  1. column storage via multiple tables and JOINs: how does this fare as a metadata *variable column count* storage approach vs. ...
  2. *one* metatable which stores all (label, value) pairs for all metadata item labels for all documents, and ...
  3. one table which has columns for the most usual / regular metadata labels (title, author, ...) *mixed* via JOIN (+ flatten) of the above metadata table which now stores all the other, remaining, (label, value) metadata items for all documents, and ...
  4. is there any kind of noticeable *optimum* there, with respect to which and *how many* columns we designate as "*regular*" and thus park in the first, *wide*, table, vs. the second *gathering* table?

  And then the question also involves the question whether we'ld benefit from different \[types of\] *indexes* on the different tables. Plus: can we otherwise *optimize* such metadata gathering SQL queries, when, say, we want the metadata record to be gathered at query level, i.e. reported as a *very wide set of columns*, vs. leaving such *pivoting* to the requesting software?
  > 
  > Initially I thought it might be handy such a "*flattened*" SQL output for third-party users, but anyone can easily gather & merge multiple rows *by themselves* when they want this type of output, while folks who are only interested in certain *columns* for feeding into follow-up queries, he *flattening* is only detrimental to *lump sum consolidated* performance of the Qiqqa database query system... 🤔 so everyone might be better off with sticking with the *mostly RAW* output, which might be a bit chaotic when we land on database design/layout *numeros* 2 or 3 rom the list above.
  
  









### Item ♯00003

While I was lookin into other stuff, I ran into a Microsoft note mentioning -- rather, someone referring to Microsoft while noting -- that it's **safer/more stable** to use UNC path specs, rather than drive-mapped path specs, as the latter can cause *infrequent, random* access errors, **also on local (in-machine) drives**. *WTF?*

🥵... and now I can't find the page where this was mentioned. *Of course.* 🤬











### Item ♯00004

#### MCMC? (Monte-Carlo simulations for optimum parameter config selection)

- ***forget*** Gibbs sampling as that one requires a predefined distribution and we don't have that -- or it would be a fat lie / low confidence guestimate anyhow.
- check QMCMC: *Quasi* Markov Chain Monte Carlo.
- also: GA (generic annealing): this is another term I forgot, but matches the idea I'm walking around with...
- **Sobol Sequences**: the name for what I keep in mind by recalling Cebas' render engine and the way it samples 3D space. In our case, the Sobol sequences are exactly what I need to keep the sampling "evenly distributed" while we dive towards local+global optima coordinates.
- On that same subject, related: QMC: Quantum Monte Carlo. And **(quantum) tunneling**, where the next estimates are chosen so we prevent sticking around a sub-par local optimum *virtually forever*.
* *differential evolution*
* [Low-discrepancy sequence](https://en.wikipedia.org/wiki/Low-discrepancy_sequence)
* 




#### Detect Text orientation on page; rotation; skew

- **Radon transform** -- very similar to the Hough transform but more closely matches hat I'm imagining might work for most images / page renders.
- [**auto-correlation**](https://en.wikipedia.org/wiki/Autocorrelation) of the scanline histograms -- because I see this stuff being quite similar to *pitch detection* in audio processing.
- (**degree of coherence**) -- as an aid to the measure evaluation while looking for the most probable rotation angle.
- 








### Item ♯00005


#### tesseract charts (debugging/diagnostics feature)

Generate using D3 and overlay them in HTML over the images they regard, where the image serve as background image of the chart: two HTML elements overlaid over one another. No need for a sophisticated C++ chart library then!



Re FTS and typos, etc.: we had a look at levenshtein distance measurements, etc. --> pick up some tricks seen elsewhere and expand on those:

- reduce the characters to a MOD 4 alphabet, so that multiple characters map onto a single alphabet char, which is then included in a 3gram. That's what the others did. Not very effective when the typo jumps in the MOD4 alphabet, i.e. 25% chance you get the typo that way, and only for replacements. Instead, how about...

Also there was the github paper which mentioned the variable length n-gram construction they did for their source code search engine. How about...

We want to be able to recognize words like:

- TL;DR  --> semicolon is not always a word sep!
- 2,830  --> find values including thousands separators, etc.
- 100
- 100ml	 --> numbers with postfixes
- 1945AD --> 1945 AD
- 200BCE --> 200 BCE
- 3-gram --> '-' dash isn't a separator or hyphenation-dash, at least not all the time.
- Covid19  --> values with prefixes
- Quote500 

Stop-words... how about we scan a document and determine its word frequency and character frequency and let the, say, top N of those determine the stop-words and the low-ranked characters, which would drive the tokenizer to produce an N-gram for higher N.

Say we keep a local and global (summary of the entire database, updated for every inserted document) character alphabet histogram, which determines how we tokenize and produce n-grams to be injected into the search index.

However, that would also mean we store a limited and *unpredictable* set of n-grams of varying length N per document as we seek to reduce the number of indexed n-grams that way and will be sure to store / filter-reject different n-grams for different documents when we use a local histogram for alphabet (and ditto for words), ...

... so the question becomes: can we produce those varying n-grams all as a larger search set when we query the database? The point here is: if we can, then we increase query cost re number of n-grams to search, against a tighter and thus faster index/database, compared to the classic approach where all n-grams, including the very frequent ones, are stored, resulting in a rather heavy bloated and slow search index, while the number of search n-grams remains smaller.

I lean towards the former approach, while we haven't tested both yet.

Additional thought re the latter: we can "optimize" a little by storing a limited set of POSitions per document, thus reducing the costliness of frequent n-grams in the index: say we store 30 positions, and position 31 stores a bloom filter of the pages where the n-gram occurs further down the document. Stop-words / stop-ngrams may be stored as a single entry per document, foregoing those 30 POS slots and immediately storing that bloomfilter-like which-pages-have-this-one bit-set so further cut down on index production cost.

The the search can be optimized for the generated set of n-grams-to-check by keeping a separate n-gram histogram index and sorting the search set against these frequencies: the rarest n-grams are sought first as those will filter the document set the fastest.

When we keep an n-gram histogram in the database, we can produce selectivity statements for our FTS searcher to use: only those n-grams that are reasonably selective will be used in the search, unless we cannt help ouselves, in which case we'll be processing a LOT of documents and should warn the user up-front. 

When you have a frequncy-of-occurrence histogram for your alphabet, you can construct variable n-grams akin to the github approach by constructing n-grams from characters while adding their "selectivity" (1-occurrencefrequency/totalDocumentCount) until we surpass a threshold for that value: that would be the end of our n-gram then. Repeat for the next one, etc. StartOfWord and EndOfword markers are important and start/terminate n-gram construction.

All of which does not address the levensthein distance and typo handling in search: for that we need to produce a typo map somehow of possible errors; then generate all possible character combos and heir ngrams and search those. Which is ... horrible. So we'ld better try another idea: at indexing time, the words are pulled through a speelchecker and the top-ranked word is picked up as a lower-ranked alias for the current word and fed to the n-gram pipeline alongside, so its n-grams will enter the search index as well. Then at search time we do the same for the search words: pull them through the same spell checker and use those top-1 or top-2 (? trying extra hard to account for typos, are we ?) as lower-prio aliases, which are n-grammed alongside and thus should deliver results as they'll match the lower-ranked aliases stored in the database, when the main words themselves don't deliver sufficient results. That way we should be able to cope with plenty typos / writing errors in the text with relatively little overhead: double size of index plus double number of n-grams to search, all of which are put through the sort + rank-according-to-global-histogram rule, so we could argue the seach set might be twice the size but still only cost the SAME as this would just mean more search set n-grams don't make the cut (threshold) to participate in the selective document search.

Which leaves me wondering whether we should store POS info in the search index at all: when we have found matching documents, we need to check against the other "non-selective" n-grams as well so we need some sort of (accellerated?) scan of the loated docuemnt anyway. As the cost of storing all n-grams + position (4 bytes per char as each char will produce at least one n-gram (32bits hash) will be prohibitive, we must do this on-line, i.e. at the end of the search. OR... we postpone this until the user specifically asks for such a detailed search/match inside the given document: only then do we rerieve the document and compare its regenerated ngram stream against our entire search set. Which will be costly, but hey, somewhere you'll have to pay to be this precies in your reporting...












### Item ♯00006








### Item ♯00007







