# IPC :: transferring and storing data in bulk between C++, C# and JavaScript components (and Java too)

TL;DR: we'll be using [SWIG](http://swig.org/).

---

## The Journey: requirements, considerations

What I have been looking for ways to save large amounts of data (document OCR/text-extraction & metadata dumps) and transfer these *quickly* and *efficiently* between the Qiqqa components, both now and in the (envisioned architectural) future.

What does that mean?

- **Today** Qiqqa is organized as a main application (`Qiqqa.exe`), which has linked in several libraries, including 
  - Lucene.NET (full text content & metadata search)
  - xulrunner (Web browser)
  - SORAX (PDF renderer)
  
  Next to that, there's `QiqqaOCR.exe` which is a separate process invoked by Qiqqa to perform all PDF document text extraction and OCR activities. *That one* also has several foreign libraries linked into its executable:
  - Tesseract.NET (tesseract 3.0, OCR engine)
  - SORAX (once again as PDF page image renderer, this time used to feed 'page images' to the Tesseract engine)
  
  Then there's yet another tool called `pdfdraw.exe` which is an *old* (v1.11 or there-abouts) mupdf tool which has been patched for Qiqqa's purposes to spit out *extracted text content* for a given PDF.
  
  > Incidentally, "text extraction" is more like modern hOCR output production: the extracted text is a mix of both *text* and *in-page coordinates for each character/word* as Qiqqa needs the latter to properly *position* the text as a hidden overlay over the rendered PDF when the user is viewing/annotating/reviewing a PDF in Qiqqa: the coordinates are needed so Qiqqa can discover which bit of text you wish to work on when clicking and dragging your mouse over the page image, produced by the SORAX render library -- which simply spits out an image, no text coordinates or whatsoever: that stuff is not always present in a PDF and is the reason why we need to OCR many PDFs: to get at the actual *text* **and** the text *positions* throughout each page.
  > 
  > *Anyhoo*... back to the subject matter:
  
  The current `pdfdraw.exe -tt` text extraction run and/or QiqqaOCR will produce a set of *cache* files in the Qiqqa `/ocr/` directory tree, indexed by the PDF document Qiqqa fingerprint. (In SHA1B format: the "b0rked" SHA1 checksum. See elsewhere (link:XXXXXXXXXX) for info about this baby.)
  
  The cached-on-disk files *today* include
  
  - a small file caching the document page count as discovered by Qiqqa itself
  - a series of 'text' files containing the extracted text from the PDF.
    - most of these files will carry the text content for 20 pages (file name will include the page numbers), while
    - some files (sometimes *many*) will carry the text content for only 1 single page.
    
    The latter happens generally when Qiqqa ran into difficulties and had to re-invoke QiqqaOCR to run a (heavy/costly) OCR job on some pages, because the regular `pdfdraw -tt` run did not deliver text for those pages, leading to the internal conclusion that those pages must be image-only (and probably the entire PDF has been done that way by extension), which requires an OCR run to discover the actual text. Hopefully. (OCR is a complex process and Qiqqa hasn't been all that great about it either, IMO, so YMMV)

  Those cached 'text' files have a line-oriented comma separated text format encoding the 4 bbox coordinates for a word or character, followed by that word or character of text.
  This way, we have a series of bounding boxes ('bbox'-es) seerving as rectangle area coordinates for where the corresponding bit of text is placed in the PDF page.
  
  Since this is all ASCII, suffice to say it's highly compressible, takes up quite some space yet still suffers from some inaccuracies which sometimes leak into the UI as obnoxious "off by some degree" colored selection strips when you click and drag to mark or select text on a page:
  
  - there the minor bit where having a single bbox for an entire word implicitly has inaccuracies and thus display errors ("off by a wee bit") for the individual characters in a given word as Qiqqa then has to assume that each character is equal in width, while this is a very rough approximation of the reality of PDF documents where almost all of them use *proportional fonts* (except maybe some very old photocopies from documents that originated off a manual typewriter -- we're talking about the pre-computer-ubiquity era a.k.a. "mid 20th century and older history". 😉  Not everybody had the luxury of access to a Linotype or similar printing press/industry equipment to produce their hardcopy.
  - the bigger error you will find in some PDFs is caused by using two different tools for different layers in your PDF reader display: SORAX does the PDF page rendering, painting pixels from the PDF input, while `pdfdraw.exe -tt` does the text extraction and related position coordinates reporting. And, of course, it so happens that for a certain percentage of PDFs available *out there in the wild*, these two tools have a slightly-to-very noticeable disagreement about where each bit of the PDF page should go on the page canvas, resulting in such curious oddities where you click and drag over a text to discover *nothing happens*, while you hit Ctrl-A (*select all*) or just click and drag across a much larger area, only to observe that the Qiqqa PDF viewer is coloring an area that has maybe about the *shape* of the paper's title, but is significantly off to the top and/or side, compared to where your own eyes are observing that title as black on white pixels.

  This type of error can be prevented/removed when we alter Qiqqa to use a single codebase for all this PDF work: the rendering *and* the text extraction. That's one important of the qiqqa *future*: Artifex' MuPDF has been selected as the future page renderer and text extractor. And that includes driving the (also updated) new Tesseract OCR 5.x engine.
  
- **The future** is where Qiqqa has been separated into a slightly different set of processes and those old libraries have been replaced by new tools:

  - As mentioned above, Artifex' MuPDF will be used for rendering the PDF pages for both user viewing and feeding them to the (new) OCR engine, while
  - that same Artifex' MuPDF bundle will be employed to deliver a *text extract*, including precise coordinates, **plus all kinds of metadata, including annotations**.
  
    This will ensure that there won't be any difference in opinion *any more* about where a particular piece of text ought to be be on the page!
    
    The only potential remaining troublemaker will be the OCR engine, or rather its **seegmentation algorithm / workflow**, which may decide incorrectly what a bunch of pixels may represent: OCR is never 100% accurate, so we may still have incorrectly identified characters or missed or non-existent characters, e.g. when some serious damage/dust in the original page image triggered the OCR engine into believing there's yet another text character there. Nevertheless, the obnoxious "off by a lot" selection markup oddities should be *gone*!
   
  - Since the current *document extracted text cache* is already pretty close, *functionality-wise*, to a generic HTML-derivative format used a lot out there: [hOCR](https://en.wikipedia.org/wiki/HOCR), I would like to move from custom ASCII-comma-separated coordinates+text files to a hOCR-based cache.
  
    There's a *spot of bother* with that idea, which we'll address in a second. Nevertheless, it's what is *stored* in both our current cache file format and hOCR that's interesting: text + in-page positions: coordinates!
    
  - The Full Text Search Engine will still be Lucene, but not in Lucene.NET form. Rather, I prefer to ride this one as close as possible to the larger developer and user communities (so I can share in their benefits) and thus Lucene will be present as a local [SOLR](https://en.wikipedia.org/wiki/Apache_Solr) instance. Native Lucene (and SOLR) are written in the Java language, so I expect to do a lot of talking to and from those systems: feeding it (postprocessed) extracted text *plus metadata* for each document and then feeding it search queries from the Qiqqa users.

    **Today's Qiqqa** runs search queries by hitting Lucene.NET with a search request, which will produce a list of *hits*: which file, i.e. *document*, has what you're looking for.
    *Then* Qiqqa goes and does the search *again*, *internally*, to find the exact spots in the document where the search query matches, for highlighting, etc.
    
    **Future Qiqqa** is supposed to do this slightly "smarter"... **but** there are problems with that which I will have to tackle: SOLR can spit out the *position in the text* by highlighting the words/lines matching the search query.
    
    Great! you say. Yes, it is. **But...** The highlighting is done in HTML (okay), and only knows about the **location in the extracted text as-fed-to-SOLR**: SOLR doesn't eat those precise positioning coordinates (check out hOCR vs. HTML: we use and cache something that quite close to hOCR internally in Qiqqa, while you need to feed SOLR plain HTML or TEXT, *sans zillions of character coordinates*, unless I have missed yet another info page about expert use of SOLR instances...
    
    Consequently, I can have SOLR do the in-text-locating bit of the search as well, but *I* still need to map that output to a match in the text-coordinates collection for that document to discover where the SOLR-reported hit is *positioned* precisely on-screen. That's not entirely trivial. 😓
    
    Meanwhile I still want to get rid of the old Qiqqa approach, because I, as a user, would 💝love💝 to have access to the more powerful search query features of SOLR. And re-implementing those search query features in Qiqqa itself "because I need to re-run the search to find the exact spots in the document(s)" is a gargantuan headache, so I'll be betting on that other problem instead: matching SOLR highlights to text snippets *as we know them* and extracting the in-page coordinates from there for painting the highlights, marking, etc.
    
  Now let's re-consider those new components-to-be from a technical *aspect*: *programming language*:
  
  - the Artifex/MuPDF+extras is all C & C++. (Oh, the SQLite database used by Qiqqa is also C/C++ code, and I am pondering moving that one closer to the place where all the PDF heavy lifting will be done, so when I talk about "mupdf", that's really:
    - mupdf draw (painting page images for user viewing and OCR feeding)
    - mupdf draw (text extraction: hOCR or similar text+coordinates formats production)
    - tesseract (for the OCR work, getting its images through some intermediate image processing for cleaning up, but it all starts with `mupdf draw` producing the raw page image)
    - mupdf draw/mutool/metadump for PDF metadata extraction
    - SQLite (because it's the same language and we may want to bulk-process PDFs as we do now in *today's Qiqqa*, where this work happens in Qiqqa. Might be handy have the DB in here.)
    
  - the FTS (Full Text Search) Engine is Java:
    - SOLR
    - and Lucene (the original!) at its core!

  - Qiqqa GUI in HTML/CSS/JavaScript: *today's Qiqqa* GUI is done in WPF/XAML which is C#/.NET but very much non-portable to Linux machines.
    Meanwhile, HTML-based UI systems have risen the last few years and are fundamentally geared towards cross-platform, i.e. Linux then is a real option! Nothing is free though: you need to re-do the UI in HTML and have a working "embedded browser component" on all platforms: this would be [CEF](https://bitbucket.org/chromiumembedded/cef/src/master/) if I get my way.
    
    Most folks may probably have heard about this under a different label: [`electron`](https://www.electronjs.org/) which is used by bigwigs like [Microsoft (Visual Studio Code)](https://code.visualstudio.com/). See also https://sharpnotions.com/blog/electron-is-it-right-for-building-cross-platform-applications/ for a bit of helicopter view.
    
  - Meanwhile, *I* want to keep at least *some* of the existing C#/.NET code (a.k.a. "business logic") which makes Qiqqa be *Qiqqa*: electron is not an optimal choice then because that one is a JavaScript-based browser (it uses CEF) *plus* a JavaScript-based backend ([NodeJS](https://nodejs.org/en/about/)), which *I like* but don't want as an extra layer between my remaining C# code and that browser we'll re-use as a UI/GUI via CEF!
    So [Chromely](https://chromely.net/) got in the picture as that is C#/.NET backend + CEF (via CEFSharp), so would be a better fit. However, [*installed base*, *user count* and *project activity*](https://github.com/chromelyapps/Chromely/network) isn't in the numbers where I would like it to be, plus there's API troubles to be dealt with with CEFSharp when you want something like a Qiqqa Sniffer to work smoothly, so I'm wiggling a bit and considering doing CEF directly, the *Chromely way*, i.e. a lean-and-mean dual-CEF UI executable (*dual* for security reasons: see link:XXXXXXXXXXXXXXXXXX) and no extra backing layer *at all*: instead, I imagine that UI executable to start the relevant backend processes (like the SOLR instance, the MuPDF+tesseract+SQLite+ALotMore instance, **and then another "businessLogic.exe" background application, that's carrying the remaining C#/.NET "busineess logic" into the future of Qiqqa. (Technically, the UI exe would kickstart that `businessLogic.exe` which would then, being the *business logic* after all, be responsible for monitoring and kickstarting the others: SOLR & `MuPDFExtraSpecialWithMayoAndCurry.exe`. Or their equivalent recompiles on Linux.
    
  ### So... what's it all coming to?
  
  The **future Qiqqa** is expected to consist of at least 4 applications/processes, each written in a specific programming language: the boundaries are defined by the tools' programming language, because I know from long experience how rough it can get when you want to talk to something that's done in another language than *yours*.
  
  So I pick Lucene **original**, which is Java. Since *I* don't want to code any Java if I don't have to, that leads to a simple choice: SOLR or ES (Elastic Search) on top. ES did some unfine shenanigans with their licensing, while their marketing has ensured a bigger market share right now, but SOLR is getting back up there and strong enough from my perspective to be selected. Then we hit a boundary layer that we can easily deal with: SOLR talks JSON and other formats, so we're good re communications. JSON is nice, not the most efficient, but we don't need that there.
  
  Meanwhile, my decision to move away from WPF for the sake of my sanity (and some potential Linux happiness for others alongside) means I've got two options today: WebView2 (which is Microsoft/Windows only and their "embedded browser component" done on top of Edge, which is just Chromium with Microsoft Sauce) **or** CEF.
  Either you noticed before in that CEF link above or know this already: CEF is C++, so there's that yet unanswered question of CEF=C++, but we need to get to C#, so now what? Classical P/Invoke like Qiqqa is already using in places? With the attached DLL/SO Module Hell now revamped with added *cross platform woes*?...
  
  That last bit can be "solved" by using a C# "wrapper" for CEF and there's several: CEFSharp (Windows only), CEFGlue (Windows, Linux), Xilium (Mac? Linux. Windows) being the top 3 today. Xilium was right out because it kept barfing hairballs while I stress-tested it on my box. CEFSharp is the leanest, but is not exactly ideal when I want to include Linuxes. So CEFGlue? It's tracking CEF (which proceeds at a murdering pace as CEF = Chromium and tracks every Chrome web browser release: Chrome = Chromium with Google Sauce, BTW).
  CEFGlue has had its weak periods in the last couple of years in terms of developer activity, and again I'ld like to be the only slow one on the block, if you please. *Besides*, going through CEFGlue adds a bit of a ho-hum-how-to-do-this trouble where that Qiqqa Sniffer functionality is concerned.
  
  How about... taking CEF as-is, then picking up the Chromely idea of a *extremely lean* platform-specific UI driver wrapped around it, and mix that with a bit of C++ code to get those Qiqqa Sniffer basics in there so I can reach for it from any "business logic" process via local IPC (Inter-Process Communications - think: "talking to a web server *locally*, maybe using JSON or whatever for data exchange").
  Sounds doable to me. Then "Linux" would be a reality when that UI wrapper is recoded for Linux, and the rest of the stuff (the other processes) should already have been done in reasonably portable code anyway. Total sum: some effort required per platform but no crazy per-platform codebases horror.

## Getting to the meat of the communications here...

All of this then brings us to the remaining question: how do these buggers *talk* to one another? And who will need to talk to whom, by the by?

What are the big data flows in there?

Well, the biggest one will, of course, be the PDF page images: while some folks have come up with pdf.js, i.e. a pdf renderer in JavaScript, hence one that can run right inside CEF, it's a bit slow and more importantly: do you recall the issue we had with **today's Qiqqa**? The two-PDF-engines-used-for-different-yet-overlapping-purposes: SORAX vs. `mudraw -tt`? Iff I were to go with pdf.js, I'ld be re-introducing that problem, just shifted to another place and more trouble then it's worth.

The alternative to this is treating our CEF UI as the browser it is and use it to display PDF page images as part of the UI it's supposed to display, just like Qiqqa does today in WPF/.NET: then we use that mupdf backend process for image rendering and we need to get those images into CEF for display, *pronto*.
Basic answer: make that mupdf+everything C/C++ process a web server and you're almost *done*: it can cough up images as any regular "webserver" would and CEF=QiqqaUI now can do a basic HTML page with images: the PDF pages! Place a HTML DIV over those images and you can do annotation editing layers, etc. as usual. (Compare to Elsevier's website's PDF viewer: *they* do image rendering in their server (because for *them*, that's the optimum way from keeping geeky/nerdy-you from inadvertently grabbing the PDF originals from behind their paywall by "sniffing" web browser traffic! 😜 ) and then they go and draw layers of stuff on top of those PDFs. Oh, Elsevier is not the only one. There's plenty sites doing it this way, and all of *them* have the added cost of added network latency to get the page images to you, so this idea is certainly *doable*. The only (minor?) drawback being your machine having to work a little harder because every page is encoded as a PNG or other image format before traveling to yor "browser" and the same will happen with our CEF plus backend processes approach, so there's the added cost of image compression and decompression  per page.


So let's analyze:

- who needs to produce/receive/see those images in the new Qiqqa processes universe?

  `mupdf` et al will be the producer. (C/C++ "local webserver") CEF UI will be the receiver. (HTML/JavaScript; the CEF-specific C++ code will *see* the image pass through, but does not need to work on it, simply observe and allow through. *Reception* will be handled by CEF itself, as that one is acting webbrowser here.
  
  So that would be C/C++ producing web-compatible image files, make them available at given URLs in the "webserver" and the rest is *done* (CEF=webbrowser).
  
- Then there are the text overlays for text marking, copy & paste, etc. in the UI: again, this stuff is produced by `mupdf` et al, so in that C/C++ "webserver" process, can *probably* be spit out as HTML or JSON or whatever the JavaScript "website code" == Qiqqa UI needs to draw that DIV overlay for you.

  So that would be C/C++ to JavaScript and data will be hOCR or functionally comparable formats. hOCR can be *huge* if you want precise positioning info per character, so 
  
  1. *can* we come up with a leaner and thus *faster* format?
  2. *should* we do that (because it needs to be expanded and postprocessed by the UI = JavaScript "website code")
  3. *should* we use the same or a different format for the *cache storage* which we use today and in the future to help reduce CPU load by only extracting that PDF text content *once*!?
 
  Gut feeling says 1=yes, 2=probably-yes-anyway-to-reduce-transfer-costs-in-local-comms and 3=😱I-dont-want-to-eat-gigabytes-if-I-dont-have-to. (Which we will discuss in a bit.)
  
What other comms do we have? Ah yes, the search queries and their results: that's CEF=UI (JavaScript) talking directly to our SOLR instance (Java+JSON). Now **unless we wish to spend time in customizing filters in SOLR**, we're stuck with either JSON or XML for this and I much prefer JSON, so we'll stick with JSON for JavaScript to Java and vice-versa talks.
  
SOLR can't do anything if it isn't fed, so there's an additional (bulk?) feed from the C/C++ "webserver", which is responsible for all PDF text extraction work, to SOLR, getting those extracted texts in there for *indexing*. Given the Java+JSON restriction, that's our C/C++ "webserver" talking JSON over the local IPC wire to Java (SOLR). That'll be fine. No problems expected here.
  
Ditto for *metadata*: the `mupdf` process has to get it from the PDFs, then feed to to SOLR (in bulk in the background) and/or Qiqqa UI upon request (due to user action).
  
How about our Qiqqa "business logic" process? Doesn't look like much anymore... Hold that! Because first we need to consider the Qiqqa Sniffer functionality!
  
Qiqqa Sniffer is, on the one side (UI), CEEF (= JavaScript) playing together with that "lean and mean UI wrapper done in C++ around it" to access websites like Scholar and Bing Academics, where the C++ bit must come to our aid to hook into the network (and web*browser* cache) traffic while each web page is rendered: while you view that Scholar page, we need to watch for you doing any clicking to download/**fetch** PDFs and we MUST hook into that network traffic (like we do with xulrunner in Qiqqa *today*) to be *first* to recognize the download/fetch occurring and grabbing the PDF copy as it comes in, storing it in the `/documents/` folder and feeding it to the PDF processes i.e. `mupdf et al`: that will be CEF+C++-wrapper sending big binary (PDF) stuff **to our local "mupdf webserver"**: that would then obviously become a kind of "file upload" from the perspective of that mupdf-et-al "webserver" of ours. Perfectly doable: we already have a webserver framework selected that supports POST & PUT and binary data uploads, so we'll do fine there.
  
This would then have that clicked=downloaded PDF end up at our `mupdf` instance, which can do the inspection (sometimes downloads fail! Or grab very evil stuff!) and then store it in your library -- including writing a record in the database to register the new item, while we're at it: that's a good reason to have SQLite in the `mupdf et al` "webserver": it's C/C++ code and all we need to do is transport a few basic queries from C# to C/C++. Not the end of the world in technical challenges. 😄
  
But the Qiqqa Sniffer does *more*: it also grabs ("scrapes") BibTeX and other metadata formats for us off Scholar and other websites, which carry such data more or less obviously: **today's Qiqqa** only grabs the *obvious* metadata records coming through the wire, but we *could* at some point allow people to tweak this a little so we can do more *scraping*, i.e. use a bit of smarts and software to "scrape" the metadata for an item off the less obvious places.
 
The way *today's Qiqqa* supports this behaviour is the added ability to select and copy bits of text (e.g. Author names) off a web page in the Sniffer and plonk it in the proper place in the metadata entry/edit/review form that's part of its UI: *here* is the reason why we need **two CEF instances together**: one can do the web browsing for us (dangerous!), while the other does the nice local UI stuff, like showing the PDF page we want to look at while we search, plus the metadata entry form at the right edge, plus a few more buttons to click on for text select, copy and paste, *plus* the ubiquitous PDF page overlay for selecting and copying text from the PDF page itself, to be fed as **search query** to the web browsing bottom part of the Qiqqa Sniffer.
The way to accomplish all this would be mostly JavaScript "site code" in one CEF instance, but we cannot easily inject that stuff in the second, web browsing CEF instance, from there, unless we add a few hooks and a bit of data/message transferring in the C++ wrapper, turning a request by user click in one CEF into a command/action for the other CEF instance: message passing via specific hooks into the second browser's processes. I expect the amount of C++ code required here to be minimal and besides: CEF itself is cross-platform portable, so I don't expect a lot of hassle from those bits of C++ code helping us to make the Sniffer happen, when we transport that code to another platform and rebuild a binary application from it: those chunks of C++ are all about talking to and from CEF internal hooks and none of it would/should be bothering itself with anything like a *UI*: that's what we have the second CEF instance for, so when there's anything to display in the UI, the C++ code there would accomplish this by sending a *message* to the "website JavaScript code" that drives our UI/GUI in that CEF instance. Then that JS code will take care of the display or user data entry work as requested, thus forming our UI/UX.
  
Since we already placed the (SQLite) database in the same process as our `mupdf et al` "webserver", all user-located, edited and -collected metadata is then received in the UI (JavaScript) and should be sent to that "webserver" so it can store it in the database: yet another query to pick from the C# code and replicate in the new C/C++ code around SQLite there. Meanwhile, this type of action would be exactly the same as "entering some form data in a webpage" as far as the UI/CEF would be concerned and the `mupdf et al` process *is* a webserver, so that'ld be a regular POST action as usual for web servers.
  
So what is left then?
  
Convenience work and "miscellaneous" for the C# process: it can act as another "Webserver" if we want to talk to it from the UI, and it can act a a "web client" (i.e. just like a web browser), when it needs to talk to the SOLR or `mupdf et al` "webservers". What it'll do depends on *choices* really: some stuff is not easily done in a C/C++ programming environment and my be handled better and swifter in either JavaScript or C#. The latter would mean it'ld run in "business logic process".
  
How about keyword extraction (LDA); the Expedition logic, etc.? I would park LDA (keyword extraction) in the C++ "webserver" because it's very CPU intensive and C++ might be slightly better suitable for this "AI stuff" in the end -- as we keep in mind, yet haven't mentioned this, that we want to "open up Qiqqa" by providing a few hooks into a script language (Python or JavaScript) where users can do their own custom thing with the Qiqqa data: they'll need database and "extracted text cache" access for that, so logically they'll need to talk to the `mupdf et al` "webserver": it already being a "webserver" would ideally *spare us from having to incorporate a scripting language* for all this and thus save time & effort for other subjects, like getting this new Qiqqa architecture on the road and *rolling*. Then anyone can come with their own favorite hack-it script and talk "web" to the qiqqa components, including SOLR! That would be very nice! And I would personally be one of those users, because there is stuff that would be nice to have in Qiqqa but is really *niche*, e.g. PDF postprocessing to clean up PDFs, strip off useless banner pages, etc. and analyze the PDFs to find or sorts of "duplicates": with quotation marks, as they would be **near duplicates** really: re-publication of the same article in another magazine (link those PDFs together, share metadata?), re-issue or re-print after corrections or because there's some anniversary or something (think arXiv and *versions*: you want to keep the versions you have, but be able to pick a specific version you want **this time**; then there's PDFs which are published as pre-prints, thus differing by yet another editing process; PDFs which are "part of": either PDFs, each a chapter, and you want the *book*, i.e. the *collection*, *or* you got a Symposium PDF or *magazine* and you want a specific article in there: how about extracting ti from there? Or *at least* telling Qiqqa where and how to extract?
Then there are the repaired, deprotected, OCRed, spellchecked, revised, edited, augmented, etc. PDF copies which are not exactly copies but still part of the same *family*.
  
Then there's all the annotation work done by folks: do you want to extract the annotations and discard the PDF to have it regenerated later (with the risk of not exactly rendering the annotation like it was before) **or** do you want to keep the annotations in the PDF exactly as-is, because they are from someone else, e.g. you're doing or receiving peer-review feedback? From a librarian's perspective, that would mean keeping multiple PDFs in the database, linking them together to ensure the relationship (or **relationships:PLURAL**) is kept for posterity. Multiple rounds of annotating would them become a sequence of *versions* a la arXiv: every time you import a freshly annotated/updated PDF, it is stored as is and linked to an existing one. BUT we don;t know what you did, so we re-execute all the annotation and text extraction analysis again **in the background** while we SHOULD have agreed on some sort of **protocol to announce the relation to an existing PDF to Qiqqa**: I'm thinking about adding/augmenting the (optional) XMP record in a PDF for that: XMP is XML data and rather well suited for this, while doing "magic annotations" etc. are far more user-visible and thus editable and thus **potentially lost between imports due to inadvertent user editing** losing us such 'link tags'. Fewer peeps go and edit their XMP records, so that should be fine and makes for a perfectly legal PDF if we edit or *add* that record.
  
Of course, considering storing and recording all those potential "near duplicate copies" is begging for a thought about storage size, but I'm confident I'll come up with some sort of "delta compression" if the need get severe. And there's always "the bigger harddisk": the real big problem I have right now is my metadata export dumps from `mupdf et al` and the text extract dump can become **huge** in their current forms, so I need to come up with something smarter there *pronto*.
  
  
## Compression and column storage thoughts...
  
For the text extract (hOCR-like!) cache format (and possibly for sending it across the local (machine-internal) wire too), I started with hOCR itself, which is nice and easy as it immediately renders in a web browser as it's basically HTML with a lot of invisible data added.
  
However, it may be smart to use a different storage format for the caches to enable tighter *compression*, plus a few tools to convert to/from the hOCR format so everyone can easily access those text extract caches without the need to know the ins and outs of a particular binary format.
  
### Column storage anyone?
  
When you look at the STEXT and hOCR dumps (and the text cache format as described for *today's Qiqqa* further above), then a few things immediately draw the eye:
  
- current `mupdf` applies "heuristics" to combine characters into words, just a bit like like today's QiqqaOCR does, to produce coordinate bboxes per word or even set of words, applying the implicit assumption that selection marking is good enough when all characters are assumed of equal length **and**, more importantly, assuming that these heuristics help 'extract' the actual words in the text so we can help SOLR/Lucene construct a high qulity search database.
  
  Suffice to say the heuristic (particularly the one used by Qiqqa) isn't up to snuff and potentially reduces output quality that way for *some* PDFs.
    
- the alternative of *not* using these heuristics and spitting out bboxes per characters is supported by `mupdf` as STEXT format, which is a gargantuan XML for quite a few PDFs: don't be surprised to get a 10-fold size expansion compared to the original PDF, even when that one was NOT compressed.
  
- while we prefer the STEXT format for its *precision*, we need to consider the storage and IPC transmission costs, even on localhost-only: sending 100+ MegaBytes to another process in your computer is not nearly *instantaneous*, even when you have a very fast rig and it is all happening locally-only.

- all characters have a bbox at least: `x0,y0,x1,y1`.

- most characters also include their *baseline*: `yb`

- some characters produce *quads*, e.g. some TeX versions I've seen produce quads when a superscript character is next: that's a parallelogram or 2 bboxes in coordinate cost: x0,y0,x1,x1,x2,y2,x3,y3

- most yN coordinate values stay constant while a *ream* of text is dumped: after all, all those character boxes sit on the same ridge, hence share common y0 and y1 coordinates.

- meanwhile, the x coordinates slowly increase (LTR IndoEuropean languages all) or decrease (Arabic), begging for a delta compressor to reduce storage cost.

I have not yet looked at the relative step size in x coordinate per character: one *could assume* a paper is mostly published in one font and size, or maybe a couple of sizes, resulting in a fixed ratio for the character **width** between different characters in a font!

Ditto for line height: the moment an y coordinate "jumps", we're very probably looking at a line break and those are pretty regular as well.

(Of course all kinds of vector graphics with text elements cause havoc with these assumptions, but so they do with OCR and **text extraction** activity, so even then we would need a good **segmenter** to heuristically detect the *figures*, *charts* and *schemas* in a publication and extract *those* separately: it would make for better content feeding into the SOLR instance and we could add support for rendering these differently when the user requests qiqqa to produce yet another PDF *version*: the PDF copy with embedded (OCR) text and optional annotation layers).

For the coordinates it would be good to look at storing these in column mode: an array for x0, x1, y0, y1, then a 5th array for *node* *type* asnd a 6th for *node content*, e.g. a bit of text.
*Quads* could then be encoded as dual entries, thus amassing 8 coordinate slots. Further "smart" MAY be employed to split those coordinate columns into ones for *relative* values and ones for *absolute* values, so that a coordinate can either be absolute (e.g. start of line) or relative (next character in a word).

Yet another potential compressible optimization wight be analyzing the text, detecting the default width for each character in the alphabet and then auto-adjusting the "delta" for the default width: all characters that fit the norm need no x-step correction (delta=zero!) as the width for that character applies and the scaling for the line or paragraph together determine what the given coordinate SHOULD HAVE BEEN.

Anyway, enough ideas to try here.

The hard part is getting that format available to multiple languages: we can do it easily in C/C++ (`mupdf et al`), but we might need access to it in the C# business logic layer and surely we will want to look at in the JavaScript frontend (CEF UI) as we need to render this stuff for select & copy+paste, plus the built-in annotation editor: mark and select, etc.

We've looked at flatbuffers, bebop and a couple of others (don't get me started on google protocolbuffers!) but I don't need *versions* of this stuff, nor do I want tags to mingle with the data as I was specifically looking for columnar storage ways to help optimize the compression processes already, so we'll have to roll our own I guess.

What I *need* is a binary format that can be read in JavaScript, C# and C/C++. Java doesn't play a role AFAICT because SOLR will never receive this data *raw*, so we only need to consider those 3 languages. While flatbuffer et al do provide an answer, it's far from a good solution in my opinion.

After a long journey, I've landed at SWIG: this tool is available for both Windows and Linux (only Qiqqa *developers* will need it!) and what it does is gobble a header file and spit out the language of choice. C, C# and JavaScript are all supported, contrary to, say, CppSharp from Mono, which would otherwise have served AFACT.

So SWIG it is: we can use that one for all regular data transfers between our processes and for storing (large) cache data files.

#### Does it hurt when the format changes? Endianess? etc...

Frankly, I don't care. When we do a format update, we VERY probably have a big tool update as well and the document library is best served then by having it re-enter the entire text extraction, etc. process from scratch once again so all old bugs go away as we overwrite the results with our new & improved proceedings.

Endianess is also not a problem as we don't expect to share these files across computers: not even Qiqqa Sync will touch them and have them regenerated on every node you run Qiqqa on, so endianess is always *good*, as there'll be only a single node looking at those files anyway.

Next: transfer and storage costs? We *expect* to benefit from storing binary data raw, both in storage and in save- and load-parsing costs, and it looks like the C# loads at least can be done relatively cheap if we follow our column-based I/O idea: load a big array of floating point, then another, etc. can be done pretty quickly in C#/.NET so I don't bother about performance there.

The added cost is in the need to produce two *converter tools*: one for reading the format and producing something standard and legible, the other *encoding* to the binary format so we can round-trip data like that. Then the encoder needs to be included in `mupdf et al` as another pdf device so mupdf can produce these files for us.




  
  
  
 
  
  




  
 

http://swig.org/Doc4.0/SWIGDocumentation.html#Introduction
