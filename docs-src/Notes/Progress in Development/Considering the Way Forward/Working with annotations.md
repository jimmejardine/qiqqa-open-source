
# Working with (PDF) Annotations

NOTE: this is currently a cut&paste dump of a reply to issue https://github.com/jimmejardine/qiqqa-open-source/issues/317, where I found myself jabbering on the keyboard and going tangential. Stream of consciousness? Whatever. Handle with care, might be confusing and causing more questions than answers to show up in your cortex.

---


## Prelude / Context for work done on Qiqqa

Re the annotation work: I have painted myself into a bit of a chicken and egg corner right now as I've done work on mupdf et al and need to integrate that back into Qiqqa first. Problem isn't the integration per se (that's done), but the *performance* of the whole darn thing: as I finally ditched the obsolete (buggy) sorax PDF render lib and replaced it with mupdf (mutool draw), there's two things that bog the whole thing down:

- .NET wrapper was/is crappy. *Plus* it would again bind me to 32-bit .NET builds (like Sorax and xulrunner are doing still), which gives me loads of trouble when running Qiqqa on (very) large libraries: 40K+ entries and you're starting to hit some serious engineering roadblocks in the codebase, eating 1G+ RAM and soon getting to the 'out of memory' fatality zone. (32 bit .NET heap limit) -- how 40K+ document reference records can eat 1G+ RAM is another story. 😅  The proper fix there would take another round of re-engineering ("refactoring") the codebase in a very significant way and one major refactor (PDF page rendering and text extraction+OCR) is enough for now. 😉 
  - Conclusion last year after several tests: don't go for .NET wrappers (which are generally less well maintained anyway: fewer people using and working with them) but stick with generic multiprocess. That's more system/execv UNIX style communications and at least *theoretically* offers a more portable situation (there's Linux Qiqqa on the horizon somewhere 😉 )
- The above runs: mupdf (mutool draw) is invoked, spits out a page image and that gets picked up and drawn on screen.
  
   The bit of this stuff that is responsible for "why hasn't this been released in a fresh test release yet?" is two-fold:
   - first, there was the interface stability (binary over stdout; spurious lockups due to very subtle pipe issues on Windows / .NET. That done.
   - second, there's the big elephant hiding in the cupboard: **performance**. For this stuff to get working (PoC), I had to nuke all the image caches that were tweaked into the Qiqqa codebase. Imagine the regular Qiqqa behaviour where the background processes kick in after a while and start OCR-ing: that's page rendering + tesseract. The "page rendering" bit obstructs the page render requests from the Qiqqa UI. That bit has NOT been refactored yet. 😓 
   - same elephant, *cloned* and distorted: fun happens when you scroll a PDF: every "page" you hit on the way fires at least one render request. (Qiqqa code is sub-optimal and does 3, on average, but that's an aside). The render requests are queued. FIFO, not LIFO. **Oops**. 😅  FIFO queueing thee background page renders is okay but exactly thee wrong way around for UI: you're very probably waiting/hovering at that *last* page and not the first, when you started rubbing the scroll wheel. *Plus* it might very well be that the previous pages are not even on-screen anymore and thus totally uninteresting: TODO (this never existed before) is code to check if a (costly) page render request is actually still *useful* once it popped out of the queue/stack and is sent off to the PDF page renderer. That's still a bit of, *ahh*, non-trivial work waiting for me. 
   - same elephant, cloned yet another way: *multiprocessing* is cute and may sound fancy but the current `system/execv` style approach has a big performance flaw that's only getting bigger: for every page you render, you have to start that render executable (mutool draw...) once again and that's a cost we can do without.
      Bulk testing my mupdf build has at least shown me that current mupdf code (plus additions) is stable and robust enough that heap/memory leaks are few or nil; they don't shown up on the radar when rendering and otherwise processing thousands of PDFs in a single inning. This signals to me that I can do something that I wanted to accomplish anyway: have this part of the toolbox run as (another) service that can be employed by Qiqqa *and* any user-coded tooling around qiqqa (custom text extraction and OCR processes for instance). In short: running the mupdf+tesseract+imageprocessing libraries as another localhost server you and qiqqa can talk to. (This replaces the current `QiqqaOCR.exe` wart, that suffers for this same problem already. Part of the current refactor and part of what makes it huge: the PDF renderer was everywhere and *is* everywhere. 😅 😅 )

That's one part of the context. Second part is related: where I want to go with this and why I'm working on it anyway:

Somewhere this year I intend to have a Qiqqa that's at least *in part* acting as a (local) webserver, say, https://localhost:9194/ with a bit of an API so you (and me, and others) can code their own special sauce wishes and "talk" to Qiqqa libraries at the machine level in a non-hacky fashion.
Next to that there's the UI I want to migrate to something that's more web-tech based than !@#$%^ WPF: other feasibility tests with Electron, Chromely, CEF and WebView2 have led me to believe this is doable. Not in Electron (which is (node) JS talking to browser/UI JS and thus one fat layer too many if I want to keep at least *some* of the current C# codebase as "business logic", so that got me to Chromely (which is sort-of-Electron but a .NET backing) and then having run some dev tests with that and running into other brittleness, I've come to the conclusion that I'd better roll something using CEF or WebView2 as a starter/intermediate migration, so I can then focus on untangling the current "business logic" from the UI. (which is already being done piecemeal and **very** preliminary as I'm recoding all UI interactions as "async" executions -- which has caused quite a few bug reports for v83, incidentally. 😓 )

Right now those two major blocks are a bit much and I find myself dawdling here and there. It's okay and it's not okay at the same time. Alas. Will get upset enough to take a 12 gage taking names and finish the job; it's just the quiet between storms right now.

## Which leads to the work and coaching

Technically, your work best fit in with the other "backend jobs" because we could treat this as just another channel of feeding metadata into Qiqqa. That would mean coding something that talks to a localhost socket and maybe sends some data and/or paths to PDFs to process after they've come back from any "external editing", e.g. annotating on mobile phone.

### Question about your current python code

*How do you currently link the new PDF to the old one, or rather its Qiqqa ID (hash)? Using the hash-named PDF from documents/XX/ and saying "same name is same document" that way, or have you coded this in another way, so as to edit the matching database record?*

### Mixing this with Qiqqa 

As I see it, this would fit best onto a "web API", i.e. socket connection talking to Qiqqa.

It would take a few things to get there:

- add a basic webserver (.NET code) to the Qiqqa codebase. (Please don't say ASP.NET. 😉 Been there, not going back to jail.)
- add a bit of router logic so we can do REST stuff or similar so we can have ourselves a 'localhost:9194/annotation/' API of some sort. Haven't thought about this yet, but it would definitely be a web API which you could then test with, say, `curl` or some basic JavaScript. Whatever it becomes, it should be sensible for a browser-side javascript chunk to peruse. (Think SPA, that sort of thing)
- (another elephant hiding in the room) come up with a solid answer how we're going to deal with "edited PDFs", given that qiqqa is conceptually treating PDFs as constant or "functional": the PDF content hash identifies a document, irrespective of filename. Different hash IS different document. And there's the bit missing in there about linking up "similar documents" for N reasons: reprint, different publication thus different layout (oops, that's another hash then!), rewrite, revision, **annotated / user edited**, ... -- this is a functionality currently lacking in qiqqa.
- symmetry perhaps?: when we import annotated PDFs, shouldn't we also be able to *export* annotated PDFs? (it's been requested). Not a requirement to have it happen at the same time, but the "how do we recognize this PDF as a 'clone' of our original document XYZ-HASH" solution should keep this outgoing road in mind as well.
- augmenting the UI: annotations types supported by mupdf? 
  - edit the current .NET/WPF codebase?
  - do we wait for the web UI? (may be *long* wait; plenty to do!)
- testing! (Are we doing the job of annotation extraction well, when user did something *externally* using app X, e.g. PDFAnnotator or FoxIt or Acrobat or ...?)


----

## Rendering and using annotations in HTML

Incidentally, observe how Elsevier solved this for showing (and editing = annotating?) their PDFs in their website: https://www.sciencedirect.com/science/article/pii/S016412121630187X --> click on Download PDF to get a (current session = token-locked) PDF view with ***overlays**: note that Elsevier has (at least) 4 DIVs for the annotation work, among which are:

- a text layer showing the printed/extracted text for selection (& rendering selection boxes while you click&drag your mouse over the page(s).

  Do note that *deleting* this layer will not alter the rendered page view, which itself is landed in a CANVAS node. This is **very** similar to the current Qiqqa approach in WPF/Net, only now done in HTML. absolute positioned DIV, transformed SPANs for the text words, etc.
  
  **Note to self**: analyze this further: it looks like you can only select per word, or it might be me, my jittery mouse hand and the quick 2 second glance at this bit in the UI and DevTools...
  
- a annotation layer, *specifically* for LINKS in the PDF: each link is a SECTION + A tag hierarchy to help render a link click area that hoverable and clickable. This is the last DIV and thus top-most, lacking any z-index in the CSS (I haven't checked that, just assuming defaults before taking time to analyze this further. So far, Elservier's execution matches my ideas about how to implement this.)

- two more DIV layers I haven't looked at. Given t heir feature set, including ability to present a list of FIGURES in the text, I bet one of them is for figures zone / segmentation markup.

  To Be Investigated on a dull day or evening when the brain is trundling at half pace again.
  


### Related projects

- https://annotator.apache.org/
- https://annotatorjs.org/
- 



