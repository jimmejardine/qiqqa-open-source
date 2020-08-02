# WPF Rich BibTeX Editor, Validator & Corrector

## Purpose

Given the plethora of BibTeX 'styles' (macros, Unicode, various ways to TeX character constructions, etc.) I want to have a bibTeX editor that's both flexible and validating at the same time.

This requires:

- a semi-rich editor, which can work in both RAW and FORMATTED = Form Fields mode
  + where ideally hitting TAB on the last field automatically adds an extra form field key+value pair to continue writing metadata info
- with an interruptible BibTeX parser backend, which comes in handy when we need to validate/check/parse the RAW input
- ditto for the form-fielded input, where we might want to use a simpler BibTeX / metadata constructor as we are already entering the stuff in the appropriate fields.
  + still we need to *validate* there as keys and/or content can be **incorrectly formatted** and/or carrying illegal characters (e.g. spaces and punctuation in a 'key' field)
- ability to help 'clean up' the content:
  + Sentence capitalization (turning an all-caps input to proper looking result)
  + Human Names processing: Surname, First Name is the preferred format, but what about company names and when the BibTeX input we got off the internet doesn't adhere to this? Can we convert/adjust such items automatically?
    + this implies that the author field at least coul do with a more, ah, complicated form field approach, where we get a line for every author and he/she/it gets split up into Surname and First Name columns so we can check that all is well.
- ability to handle/keep **comments** in the bibTeX , even when we hand-edit the item in the for view. (Those comments can be important sometimes)
- ability to handle multiple consecutive BibTeX entries: some records may contain mutiple BibTeX records.
  + this is rare, but happens in my own libraries where I wasn't sure about a certain document's metadata. 

    I'm okay with it when the form view only **signals** this sort of thing, but such information MUST never be lost when one enters the form editor mode: current Qiqqa loses all comments and secundary records, etc. when we edit any little thing in the BibTeX form editor and that is Very Bad Form.
- OPTIONAL ability to process / translate other formats, such as RIS, into BibTeX. When we do this, the original input should be appended as a **comment**: **never loose your source material** is the adagium here!
- include extra BibTeX fields, e.g. source URI (which CAN have multiple entries when we have found the same document in multiple places; current Qiqqa code does not support this, but that's definitely a feature I want in. Think wikipedia's links, where the original is listed next to a WayBack Machine URI: that's 2 URIs for a single reference. Given the duplication in Scholar and elsewhere, I want this to be able to store *many* URIs. These URIs don't live forever and if you have found a duplicate, that one may still be reachable, so any bibTeX user of this Qiqqa info can then test the URIs and do like Wikipedia and list the working one next to the 'original source (404)' one, for example.
- all validation, parsing, processing, yadayadayada MUST be interruptible and running in the background, so we have an optimum editing performance experience.

    


---

## Motto

This here is part of the technical storyboarding side of a UI & UX overhaul of Qiqqa.

Before we put it to Qiqqa, it will be tested here.
