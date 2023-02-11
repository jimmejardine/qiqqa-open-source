# Qiqqa Internals :: Configuration Overrides for Developers and Testers

## Overriding the Qiqqa library *Base Path*

The regular 'base path', i.e. the base directory where all Qiqqa libraries are stored locally, is stored in the Windows Registry.

You can override this 'base path' by specifying another base path on the commandline.

> ### Extra since v83
>
> Since v83 you can also click the 'Change this path' button in the startup dialog and point Qiqqa at another base directory,
> which is then persisted by Qiqqa, i.e. Qiqqa will keep using the new directory in subsequent runs -- until you change it again.




### Why would you want to override this registry setting?

For example:

- when you are testing Qiqqa and want to use a different (set of) Qiqqa Libraries for that. Overriding the 'base path' ensures your valuable Qiqqa libraries for regular use cannot be touched by the Qiqqa run-time under test.

  > Assuming, of course, that the regular base path directory tree and the one you specified via the commandline do not overlap.

- when you wish to work on one or more Qiqqa Libraries which should not be integrated into your regular set of libraries, e.g. when you wish to help someone else by having a look into their library/libraries you got copied locally.




### Commandline format

```sh
qiqqa.exe <basepath>
```

e.g.

```sh
qiqqa.exe D:\Qiqqa.Test.Libs\base\
```




## Overriding Qiqqa behaviour

You can override several Qiqqa behaviours by adding a [JSON5](https://json5.org/) configuration file in the Qiqqa 'base path', i.e. the base directory where all Qiqqa libraries are stored locally, named `Qiqqa.Developer.Settings.json5`. Qiqqa will load this file at application startup.




### Configuring `Qiqqa.Developer.Settings.json5`

Here's an example which lists all supported settings:

```json5
// This file may contain comments.
//
// Lines can be commented out at will.
{
	LoadKnownWebLibraries: true,
	AddLegacyWebLibrariesThatCanBeFoundOnDisk: true,
	SaveKnownWebLibraries: true,
	DoInterestingAnalysis_GoogleScholar: false,

	FolderWatcher: true,
	TextExtraction: true,           // when false: this kills the mupdf based text extraction and OCR tasks
	SuggestingMetadata: true,       // when false: this kills the metadata (Title, Author, etc.) suggesting from extracted text
	BuildSearchIndex: true,         // when false: this kills the Lucene-based search index build/update process
	RenderPDFPagesForSidePanels: true,         // this kills the rendering of PDF pages to thumbnails in preview sidepanels
	RenderPDFPagesForReading: true,            // this kills the rendering of PDF pages to main panel PDF view/read/edit tabs (NOT thumbnails!)
	RenderPDFPagesForOCR: true,                // this kills the rendering of PDF pages for OCR-ing a document's pages via QiqqaOCR background application
}
```




#### Defaults

Note that all settings in this file are assumed to be `true` by default, i.e. anything you don't mention explicitly in there is assumed to be `true`.






####    `LoadKnownWebLibraries`

Set to `false` to **disable** Qiqqa's default behaviour where it scans the 'base path' to *discover* all available Qiqqa Libraries.

All libraries which have not been included in your **load list** (as saved by Qiqqa in the previous run in the file `Guest/Qiqqa.known_web_libraries`) will be ignored.

Set to `true` to **enable** Qiqqa's default behaviour.






####    `AddLegacyWebLibrariesThatCanBeFoundOnDisk`

Normally, Qiqqa will scan the base directory for any subdirectories (one level deep only!) containing a Qiqqa library, i.e. a `Qiqqa.library` database -- and hopefully more stuff, like *documents*.

When this option is set to `false`, the scanning behaviour is **skipped**, thus producing a very *bare* library list in your Qiqqa Home page: you may expect only the `Guest` library to show up, or, when `LoadKnownWebLibraries` is `true`, the list of libraries remembered in that internal configuration file, i.e. the library list as persisted by the previous Qiqqa run (which had `SaveKnownWebLibraries` set to `true`).

Handy when your libraries are giving you headaches and you want to run Qiqqa on a minimal/reduced set.

Auto-discovery is back as soon as you set this flag to `true` again and restart Qiqqa -- after all, auto-discovery of libraries only happens at the *start*.








####    `SaveKnownWebLibraries`

Set to `false` to **disable** Qiqqa's default behaviour where it will save your **load list** (the list of Qiqqa Libraries currently discovered and loaded into Qiqqa) to disk in the file `Guest/Qiqqa.known_web_libraries`.





####    `DoInterestingAnalysis_GoogleScholar`

Set to `false` to **disable** Qiqqa's default behaviour where it will perform a background *scrape* in Google Scholar for every PDF document you open / have opened in Qiqqa.


::: tip

Since Google is pretty picky and pedantic about "proper use of Scholar" (from their perspective), hitting that search website more often than strictly necessary should be regarded with some concern: with those background scrapes (which are used to fill the "Scholar" left side panel with some suggestions while the PDF document is open in the Qiqqa Viewer) you MAY expect Google to throw a tantrum and restrict your Scholar access using convoluted Captchas and other means when you really **want** to use Google Scholar in the Qiqqa Sniffer or elsewhere in the application.

Hence the smart move here is to kill those background scrapes as they don't add a lot of value (unless you really like those left side panel Scholar suggestions, of course!)

:::







####    `FolderWatcher`

Set to `false` to **disable** Qiqqa's default behaviour where it will perform a background *scrape* in Google Scholar for every PDF document you open / have opened in Qiqqa.







####    `TextExtraction`

when false: this kills the mupdf based text extraction and OCR tasks





####    `SuggestingMetadata`

when false: this kills the metadata (Title, Author, etc.) suggesting from extracted text





####    `BuildSearchIndex`

when false: this kills the Lucene-based search index build/update process
