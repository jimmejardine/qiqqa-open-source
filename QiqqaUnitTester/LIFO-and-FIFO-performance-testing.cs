using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QiqqaUnitTester.Fundamentals
{
    [TestClass]
    public class LIFO_and_FIFO_performance_testing
    {
        // https://stackoverflow.com/questions/4786884/how-can-i-write-output-from-a-unit-test
        //
        // Notes:
        // the TestContext *setter* below is mandatory or the test WILL NOT EXECUTE. Turns out some unit test framework code
        // calls this setter under the hood (stack trace is empty when called).
        //
        // - Debug.WriteLine only outputs in DEBUG builds so is the least advisable to use.
        // - Trace.WriteLine works.
        // - Console.WriteLine also works.
        // - TestContext?.WriteLine (note the '?' in there, just in case...) also works.
        //
        // Sticking with Console.WriteLine as that's the most universal employable of them all.

        private static TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Executes once for the test class. (Optional)
            testContextInstance = context;

            Trace.WriteLine("init test trace");
            Debug.WriteLine("init test debug");
            testContextInstance?.WriteLine("init test context");
            Console.WriteLine("init test console");
        }

        [TestInitialize]
        public void Setup()
        {
            // Runs before each test. (Optional)
            Trace.WriteLine("setup test trace");
            Debug.WriteLine("setup test debug");
            TestContext?.WriteLine("setup test context");
            Console.WriteLine("setup test console");
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
            Trace.WriteLine("teardown test trace");
            Debug.WriteLine("teardown test debug");
            TestContext?.WriteLine("teardown test context");
            Console.WriteLine("teardown test console");
        }

        // Expected output under "additional output for this result" link in the Test Explorer:
        //
        // ```
        // setup test console
        // test console
        // teardown test console
        // 
        // 
        // Debug Trace:
        // setup test trace
        // setup test debug
        // I am using dot net debugging
        // I am using dot net tracing
        // test trace
        // test debug
        // teardown test trace
        // teardown test debug
        // 
        // 
        // TestContext Messages:
        // setup test context
        // test context
        // teardown test context
        // ```
        //
        [TestMethod]
        public void TestOutputDuringTestRunMethods()
        {
            Debug.WriteLine("I am using dot net debugging");
            Trace.WriteLine("I am using dot net tracing");

            Trace.WriteLine("test trace");
            Debug.WriteLine("test debug");
            TestContext?.WriteLine("test context");
            Console.WriteLine("test console");
        }

        [TestMethod]
        public void PerformanceBenchmarkTest()
        {
            // Your test code goes here.
            Collections4Test.run(TestContext);
        }
    }

    internal static class Collections4Test
    {
        public static void run(TestContext TestContext)
        {
            Random rand = new Random();
            Stopwatch sw = new Stopwatch();
            Stack<int> stack = new Stack<int>();
            Queue<int> queue = new Queue<int>();
            List<int> list1 = new List<int>();
            List<int> list2 = new List<int>();
            LinkedList<int> linkedlist1 = new LinkedList<int>();
            LinkedList<int> linkedlist2 = new LinkedList<int>();
            int dummy;

            sw.Reset();
            Console.Write("Pushing to Stack...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                stack.Push(rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("Poping from Stack...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = stack.Pop();
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("Enqueue to Queue...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                queue.Enqueue(rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("Dequeue from Queue...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = queue.Dequeue();
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("Insert to List at the front...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                list1.Insert(0, rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("RemoveAt from List at the front...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = list1[0];
                list1.RemoveAt(0);
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("Add to List at the end...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                list2.Add(rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("RemoveAt from List at the end...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = list2[list2.Count - 1];
                list2.RemoveAt(list2.Count - 1);
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("AddFirst to LinkedList...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                linkedlist1.AddFirst(rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("RemoveFirst from LinkedList...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = linkedlist1.First.Value;
                linkedlist1.RemoveFirst();
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("AddLast to LinkedList...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                linkedlist2.AddLast(rand.Next());
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("RemoveLast from LinkedList...");
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                dummy = linkedlist2.Last.Value;
                linkedlist2.RemoveLast();
                dummy++;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks\n", sw.ElapsedTicks);
        }
    }
}

