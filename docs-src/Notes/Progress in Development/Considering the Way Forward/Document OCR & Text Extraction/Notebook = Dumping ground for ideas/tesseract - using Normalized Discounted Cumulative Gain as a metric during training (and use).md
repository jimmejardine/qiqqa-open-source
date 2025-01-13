# tesseract / LSTM+CTC/Neural Networks :: using Normalized Discounted Cumulative Gain as a metric during training (and use)?

What I understand from a quick scan through NDCG literature, it is a metric that translates the neural net's output vector, which carries some kind of **ranking**, i.e. is **not** a probability but rather a **rank** value for each of the categories = items in the vector. They (a.k.a. The Internet Interpreted Through My Reading) write that:

1. **classifiers** (e.g. LSTM) do **not** output probabilities but rather produce **rank** values, which do not translate easily to *probabilities* 1:1, or in other words: one SHOULD NOT treat these numbers as probabilities, not even after "normalization" (which is, AFAIU, to sum them and scale them so the sum in 1 and '*thus*' \[sic] these scaled values now can be processed as probabilities, is *wrong* (over-simplified and incorrect use/transformation of rankings to probability *perunages*): *enter NDCG* and then, *ta.daah!*, we have a 'proper' transform of rankings to probabilities.[^1]
2. NDCG serves as a 'proper', i.e. useful, training metric for such classifiers to help determine whether these are performing well. Hmmmmm. Training tesseract has been a serious bother from day one, so *maybe* this may be able to assist a little: I was not feeling very confident yet that the current training mechanism was up to snuff. Another reason to push this one up my priority tree of *Stuff To Implement & Field-Test*!


Notes/Thoughts du Jour: my current general perception of NNs is that (almost?) all of them should be interpreted as single/multi-level *classifiers*, **ranking** the output choices, rather than spitting out probabilities for those. ('single' as in A-or-B, e.g. SVM classifiers, 'multi' as in 'output vectors' such as you get from LSTM, whether interpreted through CTC or not, etc.: each element in the vector is one element in your "*output alphabet of choices*", hence I can/do look at the codepoints that make up an LSTM's output vector as a set of categories where each codepoint ("character") is its own category. We humans read those as a time series a.k.a. "written text".

Then again, suppose I am wrong about this, would it *hurt* to use NDCG? Without having grokked it yet, gut feeling says: heck, even if the output vector *is* a bunch of probabilities, each of those is also valid as a category rank, so we are okay to feed it as a bunch of rankings into NDCG and be a-okay! Ideally we would have arrived at a 1-to-1 direct transformation then, that's all! *shrugs*...


> ### What is Normalized Discounted Cumulative Gain?
>
>Normalized Discounted Cumulative Gain (NDCG) is a ranking quality metric. It compares rankings to an ideal order where all relevant items are at the top of the list. NDCG at K is determined by dividing the Discounted Cumulative Gain (DCG) by the ideal DCG representing a perfect ranking.
> \[...]
> Normalized Discounted Cumulative Gain (NDCG) is a metric used in information retrieval to **measure the effectiveness** of search engines, recommendation systems, and other ranking algorithms. It serves as a measure of ranking quality in information retrieval and is normalized so that it is comparable across queries.
> 


### On Second Thought...

Having read a little more about it, including [Wikipedia](https://en.wikipedia.org/wiki/Discounted_cumulative_gain), it looks like this is a nice buzz term *I didn't know yet*, but is already happening in some way in tesseract. What *is* interesting, though, is whether tesseract employs those same/similar *logarithmic denominators and position-in-ranking-order-equals-gain/importance type of approach*. 

Consequently this is one more for my personal RTFC must-grok queue...

----

> ### Cons of NDCG
>
> * **Complex calculation**: The computation-intensive nature of the NDCG score calculation, particularly its normalization step, demands additional resources for processing. This complexity may also impede the evaluation process speed in large datasets or real-time systems, a factor that renders it less than ideal for applications demanding swift feedback on ranking performance.
> * **Sensitivity to rank depth**: It significantly prioritizes the top-ranked results and while this is usually desirable, at times relevant items lower down in the list may be undervalued. In scenarios where there’s a more uniform distribution of these relevant items across rankings or when identifying all relevancies trumps their positions it can lead to skewed evaluations due to this particular characteristic.
> * **Dependence on relevance judgments**: The metric’s effectiveness, NDCG in this case, hinges profoundly on the quality and granularity of relevance judgments. These can often be subjective and not to mention challenging to obtain. Such a heavy reliance implies that the accuracy of NDCG scores could bear significant influence from its initial relevance assessment process: it demands careful consideration and potentially extensive manual review to ensure an accurate reflection of user expectations or needs via relevant scores.
> 

-----

### Alternative metrics, better ones for us?

To be investigated.

There's:
- MAP (Mean Average Precision) 
- MMR (Mean Reciprocal Rank)
- ...

Quoting/interpreting from https://www.shaped.ai/blog/evaluating-recommendation-systems-map-mmr-ndcg:

> MAP and NDCG seem like they have everything \[...] — they both take all relevant items into account, and the order in which they are ranked. **However, where MRR beats them across the board is interpretability.** MRR gives an indication of the average rank of your algorithm, which can be a helpful way of communicating when a user is most likely to see their first relevant item using your algorithm. 
> Meanwhile, MAP has some interpretability characteristics in that it represents the area under the Precision-Recall curve. **NDCG is hard to interpret because of the seemingly arbitrary log discount within the DCG equation.** The advantage of NDCG is that it can be extended to use-cases when numerical *relevancy* is provided for each item, but it may be hard or even impossible to obtain this 'ground-truth' relevance score in practice, so you don't see it used as much in practice.

... and there goes my initial euphoria tonight! Still, all this leaves me with the strengthened impression that those CTC output vectors' (the codepoint-in-OCR-model-alphabet) rakings' processing is an area to be researched and tweaking those numbers, as I did a while back, is par for the course. 
*sigh* 
Yep, same ol', some ol': throw any sufficiently large load of more-or-less 'founded in solid theory'[^2] statistics at the problem and ultimately you end up with... the numeric equivalent of The Wet Finger wind-tasting approach, which one only can **hope** is better and more discriminating (as in: identifying the actual character written there) that your favorite [ouijaboard](https://en.wikipedia.org/wiki/Ouija).


-------


> **The NDCG is a ranking metric.** Imagine that you predicted a sorted list of 1000 documents and there are 100 relevant documents, the NDCG equals 1 is reached when the 100 relevant docs have the 100 highest ranks in the list.
> 
> So 0.8 NDCG is 80% of the best ranking.
> 
> This is an intuitive explanation of why the math includes some logarithms.
> 


See also: [A Theoretical Analysis of NDCG Type Ranking Measures (2013), Yining Wang, Li-Wei Wang, Yuanzhi Li, Di He, Tie-Yan Liu, Wei Chen](https://arxiv.org/abs/1304.6480):

> A central problem in ranking is to design a ranking measure for evaluation of ranking functions. In this paper we study, from a theoretical perspective, the widely used Normalized Discounted Cumulative Gain (NDCG)-type ranking measures.
> 
> Although there are extensive empirical studies of NDCG, little is known about its theoretical properties. We first show that, whatever the ranking function is, the standard NDCG, which adopts a logarithmic discount, converges to 1 as the number of items to rank goes to infinity. This result is surprising at first sight: it seems to imply that NDCG cannot differentiate good and bad ranking functions, contradicting to the empirical success of NDCG in many applications.
> In order to have a deeper understanding of ranking measures in general, we propose a notion referred to as *consistent distinguishability*. This notion captures the intuition that any ranking measure should have such a property: for every pair of substantially different ranking functions, the ranking measure can decide which one is better in a consistent manner on almost all datasets. We show that NDCG with logarithmic discount has *consistent distinguishability* although it converges to the same limit for all ranking functions.
> 
> We next characterize the set of all feasible discount functions for NDCG according to the concept of consistent distinguishability. Specifically *we show that whether NDCG has consistent distinguishability depends on how fast the discount decays*, and 1/r is a critical point. We then turn to the cut-off version of NDCG, i.e. NDCG@k. We analyze the distinguishability of NDCG@k for various choices of *k* and the discount functions. Experimental results on real Web search datasets agree well with the theory.
> 






[^1]: which, incidentally, made it sound like the current tesseract OCR output vector treatment (the LSTM+CTC output) is indeed **not** to be treated as a yet-to-scaled collection of probabilities; I already felt that way, but this might just be me being biased and reading confirmation into that pre-existing bias of mine. Anyhow, I haven't seed any mention of NDCG in the tesseract code yet, so "*to be investigated further, most assuredly!*"

[^2]: of course, using those statistics' algorithms does serve extremely well the purpose of 'splaining why your system is better than any other. I'm getting cynical at my advancing age. ... Oh, *wait*! Damn! I was born this way!