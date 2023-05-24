# Specific Tools and Tasks to Research and Produce for Cross-platform Qiqqa

## Research = Viability, First Test Version

### Prerequisites: selected technologies

- the core back-end work (PDF rendering, text extraction, OCR, SQL metadata base processing) is done in C/C++. This uses:
	- Artifex' mupdf (forked & augmented)
	- Tesseract
	- cURL
	- SQLite (also for backwards compatibility reasons: we stick with SQLite! And we'll very probably are going to use it *more* than *less*.)
	- leptonica
	- libxml2 (or libexpat if libxml2 doesn't mesh well with the other bits around XMP, etc.)
	- Adobe XMP SDK?
	- OpenCV (once we introduce [[scripting the OCR + text extraction process]])
	- libWebP -- to be decided based on caching sizes and render bitmap transfer performance, which impacts **PDF reader performance**. Would be great for the caches, though.
	- libjpeg / libjpeg-turbo / libjpeg-xl (lossless JPEG & other 21st century updates to the JPEG standard)
	- libpng
	- libzlib
	- crow (for the local webserver core)
	- QuickJS, once we introduce [[scripting the OCR + text extraction process]] (faster and supporting modern JS idiom (ES2017), contrary to `mujs` which is ES5. CPython has been considered, but turned out to be a very rough animal to compile on Windows at least.)
- the new UI front-end will be quite similar to a web browser, really. Due to special needs (Qiqqa Sniffer, mostly) and security reasons (*probably safe* library content vs. *probably un-safe* on-line browsing while you look for additional info, such as related publications and document metadata to import/scrape) we still will need a bit of a wrapper as we'll be housing **two** web viewer instances at least: one for *outside world accesses*, one for **local access**.

   The UI (cross-platform) will be mostly web-technologies-based, so:
	1. we don't have to worry about porting the intricacies of an **advanced, complex, UI** to multiple major platforms (Windows, Linux, Apple)
	2. **we can use different, and more *available*, skill sets for important sections of the Qiqqa application development process: you don't need to be a C#/C/C++/WPF/etc. *unicorn* to do important UI/UX work: this is now quite similar to web development.** 
	3. giving the application a fresh layer of paint, or doing stuff like ergonomic "dark mode", is now almost entirely relegated to the web technology layer, making it less costly in the long run to do UI 'cosmetic' changes. (CSS *theme*, anyone?)

   > And *that* is why it's important that the embedded browser fully supports F12 Developer Mode debugging, as do the major brand browsers themselves: Qiqqa sniffer is a complex animal today, thanks to Google search engine engineers being very counter-productive.

   The new front-end is envisioned as a C++-based wrapper, because that one has the tightest, most elaborate interface to the embedded browser instances, adds a *minimal* bit of UI itself and delegates the main job to the web browser instances.
   
   It uses:
   
 	- wxWidgets (née wxWindows): shouldn't go wild with this one as most of the UI can be done as embedded web pages: we need that full-fledged web browser in there *anyway*.)
   
	- CEF (Chromium Embedded Framework) -- CEFGlue/CEFSharp/Xilium/Chromely have been considered, but ultimately, those add just another layer of bug & maintenance risk. Not that we'll be better at it, but when we can do the interface in a relatively *simple* way (the Qiqqa Sniffer has the highest demands on the CEF API!) in C++, then we're good from my perspective, also cross-platform portability-wise.
- the Full Text Search Qiqqa Core (i.e. Qiqqa Content&Metadata Search) uses industry-leading Lucene (which is Java code), through a 'web interface' (SOLR as local server) so:
	1. we don't have to bother with coding any Java to make it happen
	2. users can invent their own smart ideas accessing and using the collected metadata: we are opening up Qiqqa!

- What's left for the old C# code-base then, you ask?

   WPF/.NET is **very** *non*-portable so most of it will have to go. What we untangle and isn't replaced by Core Technologies (see above: LuceneNET is out and so will be a couple of other libs and tools) will stay on a "useful middle-ware" which will be command-line / socket-interface based so **not facing users directly, i.e. no need for any GUI work any more**: all that should be handled by the new web-based GUI above.
   
   This part will then become cross-platform portable as the remains would fit the bill of C#/.NET.Core which is touted as cross-platform able *today*. We'll see what remains as "middle-ware" as we work on the other components and discover its relative usefulness along the way. I expect all sorts of import/export jobs will stay in C#/.NET.Core as there's little benefit to re-coding those processes in a otherwise-critical Core Server component, which already does the PDF and Library Database handling then.

### Demarcated Projects = producing a Tool or a Procedure To Use A Tool:

- Qiqqa GUI
	- [[wxWidgets + CEF for UI - or should we go electron anyway⁈ ⇒ WebView2 et al|wxwindows + CEF for UI - or should we go `electron` anyway?]]
	- [[The Qiqqa Sniffer UI-UX - PDF Viewer, Metadata Editor + Web Browser As WWW Search Engine|The Qiqqa Sniffer UI / UX: PDF Viewer, Metadata Editor + Web Browser As WWW Search Engine]]
	- [[Web UI/visual BibTex Editor|Web UI: visual BibTex Editor]]
	- [[Web UI/visual Metadata Editor|Web UI: visual Metadata Editor (for abstract, title, DOI, **tags**, etc.)]]
	- [[Web UI/visual Tags, Ratings, Categories Editor|Web UI: visual Tags, Ratings, Categories, ... Editor]]
	- [[Web UI/library document list view|Web UI: library document list view & selection / filtering]]
	- [[Web UI/library document list view performance|Web UI: library document list view *performance*: Virtual Table technology for scrolling & jumping through 10K+ entries per library]]
	- [[Web UI/Filtering as Search Aid|Web UI: Filtering as Search Aid: filter on title, tag(s), you-name-it...]]
	- [[Web UI/Qiqqa Expedition, now done in web tech|Web UI: Qiqqa Expedition, now done in web tech: D3, ...]]
- Qiqqa Core Local Server
	- [[Anything PDF and SQLite happens on a local server|Anything PDF and SQL(SQLite) happens on a local server]]
	- Qiqqa GUI Assist: PDF rendering
		- [[PDF page image rendering for web frontend|PDF page image rendering for (web) frontend]]
		- [[PDF rendering performance - caching at the local server|PDF page rendering performance: *smart caching* of documents and their rendered page bitmaps to save work during the session]]
	- PDF Automated (Bulk) Processing
		- [[Importing PDFs]]
		- [[Qiqqa Watch Folders/now done by the core backend|Qiqqa Watch Folders: now done by the core backend]]
		- [[Cleaning PDFs|Cleaning PDFs: removing obnoxious banner pages, readying for OCR when deemed necessary]]
		- [[Importing PDFs with annotations]]
		- [[Extracting Annotations as Metadata]]
		- [[Exporting PDFs for External Readers|Exporting PDFs for External Viewers / Readers / Editors]]
		- [[Importing PDF Revisions|Importing PDF Revisions (Externally Edited and Resubmitted into Qiqqa)]]
		- [[Processing other document types/HTML|Processing other document types: HTML]]
	- Text Extraction
		- [[Text Extraction/Processing regular PDFs|Text Extraction: processing regular PDFs]]
		- [[Text Extraction/Processing obnoxious PDFs (forced) OCR|Text Extraction: processing obnoxious PDFs, i.e. (forced) OCR via tesseract]]
		- [[Text Extraction/Cleaning Up the Extracted Content|Text Extraction: Cleaning Up the Extracted Content]]
	- Metadata Extraction
		- [[Metadata Extraction/Obtaining Author, Title, DOI, etc|Metadata Extraction: Obtaining Author, Title, DOI, etc. from the PDF XMP Data]]
		- [[Inferring Author, Title, DOI, etc from Content|Metadata Extraction: *Infering* Author, Title, DOI, etc. from the PDF Text Content]]
	- Sniffer Assist: Downloading (PDF & HTML) Documents
		- [[Using embedded cURL to download PDF or HTML document at URL]]
		- [[Using embedded cURL to obtain Metadata for a document]]
		- [[Web REST API for PDF and SQLite - metadata IO|"Web API" (REST?) for PDF and SQLite: fetching metadata for user viewing and (user/manual) editing: *updating* metadata]]
	- Sniffer Assist: Scraping Metadata?
	- Sniffer Assist: Google Scholar bulk processing (under user guidance and assistance)
	- Core Data Backup / Sync / Import / Export
		- [[Exporting PDFs with metadata embedded (XMP)]]
		- [[Exporting PDFs with metadata + annotations embedded]]
		- [[Core - Exporting Metadata to XYZ|Exporting Metadata to {Fill In Application Name / File format}]]
		- [[Core - Importing Metadata from XYZ|Importing Metadata from {Fill in Application Name / File Format}]]
- Qiqqa FTS (Full Text Search) Engine (~ Lucene/SOLR)
	- [[Automatic Setup and Running of a local SOLR instance]]
	- [[Feeding data and metadata to SOLR]]
	- [[Making SOLR highlighting work for us|Making SOLR highlighting work for us :: feeding the results to the Qiqqa user]]
	- [[For Users/Example How To Use The SOLR Intance Without Qiqqa UI FrontEnd|For Users :: Example How To Use The SOLR Instance Without Qiqqa UI FrontEnd = scripting and using for your own custom purposes]]
- Qiqqa Middleware (Sync, Import, Export?)
 
   "Middleware": where we keep the old C#/.NET code. Handy tooling on top of the Core. Aiding and abetting with the new *web-technologies*-based UI.
	- [[MiddleWare - Exporting Metadata to XYZ|Exporting Metadata to {Fill In Application Name / File format}]]
	- [[MiddleWare - Importing Metadata from XYZ|Importing Metadata from {Fill in Application Name / File Format}]]
- 