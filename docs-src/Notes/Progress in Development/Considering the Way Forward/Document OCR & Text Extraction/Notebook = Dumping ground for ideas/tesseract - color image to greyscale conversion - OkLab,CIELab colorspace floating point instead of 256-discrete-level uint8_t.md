# `tesseract`: color image to greyscale conversion: OkLab/CIELab colorspace + floating point instead of 256-discrete-level `uint8_t`?

This has been bothering me for quite a bit longer than the rest around here: why don't we *improve* the information degree in our greyscale feature set (and improve our image color to greyscale conversion besides) by converting any color input to OkLab (or CIELAB2000) floating point greyscale image instead? 
That way we don't lose some of the perceptual data in the color image that do now as we convert to 8-bit greyscale and *thus* we should be able to feed the BLSTM engine a better *informed* feature set from the greyscale source.

Besides, I suppose it'll also improve binarization results re perceptiveness of the output: as we convert to a human-perception-type *colorspace* we're bound to get better conversion results when we compare the actual output with what one would expect to feed into the engine.


