# Writing documentation :: which tools and pipeline to use?

I've looked at (and *in*) the usual suspects several times across the years and a lot has changed while not a lot has changed, if at all, at the same time, when it comes to tooling:

- **doxygen** is still, by a long mile, the go-to tool for API and miscellaneous documentation extraction from your source code, when you're coding C/C++ and similar languages -- the exception being C# when you use Microsoft Visual Studio, which I do.
  
  Alas, Qiqqa is to be multi-platform and anno 2024 A.D. the state of .NET outside the Microsoft realm is still crap from my perspective: it builds fine but then running the stuff on arbitrary Linux machines owned by end-users? 
  I wouldn't want to visit the support costs upon anyone for .NET-based software running on any Linux flavor: it works, and then it does something... a little *odd*. And good luck diagnosing. While I really like C# the environment is forever soaked in the typical Microsoftness: the days of Gates, Ballmer c.s. are long gone, but the userbase (third party developers) hasn't really changed enough in my experience: check problems vs. answers/fixes/solutions pumped up from the earth by google, what do you get? Run a .NET application on any user's Linux desktop and the end user seeks assistance... can you say: "support contract"? Exactly, not an option when such an application is to be Open Source & lone wolf / small team, such as Qiqqa. I don't want to ride that "bleeding edge", thank you very much.
  
  So we stick with *old skool* C/C++ backend and the rest of major effort in JavaScript/TypeScript, because that's what I'm comfortable with in a multi-platform environment and has more potential of finding collaborators in the short & long run: the Open Source Developers' pool for those languages (and on *diverse platforms*) is practically larger for each.
 
- **sphinx-doc** is the long-lived go-to tool for more advanced documentation, is what I observe. Plenty contenders, some very nice ones (MkDocs, docusaurus, etc.) but here my wish list kicks in and narrows the field *considerably*: 
  1. the *everybody option*: **website/HTML**. Easy: everyone does *that*.
  2. **PDF/downloadable documentation files**: *nuh-uh!* Flaky, cumbersome, dubious, those are the words that come to mind when you look further afield. sphinx-doc is used by some large projects for producing PDF format and it's not exactly hassle-free, but at least *sounds like it achievable as somebody else already covered the most major bugs*.
  3. (far future music!) *documentation in your own language* a.k.a. i18n / internationalisation --> translated documentation! Okay, but... not always obvious. This one restricts the choice to sphinx, pretty much. Alas, the people who have this working *today* in a way that I appreciate all **end at sphinx-doc as their last stage in the documentation rendering process**, while my documentation *vision* (ahem) might maybe better fit a sphinx-then-doxygen pipeline if I am any judge, but the available tooling *out there* all takes the more obvious, inverted route: doxygen-then-sphinx, possibly via `breathe` intermission. 
     Oh, almost forgot to mention: I wish to have the translation done via Open Source tooling and no TaaS (T=Translation) websites involved as those are paywalled and come & go or survive and suck the budget dry at increasing rate. That requirement gently guides us towards the common denominator in free labor a/k/a/ Open Source county: POT/PO/MO files' processing, hence GNU translate tooling, such as `poeditor` c.s..

  There you have it. Most others, at least the ones using different tools & pipelines, do not have at least *one* of the above three. *And I want it all.* So I guess that means I'll have find out if the pipeline set up by others: doxygen-breathe-sphinx, can do what *I* want to produce in the end? I'm not sure, but one thing I am very sure of by now: others may make similar marketing noises, but from the various trawls and experiments I did over time, this is the only pipeline that *delivers* for a large subset of people, while the above 3 requirements: HTML, PDF, i18n (with minimum/low hassle translations done) force my hand towards the main of the mainstream: Sphinx as the internationalisation basekit, since it can drive POT/PO files: see how the folks at Krita are dealing with this, for example.
  

## Conclusion: Preferred tooling (apparently...)

- [doxygen](https://www.doxygen.nl/index.html)
- [doxysphinx](https://github.com/boschglobal/doxysphinx) or maybe [breathe](https://breathe.readthedocs.io/en/latest/), though it sounds [like doxysphinx is more up my alley after all...](https://boschglobal.github.io/doxysphinx/docs/alternatives.html#doxysphinx-vs-breathe-vs-exhale)
- [sphinx-doc](https://www.sphinx-doc.org/en/master/index.html)
- [LaTeX](https://www.latex-project.org/get/) (MikTeX, TeTeX or other TeX processor...)
- [MathJAX](https://www.mathjax.org/)? Can we do MathJAX for the formulas online/HTML? Or is that better done by [MathJAX *server-side*](https://docs.mathjax.org/en/latest/server/start.html)?!

Quoting the doxysphinx docu (bold emphasis mine):

> The tools [Breathe](https://breathe.readthedocs.io/en/latest/) and [Exhale](https://exhale.readthedocs.io/en/latest/index.html) needs special mention, as doxysphinx was invented in **a large C++ project (> 11,000 C++ files)** where we started out with these two tools. With the large project size, Exhale did not perform too well and Breathe did not quite support all C++ and Doxygen features that C++ developers expected. **Doxysphinx was invented to overcome these limitations.**

> Breathe is useful for smaller C++ projects when parts of C++ Doxygen documentation needs to be integrated into the Sphinx documentation using [Breathe directives](https://breathe.readthedocs.io/en/latest/directives.html). \[...\]

> Doxysphinx outperforms the two options w.r.t. speed and features as it simply re-uses Doxygen output.
>
> Also note that Breathe and Doxysphinx can co-exist in the same project.

All right. That settles it: we'll go with doxysphinx, y'all!



## TODO / Unsolved / Which remains...

### monolith / merging doxygen 'sites'

Now that leaves us with the conundrum what to do / how to deal with integrating the API + misc documentation from the major libraries which we use: I hope to bundle those so it's one integrated whole, similar to the *monolith build* of the software itself: the documentation should ride with the software: thus bundled the same way, monolithic style.
See the references section below for several SO links, etc. which address this issue.

### alternative PDF production?

Another detail is the PDF file production: instead of using classic sphinx->LaTeX->PDF there also is a "simple PDF production" option using a HTML to PDF translator path via [sphinx-simplepdf](https://github.com/useblocks/sphinx-simplepdf)-->[weasyprint](https://github.com/Kozea/WeasyPrint/) which takes printer/weasy-specific CSS to help produce the PDF output.

### Open Source License investigation, reporting, documenting

For the licenses list we can re-use the tooling that Bosch is using too, or so it seems (I got it off their repo list): [the Software License ScanCode Toolkit](https://github.com/nexB/scancode-toolkit), quoting:

> A typical software project often reuses hundreds of third-party packages. License and origin information is not always easy to find and not normalized: ScanCode discovers and normalizes this data for you.
>




## References

- https://www.doxygen.nl/manual/formulas.html : Including formulas
- https://docs.mathjax.org/en/latest/server/start.html
- https://github.com/boschglobal/doxysphinx
- https://boschglobal.github.io/doxysphinx/docs/alternatives.html
- https://github.com/vovkos/doxyrest
- https://github.com/svenevs/exhale / https://exhale.readthedocs.io/en/latest/
- https://github.com/breathe-doc/breathe / https://www.breathe-doc.org/
- https://breathe.readthedocs.io/en/latest/
- https://github.com/useblocks/sphinx-simplepdf
- https://sphinx-simplepdf.readthedocs.io/en/latest/tech_details.html
- https://github.com/Kozea/WeasyPrint
- Sphinx-doc themes:
   - https://github.com/executablebooks/sphinx-book-theme
   - https://github.com/readthedocs/sphinx_rtd_theme + https://github.com/pytorch/pytorch_sphinx_theme + https://github.com/audeering/sphinx-audeering-theme
   - https://github.com/sphinx-themes/sphinx-themes.org / https://sphinx-themes.org/
   - https://github.com/danirus/sphinx-nefertiti
   - https://github.com/introt/alabester + https://github.com/sphinx-doc/alabaster
   - https://github.com/executablebooks/sphinx-book-theme
   - https://ukgovdatascience.github.io/govuk-tech-docs-sphinx-theme/#gov-uk-tech-docs-sphinx-theme
   - https://github.com/openforcefield/openff-sphinx-theme
   - https://github.com/saeiddrv/SphinxMinooTheme
   - https://github.com/pradyunsg/sphinx-basic-ng
   - https://github.com/pydata/pydata-sphinx-theme
   - https://github.com/pradyunsg/furo
   - https://github.com/kai687/sphinxawesome-theme
   - https://github.com/lepture/shibuya
   - https://github.com/Bernardo-MG/sphinx-docs-theme
   - https://github.com/PennyLaneAI/pennylane-sphinx-theme
- https://github.com/useblocks/awesomesphinx
- https://tomasfarias.dev/articles/sphinx-docs-with-poetry-and-github-pages/ -- automate a Sphinx documentation deployment pipeline, by hosting it in Github Pages
- https://www.writethedocs.org/guide/writing/beginners-guide-to-docs/
- https://www.writethedocs.org/guide/tools/testing/?highlight=sphinx -- sphinx::`make lintcheck` check your generated website's pages
- https://www.writethedocs.org/guide/tools/sphinx-themes/?highlight=sphinx
- https://docs.openedx.org/en/latest/developers/how-tos/switch-to-the-sphinx-book-theme.html
- https://github.com/fossology/fossology -- FOSSology is an open source license compliance software system and toolkit. As a toolkit you can run license, copyright and export control scans from the command line. As a system, a database and web ui are provided to give you a compliance workflow. License, copyright and export scanners are tools used in the workflow.
- https://brunowu.github.io/doxygen_sphinx_example/build.html
- https://medium.com/practical-coding/c-documentation-with-doxygen-cmake-sphinx-breathe-for-those-of-use-who-are-totally-lost-7d555386fe13 + https://medium.com/practical-coding/c-documentation-with-doxygen-cmake-sphinx-breathe-for-those-of-use-who-are-totally-lost-part-2-21f4fb1abd9f + https://medium.com/practical-coding/c-documentation-with-doxygen-cmake-sphinx-breathe-for-those-of-use-who-are-totally-lost-part-3-d20549d3b01f
- https://betterscientificsoftware.github.io/python-for-hpc/tutorials/python-doc-sphinx/
- https://cta-redmine.irap.omp.eu/issues/1068
- https://stackoverflow.com/questions/11570069/merging-doxygen-modules
- https://stackoverflow.com/questions/8247189/doxygen-is-slow/8247993#8247993
- https://stackoverflow.com/questions/57977278/doxygen-create-a-top-level-project-that-indexes-links-to-sub-projects?noredirect=1&lq=1
- example of merged doxygen sites: https://doc.cgal.org/latest/Manual/packages.html / https://github.com/CGAL/cgal
- https://stackoverflow.com/questions/2908086/how-to-manage-a-doxygen-project-with-multiple-libraries?rq=3
- [the Software License ScanCode Toolkit](https://github.com/nexB/scancode-toolkit)
- https://www.doxygen.nl/manual/customize.html
- https://gdal.org/development/dev_documentation.html
- https://docutils.sourceforge.io/docs/ref/rst/restructuredtext.html -- reStructured Text specification/documentation.
- https://www.reddit.com/r/cpp/comments/bwf8bf/clear_functional_c_documentation_with_sphinx/ --> https://devblogs.microsoft.com/cppblog/clear-functional-c-documentation-with-sphinx-breathe-doxygen-cmake/
- https://docs.translatehouse.org/projects/translate-toolkit/en/latest/commands/html2po.html -- html2po, po2html: convert translatable items in HTML to the PO format. Insert translated text into HTML templates --> https://github.com/translate/translate -- useful localization tools with Python API for building localization & translation systems.
- https://github.com/translate/virtaal
- https://docs.translatehouse.org/projects/localization-guide/en/latest/
- https://github.com/crow-translate/crow-translate
- https://github.com/googleapis/google-cloud-php-translate
- https://www.gnu.org/server/standards/translations/po-how-to.html
- https://poedit.net/ -- Easy translation of apps & sites with **PO, XLIFF, JSON or Flutter** formats
- https://github.com/SekouD/potranslator
- https://stackoverflow.com/questions/61246851/how-to-make-translations-of-full-articles-from-po-files-in-sphinx-or-gettext, however look at: https://www.sphinx-doc.org/en/master/usage/advanced/intl.html for slightly more recent intel on this.
- https://www.doxygen.nl/manual/langhowto.html
- https://www.doxygen.nl/manual/output.html
- https://developer.blender.org/docs/handbook/translating/translator_guide/
- https://www.swig.org/Doc4.2/Doxygen.html#Doxygen
- https://stackoverflow.com/questions/4099572/how-to-get-a-single-pdf-document-from-doxygen
- https://stackoverflow.com/questions/67191067/how-to-use-doxygen-to-produce-a-pdf-with-custom-latex-stylesheet-and-commands
- https://stackoverflow.com/questions/64047891/customize-doxygen-output-for-pdf-latex
- https://stackoverflow.com/questions/20405524/doxygen-how-to-include-source-files-in-latex-output
- https://stackoverflow.com/questions/46295838/add-project-brief-in-doxygen-latex-output
- https://github.com/doxygen/doxygen/issues/9159
- https://tex.stackexchange.com/questions/482060/pdf-generation-from-latex-output-generated-by-doxygen-does-not-output-function
- https://akfd.wordpress.com/2014/12/19/doxygen-latex-pdf-using-miktex-automatic-project-documentation/
- https://www.doxygen.nl/manual/tables.html
- https://wiki.freecad.org/Doxygen
- https://gramine.readthedocs.io/en/stable/devel/howto-doc.html
- https://www.rosettacommons.org/docs/latest/development_documentation/tutorials/doxygen-tips
- 
- 
- 