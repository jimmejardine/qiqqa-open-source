2019-08-13
----------

* Slighty more informative/easier to track back log lines for browsing web pages in Qiqqa
* (lint) `const`-ing a few variables, which really are constants
* Removing old debug lines that aren't required any more...
* Prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/23 to make transition to latest LuceneNET easier to do.
* Prepwork / minimal refactoring of `Object[]` to `PQRecord` done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.
* fix compile error introduced with commit SHA-1: e774847fe9b317d40ba700315dd5e67f7888850a * Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.
* fiddling with website theme...
* Move the reference HTML file out of the way. https://github.com/jimmejardine/qiqqa-open-source/issues/38
* GH_PAGES: Set theme jekyll-theme-tactile
* done: https://github.com/jimmejardine/qiqqa-open-source/issues/38 -- Part 14: added the images. Turns out there are two screens (screen0003.ai and screen0014.ai that remain unused. Looks like this was partly old stuff and maybe a chunk that still needs to go into a section of the manual. Haven't checked precisely as the job at hand was *conversion*.
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 13: adding the images. First one. Let's check because I always screw this bit of MD notation up.  :-S
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 12: Done with the roundtrip check and HTML + MD cleanup. Now we only need to get back all the images in there...
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 11: Basically we're all good now. Tweaked the MD source to use modern MarkDown header marks. Found that we still need to correct lists in there as TurnDown didn't catch all the &middot encoded lists from Word, so that'll be next, together picking up the Unicode SmartQuotes from the RoundTrip copy so that the HTML file will serve as a reference for subsequent MarkDown source editing work...
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 10: Some swift manual edits on initial comparison: looks like everything made it, but there are notable differences. Patching the destination HTML and source MD MarkDown file to ensure the next round will correct these render mistakes...
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 9: Cleaning the source HTML using https://html-cleaner.com/ to kill the MSWord left-overs and then another round of https://htmlformatter.com/ for maximum similarity (and thus faster work in reviewing the diffs next)
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 8: Same https://htmlformatter.com/ applied to source HTML
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 7: A swift kill of all styling and Dillinger editor/line left-overs in the HTML: one regex replace in Sublime.
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 6: Applying https://htmlformatter.com/ to the Dillinger output
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 5: RoundTripping the MarkDown to HTML (we need to check everything made it through and this is the quickest way for a large document like this: at the end waits a fast Beyond Compare session going through the diffs of the source and roundtrip HTML files...). Uses https://dillinger.io/
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : Part 4: Pulled the HTML through TurnDown ( http://domchristie.github.io/turndown/ ) to produce an initial MarkDown version of the documentation. Need to round-trip it to ensure we didn't loose any important chunks. :-)
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 3: patching the HTML.
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead. Part 2.
* prepwork for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : exported the MSWord DOCX source to HTML. Patched the generated `Qiqqa Manual_files/*.*` paths to point to `images/*.*` instead.
* prepwork done for https://github.com/jimmejardine/qiqqa-open-source/issues/38 : extracted all displays from the pptx file (I've no problem using PowerPoint for stuff like this, but I have more control over publishing/output when using tools like Adobe Illustrator. Besides, the images that stick partly outside the PowerPoint pages are begging for clipping issues, etc. when rendering.
* Prepwork done on https://github.com/jimmejardine/qiqqa-open-source/issues/23 as it turns out to be a tougher nut to crack then I initially expected.
* Do NOT write old format (.NET serialized binary) configuration files any more: DO load them when the new JSON format is missing, but only write the NEW JSON format. https://github.com/jimmejardine/qiqqa-open-source/issues/41
* NuGet: package Newtonsoft.JSON: the JSON-to-object deserializer/serializer. Lingering work from https://github.com/jimmejardine/qiqqa-open-source/issues/41
* https://github.com/jimmejardine/qiqqa-open-source/issues/41 tweak: more human-readable JSON formatting
* fix https://github.com/jimmejardine/qiqqa-open-source/issues/42: fixed crash.
* more of the same as SHA-1: af670a88f8fb56d090ed8d04bfb9b08cb0e53b33 * minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball
* Hm, looks like the NANT build script (`./build-installer.sh`) picks up the new Newtonsoft.Json.dll location. Lucky break. Killed the old one as we now *upgrade* JSON.NET ( https://github.com/jimmejardine/qiqqa-open-source/issues/41 )
* work done on https://github.com/jimmejardine/qiqqa-open-source/issues/41, which was triggered by the bugging and b0rking of https://github.com/jimmejardine/qiqqa-open-source/issues/40, hence a few bits from that one will peek through here.
* Using Json.NET as advised by Microsoft: https://docs.microsoft.com/en-us/dotnet/api/system.web.script.serialization.javascriptserializer?view=netframework-4.8
    
  As mentioned in the Deprecation Notice of the old serializer code:
    
  > .NET binary serialization causes too much trouble, e.g. https://stackoverflow.com/questions/6825819/how-can-i-tell-when-what-is-loading-certain-assemblies and https://social.msdn.microsoft.com/forums/vstudio/en-US/7192f23e-7d43-47b5-b401-5fcd19671cf6/invalidcastexception-thrown-when-casting-to-the-same-type. Use Json.NET instead. And then there's https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/how-to-enable-and-disable-automatic-binding-redirection (sigh)

* minimally tweak UI elements and make Microsoft Visual Studio :: XAML Designer *NOT* barf a hairball (Fatal System Exception) on many XAML dialogs/panels in Qiqqa.
    
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

* Minor UI tweak of BibTeX metadata pane in main UI: distance between buttons
* Tweak the AnnotationReportOptionsWindow XAML a little so it shows up in its entirety in de MSVS Designer.
* Fiddling with the size of the proxy config dialog panel: make it wider so we can more easily enter&see host URI and user password.
* tweak the About message: now also show the *full* build version next to the classic `vNN` version.
* Tweak the logging lines for the InCite webbrowser so its start and end can be easily detected in the logfiles.
* removed unused variable assignment
* prep for https://github.com/jimmejardine/qiqqa-open-source/issues/38
* add 'how to build setup.exe' instructions to README.md
* make Qiqqa main app and QiqqaOCR logging easily recognizable: `[Q]` or `[OCR]` tag per logline.
 
  Also print the QC-reported memory usage as a fixed-width number in MBytes



# version 81.0.7158.38371 :: alpha test release

* updated CHANGELOG files



2019-08-07
----------

  * ALPHA/TEST RELEASE v81 : version 81.0.7158.38371
  * log outgoing activity: posting BibTeX info to bibtexsearch.com aggregator
  * re-added to 'Add This PDF to Library' button in the browser; TODO: make it work akin to the <embed> handling to prevent confusion: when the browser shows a single PDF, it MAY be an <embed> web page and we should account for that!
  * IMPORTANT: this bad boy (an overzealous Dispose() which I introduced following up on the MSVS Code Analysis Reports) prevented Qiqqa from properly fetching and importing various PDFs from the Sniffer. (click on link would show the PDFs but not open them in Qiqqa nor import them into the Qiqqa library)
  * some titles/sentences seem to come with leading whitespace; title suggestion construction would produce suggested titles with leading and trailing whitespace. Fixed.
  * #ifdef/#endif unused code
  * - fix crash in PDF import when website/webserver does not provide a Content-Disposable HTTP response header
    - add ability to cope with <embed> PDF links, e.g. when a HTML page is shown with PDF embedded instead of the PDF itself
    - detect PDF files in URLs which have query parameters: '.pdf' is not always the end of the URL for downloading the filename
  * revert/fix NANT build script to produce a setup.exe once again.
  * added CHANGELOG (partly edited & full version using git log)
  * moving some Info-level logging to Debug level as that's what it is, really. (Dispose activity tracking et al)
  * added TODO to remember my own DB ...
  * Whoops. Crash when quickly opening + closing + opening.... Sniffer windows: CLOSE != DISPOSE. Crash due to loss of search_options binding on second opening...
  * Only when you play with it, you discover what works. The HasSourceURL/Local/Unsourced choices should be OR-ed together as that feels intuitive, while we also want to see 'sans PDF' entries as we can use the Sniffer to dig up the PDF on the IntarWebz if we're lucky. Meanwhile, 'invert' should clearly be positioned off to a corner to signify its purpose: inverting your selection set (while it should **probably** :thinking: have no effect if a specific document was specified by the user: then we're looking at a particular item PLUS maybe some other stuff?
  * whoops. coding typo fix. https://github.com/jimmejardine/qiqqa-open-source/issues/28
  * Merge branch 'n29'
  * Merge branch 'work'
  * Sniffer Features:
    - add checkboxes to (sub)select documents which have a URL source registered with them or no source registered at all. (https://github.com/jimmejardine/qiqqa-open-source/issues/29)
    - add 'invert' logic for the library filter (https://github.com/jimmejardine/qiqqa-open-source/issues/30)
  * fix https://github.com/jimmejardine/qiqqa-open-source/issues/28: turns out Qiqqa is feeding all the empty records to the PubMed-to-BibTex converter, which is throwing a tantrum. Improved checks and balances and all that. Jolly good, carry on, chaps. :-)
  * work being done on https://github.com/jimmejardine/qiqqa-open-source/issues/29 + https://github.com/jimmejardine/qiqqa-open-source/issues/30: augmenting our Jolly Sniffer.
  * added TODO's for a bit of ho-hum that's been bothering me all week. To Be Researched while other issues get attention first.
  * report complete build version in logging. v80, v79, ...: it's not good enough when you want to track down the 'currentness' of the log :-)
  * improving the logging while we hunt for the elusive Fail Creatures... (One of them being that OutOfMemoryException that somehow turns up out of the blue while the app still has plenty memory to go @ 200-300MB GC-reported allocation. :thinking:
  * further work on CA2000 from the Code Analysis Report: apply using (ISisposable) {...} where possible.
  * refactoring event handling code (using null conditionals as suggested by MSVS Code Analysis Report)
  * part of IDisposable cleanup work following the advice of the MSVS Code Analysis Report as much as possible (mostly the first bunch of CA2000 report lines) :
    * Message    IDE0067    Disposable object created by 'new FolderBrowserDialog()' is never disposed
    * Message    IDE0067    Disposable object created by 'new Tesseract()' is never disposed
    * Message    IDE0067    Disposable object created by 'out ms_image' is never disposed
    * Warning    CA1001    Implement IDisposable on 'BibTeXEditorControl' because it creates members of the following IDisposable types: 'WeakDependencyPropertyChangeNotifier'. If 'BibTeXEditorControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'CSLProcessorOutputConsumer' because it creates members of the following IDisposable types: 'GeckoWebBrowser'. If 'CSLProcessorOutputConsumer' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'FolderWatcher' because it creates members of the following IDisposable types: 'FileSystemWatcher'. If 'FolderWatcher' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'GoogleBibTexSnifferControl' because it creates members of the following IDisposable types: 'PDFRendererControl'. If 'GoogleBibTexSnifferControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'HtmlLexicalAnalyzer' because it creates members of the following IDisposable types: 'StringReader'.
    * Warning    CA1001    Implement IDisposable on 'Library' because it creates members of the following IDisposable types: 'LibraryIndex'. If 'Library' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'LibraryCatalogOverviewControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'LibraryCatalogOverviewControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'MainWindow' because it creates members of the following IDisposable types: 'StartPageControl'. If 'MainWindow' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'NotificationManager' because it creates members of the following IDisposable types: 'ReaderWriterLockSlim', 'AutoResetEvent'. If 'NotificationManager' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'PDFAnnotationNodeContentControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'PDFAnnotationNodeContentControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'PDFDocumentNodeContentControl' because it creates members of the following IDisposable types: 'LibraryIndexHoverPopup'. If 'PDFDocumentNodeContentControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'PDFPrinterDocumentPaginator' because it creates members of the following IDisposable types: 'DocumentPage'.
    * Warning    CA1001    Implement IDisposable on 'ReadOutLoudManager' because it creates members of the following IDisposable types: 'SpeechSynthesizer'. If 'ReadOutLoudManager' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'TagEditorControl' because it creates members of the following IDisposable types: 'WeakDependencyPropertyChangeNotifier'. If 'TagEditorControl' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1001    Implement IDisposable on 'UrlDownloader.DownloadAsyncTracker' because it creates members of the following IDisposable types: 'UrlDownloader.WebClientWithCompression'. If 'UrlDownloader.DownloadAsyncTracker' has previously shipped, adding new members that implement IDisposable to this type is considered a breaking change to existing consumers.
    * Warning    CA1063    Modify 'AugmentedPdfLoadedDocument.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'AugmentedPdfLoadedDocument.~AugmentedPdfLoadedDocument()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'AugmentedPopupAutoCloser.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'AugmentedPopupAutoCloser.~AugmentedPopupAutoCloser()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'BrainstormControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'BrainstormControl.~BrainstormControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'ChatControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'ChatControl.~ChatControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'LibraryIndex.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'LibraryIndex.~LibraryIndex()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'LibraryIndexHoverPopup.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'LibraryIndexHoverPopup.~LibraryIndexHoverPopup()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'LuceneIndex.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'LuceneIndex.~LuceneIndex()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'PDFReadingControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'PDFReadingControl.~PDFReadingControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'PDFRendererControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'PDFRendererControl.~PDFRendererControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'ProcessOutputReader.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'ProcessOutputReader.~ProcessOutputReader()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'ReportViewerControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'ReportViewerControl.~ReportViewerControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'SceneRenderingControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'SceneRenderingControl.~SceneRenderingControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'SpeedReadControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'SpeedReadControl.~SpeedReadControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'StartPageControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'StartPageControl.~StartPageControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'StopWatch.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'StopWatch.~StopWatch()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'WebBrowserControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'WebBrowserControl.~WebBrowserControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA1063    Modify 'WebBrowserHostControl.Dispose()' so that it calls Dispose(true), then calls GC.SuppressFinalize on the current object instance ('this' or 'Me' in Visual Basic), and then returns.
    * Warning    CA1063    Modify 'WebBrowserHostControl.~WebBrowserHostControl()' so that it calls Dispose(false) and then returns.
    * Warning    CA2000    In method 'AssociatePDFWithVanillaReferenceWindow.CmdLocal_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new OpenFileDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'BibTexExport.Export(List<PDFDocument>)', call System.IDisposable.Dispose on object 'new SaveFileDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'BitmapImageTools.CropImageRegion(Image, double, double, double, double)', call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
    * Warning    CA2000    In method 'BitmapImageTools.FromImage(Image)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'BrowserStarter.OpenBrowser_REGISTRY(string)', call System.IDisposable.Dispose on object 'p' before all references to it are out of scope.
    * Warning    CA2000    In method 'ChartTools.renderNoDatasetMessage(Graphics)', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
    * Warning    CA2000    In method 'ClipboardTools.SetRtf(string)', call System.IDisposable.Dispose on object 'rich_text_box' before all references to it are out of scope.
    * Warning    CA2000    In method 'ConsoleRedirector.CaptureConsole()', call System.IDisposable.Dispose on object 'cr' before all references to it are out of scope.
    * Warning    CA2000    In method 'CSLEditorControl.OnBibliographyReady(CSLProcessorOutputConsumer)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'DocumentConversion.ConvertorDOC(string, string)', call System.IDisposable.Dispose on object 'converter' before all references to it are out of scope.
    * Warning    CA2000    In method 'DocumentConversion.ConvertorDOC(string, string)', call System.IDisposable.Dispose on object 'word_document' before all references to it are out of scope.
    * Warning    CA2000    In method 'ExportToWord.ExportToTextAndLaunch(PDFDocument)', call System.IDisposable.Dispose on object 'report_view_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'FolderWatcherChooser.CmdAddFolder_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'Fonts.getLargestFont(Graphics, string, double)', object 'font' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
    * Warning    CA2000    In method 'Ghostscript.RenderPage_AsMemoryStream(string, int, int, string, ProcessPriorityClass)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'GoogleAnalysicsSubmitter.Submit_BACKGROUND(Feature)', call System.IDisposable.Dispose on object 'wc' before all references to it are out of scope.
    * Warning    CA2000    In method 'GoogleScholarScraper.ScrapeUrl(IWebProxy, string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'GUITools.RenderToBitmapImage(UIElement)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'HtmlFromXamlConverter.ConvertXamlToHtml(string)', call System.IDisposable.Dispose on object 'new StringReader(xamlString)' before all references to it are out of scope.
    * Warning    CA2000    In method 'HtmlFromXamlConverter.ConvertXamlToHtml(string)', call System.IDisposable.Dispose on object 'new StringWriter(htmlStringBuilder)' before all references to it are out of scope.
    * Warning    CA2000    In method 'ImportFromFolder.FolderLocationButton_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'ImportFromThirdParty.GetFolderNameFromDialog(string, string)', call System.IDisposable.Dispose on object 'ofd' before all references to it are out of scope.
    * Warning    CA2000    In method 'IntranetLibraryChooserControl.ObjButtonFolderChoose_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'IPCServer.StartServerPump()', call System.IDisposable.Dispose on object 'npss' before all references to it are out of scope.
    * Warning    CA2000    In method 'LibraryBundleCreationControl.CmdCreateBundle_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'dialog' before all references to it are out of scope.
    * Warning    CA2000    In method 'LibraryControl.ButtonAddDocuments_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'new OpenFileDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'LibraryExporter.Export(Library, List<PDFDocument>)', call System.IDisposable.Dispose on object 'new FolderBrowserDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'LibraryIndexHoverPopup.DisplayThumbnail()', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_document.PDFRenderer.GetPageByHeightAsImage(this.page, (this.ImageThumbnail.Height / IMAGE_PERCENTAGE)))' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentPagesWithQuery(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentPagesWithQuery(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsSimilarToDocument(string)', call System.IDisposable.Dispose on object 'new StreamReader(document_filename)' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsWithQuery(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsWithQuery(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsWithWord(string)', call System.IDisposable.Dispose on object 'index_reader' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneIndex.GetDocumentsWithWord(string)', call System.IDisposable.Dispose on object 'index_searcher' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneMoreLikeThis.Like(FileInfo)', call System.IDisposable.Dispose on object 'new StreamReader(f.FullName, Encoding.Default)' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'Console.OpenStandardOutput()' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'r' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneMoreLikeThis.Main(string[])', call System.IDisposable.Dispose on object 'searcher' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneMoreLikeThis.RetrieveTerms(int)', call System.IDisposable.Dispose on object 'new StreamReader(text[j])' before all references to it are out of scope.
    * Warning    CA2000    In method 'LuceneSimilarityQueries.FormSimilarQuery(string, Analyzer, string, Hashtable)', call System.IDisposable.Dispose on object 'new StringReader(body)' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OnShowTagOptionsComplete(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)', call System.IDisposable.Dispose on object 'report_view_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OpenDocument(PDFDocument, int?, string, bool)', object 'pdf_reading_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'pdf_reading_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OpenNewBrainstorm()', object 'brainstorm_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'brainstorm_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OpenSampleBrainstorm()', object 'brainstorm_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'brainstorm_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OpenSpeedRead()', object 'src' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'src' before all references to it are out of scope.
    * Warning    CA2000    In method 'MainWindowServiceDispatcher.OpenWebBrowser()', object 'web_browser_control' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'web_browser_control' before all references to it are out of scope.
    * Warning    CA2000    In method 'MultiChart2D.performPaintY1Axis(Graphics, ChartRegion, ChartRegion, Point2D, Point2D)', call System.IDisposable.Dispose on object 'string_format' before all references to it are out of scope.
    * Warning    CA2000    In method 'MultiChart2D.performPaintY2Axis(Graphics, ChartRegion, ChartRegion, Point2D, Point2D)', call System.IDisposable.Dispose on object 'string_format' before all references to it are out of scope.
    * Warning    CA2000    In method 'MuPDFRenderer.ReadEntireStandardOutput(string, ProcessPriorityClass)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'MYDDatabase.OpenMYDDatabase(string)', object 'ms' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200F))' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFAnnotationToImageRenderer.RenderAnnotation(PDFDocument, PDFAnnotation, float)', call System.IDisposable.Dispose on object 'new MemoryStream(pdf_document.PDFRenderer.GetPageByDPIAsImage(pdf_annotation.Page, dpi))' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFCameraLayer.GetSnappedImage(Point, Point)', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_renderer_control_stats.pdf_document.PDFRenderer.GetPageByDPIAsImage(this.page, 150F))' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFInterceptor.Response(HttpChannel)', call System.IDisposable.Dispose on object 'stream_listener_tee' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFOverlayRenderer.RenderAnnotations(Image, PDFDocument, int, PDFAnnotation)', call System.IDisposable.Dispose on object 'highlight_pen' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFOverlayRenderer.RenderHighlights(Image, PDFDocument, int)', call System.IDisposable.Dispose on object 'image_attributes' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFOverlayRenderer.RenderHighlights(int, int, PDFDocument, int)', call System.IDisposable.Dispose on object 'highlight_pen' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFOverlayRenderer.RenderHighlights(int, int, PDFDocument, int)', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFPrinterDocumentPaginator.GetPage(int)', call System.IDisposable.Dispose on object 'new MemoryStream(this.pdf_renderer.GetPageByDPIAsImage(page, 300F))' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, false, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PDFTextExtractor.GetNextJob()', call System.IDisposable.Dispose on object 'new PDFTextExtractor.NextJob(this, job, true, this.queue_lock)' before all references to it are out of scope.
    * Warning    CA2000    In method 'PkiEncryption.Decrypt(string, string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
    * Warning    CA2000    In method 'PkiEncryption.Encrypt(string, string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
    * Warning    CA2000    In method 'PkiEncryption.GenerateKeys(out string, out string)', call System.IDisposable.Dispose on object 'rsaProvider' before all references to it are out of scope.
    * Warning    CA2000    In method 'PNMLoader.CreateBitmapOffSize()', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
    * Warning    CA2000    In method 'PNMLoader.CreateGreyMap()', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
    * Warning    CA2000    In method 'PNMLoader.CreateGreyMapOffSize(byte[])', object 'bitmap' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'bitmap' before all references to it are out of scope.
    * Warning    CA2000    In method 'PNMLoader.PNMLoader(string)', object 'stream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'stream' before all references to it are out of scope.
    * Warning    CA2000    In method 'ProcessSpawning.SpawnChildProcess(string, string, ProcessPriorityClass)', object 'process' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'process' before all references to it are out of scope.
    * Warning    CA2000    In method 'ReversibleEncryption.Decrypt(byte[])', object 'encryptedStream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'encryptedStream' before all references to it are out of scope.
    * Warning    CA2000    In method 'ReversibleEncryption.Encrypt(string)', object 'memoryStream' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'memoryStream' before all references to it are out of scope.
    * Warning    CA2000    In method 'ReversibleEncryption.ReversibleEncryption()', call System.IDisposable.Dispose on object 'rm' before all references to it are out of scope.
    * Warning    CA2000    In method 'SerializeFile.ProtoSaveToByteArray<T>(T)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'SoraxPDFRendererDLLWrapper.GetPageByDPIAsImage_LOCK(SoraxPDFRendererDLLWrapper.HDOCWrapper, int, float)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamFingerprint.FromStream_DOTNET(Stream)', call System.IDisposable.Dispose on object 'sha1' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamFingerprint.FromText(string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamMD5.FromBytes(byte[])', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamMD5.FromStream(Stream)', call System.IDisposable.Dispose on object 'md5' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamMD5.FromText(string)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'StreamToFile.CopyBufferToStream(Stream, byte[], int)', call System.IDisposable.Dispose on object 'ms' before all references to it are out of scope.
    * Warning    CA2000    In method 'TopographicalChart.OnPaint(PaintEventArgs)', call System.IDisposable.Dispose on object 'brush' before all references to it are out of scope.
    * Warning    CA2000    In method 'TopographicalChart.OnPaint(PaintEventArgs)', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
    * Warning    CA2000    In method 'TopographicalChart.showForm()', call System.IDisposable.Dispose on object 'form' before all references to it are out of scope.
    * Warning    CA2000    In method 'TopographicalChart.showFormModal()', call System.IDisposable.Dispose on object 'form' before all references to it are out of scope.
    * Warning    CA2000    In method 'TweetControl.GenerateRtfCitationSnippet_OnBibliographyReady(CSLProcessorOutputConsumer)', call System.IDisposable.Dispose on object 'rich_text_box' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebBrowserHostControl.OpenNewWindow()', object 'wbc' is not disposed along all exception paths. Call System.IDisposable.Dispose on object 'wbc' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.GenericCustomiseChooser(string, string)', call System.IDisposable.Dispose on object 'dialog' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'font' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'image_attributes' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'mat' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'new MemoryStream(base.Current.pdf_document.PDFRenderer.GetPageByHeightAsImage(1, (WebLibraryDetailControl.PREVIEW_IMAGE_HEIGHT / WebLibraryDetailControl.PREVIEW_IMAGE_PERCENTAGE)))' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebLibraryDetailControl.UpdateLibraryStatistics_Stats_Background_CoverFlow()', call System.IDisposable.Dispose on object 'new StringFormat()' before all references to it are out of scope.
    * Warning    CA2000    In method 'WebsiteAccess.DownloadFile(WebsiteAccess.OurSiteFileKind)', call System.IDisposable.Dispose on object 'web_client' before all references to it are out of scope.
    * Warning    CA2000    In method 'Word2007Export.Export(List<PDFDocument>)', call System.IDisposable.Dispose on object 'new SaveFileDialog()' before all references to it are out of scope.
    * Warning    CA2000    In method 'XMLTools.ToString(XmlDocument)', call System.IDisposable.Dispose on object 'sw' before all references to it are out of scope.
    * Warning    CA2202    Object 'app_key' can be disposed more than once in method 'UserRegistry.Read(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 39
    * Warning    CA2202    Object 'app_key' can be disposed more than once in method 'UserRegistry.Write(string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 28
    * Warning    CA2202    Object 'compressed_stream' can be disposed more than once in method 'PostcodeOutcodes.PostcodeOutcodes()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
    * Warning    CA2202    Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 125
    * Warning    CA2202    Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 148
    * Warning    CA2202    Object 'compressed_stream' can be disposed more than once in method 'ScrabbleWords.ScrabbleWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 36
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'BibTeXImporter.BibTeXImporter(Library, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 37
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_SYNCHRONOUS(Library, object)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 285
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'LDASampler.FastLoad(string, int[][])'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 218
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'LDASampler.FastSave(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 176
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'UnhandledExceptionMessageBox.PopulateLog()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 213
    * Warning    CA2202    Object 'fs' can be disposed more than once in method 'VisualGalleryControl.ExportJpegImage(PdfDictionary, ref int)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 64
    * Warning    CA2202    Object 'memoryStream' can be disposed more than once in method 'ReversibleEncryption.Encrypt(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 95
    * Warning    CA2202    Object 'npss_in_callback' can be disposed more than once in method 'IPCServer.StartServerPump()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 66
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'CSLProcessorTranslator_AbbreviationsManager.LoadDefaultAbbreviations()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'PostcodeOutcodes.PostcodeOutcodes()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 80
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 125, 126
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'ScrabbleWords.CreateWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 148, 149
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'ScrabbleWords.ScrabbleWords()'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 36
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'SerializeFile.LoadCompressed(string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 297
    * Warning    CA2202    Object 'stream' can be disposed more than once in method 'SerializeFile.SaveCompressed(string, object)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 286
    * Warning    CA2202    Object 'sub_app_key' can be disposed more than once in method 'UserRegistry.Read(string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 77
    * Warning    CA2202    Object 'sub_app_key' can be disposed more than once in method 'UserRegistry.Write(string, string, string)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 63
    * Warning    CA2202    Object 'sw' can be disposed more than once in method 'PubMedXMLToBibTex.XMLNodeToPrettyString(XmlNode)'. To avoid generating a System.ObjectDisposedException you should not call Dispose more than one time on an object.: Lines: 119
    * Warning    CA2213    'ChatControl' contains field 'ChatControl.timer' that is of IDisposable type: 'Timer'. Change the Dispose method on 'ChatControl' to call Dispose or Close on this field.
    * Warning    CA2213    'LibraryIndex' contains field 'LibraryIndex.word_index_manager' that is of IDisposable type: 'LuceneIndex'. Change the Dispose method on 'LibraryIndex' to call Dispose or Close on this field.
    * Warning    CA2213    'PDFReadingControl' contains field 'PDFReadingControl.pdf_renderer_control' that is of IDisposable type: 'PDFRendererControl'. Change the Dispose method on 'PDFReadingControl' to call Dispose or Close on this field.
    * Warning    CA2213    'WebBrowserHostControl' contains field 'WebBrowserHostControl.wbc_browsing' that is of IDisposable type: 'WebBrowserControl'. Change the Dispose method on 'WebBrowserHostControl' to call Dispose or Close on this field.
  * done work on https://github.com/jimmejardine/qiqqa-open-source/issues/27 and on the lockups of Qiqqa (some critical sections in there were **humongous** in both code side and run-time duration; now the number of lock-ups due to *very* slow loading PDFs coming in from the Qiqqa Sniffer should be quite reduced: work related to https://github.com/jimmejardine/qiqqa-open-source/issues/18
  * better handling of 'unreachable code' warnings
  * Code looks like a swap/insert code pattern gone bad: did we find a bug?  Found due to MSVS Code Analysis Message    IDE0059    Unnecessary assignment of a value to 'pageranks_temp'

2019-08-06
----------

  * several like this: Warning    CA1500    'filter_terms', a parameter declared in 'LibraryCatalogControl.OnFilterChanged(LibraryFilterControl, List<PDFDocument>, Span, string, Dictionary<string, double>, PDFDocument)', has the same name as an instance field on the type. Change the name of one of these items.
  * we also don't care (yet) to validate arguments coming into public methods and neither are we interested to hear about methods which might be better off as properties' getter/setter.
  * don't care about public nested visibility issues and 'can be simplified' new object instantiations, which merely introduce a newer C# language feature that I don't particularly like (contrary to null conditionals ;-) )
  * patched the Code Analysis ruleset for now: don't care about internationalization, app-specific exceptions and the lot. Once we're good and super-pedantic, you can go and kill those disabled entries and have a ball with 10K+ issues. Now, the count is down to a mere 2449 items, and that's after the initial set of patches (see commits today)
  * MSVS Code Analysis: took a copy of the 'All Rules' ruleset and copied it to Qiqqa, assigned all projects to use the new ruleset for Code Analysis and then disabled a few items in there to cut down on the number of (unwanted) warnings (before MSVS spit out 10K+ warnings).
    Incidentally, this also kills the build warning about the missing AllRules.ruleset in the Qiqqa repo root. :-)
  * fix build error + const: both are edits triggered by MSVS Code Analysis Reporting, where the first one was a mis-edit (Exception -> CmdLineException: not good! :-) )
  * addressing https://github.com/jimmejardine/qiqqa-open-source/issues/26 : nuked the Utilities/GUI/BrainStorm copy and copied/commented all diffs into the Qiqqa source tree: every diff edit references the issue https://github.com/jimmejardine/qiqqa-open-source/issues/26 in the comments.     Utilities/GUI/BrainStorm/ === Qiqqa/BrainStorm/
  * NOT FIXED:
    + Warning    CA1801    Parameter 'node_control' of 'EllipseNodeContentControl.EllipseNodeContentControl(NodeControl, EllipseNodeContent)' is never used. Remove the parameter or use it in the method body.
    + Warning    CA1823    It appears that field 'EllipseNodeContentControl.circle_node_content' is never used or is only ever assigned to. Use this field or remove it.
    FIXED:
    + Warning    CA1802    Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.
  * Warning    CA1802    Field 'EllipseNodeContentControl.STROKE_THICKNESS' is declared as 'static readonly' but is initialized with a constant value '1'. Mark this field as 'const' instead.
  * Warning    CA1704    Correct the spelling of 'Unkown' in member name 'DragDropManager.DumpUnkownDropTypes(DragEventArgs)' or remove it entirely if it represents any sort of Hungarian notation.
  * Warning    CA1500    'node_from', a parameter declared in 'ConnectorControl.SetNodes(NodeControl, NodeControl)', has the same name as an instance field on the type. Change the name of one of these items.
    NOTE: code inspection has led me to change the code in this way that the events registered with the old nodes are UNregistered before the new nodes are assigned and events are REGISTERED with them. The old code was ambiguous, at least for me (human); I'm not entirely sure what the compiler had made of that. HMmm...
    Given these next two MSVS Code Analysis report Warnings, I guess that was a couple of lurking bugs right there:
    + Warning    CA1062    In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_from' before using it.
    + Warning    CA1062    In externally visible method 'ConnectorControl.SetNodes(NodeControl, NodeControl)', validate parameter 'node_to' before using it.
  * updated links/refs in README.md
  * + Warning    CA1715    Prefix interface name 'RecurrentNodeContent' with 'I'.
    + Warning    CA1715    Prefix interface name 'Searchable' with 'I'.
    + Warning    CA1715    Prefix interface name 'Selectable' with 'I'.
  * Merge branch 'memleak-hunting'
    # Conflicts:
    #    Qiqqa/AnnotationsReportBuilding/ReportViewerControl.xaml.cs
  * working on using a more developed build versioning approach. Have MSVS produce a unique version for each build, then (FUTURE WORK) add tooling to ensure all files carry the updated version number(s).
  * tweak Logging class: remove unused methods and group all Warn and Error methods next to one another for an easier overview of the available overloads.
  * feature added: store the source URL (!yay!) of any grabbed/sniffed PDF. Previously the source path of locally imported (via WatchFolder) PDFs was memorized in the Qiqqa database. It adds great value (to me at least) when Qiqqa can also store the source URL for any document -- this is very handy information to have as part of augmented document references!)
    This commit includes a lingering part of the memleak hunt refactor activity listed in the previous commit SHA-1: 177a2be0cff4e92a9ae285c61c2377bac1cbf1c4 as that code exists in the same source files and the activities were developed at the same time.
  * As we've been hunting hard to diagnose memory leaks which made working with Qiqqa short-lived (OutOfMemory in about 15 minutes on a large 20K+ Library), we have applied best practices coding to all IDisposible-derived classes and augmented all Dispose() methods to ensure a fast and easy GC action by unlinking/detaching any referenced objects/instances ASAP. We are still in the process of going through the MSVS2019 Code Analysis Report that we've run at the end of the memleak hunting session, which took place in the last 245 hours.
    Best practices are gleaned from https://docs.microsoft.com/en-us/dotnet/api/system.object.finalize?view=netframework-4.8 and the Dispose+Dispose(true/false) coding pattern described there is applied everywhere where applicable.
    Also note that we employ the ?. **null conditional operator**, which is part of C# 6.0 and described here: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
    It turns out the NANT build doesn't cope with this but I'm loath to revert to antiquity there, so the NANT build process has become a little hacky as MSVS2019 (IDE) can build (and debug) the Qiqqa binaries without a fuss, so we now use that one to build the binaries and the NANT build script for packaging (creating the setup.exe).
  * Warning    CA1812    'BackingUp' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  * Warning    CA2000    In method 'ReportViewerControl.ButtonToPDF_Click(object, RoutedEventArgs)', call System.IDisposable.Dispose on object 'doc' before all references to it are out of scope.
  * + Save To PDF had been disabled in the original Qiqqa source code. No idea why. Re-enabling it so that 'Save To PDF' is not a NIL activity anymore.
    + Warning    CA1811    'ReportViewerControl.ButtonExpandClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
    + Warning    CA1811    'ReportViewerControl.ButtonCollapseClickOptions_Click(object, RoutedEventArgs)' appears to have no upstream public or protected callers.
  * + Warning    CA1804    'RegionOfInterest.IsCloseTo(RegionOfInterest)' declares a variable, 'horizontal_distance', of type 'double', which is never used or is only assigned to. Use this variable or remove it.
    + Warning    CA1802    Field 'RegionOfInterest.PROXIMITY_MARGIN' is declared as 'static readonly' but is initialized with a constant value '0.0333333333333333'. Mark this field as 'const' instead.
  * Warning    CA1812    'LinkedDocsAnnotationReportBuilder' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  * Warning    CA1053    Because type 'LegacyAnnotationConvertor' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.
  * + Warning    CA1812    'InkToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
    + Warning    CA1802    Field 'InkToAnnotationGenerator.INKS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Inks*'. Mark this field as 'const' instead.
  * + Warning    CA1812    'HighlightToAnnotationGenerator' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
    + Warning    CA1802    Field 'HighlightToAnnotationGenerator.HIGHLIGHTS_TAG' is declared as 'static readonly' but is initialized with a constant value '*Highlights*'. Mark this field as 'const' instead.
  * Warning    CA1811    'AsyncAnnotationReportBuilder.OnShowTagOptionsComplete(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' appears to have no upstream public or protected callers.
  * Warning    CA1804    'AsyncAnnotationReportBuilder.BuildReport(Library, List<PDFDocument>, AnnotationReportOptionsWindow.AnnotationReportOptions)' declares a variable, 'underline', of type 'Underline', which is never used or is only assigned to. Use this variable or remove it.
  * Warning    CA1053    Because type 'AsyncAnnotationReportBuilder' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.
  * + Warning    CA1500    'OnShowTagOptionsComplete', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
    + Warning    CA1500    'library', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
    + Warning    CA1500    'pdf_documents', a parameter declared in 'AnnotationReportOptionsWindow.ShowTagOptions(Library, List<PDFDocument>, AnnotationReportOptionsWindow.OnShowTagOptionsCompleteDelegate)', has the same name as an instance field on the type. Change the name of one of these items.
  * Warning    CA1822    The 'this' parameter (or 'Me' in Visual Basic) of 'WordListCredibility.HasSufficientRepeatedWords(WordList)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.
  * Warning    CA1802    Field 'WordListCredibility.REASONABLE_WORD_LIST_LENGTH' is declared as 'static readonly' but is initialized with a constant value '10'. Mark this field as 'const' instead.
  * Warning    CA2204    Correct the spelling of the unrecognized token 'exst' in the literal '"\' does not exst"'.
  * Warning    CA1812    'TextExtractEngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  * Warning    CA1822    The 'this' parameter (or 'Me' in Visual Basic) of 'RegionExtractorTestWindow.ChooseBrush(PDFRegionLocator.Region)' is never used. Mark the member as static (or Shared in Visual Basic) or use 'this'/'Me' in the method body or at least one property accessor, if appropriate.
  * Warning    CA1811    'PDFRegionLocator.GetRegions_FULLPAGE(Bitmap, out List<PDFRegionLocator.Region>, out int)' appears to have no upstream public or protected callers.
  * Warning    CA2000    In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'ocr' before all references to it are out of scope.
  * Warning    CA2000    In method 'OCREngine.DoOCR(string, int)', call System.IDisposable.Dispose on object 'new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200F))' before all references to it are out of scope.
  * Warning    CA1801    Parameter 'no_kill' of 'OCREngine.MainEntry(string[], bool)' is never used. Remove the parameter or use it in the method body.
    Synced no_kill code with the code in TestExtractEngine.cs
  * Warning    CA1812    'OCREngine' is an internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static methods, consider adding a private constructor to prevent the compiler from generating a default constructor.
  * + MSVS Code Analysis: Warning CA1053  Because type 'Backgrounds' contains only 'static' members, mark it as 'static' to prevent the compiler from adding a default public constructor.icons
    + MSVS Code Analysis: Warning CA2211  Consider making 'Icons.QiqqaMedal' non-public or a constant.
  * !@#$%^&* NANT doesn't support Chttps://github.com/GerHobbelt/qiqqa-open-source/issues/6.0 language constructs. Spent a few hours on NANT (obtained from GitHub) but cannot get it to work right now. Not spending more time on that distraction: patched the NANT build process to use the binaries which are (correctly and without hassle) produced by MSVC2019 IDE (Batch Build all or Rebuild All, Release target). See short notes in NANT build file and bash shell driver script too. QUICK HACK. QUICK HACK. QUICK HACK. Anyone with the time on their hands and the inclination to make NANT work with a *recent* MSVC setup: more power to you. Right now, this is a bleeding edge NANT rebuild PLUS patched build/packaging scripts.

2019-08-05
----------

  * revert debug code that was part of commit SHA-1: 89307edfe7d5ba2b6de050de969d2910b147e682 -- some invalid BibTeX was crashing the Lucene indexer (AddDocumentMetadata_BibTex() would b0rk on a NULL Key)
    That problem was fixed in that commit at a higher level (in PDFDocument)
  * Don't let illegal multiple BibTeX entries for a single PDF record slip through unnoticed: one PDF having multiple BibTeX records should be noticed as a WARNING at least: the 'official' (i.e. *first*) BibTeX record is augmented with a Warning mentioning the multiplicity of the BibTeX for that PDF record.
    I had one example of this in my library where some slightly disparate BibeX records were copy-pasted into the BibTeX edit box for an obscure PDF: that had been meant "to be further investigated later" but that never happened and now showed up as a 'silent error' while checking Qiqqa library integrity.
  * some invalid BibTeX was crashing the Lucene indexer (AddDocumentMetadata_BibTex() would b0rk on a NULL Key)
    Sample invalid BibTeX:
    @empty = delete?
  * fix/tweak FolderWatcher background task: make sure we AT LEAST process ONE(1) tiny batch of PDF files when there are any to process.
  * trying to tackle the slow memory leak that's happening while Qiqqa is running  :-((   This is going on for a while now; can't seem to spot the culprit though.  :-((
  * added misc files to the solution/projects: license, readme and copyright files.
  * mention the new build-installer.sh bash shell script as equivalent of the old go.bat in the README.
  * little tug & tweak of the build bash shell script.
  * fix msbuild error (oddly enough this was okay in MSVC2019 and compiled fine in the ID, but the NANT task via ./build-installer.sh fails.  :-S  )
  * DBExplorer severely enhanced:
    - now supports wildcards in query parameters (% and _, but also * and ?, which are the MSDOS wildcards which translate directly to the SQL wildcards)
    - now supports GETting multiple records.
    - when GETting multiple records, DBExplorer not only prints the BibTeX for each record, but also the identifying fingerprint, verification MD5 and most importantly: the *PARSED* BibTeX (iff available) and BibTeX parse error diagnostics report.
    - when GETting multiple records, the DBExplorer output is also dumped to the file Qiqqa.DBexplorer.QueryDump.txt in the Qiqqa Library base directory. A previous DBExplorer query report dump will be REPLACED.
    - an extra input field has been added which allows the user to specify a maximum number of records to fetch: this speeds up queries and their reporting when working on large libraries with query criterai which would produce 1000nds of records if left unchecked.
    This allows to use the DBExplorer as a rough diagnostics tool to check the library internals, including a way to find erroneous/offending BibTeX entries which may cause havoc in Qiqqa elsewhere.

2019-08-04
----------

  * See also commit SHA-1: b38123a4ea67b4f3581826aeeac44a4ee0e9e39e: we now have Qiqqa open a VERY FAST and LEAN web page when we 'Open Browser'.
  * fix compiler warning due to unused variable.
  * fix/tweak: do NOT report 'Adds 0 of 0 document(s)' but clear the status part instead: now that we make Qiqqa work in small batches, this sort of thing MAY happen. (TODO: review WHY the Length of the todo array is actually ZERO, but low priority as things work and don't b0rk)
  * fixing https://github.com/jimmejardine/qiqqa-open-source/issues/8: not only storing Left/Top coordinate, but also Width+Height of the Qiqqa.exe window
  * fix crash in chat code when Qiqqa is shutting down (+ code review to uncover more spots where this might be happening)
    20190804.204351 INFO  [Main] Stopping MaintainableManager
    Exception thrown: 'System.NullReferenceException' in Qiqqa.exe
    20190804.204351 WARN  [9] There was a problem communicating with chat.
    System.NullReferenceException: Object reference not set to an instance of an object.
    at Qiqqa.Chat.ChatControl.ProcessDisplayResponse(MemoryStream ms) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 221
    at Qiqqa.Chat.ChatControl.PerformRequest(String url) in W:\lib\tooling\qiqqa\Qiqqa\Chat\ChatControl.xaml.cs:line 127
    20190804.204351 WARN  [9] Chat: detected Qiqqa shutting down.
  * I'm using bash rather than cmd as it comes with Git For Windows: provide a setup build script which you can invoke from the root from the project so you can work with git easily while also running this build command (and a few other things)
  * Since ExpeditionManager is the biggest OutOfMemory troublemaker (when loading a saved session :-( ), we're augmenting the logging a tad to ease diagnosis. (https://github.com/jimmejardine/qiqqa-open-source/issues/19)
  * debugging: uncollapsing rollups in dialog windows as part of a longer debugging activity. MUST REVERT!
  * augment BibTeX documentation: add URLs and note an old one as inactive (that webpage has disappeared from the Intarwebz. RIP.)
  * 'Open New Browser' was looking pretty weird due to a website/page being  loaded which was unresponsive; now we're pointing to a more readily available webpage instead. (Though in my opinion 'Open Browser' should load a VERY MINIMAL webpage, which has absolutely *minimal* content...) Referenced URL has already been set up as part of commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505
  * Mention the new CSL (Citation Styles) source websites in the credits. The links referenced in this text have already been set up as part of a previous commit in WebsiteAccess class. (commit SHA-1: 820d83356c2e119466fe5f34687000ea358f2505)
  * typo fix in comments
  * code stability: Do not crash/fail when the historical progress file is damaged
  * refactor: collect (almost!) all URLs and keep them in WebsiteAccess so we have a single place where we need to go to update URLs. (In actual practice, there remain a FEW places where URLs stay; the number of files carrying URLs is significantly reduced anyway...)
  * moving Sample.bib to be with the other TEST input files

2019-08-03
----------

  * The easy bit of https://github.com/jimmejardine/qiqqa-open-source/issues/3: synced the Qiqqa/InCite/styles/ directory with the bleeding edge of the CSL repo at https://github.com/citation-style-language/styles (Note the 'bleeding edge' in there: I didn't use https://github.com/citation-style-language/styles-distribution !). DO NOTE that Qiqqa had several CSL style definitions which don't exist in this repository: these have been kept as-is.
  * cut out all test code chunks using #region Test + #if TEST ... #endif around those chunks: this way, the test code will still exist ("just in case...") while it won't burden the compiler and never be included in a Qiqqa binary unless you *specifically* instruct the compiler to do so by -define TEST in the compiler options.
  * Flag all [Obsolete] entries as triggering a compiler error when still in use. Some class properties have been flagged in the comments as required for backwards compatibility of the serialization (reading & writing) of the configuration and BrainStorm files, so we added a Obsolete(...) report message accordingly.
    This change SHOULD have no effect on the build/code flow but 'cleans up' by making the use of these obsolete bits an ERROR instead of a WARNING.
    See also https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/common-attributes#Obsolete
  * pick up the language from the dialog where the same is said as in the text file (though the rest of the wording is slightly different: the InCiteControl dialog content and this README do NOT have identical content!)
  * whitespace police raid

2019-08-02
----------

  * correct code and explain why for https://github.com/jimmejardine/qiqqa-open-source/issues/20
  * corrected Folder Watch loop + checks for https://github.com/jimmejardine/qiqqa-open-source/issues/20: the intent here is very similar to the code done previously for https://github.com/jimmejardine/qiqqa-open-source/issues/17; we just want to add a tiny batch of PDF files from the Watch folder, irrespective of the amount of files waiting there to be added.
  * HACKY trial to catch and cope with OutOfMemory errors due to the LDAStuff etc.: https://github.com/jimmejardine/qiqqa-open-source/issues/19
  * more work regarding https://github.com/jimmejardine/qiqqa-open-source/issues/10 and https://github.com/jimmejardine/qiqqa-open-source/issues/17: when you choose to either import a large number of PDF files at once via the Watch Folder feature *or* have just reset the Watch Directory before exiting Qiqqa, you'll otherwise end up with a long running process where many/all files in the Watched Directories are inspected and possibly imported: this is undesirable when the user has decided Qiqqa should terminate (by clicking close-window or Alt-F4 keyboard shortcut).
  * attempt to cope with https://github.com/jimmejardine/qiqqa-open-source/issues/19 a little better than a chain of internal failures. :-(
  * code cleanup activity (which happened while going through the code for thread safely locks inspection)
  * https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 2, looking for thread safety locks being applied correctly and completely. Also contains a few lines from work done before related to https://github.com/jimmejardine/qiqqa-open-source/issues/10 et al.

  * https://github.com/jimmejardine/qiqqa-open-source/issues/18 work :: code review part 1, looking for thread safety locks being applied correctly and completely: for example, a few places did not follow best practices by using the dissuaded `lock(this){...}` idiom (https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement)
  * Fix https://github.com/jimmejardine/qiqqa-open-source/issues/17 by processing PDFs in any Qiqqa library in *small* batches so that Qiqqa is not unreponsive for a loooooooooooooong time when it is re-indexing/upgrading/whatever a *large* library, e.g. 20K+ PDF files. The key here is to make the '**infrequent background task**' produce *some* result quickly (like a working, yet incomplete, Lucene search index DB!) and then *updating*/*augmenting* that result as time goes by. This way, we can recover a search index for larger Qiqqa libraries!

  * fixing MSVC2019-reported 'unreachable code' compiler reports by minimal refactoring

  * commenting out a property which was reported as 'unused' and wasn't flagged as `[Obsolete]` for serialization either... (compare to Qiqqa/Brainstorm/Nodes/NodeControlSceneData.cs where this attribute is used)

  * dialing up the debug/info logging to help me find the most annoying bugs, first of them: https://github.com/jimmejardine/qiqqa-open-source/issues/10, then https://github.com/jimmejardine/qiqqa-open-source/issues/13

  * Do NOT nuke the previous run's `Qiqqa.log` file in `C:\Users\<YourName>\AppData\Local\Quantisle\Qiqqa\Logs\`: **quick hack** to add a timestamp to the qiqqa log file so we'll be quickly able to inspect logs from multiple sessions. **Warning**: this MUST NOT be present in any future production version or you'll kill users by log file collection buildup on the install drive!

  * simple stuff: updating copyright notices from 2016 to 2019 and bumping the version from 80 to 81.  **Note**: The changelog in ClientVersion.xml runs ahead of what you will observe in these commits: I did the work first, then did these commits to lock it in as only now I am confident that my attempt at https://github.com/jimmejardine/qiqqa-open-source/issues/14 actually delivers a working exe that's equal or better than last commercial release v79 (for me at least; caveat emptor!)

  * update existing Syncfusion files from v14 to v17, which helps resolve https://github.com/jimmejardine/qiqqa-open-source/issues/11
    
    **Warning**: I got those files by copying a Syncfusion install directory into qiqqa::/libs/ and overwriting existing files. v17 has a few more files, but those seem not to be required/used by Qiqqa, as **only overwriting what was already there** in the **Qiqqa** install directory seems to deliver a working Qiqqa tool. :phew:

  * - point Microsoft Visual Studio project files to the (renamed) README.md files following https://github.com/jimmejardine/qiqqa-open-source/pull/6
    - remove Syncfusion LICX file as we migrate from SyncFusion 14 to Syncfusion 17; see also bug report https://github.com/jimmejardine/qiqqa-open-source/issues/11

  * updating 7zip to latest release

 *  Merge remote-tracking branch 'remotes/jimmejardine-original/master'

 *  Merge branch 'master' of https://github.com/jimmejardine/qiqqa-open-source

v80
===

  * added first build result as some people wish to download it

 *  Merge pull request #6 from GerHobbelt/readme-cleanup-for-github
    
    cleaned the README files a bit and made them ready for GitHub

  * Update README.md

  * Update README.md


 *  Merge branch 'readme-cleanup-for-github'

  * cleaned the README files a bit and made them ready for GitHub (MarkDown formatting)

  * Update README.md

  * README: tables fixed for GFM

  * Update README.md

  * tweaking the README tables for GitHub to render them correctly.

  * fix README rendering on GitHub

  * cleaned the README files a bit and made them ready for GitHub ( MarkDown formatting )

  * Rename the `~readme.txt` files describing directory content to `README.md` so that GitHub picks them up and displays them in the web view of the repo.

  * Update README.md

  * open source version is now able to 'see' the legacy web libraries

  * initial contribution

  * Update README.md

  * Create README.md

  * Initial commit





Version 81:
- Qiqqa now copes better with damaged PDFs which are part of the librarie(s): 
  + search index does not "disappear" any more
  + Qiqqa does not continue running in the background for eternity due to locked-up PDF re-indexing task

Version 80 (FOSS):
=================

- Qiqqa goes Open Source!!!
- Enabled ALL Premium and Premium+ features for everyone.
- Removed all Web Library capabilities (create/sync/manage)
- Added the ability to copy the entire contents of a former Web Library into a library - as a migration path from Qiqqa v79 to v80

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

