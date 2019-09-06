using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using static QiqqaTestHelpers.MiscTestHelpers;
using Utilities;
using Utilities.BibTex;
using Utilities.BibTex.Parsing;

namespace QiqqaSystemTester
{
    [TestClass]
    public class TestBibTeX
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [DataRow("all-caps-0001.bib", DisplayName = "all caps property / properties")]
        [DataRow("ampersand-0001.bib", DisplayName = "ampersand...")]
        [DataRow("ampersand-0002.bib", DisplayName = "double-escaped ampersand = b0rk?")]
        [DataRow("ampersand-0003.bib", DisplayName = "un-escaped ampersand = OK?")]
        [DataRow("b0rked-0001.bib")]
        [DataRow("b0rked-0002.bib")]
        [DataRow("b0rked-0003.bib", DisplayName = "`@` missing")]
        [DataRow("b0rked-0004.bib", DisplayName = "trailing comma after last property")]
        [DataRow("b0rked-0005.bib", DisplayName = "multiple commas between two properties")]
        [DataRow("b0rked-0006.bib", DisplayName = "no comma separating properties")]
        [DataRow("b0rked-0007.bib", DisplayName = "no comma after key")]
        [DataRow("biblatex-examples.bib")]
        [DataRow("bibtex-original-btxdoc.bib")]
        [DataRow("bibtex-original-xampl.bib")]
        [DataRow("comment-0001.bib", DisplayName = "`@comment` entries")]
        [DataRow("concatenation-0001.bib", DisplayName = "the `#` concatenation operator")]
        [DataRow("misc-0002.bib", DisplayName = "multiple properties on a single line")]
        [DataRow("misc-0004.bib", DisplayName = "multiple properties on a single line")]
        [DataRow("misc-0005.bib", DisplayName = "author-split with semicolon instead of `and`")]
        [DataRow("multiple-records-0001.bib")]
        [DataRow("non-ASCII-characters-0001.bib")]
        [DataRow("non-ASCII-characters-0002.bib", DisplayName = "apostrophe is non-ASCII here")]
        [DataRow("non-ASCII-characters-0003.bib", DisplayName = "105 degrees...")]
        [DataRow("non-ASCII-characters-0004.bib", DisplayName = "Chinese...")]
        [DataRow("oddities-0001.bib", DisplayName = "odd stuff as part of a title property or other")]
        [DataRow("oddities-0002.bib", DisplayName = "extremely long 'words' as part of a property: bad data?")]
        [DataRow("oddities-0003.bib", DisplayName = "quotes strings inside curly braces = bad data?")]
        [DataRow("oddities-0006.bib", DisplayName = "URI as part of another property: bad data?")]
        [DataRow("punctuation-0001.bib", DisplayName = "superfluous punctuation at the end of a title")]
        [DataRow("simple-0001.bib")]
        [DataRow("TeX-accented-letters-0001.bib")]
        [DataRow("TeX-accented-letters-0002.bib", DisplayName = "advanced \\k escape, etc.")]
        [DataRow("TeX-accented-letters-0003.bib", DisplayName = "advanced (or b0rked?) escapes")]
        [DataRow("TeX-accented-letters-0004.bib", DisplayName = "advanced escapes")]
        [DataRow("TeX-accented-letters-0005.bib", DisplayName = "advanced escapes")]
        [DataRow("TeX-accented-letters-0006.bib", DisplayName = "advanced escapes")]
        [DataRow("TeX-accented-letters-0007.bib", DisplayName = "accented letter without curly braces = b0rk?")]
        [DataRow("TeX-accented-letters-0008.bib", DisplayName = "advanced escapes")]
        [DataRow("TeX-accented-letters-0009.bib", DisplayName = "accented letter without curly braces = b0rk?")]
        [DataRow("TeX-accented-letters-0010.bib", DisplayName = "more accents")]
        [DataRow("TeX-accented-letters-0011.bib", DisplayName = "more accents")]
        [DataRow("whitespacing-0001.bib", DisplayName = "initial whitespace + multi-spaces within `author`")]
        [DataRow("whitespacing-0002.bib", DisplayName = "initial whitespace + multi-spaces within `author`")]
        [DataTestMethod]
        public void Do_TestFiles_Exist(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);
        }

        [DataRow("simple-0001.bib", DisplayName = "a simple BibTeX record")]
        [DataTestMethod]
        public void SimpleTest(string bibtex_filepath)
        {
            ASSERT.IsTrue(true);

            string path = GetNormalizedPathToBibTeXTestFile(bibtex_filepath);
            ASSERT.FileExists(path);

            string data_in = GetBibTeXTestFileContent(path);
            BibTexParseResult rv = BibTexParser.Parse(data_in);

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
    }
}
