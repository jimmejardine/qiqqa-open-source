# Code Qiqqa on a completely new platform or not, that is the question

(from chat)


> One has to wonder at what stage should one consider replacing it with a completely new platform?

Well, replacing WPF with something else so we can get Qiqqa Linux users alongside Qiqqa Windows users, while still having a single, common, code base for maintenance, is *very* close to that.

When I say I am looking into "moving to an electron-like system, e.g. Chromely or anything else CEF-based", that unwraps to this total sum:

- the code driving the UI (buttons, lists, scrolling, menus, etc.) will have to move from C# to JavaScript or TypeScript. "move" here being an euphemism for "rewrite". 😉 
  
  Anywhere WPF or XAML gets mentioned in relation to Qiqqa (that's the UI tech currently used for it and it's Microsoft/windows only), the "moving to electron/CEF" means that part is going to be redone as "HTML web pages" one way or another.

   electron/Chromely/CEF/... are all basically concepts of a stripped down Chrome browser glued to some "business layer back-end": 

   - electron is Chrome+NodeJS, so JavaScript all the way (not my first choice therefor), 
   - Chromely is Chrome+C#/.NET, which would make my *hope* to keep most of the business logic in C# at least *possible* (regrettably Chromely isn't very active, nor is it *finished* for what I need from it 😓 ) and 
   - "CEF" is me saying: the heck with it, I'll take that CEF core (~ stripped Chrome) and do a bare minimal UI driver around it, so I can have a dual CEF UI cross-platform: one (safe) CEF for the Qiqqa UI, one (internet facing) CEF for Qiqqa Sniffer + Qiqqa Browser (web access risks: security!), where I can get my hooks into it for network traffic access and DOM = rendered web page access for Qiqqa Sniffer, thus re-enabling current PDF auto-download+import functionality + BibTeX metadata auto-download / web page scraping.  
   
      The Qiqqa "business logic" can then be kept in C# and moved into a second local application ("web server" like) which communicates with that CEF-based UI to recreate Qiqqa. This third variant is very similar to the first variant (migrating to `electron`), technical hurdles and effort/risks-wise.

- I *hope* I can keep a large part of the current "business logic" (what makes Qiqqa be Qiqqa) in C#.

   Though I am approaching this like the classical solution to the Gordian Knot though: wielding an axe and observing which parts survive. (Alexander wielded a *sword*; I'm not qualified for that 😄 ) 

- First *minor* counterpoint to that last one (keeping as much of the original C# code as possible) is my current activity around PDF processing, which is to replace SORAX + QiqqaOCR + very old tesseract.Net (document OCR + text extraction + metadata extraction) with something new, based on Artifex' MuPDF and "master branch" = leading edge `tesseract` v5.0 code-bases, which are C/C++. Ultimately that should take care of #35, #86, #165, #193 and (in part) #289. 

   QiqqaOCR (a tool used by Qiqqa under the hood) is currently C# glueing those old libraries together and is being replaced that way with an entirely different code-base in C/C++. Linux-ready? Yes. That part will then be ready for Linux et al, requiring some CMake work (or similar) to compile that collective chunk of software on Linuxes, but the code-base itself won't be in the way and the parts used are already in use on Linux platforms, individually.

- Second *larger* counterpoint is the current C# Qiqqa code-base, which has its "business logic/glue" quite tightly intertwined with the UI code: that's a bit of a bother to untangle.  The v83 experimental Qiqqa releases and the UI issues reported by several users during the last year or so is me fiddling (and screwing up) with that Gordian knot while spending too many hours in travel + house construction work.

- Another planned mandatory migration is getting rid of the antique Lucene.NET, which is the core facilitating the *document / text search* features in Qiqqa. Lucene.NET still exists out there, but is rather slow in upgrading and much less actively supported than the *true original*: Lucene, which is done in Java. Java+C# is a mix for the sufferers in the (unmentioned) 9th level of Dante's Hell, so best to avoid it, which is why I'm opting for using SOLR, which is bluntly speaking Lucene wrapped in a (local) web server. C# has no trouble talking to web sites like that, so we're staying clear of Java+C# tight mixes. 

  Meanwhile, getting Qiqqa to use SOLR instead of a Lucene clone/port, also means I can open up Qiqqa and give users direct access to the full search power that's sitting & waiting in there: handy for those of us who are doing meta-research / meta-searches: direct search access to the textified PDF documents in your libraries. That was/is an important aim of my own to achieve with Qiqqa.


As with any "redesign" / "redo", all the stuff Brooks learned the hard way applies and has to be kept in mind: Second System Syndrome et al (
---

## Conclusion

