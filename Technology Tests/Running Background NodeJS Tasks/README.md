# WPF Running Background NodeJS Tasks

## Purpose

To test the viability of using NodeJS to run scriptable background tasks, e.g. OCR (pre)processing of PDF documents.

When this works out well, we can allow for user-customized scripts to execute the required background tasks of Qiqqa that a savvy user might want to tweak / customize to their personal needs:

- PDF OCR/text extraction process
- PDF metadata inference (Qiqqa has some code built in to *derive* a Title, Author, Abstract(!), etc. from the OCR input when any overarching BibTeX metadata is absent. A user might want to fiddle with that. (I would, so I'd be able to customize that subprocess to properly extract metadata for electronic datasheet PDFs: that would save me a *lot* of hassle!)
- CSL-based citation output: currently Qiqqa runs that chunk of JavasScript via a XULrunner instance to enable producing selected-format citation for inclusion in MSWord and other editors.
- ...

This tester/demo should include these features:

- ability to run multiple NodeJS processes/instances in parallel. These will be independent.
- ability to share data with the NodeJS instance
  + both input **and** output
  + SHOULD be able to provide a pipe/stream-like communication channel where multiple read & write actions occur interspersed before the NodeJS process is finished.
  
    The purpose of this is that then the NodeJS process can fetch the required input data in a piece-wise fashion and likewise 'upload' (send) the produced resulting data piecemeal to the calling application (Qiqqa), so that we don't have to use huge buffers when a lot of data is involved (e.g. library scan/dump?) nor do we have to guess and wait until the process is done as that way process info can be made available to the user as well, so we have a good idea of the actual progress.
- ability to send and ABORT code to the background process: we should be able to abort long-running / costly background tasks when their results are no longer wanted, e.g. when documents, which have their metadata being processed, have been deleted, moved or renamed and the metadata processing is now 'obsolete'.



---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
