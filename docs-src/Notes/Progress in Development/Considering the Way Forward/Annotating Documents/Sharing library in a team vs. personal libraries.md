# Sharing library in a team vs. personal libraries

This came up while we were pondering what to sync:

old Qiqqa syncs the library bibtex data and PDF documents.

**However**, OCR output comes at a significant cost and the initial question there was: wouldn't it be good to sync the OCR data (text extracts) as well so only one machine has to spend the effort of OCR-ing document X. (Future music: share the OCR/text-extract load)

That led us to: *iff* we share/sync OCR data, we would also have to share the *edits/revisions* to that -- at the time of writing Qiqqa has no support for this **yet**, but we surely want to be able to hand-edit /correct OCR output: tesseract et al aren't perfect. Thus then the question becomes: what if we work in teams?

Those hand-edits should be shared with the team, as would the OCR text extracts. 

Then the other parts are the annotations: **do we want those to be tagged by the user who made/edited them?** (This would require the re-introduction of user ID in Qiqqa.)

## Design Decision

As I find those *'who dunnit'* tags to annotations in Microsoft Office (MSWord: *Track Edits*) quite annoying. Those are possibly useful in a team setting, but I don't believe Qiqqa would benefit from this: this is a library, not a tool to track the development of a paper to be published, like you would in MSWord or similar tools. Hence my conclusion: **Qiqqa will not track user IDs for edits**: a shared library is a shared responsibility.

> Of course, reality will surely try to catch up with me there: user ACL management, etc. because *someone* deleted an important document, and other daily excuses to turn a team into a hierarchy. I still don't think you would benefit from all that cruft (and the added cost of having to set it up and manage it for a team!) for a Qiqqa library: if you want to keep an "*inner circle*", that's fine: share two (or more) libraries, where the 'inner circle' one will carry content that's not available in the 'outer group' shared lib, etc.
> 
> This, of course, makes easy cloning and mix/merging documents+metadata across library boundaries a system requirement then.

If you don't want or like the annotations in the team library, you can always copy/clone that document + annotations and edit them to your preference in a separate, **personal library**. This goes for annotations and extracted content.

------

## Further thoughts

If folks edit or annotate documents, the idea was to import (and track) each revision of the document: this number can grow when edits happen over a longer period of time and/or in small increments, thus resulting in a large storage cost. Might we consider some sort of *delta compression* here, *iff* that's feasible at all, given the freakishness of the PDF format: a small edit may include a (hidden/non-obvious) restructuring of the binary file layout! Hmmmmmm, might not be bothered with it: if it gets too much, we should allow to easily erase "unimportant revisions", e.g. the ones between the first and the last -- I would personally keep the fist version as a marker of where we started. Besides, the first version is often the 'initial download copy' and relevant due to that fact alone: we should always keep an 'original source copy' (no matter how b0rked it may be). So I strongly prefer to keep at least the first and last revisions at all times, even forbidding deleting those when the document isn't nuked from orbit *entirely already*. 

Thus then the question becomes: what does the user feel is an "unimportant revision"? E.g.: small edits of the OCR extracted text indicate typo fixes: we always keep the latest, thus that would imply we kill the older revision then. Another example: 'unimportant' would be adding and annotation when the document (or page?) already has annotations; meanwhile we have the heuristic that adding an annotation where there were *none* before should be considered an *important* edit: then, when we nuke unimportant edits, we consequently end up with the last unannotated revision, followed by the latest fully annotated revision: all the intermediate steps (revisions) are nuked due to the 'unimportant update' rule.

> *The above 'unimportant' vs. 'neutral' and 'important' edit heuristics are just an example idea now, mind you!*
