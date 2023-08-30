# Qiqqa Internals :: Processing PDF documents' text and the impact on UI+UX

> Ripped off from my reply to https://github.com/jimmejardine/qiqqa-open-source/issues/165

Had a look at what happened exactly. It has been enlightening as I discovered I was working with a couple of internal assumptions that are clearly based on developer rather than user experience influencing my user experience.

# What is going on?

When Qiqqa imports the PDF into the library, a few things happen under the hood:
- a queued background task will attempt to *infer* metadata, such as author, from the PDF metadata.
- other *inferred* metadata, e.g. *title* and *abstract*, is obtained by going through the *OCR text* of the PDF.

Both of these 'trigger' a request to fetch the document text, i.e. the *OCR text*.

# What *is* "OCR text" (in this context)?

Qiqqa "OCR text" is the *word text* plus location rectangle coordinates collection extracted from the PDF by the *OCR background process*. Think of it as each word **plus** its precise position on the page, stored in a Qiqqa proprietary ocr file format.

## How does Qiqqa obtain this *OCR text*?

That's where some confusion can occur: Qiqqa has two methods to extract text from a PDF. **It does not matter which of these methods has produced that text content**: either way it's stored in the "Qiqqa OCR text cache".

### Text *Extraction*

The **primary** method is **direct text extraction**:  using the `mupdf` tool, Qiqqa can get the text (plus coordinates) for any PDF which has a text layer embedded.

**Your sample PDF is entirely processed by this first method, all 69 pages of it.**

### Text *Recognition*

When the primary method fails to deliver a text for a given page, that page is then *re-queued* to have it OCR-ed using a Tesseract-based sub-process. This is the **secondary** method for obtaining the text of a document (page).


# How does this impact UX?

As long as Qiqqa does *not* have the PDF text available in its cache, it will *disable* any user activity that needs this data:

- **text selection** for copy & paste, export-to-Word and similar purposes
- **highlight text** annotating text by marking it using a selection process.
- ...

The background tasks mentioned before (inferring metadata) are *postponed* until the *OCR text* is available.

There a few more background tasks which have not been mentioned yet, including the one *updating the text search index*: that task of course requires the *OCR text* as well.

From a user perspective, one can say that text searching in Qiqqa will only pick up on the new documents after *both* the  OCR process (methods 1 or 2, whatever it took to get some text out of those new PDFs) *and* the background Lucene text search indexing process have processed the new PDF documents.


## Performance 

Qiqqa *may* seem to be 'slow' in picking up new imported PDFs as the above processes all happen in the background and are currently set up to load the CPU only *moderately*: this was specifically done to make Qiqqa cope much better with large & *huge* libraries filled with technical datasheets and other PDF documents, which caused all sorts of trouble, including UI lockups and application crashes. (In commercial Qiqqa this included fatal crashes, where the application was unwilling to start up again and/or fatal loss of the text search index.)

Yes, we still have a way to go before Qiqqa will be fast and responsive as the current drive was first to make Qiqqa stable in such a 'large library' environment. To make Qiqqa behave well and *responsive* in various environments, it will take quite some more effort. 


# Now back on topic

Now we have a description of what goes on and an observed run, I can address the issue at hand:

- as described above, Qiqqa will take some time before it runs and completes the new document(s) text extraction and then allow text marking and selecting  actions. Up till that moment those user activities are disallowed.

  Hence these activities should be possible after some patience has been exercised. (Unless the PDF is one of the crappy sort, causing the "OCR" methods trouble, which is yet another chapter. 😉 )


My initial confusion was due to me thinking in Qiqqa *coding* terms: both text *extraction* and *recognition* are filed under the single title of "OCR-ing the text", because that's how Qiqqa approaches this under the hood.


---

> To complicate matters further, there's also a couple of options to *freeze* the OCR/text extraction and/or *all background processes*. Suffice to say those options (in the Qiqqa Tools menu and Qiqqa Configuration window) are **not active** unless the user has activated them (e.g. a developer or power user testing Qiqqa or importing a large set of documents). The use of these options is out of scope.

