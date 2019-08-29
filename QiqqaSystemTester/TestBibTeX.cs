using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Utilities.BibTex;

namespace QiqqaSystemTester
{
    [TestClass]
    class TestBibTeX
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void Test1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test2()
        {
            //            throw new Exception("bugger");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void EnsureBibTeXEntryTypesDefListIsLoadedAndParsed()
        {
            // force a load of the types.
            EntryTypes.ResetForTesting();
            EntryTypes t = Utilities.BibTex.EntryTypes.Instance;
            Assert.IsNotNull(t);
            Assert.IsTrue(t.FieldTypeList.Count >= 18, "expected to load a full set of BibTeX entry type specs");
        }
    }
}
