<!doctype html>
<html lang="en">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    <h1>scripting the OCR + text extraction process</h1>
<p>So I’ve been looking at getting mupdf (with built-in tesseract) for the new approach to PDF text extraction in Qiqqa.</p>
<p>Old (= Commercial) Qiqqa uses an antique (patched) version of mupdf AFAICT (<code>pdfdraw</code>) for text extraction only. OCR has been handled by tesseract 3.0.x, serving us in the form of a DLL.
New Qiqqa, I hope, will do all that, <em>plus</em> page rendering to image for the UI, via the latest <code>mupdf</code> versions, using the <code>mudraw</code>, <code>mutool</code> or possibly others from the <code>mupdf</code> package.</p>
<p>As I want to “open up” the OCR and text extraction process so I can give people the option of <em>editing</em> those processes to suit their need, for example when handling obnoxious “crypted” PDFs.  Some PDFs are scrambled to “protect” them against text select + copy/paste, for example!</p>
<p>I was considering scripting it with JavaScript and then came up with the thought that <code>mupdf</code> already includes <code>mujs</code>, a (basic) JavaScript engine used by <code>mupdf</code>, so why not use that one to ‘inject’ a custom/configurable/user script into the <code>mudraw</code> when using that one to ‘textify’ the PDF and thus hook our script in between the text extraction process and the tesseract-based OCR happening in there, when <code>mupdf/mudraw</code> decides OCR is necessary for any page in the document being processed.</p>
<p>While this and similar thoughts have been mulling in my head the last few months, I could not identify what was <em>bothering</em> me about the whole idea: the ‘scripting’ bit is good, the need to have it sit between the page image render code in there and the call to tessearact to OCR that page is also obvious – including the notion that more complicated pages, e.g. scanned newspaper pages, might benefit from some (scripted) preprocessing of the rendered image into multiple ‘gangs’, i.e. multiple images, where each page represents a particular chunk (not necessarily rectangular, mind you!) of the page and then feed <em>that</em> to tesseract to improve OCR results. In other words: I want us to be able to customize the <strong>segmentation</strong> (and image processing, a.k.a. <strong>binarization</strong>) processes of the page to make the job easier and more obvious to the tooling already built into tesseract.</p>
<p>Yes, I expect to modify the base <code>mupdf</code> package quite a bit, but hope to do it in such a way that it is still easy to ‘track’ the mainline development of that package, so that I can track the work that continues to be done by Artifex for the forseeable future.</p>
<p>Meanwhile, something was nagging me. I felt the approach was somehow flawed but could not identify what was wrong with it.</p>
<p>A short time ago, I looked at the <code>mupdf</code> code again, <em>specifically</em> for the reason of identifying where I would need to inject the script calls, which parts I’ld want to present as an API to the script (which would, if I get my way, include additional APIs from OpenCV and maybe a few other spots where the latest binarization and segmentation research results have been published as source code). And I considered how I, or <em>anyone</em>, would be able to debug such user scripts. If it would be small stuff, a couple of classic debug-print statements would suffice, and then mujs would serve well.</p>
<p>However, when looking elsewhere for work other people have done with tesseract and OCR processes, I noted a couple of things that had me reconsider all this:</p>
<ul>
<li>the image preprocessing <em>can</em> span multiple stages.</li>
<li>I had neglected the important difference between <strong>segmentation</strong> of the image (identifying the columns and blocks of text, identifying (and then <em>ignoring</em>) any <em>images</em> on the page, etc.) and the <strong>binarization</strong> process (which takes the full-color input image, cleans it up (denoising, removing scan shadows, vignettes, etc.) and produces a as-clean-as-possible black-and-white image to feed to the OCR engine proper.)</li>
<li>tesseract 4.x / 5.x, which is now included in <code>mupdf</code>, thanks to work by Artifex, uses leptonica for this. I’m still somehat vague on the algorithm(s) used for <em>segmentation</em> in there, while the <em>binarization</em> uses some classic form of Otsu binarization.</li>
<li>that’s all great and dandy, but I have got plenty PDF inputs that won’t perform well with classic Otsu (some old books from 1900-1960 and some magazine scans from that era as well) and from what I’ve seen so far on the Internet, the segmentation can use some help as well if you’re processing non-mainstream whitepapers only. <strong>That’s what I wanted the <em>scripting</em> to help us out with!</strong></li>
<li>The scripting idea is still good, but the processes are more complex than I initially thought.</li>
<li>So the debugging experience would greatly improve if we could see the images for each intermediate stage in the process, particularly when we are setting up the process and then tweaking (parameterizing) it. Still mujs was viable; I would need to code a slightly more advanced debug-print statement then to show us the current image.</li>
<li>Another thing tried to reach my frontal lobe time and again: <em>most</em> of this work <em>out there</em> is done in Python. Using PyTesseract and whatever they get their hands on. Mostly that’s OpenCV and some <code>numpty</code> work.</li>
</ul>
<p>So I started to change my mind:</p>
<ul>
<li>
<p>I know I’m not fond of Python, but given the surmounting “peer pressure” in this area… let’s face it!</p>
</li>
<li>
<p>This was helped by me running user scenarios in my head – which is a thing that I do often: I can then “see” the software at work and observe how I, as a user, would peruse a system, even when it doesn’t exist yet. I can spot difficulties and possible different behaviours I would not consider otherwise, when creating the code as an architect/programmer. The major outcome of those scenario running sessions is this: I (the Qiqqa developer) would end up with a <em>ton</em> of requests to port any python script out there that does some OCR “magic” to mujs JavaScript (<code>mujs</code> only supports ES5, not ES6/ES2017, so that’s another drawback by now in 2021AD) for the transformation of such scripts is non-trivial, even when the user has medium level programming abilities. Ergo: maintenance hell awaits and no more time left to work on moving Qiqqa forward.</p>
<p>Unless I bow to this “peer pressure” and forget about using JavaScript and use Python instead.</p>
</li>
</ul>
<p>Is that what was nagging in my hind-brain all the time. <strong>No.</strong> It’s a small part, but we’re not there yet.</p>
<p>I looked at CPython and what needs to be done, roughly, to get it embedded into <code>mupdf</code> and allow python scripts to run to do the OCR and custom preprocessing work once a PDF page image is rendered by <code>mudraw</code>. That is: Python wouldn’t be driving <code>mupdf</code>, it would be the other way around. That’s what “embedding” an interpreter like that means.</p>
<p>This took a couple of days, but then it hit me what was bothering me so much, also while I had resolved to (having to) use Python for this instead of mujs or node-based JavaScript.</p>
<p>Given the complexities around the text extraction work, <strong>including the cover page filtering</strong> I want in there, it’s <em>convoluted</em> to drive everything from the starting point of a ‘regular’ <code>mudraw</code> text extraction run: <em>yes, it is text extraction that we are working on</em>, but I probably need scripting abilities <em>before</em> I render the page image! What about AI-based libs that can help “recognize” cover pages and assist us in theen choosing different extraction processes: some cover pages contain useful metadata, so it’s not just recognize-and-ignore/discard there.</p>
<p>This means that I would probably be best served if I drive <code>mupdf</code> text extraction from a script already, so that would mean python calling <code>mupdf</code> tooling (<code>mudraw</code> et al; the other tools in there can extract metadata, such as annotations and title/publisher metadata, if available, from the PDF!), but I still need that scripting to <em>also</em> sit in between each rendered page and the follow-up OCR process. <strong>That</strong> means I’ld be invoking python from <strong>inside</strong> the C API that gets invoked by the outer script. In other words the problem is this: can I have a python add-on, done in C/C++, which calls a <strong>python callback</strong> or anything of that nature, so that I can <em>stack</em> addon calls with python code <em>recusrively</em>? At minimum, it would invvolve <em>nesting</em> of the calls. How would a python debugger cope with such a stack trace, where some stuff in the middle of the call stack is native C code (mupdf/tesseract).</p>
<p>I have not seen that sort of thing mentioned <em>anywhere</em> yet, so that’s another technology viability test I’ll have to code to make sure that python can deliver on this, without me needing to completely overhaul the mupdf code!</p>
<p>This is also something that, to me at least, if conceptually of the next higher order in complexity and therefor <em>not</em> suitable to do with a single thing like <code>mujs</code>, which comes with fundamentally zero debugging facilities: all there is, <em>right now</em>, is a custom <code>print</code> function. Which serves well for simple scripts, like I said, but these nested calls, where scripts calls API function, which in turn calls script function(s) as callbacks, are a little more complex in my opinion (scenario: user who has <em>some</em> programmer ability) and thus require a more sophisticated debugging environment, or at least a debugging environment that can be improved by someone easily: python is far more “mainstream” than mujs, so chances are higher that somebody else already has done something that might help us there, or get us under way quicker and better.</p>
<p>The crux of this whole thing is thus:</p>
<ul>
<li>
<p>use python for PDF related scripting</p>
</li>
<li>
<p>see if we can get <code>mupdf</code> hooked in as an add-on; AFAICT there’s already some code so serve it as a pythong <em>plugin</em>.</p>
</li>
<li>
<p>make sure we can <em>nest</em> python, i.e. invoke python functions (“callbacks”) from inside the <code>mupdf</code> C code. This MAY have to happen multiple times per single outer call – think processing a pdf, getting at least one callback call for each rendered page, probably a couple more in order to help out filter/postprocess/do-whatever to the raw or semi-processed extracted text.</p>
<ul>
<li><em>bonus points</em>: analyze the extracted text and run a speelchecker over it and/or decide to <strong>re-execute</strong> the OCR phase with different settings, e.g. different language set or image filter settings, so we can seek out the optimal OCR results that way perhaps.</li>
</ul>
</li>
<li>
<p>this also puts our ‘internal webserver’ idea under new scrutiny: instead of using my old fav civetweb/mongoose for that (another chunk of C code), it might be possibly best to run the ‘webserver’ as a python script: after all, that would be the outer layer as we need scripts to drive the PDF processes anyway.</p>
</li>
<li>
<p>note that you are now adhering better to your own rule: use best-of-class solutions as much as possible: python and writing python addons in C/C++ is very mainstream, also in research environments, while coding with <code>mujs</code> is <em>not</em> mainstream at all. <code>node</code> <em>is</em> mainstream, but it would be a lot harder to integrate that JavaScript engine into <code>mupdf</code> the same way you could integrate <code>mujs</code>.</p>
<p>And from what I’ve read so far, embedding python (CPython) is done more often and is a clearly documented process, at least at first glance (when I looked into embedding python, etc.)</p>
</li>
</ul>

  </head>
  <body>

    <h1>scripting the OCR + text extraction process</h1>
<p>So I’ve been looking at getting mupdf (with built-in tesseract) for the new approach to PDF text extraction in Qiqqa.</p>
<p>Old (= Commercial) Qiqqa uses an antique (patched) version of mupdf AFAICT (<code>pdfdraw</code>) for text extraction only. OCR has been handled by tesseract 3.0.x, serving us in the form of a DLL.
New Qiqqa, I hope, will do all that, <em>plus</em> page rendering to image for the UI, via the latest <code>mupdf</code> versions, using the <code>mudraw</code>, <code>mutool</code> or possibly others from the <code>mupdf</code> package.</p>
<p>As I want to “open up” the OCR and text extraction process so I can give people the option of <em>editing</em> those processes to suit their need, for example when handling obnoxious “crypted” PDFs.  Some PDFs are scrambled to “protect” them against text select + copy/paste, for example!</p>
<p>I was considering scripting it with JavaScript and then came up with the thought that <code>mupdf</code> already includes <code>mujs</code>, a (basic) JavaScript engine used by <code>mupdf</code>, so why not use that one to ‘inject’ a custom/configurable/user script into the <code>mudraw</code> when using that one to ‘textify’ the PDF and thus hook our script in between the text extraction process and the tesseract-based OCR happening in there, when <code>mupdf/mudraw</code> decides OCR is necessary for any page in the document being processed.</p>
<p>While this and similar thoughts have been mulling in my head the last few months, I could not identify what was <em>bothering</em> me about the whole idea: the ‘scripting’ bit is good, the need to have it sit between the page image render code in there and the call to tessearact to OCR that page is also obvious – including the notion that more complicated pages, e.g. scanned newspaper pages, might benefit from some (scripted) preprocessing of the rendered image into multiple ‘gangs’, i.e. multiple images, where each page represents a particular chunk (not necessarily rectangular, mind you!) of the page and then feed <em>that</em> to tesseract to improve OCR results. In other words: I want us to be able to customize the <strong>segmentation</strong> (and image processing, a.k.a. <strong>binarization</strong>) processes of the page to make the job easier and more obvious to the tooling already built into tesseract.</p>
<p>Yes, I expect to modify the base <code>mupdf</code> package quite a bit, but hope to do it in such a way that it is still easy to ‘track’ the mainline development of that package, so that I can track the work that continues to be done by Artifex for the forseeable future.</p>
<p>Meanwhile, something was nagging me. I felt the approach was somehow flawed but could not identify what was wrong with it.</p>
<p>A short time ago, I looked at the <code>mupdf</code> code again, <em>specifically</em> for the reason of identifying where I would need to inject the script calls, which parts I’ld want to present as an API to the script (which would, if I get my way, include additional APIs from OpenCV and maybe a few other spots where the latest binarization and segmentation research results have been published as source code). And I considered how I, or <em>anyone</em>, would be able to debug such user scripts. If it would be small stuff, a couple of classic debug-print statements would suffice, and then mujs would serve well.</p>
<p>However, when looking elsewhere for work other people have done with tesseract and OCR processes, I noted a couple of things that had me reconsider all this:</p>
<ul>
<li>the image preprocessing <em>can</em> span multiple stages.</li>
<li>I had neglected the important difference between <strong>segmentation</strong> of the image (identifying the columns and blocks of text, identifying (and then <em>ignoring</em>) any <em>images</em> on the page, etc.) and the <strong>binarization</strong> process (which takes the full-color input image, cleans it up (denoising, removing scan shadows, vignettes, etc.) and produces a as-clean-as-possible black-and-white image to feed to the OCR engine proper.)</li>
<li>tesseract 4.x / 5.x, which is now included in <code>mupdf</code>, thanks to work by Artifex, uses leptonica for this. I’m still somehat vague on the algorithm(s) used for <em>segmentation</em> in there, while the <em>binarization</em> uses some classic form of Otsu binarization.</li>
<li>that’s all great and dandy, but I have got plenty PDF inputs that won’t perform well with classic Otsu (some old books from 1900-1960 and some magazine scans from that era as well) and from what I’ve seen so far on the Internet, the segmentation can use some help as well if you’re processing non-mainstream whitepapers only. <strong>That’s what I wanted the <em>scripting</em> to help us out with!</strong></li>
<li>The scripting idea is still good, but the processes are more complex than I initially thought.</li>
<li>So the debugging experience would greatly improve if we could see the images for each intermediate stage in the process, particularly when we are setting up the process and then tweaking (parameterizing) it. Still mujs was viable; I would need to code a slightly more advanced debug-print statement then to show us the current image.</li>
<li>Another thing tried to reach my frontal lobe time and again: <em>most</em> of this work <em>out there</em> is done in Python. Using PyTesseract and whatever they get their hands on. Mostly that’s OpenCV and some <code>numpty</code> work.</li>
</ul>
<p>So I started to change my mind:</p>
<ul>
<li>
<p>I know I’m not fond of Python, but given the surmounting “peer pressure” in this area… let’s face it!</p>
</li>
<li>
<p>This was helped by me running user scenarios in my head – which is a thing that I do often: I can then “see” the software at work and observe how I, as a user, would peruse a system, even when it doesn’t exist yet. I can spot difficulties and possible different behaviours I would not consider otherwise, when creating the code as an architect/programmer. The major outcome of those scenario running sessions is this: I (the Qiqqa developer) would end up with a <em>ton</em> of requests to port any python script out there that does some OCR “magic” to mujs JavaScript (<code>mujs</code> only supports ES5, not ES6/ES2017, so that’s another drawback by now in 2021AD) for the transformation of such scripts is non-trivial, even when the user has medium level programming abilities. Ergo: maintenance hell awaits and no more time left to work on moving Qiqqa forward.</p>
<p>Unless I bow to this “peer pressure” and forget about using JavaScript and use Python instead.</p>
</li>
</ul>
<p>Is that what was nagging in my hind-brain all the time. <strong>No.</strong> It’s a small part, but we’re not there yet.</p>
<p>I looked at CPython and what needs to be done, roughly, to get it embedded into <code>mupdf</code> and allow python scripts to run to do the OCR and custom preprocessing work once a PDF page image is rendered by <code>mudraw</code>. That is: Python wouldn’t be driving <code>mupdf</code>, it would be the other way around. That’s what “embedding” an interpreter like that means.</p>
<p>This took a couple of days, but then it hit me what was bothering me so much, also while I had resolved to (having to) use Python for this instead of mujs or node-based JavaScript.</p>
<p>Given the complexities around the text extraction work, <strong>including the cover page filtering</strong> I want in there, it’s <em>convoluted</em> to drive everything from the starting point of a ‘regular’ <code>mudraw</code> text extraction run: <em>yes, it is text extraction that we are working on</em>, but I probably need scripting abilities <em>before</em> I render the page image! What about AI-based libs that can help “recognize” cover pages and assist us in theen choosing different extraction processes: some cover pages contain useful metadata, so it’s not just recognize-and-ignore/discard there.</p>
<p>This means that I would probably be best served if I drive <code>mupdf</code> text extraction from a script already, so that would mean python calling <code>mupdf</code> tooling (<code>mudraw</code> et al; the other tools in there can extract metadata, such as annotations and title/publisher metadata, if available, from the PDF!), but I still need that scripting to <em>also</em> sit in between each rendered page and the follow-up OCR process. <strong>That</strong> means I’ld be invoking python from <strong>inside</strong> the C API that gets invoked by the outer script. In other words the problem is this: can I have a python add-on, done in C/C++, which calls a <strong>python callback</strong> or anything of that nature, so that I can <em>stack</em> addon calls with python code <em>recusrively</em>? At minimum, it would invvolve <em>nesting</em> of the calls. How would a python debugger cope with such a stack trace, where some stuff in the middle of the call stack is native C code (mupdf/tesseract).</p>
<p>I have not seen that sort of thing mentioned <em>anywhere</em> yet, so that’s another technology viability test I’ll have to code to make sure that python can deliver on this, without me needing to completely overhaul the mupdf code!</p>
<p>This is also something that, to me at least, if conceptually of the next higher order in complexity and therefor <em>not</em> suitable to do with a single thing like <code>mujs</code>, which comes with fundamentally zero debugging facilities: all there is, <em>right now</em>, is a custom <code>print</code> function. Which serves well for simple scripts, like I said, but these nested calls, where scripts calls API function, which in turn calls script function(s) as callbacks, are a little more complex in my opinion (scenario: user who has <em>some</em> programmer ability) and thus require a more sophisticated debugging environment, or at least a debugging environment that can be improved by someone easily: python is far more “mainstream” than mujs, so chances are higher that somebody else already has done something that might help us there, or get us under way quicker and better.</p>
<p>The crux of this whole thing is thus:</p>
<ul>
<li>
<p>use python for PDF related scripting</p>
</li>
<li>
<p>see if we can get <code>mupdf</code> hooked in as an add-on; AFAICT there’s already some code so serve it as a pythong <em>plugin</em>.</p>
</li>
<li>
<p>make sure we can <em>nest</em> python, i.e. invoke python functions (“callbacks”) from inside the <code>mupdf</code> C code. This MAY have to happen multiple times per single outer call – think processing a pdf, getting at least one callback call for each rendered page, probably a couple more in order to help out filter/postprocess/do-whatever to the raw or semi-processed extracted text.</p>
<ul>
<li><em>bonus points</em>: analyze the extracted text and run a speelchecker over it and/or decide to <strong>re-execute</strong> the OCR phase with different settings, e.g. different language set or image filter settings, so we can seek out the optimal OCR results that way perhaps.</li>
</ul>
</li>
<li>
<p>this also puts our ‘internal webserver’ idea under new scrutiny: instead of using my old fav civetweb/mongoose for that (another chunk of C code), it might be possibly best to run the ‘webserver’ as a python script: after all, that would be the outer layer as we need scripts to drive the PDF processes anyway.</p>
</li>
<li>
<p>note that you are now adhering better to your own rule: use best-of-class solutions as much as possible: python and writing python addons in C/C++ is very mainstream, also in research environments, while coding with <code>mujs</code> is <em>not</em> mainstream at all. <code>node</code> <em>is</em> mainstream, but it would be a lot harder to integrate that JavaScript engine into <code>mupdf</code> the same way you could integrate <code>mujs</code>.</p>
<p>And from what I’ve read so far, embedding python (CPython) is done more often and is a clearly documented process, at least at first glance (when I looked into embedding python, etc.)</p>
</li>
</ul>


    <footer>
      © 2020 Qiqqa Contributors ::
      <a href="https://github.com/GerHobbelt/qiqqa-open-source/blob/docs-src/Progress in Development/scripting the OCR + text extraction process.md">Edit this page on GitHub</a>
    </footer>
  </body>
</html>
