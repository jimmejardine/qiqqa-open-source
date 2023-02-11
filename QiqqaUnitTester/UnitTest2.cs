using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester.Example
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void TestThrowMethod()
        {
            ASSERT.ThrowsException(() =>
            {
                throw new System.Exception("test");
            });
        }
    }
}
