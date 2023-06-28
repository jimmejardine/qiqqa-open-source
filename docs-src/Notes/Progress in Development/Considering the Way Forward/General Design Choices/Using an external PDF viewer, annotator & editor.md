# Using an external PDF viewer / annotator / editor


## Why?

So we don't have to do all the development work involved with creating and supporting an advanced UI/UX like that, i.e. a UI/UX which supports:

- viewing arbitrary PDFs (and a few other file formats, perhaps)
- **annotating** PDF documents -- while those annotations *should* be consumed by Qiqqa and added to the FTS database, etc. to support searching and reporting annotations as yet another type of PDF *metadata*.
- **editing** PDF documents -- e.g. to correct text mistakes, or otherwise produce a higher-quality *revision*, (one of the types of *near-duplicate* we intend to support in Qiqqa)


## Likely candidates


- Open Source:
	- [SumatraPDF](https://www.sumatrapdfreader.org/free-pdf-reader)
		- https://www.sumatrapdfreader.org/docs/Editing-annotations
	- [GSview / GhostScript](https://en.wikipedia.org/wiki/GSview "GSview")
- Free / non-commercial:
	- [Foxit Reader](https://en.wikipedia.org/wiki/Foxit_Reader "Foxit Reader")
	- [Nitro PDF Reader](https://en.wikipedia.org/wiki/Nitro_PDF_Reader "Nitro PDF Reader")
	- Adobe Acrobat Reader
	- [Evince](https://en.wikipedia.org/wiki/Evince "Evince")
- Commercial:
	- [PDFannotator](https://www.pdfannotator.com/en/)
	- [FoxIt](https://www.foxit.com/)
	- [Adobe Acrobat](https://en.wikipedia.org/wiki/Adobe_Acrobat "Adobe Acrobat")


(See also: https://en.wikipedia.org/wiki/List_of_PDF_software)



## Consequences for Qiqqa

### Only a minimal internal viewer / reader needed?

We intend to maintain a minimal internal reader as that would keep interaction for the *primary process of browsing/reading documents from your library*  relatively swift and hassle-free: **no application switching while you swiftly browse through your library**.

At the same time we feel we would be spending quite a large effort to, *at best*, replicate the very usable PDF editing and annotating actions, as supported by other software already, e.g. FoxIt and SumatraPDF.

Hence we would be better advised in spending our efforts on interfacing with those already existing *mature applications* and make sure the user-added value is kept intact and properly processed in Qiqqa. (I.e. import and re-export user-added annotations, edits and other kinds of *PDF document revision*.)


### Consequences

- We may obsolete the annotation edit functionality in Qiqqa.
- We would be obliged to *render* the annotations created in other applications in the internal reader/viewer *anyway*.
- Ditto for making the *text layer* visible in the internal reader -- currently that is not yet available as the text extract's positioning info (the word bboxes!) are only used to aid marking the text (for annotations and text copy& paste to the clipboard). However, there's not an option available yet to make the text layer *explicitly visible itself* (for purposes of QA, etc.).


How much would we really *gain* if we need to render all those annotations, etc. *anyway*, in our internal reader? Could we get rid of that functionality as well?

--> `mupdf` currently supports rendering all annotations (AFAICT) so the rendering is relatively cheap in maintenance terms. Offering a full-fledged *editing / annotating* UI would, however, remain an additional non-trivial task. Not having to do that would be the *gain* we seek with supporting external editors.





