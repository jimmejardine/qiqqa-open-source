# tesseract preprocessing :: discovering an optimal preprocess

Preprocessing page images for (tesseract) OCR is a process that's unsolved and will stay unsolved, for there's always yet another image that doesn't react well to any treatment X that you throw at it.

## The idea...

I had the idea that using a scripting language for this part would be useful: that way users can mess around and fiddle with the various available preprocessing steps and their parameters and thus proceed towards a preprocess that's more suitable to their particular input.

Using a DSL (Domain Specific Language) for this is a good idea and has been very useful in the past. While I picked QuickJS as the preferred engine for this (thus JavaScript as a base for the DSL), my inner geek was bikeshedding about JS, TCL, LISP. Which led me to JANET as an alternative, because on closer inspection, I didn't like the way TCL does variables: the `$` seems unnecessary and I'd rather write `awk` instead, *if ya dig*.

Anyway, the bikeshedding / procrastination there is also due to the fact that deep down I had immediately realized that offering a scripting solution wouldn't actually *solve* the issue of producing improved page images. Nor would any regular photoshop-like GUI, which is why I looked at scantailor and friends and didn't get any arousal out of it: same problem, different approach (clickity-click instead of typiddi-type), same trouble, not moving forward any.

## The next idea...

While I don't want the burden of going Deep AI with this - primarily it would guzzle my time and financial means like crazy: training a model is not cheap, au contraire, and since I am not extremely well versed in that particular niche either, I must reckon with the learning curve. More cost, less prospect of success.

After having looked at non-related material and stumbling across the now-sorely-out-of-hipness Kohonen maps (SOMs), which I happen to like a lot but always struggle a bit with the 2D output mapping to make them work well as a visual aid, I got tripped up by the root  of the GA (Genetic Algorithms) tree: another part of CS that's had its day and is now severely uncool if the amount of work happening there is any indication.
But this whole thing stuck to me like [catchweed](https://en.wikipedia.org/wiki/Galium_aparine) to cloth (Dutch: *kleefkruid*): what if we could use GA or other "unsupervised" means to find us an improved preprocess for image X? But then what would the 'gene' represent? It's all nice and dandy to brag in literature about running a GA with 10K parameters, but I'm looking for a limited number of features/parameters/phenotypes/whatever-you-wanna-call-it as this should be a relatively fast and hopefully *human reviewable* process...

So the idea is this: the gene represents the sequence of image operations to be executed during preprocessing: scaling, contrast mapping, greyscale conversion, binarization, noise reduction, sharpening, etc. where the gene can have variable length and each element in the gene codes for a specific function/algorithm to use.
If this works, we can then enhance the concept by including parameterization of these algorithms/functions as part of the 'gene' and using some sort of Monte Carlo process to test several settings. This is not exactly *pure* GA, but it's using some of those ideas, while now the problem shifts to the question: 
Since we want this to be an "unsupervised" process, hence no human in the loop, how do we teach the machine to evaluate the test results and rank them, i.e. what do we use for a output quality metric?

We can't know the "ground truth" as it is intended as an *unsupervised* approach, so we need to come up with a metric that approximates ground truth. We cannot just use the OCR engine's reported confidence and turn that into a KPI as the OCR engine can be quite confident while utterly mistaken some times, but more importantly: when the quality is *meh*, the OCR engine spits out all sorts of confidence values and I fear it won't be much different from *noise*, so a big no-no as a decision-making metric.
As we won't always be OCRing text books, applying a dictionary search + match score is also a bit naive, so then the thought becomes: we did Markov chain analysis once for language detection (using 2-grams); how about we do that again to rank text output as sensible/crazy? That way we can feed the Markov-chain based n-gram ranker *previous* ground truth to have it develop its own kind of n-gram based ground truth table and then we can apply stuff like TF-IDF to score actual option against such a *prior* to obtain a (hopefully) useful KPI to drive the GA/MCMC towards a potentially optimal preprocessing solution.

Suppose this works, then the final 'gene' can be stored with the document page for later reproduction; the 'gene' encodes the preprocessing 'script' that must be executed to produce the discovered 'best result'. 
When processing a book, this costly search can be done for a few sample pages and then that same 'gene' can be applied to all pages alike. If the user reviews the output and finds some pages lacking in quality, *hir* should be able to rerun the GA search for these particular pages, store the discovered optimal genes for each page and if the user wants to try their own hand at it, they can take the gene and manually tweak it to change the preprocessing pipeline to their taste.
Which we could call *gene programming*.

Anyway, that's the second idea and currently if feels like I might be able to pull this off...



