# Tesseract aspects to keep in mind

(Have written this for a response in the tesseract mailing list. Important bits to keep in mind and suitable for (1) technical documentation here and (2) grounds for the work done on augmenting the MuPDF + tesseract combo with maybe some OpenCV and a few helpful OCR algos, controlled through some form of *user controlled script*.  (*user controlled* as it might be GUI-driven, one day. One can hope. :-) )

> Also note: 
> - [[scripting the OCR + text extraction process]]

---

## Three aspects *(or was it four?)*

AFAICT there's three aspects that generally impact OCR output quality, two of which will apply here for sure (in some way, at least), while you may run into the third when you got 1+2 covered:

1. TABLES: page images which contain any kind of *tabular data* are best preprocessed to extract the individual "table cell images" and feed each to tesseract individually, then recombine the outputs to become your page's OCR result.
2. INVERTED COLORS: tesseract has a hard time to OCR anything that has white text on black background. The training sets are engineered from and for "the usual", i.e. black print on white paper.
3. TEXT TOUCHING THE EDGE: tesseract output quality is negatively impacted when there's no surroundings, i.e. no "border".

### Corollary:

1. "tables" are not to be taken at their *literal*, *strict* word definition. When you have multiple columns of text (think: newspapers, multi-column press-ready print layouts of papers, various wide print art layouts, etc.) that's "table" for you and the individual vertical slices (segments) should be extracted and fed to tess individually.  When you need a rule for this, consider this one: when you would take a knife and slice the page into thin horizontal strips, sized one line of print each, does that strip contain words that do belong to different parts of the text? If yes, find ways to identify the continuous vertical chunks of text and extract those. (Another case of "tables" is a wide margin print layout with so-called liner notes, where comments /annotations are printed in the wide margin. Some scholarly bible analysis publications come to mind as an example of this. Less seen on the web, despite us all having widescreens ideally suited for a layout style like that. :-/)

   Given your sample image, that's 5 "cells" I see: top chunk, bottom chunk and three columns, one "cell" each.

2. tess is trained on black print on white background. An important bit of image processing inside tesseract before actual OCR-ing takes place is "thresholding" (a.k.a. "binarization") of the image. That means *anything* that's not pure black and white is made so. (google those terms if you're not aware of them yet). One of the problems with "binarization" is finding a suitable cut-off point: the criterion which decides for the algorithm which pixel is going to become *white* and which one will become *black*. 

   The third (far right) "middle cell" is pure white on black, thus fatally flawed. You can help tesseract enormously by *inverting* that bit of the input image. (Observe this is aspect 2 for a reason: if you'ld apply that inversion to the entire image, you would go "from drip into the rain" (Dutch idiom) as the rest of the page would become black. ;-)  Hence there's real benefit of slicing the page into "cells" first! (I'm consciously trying to NOT write "sections" or "segments" here as "cells" is, at least for me, far better at describing the mental model required here.)

   Search this mailing list (and the internet at large) for various approaches that would help you there; try a few and pick the one you like.

   Then we get to the central "cell", which is kind-of-okay, because the binarization algorithm used by tesseract will do the right thing for this one (or so I *assume*), but generally it is highly beneficial to identify *any* "low contrast" images and preprocess them yourself, possibly even doing the binarization yourself as part of that preprocess (not for reasons of efficiency, but it'll help you see what tesseract will be fed most precisely and that always helps diagnosing any remaining problems, e.g. observing flecking due to particular noise characteristics in the source image or part of the text area being blacked-out entirely due to vignetting or similar copier/scanner/camera artifacts, which might thwart the default binarization algorithm used by tesseract. (google to find plenty research publications on the subject, including enlightening sample images for the various images the researchers tested their own algo on.) solution space: OpenCV and beyond.

   TL;DR: in case of doubt, make sure your preprocess produces high contrast back&white images. Old scans, faxes, or anything "irregular" like that may need a choice of sophisticated binarization applied as well before entering tesseract. There's no magic bullet here: "odd stuff" needs manual tweaking. (Likewise when feeding such inputs to commercial OCR packages, I must say. Some have included advanced heuristics, but nothing out there can decode *everything* without twiddling a knob, at least.)

   > Aside: for those of us processing color images: RGB is not the best way to think about those. You will be surprised often; reading up on color theory will help understanding there. In RGB colorspace, which is computers' default, bright red (R=255) is often much "darker" than, say, bright green (G=255) or bright blue (B=255): feeding this into any thresholding algorithm might get very odd results and thus very bad OCR output quality. Process in another colorspace can help a lot. Anyway, another reason why I'ld advise you to do your own thresholding, at least when testing and evaluating your image preprocess.   Color is not relevant in this thread here, though.

3. As can be observed in several (older) messages in this mailing list, once you have your "cell" images ready, tesseract can still produce mediocre/crap output if the black text happens to *touch the edge* of the image. The solution here is simple: always add a white border and retry. Try *both* ways as you may have some *printed borders*, bleeding or scan-from-photocopier-without-the-lid-down images there and blatant application of "add white border" would make that noisy edge area from the photocopier suddenly look like some very suspicious... text?  Hence "try both ways", before you decide which works best for your images.  

   Add the white border *after* thresholding: if you don't, you just created another problem for yourself as tesseract could see the transition from, say, /grey/ background to new /white/ border as yet another *edge* in the image to consider. We don't want that. The added border should not be visibly obvious that way, but merely a smooth continuation of the background already present: the central "Cell" in your sample image therefor MUST be "binarized" *before* you add an extra white border.

   As I mentioned at top, this "touching the edge" might be an issue you encounter once you've extracted those three "Cells" in the middle of your page. Not very likely, but since I'm writing this as a general text, I consider it *probable enough* to add this one.



### The *Fourth*

Oh! I forgot a fourth aspect, which I haven't seen lately while lurking this mailing list: text line height. Not gonna be a problem for you, I expect, but for those landing here later maybe: very tiny *and* very huge texts get the system a tad confused. 200dpi and "regular paragraph print" lettering would be best. (think: scanning your text pages at 1200dpi/2400dpi "to make sure": that's *not* gonna help, really.)  (search mailing list for discussion of this when needed; not going to mention x-height px numbers here, because right now I'm wondering if the number my brain popped up is correct, really... ;-) )


## [HTH](https://acronyms.thefreedictionary.com/HTH)

BTW, here's a couple of relevant links re aspect 1 ("TABLES" --> extract "CELLS"): 

> On Friday, February 5, 2021 at 7:53:26 PM UTC+5:30 shree wrote:
>
> See 
> - https://www.pyimagesearch.com/2020/09/07/ocr-a-document-form-or-invoice-with-tesseract-opencv-and-python/
> - https://stackoverflow.com/questions/61265666/how-to-extract-data-from-invoices-in-tabular-format

(Source:  https://groups.google.com/g/tesseract-ocr/c/Kl9LJaMs9B8/m/OwWH4aEJAAAJ?pli=1 -- Please do ignore the rest of that thread, but those two links are handy to give you a good possible starting point.)

Your page has a bit differently styled "table" than the entry forms discussed in those pages, so expect to tweak and fiddle and maybe google a bit more. "zoning" ("zones") and "sectioning" ("sections") together with maybe "algorithms" or "python" or "openCV" would be good google starters, I hope.


----

