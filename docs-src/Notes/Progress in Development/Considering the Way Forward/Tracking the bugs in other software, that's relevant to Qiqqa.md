# Tracking the bugs in other software, that's relevant to Qiqqa

- https://github.com/sumatrapdfreader/sumatrapdf/issues/2206

   This means using SumatraPDF right now for annotation editing *may nuke your PDF* --> this is **another important reason to store every PDF *revision* in our database** (instead of the original idea to extract the annotations and rewrite them upon request) -- we must reckon with other software also containing bugs and *wrecking* your library entry somehow (or at least *revision X* of it). Hence we **must** always keep the original PDF intact and **immutable**, whatever we decide to do to it, internally or externally!
- x
- x

