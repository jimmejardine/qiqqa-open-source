# OpenMP & tesseract :: *NSFQ* (Not Suitable For Qiqqa)

It's pretty useless to pursue OpenMP support for our purposes: OpenMP, simply put, adds multi-threading support in your code "under the hood". As can also be gleaned from tesseract issue [#263](https://github.com/tesseract-ocr/tesseract/issues/263) comments, OpenMP is only of *potential* benefit when your use case matches **all** of these criteria:
- your CPU cores have no other work pending, thus distributing a single task across multiple threads is not delaying other important jobs.
- you only run a **single OCR task** at a time (and need that one done as quickly as possible). 

  Corollary to this: when there are multiple OCR tasks incoming, your use case is very specific about needing them done *in order*, despite that multiple images *can potentially be OCR-ed in parallel* (by running multiple tesseract instances in multiple threads, one thread each).
- you are not bothered at all about OpenMP's task chunk distribution across threads and recombination once complete will consume noticeable, often even *severe* additional CPU sources, thus increasing both your CPU cycle count costs per OCR round *and* its carbon footprint (thanks to a lot of extra heat being produced by your CPU). 

## Why this does not apply to Qiqqa
Qiqqa will **always** employ tesseract in batch mode, even when you might not realize it does. There are several scenarios, all of which end up as classic queued batches in the end:
- you download or import a new PDF, which, upon inspection, is flagged for OCR-ing as there's no text layer (or rather: the pages don't carry any text).

  This becomes a batch because Qiqqa will immediately queue all pages in the PDF for OCR processing. For any document that spans more than 1 page, this effectively is *a batch*. Almost all documents span more than a single page.

- you copy several PDF documents into your Qiqqa Watch folder, several of which are flagged for OCR due to their text layers missing. This is an obvious *batch* action anyway, but you *may* not realize that Qiqqa queues *all* those documents' pages for OCR processing immediately. From the perspective of tesseract/OCR, that's a pretty heavy *batch load*! Adding the OpenMP overhead would incur a significant cost with zero gain as all CPU cores will max out anyway (or max out as much as they can, depending on circumstances and configuration. The bottom line: you are running a large batch and reducing overhead costs there is the way to gain time!)

- you open a PDF for viewing, which has, for some reason (previous runtime errors, requested re-OCR by specifying a different OCR language in the UI, etc.) no text layer info for the page(s) in view.

  Here Qiqqa will *at least* file the currently viewed page for OCR, but "forced language for OCR" and other such user actions, which might seem tiny to you, result in a full-on re-application of OCR activity as you have now changed (or forcibly re-issued) the parameters relevant to OCR configuration and execution. This implies that Qiqqa will attempt to OCR the entire document ASAP. That is, thanks to multiple pages in almost all documents out there, a *batch action* under the hood.

- when Qiqqa has loaded its libraries, it does inspect the state of all its documents stored there-in and queues all tasks which have not been done yet for processing. These tasks *include* OCR, but there are many more very CPU intensive tasks to be performed for a document, e.g. keyword gathering (which currently is LDA based, and that sort of stuff likes CPU cycles a lot too!), FTS indexing, i.e. feeding the document text and metadata to the Full Text Search subsystem so you, the user, will be able shortly to query the library and thus these documents, while you are looking for stuff. Building and keeping such a search index up to date (using Lucene, SOLR or other means) is no sine cure and takes a lot of CPU cycles: that's another one then which can place a high demand on cores and threads available in your system!

  Ergo: there's several other sources of CPU demand in the Qiqqa system, apart from its OCR subsystem. Using OpenMP to give OCR a possible leg up in this tug of war will slow the other processes down by the same amount *plus OpenMP overhead costs*, resulting in a poorer end-user experience: you do not benefit from any OCR output until it has been processed by the FTS index (Lucene et al) *at least*! 
  Thus it is *counter-productive* to use OpenMP: it only makes the queue/task management more complicated and loads it with a lot of extra overhead, resulting in increased costs with zero gain all around.




## Conclusion

  OpenMP can be quite beneficial for those other applications, which perform a single duty and can assume there's no competition when it comes to assigning CPU cycles. This can apply to games and does certainly apply to a lot of research applications such as simulations, which are, by decree, often allowed to hog an entire machine all by their lonesome. *Then*, when there's no smarter solution coded into those applications yet, OpenMP can produce a nett benefit -- while *very probably* increasing the carbon footprint per action, thanks to the overhead inherent in its use, but then again you might not be bothered about that little detail, so OpenMP is *good for you*.

  With Qiqqa, after I have looked into the matter in more detail, the conclusion is simple and potentially counter-intuitive: OpenMP is not a sane part of any solution or subsystem we choose to \[continue to\] use, as almost all CPU-intensive sub-tasks appear in batches, either in 'mono-culture' or 'poly-culture' form, and user-visible results depend on multiple sub-tasks in those batches, rather than a *single* sub-task, to complete as soon as possible.

  > This means that prioritized task queue management can be expected to be far more relevant and important for an optimal user experience. 
  > Indeed, we have been observing this task queue load problem already for several years (with commercial Qiqqa and later, with the open source versions as well) with our large libraries, where task queue pressures mount to very high numbers, particularly when importing / recovering / adding new libraries, which naturally do not have (most of) the OCR, text extraction & processing, FTS indexing and keyword / category detection work done yet -- or having had one or more of those components' overall results flagged as corrupted or invalid, thanks to earlier application crashes and other failures.
  
  

## Is this a tesseract specific issue?

No. 

This question whether or not using OpenMP is a smart thing to do, applies generally to all systems where you have a choice between single-task-as-fast-as-possible vs. batched tasks which must perform as best they can *together*. OpenMP is not a solution for any scenario where you already have a loaded CPU with all cores busy with the same or other work.

## References

- [good accuracy but too slow, how to improve Tesseract speed · Issue #263 · tesseract-ocr/tesseract (github.com)](https://github.com/tesseract-ocr/tesseract/issues/263)
- [RFC: Best Practices re OPENMP - for training, evaluation and recognition · Issue #3744 · tesseract-ocr/tesseract (github.com)](https://github.com/tesseract-ocr/tesseract/issues/3744)
- 
