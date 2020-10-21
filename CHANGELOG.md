
# v82pre release: v82.0.7568-29227

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.






2020-10-21
----------


* (b74f18ff) prevent superfluous fetch of illegal URI 'about:blank' when the Sniffer dialog window is opened.

* (612bc454)..(d127ba28) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/253 : this uncovered a deadlock situation due to pdf_documents_lock and access_lock interplay via the Associate...() call going directly to the PDFDocument (hence access_lock) while one of the background threads was fetching a list of documents to inspect via Library (hence pdf_documents_lock -> access_lock for each doc): the deadlock occurred because the Associate..() call internally would *add* the new PDF into the Library (hence: pdf_ddocuments_lock inside an access_lock zone, hence DEADLOCK with the bg thread!)

  Also note that this issue uncovered another matter: PDF association with a Vanilla Reference was not working AT ALL: fixed that as well. (WARNING: association via 'FromWeb' a.k.a. SearchWeb will not deliver AFAICT: only FromLocal associations will deliver.

  This uncovered yet another bug, which involved the metadata copying code (which still has some TODO's to be addressed at a later time!) resulting in the associated PDF then being marked as a vanilla reference *itself* due to overzealous metadata copying, which includes the 'FileType' field: 'pdf' or 'vanilla_reference'. Fixed as well.

* (ca328e0e) improved application shutdown behaviour




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


* (0998a2fe) provisional fix for https://github.com/jimmejardine/qiqqa-open-source/issues/240 : do not quit/abort/terminate the application when the unhandled exception is of a certain type/message and/or comes from the threadpool instead of the UI thread itself.

* (cfbc9c34)..(42fab986) added GoogleScholar_DoExtraBackgroundQueries configuration option (OFF by default) which controls whether vQiqqa will scrape Google Scholar in the background while you view/read a PDF document. Killing this behaviour (which only renders results in the sidebar of the PDF reader under the Google Scholar section there: articles of interest to view next) ensures your https://github.com/jimmejardine/qiqqa-open-source/issues/225 RECAPTCHAs will take that while longer to appear as Google counts your number of Scholar site visits before it fires that one off.

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





2020-08-21
----------


* (df2618a4) added doc about problems users may run in when downloading the installer off the net. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/223

* (f1ec34b4)..(e2148680) writing the qiqqa download + install procedure for users as per https://github.com/jimmejardine/qiqqa-open-source/issues/228 request. HTH.





2020-08-16
----------


* (581ffaa6) getsatisfaction was totally nuked. As an earlier site mirror run got b0rked, all that was left was grabbing what reamined in the search engine caches (used the BING 'cached' option). HTML pages have been roughly cleaned after the fact. GS pages are a mess, though. :-(    'fixes' https://github.com/jimmejardine/qiqqa-open-source/issues/218





2020-08-11
----------


* (8b0c9d5c)..(1fad253a) working on the docs





2020-04-27
----------


* (413768db) Merge remote-tracking branch 'remotes/mahfiaz/master' into documentation

  Thanks @mahfiaz!

* (f01f3230) Remove chat features, communication should be on homepage.

* (5d350fcd) Remove splashscreen completely.

* (49166d2f) Fix strange empty space in PDF view when PDF is available.

* (8d30d885) Revert "Remove nagging to vote on alternative.to"

  This reverts commit ad26f4f5fdef4649c0e337fd7c29ff436845caf0.

* (3e63cf12) Fixed unintentional Alt+W shortcut created in earlier commit.

* (010c04cb) Changed configuration page layout. No nested collapsible frames.





2020-04-26
----------


* (140f154d) Move citation button one place to left.

* (1280561c) Remove chat features, communication should be on homepage.

* (006f3511) Change order of left sidebar elements in PDF viewer, group by topics.

* (ad26f4f5) Remove nagging to vote on alternative.to

* (f21f5383) Deleted lots of unused icons, removed unused references.

* (ada53fac) Add Ctrl+W change to another similarly used place.





2020-04-25
----------


* (9334b883) Fix strange empty space in PDF view when PDF is available.

* (26e69147) Remove Tweeting button.





2020-04-01
----------


* (c51f31bd) Slightly improve the SORAX library error logging for when the error was due to a missing/inaccessible file: prevent an obscure SORAX log message.

* (323f25ca) get rid of the last lingering commercial-specific Qiqqa bits: hacker warnings.

* (a061a94d) Logging output fix: only output QiqqaOCR output at ERROR level when there actually has been an error reported, either as return value or in the logging itself. Otherwise just log any successful output log lines at Info level: this should prevent cluttering the Errors log files with successful PDF processing actions and thus make it easier and swifter to go through the error set in the logs.

* (aa4af8ec) Google Scholar Scraper: (1) augmented the code to recognize particular GS output and (2) only execute the background scraper when the feature is enabled in the dev-settings (ON by default)

* (892f2e8c) fiddling with the logging settings: make sure all Qiqqa tools output their log files into the Quantisle AppData log directory for easier access and retrieval.





2020-03-30
----------


* (f081962f) Add Ctrl+W shortcut - close tab

  Adding Ctrl+W shortcut in addition to Ctrl+F4, to close currently active tab.





2020-03-25
----------


* (83a76d65) Crude patch to allow commandline parameter to change the Qiqqa "base directory": useful for independent library sets and testing rigs. Also a beginning for a 'portable / non-admin' Qiqqa install (https://github.com/jimmejardine/qiqqa-open-source/issues/124)





## v82pre release: v82.0.7357.40407

2020-03-24
----------


* (4abab9d0) v82.0.7357.40407

* (df9d867e) fixed a few minor issues in the metadata inference logic (used for extracting titles, etc. from the pdf text content)

* (3a154281) don't even *try* to log an exception when we've already shut down the logger and one or two of the `Dispose()` calls happens to cause a failure inside `SafeExec()`: by that time, there's nothing we can do and we're going down anyway, **as intended**.

* (0a7e86ca) fix https://github.com/jimmejardine/qiqqa-open-source/issues/180: FolderWatcher is not working: no PDF files are found, not ever.

  the wrong AlphaFS API was used in the code.





2020-03-23
----------


* (45971f58) fix Google Scholar background scraper logic to cope with modern Google Scholar website responses; this improves the Citation References data/list shown with each viewed PDF document in Qiqqa. (This is related to commit 9d6a79dea4dd6361d4642b8248995bc98bdb4217.)

* (9d6a79de) tweak/fix(?): Qiqqa was hiding the left side panel in the PDF Document view, which is receiving the background Google Scholar SCRAPE process output. (As I was wandering where that info went, I found out the left panel is filled at all time, but remaining hidden most of the time. This is a TEST fix to see what's going on  precisely and to evaluate whether having that info available is useful to the user...)

* (c4141a17) assertion tweak: only check if the long-running code is running in a non-UI thread when Qiqqa is NOT in the middle of application shutdown/termination.

  Reason: when Qiqqa is shutting down, some work MUST still be done while the UI is shutting down/has shut down already:  at that time in the life of the Qiqqa run-time, we DO NOT care about UI responsiveness any longer but only about whether the final required actions are done ASAP.

  This fixes some spurious UNWANTED & UNINTENTED assertion failures in the code.

* (95aea20a) emphasize user account name in the UI by indenting it visually





2020-03-17
----------


* (86ed86f2) PDF metadata extraction background task: A timeout should only kick in when we have *some* work done already or we would have introduced a subtle bug for very large libraries: if the timeout is short enough for the library scan to take that long on a slow machine, the timeout would, by itself, cause no work to be done, *ever*. Hence we require a minimum amount of work done before the timeout condition is allowed to fire.

* (c080f562) PDF OCR background task: Do not flood the status update system when we zip through the work queue very fast: only update the counts every second or so, but be sure to be the first to update the counts after work has been (temporarily) stopped.

* (29932376) bundle PDF processing log lines into a single log line as it floods the logfile for large(-ish) PDF documents. Some PDFs could cause up to 500 of these long(!) log lines to be produced, cluttering the log file no end.

* (5bc962cc) code inspection following up on https://github.com/jimmejardine/qiqqa-open-source/issues/156 : bit of code robustness added for when blacklist/whitelist files have been manually edited.

* (1736b291) https://github.com/jimmejardine/qiqqa-open-source/issues/158 but do note the comments in that issue: this fix is not going to work according to https://github.com/domgho/innodependencyinstaller/issues/12 + https://github.com/domgho/innodependencyinstaller/commit/ce85626e61cc9e1dacb9578e8e04ab9d435d9edc#r16322373-permalink

* (7906a79d) QiqqaOCR: word confidence is encoded as a perunage ("per one" instead of %: "per cent"); make it so. (Some bits of the code treated it as a percentage, some as a perunage.)

* (4899077e) looking at https://github.com/jimmejardine/qiqqa-open-source/issues/133





2020-02-27
----------

* (64e02652) doi2bib added

  Added new search engine - doi2bib





2020-02-19
----------


* (a21f97c9) improved the logging/reporting of installed and active .NET/CLR versions to help analyze bug reports.

* (5751c078)
  - "Force OCR" should really nuke the previous OCR results on disk so we get a complete refresh and not a half-hearted one.
  - don't throw exceptions (which are handled internally) when delete-ing files which MAY legally not exist.

* (e71d97f4) refactored the LDAsampler + expedition source code around the words set and the GetDescriptionForTopic() API; it's still a bit of a mess, but less so than before as the words set necessary for GetDescriptionForTopic() isn't passed around a zillion member functions any more.    TODO: work on the expedition refresh to reduce the number of topics to the actual non-duplicates, i.e. the ones that are unique to the human user, as visible when produced by GetDescriptionForTopic() without a sequence number.





2020-02-16
----------


* (26ff47aa) First bit of work to make the duplicate themes in Expedition go away: it turns out that `GetDescriptionForTopic()` is at the *visual* reason for this; we need to investigate further to find the root cause.

* (c01f5270) fix long-lived bug in the Author splitting code which would render authors like "Krista Graham" as empty slots in the Author side panel of the library view.

* (412ccf43) tweak BibTeX Control to start editing in RAW mode instead of formatted mode.

* (fea42040)
  - fixed the GoogleScholarScraper logic to suit the latest Google Scholar search page HTML output.
* (0a318405) fix crash/exception in Google scraper function





2020-02-14
----------


* (2daf3ee9) Add Developer Feature: Qiqqa will load a developer test settings file (json/json5 format) when available. The settings in this file help a developer or tester to disable certain Qiqqa features and/or limit or 'tweak' specific Qiqqa behaviours to help test the application in both debug and performance measurement scenarios.

* (7aba6ca5) Merge pull request #154 from wkedziora/polish-translation-wkedziora

  Update pl.qiqqa.txt





2020-02-07
----------


* (f829fb5b) Update pl.qiqqa.txt

  Updated Polish translation using English one as an example.





2020-02-03
----------


* (3e4cc439) migrate all projects to .NET 4.8 from 4.7.2 - this completes the work started in commit SHA-1: a449560fa479c75e8f0877d85cca01bcd73e28e2 : update projects to reference .NET 4.8 instead of 4.7.2 and C# language version 8.0. This does not impact OS platform support (see https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies): Qiqqa *should* run on Windows 7 and upwards, when the .NET update has been installed on Windows 8.1 and 7 (the OS update is automatic on Windows 10).

* (5d34a0e6) UI fix: placement of OK and Cancel buttons (TODO: do the same for the other dialogs)

* (a449560f) update projects to reference .NET 4.8 instead of 4.7.2 and C# language version 8.0. This does not impact OS platform support (see https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/versions-and-dependencies): Qiqqa *should* run on Windows 7 and upwards, when the .NET update has been installed on Windows 8.1 and 7 (the OS update is automatic on Windows 10).





2020-01-23
----------


* (d25ff654) disable the unused Google Analytics tracker code (used for Qiqqa Features(?))





2020-01-22
----------


* (d94ae3f9) quite a bit of work on the OCR background process:
  - refactor Qiqqa to *always* check if the temp scratch directory exists (and create it when absent): this prevents spurious errors when an external (overzealous) cleanup process cleans out the TEMP dir space, while a long-running Qiqqa session is still working on the OCR/indexing tasks
  - adjust the internal OCR job scheduling in such a way that we detect "sure repeat failures" early, thus preventing the UI status updates about page(s) to be textified to run up and down like crazy for huge libraries which carry multiple hard-to-OCR PDFs - this makes the UI status updates more stable and sensible to the user and prevents a lot of (re-)testing and (re-)execution of code paths which are sure to fail in QiqqaOCR anyway.
  - refactor the internal APIs: TEXT_PAGES_PER_GROUP is a constant which is not transported across the relevant classes instead of taking up a (useless) call parameter as this value MUST be constant and fixed across the application
  - fix a typo in a log statement

* (1ddde46f) indexing PDFs / OCR: accept the underscore '_' as a legal word character (it is also known as "the programmer's space")





2020-01-19
----------


* (437bcd96) fix faulty argument count check in QiqqaOCR for SINGLE-FAKE task

* (f89f05e3) fix for: log4net:ERROR XmlHierarchyConfigurator: Cannot find Property [ImmediateFlush] to set object on [log4net.Appender.ColoredConsoleAppender]

* (cd8b198d) OCR task: some of the error logging wouldn't make it, on occasion. Either stdout+stderr isn't flushed properly at the sender (QiqqaOCR) and/or not waited for and picked up at the receiving end (Qiqqa), but we lost important crash info too often. Now this issue is resolved by forced flushing of Log4Net (via the LogManager) *and* waiting an additional half-second to ensure the invoking process (Qiqqa) does properly fire all related events so we can gather the output of QiqqaOCR. Seemed similar to https://stackoverflow.com/questions/14963735/log4net-rollingfileappender-not-flushing-io-buffer-with-low-volume-log but in the end it turned out that only the new added Logging.ShutDown() API is resolving the issue (**with high probability**: I am still not entire sure this is it, as this problem is hard to reproduce consistently. :-(( )





2020-01-01
----------


* (a2d50082) Expedition: the LDA process to arrive at a set of more or less decent 'themes' (topic groups) for a library is cute, but takes a horrendous amount of time for large libraries. This commit cuts down ten-fold on the number of Monte Carlo runs and tweaks the status reporting in the UI to arrive at a slightly more informative(?) and 'easy' progress feedback while that long-running process is underway in the Expedition pane.

* (c9357750) fix bug in the PagesSetAsString() utility API: finally, long lists of page numbers are properly grouped in PDF/OCR log reports about missing/failed pages in the background PDF/OCR/indexing process.

* (3e022a0b) BibTeXEditorControl: not entirely done yet, but this at least cleans it up a *little bit* regarding green-lighting the control when the BibTeX happens to positively match the PDF Document.

  TODO: cope with BibTeX manual data entry and the many errors that will give until there's a minimal bibtex record entered. Right now the control is still very user-UNfriendly in its continuous desire to jump to the error report pane and get your focus off the edited field(s).

* (0d48af16) PDFInterceptor: reduce the amount of work and time spent in the UI thread when receiving a PDF from a webpage.

  TODO: I'm still not happy about the fact that the entire (potentially very slow) stream-to-file download+save activity is done in the UI thread, but I didn't spend more time checking whether it would survive in a background thread as I'm pretty sure the transition to CefSharp+Chrome will severely impact this and related logic parts of Qiqqa.

* (86dfe3d1) further work on coping with the OCR failures and bugs in Qiqqa and adjoining tools in the PDF/OCR chain (such as zero-width and zero-height words coming out of MuPDF)

* (758b6e2c) work done on the DBExplorer: better handling of records produced by .library and .s3db databases as those records are an amalgam of metadata, annotations, inks, etc. record types -- it's not really used as a database but merely as a hash-indexed generic table store.

* (fd886c05) make sure actual work is done in a background thread while the display data is fetched and updated in the UI thread.

  TODO: Qiqqa sure needs an MVVM/MV* overhaul which takes this distribution of work serious in the software architecture as this is becoming a mess.

* (916c12d9) minor fix: report document numbers (N of M) starting at 1(one), rather than 0(zero) as it looks odd to humans otherwise





2019-12-31
----------


* (b0191cda) QiqqaOCR: fake words for empty pages or any pages that appear so to the current OCR engine! (https://github.com/jimmejardine/qiqqa-open-source/issues/73 + https://github.com/jimmejardine/qiqqa-open-source/issues/129 + https://github.com/jimmejardine/qiqqa-open-source/issues/135 )





2019-12-27
----------


* (39f6147d)
  - added SINGLE-FAKE mode for QiqqaOCR: this is invoked when GROUP and SINGLE don't deliver due to, for example, encrypted PDF source. This is a temporary hack to ensure Qiqqa doesn't repeat OCR activities ad nauseam (https://github.com/jimmejardine/qiqqa-open-source/issues/129 , https://github.com/jimmejardine/qiqqa-open-source/issues/135 , https://github.com/jimmejardine/qiqqa-open-source/issues/73 , etc.)
  - the previously added extra OCR text files' sanity checks (zero-sized areas of words, etc.) seems to pay off. At least we've observed quite a few OCR files/pages being retriggered for OCR as Qiqqa uncovers these zero-sized word areas while refreshing for Expeditions
  - added a few more UI-thread-or-not Assertions.

* (2a51f204) GetOutputsDumpString() :: oddly enough this code can produce a race condition exception for some Output: "Collection was modified; enumeration operation may not execute." -- HACK: we cope with that by re-iterating over the list until success is ours...   :-S :-S  hacky!

* (651bac5b) fix coding bug in SafeThreadPool handling: the logic to determine which tasks to skip/abort on application Exit was inverted. :-(





2019-12-26
----------


* (483ddd7f) QiqqaOCR: added some sanity checks to the output files'decoding in Qiqqa and made sure any error in the tool itself is visible in Qiqqa - any fixup for that should be done in Qiqqa, not QiqqaOCR itself.


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

* (17d84225) fix crash during application termination

* (48e05b3b) defer configuration save activity to a background thread and then only do it when either the application is terminating or when the config hasn't been saved in the last minute or so (preventing disk hammering when changing many config settings)

* (06ac00ae) fix crash during application termination

* (60392ba9) fix crash: 91224235436:System.NullReferenceException: stacktrace:

  - Object reference not set to an instance of an object
    - at Qiqqa.Documents.BibTeXEditor.BibTeXEditorControl.RegisterOverlayButtons(FrameworkElement BibTeXParseErrorButton, FrameworkElement BibTeXModeToggleButton, FrameworkElement BibTeXUndoEditButton, Double IconHeight) in \Qiqqa\Documents\BibTeXEditor\BibTeXEditorControl.xaml.cs:line 172
    - at Qiqqa.Documents.PDF.PDFControls.MetadataControls.MetadataBibTeXEditorControl.OnClosed(EventArgs e) in \Qiqqa\Documents\PDF\PDFControls\MetadataControls\MetadataBibTeXEditorControl.xaml.cs:line 122
    - at System.Windows.Window.WmDestroy()

* (39ab3330) using Win32 OpenFileDialog instead of the Windows.Forms flavor.

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

* (d333f88e) tweaking which library types can sync, i.e. which ones are read-only and which ones aren't.





2019-11-10
----------


* (db06442a) TODO: proper collision detection and merge action on the metadata when you import Documents which are already present in the library, but might need to have their metadata (partially!) updated. Currently the existing metadata is largely nuked. Check out AddNewDocumentToLibrary() in Library.cs and the TODO comments in Library.cs and PDFDocument.cs when you're working on the MERGE METADATA job.

* (f4791a0d)
  - fixing https://github.com/jimmejardine/qiqqa-open-source/issues/134 : Copy/Move to library functionality was b0rked due to crufty coding of passing the return value (picked library) while I had been a tad overzealous when going through the code hunting down memleak spots, i.e. Dispose and Close handlers' cleanup, resulting in the library picker dialog always producing a NULL library reference.

  - Also tweaked the copy/move process a little while testing the copy & move actions.

  - Migrated the copy/merge activity to a background task to ensure a more responsive UI, particularly when copying/moving large numbers of PDFDocuments at once!

  TODO: proper collision detection and merge action on the metadata when you import Documents which are already present in the library, but might need to have their metadata (partially!) updated. Currently the existing metadata is largely nuked. Check out AddNewDocumentToLibrary() in Library.cs and the TODO comments in Library.cs and PDFDocument.cs when you're working on the MERGE METADATA job.





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


* (89e9de7d) tweak the amount of logging kept in rotation - part of debugging/analyzing qiqqa behaviour





2019-11-03
----------


* (6a8dda06) don't just wait 10 seconds when extracting a Library bundle. It may be huge or you're running on a slow box and you'll get a b0rked extract. Just let 7ZIP complete.

* (38208863) Bit more refactoring work for https://github.com/jimmejardine/qiqqa-open-source/issues/95

* (d927e2bb) fix lingering crash in Dispose method. Follow-up for commit SHA-1: d05bbe2da06b825a0a079a73e14543f3af282165

* (f37c9dc2) https://github.com/jimmejardine/qiqqa-open-source/issues/95 : turns out most of it had already been done in the original Qiqqa. Upon closer inspection the remaining `Process.Start()` calls are are intended to open an (associated) application for a given file or directory, which is proper.

  Added a few `using(...)` statements around Process usage, etc. to prevent memory leaks on these IDisposables.

* (d05bbe2d) More work related to commit SHA-1: 43b1fe0972f99660e0bbbeea2deb357b2002f190 : fix crashes at application shutdown

* (148ea943) fix https://github.com/jimmejardine/qiqqa-open-source/issues/126

* (016b888e) fix https://github.com/jimmejardine/qiqqa-open-source/issues/132





2019-11-02
----------


* (c06021a8) bumped build revision




2019-11-01
----------


* (95dff9bc) fix b0rk introduced by commit SHA-1: bcd73cd877b72cd2b9aba9183172dd6c46590880 :: we don't do a *revert* action per se, but rather improve upon the patch we picked up there from the experimental branch: as it turns out, the patch caused a lot of trouble which has been resolved to allow the running background task(s) access to a reduced clone of the WebLibraryDetails, which does not exhibit the cyclic dependency that the original WebLibraryDetails instance incorporated, thus breaking the cyclic reference and allowing the .NET GC to do its job on the Library instance(s) ASAP.

  As this problem was discovered while doing work on commit SHA-1: ed2cb589a2e3562102163c4b3129310c4850e33a, these files also include the remainder of the work done for that commit, as it was important to separate out the patches which fixed the cyclic memory reference.





2019-10-31
----------


* (bcd73cd8) picked up memleak fix from experimental branch

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


* (a20685b8) working on a fix for https://github.com/jimmejardine/qiqqa-open-source/issues/57 : part 1 = making the statusbar updates more usable by truncating them. Had a long url being reported for downloading which pushed all other messages off screen. :-1:

* (458cd2ec) performance / shutdown reliability: when closing the app, the UI message may be loaded severely and may not even terminate properly in spurious circumstances (had a situation here where the WaitForUIActivityDone calls were tough to shut up for a yet unidentified reason; happened after testing several Sniffer PDF download actions so we may be looking at another hairy bit of the XulRunner/Browser interaction here). Also add a check to the PDF/OCR thread queue processing code to help hasten shutdown behaviour.





2019-10-21
----------



* (8393c556) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/82 : refactored the BibTeXEditorControl and all its users: those must now provide the toggle/view-errors/etc. buttons for the control, so that the parent has full control over the layout, while the BibTeXEditorControl itself will set the icons, tooltip, etc. for each of those buttons to ensure a consistent look of the BibTeX editor buttons throughout the application.

  TODO: see if we need to discard those registered buttons in the Unload event to ensure we're not memleaking...

* (acc4357a) add icons for BibTeX control et al: complete work started in commit SHA-1: 0f9fa67e470acb834e24cd30e946a0b71e954818 :: adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.





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

* (fd8326e1) https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits. Includes some minimal prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/87.

* (0f9fa67e) adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.

* (a5faaafa) fix https://github.com/jimmejardine/qiqqa-open-source/issues/115 : PDF Reader (which does a Scholar Scrape) does not work for users living outside US/UK. Also further fixes https://github.com/jimmejardine/qiqqa-open-source/issues/114. Also fixes https://github.com/jimmejardine/qiqqa-open-source/issues/117 by enforcing UTF8 encoding on the content: we're downloading from Google Scholar there, so we should be good. Google Scrape finally finds decent titles, author lists and even PDF download links once again.

  TODO: update the 'Google Scholar' view part in the PDFReader control.

* (65e5707a) fix https://github.com/jimmejardine/qiqqa-open-source/issues/114 as per https://stackoverflow.com/questions/13771083/html-agility-pack-get-all-elements-by-class#answer-14087707




2019-10-18
----------


* (ac042668) setup/installer: make the generated `setup.exe` should the same full build version as the Qiqqa executable itself.

* (1f6b9dfc) fix for setup/installer: set the `setup.exe` file version (as reported by Windows on right-click->Properties) to the Qiqqa release version instead of 0.0.0.0





2019-10-16
----------


* (2cdd3426) Merge pull request #110 from GerHobbelt/mainline-master

  update README with latest info about Qiqqa software releases.

* (acd9229) README: point at the various places where software releases are to be found.

* work done on run-time performance / reduce memory leaks. Code simplification.

* (305c085) fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/98: correct deadlock prevention code in CitationManager. Code robustness.

* (6ef9b44) Code robustness. Refactoring the code and making its interface threadsafe. Fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/106 / https://github.com/jimmejardine/qiqqa-open-source/issues/98.

* (6ef9b44) Tweak: `AddLegacyWebLibrariesThatCanBeFoundOnDisk()`: at startup, try to load all .known_web_libraries files you can find as those should provide better (= original) names of the various Commercial Qiqqa Web Libraries. (This is running ahead to the tune of https://github.com/jimmejardine/qiqqa-open-source/issues/109 / https://github.com/jimmejardine/qiqqa-open-source/issues/103)

* (8b90f41) Code robustness. Fix https://github.com/jimmejardine/qiqqa-open-source/issues/105 : turns out the config user GUID is NULL/not initialized at startup. This fix ensures there's a predictable (Guest) config active from the very start of the Qiqqa app.

* (d52d565) quick hack for https://github.com/jimmejardine/qiqqa-open-source/issues/104: make Qiqqa start no matter what.

* (f515ca7) "directory tree as tags" fix: recognize both \ and / as 'tag' separators in any document path.

* (626902b) (fix) Code robustness. Copy the new library background image file into place, if it is another file than the one we already have. Fix: do *not* delete the active image file when the user did not select another image file.

* (1b2daca) Performance / Code Robustness.
  - at very high loads allow the UI to update regularly while work is done in background tasks.
  - show mouse cursor as Hourglass/Busybee: during application startup, this is used to give visual feedback to the user that the work done is taking a while (relatively long startup time, particularly for large libraries which are auto-opened due to saved tab panel sets including their main library tabs)
  - fixed a couple of crashes.

* (fb775d5) Code Robustness. Take out the Sorax PDF page count API call due to suspicion of memleaking and heapcorrupting as per https://github.com/jimmejardine/qiqqa-open-source/issues/98 initial analysis report.

* (e353006) Code Robustness. PDFRendererFileLayer: when calculating the PDF page count happens to fail 3 times or more (Three Strikes Rule), then this PDF is flagged as irreparably obnoxious and the page count will be set to zero for the remainder of the current Qiqqa run -- this is not something we wish to persist in the metadata store as different software releases may have different page count abilities and *bugs*.

* (935a61f) Code Robustness.

* (171bf18) Performance / Code Robustness. Turning off those thread lock monitors which are hit often, take up a notable bit of CPU and are not suspect any more anyway.

* (eba4472) Fix bug where it looked like "Copy To Another Library" menu choice and "Move to Another Library" memu choice didn't differ at all: it's just that MOVE did not properly signal the sourcing library needed an update as well as the document itself.

* (1cc4d1f) Clone/Copy didn't always carry the document metadata across to the new lib. Fixed.

* (ec57707) fix crash - https://github.com/jimmejardine/qiqqa-open-source/issues/93

* (2373620) Google Analytics throws a 403. Probably did so before, but now we notice it once again as we watch for exceptions occuring in the code flow. Better logging of the 403 failure.

* (c8590be) Trying to cope with the bother of https://github.com/jimmejardine/qiqqa-open-source/issues/94 - quite a bit of it is inexplicable, unless Windows updates pulled the rug from under me (and CLR 4.0)

* (cdd51d6) Sniffer: BibTeX editor pane can toggle between formatted/parsed field editor mode and raw BibTeX editor mode.

  Known issue: the little green cross that is the UI element to toggle editor modes
  has the nauseating behaviour of moving along up & down and thus hiding behind
  the sniffer ok/fail/skip/clean buttons at top-right.

  (Pending: https://github.com/jimmejardine/qiqqa-open-source/issues/82)

* (1765137) fix https://github.com/jimmejardine/qiqqa-open-source/issues/74 + https://github.com/jimmejardine/qiqqa-open-source/issues/73 = https://github.com/GerHobbelt/qiqqa-open-source/issues/1 : QiqqaOCR is rather fruity when it comes to generating rectangle areas to OCR. This is now sanitized.

* Improved code robustness.

* Some performance improvements.

* (a3bc0b4) fix/tweak: when qiqqa server software revision XML response shows a *revert* to an older Qiqqa software release, such was coded but would not have been caught before as that bit of conditional code would never be reached when the user is running a later Qiqqa version.

* (4fd0bd1 et al) fixed 'recover desktop' functionality: screen positions and opened panes are now correctly remembered.

* (45f083d) upgrade projects to all reference .NET 4.7.2 instead of .NET 4.0 Client Profile

* (48daaa2) `support@qiqqa.com` is of course R.I.P., hence point at github issue tracker instead for software failures.

* (4f07d34) Legacy Web Library: such a library is NOT read-only. (What we got to do is point it to an 'Intranet' sync point = directory/URI path instead. (TODO)

* (bd923e5) fix https://github.com/jimmejardine/qiqqa-open-source/issues/72 + adding **minimal** support for bibtex concatenation macros in order to (somewhat) correctly parse google scholar patents records: fixes https://github.com/jimmejardine/qiqqa-open-source/issues/22 and a bit of work towards https://github.com/jimmejardine/qiqqa-open-source/issues/68

* (07bb569) allow tags to be separated by COMMA as well as SEMICOLON. (Added COMMA separator support)

* (ec1fe2c) one more for https://github.com/jimmejardine/qiqqa-open-source/issues/67

  WARNING: a PDF URI does *not* have to include a PDF extension!

  Case in point:

      https://pubs.acs.org/doi/pdf/10.1021/ed1010618?rand=zf7t0csx

  is an example of such a URI: this URI references a PDF but DOES NOT contain the string ".pdf" itself!

* (c4b64a4) PDF imports: add menu item to re-import all PDFs collected in the library in order to discover the as-yet-LOST/UNREGISTERED PDFs, which collected in the library due to previous Qiqqa crashes & user ABORT actions (https://github.com/jimmejardine/qiqqa-open-source/issues/64)

* (3872959) `AddNewDocumentToLibraryFromInternet_*()` APIs: some nasty/ill-configured servers don't produce a legal Content-Type header, or don't provide that header *at all* -- which made Qiqqa barf a hairball instead of properly attempting to import the downloaded PDF.

  Also don't yak about images which are downloaded as part of Google search pages, etc.: these content-types now make it through *part* of the PDF import code as we cannot rely on the Content-Type header being valid or present, hence we need to be very lenient about what we accept as "potentially a PDF document" to inspect before importing.

  Fixes: https://github.com/jimmejardine/qiqqa-open-source/issues/63

* (d2b5c22) tackling with the weird SQLite lockup issues: https://github.com/jimmejardine/qiqqa-open-source/issues/62

  As stated in the issue:

  Seems to be an SQLite issue: https://stackoverflow.com/questions/12532729/sqlite-keeps-the-database-locked-even-after-the-connection-is-closed gives off the same smell...

  Adding a `lock(x) {...}` critical section **per library instance** didn't make a difference.

  Adding a global/singleton  `lock(x) {...}` critical section **shared among /all/ library instances** *seems* to reduce the problem, but large PDF import tests show that the problem isn't *gone* with such a fix/tweak/hack.

* (abd020a) UPGRADE PACKAGES: log4net, SQLite, etc. -- the easy ones. Using NuGet Package Manager.

* (43debf6) remaining work for  https://github.com/jimmejardine/qiqqa-open-source/issues/56 / https://github.com/jimmejardine/qiqqa-open-source/issues/54 -- catch some nasty PDF URIs which weren't recognized as such before. Right now we're pretty aggressive as we fetch almost everything that crosses our path; once fetched we check if's actually a valid PDF file after all. CiteSeerX and other sites now deliver once again...

* (c3102a7) fix: BibTeX dialog doesn't scroll: that's a problem when your list of BibTeX tags is longer than the height of the dialog allows. Hence we can now scroll the bugger.

* (b46b38d) Don't get bothered by the Tweet stuff: collapse it.

* (e0d056c) fixes for https://github.com/jimmejardine/qiqqa-open-source/issues/56 ; also ensuring every document that's fetched off the Internet is opened in Qiqqa for review/editing (some PDF documents were silently downloaded and then dumped into the Guest Library just because and you'ld have to go around and check to see the stuff actually arrived in a library of yours. :'-(

* (be0d54f) fix: https://github.com/jimmejardine/qiqqa-open-source/issues/60 + https://github.com/jimmejardine/qiqqa-open-source/issues/39 + better fix for https://github.com/jimmejardine/qiqqa-open-source/issues/59

  check how many PDF files actually match and only move forward when we don't end up full circle. don't reflect if we didn't change. when we re-render the same document, we don't move at all!

* (4e791e8) fix https://github.com/jimmejardine/qiqqa-open-source/issues/59: don't reflect if we didn't change.

  We start with a dummy fingerprint to ensure that we will observe ANY initial setup/change as significant for otherwise we don't get the initial PDF document rendered at all!

  We use the PDF Fingerprint as a check for change as the numeric `pdf_documents_search_index` value might look easier but doesn't show us any library updates that may have happened in the meantime.

* (c8e3729) Locking in the current state of affairs as of https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink while I dig further to uncover the *slowtard* culprits. Mother Of All Commits. Can be split up into multiple commits if needed later on in a separate branch. Actions done in the past days while hunting the shite:

  - `FeatureTracking:GoogleAnalysicsSubmitter` begets a 403 (Forbidden) from Google Analytics. Report such in the logfile instead of letting it pass silently. (Not that I care that much about feature tracking right now, but this was part of the larger code review re HTTP access from Qiqqa)

* (c1bb4f8)
  - BibTeX Sniffer: clean up search items results in better (human readable) search criteria for some PDFs where previously the words were separated by TAB but ended up as one long concatenated string of characters in Google Scholar Search.
  - HTTP/HTTPS web grab of PDF files: we don't care which TLS/SSL protocol is required, we should just grab the PDF and not bother. Some websites require TLS2 while today I ran into a website which requires old SSL (not TLS): make sure they're **all** turned ON.
  - Register the current library with the WebBrowserHostControl so that we don't have to go through obnoxious 'Pick A Library' dialog every time we hit the "Import all PDFs available on this page" button in the browser toolbar.

* (5f00577)
  - refactor: now StandardWindow will save (and restore on demand) the window size and location for any named window; th settings will be stored in the configuration file. SHOULD be backwards compatible. Further work on https://github.com/jimmejardine/qiqqa-open-source/issues/8
  - also fix the handling of the "Has OCR" checkbox: made it a proper tri-state. VERY SLOW to filter when ticked OFF or ON. (TODO: add a hack where we only allow it to impact the filtering for N seconds so as to limit the impact on UX performance-wise)

* (720caf2)
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/54 in GoogleBibTexSnifferControl
  - Gecko these days crashes on ContentDispositionXXXX member accesses: Exception thrown: 'System.Runtime.InteropServices.COMException' in Geckofx-Core.dll

    I'm not sure why; the only change I know of is an update of MSVS2019.  :-S

  - implement the logic for the BibTeXSniffer 'Has OCR' checkbox filter criterium. It's useful but the zillion file-accesses slow the response down too much to my taste.   :-S

* (6e8ab5d) sniffer: add filter check box to only show those PDF records which have been OCRed already. (The ones that aren't are pretty hard to sniff as you cannot mark any title text bits in them yet, for instance)

* (3f1bbf2) quick fix for folder watcher going berzerk -- has to last until we refactor the async/sync PDF import code chunks. (see branch `refactoring_pdf_imports`)

* (b0b7e72)
  - 'integrate' nant build script for producing setup.exe installers into MSVS2019 solution by using a dummy project and prebuild script
  - added skeleton projects for qiqqa diagnostic and migration work. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/43

* (3a2629f) working on https://github.com/jimmejardine/qiqqa-open-source/issues/52 : FolderWatcher and PDF library import need rework / refactor as we either break the status feedback to user ("Adding document N of M...") as we loose the overall count of added documents, *or* we add/import PDF documents multiple times as we cannot destroy/Clear() the list since it is fed to the async function **as a reference**. :-(

* (716d54d) fix/tweak: just like with Sniffer AutoGuess, when a BibTeX record is picked from bibtexsearch using heuristics, it is now flagged in the bibtex with a special comment (`@COMMENT...`) which was already available before in the code but apparently disused or unused until today.

* (2eb1380) refactored: Wasn't happy with the code flow in the FolderWatcher: now the long recursive directory scan (using `EnumerateFiles()`) is only aborted whn the app is terminated or when it has run its course (or when there are more or less dire circumstances); otherwise the dirtreescan is periodically paused to give the machine a bit of air to cope with the results and/or other pending work, while an app exit is very quickly discovered still (just like before, it is also detected inside the `daemon.Sleep(N)` calls in there, so we're good re that one. Tested it and works nicely. https://github.com/jimmejardine/qiqqa-open-source/issues/50

* (7118c3c) fix https://github.com/jimmejardine/qiqqa-open-source/issues/48 : Expedition: Refresh -> "Looking for new citations in ..." is not aborted when Qiqqa is closed.

* (1053fce)
  - fixed https://github.com/jimmejardine/qiqqa-open-source/issues/50 using EnumerateFiles API instead of GetFiles. (TODO: inspect other sites in code where GetFiles is invoked)
  - introduced `Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown` as a global flag to check whether the app is terminating: this works the same way as `!thread.IsRunning` but without the need to memoize the thread reference or pass it via extra function arguments.
  - Addd a couple of extra `IsShuttingDown` checks to improve 'exit' termination speed of the application.

* (869dea1) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/8: now stores WindowState as well and fetches non-maximized window size using RestoreBounds WPF API.

* (8db7a3d) tweak the About message: now also show the *full* build version next to the classic `vNN` version.

* (7750279) make Qiqqa main app and QiqqaOCR logging easily recognizable: `[Q]` or `[OCR]` tag per logline.
  Also print the QC-reported memory usage as a fixed-width number in MBytes




# version 81.0.7158.38371 :: alpha test release
2019-08-07
----------

  * Qiqqa now copes better with damaged PDFs which are part of the librarie(s):
    + search index does not "disappear" any more
    + Qiqqa does not continue running in the background for eternity due to locked-up PDF re-indexing task

  * log outgoing activity: posting BibTeX info to bibtexsearch.com aggregator
  * re-added to 'Add This PDF to Library' button in the browser; TODO: make it work akin to the <embed> handling to prevent confusion: when the browser shows a single PDF, it MAY be an <embed> web page and we should account for that!
  * IMPORTANT FIX: this bad boy (an overzealous Dispose() which I introduced following up on the MSVS Code Analysis Reports) prevented Qiqqa from properly fetching and importing various PDFs from the Sniffer. (click on link would show the PDFs but not open them in Qiqqa nor import them into the Qiqqa library)
  * fix crash in PDF import when website/webserver does not provide a `Content-Disposable` HTTP response header
  * import PDF(s) from this web pages: added ability to cope with `<embed>` PDF links, e.g. when a HTML page is shown with PDF embedded instead of the PDF itself
  * detect PDF files in URLs which have query parameters: '.pdf' is not always the end of the URL for downloading the filename
  * added CHANGELOG (partly edited & full version using git log)
  * Whoops. Crash when quickly opening + closing + opening.... Sniffer windows: CLOSE != DISPOSE. Crash due to loss of search_options binding on second opening...
  * Only when you play with it, you discover what works. The HasSourceURL/Local/Unsourced choices should be OR-ed together as that feels intuitive, while we also want to see 'sans PDF' entries as we can use the Sniffer to dig up the PDF on the IntarWebz if we're lucky. Meanwhile, 'invert' should clearly be positioned off to a corner to signify its purpose: inverting your selection set (while it should **probably** :thinking: have no effect if a specific document was specified by the user: then we're looking at a particular item PLUS maybe some other stuff?
  * Sniffer Features:
    - add checkboxes to (sub)select documents which have a URL source registered with them or no source registered at all. (https://github.com/jimmejardine/qiqqa-open-source/issues/29)
    - add 'invert' logic for the library filter (https://github.com/jimmejardine/qiqqa-open-source/issues/30)
  * fix https://github.com/jimmejardine/qiqqa-open-source/issues/28: turns out Qiqqa is feeding all the empty records to the PubMed-to-BibTex converter, which is throwing a tantrum. Improved checks and balances and all that. Jolly good, carry on, chaps. :-)
  * report complete build version in logging.
  * improving the logging while we hunt for the elusive Fail Creatures...
  * Code Quality / Stability work following up on Microsoft Visual Studio Code Analysis Reports et al.
  * done work on https://github.com/jimmejardine/qiqqa-open-source/issues/27 and on the lockups of Qiqqa (some critical sections in there were **humongous** in both code side and run-time duration; now the number of lock-ups due to *very* slow loading PDFs coming in from the Qiqqa Sniffer should be quite reduced: work related to https://github.com/jimmejardine/qiqqa-open-source/issues/18

2019-08-07
----------

  * fix crash when quickly opening + closing + opening.... Sniffer windows

  * Sniffer Features:

    - add checkboxes to (sub)select documents which have a URL source registered with them or no source registered at all. (https://github.com/jimmejardine/qiqqa-open-source/issues/29)

      Note: Only when you play with it, you discover what works. The HasSourceURL/Local/Unsourced choices are OR-ed together as that feels intuitive, while we also want to see 'sans PDF' entries as we can use the Sniffer to dig up the PDF on the IntarWebz if we're lucky. Meanwhile, 'invert' is positioned off to a corner to signify its purpose: inverting your selection set (while it should **probably** :thinking: have no effect if a specific document was specified by the user: then we're looking at a particular item PLUS maybe some other stuff?)

     - add 'invert' logic for the library filter (https://github.com/jimmejardine/qiqqa-open-source/issues/30)

  * fix https://github.com/jimmejardine/qiqqa-open-source/issues/28: turns out Qiqqa is feeding all the empty records to the PubMed-to-BibTex converter, which is throwing a tantrum. Improved checks and balances and all that. Jolly good, carry on, chaps. :-)

  * report complete build version in logging.
  * improving the logging while we hunt for the elusive Fail Creatures...
  * Code Quality / Stability work following up on Microsoft Visual Studio Code Analysis Reports et al.

2019-08-06
----------

  * using a more Microsoft-like build versioning approach. 81.x.y.z where y.z is a build ID

  * feature added: store the source URL (!yay!) of any grabbed/sniffed PDF. Previously the source path of locally imported (via WatchFolder) PDFs was memorized in the Qiqqa database. It adds great value (to me at least) when Qiqqa can also store the source URL for any document -- this is very handy information to have as part of augmented document references!)

  * Code Quality / Stability work following up on Microsoft Visual Studio Code Analysis Reports et al.



2019-08-05
----------

  * Don't let illegal multiple BibTeX entries for a single PDF record slip through unnoticed: one PDF having multiple BibTeX records should be noticed as a WARNING at least in the logging.
  * some invalid BibTeX was crashing the Lucene indexer (AddDocumentMetadata_BibTex() would b0rk on a NULL Key)
    Sample invalid BibTeX:
        @empty = delete?
  * trying to tackle the slow memory leak that's happening while Qiqqa is running  :-((
  * DBExplorer severely enhanced:
    - now supports wildcards in query parameters (% and _, but also * and ?, which are the MSDOS wildcards which translate directly to the SQL wildcards)
    - now supports GETting multiple records.
    - when GETting multiple records, DBExplorer not only prints the BibTeX for each record, but also the identifying fingerprint, verification MD5 and most importantly: the *PARSED* BibTeX (iff available) and BibTeX parse error diagnostics report.
    - when GETting multiple records, the DBExplorer output is also dumped to the file Qiqqa.DBexplorer.QueryDump.txt in the Qiqqa Library base directory. A previous DBExplorer query report dump will be REPLACED.
    - an extra input field has been added which allows the user to specify a maximum number of records to fetch: this speeds up queries and their reporting when working on large libraries with query criterai which would produce 1000nds of records if left unchecked.
    This allows to use the DBExplorer as a rough diagnostics tool to check the library internals, including a way to find erroneous/offending BibTeX entries which may cause havoc in Qiqqa elsewhere.

2019-08-04
----------

  * fixing https://github.com/jimmejardine/qiqqa-open-source/issues/8: not only storing Left/Top coordinate, but also Width+Height of the Qiqqa.exe window
  * fix crash in chat code when Qiqqa is shutting down
  * Since ExpeditionManager is the biggest OutOfMemory troublemaker (when loading a saved session :-( ), we're augmenting the logging a tad to ease diagnosis. (https://github.com/jimmejardine/qiqqa-open-source/issues/19)
  * debugging: uncollapsing rollups in dialog windows as part of a longer debugging activity. MUST REVERT!
  * 'Open New Browser' was looking pretty weird due to a website/page being loaded which was unresponsive; now we're pointing to a more readily available webpage instead. (Though in my opinion 'Open Browser' should load a VERY MINIMAL webpage, which has absolutely *minimal* content...)
  * Mention the new CSL (Citation Styles) source websites in the credits.
  * code stability: Do not crash/fail when the historical progress file is damaged

2019-08-03
----------

  * The easy bit of https://github.com/jimmejardine/qiqqa-open-source/issues/3: synced the Qiqqa/InCite/styles/ directory with the bleeding edge of the CSL repo at https://github.com/citation-style-language/styles (Note the 'bleeding edge' in there: I didn't use https://github.com/citation-style-language/styles-distribution !). DO NOTE that Qiqqa had several CSL style definitions which don't exist in this repository: these have been kept as-is.

2019-08-02
----------

  * tackling spurious application lockups and extremely unresponsive behaviours. Addresses these issues:
    + https://github.com/jimmejardine/qiqqa-open-source/issues/18
    + https://github.com/jimmejardine/qiqqa-open-source/issues/17
    + https://github.com/jimmejardine/qiqqa-open-source/issues/10
    + https://github.com/jimmejardine/qiqqa-open-source/issues/20

  * Fix https://github.com/jimmejardine/qiqqa-open-source/issues/17 by processing PDFs in any Qiqqa library in *small* batches so that Qiqqa is not unreponsive for a loooooooooooooong time when it is re-indexing/upgrading/whatever a *large* library, e.g. 20K+ PDF files. The key here is to make the '**infrequent background task**' produce *some* result quickly (like a working, yet incomplete, Lucene search index DB!) and then *updating*/*augmenting* that result as time goes by. This way, we can recover a search index for larger Qiqqa libraries!

  * dialing up the debug/info logging to help me find the most annoying bugs, first of them: https://github.com/jimmejardine/qiqqa-open-source/issues/10, then https://github.com/jimmejardine/qiqqa-open-source/issues/13

  * Do NOT nuke the previous run's `Qiqqa.log` file in `C:\Users\<YourName>\AppData\Local\Quantisle\Qiqqa\Logs\`: **quick hack** to add a timestamp to the qiqqa log file so we'll be quickly able to inspect logs from multiple sessions.

    **Warning**: this MUST NOT be present in any future production version or you'll kill users by log file collection buildup on the install drive!

  * simple stuff: updating copyright notices from 2016 to 2019.

  * update existing Syncfusion files from v14 to v17, which helps resolve https://github.com/jimmejardine/qiqqa-open-source/issues/11

    **Warning**: I got those files by copying a Syncfusion install directory into qiqqa::/libs/ and overwriting existing files. v17 has a few more files, but those seem not to be required/used by Qiqqa, as **only overwriting what was already there** in the **Qiqqa** install directory seems to deliver a working Qiqqa tool. :phew:

  * updating 7zip to latest release

Version 80 (FOSS):
=================


> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.





- Qiqqa goes Open Source!!!
- Enabled ALL Premium and Premium+ features for everyone.
- Removed all Web Library capabilities (create/sync/manage)
- Added the ability to copy the entire contents of a former Web Library into a library - as a migration path from Qiqqa v79 to v80




----

**Start of the Qiqqa Open Source activity**

----





Commercial Qiqqa releases
=========================


Version 79 (Commercial):
- Can add regularly used user-defined keys to the BibTeX Editor
- Can add regularly used search queries to the Search Boxes.
- Adds Jamatto donate buttons to the PDF Share feature.
- Check out Qiqqa for Web at http://web.qiqqa.com

Version 78:
- Bundle Libraries allow you to bundle up libraries for read-only dstribution of your content to your customers.
- Can export linked-documents data.
- Custom abbreviations can override default ones.
- Font size change in Speed Reader.
- Fixes a BibTeX parse error causing problems with Qiqqa starting.

Version 77:
- You can override the location of your PDF and OCR files.

Version 76:
- Improved BibTeX Sniffer where you can scan linearly and review automatically found BibTeXes.
- Improved InCite screen.
- Improved location of highlighter, annotation tools in PDF Reader.

Version 75:
- Can attach PDFs to Vanilla References using Web Browser.
- Can toggle BibTeX Search Wizard and Automatic BibTeX association.
- Automatically import from EndNote.
- Qiqqa Community Chat.
- Latest embedded Firefox browser.
- Locked PDFs do not cause the Annotation Report to stop halfway through.

Version 74:
- Can watch multiple folders.
- Can automatically attach tags to PDFs from watched folders.
- Can export tags, autotags, etc.
- Updated lucene library to fix corrupted search index on rare occasions.
- Moves to the top of the library after sorting.

Version 73:
- Friendlier configuration screen.
- Scrolls to the top of the document list when a filter or sort changes.
- Annotation Report: Removes menu hyperlinks from annotation report on export to Word; Abstract and Comment titles.
- PDF renderer is robust to a variety of corrupted PDF types.
- PDFs are not blurry on *all* LCD monitor types
- If necessary, Qiqqa can now use up to 4Gb RAM on 64-bit machines.  No longer limited to circa 1.2Gb.

Version 72:
- Qiqqa automatically associates BibTeX with well-known PDFs.
- Public status web libraries
- Better duplicates detector.
- Improved brainstorm auto layout
- Can tweet a document from the document reader
- Can get a username reminder from login screen.
- Batches PDF uploads so that massive libraries (e.g. 20,000+ docs) do not time out on slow networks.
- Fix to brainstorm resize exception

Version 71:
- Can link documents so that you can quickly jump between them.
- Can customise library icon and background.
- Can open a PDF from the BibTeX Sniffer.
- Qiqqa's proxy support now includes using your Windows-user or network-user details.
- Can add 1000s of documents more quickly.
- OCR automatically uses all but one of your CPUs
- Can locate an Intranet sync folder location.

Version 70:
- Downloads of upgrades are MUCH faster.
- Premium Web Library storage space is increased to 10Gb, free storage space to 2Gb!
- Smoother highlighting.
- Library search always causes the sort mode to switch to Search Score.
- Explanation of recommendations on Start Page.
- Can resize Annotations on a PDF by double tapping and dragging - great for tablets.
- Can right-click and add an AutoTag to the while- or blacklists.
- Can promote an AutoTag to a Tag.
- Can explore an AutoTag in a Brainstorm.
- Automatically detects when you sync from another computer and syncs immediately.
- Autodetects new library memberships and new premium payments.
- Fixed the 'forgotten watch folder when you refresh memberships' issue.
- Authors in BibTeX are now split on 'mixed case aNd'

Version 69:
- You can filter annotation in the Annotation Report by creator.
- Improvements to Annotation Report formatting.
- "BibTeX Type" library filter.
- Welcome Wizard gets you up to speed with Qiqqa quickly.
- Mass-download all PDFs in web browser handles more types of URL and content type.
- Web Browser status messages.
- Ctrl-F jumps to search box.

Version 68:
- Supports imports of patent portfolios from Omnipatents.
- Can mass-download all PDFs linked to by a web page.
- Open PDF tabs are coloured.

Version 67:
- Novice/Expert mode.
- Mass edit documents metadata.
- Customised Reading Stages.
- Pivot Table of library statistics.
- Share Annotations and Brainstorms via Social Media.
- Redesigned Qiqqa InCite screen.
- Improvement to 'blue book' citation snippet formatting.
- Miscellaneous changes to GUI to remove clutter.
- Fixed window redock exception.

Version 66:
- Can filter annotations by date in the Annotation Report.
- Can add prefixes and suffixes to InCite citations.
- Sort has moved to filter area for better screen use.
- Page number in PDF Reader now respects start page number in BibTeX
- Improved citation editor control.
- Improved annotation editor control.
- Vastly improved title recognition for better BibTeX Sniffer results.
- Can move PDFs between libraries.
- EZProxy support.
- Can toggle appearance of SpeedRead shadow text.
- Improved printing of Brainstorms.
- Fixed the 'bug' where a new PDF in the watch folder 'steals' the currently selected focus.
- Much faster addition of 1,000s of PDFs.

Version 65:
- Share PDF annotations via social media.
- Sort tags, authors, etc. in the library filter by frequency.
- Web page HTML to PDF conversion is now completely inside Qiqqa, so it is more stable and has better features.
- Further integration with Datacopia.com to automatically create beautiful charts from tables of results.
- Improved stability for import where Mendeley points to broken files.

Version 64:
- BibTeX Sniffer supports international characters
- Libraries open much more quickly - especially libraries with more than 10k+ documents.
- Importing PDFs is much faster.
- Batch importing of PDFs no longer aborts after the first missing PDF: instead a report of errors is offered at the end of the import.
- Now supports 'ridiculously long filenames'.
- Updated the bundled Firefox browser to latest version - improves stability of internal web browser.

Version 63:
- Integration with Datacopia.com to automatically create charts from tables of results.
- *SpeedRead* your way through PDFs at up to 1000 WPM.  Awesome!
- Massive improvements to brainstorm for PDF document nodes and their annotations.

Version 62:
- Integration with Datacopia.com to automatically create charts from tables of results.
- Support for Bundle Libraries.
- Better author name disambiguation
- Vastly improved brainstorm automatic layout algorithm
- Thumbnail PDF pages
- Jump to the library containing a PDF

Version 61:
- You can automatically create a BibTeX record from the suggested metadata.
- Pressing CTRL-; in the BibTeX editor will add the current date (useful for the accessed field of a website record).
- Better instructions on how to cancel a Watch Folder.
- Improvements to the Import feature.

Version 60:
- When you now explore Documents or Themes in a brainstorm (right click and explore), you get some pretty epic pictures of your library.
- The BibTeX editor now offers the legal-case type.
- InCite now has separate toolbar buttons for citing (Author, Year) and Author (Year).
- PDF pages are now centered in the reader.
- The Qiqqa InCite popup can now also be activated using Win-A as well as Win-Q.

Version 59:
- You can sync to Intranet libraries even when not connected to the Internet.
- You can rotate all pages at once.
- Patch: Fixes the sync problem if you don't have a proxy.

Version 58:
- German and Turkish translations.
- Patch: InCite maps the "article" type to CSL "article-journal" so that journal and page numbers are included in the bibliography.

Version 57:
- Search queries are remembered.
- Can log in with default network proxy credentials.
- 50Gb Web Libraries for Premium+ members.
- Annotation report is ordered according to Library Screen.
- All CSL document types and location types are supported.
- Highlight rendering speed dramatically improved for long documents.

Version 56:
- Documents can have colours associated with them.
- Document title and size is coloured and sized accouring to reading stage and rating.
- Fixes Word connectivity on computers where Word has been incorrectly installed and uninstalled.
- All Document metadata is visible at once on tall screens.
- Can mass-rename publications
- F11 goes full-screen while reading PDFs
- Fixed a memory leak while reading large PDFs.

Version 55:
- Premium Fields allow you to restrict your searches to ANY of the fields that you have added in your PDF BibTeX records.
- Qiqqa now supports the Bluebook legal CSL style.
- Qiqqa supports the "short form" of journal name in your bibliographies.
- You can refresh the Annotation Report for a PDF or jump straight to a full blown Annotation Report.
- Qiqqa warns about DropBox conflicts.

Version 54:
- Added the Qiqqa Manual.

Version 53:
- Compatibility with Word 2013.
- Drag and drop PDFs straight onto Start Page libraries.
- Can export Expedition themes to text.
- Can sort PDFs by page count.
- The BibTeX types incollection inproceedings inbook chapter all map to CSL chapter type

Version 52:
- Qiqqa Champion Project.
- Support for online CSL editor.
- Individually control highlight, annotation and ink transparencies.
- Better feedback from InCite when you have a problematic reference.
- Library export has an HTML summary.
- Annotation reports are not automatic if they are too large.
- Can rebuild indices corrupted by DropBox, GoogleDrive, etc.

Version 51:
- Automatic PubMed support in BibTeX Sniffer.
- OCR supports several European languages.
- Adding a non-PDF reference automatically popup up the BibTeX editor.
- Favourites now show up with hearts in library view.
- Can explore libraries in brainstorm.
- InCite is much faster at updating references in Word.
- BibTeX Sniffer editor window is resizable.
- Better keyboard shortcut handling.
- Better zoom behaviour.

Version 50:
- Umbrella-search across all your libraries.
- Can expand most INFLUENTIAL and most SIMILAR papers for a theme in brainstorm.
- Smarter BibTeX Sniffer that highlights author names for fast cross-check.
- Smarter BibTeX Sniffer Wizard.
- Tag filters now have checkboxes for more intuitive multi-select.
- PDF preview popup has Expedition Themes.
- Can copy BibTeX key from PDF Reader screen.
- Can sort library by whether or not a document has associated BibTeX.
- You can now choose between using Tags, AutoTags or both when building your Expedition.
- "CSV database" of metadata added to Library Export.

Version 49:
- Supports read-only Web Libraries.
- Supports read-only Intranet Libraries for Premium members.
- Can import legacy PDF annotations and highlights.
- Qiqqa remembers screen location at shutdown.

Version 48:
- Libraries can be filtered by Expedition theme.
- The filter graph shows more columns.
- You can create Web Libraries from within the Qiqqa Client.
- Stability:
  + Fixes more issues around Firefox DLL clashes.

Version 47:
- Remembers last page N-up settings.
- Deleted PDFs no longer appear in cross references.
- Stability:
  + Fixes issues with clients with non-standard DPI display settings (highlighting positions do not match text).
  + Fixes issues with PDFs with corrupted XRef tables.
  + Fixes some issues around Firefox DLL clashes.

Version 46:
- Win+Q key combination brings up a mini-InCite screen so that you can easily add citations to Word.
- Expedition theme colours are visible throughout Qiqqa.
- You can print from the Browser.
- Performance improvements to the PDF renderer and to startup time.

Version 45:
- Qiqqa Premium+
- Qiqqa now supports Intranet Libraries, where you can sync completely internally to your corporate intranet.
- Automatically convert Microsoft Word files to PDF.
- You can delete duplicates in the duplicate detection screen.
- Brainstorms can automatically neaten themselves.

Version 44:
- The built-in browser uses Firefox, not Internet Explorer.
- Browsing a PDF document in the built-in web browser will automatically add it to your Guest library and offer for you to move it.
- Can turn on automatic synchronisation, so you never have to worry about forgetting to sync before leaving the office.
- Can add InCite fields to floating text areas too.
- InCite fields are locked in Word by default so that you don't need Qiqqa on a computer to maintain the contents of the fields.
- Can run a report to see which documents you have cited in InCite.
- Can open a document just by moving the mouse cursor over it in Word and pressing ENTER in Qiqqa InCite.
- BibTeX abstracts are shown in abstract sidebar when present.
- Uploads of large PDFs work over slow connections.
- You can cite Author (date) and (Author, Date) from the PDF library and PDF reader menus.
- Better duplicate detection with sort.
- Clearer PDF rendering.
- Improved keyword cloud using autotags.
- Massive all-round speed enhancements - now tested with libraries of up to 15,000 documents.

Version 43:
- NB: THIS VERSION REQUIRES AN AUTOMATIC UPGRADE TO MICROSOFT'S .NET4 FRAMEWORK if you are running an earlier version of Qiqqa than v40.
- NB: So only update if you have about 30 minutes and no urgent project deadlines.
- You can now see all duplicates across your library.
- Highlight/select/ink tools are below the toolbar items.

Version 42:
- NB: THIS VERSION REQUIRES AN AUTOMATIC UPGRADE TO MICROSOFT'S .NET4 FRAMEWORK if you are running an earlier version of Qiqqa than v40.
- NB: So only update if you have about 30 minutes and no urgent project deadlines.
- The start page now shows the coverpages of your recommended reading.
- You can turn off secure SSL communication with the Qiqqa Cloud.
- Qiqqa warns you if you cite a document that has a duplicate or a blank BibTeX key.
- Qiqqa now remembers which libraries and documents you last had open.
- Can purge deleted files from the config screen.

Version 41:
- NB: THIS VERSION REQUIRES AN AUTOMATIC UPGRADE TO MICROSOFT'S .NET4 FRAMEWORK.
- NB: So only update if you have about 30 minutes and no urgent project deadlines.
- Can cite documents from pdf reader and annotation report.
- Added "id" field to library catalog.
- Prettier library catalog layout.
- Fixed delayed update in BibTeX editor.
- Tidier menu bars.

Version 40:
- More brainstorm features - group select, better node represenations.
- Library export adds file and filename fields to BibTeX.

Version 39:
- Menus in French, German, Mandarin Chinese, Polish, Portuguese & Taiwanese.
- Qiqqa now supports password-protected PDFs.
- 8Gb storage for Premium Members.
- "Webpage" InCite and BibTeX type.
- Better highlight extraction in annotation reports.
- Performance improvements - at startup, when generating AutoTags and when generating Annotation reports.

Version 38:
- NB: This version will require an update to your Qiqqa database,
- NB: so although we have heavily tested this release,
- NB: be prudent and don't upgrade if you have a looming hard paper deadline... :-)
- You can associate PDFs with Vanilla References.
- Right-click copy BibTeX keys for fast pasting into LaTeX.
- Speed improvements at startup and during sync.
- Fixed the InCite SURNAME p. (DATE p.) issue.

Version 37:
- Built-in CSL style editor.
- You can now look inside your library search results without having to open each PDF.
- You can now contribute to the translation of Qiqqa into your own language.
- Expedition details in PDF Reading screen.
- Speed improvements to OCR.

Version 36:
- Qiqqa Expedition - Qiqqa helps you understand your research literature landscape.
- Ink annotations now show up in Annotation Report.
- Can include abstracts in Annotation Report.
- Brainstorm support multiline text.
- Open PDF multiple times.
- Improved highlighting mode.
- New webcasts for Brainstorms and Expedition.

Version 35:
- Premium Membership get you 1Gb of free Web Library space.
- Themed colours for Qiqqa.
- Qiqqa help forums.
- New Qiqqa features webcast.

Version 34:
- Awesome annotations summaries in side-bars.
- Better search results.
- Can reverse your sorts with 2nd click.
- Paper abstracts are automatically extracted.
- A great new introductory webcast by the McKillop Library.
- InCite tidies up spurious spaces around citations.

Version 33:
- Support for Qiqqa for Android!  Please make sure you upgrade to at least Qiqqa v31 on ALL your computers...
- DON'T UPGRADE IF YOU HAVE A DEADLINE THIS WEEK - just in case! :-)
- Last reading page and bookmarks are remembered.
- You can filter to PDFs that have no tags at all.

Version 32:
- Share and email a document
- Faster opening of PDFs

Version 31:
- Can backup to ZIP file.
- Better BibTeX Sniffer wizard.
- Improved EndNote importing.
- Improvements to Start Page.
- Additional webcasts and helper tips.
- Better folder watching.

Version 30:
- Qiqqa InCite - copy single citation snippets to the clipboard for pasting into OpenOffice and emails.
- More powerful search result ranking by relevance.
- Duplicate detection with indicator warning.
- Bookmarks while reading your PDFs so you can jump back-and-forth to the bibliography.

Version 29:
- Qiqqa InCite released - including dependent CSL styles.
- More powerful library search facility with quoted, boolean, proximity, fuzzy and field searches.
- WARNING: Will automatically rebuild your document indices, so please be patient for a few minutes.

Version 28:
- Faster PDF text extract and index
- You can force OCR on pages where the embedded text is corrupt in the PDF.
- Qiqqa Library export to directory.

Version 27:
- Qiqqa InCite BETA improvements (more styles, Word XP support, error messages).
- Webcasts introducing and demonstrating some of the Qiqqa functionality.

Version 26:
- Qiqqa InCite, Qiqqa's Microsoft Word bibliography management system.
- Web proxy support.

Version 25:
- Full screen reading mode.
- Recommended reading on Start Page.

Version 24:
- Full BibTeX editor.
- Better document metadata editors.
- Document metadata editors available from document library.
- Speed improvements.

Version 23:
- Explore your documents using brainstorms.
- Better BibTeX sniffer.
- Multicoloured text highlighting.
- Printing now contains your annotations and highlights.
- Better PDF text clarity.
- More EndNote article export types.
- Qiqqa Premium features.

Version 22:
- Improved tablet support.
- Performance and other minor enhancements.
- WARNING: Will automatically rebuild your document indices, so please be patient.

Version 21:
- Unlimited libraries.
- Sharper PDF rendering.
- Cross references for your papers.
- Improvements to the BibTeX sniffer.

Version 20:
- A lot of new PDF search functionality.
- Can copy and delete multiple PDFs in the library catalog.

Version 19:
- Import from EndNote, Zotero, Mendeley.
- Import PDFs recursively from folders and tag them with their folder names.
- Ink annotations for tablets.

Version 18:
- The first installer version of Qiqqa available for beta testing!

