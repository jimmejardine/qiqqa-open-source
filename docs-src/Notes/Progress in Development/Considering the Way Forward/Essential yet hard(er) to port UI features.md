# Progress in Development :: Considering the way forward : Essential yet hard(er) to port UI features

> This is considering those bits of Qiqqa which would take some significant effort to port to any system other than the current WPF/C#/.NET environment. It doesn't matter if you're considering porting to Qt or any other UI framework within the C# or even considering porting the UI to a non-C#/.NET cross-platform environment, e.g. Electron: the cost will differ, depending on the direction you choose to go, but it will be more or less severe.

## PDF viewer / editor

Qiqqa comes with a built-in PDF viewer, which also serves as editor for PDF annotations:
- highlighting user-marked text
- drawing shapes in the PDF document page
- adding text annotations to the PDF document page

All these edits happen in a graphical **overlay** which is rendered on top of the PDF page image, which is rendered by the Sorax Pdf Renderer.

Coordinates are stored in a floating point format, where the PDF page is treated as an area ranging from 0 to 1 on both axes. ^[Need to verify this statement as I write this off the top of my head without checking the Qiqqa source code. YMMV]

That way, previous annotations can be stored, recalled and then rendered at the original spot later on.

To highlight text, the OCR + text extraction output from a custom-built `mupdf` tool is cached as text words plus box coordinates for each 'word' in that same coordinate system. That way a mouse click+drag over the PDF page image can be decoded to produce a selection box for each character/word in the PDF document and thus provide text highlighting.

Notes: 

- Some PDFs do show **odd behaviour** as their PDF text extractor and PDF page image renderer are two different packages and some PDFs apparently can confuse these libraries used by Qiqqa into having text box coordinates from one, which don't exactly match the word placement by the other: when you click+drag to highlight in such a document, your "selected text" seems to be (very much) off skelter. There a user can observe the adverse artifacts of using different systems for text extraction and text rendering. Alas, Qiqqa was created in a time when `mupdf` did have a PDF page renderer, but interfacing it to a C#/.NET application in order to properly and speedily obtain PDF page images is not, and certainly was not, a *sine cure*.

- While the PDF Viewer might, at first glance, only be a PDF document viewer, it is in fact a (layered) vector graphics graphical editor at the same time: this ability of the Control is not always enabled everywhere in the program (e.g. when used in the Qiqqa Sniffer), but it is there and must be made available in any port if you wish to keep PDF annotation abilities intact.



## Qiqqa (BibTeX) Sniffer

This window has some powerful logic to scrape the Internet (in particular Google Scholar and several other metadata providing articles search websites) while displaying those web search engine pages in-app: this requires a full-fledged **embedded browser**.

Currently Qiqqa employs the (outdated, obsolete) Mozilla XULrunner technology for that, which cannot even be upgraded to a level where Google Scholar might be able to function properly once again.

Also note that the Microsoft Academy search engine website renders as a total **blank** in the current XULrunner, so there's more to worry about.

Many web sites simply discard the embedded browser (which is akin to FireFox version 33-something ^[Again, check this statement. XULrunner, IIRC, is v33, but is that **FireFox v33** or a slightly later version of the Fox? Anyhow, it's *old*. And modern websites include plenty JavaScript and other systems to disparage users with older browsers from visiting them...]

After having looked at the available .NET options, including the three different CEF (Chromium Embedded Framework) wrappers for .NET Chromium.FX, CEFsharp and what-was-the-name-again-for-that-Mono-bugger?, the only actively maintained wrapper is CEFsharp: it tracks recent Chrome browser releases nicely and seems solid.

However:

- Qiqqa Sniffer employs some special XUL hooks to get its hands on internet accesses before the embedded browser does. This is required capability for:
  + monitoring incoming responses and catching those bibTeX, XML and other metadata records as they come in: those must not be 'downloaded' or shown-as-plaintext like a regular browser would, but instead grabbed by the Qiqqa code and *imported* into the document metadata database.
  + monitoring incoming responses and catching PDF documents as they arrive: these should be collected in the database, instead of merely displayed/downloaded.
  
Note that the current Qiqqa XULrunner solution has a bug/artifact: when you have visited a web page before and the download of a PDF or bibTeX or other metadata record failed before, then XULrunner will fetch the page from cache when you revisit and this will **not trigger** a renewed fetch from the webserver, resulting in a temporary inability to collect/scrape a PDF or piece of metadata. ^[Again, verify. The observed behaviour as described is at least 80% accurate, but the exact cause is a bit ho-hum. Meanwhile, I *do* know with absolute  certainty that the registered callbacks do not fire for any page content that comes out of local browser cache and I have seen it happen *repeatedly* that bibTeX record and/or PDF urls were not revisited, simply because the registered monitor code would not be invoked on a second/subsequent page (re-)visit during a Qiqqa session. The browser cache seems dead and gone when you quit and restart Qiqqa so that was the work-around if you had something really important to grab with the sniffer, only to find out then that the Sniffer **does not keep a page visit history** and in my case it invariably happened to go wrong when you were deep inside a Sniffer session and no way you'ld recall the exact query, position and entry title in Scholar or wherever you are at that moment, so no possibility to revisit that page apart from going through the entire search effort again and hoping the colored (yes, XULrunner *does* color the URLs as **visited** but that knowledge is unavailable/inaccessible from Qiqqa!) URLs are a sufficient hint to get you back on the trail.

Now the embedded browser: given that today, 2020 AD, Chrome is the de facto Web Browser To Rule Them All, any embedded web browser **should be** Chrome/Chromium based, if only  for the fact that we always have had a strong focus on Google Scholar and that particular site turns out to be rather picky about web browsers (and users/usage) AFAICT. If you move away from WPF while staying with C#/.NET (Mono? for Linux?), the question then becomes: is there an actively maintained, up-to-date, wrapper for CEF on your platform and for your particular UI framework (Qt, ...)? If not, then you're as good as dead, porting-wise.
If your goal is to move to, say, Electron (which would be JavaScript/Node, basically), then the question becomes whether there's an Embedded Browser ability there, which would **not** cause Google Scholar to detect a 'bot system' and be totally and utterly obnoxious and unusable, like it is now when you pipe it through XULrunner? 

You (or rather, your **users**!) **must** be able to 
- log in in google via the embedded browser, 
- their login session **must** be tracked/trackable, i.e. walking around the web **must not** loose them their login session with Google, and consequently
- users must be able to complete any 'I am not a bot' captcha thrown up by Google Scholar and friends, while it is **strongly preferred** that the embedded browser is not discernible as such from the outside as Google is **extremely pro-active** when they get an inkling that you are not matching their company strategy: "If the product is free of charge to you, *you* are the product!" -- any bot (like Qiqqa Sniffer might be perceived by Google Inc.) is not amenable to visiting advertisements and other such goody goodies that pay the rent over at Google Central.
  + Note that recent experiments with a bleeding edge CEFsharp did deliver promising results, while sometimes still triggering the anti-machine behaviour of google Scholar et al for reasons I have yet to identify. Google keeps their detection and analysis strategies under lock and key, so all you have to go by are the observable side-effects of their monitoring of your traffic, while they frequently update, or at least *subtly change*, their behaviours, which makes this a tough nut to crack, particularly when you want that nut cracked for the foreseeable future: cracking it is not a one-off, but more like the viruskiller vs virus ongoing arms race.


## I don't know about you, but I have a library that's *huge* and that has... *consequences*!

When you test with a 10 - 100 document library, anything in a UI would be performant. However, when you start testing with a 40K - 50K+ long list of documents, things 'suddenly' slow down to a crawl, UX-wise: the list view of the library **must** have a 'virtual scroll' type behaviour built-in to prevent your UI from screeching to a tormented halt as you force it to render 50K+ custom formatted PDF document entries ^[each document entry displays several items from the metadata as text, while temporary/context-dependent bits like search score (0-100% estimates this is what you were looking for), ranking, etc. are included as well, plus a colorful 'mood bar' that represents the Expedition/Tags category-like overview of said item. ^[hard to explain that one; must refer to the Qiqqa manual for the proper words] All those bits together are rendered in a wide rectangular bar, which means you'll need to have a **custom sortable list view** component available which can cope with 100K custom rendered items **at speed**. ^[ Qiqqa slows down to a crawl due to lookup/render work happening *synchronously* in the UI thread, among other things. Performance profiling has not found the ListView itself to be the culprit but there's plenty around it that together makes your CPU go "unnnng!" for a while there, every time you decide to move/scroll around. Qiqqa, today, does not sport async UI activity throughout the application -- that would be another major refactor to accomplish as I've looked into using C# `async`/`await`, for example, but the current state of the UI code would mean I'll have to do it for all at once (AFAICT) unless I'm okay with (temporary) deterioration of performance while the codebase is refactored to deliver a proper (and stable!) async UX. This, by the way, is another reason why I'm looking at going to Electon: despite my worries over some very important bits, I feel more safe and 'future-proof' there then here in WPF. See also the rant page XXX.] ] ]

So you'll need have a list or grid view like component ready, which 
- can handle large numbers of rows and custom row display (same custom render for each row, though, so that's less of a worry) **and** 
- supports async rendering, i.e. can update the view when the data to render comes in from the worker thread(s).


## Another, though maybe lesser, worry is the desire to move Qiqqa forward and part of this is better UX: faster performance for growing collections

Modern day UIs should have some way to be coded **asynchronously**: once you've dealt with the 'virtual scrolling', etc. performance bits in the UI itself, it's the speed at which the data to the UI can be incoming and Qiqqa is not and will not be *instantaneous* at all times: particularly when working with the bibTeX Sniffer, you will observe in current Qiqqa (and obviously anyone else with any such functionality!) that the stuff you see and collect in the Sniffer (bibTeX and XML metadata, PDF documents which may be new *or* a copy of a document in your library) will have to wait a bit before it is actually available in the library proper and thus added/**visible** in the library list: PDF documents must be collected, hashed and stored in the library, which takes some disk I/O and *takes time*, while incoming metadata must be processed and stored in the database before it's actually part of the library, hence another couple of disk I/O actions *which take time*. Consequently, some or all of a row/record's data is not available at the moment the row is rendered for the first time. This requires async capabilities or you'ld end up with a slew of refresh/rerender actions, **or** your UI (temporarily) slows down to a halt in wait for the data to arrive -- *that* is Qiqqa's current behaviour and it's, well, bloody irritating to me, at least. So do yourself and any users a favor and check async capability before you move.



