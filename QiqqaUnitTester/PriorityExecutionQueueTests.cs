using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using Utilities.Collections;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

// NOTE: See also `LIFO_and_FIFO_performance_testing` performance test

namespace QiqqaUnitTester.Fundamentals
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class PriorityExecutionQueueTests
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
        public void SimpleLIFOBehaviour()
        {
            ASSERT.AreEqual<int>(-1, -7);
        }

        [TestMethod]
        public void TestMethod1()
        {
            ASSERT.IsTrue(true);
        }
    }
}


