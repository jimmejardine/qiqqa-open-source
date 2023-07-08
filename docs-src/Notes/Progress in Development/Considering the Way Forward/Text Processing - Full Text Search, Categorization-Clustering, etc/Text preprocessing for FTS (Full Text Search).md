# Text preprocessing for FTS (Full Text Search)

When you scour the Internet in search of a good *in depth* FTS grokking, it quickly comes across everyone and their pet dog are singing the same song & dance:

- **normalization**: ditch or replace non-standard punctuation by standard delimiters. (Of course, *you* define the standard, so this varies a bit between blog and whitepaper authors.) *Normalization* is most often described as "*cleaning up the text*", i.e. removing undesirable images, page numbers and other non-essential {page} headers & footers, that sort of thing. Just so you'ld end up with the original text *content* the original author might have written in his text processing tool of choice. *Rarely* someone mentions *Unicode Normalization* as part of this general house-cleaning activity.
- **filtering**: ditch all non-alphabet characters. ditch stop-words, i.e. words you consider to be superfluous and/or useless.
- **lowercasing**: trivial, because *You* and *you* are considered equal. *duh*. (If only it was that simple...)
- **chunking** a.k.a. **word separation**: something that's optional but *not optional, not really* when you're confronted with certain languages' input, e.g. East Asian languages such as Mandarin: turns out to be a tough nut to crack and methods vary.
- **tokenization** a.k.a. **n-gramming**: everybody says *n-grams* but they all mean *slightly* different things: there's the bag-of-words crowd where a 1-gram would be a single *text word*, while plenty peeps consider '1' to be "*one character*", but then a lot of whitepaper authors are U.S. Americans (renowned for their indomitable international language skills), so I'ld wager the boundary between "*1 means one(1) {Unicode} **codepoint***" and  "*1 means one(1) **byte***" crowds -- I've seen both -- is rather... permeable. Meanwhile, almost everyone says $n$ equals $3$. Give or take a few detractors, who advocate $n = 2$ or (shudder!) $n$ being a *range*: the latter also features as "*multi-gram*" in several papers.

That's the {snarky} summary of the state of the art, when it comes to FTS prep.

## On to the *curious* & *most interesting* bits I've seen!

### First, a bit of history: where are we coming from?

FTS is *very* desirable for Qiqqa and yours truly, but it's not a *done deal* when you start to ask questions like "*does it work well for you?", "*can it actually answer the questions you ask?*", "*do you get useful results from a search?*", "*does the machine deliver **timely** answers and are these suitably **specific**?*" or, in other words: "*is your FTS engine **effective**?*"

Long before I got involved and messed it all up, {commercial} Qiqqa started out with FTS via Apache Lucene. It was *hip* when it got into Qiqqa, by the time it was open sourced, it was antiquated cruft and -- at least with my own PDF libraries -- failing horribly.

> Qiqqa used the Lucene.Net "port/derivative" instead of the mainline Lucene, which is done in Java. Let's be swift and just summarize this into a single line: *Java and .NET don't mix*. Picking the .NET port was obvious (IMO -- I probably would've made the same mistake); the result of that choice was stagnation and small community from the start as .NET developers are basically raised on a Steve Ballmer diet. Which results in large numbers and little open source activity. I'm ditching Lucene.NET first chance I get.)

Next to that (and, *yes*: I know I'm mixing subjects together here, but these are strongly intertwined from my ***user*** perspective, so I treat them as a single {yet complex} organism in this text) there's keyword discovery (a.k.a. "*keyword extraction*") based on classic TF-IDF. 

> I'm still unsure how exactly that thing is coded; from the code inspection (a.k.a. *RTFC*) it looked like bag-of-words fed into a TF-IDF calculating codebase, but it gave me headaches and I'll have to look again to be able to make any *definitive statements* about it after 2-3 years hiatus. All I do recall (when doing the RTFC) is *urgh* and, as a ***user*** of the auto-keyword-suggestion feature: *what-the-heck*? 
> 
> **FAT WARNING**: I've yet to make something that does it better, so take my criticisms for what they're worth... not much. 


### Why do I "mix" ***keyword extraction*** and ***FTS*** together here? 

Because both use the same technologies' mix for "preparing the document text". Quite a lot of what's important for keyword extraction is also important to do (the same way!) for full text *content search*. And this blurb is focusing on that **preprocess***, *lump sum*:


### Let's revisit those different parts of "Text Processing" quickly!

Here's the set again:

- **normalization**
- **filtering**
- **lowercasing**
- **chunking** a.k.a. **word separation**
- **tokenization** a.k.a. **n-gramming**

Allow me to rephrase and relist this set with a few addendums. Just because this set is too vague and embedding too much complexity for me, as it is.

Let's split **normalization** up into different activities so I can see them better:

**Normalization** equals:
 
- **document extraction** + 
-  **document cleanup** (housecleaning, big cruft removal) + 
 - **text normalization** (*detailed* cleaning, such as Unicode normalization and *typical print things* like *ligatures*: rewriting)

I involve **document extraction**, because that's the step *just before* normalization, in its classical sense, but I find that the extraction process has a *significant* impact on the form and quality of your normalization process' **input** and thus causes all kinds of crazy failures and deteriorated quality overall:

- the *possibly best thing that could happen to you* is the PDF document(s) you wish to index (& consequently offer up for search afterwards) carrying their texts as-is, i.e. in Unicode encoding, no fancy dressing, no encryption, "copy protection" or other *bleedin'* publisher's shenanigans, as-the-author-wrote-it plain **text**. 🎉Yay! 

  Unfortunately, even this *luckiest-of-the-lot* has subcategories:
  
  - Suppose your authors were not just knowledgeable in their field *but also able to write good prose*. Also suppose they had the benefit of a golden editor and a conscientious publisher who strives to produce highest quality output, so your typo count is *zero*(0) and the content scores a perfect A on any spelling bee. *You're golden*!
  - It is with sincere regret that I have to inform you that many, even at the highest level of education, can't spell. In this Venn diagram of despair, there's another (partly overlapping) set that simply *can't write*: I'm not aiming for a Pulitzer or *literature* here, no, some just can't write a grammatically consistent and *intelligible* line, if their life depended on it. (I've ghost-written and edited a bunch of theses; I was quite naïve as it was only later that I learned some peeps earned good dosh with this activity. *SAD*! as The Donald would say.)
    Why is this bit *relevant*? Well, feed *crap* into a machine and you get *lots of crap* out; it's weird and complex and hard to explain, but you can safely assume a computer is *very good* at one thing only: *multiplying crap*. (or should we call it *exponentiation*? 🤔) Anyway, feed a complex animal, such as a FTS engine, a wee little bit of cruft and you end up with a pile of dung that can't retrieve those tiny-bit-of-crap-carrying documents, no matter how sweet you beg. All you can do, as the machine's designer, is take extra countermeasures *praying* they will suffice to overcome this little hump.
    What sort of countermeasures?
    There's many:
    - This is why peeps advocate *character-based* n-grams, i.e. part-of-word tokenization and indexing, so you *might have a chance the typo in the word you seek is not part of one or more n-grams produced from the _correct_ word as entered by the query-issuing user and _thus_ you luck into a potential match, i.e. you _might_ be able to find that document, after all!* 
    - Some peeps suggest using classical approaches we've been using for decades when *correlating multiple databases* is financially important. For example: mixing a couple of databases from various places when investigating *potential* tax or banking fraud: one typo in a street address or name and one less {potential} suspect to investigate, because the machine didn't find matching records! 
      Here's some classical approaches that I would consider; most of these have been suggested in published papers by others already:
      - ***applying Metaphone or similar rewriting algorithms***; you kill several opportunities for mistaken vowels that way, for instance. Or 'dyslexic' d-vs-p errors and such when you do a (euro-languages oriented) *visual glyph transform* vs. a *phonetic transform*. Both have their use and place and depending on input you might want to apply both (raw voice record transcription documents, for instance, that still need cleaning up). 
        Anyhow, I've seen several of these suggested for FTS index prep and several more used and published in classic CS literature. All have their use and none of them have ever been whispered to be the illicit child of the Golden Goose: you exchange robustness against certain crap with having to face *other*, **new** crap, and lots of it! -- **produced by the machine!** --  as some things now turn up as matching=*correlated* while even *granny*, hopped up to the geriatric gills on 21st century opiates, can spot the glaring difference. What they, *in the business*, refer to as: "*promising but requiring further research*". It's cute and bog-ugly, all at the same time.
      - ***sorting*** the letters in each word, so ROFL 🤣and the (typo) ROLF will match as both sort to FLOR.
        Consequences? *None what-so-ever. Honest, guv'!* See the item above. "*Promising but requiring further research*." Youse feel me?... 😉
      - pulling your input feed through a dictionary a.k.a. **spellchecker** and correct all automatically. Of course this approach *nukes* any new jargon or other bit of blurb that's not in your spellchecker's dictionary *yet*, so this is not a very happy choice to make.
      
- the *next best thing that could happen to you* is the PDF document(s) you wish to index carrying the text as-is, but *tweaked* for printing-fanciness: it's the same as above, but now with *ligatures* and possibly some *layout tweaks* that'll make your brain (and your text extraction machine) *hurt*:
  - the ligatures are the easiest thing here: simply unwrap them into, äh, their *constituent parts*, e.g. - `ﬃ` becomes `ffi`, and so on. Here's your list for starters: https://en.wikipedia.org/wiki/List_of_precomposed_Latin_characters_in_Unicode but better to check this one: https://www.unicode.org/charts/collation/ as at least there you'll find beauties like you'll encounter in old Dutch texts such as № (for `numero` (*latin*)); *oh, my heart* 😍
  - the *printer fanciness tweaks* I mention -- haven't found them mentioned elsewhere and not read or heard a "common term" for these buggers -- is where the *real hurting* starts!
    
    I've seen several PDF documents that *look* nice, possibly even *superb*, from a *visual page render perspective*. However... Look at their "text extracts" and you quickly find all is *not well* on the Western Front. Sorry, Erich. We're *fubar*! Because the PDF-producing machines/layouter apparently decided they needed to *tweak* the PDF content to get the look=layout they crave: some old TeX versions suffer from this, apparently, as do some magazine articles and other sources I've run into over the years: what happens is, is that ***what looks like a regular word*** is (in HTML5/CSS parlance) `position:aboslute`-d on a character-by-character basis and most (*all!?*) PDF text extractors simply cannot cope, so they produce stuff like `H  o li s  ti c` instead of `Holistic`, which, of course, will make your word-tokenizer, further down the FTS index-prepping process lane, go completely *bonkers* as that's *definitely* 6 *crazy* words instead of a single *sane* one.
    Some culprits did this to *titles* but I've also observed PDFs where the PDF encoder/generator apparently was coded to do this for *the entire document*, turning the text extract into *total & utter crap*. *Bloody useless.*
    
    Want more? How about the PDF document producers that did *bold* printing the ***really really classic old-fashioned way*** which is double-printing the glyph! Given my collection, that's been *in vogue* for quite a while!
    For y'all: instead of merely text-extracting to `Holistic` you get extracts like `HH oo ll ii ss tt ii cc` (see what they done? 😁) or, even better(!), `HHoolliissttiicc`. Or any combination there-of. Now you get where that "*sort characters in word and `uniq` them*" algorithm bit I mentioned (in passing) earlier comes from? Clearly I'm not the first one with that particular headache!

    Want more? How about the PDF document that has all the text, nothing amiss or "*tweaked*", only the PDF producer or rewriter or whatever bloody piece of software engineering made this, decided it was "good" to produce the page on a more-or-less *scanline* basis, *possibly to spare old Postscript printers, when (D)RAM was expensive*, from throwing a tantrum halfway down the page: your modern text extractor however ***does not know about multiple columns of text printed in that paper*** so you get garbled output like "`icine has not yet porodu-   ated several cases of cere-`" and many more "lines" like that as the 2 columns of text are neatly *interleaved* in your text extract.
    And it's a horror anyway, even if they went *without hyphenating words*, as you'ld be hard-pressed to make sense of `medicine has no yet produced   several cases of cerebral palsy` either (unless you're a ***very hard*** cynic, of course, but I wasn't talking to *you* 🤡)
    I've yet to find a text extractor that copes nicely with all or *any* of these. I can only pray I'll be able to approach anything like that myself in my own attempts at producing a *robust extractor*.

      
- ah, but we're not done with you yet! The *next (totally not best) thing that could happen to you* is the PDF document(s) you wish to index is *encrypted* by *obfuscation tools*.
  Some *mofos* (and I'm not inclined to PC my opinion on these) decide somewhere up in the corporate till that:
  - *yes, you can have this whitepaper electronically*, and 
  - *yes, you MAY print it* (*oooh, Thousands Thanks, my Benevolant Lordships! \<genuflect\> \<genuflect\> \<genuflect\>*) but 
  - *nooooo, you shall not copy(-paste) from our copy, because we're protective of our inferred IP: after all, we had a peon of ours actually **write this stuff**, you do realize!*
  Fine, just set the copy protect flag in your bloody PDF and I'll remove that nauseating flag *pronto* and we'll all be happy... NOPE. A few of those buggers went smart on my collective ass and *obfuscated the PDF content*!
  Sure, as an *engineer* (and a quite cynical one at that), I grin my wicked grin and appreciate that someone actually clonked two brain cells together and *begat an idea*. There's basically two types out there (a mix is possible, but I've yet to run into such a *cocktail* 😋):
  - *obfuscation by font glyph recoding*, i.e. *glyph code reshuffling*. Here you take a regular TrueType font, ip it to character (*glyph*) shreds and create a *new* font that's got all those glyphs, but now at different codepoints. Meanwhile you do the same remapping shuffle on the text and when you plonk that into your PDF none's the wiser: viewing is A-okay. Printing is A-okay. Text-extracting (and therefor Ctrl-C copy to clipboard) is completely conked and utterly useless.
    Say `H` maps to `#`, `o` maps to `l`, while `l` maps to `3` and `i` maps to `$`, while `s` maps to `c`, `c` maps to `U` and `t` maps to ` `(space), then `Holistic` comes out as gobble-di-gooky `#l3$c $U` and you're out of the race and back to square one. (Just pray you can statistically *recognize* this shite, then you MAY attempt a statistical cryptanalysis to discover the mapping, possibly aided by OCR efforts, but whatever wicked counter-attack scheme you come up with: you are in for some tough job!)
  * *obfuscation by text reordering*: this is the *evil spawn* of that `position:absolute`-like glyph-positioning I mentioned earlier: now it's done with *malicious intent aforethought*.
    You take the page content (text), note the $(x,y)$ coordinate of every glyph involved, then *shuffle* those and spit it into he PDF.
    Not going to show a (simplified) demo of this one as I guess it's obvious by now that you'll get gobble-di-gook a.k.a. *utter crap*, while *viewing* the rendered page is fine and printing it ditto: this aims to corrupt any chance at copy-pasting the text content. Observed several instances in the wild, just as with the previous *obfuscation* process.

  The trouble with these *obfuscated* PDFs is that your average text extractor is *severely thwarted and in a **very bad way** too*: the usual approach these tools take is this:
  1. is the PDF text content accessible as such? if yes, dump!
  2. is the PDF only pictures, then it's probably a *scanned* document and we kick-start our built-in OCR engine to produce some text for you: dump!
  and that's all.
  Now both *obfuscation* methods trigger question 1 above as a YES, because the questioner (your text extraction tool) is notably *stupid* and does not *analyze* the text content obtained to see if it's gibberish or *possibly the real deal*; at least I haven't encountered any text extractors that had such smarts built-in and working properly when faced with these *obfuscated documents*. The **consequence** of this is that your text extractor makes the wrong decision and gives you crap, instead of defaulting onto number 2 and taking the tough road: OCR-ing the rendered page in hopes of extracting some decent content that way!






LDA / LSA / NMR / Topic Modeling / Topic Extraction

What to search for: determines filtering and is non-standard: we want to be able to find these "words" too!
- TL;DR --> `;` semicolon isn't a word-sep when used that way?
- 1940AD, 1200BCE, 2018 --> we're interested in year / dates and must grok suffixes
- 100ml, 50ct --> values (with optional suffix)
- 2,470, 1500, 12.99 --> accept values, floating point and with optional thousands marker.
- `there's` --> should we expand/unroll these contractions as part of our normalization ("`there is`")? These are specific to English, Dutch has `'s morgens` --> `des morgens`, which is *archaic Dutch language*, while the contraction is still regularly used today. Hence should we do *nothing* and keep all contractions as is, only recognizing the lone `'` as part-of-word? 
- `3-gram` --> `-` dashes are not separators when used as hyphenation/connectors?
- COVID19 --> don't ditch numerals as part of word either; these would probably count as "*named entities*", which we would like to search for. Ditto for chemical formulations such as `C6H12O6`, though this would be more prone to error and confusion as such stuff is generally written as sub- or superscripts ($C_6H_{12}O_6$) and given our other findings about `position:absolute`-like placements and related obfuscation techniques in PDFs we'ld have to be watching for these subscripts to be kept as part-of-the-line, instead of extracting them as possibly-different-line-of-content. A further complication there is "*ruby*" layouts, where alternative pronunciations (or a translation into another language) is written *above the line itself*, often in a smaller font size. For reference see https://html5doctor.com/ruby-rt-rp-element/ and https://en.wikipedia.org/wiki/Bopomofo for one type of use for this in regular print. Here's a nice {complex} example from https://chenhuijing.com/blog/html-ruby/ -- *complex* because it shows **three** lines, where one line would show *above* the baseline while the second ruby annotation should show up *below* the baseline:
  
  <ruby> <ruby xml:lang="zh"> <rbc> <rb>大</rb><rp>(</rp><rt>dà</rt><rp>)</rp> <rb>马</rb><rp>(</rp><rt>mǎ</rt><rp>)</rp> <rb>女</rb><rp>(</rp><rt>nǚ</rt><rp>)</rp> <rb>子</rb><rp>(</rp><rt>zǐ</rt><rp>)</rp> <rb>篮</rb><rp>(</rp><rt>lán</rt><rp>)</rp> <rb>球</rb><rp>(</rp><rt>qiú</rt><rp>)</rp> <rb>队</rb><rp>(</rp><rt>duì</rt><rp>)</rp> </rbc> </ruby> <rtc xml:lang="en" style="ruby-position: under;"> <rp>(</rp><rt>Malaysia Women's Basketball Team</rt><rp>)</rp> </rtc> </ruby>




A good text extractor and text *preprocessor* for FTS, *topic modeling* and what-have-you always has to mind about all the above, no matter the *purpose* (FTS indexing, topic modeling via LDA, ...) the final output will be used for. It's okay if the tool can't cope with some inputs, but it should *at least* be able to somewhat intelligently *signal such occasions* so outside processes and/or user interventions can take care of the matter -- in the end there will always be some texts that will require a Mechanical Turk like treatment, i.e. transcription by hand. 
We also would like to ***store the intelligible extracted text*** for display and copy-paste purposes, ***together with position data (coordinates)*** so we can properly mark up any selected word (or character!) in the original rendered page. This "*stored text*" would be the output of the *extractor* process chain, including (both *mechanized* and *manual*) corrections and tweaks, but *before* we go and *tokenize* the text into n-grams. We would however benefit from some kind of word separation subprocess this early in the game already as it would also benefit the human reader -- some PDFs come to mind (probably from old TeX versions and maybe from a few other PDF producers) where the extracted (English!) lines come out all jumbled together as someone decided whitespace is for wannabe's and hasn't included *any of that* in their PDF output; spacing taken care of by, you guessed it, explicit positioning, using a system similar to HTML5/CSS's `position:relative` and `position:absolute`: I've seen both in the wild. The regrettable bit here is that I didn't have the tools to discover/detect and *mark* these files for future reference and I had too much else on my plate to consider making the extra effort of marking them *by hand* back then, so we'll have to produce an *analyzer tool* at least and pray they pop up again in the (recovered from system failure) collection...




Ergo: Artifex `mupdf`'s inclusion of `tesseract` into their library's process flow is nice, but insufficient as we've seen now that any text *extracted* from a PDF must be analyzed and *diagnosed* in order to properly decide when to switch to OCR output instead: their approach doesn't cope nor reckon with the (willful) *obfuscation techniques* that may have been applied to the input PDF, necessitating OCR as part of the extraction process for purposes of either reversing/recovery support (--> font glyph remapping) or as base text source otherwise, despite the input PDF having "text" content of its own.



We also see that we need our own *filtering* approach as the usual approach of ditching anything but the alphabet or alpha-numerics is not going to fly as we want to be able to *find* "words" such as `3-gram` or chemical names: for example check out this blurb from https://www.emcdda.europa.eu/publications/drug-profiles/hallucinogenic-mushrooms_en and *do note* the chemical names, which, in this case, include one with an *embedded comma* (*sic*): `4-phosphoryloxy-N,N-dimethyltryptamine`. `

----

## **Chemistry**

Psilocybin (PY, 4-phosphoryloxy-N,N-dimethyltryptamine) is the main psychoactive principle of hallucinogenic mushrooms. After ingestion, psilocybin is converted into the pharmacologically active form psilocin. Psilocin itself is also present in the mushroom, but in smaller amounts. Psilocybin and psilocin are both indolealkylamines and structurally similar to the [neurotransmitter](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#Neurotransmitter "A chemical messenger involved in passing a signal from one neuron to adjacent neurons in the brain or spinal cord.") [serotonin](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#Serotonin "Also known as 5-hydroxytryptamine. An example of a neurotransmitter, it is a naturally occurring substance closely related to naturally occcurring and synthetic hallucinogenic tryptamines.") (5-hydroxytryptamine or 5-HT). Besides psilocybin and psilocin, two further tryptamines — baeocystin and norbaeocystin — could also be present but are thought to be less active than the former two.  
  
Psilocybin (psilocybine, psilocibina, psilocybinum, psylosybiini) ([CAS](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#CAS "Chemical Abstracts Service...")-number: 520-52-5) is 4-phosphoryloxy-NN-dimethyltryptamine. According to [IUPAC](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#IUPAC "International Union of Pure and Applied Chemistry..."), the fully systematic chemical name is [3-(2-dimethylaminoethyl)-1H-indol-4-yl] dihydrogen phosphate. Psilocybin is the dihydrogen phospate of psilocin. Psilocybin is soluble in water, moderately soluble in methanol and ethanol, and insoluble in most organic solvents. Psilocybin is a prodrug of psilocin, _in vivo_ the molecule is metabolised into psilocin by dephosphorylation.

### Molecular structure of psilocybin

![molecular structure of psilocybin](https://www.emcdda.europa.eu/sites/default/files/media/embed/images/psilocybin.gif)

  
  
Molecular formula: C12H17N2O4P  
Molecular weight: 284.3 g/mol  
  
Psilocin (psilocine, psilocyn) ([CAS](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#CAS "Chemical Abstracts Service...")-number 520-53-6) is 4-hydroxy-NN-dimethyltryptamine (4-OH-DMT) or alternatively 3-(2-dimethylaminoethyl)indol-4-ol. According to [IUPAC](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#IUPAC "International Union of Pure and Applied Chemistry..."), the fully systematic chemical name is 3-(2-dimethylaminoethyl)-1H-indol-4-ol. Psilocin is an isomer of [bufotenine](https://www.emcdda.europa.eu/publications/drug-profiles/glossary#Bufotenine "Indole alkaloid commonly found in skin (parotid) glands of Bufo alvarius: Colorado river toad and Bufo marinus: cane toad. Also present in the seeds and leaves of Piptadenia peregrina and Piptadenia macrocarpa..."), it differs only in the position of the hydroxylgroup. Psilocin is relatively unstable in solution. Under alkaline conditions in the presence of oxygen it immediately forms bluish and black degradation products.

----














## Adjacency tests for n-grams and blowing up your index

When you encode your source texts as tokens (of any kind) your basic *presence test*-facilitating (inverted) index will take N slots/rows per document, where N is the number of *unique* tokens as you *only* record whether a given token is actually *present anywhere in document X*. Assuming you're including *all* words (including *stopwords*) in your index, N will be (much) smaller than T, where T is number of tokens generated from your document X. When your tokens are word-based, T is the number of words in your documents, when your tokens are n-gram based, T is the number of characters in your document -- give or take a few as you've cleaned up the text before actual tokenization; we'll ignore this slight difference for the remainder of this section.
When you store multiple token types, e.g. n-grams for n={2,3,4}, then, of course, the index size increases, its size estimate multiplied by the number of token types you store as each type will take up is own chunk of index space.

#### An initial, rough, index size estimate

Thus index size estimate will be some multiple of the cumulative document size, where a few factors impact the size:
- multiply by the number of token types, i.e. every n-gram width (*n*) number used adds 1 to the index size multiple.
- base is the number of *words* in the documents when you use word-based n-gram tokens, while it's the number of *characters* in the documents when you use character-based n-gram tokens, obviously.
- since we're talking about an *existence test* index, multiple occurrences of a single token (n-gram) *value* actually reduces the index size as the token value will only take up a single slot: ✅ *token is **present** in document*

### Index size multiple

Thus we can guestimate/assume that the index will be a (relatively small) multiple of the document size; when you find ways to *compress* the index or *sparsify the token stream* per document, i.e. reduce the number of tokens produced per document, then the index size multiple might even be less that 1.0!
This is relatively easy to achieve for word-based tokens as the index record cost (say: a single 64-bit document ID, assuming we have a (near-)zero-overhead cost inverted index) would compete the average word length in *characters*. Assuming an index where your document IDs all fit in 32 bits and 1 byte per slot *overhead* and compressed integer storage, that would be up to 5 bytes per slot index cost, say 4 on average, while my initial estimate for word length average in regular (academic) text would be 5 bytes/characters for European languages, so we'ld at or slightly below the factor 1.0 already. Any further tricks go get the index more compact would reduce that factor further.
When we, however, would start out with character-based n-gram tokens, we'ld need that same 5 bytes per index slot *per document character*, so we'ld be looking at a multiple of 5.0 right off the bat.

#### Considering stopwords' cost

Of course these initial rough calculations neglected the fact that many words and character n-grams recur multiple times in a single document. When we consider this, it is obvious that processing *stopwords* isn't adding much to the overall cost as the most markable attribute of these words is their propensity to recur *very often* in the text. We *might* have considered filtering out stopwords like `the` before, but keeping these around facilitates search queries where the stopwords occur as part of the search query, e.g. a search for `the beatles` rather than simply `beatles`: having that *stopword* `the` in there will produce a *quite different* result set as I've yet to see a biology paper enthousiastically referring to their subject matter as `the beatles` while a biography or an article from a music magazine would certainly pop up in this search. Stopwords are not as worthless as some would have you believe! And they cost little, compared to the other words, since they won't be taking up much space in the index, compared to the amount of bytes they took up in the source material.

#### Reducing index cost: crazy ideas to check

##### Sparsifying the token stream

I mentioned this one above in passing. I haven't seen this idea as such in de papers I've read thus far, but it seems to be *alluded to* by some in circumspect ways, e.g. the blog article [about how GitHub did their latest source-code search index](https://github.blog/2023-02-06-the-technology-behind-githubs-new-code-search/), where they mention "*sparse grams*" but describe those more like an n-gram system where the *n* is variable, depending on some *unspecified* ranking of the character unigrams & bigrams(?): the blog authors, nor anyone else, have been answering questions about the assignment of the rank numbers (what they call "*weight*") to the various character combos, while only a single example of the process is shown in the blog article. More a teaser ("*see how smart we are*") then anything worthwhile and ready for re-use: definitely played closed to the chest, that one. *pity*.

What they *seem to be doing* though is basically *ranking* the character bigrams in each word. In their case the rank ("*weight*") is coded as: higher is better (less frequent?). Among the bits they don't mention is the fact/question whether this rank is assigned based on classic bigrams (Markov) or *augmented bigrams* (which would be bigrams *augmented* with attributes such as POS (Part Of Speech, e.g. `keyword`, `variable`, `value`, `operator`, ....) and/or SOW/EOW/MOW (Start Of Word, End Of, Middle Of) markers. Personally, I would use the latter as they're cheap to create and enhance the bigrams and thus the n-grams they're constructing while they're yakking about 3-grams not being selective enough in the blog article: including almost-free-for-grabs metadata like S/M/EOW markers comes free. Including POS codes in the bigram weights comes at added cost, but parsing source code files is far more robust and easy to do compared to NLP POS analysis on human texts, so  would certainly consider it as well!

Meanwhile *their idea* of sparseness is them constructing all n-grams or $n \ge 3$  and only keeping the ones where the weights of the edge bigrams are both higher than the weights of the bigrams that make up the middle of the n-gram under construction. Quoting the blog article: "*Using those weights, we tokenize by selecting intervals where the inner weights are strictly smaller than the weights at the borders.*" What their animated demo shows is them constructing 3-grams *plus* (at most) one(1) extra n-gram per character position where $n \ge 4$. Contrast this with approach mentioned elsewhere in a few places in the research literature where folks went and created *all* n-grams for $n \in \{ 3,4,5 \}$ for each character position: the GitHub guys produce a *sparser* collection of n-grams where $n \in \{ 3, k \}$ where $4 \le k \le l$ and $l$ is the length of the word -- unless the GitHub dudes and dudettes aren't telling us the whole story and decided to *span* multiple words while constructing their n-grams: *possible* but *improbable* if I read the blog article tea-leaves right...

Quoting another bit of that same footnote from the blog: "*At query time, we use the exact same algorithm, but keep only the covering n-grams, as the others are redundant.*" What this comes down to is their *query parser* simply ditches the 3-grams when there's a wider n-gram for the same character position.

Note, by the way, that they encode SOW/EOW in their animated demo in another, classic, way: by including the whitespace separator as part of the n-gram under construction.

Now *my* idea of *sparseness* would be to see if we could get away with ditching more n-grams before indexing them: can we get away with, say, discarding every second *middle char* n-gram? Partial matches would suffer a bit, but the odd char positions still would have produced their n-grams for us to match against the user query -- *of course* the query parser *must produce all n-grams for every character position* if we are unsure whether we are expected to match partial words, for we won't know if the user specified a *word part* that started at an *odd* index within the word(s) they seek. 🤔 Given a 5-char word average and 3-grams, you thus would only index 3-gram numeros 1, 3 and 4 -- no. 4 would have to be included as that would be the last one carrying at least 2 characters that are part of the word; we weren't considering *effective unigrams* here either 😉.
Example word: `abcde` would normally produce these 3-grams: `<ab, abc, bcd, cde, de>` where `<` and `>` are the SOW/EOW markers; with this scheme we ditch 3-gram `bcd` so that would be a 20% storage reduction. *Another choice is to drop every second 3-gram* as the remaining overlap would still be a single character between adjacent 3-grams and thus a good chance to get a decent match at query time, while giving us *40%* storage reduction for 3-gram set `<ab, bcd, de>`.

Meanwhile the GitHub approach is a nice idea to include as it's a kind of compromise between using 3-grams alone and using *word*-based n-grams. Which we could also add to improve search speed for queries which look for whole words... Their number would be relatively low, so the index size multiple wouldn't increase frighteningly that way...

> This idea of dropping every second 3-gram from the index set might fly, but only for the larger words, say length 5 and above. (And *never drop the EOW-carrying 3-gram anywhere either!*)
> Consider the 4-char word `abcd` and its 3-gram set: `<ab, abc, bcd, cd>`: we *may* drop `abc` but the others are kinda *essential*; given `bcd` getting into the index that way, for *symmetry* we'ld better include `abc` as well. Come to think of it: `abc` represents an important initial part of the word *prefix*, so this whole *discard every second 3-gram* might not sound so very sane that way any more. Or we go whole hog and discard either from the index -- probably the one with the lower cumulative rank! -- and congratulate ourselves on a 25% redux there.
> 
> Another *tweak* to the above idea would be to select *all* middle-section 3-grams and start **discarding the lowest-cumulative ranking one**, then expanding from there while ensuring enough 3-grams remain to ensure all characters in the word are covered by at least 2 of them -- I suppose I'm saying here I want to find the lowest ranking trigram and discard every second one outward from that one as the consequence of the *overlap* rule I postulated just there is that every second 3-gram will remain, no matter what. 🤔 Hmm, shouldn't we then instead go and calculate the the *lump sum* rank of every even and odd 3-gram involved in the *potential discard set* nd ditch the set that rates lowest? After all, the above will result in an either-other scenario where either the even or the odd middle 3-grams will be ditched and we would like to keep the ones that have the highest worth for our index, for those will be *most selective*!
> 
> Definitely food for thought and experimentation!


##### Can we further reduce the estimated cost of $2 \frac 2 5$ token per character?

Following the intermezzo just above, we *could* drop almost 20% - 40% of the 3-grams there *and pray the quality of the index doesn't deteriorate too much*. Can we do the same for the second, *larger-n* n-grams we're feeding into the index?

I guess so. Take the GitHub quote again: "*Using those weights, we tokenize by selecting intervals where the inner weights are strictly smaller than the weights at the borders.*" Well, if we start off with a pretty low-rated bigram there, we'll end up with a low-ranking 3-gram *and* (iff we follow GitHub's algo here and skip/include all lower-ranking subsequent bigrams into our n-gram-under-construction) a pretty low ranking n-gram as well. Heck, chances are we won't even *be* producing a separate n-gram there as low start rank means there's so much *less chance* to find even lower ranking bigrams following our starter.

Then there's also another situation to consider: suppose we're at or very near the start of the word and the n-gram construction algo above decides to span the entire word until the end because it just so happens that all bigrams in the middle of the word are of lower rank/weight: isn't that n-gram not *functionally near equivalent to the whole-word unigram* (which we were planning on producing as well anyway)? *This* implies that n-grams that span from (near) the start to the end of the word can e safely discarded -- provided we account for this *optimization* in our search engine by including whole-word tokens in the match set for our search input, no matter whether we were were looking for parts-of or wholesome...

As our reductions won't apply to small words (less than 4 characters wide), we won't be getting a full 25 - 40% reduction due to 3-gram selective discarding, but let's assume a probably-safe 25% redux for 3-grams. Add to that some discarded n-grams for larger *n*, which would give an additional savings but expect a lower redux percentage, say 10% for the n-grams. That would reduce our $2 \frac 2 5$ token per character cost down to $75\% + 90\% + \frac 2 5 = 2.05$ token per character. At cost 5 per token the index multiple would then reduce from 12 down to slightly above 10, a 20% reduction overall.

> Which also shows that the cost estimate per token is rather important: 5 bytes per token (see also further below) is a tad much and *anything we can do to reduce that* will impact the multiple *significantly*!
> 
> Frankly, to reduce our cost multiple *tout suite* it is paramount **we reduce the largest *factor* that makes up that multiple first**: that means we won't benefit a lot from discarding tokens the way we suggested just above *until* we've been able to reduce that cost-per-token number down from 5 to 3 or less!!
> 
> **Note**: Knowing that most libraries won't have more than 10K documents, we'ld be looking at document id numbers in the range 1-to-16K, generally, which would take just less than 2 bytes, plus that 1 byte *overhead*, making *3* rather than *5* as we said we need for an *existence test*-facilitating inverted index. Conclusion: reducing the number of tokens per character, as discussed above, *is* relevant as for general user libraries we estimate a cost multiple of $3 \times 2.05 \approx 6$ that way.




#### On to... adjacency! (*positional index* vs. *inverted index* in the jargon du jour)

We are concerned about the work required to match *phrases*, i.e. sets of *words*. This is usually solved by introducing a *positional index* which not just stores the document *id* but also an offset (or coordinate) for each *occurrence* of the given token.

Obviously this *blows up your index* as this will roughly cost one index row slot per character in the documents being indexed: that's a pretty *large* multiple! And it will be *particularly huge* as -- given what we've described and consider thus far -- appear to be considering to produce 2 n-grams per character and at least word-based n-gram (unigram). Iff we are to facilitate *phrase matching* then we are encouraged to *add* word-based *bigrams*. If a single index slot is guestimated at cost $1+4=5$, then, assuming an average word width of 5, the total number of slots will be in the vicinity of $5 \times ( \frac 2 5 + 2 ) = 12$ which is a pretty hefty multiple from where I'm standing. And that is *without* the *positional information* added to the index: without that, we are fairly certain this multiple is a *theoretical worst-case only* as a lot of n-grams will *recur* throughout the document, but only show up *once* in the existence-tested index!

When we turn that inverted index into one where we include position info, we, *all at once*, loose the benefit of the reduction in token numbers stores due to recurrence within each single document, but we'll also have to adjust our "*cost equals about 5 per slot*" estimate *upwards* as we'll have to store those offsets / coordinates *somewhere*: when using an offset we'll be requiring an additional 2-3 bytes per slot *at least*, dialing up our multiple estimate from 12 to nearly 16. *Ugh!*




#### Reducing index cost: crazy ideas to check

What if we *quantize* the position info? Not the obvious way by discarding the lower bits, mind you! Bear with me here! 

What is important to us when we wish to match phrases or *adjacency of words*?

Does it matter the words are, in an absolute position sense? No. What we wish to get from the *positional index* is a swift answer whether several tokens from our query are *close together*. For that it suffices if we can provide the search engine with *relative position info*. Of course, the naïve approach is to store *absolute position* and perform a couple of subtractions to get our desired *relative deltas*, but what if we get a little *sloppy*, say?
How about this: we *quantize* the position info to $p' = p \mod r$ where $p$ is the position within the document and $r$ is a *fixed range number* that's small yet sufficiently large to be effective, say 64. Then the position would (a) only take up 6 bits of space, and (b) the most frequently recurring tokens (stopwords!) would, at worst, take up $r$ slots in the index. With $r = 64$ we would be able to quickly see if two tokens are sufficiently near by checking their quantized position coming from the index; we only have to check the document itself to verify the truthfulness of the adjacency test for a limited number of *false positives*!
Of course this still makes for a pretty bloated index, but we can reduce $r$ to a smaller number, say *16*, and still have a decent *false positive rate*, while the index won't be bulging all that much any more. 

Surely something to check and test as the bother here will be finding the sweet spot for $r$...





















## Text extraction from HTML pages

This *should* be simple as everything is text already, but it is not. Web pages are loaded with irrelevant cruft, e.g. site indexes, ads, etc. cluttering the HTML and all that SHOULD be removed to obtain a *clean informative content page* that we can read and index for search, etc., without getting thwarted/redirected/thrown off-course by search hits in any of those *irrelevant zones*.

Most of the cruft is *surrounding* the text in most pages, so that makes the job of text extraction relatively easier, but plenty web sites have dialed up their obnoxiousness for processing like this by *embedding* ads, i.e. *injecting ads into the main content stream itself*: here we are facing a extraction problem that's severe and increasingly difficult to get right as the web sites aim to obfuscate the irrelevance of the ads injected: their monetization model pivots around making ads *inescapable* o they get more hits and thus more revenue into the site/owner's hands. Meanwhile *we* are interested in indexing the actual content, so we'll have to come up with means to filter these obnoxious chunks as much as possible. Ideally we filter ALL irrelevant content.

There's also the issue of web page *production*, i.e. how the page gets to your browser and gets rendered in there: plenty websites use heavy loads of JavaScript to regenerate the content on the fly, injecting the content in real time. This makes classic web scraping a total failure as that type of *scraping* only fetches the *static* parts of the page, i.e. the page layout framework plus the JavaScripts involved in producing the page: the actual content arrives via *live* XHR/JSON web request or similar means, which your average scraper doesn't grab as it does not actually *render* the scraped page like a regular browser does. (And we forgot about willful anti-scraping measures at the site here...)

The "answer" to this is to defer the *scrape* until we have rendered the page in our (*augmented*) browser. Given that some nastier sites out there check for specific end-user facing browsers and flip the bird to all the rest, any viable scraper for such content should be done as a browser plugin/extension, which runs the (headless?) browser and pipes **HTML+CSS snapshots** of the page content to our scrape storage bin.

Keep in mind here that even such an approach is non-trivial: plenty sites, e.g. Medium, use *lazy loading* for images, graphs, etc. embedded in the page content and that stuff only gets loaded once you have scrolled the entirety of the page. Hence your scraper tool/extension should "scroll" the page in similar fashion so we get a proper, *complete*, content grab off the page when we take the snapshot. *Timing the snapshot* is also important as those loads happen asynchronously!...





