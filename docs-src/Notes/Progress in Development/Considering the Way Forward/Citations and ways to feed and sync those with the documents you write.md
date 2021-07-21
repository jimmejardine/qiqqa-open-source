# Citations and ways to feed and sync those with the documents you write

Qiqqa historically has a MSWord plugin for injecting citations from Qiqqa into MSWord, but these days folks are using quite a few tools for producing papers, both in printed and on-line form:

- Microsoft Word
- Google Docs
- LibreOffice / OpenOffice
- WPS Office
- Scrivener
- any MarkDown editor, e.g. MarkdownMonster
- any generic text editor which can be used to produce MarkDown or a comparable text format which is then processed (PanDoc, ...): Sublime, Notepad++, UltraEdit, ...
- ...

As we need to keep the required development and maintenance effort to a tolerable level, the question then becomes: how can we serve them all, or most of them, 
in a way that is agreeable to users?


## MSWord

Well, I guess we stick with the existing plugin as long as it lasts (some folks use Office 365 and I don't know if that one's still compatible).

- https://github.com/zotero/zotero-word-for-windows-integration
- https://www.zotero.org/support/word_processor_plugin_manual_installation
- https://www.zotero.org/support/word_processor_plugin_installation



## LibreOffice

Well, at first I thought "why not import/export/sync the Citation Database that comes with LibreOffice Writer and the docs you write?

Turns out that's an ODB file and effin' Libre/OpenOffice did a totally swell job of making that one "locked technology": one star from Mr. Ballmer for you; man, old Microsoft couldn't have done it better than y'all. YES, I AM UPSET! 

The bottom line is this: you CANNOT access a Libre/Open Base database like that other than by using LibreOffice/OpenOffice Base. Seriously! 

I've looked around and everyone is hitting the same showstopper: you're totally screwed if you want to create software that's intended to be doing CRUD on a `*.ODB` database. 

The only clunky round-about would be to UNZIP the ODB, PRAY that it is a HSQLDB (for *theoretically* it my be any of many database types in there as the ODB file 
is only a ZIP-like wrapper; in actual practice you MAY encounter a Firebase database instead as they switched to that one as the new default, or so I hear, 
but my LibreOffice 7.0.4 install on the dev box begs to differ; anyway...). When the database stored inside that ODB, which now should exist in an unpacked directory, is a HSQLDB format, 
you MAY go and try to write a bit of JDBC Java code to access it and read / write some records in there, then bundle/ZIP it back up into the ODB file and hope Libre Writer hasn't gone
total bonkers while you did this.

Forget automation there and don't get me started on the 'headless Office' inroad, using UNO: the documentation there is so fine (auto-generated like nearly all Java crap from the horrid source tree) you'll get swamped out before you even start: I tip my hat to all of you who got anything decent done via 'headless' at all, but then do observe that nobody out there gets anything done outside document conversion and printing so I suppose the great UNO API isn't useful for anything except those two functionalities either.

Sigh.

So I am considering going through Zotero or someone else to channel their work into getting any citations into any Libre/OpenOffice workflow, as all my enthousiasm there is revolving around a imaginary .45 and some heavy flossing. Good God, and there I thought Qiqqa had some *issues*. Face. Palm.

- http://www.ocsmag.com/2018/11/30/replacing-libreoffices-bibliography-tool-with-zotero/ -- that page describes a lot of the bad stuff that I discovered and made me very angry before I ran into that one. TL;DR: If you're sane: Fuck it and ditch thee LibreOffice functionality; use an external plugin instead (Zotero, Mendeley, what-ever you got)! So I guess we'll have to add Qiqqa to that list of 're-doing the hard way what the Libre/OpenOffice *(censored)* monkeys should have enabled already via database import/export or other quick list add/update means.
- https://academictips.org/mla-format/mla-format-openoffice/ -- useless page but comes up in lot of queries I launched, so Hail their SEO. Or whatever. LibreOffice gave me a raging case of PMS; guess you notice?...
- https://wiki.documentfoundation.org/Referencing_Systems_in_LibreOffice
- https://isg.beel.org/blog/2014/03/11/docear4libreoffice-docear4openoffice-call-for-donation-2500/ -- so I guess the 'market value' is around 2500 quid. Brave man.
- https://extensions.libreoffice.org/en/extensions/show/zotero-libreoffice-integration -- Hmmm. Might not be the brightest idea I had tonight to consider taking that one for a ride.
- https://www.ubuntubuzz.com/2016/02/how-to-create-basic-citation-bibliography-zotero-libreoffice.html
- https://docs.jabref.org/cite/openofficeintegration -- to be investigated further...
- https://wiki.openoffice.org/wiki/Documentation/OOo3_User_Guides/Writer_Guide/Adding_a_reference -- this is what your users are expected to do *by hand*, which DOES NOT mesh with the fact that that Citations Database is total and utter crap as you cannot export or import or anything but edit it manually via a honkin' form dialog.
- https://wiki.openoffice.org/wiki/Documentation/OOo3_User_Guides/Writer_Guide/Creating_a_bibliographic_db
- https://wiki.openoffice.org/wiki/Documentation/OOo3_User_Guides/Writer_Guide/Adding_a_reference
- https://wiki.openoffice.org/wiki/Documentation/OOo3_User_Guides/Writer_Guide/Formating_the_bibliography
- https://www.ubuntubuzz.com/2015/07/how-to-create-apa-style-bibliography-in-libreoffice.html -- yeah, no CSL for y'all!
- https://www.ubuntubuzz.com/2015/06/creating-basic-bibliography-libreoffice.html
- https://www.linux-magazine.com/Online/Features/Zotero-and-LibreOffice
- https://wiki.openoffice.org/wiki/Database
- https://forum.openoffice.org/en/forum/viewtopic.php?f=40&t=28803
- https://github.com/zotero/zotero-libreoffice-integration -- worth USD 2500 if you ask me. Sounds like these folks at least got the friggin' mess working. *Me gonna borrow.*
- https://ask.libreoffice.org/en/question/88866/base-firebird-vs-hsqldb-embedded-database/
- https://bugs.documentfoundation.org/show_bug.cgi?id=51780
- https://wiki.documentfoundation.org/Development/Base/FirebirdSQL
- https://blog.documentfoundation.org/blog/2018/08/06/dbms-migration-in-libreoffice/ -- Did those tenders reach the Citation Database in Writer, eh?!?! Not that it matters much as the UX/DX (Developer Experience) around that bozo is horrid in 7.x still.
- https://github.com/sbraconnier/jodconverter/ -- a sidetrack of mine, while I was looking into 'headless Base' as another potential to get that ODB open and accessible programmatically without a huge learning curve and Java hacking.
  + https://github.com/lcrea/libreoffice-headless
  + https://github.com/hannesdejager/docker-libreoffice-api
  + https://help.libreoffice.org/7.0/en-US/text/shared/guide/start_parameters.html?DbPAR=SHARED
  + https://stackoverflow.com/questions/55070766/is-libreoffice-headless-safe-to-use-on-a-web-server
  + https://api.libreoffice.org/examples/examples.html#CLI_examples -- *grrrrreat* docs.
  + https://listarchives.libreoffice.org/global/users/2011/msg04383.html
  + https://github.com/0xfeeddeadbeef/LibreOffice-ExportPDF-Demo -- which got me started on that 'headless' sidetrack. Hoping for impossible salvation on the off chance... **Alas.**
- https://stackoverflow.com/questions/15777281/node-js-connection-library-for-libreoffice-base-database
- http://hsqldb.org/ -- what they say they're using underneath in Base.


## WPS Office

Dunno if there's an API for plugins ar whatever. Not enough market share in my neighbourhood to make it worth the effort right now.

- https://blog.wps.com/citations/


## Google Docs

Guess I'll have a look at the work Zotero did.

- https://github.com/zotero/zotero-google-docs-integration




## The others:

- Scrivener
- any MarkDown editor, e.g. MarkdownMonster
- any generic text editor which can be used to produce MarkDown or a comparable text format which is then processed (PanDoc, ...): Sublime, Notepad++, UltraEdit, ...
- ...

I hope I can get away with outputting a generic copy&paste text-based 'footnote'-like output that can be copied over into the doc you're working on and then the processor takes care of formatting the ref/link to the References sections at the end where all the citations are.



# Here's how a few others do it

- https://support.alfasoft.com/hc/en-us/articles/360000976497-LibreOffice-and-EndNote
- https://researchguides.uoregon.edu/Mendeley/workscited 
- 


# General idea about workflow

I considered coming up with a 'Project' type where you gather all your citations so we can generate and then *re*-generate them whenever you want, but then I decided a 'Project' like that is just another library: what I want to have anyway is a 'sharing among libraries' where metadata done in one is 'synced/shared' with all your libraries which carry the same document, i.e. "edit in one; see everywhere". (Hm, maybe not for annotations, but definitely for the BibTeX, and *that* is what goes into the citations anyhow.
