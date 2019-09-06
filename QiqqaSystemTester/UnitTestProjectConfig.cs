using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaSystemTester
{
    [TestClass]
    public class UnitTestProjectConfig
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // Executes once before the test run. (Optional)
        }
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
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Executes once after the test run. (Optional)
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

#if false
        // Mark that this is a unit test method. (Required)
        [TestMethod]
        public void YouTestMethod()
        {
            // Your test code goes here.
        }

        [TestMethod]
        public void TestMethod1()
        {
            ASSERT.IsTrue(true);
        }
#endif

        // ----------------------------------------------------------------------------

#if TEST
        [TestMethod]
        public void TEST_Has_Been_Defined_In_The_Project_Configuration()
        {
            ASSERT.Pass("TEST has been correctly defined in the QiqqaSystemTester project configuration.");
        }
#else
        [TestMethod]
        public void TEST_Has_Not_Been_Defined_In_The_Project_Configuration()
        {
            ASSERT.Fail("TEST has not been defined in the QiqqaSystemTester project configuration.");
        }
#endif
    }
}
