using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using Utilities.BibTex;

namespace QiqqaUnitTester
{
    class TestBibTeX
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            //            throw new Exception("bugger");
            Assert.Pass();
        }

        [Test]
        public void EnsureBibTeXEntryTypesDefListIsLoadedAndParsed()
        {
            // force a load of the types.
            EntryTypes.ResetForTesting();
            EntryTypes t = Utilities.BibTex.EntryTypes.Instance;
            Assert.IsNotNull(t);
            Assert.GreaterOrEqual(t.FieldTypeList.Count, 18, "expected to load a full set of BibTeX entry type specs");
        }
    }
}
