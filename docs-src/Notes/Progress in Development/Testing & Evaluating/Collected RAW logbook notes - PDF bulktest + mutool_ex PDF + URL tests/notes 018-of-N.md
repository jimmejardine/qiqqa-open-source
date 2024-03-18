# notes




## Unicode homoglyphs for Win32/NTFS & UNIX illegal filename characters

  
PHP code based on examples and libraries from phlyLabs Berlin; part of [phlyMail](http://phlymail.com/)  
Also thanks to [http://homoglyphs.net](http://homoglyphs.net/) for helping me find more glyphs.


|  |  |
| ---- | ---- |
| **Char** | **Homoglyphs** |
|  | ᅟ ᅠ                   　 ㅤ |
| ! | ! ǃ ！ |
| " | " ״ ″ ＂ |
| $ | $ ＄ |
| % | % ％ |
| & | & ＆ |
| ' | ' ＇ |
| ( | ( ﹝ （ |
| ) | ) ﹞ ） |
| * | * ⁎ ＊ |
| + | + ＋ |
| , | , ‚ ， |
| - | - ‐ － |
| . | . ٠ ۔ ܁ ܂ ․ ‧ 。 ． ｡  ․  |
| / | / ̸ ⁄ ∕ ╱ ⫻ ⫽ ／ ﾉ |
| 0 | 0 O o Ο ο О о Օ Ｏ ｏ |
| 1 | 1   １ |
| 2 | 2 ２ |
| 3 | 3 ３ |
| 4 | 4 ４ |
| 5 | 5 ５ |
| 6 | 6 ６ |
| 7 | 7 ７ |
| 8 | 8  ８ |
| 9 | 9 ９ |
|  |  |
| : | : ։ ܃ ܄ ∶ ꞉ ： ∶  |
| ; | ; ; ； ; |
| < | < ‹ ＜ |
| = | = ＝ |
| > | > › ＞ |
| ? | ? ？ |
| @ | @ ＠ |
| [ | [ ［ |
| \|  ＼|  |
| ] | ] ］ |
| ^ | ^ ＾ |
| _ | _ ＿ |
| ` | ` ｀ |
| a | A a À Á Â Ã Ä Å à á â ã ä å ɑ Α α а Ꭺ Ａ ａ |
| b | B b ß ʙ Β β В Ь Ᏼ ᛒ Ｂ ｂ ḅ |
| c | C c ϲ Ϲ С с Ꮯ Ⅽ ⅽ Ｃ ｃ |
| d | D d Ď ď Đ đ ԁ ժ Ꭰ ḍ Ⅾ ⅾ Ｄ ｄ |
| e | E e È É Ê Ë é ê ë Ē ē Ĕ ĕ Ė ė Ę Ě ě Ε Е е Ꭼ Ｅ ｅ |
| f | F f Ϝ Ｆ ｆ |
| g | G g ɡ ɢ Ԍ ն Ꮐ Ｇ ｇ |
| h | H h ʜ Η Н һ Ꮋ Ｈ ｈ |
| i | I i  ɩ Ι І і ا Ꭵ ᛁ Ⅰ ⅰ Ｉ ｉ |
| j | J j ϳ Ј ј յ Ꭻ Ｊ ｊ |
| k | K k Κ κ К Ꮶ ᛕ K Ｋ ｋ |
| l        | L l ʟ ι ا Ꮮ Ⅼ ⅼ Ｌ ｌ    |
| m        | M m Μ Ϻ М Ꮇ ᛖ Ⅿ ⅿ Ｍ ｍ  |
| n        | N n ɴ Ν Ｎ ｎ            |
| 0        | 0 O o Ο ο О о Օ Ｏ ｏ |
| p        | P p Ρ ρ Р р Ꮲ Ｐ ｐ      |
| q        | Q q Ⴍ Ⴓ Ｑ ｑ            |
| r        | R r ʀ Ի Ꮢ ᚱ Ｒ ｒ        |
| s        | S s Ѕ ѕ Տ Ⴝ Ꮪ Ｓ ｓ   |
| t        | T t Τ τ Т Ꭲ Ｔ ｔ        |
| u        | U u μ υ Ա Ս ⋃ Ｕ ｕ      |
| v        | V v ν Ѵ ѵ Ꮩ Ⅴ ⅴ Ｖ ｖ    |
| w        | W w ѡ Ꮃ Ｗ ｗ            |
| x        | X x Χ χ Х х Ⅹ ⅹ Ｘ ｘ    |
| y        | Y y ʏ Υ γ у Ү Ｙ ｙ      |
| z        | Z z Ζ Ꮓ Ｚ ｚ            |
| {        | { ｛                     |
| \|       | \| ǀ ا ｜                |
| }        | } ｝                     |
| ~        | ~ ⁓ ～  |
| ß        | ß                       |
| ä        | Ä Ӓ                      |
| ö        | ӧ Ö Ӧ                      |
|          |                          |
|          |                          |





## PCA, PPA, SVD, LCA, auto-encoder, etc: dimension reductions for search, clustering, topic analysis, ...

Paper: Empirical comparison between autoencoders and traditional dimensionality reduction methods 

Quentin Fournier 
arXiv:2103.04874v1
8 Mar 2021 

Abstract — In order to process efficiently ever-higher dimensional data such as images, sentences, or audio recordings, one needs to find a proper way to reduce the dimensionality of such data. In this regard, SVD-based methods including PCA and Isomap have been extensively used. Recently, a neural network alternative called autoencoder has been proposed and is often preferred for its higher flexibility. This work aims to show that PCA is still a relevant technique for dimensionality reduction in the context of classification. To this purpose, we evaluated the performance of PCA compared to Isomap, a deep autoencoder, and a variational autoencoder. Experiments were conducted on three commonly used image datasets: MNIST, Fashion-MNIST, and CIFAR-10. The four different dimensionality reduction techniques were separately employed on each dataset to project data into a low-dimensional space. Then a k-NN classifier was trained on each projection with a cross-validated random search over the number of neighbours. Interestingly, our experiments revealed that k-NN achieved comparable accuracy on PCA and both autoencoders’ projections provided a big enough dimension. However, PCA computation time was two orders of magnitude faster than its neural network counterparts.


[6] J. B. Tenenbaum, V. de Silva, and J. C. Langford, “A global geometric framework for nonlinear dimensionality reduction,” Science, vol. 290, no. 5500, p. 2319, 2000. [7] D. P. Kingma and M. Welling, “Auto-encoding variational bayes.,” CoRR, vol. abs/1312.6114, 2013. [8] D. P. Kingma and J. Ba, “Adam: A method for stochastic optimization,” CoRR, vol. abs/1412.6980, 2014. [9] X. Glorot and Y. Bengio, “Understanding the difficulty of training deep feedforward neural networks,” in Proceedings of the Thirteenth International Conference on Artificial Intelligence and Statistics (Y. W. Teh and M. Titterington, eds.), vol. 9 of Proceedings of Machine Learning Research, (Chia Laguna Resort, Sardinia, Italy), pp. 249–256, PMLR, 13–15 May 2010. [10] C. Ou, D. Sun, Z. Wang, X. Zhou, and W. Cheng, “Manifold learning towards masking implementations: A first study,” IACR Cryptology ePrint Archive, vol. 2017, p. 1112, 2017. [11] N. Halko, P.-G. Martinsson, and J. A. Tropp, “Finding structure with randomness: Probabilistic algorithms for constructing approximate ma trix decompositions,” arXiv e-prints, p. arXiv:0909.4061, Sep 2009.





## smart semi-logarithmic scale for ROC charts and similar curved recall, etc. charts

[GitHub - erikbern/ann-benchmarks: Benchmarks of approximate nearest neighbor libraries in Python](https://github.com/erikbern/ann-benchmarks)

e.g.:

![[Pasted image 20240227174308.png]]

What they did is use a semi-logarithmic recall scale that more or less follows the bend, equi-distancing these:

- $10^{-n}$
- $10^{-2}$
- $10^{-1}$
- $\frac 1 2$ *(sic!)*
- $1 - 10^{-1}$
- $1 - 10^{-2}$
- $1 - 10^{-n}$

where the important "success nearly 100% ($p=1$)" part of the range is stretched thanks to using ticks at $1 - 10^{-n}$ values. Ditto for the "near zero" part. The range is centered and mirrored around $\frac 1 2$.






## kD-tree, kNN, ANN, etc. search and indexing to follow after PCA

kNN (k Nearest Neighbours) search can follow a PCA-based dimension reduction stage.
However, kD trees and similar solutions only "work" (read as in: "can be expected to perform reasonably well") when the number of dimensions is *low*: much less than 100!

As I expect my "reduced" dimension count to be generally near that number (keyword discovery, f.e.) or far higher (keyword & keyphrase discovery; content search), the general advice is to go for ANN indexes and search approaches: *Approximate Nearest Neighbour* (ANN).

This would therefore involve libraries such as

- [flann-lib/flann: Fast Library for Approximate Nearest Neighbors](https://github.com/flann-lib/flann)
- [spotify/annoy: Approximate Nearest Neighbors in C++/Python optimized for memory usage and loading/saving to disk](https://github.com/spotify/annoy)
- [nmslib/nmslib: Non-Metric Space Library (NMSLIB): An efficient similarity search library and a toolkit for evaluation of k-NN methods for generic non-metric spaces.](https://github.com/nmslib/nmslib)
- [nmslib/hnswlib: Header-only C++/python library for fast approximate nearest neighbors](https://github.com/nmslib/hnswlib)
- [facebookresearch/faiss: A library for efficient similarity search and clustering of dense vectors.](https://github.com/facebookresearch/faiss)
- [vioshyvo/mrpt: Fast and lightweight header-only C++ library (with Python bindings) for approximate nearest neighbor search](https://github.com/vioshyvo/mrpt)
- [microsoft/SPTAG: A distributed approximate nearest neighborhood search (ANN) library which provides a high quality vector index build, search and distributed online serving toolkits for large scale vector search scenario.](https://github.com/microsoft/SPTAG)
- [microsoft/DiskANN: Graph-structured Indices for Scalable, Fast, Fresh and Filtered Approximate Nearest Neighbor Search](https://github.com/microsoft/diskann)
- [zilliztech/pyglass: Graph Library for Approximate Similarity Search](https://github.com/zilliztech/pyglass)
- [pgvector/pgvector: Open-source vector similarity search for Postgres](https://github.com/pgvector/pgvector)
- [yahoojapan/NGT: Nearest Neighbor Search with Neighborhood Graph and Tree for High-dimensional Data](https://github.com/yahoojapan/NGT)

- [erikbern/ann-benchmarks: Benchmarks of approximate nearest neighbor libraries in Python](https://github.com/erikbern/ann-benchmarks)







## Qiqqa<sup>NG</sup> application architectural overview

First, let's address the major elephant in the room (from the perspective of application construction and system security): the Qiqqa Sniffer.

This one requires the co-existence of *two* **communicating** security contexts: 
1. application-local and 
2. internet-worldwide,
because the Qiqqa Sniffer includes a full-fledged web browser which can be used to freely roam and search/scavenge The Internet and thus, by choice or by undesired happenstance, encounter *Bad Actors*, people and website machinery that's actively trying to penetrate your security cordon.
Next to that, it runs/manages an extensive UI that's showing and *editing* local data: PDF documents and all related metadata.
As such, this application has a lot of potential for penetration and poses a significant information leakage and foreign system access risk.
Which is why why we must use latest WebView technology and preferably keep the *communication interface* between the two security zones minimal, while the UX dictates that the original Qiqqa design, which merges both security zones together into a single UI view, fundamentally in two panes stacked above one another with a pane splitter in the middle such that both planes (security zones) are always visible. Arguably, the Qiqqa Browser was/is another part of the original application which can be argued to have the same base, but only one security zone/pane: the *embedded* Internet Browser. Thus one might be inclined to consider Qiqqa Sniffer and Qiqqa Browser to be two different instantiations of the same set of issues and thus a single (partial?) application. I'm fine with that.

The rest of Qiqqa (the *tool*) can be simply regarded as a two-part system: a more-or-less sophisticated general *front-end* and a (possibly either segmented or amalgamated monolithic) *back-end*, which does the actual work: document storage, ~ management, ~ indexing, ~ searching, analysis, meta-analysis & reporting. Plus the bit I tend to forget as it's low on my own personal priority list: document *referencing*, which is needed when using it Zotero-style for writing academic papers. I'ld rather park that under the heading "project management / subcollections' management" as I would like/need Qiqqa to keep track of the *documents* I read and use for a particular project/task, thus task-specific libraries which carry references of general library documents; anyone who writes a paper would need the same, but then the additional ability to export this list of referenced documents as a bibtex reference list or equivalent for use/inclusion in the paper that's being written / research that's being done.

Which brings us to the Qiqqa application architecture overview:

it consists of three chunks:

1. Qiqqa Sniffer, as a separate executable and implementation technology, to keep things managable in that court,
2. Qiqqa Main UI, which I prefer to have as a web app, i.e. running in a regular desktop or mobile browser, using JavaScript/CSS/HTML or related technologies (TypeScript, UI framework such as Svelte, that sort of thing -- this choice for *web app* also facilitates developer team collaboration as others, not well versed and/or willing to write large C/C++ applications, can join and ease the burden. Personally, I've considered using wxWindows (wxWidgets) for this app for a long while, but I think that's not conducive to bringing Qiqqa as a whole *forward*: web tech is faster and easier to get collaborators for and working on as the dev pool for those technologies is an estimated multiple orders of magnitude larger than **cross-platform** classic business application development, as the latter is rather an unfavored *niche* in corporate business land.
3. Qiqqa Back-End, which is expected to be at least one database + overall managing *server* with REST-based web-access for both the UI apps and any user-driven custom scripting activities to open up the data and metadata collected and managed by Qiqqa, plus probably a secondary, communicating, server (or set of server applications), which take care of the complicated stuff, such as document OCR, etc.: the latter bunch is expected to be *somewhat unstable* (as in: expected to suffer from heap memory leaks and thus not suitable to run a single instance 24/7 if you'd like to do that): while the code would be okay, the size of the complexity is such that you can bet there's issues and a failure should be expected. This can be dealt with by having these run at reduced batch length and auto-restarted by the *main* back-end database server as needed, i.e. after an observed crash/fault of the secondary or after a certain batch size / timeout / observed heap consumption has been noted, which count as risk factors for continued stability of the secondary (OCR+misc) binary; these *can* decided to terminate once the last instruction batch, received from the main back-end server application, has been received and *possibly* processed -- I say *possibly* as we must reckon with the secondary *faulting* at unexpected times, hence any job handed to the secondary must be flagged as *concluded* before it (the job) can be removed from the queue, which is managed by the primary (database et al) back-end server.

Back-end servers are written in C/C++ and should be cross-platform. Any management or other purpose UI, next to the Qiqqa Main User UI + Sniffer, should be done in separate application binaries to allow maximum porting flexibility and enable Qiqqa users to write their own data / management scripts in any form they like / are comfortable with: after all, one of the goals of Qiqqa<sup>NG</sup> is to have the collective data/metadata and processing power of Qiqqa available to users via user-selected scripting, using regular web technologies such as REST web requests and JSON / XML based responses for their scripting/tooling to easily process: everything is *Open Data*.

People are used to web services and web APIs; while I don't like Python that doesn't mean anyone else out there shouldn't be able to whip something up in that *or any other language* to do something with their Qiqqa libraries: after all, it's *their* libraries they're working on and, like me, they should have *maximum access*. With the *minimum of fuss*; I want this to be simple for most everyone to do *custom stuff* with their Qiqqa document libraries, including, but definitely not limited to, custom data import/export/analysis.
After all, that's what *I* want too!







### Re specific bits of technology that I like or might consider

#### tvision

Cool. An old love of mine. 

*Maybe* for administrative support tools and the back-end server itself, but keep it minimal...


#### Dear ImGUI

Beautiful stuff. But... *niche* & C++: it's a major player in game industry, but game industry and research is not exactly the same thing. So there certainly is a dev pool out there for this stuff, they are *very probably* not inclined to using and thus working on Qiqqa development. Pity, but **DON'T**!


#### WxWindows / WxWidgets

Marvelous work. And the maintainers have kept on all those years while I hopped everywhere; that's commendable stamina. But... C++ and *niche*. So we'd better keep it minimal: while it definitely will be a decent choice as the wrapper/container for Qiqqa Sniffer / Qiqqa Browser, it MUST NOT be made part of the main Qiqqa *web app*: that one SHOULD run in any local mainstream browser like any regular web app out there. 

WxWidgets should be a *preferably minimal* container for the dual security zoned Qiqqa Sniffer UI layout and even then I'm inclined to advocate a two-webview pane layout, where the top pane is a local web app instance taking care of the local security zone part of the UI layout: PDF view, copy/paste, metadata edit & validation, etc.
If that is not possible or otherwise hard to accomplish, only then would I favor a wxWidgets-based "native UI" layout for the top pane in Qiqqa Sniffer (and thus a single webview, used for the internet-worldwide security zone based *browser* element of the Sniffer UI).



#### user scripting of back-end processes: OCR, page image processing, data + metadata postprocessing, etc.:

##### QuickJS (JavaScript) + SQL 

The preferred choice. After having dillied and dallied with other beauties, this still remains the obvious and only sane choice, as far as I am concerned. Everything else that's listed below does not add value compared to this one and is only of interest to language connaisseurs -- or as Pratchett would call them: conney-sewers. And he's right: all the rest (see below) is only for *specialists* -- and didn't I want to open this up for *non-specialists*? Then JavaScript or Python are the only sensible choices for DSLs (Domain Specific Languages) if you ask me, and since I was not favorably inclined towards Python, there's your answer: *modern* JavaScript it is. Hence either v8 (which is a nightmare to build and integrate) or QuickJS or equivalent power. And I happen to like the QuickJS codebase, so we have a verdict!

By the way: the other "DSL" (of sorts) that's to feature in & around Qiqqa is: SQL! I fully agree with ex-Spotify's Erik Bernhardsson: https://erikbern.com/2018/08/30/i-dont-want-to-learn-your-garbage-query-language
I *love* language design, parsers, and all related technologies, and, yes, I've created a few DSLs of my own, including a Pascal/BASIC variant once that started as a personal `yacc` challenge ("*nobody ever did it like this but I think it's _doable_!*" -- of course knowing full well *up front* **why** nobody had been doing it like that...) and having it take off rather unexpectedly: the users loved it, as they didn't usually run into the performance issues that were lurking in every corner, thanks to my approach. Meanwhile, I've always felt ORM interface libraries to be... *je ne sais quoi*. With the exception of OTL, which is simply a very minimal C++ templated header file for interfacing with multiple heavy industry databases and the answer when your C++ compiler doesn't support direct SQL embedding (the usual suspects (gcc, clang, msvc) don't so OTL is very useful): OTL just lets you write your SQL statements as if they were *embedded* and that's *good*. But all those query languages Bernhardsson is raging against? *Totally agree!!!1!*




##### BASIC

Going *retro*, aren't we? Sure. I was looking for something like GFA Basic (Atari ST), which is more Pascal-y than classic `GOTO 10` BASIC, but it's not there, at least not in easily palatable and cross-platform compile-able form. Not that I could find, anyway.

The BASICs that I did find are... after a while... *meh*. And then I wake up and ask myself: who of my supposed customer base is going to learn BASIC like that and be a happy camper while she's doing it? ... erm... doesn't feel like a solid dating tip, if you get my drift. Nah. BASIC is history. If you want GFA Basic, just take QuiskJS and run with it, because it's got **exactly** the feel I was looking for anyway. Why dither? (Well... me? kid? candystore?)



##### TCL

Oh boy. While this language is *minimal*, is *exactly* designed for what I want to do with it (use it to write customizable image pre- and postprocessing for the OCR subtask), there's that `bash`+`PHP`-y `$` obnoxious, stinging feeling in my nose. Which is *probably* why I've always at at this one a bit askance. Cute, great bum, but no cigar.
Such a pity. Another *great* *retro* going down.



##### LISP (and Scheme...)

Jeez Louise! The language you *never really grokked*! And *this little frustration of yours truly* is what you want to hand your customer base? Because, excuses, excuses, there've been several "notable people" who... not *Knuth*, you \*(bleepard)\*, just some slashdotted loudmouths from The Valley who *probably* went through their careers hopped up to the gills on coke, meth and the very latest and greatest in recent party powder research developments, who landed in the egotrip interview/speakers circuit declaring *LISP* is what made them *fast*!  Heck, 3 to 4 hours less sleep than all the rest will do that for ya whatever the bloody language you happen to carouse with on a daily basis, but who am I to judge. *Anyway*, it still irks me that I never got to really *grok* (love?) LISP to a degree that *I* was fast in it; *loooooong* time ago, in another life, I've written enough AutoCAD scripts with it to get nauseous and since I didn't like the AutoCAD UI/UX design **at! all!** (having been allowed near and *on* Integraph Workstations does that to you: when you are allowed to drive around for a few months in a loaned Lambo, free gas and insurance included, you resent your own assigned, rickety, 20 year old, rusty and claustrophobic and above all *quaint* Morris Mini *forever*. If you're *me*, anyway), I never got to truly join in the happy *nuveau* hippie LISP circle dance. To me, LISP still is a (very smartly done 👍) way of writing ASTs (Abstract Syntax Trees) *by hand*, which makes "The Powers Of Lisp-p-p-p-p-pp!" just so much more *damn powerful Macro Assembler* noise, and here I have to steal from Bernhardsson again: LISP is one of those things I apparently *would have wanted* to learn, *but not want to learn*.












## NAS, DropBox, Google Drive and similar sync services' issues with Qiqqa / SQLite

Revisited this issue after a long while of throwing up my hands in despair: turns out I had not anticipated *one more possibility*, which is mentioned elsewhere on the Net for other products (Zotero, TheBrain, VoidSoft Everything, ...):

- sync services such as the ones above *sometimes* touch files when you don't expect it; some of noises indicate to me there's some (weird!) back&forth versioning happening on the fly -- how otherwise would a file **change** when you are the only one using it and haven't saved any updates yet? That sounds like DropBox et al sometimes write some crap into your local mirror and then rewrite it; access the file at the wrong point in time and you get the 'crap'; that's how I would explain all these file corruptions without obvious cause.
- directory tree walks *may* produce duplicate filename entries as the directory tree is rewritten by the DropBox/other sync service while you traverse it, hence your "cursor" into the filesystem (which generally is *not snapshot safe like a real _database_ would*) may point where you expect it to, but the sector/cluster is rewritten having its content (file names + attributes, etc.) re-ordered due to some file tree edit by DropBox et al. That's fundamentally what the VoidSoft forum messages about logfile inspection tells me: when FindFirst spits out "duplicate" entries, it means the underlying system is written to and you're not looking at a snapshot-safe system: NTFS et al are not classical databases that way, so this kind of "erratic jumping" *can occur*.

Which leaves explaining the completely fucked up intermittant file accesses by DropBox et al where the file content *changes*: that's rediculous, unless.... we invent the following scenario (which is rather crazy but which I *can* imagine sync services coding into their drivers anyway):

1. you are the only one accessing the "shared" file; no problem there, you'ld think.
2. you edit the file.
   
	Now let's ponder this a bit: the *usual* behaviour for regular files, such as text files or MSWord and that sort of stuff where you use an editor kind-of-application to do the modifying, your work (*changes*) WILL NOT hit the drive until you hit SAVE in the application menu. Or when the AUTOSAVE triggers, which, when available, happens, say, about every 10 minutes while you have that document open in your editor. 

	Compare this with any DATABASE application: these, by design, choose to save any and all edit actions to disk ASAP: while a text/document editor is engineered to *cache all edits in RAM* (and thus loose your work when the application crashes or the machine suffers a power failure! (Argh!!) your trusty *database* software is engineered to prevent such mishaps by saving ASAP; this means disk accesses by any database editor (such as Qiqqa and many other apps) is *very frequent disk sector edits* (subsecond frequency if you're quick in typing or running complex queries which touch various spots in that database); another difference in behaviour between *document editors* and *database editors* is that the former **rewrite the file in its entirety on SAVE** while the latter **write/append only the minimum amount of small chunks (sectors, a.k.a. "pages") in the file during query execution and on COMMIT (which is the SAVE command in database speak)**. Yes, when you know these technologies intimately, you can nitpick my paragraph here, but the crux remains: **infrequent, full rewrite always** vs. **frequent, very minimal, _partial_, writes always**. And it looks like the latter type is what is b0rking databases on DropBox et al. Anyway, onwards with our scenario...
3. Let's say you save an edit to disk (and continue working on the document/database at hand)
4. The DropBox/other sync service local software picks up the change notification (via inotify or similar file system change notification feature in your OS) and does this:
5. Step 1: read the local (new!) file contents; determine which pages have changed and send a copy of those changes to the server.
6. Step 2: *suppose this happens* (which is nuts, but would explain the corruption): the first remote DropBox server response is *the original content it knew from before*: thus you get a by-now-outdated *update* to your local file. Nothing bad happens UNLESS yours is that *database type of application*: the corolary of *minimal edits* is that it also does not cache the entire file in RAM but *reads frequently when it needs*: assume this happens just after this step, hence the database software may see OLD content if it happens to refetch one of those pages it so recently changed itself: those changes are on their to the remote server but *you* only have the old stuff locally right now. Bad news for your database *integrity*...
7. Step 3: very shortly after, the DropBox/other sync local driver software receives a regular "hey, we've got updates for ya!" notification from its remote cloud server, which carries (obviously) the changes the server received from you in (Step 1) above. The sync driver software updates the pages on disk and all should be well, except.... this1-2-3 behaviour is more-or-less safe&sound when *your applications are all the document editor kind*. 
   Unfortunately, *you were running your own flavour of database type application* and you've just experienced a *race condition* in the file system as the data on disk was very temporarily changed *back* outside your control.

I haven't been able to observe this myself, but from a (rather crazed, IMO) software creation and delivery point of view I can see this happening and it would nicely explain "logically" why people with databases see what they see when using those in a local "cloud share": the physical system writes are still to local disk, as usual, but that "cloud sync/storage" service software is adding another layer of complexity as this reads a lot like running multiple *unsynchronized database instances*: exactly the same type of disaster is waiting to happen and can, depending on astral planet alignment, your zodiac sign and whether your cat is a ginger or not, happen to you on a weekly basis or "almost never". You may be even be part of the lucky karmic majority who "never have this problem": it's that timing critical to go bad.

*Why* would I feel rather confident something along these lines (single read, double rewrite within interleaved second party access: a.k.a. one classic type of "*race condition*") happens with those "cloud storage" services. And why *all of them* seem to suffer this issue more or less often?

Because, well, if we look at it this way, you're actually talking about running **two database type applications on top of the same disk storage pages**: the key observation being that these applications, by definition, are **not synchronized**:  DropBox doesn't know about us and doesn't talk to us to share a "database lock" of any kind, and vice versa: Qiqqa / SQLite / you-name-it doesn't invoke specialized DropBox "disk page locking" APIs -- something that multi-threaded and multi-process database applications do *internally* on a daily basis (read: *very! frequently!*). If you start getting where this is going, you'll also understand why big database companies (Oracle, IBM are the ones I've had to deal with in the past) *mandate* that their database software runs on **dedicated hardware**, ergo: the PC business speak for this statement: "don't you *dare* running *any* other crap on that box of yours if you want us to give *any* guarantees, buster!" They have a long history and remember I said "classic type of *race condition*": they have been burned by this, *hard*, ever since the sixties, so nobody is allowed to *forget*.
Meanwhile we run a very respectable database engine (SQLite) ourselves, but ours is a scenario where we cannot demand *dedicated hardware*. So we're bound to learn some very costly lessons, again and again, because the "PC desktop & mobile" environment is, by design, not able to cope with particular scenario very well: we either need dedicated *inter-process synchronization operations* to *start* solving a thing like this and the problem with "desktop/consumer hardware" is not that we don't have those "*synchronization primitives*" (we DO have them!) but the fine print is: **all parties involved must agree to use them the same way PLUS use the same SHARED set** or the whole game goes bust!

Now think about this:

- "first to market" (a.k.a. "release sooner rather than later") is the business adagio -- for good business reasons
- would *competitors* like Google and DropBox be happy to sit down and (gad! extra delay!) come up with a common, single, public API to use to deal with a, what some decision makers will surely argue is a *niche*, *database type application* like ours?

*Njet towaritsj!* 

So, IFF those "synchronization / locking" APIs would be made available (they haven't AFAICT: extra cost in software architecture, in software development and in all other areas without obvious monetary gain to balance this hairy work as you can't have it done by some *junior dev* or you'll be facing a metric ton of *additional bugs*: software synchronization is one of the toughest software dev tasks, if not *the toughest*.
Ergo: you do the next best thing and publish a knowledge base blog/helpdesk page where you mention running database on top of a "cloud share" is "at your own peril"; as an extra you may want to list the applications that suffer from this.

And there we find SQLite: a highly respected database engine (and that respect has been earned with very good reason!) *also* mentioning this on their documentation pages: the only guarantee you get when running SQLite on top of "cloud shares" of your local drive is the guarantee that *one day it will will go wrong very badly*; the only open question is how long you'll have to wait for that "one day" to be yours.


I hadn't thought about the 1-2-3 scenario above as I *assumed* DropBox et al kept a closer check on disk state, but now that I've read yet another zillion pages with the equivalent woes of others, with other *database type* software (Zotero, TheBrain, ...) I intuite I've come quite a bit closer to the root cause. 
As to the why of the double-write (write back old, then rewrite new after update notice from server) by DropBox et al, which is necessary to *explain why the file contents change unexpectently while you use the file*, I can easily explain that way by the "first to market / keep it simple" argument that I've faced before: your software design becomes quite a bit *simpler* when you separate the updates-sent-to-server path from the synchronize-all-connected-clients-to-latest-state-of-affairs processes asynchronously: you *gain* simplicity in the part of your cloud sync software where you handle updates: either you ding the bell asking the server for an update *now* (a.k.a. pull method) or you get dinged around the ears as the server sends you notification of updates (push method), it doesn't matter: you just go and bluntly compare your file checksums against the central servers and request any file content that happens to differ, *pronto*. Since I said "asynchronously" this means that my asking for a copy from the server (because, hey, my copy doesn't match with theirs, DUH!) can *deliver* before my outgoing hey-we-got-a-change-for-y'all transmission has made it all the way through the server yet, which will therefor give me a now-old copy of the file to write, which I do (bam! temporary change back on local disk!), after which I'm done. 
Next thing that happens is the next round of asking for updates: this time around the server has completely processed our hey-we've-got-an-update-for-y'all and thus I get a new copy of the file, which is the one I started out with thanks to my own user / application doing that application database page write. And bam! We're back to a file which is exactly as it was.
My gain? I can focus on each of those *complex* processes independently, can play with any mix of pull, push and pull+push, and *live*. 
The *editor type applications* won't be bothered (I'm lying, but we'll get to that in a minute) by this as they always do whole-reads and whole-writes, so I'm good to go and no worries about my *round trip update/sync latency* -- because that is what that double-update story is about: the effects of *latency*. Which we CANNOT resolve in this setting; it requires some (pretty sophisticated and tricky) locking mechanisms and if we would like to fix that, now we're touching upon *network synchronization primitives* and those are still, as the phrase goes "an active research area", ergo: we can be pretty sure this is bloody hard to get right and then we have no control over the precise hardware yous is running and that problem becomes insurmountable: this is why the big database vendors **specify** which hardware you're allowed to purchase for *their* (pardon, your) database, unless you like to play wild and free and without any guarantee. ;-)
Oh, almost forgot: I said I lied. The full-read/full-write apps MAY also suffer from this double-update behaviour, i.e. latency / asynchronicity effects temporarily *reverting* disk page content, but disk reading and writing is *fast* so the total time spent, i.e. the total time frame *within which this problem may be observed* is *tiny* compared to those database type applications which basically read/write the database file as long as you run the application: several minutes to hours or maybe even *days* when you don't terminate and shut down your box! Next to that, there's *file locking* features for local drives that work in all modern OSes, so those full-reads and full-writes are "atomic", i.e. cannot be interleaved by the cloud sync service software: when an update of any kind came in, it'll usually have to wait until that application read or write is *done*, thus killing the latency/double-sync issue to infinitesimal chance levels: the only remaining scenario then is full-write+full-reread of the application very shortly after another and no sane application does that.
Which leaves database type applications: they do suffer and will suffer forever.

Okay. Crap! Any way out of there?

After a lot of testing with a few people who experienced this problem *frequently* (*mano negra*?) my earlier conclusion remains unchanged: the only way I could the suffering go away was by full-writing *new files*: of course, their problems were not just this (oh, if the universe were this simple!) as it *also* turned out that rewriting (*updating*) an existing file in a single full-write was *sometimes* causing failures. I blame *file locking* -- and the probable *absence* thereof: you need to call additional OS primitives to have those locks and then there's various flavours of those locks too, so what works for you in another scenario will surely haunt you in this "store on cloud share" scenario, so you can bet your arse the software I tested has different file locking *modes*, only one of which (exclusive locking) is the one you want for this. Then you're still depend on the cloud sync service to use the same *exclusive locking* and given that that locking mode is notorious for all kinds of other user issues, I take one guess of what is used by them.

As I haven't dug so deep as to reverse-engineer DropBox / Google Drive et al -- the days I pulled up IDA and went for it are behind me, alas -- this is all *conjecture*. I'm not betting the house on this (I may be crazy but not *that* crazy) but a nice bottle of single malt, hm. I feel confident I've got closer to the root cause; it explains (to me at least) a lot of what has been going on and going wrong. The poor bit is the fact my preliminary conclusions couldn't be budged a single bit: full-write it is, so no database directly on top of cloud storage, *evar*! 
The full-write file update issues I have observed alongside are not explained by this, but are explicable once you start doubting the correct *perfect use* of exclusive file locks by all parties involved (DropBox, GoogleDrive, Qiqqa, ...), which is very sensible doubt to have.
And there, again, we only have control over *one half of the equation*, at best.
So I stick with my earlier finding: only full writes to **new files**: this "new files only" bit prevents the async update-to-server processes from having their own ideas what should-have-been as for a **new file**, there has not been *any "before" at all*. Hence: scratch that first write=copy-from-server and thus no "reverting back" due to latency. Which explains why I didn't see any problems during those particular test scenarios, while all the stuff I tried caused all kinds of mishaps: it was only a question of how *fast* the first error would occur.

Thanks to the people who helped me test this before, who remained at the controls and donated their time and patience -- once you have found a "mano negra" I've learned to *not change anything(!)* if at all possible or the jinx goes away and you're left with a strangely-now-working-WTF system while you were ready to tackle the nasty bug, finally! Which is why the very rare people who suffer from this issue **and** are willing to spend hours if not *days on end* to help test with no guaranteed success in sight are so precious!! :+1:

References:

- https://www.dropboxforum.com/t5/Delete-edit-and-organize/Corruption-of-Excel-Files-saved-to-Dropbox/td-p/289813
- https://www.dropboxforum.com/t5/Create-upload-and-share/Dropbox-seems-to-be-corrupting-files/td-p/671053
- https://www.dropboxforum.com/t5/View-download-and-export/Image-appears-to-be-corrupted/td-p/681732
- https://www.dropboxforum.com/t5/Create-upload-and-share/Syncing-is-paused-while-a-file-in-this-folder-is-open-close-the/td-p/747648
- https://www.dropboxforum.com/t5/Create-upload-and-share/Keepass-database-file-will-not-update-properly/td-p/713138/page/2
- https://www.reddit.com/r/webdev/comments/ebp26q/what_not_use_dropbox_as_a_database/
- https://www.sqlmvp.org/backup-detected-corruption-in-the-database-log/
- https://wordpress.org/support/topic/customers-are-getting-corrupted-files/
- https://github.com/rclone/rclone/issues/3609 :: Randomly, pushing files to Dropbox results in corrupted file or directory names that contain Unicode code-points
- https://positek.net/dropbox-sync-not/
- https://forum.openoffice.org/en/forum/viewtopic.php?t=102344 :: \[Solved\] File saved in Dropbox only opens corrupted in Word!
- https://github.com/dropbox/dropbox-sdk-js/issues/71 :: Files get corrupted when uploading
- https://community.spiceworks.com/t/database-corruption-in-3-databases-this-morning-stumped/601744
- https://forums.zotero.org/discussion/56316/zotero-database-on-dropbox-box-etc-a-very-bad-thing
- https://answers.microsoft.com/en-us/windows/forum/all/dropbox-file-or-directory-is-corrupted-and/f750a927-7bd8-46d5-bde9-f6b997f0cea0
- https://canterbury.libguides.com/c.php?g=243283&p=6309836
- https://forum.xojo.com/t/database-on-dropbox/30854
- https://nira.com/dropbox-not-syncing/
- https://news.ycombinator.com/item?id=6502229 :: [How to Corrupt an SQLite Database File](http://www.sqlite.org/howtocorrupt.html)
- https://forums.thebrain.com/post/dropbox-will-corrupt-your-brains-database-and-so-will-any-other-filesync-service-8926176
- https://news.ycombinator.com/item?id=4703943 :: [Dropbox Bug Can Permanently Lose Your Files](http://konklone.com/post/dropbox-bug-can-permanently-lose-your-files)
- https://www.easyasaccountingsoftware.com/mworkplace/corruptedfiles.htm :: How to Fix a Data Corruption when using DropBox
- https://www.zotero.org/support/sync#alternative_syncing_solutions
- https://forums.zotero.org/discussion/72556/can-my-zotero-data-stored-in-dropbox-be-redirected-back-to-the-original-default-location
- https://discourse.omnigroup.com/t/database-dropbox-sync-a-causes-database-corruption-use-omni-sync-server/973/2
- https://www.voidtools.com/forum/viewtopic.php?t=8931 :: database always corrupt
- https://github.com/MiniKeePass/MiniKeePass/issues/26 :: Dropbox sync and corrupt database
- https://forums.zotero.org/discussion/66980/dropbox-a-prompt-for-a-solution
- https://github.com/bpellin/keepassdroid/issues/99 :: Database corruption when using Dropbox integration

> 
> Basically, storing the Zotero database (zotero.sqlite) in Dropbox is a very bad idea, because Dropbox will try to sync the database file after each change, which is very likely to lead to corruption if Zotero is ever open in two places at once (and it might also lead to problems if you ever try to open/edit/view the database file using the Dropbox website, iPhone app, etc.). Basically, databases are simply not built to be synced with tools like Dropbox.  
>  
> The easiest way to use Zotero is just to use stored attachments and sync files using the Zotero file storage. This requires the least setup and also has the advantage that the attachment files will be viewable on the zotero.org web library interface. (This is also the only way to sync attachment files in groups due to technical reasons.)  
>  
> A second option for syncing attachment files in My Library is to use stored attachments with a WebDAV-enabled cloud server (e.g., box.com). This is also fairly easy to set up (you just enter your WebDAV account information in the Zotero preferences), but you can't view your attachments in the zotero.org web interface or use this to store group library files.  
>   
> If you really want to use Dropbox, you can use linked attachments instead of stored attachments. You set the links to point to the PDF files stored in Dropbox. You can use the Zotfile plugin to automate this process. This is safer than storing the entire Zotero database file and folder in Dropbox because the only things being stored in the cloud sync folder are the PDFs themselves. From Zotero's perspective, it is only storing the file path for the linked file, so nothing that Dropbox does touches anything that Zotero pays attention to.
> 












## Using other applications to get the UI and feature set we seek

- Zotero: use that one as our reference manager and general bridge to citations: everyone who needs the citation feature in their preferred editor should use Zotero: that takes away a **lot** of support work for us as we won't need to support any citation "plugins" any more, when we enable *back & forth* import/export to/from Zotero.
  
  We need *back & forth* import/export to cover the scenario where somebody:
  - applies some last-minute changes to their citation references in Zotero: those changes should make it back into Qiqqa!
  - shares his/her references/citations list with team members who only use Zotero: when the team sync's those Zotero databases, we (Qiqqa) should be part of that sync/update.

- https://github.com/elias-sundqvist/obsidian-annotator : use Obsidian UI for PDF note taking.
  
  This has the side benefit that the user can do all sorts of note taking as available through Obsidian; we must be able to import those annotations into Qiqqa (and rewrite them on Qiqqa sync/update), so another *back & forth* channel.
  Obsidian has a nice interface for this and the use of the hypthes.is software should solve that UI/UX problem for us, leaving us with another challenge: 
  - import/export from/to Obsidian
  - how do we jump **into** Obsidian when the user does a text search or otherwise in Qiqqa and wants to jump to the indicated document / annotation?

Anyway, the above set of applications might help us getting a full feature set going faster, without the cost of redoing their work.
The drawback is that the user will be switching applications as part of their workflow, instead of running only *one* application: Qiqqa.

Another problem is the duplicated storage of PDF documents when using Zotero: according to their documentation they only sync documents when those are not "linked"; now we *could* code Qiqqa to use hardlinks and thus use the Zotero copies as master documents, but nothing in their documentation tells me that they are careful about keeping the *documents* untouched: Zotero is a *reference manager* and doesn't seem to care particularly about *non-identical copies of document X* the way I (Qiqqa) do, so this is dangerous. 
Should Zotero only be used on a per-project basis for this? That's a restriction that nobody will care about in practice, so we'll have a serious problem with document duplication when running this set over a 10K+ *large library* like the ones I have. Hmmmm.




