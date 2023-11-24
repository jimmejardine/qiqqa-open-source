# `bezoar` :: OCR and related document page prep

`bezoar` = Qiqqa OCR *diagnostics* tool: in order to provide an improved OCR experience, we need:
  1. to improve `tesseract` diagnostic output so as to produce a more easily understandable reporting / visualization of the OCR process itself and the detailed steps taken to arrive at the final result, e.g. better control over and display of *binarization*, *page segmentation* and *raw recognized text stream post-processing*.
  2. to enhance / augment the interface between `mupdf`-based PDF page image output and the `tesseract` engine itself: currently `mupdf`/Artifex doesn't offer a way to configure/tweak the `tesseract` engine they have integrated into their codebase.
  
  and, IFF possible:
  
  3. *hopefully* add the ability to manipulate these behaviours through user-provided *scripting* for customized behaviours for individual input files. This SHOULD include arbitrary *page image* preprocessing flows, inspired on the algorithms included in `unpaper` and `libprecog` (a.k.a. `PRLib`).
     
      The key to this (scriptable) flow is that image (pre)processing is just not a single forward movement, but should allow for a preprocessing *graph* to be set up so we can create masks, etc., which are then to be applied to later stages in hat preprocessing graph
      
  1. Also add the feature to tesseract to provide a separate image for the *segmentation phase* in that codebase and/or override that phase entirely by allowing external processes (or the preprocessor) to deliver a list of segments (*bboxes*) to be OCR-ed.
   
      We note this as we saw that the LSTM OCR engine accepts/expects color or greyscale "original image data", but the thresholding in tesseract, while okay, delivers a mask that's sometimes unsuitable for *segmentation*, while it is fine for OCR (old skool tess v3 style): for *segmentation* we want fattened characters, possibly even connected together, i.e. some subtle shrink+grow / opening&closing before we binarize and feed *that* binarized image to the segmentation logic.
      Meanwhile we feed *another*, *thinner*, binarized image to the old skool tesseract v3 engine. 
      *Plus* we use a *thicker* binarized image as a *denoise/background-removal mask*, applying it to the color/greyscale "original image" data, after which we normalize the result in order to nicely span the entire available *dynamic range* in colorspace (grey or RGB), so as to feed the LSTM engine the best possible grey/color pixels, without any disturbance by further-away background noise pixels, thanks to the applied mask.
      
      > **Our Reason To Want This**: we've observed some very strange LSTM outputs when feeding that engine *unmasked* greyscale image data, which carried JPEG-artifacts!
  





