
# v82pre release: v82.0.7568-29227

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.






2020-10-21
----------


* (b74f18ff) prevent superfluous fetch of illegal URI 'about:blank' when the Sniffer dialog window is opened.

* (612bc454) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/253 post cleanup: making sure all SghowDialog() calls (which return a `bool?` type!) are checked the same way, by explicit comparison against `true`, so `null` and `false` are always treated the same way.

* (d127ba28) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/253 : this uncovered a deadlock situation due to pdf_documents_lock and access_lock interplay via the Associate...() call going directly to the PDFDocument (hence access_lock) while one of the background threads was fetching a list of documents to inspect via Library (hence pdf_documents_lock -> access_lock for each doc): the deadlock occurred because the Associate..() call internally would *add* the new PDF into the Library (hence: pdf_ddocuments_lock inside an access_lock zone, hence DEADLOCK with the bg thread!)

  Also note that this issue uncovered another matter: PDF association with a Vanilla Reference was not working AT ALL: fixed that as well. (WARNING: association via 'FromWeb' a.k.a. SearchWeb will not deliver AFAICT: only FromLocal associations will deliver.

  This uncovered yet another bug, which involved the metadata copying code (which still has some TODO's to be addressed at a later time!) resulting in the associated PDF then being marked as a vanilla reference *itself* due to overzealous metadata copying, which includes the 'FileType' field: 'pdf' or 'vanilla_reference'. Fixed as well.

* (af76f546) misc code cleanups: no functional change

* (ca328e0e) all loops which run for any noticable time should be abortable for when the application shuts down (terminates on user exit)

* (63bab7af) remove superfluous check&set: the variable has already been set when the condition is true




2020-10-08
----------


* (48e0186b) Cherrypicked BibTeX TeX-to-Unicode work downe by @mahfiaz

* (6162ac9a) Fix strange empty space in PDF view when PDF is available.

* (8d226995) Fixed unintentional Alt+W shortcut created in earlier commit.

* (3ccea169) Add Ctrl+W change to another similarly used place.

* (e511ed99) Suppress annoying exception.

* (d3dcb5f0) add shell script to more easily push the local repo to both my fork and the mainline repo.

* (aec72d23) improve upon SHA-1: 0998a2fe8d5e2b0bc19cb475280edf87c26b641f and fix the checks for GDI+ errors (due to Gecko for example, see https://github.com/jimmejardine/qiqqa-open-source/issues/244)







# v82pre release: v82.0.7555-31312

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.






2020-10-08
----------


* (4fd54da4) updated npm packages and Qiqqa version before release.

* (0998a2fe) provisional fix for https://github.com/jimmejardine/qiqqa-open-source/issues/240 : do not quit/abort/terminate the application when the unhandled exception is of a certain type/message and/or comes from the threadpool instead of the UI thread itself.

* (cfbc9c34)..(42fab986) added GoogleScholar_DoExtraBackgroundQueries configuration option (OFF by default) which controls whether vQiqqa will scrape Google Scholar in the background while you view/read a PDF document. Killing this behaviour (which only renders results in the sidebar of the PDF reader under the Google Scholar section there: articles of interest to view next) ensures your https://github.com/jimmejardine/qiqqa-open-source/issues/225 RECAPTCHAs will take that while longer to appear as Google counts your number of Scholar site visits before it fires that one off.

* (0db7606b) removed old v82beta cruft in the save windows positions code: simplified.

* (2483a3a8) This might fix https://github.com/jimmejardine/qiqqa-open-source/issues/220, at least its reported **symptom** of the page index being out of range.

* (bce0d603) fix https://github.com/jimmejardine/qiqqa-open-source/issues/236:

  per http://www.blackwasp.co.uk/WPFItemsSource.aspx where it says:

  > When ItemsSource is set, the Items property cannot be used to control the displayed values. If you later set ItemsSource to null, Items becomes usable again.

  Flipped the ItemSource nulling and clearing of the list.




2020-10-07
----------


* (0949df13)
  - fix 'Bundle Logs' command in the Configuration panel
    + moved the bundler action (7zip based) to a background thread as well, so we can see some (faked) process in the UI. The 'progress' there assumes the whole thing should be done in under about 2 minutes and sticks to 95% done when the bundling takes longer. No more effort to make this correct will not be underkaen as this good enough for a diagnostics reporting utility feature.
  - fix typo in message

* (526a38d0) 'Garbage Collect' button locks up as the GC didn't have pending finalizers when we hit the button: remove that call so we don't get surprises like that.

* (297a0862)
  - added code to properly shut down Qiqqa after an Unhandled Exception has been reported.

    This should fix obscure hangs of Qiqqa (without a window visible) and has been tested to work with **intentionally buggy** code in the ConfigurationManager (which code has been corrected before this commit)

    The consequence of this is that Unhandled Exception report dialogs should be the last thing you see before the application terminates. After all, such a situation indication the software has arrived at an unknown state, where we don't know exactly what's going on any more, so the best thing to do then is to terminate and let the user retry/restart the application.

  - augmented the code which checks the data BASEDIR at startup by looking at the commandline and registry.

    A directory path specified on the commandline will now be created, if it does not exist yet.

    If Qiqqa fails to create/access said directory, Qiqqa will fail with an Unhandled Exception and terminate after you've closed the error dialog.




2020-10-02
----------


* (8b0c9d5c) moved document listing a few nasty URLs for PDFs to look into when we have a new Sniffer working.

* (aab352d8) added more documentation notes




2020-08-21
----------


* (df2618a4) added doc about problems users may run in when downloading the installer off the net. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/223

* (f1ec34b4)..(e2148680) writing the qiqqa download + install procedure for users as per https://github.com/jimmejardine/qiqqa-open-source/issues/228 request. HTH.




2020-08-17
----------


* (95c4a7d2) adding links to the dev notes which I hadn't saved yet.




2020-08-16
----------


* (de79696e) Should we move to use ElasticSearch, Solr, Lucene or simply update to latest Lucene.NET?

* (8b22dddd)..(f21d80fb) Update `Qiqqa Sniffer - or how to access Google Scholar et al.md`

* (fb38c523) updated the notes re Google Scholar. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/225 https://github.com/jimmejardine/qiqqa-open-source/issues/113

* (6631d756) This commit contains the postprocess script for the previous commit's cleanup. The original HTML files from BING are not included in the repo as they're huge and quite messy, only loading the repo to no use. -- getsatisfaction was totally nuked. As an earlier site mirror run got b0rked, all that was left was grabbing what remained in the search engine caches (used the BING 'cached' option). HTML pages have been roughly cleaned after the fact. GS pages are a mess, though. :-(     'fixes' https://github.com/jimmejardine/qiqqa-open-source/issues/218

* (581ffaa6) getsatisfaction was totally nuked. As an earlier site mirror run got b0rked, all that was left was grabbing what reamined in the search engine caches (used the BING 'cached' option). HTML pages have been roughly cleaned after the fact. GS pages are a mess, though. :-(    'fixes' https://github.com/jimmejardine/qiqqa-open-source/issues/218




2020-08-11
----------


* (a9c352f7) rebuilt library files in Release mode for testing build process.

* (7365e1e1) augment comment SHA-1: deb707d6e2e6e06c38561c97afd49df993b77fb6 (bloody VS2019 won't produce anything but a weird error 127 (error -1) for any Qiqqa.org project Build command, be it invoking `npm` build commands, direct `bash` script execution or `Makefile` use. WTF?! --> removed the build commands in that project) as that commit had only killed the trouble in the **Debug** build target. Caught by Kenelm McKinney. Now also killed=fixed for **Release** and **Release+SETUP** Build targets.

* (04d19311) Reverted removal of icon needed for alternative.to nagger.

* (bc885099) partially reverted SHA-1: 09c5b1c9d268863297fa66b75f0c63dd18b0b3d4 :: Remove 5 more useless icons.




2020-08-10
----------


* (07bb4046) Update README.md

* (d0f69706) Update README.md

* (f30c67fe) Update Qiqqa Sniffer, BibTeX grazing and Google Scholar RECAPTCHA and Access Denied site blocking errors.md

* (a34827df) Update Qiqqa Sniffer, BibTeX grazing and Google Scholar RECAPTCHA and Access Denied site blocking errors.md




2020-08-09
----------


* (cde626f5) Create Qiqqa Sniffer, BibTeX grazing and Google Scholar RECAPTCHA and Access Denied site blocking errors.md

* (1b6cf53a) Update Links to Stuff To Look At.md

* (ed4ab929) Update Links to Stuff To Look At.md

* (df25e278) working on the docs

* (07627d33) notes, slightly edited, from email. To be worked on later.

* (7d7810c5) further work on the docs

* (86499dad) updating CRLF trouble files following .gitattributes update and forced clean + `git reset --hard` to prevent this issue on these files in the future.

* (b9f4d6ce) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into master

* (9ef4afd3) Merge remote-tracking branch 'remotes/GerHobbelt/master' into master

* (83c1e7bf) updated npm packages

* (589770f0) comment spellcheck fixes

* (794ac496) getting bothered by CRLF vs LF line ending issues again :-(

* (805dc250) working on documentation for developers

* (47d09105) add technology research documentation notes: further work on having looked into upgrading the embedded web browser : https://github.com/jimmejardine/qiqqa-open-source/issues/2

* (28f81d66) Create How to build Qiqqa from source.md

* (90a4f2a7) Update Processing PDF documents' text and the impact on UI+UX.md




2020-08-06
----------


* (1e696ea3) Merge remote-tracking branch 'remotes/GerHobbelt/master' into master

* (33b3f30e) Create Qiqqa Functionalities & Technology Areas.md




2020-08-03
----------


* (e33850a8) Typo fixes

* (d0abfbe5) list of projects I have looked at / annotated what their value could be for Google Scholar scraping, i.e. Qiqqa Sniffer, etc. Annotations (the "Analysis Notes" sections in the doc) need to be completed.




2020-08-02
----------


* (c4bbd335) ignore obnoxious windows' desktop.ini files.

* (bae78baf) Merge branch 'master' into mainline-master

* (517aab82) CRLF

* (e45d84e9) remove Qiqqa.org site build commands as they are not working right now in VS2019 - no idea why exactly.

* (dddb553c) remove local npm package `deGaulle` until that one hits first release.

* (deb707d6) bloody VS2019 won't produce anything but a weird error 127 (error -1) for any Qiqqa.org project Build command, be it invoking `npm` build commands, direct `bash` script execution or `Makefile` use. WTF?! --> removed the build commands in that project.

* (5a31d8ea) removed lingering NuGet package errors

* (aabf3828) fix following NuGet packages update: ApprovalTests library interface change.

* (3856ca03) updated NuGet packages and removed lingering troublesome config lines in C# project files

* (0e05231d) CRLF

* (22890bab) updated NuGet packages




2020-07-06
----------


* (8824d7cb) updated image links in old manual; tweaked base page to use wikilinks.

* (0249d7f5) documentation: a bit of reorganization as we work on the generic site generator.




2020-07-03
----------


* (5015bf93) updated npm packages




2020-07-02
----------


* (fcb2dae3) bit of documentation reorg + testing MarkdownMonster as additional tool for Markdown editing (even while we will render that stuff with another toolchain, which has more advanced MD features than MM (or any other editor) supports.

* (c2f0849e) Rename Progress in Development - Considering the way forward - Qiqqa library storage, database, DropBox (and *frenemies*), backups and backwards compatibility.md to Progress in Development - Considering the way forward - Qiqqa library storage, database, DropBox (and frenemies), backups and backwards compatibility.md

  Whoops! Windows! -- Doc was written online in GitHub web interface :'-(




2020-06-24
----------


* (c2a781e3) newsgroup is working...

* (85c92c9c) Create Progress in Development - Considering the way forward - Qiqqa library storage, database, DropBox (and *frenemies*), backups and backwards compatibility.md

* (bb2ac047) Create Progress in Development - Considering the way forward - Essential yet hard(er) to port UI features.md

* (bc4d2a12) Update Progress in Development - Considering the way forward - Full-Text Search Engines.md




2020-06-23
----------


* (f640e9a5) Update Progress in Development - Considering the way forward - Full-Text Search Engines.md

* (1fad253a) Create Progress in Development - Considering the way forward - Full-Text Search Engines.md




2020-04-27
----------


* (0fa9d3f0) Merge branch 'documentation'

* (5107c784) delete useless Windows Explorer auto-generated files

* (dbcfe70e) Merge branch 'documentation' into mainline-master

* (f680e9e8) make sure we don't get bothered to commit the old mupdf code we're inspecting, which came as an archive with Qiqqa.

* (1053edc6) delete useless Windows Explorer auto-generated files

* (413768db) Merge remote-tracking branch 'remotes/mahfiaz/master' into documentation

  # Conflicts:
  #	Qiqqa/Common/Configuration/ConfigurationControl.xaml
  #	icons/Icons.cs
  #	icons/icons.csproj

* (173fb104) Configuration page text fixes.

* (c8bd4081) Changed configuration page layout. No nested collapsible frames.

* (f11a7a93) Grammar in DEVELOPER-INFO

* (0af92169) Shorter README.

* (00c0a878) Update README and DEVELOPER-INFO.

* (f01f3230) Remove chat features, communication should be on homepage.

* (4a1ea0f0) Fix an unimportant build warning.

* (5d350fcd) Remove splashscreen completely.

* (09c5b1c9) Remove 5 more useless icons.

* (79e68805) Remove references to not existing icons.

* (f046e038) Deleted lots of unused icons, removed unused refereneces.

* (29be9c78) Reduce number of build warnings by removing unused exception variable.

* (49166d2f) Fix strange empty space in PDF view when PDF is available.

* (78c4da01) Reverted removal of icon needed for alternative.to nagger.

* (8d30d885) Revert "Remove nagging to vote on alternative.to"

  This reverts commit ad26f4f5fdef4649c0e337fd7c29ff436845caf0.

* (3e63cf12) Fixed unintentional Alt+W shortcut created in earlier commit.

* (5e4cef6d) Configuration page text fixes.

* (4eaa03f5) Remove temporary background color change.

* (010c04cb) Changed configuration page layout. No nested collapsible frames.




2020-04-26
----------


* (38f15360) Grammar in DEVELOPER-INFO

* (8fa77f2f) Shorter README.

* (89db66dc) Update README and DEVELOPER-INFO.

* (140f154d) Move citation button one place to left.

* (1280561c) Remove chat features, communication should be on homepage.

* (006f3511) Change order of left sidebar elements in PDF viewer, group by topics.

* (b96fda38) Fix an unimportant build warning.

* (6051f845) Remove splashscreen completely.

* (ad26f4f5) Remove nagging to vote on alternative.to

* (2cb54aca) Remove 5 more useless icons.

* (b98163b4) Remove references to not existing icons.

* (f21f5383) Deleted lots of unused icons, removed unused refereneces.

* (ada53fac) Add Ctrl+W change to another similarly used place.

* (5b5b13fa) Reduce number of build warnings by removing unused exception variable.




2020-04-25
----------


* (9334b883) Fix strange empty space in PDF view when PDF is available.

* (1a6a91eb) Remove reference to background, causing compile warning.

* (26e69147) Remove Tweeting button.

* (2a295353) Merge remote-tracking branch 'upstream/master'




2020-04-21
----------


* (0b4b3118) Merge remote-tracking branch 'remotes/GerHobbelt/master' into documentation

* (dc6fe6ab) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into documentation

  # Conflicts:
  #	docs/index.html

* (89771517) write up the PDF + C# work done.




2020-04-19
----------


* (664bf1cb) added the additional skeletons for Technology Tests for async work on the BibTeX editor, etc.




2020-04-18
----------


* (45146060) LDA = Latent Dirichlet Allocation :: I had wondered about it in the past when I hit that code during performance measurements. Now added a small comment linking to the relevant Wikipedia articles. Not sure if Jimmy coded this with the Collapsed Gibbs Sampling as per the paper by Thomas L. Grifﬁths and Mark Steyvers: I'm not well versed in this stuff and haven't taken the time to correlate the current implementation with that (and other) papers.

  Also added the BibTeX and other metadata formats from Scholar and PMC for this paper as part of the test set.

* (5a632102) added the additional skeletons for Technology Tests for async work and Lucene-based data+metadata search

* (eb038b01) added the intended Technology Tests' projects for testing of various new bits of technology to be integrated into Qiqqa as we upgrade the functional elements to modern standards (embedded browser, etc.): https://github.com/jimmejardine/qiqqa-open-source/issues/2 https://github.com/jimmejardine/qiqqa-open-source/issues/7 https://github.com/jimmejardine/qiqqa-open-source/issues/34 https://github.com/jimmejardine/qiqqa-open-source/issues/35

* (4bf6ea79) added Aside - The World of Data Extraction and Re-use - PDF Reading, Annotating and Content Extraction.md article rough copy. Regenerated website.




2020-04-17
----------


* (aafe4f18) moved sample projects to the 'Technology Tests' subdirectory: less clutter in the main dev directory.




2020-04-13
----------


* (055b4a15) Create Communications -- Where Goes What.md

* (c0323fa2) Create Contributing -- What Can You Do To Help.md

* (30ec1445) Update test.html

* (eabfef01) Update index.html

* (30eb4127) Create Communications--Where.Goes.What.html

* (e1fe054c) Rename test to test.html

* (30faa92f) Create test

* (a63b2c75) Update README.md




2020-04-09
----------


* (9f60fadc) Actually got eleventy (11ty) working now: that saves me doing a NIH hacky SSG build. Phew.

  Still troubles though, but those I would have had with my own solution as well:

  - MarkDown rendered output is not 'rewritten' for the permalink URLs generated for each file. (I want to keep 'sane ~ title' references in there, while the tool rewrites all links in there based on some sort of replacement scheme...)
  - CSS et al still has not been set up: I'm keeping that bit of the vuepress output around until the day I get that part up & running as well.




2020-04-08
----------


* (ebf0a165) switched to eleventy for generating the documentation / website. As I wrote before, that was option Number Two. Alas, 11ty and me don't get agreeable on the template chain (it doesn't want to apply the wrapper(s), no matter what I tried today), so this is here only for archiving what was done so far, as I'm pretty much fed up with these SSGs and roll my own - didn't want this to be a case of NIH but spending more on 11ty to find out *exactly* *why* the bloody bugger doesn't want to spit out complete HTML pages is now a no-no. Anyhow, `markdown-it` is doing fine, including the plugins, so at least won't be an effort "from scratch" (ugh)

  This is the last commit which employs `eleventy` for rendering the site.

* (587e4823) ditching `vuepress`. After hacking on it for a while, it's quite dissatisfactory for what I want output (both for Qiqqa docs and other projects): no control over the client-side JavaScript part of the generated website. And that part gets totally crazy huge even for medium size doc sites. Second choice when I started looking at this last month was eleventy. Switching to that one, as it gives me the control I want. https://www.zachleat.com/web/introducing-eleventy/#eleventy-is-not-a-javascript-framework

  This commit will represent the last vuepress changes I made and config updates there.




2020-04-03
----------


* (3a642b56) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into mainline-master

* (269d6511) Update README.md

* (e25fd778) `npm run docs:build`

* (24f91681) documentation: fix the docs filename fixing script. Added documentation for the developer override settings.

* (0bd1f57b) `npm run docs:build`

* (5003fe31) Merge branch 'mainline-master' into documentation

* (c095e8b5) `npm run docs:build` : regenerated documentation

* (dc941720) we're having a spot of trouble with CRLF vs LF-only in our shell scripts under Visual Studio :-(




2020-04-02
----------


* (614a72c0) documentation: worked on the vuepress rig:

  - Added a Visual Studio build project + `npm run` commands to build and dev/test the generated documentation website
  - added `npm run docs:server` next to `npm run docs:dev` to test-fly the *build* website as the build script will post-process the vuepress output, replacing absolute URLs to relative ones. This uses the latest `live-server`.
  - updated the npm packages
  - experimented with several TOC generator plugins as the first one we picked was causing vuepress build crashes. It's still not what we'd like, but at least the current state of affairs produces a site. (global-toc is b0rked, etc.)
  - augmented the vuepress markdown-it install to have access to many advanced MarkDown features
  - first round of configuring the vuepress theme; again, this is now working but not the final target
  - regenerated the website from the MarkDown files

* (13ae861d) organized the multiple projects in the Visual Studio Solution into a bunch of category folders.

* (192ce7aa) regenerated documentation website (gh-pages)

* (a0482d99) adjusted vuepress config to include generating the .nojekyll and CNAME files for gh-pages: removed those from docs-src/




2020-04-01
----------


* (1c3f1e6d) `npm run docs:build` - initial build run using vuepress.

* (b9ee5184) introducing `vuepress` as the documentation gh-pages generating tool

* (56be2d9e) Merge branch 'v82-build' into documentation

* (1a4bb935) comment typos and whitespace police

* (06b9d8b6) fix test rig: make sure the configuration is loaded before we run PDF tests as those require those settings to having been initialized to work properly at all.

* (c51f31bd) Slightly improve the SORAX library error logging for when the error was due to a missing/inaccessible file: prevent an obscure SORAX log message.

* (323f25ca) get rid of the last lingering commercial-specific Qiqqa bits: hacker warnings.

* (a061a94d) Logging output fix: only output QiqqaOCR output at ERROR level when there actually has been an error reported, either as return value or in the logging itself. Otherwise just log any successful output log lines at Info level: this should prevent cluttering the Errors log files with successful PDF processing actions and thus make it easier and swifter to go through the error set in the logs.

* (aa4af8ec) Google Scholar Scraper: (1) augmented the code to recognize particular GS output and (2) only execute the background scraper when the feature is enabled in the dev-settings (ON by default)

* (1e9ba20d) Refactor attempt to lazy-initialize the Qiqqa directory settings so as to allow commandline overrides and side-by-side execution of Qiqqa Tests/tools.

* (892f2e8c) fiddling with the logging settings: make sure all Qiqqa tools output their log files into the Quantisle AppData log directory for easier access and retrieval.

* (6dd66aa9) moving documentation sources from /docs/ to /docs-src/ as prep for using our own doc site rendering solution for gh-pages.




2020-03-30
----------


* (f081962f) Add Ctrl+W shortcut - close tab

  Adding Ctrl+W shortcut in addition to Ctrl+F4, to close currently active tab.




2020-03-28
----------


* (6cc8c39e) Create Software Releases - Where To Get  Them.md




2020-03-25
----------


* (201cc9ff) ignore generated installers and some user-specific config files: less clutter in the TortoiseGIT commit views.

* (83a76d65) Crude patch to allow commandline parameter to change the Qiqqa "base directory": useful for independent library sets and testing rigs. Also a beginning for a 'portable / non-admin' Qiqqa install (https://github.com/jimmejardine/qiqqa-open-source/issues/124)

* (7f064bab) disabled unused chunk of code: that interface to AugmentedBindable is not used any more.

* (6676f3aa) updated CHANGELOG_full.md




2020-03-24
----------


* (f63d07c4) `npm run fix-docs`

* (1c161e67) Merge remote-tracking branch 'remotes/jimmejardine-original/v82-build'

* (64fd2991) Merge remote-tracking branch 'remotes/jimmejardine-original/master'

* (4abab9d0) v82.0.7357.40407

* (149ecf4f) `npm run syncver`

* (0ac90ad7) typo fixes etc.

* (ed3ee070) assist debugging, no change in logic: help the debugger to observe at breakpoint after the fact which reason(s) were given for triggering PDF work.

* (df9d867e) fixed a few minor issues in the metadata inference logic (used for extracting titles, etc. from the pdf text content)

* (3a154281) don't even *try* to log an exception when we've already shut down the logger and one or two of the `Dispose()` calls happens to cause a failure inside `SafeExec()`: by that time, there's nothing we can do and we're going down anyway, **as intended**.

* (0a7e86ca) fix https://github.com/jimmejardine/qiqqa-open-source/issues/180: FolderWatcher is not working: no PDF files are found, not ever.

  the wrong AlphaFS API was used in the code.

* (bb2a33dd) Update README.md

* (1a653ecb) Create Qiqqa Internals - Processing PDF documents' text and the impact on UI+UX.md




2020-03-23
----------


* (42b82d1d)
  - bumped the Qiqqa version to today via `npm run syncver`, before creating a new beta release installer for testing...
  - updated Copyright year  in the syncver script `Qiqqa.Build/bump_version.js`

* (8d7c5f72) whoops! fix coding error in setup installer script!

* (2887a366) Merge branch 'v82-build' of github.com:GerHobbelt/qiqqa-open-source into v82-build

* (dab1ce8d) comment typo fixes

* (118cd6db)
  - cleaned: Remove programmer warning in the Logging implementation as it no longer applies: TriggerInit() + Init() coded this way do deliver anyhow.
  - fixed: It turns out the Logging singleton is still invoked AFTER it has been flushed and closed as part of application shutdown: some other Dispose() calls get executed after that fact and they SHOULD NOT cause a failure inside the Logging code.
  - improved the SafeExec() code a little and checked the exceptions which were occurring: turns out those happened AFTER the Logging instance has shut down already, hence we're fine with it, even though these are due to UI elements being released quite late in the game: maybe some lingering cyclic references which should be cleaned? ::thinking/wondering...::

* (ecbbead3) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (21f09c14) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (49674ce6) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (a851b63f) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (509ec6e4) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (eaa162a0) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (9d87dcbc) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (614be3c5) docs: decorative divider for doc. Just having fun.

* (5ad0d9c5) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (26b404b6) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (88291c05) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (ac2b231f) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (1b7c2b1b) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (34f8edf7) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (9672c0a1) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (d0a9c11b) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (7ae67bc6) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

  - http://xahlee.info/comp/unicode_arrows.html
  - https://stackoverflow.com/questions/25579868/how-to-add-footnotes-to-github-flavoured-markdown
  - https://en.wikipedia.org/wiki/Note_(typography)

* (61f6c4ab) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (9bfb8d7e) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

  Thanks to https://gist.github.com/pierrejoubert73/902cc94d79424356a8d20be2b382e1ab, we now have folding section(s).

* (5213be62) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (6531718e) Update Qiqqa.Internals.-.Extracting.the.text.from.PDF.documents.md

* (bc80c1c0)
  - make sure all projects and environments use the same FxCop ruleset
  - update NuGet packages

* (5006a2c6) added `npm` task to rename docs/ files which contain spaces in their name: this way, those files will map to URLs more easily when we work  on the documentation website part of Qiqqa.

* (3259db13) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into v82-build

* (45971f58) fix Goggle Scholar background scraper logic to cope with modern Google Scholar website responses; this improves the Citation References data/list shown with each viewed PDF document in Qiqqa. (This is related to commit 9d6a79dea4dd6361d4642b8248995bc98bdb4217.)

* (d60d1173) tweak: some Dispose chunks sometimes barf inside SafeExec() and I like to know where those calls originated: therefor a stacktrace is collected in the closure which invokes the UI thread execution of said chunk of code, as the debugger has lost that info by the time the Exception occurs. (It's not a big issue ATM, but I keep seeing these exceptions while debugging Qiqqa and I'd rather not have these instabilities when I can get rid of them; currently diagnosis is hampered as this trace info was lacking.)

* (ff1cd5c1) disable unused code: PDFCoherentTextExtractor apparently was an old Quantisle experiment...

* (8e88ca3f) comment typo fixes

* (9d6a79de) tweak/fix(?): Qiqqa was hiding the left side panel in the PDF Document view, which is receiving the background Google Scholar SCRAPE process output. (As I was wandering where that info went, I found out the left panel is filled at all time, but remaining hidden most of the time. This is a TEST fix to see what's going on  precisely and to evaluate whether having that info available is useful to the user...)

* (c4141a17) assertion tweak: only check if the long-running code is running in a non-UI thread when Qiqqa is NOT in the middle of application shutdown/termination.

  Reason: when Qiqqa is shutting down, some work MUST still be done while the UI is shutting down/has shut down already:  at that time in the life of the Qiqqa run-time, we DO NOT care about UI responsiveness any longer but only about whether the final required actions are done ASAP.

  This fixes some spurious UNWANTED & UNINTENTED assertion failures in the code.

* (b27856a9) comment typo fixes

* (95aea20a) emphasize user account name in the UI by indenting it visually




2020-03-22
----------


* (ca3a4063) writing up the current Qiqqa OCR / document data handling process (also to help me reply in issue https://github.com/jimmejardine/qiqqa-open-source/issues/159 )




2020-03-17
----------


* (19c938a3) Merge remote-tracking branch 'remotes/wkedziora/doiSniffer' into v82-build

* (86ed86f2) PDF metadata extraction background task: A timeout should only kick in when we have *some* work done already or we would have introduced a subtle bug for very large libraries: if the timeout is short enough for the library scan to take that long on a slow machine, the timeout would, by itself, cause no work to be done, *ever*. Hence we require a minimum amount of work done before the timeout condition is allowed to fire.

* (c080f562) PDF OCR background task: Do not flood the status update system when we zip through the work queue very fast: only update the counts every second or so, but be sure to be the first to update the counts after work has been (temporarily) stopped.

* (29932376) bundle PDF processing log lines into a single log line as it floods the logfile for large(-ish) PDF documents. Some PDFs could cause up to 500 of these long(!) log lines to be produced, cluttering the log file no end.

* (5bc962cc) code inspection following up on https://github.com/jimmejardine/qiqqa-open-source/issues/156 : bit of code robustness added for when blacklist/whitelist files have been manually edited.

* (1736b291) https://github.com/jimmejardine/qiqqa-open-source/issues/158 but do note the comments in that issue: this fix is not going to work according to https://github.com/domgho/innodependencyinstaller/issues/12 + https://github.com/domgho/innodependencyinstaller/commit/ce85626e61cc9e1dacb9578e8e04ab9d435d9edc#r16322373-permalink

* (e67ce509) comment typo fixes and a compiler warning removal

* (a4b75990) Dev/Debug: fiddling with the CPU core/thread count setting...

* (7906a79d) QiqqaOCR: word confidence is encoded as a perunage ("per one" instead of %: "per cent"); make it so. (Some bits of the code treated it as a percentage, some as a perunage.)

* (4899077e) looking at https://github.com/jimmejardine/qiqqa-open-source/issues/133

* (139b3320) further unify background daemons by introducing the delay_before_repeat_milliseconds parameter. Ensure every daemon CANNOT occupy an entire CPU core all the time by enforcing a minimum delay of 500ms between cycles.

* (a0a453ad) Config UI: "User guid" --> "User GUID:"




2020-03-07
----------


* (7584a9e4) typo cleanups in the Inno Setup scripts & added a sample unzip script to the Inno Setup products install scripts collective & added IsAppRunningU() API for slightly improved install script readability/consistency. (IsAppRunning == IsModuleLoaded, both for install and uninstall tasks)




2020-02-27
----------


* (865a17a0) small update

* (916567d1) small changes

* (64e02652) doi2bib added

  Added new search engine - doi2bib




2020-02-19
----------


* (d930d352) fiddling with stacktraces being very incomplete (single line only, hence useless) in run-time assertions.

* (947a897c) Merge remote-tracking branch 'remotes/jimmejardine-original/master'

* (fda8a20f) API usage readability improvement in the code

* (a21f97c9) improved the logging/reporting of installed and active .NET/CLR versions to help analyze bug reports.

* (a17b2134) updated the legacyformats sources: synced with originals

* (5751c078)
  - "Force OCR" should really nuke the previous OCR results on disk so we get a complete refresh and not a half-hearted one.
  - don't throw exceptions (which are handled internally) when delete-ing files which MAY legally not exist.

* (e71d97f4) refactored the LDAsampler + expedition source code around the words set and the GetDescriptionForTopic() API; it's still a bit of a mess, but less so than before as the words set necessary for GetDescriptionForTopic() isn't passed around a zillion member functions any more.    TODO: work on the expedition refresh to reduce the number of topics to the actual non-duplicates, i.e. the ones that are unique to the human user, as visible when produced by GetDescriptionForTopic() without a sequence number.

* (eecffdff) fix: rethrowing exceptions should keep the stacktrace intact. See also the note in this section: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/throw?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev16.query%3FappId%3DDev16IDEF1%26l%3DEN-US%26k%3Dk(throw_CSharpKeyword)%3Bk(TargetFrameworkMoniker-.NETFramework%2CVersion%3Dv4.8)%3Bk(DevLang-csharp)%26rd%3Dtrue%26f%3D255%26MSPPError%3D-2147217396#re-throwing-an-exception




2020-02-16
----------


* (26ff47aa) First bit of work to make the duplicate themes in Expedition go away: it turns out that `GetDescriptionForTopic()` is at the *visual* reason for this; we need to investigate further to find the root cause.

* (80bada14) fix cyclic ref bug in latest code edit for singleton

* (c01f5270) fix long-lived bug in the Author splitting code which would render authors like "Krista Graham" as empty slots in the Author side panel of the library view.

* (412ccf43) tweak BibTeX Control to start editing in RAW mode instead of formatted mode.

* (db6cfef4) new idiom: use `WPFDoEvents.SafeExec()` to wrap the bits of a class which we are `Dispose()`-ing. This replaces the larger-chunked try/catch blocks and UI thread invocations in there.

* (fea42040)
  - fixed the GoogleScholarScraper logic to suit the latest Google Scholar search page HTML output.
  - added unit tests for the GoogleScholarScraper logic. The "live test" fails due to Utilities.Configuration.Proxy not being set up properly yet.

* (bf98e54c) fix bug in manager code: inverted logic in conditional

* (0a318405) fix crash/exception in Google scraper function




2020-02-14
----------


* (9066471e) fix logging message text

* (2daf3ee9) Add Developer Feature: Qiqqa will load a developer test settings file (json/json5 format) when available. The settings in this file help a developer or tester to disable certain Qiqqa features and/or limit or 'tweak' specific Qiqqa behaviours to help test the application in both debug and performance measurement scenarios.

* (6c859c4a) removed duplicate chunk of code

* (7aba6ca5) Merge pull request #154 from wkedziora/polish-translation-wkedziora

  Update pl.qiqqa.txt




2020-02-09
----------


* (b8d4ee63) added a couple more ASSERT checks for tasks which *should* execute in background threads. This currently *BREAKS* the code (as in: some of these assertions will fail!) as several tasks are executed in the UI foreground thread during Qiqqa startup/load phase, while they SHOULD NOT.
  This is a bit of work that's part of research to improve Qiqqa UI performance.




2020-02-07
----------


* (f829fb5b) Update pl.qiqqa.txt

  Updated Polish translation using English one as an example.




2020-02-04
----------


* (69a941c9) auto-reformatting of a few XAML files. No functional change.

* (f6c2892f) added QiqqaHasher CLI tool to help us calculate Qiqqa compliant fingerprints for every possible document in any (shell) scripts.

* (0df9239b) IMPORTANT: disabled all lock monitoring code by commenting out the statements: these would consume about 16% of total CPU cost in the UI thread when we ran the latest benchmarks and we don't have any locking issues ATM so it's about time to get rid of these. Revert this commit to get them back.

* (96bbf16d) LINQ code triggered an exception because we've been a tad overzealous replacing Datetime with Stopwatch instances: the latter is not IComparable and henve not easily sortable by the LINQ statement. Reverting to the original Datetime solution for these status bar messages.

* (ecd9879c) order the status bar items in reverse order: most recent entries should show shifted towards the right edge so as to reduce the amount of jitter/flicker in the statusbar while several short(ish) activities get reported and completed while Qiqqa is running.

* (51dd38ca) Fixed issue where latest MSVS 2019 was yakking about an outdated package file.

* (8b17ea71) tweaked/reworked the MetadataExtractionDaemon to use the new Daemon logic where registered background tasks for now-discarded library instances can be safely removed from the active set, thus reducing background activity and CPU load a little too.

* (9ea554f8) refactor Daemon class and associated background tasks: make it possible for registered tasks to be *removed* properly: this is useful as the load/init phase of Qiqqa MAY otherwise exhibit multiple folderWatcher registrations for the same library, once for each time it is updated during the initial scan-for-suitable-libraries phase of the application.

  Note: The code is simplified by never re-using a slot in the daemon registration list: this does no harm to performance nor memory consumption as a *large* test environment (my own rig) clocks in at only 87 slots used during an entire run, while the test rig comes with 20+ actual libraries and various 'paths' through which the libraries will be discovered during the scan-and-load init/load phase of the application. Hence we expect even extreme and heavy users of Qiqqa will not surpass about 100 slots in the registration array/list. Thus we have designed the code to use the simplest approach for the registration array/list: grow-only, NULLing the discarded slots.

* (f643b08f) fix bug in logging lines after move to use TypedWeakReference instances for the library references.




2020-02-03
----------


* (2d841cdf) updated all NuGet packages across the board

* (3e4cc439) migrate all projects to .NET 4.8 from 4.7.2 - this completes the work started in commit SHA-1: a449560fa479c75e8f0877d85cca01bcd73e28e2 : update projects to reference .NET 4.8 instead of 4.7.2 and C# language version 8.0. This does not impact OS platform support (see https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies): Qiqqa *should* run on Windows 7 and upwards, when the .NET update has been installed on Windows 8.1 and 7 (the OS update is automatic on Windows 10).

* (33861248)
  - Mimic the Qiqqa project filetree layout in the QiqqaLegacyFileFormats project for easier comparison in the future.
  - Also go through the QiqqaLegacyFileFormats sources and keep the serializers and deserializers from Qiqqa as "sample code" for now, until we have a porting layer set up properly.

  (Part of the work to create a copy of the code & classes to reside in the QiqqaLegacyFileFormats project: that project is a 'TODO' project for supporting old Qiqqa file formats without overly buggering the Qiqqa code itself.)

* (dc6eca92) removing unused & empty sourcefiles from the projects.

* (387452a1) Make sure to flag PDFDocument attributes which should never be serialized as [NonSerialized]. (Part of the work to create a copy of the code & classes to reside in the QiqqaLegacyFileFormats project: that project is a 'TODO' project for supporting old Qiqqa file formats without overly buggering the Qiqqa code itself.)

* (b642898b) fix incorrect (copy-pasta'd) obsolescense message in the code

* (8acbca69) fix for C# language dropping back to 7.3 (see https://www.c-sharpcorner.com/article/which-version-of-c-sharp-am-i-using-in-visual-studio-2019/) and the warning "Utilities/Misc/StatusManager.cs(38,29,38,30): warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context." (see https://stackoverflow.com/questions/55492214/the-annotation-for-nullable-reference-types-should-only-be-used-in-code-within-a), following my update of MSVS 2019 to v16.4.4

* (34011f7f) Merge branch 'v82-build'

* (78eef3e6) refactorings:
  - replace all code which uses DateTime-based timers with either Stopwatch-based ones or simple counters (IFF that's what's really meant/needed in the code, i.e. when DateTime is used to "make sure" various instances have unique serial numbers)
  - use TypedWeakReference<T> references in all classes which otherwise would create a (library instance) reference cycle, e.g. all 'Manager' classes which themselves are referenced by the Library, while they each carry a reference *tot* that Library instance.
  - FolderWatcher has been recoded to use the Directory.EnumerateFiles() API in an effort to rid us of the lousy UI status update reports which would show the old internal (hacky) solution to reduce CPU loading by only processing about 5 documents per shot: now the code is much more fluid and clarify of function has improved, resulting in a clearer progress status message as well.
  - removed various hacky `WPFDoEvents.WaitForUIThreadActivityDone()` lines in the background tasks' codebase; the UI status update code now includes this call to help improve UI thread execution priority and responsiveness, thus concentrating this still hacky approach into a single spot where it would be at least a *bit* sensible to have.
  - separated the library init process into two parts: (1) the loading of the library itself (plus all database-bound metadata) and (2) the checking up on the attached PDF documents and collecting their (file-based) metadata. The latter is now available via `WebLibraryManager.Instance.InitAllLoadedLibraries()` and is only invoked once all libraries have been properly gathered from various sources, thus improving Qiqqa load time in complex environments, where libraries would previous be loaded, then replaced by updated versions, as Qiqqa scans the various places on disk for libraries. Previously this resulted in some libraries loading the PDF document data twice (at least partly, before the initial load is aborted by the update). The gain is in the amount of File I/O at startup and the associated instantiating and subsequent disposing of Librarydocument objects.
    + Consequently, we now have rid ourselves of the hacky CloneSansLibraryReference() API which was previously needed in the Library Dispose() method to prevent fatal failures.
  - rewrote the code to NOT use lambdas in the use of the WeakEventHandler<T> class as the MSVS Code Analysis kept on badgering us with warnings about incorrect use of this WeakReference-style class. Turned out this was due to the implicit `this` reference in the lambdas as far as the static code analysis was concerned so we removed the CA warnings by turning all of them into `private static` member functions.
  - fixed WebLibraryDetail reference update bug as part of the TypedWeakReference rework: now the WebLibraryDetail internal reference is correctly updated as a library gets updated from another source during Qiqqa init/load phase.
  - "stabilized" the status bar by delaying stale items' removal for 3 seconds instead of just 1: less flashing in that bottom bar when multiple processes are at work inside qiqqa.
  - DoEvents(): now using code which should (according to SO discussions referenced in the code comments) use much less CPU, leaving more cycles for useful work.
  - fixed race conditions in the status bar management code as the code was not thread-safe while being invoked from multiple (background) threads as well as the UI thread itself.

* (5d34a0e6) UI fix: placement of OK and Cancel buttons (TODO: do the same for the other dialogs)

* (a449560f) update projects to reference .NET 4.8 instead of 4.7.2 and C# language version 8.0. This does not impact OS platform support (see https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies): Qiqqa *should* run on Windows 7 and upwards, when the .NET update has been installed on Windows 8.1 and 7 (the OS update is automatic on Windows 10).




2020-01-23
----------


* (d25ff654) disable the unused Google Analytics tracker code (used for Qiqqa Features(?))

* (015c06c7) refactor: use `nameof` whenever possible with `Bindable.NotifyPropertyChanged()`: https://stackoverflow.com/questions/38324208/expression-vs-nameof

* (f7a57dcc) unified StopWatch idiom in code.

* (51b8b8f4) added empty WPF+Core.NET projects for experimentation and migration

* (21afa310) comment typo fix

* (814db2ac) moved thread unsafe version of the PDFdocument class to its own source file after long lest.

* (8debb288) updated NuGet packages




2020-01-22
----------


* (82a31fb1) Merge branch 'v82-build'

* (d94ae3f9) quite a bit of work on the OCR background process:
  - refactor Qiqqa to *always* check if the temp scratch directory exists (and create it when absent): this prevents spurious errors when an external (overzealous) cleanup process cleans out the TEMP dir space, while a long-running Qiqqa session is still working on the OCR/indexing tasks
  - adjust the internal OCR job scheduling in such a way that we detect "sure repeat failures" early, thus preventing the UI status updates about page(s) to be textified to run up and down like crazy for huge libraries which carry multiple hard-to-OCR PDFs - this makes the UI status updates more stable and sensible to the user and prevents a lot of (re-)testing and (re-)execution of code paths which are sure to fail in QiqqaOCR anyway.
  - refactor the internal APIs: TEXT_PAGES_PER_GROUP is a constant which is not transported across the relevant classes instead of taking up a (useless) call parameter as this value MUST be constant and fixed across the application
  - fix a typo in a log statement

* (13afcaea) disable and then *restore* a Roslyn Analyzer warning

* (1ddde46f) indexing PDFs / OCR: accept the underscore '_' as a legal word character (it is also known as "the programmer's space")

* (dce5cb70) debug/tweak: disable the Starting Qiqqa For The first Time hardcoded test settings, which were used to test Qiqqa behaviour as if the application had been invoked for the first time after a (fresh) install.




2020-01-19
----------


* (d19f0053) reformatted XAML

* (437bcd96) fix faulty argument count check in QiqqaOCR for SINGLE-FAKE task

* (f89f05e3) fix for: log4net:ERROR XmlHierarchyConfigurator: Cannot find Property [ImmediateFlush] to set object on [log4net.Appender.ColoredConsoleAppender]

* (cd8b198d) OCR task: some of the error logging wouldn't make it, on occasion. Either stdout+stderr isn't flushed properly at the sender (QiqqaOCR) and/or not waited for and picked up at the receiving end (Qiqqa), but we lost important crash info too often. Now this issue is resolved by forced flushing of Log4Net (via the LogManager) *and* waiting an additional half-second to ensure the invoking process (Qiqqa) does properly fire all related events so we can gather the output of QiqqaOCR. Seemed similar to https://stackoverflow.com/questions/14963735/log4net-rollingfileappender-not-flushing-io-buffer-with-low-volume-log but in the end it turned out that only the new added Logging.ShutDown() API is resolving the issue (**with high probability**: I am still not entire sure this is it, as this problem is hard to reproduce consistently. :-(( )




2020-01-02
----------


* (cc4c05ae) Merge branch 'v82-build' into mainline-master

* (b657d04b)
  - StripToASCII() now attempts to convert as many characters as possible to compatible US-ASCII characters.
  - Added the unit tests for this and GetFirstWord() utility APIs
  - Fixed JSON serialization bug in the test rig: https://stackoverflow.com/questions/29003215/newtonsoft-json-serialization-returns-empty-json-object




2020-01-01
----------


* (c7ca5752) picking up minor edits and text fixes from experimental dev branch

* (bc20c556) Merge branch 'v82-build'

* (2099a1d9)
  - code unification: migrated code from Thread to Daemon class usage where possible.
  - also moved another bit of work from UI to background thread.

* (673229ba) fix crash in Dispose cleanup logic and tweaked the UI progress feedback to be slightly more informative: with (very) large libraries, getting a green bar for progress is not enough to observe Qiqqa at work.

* (8f746cca) added a few more in-which-thread-are-we-running assertion checks.

* (a2d50082) Expedition: the LDA process to arrive at a set of more or less decent 'themes' (topic groups) for a library is cute, but takes a horrendous amount of time for large libraries. This commit cuts down ten-fold on the number of Monte Carlo runs and tweaks the status reporting in the UI to arrive at a slightly more informative(?) and 'easy' progress feedback while that long-running process is underway in the Expedition pane.

* (c9357750) fix bug in the PagesSetAsString() utility API: finally, long lists of page numbers are properly grouped in PDF/OCR log reports about missing/failed pages in the background PDF/OCR/indexing process.

* (3e022a0b) BibTeXEditorControl: not entirely done yet, but this at least cleans it up a *little bit* regarding green-lighting the control when the BibTeX happens to positively match the PDF Document.

  TODO: cope with BibTeX manual data entry and the many errors that will give until there's a minimal bibtex record entered. Right now the control is still very user-UNfriendly in its continuous desire to jump to the error report pane and get your focus off the edited field(s).

* (0d48af16) PDFInterceptor: reduce the amount of work and time spent in the UI thread when receiving a PDF from a webpage.

  TODO: I'm still not happy about the fact that the entire (potentially very slow) stream-to-file download+save activity is done in the UI thread, but I didn't spend more time checking whether it would survive in a background thread as I'm pretty sure the transition to CefSharp+Chrome will severely impact this and related logic parts of Qiqqa.

* (86dfe3d1) further work on coping with the OCR failures and bugs in Qiqqa and adjoining tools in the PDF/OCR chain (such as zero-width and zero-height words coming out of MuPDF)

* (33a71e94)
  - fix inverted logic bug
  - adjust CleanupOnShutdown() to only report utter success when all threads have been wrapped up, even the aborted ones. This fixes some edge cases in Qiqqa code flow and behaviour at application termination under heavy loads.
  - adjust CleanupOnShutdown() to use a more sensible timeout

* (e6c896a5) code reformatting

* (12ae2dc7) lock check code: report any timeout overrun every 5 seconds, not just once and then silence... We need to see the locks which wait for a very long time and the way to recognize those is seeing them reported multiple times, once for every 5 seconds of 'overtime', so the longest waits become obvious.

* (b607d282) Custom thread assertions and other custom Qiqqa assertions are important but not meant to be fatal in either debug or release builds: do not throw an error, but DO report them in the log. These are intended to help debugging Qiqqa and should not irrevocably impact the code behaviour; when there's a crash coming due to one or more of these assertions failing, then let it happen.

* (f47c98e9) much better fix for the GetOutputsDumpString() race condition which was hackily fixed in commit 2a51f204a69b039dd95b5df08b4d8422ef631003.

  DO NOTE that we have not yet removed the hacky 'retry a few times' work-around applied in that commit!

* (5b796ec7) Perunage.Calc() clips to a max of 1.0 (100%); make sure we catch any very-close-to-the-max-or-beyond value.

* (f10a6f52) code formatting

* (3b5fc4d8)
  - make sure library work (saving to file in this case) is done in a worker thread
  - added are-we-in-UI-or-other-thread-as-expected assertions
  - local custom assertions should only be active in DEBUG builds to prevent small yet fatal surprises in production while we try to iron out the bugs in Qiqqa during its lifecycle.

* (758b6e2c) work done on the DBExplorer: better handling of records produced by .library and .s3db databases as those records are an amalgam of metadata, annotations, inks, etc. record types -- it's not really used as a database but merely as a hash-indexed generic table store.

* (fd886c05) make sure actual work is done in a background thread while the display data is fetched and updated in the UI thread.

  TODO: Qiqqa sure needs an MVVM/MV* overhaul which takes this distribution of work serious in the software architecture as this is becoming a mess.

* (916c12d9) minor fix: report document numbers (N of M) starting at 1(one), rather than 0(zero) as it looks odd to humans otherwise

* (1f3495cc) deduplicate = simplify the logic inside IncrementalBuildNextDocuments() regarding GetOCRText()-driven activation of the background OCR process

* (1921ba30) reference all XAML files in the solution: one place to see them all




2019-12-31
----------


* (b0191cda) QiqqaOCR: fake words for empty pages or any pages that appear so to the current OCR engine! (https://github.com/jimmejardine/qiqqa-open-source/issues/73 + https://github.com/jimmejardine/qiqqa-open-source/issues/129 + https://github.com/jimmejardine/qiqqa-open-source/issues/135 )




2019-12-30
----------


* (3e349e5a) tweaked the AnnotationReportOptionsWindow.xaml design file.

* (066483a0) executed the XML beautifier

* (1f1d5af5) adjusted the npm package used for XML beautification

* (19fd245a) adding a XAML reformatter/beautifier tool to help ensure that we can have all XAML files formatted and cleaned consistently.




2019-12-27
----------


* (39f6147d)
  - added SINGLE-FAKE mode for QiqqaOCR: this is invoked when GROUP and SINGLE don't deliver due to, for example, encrypted PDF source. This is a temporary hack to ensure Qiqqa doesn't repeat OCR activities ad nauseam (https://github.com/jimmejardine/qiqqa-open-source/issues/129 , https://github.com/jimmejardine/qiqqa-open-source/issues/135 , https://github.com/jimmejardine/qiqqa-open-source/issues/73 , etc.)
  - the previously added extra OCR text files' sanity checks (zero-sized areas of words, etc.) seems to pay off. At least we've observed quite a few OCR files/pages being retriggered for OCR as Qiqqa uncovers these zero-sized word areas while refreshing for Expeditions
  - added a few more UI-thread-or-not Assertions.

* (2a51f204) GetOutputsDumpString() :: oddly enough this code can produce a race condition exception for some Output: "Collection was modified; enumeration operation may not execute." -- HACK: we cope with that by re-iterating over the list until success is ours...   :-S :-S  hacky!

* (651bac5b) fix coding bug in SafeThreadPool handling: the logc to determine which tasks to skip/abort on application Exit was inverted. :-(




2019-12-26
----------


* (483ddd7f) QiqqaOCR: added some sanity checks to the output files'decoding in Qiqqa and made sure any error in the tool itself is visible in Qiqqa - any fixup for that should be done in Qiqqa, not QiqqaOCR itself.

* (10f985c4) adding QPDF tool (v9.1.0) to the production set (Qiqqa) - this will be used for decrypting/repairing PDFs which fail to OCR/text-extract otherwise, such as On-Semi datasheets.

* (e0171c9e)
  - further unified the Dispatcher.Invoke() code to use WPFDoEvents.InvokeInUIThread(...)
  - slightly simplified the StatusBar logic

* (739bc093)
  - tweaked the startup process to ensure better UI responsiveness:
    + moved many parts to background tasks,
    + heavy use of WPFDoEvents.InvokeInUIThread() to ensure specific bits of code execute in the UI thread as they MUST
    + added several assert checks in the code to ensure the code pieces are executed in the UI or background threads as assumed
  - some code cleanup, killing several overloaded member function definitions
  - standardized `Application.Current.Dispatcher.BeginInvoke(....` code to use `WPFDoEvents.InvokeAsyncInUIThread(...`
  - uncovered a race condition in the code along the way: it's relatively harmless, as the crash will be caught and is itself not fatal to the behaviour of Qiqqa as this happens when libraries are loaded (and thrown away in the multi-stage load process as newer instances come in).  Check out the totally HACKY fail-check for `LibraryIsKilled` in `LoadDocumentFromMetadata()` in Library.cs -- that's where the crash would occur otherwise due to `pdf_documents == null` in the statement after that check, due to another thread throwing away the library via `Dispose()`.   **HACK HACK HACK TODO**
  - ran into another race condition as WebLibraryManager.Instance wasn't lock-protected and now that we init that bugger in a background thread, there *will* be multiple threads feeling around that one.
    + also note that a few parts of the other code have been wrapped in WPFDoEvents.Invoke calls and misc code to ensure that the UI thread does not lock up anyway, while the **long running** `WebLibraryManager.Init()` call is completing in a background thread.
  - UI/UX: Splash Screen remains visible until the Login Dialog has become visible. Ditto for the Login Dialog until the main window has been rendered.




2019-12-25
----------


* (0c1caf1e) fix crash on program termination

* (d6ce0573) typo fix in log text

* (bfe43f1c) attempt to make the Expedition AutoTag+Themes regeneration a tad faster. Introduced the `Library.IsBusyRegeneratingTags` flag to signal other tasks (OCR+indexing) that another heavy task is running at the moment and would like to have all cores for now.

* (2361f5b8) The TermsAndConditions config boolean serves two purposes:

  1. it makes the app abort when not set (which ' functionality has been removed in this commit)
  2. it acts as 'first time you started Qiqqa' flag in showing initial help screens, etc. -- This behaviour has been kept as it deemed useful.

* (4ee43c64) speed up compiling/building the app: remove the FxCop project dependencies: we've seen enough of those for a while...

* (17d84225) fix crash during application termination

* (48e05b3b) defer configuration save activity to a background thread and then only do it when either the application is terminating or when the config hasn't been saved in the last minute or so (preventing disk hammering when changing many config settings)

* (06ac00ae) fix crash during application termination

* (60392ba9) fix crash: 91224235436:System.NullReferenceException: stacktrace:

  - Object reference not set to an instance of an object
    - at Qiqqa.Documents.BibTeXEditor.BibTeXEditorControl.RegisterOverlayButtons(FrameworkElement BibTeXParseErrorButton, FrameworkElement BibTeXModeToggleButton, FrameworkElement BibTeXUndoEditButton, Double IconHeight) in \Qiqqa\Documents\BibTeXEditor\BibTeXEditorControl.xaml.cs:line 172
    - at Qiqqa.Documents.PDF.PDFControls.MetadataControls.MetadataBibTeXEditorControl.OnClosed(EventArgs e) in \Qiqqa\Documents\PDF\PDFControls\MetadataControls\MetadataBibTeXEditorControl.xaml.cs:line 122
    - at System.Windows.Window.WmDestroy()

* (05a1dc53) removed deadlock checking calls around augmentbindable internals.

* (a42b577f) regenerated uninstall exe

* (1797c7f4) DEBUGGING/TESTING: Checking whether we're okay with using `Application.Current.Dispatcher` rather than `Dispatcher` throughout the app.

* (c57f7c25) DEBUGGING/TESTING: discovering which is the code spot closest to the point when the main Qiqqa window has *completely* rendered ==> `MainWindow_ContentRendered()` i.e. the ContentRendered event for the outermost container!

* (6db5600b) developer-specific constants and checks should only be available in DEBUG builds.

* (39ab3330) using Win32 OpenFileDialog instead of the Windows.Forms flavor.

* (8b8d6e8d) updated the NuGet packages




2019-12-23
----------


* (a7c4499d) Update README.md

* (1ef34037) Merge branch 'pdf-processing'

* (cfac5154) built new beta/test release

* (a4813a39) `npm run syncver` :: update the build version info for another v82beta release

* (6ae59eaa) Merge branch 'pdf-processing' into mainline-master

* (47dbdda8)
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/142 :: fix crash in DB sync activity due to b0rked UPDATE SQLite query in the code.

  B0rked UPDATE query statement was introduced in previous edit in commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0 ("Another Mother Of All commit with loads of stability & memleak + performance improvements work") as part of the DB I/O refactor done in there.  :'-(

* (58a72666)
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/147 :: Qiqqa crash when opening Autotag Management dialog and hitting SAVE when both white and black list are empty




2019-12-22
----------


* (e165f0e9) further performance work: commented out over 95% of the deadlock checks around `lock()` code: that is 95% of the deadlock check CPU load cost as reported by the profiler. This is still a major CPU cost component of Qiqqa, ranking second just below top contender 'External code' (.NET XAML rendering, libraries used, etc.)

* (f59ee14e) UX: get the Sync info dialog up that much faster (we *still* have to wait a *long* time!) by ALWAYS telling the sync info collector that library size on disk is a non-essential information bit (as already stated in commit SHA-1: 5c66eb64b2353d1d4ff9fbb60ea8a965ae854296 :: performance: speed up the sync metadata info collect action by NOT calculating the precise storage size of each library as that entails a HUGE I/O load as each library document's file system metadata would be queried for its filesize -- which is only used in the UI in an overview table and is deemed non-essential right now.)

  Do note the TODO: we should scan the library documents for their filesize in a background process so as to allow progressively improving accuracy in the numbers reported in the sync management dialog over time.

* (fef5ee9e) clean up debug/profile hack in the code.

* (5c66eb64)
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/145 :: When v82beta created a Qiqqa.library file next to an already existing S3DB file, delete it. We explicitly delete it to the RecycleBin so the user MAY recover the database file on the off-chance that this was the wrong choice.
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/144 :: Do not try to create a Qiqqa.library DB when the directory already has an S3DB database file
  - some work done to find out how https://github.com/jimmejardine/qiqqa-open-source/issues/142 came about.
  - `WebLibraryDetails_WorkingWebLibraries_All`: make sure all 'Guest' libraries are added to the sync set: under very particular circumstances you CAN have multiple guest libraries, e.g. when manually recovering multiple Qiqqa libraries you extracted from your backups of previous qiqqa runs/releases.
  - performance: speed up the sync metadata info collect action by NOT calculating the precise storage size of each library as that entails a HUGE I/O load as each library document's file system metadata would be queried for its filesize -- which is only used in the UI in an overview table and is deemed non-essential right now.

* (14129ca7) performance tweak: take out the lock timeout check code around often-invoked LockObject() statements, which have not shown trouble since a long time: those deadlock detection via timeout calls were added way back to help dig up indicative spots in the Qiqqa sourcecode base while we were hunting down UI/UX lockup bad behaviour.

* (d69c30d4) fix two memleak diagnostic reports in OCRengine code.

* (557aa551) fix https://github.com/jimmejardine/qiqqa-open-source/issues/144 while refactoring the library filename/path construction for easier debugging/monitoring of Qiqqa behaviour.

* (c80b53b3) fix System.IndexOutOfRangeException: Index was outside the bounds of the array at Qiqqa.Documents.PDF.Search.PDFSearcher.SearchPage(PDFDocument pdf_document, Int32 page, String terms, MatchDelegate match)

* (cc5af3cc) added another bibtex lib file to the test set

* (832c28bd) regenerated uninstaller exe

* (d333f88e) tweaking which library types can sync, i.e. which ones are read-only and which ones aren't.




2019-11-26
----------


* (cbb8dba2) Update README.md




2019-11-14
----------


* (2a19b4aa) Create DEVELOPER-GOALS.md




2019-11-10
----------


* (b983d260) updated CHANGELOG_full.md

* (a50888e8) Merge branch 'v82-build'

* (110edf0a) Update DEVELOPER-INFO.md

* (6bad7f68) Update DEVELOPER-INFO.md

* (df8600ec) Update DEVELOPER-INFO.md

* (bfb55d67) Update DEVELOPER-INFO.md

* (819790fc) rebuilt uninstaller exe

* (d4d22165) bumped build revision and corrected the package dependencies/references for the UNINSTALLER: that one produced build errors in Release+SETUP build mode, so the added system references have been removed again. (TODO: clean Qiqqa registry entries on uninstall; keep the data directory, but kill a few other settings when present)

* (db06442a) TODO: proper collision detection and merge action on the metadata when you import Documents which are already present in the library, but might need to have their metadata (partially!) updated. Currently the existing metadata is largely nuked. Check out AddNewDocumentToLibrary() in Library.cs and the TODO comments in Library.cs and PDFDocument.cs when you're working on the MERGE METADATA job.

* (f4791a0d)
  - fixing https://github.com/jimmejardine/qiqqa-open-source/issues/134 : Copy/Move to library functionality was b0rked due to crufty coding of passing the return value (picked library) while I had been a tad overzealous when going through the code hunting down memleak spots, i.e. Dispose and Close handlers' cleanup, resulting in the library picker dialog always producing a NULL library reference.

  - Also tweaked the copy/move process a little while testing the copy & move actions.

  - Migrated the copy/merge activity to a background task to ensure a more responsive UI, particularly when copying/moving large numbers of PDFDocuments at once!

  TODO: proper collision detection and merge action on the metadata when you import Documents which are already present in the library, but might need to have their metadata (partially!) updated. Currently the existing metadata is largely nuked. Check out AddNewDocumentToLibrary() in Library.cs and the TODO comments in Library.cs and PDFDocument.cs when you're working on the MERGE METADATA job.




2019-11-09
----------


* (d7286dee) refactoring the keyboard decoder in PDFRendererControl

* (d2ec492c) fixed couple of crashes in Dispose() calls. Applying a single pattern to all Dispoase methods so that the look of the code and behaviour is largely comparable across classes.

* (4b1b99fb) Merge branch 'v82-build'

* (68d1568a) updated the unit tests' script.

* (d6ece3d0) Tweaked the ApprovalTest helper to create NTFS/Windows-sane filenames and ditto paths, which are limited to below 255 characters (see also https://kb.acronis.com/content/39790).

* (a007960c) picking up the useful edits from the aborted DevStudio Shared Projects refactoring branch - see commit SHA-1: ff3a08ce24f153b5c890d48ddf7d7b51fa4c001f why that line of development was aborted today.




2019-11-07
----------


* (93658b66) Merge branch 'v82-build'

* (fb7c7f81) Update README.md

* (a83c6777) Update README.md

* (79c505ef) Update README.md

* (e9a39756) Update README.md

* (0b501716) Update README.md

* (6c52e090) Update README.md




2019-11-05
----------


* (a758657e) Fixed bugs and augmented the QiqqaOCR debugging abilities while working on https://github.com/jimmejardine/qiqqa-open-source/issues/135 :
  - now 'SINGLE' mode can accept a page *range* or page *series* when running in NOKILL debug mode. When running in production mode, such input will throw an error as it's still unacceptable behaviour for when QiqqaOCR is invoked by Qiqqa itself
  - fixed stdio-as-binary handling bugs and memleaks in the 'GROUP' mode

  Other work done:
  - *ALL* tempfiles created by Qiqqa or QiqqaOCR are now located within the Qiqqa folder inside the system's TEMP folder for easier cleanup by external tools: anything that's in %TEMP%/Qiqqa/ and not touched in, say, an hour is a system cleanup candidate, i.e. to be deleted.
  - wrapped some unused code in `#if TEST ... #endif` as it is only used by TEST-routine(s).

* (377d53d4) comment typo fix

* (75d02e88) one more for https://github.com/jimmejardine/qiqqa-open-source/issues/122 after the fact.

* (ad03c3c6) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/118 ; not in the suggested way (via commandline argument), but as Utilities constant. Still a bit hacky to my tastes, but this should work and keep working. Timeout has been adjusted slightly upwards too to compensate for heavily loaded and/or slow machines: OCR'ing a page should take less than 4 minutes...




2019-11-04
----------


* (0b015c92) Merge branch 'v82-build'

* (a00facd1) edited gitignore for documentation testing

* (f11d55b8) picked up README edits from master branch

* (9984ff4d) Merge remote-tracking branch 'remotes/jimmejardine-original/master'

* (4f737ed0) Create CNAME

* (89e9de7d) tweak the amount of logging kept in rotation - part of debugging/analyzing qiqqa behaviour

* (a2ecd3ea) Update README.md

* (6beacd72) Create CNAME

* (c1b70ea3) Update README.md




2019-11-03
----------


* (4efb08c7) whoops. Forgot to run the CHANGELOG script.

* (70dc5d12) Merge branch 'v82-build'

* (5e787c15) bumped build revision

* (6a8dda06) don't just wait 10 seconds when extracting a Library bundle. It may be huge or you're running on a slow box and you'll get a b0rked extract. Just let 7ZIP complete.

* (4184e0c7) fix typo

* (7d4bcab9) Disable more unused source code files

* (38208863) Bit more refactoring work for https://github.com/jimmejardine/qiqqa-open-source/issues/95

* (d927e2bb) fix lingering crash in Dispose method. Follow-up for commit SHA-1: d05bbe2da06b825a0a079a73e14543f3af282165

* (f37c9dc2) https://github.com/jimmejardine/qiqqa-open-source/issues/95 : turns out most of it had already been done in the original Qiqqa. Upon closer inspection the remaining `Process.Start()` calls are are intended to open an (associated) application for a given file or directory, which is proper.

  Added a few `using(...)` statements around Process usage, etc. to prevent memory leaks on these IDisposables.

* (d05bbe2d) More work related to commit SHA-1: 43b1fe0972f99660e0bbbeea2deb357b2002f190 : fix crashes at application shutdown

* (06bddf60) Maintainable/MaintainableManager: refactor the shutdown code + correct the code to use non-skippable SafeThreadPool threads to Stop/Abort pending threads.

* (cef12a82) disable unused code files

* (43b1fe09)
  - Fix spurious crashes in `Dispose()` methods; these happen when terminating the application at special moments, e.g. while it is still loading the libraries on init (Hit Alt-F4 while the busybee cursor is still active and you might've run into a few of these, f.e.)
  - Make sure all `MessageBox.Show` actions go through the `MessageBoxes` class
  - Make sure every `MessageBox.Show` is executed from the UI thread, even when its wrapper was invoked from a background thread

* (148ea943) fix https://github.com/jimmejardine/qiqqa-open-source/issues/126

* (016b888e) fix https://github.com/jimmejardine/qiqqa-open-source/issues/132




2019-11-02
----------


* (c06021a8) bumped build revision




2019-11-01
----------


* (604b3dad) Merge branch 'v82-build'

* (4aaa7367) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into v82-build

* (5486dcaf) Merge pull request #125 from gitter-badger/gitter-badge

  Add a Gitter chat badge to README.md

* (112ce55c) Add Gitter badge

* (1dbf7f58) Merge branch 'v82-build'

* (c7570690) updated CHANGELOG_full.md

* (54bf3a87) bumped build revision (`npm run syncver`) and cleaned some code - no functional changes in this commit

* (95dff9bc) fix b0rk introduced by commit SHA-1: bcd73cd877b72cd2b9aba9183172dd6c46590880 :: we don't do a *revert* action per se, but rather improve upon the patch we picked up there from the experimental branch: as it turns out, the patch caused a lot of trouble which has been resolved to allow the running background task(s) access to a reduced clone of the WebLibraryDetails, which does not exhibit the cyclic dependency that the original WebLibraryDetails instance incorporated, thus breaking the cyclic reference and allowing the .NET GC to do its job on the Library instance(s) ASAP.

  As this problem was discovered while doing work on commit SHA-1: ed2cb589a2e3562102163c4b3129310c4850e33a, these files also include the remainder of the work done for that commit, as it was important to separate out the patches which fixed the cyclic memory reference.

* (ed2cb589) ran the entire codebase through DevStudio's Analyze->Code Cleanup->Run Profile 1, where Profile 1 was set up to include these:

  - Apply expression/block body preferences
  - Apply 'this.' qualification preferences
  - Sort usings
  - Remove unnecessary usings
  - Add accessibility modifiers
  - Sort accessibility modifiers

  The output has been manually code reviewed and has been adjusted to ensure all relevant files include ALL THREE AlphaFS using references anyway: Path, Directory, File to ensure we won't get surprised by odd spots *not* supporting long filenames when we later on edit the source code anywhere and happen to use one of the now 'unused' using references to AlphaFS.

  ---

  This implies there are NO FUNCTIONAL CHANGES in this commit.




2019-10-31
----------


* (bcd73cd8) picked up memleak fix from experimental branch

* (72a707f2) Merge branch 'v82-build'

  # Conflicts:
  #	Qiqqa.sln

* (2a72cbd1) Added a couple of RIS + BibTeX test files to the test set.

* (fe33552d) More work in line with commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0
  - fixing the last bit of work done in that commit where all the PageLayer-derived classes were fitted with IDisposable interfaces. Trouble is that the cleanup runs over a loop which accesses these instances via a PageLayer baseclass cast.
  - fixing several more DevStudio Code Analysis reports regarding Closed/Dispose handling of a few XAML controls; global Code Review used to update all OnClosed() handlers and treat them as we did the Dispose() code: NULL and Dispose/Clean what we can.
  - fixed crash in PageLayer-derived classes' Dispose, where the access to the `Children` member would throw a cross-thread-access error exception ( https://stackoverflow.com/questions/11923865/how-to-deal-with-cross-thread-access-exceptions ) Weird stuff happens as it looks like there are multiple Dispatchers in Qiqqa from this perspective, which is ... odd. (And, no, I'm not a WPF expert so it may be lack of understanding here.)

* (ce621333) DateVisible attribute has already been renamed and set in code via SetDatesVisible() in commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0

* (82588099) Let's see if the SyncFunction related UI hacks by Jimme Jardine are still needed: we already disabled those empty style classes in the previous commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0 ; now we take them out in the UI XAML definitions.

* (ff6e4eeb) Another Mother Of All commit with loads of stability & memleak + performance improvements work:
  - fixed https://github.com/jimmejardine/qiqqa-open-source/issues/121 : this happened due to a slightly over-eager `Dispose()` I introduced previously (:-() which reset the BibTeX content to an empty string, followed up immediately by a still-registered change event handler, which would communicate this 'change' to anyone listening, thus nuking the BibTeX metadata for the given Document.
  - fixed https://github.com/jimmejardine/qiqqa-open-source/issues/82 : part of that work has been done in earlier commits, where the size of of the editor panel (**height**) is designed to stick with the size of the 'fields editor mode' subpanel height so as not to jump up & down while you toggle edit modes. Also done in earlier commits: the RAW editor *wraps* the BibTeX text data so there's no need for a *horizontal* scroller any more; this should make diting in RAW mode a little more palatable, at least it is in my own experience.
  - Just like commit SHA-1: a540e506189ba1221ca93e09d5e5861196ed27f3, there's more IDisposable work done following the new DevStudio Code Analysis reports while I was hunting memory leaks and hunting down causes of https://github.com/jimmejardine/qiqqa-open-source/issues/112
  - Tweaked the initial library scan to first search all Qiqqa 'known_web_libraries' config files it can find in the Qiqqa libraries' **base directory**: this should help folks (like me) who wish to recover their old/crashing Qiqqa libraries, which previously would cause Qiqqa v79 and earlier Commercial Versions to crash in all sorts of spectacular ways - mostly due to buggy PDF documents sneaking into the libraries via Sniffer download b0rks and other sources of rottenness (such as the websites themselves).
    The positive effect of this change should be a stable list of libraries with as many of the original Web Libraries' Names restored as possible.
  - All libraries are flagged Read/Write instead of marking 'Web Libraries' as ReadOnly, which would block any editing/updating of those libs. As Open Source Qiqqa does not support Commercial Web Libraries in the way they were meant before (as syncable Qiqqa-based Cloud storage), you're working on 'independent copies' anyway, while we have to come up with other means to sync libraries like that. As these buggers can grow huge (mine is 20GB+), free cloud solutions (OneDrive, DropBox, etc.) with their 5GB limits are not a truely viable option. Alas, something to think about. **TODO**
  - Done another Code Review, scanning for all sorts of spots where the C#/.NET code needs a `using(...){...}` statement or something similar to ensure the allocated memory is actually *released when done*; this conjoins with the IDisposable work done in this commit.
  - LibrarySyncManager: we now cache the PDF Document Size as we already did with the PDF Document 'File Exists' flag. This should at least reduce the running cost of subsequent invocations of the Sync Details dialog after its first run, when that data is collected.
  - Performance: for large libraries, the initial load time was extreme, particularly when Qiqqa has 'remembered' that the library was open in its own Library View panel/tab. This is due to two major load factors: the BibTeX record for every document is parsed as part of deriving an AugmentedTitle and AugmentedAuthor set for display. Meanwhile, the library will be initially sorted by *date*, which took an *inordinate* amount of time as every date comparison would access and *(re)parse* the raw text date fields as obtained from the database. Now these parsed dates are cached in the PDFDocument until the cached value(s) are reset by the dates being modified within Qiqqa.
  - Performance: for large libraries, the Sync Details dialog would take an extreme amount of time, with the UI *locked*. Now we set the busy bee / wait cursor to indicate work is being done, while the work has been offloaded onto a background task. Also, while the PDF Document 'File Exists' state was cached in the PDFDocument record, the *size* of the document was not and thus was (re)calculated every time the user would invoke this dialog, resulting in huge delays as thousands of files' filesize info was (re)fetched from the disk on every invocation of the dialog. This has now been alleviated at least for *subsequent invocations* as the File Size is now also cached next to the File Exists datum in PDFDocument.

  - Here's the set of Code Analysis reports that were tackled in this commit:

  warning CA1001: Type 'BibTeXEditorControl' owns disposable field(s) 'wdpcn' but is not disposable
  warning CA1001: Type 'CSLProcessorOutputConsumer' owns disposable field(s) 'web_browser' but is not disposable
  warning CA1001: Type 'FolderWatcher' owns disposable field(s) 'file_system_watcher' but is not disposable
  warning CA1001: Type 'GoogleBibTexSnifferControl' owns disposable field(s) 'ObjWebBrowser, pdf_renderer_control' but is not disposable
  warning CA1001: Type 'Library' owns disposable field(s) 'library_index' but is not disposable
  warning CA1001: Type 'LibraryCatalogOverviewControl' owns disposable field(s) 'library_index_hover_popup' but is not disposable
  warning CA1001: Type 'MainWindow' owns disposable field(s) 'ObjStartPage' but is not disposable
  warning CA1001: Type 'PDFAnnotationNodeContentControl' owns disposable field(s) 'library_index_hover_popup' but is not disposable
  warning CA1001: Type 'PDFDocumentNodeContentControl' owns disposable field(s) 'library_index_hover_popup' but is not disposable
  warning CA1001: Type 'PDFPrinterDocumentPaginator' owns disposable field(s) 'last_document_page' but is not disposable
  warning CA1001: Type 'ReadOutLoudManager' owns disposable field(s) 'speech_synthesizer' but is not disposable
  warning CA1001: Type 'TagEditorControl' owns disposable field(s) 'wdpcn' but is not disposable
  warning CA1044: Because property AutoArrange is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property ConciseView is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property DatesVisible is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property DefaultWebSearcherKey is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property Entries is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property ImagePath is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property Items is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property Library is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property OnAddedOrSkipped is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property PageNumber is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property PaperSet is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property PDFAnnotation is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property PDFDocument is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1044: Because property TagsTitleVisibility is write-only, either add a property getter with an accessibility that is greater than or equal to its setter or convert this property into a method.
  warning CA1052: Type 'AlternativeToReminderNotification' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'BookmarkManager' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'Choices' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'CitationFinder' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'CSLProcessor' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'EndnoteImporter' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'ExpeditionBuilder' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'ExportingTools' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'Features' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'GeckoInstaller' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'GeckoManager' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'IdentifierImplementations' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'ImportingIntoLibrary' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'Interop' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'LibraryExporter' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'LibraryPivotReportBuilder' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'LibrarySearcher' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'LibraryStats' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'ListFormattingTools' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'MainEntry' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'MendeleyImporter' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'MYDBlockReader' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFCoherentTextExtractor' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFDocumentTagCloudBuilder' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFMetadataExtractor' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFMetadataInferenceFromOCR' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFMetadataInferenceFromPDFMetadata' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFMetadataSerializer' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFPrinter' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFSearcher' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'PDFTools' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'QiqqaManualTools' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'RecentlyReadDocumentManager' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'SampleMaterial' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'ScreenSize' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'SimilarAuthors' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'StandardHighlightColours' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'SyncConstants' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'TempDirectoryCreator' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'UpgradeManager' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'VanillaReferenceCreating' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'WebLibraryDocumentLocator' is a static holder type but is neither static nor NotInheritable
  warning CA1052: Type 'WebsiteAccess' is a static holder type but is neither static nor NotInheritable
  warning CA1063: Ensure that 'BrainstormControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'ChatControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'CSLProcessorOutputConsumer.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'FolderWatcher.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'LibraryIndex.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'LibraryIndexHoverPopup.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'MainWindow.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'PDFReadingControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'PDFRendererControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'ReadOutLoudManager.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'ReportViewerControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'SceneRenderingControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'SpeedReadControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'StartPageControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'TagEditorControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'WebBrowserControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1063: Ensure that 'WebBrowserHostControl.Dispose' is declared as protected, virtual, and unsealed.
  warning CA1721: The property name 'Annotations' is confusing given the existence of method 'GetAnnotations'. Rename or remove one of these members.
  warning CA1802: Field 'XXXXXX' is declared as 'readonly' but is initialized with a constant value. Mark this field as 'const' instead.
  warning CA1812: XXXXXX is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it static (Shared in Visual Basic).
  warning CA1827: Count() is used where Any() could be used instead to improve performance.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'Instance.OpenNewBrainstorm()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(cloned_pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(ddw.pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(matching_bibtex_record.pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(out_pdf_document, out_pdf_annotation.Page)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document, annotation_work.pdf_annotation.Page)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document, true)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document_node_content.PDFDocument)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(PDFDocumentBindable.Underlying, LibraryCatalogControl.FilterTerms)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(PDFDocumentBindable.Underlying, search_result.page, LibraryCatalogControl.FilterTerms, false)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(selected_pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenDocument(tag.pdf_document)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenNewBrainstorm()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenSampleBrainstorm()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'MainWindowServiceDispatcher.Instance.OpenWebBrowser()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new Bitmap(ms)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new CSLProcessorOutputConsumer(BASE_PATH, citations_javascript, brd, null)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new CSLProcessorOutputConsumer(BASE_PATH, citations_javascript, RefreshDocument_OnBibliographyReady, passthru)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new GoogleBibTexSnifferControl()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new MainWindow()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new PDFPrinterDocumentPaginator(pdf_document, pdf_renderer, page_from, page_to, new Size(print_dialog.PrintableAreaWidth, print_dialog.PrintableAreaHeight))' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new ReportViewerControl(annotation_report)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new StreamListenerTee()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'new StreamWriter(client)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'OpenWebBrowser()' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'this.OpenNewWindow(WebsiteAccess.Url_BlankWebsite)' before all references to it are out of scope.
  warning CA2000: Call System.IDisposable.Dispose on object created by 'wbhc.OpenNewWindow()' before all references to it are out of scope.
  warning CA2100: Review if the query string passed to 'SQLiteCommand.SQLiteCommand(string commandText, SQLiteConnection connection)' in 'GetIntranetLibraryItems', accepts any user input.
  warning CA2100: Review if the query string passed to 'SQLiteCommand.SQLiteCommand(string commandText, SQLiteConnection connection)' in 'GetLibraryItems', accepts any user input.
  warning CA2213: 'WebBrowserHostControl' contains field 'current_library' that is of IDisposable type 'Library', but it is never disposed. Change the Dispose method on 'WebBrowserHostControl' to call Close or Dispose on this field.
  warning CA2234: Modify 'GoogleBibTexSnifferControl.PostBibTeXToAggregator(string)' to call 'WebRequest.Create(Uri)' instead of 'WebRequest.Create(string)'.
  warning CA2234: Modify 'ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_SYNCHRONOUS(Library, string)' to call 'WebRequest.Create(Uri)' instead of 'WebRequest.Create(string)'.
  warning CA2237: Add [Serializable] to LocaleTable as this type implements ISerializable
  warning CA2237: Add [Serializable] to SynchronisationStates as this type implements ISerializable

  ## These are A-Okay and VERY MUCH INTENTIONAL: ##

  warning CA5359: The ServerCertificateValidationCallback is set to a function that accepts any server certificate, by always returning true. Ensure that server certificates are validated to verify the identity of the server receiving requests.
  warning CA5364: Hard-coded use of deprecated security protocol Ssl3
  warning CA5364: Hard-coded use of deprecated security protocol Tls
  warning CA5364: Hard-coded use of deprecated security protocol Tls11




2019-10-27
----------


* (90364ad9) more work done on https://github.com/jimmejardine/qiqqa-open-source/issues/112 / https://github.com/jimmejardine/qiqqa-open-source/issues/122 :
  - memleak prevention via `using(){...}` of IDisposable

* (bb0fde85) fix infinite call depth due to incorrect polymorphic interface use.

* (f8f64ceb) more work done on https://github.com/jimmejardine/qiqqa-open-source/issues/112 :
  - refactor the application termination handling in various parts of the application.
  - Long-running tasks (such as 'gather and save to disk') MUST NOT be done in the UI thread, while that main thread should be kept alive and responsive.
    + Hence cleanup & shutdown actions are relegated to a SafeThreadPool background task, while such cleanup/shutdown actions are flagged as 'not short-circuited at application shutdown time'.
    + Pending tasks (also in SafeThreadPool) are BY DEFAULT marked as short-circuit-able at shutdown time so that not only the PDFOCR job queue but also arbitrary pending background tasks queued in the SafeThreadPool are skipped=short-circuited at shutdown time to reduce the amount of time it takes to quit/exit Qiqqa.
  - The maximum 'reasonable wait time for threads to terminate at application shudown' has been set at 15 seconds. That number was *guestimated* as reasonable while testing a rig with 40K+ documents managed by Qiqqa. When this timeout is reached by any monitored background threads, then those threads are forcibly *aborted* as apparently their shutdown detection hasn't been functioning. Keep in mind to keep this timeout relatively high as, for instance, Lucene index saving can take a significant amount of time for large libraries (like the 40K+ one I'm testing with)
  - `Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown` is refactored to now be the end-all in answering whether Qiqqa is in the process of shutting down/terminating.
  - fixed several memleaks by wrapping IDisposable class instances in `using(){...}` statements: particularly Lucene/search-index based code had several spots where the DevStudio Code Analysis has failed to ever report these, while it did find many others before (when we started working on Qiqqa).
  - make Maintainable/MaintainableManager.cs code properly thread-safe by adding appropriate locks around minimal critical sections in order to prevent shutdown activity from being interfered with.




2019-10-26
----------


* (bc2955a6) Log file paths MAY contain spaces, hence surround those in double-quotes as well for the BundleLogs command which invokes 7zip on the commandline.

* (3f8d443a) Cleaned up the log4net init code and finally made it truly threadsafe: it turned out several threads where already trying to log/init while the log4net init process was not yet complete. `Logging.TriggerInit()` has been redesigned to check if the init phase MAY be triggered, while the init phase itself will write any 'early bird' log lines to the logging destination as soon as it is completely configured. Also removed the diagnostic init stacktrace dump to log: that one only still happen in DEBUG builds.

* (1955a86c) moving PDF Region Test UI code+XAML to the QiqqaUIPartsTester project where such test code should reside.




2019-10-25
----------


* (aedcd388) BeyondCompare 4 cannot handle UNC paths, which is what AlphaFS uses to support overlong paths (> 260 chars), hence we'll have convert these UNC paths back to local/native format for BC4 to be able to open the received+approved files for comparison. The rest of the test application should use the UNC paths though.

* (3e06f8a2) fix crash when running test code:

        Message:
          System.NullReferenceException: Object reference not set to an instance of an object.
        Stack Trace:
          Runtime.get_IsRunningInVisualStudioDesigner() line 23
          UnitTestDetector.get_StartupDirectoryForQiqqa() line 91

* (9a928b0c) updated NuPackages and added missing file from QiqqaUIPartsTester project

* (17135d71) fiddling with the UnhandledExceptionBox in a first attempt to add it to the QiqqaUITester. Turns out I'll have to take another approach there... :-\

* (41019bf9)
  - bumped + synced new build revision (v82pre4 coming up)
  - added QiqqaUIPartsTester test project - to be filled with UI test code to check the functioning of various dialogs and controls
  - added Microsoft-suggested code analyzers to the project packages (DevStudio 16.3.5)
  -




2019-10-22
----------


* (d1bc1a32) help performance checks/debug sessions: allow user access to the DisableBackgroundTasks config setting, but DO NOT persist that setting. (also we must remain backwards compat with v79 file formats for now)

* (b3fae39f) https://github.com/jimmejardine/qiqqa-open-source/issues/82 : try to set up some decent min/max sizes for the various controls/wrappers so that huge BibTeX raw content doesn't produce unworkable UI layouts and thus bad UX. MinWidth limits are meant to restrict button scaling and thus ugly/damaged looking UI, while MaxHeight limits are intended to limit the height of the BibTeXEditorControl when it is fed huge BibTeX raw content, such as when loading a BibTeX from converted PubMed XML (the source XML is appended as a multiline comment which often is very large)

* (868e5490) cleanup for https://github.com/jimmejardine/qiqqa-open-source/issues/82 : all the AugmentedButton instances which DO NOT need scaling (at least not yet in our perception of the UI) are AutoTextScale=false(default) configured. The ones which need to scale to remain legible at various screen and panel sizes are marked AutoTextScale=true.

* (2e4bce86) Add code to prevent memleaks around BibTeXEditorControl : there's no Dispose there, but we do have Unload event, which marks the end of a control's lifetime.

* (3248ea95) add validation check for https://github.com/jimmejardine/qiqqa-open-source/issues/119 : when we encounter a ClosableControl which doesn't have a name, it should be added as that will be needed for the check/persist path construction.

* (e2d84306) Tweaks to fix https://github.com/jimmejardine/qiqqa-open-source/issues/57 : handling extremely large (auto-)titles, etc.  Bummer: turns out the "Summary Details" in the right panel (QuickMetaDataControl) cannot be ellipse-trimmed like this as those are editable entities -- I didn't know (long-time Qiqqa user and still not aware of all the feature details \</snif>)

* (3ce449eb) stability: we ran into the 'waiting after close' issue again ( https://github.com/jimmejardine/qiqqa-open-source/issues/112 ); PDF/OCR threads got locked up via WaitForUIThreadActivityDone() call inside a lock, which would indirectly result in other threads to run into that lock and the main thread locked up in WM message pipe handling triggering a FORCED FLUSH, which, under the hood, would run into that same lock and thus block forever. The lock footprint has been significantly reduced as we can do now that we already have made PDFDocument largely thread-safe ( https://github.com/jimmejardine/qiqqa-open-source/issues/101 ).

  Also moved the Application Shutting Down signalling forward in time: already signal this when the user/run-time zips through the 'Do you really want to exit' dialog, i.e. near the end of the Closing event for the main window.

  `WaitForUIThreadActivityDone` now does not sit inside a lock any more so everyone is free to call it, while in shutdown state, when the WM message pipe is untrustworthy, we leave the relinquishing to standard lib `Thread.Sleep(50)` to cope with: the small delay should be negligible while we are guaranteed to not run into issues around the exact message pipe state: we also ran into issues invoking some flush/actions via a Dispatcher while the app was clearly shortly into shutdown, so we try to be safe there too.

  Also patched the StatusManager to not clutter the message pipeline at shutdown by NOT showing any status updates in the UI any more once the user has closed the app window. The status messages will continue to be logged as usual, we only do *not* try to update UI any more. This saves a bundle in cross-thread dispatches in the termination phase of the app when large numbers of pending changes and/or libraries are flushed to disk.

* (930fdb32) cleanup after commit SHA-1: 6f95dd688751fbcef0eb1c87ed8b7fd30cca863a : remove now useless debugging code.

* (6f95dd68) continuation of commits 3e82bdac0b92feca47f4c5ba1dc5261039804de5 + 337dc9ed64c65bd4144dbae579a47d915f986ad8 : make the compiled target directory tree available to the DevStudio Design View to grab external files, e.g. the splash screen image.

* (3e82bdac) previous commit already has a newer CSPROJ anyway and missed the JS script which did the work: here's the corresponding JS file

* (337dc9ed) Design View: there's no way to obtain the original build directories as VS2019 crates shadow copies of the binaries when executing them in the context of Microsoft Blend / Visual Designer / XAML Design View. The initial hack attempt, after all other 'regular' efforts failed, was to create a Settings.settings file in the Utilities project (that's where DevStudio stores the Project->Properties->Settings you set up) and add a PreBuild script which patches that file. HOWEVER, it turns out this data is copied all over the place by DevStudio: you must also patch `app.config` file to ensure DevStudio doesn't yak about change to update and alert for every single entry. THEN you find PreBuild is **too late** to have DevStudio regenerate the `Properties/Settings.Designer.cs` file. So we log this intermediate failed result and move on towards patching one of our existing files in the project instead: Constants.cs (will be filed in the next commit)

* (a20685b8) working on a fix for https://github.com/jimmejardine/qiqqa-open-source/issues/57 : part 1 = making the statusbar updates more usable by truncating them. Had a long url being reported for downloading which pushed all other messages off screen. :-1:

* (458cd2ec) performance / shutdown reliability: when closing the app, the UI message may be loaded severely and may not even terminate properly in spurious circumstances (had a situation here where the WaitForUIActivityDone calls were tough to shut up for a yet unidentified reason; happened after testing several Sniffer PDF download actions so we may be looking at another hairy bit of the XulRunner/Browser interaction here). Also add a check to the PDF/OCR thread queue processing code to help hasten shutdown behaviour.

* (52d6dd8b) splash page: barf a hairball when the splash page doesn't load as that surely means the Qiqqa install failed or got buggered somehow.

* (6d8812be) Splash page image is not loaded in MSVS Design View, among other resources. Working towards getting the bloody Design View in Visual Studio to work after all...




2019-10-21
----------


* (bfa90ed2) more work for the https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring: as the AugmentedButtons need to scale down their icons and **text** when they're part of a resizable panel in order to remain readable, we need to implement that and ensure it only happens where and when we want. TODO: fix more panels with AugmentedButton nodes as they are now already scaled down due to WPF autosize actions which are not meant to do that.

* (106d9544) code reformatting: no functional change.

* (ace54582) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/119: closables should have a name so we can create an identification path for them by traversing the WPF/UI tree: every such path can be checkec and hidden/shown individually, even persisted across runs.

* (68ad1bbc) added a couple of timers to measure the time spent in indexing the libraries

* (8393c556) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/82 : refactored the BibTeXEditorControl and all its users: those must now provide the toggle/view-errors/etc. buttons for the control, so that the parent has full control over the layout, while the BibTeXEditorControl itself will set the icons, tooltip, etc. for each of those buttons to ensure a consistent look of the BibTeX editor buttons throughout the application.

  TODO: see if we need to discard those registered buttons in the Unload event to ensure we're not memleaking...

* (acc4357a) add icons for BibTeX control et al: complete work started in commit SHA-1: 0f9fa67e470acb834e24cd30e946a0b71e954818 :: adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.

* (0cd9ea68) fix crashes in MSVS Design Viewer due to some properties not having 'get' access methods (as reported in the Exceptions thrown in the Designer View)




2019-10-20
----------


* (7dd8a599) MSVS Visual Designer fixup work: making sure the Theme stuff gets loaded timely when loaded from the Designer too.




2019-10-19
----------


* (e38715cd) correcting omission of commit SHA-1: fd8326e1e7c4878aac9ca8a1d903c7404fe7b90d * https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits. Includes some minimal prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/87.

* (26ab3dd4) fixing unfortunate edit oopsie: this part of the fixes/changes hadn't made it through in the previous fix commit for https://github.com/jimmejardine/qiqqa-open-source/issues/114 / https://github.com/jimmejardine/qiqqa-open-source/issues/115 : this corrects/augments these commits: SHA-1: 65e5707afa4e7e18181d00ef9c22b12048483b5e + SHA-1: a5faaafa233a181a47735f2e8981b6089c8ceaf7

* (fe35dbcc) fix https://github.com/jimmejardine/qiqqa-open-source/issues/116 : show left panel when switching the app from novice into export mode.

* (f7d7bce0) re-enable the 'Google Scholar' similar-documents panel in the PDF Reader left pane -- this is where most of the scrape info lands. ( https://github.com/jimmejardine/qiqqa-open-source/issues/114 / https://github.com/jimmejardine/qiqqa-open-source/issues/115 / https://github.com/jimmejardine/qiqqa-open-source/issues/117 )

* (761b1928) a fix for one path we didn't get for https://github.com/jimmejardine/qiqqa-open-source/issues/106 + be more strict in checking whether a Web Library sync directory has been properly set up.

* (e6ee95f9) OCR/text extractor: blow away the PDF/OCR queue on Qiqqa shutdown to help speed up closing the application (related to https://github.com/jimmejardine/qiqqa-open-source/issues/112). Also add thread-safety around the variables/data which cross thread boundaries.

* (27882cad) reduce a couple of now unimportant log lines to debug-level.

* (aec562b3) performance: remove a couple of lock monitors which we don't need any more.

* (0f7df40b) added one more test file for the BibTeX parser/processing: its surrounding whitespace in several fields should be cleaned up.

* (c4d7b6ee) upgrade the HtmlAgilityPack package used by Qiqqa. This is required for https://github.com/jimmejardine/qiqqa-open-source/issues/114 + https://github.com/jimmejardine/qiqqa-open-source/issues/115

* (fd8326e1) https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits. Includes some minimal prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/87.

* (0f9fa67e) adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.

* (1e3edb55) some minimal code cleaning and dead code removal

* (a5faaafa) fix https://github.com/jimmejardine/qiqqa-open-source/issues/115 : PDF Reader (which does a Scholar Scrape) does not work for users living outside US/UK. Also further fixes https://github.com/jimmejardine/qiqqa-open-source/issues/114. Also fixes https://github.com/jimmejardine/qiqqa-open-source/issues/117 by enforcing UTF8 encoding on the content: we're downloading from Google Scholar there, so we should be good. Google Scrape finally finds decent titles, author lists and even PDF download links once again.

  TODO: update the 'Google Scholar' view part in the PDFReader control.

* (65e5707a) fix https://github.com/jimmejardine/qiqqa-open-source/issues/114 as per https://stackoverflow.com/questions/13771083/html-agility-pack-get-all-elements-by-class#answer-14087707




2019-10-18
----------


* (9a1cdf91) fix typo in previous commit - this was already wrong in the experimental branch  :-(

* (ba9dcd54) typo fixes in comments and one function name. Changes ripped from the experimental branch.

* (1b24aec1) Merge remote-tracking branch 'remotes/jimmejardine-original/master'

* (4bf94053) Merge branch 'v82-build'

* (bc6b2b5b) Add files via upload

* (4ebc4dfb) Create How to locate your Qiqqa Base Directory.md

* (734ea885) make sure all NuGet packages reference .NET 4.7.2 - edit ripped from the master branch

* (8bc773ce) document the node/npm development environment, etc. in DEVELOPER-INFO.md

* (fd672197) added `superclean.sh` bash shell script to help clean up a Visual Studio environment for when you want to make sure you're starting Visual Studio *sans prejudice*.

* (7566098f) added DEVELOPER-INFO.md and pointed README.md at that document for info for developers wishing to work on Qiqqa.

* (ca772d70) document the node/npm development environment, etc. in DEVELOPER-INFO.md

* (0e7afcdb) added `superclean.sh` bash shell script to help clean up a Visual Studio environment for when you want to make sure you're starting Visual Studio *sans prejudice*.

* (0f508156) added DEVELOPER-INFO.md and pointed README.md at that document for info for developers wishing to work on Qiqqa.

* (cc6e2714) added all protobuf and binary serialization files to the legacy library -- to be cleaned up and provided with a proper interface

* (3a4df12b) Merge branch 'v82-build'

  # Conflicts:
  #	Qiqqa.sln

* (e2bef9a7) update CHANGELOG

* (ac042668) setup/installer: make the generated `setup.exe` should the same full build version as the Qiqqa executable itself.

* (1f6b9dfc) fix for setup/installer: set the `setup.exe` file version (as reported by Windows on right-click->Properties) to the Qiqqa release version instead of 0.0.0.0

* (6bb41ab0) rebuild uninstaller

* (88538dc4) regenerated approval reference files for all bibtex test data files after naming dedup fix in previous commit.

* (42c79d2a) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : fix bug where the Approvals **namer** helper function did produce the same files for different tests (because their test data files only differed in file extension, e.g. `.bibtex` vs. `.biblatex`)

* (288cbc7e) fix for `npm run refresh-data` build task: now generator includes code to cope with SHA-1: f3ed2f9640088de33cd58ed97b18891a157b4667 where we had added a comment containing one of the markers we're looking for in the code.

* (f3ed2f96) fixed unit test file for empty set for a DataTestMethod which is only there for developers to find out quickly after `npm run refresh-data` which test files have been properly employed and which have not.

* (6f3fa3fb) fixed bugs in `npm run refresh-data` build task: properly list those test data files which have not been used in any Unit test yet.

* (448e135a) add testfiles to TestData/... from elsewhere in the source tree: make sure all test files are in /TestData/...

* (7deb4ac9) introducing build task `npm run refresh-data` to help update the test C# files with the latest data set from TestData/...




2019-10-17
----------


* (2295adc5) rebuild uninstaller

* (b0e80fb1) added these build tasks: version sync: `npm run syncver`; bump release version: `npm run bump`

* (04623165) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : removed the data fixture files from the test projects themselves.

* (5c2ea6c9) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : moving all test data references, etc. data files to /TestData/... tree

* (a31480de) adding nodeJS based script to help update/sync/bump Qiqqa software release versions. Also added node/npm-style package.json file to the project: this carries the master version info and helps us quickly setup the proper node environment on any developer box.

* (d4730ed1) prepwork for the version patch / update / bump build task: manually update the copyright and version tags everywhere. Also added the ClickOnceUninstaller project (used by Inno Setup when building the Qiqqa installer) to the MSVS solution.




2019-10-16
----------


* (fe49d12e) Update README.md

* (299b0ff0) Update README.md

* (cb0b0539) Merge branch 'mainline-master' into v82-build

  # Conflicts:
  #	README.md

* (2cdd3426) Merge pull request #110 from GerHobbelt/mainline-master

  update README with latest info about Qiqqa software releases.

* (81472e17) Update README.md

* (6c1d8d69) update README with latest info about Qiqqa software releases.

* (f9d8f7d3) Update README.md

* (6ccb1475) updated CHANGELOG_full.md

* (56e9989a) Merge branch 'v82-build'

* (130239a3) Merge tag 'v82.0.7227.6146'

  # Conflicts:
  #	Qiqqa.sln

* (acd92291) README: point at the various places where software releases are to be found.

* (bf06d31f) Reorganize the README: developer info comes last.

* (db46fe8a) updated README to mention how to set up Long FilenameSupport on your Windows developer machine.

* (6470d49e) fixup for commit SHA-1: 0cf15c0d4d9377e80ddafd3063cbef038701bb3e -> add missing fixtures (auto-saved reference files) -> https://github.com/jimmejardine/qiqqa-open-source/issues/108

* (76f49107) fix unit tests: support Long Paths (>= 260 chars in length), i.e. UNC paths, by using AlphaFS `Path.GetLongPath()` API in the Approvals test rig.

* (0cf15c0d) fixup for commit SHA-1: c2e37869acba0903bc9687c27b4887990297fd03 :: moving the BibTeX (et al) data test files to the QiqqaUnitTester project as this data belongs over-there: it is required by the unit tests for the BibTeX parser, etc. in the Utilities library/project. The QiqqaSystemTester can always link/reference these test data files later on when it is desired so. This is part of https://github.com/jimmejardine/qiqqa-open-source/issues/107, while the new 'approved' files are added as part of the work done on https://github.com/jimmejardine/qiqqa-open-source/issues/108.




2019-10-15
----------


* (190ff05b) v82pre3 release.

* (ea7eca86) Disable part of the WeakEventHandler checker code as it gets fired due to come code triggering the check. TODO. (But for now I need a Qiqqa binary that at least works, pronto.)

* (fd882e77) performance / reduce memory leaks / reduce GC delays due to objects being marked by obsoleted status messages. Only remember the last message, hence no queue upkeep any more as we don't backpedal or otherwise use that status message queue of old *anyway*. Code simplification.

* (305c0851) fixes for https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/98: correct deadlock prevention code in CitationManager and shallow-copy the Attributes of a DictionaryBasedObject.

* (6ef9b446)
  - refactoring the PDFDocument code and making its interface threadsafe (apart from the inks, annotations and highlights attribute lists). Fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96.
  - improve the internal code of `CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS()` to help ensure we will be able to copy/transfer all metadata from the sourcing document to the clone. Code simplification.
  - code simplification/performance: remove useless LINQ overhead
  - `SignalThatDocumentsHaveChanged()`: improve code by making sure that a reference PDF is only provided when there really is only a single PDF signaled! Also see the spots where this API is being used.
  - belated commit of AlphaFS upgrade/refactoring for https://github.com/jimmejardine/qiqqa-open-source/issues/106 ; a multi-pass code review has been applied to uncover all places where (file/directory) paths are manipulated/processed in the Qiqqa code. TODO: Streams.
  - redesign the ADD/REMOVE/GET-password APIs for PDFDocument as the indirection in the Qiqqa made it bloody hard to make thread-safe otherwise. (Fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/98)
  - ditto for the GC-memleaking bindables of PDFDocument: all the binding stuff has been moved to the PDFDocument thread-safe wrapper class. (TODO: investigate if we can use WeakReferences more/better to prevent memleaks due to infinitely marked PDFDocument instances...)
  - tweak: `AddLegacyWebLibrariesThatCanBeFoundOnDisk()`: at startup, try to load all .known_web_libraries files you can find as those should provide better (= original) names of the various Commercial Qiqqa Web Libraries. (This is running ahead to the tune of https://github.com/jimmejardine/qiqqa-open-source/issues/109 / https://github.com/jimmejardine/qiqqa-open-source/issues/103)

* (d6f173d1) cf. SHA-1: 4c92feec44c362f4daaa4b8757f5e66aaff7359d * belated commit fixing code typo due to AlphaFS upgrade/refactoring in commit SHA-1: 7ecf0ae90d53d8961fefa25baa7b06d4bf319902 and https://github.com/jimmejardine/qiqqa-open-source/issues/106

* (c2e37869) moving the BibTeX (et al) data test files to the QiqqaUnitTester project as this data belongs over-there: it is required by the unit tests for the BibTeX parser, etc. in the Utilities library/project. The QiqqaSystemTester can always link/reference these test data files later on when it is desired so. This is part of https://github.com/jimmejardine/qiqqa-open-source/issues/107, while the new 'approved' files are added as part of the work done on https://github.com/jimmejardine/qiqqa-open-source/issues/108.

* (f25df6c9) Created new Test project for Utilities,etc. library unit tests, e.g. the BibTex parser tests. This addresses https://github.com/jimmejardine/qiqqa-open-source/issues/107. Also tweak/augment Approvals' helper classes to improve the tester UX as per https://github.com/jimmejardine/qiqqa-open-source/issues/108. The `Approver` now **saves the current test result to the 'approved' reference file when that one is not present** and then (obviously) does not invoke Beyond Compare or your favorite comparison tool. The first SO/Approvals provided solution still invoked BC for every auto-updated test, which was a HUGE nuisance. See the `ApprovalTestsConfig.cs` file for more info on this. The current implementation also results in slightly leaner test code, which is a free boon.

* (4c92feec) belated commit fixing code typo due to AlphaFS upgrade/refactoring in commit SHA-1: 7ecf0ae90d53d8961fefa25baa7b06d4bf319902 and https://github.com/jimmejardine/qiqqa-open-source/issues/106

* (8b90f41e) fix https://github.com/jimmejardine/qiqqa-open-source/issues/105 : turns out the config user GUID is NULL/not initialized at startup. This fix ensures there's a predictable (Guest)config active from the very start of the Qiqqa app.

* (d52d5655) quick hack for https://github.com/jimmejardine/qiqqa-open-source/issues/104 + v82pre3: make Qiqqa start no matter what. EFF that T&C!




2019-10-13
----------


* (f515ca73) directory tree as tags" also recon with UNIX-style path separators on Windows platforms: the DownloadLocation is not guaranteed to carry only Windows/MSDOS style '\\' path separators.

* (626902bc) (fix) customize library background image: copy the new file into place, if it is another file than the one we already have. Fix: do *not* delete the active image file when the user did not select another image file.

* (c66706bc) `FileTools.MakeSafeFilename` already performs filename length sanitization. No need to do that twice in different places.

* (ba9b9931) Add reference to source article of WeakEventHandler class + re-enable the proper use checks.

* (7ecf0ae9) part of a larger work: use AlphaFS::FIle/Directory/Path.* APIs everywhere. Also use UNIX `/`instead of MSDOS `\\` where-ever possible.




2019-10-12
----------


* (db4908df) rename: fix typo in filename




2019-10-11
----------


* (3d3b6ca5) code cleanup: remove one (semi)duplicate API




2019-10-10
----------


* (3f501caf) Merge branch 'v82-build'

* (3b5dcc16) updated CHANGELOG_full.md to current commit

* (3a0544c9) Updated README + CHANGELOG_full.md fixes for GFM

* (32d431ab) Merge remote-tracking branch 'remotes/GerHobbelt/master'

* (49a44cdd) Update README.md

* (6dd11919) Update CHANGELOG_full.md

* (9d33d6a8) Update README.md

* (2665d423) Update README.md

* (7070c53f) Merge branch 'v82-build'

* (0c108073) Merge commit '4df5d0b6f812f80bdc3e63f007a3cadf47898e44'

* (2832a078) tweak: DescriptiveTitle: trim every title to the default length + ellipsis.




2019-10-09
----------


* (099ff71f) Update README.md

* (25fbf3d2) Update README.md

* (01ded4ce) Update README.md

* (deaadc37) refactoring work necessary for fixing https://github.com/jimmejardine/qiqqa-open-source/issues/96 & https://github.com/jimmejardine/qiqqa-open-source/issues/101

* (0db18484) whitespace police

* (1b2daca9) Mother Of All commit with these fixes and changes:
  - added WPFDoEvents API for waiting for the UI thread to run its course (UI responsiveness), which is a strengthened version of DoEvents()
  - added WPFDoEvents APIs for mouse cursor Hourglass/Busybee override and reset: during application startup, this is used to give visual feedback to the user that the work done is taking a while (relatively long startup time, particularly for large libraries which are auto-opened due to saved tab panel sets including their main library tabs)
  - fixed a couple of crashes, particularly one in the RemarkOnException handler which crashed due to an exception being reported during application shutdown in one particular test run. (hard to reproduce issue, while we were hunting for causes of https://github.com/jimmejardine/qiqqa-open-source/issues/98 / https://github.com/jimmejardine/qiqqa-open-source/issues/96)

* (fb775d57) augment logging and take out the Sorax PDF page count API call due to suspicion of memleaking and heapcorrupting as per https://github.com/jimmejardine/qiqqa-open-source/issues/98 initial analysis report

* (ad576567) performance tweak: remove a thread lock monitor which is not important enough and which is loading the CPU (~6%); also reduce the critical section surface.

* (e353006a) PDFRendererFileLayer: when calculating the PDF page count happens to fail 3 times or more (Three Strikes Rule), then this PDF is flagged as irreparably obnoxious and the page count will be set to zero for the remainder of the current Qiqqa run -- this is not something we wish to persist in the metadata store as different software releases may have different page count abilities and *bugs*

* (bd3e372f) LockPerfChecker: fix name of caller to use to strip off the (useless) head lines of the stacktrace - which is included in the report when a lock happens to take longer than the timeout.

* (935a61f0) fix for WPF: correctly detect iff we're running in the UI thread or in another thread.




2019-10-07
----------


* (f35ae1f6) Revert "temporarily disable sorting - performance hogs - to see where the other perf bottlenecks hide out."

  This reverts commit b34abff27aa524c0397d5e6959bb600506292a29.

* (ceae5c6f) removed the last vestiges of performance costing thread lock monitor code. At least as far as application startup is concerned, we're now back to the classic performance hogs: UI list filling and sorting...

* (b34abff2) temporarily disable sorting - performance hogs - to see where the other perf bottlenecks hide out.

* (9eb3d590) add comments about purpose. (the tag sorting now seems to take the cake, performance-wise)

* (82c52dd6) performance testing of startup behaviour: the next big think is ReviewParameters() but we cannot ditch that one as it initiates the (re)draw of the controls. What we can do is save a little time in superfluous code.

* (359b8d84) after rerun of performance test: now the topmost consumer is the thread lock monitor in SignalThatDocumentsHaveChanged(), or at least regarding thread lock monitors. The highest bidder overall is currently: Qiqqa.DocumentLibrary.Library::BuildFromDocumentRepository	7794 (47.61%)

* (072a7c64) Delayed the PDF Page Count calculation a bit: it's not yet needed in the constructor call, so delay until actually requested. Also clean up the PDF page count helper method(s) a tad.

* (dc1e5bd5) app start performance test: second culprit was thread lock monitor code for TagManager. That code is harmless, so disabling the monitor code there.

* (171bf182) performance test: thread lock monitor for AugmentedBindable (which is invoked 40K+ for a large 40K+ lib as each PDFDoc has at least one of 'em) is eating the most. Disabling as that one is not suspect any more anyway.

* (4ab3f05a) The GetCurrentJobToken() API could be simplified without any loss in functionality. Also here's the remainder of the threading work done in SHA-1: 5e5206244190a8c599b883d17529eb59101174ff. And the heuristics around OCR job queueing have been tweaked. Should work out better for (very) large libraries this way.

* (eba4472f) Fix bug where it looked like Coty To Another Library menu choice and Move to Another Library memu choice didn't differ at all: it's just that MOVE did not properly signal the sourcing library needed an update as well as the document itself.

* (b109a929) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/96 by making sure that we pass a copy instead of a reference to the save logic. (**Incidentally, there are other thread crossings for pdf_document so we'll have to investigate this further as it's not just SAVE activity that's endangered by spurious crashes in annotation, tags or inks lists.**) Everybody should go through the QueueToStorage() API, by the way.

* (fb3de516) Feature Tracker: actually pick up the feature parameters and include them in the feature tracking info. Currently we don't store the featuretracking info (old Qiqqa had a Qiqq.utilisation file once) as we have DISABLED the GoogleAnalytics web interaction: that one was synchronous and only held up important activities.

* (1cc4d1f3) Clone/Copy didn't carry the document metadata across to the new lib: CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS() did not pass the URL, bibTeX, tags, etc. along so any action going through this API would copy only the PDF and minimal metadata. That is not what was intended, surely!?

  Also: the remainder of the tags is HashSet instead of List move.

* (55ce8022) tags: migrate from List<> to HashSet<>: that immediately solves the problem of duplicate tags too! Apply throughout the codebase. Note that Library.cs has additional changes, hence that one is also part of this, but will be committed separately.

* (269a41d5) statusbar progress bugfix: use the correct value as otherwise you'ld get a green bar with large number still to do.

* (6cc6e694) dial up the lock performance threshold for reporting from 100ms to 250ms: several log entries that are frequent now, ar indeed a bother, but not enough to merit being logged. We've got bigger fish to fry.

* (5e520624) background threads work: make sure all long running threads at least run at below-normal priority. Also ensure all stop-or-exit-due-to-disable-or-shutdown checks and logging is done at debug level (some of these changes will be committed in the subsequent commits as there's a mix of edit purposes in a few files)




2019-10-06
----------


* (9439b600) Cleaning up the logging action: the regular Debug activity is relegated to special builds which have the `DIAG` define *set* (I specifically DID NOT use `DEBUG` for this, so I can switch debug logging on in Release builds when the shit hits the fan). Meanwhile Unicode and Chinese language came to the rescue: `Debug特` is the new Debug level logging API methods set which will always do the job, in both DEBUG and RELEASE builds.

* (4df5d0b6) comment typo fix

* (fb412657) Merge branch 'v82-build'

* (037d42fd) twiddling...

* (8e40a828) fix crash in Jimme's code as I dumped some other libraries in there which have shorter names, e.g. "Guest2" (which is less than 8 characters) - https://github.com/jimmejardine/qiqqa-open-source/issues/93

* (26db1f48) Google Analytics throws a 403. Probably did so before, but now we notice it once again as we watch for exceptions occuring in the code flow. Better logging of the 403 failure.

* (4276cbf4) patched CHANGELOG roughly from CHANGELOG-full

* (9e77ee0b) Trying to cope with the bother of https://github.com/jimmejardine/qiqqa-open-source/issues/94 - quite a bit of it is inexplicable, unless Windows updates pulled the rug from under me (and CLR 4.0)

* (828d1dd4) comment typo fix

* (79231c00) dumb mistake caused the new test file not being discovered in the MSVS2019 Test Explorer: class must be public. duh.

* (b9b21393) Added another RIS test fixture file ( https://github.com/jimmejardine/qiqqa-open-source/issues/70 )

* (63bfe36c) spin off for  https://github.com/jimmejardine/qiqqa-open-source/issues/92 : add prerelease tests which will ensure there's no regression like that. (Discovering that one did hurt/smart!)

* (67df89cd) adding a few STILL FAILING TESTS' reference files: these are guaranteed to report failure until we get those bits of Qiqqa working properly (and/or the tests tweaked/corrected)

* (e8ceb91f) updated the 'approved' references for a few BibTeX test files

* (12ce5380) remove yet unused generic test rig bit

* (ff7b2437) fix https://github.com/jimmejardine/qiqqa-open-source/issues/92 : set all build targets to output x86 target code instead of 'AnyPC'

* (3f38b362) editing CHANGELOG.md, taking stuff from CHANGELOG_full.md




2019-10-05
----------


* (fdb469e7) twiddling...

* (ec57707e) fix crash in Jimme's code as I dumped some other libraries in there which have shorter names, e.g. "Guest2" (which is less than 8 characters) - https://github.com/jimmejardine/qiqqa-open-source/issues/93

* (23736203) Google Analytics throws a 403. Probably did so before, but now we notice it once again as we watch for exceptions occuring in the code flow. Better logging of the 403 failure.

* (312e9e34) patched CHANGELOG roughly from CHANGELOG-full

* (c8590be8) Trying to cope with the bother of https://github.com/jimmejardine/qiqqa-open-source/issues/94 - quite a bit of it is inexplicable, unless Windows updates pulled the rug from under me (and CLR 4.0)

* (3c97e856) comment typo fix

* (53988c2e) dump mistake caused the new test file not being discovered in the MSVS2019 Test Explorer: class must be public. duh.

* (e354aff6) Added another RIS test fixture file ( https://github.com/jimmejardine/qiqqa-open-source/issues/70 )

* (7efcff98) spin off for  https://github.com/jimmejardine/qiqqa-open-source/issues/92 : add prerelease tests which will ensure there's no regression like that. (Discovering that one did hurt/smart!)

* (26106cf2) adding a few STILL FAILING TESTS' reference files: these are guaranteed to report failure until we get those bits of Qiqqa working properly (and/or the tests tweaked/corrected)

* (193b149b) updated the 'approved' references for a few BibTeX test files

* (db433dbd) remove yet unused generic test rig bit

* (2f1d319e) fix https://github.com/jimmejardine/qiqqa-open-source/issues/92 : set all build targets to output x86 target code instead of 'AnyPC'

* (0fc1050d) Add files via upload

* (1fd984a8) Create How to locate your Qiqqa Base Directory.md

* (68be32d2) updated CHANGELOG_full.md

* (38d5a9ff) editing CHANGELOG.md, taking stuff from CHANGELOG_full.md

* (afb8260e) updated CHANGELOG_full.md

* (ed9c1291) re-did the CHANGELOG generator, using git+node. The old `changelog` tool (npm changelog / npm @g3erhobbelt/changelog) is not reliable and this was coded faster than debugging and correcting that one.

* (d4ad6d86) re-did the CHANGELOG generator, using git+node. The old `changelog` tool (npm changelog / npm @g3erhobbelt/changelog) is not reliable and this was coded faster than debugging and correcting that one.




2019-10-04
----------


* (2f21f827) Merge remote-tracking branch 'remotes/GerHobbelt/master' into v82-build

* (82b3475d) ignore build intermediates for the added legacy support lib project

* (580ed05b) built v82 setup/installer

* (758e941d) Removed unused NuGet bundles; built a v82 setup/installer:

  --------------------------------------
  Completed Packaging Qiqqa version 82.0.7216.35525 into ...\Qiqqa.Build\Packages\v82 - 20191004-194431\
  --------------------------------------

* (6f0c7abf) ignore build intermediates for the added legacy support lib project

* (476e113f) Merge remote-tracking branch 'remotes/GerHobbelt/master'

* (3d49ac07) Update README.md

* (ab39dd31) Update README.md

* (623ad006) working towards https://github.com/jimmejardine/qiqqa-open-source/issues/43 and offloading the old cruft to a dedicated library so the mainline codebase doesn't keep cluttered with old stuff, just because we want to be able to load/import old Qiqqa libraries.

* (079fb117) fix CefSharp missing report on rebuild

* (7bc8bce1) ignore the NuGet installed packages directory from now on.

* (e795da88) it wasn't such a particularly bright idea to include the NuGet packages in this repo: in principle I like it, but this is not JavaScript, this is C#, which comes with some HUGE packages (CefSharp is one) --> cleaning up and force-pushing a modified repo is in our near future...

* (4f46bc01) TODO comment added

* (b0bb67e4) CefSharp at least loads as a package but we're not using it yet, hence disable those code chunks.

* (cdd51d63) tweaking the sniffer: I have an idea how to cope with that toggle-able BibTeX advanced editor control and the nauseating behaviour of the toggle-X which moves along up & down and thus can hide behind the sniffer ok/fail/skip/clean buttons at top-right.

* (865eb87f) copied minimal code over from `experimental` branch: code cleanup + adding `DisableAllBackgroundTasks` config option, which is currently DISABLED and has no impact on the config serialization: the goal of these edits is that it'll be easier to merge with `experimental` branch later on without negative code/runtime impacts on the mainline.

* (db40efbf) QiqqaOCR: disable the image-output diagnostic code once again: this only used temporary for https://github.com/GerHobbelt/qiqqa-open-source/issues/1 diagnostics.

* (17651379) fix https://github.com/jimmejardine/qiqqa-open-source/issues/74 + https://github.com/jimmejardine/qiqqa-open-source/issues/73 = https://github.com/GerHobbelt/qiqqa-open-source/issues/1 : QiqqaOCR is rather fruity when it comes to generating rectangle areas to OCR. This is now sanitized.




2019-10-03
----------


* (6f283116) QiqqaOCR: added thread-safety for shared variables. Added a bit more logging to see what goes wrong.

* (986f9b3d) Debugging QiqqaOCR: copied chunks of the log4net config from Qiqqa itself as it turned out that the default settings for the console logging appender DOES NOT log DEBUG level statements. :-(

* (aee2f1be) store Qiqqa Configuration files (v79) for backwards compat testing

* (89a5ed5b) keep XML file for https://github.com/GerHobbelt/qiqqa-open-source/issues/3 + reduce existing XML test file in size as that one will serve another need (backwards compat)

* (74c93cfc) variables' renamed

  Former-commit-id: b0a7f18c8125e3ef51a2619ac7c86b33cad1bb7a

* (bf83faff) reduce the amount of (useless) logging


  Former-commit-id: 7f07674cf01776e117270ec40e02e5b9ff41d546

* (6d2a5de7) performance edit: reduce the time spent inside the OCR/PDFTextExtract queue lock critical sections.


  Former-commit-id: ad108b31874a112875f02683bdf4c3d91d77c453




2019-10-02
----------


* (43811862) whitespace police action


  Former-commit-id: 0f8e28f7fefe8a744699a5ee79d4acc4d8bae564

* (c77434c0) Added the beginning of CEFsharp (which will replace xulrunner in due time) as per https://github.com/cefsharp/CefSharp/issues/1714


  Former-commit-id: 0a3f9e6279681daeec7e43310155c5c7f6341620

* (3601732b) bump software version to 82 (v81 -> v82)

* (44a6af83) added and edited TODO task comments to show up properly in Visual Studio

* (f57e2e23) add additional bad/broken BibTeX records to the test set. These came out of work done resulting in https://github.com/jimmejardine/qiqqa-open-source/issues/71 + https://github.com/jimmejardine/qiqqa-open-source/issues/72

* (a3bc0b46) fix/tweak: when qiqqa server software revision XML response shows a *revert* to an older Qiqqa software release, such was coded but would not have been caught before as that bit of conditional code would never be reached when the user is running a later Qiqqa version (like I do now: v81 local build vs. server at qiqqa.com still reporting v79 as compliant / latest version).

  Added XML test file for later. (TODO)

* (6bc9cec0) document cyclic memory references discovered in the code during memory heap inspection. Nothing can be done about them right now.  :-(

* (36aeccf2) fix the background tasks starter hold-off code

  + use a simple state machine (`hold_off`) which ensures that the `OnceOff` task is only executed once the Qiqqa main window has been rendered, thus resulting in a perceptually faster load/start response
  + that same state machine is then advanced to the next (and final) state (zero(0)) which allows all other background tasks to start acting.

* (4fd0bd1f) recover 'restore desktop' functionality: partially reverting commit bc20149407175a76bb28fb015c5c25544f872721

* (95e5d5eb) fix compiler errors due to reachability error due to cherrypick mistakes while pulling this stuff from the `experimental` branch

* (e4a4968e) fix compiler errors due to cherrypick/merge mistakes for test project

* (397cb380) Qiqqa doesn't save the lib while working: checking hold-off and other 'background disable/postpone' features we've built in before when we were performance testing...

* (bba995af) added missing new project files (taken from the `experimental` branch)

* (45f083d2) upgrade projects to all reference .NET 4.7.2 instead of 4.0 Client Profile: AutoMapper and other libs we're going to use require an up-to-date .NET environment.

* (897d6cae) fixup cherrypick commit SHA-1: 97885405442be27b5f200d27a6467f30afc5faf8 which nuked testfile introduced in commit SHA-1: 2488ba6510739bd96892ee7dcb23e9bb983d9ea2 : baby steps towards https://github.com/jimmejardine/qiqqa-open-source/issues/68

* (531e119e) comment typo fix

* (48daaa2b) `support@qiqqa.com` is of course R.I.P., hence point at github issue tracker instead for software failures.

* (0090a8d8) added a couple of test files for BibTeX Importer tests

* (e77d66cb) added test files to test rig

* (0ddf0416) added a couple more specific test files for BibTeX

* (97885405) first baby steps towards https://github.com/jimmejardine/qiqqa-open-source/issues/68 : adding more tests and registering the current state of affairs in 'approved' reference files by way of `ApprovalTests` use in the test rig.

  # Conflicts:
  #	QiqqaSystemTester/data/fixtures/bibtex/b0rked-0008.bib

* (5019762b) NEVER add/register the `*.received.json` files produced by ApprovalTests in git; only the `*.approved.json` user-approved reference output files should be registered with the revision control system.

* (5d06874f) added more BibTeX test data files + tweaked reference output path for ApprovalTests custom DataTest namer/writer.

* (7916c097) further work on getting a BibTeX DataTest unit test running using the ApprovalTests library -- for this purpose we had to create a custom Writer and Namer for ApprovalTests does not provide those suitable for DataTests.

* (c3ed5dba) cherrypick fix: don't forget the latest JSON library package

* (2488ba65) add test sample files for https://github.com/jimmejardine/qiqqa-open-source/issues/68 + https://github.com/jimmejardine/qiqqa-open-source/issues/72

* (243082d3) fix crash due to config string being NULL

* (b0ae9bb0) reduce the number of internal try/catch exceptions when handling more-or-less flaky web traffic

* (4f07d34f) Legacy Web Library: such a library is NOT read-only. (What we got to do is point it to an 'Intranet' sync point = directory/URI path instead. (TODO)

* (bd923e57) fix https://github.com/jimmejardine/qiqqa-open-source/issues/72 + adding **minimal** support for bibtex concatenation macros in order to (somewhat) correctly parse google scholar patents records: fixes https://github.com/jimmejardine/qiqqa-open-source/issues/22 and a bit of work towards https://github.com/jimmejardine/qiqqa-open-source/issues/68

* (5ccc4fa7) fix https://github.com/jimmejardine/qiqqa-open-source/issues/71 : BibTeX parser no longer is stuck in infinite loop when encountering unterminated fields, etc.




2019-10-01
----------


* (63783471) add the mirrored commercial Qiqqa installers for backtesting/etc. purposes.

* (52dc2df4) bit of code cleanup in the Intranet Library Sync code section.

* (eb526783) fix cherrypicks thus far...

* (38a3d27a) fixing commit SHA-1: da2186c50c63fd9454862d751437f06210700d28 (working on the new Unit Tests for BibTeX parsing) + adding tests

* (683d27e6) OFF-TOPIC twirling in my nose...

* (581fc046) moved the `\n\n\n` append patch to the BibTeX lexer and started the refactor there. Working towards https://github.com/jimmejardine/qiqqa-open-source/issues/68

* (a19f7da7) remove old cruft in BibTeX parser: the `\textless` and `\textgreater` LaTeX macros define `<` and `>` and those should never-EVAR be present around *any* BibTeX record. Besides, we WILL add code in the parser/lexer later on to cope with TeX macros inside text strings, so this bit is an old hack at best. Now it's GONE.

* (e52c3e36) add missing reference to Utilities project in Qiqqa TestHelpers project

* (bde0c3ec) working on the new Unit Tests (for BibTeX parsing):
  - enabled some of the performance tests in the codebase by loading the same source file in the Unit Test project and having `#if TEST` code sections turned on there
  - added unit test to verify that the unit tester has `#if TEST` defined/enabled
  - added a TraceAppender to allow performance tests and others to log to the Test Explorer output via TraceLogAppender / TRACE
  - adjusted the log4net configuration(s) accordingly

* (0a675a35) (SCRATCHWORK) fiddling with the BibTeX editor control to make it behave with the Sniffer window: currently the green X to toggle between BibTeX and RAW view mode gets obscured for files which have few or no BibTeX lines, thus making that bit of the UI unusable ATM. TODO: must find a way to resize the RAW view pane to max of both or some other method to ensure a minimum height that's acceptable everywhere.

* (c1918c12) added class in Utilities for constants which are used in multiple spots in the Qiqqa codebase.

* (68679221) adding BibTeX test data from various sources into `...Tester/data/fixtures/...`

* (bc05c346)
  - upgrading NuGet packages: MSTest 2.0beta4 to 2.0
  - adding NuGet package ApprovalTests so we can more easily compare BibTeX records and other complex data input/outputs
  - slowly getting the MSTest-based unit tests set up and going: using the new MSTest v2 DataRow feature to keep the amount of test code low.
  - added `Assert.FileExists()`; used in TestBibTeX

* (aa2b7a12) code cleanup and a bit of dead code removal (there was a property in there which mentioned 'unit tests' -- which did not come with Qiqqa and are being recreated, but will not require or use this dead bit of code anyway.

* (07bb5697) allow tags to be separated by COMMA as well as SEMICOLON. (Added COMMA separator support)

* (81526f6a) compiler hint: `const`

* (8ca0b43e) ignore scratch directory

* (dad18fd1) fix for the new Sniffer feature: a full-fledged BibTeX editor pane, just like you get in the library/document view panels in Qiqqa. KNOWN ISSUE: for small BibTeXs (only a few lines of BibTeX for the given document), the green toggle X is obscured by the thumbs up/down fade in/out buttons. :-(

* (ec1fe2cf) one more for https://github.com/jimmejardine/qiqqa-open-source/issues/67

  WARNING: a PDF URI does *not* have to include a PDF extension!

  Case in point:

      https://pubs.acs.org/doi/pdf/10.1021/ed1010618?rand=zf7t0csx

  is an example of such a URI: this URI references a PDF but DOES NOT contain the string ".pdf" itself!

* (4a03a3d6) performance optimization: `PubMedXMLToBibTex.TryConvert()` doesn't have to go through the entire XML parsing process when a simple heuristic can already detect which input/content won't pass anyhow: this will quickly filter out the major number of entries fed to PubMedXMLToBibTex, while also reducing the number of useless logfile lines as each such entry would previously log one PubMedXMLToBibTex failure line.

* (a4dbef68) Merge branch 'master' into experimental-ui-edits

  # Conflicts:
  #	Qiqqa/Documents/PDF/PDFControls/MetadataControls/GoogleBibTexSnifferControl.xaml.cs

* (27ffcc4f) Fiddle with the versions: Qiqqa and QiqqaOCR should have the same build/version numbers.

  TODO: The Version number needs to be updated in a few more places too.

* (def7f3cd) Work done on https://github.com/jimmejardine/qiqqa-open-source/issues/65 ; also add an event to signal the completion of the library load task as the UI is updated asynchronously, only once and then *way too early* so the numbers for 'document(s) in library' are quite wrong 99.9% of the time. Only jiggling the UI into a refresh by jiggling the Graph/Chart button or Carousel button helps as those OnClick handlers repaint the library summary entry in the library list and thus update the 'document(s) in library' count. ... Meanwhile, that way you get a cruddy flow which doesn't update the UI when it *should* and overall takes way to much time as the library document lists also get updated at the wrong moment -- which takes quit some time for large libraries and is quite useless when the set is not yet completed and/or that UI pane is not visible at the time... :-(

* (c4b64a4a) PDF imports: add menu item to re-import all PDFs collected in the library in order to discover the as-yet-LOST/UNREGISTERED PDFs, which collected in the library due to previous Qiqqa crashes & user ABORT actions (https://github.com/jimmejardine/qiqqa-open-source/issues/64)

  Additional fix: use `AugmentedPopupAutoCloser` on *all* import menu items as otherwise the menu will remain open and obscure part of your active UI once the desired menu item has been clicked.

* (3872959d) `AddNewDocumentToLibraryFromInternet_*()` APIs: some nasty/ill-configured servers don't produce a legal Content-Type header, or don't provide that header *at all* -- which made Qiqqa barf a hairball instead of properly attempting to import the downloaded PDF.

  Also don't yak about images which are downloaded as part of Google search pages, etc.: these content-types now make it through *part* of the PDF import code as we cannot rely on the Content-Type header being valid or present, hence we need to be very lenient about what we accept as "potentially a PDF document" to inspect before importing.

  Fixes: https://github.com/jimmejardine/qiqqa-open-source/issues/63

* (d2b5c224) tackling with the weird SQLite lockup issues: https://github.com/jimmejardine/qiqqa-open-source/issues/62

  As stated in the issue:

  Seems to be an SQLite issue: https://stackoverflow.com/questions/12532729/sqlite-keeps-the-database-locked-even-after-the-connection-is-closed gives off the same smell...

  Adding a `lock(x) {...}` critical section **per library instance** didn't make a difference.

  Adding a global/singleton  `lock(x) {...}` critical section **shared among /all/ library instances** *seems* to reduce the problem, but large PDF import tests show that the problem isn't *gone* with such a fix/tweak/hack.

* (abd020ae) UPGRADE PACKAGES: log4net, SQLite, etc. -- the easy ones. Using NuGet Package Manager.

  # Conflicts:
  #	Utilities/packages.config

* (b5a42568) preparation for unit tests that can work: add a QiqqaTestHelpers library -- it turns out we're pretty much toast when we use NUnit, so that one's **out**; then there's MSTest but the standard Assert library there is rather lacking, hence we've ripped the Assertions from xUnit/nUnit and tweaked/augmented them to suit MSTest and our rig -- the intent being that you can still see **and debug** the tests from within Microsoft Visual Studio. It's all a bit hacky still to my taste, but at least now we don't get crazy NUnit execution failures any more for every !@#$ test.

* (a339b371) preparation for unit tests that can work: replace application startpath with equivalent code from the Utilities libraries which has built-in detection whether the unit tests are running or Qiqqa itself.

* (b5c64671) moving from non-working NUnit to MSTest-based unit/system test project for Qiqqa -- let's pray this works out well for us as using Nunit turned out to be non-working.

* (3efdfd4e) EXPERIMENTAL: add full BibTeX editing power to the sniffer, that is: use the same BibTeX control there as used elsewhere where you can switch between RAW view and PARSED BibTeX lines. ----- TODO: link up the edit events again.

* (f8a07627) EXPERIMENTAL: don't update the base control as the list controls get updated already thanks to commit SHA-1: babb1bcdd531db2a4aee7ca12739265beb2199c6
  In a sense this is a partial revert of commit commit SHA-1: bcdd43f2b329363b0c953f22a2c1630533081fdf just to see if it flies...

* (43debf6f) remaining work for  https://github.com/jimmejardine/qiqqa-open-source/issues/56 / https://github.com/jimmejardine/qiqqa-open-source/issues/54 -- catch some nasty PDF URIs which weren't recognized as such before. Right now we're pretty aggressive as we fetch almost everything that crosses our path; once fetched we check if's actually a valid PDF file after all. CiteSeerX and other sites now deliver once again...

* (c3102a76) fix: BibTeX dialog doesn't scroll: that's a problem when your list of BibTeX tags is longer than the height of the dialog allows. Hence we can now scroll the bugger.

* (b46b38d3) Don't get bothered by the Tweet stuff: collapse it.

* (c3567cf4) further tweaks for the MS Designer preview. AugmentedButton now kills its (preset) caption when its icon is set up programmatically. This should keep the icon buttons clean.

* (351429ca) [FEATURE] work done for https://github.com/jimmejardine/qiqqa-open-source/issues/21 : the BibTeX entry definitions are now sitting in an external JSON file so every savvy user can edit/add their own set of fields there now.

  BTW: Nunit testing was a great idea but I'm getting savagely abused around my Glutus Maximus with a Cactus by way of the woes of https://stackoverflow.com/questions/15614192/run-a-specified-nunit-test-as-a-32-bit-process-in-a-64-bit-environment . I effin' *hate* .NET right now - and particularly old cruft that needs upgrading so I can move out of that fugly 32bit shyte sewer.      \< angry like mad! />

* (e0d056ce) fixes for https://github.com/jimmejardine/qiqqa-open-source/issues/56 ; also ensuring every document that's fetched off the Internet is opened in Qiqqa for review/editing (some PDF documents were silently downloaded and then dumped into the Guest Library just because and you'ld have to go around and check to see the stuff actually arrived in a library of yours. :'-(

* (4b7bec3e) nuke unused sourcecode files

* (b12bddf0) further work to accommodate the Microsoft Designer tool: previously the code did not provide many 'getters' which caused many crashes/exceptions in the Microsoft XAML Designer.

* (55ba0f65) BibTexEditorControl fix: bibtex keys such as "organisation" were clipped in the UI; now the key column had been widened a little to accommodate these larger key names.

* (f8d5ff7c) BibTeX modes for manual editing and vetting in the Sniffer.

* (4a1bb748) tweaked XAMLs for better Designer preview: added button texts to the XAML instead of only having it set in the code.

* (be0d54fa) fix: https://github.com/jimmejardine/qiqqa-open-source/issues/60 + https://github.com/jimmejardine/qiqqa-open-source/issues/39 + better fix for https://github.com/jimmejardine/qiqqa-open-source/issues/59

  check how many PDF files actually match and only move forward when we don't end up full circle. don't reflect if we didn't change. when we re-render the same document, we don't move at all!

* (4e791e85) fix https://github.com/jimmejardine/qiqqa-open-source/issues/59: don't reflect if we didn't change.

  We start with a dummy fingerprint to ensure that we will observe ANY initial setup/change as significant for otherwise we don't get the initial PDF document rendered at all!

  We use the PDF Fingerprint as a check for change as the numeric `pdf_documents_search_index` value might look easier but doesn't show us any library updates that may have happened in the meantime.

* (df8c9f77)
  - fixed the focus loss while scrolling through the library PDF list using the keyboard: it's the repeated invocation of the code line `ThemeTabItem.AddToApplicationResources(app)` for each individual control being instantiated that killed the focus semi-randomly. That code was introduced/activated that way a while ago to better support the MSVS2019 Designer -- which should still work as before.

  - fixes several issues reported in https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632 : the keyboard scrolling no longer suffers from long delays -- where I first suspected thread locking it turns out to be a hassle with WPF (grrrrrr); I cannot explain **how exactly** WPF caused the lethargic slowdown, but the fact is that it's gone now. The fact I cannot explain this makes this codebase brittle from my point of view. Alas.

  - it *looks* like I can gt away with a bit of a performance boost as that `listview.UpdateLayout();` from `GUITools::ScrollToTop()` looks like it can be safely moved into the IsVisibleChanged handler for said ListView(s)... hmmmmm

* (f417ea87) !@#$%^ got it! Had been too zealous when hack-patching the faults at ScrollToTop. Dang!  https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink & commit SHA-1: babb1bcdd531db2a4aee7ca12739265beb2199c6

* (bc201494) building x86 only as otherwise antique tesseract et al will fail dramatically. Otherwise aligned the settings of the projects and disabled a few config items in the cod for testing the current view update woes. >:-(   I !@#$%^&^%$#@#$%^ loath WPF.

  # Conflicts:
  #	Qiqqa/packages.config

* (be4e884b) fiddling: add a *failing* dummy test case to the test suite -- to be written when we address BibTeX parsing for real.

* (0b45a6dc) Temporarily DISABLE MSVC Designer Mode detection

* (c8e37299) Locking in the current state of affairs as of https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink while I dig further to uncover the *slowtard* culprits. Mother Of All Commits. Can be split up into multiple commits if needed later on in a separate branch. Actions done in the past days while hunting the shite:

  - `library.GetDocumentByFingerprint(fingerprint)` can get you a **NULL** and quite a few spots in the code didn't account for this; ran into several crashes along the way due to this. Now code has been augmented with Error logging, etc. to at least report these occurrences as they often hint at internal sanity issues of the library or codebase.
  - using C# 6.0 language constructs where it helps readability. I'm happy the `?` and `??` operators are around today.
  - **IMPORTANT**: instrument the Hell out of those C# `lock(...)` constructs to detect if any of those buggers is deadlocking or at least retarding on me. The code idiom looks like this:

              Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
              lock (thread_lock)
              {
                  l1_clk.LockPerfTimerStop();

     where the `LockPerfChecker.Start()` call sets up an async timer to fire when waiting for the `.LockPerfTimerStop()` takes too much time. Any such occurrence is then logged to Qiqqa.log

  - *NUKED* the Chat timer ticker as it hampered the debugging sessions no end and is pretty useless right now anyway.

    TODO: Uncomment that `#if false` in ChatControl when I'm done.

  - fixed a NULL string crash in the Window Size Restore logic (StandardWindow.cs)

  - added more Debug level logging to monitor the action while the large Library fills up

  - `BlackWhiteListManager`: improved error reporting: either the file isn't there *or* it's corrupt and we should be able to identify both situations from the log.

  - `FolderWatcher`: rename the ill-named `previous_folder_to_watch` variable.

  - `FolderWatcher`: `file_system_watcher.Path = folder_to_watch;` will crash when you happen to have configured a non-existing path to watch. The new code takes proper care of this situation.

  - `NotificationManager.Notification`: when the tooltip is NULL, it will use the notification text itself instead. Corrected the invocations of this API to use this feature and reduce the amount of string duplication in the source code.

  - use the preferred/MS-advised idiom for `lock(xxx_lock) { ...use xxx...}` where a separate `object` is created for the lock thoughout the codebase: this prevents odd failures with locking Dictionary-types etc. -- I haven't explicitly scanned the codebase to uncover all but most such situations have been corrected.

  - `Library:BuildFromDocumentRepository()`: log the progress loading the documents once every X seconds rather than every 250 documents: sometimes loading is fast, sometimes slow and I'd rather observe such in the logfile.

  - `Library` performance: `internal HashSet<string> GetAllDocumentFingerprints()` has been recoded and should be (marginally) faster now.

  - `Library` `PDFDocuments` performance: several controls used code like `library.PDFDocuments.Count` which is rather costly as it copies an entire set just to measure its size. Using the `library.PDFDocuments_IncludingDeleted_Count` API instead.

  - `Library`: `internal HashSet<string> GetDocumentFingerprintsWithKeyword(string keyword)` has been augmented with a `Publication` match check and has been made thread-safe like the rest of them: all code accessing the `pdf_documents` member should be thread-safe or you're in for spurious trouble.

  - `LibraryFilterControl`: encoded all sorting mode delegates the same way: as functions in `PDFDocumentListSorters`. (Code consistency)

  - As I ran into several crashes in the application init phase, a few *singleton objects*' idiom has changed from

          class WebLibraryManager
          {
            public static WebLibraryManager Instance = new WebLibraryManager();

     to

          class WebLibraryManager
          {
            private static WebLibraryManager __instance = null;
            public static WebLibraryManager Instance
            {
              get
              {
                  if (null == __instance)
                  {
                      __instance = new WebLibraryManager();
                  }
                  return __instance;
              }
           }

     which results in a singleton with 'init on first demand' behaviour, hence singleton instances which initialize later during the application init phase, where various internal APIs become safely available. (See also the `Logging` entry below)

  - `PDFDocumentCitationManager`: despite a slew of `lock(...)` code, there was still thread-UNSAFE usage in there in the `CloneFrom()` method, due to the lock being **instance**-particular. This has been resolved in the class' code.

  - `GoogleBibTexSnifferControl.xaml.cs`: fixed another spurious NULL-induced crash

  - `FeatureTracking:GoogleAnalysicsSubmitter` begets a 403 (Forbidden) from Google Analytics. Report such in the logfile instead of letting it pass silently. (Not that I care that much about feature tracking right now, but this was part of the larger code review re HTTP access from Qiqqa)

  - `StatusBar`: move the thread-creating/queueing `Dispatcher.BeginInvoke(...)` call outside the critical section: this will prevent potential deadlock scenarios (not sure if this is one of the retarders, but at least it popped up during code review).

  - BibTeXAssembler/Parser: append some harmless whitespace at the end to help reduce the number of out-of-bounds exceptions in the parser/lexer while keeping the code simple

  - `GUITools::public static void ScrollToTop(ListView listview)` was seriously haranguing the logfile with a slew of deep-level exceptions while large libraries are loading and the UI is set up in Qiqqa, due to the listview not yet being ready to receive such attention. After trying a few things, I *think* I got it fixed by checking the `Height` property for `NaN` to preclude the entire offending code chunk.

     **Interestingly enough, said code chunk will now *never execute* due to the premature UI updating done via those `.Library = library` code lines mentioned in  https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink.**

     See the comment chunk above this edit for more info.

  - `Logging`: ohhhh boy! It started with working on the crashes detected at early app init time, where there were several `Logging.Info()` calls which would trigger a crash in the `Logger` constructor code, due to other bits of the code not being ready yet. Now the `Logging` class buffers any such 'premature' logging reports in a RAM buffer and dumps them to logfile as soon as it comes available. Meanwhile...

  - `Logging`:  **WEIRDING-OUT ALERT**

     ... seems to be a **very odd class** as any methods added to it seem to **automagickally lock** i.e. are thread-safety-protected, but I cannot find out how this was done -- it's been too long since I last went neck-deep into .NET and I seem to recall some behaviour like that from anything log4net-related from back in my days with .NET and log4net. Anyhow, after gnashing my teeth and killing a few debuggers I went for the alternative: the work-around. Those functions have been moved into the new `LockPerfChecker.cs` file/class, which is nice re code organization as well. Nevertheless I believe I should those **weird** lockups I had with lock test code in `LogError` there while I was debugging and testing the new `lock()` instrumentation code. Very odd indeed, unless one assumes the entire `Logging` class is thread-lock-protected in *bulk*. **WEIRDING-OUT ALERT**

  - `QueueUserWorkItem`: disable test code in there which is only slowing us down.

  - removed unused source files, e.g.

        <Compile Include="Finance\YieldSolverAnnualised.cs" />
        <Compile Include="Finance\YieldSolverContinuous.cs" />
        <Compile Include="Finance\YieldSolver.cs" />

* (afa4e50c) fix cherrypicks thus far...

* (ee6ebac6) `WebLibraryDetail`: augmented the class with a `public string DescriptiveTitle` derived property which is used for the library document list view: some derived titles can be extremely long, e.g. when there's no title text available, yet a long SourceDocument URI is used instead. This code is combined with new `StringTools` APIs to produce a title that's limited to a given length and when longer prints the first part plus an ELLIPSIS. This dals with https://github.com/jimmejardine/qiqqa-open-source/issues/57

* (04089fdc) code cleanup: killing unused code/classes

* (70650706) code cleanup: encode all reading stages

* (8b50a356) part of the thread code refactor: Import the Qiqqa manuals into the guest library **in the background, waiting until the library has loaded**.

* (b004c53d) bit of code cleanup

* (5e2ec578) string=null + string=value --> cleaner is to add string:"" + string:value then.

* (6e2a2171) BibTeX processing: augmented the error logging

* (0cbda4e3) thread management refactor:

  - `MaintainableManager.Instance.Register` --> `MaintainableManager.Instance.RegisterHeldOffTask`
  - registered background tasks only start to execute once the 'hold off' has been signaled, AFTER WHICH the preset delay is started before the actual task code is executed itself for the first time.
  - obsoleted the `GeneralTaskDaemon` code/class
  - added more thread safety code for the folder watchers as the check `FolderContentsHaveChanged` may be invoked from multiple threads
  - postpone the call to `RememberProcessedFile` as much as possible; the PDF processing code flow has to be refactored still (TODO!)
  - added more thread safety code for FolderWatcherManager
  - always keep the Word Connection Setup thread running, even when other background tasks have been disabled, since this thread is required for the InCite front work (direct user activity)

  # Conflicts:
  #	Qiqqa/Common/BackgroundWorkerDaemonStuff/BackgroundWorkerDaemon.cs
  #	Qiqqa/Common/GeneralTaskDaemonStuff/GeneralTaskDaemon.cs
  #	Qiqqa/DocumentLibrary/FolderWatching/FolderWatcher.cs
  #	Qiqqa/InCite/WordConnector.cs
  #	Utilities/Maintainable/MaintainableManager.cs

* (c94c3623) fixd bug, found as part of the task register code refactor: Quit this delayed storing of PDF files when we've hit the end of the execution run: we'll have to save them all to disk in one go then, and quickly too!

  Note the new code line

              Utilities.Shutdownable.ShutdownableManager.Instance.Register(Shutdown);

  in particular!

  # Conflicts:
  #	Qiqqa/Documents/Common/DocumentQueuedStorer.cs

* (9fe39eff)
  - shut up Visual Studio Designer - at least as much as we can on short notice: give 'em the Theme colors and brushes, limit the number of XUL crashes while loading the Sniffer XAML, etc.
  - BibTeX Sniffer: put a time limit of slow filter criteria (HasPDF and IsOCR checks); mark the checkboxes for those filter options accordingly when the time limit (5 seconds) kicks in so the user can know the resulting list is *inaccurate*.
  - **TODO**: put Tooltips with state info as user help on the filter options.

* (4bbfd65e) bit of code cleanup

* (c1bb4f81)
  - BibTeX Sniffer: clean up search items results in better (human readable) search criteria for some PDFs where previously the words were separated by TAB but ended up as one long concatenated string of characters in Google Scholar Search.
  - HTTP/HTTPS web grab of PDF files: we don't care which TLS/SSL protocol is required, we should just grab the PDF and not bother. Some websites require TLS2 while today I ran into a website which requires old SSL (not TLS): make sure they're **all** turned ON.
  - Register the current library with the WebBrowserHostControl so that we don't have to go through obnoxious 'Pick A Library' dialog every time we hit the "Import all PDFs available on this page" button in the browser toolbar.
  - REVERT the DISabling of switching the active searcher: the behaviour was b0rked (I knew this) and before we go and fix https://github.com/jimmejardine/qiqqa-open-source/issues/46 we first restore the original behaviour (which was disabled in commit SHA-1: 0da072bc26bd5492a68dcb41a3a83e0b2acf7c00

* (e9831307) bit of debugger logging code cleanup

* (3b4ca9a4) fix crash in "grab/download all PDF files which are available on this page" webbrowser toolbar button functionality: the code crashed on relative URIs being fed into `new Uri(url)` code lines. Now the code copes correctly with both absolute and relative URIs and also corrupt/invalid URIs don't crash the grab-extractor code any more. Also improved the check for any URI found in the page being a PDF file a little: check for ".pdf" rather than "pdf": this will prevent us from trying not-a-pdf-file URIs such as "http://www.example.com/blog-about-pdf".

* (285456b2) further fiddling with the weird download issue reported in commit SHA-1: d39a2344be710a32a3b4698b5415c4398806b4de --> had a look if and how Chrome browser does it. It succeeds, with these headers:

  ```
  General:

  Request URL: https://ora.ox.ac.uk/objects/uuid:49e0183f-277e-486c-87bc-17097cbef0b3/download_file?file_format=pdf&safe_filename=fmcad2012.pdf&type_of_work=Conference+item
  Request Method: GET
  Status Code: 200 OK
  Remote Address: 127.0.0.1:8118
  Referrer Policy: no-referrer-when-downgrade

  Response Headers:

  Cache-Control: private
  Content-Disposition: inline; filename="fmcad2012.pdf"
  Content-Transfer-Encoding: binary
  Content-Type: application/pdf
  Date: Sat, 17 Aug 2019 23:06:36 GMT
  Referrer-Policy: strict-origin-when-cross-origin
  Server: Apache/2.4.34 (Red Hat)
  Status: 200 OK
  Strict-Transport-Security: max-age=15768000; includeSubDomains
  Transfer-Encoding: chunked
  X-Content-Type-Options: nosniff
  X-Download-Options: noopen
  X-Frame-Options: SAMEORIGIN
  X-Permitted-Cross-Domain-Policies: none
  X-Powered-By: Phusion Passenger 6.0.2
  X-Request-Id: 4f9a59d9-dcc9-49d1-8216-970461db251d
  X-Runtime: 0.063538
  X-XSS-Protection: 1; mode=block

  Request Headers:

  Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3
  Accept-Encoding: gzip, deflate, br
  Accept-Language: en-US,en;q=0.9,nl;q=0.8,de;q=0.7
  Cache-Control: no-cache
  Connection: keep-alive
  Host: ora.ox.ac.uk
  Pragma: no-cache
  Sec-Fetch-Mode: navigate
  Sec-Fetch-Site: none
  Sec-Fetch-User: ?1
  Upgrade-Insecure-Requests: 1
  User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36

  Query Strings:

  file_format: pdf
  safe_filename: fmcad2012.pdf
  type_of_work: Conference item

  ```

  ---

  * Trying to tackle a very weird PDF download problem, which doesn't go away.

  HTR:
  - Sniffer search on Scholar: "Deciding Floating point logic with systematic abstraction"
  - no click the first PDF link (ora.ox.ac.uk) to download PDF.

    This will silently FAIL! (same error as mentioned here: https://stackoverflow.com/questions/21728773/the-underlying-connection-was-closed-an-unexpected-error-occurred-on-a-receiv and here https://stackoverflow.com/questions/21481682/httpwebrequest-the-underlying-connection-was-closed-the-connection-was-closed )

    Link: https://ora.ox.ac.uk/objects/uuid:49e0183f-277e-486c-87bc-17097cbef0b3/download_file?file_format=pdf&safe_filename=fmcad2012.pdf&type_of_work=Conference+item

  - open the same search in "Google.com" tab: will show a PDF entry as a result at the down.
  - download that one without trouble.

    Link: http://www.cs.ox.ac.uk/people/leopold.haller/papers/fmcad2012.pdf

* (6ec1466a) fixing a couple of Exceptions thrown by the Visual Studio Designer due to missing `get`s...

* (7c1e4949) Trying to tackle a very weird PDF download problem, which doesn't go away.

  HTR:
  - Sniffer search on Scholar: "Deciding Floating point logic with systematic abstraction"
  - no click the first PDF link (ora.ox.ac.uk) to download PDF.

    This will silently FAIL! (same error as mentioned here: https://stackoverflow.com/questions/21728773/the-underlying-connection-was-closed-an-unexpected-error-occurred-on-a-receiv and here https://stackoverflow.com/questions/21481682/httpwebrequest-the-underlying-connection-was-closed-the-connection-was-closed )

    Link: https://ora.ox.ac.uk/objects/uuid:49e0183f-277e-486c-87bc-17097cbef0b3/download_file?file_format=pdf&safe_filename=fmcad2012.pdf&type_of_work=Conference+item

  - open the same search in "Google.com" tab: will show a PDF entry as a result at the down.
  - download that one without trouble.

    Link: http://www.cs.ox.ac.uk/people/leopold.haller/papers/fmcad2012.pdf

* (5f005770)
  - refactor: now StandardWindow will save (and restore on demand) the window size and location for any named window; th settings will be stored in the configuration file. SHOULD be backwards compatible. Further work on https://github.com/jimmejardine/qiqqa-open-source/issues/8
  - also fix the handling of the "Has OCR" checkbox: made it a proper tri-state. VERY SLOW to filter when ticked OFF or ON. (TODO: add a hack where we only allow it to impact the filtering for N seconds so as to limit the impact on UX performance-wise)

* (f31ebb04) bit of code cleanup

* (720caf21)
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/54 in GoogleBibTexSnifferControl
  - Gecko these days crashes on ContentDispositionXXXX member accesses: Exception thrown: 'System.Runtime.InteropServices.COMException' in Geckofx-Core.dll

    I'm not sure why; the only change I know of is an update of MSVS2019.  :-S

  - implement the logic for the BibTeXSniffer 'Has OCR' checkbox filter criterium. It's useful but the zillion file-accesses slow the response down too much to my taste.   :-S

* (6e8ab5da) sniffer: add filter check box to only show those PDF records which have been OCRed already. (The ones that aren't are pretty hard to sniff as you cannot mark any title text bits in them yet, for instance)

* (dc8586c2) augment debug logging for OCRengine

* (73e0d6e5) Removed all `[Obfuscation(Feature = "properties renaming")]` lines from the source code: those were needed when Qiqqa was commercial and the binaries were obfuscated to make reverse-engineering & cracking more difficult. Now they only clutter.

* (add86ca2) typo

* (4aa52b0f) added threadpool limits and heuristic stretching of the inter-batch sleep/delay in the FolderWatcher to allow the other background threads to process the imported PDF files; the more pending work from those other background tasks there is, the longer the sleep interval in the FolderWatcher to hand over CPU cycles to those other tasks (indexing, OCR, text extraction, metadata database flushing, ...)

* (3f1bbf28) quick fix for folder watcher going berzerk -- has to last until we refactor the async/sync PDF import code chunks. (see branch `refactoring_pdf_imports`)

* (46d5e4ef) provide code to set the threadpool/queue to a more or less sane amount of active threads.

* (5ecb1ca0) do NOT build the packager project when in Debug mode in the IDE.

* (c503e17d) Add debug logging re thread management. Threadpool on this machine has max thread count at 1000, which is way too high for my tastes...

* (70530e3c) clan up AppendStackTrace: don't list th stack lines which concern the call to AppendStackTrace itself as they don't add any information and merely clutter the output.

* (2c821579) picked up the saf and simple bits of refactor commit SHA-1: c315645d0bd2b69536030c55b1504a4303d59bce - a few locks added for thread safety and a few minimal code cleanups

* (b4ec8005) fix cherry-picked merge SHA-1: a026583c947a0ce18004582f4e581d535af57aad

* (0238a7e9) Part 1 of refactoring of the async/sync processing of PDFs being imported from various directions. Current code has some thread-unsafe practices, e.g. by passing references to List<T> instances across thread boundaries while the sender keeps working on the same set.

* (23c36449) removed unnecessary code: `ConvertTagBundleToTags` already produces a `HashSet`

* (b0b7e722)
  - 'integrate' nant build script for producing setup.exe installers into MSVS2019 solution by using a dummy project and prebuild script
  - added skeleton projects for qiqqa diagnostic and migration work. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/43

* (4e429f63) comment typo fixes

* (f5db12ba) debugging views: not all theme colours were correctly exported for Microsoft Blend / XAML Designer.

* (32414082) added debug logging in dirtree scanner code

* (5a643b76) collapse=false tweak; debugging views

* (50f1dd48) code cleanup + Stopwatch new + Start --> Stopwatch.StartNew()

* (ec06fdcb) fix for bug introduced by me while working on https://github.com/jimmejardine/qiqqa-open-source/issues/50 : on large dirtrees being watched, the time limit can be reached and is never reset ==> no more PDFs are added! **Whoops!** **BUG**

  Also: when importing PDFs from a watched directory the proper check to see if a PDF is already existing in the library is by checking its HASH, rather than the DownloadSource location, which can be **different** for identical files, or even **identical** when files have been patched in-line using tools such as QPDF (https://github.com/GerHobbelt/qiqqa-revengin)

* (428e54c9) remove Stopwatch class as it collides/duplicates standard lib code: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=netframework-4.8

* (3a2629f6) working on https://github.com/jimmejardine/qiqqa-open-source/issues/52 : FolderWatcher and PDF library import need rework / refactor as we either break the status feedback to user ("Adding document N of M...") as we loose the overall count of added documents, *or* we add/import PDF documents multiple times as we cannot destroy/Clear() the list since it is fed to the async function **as a reference**. :-(

  Meanwhile, we ensure that only ONE instance of the 'There a problem with some imports" AlertBox is shown at any time; multiple threads append to the report file which is then shown to the user, if (s)he desires so.

* (25f029f7) documented the most important refactored API. https://github.com/jimmejardine/qiqqa-open-source/issues/44

* (716d54dc) fix/tweak: just like with Sniffer AutoGuess, when a BibTeX record is picked from bibtexsearch using heuristics, it is now flagged in the bibtex with a special comment (`@COMMENT...`) which was already available before in the code but apparently disused or unused until today.

* (2eb1380c) refactored: Wasn't happy with the code flow in the FolderWatcher: now the long recursive directory scan (using `EnumerateFiles()`) is only aborted whn the app is terminated or when it has run its course (or when there are more or less dire circumstances); otherwise the dirtreescan is periodically paused to give the machine a bit of air to cope with the results and/or other pending work, while an app exit is very quickly discovered still (just like before, it is also detected inside the `daemon.Sleep(N)` calls in there, so we're good re that one. Tested it and works nicely. https://github.com/jimmejardine/qiqqa-open-source/issues/50

* (a7b2ca33)
  - fixed: https://github.com/jimmejardine/qiqqa-open-source/issues/53
  - bit of related code cleanup
  - added debug code (to be removed shortly)

* (c62329cb) fixed typo in tooltip: part of https://github.com/jimmejardine/qiqqa-open-source/issues/44

* (416fc56e) white space

* (a758fa16) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/31

* (d22e0032) just a few lines of code cleanup

* (7118c3ca) fix https://github.com/jimmejardine/qiqqa-open-source/issues/48 : Expedition: Refresh -> "Looking for new citations in ..." is not aborted when Qiqqa is closed.

* (14b96cb2) added debugging logging

* (34f74b1e) updated CHANGELOG_full.md

* (6533062a) additional debugging code and code formatting

* (1053fcea)
  - fixed https://github.com/jimmejardine/qiqqa-open-source/issues/50 using EnumerateFiles API instead of GetFiles. (TODO: inspect other sites in code where GetFiles is invoked)
  - introduced `Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown` as a global flag to check whether the app is terminating: this works the same way as `!thread.IsRunning` but without the need to memoize the thread reference or pass it via extra function arguments.
  - Addd a couple of extra `IsShuttingDown` checks to improve 'exit' termination speed of the application.

* (c1de3ca8) Added/tweaked debugging via logging: add check to logger to catch particular phrases so we can debugger-break on those and track down via call stack.

* (869dea1d) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/8: now stores WindowState as well and fetches non-maximized window size using RestoreBounds WPF API.

* (af2e32a5) Slighty more informative/easier to track back log lines for browsing web pages in Qiqqa

* (59eede74) fiddling with website theme...

* (f9918c89) fiddling with website theme...

  # Conflicts:
  #	docs/_config.yml

* (831edb37) (lint) `const`-ing a few variables, which really are constants

* (1b390f8f) Removing old debug lines that aren't required any more...

* (05bfb205) Prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/23 to make transition to latest LuceneNET easier to do.

* (46013825) Prepwork / minimal refactoring of `Object[]` to `PQRecord` done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.

* (12d6496e) fix compile error introduced with commit SHA-1: 453bd2e41b02f9609303c27f04a6392a127880eb * Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.

* (f069079c) fiddling with website theme...

* (44c1782c) fiddling with website theme...

* (85339e73) Merge remote-tracking branch 'remotes/GerHobbelt/master'

* (94aa67b3) Move the reference HTML file out of the way. https://github.com/jimmejardine/qiqqa-open-source/issues/38

* (5097f98b) Create README.md

* (0a28d983) Set theme jekyll-theme-tactile

* (4e8e48d5) Set theme jekyll-theme-minimal

* (ca0681f1) done: https://github.com/jimmejardine/qiqqa-open-source/issues/38 -- Part 14: added the images. Turns out there are two screens (screen0003.ai and screen0014.ai that remain unused. Looks like this was partly old stuff and maybe a chunk that still needs to go into a section of the manual. Haven't checked precisely as the job at hand was *conversion*.

* (e0a919e9) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 13: adding the images. First one. Let's check because I always screw this bit of MD notation up.  :-S

* (c91ce1e2) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 12: Done with the roundtrip check and HTML + MD cleanup. Now we only need to get back all the images in there...

* (d3cbbfab) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 13: adding the images. First one. Let's check because I always screw this bit of MD notation up.  :-S

* (cb736584) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 12: Done with the roundtrip check and HTML + MD cleanup. Now we only need to get back all the images in there...

* (d73d392c) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 11: Basically we're all good now. Tweaked the MD source to use modern MarkDown header marks. Found that we still need to correct lists in there as TurnDown didn't catch all the &middot encoded lists from Word, so that'll be next, together picking up the Unicode SmartQuotes from the RoundTrip copy so that the HTML file will serve as a reference for subsequent MarkDown source editing work...

* (bb4792be) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 10: Some swift manual edits on initial comparison: looks like everything made it, but there are notable differences. Patching the destination HTML and source MD MarkDown file to ensure the next round will correct these render mistakes...

* (05a654f5) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 9: Cleaning the source HTML using https://html-cleaner.com/ to kill the MSWord left-overs and then another round of https://htmlformatter.com/ for maximum similarity (and thus faster work in reviewing the diffs next)

* (96496b92) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 8: Same https://htmlformatter.com/ applied to source HTML

* (b7d61c88) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 7: A swift kill of all styling and Dillinger editor/line left-overs in the HTML: one regex replace in Sublime.

* (21bdf1d5) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 6: Applying https://htmlformatter.com/ to the Dillinger output

* (e9efd60a) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 5: RoundTripping the MarkDown to HTML (we need to check everything made it through and this is the quickest way for a large document like this: at the end waits a fast Beyond Compare session going through the diffs of the source and roundtrip HTML files...). Uses https://dillinger.io/

* (6e410f8c) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 4: Pulled the HTML through TurnDown ( http://domchristie.github.io/turndown/ ) to produce an initial MarkDown version of the documentation. Need to round-trip it to ensure we didn't loose any important chunks. :-)

* (2c99bc80) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 3: patching the HTML.

* (5b5039c4) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 2.

* (fc389597) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead.

* (540fe4db) prepwork done for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : extracted all displays from the pptx file (I've no problem using PowerPoint for stuff like this, but I have more control over publishing/output when using tools like Adobe Illustrator. Besides, the images that stick partly outside the PowerPoint pages are begging for clipping issues, etc. when rendering.

* (4c75cdb9) Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.

* (6e37242c) fix https://github.com/jimmejardine/qiqqa-open-source/issues/42: fixed crash.

* (0cb6b26e) more of the same as SHA-1: af670a88f8fb56d090ed8d04bfb9b08cb0e53b33 * minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball




2019-08-09
----------


* (af670a88) minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball (Fatal System Exception) on many XAML dialogs/panels in Qiqqa.

  The key to the latter is tweaked the dialog setup/init code such that XDevProc (which is part of Visual Studio) can execute that part of the Qiqqa code and thus render the panel -- or at least not b0rk and fail on it when you open the XAML file in the Designer.

  ---

  WARNING NOTE:

  TODO: The bit at the end is a copy/dump off SO where the same problem as https://github.com/jimmejardine/qiqqa-open-source/issues/40

  Must be tweaked and then retried with Qiqqa as it didn't fly for me yet. :-(

  The blunter hack of tweaked the Qiqqa code and sneakily support XDesProc code flow too
  by moving the Theme loader into Utilities.GUI is nice, but somehow still makes Designer
  blue-wave the Brushes and NOT render them.

  ---

  ```
  <UserControl.Resources>
      <ResourceDictionary>
          <ResourceDictionary.MergedDictionaries>
  			<ResourceDictionary Source="pack://application:,,,/Styles;component/Resources.xaml" />
          </ResourceDictionary.MergedDictionaries>
      </ResourceDictionary>
  </UserControl.Resources>
  ```

* (3d1b26ff) Minor UI tweak of BibTeX metadata pane in main UI: distance between buttons

* (348e58de) Tweak the AnnotationReportOptionsWindow XAML a little so it shows up in its entirety in de MSVS Designer.

* (be65d7de) Fiddling with the size of the proxy config dialog panel: make it wider so we can more easily enter&see host URI and user password.

* (8db7a3d7) tweak the About message: now also show the *full* build version next to the classic `vNN` version.

* (57b65887) Tweak the logging lines for the InCite webbrowser so its start and end can be easily detected in the logfiles.

* (d51b1196) removed unused variable assignment




2019-08-08
----------


* (d6906191) prep for https://github.com/jimmejardine/qiqqa-open-source/issues/38

* (a4efd50a) add 'how to build setup.exe' instructions to README.md

* (77502798) make Qiqqa main app and QiqqaOCR logging easily recognizable: `[Q]` or `[OCR]` tag per logline.
  Also print the QC-reported memory usage as a fixed-width number in MBytes




2019-08-07
----------


* (2236c9ad) updated CHANGELOG files

* (ede83a8e) ALPHA/TEST RELEASE v81 : version 81.0.7158.38371

* (e37448f2) log outgoing activity: posting BibTeX info to bibtexsearch.com aggregator

* (7bf0c72c) re-added to 'Add This PDF to Library' button in the browser; TODO: make it work akin to the <embed> handling to prevent confusion: when the browser shows a single PDF, it MAY be an <embed> web page and we should account for that!

* (efbdd0ce) IMPORTANT: this bad boy (an overzealous Dispose() which I introduced following up on the MSVS Code Analysis Reports) prevented Qiqqa from properly fetching and importing various PDFs from the Sniffer. (click on link would show the PDFs but not open them in Qiqqa nor import them into the Qiqqa library)

* (399b4c39) some titles/sentences seem to come with leading whitespace; title suggestion construction would produce suggested titles with leading and trailing whitespace. Fixed.

* (2b39e66d) #ifdef/#endif unused code

* (4c3b1ed0)
  - fix crash in PDF import when website/webserver does not provide a Content-Disposable HTTP response header
  - add ability to cope with <embed> PDF links, e.g. when a HTML page is shown with PDF embedded instead of the PDF itself
  - detect PDF files in URLs which have query parameters: '.pdf' is not always the end of the URL for downloading the filename

* (e702b0c7) revert/fix NANT build script to produce a `setup.exe` once again.

* (c11ff51a) added CHANGELOG (partly edited & full version using `git log`)

* (c0ed1329) moving some Info-level logging to Debug level as that's what it is, really. (Dispose activity tracking et al)

* (70f1a6a4) added TODO to remember my own DB ...

* (7c05d0fc) Whoops. Crash when quickly opening + closing + opening.... Sniffer windows: CLOSE != DISPOSE. Crash due to loss of search_options binding on second opening...

* (9a7e6208) Only when you play with it, you discover what works. The HasSourceURL/Local/Unsourced choices should be OR-ed together as that feels intuitive, while we also want to see 'sans PDF' entries as we can use the Sniffer to dig up the PDF on the IntarWebz if we're lucky. Meanwhile, 'invert' should clearly be positioned off to a corner to signify its purpose: inverting your selection set (while it should **probably** :thinking: have no effect if a specific document was specified by the user: then we're looking at a particular item PLUS maybe some other stuff?

* (0d263044) whoops. coding typo fix. https://github.com/jimmejardine/qiqqa-open-source/issues/28

* (4acc2532) Merge branch 'n29'

* (27179242) Merge branch 'work'

* (604a5db3) Sniffer Features:
  - add checkboxes to (sub)select documents which have a URL source registered with them or no source registered at all. (https://github.com/jimmejardine/qiqqa-open-source/issues/29)
  - add 'invert' logic for the library filter (https://github.com/jimmejardine/qiqqa-open-source/issues/30)

* (785c4879) fix https://github.com/jimmejardine/qiqqa-open-source/issues/28: turns out Qiqqa is feeding all the empty records to the PubMed-to-BibTex converter, which is throwing a tantrum. Improved checks and balances and all that. Jolly good, carry on, chaps. :-)

* (6283d897) work being done on https://github.com/jimmejardine/qiqqa-open-source/issues/29 + https://github.com/jimmejardine/qiqqa-open-source/issues/30: augmenting our Jolly Sniffer.

* (956e6930) added TODO's for a bit of ho-hum that's been bothering me all week. To Be Researched while other issues get attention first.

* (5ac9585f) report complete build version in logging. v80, v79, ...: it's not good enough when you want to track down the 'currentness' of the log :-)

* (e58df0ae) improving the logging while we hunt for the elusive Fail Creatures... (One of them being that OutOfMemoryException that somehow turns up out of the blue while the app still has plenty memory to go @ 200-300MB GC-reported allocation. :thinking:

* (9a604ae0) further work on CA2000 from the Code Analysis Report: apply `using (ISisposable) {...}` where possible.

* (cff8c623) refactoring event handling code (using null conditionals as suggested by MSVS Code Analysis Report)

* (a540e506) part of IDisposable cleanup work following the advice of the MSVS Code Analysis Report as much as possible (mostly the first bunch of CA2000 report lines) :

  * Message	IDE0067	Disposable object created by 'new FolderBrowserDialog()' is never disposed
  * Message	IDE0067	Disposable object created by 'new Tesseract()' is never disposed
  * Message	IDE0067	Disposable object created by 'out ms_image' is never disposed
  * Warning	CA1001	Implement IDisposable on 'BibTeXEditorControl' because it creates members of the following IDisposable types: 'WeakDependencyPropertyChangeNotifier'. If 'BibTeXEditorControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'CSLProcessorOutputConsumer' because it creates members of the following IDisposable types: 'GeckoWebBrowser'. If 'CSLProcessorOutputConsumer' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'FolderWatcher' because it creates members of the following IDisposable types: 'FileSystemWatcher'. If 'FolderWatcher' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'GoogleBibTexSnifferControl' because it creates members of the following IDisposable types: 'PDFRendererControl'. If 'GoogleBibTexSnifferControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'HtmlLexicalAnalyzer' because it creates members of the following IDisposable types: 'StringReader'.
  * Warning	CA1001	Implement IDisposable on 'Library' because it creates members of the following IDisposable types: 'LibraryIndex'. If 'Library' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'LibraryCatalogOverviewControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'LibraryCatalogOverviewControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'MainWindow' because it creates members of the following IDisposable types: 'StartPageControl'. If 'MainWindow' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'NotificationManager' because it creates members of the following IDisposable types: 'ReaderWriterLockSlim', 'AutoResetEvent'. If 'NotificationManager' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'PDFAnnotationNodeContentControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'PDFAnnotationNodeContentControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'PDFDocumentNodeContentControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'PDFDocumentNodeContentControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'PDFPrinterDocumentPaginator' because it creates members of the following IDisposable types: 'DocumentPage'.
  * Warning	CA1001	Implement IDisposable on 'ReadOutLoudManager' because it creates members of the following IDisposable types: 'SpeechSynthesizer'. If 'ReadOutLoudManager' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'TagEditorControl' because it creates members of the following IDisposable types: 'WeakDependencyPropertyChangeNotifier'. If 'TagEditorControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1001	Implement IDisposable on 'UrlDownloader.DownloadAsyncTracker' because it creates members of the following IDisposable types: 'UrlDownloader.WebClientWithCompression'. If 'UrlDownloader.DownloadAsyncTracker' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
  * Warning	CA1063	Modify 'AugmentedPdfLoadedDocument.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'AugmentedPdfLoadedDocument.~AugmentedPdfLoadedDocument()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'AugmentedPopupAutoCloser.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'AugmentedPopupAutoCloser.~AugmentedPopupAutoCloser()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'BrainstormControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'BrainstormControl.~BrainstormControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'ChatControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'ChatControl.~ChatControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'LibraryIndex.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'LibraryIndex.~LibraryIndex()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'LibraryIndexHoverPopup.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'LibraryIndexHoverPopup.~LibraryIndexHoverPopup()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'LuceneIndex.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'LuceneIndex.~LuceneIndex()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'PDFReadingControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'PDFReadingControl.~PDFReadingControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'PDFRendererControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'PDFRendererControl.~PDFRendererControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'ProcessOutputReader.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'ProcessOutputReader.~ProcessOutputReader()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'ReportViewerControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'ReportViewerControl.~ReportViewerControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'SceneRenderingControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'SceneRenderingControl.~SceneRenderingControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'SpeedReadControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'SpeedReadControl.~SpeedReadControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'StartPageControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'StartPageControl.~StartPageControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'StopWatch.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'StopWatch.~StopWatch()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'WebBrowserControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'WebBrowserControl.~WebBrowserControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA1063	Modify 'WebBrowserHostControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
  * Warning	CA1063	Modify 'WebBrowserHostControl.~WebBrowserHostControl()' so that it calls Dispose(false) and then returns.
  * Warning	CA2000	In method 'AssociatePDFWithVanillaReferenceWindow.CmdLocal_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new OpenFileDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'BibTexExport.Export(List<PDFDocument>)', call System.IDisposable.Dispose on object 'new SaveFileDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'BitmapImageTools.CropImageRegion(Image, double, double, double, double)', call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
  * Warning	CA2000	In method 'BitmapImageTools.FromImage(Image)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'BrowserStarter.OpenBrowser_REGISTRY(string)', call System.IDisposable.Dispose on object 'p' before all references to it are out of scope.
  * Warning	CA2000	In method 'ChartTools.renderNoDatasetMessage(Graphics)', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
  * Warning	CA2000	In method 'ClipboardTools.SetRtf(string)', call System.IDisposable.Dispose on object 'rich_text_box' before all references to it are out of scope.
  * Warning	CA2000	In method 'ConsoleRedirector.CaptureConsole()', call System.IDisposable.Dispose on object 'cr' before all references to it are out of scope.
  * Warning	CA2000	In method 'CSLEditorControl.OnBibliographyReady(CSLProcessorOutputConsumer)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'DocumentConversion.ConvertorDOC(string, string)', call System.IDisposable.Dispose on object 'converter' before all references to it are out of scope.
  * Warning	CA2000	In method 'DocumentConversion.ConvertorDOC(string, string)', call System.IDisposable.Dispose on object 'word_document' before all references to it are out of scope.
  * Warning	CA2000	In method 'ExportToWord.ExportToTextAndLaunch(PDFDocument)', call System.IDisposable.Dispose on object 'report_view_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'FolderWatcherChooser.CmdAddFolder_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'Fonts.getLargestFont(Graphics, string, double)', object 'font' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
  * Warning	CA2000	In method 'Ghostscript.RenderPage_AsMemoryStream(string, int, int, string, ProcessPriorityClass)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'GoogleAnalysicsSubmitter.Submit_BACKGROUND(Feature)', call System.IDisposable.Dispose on object 'wc' before all references to it are out of scope.
  * Warning	CA2000	In method 'GoogleScholarScraper.ScrapeUrl(IWebProxy, string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'GUITools.RenderToBitmapImage(UIElement)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'HtmlFromXamlConverter.ConvertXamlToHtml(string)', call System.IDisposable.Dispose on object 'new StringReader(xamlString)' before all references to it are out of scope.
  * Warning	CA2000	In method 'HtmlFromXamlConverter.ConvertXamlToHtml(string)', call System.IDisposable.Dispose on object 'new StringWriter(htmlStringBuilder)' before all references to it are out of scope.
  * Warning	CA2000	In method 'ImportFromFolder.FolderLocationButton_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'ImportFromThirdParty.GetFolderNameFromDialog(string, string)', call System.IDisposable.Dispose on object 'ofd' before all references to it are out of scope.
  * Warning	CA2000	In method 'IntranetLibraryChooserControl.ObjButtonFolderChoose_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'IPCServer.StartServerPump()', call System.IDisposable.Dispose on object 'npss' before all references to it are out of scope.
  * Warning	CA2000	In method 'LibraryBundleCreationControl.CmdCreateBundle_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'dialog' before all references to it are out of scope.
  * Warning	CA2000	In method 'LibraryControl.ButtonAddDocuments_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new OpenFileDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'LibraryExporter.Export(Library, List<PDFDocument>)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'LibraryIndexHoverPopup.DisplayThumbnail()', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_document.PDFRenderer.GetPageByHeightAsImage(this.page, (this.ImageThumbnail.Height / IMAGE_PERCENTAGE)))' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentPagesWithQuery(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentPagesWithQuery(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'new StreamReader(document_filename)' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsWithQuery(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsWithQuery(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsWithWord(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneIndex.GetDocumentsWithWord(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneMoreLikeThis.Like(FileInfo)', call System.IDisposable.Dispose on object 'new StreamReader(f.FullName, Encoding.Default)' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'Console.OpenStandardOutput()' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'r' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'searcher' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneMoreLikeThis.RetrieveTerms(int)', call System.IDisposable.Dispose on object 'new StreamReader(text[j])' before all references to it are out of scope.
  * Warning	CA2000	In method 'LuceneSimilarityQueries.FormSimilarQuery(string, Analyzer, string, Hashtable)', call System.IDisposable.Dispose on object 'new StringReader(body)' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OnShowTagOptionsComplete(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)', call System.IDisposable.Dispose on object 'report_view_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OpenDocument(PDFDocument, int?, string, bool)', object 'pdf_reading_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'pdf_reading_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OpenNewBrainstorm()', object 'brainstorm_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'brainstorm_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OpenSampleBrainstorm()', object 'brainstorm_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'brainstorm_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OpenSpeedRead()', object 'src' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'src' before all references to it are out of scope.
  * Warning	CA2000	In method 'MainWindowServiceDispatcher.OpenWebBrowser()', object 'web_browser_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'web_browser_control' before all references to it are out of scope.
  * Warning	CA2000	In method 'MultiChart2D.performPaintY1Axis(Graphics, ChartRegion, ChartRegion, Point2D, Point2D)', call System.IDisposable.Dispose on object 'string_format' before all references to it are out of scope.
  * Warning	CA2000	In method 'MultiChart2D.performPaintY2Axis(Graphics, ChartRegion, ChartRegion, Point2D, Point2D)', call System.IDisposable.Dispose on object 'string_format' before all references to it are out of scope.
  * Warning	CA2000	In method 'MuPDFRenderer.ReadEntireStandardOutput(string, ProcessPriorityClass)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'MYDDatabase.OpenMYDDatabase(string)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200F))' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFAnnotationToImageRenderer.RenderAnnotation(PDFDocument, PDFAnnotation, float)', call System.IDisposable.Dispose on object 'new MemoryStream(pdf_document.PDFRenderer.GetPageByDPIAsImage(pdf_annotation.Page, dpi))' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFCameraLayer.GetSnappedImage(Point, Point)', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_renderer_control_stats.pdf_document.PDFRenderer.GetPageByDPIAsImage(this.page, 150F))' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFInterceptor.Response(HttpChannel)', call System.IDisposable.Dispose on object 'stream_listener_tee' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFOverlayRenderer.RenderAnnotations(Image, PDFDocument, int, PDFAnnotation)', call System.IDisposable.Dispose on object 'highlight_pen' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFOverlayRenderer.RenderHighlights(Image, PDFDocument, int)', call System.IDisposable.Dispose on object 'image_attributes' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFOverlayRenderer.RenderHighlights(int, int, PDFDocument, int)', call System.IDisposable.Dispose on object 'highlight_pen' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFOverlayRenderer.RenderHighlights(int, int, PDFDocument, int)', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFPrinterDocumentPaginator.GetPage(int)', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_renderer.GetPageByDPIAsImage(page, 300F))' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, true, this.queue_lock)' before all references to it are out of scope.
  * Warning	CA2000	In method 'PkiEncryption.Decrypt(string, string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
  * Warning	CA2000	In method 'PkiEncryption.Encrypt(string, string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
  * Warning	CA2000	In method 'PkiEncryption.GenerateKeys(out string, out string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
  * Warning	CA2000	In method 'PNMLoader.CreateBitmapOffSize()', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
  * Warning	CA2000	In method 'PNMLoader.CreateGreyMap()', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
  * Warning	CA2000	In method 'PNMLoader.CreateGreyMapOffSize(byte[])', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
  * Warning	CA2000	In method 'PNMLoader.PNMLoader(string)', object 'stream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'stream' before all references to it are out of scope.
  * Warning	CA2000	In method 'ProcessSpawning.SpawnChildProcess(string, string, ProcessPriorityClass)', object 'process' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'process' before all references to it are out of scope.
  * Warning	CA2000	In method 'ReversibleEncryption.Decrypt(byte[])', object 'encryptedStream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'encryptedStream' before all references to it are out of scope.
  * Warning	CA2000	In method 'ReversibleEncryption.Encrypt(string)', object 'memoryStream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'memoryStream' before all references to it are out of scope.
  * Warning	CA2000	In method 'ReversibleEncryption.ReversibleEncryption()', call System.IDisposable.Dispose on object 'rm' before all references to it are out of scope.
  * Warning	CA2000	In method 'SerializeFile.ProtoSaveToByteArray<T>(T)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'SoraxPDFRendererDLLWrapper.GetPageByDPIAsImage_LOCK(SoraxPDFRendererDLLWrapper.HDOCWrapper, int, float)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamFingerprint.FromStream_DOTNET(Stream)', call System.IDisposable.Dispose on object 'sha1' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamFingerprint.FromText(string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamMD5.FromBytes(byte[])', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamMD5.FromStream(Stream)', call System.IDisposable.Dispose on object 'md5' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamMD5.FromText(string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'StreamToFile.CopyBufferToStream(Stream, byte[], int)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
  * Warning	CA2000	In method 'TopographicalChart.OnPaint(PaintEventArgs)', call System.IDisposable.Dispose on object 'brush' before all references to it are out of scope.
  * Warning	CA2000	In method 'TopographicalChart.OnPaint(PaintEventArgs)', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
  * Warning	CA2000	In method 'TopographicalChart.showForm()', call System.IDisposable.Dispose on object 'form' before all references to it are out of scope.
  * Warning	CA2000	In method 'TopographicalChart.showFormModal()', call System.IDisposable.Dispose on object 'form' before all references to it are out of scope.
  * Warning	CA2000	In method 'TweetControl.GenerateRtfCitationSnippet_OnBibliographyReady(CSLProcessorOutputConsumer)', call System.IDisposable.Dispose on object 'rich_text_box' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebBrowserHostControl.OpenNewWindow()', object 'wbc' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'wbc' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.GenericCustomiseChooser(string, string)', call System.IDisposable.Dispose on object 'dialog' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'image_attributes' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'mat' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'new MemoryStream(base.Current.pdf_document.PDFRenderer.GetPageByHeightAsImage(1, (WebLibraryDetailControl.PREVIEW_IMAGE_HEIGHT / WebLibraryDetailControl.PREVIEW_IMAGE_PERCENTAGE)))' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'new StringFormat()' before all references to it are out of scope.
  * Warning	CA2000	In method 'WebsiteAccess.DownloadFile(WebsiteAccess.OurSiteFileKind)', call System.IDisposable.Dispose on object 'web_client' before all references to it are out of scope.
  * Warning	CA2000	In method 'Word2007Export.Export(List<PDFDocument>)', call System.IDisposable.Dispose on object 'new SaveFileDialog()' before all references to it are out of scope.
  * Warning	CA2000	In method 'XMLTools.ToString(XmlDocument)', call System.IDisposable.Dispose on object 'sw' before all references to it are out of scope.
  * Warning	CA2202	Object 'app_key' can be disposed more than once in method 'UserRegistry.Read(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 39
  * Warning	CA2202	Object 'app_key' can be disposed more than once in method 'UserRegistry.Write(string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 28
  * Warning	CA2202	Object 'compressed_stream' can be disposed more than once in method 'PostcodeOutcodes.PostcodeOutcodes()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
  * Warning	CA2202	Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 125
  * Warning	CA2202	Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 148
  * Warning	CA2202	Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.ScrabbleWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 36
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'BibTeXImporter.BibTeXImporter(Library, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 37
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_SYNCHRONOUS(Library, object)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 285
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'LDASampler.FastLoad(string, int[][])'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 218
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'LDASampler.FastSave(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 176
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'UnhandledExceptionMessageBox.PopulateLog()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 213
  * Warning	CA2202	Object 'fs' can be disposed more than once in method 'VisualGalleryControl.ExportJpegImage(PdfDictionary, ref int)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 64
  * Warning	CA2202	Object 'memoryStream' can be disposed more than once in method 'ReversibleEncryption.Encrypt(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 95
  * Warning	CA2202	Object 'npss_in_callback' can be disposed more than once in method 'IPCServer.StartServerPump()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 66
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'CSLProcessorTranslator_AbbreviationsManager.LoadDefaultAbbreviations()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'PostcodeOutcodes.PostcodeOutcodes()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 125, 126
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 148, 149
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'ScrabbleWords.ScrabbleWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 36
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'SerializeFile.LoadCompressed(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 297
  * Warning	CA2202	Object 'stream' can be disposed more than once in method 'SerializeFile.SaveCompressed(string, object)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 286
  * Warning	CA2202	Object 'sub_app_key' can be disposed more than once in method 'UserRegistry.Read(string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 77
  * Warning	CA2202	Object 'sub_app_key' can be disposed more than once in method 'UserRegistry.Write(string, string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 63
  * Warning	CA2202	Object 'sw' can be disposed more than once in method 'PubMedXMLToBibTex.XMLNodeToPrettyString(XmlNode)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 119
  * Warning	CA2213	'ChatControl' contains field 'ChatControl.timer' that is of IDisposable type: 'Timer'. Change the Dispose method on 'ChatControl' to call Dispose or Close on this field.
  * Warning	CA2213	'LibraryIndex' contains field 'LibraryIndex.word_index_manager' that is of IDisposable type: 'LuceneIndex'. Change the Dispose method on 'LibraryIndex' to call Dispose or Close on this field.
  * Warning	CA2213	'PDFReadingControl' contains field 'PDFReadingControl.pdf_renderer_control' that is of IDisposable type: 'PDFRendererControl'. Change the Dispose method on 'PDFReadingControl' to call Dispose or Close on this field.
  * Warning	CA2213	'WebBrowserHostControl' contains field 'WebBrowserHostControl.wbc_browsing' that is of IDisposable type: 'WebBrowserControl'. Change the Dispose method on 'WebBrowserHostControl' to call Dispose or Close on this field.

* (a80be7d7) done work on https://github.com/jimmejardine/qiqqa-open-source/issues/27 and on the lockups of Qiqqa (some critical sections in there were **humongous** in both code side and run-time duration; now the number of lock-ups due to *very* slow loading PDFs coming in from the Qiqqa Sniffer should be quite reduced: work related to https://github.com/jimmejardine/qiqqa-open-source/issues/18

* (409d5124) better handling of 'unreachable code' warnings

* (37c8b2cd) Code looks like a swap/insert code pattern gone bad: did we find a bug?  Found due to MSVS Code Analysis Message	IDE0059	Unnecessary assignment of a value to 'pageranks_temp'




2019-08-06
----------


* (c48bca95) several like this: Warning	CA1500	'filter_terms', a parameter declared in 'LibraryCatalogControl.OnFilterChanged(LibraryFilterControl, List<PDFDocument>, Span, string, Dictionary<string, double>, PDFDocument)', has the same name as an instance field on the type. Change the name of one of these items.

* (c2d3a639) we also don't care (yet) to validate arguments coming into public methods and neither are we interested to hear about methods which might be better off as properties' getter/setter.

* (67942dbb) don't care about public nested visibility issues and 'can be simplified' `new` object instantiations, which merely introduce a newer C# language feature that I don't particularly like (contrary to null conditionals ;-) )

* (63b00f7c) patched the Code Analysis ruleset for now: don't care about internationalization, app-specific exceptions and the lot. Once we're good and super-pedantic, you can go and kill those disabled entries and have a ball with 10K+ issues. Now, the count is down to a mere 2449 items, and that's after the initial set of patches (see commits today)

* (abdac36d) MSVS Code Analysis: took a copy of the 'All Rules' ruleset and copied it to Qiqqa, assigned all projects to use the new ruleset for Code Analysis and then disabled a few items in there to cut down on the number of (unwanted) warnings (before MSVS spit out 10K+ warnings).

  Incidentally, this also kills the build warning about the missing AllRules.ruleset in the Qiqqa repo root. :-)

* (0cb9c36f) fix build error + `const`: both are edits triggered by MSVS Code Analysis Reporting, where the first one was a mis-edit (Exception -> CmdLineException: not good! :-) )

* (2d51712e) addressing https://github.com/jimmejardine/qiqqa-open-source/issues/26 : nuked the Utilities/GUI/BrainStorm copy and copied/commented all diffs into the Qiqqa source tree: every diff edit references the issue https://github.com/jimmejardine/qiqqa-open-source/issues/26 in the comments.     `Utilities/GUI/BrainStorm/` === `Qiqqa/BrainStorm/`

* (041b1609) NOT FIXED:
  + Warning	CA1801	Parameter 'node_control' of 'EllipseNodeContentControl.EllipseNodeContentControl(NodeControl, EllipseNodeContent)' is never used. Remove the parameter or use it in the method body.
  + Warning	CA1823	It appears that field 'EllipseNodeContentControl.circle_node_content' is never used or is only ever assigned to. Use this field or remove it.
  FIXED:
  + Warning	CA1802	Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.

* (e494e597) Warning	CA1802	Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.

* (9f640a62) Warning	CA1704	Correct the spelling of 'Unkown' in member name 'DragDropManager.DumpUnkownDropTypes(DragEventArgs)' or remove it entirely if it represents any sort of Hungarian notation.

* (76c47e77) Warning	CA1500	'node_from', a parameter declared in 'ConnectorControl.SetNodes(NodeControl, NodeControl)', has the same name as an instance field on the type. Change the name of one of these items.

  NOTE: code inspection has led me to change the code in this way that the events registered with the old nodes are UNregistered before the new nodes are assigned and events are REGISTERED with them. The old code was ambiguous, at least for me (human); I'm not entirely sure what the compiler had made of that. HMmm...

  Given these next two MSVS Code Analysis report Warnings, I guess that was a couple of lurking bugs right there:

  + Warning	CA1062	In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_from' before using it.
  + Warning	CA1062	In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_to' before using it.

* (39f50e05) updated links/refs in README.md

* (99bb1817)
  + Warning	CA1715	Prefix interface name 'RecurrentNodeContent' with 'I'.
  + Warning	CA1715	Prefix interface name 'Searchable' with 'I'.
  + Warning	CA1715	Prefix interface name 'Selectable' with 'I'.

* (c186a6ba) Merge branch 'memleak-hunting'

  # Conflicts:
  #	Qiqqa/AnnotationsReportBuilding/ReportViewerControl.xaml.cs

* (44e15752) working on using a more developed build versioning approach. Have MSVS produce a unique version for each build, then (FUTURE WORK) add tooling to ensure all files carry the updated version number(s).

* (6e55393f) tweak Logging class: remove unused methods and group all Warn and Error methods next to one another for an easier overview of the available overloads.

* (84fe992e) feature added: store the source URL (!yay!) of any grabbed/sniffed PDF. Previously the source path of locally imported (via WatchFolder) PDFs was memorized in the Qiqqa database. It adds great value (to me at least) when Qiqqa can also store the source URL for any document -- this is very handy information to have as part of augmented document references!)

  This commit includes a lingering part of the memleak hunt refactor activity listed in the previous commit SHA-1: 177a2be0cff4e92a9ae285c61c2377bac1cbf1c4 as that code exists in the same source files and the activities were developed at the same time.

* (177a2be0) As we've been hunting hard to diagnose memory leaks which made working with Qiqqa short-lived (OutOfMemory in about 15 minutes on a large 20K+ Library), we have applied best practices coding to all `IDisposible`-derived classes and augmented all `Dispose()` methods to ensure a fast and easy GC action by unlinking/detaching any referenced objects/instances ASAP. We are still in the process of going through the MSVS2019 Code Analysis Report that we've run at the end of the memleak hunting session, which took place in the last 245 hours.

  Best practices are gleaned from https://docs.microsoft.com/en-us/dotnet/api/system.object.finalize?view=netframework-4.8 and the Dispose+Dispose(true/false) coding pattern described there is applied everywhere where applicable.

  Also note that we employ the `?.` **null conditional operator**, which is part of C# 6.0 and described here: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
  It turns out the NANT build doesn't cope with this but I'm loath to revert to antiquity there, so the NANT build process has become a little hacky as MSVS2019 (IDE) can build (and debug) the Qiqqa binaries without a fuss, so we now use that one to build the binaries and the NANT build script for packaging (creating the `setup.exe`).

* (53c97510) Warning	CA1812	'BackingUp' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.

* (9b6a61e1) Warning	CA2000	In method 'ReportViewerControl.ButtonToPDF_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'doc' before all references to it are out of scope.

* (7aa66dd0)
  + Save To PDF had been disabled in the original Qiqqa source code. No idea why. Re-enabling it so that 'Save To PDF' is not a NIL activity anymore.
  + Warning	CA1811	'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
  + Warning	CA1811	'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.

* (602a1ebb)
  + Save To PDF had been disabled in the original Qiqqa source code. No idea why. Re-enabling it so that 'Save To PDF' is not a NIL activity anymore.
  + Warning	CA1811	'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
  + Warning	CA1811	'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.

* (a42a79b9)
  + Warning	CA1804	'RegionOfInterest.IsCloseTo(RegionOfInterest)' declares a variable, 'horizontal_distance', of type 'double', which is never used or is only assigned to. Use this variable or remove it.
  + Warning	CA1802	Field 'RegionOfInterest.PROXIMITY_MARGIN' is declared as 'static readonly' but is initialized with a constant value '0.0333333333333333'. Mark this field as 'const' instead.

* (80cc9b1b) Warning	CA1812	'LinkedDocsAnnotationReportBuilder' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.

* (45e6d8f5) Warning	CA1053	Because type 'LegacyAnnotationConvertor' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.

* (17a97f81)
  + Warning	CA1812	'InkToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  + Warning	CA1802	Field 'InkToAnnotationGenerator.INKS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Inks*'. Mark this field as 'const' instead.

* (9c925bde)
  + Warning	CA1812	'HighlightToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  + Warning	CA1802	Field 'HighlightToAnnotationGenerator.HIGHLIGHTS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Highlights*'. Mark this field as 'const' instead.

* (dbf9173c) Warning	CA1811	'AsyncAnnotationReportBuilder.OnShowTagOptionsComplete(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' appears to have no upstream public or protected callers.

* (25a337b0) Warning	CA1804	'AsyncAnnotationReportBuilder.BuildReport(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' declares a variable, 'underline', of type 'Underline', which is never used or is only assigned to. Use this variable or remove it.

* (1025b6b1) Warning	CA1053	Because type 'AsyncAnnotationReportBuilder' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.

* (490c7a46) Warning	CA1053	Because type 'AsyncAnnotationReportBuilder' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.

* (2032827a)
  + Warning	CA1500	'OnShowTagOptionsComplete', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
  + Warning	CA1500	'library', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
  + Warning	CA1500	'pdf_documents', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.

* (59eab8ae) Warning	CA1822	The 'this' parameter (or 'Me' in Visual Basic) of 'WordListCredibility.HasSufficientRepeatedWords(WordList)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.

* (f40f507f) Warning	CA1802	Field 'WordListCredibility.REASONABLE_WORD_LIST_LENGTH' is declared as 'static readonly' but is initialized with a constant value '10'. Mark this field as 'const' instead.

* (40a3ce72) Warning	CA2204	Correct the spelling of the unrecognized token 'exst' in the literal '"\' does not exst"'.

* (23f5ca6d) Warning	CA1812	'TextExtractEngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.

* (9e9490af) Warning	CA1822	The 'this' parameter (or 'Me' in Visual Basic) of 'RegionExtractorTestWindow.ChooseBrush(PDFRegionLocator.Region)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.

* (1bfee58f) Warning	CA1811	'PDFRegionLocator.GetRegions_FULLPAGE(Bitmap, out List<PDFRegionLocator.Region>, out int)' appears to have no upstream public or protected callers.

* (ff0a1812) Warning	CA2000	In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'ocr' before all references to it are out of scope.

* (8f128987) Warning	CA2000	In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200F))' before all references to it are out of scope.

* (e6531a75) Warning	CA2204	Correct the spelling of the unrecognized token 'exst' in the literal '"\' does not exst"'.

* (bf35cc88) Warning	CA1801	Parameter 'no_kill' of 'OCREngine.MainEntry(string[], bool)' is never used. Remove the parameter or use it in the method body.

  Synced `no_kill` code with the code in TestExtractEngine.cs

* (abfe48c9) Warning	CA1812	'OCREngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.

* (f7f62b1d)
  + MSVS Code Analysis: Warning CA1053  Because type 'Backgrounds' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.icons
  + MSVS Code Analysis: Warning CA2211  Consider making 'Icons.QiqqaMedal' non-public or a constant.

* (aac705aa) !@#$%^&* NANT doesn't support C#6.0 language constructs. Spent a few hours on NANT (obtained from GitHub) but cannot get it to work right now. Not spending more time on that distraction: patched the NANT build process to use the binaries which are (correctly and without hassle) produced by MSVC2019 IDE (Batch Build all or Rebuild All, Release target). See short notes in NANT build file and bash shell driver script too. QUICK HACK. QUICK HACK. QUICK HACK. Anyone with the time on their hands and the inclination to make NANT work with a *recent* MSVC setup: more power to you. Right now, this is a bleeding edge NANT rebuild PLUS patched build/packaging scripts.




2019-08-05
----------


* (d58bd7ae) revert debug code that was part of commit SHA-1: 89307edfe7d5ba2b6de050de969d2910b147e682 -- some invalid BibTeX was crashing the Lucene indexer (`AddDocumentMetadata_BibTex()` would b0rk on a NULL `Key`)

  That problem was fixed in that commit at a higher level (in PDFDocument)

* (9ec342cd) Don't let illegal multiple BibTeX entries for a single PDF record slip through unnoticed: one PDF having multiple BibTeX records should be noticed as a WARNING at least: the 'official' (i.e. *first*) BibTeX record is augmented with a Warning mentioning the multiplicity of the BibTeX for that PDF record.

  I had one example of this in my library where some slightly disparate BibeX records were copy-pasted into the BibTeX edit box for an obscure PDF: that had been meant "to be further investigated later" but that never happened and now showed up as a 'silent error' while checking Qiqqa library integrity.

* (89307edf) some invalid BibTeX was crashing the Lucene indexer (`AddDocumentMetadata_BibTex()` would b0rk on a NULL `Key`)

  Sample invalid BibTeX:

  ```
  @empty = delete?
  ```

* (dc740d77) fix/tweak FolderWatcher background task: make sure we AT LEAST process ONE(1) tiny batch of PDF files when there are any to process.

* (928d55b9) trying to tackle the slow memory leak that's happening while Qiqqa is running  :-((   This is going on for a while now; can't seem to spot the culprit though.  :-((

* (ca89ba62) added misc files to the solution/projects: license, readme and copyright files.

* (5a518ebd) mention the new `build-installer.sh` bash shell script as equivalent of the old `go.bat` in the README.

* (f5b80fa2) little tug & tweak of the build `bash` shell script.

* (542fc818) fix msbuild error (oddly enough this was okay in MSVC2019 and compiled fine in the ID, but the NANT task via `./build-installer.sh` fails.  :-S  )

* (6dcc9717) DBExplorer severely enhanced:
  - now supports wildcards in query parameters (% and _, but also * and ?, which are the MSDOS wildcards which translate directly to the SQL wildcards)
  - now supports GETting multiple records.
  - when GETting multiple records, DBExplorer not only prints the BibTeX for each record, but also the identifying fingerprint, verification MD5 and most importantly: the *PARSED* BibTeX (iff available) and BibTeX parse error diagnostics report.
  - when GETting multiple records, the DBExplorer output is also dumped to the file `Qiqqa.DBexplorer.QueryDump.txt` in the Qiqqa Library base directory. A previous DBExplorer query report dump will be REPLACED.
  - an extra input field has been added which allows the user to specify a maximum number of records to fetch: this speeds up queries and their reporting when working on large libraries with query criterai which would produce 1000nds of records if left unchecked.

  This allows to use the DBExplorer as a rough diagnostics tool to check the library internals, including a way to find erroneous/offending BibTeX entries which may cause havoc in Qiqqa elsewhere.




2019-08-04
----------


* (9c85acc3) See also commit SHA-1: b38123a4ea67b4f3581826aeeac44a4ee0e9e39e: we now have Qiqqa open a VERY FAST and LEAN web page when we 'Open Browser'.

* (8176c4eb) fix compiler warning due to unused variable.

* (0b7d3b46) fix/tweak: do NOT report 'Adds 0 of 0 document(s)' but clear the status part instead: now that we make Qiqqa work in small batches, this sort of thing MAY happen. (TODO: review WHY the Length of the todo array is actually ZERO, but low priority as things work and don't b0rk)

* (b743d721) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/8: not only storing Left/Top coordinate, but also Width+Height of the Qiqqa.exe window

* (d59d6f08) fix crash in chat code when Qiqqa is shutting down (+ code review to uncover more spots where this might be happening)

  ```
  20190804.204351 INFO  [Main] Stopping MaintainableManager
  Exception thrown: 'System.NullReferenceException' in Qiqqa.exe
  20190804.204351 WARN  [9] There was a problem communicating with chat.
  System.NullReferenceException: Object reference not set to an instance of an object.
     at Qiqqa.Chat.ChatControl.ProcessDisplayResponse(MemoryStream ms) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 221
     at Qiqqa.Chat.ChatControl.PerformRequest(String url) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 127
  20190804.204351 WARN  [9] Chat: detected Qiqqa shutting down.
  ```

* (5a84d1be) I'm using `bash` rather than `cmd` as it comes with Git For Windows: provide a setup build script which you can invoke from the root from the project so you can work with git easily while also running this build command (and a few other things)

* (6febb358) Since ExpeditionManager is the biggest OutOfMemory troublemaker (when loading a saved session :-( ), we're augmenting the logging a tad to ease diagnosis. (https://github.com/jimmejardine/qiqqa-open-source/issues/19)

* (eab712b0) debugging: uncollapsing rollups in dialog windows as part of a longer debugging activity. MUST REVERT!

* (170a4ca4) augment BibTeX documentation: add URLs and note an old one as inactive (that webpage has disappeared from the Intarwebz. RIP.)

* (b38123a4) 'Open New Browser' was looking pretty weird due to a website/page being  loaded which was unresponsive; now we're pointing to a more readily available webpage instead. (Though in my opinion 'Open Browser' should load a VERY MINIMAL webpage, which has absolutely *minimal* content...) Referenced URL has already been set up as part of commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505

* (02d2aa72) Mention the new CSL (Citation Styles) source websites in the credits. The links referenced in this text have already been set up as part of a previous commit in `WebsiteAccess` class. (commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505)

* (c8c2fd85) typo fix in comments

* (bab04996) code stability: Do not crash/fail when the historical progress file is damaged

* (820d8335) refactor: collect (almost!) all URLs and keep them in WebsiteAccess so we have a single place where we need to go to update URLs. (In actual practice, there remain a FEW places where URLs stay; the number of files carrying URLs is significantly reduced anyway...)

* (866b15a5) moving Sample.bib to be with the other TEST input files




2019-08-03
----------


* (2fc66dd0) The easy bit of https://github.com/jimmejardine/qiqqa-open-source/issues/3: synced the Qiqqa/InCite/styles/ directory with the bleeding edge of the CSL repo at https://github.com/citation-style-language/styles (Note the 'bleeding edge' in there: I didn't use https://github.com/citation-style-language/styles-distribution !). DO NOTE that Qiqqa had several CSL style definitions which don't exist in this repository: these have been kept as-is.

* (37c1ae0f) cut out all test code chunks using `#region Test` + `#if TEST ... #endif` around those chunks: this way, the test code will still exist ("just in case...") while it won't burden the compiler and never be included in a Qiqqa binary unless you *specifically* instruct the compiler to do so by `-define TEST` in the compiler options.

* (6128cfc1) Flag all [Obsolete] entries as triggering a compiler error when still in use. Some class properties have been flagged in the comments as required for backwards compatibility of the serialization (reading & writing) of the configuration and BrainStorm files, so we added a `Obsolete(...)` report message accordingly.

  This change SHOULD have no effect on the build/code flow but 'cleans up' by making the use of these obsolete bits an ERROR instead of a WARNING.

  See also https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/common-attributes#Obsolete

* (7af6513e) pick up the language from the dialog where the same is said as in the text file (though the rest of the wording is slightly different: the InCiteControl dialog content and this README do NOT have identical content!)

* (32dd1c3c) whitespace police raid




2019-08-02
----------


* (95043ea7) correct code and explain why for https://github.com/jimmejardine/qiqqa-open-source/issues/20

* (da3f8531) corrected Folder Watch loop + checks for https://github.com/jimmejardine/qiqqa-open-source/issues/20: the intent here is very similar to the code done previously for https://github.com/jimmejardine/qiqqa-open-source/issues/17; we just want to add a tiny batch of PDF files from the Watch folder, irrespective of the amount of files waiting there to be added.

* (bd656807) HACKY trial to catch and cope with OutOfMemory errors due to the LDAStuff etc.: https://github.com/jimmejardine/qiqqa-open-source/issues/19

* (7bd3ee66) more work regarding https://github.com/jimmejardine/qiqqa-open-source/issues/10 and https://github.com/jimmejardine/qiqqa-open-source/issues/17: when you choose to either import a large number of PDF files at once via the Watch Folder feature *or* have just reset the Watch Directory before exiting Qiqqa, you'll otherwise end up with a long running process where many/all files in the Watched Directories are inspected and possibly imported: this is undesirable when the user has decided Qiqqa should terminate (by clicking close-window or Alt-F4 keyboard shortcut).

* (e83b4df7) attempt to cope with https://github.com/jimmejardine/qiqqa-open-source/issues/19 a little better than a chain of internal failures. :-(

* (53f2ca86) code cleanup activity (which happened while going through the code for thread safely locks inspection)

* (8b2b3de3) https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 2, looking for thread safety locks being applied correctly and completely. Also contains a few lines from work done before related to https://github.com/jimmejardine/qiqqa-open-source/issues/10 et al.

* (5dcda970) https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 1, looking for thread safety locks being applied correctly and completely: for example, a few places did not follow best practices by using the dissuaded `lock(this){...}` idiom (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement)

* (8a1d7660) Fix https://github.com/jimmejardine/qiqqa-open-source/issues/17 by processing PDFs in any Qiqqa library in *small* batches so that Qiqqa is not unreponsive for a loooooooooooooong time when it is re-indexing/upgrading/whatever a *large* library, e.g. 20K+ PDF files. The key here is to make the '**infrequent background task**' produce *some* result quickly (like a working, yet incomplete, Lucene search index DB!) and then *updating*/*augmenting* that result as time goes by. This way, we can recover a search index for larger Qiqqa libraries!

* (98083f86) fixing MSVC2019-reported 'unreachable code' compiler reports by minimal refactoring

* (67e9efdf) commenting out a property which was reported as 'unused' and wasn't flagged as `[Obsolete]` for serialization either... (compare to Qiqqa/Brainstorm/Nodes/NodeControlSceneData.cs where this attribute is used)

* (72b8d257) dialing up the debug/info logging to help me find the most annoying bugs, first of them: https://github.com/jimmejardine/qiqqa-open-source/issues/10, then https://github.com/jimmejardine/qiqqa-open-source/issues/13

* (12367900) Do NOT nuke the previous run's `Qiqqa.log` file in `C:\Users\<YourName>\AppData\Local\Quantisle\Qiqqa\Logs\`: **quick hack** to add a timestamp to the qiqqa log file so we'll be quickly able to inspect logs from multiple sessions. **Warning**: this MUST NOT be present in any future production version or you'll kill users by log file collection buildup on the install drive!

* (277c3925) simple stuff: updating copyright notices from 2016 to 2019 and bumping the version from 80 to 81.  **Note**: The changelog in ClientVersion.xml runs ahead of what you will observe in these commits: I did the work first, then did these commits to lock it in as only now I am confident that my attempt at https://github.com/jimmejardine/qiqqa-open-source/issues/14 actually delivers a working exe that's equal or better than last commercial release v79 (for me at least; caveat emptor!)

* (b3590395) update existing Syncfusion files from v14 to v17, which helps resolve https://github.com/jimmejardine/qiqqa-open-source/issues/11

  **Warning**: I got those files by copying a Syncfusion install directory into qiqqa::/libs/ and overwriting existing files. v17 has a few more files, but those seem not to be required/used by Qiqqa, as **only overwriting what was already there** in the **Qiqqa** install directory seems to deliver a working Qiqqa tool. :phew:

* (142e06dd)
  - point Microsoft Visual Studio project files to the (renamed) README.md files following https://github.com/jimmejardine/qiqqa-open-source/pull/6
  - remove Syncfusion LICX file as we migrate from SyncFusion 14 to Syncfusion 17; see also bug report https://github.com/jimmejardine/qiqqa-open-source/issues/11

* (b00c7222) updating 7zip to latest release




2019-07-30
----------


* (4c2de1fe) Merge remote-tracking branch 'remotes/jimmejardine-original/master'




2019-07-15
----------


* (db3c1184) Merge branch 'master' of https://github.com/jimmejardine/qiqqa-open-source

* (77469ba1) added first build result as some people wish to download it

* (28c0f3f5) Merge pull request #6 from GerHobbelt/readme-cleanup-for-github

  cleaned the README files a bit and made them ready for GitHub

* (90be5416) Update README.md

* (5db5b2e7) Update README.md




2019-07-14
----------


* (3e4d5015) Merge branch 'readme-cleanup-for-github'

* (4629eee7) cleaned the README files a bit and made them ready for GitHub (MarkDown formatting)

* (51214fee) Update README.md

* (26d171ad) README: tables fixed for GFM

* (4dc438c8) Update README.md

* (a8f762dc) tweaking the README tables for GitHub to render them correctly.

* (874e6001) fix README rendering on GitHub

* (5f23d01c) cleaned the README files a bit and made them ready for GitHub ( MarkDown formatting )

* (de85ee24) Rename the `~readme.txt` files describing directory content to `README.md` so that GitHub picks them up and displays them in the web view of the repo.

* (d05e4605) Update README.md




2019-07-03
----------


* (4ad8e5f0) open source version is now able to 'see' the legacy web libraries

* (4b2ea5e1) initial contribution

* (dd17b0a2) Update README.md

* (60199c03) Create README.md

