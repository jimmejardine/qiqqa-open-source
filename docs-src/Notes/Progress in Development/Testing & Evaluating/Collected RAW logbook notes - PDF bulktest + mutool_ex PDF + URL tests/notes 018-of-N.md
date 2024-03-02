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

















