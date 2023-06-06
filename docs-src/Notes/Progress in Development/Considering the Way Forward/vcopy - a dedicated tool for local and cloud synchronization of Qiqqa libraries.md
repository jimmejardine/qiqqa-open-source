	# `vcopy`: a dedicated tool for local and cloud synchronization of Qiqqa libraries

Multiple issues have been identified around the problems of failing Qiqqa Sync actions onto the lowest common denominator of Cloud storage: DropBox, GoogleDrive, OneDrive, et al.

Generally speaking, anything that's not *sequentially written* onto the store/mirror is bound to cause trouble; not consistently and not predictably, but often enough that people feel the current (Qiqqa Classic) situation is hazardous and *does corrupt / nuke expensive metadata* in their Qiqqa libraries.

A second group has complained about not being able to directly access their collected PDF through "reasonable file names on local drives" -- something hat is very close (in my mind at least) to accessing the *Qiqqa Export* of a library.

And then there's a potential third group, which likes to use PDFs as the carrier for annotation *edits*, not just annotation *presentation*. Which sounds useful and, at least at the surface level, sounds like a sensible approach to transport annotations across to other applications for direct (edit-level) access there, but the one signal  see that hints that this will be more difficult in *practice* than you'ld initially expect is that nobody out there is offering such a functionality -- unless it one or few of the pricey commercial offerings for document management. All the others each offer their own incompatible flavour of metadata + annotations import/export.

## Primary Goal

`vcopy` is primary geared towards ensuring we can copy a local file set safely onto one of the mentioned Cloud Storage service providers, without breaking/damaging the file content and/or confusing the Cloud Storage synchronization applications.

For that reason, `vcopy` will *sanitize* outgoing filenames & paths, as there are plenty pitfalls when it comes to producing cross-platform safe filenames: see also [[../../Technology/Odds 'n' Ends/Filesystems - caveat emptor - and then I thought NTFS was a little nasty|Filesystems - caveat emptor - and then I thought NTFS was a little nasty]].
As such, `vopy` thus also will have at least one of the mandatory functionalities to help produce sane & usable local *library exports* as those also need *sanitized filenames*.



### `vcopy` may accept or produce a filename mapping list to map document titles to actual filenames

Or should we encode that in the metadata database as yet another metadata item, so users can write their own scripts and get at this information to perform their own custom fetching / exporting?

At least we know we'll have to restrict the Document Title To FileName mapping to produce *sanitized filenames*, no longer than 255 BYTES and using only *vetted* Unicode codepoints so we don't end up with undesirable stuff like `$`, `;`, `:`, etc. in generated filenames.



### `vcopy` would also need SQLite DB access so the question becomes: SHOULD it be a separate tool, at all?!

As we wish to keep SQLite access restricted to a single process, we either will be duplicating a lot of DB communications inter-process, or we *integrate* this tool into our *core server*, which accesses and manages that SQLite database for us, thus *bloating* the *core server application*.
We already had that same consideration a while back when it came to PDF processing (including complex OCR work and/or potentially dangerous PDF I/O due to security 0-days be attached by the *arbitrary* PDF documents we'll be loading into our libraries); see:
- [[../../The Qiqqa Sniffer UI/Design Goals (Intent) & UX - The Good, The Bad and|Design Goals (Intent) & UX - The Good, The Bad and]]
- [[IPC/Considering IPC methods - HTTP vs WebSocket, Pipe, etc|Considering IPC methods - HTTP vs WebSocket, Pipe, etc]]
- [[Database Design/Considering the Database Design - we want ... PDF and BibTeX metadata versioning, conflict resolution on sync=merge, etc|Considering the Database Design - we want ... PDF and BibTeX metadata versioning, conflict resolution on sync=merge, etc]]
- [[IPC/IPC - Qiqqa Monitor|IPC - Qiqqa Monitor]]
- [[IPC/IPC - transferring and storing data|IPC - transferring and storing data]]
- [[IPC/IPC - monitoring & managing the Qiqqa application components|IPC - monitoring & managing the Qiqqa application components]]
- [[IPC/IPC - why do we use ZeroMQ|IPC - why do we use ZeroMQ]]
- [[IPC/Multiprocessing and IPC|Multiprocessing and IPC]]
- [[Multi-user, Multi-node, Sync-Across and Remote-Backup Considerations]]
- [[../../Qiqqa Internals/Extracting the text from PDF documents|Extracting the text from PDF documents]]
- [[Annotating Documents/Extracting Annotations as Metadata|Extracting Annotations as Metadata]]
- [[Stuff To Look At/Keyword Extraction, Topic Extraction|Keyword Extraction, Topic Extraction]]
- [[../../Technology/Odds 'n' Ends/Network Troubles, NAS and SQLite|Network Troubles, NAS and SQLite]]
- [[Document OCR & Text Extraction/OCR text extract engine - thoughts on the new design|OCR text extract engine - thoughts on the new design]]
  - [[Document OCR & Text Extraction/OpenMP & tesseract - NSFQ (Not Suitable For Qiqqa)|OpenMP & tesseract - NSFQ (Not Suitable For Qiqqa)]]
  - [[../../Why I consider OpenMP the spawn of evil and disable it in tesseract|Why I consider OpenMP the spawn of evil and disable it in tesseract]]
- [[Qiqqa library storage, database, DropBox (and frenemies), backups and backwards compatibility]]
- [[General Design Choices/Simple - One process per programming language or should we divide it up further into subsystems|Simple - One process per programming language or should we divide it up further into subsystems]]
- [[Stability & Security/Stability։ back end (local server components)|Stability։ back end (local server components)]]
- [[Document OCR & Text Extraction/Tesseract aspects to keep in mind|Tesseract aspects to keep in mind]]
- [[General Design Choices/Storing large (OCR) data in a database vs. in a directory tree - and which db storage lib to use then|Storing large (OCR) data in a database vs. in a directory tree - and which db storage lib to use then]]
- [[The woes and perils of invoking other child applications]]



We're still a little unclear about the better path to take: either a big monolith that does it all, as fast and best as possible, with the increased risk of some *über-nasty* PDF or other document file type being processed causing the entire monolith to barf a hairball and *terminate*, OR accept that we'll be transferring a lot of data to & from the a rather lithe database server, that's much more stable as it communicates with other *processes* through named pipes or similar high efficiency, large transfer rate, message-based intra-machine communication means, while a *monitor* keeps tabs on everybody and restarts (previously terminated or *crashed* processes **on demand**) -- I still leaning towards the latter from the perspective of *durability* of the components, but it *will* degrade the *lump sum* performance, particularly on lesser hardware...

In the latter design approach, `vcopy` would be one such a separate *process* with the in-built ability to talk to the Qiqqa database server and/or other relevant Qiqqa *processes*. In that case the question is: do we use [ZeroMQ](https://zeromq.org/) or do we ride another horse to victory?

See also https://news.ycombinator.com/item?id=23259476:

---

[+](https://news.ycombinator.com/vote?id=23259711&how=up&goto=item%3Fid%3D23259476) [heinrichhartman](https://news.ycombinator.com/user?id=heinrichhartman) [on May 21, 2020](https://news.ycombinator.com/item?id=23259711) 

You might find it interesting to note, that Peter Hintjens, was one of the core authors of the AMQP 0-9-1 Specification [1], that RabbitMQ is implementing.

[ZeroMQ](https://zeromq.org/) was born out of a frustration with complex routing patterns and the need for a broker-less architecture for maximal performance message delivery.

[1] [https://www.rabbitmq.com/resources/specs/amqp0-9-1.pdf](https://www.rabbitmq.com/resources/specs/amqp0-9-1.pdf)

---

[+](https://news.ycombinator.com/vote?id=23268703&how=up&goto=item%3Fid%3D23259476) [jonathanoliver](https://news.ycombinator.com/user?id=jonathanoliver) [on May 22, 2020](https://news.ycombinator.com/item?id=23268703) 

He and Martin Sustrik both created [ZeroMQ](https://zeromq.org/). Then after that, they saw some of the limits of [ZeroMQ](https://zeromq.org/) and created [nanomsg](https://github.com/nanomsg/nanomsg). It's excited to see what cool stuff they were working on. It's a little hard to see [ZeroMQ](https://zeromq.org/) become abandonware from them. That said, the community is solid and supportive around [ZeroMQ](https://zeromq.org/) which actually I would say is the best part. In other words, you can tell if a project has staying power when the original creator no longer has to be there to maintain it.

---

[+](https://news.ycombinator.com/vote?id=23259955&how=up&goto=item%3Fid%3D23259476) [jkarneges](https://news.ycombinator.com/user?id=jkarneges) [on May 21, 2020](https://news.ycombinator.com/item?id=23259955)

If the [ZeroMQ](https://zeromq.org/) community seems quieter lately it's because things work well and there's not much left to do within the project's intentionally limited scope. [libzmq](https://github.com/zeromq/libzmq) is certainly maintained.

Our company has been using [ZeroMQ](https://zeromq.org/) for over 8 years. We'll be putting out another ZeroMQ-based open source project soon too.

---

[+](https://news.ycombinator.com/vote?id=23259765&how=up&goto=item%3Fid%3D23259476) [trevyn](https://news.ycombinator.com/user?id=trevyn) [on May 21, 2020](https://news.ycombinator.com/item?id=23259765)

I think they’re slightly different solutions — [ZeroMQ](https://zeromq.org/) works without a broker, RabbitMQ requires a server process.

If you use the brokerless model, there was a bit of drama over [ZeroMQ](https://zeromq.org/) — the original technical developer (Martin Sustrik) left and created a successor, [nanomsg](https://github.com/nanomsg/nanomsg), with what he learned. At some point, Martin lost interest, and Garrett D’Amore took over maintenance and did a rewrite called [nng](https://github.com/nanomsg/nng). Both the old [nanomsg](https://github.com/nanomsg/nanomsg) and [nng](https://github.com/nanomsg/nng) are maintained, with [nng](https://github.com/nanomsg/nng) being somewhat actively developed, but also fairly “complete”, so there’s not a lot of excitement like you see with some projects. ;) [nanomsg](https://github.com/nanomsg/nanomsg) and [nng](https://github.com/nanomsg/nng) are essentially wire-compatible, so you can mix and match depending on bindings availability for your language.

---

[+](https://news.ycombinator.com/vote?id=23261945&how=up&goto=item%3Fid%3D23259476) [dirtydroog](https://news.ycombinator.com/user?id=dirtydroog) [on May 21, 2020](https://news.ycombinator.com/item?id=23261945)

Yes, my handwavy reading of the situation was that he left due to issues with [ZeroMQ](https://zeromq.org/) that he couldn't/wasn't allowed to 'fix'. Then Peter Hintjens unfortunately died a few years back. I haven't heard about [nng](https://github.com/nanomsg/nng), so thanks for that, I'll check it out.

[ZeroMQ](https://zeromq.org/) certainly isn't perfect, for example there's no way to tell if a message was successfully written to a PUB socket, or if it was dropped (just one minor issue)

[https://stackoverflow.com/questions/13891682/detect-dropped-...](https://stackoverflow.com/questions/13891682/detect-dropped-messages-in-zeromq-queues)

Anyway, This is digressing from the main topic

---

[+](https://news.ycombinator.com/vote?id=23262505&how=up&goto=item%3Fid%3D23259476) [wallstprog](https://news.ycombinator.com/user?id=wallstprog) [on May 21, 2020](https://news.ycombinator.com/item?id=23262505)

For now, I'll just address the one point -- obsolete? NOT!

We've been working with ZeroMQ a lot over the past couple of years, and have gotten to know some of the maintainers -- we've been very favorably impressed by their ability and dedication.

Pieter Hintjens was the "voice" of ZeroMQ, and with his passing things have gotten a bit quieter, but no less active. (Just take a look at the commit log: [https://github.com/zeromq/libzmq/commits/master](https://github.com/zeromq/libzmq/commits/master)).

----


