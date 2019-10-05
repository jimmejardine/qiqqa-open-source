using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaPreProductionReleaseTester
{
    // This file is a result of https://github.com/jimmejardine/qiqqa-open-source/issues/92 : 
    //
    // there's only one way to prevent regressions: testing the binaries of both builds from any test environment.

    //[TestCategory("Edit Tests"), TestCategory("Non-Smoke")]
    [TestClass]
    class DebugAndRelaseBinariesExecTest
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
        }

        [TestInitialize]
        public void Setup()
        {
            // Runs before each test. (Optional)
        }

        [ClassCleanup]
        public static void TestFixtureTearDown()
        {
            // Runs once after all tests in this class are executed. (Optional)
            // Not guaranteed that it executes instantly after all tests from the class.
        }

        [TestCleanup]
        public void TearDown()
        {
            // Runs after each test. (Optional)
        }

        // Mark that this is a unit test method. (Required)
        [TestMethod]
        public void ExecQiqqaOCRDebugBuildTestMethod()
        {
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void ExecQiqqaOCRReleaseBuildTestMethod()
        {
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void ExecQiqqaDebugBuildTestMethod()
        {
            ASSERT.IsTrue(true);
        }

        [TestMethod]
        public void ExecQiqqaReleaseBuildTestMethod()
        {
            ASSERT.IsTrue(true);
        }
    }
}

