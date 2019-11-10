using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using QiqqaTestHelpers;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;
using static QiqqaTestHelpers.MiscTestHelpers;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
                            

namespace QiqqaUnitTester
{
    [TestClass]
    public class TestBibTeX
    {
        [TestInitialize]
        public void Setup()
        {
        }

        // To (re)generate the DataRow list:
        //
        //     npm run refresh-data
        //
        // (Note that this same set is distributed across the Check_***() Tests further below.)
        [DataRow("API-test0001.input.bib")]
        [DataRow("Better-BibTeX/export/(non-)dropping particle handling #313.biblatex")]
        [DataRow("Better-BibTeX/export/@jurisdiction; map court,authority to institution #326.biblatex")]
        [DataRow("Better-BibTeX/export/@legislation; map code,container-title to journaltitle #327.biblatex")]
        [DataRow("Better-BibTeX/export/ADS exports dates like 1993-00-00 #1066.biblatex")]
        [DataRow("Better-BibTeX/export/Abbreviations in key generated for Conference Proceedings #548.biblatex")]
        [DataRow("Better-BibTeX/export/Allow explicit field override.biblatex")]
        [DataRow("Better-BibTeX/export/BBT export of square brackets in date #245 -- xref should not be escaped #246.biblatex")]
        [DataRow("Better-BibTeX/export/Be robust against misconfigured journal abbreviator #127.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.001.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.002.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.003.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.004.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.005.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.006.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.007.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.009.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.010.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.011.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.012.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.013.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.014.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.015.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.016.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.017.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.019.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.020.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.021.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.022.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.023.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.stable-keys.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibTeX does not export collections #901.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX does not use biblatex fields eprint and eprinttype #170.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.018.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.026.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.027.bibtex")]
        [DataRow("Better-BibTeX/export/BetterBibLaTeX; Software field company is mapped to publisher instead of organization #1054.biblatex")]
        [DataRow("Better-BibTeX/export/BetterBibtex export fails for missing last name #978.bibtex")]
        [DataRow("Better-BibTeX/export/BibLaTeX Patent author handling, type #1060.biblatex")]
        [DataRow("Better-BibTeX/export/BibLaTeX; export CSL override 'issued' to date or year #351.biblatex")]
        [DataRow("Better-BibTeX/export/BibTeX name escaping has a million inconsistencies #438.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX variable support for journal titles. #309.biblatex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.note.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.off.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.url.bibtex")]
        [DataRow("Better-BibTeX/export/Bibtex key regenerating issue when trashing items #117.biblatex")]
        [DataRow("Better-BibTeX/export/Book converted to mvbook #288.biblatex")]
        [DataRow("Better-BibTeX/export/Book sections have book title for journal in citekey #409.biblatex")]
        [DataRow("Better-BibTeX/export/BraceBalancer.biblatex")]
        [DataRow("Better-BibTeX/export/Braces around author last name when exporting BibTeX #565.bibtex")]
        [DataRow("Better-BibTeX/export/Bulk performance test.bib")]
        [DataRow("Better-BibTeX/export/CSL status = biblatex pubstate #573.biblatex")]
        [DataRow("Better-BibTeX/export/CSL title, volume-title, container-title=BL title, booktitle, maintitle #381.biblatex")]
        [DataRow("Better-BibTeX/export/CSL variables only recognized when in lowercase #408.biblatex")]
        [DataRow("Better-BibTeX/export/Capitalisation in techreport titles #160.biblatex")]
        [DataRow("Better-BibTeX/export/Capitalize all title-fields for language en #383.biblatex")]
        [DataRow("Better-BibTeX/export/Citations have month and day next to year #868.biblatex")]
        [DataRow("Better-BibTeX/export/Citekey generation failure #708 and sort references on export #957.biblatex")]
        [DataRow("Better-BibTeX/export/Colon in bibtex key #405.biblatex")]
        [DataRow("Better-BibTeX/export/Colon not allowed in citation key format #268.biblatex")]
        [DataRow("Better-BibTeX/export/DOI with underscores in extra field #108.biblatex")]
        [DataRow("Better-BibTeX/export/Date export to Better CSL-JSON #360 #811.biblatex")]
        [DataRow("Better-BibTeX/export/Date parses incorrectly with year 1000 when source Zotero field is in datetime format. #515.biblatex")]
        [DataRow("Better-BibTeX/export/Dates incorrect when Zotero date field includes times #934.biblatex")]
        [DataRow("Better-BibTeX/export/Diacritics stripped from keys regardless of ascii or fold filters #266-fold.biblatex")]
        [DataRow("Better-BibTeX/export/Diacritics stripped from keys regardless of ascii or fold filters #266-nofold.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect literal lists #391.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.biber26.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.bibtex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.noopsort.bibtex")]
        [DataRow("Better-BibTeX/export/Do not use more than three initials in case of authshort key #1079.biblatex")]
        [DataRow("Better-BibTeX/export/Dollar sign in title not properly escaped #485.biblatex")]
        [DataRow("Better-BibTeX/export/Don't title-case sup-subscripts #1037.biblatex")]
        [DataRow("Better-BibTeX/export/Double superscript in title field on export #1217.bibtex")]
        [DataRow("Better-BibTeX/export/EDTF dates in BibLaTeX #590.biblatex")]
        [DataRow("Better-BibTeX/export/Empty bibtex clause in extra gobbles whatever follows #99.bibtex")]
        [DataRow("Better-BibTeX/export/Error exporting duplicate eprinttype #1128.biblatex")]
        [DataRow("Better-BibTeX/export/Error exporting with custom Extra field #1118.bibtex")]
        [DataRow("Better-BibTeX/export/Export C as {v C}, not v{C} #152.bibtex")]
        [DataRow("Better-BibTeX/export/Export Forthcoming as Forthcoming.biblatex")]
        [DataRow("Better-BibTeX/export/Export Newspaper Article misses section field #132.biblatex")]
        [DataRow("Better-BibTeX/export/Export error for items without publicationTitle and Preserve BibTeX variables enabled #201.biblatex")]
        [DataRow("Better-BibTeX/export/Export mapping for reporter field #219.biblatex")]
        [DataRow("Better-BibTeX/export/Export of creator-type fields from embedded CSL variables #365 uppercase DOI #825.biblatex")]
        [DataRow("Better-BibTeX/export/Export of item to Better Bibtex fails for auth3_1 #98.bibtex")]
        [DataRow("Better-BibTeX/export/Export unicode as plain text fails for Vietnamese characters #977.bibtex")]
        [DataRow("Better-BibTeX/export/Export web page to misc type with notes and howpublished custom fields #329.bibtex")]
        [DataRow("Better-BibTeX/export/Exporting of single-field author lacks braces #130.biblatex")]
        [DataRow("Better-BibTeX/export/Exporting to bibtex with unicode as plain-text latex commands does not convert U+2040 #1265.bibtex")]
        [DataRow("Better-BibTeX/export/Extra semicolon in biblatexadata causes export failure #133.biblatex")]
        [DataRow("Better-BibTeX/export/Fields in Extra should override defaults.biblatex")]
        [DataRow("Better-BibTeX/export/German Umlaut separated by brackets #146.biblatex")]
        [DataRow("Better-BibTeX/export/HTML Fragment separator escaped in url #140 #147.biblatex")]
        [DataRow("Better-BibTeX/export/Hang on non-file attachment export #112 - URL export broken #114.biblatex")]
        [DataRow("Better-BibTeX/export/Hyphenated last names not escaped properly (or at all) in BibTeX #976.bibtex")]
        [DataRow("Better-BibTeX/export/Ignore HTML tags when generating citation key #264.biblatex")]
        [DataRow("Better-BibTeX/export/Ignoring upper cases in German titles #456.biblatex")]
        [DataRow("Better-BibTeX/export/Ignoring upper cases in German titles #456.bibtex")]
        [DataRow("Better-BibTeX/export/Include first name initial(s) in cite key generation pattern (86).bibtex")]
        [DataRow("Better-BibTeX/export/Japanese rendered as Chinese in Citekey #979.biblatex")]
        [DataRow("Better-BibTeX/export/Japanese rendered as Chinese in Citekey #979.juris-m.biblatex")]
        [DataRow("Better-BibTeX/export/Journal abbreviations exported in bibtex (81).bibtex")]
        [DataRow("Better-BibTeX/export/Journal abbreviations.bibtex")]
        [DataRow("Better-BibTeX/export/Juris-M missing multi-lingual fields #482.biblatex")]
        [DataRow("Better-BibTeX/export/Juris-M missing multi-lingual fields #482.juris-m.biblatex")]
        [DataRow("Better-BibTeX/export/Latex commands in extra-field treated differently #1207.biblatex")]
        [DataRow("Better-BibTeX/export/Malformed HTML.biblatex")]
        [DataRow("Better-BibTeX/export/Math parts in title #113.biblatex")]
        [DataRow("Better-BibTeX/export/Mismatched conversion of braces in title on export means field never gets closed #1218.bibtex")]
        [DataRow("Better-BibTeX/export/Missing JabRef pattern; authEtAl #554.bibtex")]
        [DataRow("Better-BibTeX/export/Missing JabRef pattern; authorsN+initials #553.bibtex")]
        [DataRow("Better-BibTeX/export/Month showing up in year field on export #889.biblatex")]
        [DataRow("Better-BibTeX/export/Multiple locations and-or publishers and BibLaTeX export #689.biblatex")]
        [DataRow("Better-BibTeX/export/No booktitle field when exporting references from conference proceedings #1069.bibtex")]
        [DataRow("Better-BibTeX/export/No brace protection when suppressTitleCase set to true #1188.bibtex")]
        [DataRow("Better-BibTeX/export/No space between author first and last name because last char of first name is translated to a latex command #1091.bibtex")]
        [DataRow("Better-BibTeX/export/Non-ascii in dates is not matched by date parser #376.biblatex")]
        [DataRow("Better-BibTeX/export/Normalize date ranges in citekeys #356.biblatex")]
        [DataRow("Better-BibTeX/export/Numbers confuse capital-preservation #295.bibtex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.default.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.groups3.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.prefer-DOI.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.prefer-url.biblatex")]
        [DataRow("Better-BibTeX/export/Open date range crashes citekey generator #1227.bibtex")]
        [DataRow("Better-BibTeX/export/Oriental dates trip up date parser #389.biblatex")]
        [DataRow("Better-BibTeX/export/Protect math sections #1148.biblatex")]
        [DataRow("Better-BibTeX/export/Really Big whopping library.bib")]
        [DataRow("Better-BibTeX/export/References with multiple notes fail to export #174.biblatex")]
        [DataRow("Better-BibTeX/export/Replicate Zotero key algorithm #439.bibtex")]
        [DataRow("Better-BibTeX/export/Season ranges should be exported as pseudo-months (13-16, or 21-24) #860.biblatex")]
        [DataRow("Better-BibTeX/export/Set IDS field when merging references with different citation keys #1221.biblatex")]
        [DataRow("Better-BibTeX/export/Setting the item type via the cheater syntax #587.biblatex")]
        [DataRow("Better-BibTeX/export/Setting the item type via the cheater syntax #587.bibtex")]
        [DataRow("Better-BibTeX/export/Shortjournal does not get exported to biblatex format #102 - biblatexcitekey #105.biblatex")]
        [DataRow("Better-BibTeX/export/Sorting and optional particle handling #411.off.biblatex")]
        [DataRow("Better-BibTeX/export/Sorting and optional particle handling #411.on.biblatex")]
        [DataRow("Better-BibTeX/export/Spaces not stripped from citation keys #294.biblatex")]
        [DataRow("Better-BibTeX/export/Square brackets in Publication field (85), and non-pinned keys must change when the pattern does.bibtex")]
        [DataRow("Better-BibTeX/export/Suppress brace protection #1139.biblatex")]
        [DataRow("Better-BibTeX/export/Thin space in author name #859.biblatex")]
        [DataRow("Better-BibTeX/export/Title case of latex greek text on biblatex export #564.biblatex")]
        [DataRow("Better-BibTeX/export/Treat dash-connected words as a single word for citekey generation #619.biblatex")]
        [DataRow("Better-BibTeX/export/Unbalanced vphantom escapes #1043-mathmode.bibtex")]
        [DataRow("Better-BibTeX/export/Unbalanced vphantom escapes #1043.bibtex")]
        [DataRow("Better-BibTeX/export/Underscores break capital-preservation #300.bibtex")]
        [DataRow("Better-BibTeX/export/[authN_M] citation key syntax has off-by-one error #899.bibtex")]
        [DataRow("Better-BibTeX/export/arXiv identifiers in BibLaTeX export #460.biblatex")]
        [DataRow("Better-BibTeX/export/arXiv identifiers in BibLaTeX export #460.bibtex")]
        [DataRow("Better-BibTeX/export/auth leaves punctuation in citation key #310.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.after.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.after.coll.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.before.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.before.coll.biblatex")]
        [DataRow("Better-BibTeX/export/automatic tags in export #1270.bibtex")]
        [DataRow("Better-BibTeX/export/biblatex export of Presentation; Use type and venue fields #644.biblatex")]
        [DataRow("Better-BibTeX/export/biblatex; Language tag xx is exported, xx-XX is not #380.biblatex")]
        [DataRow("Better-BibTeX/export/bibtex export of phdthesis does not case-protect -type- #435.bibtex")]
        [DataRow("Better-BibTeX/export/bibtex; url export does not survive underscores #402.biblatex")]
        [DataRow("Better-BibTeX/export/bibtex; url export does not survive underscores #402.bibtex")]
        [DataRow("Better-BibTeX/export/bookSection is always converted to @inbook, never @incollection #282.biblatex")]
        [DataRow("Better-BibTeX/export/braces after textemdash followed by unicode #980.bibtex")]
        [DataRow("Better-BibTeX/export/capital delta breaks .bib output #141.bibtex")]
        [DataRow("Better-BibTeX/export/citekey firstpage-lastpage #1147.bibtex")]
        [DataRow("Better-BibTeX/export/condense in cite key format not working #308.biblatex")]
        [DataRow("Better-BibTeX/export/creating a key with [authForeIni] and [authN] not working properly #892.bibtex")]
        [DataRow("Better-BibTeX/export/csquotes #302.biblatex")]
        [DataRow("Better-BibTeX/export/custom fields should be exported as-is #441.bibtex")]
        [DataRow("Better-BibTeX/export/customized fields with curly brackets are not exported correctly anymore #775.biblatex")]
        [DataRow("Better-BibTeX/export/date and year are switched #406.biblatex")]
        [DataRow("Better-BibTeX/export/date not always parsed properly into month and year with PubMed #1112.bibtex")]
        [DataRow("Better-BibTeX/export/date ranges #747+#746.biblatex")]
        [DataRow("Better-BibTeX/export/date ranges #747+#746.bibtex")]
        [DataRow("Better-BibTeX/export/don't escape entry key fields for #296.biblatex")]
        [DataRow("Better-BibTeX/export/error on exporting note with pre tags; duplicate field howpublished #1092.bibtex")]
        [DataRow("Better-BibTeX/export/export missing the a accent #691.biblatex")]
        [DataRow("Better-BibTeX/export/italics in title - capitalization #541.biblatex")]
        [DataRow("Better-BibTeX/export/key migration.biblatex")]
        [DataRow("Better-BibTeX/export/map csl-json variables #293.biblatex")]
        [DataRow("Better-BibTeX/export/markup small-caps, superscript, italics #301.biblatex")]
        [DataRow("Better-BibTeX/export/pre not working in Extra field #559.biblatex")]
        [DataRow("Better-BibTeX/export/preserve @strings between import-export #1162.biblatex")]
        [DataRow("Better-BibTeX/export/preserve @strings between import-export #1162.bibtex")]
        [DataRow("Better-BibTeX/export/preserve BibTeX Variables does not check for null values while escaping #337.bibtex")]
        [DataRow("Better-BibTeX/export/referencetype= does not work #278.biblatex")]
        [DataRow("Better-BibTeX/export/remove the field if the override is empty #303.biblatex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.biblatex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.bibtex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.sbp.bibtex")]
        [DataRow("Better-BibTeX/export/thesis zotero entries always create @phdthesis bibtex entries #307.biblatex")]
        [DataRow("Better-BibTeX/export/thesis zotero entries always create @phdthesis bibtex entries #307.bibtex")]
        [DataRow("Better-BibTeX/export/titles are title-cased in .bib file #558.bibtex")]
        [DataRow("Better-BibTeX/export/transliteration for citekey #580.biblatex")]
        [DataRow("Better-BibTeX/export/two ISSN number are freezing browser #110 + Generating keys and export broken #111.biblatex")]
        [DataRow("Better-BibTeX/export/typo stature-statute (zotero item type) #284.biblatex")]
        [DataRow("Better-BibTeX/export/underscores in URL fields should not be escaped #104.biblatex")]
        [DataRow("Better-BibTeX/export/urldate when only DOI is exported #869.biblatex")]
        [DataRow("Better-BibTeX/export/veryshorttitle and compound words #551.bibtex")]
        [DataRow("Better-BibTeX/import/Async import, large library #720.bib")]
        [DataRow("Better-BibTeX/import/Author splitter failure.bib")]
        [DataRow("Better-BibTeX/import/Better BibLaTeX import improvements #549.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX Import 2.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.001.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.003.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.004.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.005.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.006.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.008.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.009.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.010.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.011.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.012.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.014.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.015.bib")]
        [DataRow("Better-BibTeX/import/BibLaTeX Patent author handling, type #1060.bib")]
        [DataRow("Better-BibTeX/import/BibTeX import; preamble with def create problems #732.bib")]
        [DataRow("Better-BibTeX/import/Biblatex Annotation Import Bug #613.bib")]
        [DataRow("Better-BibTeX/import/Endnote should parse.bib")]
        [DataRow("Better-BibTeX/import/Failure to handle unparsed author names (92).bib")]
        [DataRow("Better-BibTeX/import/Import Jabref fileDirectory, unexpected reference type #1058.bib")]
        [DataRow("Better-BibTeX/import/Import fails to perform @String substitutions #154.bib")]
        [DataRow("Better-BibTeX/import/Import location to event-place for conference papers.bib")]
        [DataRow("Better-BibTeX/import/Issues with round instead of curly braces do not import correctly #871.bib")]
        [DataRow("Better-BibTeX/import/Jabref groups import does not work #717.2.10.bib")]
        [DataRow("Better-BibTeX/import/Jabref groups import does not work #717.3.8.bib")]
        [DataRow("Better-BibTeX/import/Literal names.bib")]
        [DataRow("Better-BibTeX/import/Maintain the JabRef group and subgroup structure when importing a BibTeX db #97.bib")]
        [DataRow("Better-BibTeX/import/Math formatting lost on import #627.bib")]
        [DataRow("Better-BibTeX/import/Math markup to unicode not always imported correctly #472.bib")]
        [DataRow("Better-BibTeX/import/Math markup to unicode not always imported correctly #472.roundtrip.bib")]
        [DataRow("Better-BibTeX/import/Problem when importing BibTeX entries with percent sign #95 or preamble #96.bib")]
        [DataRow("Better-BibTeX/import/Problem when importing BibTeX entries with square brackets #94.bib")]
        [DataRow("Better-BibTeX/import/Some bibtex entries quietly discarded on import from bib file #873.bib")]
        [DataRow("Better-BibTeX/import/Spaces lost when expanding string variables during import #1081.bib")]
        [DataRow("Better-BibTeX/import/Wrong ring-above import #1115.bib")]
        [DataRow("Better-BibTeX/import/eprinttype field dropped on import #959.bib")]
        [DataRow("Better-BibTeX/import/importing a title-cased bib #1246.bib")]
        [DataRow("Better-BibTeX/import/importing a title-cased bib #1246.roundtrip.bib")]
        [DataRow("Better-BibTeX/import/space after citekey creates confusion #716.bib")]
        [DataRow("Better-BibTeX/import/support Local-Zo-Url-x field from BibDesk2Zotero_attachments #667.bib")]
        [DataRow("Better-BibTeX/import/zbb (quietly) chokes on this .bib #664.bib")]
        [DataRow("CrossTeX/comments.bib")]
        [DataRow("CrossTeX/diss.bib")]
        [DataRow("CrossTeX/sample-0001.bib")]
        [DataRow("CrossTeX/sample-0002.bib")]
        [DataRow("CrossTeX/sample-0003.bib")]
        [DataRow("CrossTeX/sample-0004.bib")]
        [DataRow("CrossTeX/sample-0005.bib")]
        [DataRow("CrossTeX/sample-0006.bib")]
        [DataRow("CrossTeX/sample-0007.bib")]
        [DataRow("CrossTeX/sample-0008.bib")]
        [DataRow("CrossTeX/sample-0009.bib")]
        [DataRow("CrossTeX/sample-comment-0001.bib")]
        [DataRow("CrossTeX/sample-comment-0002.bib")]
        [DataRow("CrossTeX/sample-comment-0003.bib")]
        [DataRow("CrossTeX/styles.bib")]
        [DataRow("CrossTeX/url.bib")]
        [DataRow("IEEEtran/IEEEabrv.bib")]
        [DataRow("IEEEtran/IEEEexample.bib")]
        [DataRow("IEEEtran/IEEEfull.bib")]
        [DataRow("IEEEtran/sample-from-website1.bib")]
        [DataRow("TeX-accented-letters-0001.bib")]
        [DataRow("TeX-accented-letters-0002.bib")]
        [DataRow("TeX-accented-letters-0003.bib")]
        [DataRow("TeX-accented-letters-0004.bib")]
        [DataRow("TeX-accented-letters-0005.bib")]
        [DataRow("TeX-accented-letters-0006.bib")]
        [DataRow("TeX-accented-letters-0007.bib")]
        [DataRow("TeX-accented-letters-0008.bib")]
        [DataRow("TeX-accented-letters-0009.bib")]
        [DataRow("TeX-accented-letters-0010.bib")]
        [DataRow("TeX-accented-letters-0011.bib")]
        [DataRow("all-caps-0001.bib")]
        [DataRow("ampersand-0001.bib")]
        [DataRow("ampersand-0002.bib")]
        [DataRow("ampersand-0003.bib")]
        [DataRow("b0rked-0001.bib")]
        [DataRow("b0rked-0002.bib")]
        [DataRow("b0rked-0003.bib")]
        [DataRow("b0rked-0004.bib")]
        [DataRow("b0rked-0005.bib")]
        [DataRow("b0rked-0006.bib")]
        [DataRow("b0rked-0007.bib")]
        [DataRow("b0rked-0008.bib")]
        [DataRow("b0rked-0009.bib")]
        [DataRow("b0rked-0010.bib")]
        [DataRow("b0rked-0011.bib")]
        [DataRow("b0rked-0012.bib")]
        [DataRow("b0rked-0013.bib")]
        [DataRow("b0rked-0014.bib")]
        [DataRow("b0rked-0015.bib")]
        [DataRow("b0rked-0100.bib")]
        [DataRow("b0rked-0101.bib")]
        [DataRow("b0rked-0102.bib")]
        [DataRow("b0rked-0103.bib")]
        [DataRow("b0rked-0104.bib")]
        [DataRow("biber/annotations.bib")]
        [DataRow("biber/bibtex-aliases.bib")]
        [DataRow("biber/crossrefs.bib")]
        [DataRow("biber/datalists.bib")]
        [DataRow("biber/dateformats.bib")]
        [DataRow("biber/definitions.bib")]
        [DataRow("biber/dm-constraints.bib")]
        [DataRow("biber/encoding1.bib")]
        [DataRow("biber/encoding2.bib")]
        [DataRow("biber/encoding3.bib")]
        [DataRow("biber/encoding4.bib")]
        [DataRow("biber/encoding5.bib")]
        [DataRow("biber/encoding6.bib")]
        [DataRow("biber/examples.bib")]
        [DataRow("biber/extradate.bib")]
        [DataRow("biber/extratitle.bib")]
        [DataRow("biber/extratitleyear.bib")]
        [DataRow("biber/full-bbl.bib")]
        [DataRow("biber/full-bibtex_biber.bib")]
        [DataRow("biber/full-dot.bib")]
        [DataRow("biber/labelalpha.bib")]
        [DataRow("biber/labelalphaname.bib")]
        [DataRow("biber/names.bib")]
        [DataRow("biber/names_x.bib")]
        [DataRow("biber/options.bib")]
        [DataRow("biber/papers.bib")]
        [DataRow("biber/related.bib")]
        [DataRow("biber/sections1.bib")]
        [DataRow("biber/sections2.bib")]
        [DataRow("biber/sections3.bib")]
        [DataRow("biber/sections4.bib")]
        [DataRow("biber/sets.bib")]
        [DataRow("biber/skips.bib")]
        [DataRow("biber/skipsg.bib")]
        [DataRow("biber/sort-case.bib")]
        [DataRow("biber/sort-order.bib")]
        [DataRow("biber/sort-uc.bib")]
        [DataRow("biber/sort.bib")]
        [DataRow("biber/tool.bib")]
        [DataRow("biber/translit.bib")]
        [DataRow("biber/truncation.bib")]
        [DataRow("biber/tugboat.bib")]
        [DataRow("biber/uniqueness-nameparts.bib")]
        [DataRow("biber/uniqueness1.bib")]
        [DataRow("biber/uniqueness2.bib")]
        [DataRow("biber/uniqueness3.bib")]
        [DataRow("biber/uniqueness4.bib")]
        [DataRow("biber/uniqueness5.bib")]
        [DataRow("biber/uniqueness6.bib")]
        [DataRow("biber/xdata.bib")]
        [DataRow("biblatex-examples.bib")]
        [DataRow("biblatex/95-customlists.bib")]
        [DataRow("biblatex/97-annotations.bib")]
        [DataRow("biblatex/biblatex-examples.bib")]
        [DataRow("bibtex-original-btxdoc.bib")]
        [DataRow("bibtex-original-xampl.bib")]
        [DataRow("btparse/commas.bib")]
        [DataRow("btparse/comment.bib")]
        [DataRow("btparse/corpora.bib")]
        [DataRow("btparse/empty.bib")]
        [DataRow("btparse/errors.bib")]
        [DataRow("btparse/foreign.bib")]
        [DataRow("btparse/macro.bib")]
        [DataRow("btparse/preamble.bib")]
        [DataRow("btparse/regular.bib")]
        [DataRow("btparse/sample-0001.bib")]
        [DataRow("btparse/sample-0002.bib")]
        [DataRow("btparse/sample-0003.bib")]
        [DataRow("btparse/simple.bib")]
        [DataRow("btparse/unlimited.bib")]
        [DataRow("comment-0001.bib")]
        [DataRow("comment-0002.bib")]
        [DataRow("comment-0003.bib")]
        [DataRow("comment-0004.bib")]
        [DataRow("comment-0005.bib")]
        [DataRow("comment-0006.bib")]
        [DataRow("concatenation-0001.bib")]
        [DataRow("concatenation-0002.bib")]
        [DataRow("issue-0072-b0rked-01.bib")]
        [DataRow("macros-0001.bib")]
        [DataRow("macros-0002.bib")]
        [DataRow("macros-0003.bib")]
        [DataRow("macros-0004.bib")]
        [DataRow("macros-0005.bib")]
        [DataRow("misc-0001.bib")]
        [DataRow("misc-0002.bib")]
        [DataRow("misc-0003.bib")]
        [DataRow("misc-0004.bib")]
        [DataRow("misc-0005.bib")]
        [DataRow("misc-0006.bib")]
        [DataRow("misc-0007.bib")]
        [DataRow("misc-0008.bib")]
        [DataRow("multiple-records-0001.bib")]
        [DataRow("non-ASCII-characters-0001.bib")]
        [DataRow("non-ASCII-characters-0002.bib")]
        [DataRow("non-ASCII-characters-0003.bib")]
        [DataRow("non-ASCII-characters-0004.bib")]
        [DataRow("oddities-0001.bib")]
        [DataRow("oddities-0002.bib")]
        [DataRow("oddities-0003.bib")]
        [DataRow("oddities-0006.bib")]
        [DataRow("punctuation-0001.bib")]
        [DataRow("simple-0001.bib")]
        [DataRow("whitespacing-0001.bib")]
        [DataRow("whitespacing-0002.bib")]
        [DataRow("whitespacing-0003.bib")]
        [DataTestMethod]
        public void Do_TestFiles_Exist(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);
        }

        // TestData items: All data files are employed in at least one BibTeX test! Hence this list is empty!
        //
        // (added this dummy entry to ensure the test runner doesn't barf a hairball on this otherwise
        // empty [DataTestMethod]:
        [DataRow("simple-0001.bib")]
        [DataTestMethod]
        public void Pending_TestFiles(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);
        }

        [DataRow("Utilities/BibTeX/TestFiles/Sample.bib")]
        [DataTestMethod]
        public void Do_Other_TestFiles_Exist_Outside_the_Test_Realm(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToAnyFile(bibtex_filepath);
            ASSERT.FileExists(path);
        }

        [DataRow("simple-0001.bib", DisplayName = "a simple BibTeX record")]
        [DataTestMethod]
        public void SimpleTest(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("TeX-accented-letters-0001.bib")]
        [DataRow("TeX-accented-letters-0002.bib")]
        [DataRow("TeX-accented-letters-0003.bib")]
        [DataRow("TeX-accented-letters-0004.bib")]
        [DataRow("TeX-accented-letters-0005.bib")]
        [DataRow("TeX-accented-letters-0006.bib")]
        [DataRow("TeX-accented-letters-0007.bib")]
        [DataRow("TeX-accented-letters-0008.bib")]
        [DataRow("TeX-accented-letters-0009.bib")]
        [DataRow("TeX-accented-letters-0010.bib")]
        [DataRow("TeX-accented-letters-0011.bib")]
        [DataTestMethod]
        public void Test_TeX_Accented_Letters(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [TestMethod]
        public void Ensure_BibTeX_EntryTypes_DefList_Is_Loaded_And_Parsed()
        {
            // force a load of the types.
            EntryTypes.ResetForTesting();
            EntryTypes t = EntryTypes.Instance;
            ASSERT.IsNotNull(t);
            ASSERT.IsGreaterOrEqual(t.EntryTypeList.Count, 17, "expected to load a full set of BibTeX entry type specs");
            ASSERT.IsGreaterOrEqual(t.FieldTypeList.Count, 28, "expected to load a full set of BibTeX field types");
        }

        [DataRow("Utilities/BibTeX/TestFiles/Sample.bib")]
        [DataTestMethod]
        public void Test_Parser_On_Original_Qiqqa_TestSet_SampleBib(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToAnyFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

#if false
        [DataRow("API-test0001.input.bib")]
        [DataTestMethod]
        public void Test_BibTeXTools_API(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToAnyFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string sample_bibtext = GetTestFileContent(path);

            Dictionary<string, string> datums = new Dictionary<string, string>();

            // parse the BibTeX input
            BibTexItem bibtex_item = BibTexParser.ParseOne(sample_bibtext, false);

            // exercise every "get" type API member function & property:
            datums["has_key"] = bibtex_item.HasKey() ? "true" : "false";
            datums["has_type"] = bibtex_item.HasType() ? "true" : "false";
            datums["has_title"] = bibtex_item.HasTitle() ? "true" : "false";
            datums["has_author"] = bibtex_item.HasAuthor() ? "true" : "false";
            datums["is_empty"] = bibtex_item.IsEmpty() ? "true" : "false";

            datums["key"] = bibtex_item.Key;
            datums["type"] = bibtex_item.Type;

            datums["title"] = bibtex_item.GetTitle();
            datums["author"] = bibtex_item.GetAuthor();
            datums["year"] = bibtex_item.GetYear();

            datums["field_keys"] = String.Join("; ", bibtex_item.FieldKeys);

            StringWriter sw = new StringWriter();
            foreach (var r in bibtex_item.Fields)
            {
                sw.WriteLine("  {0} = {1}", r.Key, r.Value);
            }
            datums["fields"] = sw.ToString();

            datums["has_author_field"] = bibtex_item.ContainsField("author") ? "true" : "false";

            datums["generic_publication"] = bibtex_item.GetGenericPublication();
            datums["unparseable_content"] = bibtex_item.UnparsableContent;

            datums["exceptions"] = bibtex_item.GetExceptionsString();
            datums["warnings"] = bibtex_item.GetWarningsString();

            datums["bibtex_record"] = bibtex_item.ToBibTex();

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(datums, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );

            // set up reference for write-access comparison
            string write_filepath = path.Replace(".input.bib", ".edited.bib");

            // exercise every "set" type API member function & property:
            bibtex_item.Key = null;

            bibtex_item.SetTitle("New title");
            bibtex_item.SetAuthor("New author");
            bibtex_item.SetYear("New year");

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            json_out = JsonConvert.SerializeObject(bibtex_item, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, write_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }
#endif

        [TestMethod]
        public void Measure_Parse_Performance()
        {
            string path = GetNormalizedPathToBibTeXTestFile("");
            ASSERT.DirectoryExists(path);

            // collect all test files
            long charcount = 0;
            Dictionary<string, string> filelist = new Dictionary<string, string>();
            foreach (string file in Directory.EnumerateFiles(path, "*.bib", SearchOption.AllDirectories))
            {
                ASSERT.FileExists(file);
                string content = GetTestFileContent(file);
                filelist.Add(file, content);
                charcount += content.Length;
            }
            foreach (string file in Directory.EnumerateFiles(path, "*.bibtex", SearchOption.AllDirectories))
            {
                ASSERT.FileExists(file);
#if false
                // EnumerateFiles(*.bib) has also (unexpectedly) picked up the *.bibtex files. :-S
                // I consider that a .NET bug. Alas.
                ASSERT.IsTrue(filelist.ContainsKey(file), "EnumerateFiles(*.bib) is supposed to have already picked up the *.bibtex files.");
#else
                ASSERT.IsFalse(filelist.ContainsKey(file), "EnumerateFiles(*.bib) already picked up the *.bibtex files in MSVS2019 before 2019/October updates (Windows 10 updates or MSVS updates?).");
                string content = GetTestFileContent(file);
                filelist.Add(file, content);
                charcount += content.Length;
#endif
            }

            // and then run them through the parser while we run the stopwatch
            int filecount = filelist.Count;
            BibTexParseResult rv;
            Stopwatch clock = Stopwatch.StartNew();
            foreach (var entry in filelist)
            {
                try
                {
                    rv = BibTexParser.Parse(entry.Value);

                    if (0 == rv.Items.Count)
                    {
                        Logging.Warn("testfile <{0}>: NO BibTeX record found. RAW record:\n  {1}", entry.Key, entry.Value);
                    }
                    else
                    {
                        int index = 0;
                        foreach (var record in rv.Items)
                        {
                            if (record.Exceptions.Count > 0)
                            {
                                string errors = record.GetExceptionsString();
                                Logging.Warn("testfile <{0}>, record index {1}: BibTeX parse error: {2}", entry.Key, index, errors);
                            }
                            index++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "testfile <{0}>: fatal BibTeX parse error while parsing...", entry.Key);
                }
            }
            double tt = charcount * 1.0 / clock.ElapsedMilliseconds;
            Logging.Info("BibTex parsing can do {0:0.000}K operations per second per character", tt);
        }

        // ==================================================================================================
        //
        // Tests using the collected data fixtures...
        //
        // ==================================================================================================

        [DataRow("all-caps-0001.bib")]
        [DataTestMethod]
        public void Check_BibTeX_AllCapitals(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("ampersand-0001.bib")]
        [DataRow("ampersand-0002.bib")]
        [DataRow("ampersand-0003.bib")]
        [DataTestMethod]
        public void Check_BibTeX_AmpersandProcessing(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("API-test0001.input.bib")]
        [DataTestMethod]
        public void Check_BibTeX_API(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("b0rked-0001.bib")]
        [DataRow("b0rked-0002.bib")]
        [DataRow("b0rked-0003.bib")]
        [DataRow("b0rked-0004.bib")]
        [DataRow("b0rked-0005.bib")]
        [DataRow("b0rked-0006.bib")]
        [DataRow("b0rked-0007.bib")]
        [DataRow("b0rked-0008.bib")]
        [DataRow("b0rked-0009.bib")]
        [DataRow("b0rked-0010.bib")]
        [DataRow("b0rked-0011.bib")]
        [DataRow("b0rked-0012.bib")]
        [DataRow("b0rked-0013.bib")]
        [DataRow("b0rked-0014.bib")]
        [DataRow("b0rked-0015.bib")]
        [DataRow("b0rked-0100.bib")]
        [DataRow("b0rked-0101.bib")]
        [DataRow("b0rked-0102.bib")]
        [DataRow("b0rked-0103.bib")]
        [DataRow("b0rked-0104.bib")]
        [DataTestMethod]
        public void Check_How_BibTeX_Parser_Copes_With_B0rked_Input(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("Better-BibTeX/export/(non-)dropping particle handling #313.biblatex")]
        [DataRow("Better-BibTeX/export/@jurisdiction; map court,authority to institution #326.biblatex")]
        [DataRow("Better-BibTeX/export/@legislation; map code,container-title to journaltitle #327.biblatex")]
        [DataRow("Better-BibTeX/export/ADS exports dates like 1993-00-00 #1066.biblatex")]
        [DataRow("Better-BibTeX/export/Abbreviations in key generated for Conference Proceedings #548.biblatex")]
        [DataRow("Better-BibTeX/export/Allow explicit field override.biblatex")]
        [DataRow("Better-BibTeX/export/BBT export of square brackets in date #245 -- xref should not be escaped #246.biblatex")]
        [DataRow("Better-BibTeX/export/Be robust against misconfigured journal abbreviator #127.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.001.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.002.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.003.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.004.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.005.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.006.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.007.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.009.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.010.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.011.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.012.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.013.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.014.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.015.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.016.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.017.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.019.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.020.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.021.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.022.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.023.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibLaTeX.stable-keys.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibTeX does not export collections #901.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX does not use biblatex fields eprint and eprinttype #170.biblatex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.018.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.026.bibtex")]
        [DataRow("Better-BibTeX/export/Better BibTeX.027.bibtex")]
        [DataRow("Better-BibTeX/export/BetterBibLaTeX; Software field company is mapped to publisher instead of organization #1054.biblatex")]
        [DataRow("Better-BibTeX/export/BetterBibtex export fails for missing last name #978.bibtex")]
        [DataRow("Better-BibTeX/export/BibLaTeX Patent author handling, type #1060.biblatex")]
        [DataRow("Better-BibTeX/export/BibLaTeX; export CSL override 'issued' to date or year #351.biblatex")]
        [DataRow("Better-BibTeX/export/BibTeX name escaping has a million inconsistencies #438.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX variable support for journal titles. #309.biblatex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.note.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.off.bibtex")]
        [DataRow("Better-BibTeX/export/BibTeX; URL missing in bibtex for Book Section #412.url.bibtex")]
        [DataRow("Better-BibTeX/export/Bibtex key regenerating issue when trashing items #117.biblatex")]
        [DataRow("Better-BibTeX/export/Book converted to mvbook #288.biblatex")]
        [DataRow("Better-BibTeX/export/Book sections have book title for journal in citekey #409.biblatex")]
        [DataRow("Better-BibTeX/export/BraceBalancer.biblatex")]
        [DataRow("Better-BibTeX/export/Braces around author last name when exporting BibTeX #565.bibtex")]
        [DataRow("Better-BibTeX/export/Bulk performance test.bib")]
        [DataRow("Better-BibTeX/export/CSL status = biblatex pubstate #573.biblatex")]
        [DataRow("Better-BibTeX/export/CSL title, volume-title, container-title=BL title, booktitle, maintitle #381.biblatex")]
        [DataRow("Better-BibTeX/export/CSL variables only recognized when in lowercase #408.biblatex")]
        [DataRow("Better-BibTeX/export/Capitalisation in techreport titles #160.biblatex")]
        [DataRow("Better-BibTeX/export/Capitalize all title-fields for language en #383.biblatex")]
        [DataRow("Better-BibTeX/export/Citations have month and day next to year #868.biblatex")]
        [DataRow("Better-BibTeX/export/Citekey generation failure #708 and sort references on export #957.biblatex")]
        [DataRow("Better-BibTeX/export/Colon in bibtex key #405.biblatex")]
        [DataRow("Better-BibTeX/export/Colon not allowed in citation key format #268.biblatex")]
        [DataRow("Better-BibTeX/export/DOI with underscores in extra field #108.biblatex")]
        [DataRow("Better-BibTeX/export/Date export to Better CSL-JSON #360 #811.biblatex")]
        [DataRow("Better-BibTeX/export/Date parses incorrectly with year 1000 when source Zotero field is in datetime format. #515.biblatex")]
        [DataRow("Better-BibTeX/export/Dates incorrect when Zotero date field includes times #934.biblatex")]
        [DataRow("Better-BibTeX/export/Diacritics stripped from keys regardless of ascii or fold filters #266-fold.biblatex")]
        [DataRow("Better-BibTeX/export/Diacritics stripped from keys regardless of ascii or fold filters #266-nofold.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect literal lists #391.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.biber26.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.biblatex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.bibtex")]
        [DataRow("Better-BibTeX/export/Do not caps-protect name fields #384 #565 #566.noopsort.bibtex")]
        [DataRow("Better-BibTeX/export/Do not use more than three initials in case of authshort key #1079.biblatex")]
        [DataRow("Better-BibTeX/export/Dollar sign in title not properly escaped #485.biblatex")]
        [DataRow("Better-BibTeX/export/Don't title-case sup-subscripts #1037.biblatex")]
        [DataRow("Better-BibTeX/export/Double superscript in title field on export #1217.bibtex")]
        [DataRow("Better-BibTeX/export/EDTF dates in BibLaTeX #590.biblatex")]
        [DataRow("Better-BibTeX/export/Empty bibtex clause in extra gobbles whatever follows #99.bibtex")]
        [DataRow("Better-BibTeX/export/Error exporting duplicate eprinttype #1128.biblatex")]
        [DataRow("Better-BibTeX/export/Error exporting with custom Extra field #1118.bibtex")]
        [DataRow("Better-BibTeX/export/Export C as {v C}, not v{C} #152.bibtex")]
        [DataRow("Better-BibTeX/export/Export Forthcoming as Forthcoming.biblatex")]
        [DataRow("Better-BibTeX/export/Export Newspaper Article misses section field #132.biblatex")]
        [DataRow("Better-BibTeX/export/Export error for items without publicationTitle and Preserve BibTeX variables enabled #201.biblatex")]
        [DataRow("Better-BibTeX/export/Export mapping for reporter field #219.biblatex")]
        [DataRow("Better-BibTeX/export/Export of creator-type fields from embedded CSL variables #365 uppercase DOI #825.biblatex")]
        [DataRow("Better-BibTeX/export/Export of item to Better Bibtex fails for auth3_1 #98.bibtex")]
        [DataRow("Better-BibTeX/export/Export unicode as plain text fails for Vietnamese characters #977.bibtex")]
        [DataRow("Better-BibTeX/export/Export web page to misc type with notes and howpublished custom fields #329.bibtex")]
        [DataRow("Better-BibTeX/export/Exporting of single-field author lacks braces #130.biblatex")]
        [DataRow("Better-BibTeX/export/Exporting to bibtex with unicode as plain-text latex commands does not convert U+2040 #1265.bibtex")]
        [DataRow("Better-BibTeX/export/Extra semicolon in biblatexadata causes export failure #133.biblatex")]
        [DataRow("Better-BibTeX/export/Fields in Extra should override defaults.biblatex")]
        [DataRow("Better-BibTeX/export/German Umlaut separated by brackets #146.biblatex")]
        [DataRow("Better-BibTeX/export/HTML Fragment separator escaped in url #140 #147.biblatex")]
        [DataRow("Better-BibTeX/export/Hang on non-file attachment export #112 - URL export broken #114.biblatex")]
        [DataRow("Better-BibTeX/export/Hyphenated last names not escaped properly (or at all) in BibTeX #976.bibtex")]
        [DataRow("Better-BibTeX/export/Ignore HTML tags when generating citation key #264.biblatex")]
        [DataRow("Better-BibTeX/export/Ignoring upper cases in German titles #456.biblatex")]
        [DataRow("Better-BibTeX/export/Ignoring upper cases in German titles #456.bibtex")]
        [DataRow("Better-BibTeX/export/Include first name initial(s) in cite key generation pattern (86).bibtex")]
        [DataRow("Better-BibTeX/export/Japanese rendered as Chinese in Citekey #979.biblatex")]
        [DataRow("Better-BibTeX/export/Japanese rendered as Chinese in Citekey #979.juris-m.biblatex")]
        [DataRow("Better-BibTeX/export/Journal abbreviations exported in bibtex (81).bibtex")]
        [DataRow("Better-BibTeX/export/Journal abbreviations.bibtex")]
        [DataRow("Better-BibTeX/export/Juris-M missing multi-lingual fields #482.biblatex")]
        [DataRow("Better-BibTeX/export/Juris-M missing multi-lingual fields #482.juris-m.biblatex")]
        [DataRow("Better-BibTeX/export/Latex commands in extra-field treated differently #1207.biblatex")]
        [DataRow("Better-BibTeX/export/Malformed HTML.biblatex")]
        [DataRow("Better-BibTeX/export/Math parts in title #113.biblatex")]
        [DataRow("Better-BibTeX/export/Mismatched conversion of braces in title on export means field never gets closed #1218.bibtex")]
        [DataRow("Better-BibTeX/export/Missing JabRef pattern; authEtAl #554.bibtex")]
        [DataRow("Better-BibTeX/export/Missing JabRef pattern; authorsN+initials #553.bibtex")]
        [DataRow("Better-BibTeX/export/Month showing up in year field on export #889.biblatex")]
        [DataRow("Better-BibTeX/export/Multiple locations and-or publishers and BibLaTeX export #689.biblatex")]
        [DataRow("Better-BibTeX/export/No booktitle field when exporting references from conference proceedings #1069.bibtex")]
        [DataRow("Better-BibTeX/export/No brace protection when suppressTitleCase set to true #1188.bibtex")]
        [DataRow("Better-BibTeX/export/No space between author first and last name because last char of first name is translated to a latex command #1091.bibtex")]
        [DataRow("Better-BibTeX/export/Non-ascii in dates is not matched by date parser #376.biblatex")]
        [DataRow("Better-BibTeX/export/Normalize date ranges in citekeys #356.biblatex")]
        [DataRow("Better-BibTeX/export/Numbers confuse capital-preservation #295.bibtex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.default.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.groups3.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.prefer-DOI.biblatex")]
        [DataRow("Better-BibTeX/export/Omit URL export when DOI present. #131.prefer-url.biblatex")]
        [DataRow("Better-BibTeX/export/Open date range crashes citekey generator #1227.bibtex")]
        [DataRow("Better-BibTeX/export/Oriental dates trip up date parser #389.biblatex")]
        [DataRow("Better-BibTeX/export/Protect math sections #1148.biblatex")]
        //[DataRow("Better-BibTeX/export/Really Big whopping library.bib")]
        [DataRow("Better-BibTeX/export/References with multiple notes fail to export #174.biblatex")]
        [DataRow("Better-BibTeX/export/Replicate Zotero key algorithm #439.bibtex")]
        [DataRow("Better-BibTeX/export/Season ranges should be exported as pseudo-months (13-16, or 21-24) #860.biblatex")]
        [DataRow("Better-BibTeX/export/Set IDS field when merging references with different citation keys #1221.biblatex")]
        [DataRow("Better-BibTeX/export/Setting the item type via the cheater syntax #587.biblatex")]
        [DataRow("Better-BibTeX/export/Setting the item type via the cheater syntax #587.bibtex")]
        [DataRow("Better-BibTeX/export/Shortjournal does not get exported to biblatex format #102 - biblatexcitekey #105.biblatex")]
        [DataRow("Better-BibTeX/export/Sorting and optional particle handling #411.off.biblatex")]
        [DataRow("Better-BibTeX/export/Sorting and optional particle handling #411.on.biblatex")]
        [DataRow("Better-BibTeX/export/Spaces not stripped from citation keys #294.biblatex")]
        [DataRow("Better-BibTeX/export/Square brackets in Publication field (85), and non-pinned keys must change when the pattern does.bibtex")]
        [DataRow("Better-BibTeX/export/Suppress brace protection #1139.biblatex")]
        [DataRow("Better-BibTeX/export/Thin space in author name #859.biblatex")]
        [DataRow("Better-BibTeX/export/Title case of latex greek text on biblatex export #564.biblatex")]
        [DataRow("Better-BibTeX/export/Treat dash-connected words as a single word for citekey generation #619.biblatex")]
        [DataRow("Better-BibTeX/export/Unbalanced vphantom escapes #1043-mathmode.bibtex")]
        [DataRow("Better-BibTeX/export/Unbalanced vphantom escapes #1043.bibtex")]
        [DataRow("Better-BibTeX/export/Underscores break capital-preservation #300.bibtex")]
        [DataRow("Better-BibTeX/export/[authN_M] citation key syntax has off-by-one error #899.bibtex")]
        [DataRow("Better-BibTeX/export/arXiv identifiers in BibLaTeX export #460.biblatex")]
        [DataRow("Better-BibTeX/export/arXiv identifiers in BibLaTeX export #460.bibtex")]
        [DataRow("Better-BibTeX/export/auth leaves punctuation in citation key #310.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.after.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.after.coll.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.before.biblatex")]
        [DataRow("Better-BibTeX/export/auto-export.before.coll.biblatex")]
        [DataRow("Better-BibTeX/export/automatic tags in export #1270.bibtex")]
        [DataRow("Better-BibTeX/export/biblatex export of Presentation; Use type and venue fields #644.biblatex")]
        [DataRow("Better-BibTeX/export/biblatex; Language tag xx is exported, xx-XX is not #380.biblatex")]
        [DataRow("Better-BibTeX/export/bibtex export of phdthesis does not case-protect -type- #435.bibtex")]
        [DataRow("Better-BibTeX/export/bibtex; url export does not survive underscores #402.biblatex")]
        [DataRow("Better-BibTeX/export/bibtex; url export does not survive underscores #402.bibtex")]
        [DataRow("Better-BibTeX/export/bookSection is always converted to @inbook, never @incollection #282.biblatex")]
        [DataRow("Better-BibTeX/export/braces after textemdash followed by unicode #980.bibtex")]
        [DataRow("Better-BibTeX/export/capital delta breaks .bib output #141.bibtex")]
        [DataRow("Better-BibTeX/export/citekey firstpage-lastpage #1147.bibtex")]
        [DataRow("Better-BibTeX/export/condense in cite key format not working #308.biblatex")]
        [DataRow("Better-BibTeX/export/creating a key with [authForeIni] and [authN] not working properly #892.bibtex")]
        [DataRow("Better-BibTeX/export/csquotes #302.biblatex")]
        [DataRow("Better-BibTeX/export/custom fields should be exported as-is #441.bibtex")]
        [DataRow("Better-BibTeX/export/customized fields with curly brackets are not exported correctly anymore #775.biblatex")]
        [DataRow("Better-BibTeX/export/date and year are switched #406.biblatex")]
        [DataRow("Better-BibTeX/export/date not always parsed properly into month and year with PubMed #1112.bibtex")]
        [DataRow("Better-BibTeX/export/date ranges #747+#746.biblatex")]
        [DataRow("Better-BibTeX/export/date ranges #747+#746.bibtex")]
        [DataRow("Better-BibTeX/export/don't escape entry key fields for #296.biblatex")]
        [DataRow("Better-BibTeX/export/error on exporting note with pre tags; duplicate field howpublished #1092.bibtex")]
        [DataRow("Better-BibTeX/export/export missing the a accent #691.biblatex")]
        [DataRow("Better-BibTeX/export/italics in title - capitalization #541.biblatex")]
        [DataRow("Better-BibTeX/export/key migration.biblatex")]
        [DataRow("Better-BibTeX/export/map csl-json variables #293.biblatex")]
        [DataRow("Better-BibTeX/export/markup small-caps, superscript, italics #301.biblatex")]
        [DataRow("Better-BibTeX/export/pre not working in Extra field #559.biblatex")]
        [DataRow("Better-BibTeX/export/preserve @strings between import-export #1162.biblatex")]
        [DataRow("Better-BibTeX/export/preserve @strings between import-export #1162.bibtex")]
        [DataRow("Better-BibTeX/export/preserve BibTeX Variables does not check for null values while escaping #337.bibtex")]
        [DataRow("Better-BibTeX/export/referencetype= does not work #278.biblatex")]
        [DataRow("Better-BibTeX/export/remove the field if the override is empty #303.biblatex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.biblatex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.bibtex")]
        [DataRow("Better-BibTeX/export/suppressBraceProtection does not work for BibTeX export (non-English items) #1194.sbp.bibtex")]
        [DataRow("Better-BibTeX/export/thesis zotero entries always create @phdthesis bibtex entries #307.biblatex")]
        [DataRow("Better-BibTeX/export/thesis zotero entries always create @phdthesis bibtex entries #307.bibtex")]
        [DataRow("Better-BibTeX/export/titles are title-cased in .bib file #558.bibtex")]
        [DataRow("Better-BibTeX/export/transliteration for citekey #580.biblatex")]
        [DataRow("Better-BibTeX/export/two ISSN number are freezing browser #110 + Generating keys and export broken #111.biblatex")]
        [DataRow("Better-BibTeX/export/typo stature-statute (zotero item type) #284.biblatex")]
        [DataRow("Better-BibTeX/export/underscores in URL fields should not be escaped #104.biblatex")]
        [DataRow("Better-BibTeX/export/urldate when only DOI is exported #869.biblatex")]
        [DataRow("Better-BibTeX/export/veryshorttitle and compound words #551.bibtex")]
        [DataRow("Better-BibTeX/import/Async import, large library #720.bib")]
        [DataRow("Better-BibTeX/import/Author splitter failure.bib")]
        [DataRow("Better-BibTeX/import/Better BibLaTeX import improvements #549.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX Import 2.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.001.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.003.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.004.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.005.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.006.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.008.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.009.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.010.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.011.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.012.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.014.bib")]
        [DataRow("Better-BibTeX/import/Better BibTeX.015.bib")]
        [DataRow("Better-BibTeX/import/BibLaTeX Patent author handling, type #1060.bib")]
        [DataRow("Better-BibTeX/import/BibTeX import; preamble with def create problems #732.bib")]
        [DataRow("Better-BibTeX/import/Biblatex Annotation Import Bug #613.bib")]
        [DataRow("Better-BibTeX/import/Endnote should parse.bib")]
        [DataRow("Better-BibTeX/import/Failure to handle unparsed author names (92).bib")]
        [DataRow("Better-BibTeX/import/Import Jabref fileDirectory, unexpected reference type #1058.bib")]
        [DataRow("Better-BibTeX/import/Import fails to perform @String substitutions #154.bib")]
        [DataRow("Better-BibTeX/import/Import location to event-place for conference papers.bib")]
        [DataRow("Better-BibTeX/import/Issues with round instead of curly braces do not import correctly #871.bib")]
        [DataRow("Better-BibTeX/import/Jabref groups import does not work #717.2.10.bib")]
        [DataRow("Better-BibTeX/import/Jabref groups import does not work #717.3.8.bib")]
        [DataRow("Better-BibTeX/import/Literal names.bib")]
        [DataRow("Better-BibTeX/import/Maintain the JabRef group and subgroup structure when importing a BibTeX db #97.bib")]
        [DataRow("Better-BibTeX/import/Math formatting lost on import #627.bib")]
        [DataRow("Better-BibTeX/import/Math markup to unicode not always imported correctly #472.bib")]
        [DataRow("Better-BibTeX/import/Math markup to unicode not always imported correctly #472.roundtrip.bib")]
        [DataRow("Better-BibTeX/import/Problem when importing BibTeX entries with percent sign #95 or preamble #96.bib")]
        [DataRow("Better-BibTeX/import/Problem when importing BibTeX entries with square brackets #94.bib")]
        [DataRow("Better-BibTeX/import/Some bibtex entries quietly discarded on import from bib file #873.bib")]
        [DataRow("Better-BibTeX/import/Spaces lost when expanding string variables during import #1081.bib")]
        [DataRow("Better-BibTeX/import/Wrong ring-above import #1115.bib")]
        [DataRow("Better-BibTeX/import/eprinttype field dropped on import #959.bib")]
        [DataRow("Better-BibTeX/import/importing a title-cased bib #1246.bib")]
        [DataRow("Better-BibTeX/import/importing a title-cased bib #1246.roundtrip.bib")]
        [DataRow("Better-BibTeX/import/space after citekey creates confusion #716.bib")]
        [DataRow("Better-BibTeX/import/support Local-Zo-Url-x field from BibDesk2Zotero_attachments #667.bib")]
        [DataRow("Better-BibTeX/import/zbb (quietly) chokes on this .bib #664.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_BetterBibTeX_Output(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("Better-BibTeX/export/Really Big whopping library.bib")]
        [DataTestMethod]
        public void Check_How_We_Fare_With_Big_Whopper_Database(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("biber/annotations.bib")]
        [DataRow("biber/bibtex-aliases.bib")]
        [DataRow("biber/crossrefs.bib")]
        [DataRow("biber/datalists.bib")]
        [DataRow("biber/dateformats.bib")]
        [DataRow("biber/definitions.bib")]
        [DataRow("biber/dm-constraints.bib")]
        [DataRow("biber/encoding1.bib")]
        [DataRow("biber/encoding2.bib")]
        [DataRow("biber/encoding3.bib")]
        [DataRow("biber/encoding4.bib")]
        [DataRow("biber/encoding5.bib")]
        [DataRow("biber/encoding6.bib")]
        [DataRow("biber/examples.bib")]
        [DataRow("biber/extradate.bib")]
        [DataRow("biber/extratitle.bib")]
        [DataRow("biber/extratitleyear.bib")]
        [DataRow("biber/full-bbl.bib")]
        [DataRow("biber/full-bibtex_biber.bib")]
        [DataRow("biber/full-dot.bib")]
        [DataRow("biber/labelalpha.bib")]
        [DataRow("biber/labelalphaname.bib")]
        [DataRow("biber/names.bib")]
        [DataRow("biber/names_x.bib")]
        [DataRow("biber/options.bib")]
        [DataRow("biber/papers.bib")]
        [DataRow("biber/related.bib")]
        [DataRow("biber/sections1.bib")]
        [DataRow("biber/sections2.bib")]
        [DataRow("biber/sections3.bib")]
        [DataRow("biber/sections4.bib")]
        [DataRow("biber/sets.bib")]
        [DataRow("biber/skips.bib")]
        [DataRow("biber/skipsg.bib")]
        [DataRow("biber/sort.bib")]
        [DataRow("biber/sort-case.bib")]
        [DataRow("biber/sort-order.bib")]
        [DataRow("biber/sort-uc.bib")]
        [DataRow("biber/tool.bib")]
        [DataRow("biber/translit.bib")]
        [DataRow("biber/truncation.bib")]
        [DataRow("biber/tugboat.bib")]
        [DataRow("biber/uniqueness1.bib")]
        [DataRow("biber/uniqueness2.bib")]
        [DataRow("biber/uniqueness3.bib")]
        [DataRow("biber/uniqueness4.bib")]
        [DataRow("biber/uniqueness5.bib")]
        [DataRow("biber/uniqueness6.bib")]
        [DataRow("biber/uniqueness-nameparts.bib")]
        [DataRow("biber/xdata.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_Biber(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("biblatex/95-customlists.bib")]
        [DataRow("biblatex/97-annotations.bib")]
        [DataRow("biblatex/biblatex-examples.bib")]
        [DataRow("biblatex-examples.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_BibLaTeX(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("bibtex-original-btxdoc.bib")]
        [DataRow("bibtex-original-xampl.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_Original_BibTeX_Specification(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("btparse/commas.bib")]
        [DataRow("btparse/comment.bib")]
        [DataRow("btparse/corpora.bib")]
        [DataRow("btparse/empty.bib")]
        [DataRow("btparse/errors.bib")]
        [DataRow("btparse/foreign.bib")]
        [DataRow("btparse/macro.bib")]
        [DataRow("btparse/preamble.bib")]
        [DataRow("btparse/regular.bib")]
        [DataRow("btparse/sample-0001.bib")]
        [DataRow("btparse/sample-0002.bib")]
        [DataRow("btparse/sample-0003.bib")]
        [DataRow("btparse/simple.bib")]
        [DataRow("btparse/unlimited.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_BTParse(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("comment-0001.bib")]
        [DataRow("comment-0002.bib")]
        [DataRow("comment-0003.bib")]
        [DataRow("comment-0004.bib")]
        [DataRow("comment-0005.bib")]
        [DataRow("comment-0006.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Comment_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("concatenation-0001.bib")]
        [DataRow("concatenation-0002.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Concatenation_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("CrossTeX/comments.bib")]
        [DataRow("CrossTeX/diss.bib")]
        [DataRow("CrossTeX/sample-0001.bib")]
        [DataRow("CrossTeX/sample-0002.bib")]
        [DataRow("CrossTeX/sample-0003.bib")]
        [DataRow("CrossTeX/sample-0004.bib")]
        [DataRow("CrossTeX/sample-0005.bib")]
        [DataRow("CrossTeX/sample-0006.bib")]
        [DataRow("CrossTeX/sample-0007.bib")]
        [DataRow("CrossTeX/sample-0008.bib")]
        [DataRow("CrossTeX/sample-0009.bib")]
        [DataRow("CrossTeX/sample-comment-0001.bib")]
        [DataRow("CrossTeX/sample-comment-0002.bib")]
        [DataRow("CrossTeX/sample-comment-0003.bib")]
        [DataRow("CrossTeX/styles.bib")]
        [DataRow("CrossTeX/url.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_CrossTeX(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("IEEEtran/IEEEabrv.bib")]
        [DataRow("IEEEtran/IEEEexample.bib")]
        [DataRow("IEEEtran/IEEEfull.bib")]
        [DataRow("IEEEtran/sample-from-website1.bib")]
        [DataTestMethod]
        public void Check_How_We_Compare_Against_IEEEtran(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("issue-0072-b0rked-01.bib")]
        [DataTestMethod]
        public void Check_Issue0072(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("macros-0001.bib")]
        [DataRow("macros-0002.bib")]
        [DataRow("macros-0003.bib")]
        [DataRow("macros-0004.bib")]
        [DataRow("macros-0005.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Macro_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("misc-0001.bib")]
        [DataRow("misc-0002.bib")]
        [DataRow("misc-0003.bib")]
        [DataRow("misc-0004.bib")]
        [DataRow("misc-0005.bib")]
        [DataRow("misc-0006.bib")]
        [DataRow("misc-0007.bib")]
        [DataRow("misc-0008.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Misc_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("multiple-records-0001.bib")]
        [DataTestMethod]
        public void Check_Multiple_BibTeX_Record_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("non-ASCII-characters-0001.bib")]
        [DataRow("non-ASCII-characters-0002.bib")]
        [DataRow("non-ASCII-characters-0003.bib")]
        [DataRow("non-ASCII-characters-0004.bib")]
        [DataTestMethod]
        public void Check_BibTeX_NonASCII_Content_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("oddities-0001.bib")]
        [DataRow("oddities-0002.bib")]
        [DataRow("oddities-0003.bib")]
        [DataRow("oddities-0006.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Oddities_In_Input(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("punctuation-0001.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Punctuation_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("simple-0001.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Basic(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("TeX-accented-letters-0001.bib")]
        [DataRow("TeX-accented-letters-0002.bib")]
        [DataRow("TeX-accented-letters-0003.bib")]
        [DataRow("TeX-accented-letters-0004.bib")]
        [DataRow("TeX-accented-letters-0005.bib")]
        [DataRow("TeX-accented-letters-0006.bib")]
        [DataRow("TeX-accented-letters-0007.bib")]
        [DataRow("TeX-accented-letters-0008.bib")]
        [DataRow("TeX-accented-letters-0009.bib")]
        [DataRow("TeX-accented-letters-0010.bib")]
        [DataRow("TeX-accented-letters-0011.bib")]
        [DataTestMethod]
        public void Check_BibTeX_AccentedChracters_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }

        [DataRow("whitespacing-0001.bib")]
        [DataRow("whitespacing-0002.bib")]
        [DataRow("whitespacing-0003.bib")]
        [DataTestMethod]
        public void Check_BibTeX_Whitespace_Handling(string bibtex_filepath)
        {
            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

            // Serialize the result to JSON for easier comparison via ApprovalTests->BeyondCompare (that's what I use for *decades* now)
            string json_out = JsonConvert.SerializeObject(rv, Formatting.Indented).Replace("\r\n", "\n");
            //ApprovalTests.Approvals.VerifyJson(json_out);   --> becomes the code below:
            ApprovalTests.Approvals.Verify(
                new QiqqaApprover(json_out, bibtex_filepath),
                ApprovalTests.Approvals.GetReporter()
            );
        }
    }
}
