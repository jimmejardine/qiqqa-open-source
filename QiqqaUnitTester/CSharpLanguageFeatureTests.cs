using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester.Fundamentals
{
    /// <summary>
    /// Because I'm not 200% fluent in everything C# and sometimes, when coming here from another programming language I wonder...
    /// </summary>
    [TestClass]
    public class CSharpLanguageFeatureTests
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

        private int NullableTypeTestHelperA(int? v = null)
        {
            // a Nullable type can be NULL. Of course. (Well, I doubted at some point, but know now that the Qiqqa code has... issues with this?
            //ASSERT.IsNotNull(v);

            try
            {
                // HOWEVER, you *CAN* invoke `HasValue` on the NULLed `v`, without having to check for `v == null` first nor using the v?.HasValuee idiom.  :-S
                return v.HasValue ? v.Value : -7;
            }
            catch (Exception ex)
            {
                return -3;
            }
        }

        // Mark that this is a unit test method. (Required)
        [TestMethod]
        public void NullableTypeBehaviour()
        {
            ASSERT.AreEqual<int>(NullableTypeTestHelperA(), -7);
            ASSERT.AreEqual<int>(NullableTypeTestHelperA(4), 4);
            ASSERT.AreEqual<int>(NullableTypeTestHelperA(null), -7);
        }

        private int TypecastToObjectAllowsNULLCheckHelper<T>(T item)
        {
            object o = (object)item;

            return TypecastToObjectAllowsNULLCheckHelperO(o);
        }

        private int TypecastToObjectAllowsNULLCheckHelperO(object obj)
        {
            return obj == null ? -7 : 3;
        }

        public void TypecastToObjectAllowsNULLCheck()
        {
            ASSERT.AreEqual<int>(TypecastToObjectAllowsNULLCheckHelper<int>(5), 3);
            ASSERT.AreEqual<int>(TypecastToObjectAllowsNULLCheckHelper<string>("help"), 3);
            ASSERT.AreEqual<int>(TypecastToObjectAllowsNULLCheckHelper<string>(null), -7);

            int? nullable_int = null;
            int? nullable_int2 = 4;

            ASSERT.AreEqual<int>(TypecastToObjectAllowsNULLCheckHelper<int?>(nullable_int), -7);
            ASSERT.AreEqual<int>(TypecastToObjectAllowsNULLCheckHelper<int?>(nullable_int2), 3);
        }


        [TestMethod]
        public void TestMethod1()
        {
            ASSERT.IsTrue(true);
        }
    }
}


