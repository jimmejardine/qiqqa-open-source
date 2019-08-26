using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
            throw new Exception("bugger");
        }
    }
}
