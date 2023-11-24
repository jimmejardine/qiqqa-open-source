# scripting the OCR + text extraction process

So I've been looking at getting mupdf (with built-in tesseract) for the new approach to PDF text extraction in Qiqqa. 

Old (= Commercial) Qiqqa uses an antique (patched) version of mupdf AFAICT (`pdfdraw`) for text extraction only. OCR has been handled by tesseract 3.0.x, serving us in the form of a DLL.
New Qiqqa, I hope, will do all that, *plus* page rendering to image for the UI, via the latest `mupdf` versions, using the `mudraw`, `mutool` or possibly others from the `mupdf` package.

I want to "open up" the OCR and text extraction process so I can give people the option of *editing* those processes to suit their need, for example when handling obnoxious "crypted" PDFs.  Some PDFs are scrambled to "protect" them against text select + copy/paste, for example!

I was considering scripting it with JavaScript and then came up with the thought that `mupdf` already includes `mujs`, a (basic) JavaScript engine used by `mupdf`, so why not use that one to 'inject' a custom/configurable/user script into the `mudraw` tool when using that one to 'textify' the PDF and thus hook our script in between the text extraction process and the tesseract-based OCR happening in there, when `mupdf/mudraw` decides OCR is necessary for any page in the document being processed.

While this and similar thoughts have been mulling in my head the last few months, I could not identify what was *bothering* me about the whole idea: the 'scripting' bit is good, the need to have it sit between the page image render code in there and the call to tesseract to OCR that page is also obvious -- including the notion that more complicated pages, e.g. scanned newspaper pages, might benefit from some (scripted) preprocessing of the rendered image into multiple 'gangs', i.e. multiple images, where each page represents a particular chunk (not necessarily rectangular, mind you!) of the page and then feed *that* to tesseract to improve OCR results. In other words: I want us to be able to customize the **segmentation** (and image processing, a.k.a. **binarization**) processes of the page to make the job easier and more obvious to the tooling already built into tesseract.

Yes, I expect to modify the base `mupdf` package quite a bit, but hope to do it in such a way that it is still easy to 'track' the mainline development of that package, so that I can track the work that continues to be done by Artifex for the foreseeable future.

Meanwhile, something was nagging me. I felt the approach was somehow flawed but could not identify what was wrong with it.

A short time ago, I looked at the `mupdf` code again, *specifically* for the reason of identifying where I would need to inject the script calls, which parts I'ld want to present as an API to the script (which would, if I get my way, include additional APIs from OpenCV and maybe a few other spots where the latest binarization and segmentation research results have been published as source code). And I considered how I, or *anyone*, would be able to debug such user scripts. If it would be small stuff, a couple of classic debug-print statements would suffice, and then `mujs` would serve well.

However, when looking elsewhere for work other people have done with tesseract and OCR processes, I noted a couple of things that had me reconsider all this:

- the image preprocessing *can* span multiple stages. 
- I had neglected the important difference between **segmentation** of the image (identifying the columns and blocks of text, identifying (and then *ignoring*) any *images* on the page, etc.) and the **binarization** process (which takes the full-color input image, cleans it up (denoising, removing scan shadows, vignettes, etc.) and produces a as-clean-as-possible black-and-white image to feed to the OCR engine proper.)
- tesseract 4.x / 5.x, which is now included in `mupdf`, thanks to work by Artifex, uses leptonica for this. I'm still somewhat vague on the algorithm(s) used for *segmentation* in there, while the *binarization* uses some classic form of Otsu binarization.
- that's all great and dandy, but I have got plenty PDF inputs that won't perform well with classic Otsu (some old books from 1900-1960 and some magazine scans from that era as well) and from what I've seen so far on the Internet, the segmentation can use some help as well if you're processing non-mainstream whitepapers only. **That's what I wanted the *scripting* to help us out with!**
- The scripting idea is still good, but the processes are more complex than I initially thought.
- So the debugging experience would greatly improve if we could see the images for each intermediate stage in the process, particularly when we are setting up the process and then tweaking (parameterizing) it. Still `mujs` was viable; only I would need to code a slightly more advanced debug-print statement then to show us the current image.
- Another thing tried to reach my frontal lobe time and again: *most* of this work *out there* is done in Python. Using PyTesseract and whatever they get their hands on. Mostly that's OpenCV and some `numpy` work.

So I started to change my mind:

- I know I'm not fond of Python, but given the surmounting "peer pressure" in this area... let's face it!
- This was helped by me running user scenarios in my head -- which is a thing that I do often: I can then "see" the software at work and observe how I, as a user, would peruse a system, even when it doesn't exist yet. I can spot difficulties and possible different behaviours I would not consider otherwise, when creating the code as an architect/programmer. The major outcome of those scenario running sessions is this: I (the Qiqqa developer) would end up with a *ton* of requests to port any python script out there that does some OCR "magic" to `mujs` JavaScript (`mujs` only supports ES5, not ES6/ES2017, so that's another drawback by now in 2021AD) for the transformation of such scripts is non-trivial, even when the user has medium level programming abilities. Ergo: maintenance hell awaits and no more time left to work on moving Qiqqa forward.

  Unless I bow to this "peer pressure" and forget about using JavaScript and use Python instead.
  
Is that what was nagging in my hind-brain all the time? **No.** It's a small part, but we're not there yet.
  
I looked at CPython and what needs to be done, roughly, to get it embedded into `mupdf` and allow python scripts to run to do the OCR and custom preprocessing work once a PDF page image is rendered by `mudraw`. That is: Python wouldn't be driving `mupdf`, it would be the other way around. That's what "embedding" an interpreter like that means.

This took a couple of days, but then it hit me what was bothering me so much, also while I had resolved to (having to) use Python for this instead of `mujs` or node-based JavaScript.

Given the complexities around the text extraction work, **including the cover page filtering** I want in there, it's *convoluted* to drive everything from the starting point of a 'regular' `mudraw` text extraction run: *yes, it is text extraction that we are working on*, but I probably need scripting abilities *before* I render the page image! What about AI-based libs that can help "recognize" cover pages and assist us in then choosing different extraction processes: some cover pages contain useful metadata, so it's not just recognize-and-ignore/discard there.

This means that I would probably be best served if I drive `mupdf` text extraction from a script already, so that would mean python calling `mupdf` tooling (`mudraw` et al; the other tools in there can extract metadata, such as annotations and title/publisher metadata, if available, from the PDF!), but I still need that scripting to *also* sit in between each rendered page and the follow-up OCR process. **That** means I'ld be invoking python from **inside** the C API that gets invoked by the outer script. In other words the problem is this: can I have a python add-on, done in C/C++, which calls a **python callback** or anything of that nature, so that I can *stack* addon calls with python code *recursively*? At minimum, it would involve *nesting* of the calls. How would a python debugger cope with such a stack trace, where some stuff in the middle of the call stack is native C code (mupdf/tesseract)?

I have not seen that sort of thing mentioned *anywhere* yet, so that's another technology viability test I'll have to code to make sure that python can deliver on this, without me needing to completely overhaul the mupdf code!

This is also something that, to me at least, is conceptually of the next higher order in complexity and therefor *not* suitable to do with a single thing like `mujs`, which comes with fundamentally zero debugging facilities: all there is, *right now*, is a custom `print` function. Which serves well for simple scripts, like I said, but these nested calls, where scripts call API functions, which in turn call script functions as callbacks, are a little more complex in my opinion (scenario: user who has *some* programmer ability) and thus require a more sophisticated debugging environment, or at least a debugging environment that can be improved by someone easily: python is far more "mainstream" than `mujs`, so chances are higher that somebody else already has done something that might help us there, or get us under way quicker and better.

The crux of this whole thing is thus:

- use python for PDF related scripting
- see if we can get `mupdf` hooked in as an add-on; AFAICT there's already some code so serve it as a python *plugin*. 
- make sure we can *nest* python, i.e. invoke python functions ("callbacks") from inside the `mupdf` C code. This MAY have to happen multiple times per single outer call -- think processing a pdf, getting at least one callback call for each rendered page, probably a couple more in order to help out filter/postprocess/do-whatever to the raw or semi-processed extracted text.
  + *bonus points*: analyze the extracted text and run a spellchecker over it and/or decide to **re-execute** the OCR phase with different settings, e.g. different language set or image filter settings, so we can seek out the optimal OCR results that way perhaps.
- this also puts our 'internal webserver' idea under new scrutiny: instead of using my old fav civetweb/mongoose for that (another chunk of C code), it might be possibly best to run the 'webserver' as a python script: after all, that would be the outer layer as we need scripts to drive the PDF processes anyway.
- note that you are now adhering better to your own rule: use best-of-class solutions as much as possible: python and writing python addons in C/C++ is very mainstream, also in research environments, while coding with `mujs` is *not* mainstream at all. `node` *is* mainstream, but it would be a lot harder to integrate that JavaScript engine into `mupdf` the same way you could integrate `mujs`. 

  And from what I've read so far, embedding python (CPython) is done more often and is a clearly documented process, at least at first glance (when I looked into embedding python, etc.)
  
  



  
 
