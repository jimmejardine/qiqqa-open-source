Upcoming release: v82
=====================

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.






2019-11-03
----------

			
* (5e787c1) bumped build revision
			
* (6a8dda0) don't just wait 10 seconds when extracting a Library bundle. It may be huge or you're running on a slow box and you'll get a b0rked extract. Just let 7ZIP complete.
			
* (4184e0c) fix typo
			
* (7d4bcab) Disable more unused source code files
			
* (3820886) Bit more refactoring work for https://github.com/jimmejardine/qiqqa-open-source/issues/95
			
* (d927e2b) fix lingering crash in Dispose method. Follow-up for commit SHA-1: d05bbe2da06b825a0a079a73e14543f3af282165
			
* (f37c9dc) https://github.com/jimmejardine/qiqqa-open-source/issues/95 : turns out most of it had already been done in the original Qiqqa. Upon closer inspection the remaining `Process.Start()` calls are are intended to open an (associated) application for a given file or directory, which is proper.
  
  Added a few `using(...)` statements around Process usage, etc. to prevent memory leaks on these IDisposables.
			
* (d05bbe2) More work related to commit SHA-1: 43b1fe0972f99660e0bbbeea2deb357b2002f190 : fix crashes at application shutdown
			
* (06bddf6) Maintainable/MaintainableManager: refactor the shutdown code + correct the code to use non-skippable SafeThreadPool threads to Stop/Abort pending threads.
			
* (cef12a8) disable unused code files
			
* (43b1fe0) 
  - Fix spurious crashes in `Dispose()` methods; these happen when terminating the application at special moments, e.g. while it is still loading the libraries on init (Hit Alt-F4 while the busybee cursor is still active and you might've run into a few of these, f.e.)
  - Make sure all `MessageBox.Show` actions go through the `MessageBoxes` class
  - Make sure every `MessageBox.Show` is executed from the UI thread, even when its wrapper was invoked from a background thread
			
* (148ea94) fix https://github.com/jimmejardine/qiqqa-open-source/issues/126
			
* (016b888) fix https://github.com/jimmejardine/qiqqa-open-source/issues/132
			



2019-11-02
----------

			
* (c06021a) bumped build revision
			



2019-11-01
----------

			
* (4aaa736) Merge remote-tracking branch 'remotes/jimmejardine-original/master' into v82-build
			
* (5486dca) Merge pull request #125 from gitter-badger/gitter-badge
  
  Add a Gitter chat badge to README.md
			
* (112ce55) Add Gitter badge
			
* (c757069) updated CHANGELOG_full.md
			
* (54bf3a8) bumped build revision (`npm run syncver`) and cleaned some code - no functional changes in this commit
			
* (95dff9b) fix b0rk introduced by commit SHA-1: bcd73cd877b72cd2b9aba9183172dd6c46590880 :: we don't do a *revert* action per se, but rather improve upon the patch we picked up there from the experimental branch: as it turns out, the patch caused a lot of trouble which has been resolved to allow the running background task(s) access to a reduced clone of the WebLibraryDetails, which does not exhibit the cyclic dependency that the original WebLibraryDetails instance incorporated, thus breaking the cyclic reference and allowing the .NET GC to do its job on the Library instance(s) ASAP.
  
  As this problem was discovered while doing work on commit SHA-1: ed2cb589a2e3562102163c4b3129310c4850e33a, these files also include the remainder of the work done for that commit, as it was important to separate out the patches which fixed the cyclic memory reference.
			
* (ed2cb58) ran the entire codebase through DevStudio's Analyze->Code Cleanup->Run Profile 1, where Profile 1 was set up to include these:
  
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

			
* (bcd73cd) picked up memleak fix from experimental branch
			
* (2a72cbd) Added a couple of RIS + BibTeX test files to the test set.
			
* (fe33552) More work in line with commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0
  - fixing the last bit of work done in that commit where all the PageLayer-derived classes were fitted with IDisposable interfaces. Trouble is that the cleanup runs over a loop which accesses these instances via a PageLayer baseclass cast.
  - fixing several more DevStudio Code Analysis reports regarding Closed/Dispose handling of a few XAML controls; global Code Review used to update all OnClosed() handlers and treat them as we did the Dispose() code: NULL and Dispose/Clean what we can.
  - fixed crash in PageLayer-derived classes' Dispose, where the access to the `Children` member would throw a cross-thread-access error exception ( https://stackoverflow.com/questions/11923865/how-to-deal-with-cross-thread-access-exceptions ) Weird stuff happens as it looks like there are multiple Dispatchers in Qiqqa from this perspective, which is ... odd. (And, no, I'm not a WPF expert so it may be lack of understanding here.)
			
* (ce62133) DateVisible attribute has already been renamed and set in code via SetDatesVisible() in commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0
			
* (8258809) Let's see if the SyncFunction related UI hacks by Jimme Jardine are still needed: we already disabled those empty style classes in the previous commit SHA-1: ff6e4eebfc40d072d0b37df3a950dd15681fcfc0 ; now we take them out in the UI XAML definitions.
			
* (ff6e4ee) Another Mother Of All commit with loads of stability & memleak + performance improvements work:
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

			
* (90364ad) more work done on https://github.com/jimmejardine/qiqqa-open-source/issues/112 / https://github.com/jimmejardine/qiqqa-open-source/issues/122 :
  - memleak prevention via `using(){...}` of IDisposable
			
* (bb0fde8) fix infinite call depth due to incorrect polymorphic interface use.
			
* (f8f64ce) more work done on https://github.com/jimmejardine/qiqqa-open-source/issues/112 :
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

			
* (bc2955a) Log file paths MAY contain spaces, hence surround those in double-quotes as well for the BundleLogs command which invokes 7zip on the commandline.
			
* (3f8d443) Cleaned up the log4net init code and finally made it truly threadsafe: it turned out several threads where already trying to log/init while the log4net init process was not yet complete. `Logging.TriggerInit()` has been redesigned to check if the init phase MAY be triggered, while the init phase itself will write any 'early bird' log lines to the logging destination as soon as it is completely configured. Also removed the diagnostic init stacktrace dump to log: that one only still happen in DEBUG builds.
			
* (1955a86) moving PDF Region Test UI code+XAML to the QiqqaUIPartsTester project where such test code should reside.
			



2019-10-25
----------

			
* (aedcd38) BeyondCompare 4 cannot handle UNC paths, which is what AlphaFS uses to support overlong paths (> 260 chars), hence we'll have convert these UNC paths back to local/native format for BC4 to be able to open the received+approved files for comparison. The rest of the test application should use the UNC paths though.
			
* (3e06f8a) fix crash when running test code:
  
        Message:
          System.NullReferenceException: Object reference not set to an instance of an object.
        Stack Trace:
          Runtime.get_IsRunningInVisualStudioDesigner() line 23
          UnitTestDetector.get_StartupDirectoryForQiqqa() line 91
			
* (9a928b0) updated NuPackages and added missing file from QiqqaUIPartsTester project
			
* (17135d7) fiddling with the UnhandledExceptionBox in a first attempt to add it to the QiqqaUITester. Turns out I'll have to take another approach there... :-\
			
* (41019bf) 
  - bumped + synced new build revision (v82pre4 coming up)
  - added QiqqaUIPartsTester test project - to be filled with UI test code to check the functioning of various dialogs and controls
  - added Microsoft-suggested code analyzers to the project packages (DevStudio 16.3.5)
			



2019-10-22
----------

			
* (d1bc1a3) help performance checks/debug sessions: allow user access to the DisableBackgroundTasks config setting, but DO NOT persist that setting. (also we must remain backwards compat with v79 file formats for now)
			
* (b3fae39) https://github.com/jimmejardine/qiqqa-open-source/issues/82 : try to set up some decent min/max sizes for the various controls/wrappers so that huge BibTeX raw content doesn't produce unworkable UI layouts and thus bad UX. MinWidth limits are meant to restrict button scaling and thus ugly/damaged looking UI, while MaxHeight limits are intended to limit the height of the BibTeXEditorControl when it is fed huge BibTeX raw content, such as when loading a BibTeX from converted PubMed XML (the source XML is appended as a multiline comment which often is very large)
			
* (868e549) cleanup for https://github.com/jimmejardine/qiqqa-open-source/issues/82 : all the AugmentedButton instances which DO NOT need scaling (at least not yet in our perception of the UI) are AutoTextScale=false(default) configured. The ones which need to scale to remain legible at various screen and panel sizes are marked AutoTextScale=true.
			
* (2e4bce8) Add code to prevent memleaks around BibTeXEditorControl : there's no Dispose there, but we do have Unload event, which marks the end of a control's lifetime.
			
* (3248ea9) add validation check for https://github.com/jimmejardine/qiqqa-open-source/issues/119 : when we encounter a ClosableControl which doesn't have a name, it should be added as that will be needed for the check/persist path construction.
			
* (e2d8430) Tweaks to fix https://github.com/jimmejardine/qiqqa-open-source/issues/57 : handling extremely large (auto-)titles, etc.  Bummer: turns out the "Summary Details" in the right panel (QuickMetaDataControl) cannot be ellipse-trimmed like this as those are editable entities -- I didn't know (long-time Qiqqa user and still not aware of all the feature details \</snif>)
			
* (3ce449e) stability: we ran into the 'waiting after close' issue again ( https://github.com/jimmejardine/qiqqa-open-source/issues/112 ); PDF/OCR threads got locked up via WaitForUIThreadActivityDone() call inside a lock, which would indirectly result in other threads to run into that lock and the main thread locked up in WM message pipe handling triggering a FORCED FLUSH, which, under the hood, would run into that same lock and thus block forever. The lock footprint has been significantly reduced as we can do now that we already have made PDFDocument largely thread-safe ( https://github.com/jimmejardine/qiqqa-open-source/issues/101 ).
  
  Also moved the Application Shutting Down signalling forward in time: already signal this when the user/run-time zips through the 'Do you really want to exit' dialog, i.e. near the end of the Closing event for the main window.
  
  `WaitForUIThreadActivityDone` now does not sit inside a lock any more so everyone is free to call it, while in shutdown state, when the WM message pipe is untrustworthy, we leave the relinquishing to standard lib `Thread.Sleep(50)` to cope with: the small delay should be negligible while we are guaranteed to not run into issues around the exact message pipe state: we also ran into issues invoking some flush/actions via a Dispatcher while the app was clearly shortly into shutdown, so we try to be safe there too.
  
  Also patched the StatusManager to not clutter the message pipeline at shutdown by NOT showing any status updates in the UI any more once the user has closed the app window. The status messages will continue to be logged as usual, we only do *not* try to update UI any more. This saves a bundle in cross-thread dispatches in the termination phase of the app when large numbers of pending changes and/or libraries are flushed to disk.
			
* (930fdb3) cleanup after commit SHA-1: 6f95dd688751fbcef0eb1c87ed8b7fd30cca863a : remove now useless debugging code.
			
* (6f95dd6) continuation of commits 3e82bdac0b92feca47f4c5ba1dc5261039804de5 + 337dc9ed64c65bd4144dbae579a47d915f986ad8 : make the compiled target directory tree available to the DevStudio Design View to grab external files, e.g. the splash screen image.
			
* (3e82bda) previous commit already has a newer CSPROJ anyway and missed the JS script which did the work: here's the corresponding JS file
			
* (337dc9e) Design View: there's no way to obtain the original build directories as VS2019 crates shadow copies of the binaries when executing them in the context of Microsoft Blend / Visual Designer / XAML Design View. The initial hack attempt, after all other 'regular' efforts failed, was to create a Settings.settings file in the Utilities project (that's where DevStudio stores the Project->Properties->Settings you set up) and add a PreBuild script which patches that file. HOWEVER, it turns out this data is copied all over the place by DevStudio: you must also patch `app.config` file to ensure DevStudio doesn't yak about change to update and alert for every single entry. THEN you find PreBuild is **too late** to have DevStudio regenerate the `Properties/Settings.Designer.cs` file. So we log this intermediate failed result and move on towards patching one of our existing files in the project instead: Constants.cs (will be filed in the next commit)
			
* (a20685b) working on a fix for https://github.com/jimmejardine/qiqqa-open-source/issues/57 : part 1 = making the statusbar updates more usable by truncating them. Had a long url being reported for downloading which pushed all other messages off screen. :-1:
			
* (458cd2e) performance / shutdown reliability: when closing the app, the UI message may be loaded severely and may not even terminate properly in spurious circumstances (had a situation here where the WaitForUIActivityDone calls were tough to shut up for a yet unidentified reason; happened after testing several Sniffer PDF download actions so we may be looking at another hairy bit of the XulRunner/Browser interaction here). Also add a check to the PDF/OCR thread queue processing code to help hasten shutdown behaviour.
			
* (52d6dd8) splash page: barf a hairball when the splash page doesn't load as that surely means the Qiqqa install failed or got buggered somehow.
			
* (6d8812b) Splash page image is not loaded in MSVS Design View, among other resources. Working towards getting the bloody Design View in Visual Studio to work after all...
			



2019-10-21
----------

			
* (bfa90ed) more work for the https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring: as the AugmentedButtons need to scale down their icons and **text** when they're part of a resizable panel in order to remain readable, we need to implement that and ensure it only happens where and when we want. TODO: fix more panels with AugmentedButton nodes as they are now already scaled down due to WPF autosize actions which are not meant to do that.
			
* (106d954) code reformatting: no functional change.
			
* (ace5458) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/119: closables should have a name so we can create an identification path for them by traversing the WPF/UI tree: every such path can be checkec and hidden/shown individually, even persisted across runs.
			
* (68ad1bb) added a couple of timers to measure the time spent in indexing the libraries
			
* (8393c55) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/82 : refactored the BibTeXEditorControl and all its users: those must now provide the toggle/view-errors/etc. buttons for the control, so that the parent has full control over the layout, while the BibTeXEditorControl itself will set the icons, tooltip, etc. for each of those buttons to ensure a consistent look of the BibTeX editor buttons throughout the application.
  
  TODO: see if we need to discard those registered buttons in the Unload event to ensure we're not memleaking...
			
* (acc4357) add icons for BibTeX control et al: complete work started in commit SHA-1: 0f9fa67e470acb834e24cd30e946a0b71e954818 :: adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.
			
* (0cd9ea6) fix crashes in MSVS Design Viewer due to some properties not having 'get' access methods (as reported in the Exceptions thrown in the Designer View)
			



2019-10-20
----------

			
* (7dd8a59) MSVS Visual Designer fixup work: making sure the Theme stuff gets loaded timely when loaded from the Designer too.
			



2019-10-19
----------

			
* (e38715c) correcting omission of commit SHA-1: fd8326e1e7c4878aac9ca8a1d903c7404fe7b90d * https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits. Includes some minimal prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/87.
			
* (26ab3dd) fixing unfortunate edit oopsie: this part of the fixes/changes hadn't made it through in the previous fix commit for https://github.com/jimmejardine/qiqqa-open-source/issues/114 / https://github.com/jimmejardine/qiqqa-open-source/issues/115 : this corrects/augments these commits: SHA-1: 65e5707afa4e7e18181d00ef9c22b12048483b5e + SHA-1: a5faaafa233a181a47735f2e8981b6089c8ceaf7
			
* (fe35dbc) fix https://github.com/jimmejardine/qiqqa-open-source/issues/116 : show left panel when switching the app from novice into export mode.
			
* (f7d7bce) re-enable the 'Google Scholar' similar-documents panel in the PDF Reader left pane -- this is where most of the scrape info lands. ( https://github.com/jimmejardine/qiqqa-open-source/issues/114 / https://github.com/jimmejardine/qiqqa-open-source/issues/115 / https://github.com/jimmejardine/qiqqa-open-source/issues/117 )
			
* (761b192) a fix for one path we didn't get for https://github.com/jimmejardine/qiqqa-open-source/issues/106 + be more strict in checking whether a Web Library sync directory has been properly set up.
			
* (e6ee95f) OCR/text extractor: blow away the PDF/OCR queue on Qiqqa shutdown to help speed up closing the application (related to https://github.com/jimmejardine/qiqqa-open-source/issues/112). Also add thread-safety around the variables/data which cross thread boundaries.
			
* (27882ca) reduce a couple of now unimportant log lines to debug-level.
			
* (aec562b) performance: remove a couple of lock monitors which we don't need any more.
			
* (0f7df40) added one more test file for the BibTeX parser/processing: its surrounding whitespace in several fields should be cleaned up.
			
* (c4d7b6e) upgrade the HtmlAgilityPack package used by Qiqqa. This is required for https://github.com/jimmejardine/qiqqa-open-source/issues/114 + https://github.com/jimmejardine/qiqqa-open-source/issues/115
			
* (fd8326e) https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits. Includes some minimal prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/87.
			
* (0f9fa67) adding icons as part of https://github.com/jimmejardine/qiqqa-open-source/issues/82 refactoring/rework of the BibTeX related UI bits.
			
* (1e3edb5) some minimal code cleaning and dead code removal
			
* (a5faaaf) fix https://github.com/jimmejardine/qiqqa-open-source/issues/115 : PDF Reader (which does a Scholar Scrape) does not work for users living outside US/UK. Also further fixes https://github.com/jimmejardine/qiqqa-open-source/issues/114. Also fixes https://github.com/jimmejardine/qiqqa-open-source/issues/117 by enforcing UTF8 encoding on the content: we're downloading from Google Scholar there, so we should be good. Google Scrape finally finds decent titles, author lists and even PDF download links once again.
  
  TODO: update the 'Google Scholar' view part in the PDFReader control.
			
* (65e5707) fix https://github.com/jimmejardine/qiqqa-open-source/issues/114 as per https://stackoverflow.com/questions/13771083/html-agility-pack-get-all-elements-by-class#answer-14087707
			



2019-10-18
----------

			
* (9a1cdf9) fix typo in previous commit - this was already wrong in the experimental branch  :-(
			
* (ba9dcd5) typo fixes in comments and one function name. Changes ripped from the experimental branch.
			
* (bc6b2b5) Add files via upload
			
* (4ebc4df) Create How to locate your Qiqqa Base Directory.md
			
* (734ea88) make sure all NuGet packages reference .NET 4.7.2 - edit ripped from the master branch
			
* (8bc773c) document the node/npm development environment, etc. in DEVELOPER-INFO.md
			
* (fd67219) added `superclean.sh` bash shell script to help clean up a Visual Studio environment for when you want to make sure you're starting Visual Studio *sans prejudice*.
			
* (7566098) added DEVELOPER-INFO.md and pointed README.md at that document for info for developers wishing to work on Qiqqa.
			
* (e2bef9a) update CHANGELOG
			
* (ac04266) setup/installer: make the generated `setup.exe` should the same full build version as the Qiqqa executable itself.
			
* (1f6b9df) fix for setup/installer: set the `setup.exe` file version (as reported by Windows on right-click->Properties) to the Qiqqa release version instead of 0.0.0.0
			
* (6bb41ab) rebuild uninstaller
			
* (88538dc) regenerated approval reference files for all bibtex test data files after naming dedup fix in previous commit.
			
* (42c79d2) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : fix bug where the Approvals **namer** helper function did produce the same files for different tests (because their test data files only differed in file extension, e.g. `.bibtex` vs. `.biblatex`)
			
* (288cbc7) fix for `npm run refresh-data` build task: now generator includes code to cope with SHA-1: f3ed2f9640088de33cd58ed97b18891a157b4667 where we had added a comment containing one of the markers we're looking for in the code.
			
* (f3ed2f9) fixed unit test file for empty set for a DataTestMethod which is only there for developers to find out quickly after `npm run refresh-data` which test files have been properly employed and which have not.
			
* (6f3fa3f) fixed bugs in `npm run refresh-data` build task: properly list those test data files which have not been used in any Unit test yet.
			
* (448e135) add testfiles to TestData/... from elsewhere in the source tree: make sure all test files are in /TestData/...
			
* (7deb4ac) introducing build task `npm run refresh-data` to help update the test C# files with the latest data set from TestData/...
			



2019-10-17
----------

			
* (2295adc) rebuild uninstaller
			
* (b0e80fb) added these build tasks: version sync: `npm run syncver`; bump release version: `npm run bump`
			
* (0462316) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : removed the data fixture files from the test projects themselves.
			
* (5c2ea6c) https://github.com/jimmejardine/qiqqa-open-source/issues/111 : moving all test data references, etc. data files to /TestData/... tree
			
* (a31480d) adding nodeJS based script to help update/sync/bump Qiqqa software release versions. Also added node/npm-style package.json file to the project: this carries the master version info and helps us quickly setup the proper node environment on any developer box.
			
* (d4730ed) prepwork for the version patch / update / bump build task: manually update the copyright and version tags everywhere. Also added the ClickOnceUninstaller project (used by Inno Setup when building the Qiqqa installer) to the MSVS solution.
			



2019-10-16
----------

			
* (cb0b053) Merge branch 'mainline-master' into v82-build
  
* (2cdd342) Merge pull request #110 from GerHobbelt/mainline-master
  
  update README with latest info about Qiqqa software releases.
			
* (81472e1) Update README.md
			
* (6c1d8d6) update README with latest info about Qiqqa software releases.
			
* (6ccb147) updated CHANGELOG_full.md
			
* (acd9229) README: point at the various places where software releases are to be found.
			
* (bf06d31) Reorganize the README: developer info comes last.
			
* (db46fe8) updated README to mention how to set up Long FilenameSupport on your Windows developer machine.
			
* (6470d49) fixup for commit SHA-1: 0cf15c0d4d9377e80ddafd3063cbef038701bb3e -> add missing fixtures (auto-saved reference files) -> https://github.com/jimmejardine/qiqqa-open-source/issues/108
			
* (76f4910) fix unit tests: support Long Paths (>= 260 chars in length), i.e. UNC paths, by using AlphaFS `Path.GetLongPath()` API in the Approvals test rig.
			
* (0cf15c0) fixup for commit SHA-1: c2e37869acba0903bc9687c27b4887990297fd03 :: moving the BibTeX (et al) data test files to the QiqqaUnitTester project as this data belongs over-there: it is required by the unit tests for the BibTeX parser, etc. in the Utilities library/project. The QiqqaSystemTester can always link/reference these test data files later on when it is desired so. This is part of https://github.com/jimmejardine/qiqqa-open-source/issues/107, while the new 'approved' files are added as part of the work done on https://github.com/jimmejardine/qiqqa-open-source/issues/108.
			



2019-10-15
----------

			
* (190ff05) v82pre3 release.
			
* (ea7eca8) Disable part of the WeakEventHandler checker code as it gets fired due to come code triggering the check. TODO. (But for now I need a Qiqqa binary that at least works, pronto.)
			
* (fd882e7) performance / reduce memory leaks / reduce GC delays due to objects being marked by obsoleted status messages. Only remember the last message, hence no queue upkeep any more as we don't backpedal or otherwise use that status message queue of old *anyway*. Code simplification.
			
* (305c085) fixes for https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/98: correct deadlock prevention code in CitationManager and shallow-copy the Attributes of a DictionaryBasedObject.
			
* (6ef9b44) 
  - refactoring the PDFDocument code and making its interface threadsafe (apart from the inks, annotations and highlights attribute lists). Fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96.
  - improve the internal code of `CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS()` to help ensure we will be able to copy/transfer all metadata from the sourcing document to the clone. Code simplification.
  - code simplification/performance: remove useless LINQ overhead
  - `SignalThatDocumentsHaveChanged()`: improve code by making sure that a reference PDF is only provided when there really is only a single PDF signaled! Also see the spots where this API is being used.
  - belated commit of AlphaFS upgrade/refactoring for https://github.com/jimmejardine/qiqqa-open-source/issues/106 ; a multi-pass code review has been applied to uncover all places where (file/directory) paths are manipulated/processed in the Qiqqa code. TODO: Streams.
  - redesign the ADD/REMOVE/GET-password APIs for PDFDocument as the indirection in the Qiqqa made it bloody hard to make thread-safe otherwise. (Fixes https://github.com/jimmejardine/qiqqa-open-source/issues/96 / https://github.com/jimmejardine/qiqqa-open-source/issues/98)
  - ditto for the GC-memleaking bindables of PDFDocument: all the binding stuff has been moved to the PDFDocument thread-safe wrapper class. (TODO: investigate if we can use WeakReferences more/better to prevent memleaks due to infinitely marked PDFDocument instances...)
  - tweak: `AddLegacyWebLibrariesThatCanBeFoundOnDisk()`: at startup, try to load all .known_web_libraries files you can find as those should provide better (= original) names of the various Commercial Qiqqa Web Libraries. (This is running ahead to the tune of https://github.com/jimmejardine/qiqqa-open-source/issues/109 / https://github.com/jimmejardine/qiqqa-open-source/issues/103)
			
* (d6f173d) cf. SHA-1: 4c92feec44c362f4daaa4b8757f5e66aaff7359d * belated commit fixing code typo due to AlphaFS upgrade/refactoring in commit SHA-1: 7ecf0ae90d53d8961fefa25baa7b06d4bf319902 and https://github.com/jimmejardine/qiqqa-open-source/issues/106
			
* (c2e3786) moving the BibTeX (et al) data test files to the QiqqaUnitTester project as this data belongs over-there: it is required by the unit tests for the BibTeX parser, etc. in the Utilities library/project. The QiqqaSystemTester can always link/reference these test data files later on when it is desired so. This is part of https://github.com/jimmejardine/qiqqa-open-source/issues/107, while the new 'approved' files are added as part of the work done on https://github.com/jimmejardine/qiqqa-open-source/issues/108.
			
* (f25df6c) Created new Test project for Utilities,etc. library unit tests, e.g. the BibTex parser tests. This addresses https://github.com/jimmejardine/qiqqa-open-source/issues/107. Also tweak/augment Approvals' helper classes to improve the tester UX as per https://github.com/jimmejardine/qiqqa-open-source/issues/108. The `Approver` now **saves the current test result to the 'approved' reference file when that one is not present** and then (obviously) does not invoke Beyond Compare or your favorite comparison tool. The first SO/Approvals provided solution still invoked BC for every auto-updated test, which was a HUGE nuisance. See the `ApprovalTestsConfig.cs` file for more info on this. The current implementation also results in slightly leaner test code, which is a free boon.
			
* (4c92fee) belated commit fixing code typo due to AlphaFS upgrade/refactoring in commit SHA-1: 7ecf0ae90d53d8961fefa25baa7b06d4bf319902 and https://github.com/jimmejardine/qiqqa-open-source/issues/106
			
* (8b90f41) fix https://github.com/jimmejardine/qiqqa-open-source/issues/105 : turns out the config user GUID is NULL/not initialized at startup. This fix ensures there's a predictable (Guest)config active from the very start of the Qiqqa app.
			
* (d52d565) quick hack for https://github.com/jimmejardine/qiqqa-open-source/issues/104 + v82pre3: make Qiqqa start no matter what. EFF that T&C!
			



2019-10-13
----------

			
* (f515ca7) directory tree as tags" also recon with UNIX-style path separators on Windows platforms: the DownloadLocation is not guaranteed to carry only Windows/MSDOS style '\\' path separators.
			
* (626902b) (fix) customize library background image: copy the new file into place, if it is another file than the one we already have. Fix: do *not* delete the active image file when the user did not select another image file.
			
* (c66706b) `FileTools.MakeSafeFilename` already performs filename length sanitization. No need to do that twice in different places.
			
* (ba9b993) Add reference to source article of WeakEventHandler class + re-enable the proper use checks.
			
* (7ecf0ae) part of a larger work: use AlphaFS::FIle/Directory/Path.* APIs everywhere. Also use UNIX `/`instead of MSDOS `\\` where-ever possible.
			



2019-10-12
----------

			
* (db4908d) rename: fix typo in filename
			



2019-10-11
----------

			
* (3d3b6ca) code cleanup: remove one (semi)duplicate API
			



2019-10-10
----------

			
* (3b5dcc1) updated CHANGELOG_full.md to current commit
			
* (3a0544c) Updated README + CHANGELOG_full.md fixes for GFM
			
* (2832a07) tweak: DescriptiveTitle: trim every title to the default length + ellipsis.
			



2019-10-09
----------

			
* (deaadc3) refactoring work necessary for fixing https://github.com/jimmejardine/qiqqa-open-source/issues/96 & https://github.com/jimmejardine/qiqqa-open-source/issues/101
			
* (0db1848) whitespace police
			
* (1b2daca) Mother Of All commit with these fixes and changes:
  - added WPFDoEvents API for waiting for the UI thread to run its course (UI responsiveness), which is a strengthened version of DoEvents()
  - added WPFDoEvents APIs for mouse cursor Hourglass/Busybee override and reset: during application startup, this is used to give visual feedback to the user that the work done is taking a while (relatively long startup time, particularly for large libraries which are auto-opened due to saved tab panel sets including their main library tabs)
  - fixed a couple of crashes, particularly one in the RemarkOnException handler which crashed due to an exception being reported during application shutdown in one particular test run. (hard to reproduce issue, while we were hunting for causes of https://github.com/jimmejardine/qiqqa-open-source/issues/98 / https://github.com/jimmejardine/qiqqa-open-source/issues/96)
			
* (fb775d5) augment logging and take out the Sorax PDF page count API call due to suspicion of memleaking and heapcorrupting as per https://github.com/jimmejardine/qiqqa-open-source/issues/98 initial analysis report
			
* (ad57656) performance tweak: remove a thread lock monitor which is not important enough and which is loading the CPU (~6%); also reduce the critical section surface.
			
* (e353006) PDFRendererFileLayer: when calculating the PDF page count happens to fail 3 times or more (Three Strikes Rule), then this PDF is flagged as irreparably obnoxious and the page count will be set to zero for the remainder of the current Qiqqa run -- this is not something we wish to persist in the metadata store as different software releases may have different page count abilities and *bugs*
			
* (bd3e372) LockPerfChecker: fix name of caller to use to strip off the (useless) head lines of the stacktrace - which is included in the report when a lock happens to take longer than the timeout.
			
* (935a61f) fix for WPF: correctly detect iff we're running in the UI thread or in another thread.
			



2019-10-07
----------

			
* (f35ae1f) Revert "temporarily disable sorting - performance hogs - to see where the other perf bottlenecks hide out."
  
  This reverts commit b34abff27aa524c0397d5e6959bb600506292a29.
			
* (ceae5c6) removed the last vestiges of performance costing thread lock monitor code. At least as far as application startup is concerned, we're now back to the classic performance hogs: UI list filling and sorting...
			
* (b34abff) temporarily disable sorting - performance hogs - to see where the other perf bottlenecks hide out.
			
* (9eb3d59) add comments about purpose. (the tag sorting now seems to take the cake, performance-wise)
			
* (82c52dd) performance testing of startup behaviour: the next big think is ReviewParameters() but we cannot ditch that one as it initiates the (re)draw of the controls. What we can do is save a little time in superfluous code.
			
* (359b8d8) after rerun of performance test: now the topmost consumer is the thread lock monitor in SignalThatDocumentsHaveChanged(), or at least regarding thread lock monitors. The highest bidder overall is currently: Qiqqa.DocumentLibrary.Library::BuildFromDocumentRepository	7794 (47.61%)
			
* (072a7c6) Delayed the PDF Page Count calculation a bit: it's not yet needed in the constructor call, so delay until actually requested. Also clean up the PDF page count helper method(s) a tad.
			
* (dc1e5bd) app start performance test: second culprit was thread lock monitor code for TagManager. That code is harmless, so disabling the monitor code there.
			
* (171bf18) performance test: thread lock monitor for AugmentedBindable (which is invoked 40K+ for a large 40K+ lib as each PDFDoc has at least one of 'em) is eating the most. Disabling as that one is not suspect any more anyway.
			
* (4ab3f05) The GetCurrentJobToken() API could be simplified without any loss in functionality. Also here's the remainder of the threading work done in SHA-1: 5e5206244190a8c599b883d17529eb59101174ff. And the heuristics around OCR job queueing have been tweaked. Should work out better for (very) large libraries this way.
			
* (eba4472) Fix bug where it looked like Coty To Another Library menu choice and Move to Another Library memu choice didn't differ at all: it's just that MOVE did not properly signal the sourcing library needed an update as well as the document itself.
			
* (b109a92) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/96 by making sure that we pass a copy instead of a reference to the save logic. (**Incidentally, there are other thread crossings for pdf_document so we'll have to investigate this further as it's not just SAVE activity that's endangered by spurious crashes in annotation, tags or inks lists.**) Everybody should go through the QueueToStorage() API, by the way.
			
* (fb3de51) Feature Tracker: actually pick up the feature parameters and include them in the feature tracking info. Currently we don't store the featuretracking info (old Qiqqa had a Qiqq.utilisation file once) as we have DISABLED the GoogleAnalytics web interaction: that one was synchronous and only held up important activities.
			
* (1cc4d1f) Clone/Copy didn't carry the document metadata across to the new lib: CloneExistingDocumentFromOtherLibrary_SYNCHRONOUS() did not pass the URL, bibTeX, tags, etc. along so any action going through this API would copy only the PDF and minimal metadata. That is not what was intended, surely!?
  
  Also: the remainder of the tags is HashSet instead of List move.
			
* (55ce802) tags: migrate from List<> to HashSet<>: that immediately solves the problem of duplicate tags too! Apply throughout the codebase. Note that Library.cs has additional changes, hence that one is also part of this, but will be committed separately.
			
* (269a41d) statusbar progress bugfix: use the correct value as otherwise you'ld get a green bar with large number still to do.
			
* (6cc6e69) dial up the lock performance threshold for reporting from 100ms to 250ms: several log entries that are frequent now, ar indeed a bother, but not enough to merit being logged. We've got bigger fish to fry.
			
* (5e52062) background threads work: make sure all long running threads at least run at below-normal priority. Also ensure all stop-or-exit-due-to-disable-or-shutdown checks and logging is done at debug level (some of these changes will be committed in the subsequent commits as there's a mix of edit purposes in a few files)
			



2019-10-06
----------

			
* (9439b60) Cleaning up the logging action: the regular Debug activity is relegated to special builds which have the `DIAG` define *set* (I specifically DID NOT use `DEBUG` for this, so I can switch debug logging on in Release builds when the shit hits the fan). Meanwhile Unicode and Chinese language came to the rescue: `Debug特` is the new Debug level logging API methods set which will always do the job, in both DEBUG and RELEASE builds.
			
* (4df5d0b) comment typo fix
			



2019-10-05
----------

			
* (fdb469e) twiddling...
			
* (ec57707) fix crash in Jimme's code as I dumped some other libraries in there which have shorter names, e.g. "Guest2" (which is less than 8 characters) - https://github.com/jimmejardine/qiqqa-open-source/issues/93
			
* (2373620) Google Analytics throws a 403. Probably did so before, but now we notice it once again as we watch for exceptions occuring in the code flow. Better logging of the 403 failure.
			
* (312e9e3) patched CHANGELOG roughly from CHANGELOG-full
			
* (c8590be) Trying to cope with the bother of https://github.com/jimmejardine/qiqqa-open-source/issues/94 - quite a bit of it is inexplicable, unless Windows updates pulled the rug from under me (and CLR 4.0)
			
* (3c97e85) comment typo fix
			
* (53988c2) dump mistake caused the new test file not being discovered in the MSVS2019 Test Explorer: class must be public. duh.
			
* (e354aff) Added another RIS test fixture file ( https://github.com/jimmejardine/qiqqa-open-source/issues/70 )
			
* (7efcff9) spin off for  https://github.com/jimmejardine/qiqqa-open-source/issues/92 : add prerelease tests which will ensure there's no regression like that. (Discovering that one did hurt/smart!)
			
* (26106cf) adding a few STILL FAILING TESTS' reference files: these are guaranteed to report failure until we get those bits of Qiqqa working properly (and/or the tests tweaked/corrected)
			
* (193b149) updated the 'approved' references for a few BibTeX test files
			
* (db433db) remove yet unused generic test rig bit
			
* (2f1d319) fix https://github.com/jimmejardine/qiqqa-open-source/issues/92 : set all build targets to output x86 target code instead of 'AnyPC'
			
* (38d5a9f) editing CHANGELOG.md, taking stuff from CHANGELOG_full.md
			
* (afb8260) updated CHANGELOG_full.md
		

v82pre release
==============

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.





2019-10-05
----------

			
* (d4ad6d8) re-did the CHANGELOG generator, using git+node. The old `changelog` tool (npm changelog / npm @g3erhobbelt/changelog) is not reliable and this was coded faster than debugging and correcting that one.
			



2019-10-04
----------

			
* (2f21f82) Merge remote-tracking branch 'remotes/GerHobbelt/master' into v82-build
			
* (82b3475) ignore build intermediates for the added legacy support lib project
			
* (580ed05) built v82 setup/installer
			
* (758e941) Removed unused NuGet bundles; built a v82 setup/installer:
  
  --------------------------------------
  Completed Packaging Qiqqa version 82.0.7216.35525 into ...\Qiqqa.Build\Packages\v82 - 20191004-194431\
  --------------------------------------
			
* (3d49ac0) Update README.md
			
* (ab39dd3) Update README.md
			
* (079fb11) fix CefSharp missing report on rebuild
			
* (7bc8bce) ignore the NuGet installed packages directory from now on.
			
* (e795da8) it wasn't such a particularly bright idea to include the NuGet packages in this repo: in principle I like it, but this is not JavaScript, this is C#, which comes with some HUGE packages (CefSharp is one) --> cleaning up and force-pushing a modified repo is in our near future...
			
* (4f46bc0) TODO comment added
			
* (b0bb67e) CefSharp at least loads as a package but we're not using it yet, hence disable those code chunks.
			
* (cdd51d6) tweaking the sniffer: I have an idea how to cope with that toggle-able BibTeX advanced editor control and the nauseating behaviour of the toggle-X which moves along up & down and thus can hide behind the sniffer ok/fail/skip/clean buttons at top-right.
			
* (865eb87) copied minimal code over from `experimental` branch: code cleanup + adding `DisableAllBackgroundTasks` config option, which is currently DISABLED and has no impact on the config serialization: the goal of these edits is that it'll be easier to merge with `experimental` branch later on without negative code/runtime impacts on the mainline.
			
* (db40efb) QiqqaOCR: disable the image-output diagnostic code once again: this only used temporary for https://github.com/GerHobbelt/qiqqa-open-source/issues/1 diagnostics.
			
* (1765137) fix https://github.com/jimmejardine/qiqqa-open-source/issues/74 + https://github.com/jimmejardine/qiqqa-open-source/issues/73 = https://github.com/GerHobbelt/qiqqa-open-source/issues/1 : QiqqaOCR is rather fruity when it comes to generating rectangle areas to OCR. This is now sanitized.
			



2019-10-03
----------

			
* (6f28311) QiqqaOCR: added thread-safety for shared variables. Added a bit more logging to see what goes wrong.
			
* (986f9b3) Debugging QiqqaOCR: copied chunks of the log4net config from Qiqqa itself as it turned out that the default settings for the console logging appender DOES NOT log DEBUG level statements. :-(
			
* (aee2f1b) store Qiqqa Configuration files (v79) for backwards compat testing
			
* (89a5ed5) keep XML file for https://github.com/GerHobbelt/qiqqa-open-source/issues/3 + reduce existing XML test file in size as that one will serve another need (backwards compat)
			
* (74c93cf) variables' renamed
  
  Former-commit-id: b0a7f18c8125e3ef51a2619ac7c86b33cad1bb7a
			
* (bf83faf) reduce the amount of (useless) logging
  
  
  Former-commit-id: 7f07674cf01776e117270ec40e02e5b9ff41d546
			
* (6d2a5de) performance edit: reduce the time spent inside the OCR/PDFTextExtract queue lock critical sections.
  
  
  Former-commit-id: ad108b31874a112875f02683bdf4c3d91d77c453
			



2019-10-02
----------

			
* (4381186) whitespace police action
  
  
  Former-commit-id: 0f8e28f7fefe8a744699a5ee79d4acc4d8bae564
			
* (c77434c) Added the beginning of CEFsharp (which will replace xulrunner in due time) as per https://github.com/cefsharp/CefSharp/issues/1714
  
  
  Former-commit-id: 0a3f9e6279681daeec7e43310155c5c7f6341620
			
* (3601732) bump software version to 82 (v81 -> v82)
			
* (44a6af8) added and edited TODO task comments to show up properly in Visual Studio
			
* (f57e2e2) add additional bad/broken BibTeX records to the test set. These came out of work done resulting in https://github.com/jimmejardine/qiqqa-open-source/issues/71 + https://github.com/jimmejardine/qiqqa-open-source/issues/72
			
* (a3bc0b4) fix/tweak: when qiqqa server software revision XML response shows a *revert* to an older Qiqqa software release, such was coded but would not have been caught before as that bit of conditional code would never be reached when the user is running a later Qiqqa version (like I do now: v81 local build vs. server at qiqqa.com still reporting v79 as compliant / latest version).
  
  Added XML test file for later. (TODO)
			
* (6bc9cec) document cyclic memory references discovered in the code during memory heap inspection. Nothing can be done about them right now.  :-(
			
* (36aeccf) fix the background tasks starter hold-off code
  
  + use a simple state machine (`hold_off`) which ensures that the `OnceOff` task is only executed once the Qiqqa main window has been rendered, thus resulting in a perceptually faster load/start response
  + that same state machine is then advanced to the next (and final) state (zero(0)) which allows all other background tasks to start acting.
			
* (4fd0bd1) recover 'restore desktop' functionality: partially reverting commit bc20149407175a76bb28fb015c5c25544f872721
			
* (95e5d5e) fix compiler errors due to reachability error due to cherrypick mistakes while pulling this stuff from the `experimental` branch
			
* (e4a4968) fix compiler errors due to cherrypick/merge mistakes for test project
			
* (397cb38) Qiqqa doesn't save the lib while working: checking hold-off and other 'background disable/postpone' features we've built in before when we were performance testing...
			
* (bba995a) added missing new project files (taken from the `experimental` branch)
			
* (45f083d) upgrade projects to all reference .NET 4.7.2 instead of 4.0 Client Profile: AutoMapper and other libs we're going to use require an up-to-date .NET environment.
			
* (897d6ca) fixup cherrypick commit SHA-1: 97885405442be27b5f200d27a6467f30afc5faf8 which nuked testfile introduced in commit SHA-1: 2488ba6510739bd96892ee7dcb23e9bb983d9ea2 : baby steps towards https://github.com/jimmejardine/qiqqa-open-source/issues/68
			
* (531e119) comment typo fix
			
* (48daaa2) `support@qiqqa.com` is of course R.I.P., hence point at github issue tracker instead for software failures.
			
* (0090a8d) added a couple of test files for BibTeX Importer tests
			
* (e77d66c) added test files to test rig
			
* (0ddf041) added a couple more specific test files for BibTeX
			
* (9788540) first baby steps towards https://github.com/jimmejardine/qiqqa-open-source/issues/68 : adding more tests and registering the current state of affairs in 'approved' reference files by way of `ApprovalTests` use in the test rig.
  
* (5019762) NEVER add/register the `*.received.json` files produced by ApprovalTests in git; only the `*.approved.json` user-approved reference output files should be registered with the revision control system.
			
* (5d06874) added more BibTeX test data files + tweaked reference output path for ApprovalTests custom DataTest namer/writer.
			
* (7916c09) further work on getting a BibTeX DataTest unit test running using the ApprovalTests library -- for this purpose we had to create a custom Writer and Namer for ApprovalTests does not provide those suitable for DataTests.
			
* (c3ed5db) cherrypick fix: don't forget the latest JSON library package
			
* (2488ba6) add test sample files for https://github.com/jimmejardine/qiqqa-open-source/issues/68 + https://github.com/jimmejardine/qiqqa-open-source/issues/72
			
* (243082d) fix crash due to config string being NULL
			
* (b0ae9bb) reduce the number of internal try/catch exceptions when handling more-or-less flaky web traffic
			
* (4f07d34) Legacy Web Library: such a library is NOT read-only. (What we got to do is point it to an 'Intranet' sync point = directory/URI path instead. (TODO)
			
* (bd923e5) fix https://github.com/jimmejardine/qiqqa-open-source/issues/72 + adding **minimal** support for bibtex concatenation macros in order to (somewhat) correctly parse google scholar patents records: fixes https://github.com/jimmejardine/qiqqa-open-source/issues/22 and a bit of work towards https://github.com/jimmejardine/qiqqa-open-source/issues/68
			
* (5ccc4fa) fix https://github.com/jimmejardine/qiqqa-open-source/issues/71 : BibTeX parser no longer is stuck in infinite loop when encountering unterminated fields, etc.
			



2019-10-01
----------


> ## NOTE:
>
> All these commits are listed on this particular day because these are all git-cherrypick'ed commits from the EXPERIMENTAL branch, which includes other not-yet-ready work. Unfortunately, the dates of these commits are thus a bit mangled, but the commit hashes are the relevant entries in the upcoming software release anyway.

			
* (6378347) add the mirrored commercial Qiqqa installers for backtesting/etc. purposes.
			
* (52dc2df) bit of code cleanup in the Intranet Library Sync code section.
			
* (eb52678) fix cherrypicks thus far...
			
* (38a3d27) fixing commit SHA-1: da2186c50c63fd9454862d751437f06210700d28 (working on the new Unit Tests for BibTeX parsing) + adding tests
			
* (683d27e) OFF-TOPIC twirling in my nose...
			
* (581fc04) moved the `\n\n\n` append patch to the BibTeX lexer and started the refactor there. Working towards https://github.com/jimmejardine/qiqqa-open-source/issues/68
			
* (a19f7da) remove old cruft in BibTeX parser: the `\textless` and `\textgreater` LaTeX macros define `<` and `>` and those should never-EVAR be present around *any* BibTeX record. Besides, we WILL add code in the parser/lexer later on to cope with TeX macros inside text strings, so this bit is an old hack at best. Now it's GONE.
			
* (e52c3e3) add missing reference to Utilities project in Qiqqa TestHelpers project
			
* (bde0c3e) working on the new Unit Tests (for BibTeX parsing):
  - enabled some of the performance tests in the codebase by loading the same source file in the Unit Test project and having `#if TEST` code sections turned on there
  - added unit test to verify that the unit tester has `#if TEST` defined/enabled
  - added a TraceAppender to allow performance tests and others to log to the Test Explorer output via TraceLogAppender / TRACE
  - adjusted the log4net configuration(s) accordingly
			
* (0a675a3) (SCRATCHWORK) fiddling with the BibTeX editor control to make it behave with the Sniffer window: currently the green X to toggle between BibTeX and RAW view mode gets obscured for files which have few or no BibTeX lines, thus making that bit of the UI unusable ATM. TODO: must find a way to resize the RAW view pane to max of both or some other method to ensure a minimum height that's acceptable everywhere.
			
* (c1918c1) added class in Utilities for constants which are used in multiple spots in the Qiqqa codebase.
			
* (6867922) adding BibTeX test data from various sources into `...Tester/data/fixtures/...`
			
* (bc05c34) 
  - upgrading NuGet packages: MSTest 2.0beta4 to 2.0
  - adding NuGet package ApprovalTests so we can more easily compare BibTeX records and other complex data input/outputs
  - slowly getting the MSTest-based unit tests set up and going: using the new MSTest v2 DataRow feature to keep the amount of test code low.
  - added `Assert.FileExists()`; used in TestBibTeX
			
* (aa2b7a1) code cleanup and a bit of dead code removal (there was a property in there which mentioned 'unit tests' -- which did not come with Qiqqa and are being recreated, but will not require or use this dead bit of code anyway.
			
* (07bb569) allow tags to be separated by COMMA as well as SEMICOLON. (Added COMMA separator support)
			
* (81526f6) compiler hint: `const`
			
* (8ca0b43) ignore scratch directory
			
* (dad18fd) fix for the new Sniffer feature: a full-fledged BibTeX editor pane, just like you get in the library/document view panels in Qiqqa. KNOWN ISSUE: for small BibTeXs (only a few lines of BibTeX for the given document), the green toggle X is obscured by the thumbs up/down fade in/out buttons. :-(
			
* (ec1fe2c) one more for https://github.com/jimmejardine/qiqqa-open-source/issues/67
  
  WARNING: a PDF URI does *not* have to include a PDF extension!
  
  Case in point:
  
      https://pubs.acs.org/doi/pdf/10.1021/ed1010618?rand=zf7t0csx
  
  is an example of such a URI: this URI references a PDF but DOES NOT contain the string ".pdf" itself!
			
* (4a03a3d) performance optimization: `PubMedXMLToBibTex.TryConvert()` doesn't have to go through the entire XML parsing process when a simple heuristic can already detect which input/content won't pass anyhow: this will quickly filter out the major number of entries fed to PubMedXMLToBibTex, while also reducing the number of useless logfile lines as each such entry would previously log one PubMedXMLToBibTex failure line.
			
* (a4dbef6) Merge branch 'master' into experimental-ui-edits
  
			
* (27ffcc4) Fiddle with the versions: Qiqqa and QiqqaOCR should have the same build/version numbers.
  
  TODO: The Version number needs to be updated in a few more places too.
			
* (def7f3c) Work done on https://github.com/jimmejardine/qiqqa-open-source/issues/65 ; also add an event to signal the completion of the library load task as the UI is updated asynchronously, only once and then *way too early* so the numbers for 'document(s) in library' are quite wrong 99.9% of the time. Only jiggling the UI into a refresh by jiggling the Graph/Chart button or Carousel button helps as those OnClick handlers repaint the library summary entry in the library list and thus update the 'document(s) in library' count. ... Meanwhile, that way you get a cruddy flow which doesn't update the UI when it *should* and overall takes way to much time as the library document lists also get updated at the wrong moment -- which takes quit some time for large libraries and is quite useless when the set is not yet completed and/or that UI pane is not visible at the time... :-(
			
* (c4b64a4) PDF imports: add menu item to re-import all PDFs collected in the library in order to discover the as-yet-LOST/UNREGISTERED PDFs, which collected in the library due to previous Qiqqa crashes & user ABORT actions (https://github.com/jimmejardine/qiqqa-open-source/issues/64)
  
  Additional fix: use `AugmentedPopupAutoCloser` on *all* import menu items as otherwise the menu will remain open and obscure part of your active UI once the desired menu item has been clicked.
			
* (3872959) `AddNewDocumentToLibraryFromInternet_*()` APIs: some nasty/ill-configured servers don't produce a legal Content-Type header, or don't provide that header *at all* -- which made Qiqqa barf a hairball instead of properly attempting to import the downloaded PDF.
  
  Also don't yak about images which are downloaded as part of Google search pages, etc.: these content-types now make it through *part* of the PDF import code as we cannot rely on the Content-Type header being valid or present, hence we need to be very lenient about what we accept as "potentially a PDF document" to inspect before importing.
  
  Fixes: https://github.com/jimmejardine/qiqqa-open-source/issues/63
			
* (d2b5c22) tackling with the weird SQLite lockup issues: https://github.com/jimmejardine/qiqqa-open-source/issues/62
  
  As stated in the issue:
  
  Seems to be an SQLite issue: https://stackoverflow.com/questions/12532729/sqlite-keeps-the-database-locked-even-after-the-connection-is-closed gives off the same smell...
  
  Adding a `lock(x) {...}` critical section **per library instance** didn't make a difference.
  
  Adding a global/singleton  `lock(x) {...}` critical section **shared among /all/ library instances** *seems* to reduce the problem, but large PDF import tests show that the problem isn't *gone* with such a fix/tweak/hack.
			
* (abd020a) UPGRADE PACKAGES: log4net, SQLite, etc. -- the easy ones. Using NuGet Package Manager.
  
			
* (b5a4256) preparation for unit tests that can work: add a QiqqaTestHelpers library -- it turns out we're pretty much toast when we use NUnit, so that one's **out**; then there's MSTest but the standard Assert library there is rather lacking, hence we've ripped the Assertions from xUnit/nUnit and tweaked/augmented them to suit MSTest and our rig -- the intent being that you can still see **and debug** the tests from within Microsoft Visual Studio. It's all a bit hacky still to my taste, but at least now we don't get crazy NUnit execution failures any more for every !@#$ test.
			
* (a339b37) preparation for unit tests that can work: replace application startpath with equivalent code from the Utilities libraries which has built-in detection whether the unit tests are running or Qiqqa itself.
			
* (b5c6467) moving from non-working NUnit to MSTest-based unit/system test project for Qiqqa -- let's pray this works out well for us as using Nunit turned out to be non-working.
			
* (3efdfd4) EXPERIMENTAL: add full BibTeX editing power to the sniffer, that is: use the same BibTeX control there as used elsewhere where you can switch between RAW view and PARSED BibTeX lines. ----- TODO: link up the edit events again.
			
* (f8a0762) EXPERIMENTAL: don't update the base control as the list controls get updated already thanks to commit SHA-1: babb1bcdd531db2a4aee7ca12739265beb2199c6
  In a sense this is a partial revert of commit commit SHA-1: bcdd43f2b329363b0c953f22a2c1630533081fdf just to see if it flies...
			
* (43debf6) remaining work for  https://github.com/jimmejardine/qiqqa-open-source/issues/56 / https://github.com/jimmejardine/qiqqa-open-source/issues/54 -- catch some nasty PDF URIs which weren't recognized as such before. Right now we're pretty aggressive as we fetch almost everything that crosses our path; once fetched we check if's actually a valid PDF file after all. CiteSeerX and other sites now deliver once again...
			
* (c3102a7) fix: BibTeX dialog doesn't scroll: that's a problem when your list of BibTeX tags is longer than the height of the dialog allows. Hence we can now scroll the bugger.
			
* (b46b38d) Don't get bothered by the Tweet stuff: collapse it.
			
* (c3567cf) further tweaks for the MS Designer preview. AugmentedButton now kills its (preset) caption when its icon is set up programmatically. This should keep the icon buttons clean.
			
* (351429c) [FEATURE] work done for https://github.com/jimmejardine/qiqqa-open-source/issues/21 : the BibTeX entry definitions are now sitting in an external JSON file so every savvy user can edit/add their own set of fields there now.
  
  BTW: Nunit testing was a great idea but I'm getting savagely abused around my Glutus Maximus with a Cactus by way of the woes of https://stackoverflow.com/questions/15614192/run-a-specified-nunit-test-as-a-32-bit-process-in-a-64-bit-environment . I effin' *hate* .NET right now - and particularly old cruft that needs upgrading so I can move out of that fugly 32bit shyte sewer.      \< angry like mad! />
			
* (e0d056c) fixes for https://github.com/jimmejardine/qiqqa-open-source/issues/56 ; also ensuring every document that's fetched off the Internet is opened in Qiqqa for review/editing (some PDF documents were silently downloaded and then dumped into the Guest Library just because and you'ld have to go around and check to see the stuff actually arrived in a library of yours. :'-(
			
* (4b7bec3) nuke unused sourcecode files
			
* (b12bddf) further work to accommodate the Microsoft Designer tool: previously the code did not provide many 'getters' which caused many crashes/exceptions in the Microsoft XAML Designer.
			
* (55ba0f6) BibTexEditorControl fix: bibtex keys such as "organisation" were clipped in the UI; now the key column had been widened a little to accommodate these larger key names.
			
* (f8d5ff7) BibTeX modes for manual editing and vetting in the Sniffer.
			
* (4a1bb74) tweaked XAMLs for better Designer preview: added button texts to the XAML instead of only having it set in the code.
			
* (be0d54f) fix: https://github.com/jimmejardine/qiqqa-open-source/issues/60 + https://github.com/jimmejardine/qiqqa-open-source/issues/39 + better fix for https://github.com/jimmejardine/qiqqa-open-source/issues/59
  
  check how many PDF files actually match and only move forward when we don't end up full circle. don't reflect if we didn't change. when we re-render the same document, we don't move at all!
			
* (4e791e8) fix https://github.com/jimmejardine/qiqqa-open-source/issues/59: don't reflect if we didn't change.
  
  We start with a dummy fingerprint to ensure that we will observe ANY initial setup/change as significant for otherwise we don't get the initial PDF document rendered at all!
  
  We use the PDF Fingerprint as a check for change as the numeric `pdf_documents_search_index` value might look easier but doesn't show us any library updates that may have happened in the meantime.
			
* (df8c9f7) 
  - fixed the focus loss while scrolling through the library PDF list using the keyboard: it's the repeated invocation of the code line `ThemeTabItem.AddToApplicationResources(app)` for each individual control being instantiated that killed the focus semi-randomly. That code was introduced/activated that way a while ago to better support the MSVS2019 Designer -- which should still work as before.
  
  - fixes several issues reported in https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632 : the keyboard scrolling no longer suffers from long delays -- where I first suspected thread locking it turns out to be a hassle with WPF (grrrrrr); I cannot explain **how exactly** WPF caused the lethargic slowdown, but the fact is that it's gone now. The fact I cannot explain this makes this codebase brittle from my point of view. Alas.
  
  - it *looks* like I can gt away with a bit of a performance boost as that `listview.UpdateLayout();` from `GUITools::ScrollToTop()` looks like it can be safely moved into the IsVisibleChanged handler for said ListView(s)... hmmmmm
			
* (f417ea8) !@#$%^ got it! Had been too zealous when hack-patching the faults at ScrollToTop. Dang!  https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink & commit SHA-1: babb1bcdd531db2a4aee7ca12739265beb2199c6
			
* (bc20149) building x86 only as otherwise antique tesseract et al will fail dramatically. Otherwise aligned the settings of the projects and disabled a few config items in the cod for testing the current view update woes. >:-(   I !@#$%^&^%$#@#$%^ loath WPF.
  
			
* (be4e884) fiddling: add a *failing* dummy test case to the test suite -- to be written when we address BibTeX parsing for real.
			
* (0b45a6d) Temporarily DISABLE MSVC Designer Mode detection
			
* (c8e3729) Locking in the current state of affairs as of https://github.com/jimmejardine/qiqqa-open-source/issues/55#issuecomment-524846632-permalink while I dig further to uncover the *slowtard* culprits. Mother Of All Commits. Can be split up into multiple commits if needed later on in a separate branch. Actions done in the past days while hunting the shite:
  
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
			
* (afa4e50) fix cherrypicks thus far...
			
* (ee6ebac) `WebLibraryDetail`: augmented the class with a `public string DescriptiveTitle` derived property which is used for the library document list view: some derived titles can be extremely long, e.g. when there's no title text available, yet a long SourceDocument URI is used instead. This code is combined with new `StringTools` APIs to produce a title that's limited to a given length and when longer prints the first part plus an ELLIPSIS. This dals with https://github.com/jimmejardine/qiqqa-open-source/issues/57
			
* (04089fd) code cleanup: killing unused code/classes
			
* (7065070) code cleanup: encode all reading stages
			
* (8b50a35) part of the thread code refactor: Import the Qiqqa manuals into the guest library **in the background, waiting until the library has loaded**.
			
* (b004c53) bit of code cleanup
			
* (5e2ec57) string=null + string=value --> cleaner is to add string:"" + string:value then.
			
* (6e2a217) BibTeX processing: augmented the error logging
			
* (0cbda4e) thread management refactor:
  
  - `MaintainableManager.Instance.Register` --> `MaintainableManager.Instance.RegisterHeldOffTask`
  - registered background tasks only start to execute once the 'hold off' has been signaled, AFTER WHICH the preset delay is started before the actual task code is executed itself for the first time.
  - obsoleted the `GeneralTaskDaemon` code/class
  - added more thread safety code for the folder watchers as the check `FolderContentsHaveChanged` may be invoked from multiple threads
  - postpone the call to `RememberProcessedFile` as much as possible; the PDF processing code flow has to be refactored still (TODO!)
  - added more thread safety code for FolderWatcherManager
  - always keep the Word Connection Setup thread running, even when other background tasks have been disabled, since this thread is required for the InCite front work (direct user activity)
  
			
* (c94c362) fixd bug, found as part of the task register code refactor: Quit this delayed storing of PDF files when we've hit the end of the execution run: we'll have to save them all to disk in one go then, and quickly too!
  
  Note the new code line
  
              Utilities.Shutdownable.ShutdownableManager.Instance.Register(Shutdown);
  
  in particular!
  
			
* (9fe39ef) 
  - shut up Visual Studio Designer - at least as much as we can on short notice: give 'em the Theme colors and brushes, limit the number of XUL crashes while loading the Sniffer XAML, etc.
  - BibTeX Sniffer: put a time limit of slow filter criteria (HasPDF and IsOCR checks); mark the checkboxes for those filter options accordingly when the time limit (5 seconds) kicks in so the user can know the resulting list is *inaccurate*.
  - **TODO**: put Tooltips with state info as user help on the filter options.
			
* (4bbfd65) bit of code cleanup
			
* (c1bb4f8) 
  - BibTeX Sniffer: clean up search items results in better (human readable) search criteria for some PDFs where previously the words were separated by TAB but ended up as one long concatenated string of characters in Google Scholar Search.
  - HTTP/HTTPS web grab of PDF files: we don't care which TLS/SSL protocol is required, we should just grab the PDF and not bother. Some websites require TLS2 while today I ran into a website which requires old SSL (not TLS): make sure they're **all** turned ON.
  - Register the current library with the WebBrowserHostControl so that we don't have to go through obnoxious 'Pick A Library' dialog every time we hit the "Import all PDFs available on this page" button in the browser toolbar.
  - REVERT the DISabling of switching the active searcher: the behaviour was b0rked (I knew this) and before we go and fix https://github.com/jimmejardine/qiqqa-open-source/issues/46 we first restore the original behaviour (which was disabled in commit SHA-1: 0da072bc26bd5492a68dcb41a3a83e0b2acf7c00
			
* (e983130) bit of debugger logging code cleanup
			
* (3b4ca9a) fix crash in "grab/download all PDF files which are available on this page" webbrowser toolbar button functionality: the code crashed on relative URIs being fed into `new Uri(url)` code lines. Now the code copes correctly with both absolute and relative URIs and also corrupt/invalid URIs don't crash the grab-extractor code any more. Also improved the check for any URI found in the page being a PDF file a little: check for ".pdf" rather than "pdf": this will prevent us from trying not-a-pdf-file URIs such as "http://www.example.com/blog-about-pdf".
			
* (285456b) further fiddling with the weird download issue reported in commit SHA-1: d39a2344be710a32a3b4698b5415c4398806b4de --> had a look if and how Chrome browser does it. It succeeds, with these headers:
  
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
			
* (6ec1466) fixing a couple of Exceptions thrown by the Visual Studio Designer due to missing `get`s...
			
* (7c1e494) Trying to tackle a very weird PDF download problem, which doesn't go away.
  
  HTR:
  - Sniffer search on Scholar: "Deciding Floating point logic with systematic abstraction"
  - no click the first PDF link (ora.ox.ac.uk) to download PDF.
  
    This will silently FAIL! (same error as mentioned here: https://stackoverflow.com/questions/21728773/the-underlying-connection-was-closed-an-unexpected-error-occurred-on-a-receiv and here https://stackoverflow.com/questions/21481682/httpwebrequest-the-underlying-connection-was-closed-the-connection-was-closed )
  
    Link: https://ora.ox.ac.uk/objects/uuid:49e0183f-277e-486c-87bc-17097cbef0b3/download_file?file_format=pdf&safe_filename=fmcad2012.pdf&type_of_work=Conference+item
  
  - open the same search in "Google.com" tab: will show a PDF entry as a result at the down.
  - download that one without trouble.
  
    Link: http://www.cs.ox.ac.uk/people/leopold.haller/papers/fmcad2012.pdf
			
* (5f00577) 
  - refactor: now StandardWindow will save (and restore on demand) the window size and location for any named window; th settings will be stored in the configuration file. SHOULD be backwards compatible. Further work on https://github.com/jimmejardine/qiqqa-open-source/issues/8
  - also fix the handling of the "Has OCR" checkbox: made it a proper tri-state. VERY SLOW to filter when ticked OFF or ON. (TODO: add a hack where we only allow it to impact the filtering for N seconds so as to limit the impact on UX performance-wise)
			
* (f31ebb0) bit of code cleanup
			
* (720caf2) 
  - fix https://github.com/jimmejardine/qiqqa-open-source/issues/54 in GoogleBibTexSnifferControl
  - Gecko these days crashes on ContentDispositionXXXX member accesses: Exception thrown: 'System.Runtime.InteropServices.COMException' in Geckofx-Core.dll
  
    I'm not sure why; the only change I know of is an update of MSVS2019.  :-S
  
  - implement the logic for the BibTeXSniffer 'Has OCR' checkbox filter criterium. It's useful but the zillion file-accesses slow the response down too much to my taste.   :-S
			
* (6e8ab5d) sniffer: add filter check box to only show those PDF records which have been OCRed already. (The ones that aren't are pretty hard to sniff as you cannot mark any title text bits in them yet, for instance)
			
* (dc8586c) augment debug logging for OCRengine
			
* (73e0d6e) Removed all `[Obfuscation(Feature = "properties renaming")]` lines from the source code: those were needed when Qiqqa was commercial and the binaries were obfuscated to make reverse-engineering & cracking more difficult. Now they only clutter.
			
* (add86ca) typo
			
* (4aa52b0) added threadpool limits and heuristic stretching of the inter-batch sleep/delay in the FolderWatcher to allow the other background threads to process the imported PDF files; the more pending work from those other background tasks there is, the longer the sleep interval in the FolderWatcher to hand over CPU cycles to those other tasks (indexing, OCR, text extraction, metadata database flushing, ...)
			
* (3f1bbf2) quick fix for folder watcher going berzerk -- has to last until we refactor the async/sync PDF import code chunks. (see branch `refactoring_pdf_imports`)
			
* (46d5e4e) provide code to set the threadpool/queue to a more or less sane amount of active threads.
			
* (5ecb1ca) do NOT build the packager project when in Debug mode in the IDE.
			
* (c503e17) Add debug logging re thread management. Threadpool on this machine has max thread count at 1000, which is way too high for my tastes...
			
* (70530e3) clan up AppendStackTrace: don't list th stack lines which concern the call to AppendStackTrace itself as they don't add any information and merely clutter the output.
			
* (2c82157) picked up the saf and simple bits of refactor commit SHA-1: c315645d0bd2b69536030c55b1504a4303d59bce - a few locks added for thread safety and a few minimal code cleanups
			
* (b4ec800) fix cherry-picked merge SHA-1: a026583c947a0ce18004582f4e581d535af57aad
			
* (0238a7e) Part 1 of refactoring of the async/sync processing of PDFs being imported from various directions. Current code has some thread-unsafe practices, e.g. by passing references to List<T> instances across thread boundaries while the sender keeps working on the same set.
			
* (23c3644) removed unnecessary code: `ConvertTagBundleToTags` already produces a `HashSet`
			
* (b0b7e72) 
  - 'integrate' nant build script for producing setup.exe installers into MSVS2019 solution by using a dummy project and prebuild script
  - added skeleton projects for qiqqa diagnostic and migration work. Related to https://github.com/jimmejardine/qiqqa-open-source/issues/43
			
* (4e429f6) comment typo fixes
			
* (f5db12b) debugging views: not all theme colours were correctly exported for Microsoft Blend / XAML Designer.
			
* (3241408) added debug logging in dirtree scanner code
			
* (5a643b7) collapse=false tweak; debugging views
			
* (50f1dd4) code cleanup + Stopwatch new + Start --> Stopwatch.StartNew()
			
* (ec06fdc) fix for bug introduced by me while working on https://github.com/jimmejardine/qiqqa-open-source/issues/50 : on large dirtrees being watched, the time limit can be reached and is never reset ==> no more PDFs are added! **Whoops!** **BUG**
  
  Also: when importing PDFs from a watched directory the proper check to see if a PDF is already existing in the library is by checking its HASH, rather than the DownloadSource location, which can be **different** for identical files, or even **identical** when files have been patched in-line using tools such as QPDF (https://github.com/GerHobbelt/qiqqa-revengin)
			
* (428e54c) remove Stopwatch class as it collides/duplicates standard lib code: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch?view=netframework-4.8
			
* (3a2629f) working on https://github.com/jimmejardine/qiqqa-open-source/issues/52 : FolderWatcher and PDF library import need rework / refactor as we either break the status feedback to user ("Adding document N of M...") as we loose the overall count of added documents, *or* we add/import PDF documents multiple times as we cannot destroy/Clear() the list since it is fed to the async function **as a reference**. :-(
  
  Meanwhile, we ensure that only ONE instance of the 'There a problem with some imports" AlertBox is shown at any time; multiple threads append to the report file which is then shown to the user, if (s)he desires so.
			
* (25f029f) documented the most important refactored API. https://github.com/jimmejardine/qiqqa-open-source/issues/44
			
* (716d54d) fix/tweak: just like with Sniffer AutoGuess, when a BibTeX record is picked from bibtexsearch using heuristics, it is now flagged in the bibtex with a special comment (`@COMMENT...`) which was already available before in the code but apparently disused or unused until today.
			
* (2eb1380) refactored: Wasn't happy with the code flow in the FolderWatcher: now the long recursive directory scan (using `EnumerateFiles()`) is only aborted whn the app is terminated or when it has run its course (or when there are more or less dire circumstances); otherwise the dirtreescan is periodically paused to give the machine a bit of air to cope with the results and/or other pending work, while an app exit is very quickly discovered still (just like before, it is also detected inside the `daemon.Sleep(N)` calls in there, so we're good re that one. Tested it and works nicely. https://github.com/jimmejardine/qiqqa-open-source/issues/50
			
* (a7b2ca3) 
  - fixed: https://github.com/jimmejardine/qiqqa-open-source/issues/53
  - bit of related code cleanup
  - added debug code (to be removed shortly)
			
* (c62329c) fixed typo in tooltip: part of https://github.com/jimmejardine/qiqqa-open-source/issues/44
			
* (416fc56) white space
			
* (a758fa1) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/31
			
* (d22e003) just a few lines of code cleanup
			
* (7118c3c) fix https://github.com/jimmejardine/qiqqa-open-source/issues/48 : Expedition: Refresh -> "Looking for new citations in ..." is not aborted when Qiqqa is closed.
			
* (14b96cb) added debugging logging
			
* (34f74b1) updated CHANGELOG_full.md
			
* (6533062) additional debugging code and code formatting
			
* (1053fce) 
  - fixed https://github.com/jimmejardine/qiqqa-open-source/issues/50 using EnumerateFiles API instead of GetFiles. (TODO: inspect other sites in code where GetFiles is invoked)
  - introduced `Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown` as a global flag to check whether the app is terminating: this works the same way as `!thread.IsRunning` but without the need to memoize the thread reference or pass it via extra function arguments.
  - Addd a couple of extra `IsShuttingDown` checks to improve 'exit' termination speed of the application.
			
* (c1de3ca) Added/tweaked debugging via logging: add check to logger to catch particular phrases so we can debugger-break on those and track down via call stack.
			
* (869dea1) fixed https://github.com/jimmejardine/qiqqa-open-source/issues/8: now stores WindowState as well and fetches non-maximized window size using RestoreBounds WPF API.
			
* (af2e32a) Slighty more informative/easier to track back log lines for browsing web pages in Qiqqa
			
* (59eede7) fiddling with website theme...
			
* (f9918c8) fiddling with website theme...
  
			
* (831edb3) (lint) `const`-ing a few variables, which really are constants
			
* (1b390f8) Removing old debug lines that aren't required any more...
			
* (05bfb20) Prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/23 to make transition to latest LuceneNET easier to do.
			
* (4601382) Prepwork / minimal refactoring of `Object[]` to `PQRecord` done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.
			
* (12d6496) fix compile error introduced with commit SHA-1: 453bd2e41b02f9609303c27f04a6392a127880eb * Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.
			
* (f069079) fiddling with website theme...
			
* (44c1782) fiddling with website theme...
			
* (85339e7) Merge remote-tracking branch 'remotes/GerHobbelt/master'
			
* (94aa67b) Move the reference HTML file out of the way. https://github.com/jimmejardine/qiqqa-open-source/issues/38
			
* (5097f98) Create README.md
			
* (0a28d98) Set theme jekyll-theme-tactile
			
* (4e8e48d) Set theme jekyll-theme-minimal
			
* (ca0681f) done: https://github.com/jimmejardine/qiqqa-open-source/issues/38 -- Part 14: added the images. Turns out there are two screens (screen0003.ai and screen0014.ai that remain unused. Looks like this was partly old stuff and maybe a chunk that still needs to go into a section of the manual. Haven't checked precisely as the job at hand was *conversion*.
			
* (e0a919e) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 13: adding the images. First one. Let's check because I always screw this bit of MD notation up.  :-S
			
* (c91ce1e) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 12: Done with the roundtrip check and HTML + MD cleanup. Now we only need to get back all the images in there...
			
* (d3cbbfa) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 13: adding the images. First one. Let's check because I always screw this bit of MD notation up.  :-S
			
* (cb73658) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 12: Done with the roundtrip check and HTML + MD cleanup. Now we only need to get back all the images in there...
			
* (d73d392) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 11: Basically we're all good now. Tweaked the MD source to use modern MarkDown header marks. Found that we still need to correct lists in there as TurnDown didn't catch all the &middot encoded lists from Word, so that'll be next, together picking up the Unicode SmartQuotes from the RoundTrip copy so that the HTML file will serve as a reference for subsequent MarkDown source editing work...
			
* (bb4792b) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 10: Some swift manual edits on initial comparison: looks like everything made it, but there are notable differences. Patching the destination HTML and source MD MarkDown file to ensure the next round will correct these render mistakes...
			
* (05a654f) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 9: Cleaning the source HTML using https://html-cleaner.com/ to kill the MSWord left-overs and then another round of https://htmlformatter.com/ for maximum similarity (and thus faster work in reviewing the diffs next)
			
* (96496b9) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 8: Same https://htmlformatter.com/ applied to source HTML
			
* (b7d61c8) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 7: A swift kill of all styling and Dillinger editor/line left-overs in the HTML: one regex replace in Sublime.
			
* (21bdf1d) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 6: Applying https://htmlformatter.com/ to the Dillinger output
			
* (e9efd60) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 5: RoundTripping the MarkDown to HTML (we need to check everything made it through and this is the quickest way for a large document like this: at the end waits a fast Beyond Compare session going through the diffs of the source and roundtrip HTML files...). Uses https://dillinger.io/
			
* (6e410f8) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 4: Pulled the HTML through TurnDown ( http://domchristie.github.io/turndown/ ) to produce an initial MarkDown version of the documentation. Need to round-trip it to ensure we didn't loose any important chunks. :-)
			
* (2c99bc8) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 3: patching the HTML.
			
* (5b5039c) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 2.
			
* (fc38959) prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead.
			
* (540fe4d) prepwork done for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : extracted all displays from the pptx file (I've no problem using PowerPoint for stuff like this, but I have more control over publishing/output when using tools like Adobe Illustrator. Besides, the images that stick partly outside the PowerPoint pages are begging for clipping issues, etc. when rendering.
			
* (4c75cdb) Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.

* Do NOT write old format (.NET serialized binary) configuration files any more: DO load them when the new JSON format is missing, but only write the NEW JSON format. https://github.com/jimmejardine/qiqqa-open-source/issues/41
* NuGet: package Newtonsoft.JSON: the JSON-to-object deserializer/serializer. Lingering work from https://github.com/jimmejardine/qiqqa-open-source/issues/41
* https://github.com/jimmejardine/qiqqa-open-source/issues/41 tweak: more human-readable JSON formatting

* (6e37242) fix https://github.com/jimmejardine/qiqqa-open-source/issues/42: fixed crash.
			
* (0cb6b26) more of the same as SHA-1: af670a88f8fb56d090ed8d04bfb9b08cb0e53b33 * minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball

* Hm, looks like the NANT build script (`./build-installer.sh`) picks up the new Newtonsoft.Json.dll location. Lucky break. Killed the old one as we now *upgrade* JSON.NET ( https://github.com/jimmejardine/qiqqa-open-source/issues/41 )
* work done on https://github.com/jimmejardine/qiqqa-open-source/issues/41, which was triggered by the bugging and b0rking of https://github.com/jimmejardine/qiqqa-open-source/issues/40, hence a few bits from that one will peek through here.
* Using Json.NET as advised by Microsoft: https://docs.microsoft.com/en-us/dotnet/api/system.web.script.serialization.javascriptserializer?view=netframework-4.8
    
  As mentioned in the Deprecation Notice of the old serializer code:
    
  > .NET binary serialization causes too much trouble, e.g. https://stackoverflow.com/questions/6825819/how-can-i-tell-when-what-is-loading-certain-assemblies and https://social.msdn.microsoft.com/forums/vstudio/en-US/7192f23e-7d43-47b5-b401-5fcd19671cf6/invalidcastexception-thrown-when-casting-to-the-same-type. Use Json.NET instead. And then there's https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/how-to-enable-and-disable-automatic-binding-redirection (sigh)

			



2019-08-09
----------

			
* (af670a8) minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball (Fatal System Exception) on many XAML dialogs/panels in Qiqqa.
  
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
			
* (3d1b26f) Minor UI tweak of BibTeX metadata pane in main UI: distance between buttons
			
* (348e58d) Tweak the AnnotationReportOptionsWindow XAML a little so it shows up in its entirety in de MSVS Designer.
			
* (be65d7d) Fiddling with the size of the proxy config dialog panel: make it wider so we can more easily enter&see host URI and user password.
			
* (8db7a3d) tweak the About message: now also show the *full* build version next to the classic `vNN` version.
			
* (57b6588) Tweak the logging lines for the InCite webbrowser so its start and end can be easily detected in the logfiles.
			
* (d51b119) removed unused variable assignment
			



2019-08-08
----------

			
* (d690619) prep for https://github.com/jimmejardine/qiqqa-open-source/issues/38
			
* (a4efd50) add 'how to build setup.exe' instructions to README.md
			
* (7750279) make Qiqqa main app and QiqqaOCR logging easily recognizable: `[Q]` or `[OCR]` tag per logline.
  Also print the QC-reported memory usage as a fixed-width number in MBytes
			



# version 81.0.7158.38371 :: alpha test release

* updated CHANGELOG files


Version 81:
- Qiqqa now copes better with damaged PDFs which are part of the librarie(s): 
  + search index does not "disappear" any more
  + Qiqqa does not continue running in the background for eternity due to locked-up PDF re-indexing task


**WARNING**: this was an EXPERIMENTAL release. Do not expect full forward or backwards compatibility.




2019-08-07
----------

			
* (2236c9a) updated CHANGELOG files
			
* (ede83a8) ALPHA/TEST RELEASE v81 : version 81.0.7158.38371
			
* (e37448f) log outgoing activity: posting BibTeX info to bibtexsearch.com aggregator
			
* (7bf0c72) re-added to 'Add This PDF to Library' button in the browser; TODO: make it work akin to the <embed> handling to prevent confusion: when the browser shows a single PDF, it MAY be an <embed> web page and we should account for that!
			
* (efbdd0c) IMPORTANT: this bad boy (an overzealous Dispose() which I introduced following up on the MSVS Code Analysis Reports) prevented Qiqqa from properly fetching and importing various PDFs from the Sniffer. (click on link would show the PDFs but not open them in Qiqqa nor import them into the Qiqqa library)
			
* (399b4c3) some titles/sentences seem to come with leading whitespace; title suggestion construction would produce suggested titles with leading and trailing whitespace. Fixed.
			
* (2b39e66) #ifdef/#endif unused code
			
* (4c3b1ed) 
  - fix crash in PDF import when website/webserver does not provide a Content-Disposable HTTP response header
  - add ability to cope with <embed> PDF links, e.g. when a HTML page is shown with PDF embedded instead of the PDF itself
  - detect PDF files in URLs which have query parameters: '.pdf' is not always the end of the URL for downloading the filename
			
* (e702b0c) revert/fix NANT build script to produce a `setup.exe` once again.
			
* (c11ff51) added CHANGELOG (partly edited & full version using `git log`)
			
* (c0ed132) moving some Info-level logging to Debug level as that's what it is, really. (Dispose activity tracking et al)
			
* (70f1a6a) added TODO to remember my own DB ...
			
* (7c05d0f) Whoops. Crash when quickly opening + closing + opening.... Sniffer windows: CLOSE != DISPOSE. Crash due to loss of search_options binding on second opening...
			
* (9a7e620) Only when you play with it, you discover what works. The HasSourceURL/Local/Unsourced choices should be OR-ed together as that feels intuitive, while we also want to see 'sans PDF' entries as we can use the Sniffer to dig up the PDF on the IntarWebz if we're lucky. Meanwhile, 'invert' should clearly be positioned off to a corner to signify its purpose: inverting your selection set (while it should **probably** :thinking: have no effect if a specific document was specified by the user: then we're looking at a particular item PLUS maybe some other stuff?
			
* (0d26304) whoops. coding typo fix. https://github.com/jimmejardine/qiqqa-open-source/issues/28
			
* (4acc253) Merge branch 'n29'
			
* (2717924) Merge branch 'work'
			
* (604a5db) Sniffer Features:
  - add checkboxes to (sub)select documents which have a URL source registered with them or no source registered at all. (https://github.com/jimmejardine/qiqqa-open-source/issues/29)
  - add 'invert' logic for the library filter (https://github.com/jimmejardine/qiqqa-open-source/issues/30)
			
* (785c487) fix https://github.com/jimmejardine/qiqqa-open-source/issues/28: turns out Qiqqa is feeding all the empty records to the PubMed-to-BibTex converter, which is throwing a tantrum. Improved checks and balances and all that. Jolly good, carry on, chaps. :-)
			
* (6283d89) work being done on https://github.com/jimmejardine/qiqqa-open-source/issues/29 + https://github.com/jimmejardine/qiqqa-open-source/issues/30: augmenting our Jolly Sniffer.
			
* (956e693) added TODO's for a bit of ho-hum that's been bothering me all week. To Be Researched while other issues get attention first.
			
* (5ac9585) report complete build version in logging. v80, v79, ...: it's not good enough when you want to track down the 'currentness' of the log :-)
			
* (e58df0a) improving the logging while we hunt for the elusive Fail Creatures... (One of them being that OutOfMemoryException that somehow turns up out of the blue while the app still has plenty memory to go @ 200-300MB GC-reported allocation. :thinking:
			
* (9a604ae) further work on CA2000 from the Code Analysis Report: apply `using (ISisposable) {...}` where possible.
			
* (cff8c62) refactoring event handling code (using null conditionals as suggested by MSVS Code Analysis Report)
			
* (a540e50) part of IDisposable cleanup work following the advice of the MSVS Code Analysis Report as much as possible (mostly the first bunch of CA2000 report lines) :
  
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
			
* (a80be7d) done work on https://github.com/jimmejardine/qiqqa-open-source/issues/27 and on the lockups of Qiqqa (some critical sections in there were **humongous** in both code side and run-time duration; now the number of lock-ups due to *very* slow loading PDFs coming in from the Qiqqa Sniffer should be quite reduced: work related to https://github.com/jimmejardine/qiqqa-open-source/issues/18
			
* (409d512) better handling of 'unreachable code' warnings
			
* (37c8b2c) Code looks like a swap/insert code pattern gone bad: did we find a bug?  Found due to MSVS Code Analysis Message	IDE0059	Unnecessary assignment of a value to 'pageranks_temp'
			



2019-08-06
----------

			
* (c48bca9) several like this: Warning	CA1500	'filter_terms', a parameter declared in 'LibraryCatalogControl.OnFilterChanged(LibraryFilterControl, List<PDFDocument>, Span, string, Dictionary<string, double>, PDFDocument)', has the same name as an instance field on the type. Change the name of one of these items.
			
* (c2d3a63) we also don't care (yet) to validate arguments coming into public methods and neither are we interested to hear about methods which might be better off as properties' getter/setter.
			
* (67942db) don't care about public nested visibility issues and 'can be simplified' `new` object instantiations, which merely introduce a newer C# language feature that I don't particularly like (contrary to null conditionals ;-) )
			
* (63b00f7) patched the Code Analysis ruleset for now: don't care about internationalization, app-specific exceptions and the lot. Once we're good and super-pedantic, you can go and kill those disabled entries and have a ball with 10K+ issues. Now, the count is down to a mere 2449 items, and that's after the initial set of patches (see commits today)
			
* (abdac36) MSVS Code Analysis: took a copy of the 'All Rules' ruleset and copied it to Qiqqa, assigned all projects to use the new ruleset for Code Analysis and then disabled a few items in there to cut down on the number of (unwanted) warnings (before MSVS spit out 10K+ warnings).
  
  Incidentally, this also kills the build warning about the missing AllRules.ruleset in the Qiqqa repo root. :-)
			
* (0cb9c36) fix build error + `const`: both are edits triggered by MSVS Code Analysis Reporting, where the first one was a mis-edit (Exception -> CmdLineException: not good! :-) )
			
* (2d51712) addressing https://github.com/jimmejardine/qiqqa-open-source/issues/26 : nuked the Utilities/GUI/BrainStorm copy and copied/commented all diffs into the Qiqqa source tree: every diff edit references the issue https://github.com/jimmejardine/qiqqa-open-source/issues/26 in the comments.     `Utilities/GUI/BrainStorm/` === `Qiqqa/BrainStorm/`
			
* (041b160) NOT FIXED:
  + Warning	CA1801	Parameter 'node_control' of 'EllipseNodeContentControl.EllipseNodeContentControl(NodeControl, EllipseNodeContent)' is never used. Remove the parameter or use it in the method body.
  + Warning	CA1823	It appears that field 'EllipseNodeContentControl.circle_node_content' is never used or is only ever assigned to. Use this field or remove it.
  FIXED:
  + Warning	CA1802	Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.
			
* (e494e59) Warning	CA1802	Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.
			
* (9f640a6) Warning	CA1704	Correct the spelling of 'Unkown' in member name 'DragDropManager.DumpUnkownDropTypes(DragEventArgs)' or remove it entirely if it represents any sort of Hungarian notation.
			
* (76c47e7) Warning	CA1500	'node_from', a parameter declared in 'ConnectorControl.SetNodes(NodeControl, NodeControl)', has the same name as an instance field on the type. Change the name of one of these items.
  
  NOTE: code inspection has led me to change the code in this way that the events registered with the old nodes are UNregistered before the new nodes are assigned and events are REGISTERED with them. The old code was ambiguous, at least for me (human); I'm not entirely sure what the compiler had made of that. HMmm...
  
  Given these next two MSVS Code Analysis report Warnings, I guess that was a couple of lurking bugs right there:
  
  + Warning	CA1062	In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_from' before using it.
  + Warning	CA1062	In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_to' before using it.
			
* (39f50e0) updated links/refs in README.md
			
* (99bb181) 
  + Warning	CA1715	Prefix interface name 'RecurrentNodeContent' with 'I'.
  + Warning	CA1715	Prefix interface name 'Searchable' with 'I'.
  + Warning	CA1715	Prefix interface name 'Selectable' with 'I'.
			
* (c186a6b) Merge branch 'memleak-hunting'
  
			
* (44e1575) working on using a more developed build versioning approach. Have MSVS produce a unique version for each build, then (FUTURE WORK) add tooling to ensure all files carry the updated version number(s).
			
* (6e55393) tweak Logging class: remove unused methods and group all Warn and Error methods next to one another for an easier overview of the available overloads.
			
* (84fe992) feature added: store the source URL (!yay!) of any grabbed/sniffed PDF. Previously the source path of locally imported (via WatchFolder) PDFs was memorized in the Qiqqa database. It adds great value (to me at least) when Qiqqa can also store the source URL for any document -- this is very handy information to have as part of augmented document references!)
  
  This commit includes a lingering part of the memleak hunt refactor activity listed in the previous commit SHA-1: 177a2be0cff4e92a9ae285c61c2377bac1cbf1c4 as that code exists in the same source files and the activities were developed at the same time.
			
* (177a2be) As we've been hunting hard to diagnose memory leaks which made working with Qiqqa short-lived (OutOfMemory in about 15 minutes on a large 20K+ Library), we have applied best practices coding to all `IDisposible`-derived classes and augmented all `Dispose()` methods to ensure a fast and easy GC action by unlinking/detaching any referenced objects/instances ASAP. We are still in the process of going through the MSVS2019 Code Analysis Report that we've run at the end of the memleak hunting session, which took place in the last 245 hours.
  
  Best practices are gleaned from https://docs.microsoft.com/en-us/dotnet/api/system.object.finalize?view=netframework-4.8 and the Dispose+Dispose(true/false) coding pattern described there is applied everywhere where applicable.
  
  Also note that we employ the `?.` **null conditional operator**, which is part of C# 6.0 and described here: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
  It turns out the NANT build doesn't cope with this but I'm loath to revert to antiquity there, so the NANT build process has become a little hacky as MSVS2019 (IDE) can build (and debug) the Qiqqa binaries without a fuss, so we now use that one to build the binaries and the NANT build script for packaging (creating the `setup.exe`).
			
* (53c9751) Warning	CA1812	'BackingUp' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
			
* (9b6a61e) Warning	CA2000	In method 'ReportViewerControl.ButtonToPDF_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'doc' before all references to it are out of scope.
			
* (7aa66dd) 
  + Save To PDF had been disabled in the original Qiqqa source code. No idea why. Re-enabling it so that 'Save To PDF' is not a NIL activity anymore.
  + Warning	CA1811	'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
  + Warning	CA1811	'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
			
* (602a1eb) 
  + Save To PDF had been disabled in the original Qiqqa source code. No idea why. Re-enabling it so that 'Save To PDF' is not a NIL activity anymore.
  + Warning	CA1811	'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
  + Warning	CA1811	'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
			
* (a42a79b) 
  + Warning	CA1804	'RegionOfInterest.IsCloseTo(RegionOfInterest)' declares a variable, 'horizontal_distance', of type 'double', which is never used or is only assigned to. Use this variable or remove it.
  + Warning	CA1802	Field 'RegionOfInterest.PROXIMITY_MARGIN' is declared as 'static readonly' but is initialized with a constant value '0.0333333333333333'. Mark this field as 'const' instead.
			
* (80cc9b1) Warning	CA1812	'LinkedDocsAnnotationReportBuilder' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
			
* (45e6d8f) Warning	CA1053	Because type 'LegacyAnnotationConvertor' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.
			
* (17a97f8) 
  + Warning	CA1812	'InkToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  + Warning	CA1802	Field 'InkToAnnotationGenerator.INKS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Inks*'. Mark this field as 'const' instead.
			
* (9c925bd) 
  + Warning	CA1812	'HighlightToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  + Warning	CA1802	Field 'HighlightToAnnotationGenerator.HIGHLIGHTS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Highlights*'. Mark this field as 'const' instead.
			
* (dbf9173) Warning	CA1811	'AsyncAnnotationReportBuilder.OnShowTagOptionsComplete(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' appears to have no upstream public or protected callers.
			
* (25a337b) Warning	CA1804	'AsyncAnnotationReportBuilder.BuildReport(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' declares a variable, 'underline', of type 'Underline', which is never used or is only assigned to. Use this variable or remove it.
			
* (1025b6b) Warning	CA1053	Because type 'AsyncAnnotationReportBuilder' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.
			
* (490c7a4) Warning	CA1053	Because type 'AsyncAnnotationReportBuilder' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.
			
* (2032827) 
  + Warning	CA1500	'OnShowTagOptionsComplete', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
  + Warning	CA1500	'library', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
  + Warning	CA1500	'pdf_documents', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
			
* (59eab8a) Warning	CA1822	The 'this' parameter (or 'Me' in Visual Basic) of 'WordListCredibility.HasSufficientRepeatedWords(WordList)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.
			
* (f40f507) Warning	CA1802	Field 'WordListCredibility.REASONABLE_WORD_LIST_LENGTH' is declared as 'static readonly' but is initialized with a constant value '10'. Mark this field as 'const' instead.
			
* (40a3ce7) Warning	CA2204	Correct the spelling of the unrecognized token 'exst' in the literal '"\' does not exst"'.
			
* (23f5ca6) Warning	CA1812	'TextExtractEngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
			
* (9e9490a) Warning	CA1822	The 'this' parameter (or 'Me' in Visual Basic) of 'RegionExtractorTestWindow.ChooseBrush(PDFRegionLocator.Region)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.
			
* (1bfee58) Warning	CA1811	'PDFRegionLocator.GetRegions_FULLPAGE(Bitmap, out List<PDFRegionLocator.Region>, out int)' appears to have no upstream public or protected callers.
			
* (ff0a181) Warning	CA2000	In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'ocr' before all references to it are out of scope.
			
* (8f12898) Warning	CA2000	In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200F))' before all references to it are out of scope.
			
* (e6531a7) Warning	CA2204	Correct the spelling of the unrecognized token 'exst' in the literal '"\' does not exst"'.
			
* (bf35cc8) Warning	CA1801	Parameter 'no_kill' of 'OCREngine.MainEntry(string[], bool)' is never used. Remove the parameter or use it in the method body.
  
  Synced `no_kill` code with the code in TestExtractEngine.cs
			
* (abfe48c) Warning	CA1812	'OCREngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
			
* (f7f62b1) 
  + MSVS Code Analysis: Warning CA1053  Because type 'Backgrounds' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.icons
  + MSVS Code Analysis: Warning CA2211  Consider making 'Icons.QiqqaMedal' non-public or a constant.
			
* (aac705a) !@#$%^&* NANT doesn't support C#6.0 language constructs. Spent a few hours on NANT (obtained from GitHub) but cannot get it to work right now. Not spending more time on that distraction: patched the NANT build process to use the binaries which are (correctly and without hassle) produced by MSVC2019 IDE (Batch Build all or Rebuild All, Release target). See short notes in NANT build file and bash shell driver script too. QUICK HACK. QUICK HACK. QUICK HACK. Anyone with the time on their hands and the inclination to make NANT work with a *recent* MSVC setup: more power to you. Right now, this is a bleeding edge NANT rebuild PLUS patched build/packaging scripts.
			



2019-08-05
----------

			
* (d58bd7a) revert debug code that was part of commit SHA-1: 89307edfe7d5ba2b6de050de969d2910b147e682 -- some invalid BibTeX was crashing the Lucene indexer (`AddDocumentMetadata_BibTex()` would b0rk on a NULL `Key`)
  
  That problem was fixed in that commit at a higher level (in PDFDocument)
			
* (9ec342c) Don't let illegal multiple BibTeX entries for a single PDF record slip through unnoticed: one PDF having multiple BibTeX records should be noticed as a WARNING at least: the 'official' (i.e. *first*) BibTeX record is augmented with a Warning mentioning the multiplicity of the BibTeX for that PDF record.
  
  I had one example of this in my library where some slightly disparate BibeX records were copy-pasted into the BibTeX edit box for an obscure PDF: that had been meant "to be further investigated later" but that never happened and now showed up as a 'silent error' while checking Qiqqa library integrity.
			
* (89307ed) some invalid BibTeX was crashing the Lucene indexer (`AddDocumentMetadata_BibTex()` would b0rk on a NULL `Key`)
  
  Sample invalid BibTeX:
  
  ```
  @empty = delete?
  ```
			
* (dc740d7) fix/tweak FolderWatcher background task: make sure we AT LEAST process ONE(1) tiny batch of PDF files when there are any to process.
			
* (928d55b) trying to tackle the slow memory leak that's happening while Qiqqa is running  :-((   This is going on for a while now; can't seem to spot the culprit though.  :-((
			
* (ca89ba6) added misc files to the solution/projects: license, readme and copyright files.
			
* (5a518eb) mention the new `build-installer.sh` bash shell script as equivalent of the old `go.bat` in the README.
			
* (f5b80fa) little tug & tweak of the build `bash` shell script.
			
* (542fc81) fix msbuild error (oddly enough this was okay in MSVC2019 and compiled fine in the ID, but the NANT task via `./build-installer.sh` fails.  :-S  )
			
* (6dcc971) DBExplorer severely enhanced:
  - now supports wildcards in query parameters (% and _, but also * and ?, which are the MSDOS wildcards which translate directly to the SQL wildcards)
  - now supports GETting multiple records.
  - when GETting multiple records, DBExplorer not only prints the BibTeX for each record, but also the identifying fingerprint, verification MD5 and most importantly: the *PARSED* BibTeX (iff available) and BibTeX parse error diagnostics report.
  - when GETting multiple records, the DBExplorer output is also dumped to the file `Qiqqa.DBexplorer.QueryDump.txt` in the Qiqqa Library base directory. A previous DBExplorer query report dump will be REPLACED.
  - an extra input field has been added which allows the user to specify a maximum number of records to fetch: this speeds up queries and their reporting when working on large libraries with query criterai which would produce 1000nds of records if left unchecked.
  
  This allows to use the DBExplorer as a rough diagnostics tool to check the library internals, including a way to find erroneous/offending BibTeX entries which may cause havoc in Qiqqa elsewhere.
			



2019-08-04
----------

			
* (9c85acc) See also commit SHA-1: b38123a4ea67b4f3581826aeeac44a4ee0e9e39e: we now have Qiqqa open a VERY FAST and LEAN web page when we 'Open Browser'.
			
* (8176c4e) fix compiler warning due to unused variable.
			
* (0b7d3b4) fix/tweak: do NOT report 'Adds 0 of 0 document(s)' but clear the status part instead: now that we make Qiqqa work in small batches, this sort of thing MAY happen. (TODO: review WHY the Length of the todo array is actually ZERO, but low priority as things work and don't b0rk)
			
* (b743d72) fixing https://github.com/jimmejardine/qiqqa-open-source/issues/8: not only storing Left/Top coordinate, but also Width+Height of the Qiqqa.exe window
			
* (d59d6f0) fix crash in chat code when Qiqqa is shutting down (+ code review to uncover more spots where this might be happening)
  
  ```
  20190804.204351 INFO  [Main] Stopping MaintainableManager
  Exception thrown: 'System.NullReferenceException' in Qiqqa.exe
  20190804.204351 WARN  [9] There was a problem communicating with chat.
  System.NullReferenceException: Object reference not set to an instance of an object.
     at Qiqqa.Chat.ChatControl.ProcessDisplayResponse(MemoryStream ms) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 221
     at Qiqqa.Chat.ChatControl.PerformRequest(String url) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 127
  20190804.204351 WARN  [9] Chat: detected Qiqqa shutting down.
  ```
			
* (5a84d1b) I'm using `bash` rather than `cmd` as it comes with Git For Windows: provide a setup build script which you can invoke from the root from the project so you can work with git easily while also running this build command (and a few other things)
			
* (6febb35) Since ExpeditionManager is the biggest OutOfMemory troublemaker (when loading a saved session :-( ), we're augmenting the logging a tad to ease diagnosis. (https://github.com/jimmejardine/qiqqa-open-source/issues/19)
			
* (eab712b) debugging: uncollapsing rollups in dialog windows as part of a longer debugging activity. MUST REVERT!
			
* (170a4ca) augment BibTeX documentation: add URLs and note an old one as inactive (that webpage has disappeared from the Intarwebz. RIP.)
			
* (b38123a) 'Open New Browser' was looking pretty weird due to a website/page being  loaded which was unresponsive; now we're pointing to a more readily available webpage instead. (Though in my opinion 'Open Browser' should load a VERY MINIMAL webpage, which has absolutely *minimal* content...) Referenced URL has already been set up as part of commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505
			
* (02d2aa7) Mention the new CSL (Citation Styles) source websites in the credits. The links referenced in this text have already been set up as part of a previous commit in `WebsiteAccess` class. (commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505)
			
* (c8c2fd8) typo fix in comments
			
* (bab0499) code stability: Do not crash/fail when the historical progress file is damaged
			
* (820d833) refactor: collect (almost!) all URLs and keep them in WebsiteAccess so we have a single place where we need to go to update URLs. (In actual practice, there remain a FEW places where URLs stay; the number of files carrying URLs is significantly reduced anyway...)
			
* (866b15a) moving Sample.bib to be with the other TEST input files
			



2019-08-03
----------

			
* (2fc66dd) The easy bit of https://github.com/jimmejardine/qiqqa-open-source/issues/3: synced the Qiqqa/InCite/styles/ directory with the bleeding edge of the CSL repo at https://github.com/citation-style-language/styles (Note the 'bleeding edge' in there: I didn't use https://github.com/citation-style-language/styles-distribution !). DO NOTE that Qiqqa had several CSL style definitions which don't exist in this repository: these have been kept as-is.
			
* (37c1ae0) cut out all test code chunks using `#region Test` + `#if TEST ... #endif` around those chunks: this way, the test code will still exist ("just in case...") while it won't burden the compiler and never be included in a Qiqqa binary unless you *specifically* instruct the compiler to do so by `-define TEST` in the compiler options.
			
* (6128cfc) Flag all [Obsolete] entries as triggering a compiler error when still in use. Some class properties have been flagged in the comments as required for backwards compatibility of the serialization (reading & writing) of the configuration and BrainStorm files, so we added a `Obsolete(...)` report message accordingly.
  
  This change SHOULD have no effect on the build/code flow but 'cleans up' by making the use of these obsolete bits an ERROR instead of a WARNING.
  
  See also https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/common-attributes#Obsolete
			
* (7af6513) pick up the language from the dialog where the same is said as in the text file (though the rest of the wording is slightly different: the InCiteControl dialog content and this README do NOT have identical content!)
			
* (32dd1c3) whitespace police raid
			



2019-08-02
----------

			
* (95043ea) correct code and explain why for https://github.com/jimmejardine/qiqqa-open-source/issues/20
			
* (da3f853) corrected Folder Watch loop + checks for https://github.com/jimmejardine/qiqqa-open-source/issues/20: the intent here is very similar to the code done previously for https://github.com/jimmejardine/qiqqa-open-source/issues/17; we just want to add a tiny batch of PDF files from the Watch folder, irrespective of the amount of files waiting there to be added.
			
* (bd65680) HACKY trial to catch and cope with OutOfMemory errors due to the LDAStuff etc.: https://github.com/jimmejardine/qiqqa-open-source/issues/19
			
* (7bd3ee6) more work regarding https://github.com/jimmejardine/qiqqa-open-source/issues/10 and https://github.com/jimmejardine/qiqqa-open-source/issues/17: when you choose to either import a large number of PDF files at once via the Watch Folder feature *or* have just reset the Watch Directory before exiting Qiqqa, you'll otherwise end up with a long running process where many/all files in the Watched Directories are inspected and possibly imported: this is undesirable when the user has decided Qiqqa should terminate (by clicking close-window or Alt-F4 keyboard shortcut).
			
* (e83b4df) attempt to cope with https://github.com/jimmejardine/qiqqa-open-source/issues/19 a little better than a chain of internal failures. :-(
			
* (53f2ca8) code cleanup activity (which happened while going through the code for thread safely locks inspection)
			
* (8b2b3de) https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 2, looking for thread safety locks being applied correctly and completely. Also contains a few lines from work done before related to https://github.com/jimmejardine/qiqqa-open-source/issues/10 et al.
			
* (5dcda97) https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 1, looking for thread safety locks being applied correctly and completely: for example, a few places did not follow best practices by using the dissuaded `lock(this){...}` idiom (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement)
			
* (8a1d766) Fix https://github.com/jimmejardine/qiqqa-open-source/issues/17 by processing PDFs in any Qiqqa library in *small* batches so that Qiqqa is not unreponsive for a loooooooooooooong time when it is re-indexing/upgrading/whatever a *large* library, e.g. 20K+ PDF files. The key here is to make the '**infrequent background task**' produce *some* result quickly (like a working, yet incomplete, Lucene search index DB!) and then *updating*/*augmenting* that result as time goes by. This way, we can recover a search index for larger Qiqqa libraries!
			
* (98083f8) fixing MSVC2019-reported 'unreachable code' compiler reports by minimal refactoring
			
* (67e9efd) commenting out a property which was reported as 'unused' and wasn't flagged as `[Obsolete]` for serialization either... (compare to Qiqqa/Brainstorm/Nodes/NodeControlSceneData.cs where this attribute is used)
			
* (72b8d25) dialing up the debug/info logging to help me find the most annoying bugs, first of them: https://github.com/jimmejardine/qiqqa-open-source/issues/10, then https://github.com/jimmejardine/qiqqa-open-source/issues/13
			
* (1236790) Do NOT nuke the previous run's `Qiqqa.log` file in `C:\Users\<YourName>\AppData\Local\Quantisle\Qiqqa\Logs\`: **quick hack** to add a timestamp to the qiqqa log file so we'll be quickly able to inspect logs from multiple sessions. **Warning**: this MUST NOT be present in any future production version or you'll kill users by log file collection buildup on the install drive!
			
* (277c392) simple stuff: updating copyright notices from 2016 to 2019 and bumping the version from 80 to 81.  **Note**: The changelog in ClientVersion.xml runs ahead of what you will observe in these commits: I did the work first, then did these commits to lock it in as only now I am confident that my attempt at https://github.com/jimmejardine/qiqqa-open-source/issues/14 actually delivers a working exe that's equal or better than last commercial release v79 (for me at least; caveat emptor!)
			
* (b359039) update existing Syncfusion files from v14 to v17, which helps resolve https://github.com/jimmejardine/qiqqa-open-source/issues/11
  
  **Warning**: I got those files by copying a Syncfusion install directory into qiqqa::/libs/ and overwriting existing files. v17 has a few more files, but those seem not to be required/used by Qiqqa, as **only overwriting what was already there** in the **Qiqqa** install directory seems to deliver a working Qiqqa tool. :phew:
			
* (142e06d) 
  - point Microsoft Visual Studio project files to the (renamed) README.md files following https://github.com/jimmejardine/qiqqa-open-source/pull/6
  - remove Syncfusion LICX file as we migrate from SyncFusion 14 to Syncfusion 17; see also bug report https://github.com/jimmejardine/qiqqa-open-source/issues/11
			
* (b00c722) updating 7zip to latest release
			



2019-07-30
----------

			
* (4c2de1f) Merge remote-tracking branch 'remotes/jimmejardine-original/master'

 *  Merge branch 'master' of https://github.com/jimmejardine/qiqqa-open-source





v80 (FOSS)
==========

v80 software installer published.

> ## Note
>
> This release is **binary compatible with v80 and v79**: any library created using this version MUST be readable and usable by v80 and v79 software releases.




			
- Qiqqa goes Open Source!!!
- Enabled ALL Premium and Premium+ features for everyone.
- Removed all Web Library capabilities (create/sync/manage)
- Added the ability to copy the entire contents of a former Web Library into a library - as a migration path from Qiqqa v79 to v80



2019-07-15
----------

			
* (db3c118) Merge branch 'master' of https://github.com/jimmejardine/qiqqa-open-source
			
* (77469ba) added first build result as some people wish to download it
			
* (28c0f3f) Merge pull request #6 from GerHobbelt/readme-cleanup-for-github
  
  cleaned the README files a bit and made them ready for GitHub
			
* (90be541) Update README.md
			
* (5db5b2e) Update README.md
			



2019-07-14
----------

			
* (3e4d501) Merge branch 'readme-cleanup-for-github'
			
* (4629eee) cleaned the README files a bit and made them ready for GitHub (MarkDown formatting)
			
* (51214fe) Update README.md
			
* (26d171a) README: tables fixed for GFM
			
* (4dc438c) Update README.md
			
* (a8f762d) tweaking the README tables for GitHub to render them correctly.
			
* (874e600) fix README rendering on GitHub
			
* (5f23d01) cleaned the README files a bit and made them ready for GitHub ( MarkDown formatting )
			
* (de85ee2) Rename the `~readme.txt` files describing directory content to `README.md` so that GitHub picks them up and displays them in the web view of the repo.
			
* (d05e460) Update README.md
			



2019-07-03
----------

			
* (4ad8e5f) open source version is now able to 'see' the legacy web libraries
			
* (4b2ea5e) initial contribution
			
* (dd17b0a) Update README.md
			
* (60199c0) Create README.md
			



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

