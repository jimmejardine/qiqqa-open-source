# Qiqqa Sniffer:: Design Goals (*Intent*) & *UX* :: The Good, The Bad and ...

## The Purpose of the Sniffer

Serves multiple purposes / tasks:

- **seek & import documents into the library**: using mostly the web browser + web search parts, we can dig up documents of interest on the Internet and have those imported automatically (downloading them). This is *one* of several ways to import documents into your library.
- **document metadata gathering**: using both the "current document" view and web search parts of thee UI, we can dig up and import (BibTeX-based) metadata bout the documents we already have in our library.

  This is *an integral part of the seek & import process above* whenever possible, but is to be considered a separate task as document import and metadata import are separate user actions while we remain in the same screen / interface to help make this combo of actions as smooth and fast as possible.
  
- **document metadata vetting & correcting**: the metadata editor part of the UI allows us to vet existing metadata records and augment / correct these in two ways:
 
  - importing metadata records from internet sites (e.g. importing Google Scholar BibTeX files)
  - manually completing / correcting the metadata record we have.

## The Good

The idea and execution is fine. It works well -- when the components are not hampered by external politics and technological issues: see The Bad section below.

Having all component panes together in a single-screen "dashboard" like this speeds up our work tremendously for all purposes listed above.


## The Bad
The Sniffer UX is rotten, thanks to both

- debilitating technical debt issues: obsoleted, starkly outmoded and defunct components (primarily SORAX & XULrunner) introduce a slew of failures to perform on technical grounds. Often, doing any work is downright *impossible*. The old cruft is also keeping us locked in 32bit build mode, which prevents replacing several unrelated components as well. It's a quagmire and has been the driving force behind considering a multi-process rewrite of the software -- a very risky and costly endeavor for any sizable product!
- external political choices severely hampering our ability to gather (meta)data: Google Scholar, for example, has made metadata gathering in small or large batches neigh impossible by now by throwing up all kinds of barriers to (continued) access. Where previously ready-for-import bibTeX records were relatively easy to download, those are now very hard to access, if at all. And that's *apart from our total inability to get at them today (2022 AD) thanks to our tech debt issues*.

Further critique:

- the metadata editor is a bit of a hacked together horror -- I myself having contributed a lot to that horror thanks to a quick hack to introduce field-based data entry next to raw bibTeX record editing. The metadata editor pane should be improved across the entire application.
- metadata parsing (bibTeX parsing) is subpar at times, both for diacritics handling and processing more advanced (macro carrying) bibTeX records -- though the latter are quite rare in the wild, in my experience.


## Further Notes / Future Music

The general layout of the Sniffer screen is good and should be kept as-is.

Few notes about some of the (non-technical) details:

- would be lovely to have an easy way to add new search engines in the control panel. Now that set is fixed, more or less, and we can disable or enable their inclusion in the overview list. I've had several times during my use of Qiqqa that this was a drawback. "*Oh, wouldn't it be nice if I can permanently this search / indexing site here, now that I ran into it by happenstance!*"
- the metadata editor is a UX nightmare and can do with an overhaul. Also do NOT EVER obscure metadata with (semi-transparant) overlays for metadata record vetting control (or anything else, for that matter). Make that stuff a separate chunk, maybe near the *step-through controls* (i.e. next/previous document + document list filter controls)

On the technology front we're facing an ever-changing and generally low quality sourcing of metadata: Google Scholar records were (and rare) often of low quality: under the hood there's a lot of struggle going on to turn it into a useful stream. Hence the importance of having a UI which gives you sped and ease of use when you're own *Mechanical Turk*. (Gathering, checking, correcting and completing the metadata collected, that is.)

Thee latter implies that any future design should allow for some means to make this an easily **adaptive** process, either through scripting or plugins. Or using any other means enabling users to customize their extraction and filtration processes.

For me, the Sniffer is the most powerful component of Qiqqa entirely -- search and document analysis, while important as well, all strongly depend on the *quality* and *speed* of the metadata stream which can be produced by this Qiqqa component. In other words: the Sniffer enables you to be your own Mechanical Turk as the means to improve the quality of your document metadata -- ultimately that's the only way I believe available to us to improve our data. Having "smart" AI and "pool sharing" mechanisms ^[Quantisle had a silent server running for a *very long time* which collected your bibTeX metadata per document hash; I haven't noticed it *also giving back*, but that's the obvious next step: by pooling our collective work like that we can have improved metadata without having to do it all ourselves: this makes the system *scalable*. Of course you still should be able to vet the incoming metadata and adjust what you don't like, so we can expect *multiple versions/revisions* of the metadata collected per document like that, but it's a big step up from having to rely on the Google Scholar (and others') crappy baseline. Naturally, when you consider this realm, paywalls and revenue streams for providing *Turked* high quality metadata quickly appear into our field of view and will hinder progress -- data *may want to be free*, but fact of the matter is *data is our slave forever*, alas.]







