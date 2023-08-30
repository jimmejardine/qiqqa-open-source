
# Working with PDFs as a developer :: Odd, odder, oddest, ...ehm... *PDF!*

Just a collection of notes of what I've run into while working on Qiqqa and related tools to process PDFs.

Most[^not100pct] of this stuff is reproducible using the PDFs in the Evil Collection of PDFs. 

[^not100pct]: You don't get **all** of the *eavil basturds* as there's at least lacking a few remarkable PDFs which were over 500MBytes(!) *each* when I received them -- and we're not talking *press-ready pre-flight PDFs* here, as I would *expect* sizes like that for *that* kind of PDF! *No sir-ee!* These fine specimens came straight off the IntarWebz looking like someone had already made the, ah, *effort* to have them "reduced" for web-only screen display viewing (e.g. highly compressed lossy page images, etc.).

Alas, you can't have them all, they say...




## Repair & Linearize for Web?... You Better *NOT*!

`qpdf -qdr` does a fine job, but sometimes that job is just **way too fine**: as I was considering *iff* `qpdf` and/or `mutool repair` and/or other tools could be used to produce PDFs (from source PDFs) which would be more palatable and easier to process by Qiqqa et al, I discovered that, on a few occasions, `qpdf -qdr` was able to produce PDFs of over 1 GIGAbyte(!) from humble beginnings such as a ~10MByte source PDF. Thus 'linearize for web' and similar 'restructuring cleanup' actions SHOULD NOT be performed to help displaying / processing these buggers in Qiqqa et al.

**Note**: this, of course, does not mean that `qpdf` is an unuseful tool: on the contrary! `qpdf decrypt` and a few other tools (`mutool repair`) help greatly in getting some obnoxious PDFs displayed on screen, just like Acrobat would show them (see also https://github.com/jimmejardine/qiqqa-open-source/issues/9 and https://github.com/jimmejardine/qiqqa-open-source/issues/136).





## There's 3 ways Qiqqa's is attempting to calculate a PDF's total page count: Jimme must have gotten some real bad trouble too!

Check the source code.

*Maybe* we can improve this by getting back to one or maybe two different ways there when we introduce the new MuPDF-based rendering engine (to replace the Sorax one). But until that time: note that the Sorax version (which is the first attempt at getting a page count) rarely delivers; the Jimme hack (attempt number 3 in there) is often reached as the other two modes don't deliver a viable page count. *Oops?!*




-—

## \[Update 2021/03/29]

This whole "page count" and "rendering (semi-)crypted PDFs" hassle is now tackled by the MuPDF toolset (`mutool metadump` and `mutool draw`, mostly), yet text extraction + OCR through that toolchain (via `mutool draw`) is still under development, as the old `mudraw -tt` output from way back when isn't available with the new tools and we reconsider what is best to use from now on as this should (*possibly*) also help fix several mark&select issues when working with PDFs in Qiqqa: the (*hidden*) text overlay is often flawed and the new text extract process isn't powerful enough to attack these issues as it is, either. 
To be continued…

Anyway, the `qpdf` tool won't bee needed any longer, as current `mutool draw` et al can deliver the same (or better) results, while `mutool metadump` delivers all the available metadata and then some!





