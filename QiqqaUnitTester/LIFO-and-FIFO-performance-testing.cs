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

        internal class LifoElement
        {
            public string a = "buggerit";
            public int v1 = 42;
            public string b = "millenium hand & shrimp";
        }

        [TestMethod]
        public void PerformanceBenchmarkTest()
        {
            // Your test code goes here.
            const int limit = 4000000;
            Stopwatch sw = new Stopwatch();
            Stack<LifoElement> stack = new Stack<LifoElement>();
            Queue<LifoElement> queue = new Queue<LifoElement>();
            List<LifoElement> list1 = new List<LifoElement>();
            List<LifoElement> list2 = new List<LifoElement>();
            LinkedList<LifoElement> linkedlist1 = new LinkedList<LifoElement>();
            LinkedList<LifoElement> linkedlist2 = new LinkedList<LifoElement>();
            int dummy;
            LifoElement[] data = new LifoElement[limit];
            for (int i = 0; i < limit; i++)
            {
                data[i] = new LifoElement();
                data[i].v1 = i;
            }

            sw.Reset();
            Console.Write("{0,40}  ", "stack.Push");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                stack.Push(data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "stack.Pop");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                stack.Pop();
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("{0,40}  ", "queue.Enqueue");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                queue.Enqueue(data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "queue.Dequeue");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                queue.Dequeue();
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);

#if false   // these are extremely slow operations!

            sw.Reset();
            Console.Write( "{0,40}  ", "Insert to List at the front..." );
            sw.Start();
            for ( int i = 0; i < limit; i++ ) {
              list1.Insert( 0, data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            
            sw.Reset();
            Console.Write( "{0,40}  ", "RemoveAt from List at the front..." );
            sw.Start();
            for ( int i = 0; i < limit; i++ ) {
              list1.RemoveAt( 0 );
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);

#endif

            sw.Reset();
            Console.Write("{0,40}  ", "list2.Add");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                list2.Add(data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "list2.RemoveAt(END)");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                list2.RemoveAt(list2.Count - 1);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);

#if false   // these are extremely slow operations!

            sw.Reset();
            Console.Write("{0,40}  ", "list2.Insert(START)");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                list2.Insert(0, data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "list2.RemoveAt(START)");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                list2.RemoveAt(0);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);

#endif

            sw.Reset();
            Console.Write("{0,40}  ", "linkedlist1.AddFirst");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                linkedlist1.AddFirst(data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "linkedlist1.RemoveFirst");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                linkedlist1.RemoveFirst();
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);


            sw.Reset();
            Console.Write("{0,40}  ", "linkedlist2.AddLast");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                linkedlist2.AddLast(data[i]);
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
            sw.Reset();
            Console.Write("{0,40}  ", "linkedlist2.RemoveLast");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                linkedlist2.RemoveLast();
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);


            // Fill again
            for (int i = 0; i < limit; i++)
            {
                list2.Add(data[i]);
            }
            sw.Reset();
            Console.Write("{0,40}  ", "list2[i]");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                dummy = list2[i].v1;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);


            // Fill array
            sw.Reset();
            Console.Write("{0,40}  ", "FillArray");
            sw.Start();
            var array = new LifoElement[limit];
            for (int i = 0; i < limit; i++)
            {
                array[i] = data[i];
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);

            sw.Reset();
            Console.Write("{0,40}  ", "array[i]");
            sw.Start();
            for (int i = 0; i < limit; i++)
            {
                dummy = array[i].v1;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);


            // Fill again
            for (int i = 0; i < limit; i++)
            {
                linkedlist1.AddFirst(data[i]);
            }
            sw.Reset();
            Console.Write("{0,40}  ", "foreach_linkedlist1");
            sw.Start();
            foreach (var item in linkedlist1)
            {
                dummy = item.v1;
            }
            sw.Stop();
            Console.WriteLine("  Time used: {0,9} ticks", sw.ElapsedTicks);
        }
    }
}

