# Our position on using ⧸ including ChatGPT (et al)

Several users, developers and other parties interested in Qiqqa and its outlook towards the future have been inquiring about this in various ways: *how soon will we offer ChatGPT with Qiqqa?* etc.

I've been pondering this for a while before I felt a sensible answer could be given. This is my stance on the matter as per April 2023. Opinions can and do change, so this is not set in stone. *However*, as this has been considered since ChatGPT *became a thing of note* last year, I've wavered a bit, but I also have found that the main thrust of my rationale hasn't been upset all that much, so I consider this stance pretty solid by now.

## I intend to solve *shared* problems, through software. What does ChatGPT & friends *solve*?

Fortunately, some problems are *potentially* solvable, or their solutions can be at least *assisted*, through the application of computer software. That's where I encountered Qiqqa (when it was still a commercial product) and found it was the one with the most overlap with my problem definition and acceptable (tolerable?) solution space. As I wrote elsewhere, rather cynically, I'm *not in the publish or perish business* (a.k.a. pursuing a doctorate and career towards tenure at a university), so Zotero wasn't cutting it, as that one is all about assisting with *writing papers*: if your biggest scare is being accused of plagiarism and one of your major KPIs is faultless adherence to 'proper behaviour' re citations, then Zotero is your man. There's others out there, both open source and commercial, but they all more or less pander to the same niche: you're at or in a university, where one of your targets is making it onto the pages of a high-profile peer-reviewed *journal* to further your career.
Qiqqa also addresses that part of the potential audience (heck, it was *born* there), but its strength is its focus on becoming your privately-owned library, *avec* librarian: it alludes you can now own a library yourself, *independent of outside forces*[^2], while the *administration* of said library is largely automated, thus relieving you from that drawback of *owning a library*, so you can devote yourself to **what you *want to be*: a _library user_** busy with (literature) *research*, via *content search* and meta-analysis, such as relationship analysis via Qiqqa Explorer or Qiqqa Expedition.
In practice, at least for me personally, current Qiqqa falls short on those goals when your library grows beyond about a 1000 documents: the sad state of affairs is I haven't had a fully operational library for more than 6 weeks in a row since commercial v67 as either the in-built Lucene search facility will falter and fatally corrupt the search index, or some other major fault or deficiency occurs thanks to the chosen technology and some coding / engineering choices that made sense at its date of inception for getting a product out the door *pronto* with a small team and a target of *project-specific libraries*, hence no more than, say, several hundred documents. Anything larger or longer-lived is going to suffer. That's where I am right now, with a library of 40K to 90K, depending on how you count/rate, and no medal for myself as almost 3 years[^3] of my own input and effort haven't yet brought about a stable product that's usable for all parties. Some are lucky, others less so.[^5]

The first 100, maybe 200, open issues at github *hint* at the root causes of this failure to cope: 
- Qiqqa uses a PDF display/render library that b0rks in several ways: white pages (where all content is wiped off the face of the document page) and catastrophic failures for other pages, resulting in uncontrolled, abrupt termination of the Qiqqa application. While its SQLite metadata database will survive such failures, Qiqqa will sometimes *destroy everything* on recovery/restart as some cryptographic validation hashes[^4] may not have been properly and timely updated, thanks to these (and other) crashes. What one calls: *a very good start*. 
  This library is commercial, *closed source* and, yes, the company that sold it is long gone the way of the Dodo. Nobody home any more.
- Qiqqa uses Lucene.NET for its content and metadata search engine. Great choice, were it not for the fact that ports like that often lag behind the mainline, dragging their feet. 
  
  > Lucene is Java-based. Lucene.NET was/is a somewhat active port to C#/.NET) plus it's old enough that Lucene itself probably suffered the same issues: feed it enough arbitrary data and it starts to fail in all sorts of ways. Sure, it'll have passed the *unit tests*, but I bet a pretty dollar it never got stress-tested with a *large and diverse* dataset. 
   
  Meanwhile I had two options (or three, if I was a veritable madman -- some days I fear I might be one): 
  1. upgrade Lucene.NET
  2. migrate to Lucene
  3. find the revision in the Lucene.NET source tree that got turned into a binary Qiqqa release and fix the b0rks there.
  I looked into (3) and quickly found that option was nuts: good luck finding the precise revision and rebuilding for testing was a headache. (2) is not my cup of tea -- I've committed many sins, but writing Java code has only made a minor smudge on my aeternal ledger thus far -- so... (1)? Rrrright. API changed. Significantly. And to my 2019/2020 mind, *incomprehensibly*. How the mighty falter. So we're all still stuck with that piece of cruft.
  
  As far as I'm concerned, anno 2023, Lucene.NET can go paint radishes: I'm looking at SQLite FTS and possibly `manticore` for absolution. (While having looked into Apache SOLR before: that one *is* very nice, but somehow not exactly *end user ready* from my perspective. A matter of taste and limited ability, perhaps? You tell me.)


............................................................................................

ChatGPT is the Christian Louboutin of user-facing software *couture* today. If you don't own (and wear) a pair, you're not OSM/OKP[^1].


............................................................................................

............................................................................................

............................................................................................

---


[^1]: OSM[^OSM]: Dutch: *Ons Soort Mensen*. In English: *Our Kind \[of\] People*. Terribly sorry, but *you* don't *rate*. Now *shoo*.

[^OSM]: OSM is an acronym usually employed by people who consider themselves, äh, shall we say: representatives of the upper crust, anywhere. The word "snooty" comes to mind...

[^2]: such as journal subscriptions and other paid-for *access licenses*: once you've gained access, Qiqqa will grab a copy and *keep it in your own library* forever. Or until your HDD storage and backups go up in smoke.

[^3]:  Qiqqa was open sourced at the end of 2019, we're now in April 2023, so that must be an error: "3 years"! It isn't. As I had decided, after a long bout of trial and error and desperation with it, to reverse-engineer commercial Qiqqa, it got open sourced about a year later, so I grabbed the opportunity to get my hands on the original: reverse-engineering may be *cool* to some, but it sure is far more costly than grabbing the original sources. That was Qiqqa v80, december 2019. I went to work, coming to terms with the codebase and fixing bugs I kept running into with my own library (which was (and is) stress-testing the 32bit .NET codebase to the breaking point) until about 2021, just before corona hit the streets and I got myself into a construction project that rapidly got out of hand in such a way that "sleep is an option". To keep myself motivated *at anything* I dabbled a bit in git (great! more sleep deprivation! very sane!) and somehow got through that year. Then I spent a year (2022) what I'd call "*Bob Ross-ing through source code libraries of interest*" while I tried to recover and meanwhile wondered whether my increased inclination to kill the Qiqqa SOB and start anew wasn't inviting in all the perils so accurately and truthfully described by Frederick P. Brooks in The Mythical Man Month: been there, done that, *repeatedly*, during my couple of decades as a software buccaneer, a.k.a. *contractor*.  “I’ve seen things you people wouldn’t believe. Attack ships on fire off the shoulder of Orion. I watched c-beams glitter in the dark near the Tannhauser Gate. All those moments will be lost in time, like tears in rain. Time to die.” I'm not dead yet, but 2 years of effective hiatus makes 2019-2023 3 years, *optimistically speaking*.

[^4]: Why on earth would one drop cryptographic content hashes (MD5, by the way) into a database record, next to the JSON-over-BibTeX-ed data BLOB itself? When I found out during my reverse-engineering streak, I already wondered, but the source code code comments have been blithely silent on the matter. My uncorroborated suspicion is they were having "weird" issues with their codebase and lost faith/trust in their own metadata serialization & processing code, or *anything* for that matter: add a hash to at least *detect* corruption and then *nuke* the record upon fault discovery any time later (before next use), rather than proliferate the now-dodgy and probably totally hallucinatory metadata. Regrettably other bugs and failure modes can drive Qiqqa to blow away large parts or even your entire database under rather obscure circumstances. *This is insane! This cannot happen*, you say? Guess how many utterly botched metadata database backup copies I have in cold storage...

[^5]: I have the niggling, unsubstantiated, *suspicion* that it matters *a lot* what kind of PDF documents you feed this animal: as long as it is relatively recent academic papers, your chances have just much improved towards having a stable, enjoyable experience. In my case, I'm feeding the bugger academic papers, but also a lot of other stuff, including, for example, electronic datasheets published by electronics manufacturers. Suffice to say my surprise was considerable when I found out several of them do all kinds of nasty publishing things to make those PDFs hard to process, both for the Qiqqa page renderer and the text extractor + OCR engine. If you then also feed it some more "cruft" such as *brochures*, then, old boy, you're in for a stability treat! Still not brittle enough for you, you say? Do like me: feed it several series of PDFs obtained from all over, no questions asked, scans of pre-war (*any* war, as long as it was *big*, *bloody* and, above all, *vicious*) publications, archive.org and *anywhere*, and shove that into the meat grinder for optimum performance: you now have about a 80%[^7] chance of Qiqqa already catastrophically crashing at import time![^6]

[^6]: yes, those crashes are non-deterministic: they happen at random. And woe the naivitee who thought he could observe better from the vantage point of a running debugger session: now you're *guaranteed* to fail with out-of-memory and other random failures *elsewhere*, accompanied by catastrophic failure of the (Visual Studio) debugger itself, in a joint effort to lift your spirits. What fun! The joy is great!

[^7]: ah, still with us! Oh dear. Yes, that 80% number was plucked from thin air. But it's not total bollocks: let me rephrase that 80% statement as a question: do you, to the best of your recollection (as we noted your logbook usage execution has something to beg for, alas), remember to *ever* having observed a successfully *completed* import into Qiqqa of your library, ever after you got v76 (and your library being that much smaller back then)? And if so, how many times? ... No, your honour. 

 
 