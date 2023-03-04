# Considering the relevant atomic search unit

- as one essay put it: "*3-grams are the sweet spot*" (*for practical implementations* of (inverse) search indexes): 2-grams (bigrams) are not *specific* enough, i.e. result in huge scan lists per index hit, while quadgrams (4-grams) explode your index size, taking up too much harddisk space for too little gain.
- 1-gram equals 1 *syllable* or 1-gram equals 1 *word*? Indo-European languages (such as English) come with their words nicely pre-separated (most of the time) and *selectivity* improves for gram=word: you are hashing more *context* (or a longer Markov Chain) into a single n-gram hash, then when using gram=syllable. However, if that's a success, you're in some deep (word separation task) trouble for most Asian languages as they don't know about *whitespace characters*: Chinese is *hard* to chop into actual *words*.
- lots of advice starts with chucking *stopwords* out. However, if you look at many published stopword lists, there's quite a few *useful* words in there, for example "*high*" or "*extra*" may be listed in the stopwords set, but when you're looking for "*high voltage*" specifically (e.g. Marx generators, but you didn't recall *that* particular name), then you don't want to be bothered with Fluke Multimeters For The Installation Professional, for example. So there's a few folks advocating **not to use stopword filters**: the classical example being the search for the Shakespeare play which contains the line "*To be or not be!*": that one would be *annihilated* by any stopword filter out there as it's *all stopwords*.
  However, *not ditching stopwords* makes your index bloated (the lesser problem) and probably *useless* due to the machine weighting the index statistics as *not specific enough to be useful* to be used in any sane query execution plan (as considered by the search engine machinery).
- 1-gram=syllable and the *word edge*: some peeps argue that the *position within the word* for a given syllable is useful info, which also helps to improve *selectivity*, e.g. 3gram "*ali*" should be *weighted* differently when expected as the start-of-word, whole-word or smack-in-the-middle-of-a-word: "_**Ali** Express_", "_**Ali**|as Black Bra_", "_He|**ali**|ng Crystals_". As this *ranking* should be replicable at the search query construction site for search 3grams to *match* index 3grams, these folks do suggest some *search query introspection* at query compilation time and marking anything that has equal-*or-lower* rank as a *potential hit*, with some rank-sorting heuristics added for improved results' quality, of course.
  
   >You want the *whole-word* or *start-of-word* / *end-of-word* ranked n-gram hits to show up at the top of the list, when it looked like the user query was looking for that particular type, for instance.

-  1-gram=syllable and the *word edge*: others have mentioned another idea about *n-grams at the word edges* -- which incidentally would help *somewhat* when you're also in the business of *not* throwing out those pesky short stopwords: mark up every word with special border markers (which otherwise wouldn't show up in your search index feed), e.g. `<word>`, like: `<a> <cherry> <in> <the> <salad> <bowl>` --> 3grams: {`<a>`, `<ch`, `che`, `her`, `err`, `rry`, `ry>`, `<in`,  `in>`, `<th`, `the`, `he>`, `<sa`, ...}
  Note that they *do not* include 3grams such as `><a`, `><c` as they *sensibly* argue those have the same selectivity as a (worthless) 1-gram, such as `a` or `c`. However, they argue the *almost-equivaleent-to-2-grams* `<ch`, etc. are quite useful when these can be matched against start/end-of-word 3grams from the user-specified search query. And `<a>` just happens to be a legal 3gram that way when you allow all words, so as you can search for "*a book*" vs. "*the book*" and get significantly different search answers: "*the book*" might, depending on other context, place more emphasis on results which mention "*the bible*" or "*the koran*", for example.
- *Attributed 3-grams* are another *quite interesting* proposition I encountered while reading up on search/indexing research: the idea there was to take a 3-gram and *make it more specific*  by turning it in a sort of 3.5-gram: 4-grams being too specific, i.e. producing *too many different hash values* (about 4 billion), while 3-grams produce a search space of about $256^3 = 2^{24} \approx 16 \times 10^6$ hash values. Their idea was to take the input, chop it into 3-grams, *but then take the tail following the **bi**gram and hash it to a single code that turns this into a trigram*, in order to help search indexes produce better rankable / more specific index hits. For example "*James*" would be *grammed* as { `Ja+(mes)`, `Jam`, `am+(es)`, `ame`, `mes` } where the `(...)` parts are first hashed into a single symbol before further 3gram processing is done, e.g. --> { `Ja7`, `Jam`, `am#`, `ame`, `mes`}. 
  Note that *the bare 3grams are also included* in the set: the "*attributed*" 3grams are only expected to help when the search query compiler manages to produce exactly the same *attributed* 3grams. Which would only happen when the user searches for `James`, but not when he looks for `*ames` (where "`*`" is a wildcard).
- Additional / similar *attributed n-gram* approaches I've seen mention mixing start-of-word and end-of-word marker bits into the 3-gram before it is converted to a search hash index slot number.
- Ditto for some folks suggesting *improved results* when you mix 3-grams with 4-grams, 5-grams and/or word-grams, where the latter are all hashed and then mapped onto the 3-gram sized search space. 
  The *3.5-grams* I mentioned above (*my word, not found as such in the research publications*) are *my interpretation* of the latter couple of items, which is taking your *whatever*-grams, producing **unique hash values** for them (so their specificity/quality remains unchanged) and them *folding* / *mapping* them onto a $N$-sized index space using a *mapping/folding operator*, e.g. *modulo*, where 
  $$h = H(ngram) \bmod n \enspace | \enspace 0 \le n < {256^p} \wedge 3 \le p < 4$$
   and $p$ may be larger than 3 to facilitate (mostly) unique mapping of those n-grams into the search index. For example, when we decide 2 bits extra is enough / a good practical compromise, then the modulo value would be $2^{26}$ rather than 3gram's regular $2^{24}$ and we end up with a search space $\approx 64 \times 10^6$ index slots. For a yet-collision-less index at, say, 8 bytes per slot, that would naively cost $64*8 = 512$ MBytes, i.e. half a gig of disk or RAM space, which is still within sane limits for modern user-level desktop hardware. This would then be a "$3 \frac 1 4$-gram" or "*3.25-gram*".
- *Codepoints versus character bytes*: of course, when you consider non-U.S. American English languages too, i.e. you consider *international language space*, you're going to talk *Unicode*. Which comes as *codepoints*, rather than old-skool ANSI/ASCII *bytes*. And then you have to reconsider the question: "*what is my atomic character unit that I'm constructing my n-gram from?*" 
  Some folks just don't bother and rip *raw bytes*, stating that UTF8 will take care of the rest. Others argue this blunt way of thinking reduces selectivity and therefor *quality* of the search results, both in *performance* and *matching* (ranking, selectivity, ...) as you would now find *half-characters* or even *quarter-characters* treated as full character citizens in your trigram-based search index hashes, so they propose to use *Unicode codepoints* for the trigrams instead. 
  The extra argument for that approach is that it could potentially help improve search quality for Chinese and similar non-word-delimited languages as those usually have a larger *alphabet* than English/European languages, which *might* offset the possible quality gain you are expected to get from including start/end-of-word markers as part of your trigramming action. (Remember those "`<`" and "`>`"?)
  > Personally, I suspect this might be more advantageous for Chinese, with its huge glyph "*alphabet*", than for, say, Indian languages, such as Urdu or Devanagari, as those have a much smaller *alphabet* and are using quite a few *modifiers* instead. Regrettably I haven't been able to dig up some good, interesting, search index research for *those* languages, while they are closer to my heart than Chinese (due to personal history).
  > However, there's [UAX #29: Unicode Text Segmentation](https://www.unicode.org/reports/tr29/) which can be of use, as we are now entering the realm of *side effects*/*side issues*: these languages can represent a character (or rather: a syllable) in various ways, so **Unicode normalization** becomes an important part of the operation, and you will find that discussed that article. 
  > Also do note the mention of **graphemes**: another bit of jargon that should sit next to *character* and *codepoint*: a *grapheme* here is considered to be the *normalized* atom consisting of a base "alphabet" symbol *plus* requisite *modifiers*. Language-wise it represents a single *syllable* (more or less) and thus approaches the *symbol density* of Chinese, where one symbol represents either an *entire word* or one (or more!) *syllables* in a compound word.
  > So we end up, *iff* we want to be, äh, *international language diversified* in our search engine technology, with using trigrams of **graphemes**. Or maybe *3.5-grams* of the same. Anyhow, the sub-atomic search unit should be a *grapheme* rather than a Unicode codepoint or a *raw byte*. (With a single n-gram hash being our *atomic* unit.)


## Ideas/solutions I've not seen mentioned elsewhere

**The Problem**: ditching your stopwords, whichever they are according to *your definition*, is reducing your chances at success when you expect to search texts for *phrases* such as the classic "*to be or not be*" example, i.e. when you wish to *find exact phrase elements which may contain locally-relevant stopwords*. When, f.e., you wish to find precisely the phrase "*the extra dose*" instead of just *any* "*extra dose*", or, god forbid, *any mention of any "dose" at all* -- if "*extra*" somehow also made it into your stopwords list.

The (theoretical) solution then is to abstain from using stopword filters. Good for the theoreticians, much less lucky for you, as you now will have a low-specificity search index: "*the*" will drive the search index statistics *nuts* by appearing in almost every line of text in your entire database, while the reverse search index record *construction* for that "*the*" trigram alone will mimic the re-enactment of a full table import: **not fun at all**. And all around horrible performance to boot: at index *creation* and while *using* the bloody result.)

Not much data about how to tackle that conundrum, out there.

**So here's an idea:**

- we take the idea of the *attributed n-gram*, but... with a twist.
- as we want the stopwords to be in there, together with the rest, the question should now be: **how valuable are those stopwords to me?** 
  Or in other words: is a *stopword* a first-class citizen, like any other word? Or should I *rank* it differently (*lower*, perhaps)?
- how about we rank a stopword as value-equivalent to about *one grapheme*? 
  After all, the original argument for introducing *stopwords* was those buggers were as obnoxious and frequent as your regular `a`, `b`, `c` *monograms* (1grams). Or *bigrams*. And about as valuable/useless for *fast* & accurate/specific search indexing. When compared to the occurrence frequencies (*percentages*) for *regular* trigrams...

So... we're talking about *mixing* stopwords into the n-gram production process that's part of the search index fill & use (= *search*) procedures, at a rate of 1 stopword equals 1 grapheme (or there-abouts) and our n-gram is a 3-gram, so we'll take 3 of those subatomic units to construct our 3-grams. We can always decide later to fiddle with that 1:1 conversion ratio, but now we'd see something like this for "*to be or not to be*":
--> sub-atoms (**grapheme** equiv.): { "`<to>`", "`<be>`", "`<or>`", "`<not>`", "`<to>`", "`<be>`" }
--> 3-grams: { "`<to><be><or>`", "`<be><or><not>`", "`<or><not><to>`","`<not><to><be>`" }
which would be *quite specific* per 3-gram (i.e. a low frequency of occurrence per index entry, thus good overall statistics on the search index to help make the search engine decide to use this search index for the user-specified search, thus expected *fast* and *high quality* search results, when the user searches for "`not to be`" or similar short extracts of the original line.
Ditto for the search example "`the daily dose`" when the search index has lots of articles mentioning some "*dose*", discuss quite a few "*daily dose regimes*" and a rare one refers to the title / term "*the daily dose*" (which is the one you want): here the sub-atoms and trigrams look like this:
--> sub-atoms (**grapheme** equiv.): { "`<the>`", "`<`", "`d`", "`a`", "`i`", "`l`", "`y`", "`>`", "`<`", "`d`", "`o`", "`s`", "`e`", "`>`" }
--> 3-grams: { "`<the><d`", "`<da`", "`dai`","`ail`" ,"`ily`","`ly>`","`<do`","`dos`","`ose`","`se>`"}
where clearly the trigram "`<the><d`" made all the difference at search index & search result *ranking* time.

**Add this second idea:**

Ah! But what possibly already felt a bit ho-hum-this-is-great-but-doesn't-that-make-it-kind-of-a-loathed-bigram-anyway now starts to scream at me: those start/end-of-word "`<`" / "`>`" markers: the way I read them in those others, these are treated as whole characters, or to put it in our new local lingo: these markers are processed as if whole single *graphemes* themselves! 
Which, given their *localized alphabet*, is ridiculous: the *localized*, i.e. *position-determined* "*alphabet*", can be viewed as a size 3 (!): { *start-of-word*, *end-of-word*, *anything-else* }. Sure, if you look at it, face value, it *seems* it's just growing the existing language's alphabet by 2 characters, which is great for spread/specificity, but from a Markov Chain perspective, I argue that the question of what-set-do-we-have-here should bee answered by taking their context into consideration and then I'd say those *markers* are *pushing the rest of the characters out*. 
In different words: when you actually *have* a *marker* occur in your input stream, it's much less *diverse* that the usual alphabet, a one-of-3 as mentioned above (start, end, otherwise) because we are driven by the ultimate question: **how specific is this chunk that we obtain?**

And we are *reduced* significantly in our chunk's *specificity* when it has one *marker* and 2 *graphemes*, when we compare that against a same-sized chunks of *three graphemes* : for English/ASCII-restricted input with all punctuation stripped, our regular alphabet is size 26 -- *officially 36*, but the occurrence of digits together or *after* alpha-numerics is **extremely rare** so we ignore those; the numbers are bad enough in favour of our argument already -- and the number of permutations is therefore: $2 \times 26 \times 26$ (for we only have *2* markers: start-of-word and end-of-word) vs. $26 \times 26 \times 26}.

So the *idea* of adding `<` and `>` word boundary markers is cute and, as you can imagine, *might* improve the *quality* of your search results, I'm facing the additional hurdle of coming up with a search index that's potentially *performant enough to run on end-user hardware*, so *performance* remains a major concern for me. Then those markers aren't so smart as they seemed at first glance, anymore...

**The second idea**

The idea is quite obvious by now, but for arguments sake, the leading question should be phrased first: **how important do we rank our word boundary markers**?
Can we put them in a single *bit*? **Should** we weight them as a single *bit*?
Yes, and yes. *With some hand-waving.*
The precise formulation would be start/end/otherwise so that would be a 3-state value, hence about 1.4 bits worth of entropy. Or a 2-bit storage cost.
So we can either reserve additional bits in the input/construction space when we calculate our trigram hashes, *or* we could do something of similar *weight* without this added bit-fiddling complexity:
As we are already busy about using *graphemes* as sub-atomic units, we do realize that our trigram hashes will be formed from a *variable number of input bytes*.
How about we do this: we keep those markers as *characters*/*graphemes*, i.e. `<` and `>`, like the others already do, **but** we DO NOT count them as *graphemes*, but rather as *modifiers* (recall my Urdu / Hindi / Devanagari lament above?) of the *previous* or *next* grapheme, respectively? This would add about 1.4 bits of entropy to such start/end-of-word positioned trigrams and make them identifiable, both at indexing and search time.

> ~~TODO:~~ 
> analyze which side needs extra work, *if any*, to ensure we can search for edge-of-word trigrams **and** these trigram positions in the original streams will cause a hit when/*if* suitable when the user-specified search query DOES NOT give us a solid edge-of-word marker, i.e.: *can these special trigram positions be a match (or part of a larger match) when we're looking for wildcarded stuff, where edge-of-word is **not** a requisite for a match?*
> 
> Analysis: example: database has "`color theory`", "`odour and vapour`", "`colouring monochrome images`", ... and the user, aware she's looking for both US English and International English matches, fires off a search for words with "or/our" syllables, e.g. "`*o[u]?r*`. Which could also/better(?) be written as "`*(or|our)*`" query regex.
> x
> As I write the above example I realize it adds another problem: *matching `or` via the search index **at all** as the search query element is a **bigram**.* Let's file this second question for later, so we concentrate on "`our`" today:
> - "`our`" is a valid *grapheme trigram*; the "`*`" wildcard before and after indicates it MAY appear anywhere in a word, so we go two ways with this:
>   + we go for a minimal search set, thus a increased index size, as we now need to *index* every start/end-of-word trigram also as a regular one. Bad idea. That would obsolete the *attributed trigrams*, as they would uselessly grow the index -- at least when we consider an *advanced* search query like this one.
>   + we go for a minimal index set (good! less disk space! potentially faster lookup!) and expand the search query to its total potential set, where we replace thee wildcards with start/end-of-word markers, as those would match the wildcards by matching *nothing*:
>       --> expanded search regex: "`*or*|*our*|*our|our*|our`"
>       --> search trigrams: { `our`, `our>`, `<our`, `<our>`} (and `*or*`, of course)
>       
>   These would, as intended, correctly match: "_od|**our**_", "_vap|**our**_" and "_col|**our**|ing_".
>    
>   Which leaves the *bigram* search problem:
> - "`*or*`"  would *require* us indexing *bigrams* as well, *or* okay-ing a full table/content scan for such searches.
>   While I'm loath to consider such an index, let alone polluting the trigram index with these, a few half-hearted *chewing gum fixes* come to mind:
>   - keep a separate index for bigrams. 
>   - ditch slots (such as "`ph`") when their frequency of occurrence in the index is going overboard, relative to the rest of them, i.e. the top 10 or whatever in the index statistics.
>   - do not index stopwords' bigrams, i.e. *further* filter the input index stream to only index bigrams for "important/relatively rare words". 
>   - ...  
>    











Research samples / References:

-  Urdu in Devanagari: Shifting orthographic practices and Muslim identity in Delhi
- 
  