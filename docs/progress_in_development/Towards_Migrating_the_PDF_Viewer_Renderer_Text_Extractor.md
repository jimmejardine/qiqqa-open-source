<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <h1>Towards migrating the PDF viewer / renderer ( / text extractor)</h1>
<h2>The old situation (Qiqqa v82 and below)</h2>
<p>Qiqqa used the commercial closed source Sorax lib for rendering PDF pages to screen and frankly that’s a dud as quite a few legal PDFs are not rendered by that lib (blank pages result).</p>
<p>Qiqqa also used an old patched<sup class="footnote-ref"><a href="#fn1" id="fnref1">[1]</a></sup> version of <code>pdfdraw</code> (v1.1.4) in the QiqqaOCR tool for text extraction (<code>pdfdraw -tt</code>).</p>
<hr>
<h2>The goal / what’ld be ideal</h2>
<ul>
<li>Use a single PDF renderer / text extractor, so that <em>when</em> a PDF is accepted, both will “interpret” the PDF the same way: the renderer producing page images while the text extractor part will produce hOCR or similar text+coordinates data from those pages: when both outputs are produced by a single toolchain then my <strong>assumption / expectation</strong> is that the hOCR words SHOULD be at the same spot as the image rendered pixels for them, even when we’re processing a somewhat <em>odd</em> PDF.</li>
<li>No closed source libraries anywhere: if bugs aren’t fixed quickly by a support team, they should at least be allowed to be analyzed in depth and for that you need <strong>source code</strong>. Too many very bad experiences with closed source for this fellow. 🤕</li>
<li>available in 32bit <em>and</em> 64bit with a C# interface so we can move Qiqqa into the 64-bit realm once we’ve got rid of the 32bit requirement thanks to antiquated XULrunner – this should make life easier on modern boxes and when perusing (very) large libraries.</li>
<li>very near or at Acrobat performance and PDF compatibility, i.e. SHOULD NOT b0rk on many PDFs, even evil ones.<sup class="footnote-ref"><a href="#fn2" id="fnref2">[2]</a></sup></li>
</ul>
<hr>
<h2>What I’ve been looking at so far? &amp; What has been achieved?</h2>
<h3>For rendering pages to images for printing / OCR / display-on-screen</h3>
<ul>
<li>GhostScript</li>
<li>MuPDF</li>
<li><em>misc</em></li>
</ul>
<p>I have looked at “done in pure C#” libraries, of which there were none even close to viable, outside the closed source commercial realm.</p>
<p>Then I looked at various “C# to native” interface libraries / projects, while keeping in mind my Plan B which is to make this whole process entirely <em>external</em> to Qiqqa: the PDF processors would then be invoked executables which produce images cached on disk[^ramdisk], where Qiqqa will pick them up for display. But that’s Plan B, where Plan A is where I can get a <em>stable</em> system working <em>without</em> any intermediate disk I/O: that SHOULD, I expect, deliver a better rendering performance overall.</p>
<p>[^ramdisk] there’s thoughts of using a RAMdisk to speed this caching up, but that’s not for everyone. Meanwhile I have to reckon with not every user having bleeding edge fast and copious SSD drive space available either, so caching to disk will come at the cost of a couple of HDD I/Os sitting in the PDF-to-screen-display pipeline, slowing things down due to disk access costs.</p>
<hr>
<p><strong>Rendering Performance</strong> is important as the user will want to see a page show up quickly on screen.<sup class="footnote-ref"><a href="#fn3" id="fnref3">[3]</a></sup>
Having an in-app PDF renderer which can deliver image data quickly is considered a boon, hence the focus on ‘native’ libraries/projects, such as GhostScript, instead of alternatives such as PDF.js, which renders PDF in JavaScript.</p>
<hr>
<h4>Candidate 1: GhostScript</h4>
<p>Tried a few projects for using GhostScript in a .NET setting, which were a dud.<sup class="footnote-ref"><a href="#fn4" id="fnref4">[4]</a></sup></p>
<hr>
<p>Then I tried my hand at <a href="http://GhostScript.NET">GhostScript.NET</a>, which looked promissing despite giving off the air of perhaps being a little stale<sup class="footnote-ref"><a href="#fn5" id="fnref5">[5]</a></sup> – heck, if a little effort can make it go again, then I’m okay with that.</p>
<hr>
<p>The result: I got <a href="http://GhostScript.NET">GhostScript.NET</a> to a point where it is using the latest GhostScript and rendering page images to disk quite nicely, but the latest GhostScript releases (v9.50+) have a <code>-dSAFER</code> setting <a href="https://www.ghostscript.com/doc/9.50/News.htm#28_Incompatible_changes">by default</a>, which makes sense from the security perspective, but turns out to completely b0rk the “output redirected to I/O <code>%handle%</code>” functionality, which is <strong>one way of skipping that cache-images-on-disk overhead</strong>. As you would be using GhostScript as an invoked executable, that’s about the only way to get at the image data without going through some disk I/O &amp; image file formats first<sup class="footnote-ref"><a href="#fn6" id="fnref6">[6]</a></sup>. Hence <a href="https://github.com/GerHobbelt/Ghostscript.NET/commits/master">the failing <code>%handle%</code> support in my v9.52 tests</a> made that one a no-go. 💣</p>
<hr>
<p>Such an image cache would require at least one compression action on image creation, hence taking a little extra time after rendering and before first view there, while every vieweing would require a corresponding decompression action, thus taking a little extra time (though minimal) before actual viewing of the page.</p>
<p>If you can skip that<sup class="footnote-ref"><a href="#fn9" id="fnref9">[9]</a></sup> and replace it with the minimal cost of marshaling the raw image data into C#, which happens entirely in RAM, then you should be pretty much near what would be your maximum achivable rendering+viewing performance.</p>
<hr>
<h4>Candidate 2: MuPDF</h4>
<p>That one comes with <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a>, where the .NET component looks a bit stale<sup class="footnote-ref"><a href="#fn11" id="fnref11">[11]</a></sup> but MuPDF is <a href="">on GitHub</a> and is also available at the ArtiFex company git repository. Given that there are already some (older) Visual Studio <code>.sln</code> solutions available for this one, hopes are high (and it’s our last option anyway, so we’re getting a wee bit desperate 🥶) and a long story short: got the whole <code>mupdf</code> + <code>mutools</code> kaboodle compiling and running in both 32 and 64 bit target form. 🎉</p>
<hr>
<p>This did not include the <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a> part yet, which expected MuPDF to come as a DLL, which wasn’t there. (Turned out one of the projects in <code>platforms/win32/</code> was meant for that but was not included in the Visual Studio solution file.)</p>
<p>After a bit of tweaking I got that going as well, also in 64bit, so I’m now sure to be able to produce a PDF renderer that can be incorporated into Qiqqa. 🥳</p>
<p><strong>There remain a few important things to be done though</strong>:</p>
<ul>
<li>
<p>current MuPDF code uses a custom try/throw/catch system in its C-language code, which does not transport over into .NET.<sup class="footnote-ref"><a href="#fn12" id="fnref12">[12]</a></sup></p>
</li>
<li>
<p>also there’s the matter of MuPDF printing its error/diagnositcs to stdout/stderr, which we’ll have a hard time capturing from .NET<sup class="footnote-ref"><a href="#fn13" id="fnref13">[13]</a></sup></p>
</li>
<li>
<p>there’s thee old patch report from Jimme in the Qiqqa repo at <a href="https://github.com/jimmejardine/qiqqa-open-source/blob/master/libs/3rdparty/mupdf.changes.txt"><code>libs/3rdpart/mupdf.changes.txt</code></a>, which must be inspected and merged with the current MuPDF state as there are a few changes in there which are important for the MuPDF-Qiqqa interaction in the QiqqaOCR Text Extraction process.</p>
</li>
<li>
<p>there’s the problem  of <a href="https://bugs.ghostscript.com/show_bug.cgi?id=701945">https://bugs.ghostscript.com/show_bug.cgi?id=701945</a>, which is reproducible in my copy: that PDF indeed takes <em>ages</em> and emphasizes another concern I have after having observed <code>mupdf</code> with other PDFs in use already: <strong>page rendering should be interruptible</strong> so as not to take an inordinate amount of time (better to produce an “<em>insert</em>” page image for such pages when the timeout/abort is <em>not</em> due to the user scrolling away from the page – thus making the effort of rendering that page moot).</p>
<p>The key here is to improve PDF paging/browsing response times while unloading the CPU as soon as it has become clear that the current render job is not needed any longer.</p>
</li>
</ul>
<p>All of the above require additional software work to be done on the MuPDF source code repo before this thing is ready for prime time, but at least there are no visible or <em>expected</em> roadblocks there any more.  🥳</p>
<blockquote>
<p>commit SHA-1: b8ecec20bffce73fec6d49e141cbb2d24b820ac3</p>
<p>Finally we’ve got 64-bit P/Invoke native MuPDF access going.</p>
<p><strong>TODO</strong>: add interface robustness, as the mupdf native code prints to stdout/stderr, which we won’t see, plus, more importantly, it uses <a href="https://github.com/ArtifexSoftware/mupdf/blob/master/include/mupdf/fitz/context.h#L28-L45">a custom-made C-language <code>try/throw/catch</code> mechanism using <code>setjmp</code>/<code>longjmp</code></a>, which will try to cross the interface boundary: we MUST add an interface barrier at the native DLL level for this to be stable (<a href="https://github.com/ArtifexSoftware/mupdf/blob/master/source/fitz/error.c#L141-L155">lacking an outer <code>setjmp()</code>, the lib code will call <code>exit(n)</code></a>, <strong>thus killing the entire application when an internal error occurs</strong>! yuk! 🤯 )</p>
<p><strong>R&amp;D : articles looked at:</strong></p>
<ul>
<li><a href="https://docs.microsoft.com/en-us/dotnet/standard/native-interop/exceptions-interoperability">https://docs.microsoft.com/en-us/dotnet/standard/native-interop/exceptions-interoperability</a>  (note the longjmp remark there!)</li>
<li><a href="https://github.com/dotnet/runtime/issues/4756">https://github.com/dotnet/runtime/issues/4756</a></li>
<li><a href="https://docs.microsoft.com/en-us/cpp/dotnet/using-explicit-pinvoke-in-cpp-dllimport-attribute?view=vs-2019">https://docs.microsoft.com/en-us/cpp/dotnet/using-explicit-pinvoke-in-cpp-dllimport-attribute?view=vs-2019</a></li>
<li><a href="https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-ansi-strings-using-cpp-interop?view=vs-2019">https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-ansi-strings-using-cpp-interop?view=vs-2019</a></li>
<li><a href="https://mark-borg.github.io/blog/2017/interop/">https://mark-borg.github.io/blog/2017/interop/</a></li>
<li><a href="https://github.com/abock/CurlSharp">https://github.com/abock/CurlSharp</a></li>
<li><a href="https://github.com/MicrosoftDocs/cpp-docs/blob/master/docs/dotnet/using-cpp-interop-implicit-pinvoke.md">https://github.com/MicrosoftDocs/cpp-docs/blob/master/docs/dotnet/using-cpp-interop-implicit-pinvoke.md</a></li>
<li><a href="http://www.pinvoke.net/index.aspx">http://www.pinvoke.net/index.aspx</a></li>
<li><a href="https://stackoverflow.com/questions/11515125/how-do-i-re-assign-the-standard-error-stream-such-that-exceptions-thrown-by-libr?noredirect=1&amp;lq=1">https://stackoverflow.com/questions/11515125/how-do-i-re-assign-the-standard-error-stream-such-that-exceptions-thrown-by-libr?noredirect=1&amp;lq=1</a></li>
<li><a href="https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll?noredirect=1&amp;lq=1">https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll?noredirect=1&amp;lq=1</a></li>
<li><a href="https://stackoverflow.com/questions/1579074/redirect-stdoutstderr-on-a-c-sharp-windows-service">https://stackoverflow.com/questions/1579074/redirect-stdoutstderr-on-a-c-sharp-windows-service</a></li>
<li><a href="https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll">https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll</a></li>
<li><a href="https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/pinvokestackimbalance-mda?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/pinvokestackimbalance-mda?redirectedfrom=MSDN</a></li>
<li><a href="https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ee941656(v=vs.100)?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ee941656(v=vs.100)?redirectedfrom=MSDN</a> (“NET 4 Migration Issues”)</li>
<li><a href="https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/legacycorruptedstateexceptionspolicy-element?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/legacycorruptedstateexceptionspolicy-element?redirectedfrom=MSDN</a></li>
</ul>
</blockquote>
<hr>
<h2>The journey goes on</h2>
<p>At the time of this writing, I still have a ways to go with MuPDF + <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a> before it’s ready for incorporation in the Qiqqa application, but the results so far have, after several setbacks, given me hope that this is achievable after all.</p>
<p>Next to the
Looking ahead, with this <em>done</em>, there’s the embedded browser upgrade that needs doing urgently as Google Scholar is throwing all sorts of tantrums.<sup class="footnote-ref"><a href="#fn14" id="fnref14">[14]</a></sup></p>
<ul>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/199">https://github.com/jimmejardine/qiqqa-open-source/issues/199</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/155">https://github.com/jimmejardine/qiqqa-open-source/issues/155</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/113">https://github.com/jimmejardine/qiqqa-open-source/issues/113</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/2">https://github.com/jimmejardine/qiqqa-open-source/issues/2</a></li>
</ul>
<hr>
<p>And then we have not yet addressed the various Text Extraction pericles around Qiqqa, whether it’s <em>technically</em> OCR or “<em>mere</em>” Text Extraction issues<sup class="footnote-ref"><a href="#fn15" id="fnref15">[15]</a></sup>:</p>
<ul>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/193">https://github.com/jimmejardine/qiqqa-open-source/issues/193</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/179">https://github.com/jimmejardine/qiqqa-open-source/issues/179</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/172">https://github.com/jimmejardine/qiqqa-open-source/issues/172</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/169">https://github.com/jimmejardine/qiqqa-open-source/issues/169</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/166">https://github.com/jimmejardine/qiqqa-open-source/issues/166</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/165">https://github.com/jimmejardine/qiqqa-open-source/issues/165</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/160">https://github.com/jimmejardine/qiqqa-open-source/issues/160</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/159">https://github.com/jimmejardine/qiqqa-open-source/issues/159</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/136">https://github.com/jimmejardine/qiqqa-open-source/issues/136</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/135">https://github.com/jimmejardine/qiqqa-open-source/issues/135</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/129">https://github.com/jimmejardine/qiqqa-open-source/issues/129</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/127">https://github.com/jimmejardine/qiqqa-open-source/issues/127</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/73">https://github.com/jimmejardine/qiqqa-open-source/issues/73</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/35">https://github.com/jimmejardine/qiqqa-open-source/issues/35</a></li>
</ul>
<p>These largely involve [our use of an old version of] Tesseract, but it also involves <code>pdfdraw</code>, which is part of MuPDF, as this is also about Text Extraction without having to go through the OCR process since the PDF under scrutiny already has an embedded text layer.</p>
<p>Anyhow, there’s plenty to do before we can go to sleep. 🎭</p>
<hr>
<hr class="footnotes-sep">
<section class="footnotes">
<ol class="footnotes-list">
<li tabindex="-1" id="fn1" class="footnote-item"><p><em>patched</em> because there’s Quantisle being mentioned in the executable and there’s <code>libs/3rdparty/mupdf.changes.txt</code>. <em>Hints!</em> 😁 <a href="#fnref1" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn2" class="footnote-item"><p>In the end, the PDF renderer WILL be Internet facing – even while only <em>indirectly</em>, but as PDFs are downloaded and then viewed=rendered and processed by Qiqqa, those PDFs are essentially straight off the Internet and consequently security / stability of the PDF processing code should be up for that level of (unintentional) abuse. <a href="#fnref2" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn3" class="footnote-item"><p>The other PDF processing tasks, such as Text Extraction, are important too, but happen in the background and allow for a performance that’s perhaps a tad lower/slower. <a href="#fnref3" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn4" class="footnote-item"><p>Turns out that <em>had I discovered the trouble with GS’s new <code>-dSAFER</code> default earlier</em>, then those projects would probably still be duds for me, but some of them <em>might</em> have worked. Kinda. Like <a href="http://GhostScript.NET">GhostScript.NET</a>. But we’ll get there. Read on… <a href="#fnref4" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn5" class="footnote-item"><p>Check out the last commit dates <a href="https://github.com/jhabjan/Ghostscript.NET/commits/master">there</a>. 6 months and counting. So that’s a possibly maybe. 😇 <a href="#fnref5" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn6" class="footnote-item"><p>Note that that disk caching overhead is <em>significant</em>, despite the word ‘cache’: you DO NOT want to save uncompressed images to disk, particularly when rendered at higher DPI settings<sup class="footnote-ref"><a href="#fn7" id="fnref7">[7]</a></sup>, so your cache will need to consist of <strong>compressed format</strong> image files, e.g. PNG.<sup class="footnote-ref"><a href="#fn8" id="fnref8">[8]</a></sup> <a href="#fnref6" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn7" class="footnote-item"><p>say 300dpi for OCR and print, and 144 to 288dpi for high retina screens, depending on font smoothing ability of the renderer and other factors that could otherwise negatively impact the view quality of rendered text. <a href="#fnref7" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn8" class="footnote-item"><p>Why not use JPEG? JPEG causes it’s own set of compression artifacts and thus viewer cruft, unless compressed at (very) high quality settings, so better to go for a lossless compressed format such as PNG or TIFF. <a href="#fnref8" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn9" class="footnote-item"><p>Of course, you <em>can</em> still keep the cache around for later (re)viewing of those same pages if you expect such a thing to happen<sup class="footnote-ref"><a href="#fn10" id="fnref10">[10]</a></sup>, but then you can do all that caching == image compressing business in the background and slowly, because nobody’s waiting for it to show up results <em>urgently</em>. <a href="#fnref9" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn10" class="footnote-item"><p>e.g. a user scrolling through a PDF in a viewer is not a totally linear process: when you read <em>information</em>, e.g. <em>papers</em>, rather than <em>entertainment</em>, chances are you’ll be skipping back regularly to check a few things while reading that new paragraph. <a href="#fnref10" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn11" class="footnote-item"><p>just like the <a href="http://GhostScript.NET">GhostScript.NET</a> project did. <a href="#fnref11" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn12" class="footnote-item"><p>Here’s the commit message for that bit, which includes <abbr title="[object Object]">links</abbr> to several pages which further address this issue (and related ones): <a href="#fnref12" class="footnote-backref">↩︎</a> <a href="#fnref12:1" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn13" class="footnote-item"><p>as this is not a separate <code>.exe</code> application we’re running but a DLL that is an integral part of the Qiqqa application itself! See also the reference <abbr title="[object Object]">links</abbr> listed above.<sup class="footnote-ref"><a href="#fn12" id="fnref12:1">[12:1]</a></sup> <a href="#fnref13" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn14" class="footnote-item"><p>See these for that horror show: <a href="#fnref14" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn15" class="footnote-item"><p>where Text Extraction would <strong>probably</strong> be done through MuPDF in its upgraded form, as that has worked for us pretty well in the past, despite some very odd PDF output being produced that way.<sup class="footnote-ref"><a href="#fn16" id="fnref16">[16]</a></sup> <a href="#fnref15" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn16" class="footnote-item"><p>I have a couple of ideas how to improve on that as well, but that has lower priority ATM <a href="#fnref16" class="footnote-backref">↩︎</a></p>
</li>
</ol>
</section>

  </head>
  <body>

    <h1>Towards migrating the PDF viewer / renderer ( / text extractor)</h1>
<h2>The old situation (Qiqqa v82 and below)</h2>
<p>Qiqqa used the commercial closed source Sorax lib for rendering PDF pages to screen and frankly that’s a dud as quite a few legal PDFs are not rendered by that lib (blank pages result).</p>
<p>Qiqqa also used an old patched<sup class="footnote-ref"><a href="#fn1" id="fnref1">[1]</a></sup> version of <code>pdfdraw</code> (v1.1.4) in the QiqqaOCR tool for text extraction (<code>pdfdraw -tt</code>).</p>
<hr>
<h2>The goal / what’ld be ideal</h2>
<ul>
<li>Use a single PDF renderer / text extractor, so that <em>when</em> a PDF is accepted, both will “interpret” the PDF the same way: the renderer producing page images while the text extractor part will produce hOCR or similar text+coordinates data from those pages: when both outputs are produced by a single toolchain then my <strong>assumption / expectation</strong> is that the hOCR words SHOULD be at the same spot as the image rendered pixels for them, even when we’re processing a somewhat <em>odd</em> PDF.</li>
<li>No closed source libraries anywhere: if bugs aren’t fixed quickly by a support team, they should at least be allowed to be analyzed in depth and for that you need <strong>source code</strong>. Too many very bad experiences with closed source for this fellow. 🤕</li>
<li>available in 32bit <em>and</em> 64bit with a C# interface so we can move Qiqqa into the 64-bit realm once we’ve got rid of the 32bit requirement thanks to antiquated XULrunner – this should make life easier on modern boxes and when perusing (very) large libraries.</li>
<li>very near or at Acrobat performance and PDF compatibility, i.e. SHOULD NOT b0rk on many PDFs, even evil ones.<sup class="footnote-ref"><a href="#fn2" id="fnref2">[2]</a></sup></li>
</ul>
<hr>
<h2>What I’ve been looking at so far? &amp; What has been achieved?</h2>
<h3>For rendering pages to images for printing / OCR / display-on-screen</h3>
<ul>
<li>GhostScript</li>
<li>MuPDF</li>
<li><em>misc</em></li>
</ul>
<p>I have looked at “done in pure C#” libraries, of which there were none even close to viable, outside the closed source commercial realm.</p>
<p>Then I looked at various “C# to native” interface libraries / projects, while keeping in mind my Plan B which is to make this whole process entirely <em>external</em> to Qiqqa: the PDF processors would then be invoked executables which produce images cached on disk[^ramdisk], where Qiqqa will pick them up for display. But that’s Plan B, where Plan A is where I can get a <em>stable</em> system working <em>without</em> any intermediate disk I/O: that SHOULD, I expect, deliver a better rendering performance overall.</p>
<p>[^ramdisk] there’s thoughts of using a RAMdisk to speed this caching up, but that’s not for everyone. Meanwhile I have to reckon with not every user having bleeding edge fast and copious SSD drive space available either, so caching to disk will come at the cost of a couple of HDD I/Os sitting in the PDF-to-screen-display pipeline, slowing things down due to disk access costs.</p>
<hr>
<p><strong>Rendering Performance</strong> is important as the user will want to see a page show up quickly on screen.<sup class="footnote-ref"><a href="#fn3" id="fnref3">[3]</a></sup>
Having an in-app PDF renderer which can deliver image data quickly is considered a boon, hence the focus on ‘native’ libraries/projects, such as GhostScript, instead of alternatives such as PDF.js, which renders PDF in JavaScript.</p>
<hr>
<h4>Candidate 1: GhostScript</h4>
<p>Tried a few projects for using GhostScript in a .NET setting, which were a dud.<sup class="footnote-ref"><a href="#fn4" id="fnref4">[4]</a></sup></p>
<hr>
<p>Then I tried my hand at <a href="http://GhostScript.NET">GhostScript.NET</a>, which looked promissing despite giving off the air of perhaps being a little stale<sup class="footnote-ref"><a href="#fn5" id="fnref5">[5]</a></sup> – heck, if a little effort can make it go again, then I’m okay with that.</p>
<hr>
<p>The result: I got <a href="http://GhostScript.NET">GhostScript.NET</a> to a point where it is using the latest GhostScript and rendering page images to disk quite nicely, but the latest GhostScript releases (v9.50+) have a <code>-dSAFER</code> setting <a href="https://www.ghostscript.com/doc/9.50/News.htm#28_Incompatible_changes">by default</a>, which makes sense from the security perspective, but turns out to completely b0rk the “output redirected to I/O <code>%handle%</code>” functionality, which is <strong>one way of skipping that cache-images-on-disk overhead</strong>. As you would be using GhostScript as an invoked executable, that’s about the only way to get at the image data without going through some disk I/O &amp; image file formats first<sup class="footnote-ref"><a href="#fn6" id="fnref6">[6]</a></sup>. Hence <a href="https://github.com/GerHobbelt/Ghostscript.NET/commits/master">the failing <code>%handle%</code> support in my v9.52 tests</a> made that one a no-go. 💣</p>
<hr>
<p>Such an image cache would require at least one compression action on image creation, hence taking a little extra time after rendering and before first view there, while every vieweing would require a corresponding decompression action, thus taking a little extra time (though minimal) before actual viewing of the page.</p>
<p>If you can skip that<sup class="footnote-ref"><a href="#fn9" id="fnref9">[9]</a></sup> and replace it with the minimal cost of marshaling the raw image data into C#, which happens entirely in RAM, then you should be pretty much near what would be your maximum achivable rendering+viewing performance.</p>
<hr>
<h4>Candidate 2: MuPDF</h4>
<p>That one comes with <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a>, where the .NET component looks a bit stale<sup class="footnote-ref"><a href="#fn11" id="fnref11">[11]</a></sup> but MuPDF is <a href="">on GitHub</a> and is also available at the ArtiFex company git repository. Given that there are already some (older) Visual Studio <code>.sln</code> solutions available for this one, hopes are high (and it’s our last option anyway, so we’re getting a wee bit desperate 🥶) and a long story short: got the whole <code>mupdf</code> + <code>mutools</code> kaboodle compiling and running in both 32 and 64 bit target form. 🎉</p>
<hr>
<p>This did not include the <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a> part yet, which expected MuPDF to come as a DLL, which wasn’t there. (Turned out one of the projects in <code>platforms/win32/</code> was meant for that but was not included in the Visual Studio solution file.)</p>
<p>After a bit of tweaking I got that going as well, also in 64bit, so I’m now sure to be able to produce a PDF renderer that can be incorporated into Qiqqa. 🥳</p>
<p><strong>There remain a few important things to be done though</strong>:</p>
<ul>
<li>
<p>current MuPDF code uses a custom try/throw/catch system in its C-language code, which does not transport over into .NET.<sup class="footnote-ref"><a href="#fn12" id="fnref12">[12]</a></sup></p>
</li>
<li>
<p>also there’s the matter of MuPDF printing its error/diagnositcs to stdout/stderr, which we’ll have a hard time capturing from .NET<sup class="footnote-ref"><a href="#fn13" id="fnref13">[13]</a></sup></p>
</li>
<li>
<p>there’s thee old patch report from Jimme in the Qiqqa repo at <a href="https://github.com/jimmejardine/qiqqa-open-source/blob/master/libs/3rdparty/mupdf.changes.txt"><code>libs/3rdpart/mupdf.changes.txt</code></a>, which must be inspected and merged with the current MuPDF state as there are a few changes in there which are important for the MuPDF-Qiqqa interaction in the QiqqaOCR Text Extraction process.</p>
</li>
<li>
<p>there’s the problem  of <a href="https://bugs.ghostscript.com/show_bug.cgi?id=701945">https://bugs.ghostscript.com/show_bug.cgi?id=701945</a>, which is reproducible in my copy: that PDF indeed takes <em>ages</em> and emphasizes another concern I have after having observed <code>mupdf</code> with other PDFs in use already: <strong>page rendering should be interruptible</strong> so as not to take an inordinate amount of time (better to produce an “<em>insert</em>” page image for such pages when the timeout/abort is <em>not</em> due to the user scrolling away from the page – thus making the effort of rendering that page moot).</p>
<p>The key here is to improve PDF paging/browsing response times while unloading the CPU as soon as it has become clear that the current render job is not needed any longer.</p>
</li>
</ul>
<p>All of the above require additional software work to be done on the MuPDF source code repo before this thing is ready for prime time, but at least there are no visible or <em>expected</em> roadblocks there any more.  🥳</p>
<blockquote>
<p>commit SHA-1: b8ecec20bffce73fec6d49e141cbb2d24b820ac3</p>
<p>Finally we’ve got 64-bit P/Invoke native MuPDF access going.</p>
<p><strong>TODO</strong>: add interface robustness, as the mupdf native code prints to stdout/stderr, which we won’t see, plus, more importantly, it uses <a href="https://github.com/ArtifexSoftware/mupdf/blob/master/include/mupdf/fitz/context.h#L28-L45">a custom-made C-language <code>try/throw/catch</code> mechanism using <code>setjmp</code>/<code>longjmp</code></a>, which will try to cross the interface boundary: we MUST add an interface barrier at the native DLL level for this to be stable (<a href="https://github.com/ArtifexSoftware/mupdf/blob/master/source/fitz/error.c#L141-L155">lacking an outer <code>setjmp()</code>, the lib code will call <code>exit(n)</code></a>, <strong>thus killing the entire application when an internal error occurs</strong>! yuk! 🤯 )</p>
<p><strong>R&amp;D : articles looked at:</strong></p>
<ul>
<li><a href="https://docs.microsoft.com/en-us/dotnet/standard/native-interop/exceptions-interoperability">https://docs.microsoft.com/en-us/dotnet/standard/native-interop/exceptions-interoperability</a>  (note the longjmp remark there!)</li>
<li><a href="https://github.com/dotnet/runtime/issues/4756">https://github.com/dotnet/runtime/issues/4756</a></li>
<li><a href="https://docs.microsoft.com/en-us/cpp/dotnet/using-explicit-pinvoke-in-cpp-dllimport-attribute?view=vs-2019">https://docs.microsoft.com/en-us/cpp/dotnet/using-explicit-pinvoke-in-cpp-dllimport-attribute?view=vs-2019</a></li>
<li><a href="https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-ansi-strings-using-cpp-interop?view=vs-2019">https://docs.microsoft.com/en-us/cpp/dotnet/how-to-marshal-ansi-strings-using-cpp-interop?view=vs-2019</a></li>
<li><a href="https://mark-borg.github.io/blog/2017/interop/">https://mark-borg.github.io/blog/2017/interop/</a></li>
<li><a href="https://github.com/abock/CurlSharp">https://github.com/abock/CurlSharp</a></li>
<li><a href="https://github.com/MicrosoftDocs/cpp-docs/blob/master/docs/dotnet/using-cpp-interop-implicit-pinvoke.md">https://github.com/MicrosoftDocs/cpp-docs/blob/master/docs/dotnet/using-cpp-interop-implicit-pinvoke.md</a></li>
<li><a href="http://www.pinvoke.net/index.aspx">http://www.pinvoke.net/index.aspx</a></li>
<li><a href="https://stackoverflow.com/questions/11515125/how-do-i-re-assign-the-standard-error-stream-such-that-exceptions-thrown-by-libr?noredirect=1&amp;lq=1">https://stackoverflow.com/questions/11515125/how-do-i-re-assign-the-standard-error-stream-such-that-exceptions-thrown-by-libr?noredirect=1&amp;lq=1</a></li>
<li><a href="https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll?noredirect=1&amp;lq=1">https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll?noredirect=1&amp;lq=1</a></li>
<li><a href="https://stackoverflow.com/questions/1579074/redirect-stdoutstderr-on-a-c-sharp-windows-service">https://stackoverflow.com/questions/1579074/redirect-stdoutstderr-on-a-c-sharp-windows-service</a></li>
<li><a href="https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll">https://stackoverflow.com/questions/2215312/getting-stdout-when-p-invoking-to-unmanaged-dll</a></li>
<li><a href="https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/pinvokestackimbalance-mda?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/pinvokestackimbalance-mda?redirectedfrom=MSDN</a></li>
<li><a href="https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ee941656(v=vs.100)?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/ee941656(v=vs.100)?redirectedfrom=MSDN</a> (“NET 4 Migration Issues”)</li>
<li><a href="https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/legacycorruptedstateexceptionspolicy-element?redirectedfrom=MSDN">https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/legacycorruptedstateexceptionspolicy-element?redirectedfrom=MSDN</a></li>
</ul>
</blockquote>
<hr>
<h2>The journey goes on</h2>
<p>At the time of this writing, I still have a ways to go with MuPDF + <a href="http://MuPDFSharp.NET">MuPDFSharp.NET</a> before it’s ready for incorporation in the Qiqqa application, but the results so far have, after several setbacks, given me hope that this is achievable after all.</p>
<p>Next to the
Looking ahead, with this <em>done</em>, there’s the embedded browser upgrade that needs doing urgently as Google Scholar is throwing all sorts of tantrums.<sup class="footnote-ref"><a href="#fn14" id="fnref14">[14]</a></sup></p>
<ul>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/199">https://github.com/jimmejardine/qiqqa-open-source/issues/199</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/155">https://github.com/jimmejardine/qiqqa-open-source/issues/155</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/113">https://github.com/jimmejardine/qiqqa-open-source/issues/113</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/2">https://github.com/jimmejardine/qiqqa-open-source/issues/2</a></li>
</ul>
<hr>
<p>And then we have not yet addressed the various Text Extraction pericles around Qiqqa, whether it’s <em>technically</em> OCR or “<em>mere</em>” Text Extraction issues<sup class="footnote-ref"><a href="#fn15" id="fnref15">[15]</a></sup>:</p>
<ul>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/193">https://github.com/jimmejardine/qiqqa-open-source/issues/193</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/179">https://github.com/jimmejardine/qiqqa-open-source/issues/179</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/172">https://github.com/jimmejardine/qiqqa-open-source/issues/172</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/169">https://github.com/jimmejardine/qiqqa-open-source/issues/169</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/166">https://github.com/jimmejardine/qiqqa-open-source/issues/166</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/165">https://github.com/jimmejardine/qiqqa-open-source/issues/165</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/160">https://github.com/jimmejardine/qiqqa-open-source/issues/160</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/159">https://github.com/jimmejardine/qiqqa-open-source/issues/159</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/136">https://github.com/jimmejardine/qiqqa-open-source/issues/136</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/135">https://github.com/jimmejardine/qiqqa-open-source/issues/135</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/129">https://github.com/jimmejardine/qiqqa-open-source/issues/129</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/127">https://github.com/jimmejardine/qiqqa-open-source/issues/127</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/73">https://github.com/jimmejardine/qiqqa-open-source/issues/73</a></li>
<li><a href="https://github.com/jimmejardine/qiqqa-open-source/issues/35">https://github.com/jimmejardine/qiqqa-open-source/issues/35</a></li>
</ul>
<p>These largely involve [our use of an old version of] Tesseract, but it also involves <code>pdfdraw</code>, which is part of MuPDF, as this is also about Text Extraction without having to go through the OCR process since the PDF under scrutiny already has an embedded text layer.</p>
<p>Anyhow, there’s plenty to do before we can go to sleep. 🎭</p>
<hr>
<hr class="footnotes-sep">
<section class="footnotes">
<ol class="footnotes-list">
<li tabindex="-1" id="fn1" class="footnote-item"><p><em>patched</em> because there’s Quantisle being mentioned in the executable and there’s <code>libs/3rdparty/mupdf.changes.txt</code>. <em>Hints!</em> 😁 <a href="#fnref1" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn2" class="footnote-item"><p>In the end, the PDF renderer WILL be Internet facing – even while only <em>indirectly</em>, but as PDFs are downloaded and then viewed=rendered and processed by Qiqqa, those PDFs are essentially straight off the Internet and consequently security / stability of the PDF processing code should be up for that level of (unintentional) abuse. <a href="#fnref2" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn3" class="footnote-item"><p>The other PDF processing tasks, such as Text Extraction, are important too, but happen in the background and allow for a performance that’s perhaps a tad lower/slower. <a href="#fnref3" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn4" class="footnote-item"><p>Turns out that <em>had I discovered the trouble with GS’s new <code>-dSAFER</code> default earlier</em>, then those projects would probably still be duds for me, but some of them <em>might</em> have worked. Kinda. Like <a href="http://GhostScript.NET">GhostScript.NET</a>. But we’ll get there. Read on… <a href="#fnref4" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn5" class="footnote-item"><p>Check out the last commit dates <a href="https://github.com/jhabjan/Ghostscript.NET/commits/master">there</a>. 6 months and counting. So that’s a possibly maybe. 😇 <a href="#fnref5" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn6" class="footnote-item"><p>Note that that disk caching overhead is <em>significant</em>, despite the word ‘cache’: you DO NOT want to save uncompressed images to disk, particularly when rendered at higher DPI settings<sup class="footnote-ref"><a href="#fn7" id="fnref7">[7]</a></sup>, so your cache will need to consist of <strong>compressed format</strong> image files, e.g. PNG.<sup class="footnote-ref"><a href="#fn8" id="fnref8">[8]</a></sup> <a href="#fnref6" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn7" class="footnote-item"><p>say 300dpi for OCR and print, and 144 to 288dpi for high retina screens, depending on font smoothing ability of the renderer and other factors that could otherwise negatively impact the view quality of rendered text. <a href="#fnref7" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn8" class="footnote-item"><p>Why not use JPEG? JPEG causes it’s own set of compression artifacts and thus viewer cruft, unless compressed at (very) high quality settings, so better to go for a lossless compressed format such as PNG or TIFF. <a href="#fnref8" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn9" class="footnote-item"><p>Of course, you <em>can</em> still keep the cache around for later (re)viewing of those same pages if you expect such a thing to happen<sup class="footnote-ref"><a href="#fn10" id="fnref10">[10]</a></sup>, but then you can do all that caching == image compressing business in the background and slowly, because nobody’s waiting for it to show up results <em>urgently</em>. <a href="#fnref9" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn10" class="footnote-item"><p>e.g. a user scrolling through a PDF in a viewer is not a totally linear process: when you read <em>information</em>, e.g. <em>papers</em>, rather than <em>entertainment</em>, chances are you’ll be skipping back regularly to check a few things while reading that new paragraph. <a href="#fnref10" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn11" class="footnote-item"><p>just like the <a href="http://GhostScript.NET">GhostScript.NET</a> project did. <a href="#fnref11" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn12" class="footnote-item"><p>Here’s the commit message for that bit, which includes <abbr title="[object Object]">links</abbr> to several pages which further address this issue (and related ones): <a href="#fnref12" class="footnote-backref">↩︎</a> <a href="#fnref12:1" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn13" class="footnote-item"><p>as this is not a separate <code>.exe</code> application we’re running but a DLL that is an integral part of the Qiqqa application itself! See also the reference <abbr title="[object Object]">links</abbr> listed above.<sup class="footnote-ref"><a href="#fn12" id="fnref12:1">[12:1]</a></sup> <a href="#fnref13" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn14" class="footnote-item"><p>See these for that horror show: <a href="#fnref14" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn15" class="footnote-item"><p>where Text Extraction would <strong>probably</strong> be done through MuPDF in its upgraded form, as that has worked for us pretty well in the past, despite some very odd PDF output being produced that way.<sup class="footnote-ref"><a href="#fn16" id="fnref16">[16]</a></sup> <a href="#fnref15" class="footnote-backref">↩︎</a></p>
</li>
<li tabindex="-1" id="fn16" class="footnote-item"><p>I have a couple of ideas how to improve on that as well, but that has lower priority ATM <a href="#fnref16" class="footnote-backref">↩︎</a></p>
</li>
</ol>
</section>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/Progress in Development/Towards Migrating the PDF Viewer + Renderer (+ Text Extractor).md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
