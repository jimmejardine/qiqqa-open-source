# Where to find papers

- https://core.ac.uk/
- ...


# Metadata / Attributes

- a paper can be found on multiple sites, hence MANY URLs as source.
  + some of these may be 'dead' by the time the reader of the reference (in your own paper/document, which references it), so it is paramount to have:
    - ability to store and ADD source URLs at any time
    - check the validity / availability of a URL at any time
      + Note here that some URLs behave *oddly* and a simple `curl`-style download test might not give the correct answer: see [[Testing: nasty URLs for PDFs]]
- metadata may be sourced from various locations, including
  + BibTeX sources (Scholar for example)
  + RIS/... sources
  + XML sources (MDPI for example)
  + Inferral from HTML page content (DOI, ArXiv, etc.)
  + ADS (Windows: some browsers store the source URL in an 'Alternative Stream' on NTFS: very useful for us to extract that metadata and include it in the document record as it automatically lists the URL where the file has been downloaded, irrespective of any file move/copy/rename actions in between the time it was downloaded and the moment it was imported into Qiqqa.)

  The question here is: do we wish to store/track the *imported* metadata next to the final mixed output?

  My answer would be YES, as I like to see what came from where, like in a VCS (git, ...), when you investigate the correctness of metadata: a task which occurs too often to ignore IMO.
- Annotations are a feature in high demand. Qiqqa has annotations, but then there's a plethora of annotation import/export/enhancements that it currently DOES NOT have (Oct 2020)
- papers can have MANY
  - duplicates (exact copies)

    Qiqqa prevents this by content-hash matching them, so that would result in multiple import source URIs for a single paper
  - near duplicates Type S ('Sources')

    These have *very probably* the exact same content, but with different *leader pages*. I've found this with
    - researchgate website
    - some Korean universities which prepend their own leader page on everything
    - electronic datasheets

      Where it gets worse, by the way, is when a company is bought by another and you happen to find the document published in the few years afterwards, i.e. in their transitional period. Then you can easily end up with TWO OR MORE leader pages before you hit the actual content. On-Semi/Fairchild is one such example.

      Another occasion where you may observe this is when you get a datasheet off a generic datasheet finder/provider website or *component seller*, which appends their own, often oddly sized, *additional* leader pages.

  - near duplicates Type T ('Typesetting')

    Really another Type S, but you'll find this quite often with some older papers (typewriter format and magazine preprint of the same paper: Courier vs. proportional serif font layout) and older datasheets (Toshiba, RCA)

    This occurs most often with datasheets when you happen to find the same datasheet for multiple publication years (electron tube manuals, etc.)

  - near duplicates Type C ('Content')

    This is more relevant for scientific papers, where you may happen to find all or some of this set:

    - preprint
    - personal author copy (which may be different for different authors when it's a co-authored paper)
    - MULTIPLE magazine prints (though they want exclusivity, some papers have been published in multiple publications, e.g. magazine, symposium/summit syllabi, reprint in a later year for historical papers)
    - abridged versions for popular press or special magazine issues and/or press releases
    - corrected versions which have been updated and reprinted after processing of critique
    - versions marked **RETRACTED**: this is really a kind of Type T and are rare as most papers are VERY SILENTLY retracted: you need to spend quite some effort to make sure (Retraction Watch site, ...). There's some notorious stuff in the social-psychology corner that got retracted after it finally came out the world-renowned Dutch researcher had invented his "meat eaters are more anti-social than vegetarians" data wholesale, for example. See (this New York Times article)[https://www.nytimes.com/2013/04/28/magazine/diederik-stapels-audacious-academic-fraud.html]. There's a lot more of that: check Retraction Watch site.
    - some papers happen to be **plagiarized** in other journals by other authors. Sometimes this is hard to detect, sometimes it's blatantly obvious. I've seen some Indian journals publish stuff with Indian authors listed, which can also be found with different journals and different authors (Chinese, American, etc.), but this is not 'an Indian thing': it's just that I happen to encounter it the first few times with PDFs from more or less 'obscure' Indian journals.
    - a more difficult to categorize variant of a paper is one where the same dataset and part of the text is re-used by the same authors to look at maybe a different aspect in that research or something else. I've encountered a few like that: these are not really *duplicates* but there the papers have a *major overlap*.

  I guess it is, like so much else in life, a continuous gradient from black to white when it comes to *duplicates*: the question to ask when reading a new paper instance is "how much of a *duplicate* are you then?"

- people like to refer to papers with a shorthand reference, which should be *unique on a per-publication bases* at least.

  What I mean here is that (BibTeX) metadata always comes with a reference shorthand, e.g. `[Knuth78]`, but folks have preferences, so this may not always suite your taste or the incoming metadata reference code may look contrived or senseless/random, e.g. `[Bur7X1q2]` and people will want to be able to apply their own 'naming scheme' for this.

  The key here is that this is usually done on a *per-library* basis, where the generated shorthand should thusly be unique across the entire *library*, where the references *really* only need to be unique in your own production, i.e. in the paper/document/book you are writing yourself, in which you drop these articles as references!

  Here one could go and create a Qiqqa library for every project, clone/link documents into that project library from your main collection library (currently Qiqqa only supports COPY or MOVE, but not LINK/REFERENCE, by the way) and then create/generate a unique reference label set for that reduced collection and use it in your paper. Or you could create a Project and have Project-specific metadata alterations/augmentations, like the reference shorthand code, kept as a separate 'overlay'? (Qiqqa currently does not have any such 'library metadata overlay/layering' facility: this is just us thinking about what should be / could be for optimal usability.)

- people also have requested the 'sane filename and/or keep my personal file organization' as a feature over the years. (#205, ...)

  While Qiqqa can create a **library export** where every PDF paper gets a sane name (author, title) asnd is referenced by a HTML index sheet, this is not really the same thing.

  The question here is: can we give people access to the Qiqqa library files via 'sane names' (which, again, are a matter of *taste*!) while the Qiqqa machinery can keep track of those PDFs and everything else using the content hash based scheme it already uses? (Content hash based VCS like `git` work alike, but the goal/purpose is different, hence you always have a 'working copy' of a file in `git` versus the archived versions, which are stored on disk in a git-specific database structure. Qiqqa does not need a 'working copy' (more on multiple copies of the same document later, though) as it's basically an *archival* system: it's your local librarian, so any 'working copy' is not managed by Qiqqa and is *not meant to be managed by Qiqqa* as that 'working copy' would not be a ready (published or unpublished) paper yet!)

  I believe the technology answer there is using hardlinks, which are available in NTFS / Windows and on all UNIX filesystems: Qiqqa can maybe keep a master file in the repository, using the standard content hash based naming scheme, while it may keep hardlinks or softlinks to those 'sensibly named' imported source files and/or 'sensibly named' copies which the user wishes to see and use. As long as it all stays on a single drive in Windows (NTFS), you're golden with hardlinks, but it gets tricky and bothersome when you move those copies onto a different drive... There's some stuff to consider there.

- I have found many PDFs which needed processing to make them display or otherwise work correctly (OCR!) in Qiqqa.

  While I use tools like `qpdf` and `mupdf` for some of these tasks, the important subject here is: *how do we keep track of the various file versions/copies* here?

  I suppose this is another type of *duplicate* which we will dub *Type V* ('View\[ability]') for now: a single imported PDF would become, after all the processing, a series of semi-duplicates:

  - the original, imported, PDF file
  - the unlocked PDF file (`qpdf`) which will render and print properly at least
  - the OCR'ed PDF file which has gained a text layer if the original didn't have one (or has an incomplete or considered-wrong-by-the-user one)
  - the PDF file which has everything above plus embedded annotations perhaps? Or attachments? -- Do we really want this variant in the library or do we wish to generate it on the fly every time a user requests it?

  As these variants all will have quite different content hashes, there's the additional task for Qiqqa to link up multiple content hashes as 'technical duplicates' of the paper. Hence my take on this as calling this another type of (semi-)duplicate: all those semi-duplicates discussed above suffer from the same ailment: different content hashes for the same base content due to slew of different reasons.

  Qiqqa currently (Oct 2020) only supports "one document, one content hash", but this must be turned into something where multiple PDF files can be linked up and 'clustered' into a single overarching item, while the different *files* *will* have slightly different metadata in some places, e.g. their 'state' or 'type' or whatever you wish to call it: 

  - is this the imported original?
  - is this a copy of X, which has had superfluous leader pages stripped off?
  - is this a copy of X, which has been unlocked for regular viewing and printing?
  - is this a copy of X, which has a searchable text layer?
    + has this text layer been vetted by the user as 'good'/'use this!'? 

      e.g. some (older) TeX output PDFs have a non-Unicode character encoding which does not convert to **intelligible** searchable text: it is quite useful to have these carry an additional or maybe altered text layer which can be extracted to produce legible text easily. 

      Some TeX files also seem to have lost all their whitespace in the text extract, so some more work needs to be done on the tools anyway and an additional text layer might help.

    + is this a text layer in a language that you can read / *comprehend*?

      Then there are the 'foreign' publications, e.g. Chinese papers in my particular case, which require **language translation** to become intelligible to me.
      Once again, an additional text layer might come in handy to show me a translated overlay of the content. When this translation is stored in the PDF, we end up with a *technically different content* and thus a different file with a different content hash.

    And leading up to that, there's:

    + has this text layer been vetted/rated by the automation in Qiqqa? (Automatic rating / TOC building / ...whatever...)
    + has this text layer been (auto-)translated by the automation in Qiqqa to your language(s) of choice?

    As the text layer would quite probably be obtained through the OCR process, we may also want to store/track the OCR and OCR preprocessing setup which resulted in this text layer: what have we done with the original to produce this?
    
    Though one might want to store that sort of info in a separate process configuration file or database record, instead of including it *inside* the PDF; the resulting text layer *will* be part of the new PDF anyhow!




