using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester
{
    [TestClass]
    public class AsyncFireAndForgetSimple
    {
        [TestInitialize]
        public void Setup()
        {
            caught_ex = null;
            completed = false;
            faulted = false;
            canceled = false;
        }

        // https://stackoverflow.com/questions/15522900/how-to-safely-call-an-async-method-in-c-sharp-without-await
        [TestMethod]
        public void TestCallAsyncFireAndForget_ImmediateThrow()
        {
            try
            {
                // output "hello world" as method returns early
                ASSERT.AreEqual(ExampleFireAndForgetFunc(), "hello world");
            }
            catch
            {
                // Exception is NOT caught here
                ASSERT.Fail("exception should not be caught here");
            }
            //ASSERT.IsNull(caught_ex);

            Thread.Sleep(500);

            ASSERT.IsNotNull(caught_ex);
            ASSERT.IsTrue(faulted, "faulted");
            ASSERT.IsTrue(completed, "completed");
            ASSERT.IsFalse(canceled, "cancelled");
        }

        [TestMethod]
        public void TestCallAsyncFireAndForget_ThrowAfterAWhile()
        {
            try
            {
                // output "hello world" as method returns early
                ASSERT.AreEqual(ExampleFireAndForgetFunc(() =>
                {
                    Thread.Sleep(100);
                    return true;  // do throw exception
                }), "hello world");
            }
            catch
            {
                // Exception is NOT caught here
                ASSERT.Fail("exception should not be caught here");
            }
            ASSERT.IsNull(caught_ex);

            Thread.Sleep(500);

            ASSERT.IsNotNull(caught_ex);
            ASSERT.IsTrue(faulted, "faulted");
            ASSERT.IsTrue(completed, "completed");
            ASSERT.IsFalse(canceled, "cancelled");
        }

        [TestMethod]
        public void TestCallAsyncFireAndForget_Completed()
        {
            try
            {
                // output "hello world" as method returns early
                ASSERT.AreEqual(ExampleFireAndForgetFunc(() =>
                {
                    Thread.Sleep(100);
                    return false;  // don't throw exception
                }), "hello world");
            }
            catch
            {
                // Exception is NOT caught here
                ASSERT.Fail("exception should not be caught here");
            }
            ASSERT.IsNull(caught_ex);

            Thread.Sleep(500);

            ASSERT.IsNull(caught_ex);
            ASSERT.IsFalse(faulted, "faulted");
            ASSERT.IsTrue(completed, "completed");
            ASSERT.IsFalse(canceled, "cancelled");
        }

        // --------------------------------------------------------------------------

        public string ExampleFireAndForgetFunc(Func<bool> exec = null)
        {
            MyAsyncMethod(exec)
                .ContinueWith(OnMyAsyncMethodFinishedAnyWay, TaskContinuationOptions.None);
            // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/chaining-tasks-by-using-continuation-tasks#continuation-options
            // --> the continuation handler must check / dispatch depending on IsCanceled / IsFaulted / IsCompleted; can't register
            //     different handlers for different completion options this way.
            //
            //.ContinueWith(OnMyAsyncMethodFailed, TaskContinuationOptions.OnlyOnFaulted)
            //.ContinueWith(OnMyAsyncMethodCancelled, TaskContinuationOptions.OnlyOnCanceled)
            //.ContinueWith(OnMyAsyncMethodCompleted, TaskContinuationOptions.OnlyOnRanToCompletion);

            return "hello world";
        }

        public async Task MyAsyncMethod(Func<bool> exec)
        {
            await Task.Run(() =>
            {
                if (exec?.Invoke() ?? true)
                {
                    throw new Exception("thrown on background thread");
                }
            });
        }

        private Exception caught_ex;
        private bool canceled;
        private bool completed;
        private bool faulted;

        public void OnMyAsyncMethodFinishedAnyWay(Task task)
        {
            if (task.IsFaulted)
            {
                OnMyAsyncMethodFailed(task);
            }
            else if (task.IsCanceled)
            {
                OnMyAsyncMethodCancelled(task);
            }
            else
            {
                OnMyAsyncMethodCompleted(task);
            }
        }

        public void OnMyAsyncMethodFailed(Task task)
        {
            Exception ex = task.Exception;

            // Deal with exceptions here however you want
            ASSERT.IsNotNull(ex);
            ASSERT.IsTrue(task.IsFaulted);
            ASSERT.IsTrue(task.IsCompleted);   // WARNING: FAULTED Tasks are also flagged as COMPLETED!
            ASSERT.IsFalse(task.IsCanceled);
            caught_ex = ex;
            faulted = task.IsFaulted;
            completed = task.IsCompleted;
            canceled = task.IsCanceled;
        }

        public void OnMyAsyncMethodCancelled(Task task)
        {
            Exception ex = task.Exception;
            ASSERT.IsFalse(task.IsFaulted);
            ASSERT.IsTrue(task.IsCompleted);
            ASSERT.IsTrue(task.IsCanceled, "Canceled task");
            ASSERT.IsNull(ex);
            caught_ex = ex;
            faulted = task.IsFaulted;
            completed = task.IsCompleted;
            canceled = task.IsCanceled;
        }

        public void OnMyAsyncMethodCompleted(Task task)
        {
            Exception ex = task.Exception;
            ASSERT.IsFalse(task.IsFaulted);
            ASSERT.IsTrue(task.IsCompleted, "Completed task");
            ASSERT.IsFalse(task.IsCanceled);
            ASSERT.IsNull(ex);
            caught_ex = ex;
            faulted = task.IsFaulted;
            completed = task.IsCompleted;
            canceled = task.IsCanceled;
        }


        // https://stackoverflow.com/questions/15439245/get-task-cancellationtoken#answer-54541233
        //public static CancellationToken GetCancellationToken(this Task task)
        //{
        //    return new TaskCanceledException(task).CancellationToken;
        //}
    }
}
