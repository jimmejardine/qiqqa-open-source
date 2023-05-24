# Considering IPC methods - HTTP vs WebSocket, Pipe, etc.

We've been looking at [ZeroMQ](https://github.com/zeromq/), among others, for our communications.

But before we go there, let's backpedal a bit and look at the bigger picture of application communications:

## Old Qiqqa

'*Old Qiqqa*' is a near-monolithic application that way: it has a UI (which in WPF/.NET is served by a single thread by (Microsoft) design), *business logic*, a database (SQLite) and a FTS Engine (Full Text Search Engine) through Lucene.NET, all as *libraries* (Windows DLLs). Then there's a couple of commercial libraries used as well: Infragistics for parts of the UI and some PDF metadata (*page count* extraction) and (*now defunct*) SORAX for PDF page image rendering. All *communications* among these is via library function calls as usual so call overhead is cheap (the biggest costs there are data marshalling in various places and some thread context switching).

'*Old Qiqqa*' also has a few external applications it uses for specific tasks: `QiqqaOCR.exe` is a C#/.NET application using SORAX and `tesseract.net` (a .NET wrapper for `tesseract` v3, the Open Source OCR engine) to help with PDF text extraction and PDF OCR, when the text layer is not present in the PDF. `QiqqaOCR.exe` also uses an old, patched, `pdfdraw` application from Artifex (MuPDF v1.11 or there-about) for the text extraction work itself, when no OCR is required. Qiqqa does this through this external application to improve overall user-facing application stability as these tools are/were quite finicky and brittle.

v83 Qiqqa is still '*Old Qiqqa*' that way, but has been slowly moving all costly business logic out of the UI thread to improve UI responsiveness. A *synchronous* to *asynchronous* event handling transition in the application, which is generally cause for reams of bugs to surface -- as has happened to the various v83 experimental releases. It's never a nice story, but it had to happen as a prelude to transitioning Qiqqa to a '*New Qiqqa*': this phase showed various issues in the Qiqqa code-base that will also cause difficulties when we pull the monolith apart into several processing chunks.

### Important Note

While generally it is easy to invoke external applications on Windows via `execve()` et al (Windows **does not provide a `fork()` API of any kind!**), redirecting stdin/stdout/stderr is a little more involved and pretty important in our case:

- `QiqqaOCR` spits out all kinds of logging info via stdout/stderr, which has to be filed in the log-files using qiqqa's own log4net logging library. No problem so far.
- `QiqqaOCR` -- or another external application, e.g. bleeding edge MuPDF `mudraw` -- was initially designated as the tool to replace the SORAX render library: *external tool* instead of *library* so we don't have to hassle with marshalling *image data* across the native-to-.NET DLL boundary, *plus* we hoped it would allow us to run 64-bit modern applications as part of the Qiqqa 'back-end'. 

   This was considered a good first step as part of the *transitional phase* of Qiqqa from Old to New.
   
   **No such luck.**
   
   + The .NET code for external execution invocation allows for *redirection* of stdin/stdout/stderr. So far, so good. 
   + After significant effort has been spent on that part of the code, liberally borrowing from other sources, we can only conclude that this interface is **brittle** when used to transfer *binary data such as images* through the STDOUT pipe. 
   
      While the Win32/Microsoft documentation states that this is doable, and my own previous work in C/C++ projects over the years support that, it turned out that the .NET `exec` code remains brittle as it produces spurious errors, particularly when (once again) the invoked `QiqqaOCR` application crashes half-way through.
	  
	  Right now, I consider this **a serious headache** and a **non-viable development path** -- whether it's due to my own stupidity or yet-to-be-discovered-by-me .NET quirks is not important: the alternative route, while more costly *initially*, is the route we wanted to end up with anyway: localhost-based HTTP/socket communications, i.e. "*talking client-server*", sending 'web requests' to a local server for PDF pagee images and the like. I'm very much inclined to climb that steep hill -- steep because the server-side doesn't exist yet.
	  
### Summarizing the technology used

Qiqqa is almost entirely C#/.NET, with .NET libraries, some only available in binary form (WPF, xulrunner, Infragistics, SORAX, pdfdraw -- though the patches and base sources for the latter have been provided, recreating the binary failed to deliver in 2019):

- UI (WPF, Infragistics, SORAX)
- PDF page images for the PDF viewer/reader UI: SORAX
- OCR: tesseract.net v3
- FTS: Lucene.net (lagging behind at v2, BTW)
- Embedded Browser (for Sniffer UI): xulrunner (~ Firefox 36, very much obsolete)

The remainder is served via external applications, which are invoked for every 'call':

- `pdfdraw` (~ MuPDF 1.11, patched)
- `QiqqaOCR` (.NET, mixing tesseract and SORAX above, plus `pdfdraw` invocations, to provide PDF text and metadata to Qiqqa)


## New Qiqqa

'*New Qiqqa*' is a design with a strong focus on *multi-platform*, particularly Linux support. (OSX/Mac is for someone else to do, as I don't have Apple and my horoscope tells me there won't be much Apple in my near future unless several planetes shift dramatically. However, the codebase should be at least 'prepped' for this, ideally.)

A few thoughts have driven the design considerations and decisions here:

- Several components in the Qiqqa biome are *near antique* and upgrading them is not a sine cure, due to significant API changes or simply *unavailability today*.
- Several of those components, *when still alive today*, are lagging behind *significantly* as they 'track' other projects but have fallen behind due to a zillion reasons. (Lucene.NET vs. Lucene itself, tesseract.net vs. tesseract) -- hence I strongly believe we should strive to use those leading systems instead: Lucene and tesseract, as these are very important components of the functionality offered by Qiqqa.

  > Let's call those leading systems the '**native ones**'. That'll make the rest of this text less verbose.
- Anything that makes porting a hard problem should be removed. (WPF, Infragistics, ...)
- .NET wrappers are cute, but a real bother cross-platform **and word-size**: I'm currently **stuck** with 32bit Qiqqa, due to its use of xulrunner, SORAX et al, which are binary-only libraries, only available in 32bit.

  > It turns out that you **cannot** invoke 64bit executables even from a 32bit app (unless you like a *hairy and unstable wizardry to go with that, as it won't run on every user's box, that's for sure*). 
  > 
  > As Qiqqa already is pretty unstable with very large libraries (due to Qiqqa loading all data and keeping it in memory after that, resulting in an overworked .NET garbage collector after a while, thanks to crazy fragmentation and other effects), there's a real need for at least the *transitional phase* to be in 64bit .NET, almost all current major components **block** such a move.
  
Then there are a few personal preferences that matter too:

- I do not intend to use XAML-based UIs *anywhere else*. I consider those a dead-end development line, nice for corporate, but not something that my ambition is geared towards. I believe I should invest more in HTML-based, a.k.a. 'web-based' UI solutions, as those are a better fit for my customers, generally. Ergo: when I have to choose, it's `electron`/`Chromely`/etc. over WPF/Xamarin/Avalonia.

  > I *have* been looking at an old acquaintance of mine though, for limited use: `wxWidgets` (née `wxWindows`) as that would be a nice departure point for a WebView/CEF/???-based **embedded browser** approach: we'll need both a local, open, one and a *secure* WebView, as the latter will be needed for our Sniffer/Browser functionality in Qiqqa.
  > 
  > And, nope, Qt hasn't been on my friends list. So it's gonna be native, wxW or electron/Chromely-*something* then.
  
- I'm not a big fan of Java either.

  Meanwhile I do like Lucene -- and some quick research showed it's positioned very lonely at the top of the capability tree re FTS. So I'll either have to find some Java love or 'architect my way around that one': guess I'll try the latter. ;-)
  
  
  
### This results in this design:

- rule 1: let's use the primary OSS goods, **not any .NET wrappers or ports that 'track' the mainline, if we can help it.**
- rule 2: **no commercial closed source bits in there if I can help it**. It's not just SORAX but 30+ years of commercial experience: *defunct* is identical to *dead & disappeared*. And that's also for individual products. OSS is *not sweet*, it's pretty rough too, but at least it has some *potential persistence* that commercial software never had once they found you didn't need to send source code on tape any more. ;-)
- Lucene is Java. However, I don't want to write (& build, & debug...) Java if I can help it, so that means...: **web API**!? Thus we have a choice between vanilla and strawberry: SOLR and ElasticSearch.

   > While Elastic has been pumping the PR spigot for years, telling everyone and their nanny they're the bee's knees when it comes to **log analysis**, I don't particularly feel addressed by that, as I'm into **document analysis** instead.
   > 
   > Meanwhile, ES did take a dump in my strawberry icecream by showing how companies like theirs (involved with VCs and envisioning IPOs in their future) can do a full 180 without any remorse or empathy when it comes to Open Source. You've clearly shown OSS is not part of your focus and thus I must reckon with ES halting (or at least significantly degrading) OSS ES progress, making ES (the software) a dead end for me.
   > 
   > > I hate it when my *old man* (R.I.P.) was & is right: 
   > > "Son, do remember: **companies do not have a soul.** ... For starters, they lack a *heart*. And once you're old enough, you'll have found they also lack a *brain* -- most of the time. Hence: no chance at having a soul. At all. Reckon with all that, please."
   > > (Plus his corollary to that adage: "The people in there **may**, however. But please don't count on it.")
   > 
   > Thank you, ES. I didn't know which one to pick, 'pre-COVID', but now you've helped me very nicely across my Rubicon of Indecision: SOLR it is, SOLR it's gonna be, no matter what. Because there's nothing else of that magnitude out there.
   > 
   > During the same year-or-there-about, SOLR activity has, it seems, increased and it may be *me* or it may be *others*, but reading up on SOLR every once in a while now has given me the impression that it is indeed a Sane Choice(tm): next to ugly, bloated XML[^**not the fault of SOLR!** Once you're in the Java camp, you're bound to work with XML *a lot* and it's just... (\*\*censored\*\*) ... let's just say XML is exhibit A in the class of Design By Committee and leave it at that...] it comes with extensive JSON support, which is good enough in my book. And very suitable for 'web development' and its ilk. 
   > 
   > SOLR also happens to have all the bits and pieces for flexible **document search** that I did not understand yet, but heard ES was great at through all the advert/fanboy blogs, so also from that perspective either I don't feel I'm 'loosing' anything by not testing ES, but going straight for SOLR.[^that is: after my initial period of indecision, because I didn't fully grasp the technologies and couldn't tell the ape from the monkey there.]
   > 

-->

- Separation of application components across programming language boundaries. In other words: all Java stuff together, all C/C++ stuff bundled together in another executable, ditto for all the C# stuff and then have those different binaries talk to one another via sockets.

   Ergo: no `P/Invoke`-style marshaling, but everyone operates and talks to the others like they're fellow web applications *somewhere*: think 'web requests', HTTP GET & PUT queries, that sort of thing. In short: make it very similar to web development and classical client/server. No smart *solutions* linking language A to B allowed. (Except, perhaps, on the UI side of things where we expect JavaScript/TypeScript to be present in significant numbers, or 'server-side' where we hope to open up our services to '*end-user scripting*' using languages such as JavaScript, Lua or Python to customize their workflows, e.g. tweaking and improving their document OCR workflow, or using custom scripts to process the document data (text / content) stored in the SOLR database.)
   
- replace the current WPF UI with a largely web-tech-based UI. Think `electron` but *leaner* -- `electron` offers a WebView, but also includes a full-fledged `nodeJS` backend, which we don't need as we intend to keep our *business logic* in C#. 

  Do note that we need at least *two completely independent browser views* in our UI: one for our own stuff, including the PDF reader, i.e. the view over which we have *full control*, and then the *other* view which we'll need for secure access to the Big Bad Wolf,pardon,*Web* out there: our 'embedded web browser' as is used in the Qiqqa Sniffer and as-is in the Qiqqa Web Browser panel. `electron` of old had `<webview>` for that, but it has been flagged as *deprecated*/**"do not use"**, so we'll [have to use multiple `BrowserView` instances](https://stackoverflow.com/questions/55023882/two-browsers-in-the-same-electron-window) -- or our own incantation thereof while we don't intend to use `electron` for the UI.

- SQLite is great stuff and I want to keep it. But there's one, no, *two* problems we must solve:

	1. SQLite doesn't play nice with networked disk storage, so we have to come up with a mechanism that doesn't involve using SQLite queries to sync databases -- as is done now in Qiqqa -- but instead provide a more file/disk-image/copy type of 'remote backup & sync' facility.
	
	   I currently do not have a great answer for this item, as Qiqqa Sync is more than merely copying the library database. However, I now do have a generic locking library set up and tested, which is intended to help with this, while either the new C/C++ application server component uses this and takes care of your Sync needs (the locking library is written in C, too) or Sync/Backup is to be delivered through an additional component perhaps?
	   
	2. SQLite isn't designed for client/server access (like PostgreSQL/MariaDB/MySQL are), so we must provide a 'server' for our own and user scripts to talk to, if we want to keep SQLite: then we'll have it running in a single node/application and that application will then receive queries and transmit responses from SQLite so it never gets to see the 'outside world' in a bad way. SQLite is itself written in C, so it should sit in the C/C++ server application part of Qiqqa.
	
- Since we'e talking about Sync just above, we know that *part* of a Sync action is copying the collected PDF documents across to the network/share store: Sync is fundamentally a database copy *plus* a copy of all registered PDF documents. 
 
  > The FTS database, OCR text extracts, etc. are all *local* in current Qiqqa and regenerated on each machine. Given that we intend to allow users *edit access* to that extracted text content, it then *would* become part of the Sync/Backup. Once we do *that*, we'll have introduced a feature into Qiqqa that is **not binary backwards compatible** -- even when we decide to store those OCR extracts in the SQLite database instead of in a separate `ocr/` directory tree containing lots of files, as is the case for '*Old Qiqqa*'.
  
- As we replace Lucene(.NET) with SOLR, *that* will be our Java application part of Qiqqa. SOLR already comes with a nice web interface, that's also usable by user scripts or even manual user access, so using SOLR *properly* would mean we've opened up our metadata and document texts to customizable user processes as anyone can use any language-of-choice to talk to the SOLR web API directly.

   What we need to do there, though, is help users get access to the real documents too, plus maybe some other stuff, but this can be dealt with using our *document content hash* throughout: we need it in SOLR ourselves, for how would we be able to link found texts to actual documents otherwise anyway? So all we need to do to 'complete the Qiqqa API' in that regard is offer API access to our backend server application, where we keep the SQLite database records -- and the knowledge how to access those original PDF documents too?
   
   Sounds to me like that would be a very good idea to keep bundled together: SQLite, basic PDF processing (text & metadata extraction through latest `mupdf`, OCR via `tesseract`, both C/C++ components themselves) and PDF document *storage management*: when you then provide that C/C++-based server for the metadata or thee PDF file itself, it all turns into a few RESTful web API queries, probably using HTTP GET?
   
- Consequently *New Qiqqa* would be formed by 4 (or rather 5, but we'll get to that in a minute) applications, which talk to one another to get things done:

	1. Qiqqa UI: CEF or 'native webview'-based on all platforms, running a minimal *native UI* around a dual-embedded-browser-instance rig, where the UI application includes the necessary HTML+CSS+JS to make the UI work, while any user activity will be handled by that JS bunch, just like in a regular browser-based app, by sending messages to the appropriate backend server or 'business logic' server component of Qiqqa. As a result of such a design, a "Qiqqa API" is an implicit side effect as we'll need it ourselves.
	2. Qiqqa DB/Document Core Server: The C/C++ code all snug together: SQLite for the database(s), `mupdf` for all the PDF work, including rendering PDF page images for the (now browser- instead of WPF-based) Qiqqa PDF Reader and Sniffer UI sections and PDF metadata and text extraction + OCR technology to help us feed the SOLR Full Text Search engine.
	3. Qiqqa SOLR: our own SOLR instance where we keep all those document texts and metadata searchable, so users can use the Qiqqa UI (or their own scripts!) to search for and display lists of relevant documents, including text highlighting (SOLR offers text highlighting itself, which is *expected* to be useful in providing our own -- *Old Qiqqa* did the highlighting completely *in-house* as the old Lucene.NET search hits only reported which PDF documents actually *matched*, but no more. This becomes a bit hairy when you consider complex search queries as you would then need to parse and process that complex search query in *two* code bases (Lucene's and our own!), so having SOLR send us *hints* where the stuff we've been searching for has been found within document X is a *potentially useful boon* in the new system.)

        Qiqqa SOLR needs to be fed data in order to work: that's obviously another task for our Qiqqa Core Server as that one has all the goodies on board to produce text and metadata from documents. Thus this would mean you'll add documents to your library via Qiqqa Core Server, which then, when all the prepwork has been done, will feed the text extract and metadata to Qiqqa SOLR.
		
		When there's an *update* to any of that data, either through the user editing the metadata or the text extract, then we need to update the SOLR record too, which, IIRC, is a delete+insert_same action SOLR-side, to be driven by our Qiqqa Core Server as that's the authority on metadata and documents.
		
		Then there's the other user-driven *edit* action: adding annotations, *revising* the document itself, etc.: all that edit stuff that will *change* the document content hash! This, we haven't completely answered yet, but the basic stance here is that we wish to support (and store) all document revisions as part of our "store all near duplicates and bundle them into a single entry, where desired in reporting": any revision/annotating work would thus be considered a *related document* (no need to go through the whole rigmarole of near-duplicate detection as we'll know iut's us who fed the document to the user or UI component for editing and just got it back, edited: thus we can quickly and simply link the new revision to the existing document/set. As every revision thus gets its own slot in the hash set and consequently in the SOLR database, one important bit in our metadata would thus be the *kind of near-duplicate* we are (e.g. '*user revision*'), **plus** possibly another SOLR metadata *update* to ensure we can select 'latest revision of kind K' in there only: no use searching old revisions when we don't want to. (Of course, this could be minimized to only storing such info in our own SQLite library database, but then we would have to tolerate SOLR always going through all document revisions we ever filed, which can be expected to be a bit of a hassle, not just CPU-load wise, but it might well be that the old revision(s) did contain phrases or words you don't want to be found any more -- at least not until you execute a targeted *historic search* nolens volens. So we'll have to reckon with at least two document slots in SOLR being updated when we import a document revision of any kind.)
		
		The above does not address the scenario where a user decides she needs to 'clean up her collection' by, for example, instructing Qiqqa to *delete old revisions of kind K*: current '*Old Qiqqa*' is a bit half-hearted about *delete* activity, and for good reason: if flags the database record as *deleted* but jkeeps everything else as it was before, because you, the user, *should* be able to *undo* such a drastic move as a *delete* action; we know from personal experience that the time to repent isn't always the next 5 minutes, but occasionally happens the week after, when we suddenly discover that that deleted document was important after all.
		
		Thus Qiqqa is loath to delete material, especially *documents*, and I believe that is a very good stance to take. (For those who **really really** want to clean/strip their libraries, there's always the option to create a new library, copy everything of interest over to the new one and then discard the old library. That's pretty *final*.)
	4. Qiqqa BusinessLogic a.k.a. Middleware
	    Yeah, the remaining C# stuff. Bundled in its own little server component so everyone can do web/socket queries to it and receive responses, while the .NET code is blisteringly *portable* across platforms that way: as long as .NET code doesn't come with anything beyond basic CLI user interfacing, you're *most probably* good to go on the portability scale.
		
		The question remains, however, *what bits* will stay in the C# codebase, when apparently a *lot* of our stuff has been moved into the C/C++ corner (all things *document*, at least!)?!
		AFAICT the honest answer is: "ultimately, not much." Probably import/export bits like importing certain biblio formats (RIS et al?), unless we happen to stumble over a nice parser/processor for that stuff, done in C++. Possibly some other processing stuff that's got nice libraries done in .NET (the LDA stuff is a memory hog and also available in C/C++, while probably also slightly *faster* there). Maybe some interfacing to other applications, e.g. Microsoft Office? The current Qiqqa has a Word extension included for injecting citations and writing Office addons always was a horror in C/C++, so chances are it'll remain alive for at least that bit of functionality.
		
		Ultimately, unless things end up quite different from my current vision, you can expect the C#/.NET code to dwindle: if it doesn't, then that is indicative of me not getting the C/C++ server/core component done the way I expected it to go.
		
- Another area of research has been inter-process communications in the New Qiqqa era: wee'll be needing (much) more of it and preferably at the lowest possible *latencies* all around -- which makes *serialization*/*de-serialization* an *item* to watch carefully, as that bit of process alone can account for some serious burden in a high-traffic environment: de-serialization means *parsing*, which means we need *lexing*, and those fellas have always been costly brethren when you take the down-trodden *path of least resistance*: serializing to a generic text format, e.g. XML or JSON, and then have it processes by your server.

  Thus *we* are **very interested in binary transfer protocols**, especially ones where we can keep the number of in-memory *copies* of a message/object to a *minimum*.
  
  But it all starts with the fastest possible serialization and lexing/parsing a.k.a. de-serialization of each message: 
  
  + we know we cannot pull any tricks while we interact with the FTS database (Qiqqa SOLR) as that server component is rather *frozen*, code-wise -- unless we find a deep motivation **or some one else interested in this job** of adding a fast binary request/response mechanism to the SOLR codebase. I don't worry to much about it though as I do not expect that path to become a bottleneck all that quickly -- even though raw old Lucene *should at least theoretically be faster than a system, such as SOLR, which wraps raw Lucene in one or more layers of additional (message) processing*: old Lucene has it's own performance issues already, so I'ld be far more exited meeting and talking to people who've been playing the *optimizing-SOLR-search-databases* game, as that's the modern and potentially much more fruitful route to take re FTS query performance: we simply didn't have anything to optimize before because it was pretty much fixed, but it got slow as molasses once you fed the system 10K+ documents to munch, so I *expect* Qiqqa users like myself at least *might* benefit more from modern SOLR+Lucene optimization technologies. It's only a petty that I read a lot about it, but only very few folks seem to be actually involved in this and it's all closed commercial work AFAICT, while I myself are a complete and utter *greenhorn* at SOLR usage, so I can forget about *optimizing SOLR* until I graduate. ;-)
  + we also know we'll be sending pretty much every click-to-action in the UI to *a* Qiqqa Server Component (which, in turn, might fire off addition messages to other components) and *this* is where we hope our new design will be able to respond quickly enough for long-term user satisfaction to stay high: this path originally was all direct-call, P/Invoke and maybe some marshalling, so we're heaping on the communication overhead there, relatively speaking. Even in a *localhost* setting, *latency* is an important issue! And so is *overhead*: encoding/decoding, headers, etc. increasing the number of bytes transmitted and thus processed without directly contributing to the end result!
  
     Of course, *Old Qiqqa* had it's own horrible delays (and action/response *latencies*), thanks to several background threads vying for attention and filing tons of (text extract and OCR) work into the same restricted pipeline where user UI actions needed the same sort of work done, due to direct or *indirect* causes: a prime example of the latter is the user clicking on the sort-by sidebar in the library's Document List view/tab: if the user clicks on, say, sort-by-page-count, that implies we need to obtain the (page-count) metadata for all documents in the library (for we need to sort on this criterium), which in turn implies we need to obtain the *metadata* for every document in the library. This, in turn, (yes, it get hairy like that!) implies we need to invoke Infragistics or other means to our disposal for every document we haven't yet obtained a solid page count for. Which is what happens when you just re-imported your *huge* library some time ago (i.e. somewhere during these last few *weeks*!!!) as metadata collection is *sensibly* off-loaded to a background task as it involved going through the PDF documents, decoding and scanning them at least *partially* in order to obtain that page count number, and that, my friends, culminates in lots of disk I/O, reading = loading each of those PDF documents into memory and doing some work on it, thus resulting in a significant chunk of time needed to scan a fresh library for a mere metadata item like *page count* -- don't get me started on title and author, i.e. *generic metadata* **inference**, which is also part of Qiqqa background processes, to help you on your way while you haven't personally vetted every bibliographic record yet. All great stuff!
	 
	 **BUT!**
	 
	 *Old Qiqqa* isn't much smarter than this: it all files such 'background task requests' in order of appearance. Which is swell, as long as you have a small library and no troubles.
	 
	 It becomes an entirely different kettle of fish once you grow your library/libraries to a more, ah, *remarkable* size, say 5K+ documents -- the numbers are soft, as this is a gradually worsening phenomenon, even though it often ends up as *suddenly not funny anymore all at once!*
	 What happens then is that *Old Qiqqa* would (re-)start, prep a bit of stuff, like loading all your library data records into memory, and then ready itself for user action. (Because I've had a **lot** of trouble with those background processes doing what they're about to do, I delay them in v82+, so the first seconds are rather hassle-free. Buckle up, for here it comes.)
	 
	 Say you click on a benign action button (**not** that sort-all-of-them-by-page-count, but doubleclick on a single document to view it in the Qiqqa Reader, for example), then Qiqqa will send your action to a background thread (because we cannot have it all done in the UI thread as it would completely lock up your UI while it's working - a phenomenon seen everywhere in Qiqqa in v81 and before) and it will be queued for execution. Which happens almost instantly as the queue was empty before.
	 
	 However, the background tasks are warming up too, their initial delays have expired, and those now happen to discover that quite a few of your library's documents have had their author/title/abstract/etc. metadata collected. Which results in the auto-discovery helpfulness we mentioned earlier. Which needs the PDF document *text extract*, pronto, for we need to detect title, author, etc. in there, *guestimate them* if you will, and then use that until you make the effort to either vet them or use the Sniffer or other means to *edit* the auto-guessed metadata and thus mark them as *final* and have them stored in the metadata database. *Text extraction*, however, is that costly process where we invoke `QiqqaOCR.exe` and maybe have that one invoke `pdfdraw.exe` to obtain the text content, IFF a text layer is present. (When it's not, no worries, we'll fire off another round of `QiqqaOCR.exe` with demands to OCR those pages for you.
	 
	 Oh yeah, I forgot to mention: this hapiness occurs for every 20 pages, unless it's OCR work, which happens for every single page where text is lacking, resulting in quite a few queued `QiqqaOCR` invocations for a single document, even when all you were doing was looking for a title and author only.
	 
	 This isn't *bad* or anything, heck, it's *necessary* to deliver that information to you, but it becomes quite a bother when this stuff is (partly) pending when you, the user, decide to click on another beenign button that demands some ready PDF metadata under the hood: your user action will queue its own demands, whatever they are, **but those will have to wait until the pending queue has depleted** as there's no real sense of *priority calls* in there. 
	 
	 Well, heck, why didn't I add that already, then, eh? 
	 
	 Because that was a bit hard to add, particularly as some actions *require* other actions to be *finished* before they can even start, resulting in *priority chains*: I can *prioritize* the page count request action all I like, but it won't deliver until the relevant *metadata* task has completed, which depends on the *text extraction* task to be completed successfully. And if you still have that bit of process chain vivid on your retina, you now realize that that dependency chain can go quite deep: failing 'regular' text extraction for X out of Y pages of a single document will be cause to file X *additional* OCR requests, which *may fail*, thanks to the complexity of OCR technology and the age of tesseract.net et al: it's not exactly the smartest kid on the block any more, is what we have...
	 
	 So, yes, it's a quagmire. And while doable, it wasn't the primary concern to date for me: stripping out SORAX, replacing tesseract.net, getting rid of Lucene.net, etc. are all higher priorities as those are *all* required to lift Qiqqa-as-it-is-today out of the 32bit memory-limited cesspool that's a big burden to application stability for folks with 10K+ libraries, like your truly. So the queueing and piling up of action requests was relatively benign... **until...**
	 
	 ...Until I started to remove SORAX for the PDF page image rendering work and replace it with another external application: `mudraw.exe` (the modern nephew of `pdfdraw.exe`, now also capable of producing very nice PDF page images *and* PDF text extracts and a whole lot more -- with the help of some patches and some more work, it's a veritable Swiss Army Knife as far as PDFs are concerned.) However, this *great move* results in yet another action path traveling through that action queue bottleneck: now, instead of a P/Invoke call to SORAX internals, every PDF page image we render for you in the viewport of the PDF Reader *must* be queued to invoke `mudraw` for each of these -- queueing here is *mandatory* as invoking all those `mudraw` instances all at once will have your computer choke on multiple instances of a PDF renderer and memory overflows and OS Blue Screens are in your future when you try that 'simple approach'. So queueing it is, and in order to keep the number of running, memory and CPU-guzzling tasks manageable, we use that same ol' same ol' friend: the ThreadPool. Which was, is the scenario before, already pretty loaded with work coming in from the background threads, demanding all sorts of PDF text extraction work already, so, lacking a priority+task-dependency graph, ... you'll just have to wait.
	 
	 Which is the current state of affairs in the bleeding edge v83 experimentals at the time of this writing. And a, let's be frank, *utterly unusable state of affairs* as you'll observe Qiqqa rendering a couple of pages, then choking internally and printing only 'please wait' pages until Thee End Of Days, because some background threads decided we need to call those same MuPDF tools for text extraction already and that comes first, regrettably.
	 
	 So the current task is to pull that stuff apart and have it managed, perhaps, through multiple queues? (As a probably-much-eeasier-to-do approach than a full-fledged dependency-analysis-and-management prioritized singie queue...)
	 
	 
	 
	 Okay, that rant ran off the tracks, because I started writing about *New Qiqqa* and ended up yakking about the current Transitional Qiqqa / Old Qiqqa state of affairs....
	 
	 
	 
	 ... Which is *relevant*, as that queueing/prioritization/dependency problem **will persist in the new design** as it's implicitly linked to what we're doing with Qiqqa: it's kind of *fundamental* to our *feature set*.
	 
	 Yes, we do have *ideas* how to cope with it in the new regime. But that doesn't make it go away.
	 
	 The current thought is that the C/C++ Core takes care of this, as it's the closest and most involved with those (costly) tasks, so *solving* it in the C/C++ codebase would mean we'll have the least amount of inter-process messaging and running-around / panicking while we process the incoming demands: 
	 + user/UI initiated actions have priority, **but**...
	 + ... we must assign multiple priority levels there as there's 'swift/precise work' and 'slow/large/**tolerant** work' incoming from the user side:
		 + 'swift/precise work' would be, for example, looking at PDF pages, hence PDF page renderings.
		 + 'slow/**tolerant** work' would be anything from showing a page count to *sorting a library by page count*: all these require potentially costly action chains, so the cop-out there (the '**tolerant**' bit) would be to deliver an '*I don't know yet*'' **preliminary response** that can be used to render the UI and would imply the UI will be expecting a more *precise* answer any time later. The *sorting* scenario can be resolved (i.e. *sped up*) by simply taking those '*I don't know*' responses and render them nil, resulting in a *incomplete* sort order,which *may* be signaled in the UI, without any automatic follow-up -- experiments in current Qiqqa show that it's pretty confusing to have your UI *update in major ways* without a very clear and direct relation to your own actions in recent time: if it takes more than a second or so for the missing page counts to arrive, it's much better to ride with what you've got, than keep (automatically) updating of the sort order, which would result in documents/rows jumping around "like crazy", because the user has mentally lost the relationship between this haphazard activity and the (now *old*) sort instruction. You also don't want to bother the user with extra work here, e.g. acting on the choice whether or when to *stop* the automatic update of the requested sort order.
	 + most of the costly actions involve codebases in the C/C++ realm, so messaging overhead would be minimal there. Meanwhile, any SOLR search action is expected to be pretty swift (and using what's currently in the SOLR database anyway, **not** implicitly waiting for incoming additional data), while all SOLR data updates (resulting from documents being processed in the Core) can happen in the background without risk of locking up a queue. All we might need there is possibly a bit of monitoring and progress reporting so the user can be informed about the actual state of affairs re fill rate, etc. of the FTS database.
 + We'll have to code that priority/dependency-chain queue in C# as well, for it is needed by current Transitional Qiqqa, while future New Qiqqa will need a mechanism like that done in C/C++.
    
	For a while I hoped I could get away with coding it in C/C++ immediately, offering the PDF page rendering, etc. as a preliminary Transitional Core, but alas, time and RL intervene.
	
		 





# TO BE COMPLETED


Note to self: the 5th component would be the Qiqqa Launcher: we'll need that bugger for Transitional Qiqqa at least for that's the only 'sane' way to launch both a 64bit server/Core base and a 32bit-locked .NET application that can't move until I got rid of the rest of the buggers in there.

And then, when you consider using a Launcher like that, the question pops up: why not have it do actually useful stuff, like, say, *monitoring* our server components and relaunching them when they crash, or present some diagnostics and advice for the user when such a nasty thing happens under the hood?
Hm... which makes this the 5th component. 

1=UI, 2=C/C++ core (DB, PDF, OCR, tough work), 3=SOLR, 4=remaining C#/.NET middleware.

See also [[The Transitional Period#Tackling the transition from 32bit to 64bit]].




Also we looked at sockets vs. pipes vs. regular HTTP/1.1 request/response and HTTP/2 for our inter-process communications. Cross-platform, the current experiments indicate that using pipes, while *cool*, is not easy nor *robust*, at least not on Windows -- and probably also on Linux when you expect to have relatively large, compound applications, thus elevated per-component stability risks.
Pipes are not significantly faster than raw sockets, at least not on Windows, so pipes (and Completion Ports and all that Jazz) are *out*.
Shared Memory would have been cool for multiprocess apps if those weren't coded in at least two different run-time bases: C# and C/C++ -- unless you want to revert to plurging in P/Invoke calls and consequent cross-platform hassles.
Which leaves the various *socket*-baseed flavors: TCP or UDP? UDP is cute for some usages, but ours benefits more from TCP: as long as we make sure our **message-based** communication system runs supple on top of **stream-based TCP**, we're going to be golden. This, of course, requires us to be abnle to code the TCP sockets to `telnet`-style behaviour, i.e. `NO_NAGLE` will be our primary friend there.

For this subject, we've been looking at off-loading the high-perf comms code to someone else's codebase for both C# and C/C++ (Java doesn't play as we'll interface with SOLR like everyone else, through regular HTTP and JSON exchanges. *JavaScript*, i.e. the embedded browser-based 'web technology' UI, is another one that's best served with 'regular client/server traffic approaches', thus JSON, images as regular `<img src=xyz>`  binary subrequests, etc. -- unless we have to find ways around that for performance reasons and do some advanced WASM binary processing work or some such. I do hope *not*, for it would invalidate the basic design, IMO.  Meanwhile, wxWindows is waiting in the coulissen and can be used for those those UI bits that *absolutely need native UI performance* -- since that chunk of UI code would then be C++, we're good with fast binary transfers from Core anyway, as it'll be C/C++ to C/C++ then.)

One of the 'external libraries' for fast binary communications we've been looking at is ZeroMQ: it's available for several languages and mature for the ones we are interested in (C# and C/C++). Whether we use ZeroMQ as-is or rip the socket technology it uses to ensure *low latency message transmission over a stream protocol (TCP)* is less relevant then the discovery that it has already been useful in teaching me about the existence of `SIO_LOOPBACK_FAST_PATH` and its usefulness on Windows systems: I know from previous encounters that localhost socket programming on Windows isn't always as fast as you might expect and that one explains it nicely. Very useful.

While we won't need all the ZeroMQ features, it is a clean and lean comms/message design and is expected to be useful to us, together with our own 'custom' object/message binary serialization/deserialization (for which we intend to employ the `bebop` tool).

Flatbuffers/Protocolbuffers/Msgpack have been considered as well and deemed *too much hassle*: suboptimal in data transformation. Protocolbuffers and flatbuffers are simply too complex for what they deeliver, while msgpack seems primarily focused on type- and version-safe transmissions with cheap data packing -- which is quite different from what we're looking for:

- blazing fast transmission + transformation: latency != network but latency is end-to-end cost, including the message serialization and data-copying work. flatbuffers looked nice, but better/smarter solutions were found.
- we don't care about versioning: the entire conglomerate is guaranteed to be installed and updated simultaneously, so they will always run the same 'message version'. Also, we don't need messages to *persist* beyond the application lifetime, so message versioning is a non-feature here.
- we're communicating through an 8bit-clean channel (localhost/TCP), so anything goes, no need for ASCII-only nor similar restrictive charset approaches needed. JSON is right out as it's too costly, both in encoding and decoding, vs bebop and similar binary encoding systems.
- we use a safe channel in that wee do not expect to *loose arbitrary data*: TCP ensures we'll have a failure on the first corruption like that. Consequently, we don't need to 'sync to message start' while the TCP connection remains alive; all we need is the knowledge of incoming message size, so that we can properly collect it and process it, before picking up the next one incoming. This is resolved by using a system like ZeroMQ; which is also why this has been considered for use, next to the 'regular vanilla HTTP interface' to to our server components: the C/C++ Core server is thus expected to offer both regular HTTP access for user scripts and the like, while also offering the same (optimized) services via ZeroMQ socket.
 
  We have yet to investigate the use of 'websockets' though, and possibly mixing their use with ZeroMQ: *websockets* would be a good answer for our UI challenge where we would want to send many messages from the UI to the backend(s) while the user moves around and triggers actions (clicking, etc.): using 'vanilla HTTP/1.1' for that stuff would be pretty costly *latency-wise* as it would incur the cost of HTTP connection setup *for every since user action* -- thus a rather more *permanent* communication connection would be preferable. That would then be either HTTP/2 or websockets, depending on how we'ld end up 'doing our UI'. HTTP/2 has a *slight preference with me* as it would '*automagically*' support `<img>` tag updates and such-like, if I am not mistaken, so a HTTP/2-capable Core server would be a nice feature to give UI components, such as the Qiqqa PDF Viewer, a bit of a leg up in the performance/latency department.
  
  However, [as stated by the HTTP/2 FAQ](https://http2.github.io/faq/#does-http2-require-encryption): "*currently no browser supports HTTP/2 unencrypted*" which would mean we're setting ourselves up for a pretty amount of useless work for localhost-based HTTP/2 to work for us (as the receiving end will be either CEF, WebView2 = Edge, or similar modern browser as embedded application component.)
  
  Additional infos on HTTP/2 vs. websockets:
  
  - sending images through websockets: [https://stackoverflow.com/questions/11089732/display-image-from-blob-using-javascript-and-websockets] -- also do note the comments there, including the one about `revokeObjectURL` [here](https://stackoverflow.com/a/42155934). Plus https://developer.mozilla.org/en-US/docs/Web/API/URL/revokeObjectURL
  - https://www.litespeedtech.com/http3-faq -- we won't be using HTTP/3, but it's good to know *why*: HTTP/2 with its mandatory encryption is probably already a bit too heavy for the localhost comms we envision. HTTP/3 is UDP-based and only suitable in a real network use.
  - 
  
  


	 
	 
	 







## See Also

- [David Luecke, 2018: HTTP vs. Websockets: A performance comparison](https://blog.feathersjs.com/http-vs-websockets-a-performance-comparison-da2533f13a77)

   > On average a single HTTP request took about 107ms and a Socket.io request 83ms. For a larger number of parallel requests things started to look quite different. 50 requests via Socket.io took ~180ms while completing the same number of HTTP requests took around 5 seconds. Overall HTTP allowed to complete about 10 requests per second while Socket.io could handle almost 4000 requests in the same time. The main reason for this large difference is that the browser limits the number of concurrent HTTP connections (6 by default in Chrome), while there is no limitation how many messages a websocket connection can send or receive. We will look at some benchmarks without that restriction a little later.
   > 
   > \[...]
   > Another interesting number to look at was the amount of data transferred between both protocols. Once established, a websocket connection does not have to send headers with its messages so we can expect the total data transfer per message to be less than an equivalent HTTP request. Establishing a Socket.io connection takes 1 HTTP request (~230 bytes) and one 86 byte websocket frame.
   > 
   > \[...] 
   > I was also curious about load tests from multiple clients and ran some benchmarks. The only common data point was the total runtime of the benchmark, which is what we will compare here by charting the total (fixed) number of requests (per connection) over the time it took to make them (as requests per second). The benchmarks are each running 100 concurrent connections and, unlike the browser numbers above, include the time it takes to establish the websocket connection.
   > 
   > \[...] 
   > making a single request per connection is about 50% slower using Socket.io since the connection has to be established first. This overhead is smaller but still noticeable for ten requests. At 50 requests from the same connection, Socket.io is already 50% faster.
   > 
   > \[...] 
   > the HTTP benchmark peaks at about~950 requests per second while Socket.io serves about ~3900 requests per second.
   > 
   > \[...] 
   > An important thing to note is that even when used via websockets, the communication with the Feathers server is still RESTful. Although most often used in the context of HTTP, [Representational State Transfer](https://en.wikipedia.org/wiki/Representational_state_transfer) (REST) is an **architectural design pattern and not a transport protocol**. The HTTP protocol is just one implementation of the REST architecture.
   >  
   > \[...] 
   > Enabling different communication protocols and being able to transparently switch between them without having to change your application logic was one of the key design goals of Feathers. There is nothing wrong with web frameworks that help handling HTTP requests and responses with newer language features, different design patterns or that are simply faster. However, I still believe that a protocol independent architecture and being able to dynamically choose the most appropriate transport protocol will be crucial for the future of connected APIs and applications. In the case of our benchmark, we were able to get a 400% performance boost by using a different protocol without having to change anything in our actual application.

- [Alexis Abril, 2017, A comparison between WebSockets, server-sent events, and polling](https://medium.com/dailyjs/a-comparison-between-websockets-server-sent-events-and-polling-7a27c98cb1e3)

   > ### Methods
   >
   > -  **Manual page refresh** (control)  
   >    After each 1 second interval, the page is refreshed. No real-time behaviors are observed with this method.
   > -  **HTTP short-polling** (250ms interval)  
   >    Short-polling continuously opens and closes HTTP requests seeking new updates on an interval. The interval ideally is relative to expected updates.
   > -  **HTTP long-polling**  
   >    Long-polling opens an HTTP request and remains open until an update is received. Upon receiving an update, a new request is immediately opened awaiting the next update.
   > -  **Server-sent events**  
   >    Server-sent events(SSE) rely on a long-lived HTTP connection, where updates are continuously sent to the client.
   > -  **WebSockets** 
   >    The WebSocket protocol allows for constant, bi-directional communication between the server and client.
   >  
   > \[...\]
   >  
   > ### Conclusion
   > 
   > In the current state of the web, short and long-polling have a much higher bandwidth cost than other options, but may be considered if supporting legacy browsers. Short-polling requires estimating an interval that suits an application’s requirements. Regardless of the estimation’s accuracy, there will be a higher cost due to continuously opening new requests. If using HTTP/1.1, this results in passing headers unnecessarily and potentially opening multiple TCP connections if parallel polls are open. Long-polling reduces these costs significantly by receiving one update per request.
   > 
   > Server-sent events are able to take performance a step further. Rather than having an open request per update, such as long-polling, server-sent events provide a single, long-lived request to allow for the streaming of updates. Benefits of this method include only passing headers once, when the request is made, limiting data across the wire to necessary information. An EventSource will attempt to reconnect if the current connection is interrupted. Server-sent events are native to most modern browsers and reside within the existing HTTP spec. Meaning, if a legacy application has an existing RESTful layer, security, and authentication, the modification to use server-sent events is minimal. Server-sent events are uni-directional, allowing only for updates to travel from the server to the client. Additional client messages to the server, such as a POST request, would require an additional HTTP request.
   > 
   > WebSockets allow for full-duplex communication over a single connection. To compare, uni-directional communication(SSE) is akin to a radio. A single source broadcasts information to a listener. Half-duplex communication is similar to using a walkie-talkie. Communication may travel in both directions, however, only one party may broadcast at a time. Full-duplex is similar to using a phone. Information may freely travel in both directions, simultaneously. WebSockets is a different protocol and as such, security must be considered with implementation. Authentication and security concerns are similar to HTTP communication, however, may need to be duplicated for a secure WebSocket channel. An example of a common scenario is to authenticate a user, providing them with a token to be sent for HTTP communication. However, if a user is authenticated, an application may create a WebSocket connection without validating the token. This allows for direct access to the WebSocket API, bypassing any HTTP security measures.
   > 
   > In the past, server-sent events have had a higher bandwidth cost due to HTTP/1.1’s handling of headers. In the scenario of uni-directional, push updates with HTTP/2 are now almost as cheap as a WebSocket transfer in terms of bandwidth. However, there is a consideration of development overhead when using WebSockets. The handling of WebSocket reconnects or “heartbeat” mechanisms, authorization, and/or including a WebSocket library result in development costs. The WebSocket API is able to be used natively, as opposed to differing libraries behind Primus in this test, however, a real-world implementation benefits from the existing libraries(WS, Engine.IO, etc) to handle reconnects and fallback methods.
   > 
   > For the development of most form-based web applications, server-sent events provide a low development overhead while benefiting from existing HTTP practices. If an application requires a high rate of upstream communication, a WebSocket implementation would provide a much lower bandwidth cost and may be worth the development overhead.   

- [Be aware of Windows localhost TCP for IPC](https://codeistry.wordpress.com/2018/08/23/be-aware-of-windows-localhost-tcp-for-ipc/)] a.k.a. [`SIO_LOOPBACK_FAST_PATH`](https://codeistry.wordpress.com/tag/sio_loopback_fast_path/)

   > I came across a strange problem on Windows OS.
   >
   > For inter-process communication (IPC) I was relying on localhost TCP/IP communication on Windows. A TCP server and TCP client both running on my localhost and data transfer between these processes is happening on the TCP socket.  
   > 
   > The problem was the rate of data transfer on the localhost TCP/IP connection was very slow.  
   > When the similar code was ported on Ubuntu we got 4X improvement in data transfer speed on localhost TCP. That made us think that there is nothing wrong with our program but there has to be some issue with the Windows localhost TCP/IP.
   > 
   > After doing some web search came across this link:  
   > [https://blogs.technet.microsoft.com/wincat/2012/12/05/fast-tcp-loopback-performance-and-low-latency-with-windows-server-2012-tcp-loopback-fast-path/](https://blogs.technet.microsoft.com/wincat/2012/12/05/fast-tcp-loopback-performance-and-low-latency-with-windows-server-2012-tcp-loopback-fast-path/)
   > 
   > Windows has provided an IOCTL **SIO_LOOPBACK_FAST_PATH** to improve the speed of localhost TCP/IP communication.
   > 
   > The following piece of code did the magic for our application to achieve the 4X performance.
   > 
   > ```
   > int OptionValue = 1;
   > DWORD NumberOfBytesReturned = 0;
   >  
   > int status =
   >     WSAIoctl(
   >         Socket,
   >         SIO_LOOPBACK_FAST_PATH,
   >         &OptionValue,
   >         sizeof(OptionValue),
   >         NULL,
   >         0,
   >         &NumberOfBytesReturned,
   >         0,
   >         0);
   >  
   > if (SOCKET_ERROR == status) {
   >     DWORD LastError = ::GetLastError();
   >  
   >     if (WSAEOPNOTSUPP == LastError) {
   >         // This system is not Windows Windows
   >         // Server 2012, and the call is not
   >         // supported.
   >     }
   >     else {
   >         LogAMessageSomeWhere(
   >             "Loopback Fastpath WSAIoctl failed: ",
   >             LastError);
   >     }
   > } 
   > ```

-------------------

## Update May 2023

I was of two minds whether ZeroMQ(+TCP) would be useful at all for local communications between application components, the idea being that I want everyone to be able to access the backend anyway and using REST is the obvious approach there as that makes access simple & easy for any programmer and language they want to use.

REST (HTTP/GET) nowadays would imply we'ld need HTTPS for localhost -- see also https://weblog.west-wind.com/posts/2022/Oct/24/Fix-automatic-rerouting-of-http-to-https-on-localhost-in-Web-Browsers for one of the reasons: we MUST assume the user will be running other applications on their machine at any time and when only one of them uses HSTS, we're toast when we do not also include the HTTPS encryption/decryption *overhead* for our REST messages -- something I don't see as adding any kind of benefit when comms are all `localhost` loopback...)

The alternative, *faster* IPC for `localhost` traffic would be using a memory-mapped comms system, such as [iceoryx](https://github.com/eclipse-iceoryx/iceoryx), but I wasn't eager to implement that *immediately* as those performance benefits only MAY become apparent when the Qiqqa system is under heavy load and reducing overhead would become important enough to save on energy consumption and general overhead costs. However, [the article referenced above](https://github.com/eclipse-iceoryx/iceoryx), reminded me of the importance of having a simple & *fast* comms system next to regular REST and it should be implemented straight away as HSTS is a hidden issue that can pop up at arbitrary customer machines, thanks to the domain-wide enforcement by the browsers (which are backing our `webviews`  in any GUI we're about to produce), so it' either immediate use of [iceoryx](https://github.com/eclipse-iceoryx/iceoryx) throughout our design, with REST-style access for "external use by others", and MAYBE adding ZeroMQ-based IPC as well as a simple TCP-based, ah, "intermediate level alternative for arbitrary programming languages", because shared memory and named pipes are beautiful, but, in my experience, not everyone' cup of tea -- *simplicity of access* then being the reason to offer REST access (HTTP**S**/GET!) and ZeroMQ-TCP / iceoryx-shmem for advanced users and internal usage...

🤔 ... and then we'ld have to come up with an iceoryx-compatible IPC codebase for C# and JavaScript as those will be our own *interfacing languages* for the GUI/front-end side of the overall Qiqqa application 🤔🤔 ... which is no sine cure for JavaScript, at least, so we might be well served ourselves by offering ZeroMQ/TCP alongside, so we can guarantee low interfacing overhead (no encrypt/decrypt) between our own old (C#) + new (JavaScript/webview) front-ends and C/C++ backend. 🤔 Unless we can find a cross-platform way to get iceoryx-style shared memory access into those platforms without much effort -- not a great  chance at achieving that, so we'ld better consider using ZeroMQ and seeing how can integrate iceoryx-style shmem access into that one: IIRC ZeroMQ doesn't support shmem on Windows?



### The solution to this conundrum? `nanomsg`?

- https://nanomsg.org/documentation-zeromq.html

The important bits follow below:

---------------

> **NOTE**
> 
> Much has changed since this was written, both in nanomsg and ZeroMQ. Nonetheless this document may be of interest to understand the historical motivations behind nanomsg in the words of the original author of both ZeroMQ and nanomsg.
> 


### POSIX Compliance

ZeroMQ API, while modeled on BSD socket API, doesn’t match the API fully. nanomsg aims for full POSIX compliance.

-   Sockets are represented as ints, not void pointers.
    
-   Contexts, as known in ZeroMQ, don’t exist in nanomsg. This means simpler API (sockets can be created in a single step) as well as the possibility of using the library for communication between different modules in a single process (think of plugins implemented in different languages speaking each to another). More discussion can be found [here](http://250bpm.com/blog:23).
    
  

### Implementation Language

The library is implemented in C instead of C++.

-   Number of memory allocations is drastically reduced as intrusive containers are used instead of C++ STL containers.
    
-   The above also means less memory fragmentation, less cache misses, etc.
    
-   More discussion on the C vs. C++ topic can be found [here](http://250bpm.com/blog:4) and [here](http://250bpm.com/blog:8).
    

### Pluggable Transports and Protocols

In ZeroMQ there was no formal API for plugging in new transports (think WebSockets, DCCP, SCTP) and new protocols (counterparts to REQ/REP, PUB/SUB, etc.) As a consequence there were no new transports added since 2008. No new protocols were implemented either. The formal internal transport API (see [transport.h](https://raw.github.com/nanomsg/nanomsg/master/src/transport.h) and [protocol.h](https://raw.github.com/nanomsg/nanomsg/master/src/protocol.h)) are meant to mitigate the problem and serve as a base for creating and experimenting with new transports and protocols.

Please, be aware that the two APIs are still new and may experience some tweaking in the future to make them usable in wide variety of scenarios.

-   nanomsg implements a new SURVEY protocol. The idea is to send a message ("survey") to multiple peers and wait for responses from all of them. For more details check the article [here](http://250bpm.com/blog:5). Also look [here](http://250bpm.com/blog:20).
    
-   In financial services it is quite common to use "deliver messages from anyone to everyone else" kind of messaging. To address this use case, there’s a new BUS protocol implemented in nanomsg. Check the details [here](http://250bpm.com/blog:17).
    

### Threading Model

One of the big architectural blunders I’ve done in ZeroMQ is its threading model. Each individual object is managed exclusively by a single thread. That works well for async objects handled by worker threads, however, it becomes a trouble for objects managed by user threads. The thread may be used to do unrelated work for arbitrary time span, e.g. an hour, and during that time the object being managed by it is completely stuck. Some unfortunate consequences are: 
- inability to implement request resending in REQ/REP protocol, 
- PUB/SUB subscriptions not being applied while application is doing other work, and similar. 
In nanomsg the objects are not tightly bound to particular threads and thus these problems don’t exist.

-   REQ socket in ZeroMQ cannot be really used in real-world environments, as they get stuck if message is lost due to service failure or similar. Users have to use XREQ instead and implement the request re-trying themselves. With nanomsg, the re-try functionality is built into REQ socket.
    
-   In nanomsg, both REQ and REP support cancelling the ongoing processing. Simply send a new request without waiting for a reply (in the case of REQ socket) or grab a new request without replying to the previous one (in the case of REP socket).
    
-   In ZeroMQ, due to its threading model, bind-first-then-connect-second scenario doesn’t work for inproc transport. It is fixed in nanomsg.
    
-   For similar reasons auto-reconnect doesn’t work for inproc transport in ZeroMQ. This problem is fixed in nanomsg as well.
    
-   Finally, nanomsg attempts to make nanomsg sockets thread-safe. While using a single socket from multiple threads in parallel is still discouraged, the way in which ZeroMQ sockets failed randomly in such circumstances proved to be painful and hard to debug.
    

### IOCP Support

One of the long-standing problems in ZeroMQ was that internally it uses BSD socket API even on Windows platform where it is a second class citizen. Using IOCP instead, as appropriate, would require major rewrite of the codebase and thus, in spite of multiple attempts, was never implemented. IOCP is supposed to have better performance characteristics and, even more importantly, it allows to use additional transport mechanisms such as NamedPipes which are not accessible via BSD socket API. For these reasons nanomsg uses IOCP internally on Windows platforms.

### Level-triggered Polling

One of the aspects of ZeroMQ that proved really confusing for users was the ability to integrate ZeroMQ sockets into an external event loops by using ZMQ_FD file descriptor. The main source of confusion was that the descriptor is edge-triggered, i.e. it signals only when there were no messages before and a new one arrived. nanomsg uses level-triggered file descriptors instead that simply signal when there’s a message available irrespective of whether it was available in the past.

### Zero-Copy

**While ZeroMQ offers a "zero-copy" API, it’s not true zero-copy. Rather it’s "zero-copy till the message gets to the kernel boundary". From that point on data is copied as with standard TCP.** nanomsg, on the other hand, aims at supporting true zero-copy mechanisms such as RDMA (CPU bypass, direct memory-to-memory copying) and shmem (transfer of data between processes on the same box by using shared memory). The API entry points for zero-copy messaging are `nn_allocmsg` and `nn_freemsg` functions in combination with NN_MSG option passed to send/recv functions.

-------------------

🤔 ... or should we rather look into ZeroMQ to make sure those nanomsg improvements & fixes have also made it into ZeroMQ?

- ZeroMQ: https://github.com/zeromq/libzmq, https://github.com/zeromq/libzmq/issues/3934, http://wiki.zeromq.org/blog:zero-copy, 