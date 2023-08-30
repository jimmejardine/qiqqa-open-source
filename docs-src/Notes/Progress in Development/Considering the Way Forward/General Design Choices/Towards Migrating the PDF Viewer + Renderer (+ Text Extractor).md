
# Towards migrating the PDF viewer / renderer ( / text extractor)

## The old situation (Qiqqa v82 and below)

Qiqqa used the commercial closed source Sorax lib for rendering PDF pages to screen and frankly that's a dud as quite a few legal PDFs are not rendered by that lib (blank pages result).

Qiqqa also used an old patched[^patched] version of `pdfdraw` (v1.1.4) in the QiqqaOCR tool for text extraction (`pdfdraw -tt`).

[^patched]: *patched* because there's Quantisle being mentioned in the executable and there's `libs/3rdparty/mupdf.changes.txt`. *Hints!* 😁 

---


## The goal / what'ld be ideal

- Use a single PDF renderer / text extractor, so that *when* a PDF is accepted, both will "interpret" the PDF the same way: the renderer producing page images while the text extractor part will produce hOCR or similar text+coordinates data from those pages: when both outputs are produced by a single tool-chain then my **assumption / expectation** is that the hOCR words SHOULD be at the same spot as the image rendered pixels for them, even when we're processing a somewhat *odd* PDF.
- No closed source libraries anywhere: if bugs aren't fixed quickly by a support team, they should at least be allowed to be analyzed in depth and for that you need **source code**. Too many very bad experiences with closed source for this fellow. 🤕
- available in 32-bit *and* 64-bit with a C# interface so we can move Qiqqa into the 64-bit realm once we've got rid of the 32-bit requirement thanks to antiquated XULrunner -- this should make life easier on modern boxes and when perusing (very) large libraries.
- very near or at Acrobat performance and PDF compatibility, i.e. SHOULD NOT b0rk on many PDFs, even evil ones.[^evilPDF]

[^evilPDF]: In the end, the PDF renderer WILL be Internet facing -- even while only *indirectly*, but as PDFs are downloaded and then viewed=rendered and processed by Qiqqa, those PDFs are essentially straight off the Internet and consequently security / stability of the PDF processing code should be up for that level of (unintentional) abuse.

---



## What I've been looking at so far? & What has been achieved?

### For rendering pages to images for printing / OCR / display-on-screen

- GhostScript
- MuPDF
- *misc*

I have looked at "done in pure C#" libraries, of which there were none even close to viable, outside the closed source commercial realm.

Then I looked at various "C# to native" interface libraries / projects, while keeping in mind my Plan B which is to make this whole process entirely *external* to Qiqqa: the PDF processors would then be invoked executables which produce images cached on disk[^ramdisk], where Qiqqa will pick them up for display. But that's Plan B, where Plan A is where I can get a *stable* system working *without* any intermediate disk I/O: that SHOULD, I expect, deliver a better rendering performance overall. 

[^ramdisk] there's thoughts of using a RAMdisk to speed this caching up, but that's not for everyone. Meanwhile I have to reckon with not every user having bleeding edge fast and copious SSD drive space available either, so caching to disk will come at the cost of a couple of HDD I/Os sitting in the PDF-to-screen-display pipeline, slowing things down due to disk access costs.

---


**Rendering Performance** is important as the user will want to see a page show up quickly on screen.[^bgtasks]
Having an in-app PDF renderer which can deliver image data quickly is considered a boon, hence the focus on 'native' libraries/projects, such as GhostScript, instead of alternatives such as PDF.js, which renders PDF in JavaScript.

[^bgtasks]: The other PDF processing tasks, such as Text Extraction, are important too, but happen in the background and allow for a performance that's perhaps a tad lower/slower.

---



#### Candidate 1: GhostScript

Tried a few projects for using GhostScript in a .NET setting, which were a dud.[^dud1] 

[^dud1]: Turns out that *had I discovered the trouble with GS's new `-dSAFER` default earlier*, then those projects would probably still be duds for me, but some of them *might* have worked. Kinda. Like GhostScript.NET. But we'll get there. Read on...

---


Then I tried my hand at GhostScript.NET, which looked promissing despite giving off the air of perhaps being a little stale[^stale1] -- heck, if a little effort can make it go again, then I'm okay with that.

[^stale1]: Check out the last commit dates [there](https://github.com/jhabjan/Ghostscript.NET/commits/master). 6 months and counting. So that's a possibly maybe. 😇

---



The result: I got GhostScript.NET to a point where it is using the latest GhostScript and rendering page images to disk quite nicely, but the latest GhostScript releases (v9.50+) have a `-dSAFER` setting [by default](https://www.ghostscript.com/doc/9.50/News.htm#28_Incompatible_changes), which makes sense from the security perspective, but turns out to completely b0rk the "output redirected to I/O `%handle%`" functionality, which is **one way of skipping that cache-images-on-disk overhead**. As you would be using GhostScript as an invoked executable, that's about the only way to get at the image data without going through some disk I/O & image file formats first[^diskIO]. Hence [the failing `%handle%` support in my v9.52 tests]( https://github.com/GerHobbelt/Ghostscript.NET/commits/master ) made that one a no-go. 💣

[^diskIO]: Note that that disk caching overhead is *significant*, despite the word 'cache': you DO NOT want to save uncompressed images to disk, particularly when rendered at higher DPI settings[^300dpi], so your cache will need to consist of **compressed format** image files, e.g. PNG.[^jpeg]

[^300dpi]: say 300dpi for OCR and print, and 144 to 288dpi for high retina screens, depending on font smoothing ability of the renderer and other factors that could otherwise negatively impact the view quality of rendered text.

[^jpeg]: Why not use JPEG? JPEG causes it's own set of compression artifacts and thus viewer cruft, unless compressed at (very) high quality settings, so better to go for a lossless compressed format such as PNG or TIFF.

---



Such an image cache would require at least one compression action on image creation, hence taking a little extra time after rendering and before first view there, while every vieweing would require a corresponding decompression action, thus taking a little extra time (though minimal) before actual viewing of the page.

If you can skip that[^pluscache] and replace it with the minimal cost of marshaling the raw image data into C#, which happens entirely in RAM, then you should be pretty much near what would be your maximum achivable rendering+viewing performance.


[^pluscache]: Of course, you *can* still keep the cache around for later (re)viewing of those same pages if you expect such a thing to happen[^scrollscenario], but then you can do all that caching == image compressing business in the background and slowly, because nobody's waiting for it to show up results *urgently*.

[^scrollscenario]: e.g. a user scrolling through a PDF in a viewer is not a totally linear process: when you read *information*, e.g. *papers*, rather than *entertainment*, chances are you'll be skipping back regularly to check a few things while reading that new paragraph.

---







#### Candidate 2: MuPDF

That one comes with MuPDFSharp.NET, where the .NET component looks a bit stale[^likegsnet] but MuPDF is [on GitHub]() and is also available at the ArtiFex company git repository. Given that there are already some (older) Visual Studio `.sln` solutions available for this one, hopes are high (and it's our last option anyway, so we're getting a wee bit desperate 🥶) and a long story short: got the whole `mupdf` + `mutools` kaboodle compiling and running in both 32 and 64 bit target form. 🎉

[^likegsnet]: just like the GhostScript.NET project did.

---

This did not include the MuPDFSharp.NET part yet, which expected MuPDF to come as a DLL, which wasn't there. (Turned out one of the projects in `platforms/win32/` was meant for that but was not included in the Visual Studio solution file.) 

After a bit of tweaking I got that going as well, also in 64bit, so I'm now sure to be able to produce a PDF renderer that can be incorporated into Qiqqa. 🥳

**There remain a few important things to be done though**: 

- current MuPDF code uses a custom try/throw/catch system in its C-language code, which does not transport over into .NET.[^okcommit1]
- also there's the matter of MuPDF printing its error/diagnositcs to stdout/stderr, which we'll have a hard time capturing from .NET[^whoops1]
- there's thee old patch report from Jimme in the Qiqqa repo at [`libs/3rdpart/mupdf.changes.txt`]( https://github.com/jimmejardine/qiqqa-open-source/blob/master/libs/3rdparty/mupdf.changes.txt ), which must be inspected and merged with the current MuPDF state as there are a few changes in there which are important for the MuPDF-Qiqqa interaction in the QiqqaOCR Text Extraction process.
- there's the problem  of https://bugs.ghostscript.com/show_bug.cgi?id=701945, which is reproducible in my copy: that PDF indeed takes *ages* and emphasizes another concern I have after having observed `mupdf` with other PDFs in use already: **page rendering should be interruptible** so as not to take an inordinate amount of time (better to produce an "*insert*" page image for such pages when the timeout/abort is *not* due to the user scrolling away from the page -- thus making the effort of rendering that page moot). 

  The key here is to improve PDF paging/browsing response times while unloading the CPU as soon as it has become clear that the current render job is not needed any longer.


All of the above require additional software work to be done on the MuPDF source code repo before this thing is ready for prime time, but at least there are no visible or *expected* roadblocks there any more.  🥳


[^okcommit1]: Here's the commit message for that bit, which includes links to several pages which further address this issue (and related ones):

> commit SHA-1: b8ecec20bffce73fec6d49e141cbb2d24b820ac3
>
> Finally we've got 64-bit P/Invoke native MuPDF access going.    
> 
> **TODO**: add interface robustness, as the mupdf native code prints to stdout/stderr, which we won't see, plus, more importantly, it uses [a custom-made C-language `try/throw/catch` mechanism using `setjmp`/`longjmp`]( https://github.com/ArtifexSoftware/mupdf/blob/master/include/mupdf/fitz/context.h#L28-L45 ), which will try to cross the interface boundary: we MUST add an interface barrier at the native DLL level for this to be stable ([lacking an outer `setjmp()`, the lib code will call `exit(n)`]( https://github.com/ArtifexSoftware/mupdf/blob/master/source/fitz/error.c#L141-L155 ), **thus killing the entire application when an internal error occurs**! yuk! 🤯 )
>
> **R&D : articles looked at:**
>
> - https://docs.microsoft.com/en-us/dotnet/standard/native-interop/exceptions-interoperability  (note the longjmp remark there!)
> - https://github.com/dotnet/runtime/issues/4756
> - https://docs.microsoft.com/en-us/cpp/dotnet/using-explicit-pinvoke-in-cpp-dllimport-attribute?view=vs-2019
> - https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-ansi-strings-using-cpp-interop?view=vs-2019
> - https://mark-borg.github.io/blog/2017/interop/
> - https://github.com/abock/CurlSharp
> - https://github.com/MicrosoftDocs/cpp-docs/blob/master/docs/dotnet/using-cpp-interop-implicit-pinvoke.md
> - http://www.pinvoke.net/index.aspx
> - https://stackoverflow.com/questions/11515125/how-do-i-re-assign-the-standard-error-stream-such-that-exceptions-thrown-by-libr?noredirect=1&lq=1
> - https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll?noredirect=1&lq=1
> - https://stackoverflow.com/questions/1579074/redirect-stdoutstderr-on-a-c-sharp-windows-service
> - https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll
> - https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/pinvokestackimbalance-mda?redirectedfrom=MSDN
> - https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ee941656(v=vs.100)?redirectedfrom=MSDN ("NET 4 Migration Issues")
> - https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/legacycorruptedstateexceptionspolicy-element?redirectedfrom=MSDN

[^whoops1]: as this is not a separate `.exe` application we're running but a DLL that is an integral part of the Qiqqa application itself! See also the reference links listed above.[^okcommit1]

---








## The journey goes on

At the time of this writing, I still have a ways to go with MuPDF + MuPDFSharp.NET before it's ready for incorporation in the Qiqqa application, but the results so far have, after several setbacks, given me hope that this is achievable after all.

Next to the 
Looking ahead, with this *done*, there's the embedded browser upgrade that needs doing urgently as Google Scholar is throwing all sorts of tantrums.[^xulrunner]

[^xulrunner]: See these for that horror show:
- https://github.com/jimmejardine/qiqqa-open-source/issues/199
- https://github.com/jimmejardine/qiqqa-open-source/issues/155
- https://github.com/jimmejardine/qiqqa-open-source/issues/113
- https://github.com/jimmejardine/qiqqa-open-source/issues/2

---

And then we have not yet addressed the various Text Extraction pericles around Qiqqa, whether it's *technically* OCR or "*mere*" Text Extraction issues[^textext1]:

- https://github.com/jimmejardine/qiqqa-open-source/issues/193
- https://github.com/jimmejardine/qiqqa-open-source/issues/179
- https://github.com/jimmejardine/qiqqa-open-source/issues/172
- https://github.com/jimmejardine/qiqqa-open-source/issues/169
- https://github.com/jimmejardine/qiqqa-open-source/issues/166
- https://github.com/jimmejardine/qiqqa-open-source/issues/165
- https://github.com/jimmejardine/qiqqa-open-source/issues/160
- https://github.com/jimmejardine/qiqqa-open-source/issues/159
- https://github.com/jimmejardine/qiqqa-open-source/issues/136
- https://github.com/jimmejardine/qiqqa-open-source/issues/135
- https://github.com/jimmejardine/qiqqa-open-source/issues/129
- https://github.com/jimmejardine/qiqqa-open-source/issues/127
- https://github.com/jimmejardine/qiqqa-open-source/issues/73
- https://github.com/jimmejardine/qiqqa-open-source/issues/35

These largely involve \[our use of an old version of] Tesseract, but it also involves `pdfdraw`, which is part of MuPDF, as this is also about Text Extraction without having to go through the OCR process since the PDF under scrutiny already has an embedded text layer.

Anyhow, there's plenty to do before we can go to sleep. 🎭


[^textext1]: where Text Extraction would **probably** be done through MuPDF in its upgraded form, as that has worked for us pretty well in the past, despite some very odd PDF output being produced that way.[^textext2]

[^textext2]: I have a couple of ideas how to improve on that as well, but that has lower priority ATM 

---

