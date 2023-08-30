# `libelle` :: compare PDF page renders / images


In one of the bug report's comments over at the Artifex bugtracker, the Artifex response (by one of the developers, don't recall which one) said `mupdf clean`  et al are NOT meant to keep all PDF details intact, so some page renders will produce different output and MAY lack some parts compared to the original PDF.

As I intend to use mupdf to clean up and produce *consistently renderable* PDFs, this becomes an issue for me that must be checked. Hence the need for a (bulk) test tool which renders PDF pages "before and after" and then compares the page images on a pixel-by-pixel basis to see if anything change *significantly*: we SHOULD tolerate minor deviations due to (anti-)aliasing and such, so a pixel-perfect comparison, while important to have, is expected to *not suffice*. Which leads us to -- once again -- image similarity metrics and similarity ranking.

Another comparison to perform would be between a mupdf render of a page and, say, the browser output (pdf.js?)  and/or poppler, where the browser-render of a PDF page is the most important one as we expect the browser to render our PDFs exactly like we'ld do when using mupdf, so another bulk test is called for to test *and compare* these render engines. Comparing to poppler or other PDF render libraries is only interesting to find PDFs which render quite different by different render engines; we will be using the browser and mupdf *exclusively* so poppler is, in that way, far less important to compare against and spend effort on.

Name of the tool? `libelle` -- an excellent and very active insect (bug) hunter. 😁

