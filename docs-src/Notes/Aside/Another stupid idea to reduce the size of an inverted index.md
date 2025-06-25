# Another (possibly stupid?) idea to reduce the number of references in a unigram/bigram/trigram inverted index

Assume we have an "inverted index" lookup table which translates n-grams into *original words*. That's something we'd use for FTS (Full Text Search).

Speed is of the essence here, so anything that can make this baby faster, both for single lookups and bulk queries, is beneficial to the final UX.

Suppose we store *n-grams* for varying sizes of `n` -- we intend to do so anyway, using a similar-to-what-github-did *sparse n-gram*-ming approach, which is still under research as we need to tune ours for our purpose, which is *not* source code lookups.
The naïve approach here is to have every *n-gram* record reference the *original word* it was derived from when the index entry was generated.

For tri-grams and *n-grams* where $n > 3$ we assume the number of *original word* references `w` to be *reasonable*: close to 1. 
For bigrams ($n = 2$) this list of original words becomes quite large and for unigrams ($n = 1$) tat list would be *prohibitively large* as is would be about $\frac{1}{26}$ of our dictionary size (total original word count).

The idea is simple: keep those original word reference lists as small as possible. To do this, we have the unigrams reference the *original word bigrams* which incorporated that particular unigram: that way, we can expect a $2 \times 26$ size: 26 bigrams for when the unigram is for the *prefix* of the bigram, plus another 26 for when the unigram represents the *suffix* of the bigram.
Ditto for the bigrams: they should reference the related *trigrams*: another suffix + prefix permutation hence a worst-case size of $2 \times 26$ again for English language words.

The *original word set* lookup would then become multi-stage: expand unigram to bigram set, expand that to trigrams and expand those to original words. (As this would be an explosive lookup either way, we better make sure we never need it that way, but that's not the point here: the point is that a previously *single lookup* now will require up to *four* lookup queries to get us to the original word we're interested in, iff we started from a unigram. 
Let's just hope we can severely restrict that expansion along the way, using other criteria. 
Meanwhile, this idea reduces the amount of storage space required per record in the inverted index, so I want to test it to see if it's going to be useful.



