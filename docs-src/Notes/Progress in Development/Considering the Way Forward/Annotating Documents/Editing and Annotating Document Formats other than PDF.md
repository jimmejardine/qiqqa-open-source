# Editing and Annotating Document Formats other than PDF

For PDF we have FoxIt, SumatraPDF, etc. See also [[../General Design Choices/Using an external PDF viewer, annotator & editor]].

However, we've noted elsewhere that we want/need to support a few more base formats at least:

- MHTML, CHM or another HTML+CSS+JS bundling which freezes/*archives* web pages for off-line and later peruse.

  Also **consider using the HTMLZ format for all of these**: it's compressed and zipped, thus bundling all bits and pieces in a tight format that consumes little disk space and is fast to access and create. 
  
  > HTMLZ is one or more HTML files + assets zipped in a ZIP archive, plus an added [OPF metadata](http://idpf.org/epub/20/spec/OPF_2.0_latest.htm) file, which is, for example, used by [Calibre](https://calibre-ebook.com/).

- Images (which serve as single-page documents)

  However, we *could* easily wrap any images in a fresh PDF of our own making and then allow the user to edit & annotate *that*: that way we have the annotation and all the other work persisted with minimal fuss, while we only have account for the image file to be *the original document*, i.e. one of the supported *near duplicates* in your Qiqqa library.
  

 
 Which leaves the HTML-based pages (and all document formats which transform to this format, e.g. MarkDown documents): [SumatraPDF](https://www.sumatrapdfreader.org/free-pdf-reader) & [FoxIt](https://www.foxit.com/pdf-reader/) are particularly geared towards *editing* and *annotating* such documents. 
 
 We *could* use the same cop-out as for our image file based documents *by first transforming it into a PDF (using [wkhtmltopdf](https://wkhtmltopdf.org/index.html)) before enabling user editing and annotating*, but I feel we should also offer a *native* edit facility here, even when that would involve yet another external application.
 
 Turns out a quick search for Open Source WYSIWYG HTML editors are all in-page and JS-based:
 
 - [Medium](https://github.com/yabwe/medium-editor) + https://yabwe.github.io/medium-editor/
 - [Jodit](https://github.com/xdan/jodit) + https://xdsoft.net/jodit/play.html
 - [Summernote](https://github.com/summernote/summernote/) + https://summernote.org/
 - https://jaredreich.com/pell/ + https://github.com/jaredreich/pell -- tiny size
 - [Froala](https://github.com/froala/wysiwyg-editor) + https://froala.com/wysiwyg-editor/examples
 - [TinyMCE](https://github.com/tinymce/tinymce)
 - [CKEditor](https://github.com/ckeditor) + https://ckeditor.com/
  
 - Special Mention:
	 - https://github.com/brackets-cont/brackets -- though this is more a source editor, rather than WYSIWYG when it comes to HTML
	 - https://github.com/dok/awesome-text-editing
	 