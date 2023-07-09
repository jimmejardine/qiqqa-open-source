# `bezoar` :: OCR and related document page prep

`bezoar` = Qiqqa OCR *diagnostics* tool: in order to provide an improved OCR experience, we need:
  1. to improve `tesseract` diagnostic output so as to produce a more easily understandable reporting / visualization of the OCR process itself and the detailed steps taken to arrive at the final result, e.g. better control over and display of *binarization*, *page segmentation* and *raw recognized text stream post-processing*.
  2. to enhance / augment the interface between `mupdf`-based PDF page image output and the `tesseract` engine itself: currently `mupdf`/Artifex doesn't offer a way to configure/tweak the `tesseract` engine they have integrated into their codebase.
  
  and, IFF possible:
  
  3. *hopefully* add the ability to manipulate these behaviours through user-provided *scripting* for customized behaviours for individual input files.

