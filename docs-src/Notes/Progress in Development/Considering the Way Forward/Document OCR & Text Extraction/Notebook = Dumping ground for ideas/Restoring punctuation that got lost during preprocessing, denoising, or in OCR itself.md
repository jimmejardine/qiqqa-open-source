# Restoring punctuation that got lost during preprocessing (denoising, etc.) or in OCR itself

Inspect papers with titles like "[Restoring punctuation and capitalization in transcribed speech](https://ieeexplore.ieee.org/abstract/document/4960690/)"; search phrases are:

- restoring punctuation
- punctuation prediction

Most of the stuff I see is concentrated around speech recognition / transcribing, but we can lend from that: OCR-ing punctuation (period, comma, colon, semicolon) is pretty hard as the preprocessing stage might already have discarded it as part of the overall (scanned) page noise.

The idea here is to *let go*, i.e. to *bother less* about punctuation, if any, in the *source image* making it through into the OCR engine itself, due to slightly overzealous noise removal or other factors, and *instead* focus on recovering/estimating the punctuation that *should have been*.




