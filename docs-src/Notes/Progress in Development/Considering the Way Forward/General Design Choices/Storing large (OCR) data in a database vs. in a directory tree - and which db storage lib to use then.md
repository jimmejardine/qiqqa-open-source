# Storing large (OCR) data in a database vs. in a directory tree <br><sub>(and which db storage lib to use then?)</sub>

Classic Qiqqa stores most larger data in a directory tree, which is a two level, document hash based, affair. This works well and scales to storing 100K+ files, which is what happens when you have a 5K+ documents library: classic Qiqqa stores **at least** 1 OCR file per 20 pages, hence the multiple vs. document count: quite a few documents in my collections span at 30 pages or more. (In a plain vanilla research environment where you *only* store published papers and maybe a published thesis or two, the average page count per document is expected to be lower, but we also keep larger publications (books, etc.) in our libraries, so I assume my average page count is a tad higher than that.
What matters is: does this scale and are there other solutions which may be more effective?

## First: Classic Qiqqa - or what *is* /today/

Qiqqa stores the text extracted from the PDF documents it manages into separate 'ocr' files. Here, there's a few possibilities you will observe in the wild:
- some of these files span 20 pages (or less, if they're the last one that runs until the end of the document) and these are relatively large: every word in the text is accompanied by a ASCII-text-printed (think `printf %f` if that helps) coordinate *box* (4 coordinates) indicating the *position* of the word on the page. 

  > This coordinate box is very important: it is used (among other things) to render an overlay over the rendered page so you can use your mouse and select a chunk of text on the page by click&drag. 
  > x
  > When the extractor has done a *proper job* this will look like you're actually selecting the text you're looking at and all is well in the world.
  > 
  > When, however, there's been a slight misunderstanding, a tiny b0rk b0rk b0rk of sorts, then it can happen that you're observing the reddish coloured selection area happening in the page's whitespace more or less 'nearby'. This, incidentally, is due to Qiqqa using two different subsystems for PDF rendering and processing: SORAX does the page render job, while an old, minimally hacked, muPDF tool does the text & position decoding and extraction legwork. These two fellows *may* have a different opinion about how the current page is supposed to display, resulting in such "why is my selection off to the side or way above my actual text?!" user questions.
  > x
  > > We do *intentionally* neglect the added fact that both subsystems have quite different ideas about the *words* in the text as well, which can be observed when you select text in a PDF and the selection area acts all kinds of 'choppy and fragmented into tiny vertical slices' when you drag your mouse over it to select a chunk of text. This type of behaviour is highly undesirable but requires a complete overhaul of the PDF processing and rendering subsystems. That, however, is a story for another time.
 
  The box coordinates account, at about 8 characters per value plus a separating space, for $(8+1) \times 4 = 36$ characters peer content *word*. Each word is also separated only its own line, but that doesn't matter: NewLine (NL) vs. Space (SP), it's all the same size.

  *Characters* within a word are guestimated at having all the same size for the purpose of visualizing text selection, which is good enough for almost all cases.

- When Qiqqa ran into a spot of trouble during *basic text extract duty*,  i.e. the scrutinized PDF doesn't already come with a nice, usable, text layer itself, *for every page*, then Qiqqa decides to blithely ignore this cruft and kick off an OCR task for all pages in the (20 pages') range. 

  The product of this variation is one OCR text file *per page*.

  Thus we can deduce where OCR activity happened in the past: those 20-page ranges are represented by 20 files (or fewer, when it's thee end of the document) instead of just 1(one).

  The *cute bit* to know here is that when Qiqqa encounters a page with no text or very little text at all, f.e. a full page graphic / diagram, those same "did it go well" heuristic analysis *will* decide this is on par with a "no text layer at all" situation and kick off the OCR machine for every page in the range, no holds barred.

  > Hence you will be able to observe some documents having *some* 20-page text/ocr files, while other page ranges in the very same document present as one-file-per-page, without obvious logic -- until you realize there's a full page graphic or an *actual empty page* in that part of the PDF. Given that OCR (done by tesseract version 3) isn't always up to snuff, you can then observe suddenly lower quality content appearing in some pages of the full document text representation, when you export to text.

- Next to this ream of text files, Qiqqa will also store the actual page count of the document in its own tiny text file.

Consequently, the number of files per document in the `/ocr/` Qiqqa directory tree will bee between $2$ and $1 + p$ where $p$ is the actual page count.



## Second: Future Qiqqa - or what *might be* /tomorrow/

First, note that we've been looking at the various (technical) aspects of the various text extraction and page rendering quirks and issues. SORAX was commercial is now defunct and *gone*. muPDF is still thriving (the same company does a great job publishing GhostScript, by the way) and has slowly improved over the years as PDFs get even more wild and crazy complicated.

So the idea there is to use muPDF for *everything*: both rendering and text extraction, etc., so that any discussion about *position within the page* can be reduced to a single subsystem's *software bug report* and those pesky off-by-a-lot text selection visual feedbacks can become a thing of the past.
Meanwhile, muPDF is still able to do the text extract job. And with progress come new formats to dump this info in, f.e. hOCR, which is a derivative of good old HTML (plus extra attributes and stuff to keep a tracking record of those positioning boxes we mentioned). 
Default hOCR, however, does the bbox (*bounding box*) coordinate thing *per character*, which can quickly bloat your output, so **maybe** we should look into some Smart Alec optimizations there to get a nice *storage space reduction*.

> Yes, I've considered using different storage formats, including some binary raw data based ones, which would cut down on our parse-upon-reading costs when rendering or analyzing an already processed page in Qiqqa, but I'm still on the fence about that one: having a standard format like hOCR allows us to also look at such page extracts in regular tools **outside Qiqqa**, e.g. a plain web browser! 
> While binary storage of the bbox integers (or `float`s, if you're going for the (0,0) -> (1,1) page coordinate space as employed by Qiqqa) is *cool* and storage space saving -- plus very much parse costs reducing during the multiple times you're expected to *read* this file any time later -- you'll need to do extra tooling to make the "also viewable in external tools" option viable again.
> 
> > Reminder to self: while we're still on the fence, may I interest you in SQLite *custom functions* support, which means you can have cool stuff like `base58x(blob)` and `decode_as_hOCR(blob)` next to standard `hex(blob)` and such-like? All of this at the low price of an API call -- and the consequent requirement that everyone use your customized SQLite tool if they want the Full Monty of under-the-hood *formatted* data access: 
> > - [Application-Defined SQL Functions (sqlite.org)](https://www.sqlite.org/appfunc.html)
> > - [User-defined functions - Microsoft.Data.Sqlite | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/user-defined-functions)
> > - [Create Or Redefine SQL Functions (sqlite.org)](https://sqlite.org/c3ref/create_function.html)
> > - [How to create custom functions in SQLite - Stack Overflow](https://stackoverflow.com/questions/2108870/how-to-create-custom-functions-in-sqlite)
> >  
> >  That way we should be able to have both: binary storage of hOCR-alike data, including raw bbox coordinates (so reduced parse costs on repeated reads), while external users (and our own FTS/SOLR search engine subsystem perhaps as well!) can use statements like `SELECT as_hOCR(text_extract) FROM ocr_table ...` to get easy to use hOCR-formatted HTML pages.
> >  
> >  > Which leaves the images (diagrams, formulas, etc.) embedded in the original PDF: we *should* have separated those out at the `ocr` file / database storage level already. Thus we can have that `ocr` store as the backend of a regular web server, which can then produce every PDF as hOCR/HTML web page.
> >  > Going nuts with this, that same *web server* can use the same (or similar) mechanisms to produce crisp fully  rendered page images *alongside* as an alternative for both thee Qiqqa UI and others to see the original PDF, (pre)rendered as high rez page images. The idea there being that we need a page image renderer anyway (to replace the SORAX one) and it being a *smart* choice to have to page images *cached* and (at least partly) pre-rendered for a smoother viewing and PDF document scrolling experience, both inside and outside Qiqqa.
> >  > 
> >  > #### PDF page renders: caching the images - for how long and at what resolutions?
> >  > 
> >  > For regular Qiqqa use we need two resolutions: one for side panel thumbnails and one for full image page reader views. Given a 4K monitor screen and a 'full-screened' application window, that'ld be worst-case 4K wide page images for the reader.
> >  > As this gets hefty pretty quickly, this should be treated in a rather more **on demand dynamic way**: why not render these on demand only and at the requested resolution if no larger rez is available yet in the cache? While a minimum rez for any request would perhaps be equivalent to 120..200ppi, so we can always serve basic views and thumbnails of any size by rescaling and serving viewers and OCR tools (tesseract!) as needed with a minimum amount of (costly) re-rendering activity. 
> >  > Also restrict re-rendering decisions to 10..20% size increases at minimum to reduce the number of re-render actions; maybe go even as far as to state that any re-render will increase the render size by 50% at a minimum, so any further demand within that range gets a rescale of that image or the image itself, iff the caller can provide more suitable rescaling facilities itself (an option that would be useful for web browsers, f.e.).
> >  > 
> >  > #### That leaves the question of "okay, fine, we cache those images. For how long?!"
> >  > 
> >  > As we intend to compress those images using WebP format (the best performing image format that's supported by all modern web browsers and several other tools outside Qiqqa - AVIF would have been a candidate as well, but there's little support for that one yet and a re-encode on every read access is costly enough for basic hardware that we rather do without):
> >  >   - ["avif webp jpeg-xl" | Can I use... Support tables for HTML5, CSS3, etc](https://caniuse.com/?search=avif%20webp%20jpeg-xl)
> >  >   - [paambaati/avif-benchmark: 🏋🏽‍♀️ Benchmarking sharp's AVIF vs WebP conversion (github.com)](https://github.com/paambaati/avif-benchmark)
> >  >   - [sharp - High performance Node.js image processing (pixelplumbing.com)](https://sharp.pixelplumbing.com/)
> >  >   - [libvips/libvips: A fast image processing library with low memory needs. (github.com)](https://github.com/libvips/libvips)
> >  >   - [libvips/nip2: A spreadsheet-like GUI for libvips. (github.com)](https://github.com/libvips/nip2) -- off-topic here, but one option pointing me at the sanity of my idea to have an image processing GUI sit between the PDFs and their OCR engine (tesseract) to allow users to customize their page images before OCR-ing them: think page cleanup, custom thresholding and the like.
> >  >   - [Benchmarks · libvips/libvips Wiki (github.com)](https://github.com/libvips/libvips/wiki/Benchmarks)
> >  > 
> >  > 
> >  







