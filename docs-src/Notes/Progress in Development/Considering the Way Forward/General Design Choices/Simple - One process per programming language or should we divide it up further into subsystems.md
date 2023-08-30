# Simple - One process per programming language or should we divide it up further into subsystems?

The initial idea was to have one monolithic process per programming language -- the intent being the separation of concerns in communications across language boundaries, no more P/Invoke and related stuff (such as one several parts forcing us down to 32bit and keeping us there unless we upgrade *wholesale*) -- which nicely coincided with the architectural subsystems we identified (with the C/C++ monolith serving several purposes instead of just *one*).

Today, I wonder if the C/C++ monolith shouldn't be divided into a few separate processes, each serving one architectural purpose (i.e. subsystem):

- metadata + data storage (database)
	- *possibly mixing in the FTS capability via FTS5 or manticore if we don't go the SOLR route after all*
- PDF processing and rendering, including LDA keyword/category extraction, OCR and text / metadata extraction
	- *possibly including rendered page image caching (second -- ephemeral? -- database), OCR/text extract caching (`/ocr/` directory tree or database)*
- ...misc...

The grounds for this being that we know we have some *huge* PDFs, which will explode the heap memory need at the time of rendering and PDF processing, because muPDF always loads the entire PDF. This would be a serious stability risk (OutOfMemory-induced forced terminations) for the entire monolith, while we already have been worrying over how to manage this (and other) types of "wild behaviour" one can expect in the PDF processing section.

While we do realize this would increase the inter-process data transfer costs significantly as we'll be pumping large quantities of PDF page data (image and text) across to other subsystems (SOLR or FT5/manticore), separating this subsystem into its own process would mean we can expect a higher overall system reliability from scratch: the databases and other processes can remain operational while aborted/crashed/terminated PDF/
OCR processes are restarted under the hood (by the Qiqqa Monitor application).

Meanwhile, critical path user facing data comms will have the same or comparable costs: one of the CPU-heaviest, data-intensive tasks is rendering PDF pages: the UX is, of course, immediately impacted by the performance of that sub-task. It does not matter for total system latency and costs whether subsystem A (the previously envisioned database+ monolith) or subsystem B (PDF page renderer + cache) delivers the image data to show on screen. Ditto for the equally important text+position PDF page info stream: in the new design it will be still 1(one) localhost TCP transfer away.

Added costs are to be found in the background batch processes (PDF page text copied over to the cache and FTS systems) as those now will need to communicate through localhost sockets or memory-mapped I/O instead of a simple pointer reference to internal heap memory.

The other localhost socket-based communication costs are expected to be slightly larger as well as more command and metadata messages will have to travel through a localhost socket between the subsystems (instead of using a memory reference transfer for that purpose internally).


## Conclusion / Decision

Nevertheless, I think it would be good to think of "*one process per architectural subsystem*" from now on. This means we'll have a few more processes (be very conservative = *don't go crazy* on the number of abstractions, as usual) than we imagined before, but this gives us a much lower risk (stress) level re overall application stability. Which was, after all, one of the main driving reasons to start considering the migration/rebuild.


## Post Scriptum

Each of the subsystems can be its own web-server then for when we code the UI as a web browser:

- main Qiqqa metadata database -> static web content templates + metadata filled in the views = web pages. 
- PDF renderer / processor -> cached document content, serves as a CDN when it comes to displaying rendered PDF document pages and text(ified) content. 
	- SHOULD accept edits from the user at one point (Qiqqa currently does not have this ability) so we can *Mechanical Turk* our document texts to improve their content quality --> this would then have to result in FTS index *updates* for the given document.
	- Ditto for (user produced) annotations, abstract, etc., whether those are produced by heuristic automatons or hand-fed and hand-vetted by the user herself -> FTS and metadata database updates would result from this as well.
	- From this perspective it doesn't matter whether we integrate the FTS engine in the main (metadata) database subsystem (FTS5/manticore) or go the SOLR route: it'll be 1 socket data transfer away from the producer, anyway.

It would be good for purposes of task priority management flexibility to keep all those queued batch tasks (render page, extract text, OCR page, extract keywords, etc.) still in a single *process*, so we can add a minimally cumbersome priority management control onto that one in order to manage thee relative processing priorities of the various batch and real-time tasks requested by thee GUI and other subsystems. Managing those heavy work threads across multiple processes won't be so easy, so we better not split that stuff up and rather accept/tolerate the data transfer costs (which we would have had anyway -- for the most part).

