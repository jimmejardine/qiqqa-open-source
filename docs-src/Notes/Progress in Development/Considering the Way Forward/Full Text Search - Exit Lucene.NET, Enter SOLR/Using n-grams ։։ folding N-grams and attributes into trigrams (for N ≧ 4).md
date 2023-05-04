# Using *n-grams* ։։ folding N-grams and attributes into *trigrams* (for N ≧ 4)

This idea was triggered after reading [The technology behind GitHub’s new code search | The GitHub Blog](https://github.blog/2023-02-06-the-technology-behind-githubs-new-code-search/). Quoting some relevant parts from there:

> The n-gram indices we use are especially interesting. While trigrams are a known sweet spot in the design space (as [Russ Cox and others](https://swtch.com/~rsc/regexp/regexp4.html) have noted: bigrams aren’t selective enough and quadgrams take up too much space), they cause some problems at our scale.
>
> For common grams like `for` trigrams aren’t selective enough. We get way too many false positives and that means slow queries. An example of a false positive is something like finding a document that has each individual trigram, but not next to each other. You can’t tell until you fetch the content for that document and double check at which point you’ve done a lot of work that has to be discarded. We tried a number of strategies to fix this like adding follow masks, which use bitmasks for the character following the trigram (basically halfway to quad grams), but they saturate too quickly to be useful.
>
> We call the solution “*sparse grams*,” and it works like this. Assume you have some function that given a bigram gives a weight. As an example, consider the string `chester`. We give each bigram a weight: 9 for “ch”, 6 for “he”, 3 for “es”, and so on.
> 
> ```
>    9  6  3  1  2  0  7
> +------------------------------------------
> | c | h | e | s | t | e | r | ... |  
> +------------------------------------------
> 
> --> Intervals
>  [9,7] = "chester " 
>  [9,6] = "che"
>  [6,7] = "hester "
>  [6,3] = "hes"
>  [3,7] = "ester "
>  [3,2] = "este"
>  [3,1] = "est"
>  [2,7] = "ter "
>  [2,0] = "ter"
>  [0,7] = "er "
>  ```
> 
> Using those weights, we tokenize by selecting intervals where the inner weights are strictly smaller than the weights at the borders. The inclusive characters of that interval make up the n-gram and we apply this algorithm recursively until its natural end at trigrams. At query time, we use the exact same algorithm, but keep only the covering n-grams, as the others are redundant.

While the weights shown in the quoted article's diagram don't make sense (at least the 0-weight I'ld have expected to be something like 4 or 5, or the sequence start with 9,3,6 instead of 9,6,3) the trigger for me was "*like adding follow masks, which use bitmasks for the character following the trigram (basically halfway to quad grams), but they saturate too quickly to be useful*": how about we *encode attributes* in a trigram, eh?

Okay, this comes in three parts:

1. what I'ld expect those weights to be, really (1,2,0,7 feels like bullshit)
2. idea A: folding N-grams into trigrams, *not* using a bitmask but a hash?
3. idea B: ditto, but now encoding / "*folding*" *attributes* into the trigrams, e.g. *case-sensitivity*.

## 1. Weighting "sparse grams"

What would make more sense, at least to me, is using a weighting algo where you take a full word to weight (the blog article is about code search, so there's a clear 'word separation' rule for every programming language you'll be ingesting; nothing bothersome like Mandarin, where word boundaries are basically *lost*), and then:
- determine the middle of the word
- assign increasing weights radiating out from that position.
- when a word has an even number of code points, pick left or right as 'mid point', but do so consistently, and weight them as if there is an *invisible* midpoint in between.
Hence `"chester "`  (*note their inclusion of the trailing whitespace, but not a leading one -- we'll stick with that for now*) would then have 'midpoint' `"st"`  (thanks to that trailing whitespace being accounted for) and weights 2,1. 
*Radiating out* this would give the weight sequence:  6,4,2,1,3,5,7.

Okay, now redo this with word delimiters like I've seen described for regular human languages' word tokenizers: `"<chester>"`, where "<>" are arbitrarily chosen SOW (Start Of Word) and EOW (End Of Word) markers. Of course, when you intend to parse, index and search *program source code*, those "<>" would be a *particularly* bad choice as for *programming languages* those 'punctuation characters' are rather actual keywords themselves (*operators*), so you might want to pick something way off in the Unicode range where you are sure nobody will be bothering you by feeding you content that carries those actual delimiter code-points you just picked as part of their incoming token stream.

So let's pick something cute for our example:   🙞  🙜 

Then the word "chester" would be written as "🙞 chester 🙜". The midpoint now is "s" and the bigram weights would start at 1 for "st", radiating out as 8,6,4,2,1,3,5,7: "🙞c" = 8, "ch" = 6, "he" = 4, "es" = 2, "st" = 1 (midpoint bigram), "te" = 3, "er" = 5, "r🙜" = 7.
Then store the max weight with each trigram.

When tokenizing any *query*, the same process would produce either the same weight trigrams (when the precise, *entire* word is part of the query), or *lower weight* trigrams (when only part of the word is specified in the query, e.g. "hest"): only trigrams with same or higher weight in the index would then *match* the query.

> Odd-length words would be weighted as if they were as large as the next even-length word, with one or more trigrams less. E.g. "chesty" would, at index time, be weighted as follows:
> 
> "🙞 chesty 🙜": midpoint is "es", hence we pick "s" as midpoint (delivering higher weights to the first half of the word, as seen before with "chester").
> 
> Weights now start at **2** because we assume a missing midpoint character (at bigram weight 1), so we would now start at weight = 2: 
> 
> 8,6,4,2,3,5,7: "🙞c" = 8, "ch" = 6, "he" = 4, "es" = 2 (midpoint bigram), "st" = 3 , "ty" = 5, "y🙜" = 7.
>  

### Considering words of varying sizes

Quoting:

> For common grams like `for` trigrams aren’t selective enough. We get way too many false positives and that means slow queries. An example of a false positive is something like finding a document that has each individual trigram, but not next to each other.

Here the thought is about the *keyword* "`for`" and "for" as part of another word, e.g. variable in the indexed source code input: "beaufort": as the keyword is a lot shorter, its trigram weights would thus also *match* the variable "for" particle, given the weighting scheme above.

So the conclusion is **we should weight all incoming words about equally** in this scheme: still we decrease the weight while moving inwards while producing ngrams from a given input word, but we start at a fixed weight value, say 100. Then `for` will be indexed as

> Notes: 
> - we'll now be using trigrams as a base instead of bigrams
> - the midpoint code point is consequently also the middle of the innermost trigram
> - weighting still starts at the middle, but now always at weight 0, radiating out towards the end first, i.e. the trigram left from the middle gets weight 2, while the one to the right gets weight 1.
> - Once the weights are assigned as before, we recalculate the weights from the word weight value 100 down. (Horribly large words would **stop** at weight = 0, or should we allow *negative weights*? We'd be talking real edge cases there anyway, so might be okay to keep it all *non-zero positive*?)

> 2,0,1: "🙞fo" = 2, "for" = 0 (midpoint trigram), "or🙜" = 1 
>  ⇒
> **100,98,99: "🙞fo" = 100, "for" = 98, "or🙜" = 99**  

Ditto for the query "for" (assuming the user didn't add wildcards making this *part-of-a-word* "for", in which case we would use the lowest weights for the query "for" instead as it **should** match any part of a larger word in the index!)

Meanwhile "beaufort" would be indexed as:

> 8,6,4,2,0,1,3,5: "🙞be" = 8, "bea" = 6, "eau" = 4, "auf" = 2, "ufo" = 0 (midpoint trigram), "for" = 1 , "ort" = 3, "rt🙜" = 5
> x
> This makes the weight series a little skewed, so we *could* consider radiating out the other way around for even-length words instead:
> x
> 7,5,3,1,0,2,4,6: "🙞be" = 7, "bea" = 5, "eau" = 3, "auf" = 1, "ufo" = 0 (midpoint trigram), "for" = 2 , "ort" = 4, "rt🙜" = 6
> x
> where the front of the word still gets the higher initial weight overall.
>  ⇒
> **100,98,96,94,92,93,95,97,99: "🙞be" = 100, "bea" = 98, "eau" = 96, "auf" = 94, "ufo" = 93, "for" = 95 , "ort" = 97, "rt🙜" = 99**  


#### Open question: where's the gain for this scheme?

The gain, as the blog mentioned above, is when *querying* using words, e.g. "fort": when that one is identified as a whole word, its trigrams would weight as:

> 3,1,0,2: "🙞fo" = 3, "for" = 1, "ort" = 0 (midpoint trigram), "rt🙜" = 2
> *or starting at 1 instead of 0, so our weighting scheme remains the same for odd- and even-lengths:*
> 4,2,1,3: "🙞fo" = 4, "for" = 2, "ort" = 1 (midpoint trigram), "rt🙜" = 3
>  ⇒
> **100,98,97,99: "🙞fo" = 100, "for" = 98, "ort" = 97, "rt🙜" = 99**  

where searching for trigram "for" **would not** return a hit for the "for" trigram of indexed word "beaufort" as its weight is too low: we should only match same weight or higher in the index!

Of course, this implies that regexes and wildcards in the search query will need to be parsed and impact the weight assignment at search time, e.g. a search for "for*" (wildcard '\*' for any number of code points) would give a *search* trigram sequence of:

> "🙞 for \* 🙜"
>  ⇒ (wildcards cannot be part of any trigram: discard)
> "🙞 for🙜"
>  ⇒
>  2,0,1: "🙞fo" = 2, "for" = 0 (midpoint trigram), "or🙜" = 1 
>  ⇒
> **100,98,99: "🙞fo" = 100, "for" = 98, "or🙜" = 99**  
 
which' "for" trigram (at weight 98) would only match the "for" (at weight 98) for the indexed word "for": quite appropriate! Meanwhile, the trigram "for" (at weight 95) for indexed word "beaufort" is conveniently ignored.

Stuff gets rougher for no-prefix wildcard/regex searches, e.g. a search for "\*for" would result in these search weights:

> "🙞 \* for 🙜"
>  ⇒ (wildcards cannot be part of any trigram: discard)
> "🙞 for🙜"
>  ⇒
>  2,0,1: "🙞fo" = 2, "for" = 0 (midpoint trigram), "or🙜" = 1 
>  ⇒
> **100,98,99: "🙞fo" = 100, "for" = 98, "or🙜" = 99**  

where "for" @ 98 would match indexed trigram "for" @ 98 for the indexes word "fort", which does not match the wildcard search. However, "beaufort"'s "for" @ 95 is still conveniently skipped.

It gets hairy where the partial is *surrounded by wildcards*, e.g. in a search for "\*for\*" (which is also pretty bad if you would have a regular index, by the way): due to the '\*' you cannot say anything useful about the length of the word this part is supposed to bee part *of*, so we have to assume the longest word possible, resulting in these weights:

> "🙞 \* for \* *🙜"
>  ⇒ (wildcards cannot be part of any trigram: discard, but be weary and mark the surrounding of the partial!)
> "🙞 for🙜"
>  ⇒
>  2,0,1: "🙞fo" = 2, "for" = 0 (midpoint trigram), "or🙜" = 1 
>  ⇒ (must assume longest possible word, so lowest rank will be 1 if we stick with the "only positive weights" bit as pondered earlier -- by clipping the weight decrease from 100 down at weight = 1)
> **100,98,99: "🙞fo" = 3, "for" = 1, "or🙜" = 2**  

which will (correctly, I must say!) match all indexed "for" trigrams: "for"@98 for indexed word "for" (correct!), "for"@98 for "fort" (another match!) and "for"@95 for "beaufort" (yet another query match!)

Here (as one would have intuitively expected otherwise as well) the only saving grace is the use of trigrams themselves to reduce the search space for surround-wildcarded queries such as "\*for\*": any indexed word that DID NOT produce a "for" trigram as part of its indexing process WILL NOT feature in the initial hit/match list when searching.

**However** do note that we glossed over a potential **bug** above: we 'discarded' the wildcards while tokenizing to showcase the correct weight assignments procedure, but trigram "🙞fo" (and any other trigram spanning a search wildcard of any kind) CANNOT be part of a search 'AND' expression: we either *discard* those trigrams or add them in a (superfluous) 'OR' search expression -- superfluous because any match for the wildcarded search will have to match the inner "for" partial anyway, so searching for `("🙞fo" OR "for" OR "or🙜")` takes longer (3 scans) while the only important part is matching the given partial, i.e. `("for")`, which only takes 1 scan. Hence it's safe to discard wildcard-spanning trigrams, while we SHOULD account for their 'position' as part of the weight assignment process, as shown above. Because suppose we hadn't done that, than our lonely "for" search trigram would clock in at weight 100 and nothing would have matched against it. Of course, the simple approach is discard the wildcard trigrams and **lower the start weight to 98**: this simplified approach would then match slightly *too many* indexed trigrams, which will have to be processed by the post filter. Consider the "\*for" and "for\*" searches for this: there the start weight would be dropped to 98 too:

> "🙞 for \* 🙜"
>  ⇒ (wildcards cannot be part of any trigram: discard v2 + drop by 2)
> "🙞 for"
>  ⇒ (even length: start at 1)
>  2,1: "🙞fo" = 2, "for" = 1 (midpoint trigram) 
>  ⇒
> **98,97: "🙞fo" = 98, "for" = 97**  
 
Compare that with the example further above where we produced weight 98 for the same "for" search trigram, hence early and total discard plus drop weight by 2 will slightly overmatch during search. And dropping by 1 only is not an option, as that would push weights the other way, resulting in incorrectly missing matches for "\*for\*"; 🤔 *perhaps drop weight by 1 for every wildcard that's discarded then?* 🤔

> "🙞 \* for 🙜"
>  ⇒ (wildcards cannot be part of any trigram: discard v2 + drop by 2)
> "for🙜"
>  ⇒ (even length: start at 1)
>  2,1: "for" = 98, "or🙜" = 97 
>  ⇒
> **98,97: "for" = 98, "or🙜" = 97**  

where (probably much less harmful) trigram "or🙜" drops to 97, rather than its previous 99 (see earlier example for the same wildcarded query). 🤔 Nah, this is as detrimental as the other example: imagine we're processing larger words, than this means all left-half trigrams drop by 2 in weight vs. the previous discard-late-in-the-game approach, resulting in *way* more chances to get a hit in the index for those trigrams, that will ultimately not pan out as we have a known word edge boundary here at right side, so those trigrams have important weights and should be rated from 99 on down no matter what!

Hence the conclusion: the simple "*discard all wildcard spanning trigrams early and drop weight by two*" approach is not a smart idea, for it will always deliver an oversized hit collection.

🤔 I think it's thus 'optimal' to replace every wildcard with its minimum-length equivalent (nil for `*` and regex `?`, for example, but 1 code point for regex `+` or `.`) before trigramming and weighting the trigrams. Then only once that has been done should we discard the wildcard-spanning trigrams.
Which leaves the question about that *surrounded*  partial query and the open question whether there are more wildcard / regex search expressions imaginable which drop the weight to bottom 1, thus not adhering the party line of '100-and-decreasing' weighting? 

e.g. `"be*for*"` query: that one has two worth-while trigrams: "🙞be" = 100, "for" = **1** *(!)* where "for" is only useful in-so-far as to reduce the hit set as produced by "🙞be".


 















## 2. Idea A: folding N-grams into trigrams

As the blog article says:

"*We tried a number of strategies to fix this like adding follow masks, which use bitmasks for the character following the trigram (basically halfway to quad grams), but they saturate too quickly to be useful.*"

How about taking arbitrary length n-grams (n > 3), next to the trigrams themselves, and then "folding" the n-grams into a trigrams-like space?

Here the idea is simple: each trigram is a hash derived from 3 code points, each of which require a basic `int32_t`  (as an `uint16_t` does not cover the entire Unicode range) and we can either define the *third code point* as having a codepoint *value* **above** the Unicode range, but still within the `int32_t` space, so as to produce 'code points' which Unicode will never assign nor define. And then we take that combo as if it were a regular trigram input and hash it accordingly. This would only increase the total range by a small factor.
Or we could do almost the same but *stay Unicode-legal* by defining the *constructed third codepoint* as MUST-reside-in the Unicode Private Use Area space: that's still 128K of range so currently comparable to the other approach here.

To be more precise:

### Approach A

- take any word (codepoint input) of length  n > 3 and *hash its 3rd-plus characters*.
- map the given hash into single-codepoint-space **but shifted**: Unicode has roughly a $2^{20}$ codepoint value range. We use the same for our *masked hash* so as to mimic the cost of a single codepoint in any trigram.
- this now constitutes a complete trigram input. Process (hash) it as usual.

In pseudo-code:

```
let word = 'abcdefgh'
let ngram_hash = hash(word[2..])   // hash 'cdefgh'
// now make it a single codepoint worth of range:
let codepoint = ngram_hash & 0x001FFFFF  
// SHIFT the constructed codepoint so it's an
// otherwise *impossible* codepoint value:
codepoint |= 0x00200000
// inject the constructed codepoint into the given word:
word[2] = codepoint
// make it a 'real' trigram:
word.length = 3
// process as usual
let trigram_hash = hash(word[0..2])
```



### Approach B

- take any word (codepoint input) of length  n > 3 and *hash its 3rd-plus characters*.
- map the given hash into Private Use Area space, thus making it a *legal* Unicode codepoint. This is done so we equal the cost of a single codepoint in any trigram.
- this now constitutes a complete trigram input. Process (hash) it as usual.

In pseudo-code:

```
let word = 'abcdefgh'
let ngram_hash = hash(word[2..])   // hash 'cdefgh'
// now make it a single codepoint worth of range:
let codepoint = ngram_hash & 0x001FFFF  
// SHIFT the constructed codepoint so it's in
// a suitable Private Use Area:
if (codepoint < 0xFFFE)
  codepoint += 0xF0000   // move into Unicode plane 15
else
  codepoint -= 0xFFFE
  if (codepoint < 0xFFFE)
    codepoint += 0x100000 // move into Unicode plane 16
  else
    codepoint -= 0xFFFE
    codepoint += 0xE000   // move into Unicode Basic Multilingual Plane
  endif
endif
// codepoint is now a legal Unicode codepoint.  
// inject the constructed codepoint into the given word:
word[2] = codepoint
// make it a 'real' trigram:
word.length = 3
// process as usual
let trigram_hash = hash(word[0..2])
```

where the shifting into the Private Use Area spaces can be accomplished in various ways. Here's other approaches for that part:

```
// SHIFT the constructed codepoint so it's in
// a suitable Private Use Area:
let char = codepoint + 0xF0000   // into planes 15+16
let checkmask = (codepoint & 0x1FFF0) ^ 0x1FFF0;
if (checkmask == 0)
  // out-of-bounds in plane 15 or 16: move these into
  // the Basic Plane instead:
  char = 0x0E000 + (codepoint >> (16 - 4) | (codepoint & 0x0000F))
endif
codepoint = char
// codepoint is now a legal Unicode codepoint.
```


```
// SHIFT the constructed codepoint so it's in
// a suitable Private Use Area:
codepoint += 0xF0000   // into planes 15+16
// check codepoint legality:
if (codepoint > 0xFFFFD && codepoint < 0x100000)
  codepoint &= 0x0000F
  codepoint += 0x0E000
elif (codepoint > 0x10FFFD && codepoint < 0x110000)
  codepoint &= 0x0000F
  codepoint += 0x0E010
endif
// codepoint is now a legal Unicode codepoint.  
```




## 3. Idea B: encoding / "*folding*" *attributes* into trigrams

Same as the previous idea, but now targeting other information: **attributes**.

Say we intend to support *case-INsensitive* searches, next to regular *case-sensitive* (a.k.a. *exact*) searches. Then we can either query the index with every combination of upper- and lowercase of our input, or we can load the index at indexing time with every combination of, etc. Either way, the cost is pretty steep, possibly in storage and surely in processing time.

Another approach seen elsewhere is to index all material *cased*, e.g. lowercased and then cope with *exact queries* by post-filtering the search results against the original data to remove the wrong-case preliminary matches. Another costly approach, if slightly less so.

Here the idea is take those approaches and *combine them* into a more optimal storage cost and search effort: the idea isn't far removed from keeping two dedicated indexes: one for exact searches, plus one for case-insensitive searches. But now using a single index by tweaking our trigrams: we encode the 'is-this-case-exact-or-lowercased' attribute bit by injecting it into the trigrams themselves, slightly increasing the codespace so we can store both unique trigrams in the same index and search for them when we want to.



To be more precise:

- take the lowercased-or-exact attribute bit.
- use it to shift the first codepoint in the trigram to end up *above* the regular Unicode range: regular Unicode has an about $2^{20}$ codepoint value range, now the first codepoint only has an effective $2^{21}$ codepoint value range.
- this now constitutes a complete trigram input. Process (hash) it as usual.

In pseudo-code:

```
let word = 'abc'
let lowercased_attrib = {true | false}
// shift first codepoint accordingly:
if (lowercased_attrib)
  word[0] |= 0x200000  
// process as usual
let trigram_hash = hash(word[0..2])
```

We do the same at query/search time, so case-sensitive and case-insensitive searches can execute in parallel if need be (e.g. when resolving compound queries); each search will hit the corresponding trigrams in the index without the need to post-filter against original data.

A small optimization might be: if the *exact* trigram input is *unchanged* when lowercased, then only store the *exact* trigram in the index: we will be able to discover the same at query time, so we can then search for the *exact* trigram without any problem. Ergo:

```
let exact_word = 'Abc'
let lowercased_word = to_lowercase(exact_word)

let trigram_hash = hash(word[0..2])
process(trigram_hash)

if (lowercased_word != exact_word)
  let lowercased_attrib = true
  let lowered_trigram_hash = hash(word[0..2])
  process(lowered_trigram_hash)
endif
```



References / See also:

- [Private Use Areas - Wikipedia](https://en.wikipedia.org/wiki/Private_Use_Areas)
- [Unicode - Wikipedia](https://en.wikipedia.org/wiki/Unicode)
- [Unicode Utilities: Description and Index](https://util.unicode.org/UnicodeJsps/)
- [The technology behind GitHub’s new code search | The GitHub Blog](https://github.blog/2023-02-06-the-technology-behind-githubs-new-code-search/)
- [Regular Expression Matching with a Trigram Index (swtch.com)](https://swtch.com/~rsc/regexp/regexp4.html)
 
