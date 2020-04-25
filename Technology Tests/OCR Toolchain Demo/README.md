# WPF PDF OCR Toolchain Tester/Demo

## Purpose

To test how best to approach the OCR process (which is not just Tesseract, but the entire chain from source PDF to 'archival PDF' and/or additional content+metadata storage files.

This tester/demo should include these features:

- shows the stages in the OCR process for visual evaluation of progress.
- MAY have ways to make the process user-adjustable or -alterable.
  + this can range from mere parameterization of the standard process to full-fledged custom PDF OCR toolchain where users provide their own (commercial/in-house/...) OCR solutions and 'Qiqqa' in the right direction to make them cooperate. Think OmniPage / ABBYY / ReadIris / NameYourPoison batch PDF conversion and/or batch hOCR extraction (**do note** that mere text extraction is not satisfactory for the needs of Qiqqa!)
  + my preference is to have all this tweakable/scriptable stuff done in JavaScript, so executing a JavaScript script which does all the hard work & tough love sounds okay to me, but we'll have to see how that works out when a user actually wants to tweak the process, e.g. adding / adjusting page image preprocessing to cope with particular scan artifacts, etc.
- offers a way to cope with 'non-ideal scanned images', e.g. older page scans, 'Xeroxed book scans' (which often have huge variable borders and dark center edges), etc.
- MAY have ability to store this configuration/setup/parameterization with the document as additional metadata. 
  + This would permit automated re-runs of the toolchain without the need to tweak everything from scratch.
  + This MAY also be used as 'template' for other documents: when you have multiple old newpaper scans to process, 'copy/use template from document X' would be a nice-to-have workflow improvement
- has ability to produce:
  + hOCR or similar format which includes both content and placement
  + metadata and attachments extracted from the document file (PDF)
  + PDF/A with text overlay (or whats-the-spec-for-archival-quality-text-carrying PDFs?)
- ability to FORCE an image-based OCR text extraction process, even wehn the PDF reportedly has text embedded already. 
  + This comes in very handy for PDFs which have been (security)-scrambled or otherwise total gibberish when you follow the default route of text extraction *without* OCR.
- ability to extract content text without OCR when the PDF already provides embedded text. (Of course, hOCR is still required there as output, so it's **text + position** what we need every time.)
- MAY have a UI to manually crop / region-mark document pages. 
  + again, it might be worthwhile to have this user-added value stored in metadata stores, so we can reproduce the edit work done.

  + Tesseract technicality:

    Tess 3.x had an API where regions could be fed into the OCR engine but I have understood from the various documentation pages and forum discussions (StackOverflow et al) that these regions are treated as *advisory*. IIRC Tess 4.x and 5.x won't be bothered with these 'suggested regions from outer space' any more and I concur, given that Qiqqa was absolutely *lousy* in heuristically discovering those regions to OCR (there was nno user control in v82 and below).
    
    However, when a user wants to paint regions for OCR processing, I  believe the best way forward there would be to use a process where Tesseract MUST adhere to the indicated areas:
    
    1. paint the regions.
    2. loop through the regions one by one:
       a. white-out everything that's not in the current region
       b. have Tess OCR the region
       c. merge the hOCR output with what you already have for this page
       d. rinse & repeat until you've processed all regions in the page and thus, thanks to hOCR merging,  arrived at a total page OCR result.

    (Of course, "white-out" **assumes** any background color has already been adjusted to become white (and text black) before you feed these regions to Tesseract via that  'do one region at a time' loop described above.
    
    I haven't seen better approaches for this workflow where user-defined regions should prevale over OCR engine heuristics.
- can this process run in mutiple instance in parallel? Is that smart to even consider or can we load a multi-core machine sufficiently without parallellizing this? And how do the tools we'll be using cope with this 'running mutiple instances' goal of doing more OCR work faster?
- should this become a tool separate from Qiqqa itself, as suggested by Narayan @raindropsfromsky?
  + **do note** that this is sounding a lot like building a toolchain that is an alternative to PaperPort, OmniPage and other document scanning+management tool sets. I don't the "alternative to" but that implies it's a lot of work for a single guy right now. Hmmmmmmmmm...
  


---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
