using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using Utilities.BibTex;

namespace QiqqaSystemTester
{
    [TestClass]
    public class TestBibTeX
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void Test1()
        {
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void Test2()
        {
            //            throw new Exception("bugger");
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void EnsureBibTeXEntryTypesDefListIsLoadedAndParsed()
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
