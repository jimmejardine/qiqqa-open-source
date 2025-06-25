# Megalo...? ... To go where no-one dared go before: a new OCR + PDF/text processing facility!



**TBD**




One idea: run an "adaptive" OCR process cycle where the page image preprocess (color-grading, thresholding, …) gets tuned by the quality metrics produced by the follow-up OCR phase: tesseract produces text + statistics AND we can add our own "sensible text" statistics afterwards, all of which combine in a quality indicator for this specific round. Then fiddle the preprocessing parameters a little and see if the quality metric improves. When it does, try settings in the neighbourhood. Keep doing this until either your time budget runs out or you reach an OCR output optimum.

Of course, this sounds a lot like running the entire pre+OCR+post process inside a Monte Carlo simulation and that's exactly what this is about: give the machine extra time (and space) to help you find OCR optima for your particular input images.

The next idea there is to apply this (costly) tuning process to a sample set only -- assuming you're feeding the OCR engine a batch of scans from the same scan/book/environment session -- and then run the entire batch with the discovered optimal configuration parameters. 
Only re-tune when the quality metric drops noticeably for that would be an indicator of the current image series having ended: this way you can run large multi-sourced batches unattended, as the machine will detect and try to correct quality deterioration along the way.


