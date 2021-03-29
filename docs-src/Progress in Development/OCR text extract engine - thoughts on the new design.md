# OCR / text extract engine

See if we can get [civetweb](https://github.com/civetweb/civetweb) or other embedded webserver mixed with [mupdf](https://github.com/GerHobbelt/mupdf).

First thought I had was if [mujs](https://github.com/ArtifexSoftware/mujs) (javascript engine by Artifex, part of mupdf) is an interruptable/continueable system, but best to assume it isn't as very few systems are.

Point is: we need some way to observe and steer the image extraction to OCR process inside mupdf, so we can improve images before feeding them to the [tesseract](https://github.com/tesseract-ocr/tesseract) engine.

That involves image filtering (possibly configurable/adjustable per page as some pages may be scanned more badly or differently than other in a pdf).

That also involves image sectioning: multi-column and other non-super-simple layouts can be best OCRed when we feed the image piece-meal, i.e. apply a series of *masks* sequentially, then recombine the OCR outputs of all those.

> Forgot, but there's another process around PDFs where we want to ditch, or at least clean up, "cover pages" and such: that would involve feeding page images to some sort of AI/statistical engine to recognize cover pages and then have only the relevant bits extracted, or at least marked cleearly in the text layer, so that text extraction by mupdf will deliver these in optimum quality.
>
> Another approach there would be a post-mortem filter where *everything* is extracted and we deal will cover pages by recognizing and filtering them at the hOCR/extracted-text level, i.e. not by looking at the original layout/image, but by looking at the text/layout output. While this is a viable approach IMO, I'd rather also wish to end up with *cleaned up PDFs*, derived from the originals. For print and other viewing purposes where I don't want to be bothered with the coverpage clutter.

Having the entire OCR+textExtract process configurable/modifyable through a little scripting is all nice, but it would lengthen and already long running process, which is, without additional means, very *unsuitable* for any UI responsive approach, be it based on chatting with a webserver or otherwise.

I've consider "push technology" or "chat lines" (SocketIO et al) for the mupdf system to talk to the top layer, i.e. the UI. However, while these technologies exist, it is still a "unconventional"/irregular to have a (web) interface like that. It's easier to code (also by others) to have a system which is basically "web page" with possibly a polling system underneath to provide UI updates.

I think polling here would be easier, as that would make for some very simple JavaScript actions to update the UI. Reactive is all nice & dandy, but that would then have travel through the entire system, down into the bowels of mupdf and other systems we're using. So at some point a polling mechanism would be in order to ensure the UI gets periodic updates.

Another though in *support* of polling (vs fully reactive / event driven) is the notion that:
- UI updates for a batch processing system don't need to show *all of it*: the batch system should hopefully be faster than the human eye could perceive UI updates, so you want to see work happening, either through snapshots or less strict means where your UI shows updates of the work done in the backend "as it happens". With events and reactive you then would need to code filters, which collect or discard events percolating up, for otherwise you'ld be swamped in updates that are unnecessary and only loading the UI system tremendously: imagine a system which does process about, say, 50 pages per second through the pipeline. That would mean your entire stack should be realtime video-capable if all those pages (and their intermediate filter stages when you're watching the pipeline progress page I imagine) are to be drawn in the UI. Let alone the cost in computing, it's too fast to be useful for folks anyway. That power could better be used to speeed up the PDF processing itself.
- polling as a mechanism to talk to the mupdf+extras backend means we can use basic HTTP request-response interface approaches, which are well known and supported by many. A lot of knowledge about such data flows is available, when we're stuck. Restartable/Continuable systems are *hard* to engineer and rare besides. Any push tek (or event propagation from backend to UI front, which would be the same, conceptually) is harder to mix with an otherwise request-response based system. Better to keep the reactive part to the UI only and do the mupdf augmentations in a classic fashion.

That drives the question how we can ensure the caller (UI) is able to find out about the progress of our backend work? Do we define a set of states in the process to monitor and list those as part of the initial response? Hmmmm.

The whole shebang would start with a PDF (or a bunch of them) and at least the scripts that are going to guide the processing of said PDFs. Of course, we could define a number of "checkpoints" in the code, where we can store images and other data for the UI client to request / poll at any time, but that sounds rather restricted to: it would require script analysis, guaranteed unique numbering of states up front, etc. 

Can we come up with a system where we can "debug" such a script as it happens, without a lot of up-front analysis and so forth?

Every image or action will have this key:
- document
- page
- line # in the script
- number of times execution passed through there (loops can visit source lines multiple times!)

That's a complex key and caching images just because a user/UI *may* want to look at them sounds like a lot of storage consumed for a non-primary purpose.

How about we allow the user to code `debugger` statements in there (JavaScript already has that one, after all) where we can **break/pause** the process. 

> I say **pause** because I want the process to be continuable after that: put in a `debugger` statement to see the state of affairs at that point in time, then hit a key to allow the process to continue if you like.
>
> **Breakpoints** is the term I'm looking for here, I guess.

So how can we cope, in a batch process, with such *breakpoints*? And with their cousins, the 'print'-like debug statements, where the script coder coded a point where you get to look at the state of affairs while the process barges on?

What about caching those images, etc.?

We don't know what images will be requested by the UI, and in what order. The UI view may be a simplified/contracted display of the process stages, each of which *may* have a 'print' statement at some point. When the user *refreshes* a web page, we loose all images downloaded so far: if we are going to cache all of them, we're back at the memory/storage size problem again. So some images have to loose and disappear...

Current thought is that the cache should monitor the age of creation/updating of the image -- the image may be updated due to looping, next page or whatever the conditions are in the `print` statement in the processing script.
The cache should also keep track of when which image was requested last. Knee-jerk design would say the last-accessed images should be kept around, but then thinking about the visualization of the process, which may follow through different sub-pipelines while processing different pages/documents, we always wish to see "what's been happening most recently in there", so the creation date is perhaps more important. At least the 'bleeding edge' should be maintained and not discarded because older images happen to be requested a lot and thus bumped up the cache lifecycle. 

That means we should create a cache which tracks two 'ages' (not necessarily *timestamps*; it can be counter-trackers which determine the age by subtracting from a 'right-now' counter marker) at least: create/update + fetch times. Because we cannot know how long it takes for another image to be created and thus land in the cache, if we want only a limited number of 'front of the activity wave' images to stay around no matter what, timestamps are *out*. Tracking this sort of thing is much easier with counter-trackers as the 'front of the wave' will simply be aged 1..N for N front-of-the-wave images. That makes cache management and thus cache coding easy: we can fiddle with the control where request/fetch age becomes more important for live-or-die consideration, past the age=N point for the front-of-the-wave images in there.

But! How to get at that stuff? 

If we say the backend process (batch-like) is running along and not abort/continuable, the way in there would be via a *monitor thread* which handles the user/UI requests for data, while another thread is doing the actual work and filling the cache slots as it goes along. The monitor thread can then pluck these images from the cache on demand.

Which leaves with the *debugging* question: how to "step through a process", e.g. when we are closely monitoring or *debugging* the processing script(s)? 

Here the main processing thread would need to **WAIT** -- not necessarily **stop**! We may wish to *continue* when we're debugging this stuff and stepping through the code = process!

Then a user/UI request can send a STOP or STEP/RUN command to release the waiting thread, having it continue from where it stopped, e.g. at a `debugger` statement in the script, just like everyone used to JavaScript would expect. Only this time not with Chrome DevTools but while using custom viz for PDF processing scripts.

> Incidentally, the way we're thinking right now, it would mean we have two(2) JavaScript engines in there in the end: node/electron and mujs. Wouldn't it be smart & leaner to just have one? Why, yes, *theoretically*. But mupdf et al has mujs already integrated in a way, while hooking up Chrome V8 (or `node`) is no *sine cure* in there, so, though I'm a bit bothered about this, yes, we will end up with two JS engines.
>
> Regrettably, that means the PDF processing scripts will be limited to the mujs abilities and users should therefor not expect to be able to write entire *applications* in that stuff. That's what Electron/node is for, but not for PDF processing here.

Incidentally, I haven't addressed the issue of "how do we know which image to request for what stage, etc. in the process". 

We mentioned the compound 'cache key' before:

- document
- page
- script source line
- visit # for this source line since start of process (to help cope with loops, etc.)
- extra: part # of the work done in this line (*will explain this next*)

First about visually developed scripts, i.e. *generated scripts*: since we generated the script, we can include `print` snapshots anywhere we need for the visual elements to receive suitable visual updates: here we can either stick with the line-number approach to link them up, *or* we have the code generator produce unique identifiers for each `print` to mention as part of the storage/request key, e.g. `"thresholding-001"` for the first thresholding stage in a simple or complex workflow ('process'). The user/UI side can then poll these keys for up-to-date images to be fetched from the backend cache. (Or show a "not available any more" replacement when the backend has decided the image had to go already to save storage space.)

When one is writing such scripts by hand, creating "unique keys" like that is a hassle and **very error prone**: a key collision is only one *copy-pasta* away, after all.
So we should *also* note the source line in the script.

Is that safe? What if the script changes?

That's the beauty of it: when the script, if ever so slightly, it needs to re-run and the entire cache/display is completely outdated and to be discarded anyway. 

So all we have to do is scan the script for any `print` statements, note their source line, and then request the most recent image for that line from the backend.

What about loops and deep caches? Where we want to see progress by being able to see older results at this same spot as well?

Well, how about the cache being able to report a list of entries available then, sorted by age? Next, the user/UI can request each of those images and show them in a fancy visual stack or however you want them to be displayed in your debugging UI environment.

Hence having the *code generator* produce unique keys is *bogus*, for it is only useful for that particular *CASE tool* scenario. Better then to have that additional string work as a "description" for us, to be displayed in other monitoring systems where ready access to the code is not available. Thing *logging* here: there it would be very handy to have a bit of a *legible* marker for the `print` spots in the code.

Oh! Oh! Oh! How about when running the same script in parallel? Or how about images still in the cache, while we already moved on in the batch cycle to the next document? We would be displaying the *wrong stuff* there, would we?

Last one first: in the batch-cycle scenario, the older image would have been superseded by a newer update already. **Unless** the new document traverses a different *path* in the code and the current line is **not* hit by the new execution: when the cache is deep/long enough, that will happen. And as a result you'ld see non-visited branches adorned with outdated ("wrong") images. *Outdated* here could be for a different document, but also for a *different page* in the same document, when we're talking about a loop being executed. So that's where the other bits of that compound become important: current document, current page, etc. need to be tracked and reported back with every cached image. Or an equivalent *hash* representing it, but then the question becomes: is *outdated* here only really *outdated* when it doesn't concern the current document, does it not matter here that the page is a previous one, etc. -- stuff to reflect when we develop and test that part. Keep in mind to track the entire compound key.

Ditto for the parallel running script: assuming we're not doing dumb *duplicate work* in there, we can safely assume it's happening to a different page range or document, so that compound key is again our aide to determine which image goes where.

Oh! Do we keep that "front of the wave" cache *per thread*? If we do, we can guarantee equal attention and availability of viasual feedback for each task, when multiple tasks are executed in parallel.

Alternatively, we could have a single global cache and a simple 2*N age wave freshness bound if we have 2 threads doing work: the faster thread will gobble up the cache if we're unlucky.

Then again, there's the question **iff** we would want to monitor multiple tasks in parallel like that? For a general progress monitor it isn't critical to be able to watch both "waves" simultaneously. Or is it?

My current hunch is that *visual feedback* at this level is to be reserved to a single task, as otherwise *debugging* becomes a nightmare to do. So that removes the entire problem from the table. And would probably "attach" the visual feedback image cache to a single task/thread to begin with, so in that way the problem is also non-existent.

When you want visual feedback in a generic progress monitor, would youu want to be so precise in your monitoring/reporting that you want report on each thread, like some kind of valve pressure monitor in your critical submarine diesel engine? To be discovered when we get there. At least, we could then extend the *compound key* with the *thread number / process identifier* to address each cache/image uniquely and possibly code it with multiple caches, one per thread/process, and the webserver properly addressing each one as requested -- since you can send the thread identifier to use for the lookup with ease if you decide to code such a monitoring system. Problem solved, [AFAIAC](https://acronyms.thefreedictionary.com/AFAIAC).



### And the 'extra: part of work' key bit that you just added there?

Yeah. That bit is a bit complex: imagine you have a *splitter* API, where a page image is *split* into multiple parts all at once, given a set of clipping/masking specs (not necessarily all rectangles: think boolean-ed polygons which determine the extraction mask for each part): the output would be an **array of images** instead of a single image.

That would mean other APIs would also be able to process a batch of images (think *array* processing) then, or this would be a rare specimen indeed.

The alternative is to apply only a single mask spec at a time, hence the API would maybe accept an array of clipping specs, but that would then come with a single *index*. Hm. More obvious would be to have the API accept only a single mask spec (the JS code can do the array indexing for us beforehand, *easy*) and thus output a single image, rather than a bunch of them.

On the other hand, if we ever were to progress towards array processing in there, such an array would be itself a *single entity* and thus a single cache slot. And thus requestable as a single entity: the server response might then become an image list, rather than an image *direct*. Anyway, case closed, problem solved, AFAIAC.





## Processing multiple PDFs in parallel

mupdf *schlepps* a `ctx` context around, so we should be fine if we process multiple PDFs in parallel, one per thread. I haven't tested the stability of this yet, but at least *theoretically* this should be doable, given my current knowledge of the mupdf design. So, yes, we can process a 'viewed right now' PDF next to a batch backend process updating our search engine, etc. via metadata and text extraction.



## STOP / ABORT / BREAK / STEP / CONTINUE / RUN debugging: how to go about it

Let's consider this a second. Fundamentally, there's only two possible commands coming from the user/UI when responding to a "what to do after I stop waiting on you" `debugging` blocking statement in a script: **ABORT** or **CONTINUE**. Stop & Break are basically aliases for Abort, while Step and Run are for Continue.

So that's makes for a simple playing field:

### How to handle ABORT?

Well, mupdf has these throw/try/catch C macros: use those to throw an exception and abort the process. 

> **Whoops! That would imply we've got memory leaks!
>
> Which leads us to another old question I've been pondering on and off: how about restartability of that "mupdf server stuff"? Because the only sure way to cope with memory hogs in the long run is either do your old-skool server work on this and find and plug every leak -- which is bloody hard to *neigh impossible* with C exceptions, as it is, despite all cuteness, still a bloody `longjmp` underneath! *Or* we kill the process and let the OS clean up for us, every once in a while...

Given that I do not want too many layers of interfacing from backend to UI (for that would impact the amount of comms cost and thus attainable UI performance), I can't really accept the "webserver" running those "mupdf processing threads" as a kind of cgi CLI process. Besides, it would complicate the needed wait/stop/continue debugging feature that way too, having the 'server' invoke separate executables to do the actual work. 

Meanwhile, there's another approach that would save my bacon: `fork`ing and then having this abort/exception=`longjmp` stuff happening in the *child* process, which is then `exit`ed, does clean your heap allocations IIRC... Quickly double-checked my *bio-recall* from W. Richard Stevens works about this and [here's what SO says: good to go with `execvp` et al](https://stackoverflow.com/questions/23440132/fork-after-malloc-in-parent-does-the-child-process-need-to-free-it). And I like doing inter-process comms anyway, so there's a nice one there! Just as long as I don't have to exec multiple `*.exe` executables as the startup/init time for each such invok would become horrible, performance-wise. So I better dust off my Stevens pumps. :-)



---

---

# Tesseract aspects to keep in mind

(Have written this for a response in the tesseract mailing list. Important bits to keep in mind and suitable for (1) technical documentation here and (2) grounds for the work done on augmenting the MuPDF + tesseract combo with maybe some OpenCV and a few helpful OCR algos, controlled through some form of *user controlled script*.  (*user controlled* as it might be GUI-driven, one day. One can hope. :-) )

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

