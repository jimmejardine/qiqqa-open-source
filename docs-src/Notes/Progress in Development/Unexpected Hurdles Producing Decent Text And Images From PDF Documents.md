# Unexpected Hurdles Producing Decent Text And Images From PDF Documents

This document describes several surprising artifacts discovered in many PDFs out there available online, which make the processes of decent quality text and image (figure / graph) extraction all that much harder! These nasties are described on a case-by-case basis. 

> ## TODO: categorize/rubricate these and cluster them by type?
>
> ## TODO: cluster them by *troublesomeness* ranking of some sort?
>
> Some bothersome aspects are probably easier to overcome than others. May be useful to rank and order these buggers by their *Degree Of Bothersome* or *whatchammeyecallit*?




## Captain Obvious Abound: PDFs which are a set of page images

**This is about PDFs which look to be _in mint condition_ when viewed on screen or in print. [[#Major Malorum PH Ret OG PDFs which are a set of scanned page images|See the other sections for the _others_.]]**

Some do it to “protect” against copy/pasting their texts. (Those are rather *rare*, it seems.)

Quote: “[Never attribute to malice that which is adequately explained by stupidity or indolence.](https://quoteinvestigator.com/2016/12/30/not-malice/#return-note-15115-9)”

Some, it seems, do it out of sheer laziness (I think the word “indolence” fits this one to a tee): when the design department likes their pixel-perfect magazine or brochure output enough to ignore some SEO benefits that come from PDFs with their *copy* (i.e. *text*) embedded. I’ve seen this done to brochures, magazines and above all: *books*: while one might want a brochure or magazine PDF to serve some SEO purpose by itself (“we want this to be easy to find by google and friends”?), *books* often come with “cover web pages” where they are introduced, together with their publisher, author and maybe some other (meta-)data, thus making the book copy itself much less relevant re any “making sure people can find this” goal.

> I’m **not** talking about the, *äh*, *repropriated* copies we may discover in the murkier crevices of on-line search databases, but publisher/author-provided authorized instances *only* here: from downloadable magazine issues and commercial product brochures to art house ebooks – in one instance amounting to a whopping 600MB+ *übersized* PDF which apparently was the very same file they fed their printer. *Gorgeous* hardcopy, that one, by the way. And a PDF including overprint high rez TIFFs (for edge-to-edge image printing) and a few other tidbits that only a high-end quality printer can truly appreciate.
>
> And then there were those instances where I suspect their InDesign or Quark jockeys had had some debilitating headaches getting the pages laid out as demanded and left it at that once it got approved, resulting in pages which were strangely **partly** or *entirely* rendered as *character shapes*, which is just another way to nuke your text extraction for the sake of render/view/print output quality. 
> Been there myself in days gone by. (Lost the T-shirt though. #sadface.) 

While the original authors don’t have to resort to crutches such as OCR to have their PDF/document be present in text form, I’ve seen quite a few math books (or books which contained quite a bit of *math* while discussing some technology area) go this route of rendering all pages as high-rez pixel images. Just to make sure the math looks perfect anywhere, I guess.


## Constable Horroratio Obnoxious: PDFs which carry *characters as shapes*

**This is about PDFs which look to be _in mint condition_ when viewed on screen or in print. See the other sections for the _others_.**

Similar to the [Captain Obvious Abound](#CaptainObviousAbound) image-based pages, yet *so very* different. A double bother!

Much rarer are these ones which hurt text extraction as bad – or even worse: I’ll get to that!

Those which have their text (and possibly everything else that’s not *pixels* already when imported in their DTP package of choice) converted to **shapes**: while a printer won’t mind nor would a screen viewer (a.k.a. PDF renderer) balk, any *semblance* of text is permanently eradicated from the PDF, so we’ld have to reach to reach for that OCR toolkit once more.

“Same thing as images!” you say?

Pardon me, my dear chap. These are, in a sense, *worse*. Allow me to mansplain this bother.

While *images* can be easily detected as such in the PDF data streams – where I smoothly ignore **image strips** until a later entry – you’ll need quite a bit more sophistication to address **characters as shapes** PDF output: it is not the *shapes* per sé, but the fact that *charts* and other *data graphs* also may appear as a bunch of *shapes* instead of a mere pixeled image. And that in the same document as the *character shapes*!

So you are down to applying some (smart?) heuristics to help your automaton decide what this particular blob of shapes represents: another data graph or a chunk of text with maybe a few odd characters, but plain reams of type otherwise? **We are talking applying _computer vision_ technology now, which is all jolly well but another major obstacle _apart_ from OCR when you’re concerned with optimal (“near original”) text extraction quality!** 




## Zorro: when the mask’s the *thang*

**This is about PDFs which look to be _in mint condition_ when viewed on screen or in print. See the other sections for the _others_.**

I still wonder if this is a setting in `dvips` or other PDF-excreting tools somewhere: I have encountered *several* PDFs (but not enough to announce a clear pattern yet) where the text was rendered as images. 

*Duh*!

*Uh Unh!* No sir! Not the easy-peasy embedded TIFF, PNG or JPEG ones! I’m talking about those (scientific papers all, BTW) where, when you tool up and extract the images, you’re looking at **100% black fill** center stage! Turns out these PDFs had *additional* images serving as **image masks** and the text (and graphics; and formula’s) were rendered in the *masks*, rather than the content images themselves.

Funny really: you take a black sheet and define a mask where the black is allowed to “shine through”, so to say, and you end up with a page which looks perfectly fine in a screen reader or in print.

> I’ve also noticed there are variants to this theme (while I was busy getting hOCR formatted extracts from PDFs): some PDFs don’t settle for a single image per page that way, but some go the extra mile and apparently transport their layout stack into the output that way: pages which turned up as a bunch of tiny and large-ish black images sprinkled throughout the page, which upon further scrutiny turned out to constitute the page’s paragraphs and data graphics, *all of it encoded in the masks, one for each little black blob image*. Here the graphs (and electronic schematics, for it was electronics being discussed in those PDFs) clocking in at a single teensy tiny image per **element**, making for a couple of hundred images **plus masks** for the data graph and schematics alone on that one page.

Of course, the basic answer for this, as with mostly all else on this page, is forego the “page layout extraction” techniques based on original PDF data and shoulder your trusty ol’ *elephant gun*, take aim and pull the trigger: rendering each page as a single page (as one would do for a PDF screen viewer) and then hope the OCR engine internals will sort it out, paragraph text reams, charts, schematics an’ all.

> When that’s your standard answer to everything, you’ll be surprised to learn that these pages are often very hard to *grok* for the page layout heuristics embedded in OCR engines (such as `tesseract`) and your text extraction quality rates will plummet… I don’t have an answer to that one yet, apart from “[bugger it, millennium hand & shrimp!](http://www.nblogn.com/2015/03/bugrit-bugrit-millenium-hand-and-shrimp/)”







## Scanlines Is So Retro: Cool & The PDF Gang

**This is about PDFs which look to be _in mint condition_ when viewed on screen or in print. See the other sections for the _others_.**

And there you are! You thought we were done with *image based PDFs*, eh?

Нет товарища, мы все еще не приехали на дно кроличья дыра.
If you thought images are these rectangular areas ablaze with pixels, you’re close but *no cigar*. Not this time anyway.

Turns out some PDFs do their image rendering a *scanline* at a time and, boy, let they know it in their output.

This means that a regular picture image of, say, 100pt height, would be presented as 100 (sic!) images of 1pt height each. Again, not a problem when you *render* the page for viewing or printing, but when you think you can do some easy, simple content analysis on the PDF to help improve text extraction rates, you’re in for a surprise there: turns out these PDFs come in triple flavours *at least*:

- the *easy* ones are where the text is still text (phew!) but the images are *striped* (“scanlined”) and it’s up to you and your heuristics to combine adjacent image strips into a single (rectangular) picture where applicable.
- then there’s a variation on this theme for arbitrary shaped images, e.g. a camel which had its background masked entirely: in that case you’ll get the *full scanline treatment*: that one camel will show up as a slice of ears, then slices of the head (still the images will be adjacent vertically but their horizontal offsets will shift, *obviously*) and then not-so-surprisingly you get the hump of the dromedary showing up in the sequence as that would be the next this the *scanline* would hit while zipping across the page, right?
   
   If you’re into extracting *images* alongside the page text, you’ve got a bit more work to do in your picture recombination heuristic logic routine right there.
   
   Of course, when multiple pictures left & right on the page are “hit” by the scanline, their 1pt *strips* will show up in that order, so don’t go haring off and merge all strips in an image series with impunity, for you may get some surprising distorted babies out of that.
   
- You can guess already? Yeah. Same thing for reams of text rendered as images: they get the same 1pt strip treatment some times, so it ain’t “*take one picture off the page and feed that to the OCR engine*” for you!










## Crypto-mangling your PDF for *what* purpose exactly?

**This is about PDFs which look to be _in mint condition_ when viewed on screen or in print. See the other sections for the _others_.**

While I’ve seen this procedure advertised on the web as the answer to blocking text copying (“plagiarism”) off your publication, this turns out to be pretty rare, at least in my practice. 

What I’ve seen a lot though, which behaves the same from a *technical perspective*, are older TeX/dvi PDFs produced with something that wasn’t aware of Good Things™ like Unicode or TrueType / OpenType fonts: I suspect these are old / antique TeX/MetaFONT productions where the character maps are somehow screwed up to infinity: the text is text but comes out [gŏb′əl-dē-goo͝k](https://www.merriam-webster.com/dictionary/gobbledygook) due to a screwed/mangled character mapping in the font(s) used in the publication. I’ve encountered this almost exclusively with (very) old PDFs, produced back in the ‘80s.

Again the blunt answer is rendering to image for “viewing” and then feeding that to your OCR engine.
If you want better results, you might go and take that output and probability-map it onto the gobbledygook output you got earlier to produce a character translation map for the font(s) at hand. I haven’t tried that one myself yet and I should mention this rides on a particular promise: all fonts used in the page are assumed to adhere to the same ill character mapping. Which might be true for old MetaFONT/TeX/dvi combo’s but wouldn’t fly for any “encrypted” PDF which has been mangled *intentionally*.

YMMV.




## Major Malorum (PH)(Ret.)(OG): PDFs which are a set of *scanned* page images

Here we got the granddaddy of them all: page scans of various vintages, quality and *registration*: anything goes.

We’re talking yellowed pages stored in their scanned, off-white colors.
We’re talking page images which were done with mediocre equipment and/or techniques, producing low-rez images for each page. (Mostly due to the human behind the machine dialing up the JPEG compression because *he can*.)
We’re talking page images apparently pulled from Xerox machines and other pre-computer-digital-imaging eras where you’ll be treated to black ridges, pages skewed and otherwise distorted as the pages weren’t thoroughly flattened against the plate, etcetera etcetera.
We’re talking *faded* scans or *truly antique manuscripts* which were scanned to preserve them in a digital arena, no matter how faded, gnawed on or otherwise deteriorated they already are (including pages which were pulled from a fire before being scanned).

The hardest to process are those PDFs where some previous *technician* has been *helpful* (DON’T!!!1!):

- by postprocessing the already faded image with a rough&ready thresholding algorithm i.e. turning the pages to pure black&white to “help” PDF file size. You’ll have a field day OCR-ing those, my friend!
- by cropping the pages to their content size. This includes PDFs which cropped off important text because someone was in a hurry and didn’t check their (already botched) registration, resulting in such fun things as (1) chopped off content, lost *forever*, (2) 1.5 pages (yes, that’s *1½ page*!) in the shot, because a book got scanned but didn’t completely fit on the plate and it’s up to you to find ways to de-skew and then clip off the half page that doesn’t belong in the given page image, (3) pages which have *exactly* trimmed to the very edge of the printed word, which `tesseract`(and others) **do not like at all**: you’ll have to regenerate a new page border in the matching background color for the given scan.
- *scaling* the page scans to a varying degree, based on some haphazard algorithm available under some clickable software button some place, resulting in pages where the already mediocre scans, deteriorated by *enthousiastic* (or should I say *zealous*?) JPEG compression settings and non-integer scaling factors combined with older, poor choice, *worse*, scaling algorithms, are to be your inputs for some valiant OCR efforts. 
- … anything, really … (the amount of crap I’ve seen...)

If you guestimate elevated blood pressure, adrenaline levels soaring, flushed skin and a wide eyed look with jaw muscles working out crunching the remaining tooth enamel to a fine paste on the other side of our connect, you’re *pretty warm*. I like a good rant at times. Flushes the valves, unclots the cholesterol quite nicely.

Keep in mind though that many of those (mostly older) PDFs were done at then best-effort-in-a-hurry: the human race has never been very good at preserving its history, being more akin to the other animals then we like to know.

While most other PDF curiosities I’ve encountered would nicely fit the “indolence” criterium re human behaviour (and thus: human software engineering), this one also represents be-glad-you-got-a-digital-copy-at-all well-meaning preservation efforts, using the tools and time available. Again, you’ll have your text extraction work (and thus tweaking your OCR toolchain) cut out for you. The worst results which may be important to you might even need some Mechanical Turk postprocessing. 

> This category is present in significant numbers in my own collections and have yet to see organized effort to OCR/process these in any manner that might **reproducible**: “reproducible”because I ultimately want a process which can re-run using the same (stored) settings after I wipe a test run, improve or fiddle with some bit of the process, all for the goal of getting at a workable (semi)automated process.
>
> As such, ideas like storing a “OCR toolchain setup configuration or script” on a per-file basis has been floating around my brain. Here *von Moltke* applies as usual: “no plan survives contact with the enemy”. And right now this is just a *plan*. We’ll see what I come up with once this hits field tests.





## “Organized” PDFs: anything special there?

Not really.

Of course there’s plenty crap coming from “disreputable sources”, but AFAICT the particulars are limited to obnoxious headers and/or footers and/or site ad “cover pages” added to otherwise okay PDF content.

###  Electronics/datasheets however! Don’t get me started!

If you want real crap, look no further than electronic datasheets from **reputable sources**: I experienced my first crap-up of (then *commercial*) Qiqqa when I had loaded it with a couple of electronic datasheets from American manufacturers.

While the Chinese chip *Blätter* are often *extremely* light on *content*, it is the Americans that add a shitload of crap to make sure their *acquisitions* of other firms don’t go unnoticed. (I’m looking at you, On-Semi!) This includes *cover pages of no value what-so-ever* spam-merged with every PDF they put online. And when you’re lucky multiple CEOs will have had their own dick-swinging contest along the way, shark eating shark, producing PDF beauties with up to three(3!) cover page sets and “watermarks” added to PDFs for some older ICs.

Also reckon with some surprise *tail pages* having been added to such content (electronic datasheets), though fluffing up the backside like that is quite rare compared to adding all kinds of odd- or even-sized *cover pages* to the same.

Another thing you will run into there (still talking electronic datasheets – but *do* add application notes for electronics to your list of usual suspects!) is “watermarked” PDFs. These come in a couple of flavours:

1. the obvious kind where a watermark text is printed across the page, either behind or in front of the original text. Think words like “Preprint” for papers and “Obsoleted” or “Not for new designs” in the case of electronics.
2. the other obvious kind where the watermark is an image rather than a bit of text. Ditto for vector logos i.e. “shapes”. Sometimes in their own layer, sometimes added to the content stream as-is.
3. the obnoxious rotten kind: watermark text which is “booleaned” with the text, partly obscuring or damaging it: this is rare but I have seen a couple pass through my system where the original text-based PDF was converted to a **shapes-based** page as the watermark (grey/blue) was *booleaned* with the original text (black) producing a mostly-legible text in rendered view, but clearly badly damaged, failing all basic text extraction attempts – this is how I discovered them as (*commercial*) Qiqqa was unable to extract anything sensible and these showed up as part of a PDF list I filtered from my library and fed into Qiqqa Sniffer to see what was going on with the PDFs that would not produce any text nor had any BibTeX metadata.




## In Closing…

Alas, Qiqqa still hasn’t any means to help us cope with these bozos any better than before. As I said before: this is *Future Music* you’re listening to.

The above is the result of a long empirical personal trail so YMMV. While I believe I’ve seen most of the patterns that make PDFs such a bother for processes other than basic *screen viewing* (*note* that I didn’t say “screen _reading_”!) there’s always another surprise lurking around the next corner.
To give you a bit of an idea about that: while I was testing the *robustness* of my `mupdf`basic tools’ build by running some very simple commands over a rough collected bunch of PDFs I had collected from basically *anywhere*, I went to bed one day – it happens! – to return next morning discovering the machine had been waiting patiently for me to acknowledge a nasty crash. Turns out that this was a bug in the mupdf software which was only triggered by a single(!) PDF in a collection of 90.000 buggers collected from all around. Plus it took a specific command to trigger the bug!

1 : 90000 you say? 
Why bother? 
Because I care. 
I’ve been around too many places where people were exhibiting that *exact same* indolence, calling it “*efficiency*” to cover for it in their minds, resulting in very rare *way*-out-of-bounds results in their financial calculations. Regrettably these triggered payouts that went unmonitored. Only minimal loss of revenue, that one.
Others with that same *efficient* mindset at an insurance company had their own Meet&Greet with the Lord Murphy when they had a fatal server park failure, resulting in two entire weeks of financial records completely *wiped*, including *all* backups – that includes *remotely stored* backups! (The backup system had an undiscovered flaw because their *efficiency* did not allow them to plan and run a “failure mode exercise” any time during the year so the defective hardware was not discovered until it was too late. The “failure mode exercise” had been deemed “too expensive” by the board, which were focused on *conversion ratios* and other KPIs to boost their stock options. The manual recovery took an all-hands-on-deck 2 *months* effort, recall *everyone*, no holidays, no weekends, *crunch*, including some serious Mechanical Turk efforts as the paper records had to be re-entered and manually updated in a race with the restored servers, while the math wizards had a *no R&R* clamp-down as these efforts had to be monitored and *verified* to prevent showing up on the Failure Radar of both institutional and consumer organizations.
While a major new shipping yard was introduced, the architects had neglected a totally worthless piece of *solid engineering* – this included all lead engineers and the entire software team, BTW. Is indolence contagious? I believe it is – this little datum turned out to be a complete showstopper at the kick-off party as it would cause a traffic jam that would make national headlines, guaranteed. Simple. Electronic access keys *can* fail. In the hands, and particularly when in our modern wallets and tucked in the “ass pockets” of your jeans, they *do* fail, *massively*. Now *that* particular flavour of *massively* is not precisely know but empirical data collected so far says it’s 1:100000 or *worse*. Combine that “almost *zero*” number with a *great* looking site design where nobody had accounted for a *single* access token failure (nor, unsurprisingly enough, had asked a what-if question about it), add a bit of human-in-rush-hour mentality when you just got yourself a “this cannot happen” failure event and you got yourself a friggin’ warzone in no time flat. So the bubbles went back into the bottles, the nibbles forgotten and some “unexpected delay” in the opening of the new site from a spokes-person.
Shall I go on?
Closer to home:
Current Qiqqa tools (including a very old copy of `mudraw`used for all fundamental text extraction work right now) completely lock up when you feed it only *one* of a certain set of these buggers: that one PDF is not “generated” by a “fuzzer” or anything else remotely *artificial*: it’s coming straight off a reputable scientific publication server and completely freezes the background operations, rendering your Qiqqa install totally dysfunctional. Forever. Until you find the culprit and remove it manually, diagnostics requiring a technical level of *savvy* that’s not widely available.

While I may be known to some folks as the “It compiles, hence it ships!” guy, that same guy (me!) will proclaim that motto for shock effect while making damn sure I did my utmost to test all the fringe cases, *particularly* for those areas of the process where I cannot fix or circumvent trouble *easily*. “Backend (automated) processes” such as text extraction are complex. With Qiqqa, lacking a test team I can motivate to break my stuff a thousand-fold with *any* nastiness they can come up with, the best I’ve got are bulk tests with inputs that I did not pick myself – I don’t have hard data to back this up, but empirical evidence has shown I *need* someone *else* to test my produce for real, because somehow I am *unconsciously* able to *not* hit hidden stumbling blocks in my own designs, even when I’m running 100% coverage tests (pop quiz: what does a 100% coverage test *never* cover?) Given this state of affairs I’ld say 1:90000 MTBF is *cute*, but *not production ready*. 0:90000 MTBF sounds *much* better to me. 
The Known Unknowns and Unknown Unknowns. And the old lifetime test charts from IBM: how far the exponential effort ladder am I willing to go? It depends, but in this case: pretty high.


This is just me writing down these items lest I forget. And who knows, maybe someone else will benefit as well. Or even better yet: will feel tempted to try their hand at mechanising these processes for the benefit of all of us!  

Don’t mind me: while I like this sort of challenge myself, I’m still busy getting the general basics in order for the Long March™ of Qiqqa so I will be pretty busy nearby.  Wink wink, nudge nudge. 


