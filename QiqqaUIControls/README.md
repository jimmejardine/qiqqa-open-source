# Qiqqa WPF User Controls and Custom Controls

## Purpose

Collects the various WPF Controls which are developed for Qiqqa and Qiqqa Technology Tests.

Reckon with Separation Of Concerns: make sure no *application functionalities* become/remain part of the controls. That's what the Models are for in MVVM after all.

Note: a bit of an odd one there might be the BibTeX Editor Control: is it BibTeX specific enough to warrant the direct use of an (interruptable) BibTeX parser after all? Well, yes and no. That BibTeX parser is not merely useful for the control alone but also for bulk/batch imports, background tasks validating/fixing existing libraries, etc., so the bibTeX Editor should have an interface to such a beast only -- is my current thought. Time to find out how to do this properly in an MVVM architectural setting?


