# Processing other document types: HTML

Today (2022 AD) I notice that quite a few of my relevant documents are published as *direct web pages*, rather than as PDF. 

This may apply in lesser degree for a *purely academic environment*, but there I observe several papers having been made accessible (to the non-paywall-scaling plebs at least) as HTML pages only.

Next to that, there's notes, comments/critiques and (regrettably relevant some times) retraction sites, all of which are web (HTML) based, rather than PDF.

Now we could take the position where we store everything we encounter as PDF, which is doable most of the time, but sub-optimal, both in formatting and storage costs per item: the HTML-to-PDF renderers available do their best, but most such web-based publications have never been near a page-layout CSS style sheet in their life, or page-layout "print output" work like that is done in a rather low quality fashion: creating great print-ready stylesheets is no *sine cure* and often requires customization per document. Combine this with the relative low priority for the originator to deliver this type of format for their own ultimate benefit and you end up with good will and a "*meh*" result 9 times out of 10.

We also should not forget that, under the hood, we do a lot of work to get back from PDF to a HTML-kind of content format -- you may not notice it as such, but any PDF text extraction process, whether OCR assisted or not, is hard-pressed to produce a continuous data stream uninterrupted by obnoxious page numbers and other page-boundary content that's not relevant to the document flow and the information provided there-in. When we say we're interested in formats such as hOCR, we're not looking at an *addition to the Qiqqa feature set* but rather wonder whether we can store the current text extracts any way **smarter**. Thus the obvious "*least common denominator format for **accessible & processable** document storage*" is HTML/hOCR, rather than PDF. PDF is only very handy because we are, in our own way, *librarians* and thus want to keep the **original source** around for posterity/reference. The academic field (and anyone sane) demands that: no original sources, no back-up for your arguments. *Fake News* and similar patterns make this a still-insecure approach, but it's the best we've got, so storing and keeping those original sources *intact* is a *must*. And there is where PDF shines -- unless our *original sources* are already HTML web pages themselves: then PDF, a page based storage system, shows its non-web roots and fails to deliver. So we would do well to copy/*mirror* web pages we wish to import as *documents* -- keep in mind that the *average* website pages' "*lifetime*" is about 2 years, according to some research and a lot of personal experience. Keep your own copy, which can reproduce the page that *was*, isn't just a nice *hobby* or to be relegated to a visionary like The WayBack Machine: keeping a local copy is an essential part of being a library.

Hence we should be able to store any HTML page *mirror*, i.e. including its CSS, images, etc. 

*Possibly* we should store those pages as *DOM snapshots* so we are not dependent on changing and buggy (or disappeared) JavaScript code loaded from third-party sites as part of the web page render. The only drawback there is you cannot really snapshot a web page that's very dynamic, i.e. has all kinds of fancy JS-driven content-hiding and showing/revealing built-in. That is regrettable, but does not diminish of having the capability of mirroring/snapshotting a given web page; it rather points at the technical complexities involved when we want to achieve this goal.


## Do we consider storing other *original source* formats, given the logic above? YouTube movies?

No. At least not at this moment.

Storing *multimedia* like that will require a few other additions (viewing, processing: CC (Closed Captions) as text extract, etc. -- what to do there and how doable is it, really?!?!) so we'll stop at *documents* which have a large(-ish) *text content* component, which we can then index and search. For video-based multimedia, that *search process* is still the field of a few select players (e.g. automatic CC production in the original language, thanks to speech recognition, as done by Google/YouTube).



## Do we support more formats?

No. We're not The WayBack Machine: the purpose of all *original source* collecting and storing is to be produce both the original and the text information available therein so we can *find* stuff we seek and *analyze* that (text) content to help our discovery investigative processes.

Storing stuff like MS-word documents, etc. may sound nice at first, but makes for a quite different library approach: we then SHOULD also offer easy reading/viewing capabilities for all those formats and that's not what we wish to spend our efforts on; meanwhile storing these various formats and only having PDF or HTML/hOCR formats easy-to-read, makes that limited (yet already very complex!) capability a nuisance, rather than a boon. As we cannot treat those other storage formats democratically from a view/render perspective, we are all best served by not accepting those formats: after all, (almost) all of them can be transported to either the page-oriented PDF format or the continuous-stream-oriented HTML format.

With those two we have covered 99.9% of the *original sources* market as far as I'm concerned.


