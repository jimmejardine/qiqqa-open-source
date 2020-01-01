# TBD 
#### (structure this blurb, which was written as stream-of-consciousness while answering [#138](https://github.com/jimmejardine/qiqqa-open-source/issues/138) -- probably should be turned into design docs series, as some of the work done and dev paths visited should also be documented)

> Will you be interested in a complete UI overhaul , or at least willing to see some updated UI/UX samples?

Absolutely.  :+1: 

A working UI demo in C# has a very significant added value as I have to divide my time between various priorities and if you've looked closely at the commit tree you'll have seen I've been fighting with the Design View/XAML environment several times already, so *any* assistance in getting Qiqqa towards a more usable and modern UX is appreciated.

FYI: my main focus is getting the Qiqqa functionality *working* again, with myself as the major test case: Commercial Qiqqa broke down fatally for me about 2 years ago, which killed my ability to search my library, resulting in some angry reverse engineering work in https://github.com/GerHobbelt/qiqqa-revengin and ultimately turned out, once Qiqqa was open sourced, to be caused by a plethora of issues (most of 'em closed now) around PDF rendering, OCR and text indexing. Of course, when you remove one layer of loud bugs and noise (memleaks/ sudden death Qiqqa crashes while you are *using* it), you can expect more insidious bugs to crawl out of the woodwork (#135, #136, the very very nasty #129, which answers my own question "why can't Qiqqa *some* of my documents I *know* are available in the library?") so I am still focused on getting the underlying functionality working with a (very?) large library of about 40K PDF files from all walks of life.
The rest of the work done in Qiqqa is just me doing finger exercises to keep myself motivated while working on the big problems. 😉  Consequently, *any* assistance would be much appreciated!

Workflow/process-wise, I'd suggest you fork the repo (I don't have full owner access to Qiqqa so I cannot add collaborators myself, Jimme has to do that when he's got time) and I can then merge at any time. Tip: fork off my own fork as that's the bleeding edge in development and would make my merge job maybe a tiny bit easier as we progress, but that's just a guess.

What I (or you yourself) can do is add a project to the DevStudio solution to do the new UI development in and take it from there. That would help in two ways:

1. On merging our work, we wouldn't have to worry over merge conflicts in git, making that process really fast and easy.
2. It would give me a target project to migrate Qiqqa *functionality* to in a way that would give me separated functionality and UI the way I want to accomplish in due time:

  Another task that's lurking for/in me is to pull apart the UI and Qiqqa functionality as that's a closely knit jumble in several spots right now and makes it that much harder for me to move towards a final goal of mine: make Qiqqa \[subprocesses\] more or less scriptable so that power-users like me can tailor it to fit their particular document workflows, e.g. PDF preprocessing, custom auto-tagging, #51, etc.)


## Side notes: 

* I've been looking at migrating this thing towards using something like [MUI](https://github.com/firstfloorsoftware/mui) but that project has seen little activity lately AFAICT (also in its dev forks) plus I am sometimes already upset enough by DevStudio Design View + XAML itself, so I decided I don't need that extra hassle. 

  Bottom line: if we can make it look *nice*, there's a possibly larger market for new users, despite the big sluggos Zotero and Mendeley, as there must be others like me, who have found those wanting. (I've looked at getting a Qiqqa-style sniffer into Zotero and I got a headache. Mendeley seems to have a focus that I can't really put to words but somehow *feels* perpendicular to my use of a \[document\] library, which is mostly *search*, not citation. (My *output* is generally a bunch of PDFs plus metadata for others to peruse, not an article with some citations. Now that I write this, I think it's more a *filtration* job than a *writing* job that I do.) Then there's DTSearch which gave me the willies almost immediately, so I guess I'm stuck with Qiqqa for life. Looked at ReadCube and a few others: all *no dice*.)

* I would dearly love it when the new UI 'works' in both Design View (or Microsoft Blend) and in real life so that I can always *look* at the screens instead of having to crawl through XML (*yuck*!): current Qiqqa is sometimes very obnoxious when it comes to previewing UI changes and debugging those.

* **There a 2 main functionalities which must work in any UI**: 

  + *Modern* Embedded Browser (Chromium) as I need to come up with a fully functioning Qiqqa Sniffer again and thus fix #113. Given the C#/WPF constaint, I guess we have a choice between [CEFSharp](https://cefsharp.github.io/) and [ChromiumFX](https://bitbucket.org/chromiumfx/chromiumfx/src/default/) -- I don't know which I'll pick, because there's some hairiness regarding getting your paws on the network messages + browser cache or other ways to access the PDF being downloaded/previewed in the browser pane plus web page DOM access in case you want to grab BibTeX or similar metadata off a web page: several sites are not well-behaved in providing BibTeX *downloads*.

   This is not relevant for UI design *per sé*, but *is* relevant when we start to integrate UI and functionality as having an embedded browser like that in there would also require UI elements like URI edit bar, back/forward history move, etc.

  + PDF viewing with overlays (sniffer has a PDF view pane with text selection & marking, plus visual feedback as marking text/authors, etc. which match the metadata grabbed off the net and make the Sniffer turn green to signal a metadata *match*)

    For that, I guess we'll go with someone like [PDFSharp](https://github.com/empira/PDFsharp) as that seems to be the more active and workable PDF render library. (I haven't looked at [iText7 a.k.a. iTextSharp](https://github.com/itext/itext7-dotnet) for PDF text extraction, as [MuPDF](https://github.com/ArtifexSoftware/mupdf) is currently used by Qiqqa and functioning well (`pdfdraw -tt` is used by Qiqqa as part of the PDF indexing process).



