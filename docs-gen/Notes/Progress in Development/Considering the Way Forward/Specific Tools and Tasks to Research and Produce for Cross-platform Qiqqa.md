# Specific Tools and Tasks to Research and Produce for Cross-platform Qiqqa

## Research = Viability, First Test Version

### Prerequisites: selected technologies

* the core backend work (PDF rendering, text extraction, OCR, SQL metadata base processing) is done in C/C++. This uses:
  
  * Artifex' mupdf (forked & augmented)
  * Tesseract
  * cURL
  * SQLite (also for backwards compatibility reasons: we stick with SQLite! And we'll very probably are going to use it *more* than *less*.)
  * leptonica
  * libxml2 (or libexpat if libxml2 doesn't mesh well with the other bits around XMP, etc.)
  * Adobe XMP SDK?
  * OpenCV (once we introduce [scripting the OCR + text extraction process](Document%20OCR%20&%20Text%20Extraction/scripting%20the%20OCR%20+%20text%20extraction%20process.md))
  * libWebP -- to be decided based on caching sizes and render bitmap transfer performance, which impacts **PDF reader performance**. Would be great for the caches, though.
  * libjpeg / libjpeg-turbo / libjpeg-xl (lossless JPEG & other 21st century updates to the JPEG standard)
  * libpng
  * libzlib
  * crow (for the local webserver core)
  * QuickJS, once we introduce [scripting the OCR + text extraction process](Document%20OCR%20&%20Text%20Extraction/scripting%20the%20OCR%20+%20text%20extraction%20process.md) (faster and supporting modern JS idiom (ES2017), contrary to `mujs` which is ES5. CPython has been considered, but turned out to be a very rough animal to compile on Windows at least.)
* the new UI frontend will be quite similar to a web browser, really. Due to special needs (Qiqqa Sniffer, mostly) and security reasons (*probably safe* library content vs. *probably un-safe* on-line browsing while you look for additional info, such as related publications and document metadata to import/scrape) we still will need a bit of a wrapper as we'll be housing **two** web viewer instances at least: one for *outside world accesses*, one for **local access**.
  
  The UI (cross-platform) will be mostly web-technologies-based, so:
  
  1. we don't have to worry about porting the intricacies of an **advanced, complex, UI** to multiple major platforms (Windows, Linux, Apple)
  1. **we can use different, and more *available*, skill sets for important sections of the Qiqqa application development process: you don't need to be a C#/C/C++/WPF/etc. *unicorn* to do important UI/UX work: this is now quite similar to web development.** 
  1. giving the application a fresh layer of paint, or doing stuff like ergonomic "dark mode", is now almost entirely relegated to the web technology layer, making it less costly in the long run to do UI 'cosmetic' changes. (CSS *theme*, anyone?)
   > 
   > And *that* is why it's important that the embedded browser fully supports F12 Developer Mode debugging, as do the major brand browsers themselves: Qiqqa sniffer is a complex animal today, thanks to Google search engine engineers being very counter-productive.
  
  The new front-end is envisioned as a C++-based wrapper, because that one has the tightest, most elaborate interface to the embedded browser instances, adds a *minimal* bit of UI itself and delegates the main job to the web browser instances.
  
  It uses:
  
  * wxWidgets (née wxWindows): shouldn't go wild with this one as most of the UI can be done as embedded web pages: we need that full-fledged web browser in there *anyway*.)
  
  * CEF (Chromium Embedded Framework) -- CEFGlue/CEFSharp/Xilium/Chromely have been considered, but ultimately, those add just another layer of bug & maintenance risk. Not that we'll be better at it, but when we can do the interface in a relatively *simple* way (the Qiqqa Sniffer has the highest demands on the CEF API!) in C++, then we're good from my perspective, also cross-platform portability-wise.

* the Full Text Search Qiqqa Core (i.e. Qiqqa Content&Metadata Search) uses industry-leading Lucene (which is Java code), through a 'web interface' (SOLR as local server) so:
  
  1. we don't have to bother with coding any Java to make it happen
  1. users can invent their own smart ideas accessing and using the collected metadata: we are opening up Qiqqa!
* What's left for the old C# codebase then, you ask?
  
  WPF/.NET is **very** *non*-portable so most of it will have to go. What we untangle and isn't replaced by Core Technologies (see above: LuceneNET is out and so will be a couple of other libs and tools) will stay on a "useful middleware" which will be commandline / socket-interface based so **not facing users directly, i.e. no need for any GUI work any more**: all that should be handled by the new web-based GUI above.
  
  This part will then become cross-platform portable as the remains would fit the bill of C#/.NET.Core which is touted as cross-platform able *today*. We'll see what remains as "middleware" as we work on the other components and discover its relative usefulness along the way. I expect all sorts of import/export jobs will stay in C#/.NET.Core as there's little benefit to re-coding those processes in a otherwise-critical Core Server component, which already does the PDF and Library Database handling then.

### Demarcated Projects = producing a Tool or a Procedure To Use A Tool:

* Qiqqa GUI
  
  * \[\[wxWidgets + CEF for UI - or should we go electron anyway⁈ ⇒ WebView2 et al|wxwindows + CEF for UI - or should we go `electron` anyway?\]\]
  * [The Qiqqa Sniffer UI / UX: PDF Viewer, Metadata Editor + Web Browser As WWW Search Engine](The%20Qiqqa%20Sniffer%20UI-UX%20-%20PDF%20Viewer,%20Metadata%20Editor%20+%20Web%20Browser%20As%20WWW%20Search%20Engine.md)
  * [Web UI: visual BibTex Editor](../../Web%20UI/visual%20BibTex%20Editor.md)
  * \[\[Web UI/visual Metadata Editor|Web UI: visual Metadata Editor (for abstract, title, DOI, **tags**, etc.)\]\]
  * [Web UI: visual Tags, Ratings, Categories, ... Editor](../../Web%20UI/visual%20Tags,%20Ratings,%20Categories%20Editor.md)
  * [Web UI: library document list view & selection / filtering](../../Web%20UI/library%20document%20list%20view.md)
  * \[\[Web UI/library document list view performance|Web UI: library document list view *performance*: Virtual Table technology for scrolling & jumping through 10K+ entries per library\]\]
  * [Web UI: Filtering as Search Aid: filter on title, tag(s), you-name-it...](../../Web%20UI/Filtering%20as%20Search%20Aid.md)
  * [Web UI: Qiqqa Expedition, now done in web tech: D3, ...](../../Web%20UI/Qiqqa%20Expedition,%20now%20done%20in%20web%20tech.md)
* Qiqqa Core Local Server
  
  * [Anything PDF and SQL(SQLite) happens on a local server](Anything%20PDF%20and%20SQLite%20happens%20on%20a%20local%20server.md)
  * Qiqqa GUI Assist: PDF rendering
    * [PDF page image rendering for (web) frontend](PDF%20page%20image%20rendering%20for%20web%20frontend.md)
    * \[\[PDF rendering performance - caching at the local server|PDF page rendering performance: *smart caching* of documents and their rendered page bitmaps to save work during the session\]\]
  * PDF Automated (Bulk) Processing
    * [Importing PDFs](Importing%20PDFs.md)
    * [Qiqqa Watch Folders: now done by the core backend](../../Qiqqa%20Watch%20Folders/now%20done%20by%20the%20core%20backend.md)
    * [Cleaning PDFs: removing obnoxious banner pages, readying for OCR when deemed necessary](Cleaning%20PDFs.md)
    * [Importing PDFs with annotations](Importing%20PDFs%20with%20annotations.md)
    * [Extracting Annotations as Metadata](Annotating%20Documents/Extracting%20Annotations%20as%20Metadata.md)
    * [Exporting PDFs for External Viewers / Readers / Editors](Exporting%20PDFs%20for%20External%20Readers.md)
    * [Importing PDF Revisions (Externally Edited and Resubmitted into Qiqqa)](Importing%20PDF%20Revisions.md)
    * [Processing other document types: HTML](../../Processing%20other%20document%20types/HTML.md)
  * Text Extraction
    * [Text Extraction: processing regular PDFs](../../Text%20Extraction/Processing%20regular%20PDFs.md)
    * [Text Extraction: processing obnoxious PDFs, i.e. (forced) OCR via tesseract](../../Text%20Extraction/Processing%20obnoxious%20PDFs%20%28forced%29%20OCR.md)
    * [Text Extraction: Cleaning Up the Extracted Content](../../Text%20Extraction/Cleaning%20Up%20the%20Extracted%20Content.md)
  * Metadata Extraction
    * [Metadata Extraction: Obtaining Author, Title, DOI, etc. from the PDF XMP Data](../../Metadata%20Extraction/Obtaining%20Author,%20Title,%20DOI,%20etc.md)
    * \[\[Inferring Author, Title, DOI, etc from Content|Metadata Extraction: *Infering* Author, Title, DOI, etc. from the PDF Text Content\]\]
  * Sniffer Assist: Downloading (PDF & HTML) Documents
    * [Using embedded cURL to download PDF or HTML document at URL](Using%20embedded%20cURL%20to%20download%20PDF%20or%20HTML%20document%20at%20URL.md)
    * [Using embedded cURL to obtain Metadata for a document](Using%20embedded%20cURL%20to%20obtain%20Metadata%20for%20a%20document.md)
    * \[\[Web REST API for PDF and SQLite - metadata IO|"Web API" (REST?) for PDF and SQLite: fetching metadata for user viewing and (user/manual) editing: *updating* metadata\]\]
  * Sniffer Assist: Scraping Metadata?
  * Sniffer Assist: Google Scholar bulk processing (under user guidance and assistance)
  * Core Data Backup / Sync / Import / Export
    * [Exporting PDFs with metadata embedded (XMP)](Exporting%20PDFs%20with%20metadata%20embedded%20%28XMP%29.md)
    * [Exporting PDFs with metadata + annotations embedded](Exporting%20PDFs%20with%20metadata%20+%20annotations%20embedded.md)
    * [Exporting Metadata to {Fill In Application Name / File format}](Core%20-%20Exporting%20Metadata%20to%20XYZ.md)
    * [Importing Metadata from {Fill in Application Name / File Format}](Core%20-%20Importing%20Metadata%20from%20XYZ.md)
* Qiqqa FTS (Full Text Search) Engine (~ Lucene/SOLR)
  
  * [Automatic Setup and Running of a local SOLR instance](Full%20Text%20Search%20-%20Exit%20Lucene.NET,%20Enter%20SOLR/Automatic%20Setup%20and%20Running%20of%20a%20local%20SOLR%20instance.md)
  * [Feeding data and metadata to SOLR](Full%20Text%20Search%20-%20Exit%20Lucene.NET,%20Enter%20SOLR/Feeding%20data%20and%20metadata%20to%20SOLR.md)
  * [Making SOLR highlighting work for us :: feeding the results to the Qiqqa user](Full%20Text%20Search%20-%20Exit%20Lucene.NET,%20Enter%20SOLR/Making%20SOLR%20highlighting%20work%20for%20us.md)
  * [For Users :: Example How To Use The SOLR Instance Without Qiqqa UI FrontEnd = scripting and using for your own custom purposes](../../For%20Users/Example%20How%20To%20Use%20The%20SOLR%20Intance%20Without%20Qiqqa%20UI%20FrontEnd.md)
* Qiqqa Middleware (Sync, Import, Export?)
  
  "Middleware": where we keep the old C#/.NET code. Handy tooling on top of the Core. Aiding and abetting with the new *web-technologies*-based UI.
  
  * [Exporting Metadata to {Fill In Application Name / File format}](MiddleWare/MiddleWare%20-%20Exporting%20Metadata%20to%20XYZ.md)
  * [Importing Metadata from {Fill in Application Name / File Format}](MiddleWare/MiddleWare%20-%20Importing%20Metadata%20from%20XYZ.md)
* 
