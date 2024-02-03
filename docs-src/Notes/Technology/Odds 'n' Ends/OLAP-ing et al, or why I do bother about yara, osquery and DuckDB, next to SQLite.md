# Why do I bother about yara, osquery and DuckDB, next to SQLite?

Because I'm looking into accessing and using the Qiqqa-managed PDF/document library metadata, including document content and derived metadata (such as automated title, abstract and cross-references extracts, produced using additional tooling provided in the Qiqqa backend).

While yara may look to you like off-topic, overkill or maybe both -- after all, I was initially considering yara as a jumped-up replacement/alternative for GNU file, after all -- it's mostly DuckDB that might become handy as a column-oriented OLAP core/base, on top of SQLite.

osquery came into view a long time ago, not so much for Operating system monitoring, but rather more Qiqqa internal application monitoring. However, osquery was also considered as a possible Programmer's Front End by way of its SQL language support and thus easy unification of all information querying over SQL as the chosen *lingua franca* for any Qiqqa-related information gathering and ~ analysis. And since no-one had bothered about a osquery-GNU.file interface but several peeps were/are busy with osquery-yara, I thought: let's ride those coat-tails as they're close enough to what I want: SQL input, able to reach anywhere within my Qiqqa application. 

Did it have to be SQL as lingua franca? Nah. Other "REST-full" approaches were considered; meanwhile Qiqqa backend should/MUST be fitted with a user-usable scripting language for customizable stuff, such as the precise PDF/document "page render to OCR" pipeline, where QuickJS (modern JavaScript) and TCL are competing for attention in my mind, so doing something *JSON*+*JSONpath* style-ish (à la *XML* + *XMLpath* for the XML lovers, of which I am not a club member) over REST would have been an obvious choice, but then I liked the SQL approach for the query transmission part much more than writing "JSON queries" which feel rather ridiculous to my mind. JSON for the *answers* though... now **that** is a good idea!

SQL *in*, JSON *out* led me to looking for flexible SQL processors and that's where `osquery` came into view some time ago. As I hoped/intended to use the same REST-like webby interfaces to the backend for configuring and controlling the backend Qiqqa "server" beast, SQLite + osquery were the prime candidates and yara popped up on the radar as part of the search for system scanning ("globbing") solutions, which should be configurable with filters and such as I want to be able to ingest* a few different document formats, not just PDF only, and *possibly* allow users to *ingest* their research *data files* as part of their documentation: think "original research data files as **attachments** to the research report/write-up", all kept in Qiqqa and thus *searchable in Qiqqa* and "thus" art of the data sources that feed the meta-analysis that's part of Qiqqa too.

Frankly, I have no idea how wild this may get, but the core idea is sane, I believe: a way to bundle your research data with your research papers & notes and then not have that data sit there like a stuffed duck, dead-as-a-doornail and gathering dust on top of a shelf, but as "equal citizen" searchable data: after all, as long as it's text of some sort, the FTS (Full Text Search) engine shouldn't be bothered about it what we feed it and query it about: after all, *I* don't care where the searched phrase is found, whether it be in my *notes*, *write-up* or *data sets*, as long as the search machine can dig it up for me, as *that* is the only bit that's relevant to *me*: I want that search phrase **found**!

Okay, back to my off-brand intended use of yara, which is mainly employed *out there* for helping digging up security risks by way of fingerprints (hey! GNU file! But *improved*! *You dig?*... *Me digs*, anyway.)
I haven't checked yet how hard it might be to use yara as a glorified GNU file filetype recognizer and/or whether it's really worth the effort: that part is still in the future. What I *do* know is that I like SQLite a lot, so it's going to be in Qiqqa<sup>NG</sup> anyhow, not just because Qiqqa<sup>old skool</sup> stores *most* of its metadata in the SQLite format. Yet we might need a little more: either I code up a few fake SQLite *virtual tables* to service the need or I do the same/similar in `osquery`, where globbing, file system access, file system monitoring (document import directory monitoring for example), etc. are already mostly provided for -- where yara or GNU file (`libmagic`) then serves as fingerprinters to help detect the various file types and route them to their designated storage/processing facility inside the Qiqqa core: PDFs to be cleaned and have their content *extracted*, ditto for HTML pages (my secondary document target, second in priority right after PDF), ditto for EPUB+MOBI for peeps (like me) who like those other books *ingested into Qiqqa* as well, other file types designated as research data files, a.k.a. "attachments", etc.etc. It just so happens that osquery has a rather close relationship with yara if I put my ear to the ground and listen to the Interwebz and I *think* osquery might come in quite handy after a while.

Which is where DuckDB shows up, as part of the osquery + misc OLAP/monitoring+(meta-)analysis crowd: do I need DuckDB? Do I *benefit* from using it? I'm not sure yet, but let's see what it brings, apart from "column-oriented high performance for OLAP queries" yada yada *yara* 😜




-----------------

**\[Edit]:** turns out osquery isn't so easy to compile on MSVC, so the amount of effort required to make that one run smoothly may not balance against the perceived benefits... Maybe I'm better off with the 'alternative approach' which is directly writing the stuff OS + ingest stuff I want as SQLite virtual table extensions instead? 

After all, I don't need osquery for its SQL query parser any more as SQLite can do that job, easy. Which leaves DuckDB and *its* usefulness.

Meanwhile, `yara` is still "on the map" as far as I'm concerned: I like its expressive fingerprint power and format, which is useful to help detect and route/dispatch datafiles and others that come in via directory tree watch and other *ingest methods*.




