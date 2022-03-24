# What *is* Qiqqa doing for me? What is Qiqqa *going to do* for me?

> Written/braindumped while having come to an unfortunate *final* conclusion re Lucene.NET.
>
> ## TODO
>
> - Organize the stream-of-cons material below: 4 short items of Qiqqa processes (which I only identified while writing this; before this moment I felt my thoughts about *three** processes were both very inaccurate and exhibited a lot of "dithering about it", shall we say? (Can't recall the correct English term for the semi-random set I had: that bloody Chicago prof is so bloody right about writing it *hurts*!)
>
> - While I like the rhythm of what came out, the music should be moved into four *movements* (sections); consider making that *five* as the fifth isn't exactly *reminiscing* (**wrong word**!) but is rather *meta* as my college nerds would say.
>
> - This is about the section below that's `>` blockquoted; the part above that should land in another document / chapter addressing policy / forward vision / "Where do we want to go \[after today]?" (paraphrasing MS/BG 😄)
>
> - I use *fringe* for when you're running the risk of being lethally locked into using specific, and therefor potentially *expensive* and otherwise access restricted monitoring and debugging equipment. When you (or the folks you're consulting for) don't want to spend the big bucks there, you invariably end up with flaky and nerving development and test environments, plus lots of "weird inexplicable crap" happening at your customers'. The "weird inexplicable crap" happens *anyway*, that's been around me *forever* as that's where I apparently become *real useful*, but "fringe" is about being able to second-source top quality diagnostic equipment at non-extortionist rates. "Fringe" is where such equipment doesn't require a special *mandatory license* (join the local KKK?) to be allowed through the door with your purchase request. Doesn't matter if the equipment you seek comes with a handle, jack or filename.

Dump of what was written at  https://github.com/jimmejardine/qiqqa-open-source/issues/23 for now as it's nearing 1AM again, *thrice too many*. 

---

Note source code [commit ecf6c3c4](https://github.com/jimmejardine/qiqqa-open-source/commit/ecf6c3c47292292971d0e905e516b9d04f1f88d4) comment. General direction: pick an original software that's used widely for every technology area, don't care about the language it's written in. (Acceptable deviation from this general approach: if I really *like* some stuff. Potential example of *coolness*: Chromely.)  

For text search all sane choices are Java-based so we'll need some sort of 'client/server comms' mechanism to interface then, which reduces the choice to prevent NIH: SOLR or ElasticSearch (and then maybe fiddle with them, that's fine with me). Only Python-based Sphinx was another *potential* but it's pretty fringe if you ask me, as every one and their granny is riding the ES/SOLR train these days, so I'd better hop on the bus too if I have any brains. 😉  

## General policy for picking a direction and selecting an "IT solution"

Do NOT ride on *ports* nor on *wrappers* for .NET (or any other language for that matter).

While I would get a real kick out of coding some bits of this baby in C++.NET to, say, merge MuPDF (C/C++)  and Qiqqa Core (C# = .NET), that stuff, while looking *great* in code, doesn't port to UNIX -- which is an important market from my perspective on 'target audiences' for Qiqqa, while already requested in #215.

There's the 20+ years of everywhere-*except*-*probably*-*automotive* IT comms & interfacing [*empirical* 😄 experience](https://en.wikipedia.org/wiki/Empirical_evidence) gathered in this brain that I shouldn't neglect. 
In the old days, you were looking at something *fringe* (read: *fancy*) if it didn't come with an industry standard RS232 or RS422/485 port *avec du* **simple text protocol** (yes, IEEE484 is *fringe*, for example. *Very* nice stuff, but bloody expensive -- anyway, *I digress*). Since at least 1998 my unchanging opinion and "vision of the long-term future" has been: if it doesn't come with a *text-based* TCP/IP *port* based interface, you're *fringe* and often very hard to *debug* too when the shit hits the fan (hello, XML aficionados 👋 and DCOM lovers 😈 -- you know who you are! 😈 ). 

Translation: if you cannot hook up a "generic terminal emulator of sorts" and get some decent, if maybe a bit reduced, interaction going, you're looking at an interface that's going to get you intro trouble and *hemorrhaging* weeks of concerted effort when the proverbial dung finally has arrived.

We're at that point in time where running a bunch of local servers on a *schlepp-around-everywhere* laptop isn't nerdy any more. Microservices, etc. If it doesn't come with a sockets + JSON API, one can be added, I say. *So glad the Java boys have finally given in and embraced JSON.* 😈 

SOLR it's gonna be, therefor. 

Spending energy on anything outside ES-or-SOLR is not healthy nor conducive to my goals with Qiqqa: getting this baby to be a **research tool** with "Open Access": that's where "analytics applied to our documents (*avec related metadata*) collection" is  integral to your *research* and **you** may come up with ideas and questions that a "local google machine" could help provide. SOLR/ES come with their own web style interfaces and that meshes very nicely with the sockets-and-ports vision: the web browser is the modern "terminal emulator" so when the thing speaks HTML, we're golden and you can do with data fed into that search engine whatever you want, not *restricted* by Qiqqa as the *single channel* into that data.

> 2021/Jan/14: had the above in my brain for some time already; it wanted to get out. So here's the rough view on my take on Qiqqa and where I want to go.
>
> Qiqqa *interfaces* (mixes / sits in between) about *four*(4) fundamental processes, where I assume any user will use *at least 3 of those processes* in their daily work, driving towards their own goals:
>
> 1. document management (gathering, categorizing, organizing -- you're playing *librarian* in a sense)
> 2. OCR -- call it "raw data to document transformation" if you like bigger words. "Making *potential* documentation *accessible* so it MAY become *usable*, after all. 
>
>   Do note that **I** collect a whole bunch of very particular techniques for **data extraction** under the *nomer* of "OCR". (Sorry for the confusion before, @raindropsfromsky) 
>   Some examples are:
>   + *decrypting* documents so they can be *processed*: `qpdf` is going to be part of that toolchain inside Qiqqa, as it already for *me* when I use Qiqqa. What's *lacking* there is me having spent the effort to automate / structure thee process where raw *original* PDFs come in and fully accessible, text-extractable PDFs come *out*.
>   + *text and layout (read: **structure**) extraction* from any PDF (incoming document): this is where MuPDF was used already and will see a more intense focus as the folks over at the MuPDF business are busy integrating tesseract. :+1:
>   + *conversion* where *other file formats* MAY see transformation into PDFs or HTML documents (the only two document forms I want to support in *Qiqqa Core*: all the others should see themselves transformed at the door, before entering. *Technically*, I'm thinking about going *single format*, that format being some form of HOCR == HTML *with extras* for referencing original document layout. We're already halfway there anyhow, as Qiqqa Core isn't reading PDFs in actual reality: it's using its own plaintext-based HOCR-like format for feeding the Lucene engine, displays, etc.)
>
> 3. Writing:
>   + Publishing their own *original research*: writing whitepapers and other publications about their own endeavours.
>   + Reviewing, Critiquing, etc. activities which some may call *meta research*. Examples would be writing papers discussing *the state of the art* in a field or providing a *reference guide* for others in a field so they can be made *aware* quickly and easily about a breath of data produced elsewhere.
>
>   This process would depend heavily on top quality *citations* and *citation management*: that's where the likes of Mendeley, Zotero, etc. are focusing as they have strong ties with / do live in the "*publish or perish*" *biome*. Simply put: *from* professors, *for* professors (and *potentially-going-to-be* professors). A.k.a. "the scientific community".
>
>   > I live most of my life in another *biome*: commercial consulting and R&D: there, information has a different *price* and *dissemination of knowledge* is often regarded with a very *military eye*. What we jokingly may allude to as "I may tell you, but then I got to kill you". Except that's not meant to be funny beyond the surface sheen. 
>   > It's not so different from the other *biome*, it's just... ah... *differently oriented* towards the **publish** concept. It's focus is better phrased as "*procure or perish*". Just one word difference there, right? 😜 
>
> 4. Searching
>  
>    All of us who like bigger words would call this: "*meta analysis*" or "*meta research*". That would simply be "googling my stuff, looking for hints in my library for what I'm working on now." Just *google*, **but** nicely delimited by a local, controlled, set of information, so we don't have to wade through tens of of pages of search results yakking about the crap we're not interested in *right now*. 
>
>   It's about *not* being dependent on the *preferences* of an outside party about which stuff to feed me when I ask a question that's pretty *niche* if I were blatantly honest. (*Niche* being anything that doesn't feature prominently on national TV or rules your *Insta*.  
>
> It's the ultimate reason why a *lot* of people have created and *maintained* a library *on the premises*. The OG had the *extra* argument that you could easily and *quickly* walk into the *library* and *look*, instead of *travel* and *wait and pray*, reducing the *turn-around cycle time* (a.k.a. round-trip time, response delay, 𝛕 : *tau*) to a *fraction* and thus speeding up the fundamental process cycle. 
> *We* collect our own libraries for the same two reasons as the OG: 
> - *specificity* (everything in there has our interest, or at least relates to it sideways, thus reducing the crap feeding into our search filters *du jour*) and 
> - *accessibility*: it's always there for us, come war, pestilence or other breakdowns of societal communications; 
> - *plus* it's still about 𝛕 as well, while the *context* has changed: now the improvement is not in *physical* travel distance, but rather in *number of hits to wade through manually in the search results* (effort per find). 
>
> The *accessibility* aspect hasn't changed much: when it's your own library, you only need to have gained access to the original material **once**: the OG had a photocopier (or a *scribe*, if we're talking about the *supra* OG 😉 ), while we have email, download and flash disk.
>
>  > If you're with me still, than you now know *why* I consider `qpdf` an important tool in the "OCR process": PDFs from behind paywalls and other not-*über*-public places are often tagged with all sorts of extremely vexing *access restrictions* -- *printing prohibition* being the most well known -- but there's also restrictions on select-copy-paste of text, images, and several other areas of handy use for a PDF.
>  >
>  > From the perspective of a librarian, *decrypting* a PDF should ideally only need to happen *once*: when it enters the local library. I DO NOT want to remember account log-in info for *decades* to come, NOR do I wish to pray to the heavens above and below the "service contract" which gave me access is still valid and *reasonably priced* (hello, Elsevier). 
>   


