# DO NOT use GCC together with binaries produced by other tools or you'll be royally *fchked*!

Via some testing work done on a lib I spend several hours on some *fcked* ideology crap: https://gcc.gnu.org/bugzilla/show_bug.cgi?id=40838#c91

## TL;DR? The Moral?

1. Roll back to my old stance ^[which grew from very bad experiences over the past decades on various dev boxes and target hardware; there's more trouble out there than stack alignment alone :wink:]: 

1. <p style="font-variant: small-caps;">Make sure you compile the *entire application*, including its libraries, using a single compiler version. You <em>MAY</em> use the vendor-provided stdlib binaries, but trust nothing beyond that.</p>

  <p style="font-variant: small-caps;"><strong>Compile from source, all the way.</strong></p>



## The longer version:

If you've got the time and got good safe margins on your blood pressure or a very positive mindset towards high horses and technology politicking you'll encounter while under way, browse these & read the threads:

- https://gcc.gnu.org/bugzilla/show_bug.cgi?id=40838
- https://gcc.gnu.org/bugzilla/show_bug.cgi?id=40838
- https://gcc.gnu.org/bugzilla/show_bug.cgi?id=38496
- http://math-atlas.sourceforge.net/errata.html#gccCrazy (I feel the pain of the ATLAS maintainer.)

got there via a wild I-dont-know-how-Im-gonna-fix-this google chase following a totally weird WTF in my test binaries: 
http://math-atlas.sourceforge.net/errata.html#gccCrazy


## Relevance of this towards Qiqqa?

Been moving towards migrating the crucial bits of backend tech employed by the app, including the PDF renderer and OCR package (antiquated tesseract 3.x something). 

When you, like me, appreciate looking into viability of a new approach while you have *ideas* what the future should look like, user-visible functionality wise, you'll be 
investigating the lay of the land in OCR land, etc. And there you run into several libs used via Python -- if you forget or otherwise have reasons to give commercial solutions a pass^[in my case that would make the free Qiqqa tool very much **not free at all** if users have to pay license fees for extra software they need alongside to make the basic functionalities of the tool work] -- and binary distros of some libs that you can use. *However*, combining these is no sine cure if you want to forego the Python intermediate and link these libs together into a single application. OpenCV, etc. are involved here as OCR isn't an exact science and you need a bit of help to get your page images to OCR reasonably well when you've got a library to process 
that's not exactly "*mainstream recently published whitepapers only*". 

Next thing that happens is you ignore your own adage about compiling everything from scratch (heck, it's a viability test, after all!) and several hours later you discover you're getting cactussed, not by the old famous Windows function stackframes discrepancies, but by entirely different stuff altogether.
(I'm running latest MSVC here; the gcc pre-built work came from a foreign place.)

So viability tests cannot use precompiled binary **libraries** from anywhere. /Period./



