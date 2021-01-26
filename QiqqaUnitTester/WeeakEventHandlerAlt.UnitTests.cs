using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QiqqaTestHelpers;
using Utilities.Misc;

// https://www.automatetheplanet.com/mstest-cheat-sheet/

namespace QiqqaUnitTester.A
{
    using Wrapper = WeakEventHandler2;

    [TestClass]
    public class WeakEventHandlerTest
    {
        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void WrapInReflectionCall_T001()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Wrapper.Wrap(null));
        }

        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void WrapInReflectionCall_T002()
        {
            EventHandler expected = EventListener.HandleStaticRaised;

            var actual = Wrapper.Wrap(expected);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void WrapInReflectionCall_T003()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap(test.Listener.HandleRaised);

            test.Raiser.Raise();

            Assert.IsTrue(test.Result.RaisedCalled);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void WrapInReflectionCall_T004()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap(test.Listener.HandleRaised);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            test.Raiser.Raise();

            Assert.IsFalse(test.Result.RaisedCalled);
        }

        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void WrapInReflectionCall_TArgs_T001()
        {
            EventHandler<RaisedIdEventArgs> handler = null;

            Assert.ThrowsException<ArgumentNullException>(() => Wrapper.Wrap(handler));
        }

        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void WrapInReflectionCall_TArgs_T002()
        {
            EventHandler<RaisedIdEventArgs> expected = EventListener.HandleStaticRaisedId;

            var actual = Wrapper.Wrap(expected);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void WrapInReflectionCall_TArgs_T003()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs>(
                test.Listener.HandleRaisedId);

            test.Raiser.RaiseId(1);

            Assert.IsTrue(test.Result.RaisedIdCalled);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void WrapInReflectionCall_TArgs_T004()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs>(
                test.Listener.HandleRaisedId);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            test.Raiser.RaiseId(1);

            Assert.IsFalse(test.Result.RaisedIdCalled);
        }

        [TestMethod]
        [Description("SHOULD NOT affect event data passed to the handler.")]
        public void WrapInReflectionCall_TArgs_T005()
        {
            var test = new EventTestContext();

            const int Id = 1;

            EventHandler<RaisedIdEventArgs> handler = (sender, e) => Assert.AreEqual(Id, e.Id);

            test.Raiser.RaisedId += Wrapper.Wrap(handler);

            test.Raiser.RaiseId(Id);
        }

        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void Wrap_T001()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Wrapper.Wrap(null));
        }

        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void Wrap_T002()
        {
            EventHandler expected = EventListener.HandleStaticRaised;

            var actual = Wrapper.Wrap(expected);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void Wrap_T003()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap(test.Listener.HandleRaised);

            test.Raiser.Raise();

            Assert.IsTrue(test.Result.RaisedCalled);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void Wrap_T004()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap(test.Listener.HandleRaised);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            test.Raiser.Raise();

            Assert.IsFalse(test.Result.RaisedCalled);
        }

#if false
        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void Wrap_TListener_T001()
        {
            EventHandler handler = null;

            Assert.ThrowsException<ArgumentNullException>(() => Wrapper.Wrap<EventListener>(handler));
        }
#endif

#if false
        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void Wrap_TListener_T002()
        {
            EventHandler expected = EventListener.HandleStaticRaised;

            var actual = Wrapper.Wrap<EventListener>(expected);

            Assert.AreEqual(expected, actual);
        }
#endif

#if false
        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void Wrap_TListener_T003()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap<EventListener>(test.Listener.HandleRaised);

            test.Raiser.Raise();

            Assert.IsTrue(test.Result.RaisedCalled);
        }
#endif

#if false
        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void Wrap_TListener_T004()
        {
            var test = new EventTestContext();

            test.Raiser.Raised += Wrapper.Wrap<EventListener>(
                test.Listener.HandleRaised);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(test.Result.RaisedCalled);
        }
#endif

#if false
        [TestMethod]
        [Description("An exception SHOULD be thrown on type mismatch between compile time listener type and runtime type of handler target.")]
        public void Wrap_TListener_T005()
        {
            var test = new EventTestContext();

            Assert.ThrowsException<ArgumentException>(
                () => Wrapper.Wrap<EventRaiser>(test.Listener.HandleRaised));
        }
#endif

        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void Wrap_TArgs_T001()
        {
            EventHandler<RaisedIdEventArgs> handler = null;

            Assert.ThrowsException<ArgumentNullException>(() => Wrapper.Wrap(handler));
        }

        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void Wrap_TArgs_T002()
        {
            EventHandler<RaisedIdEventArgs> expected = EventListener.HandleStaticRaisedId;

            var actual = Wrapper.Wrap(expected);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void Wrap_TArgs_T003()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs>(
                test.Listener.HandleRaisedId);

            test.Raiser.RaiseId(1);

            Assert.IsTrue(test.Result.RaisedIdCalled);
        }

        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void Wrap_TArgs_T004()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs>(
                test.Listener.HandleRaisedId);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            test.Raiser.RaiseId(1);

            Assert.IsFalse(test.Result.RaisedIdCalled);
        }

        [TestMethod]
        [Description("SHOULD NOT affect event data passed to the handler.")]
        public void Wrap_TArgs_T005()
        {
            var test = new EventTestContext();

            const int Id = 1;

            EventHandler<RaisedIdEventArgs> handler = (sender, e) => Assert.AreEqual(Id, e.Id);

            test.Raiser.RaisedId += Wrapper.Wrap(handler);

            test.Raiser.RaiseId(Id);
        }

#if false
        [TestMethod]
        [Description("An exception SHOULD be thrown if the provided handler is a null reference.")]
        public void Wrap_TArgs_TListener_T001()
        {
            EventHandler<RaisedIdEventArgs> handler = null;

            Assert.ThrowsException<ArgumentNullException>(
                () => Wrapper.Wrap<RaisedIdEventArgs, EventListener>(handler));
        }
#endif

#if false
        [TestMethod]
        [Description("SHOULD return exact same handler if provided with a static method handler.")]
        public void Wrap_TArgs_TListener_T002()
        {
            EventHandler<RaisedIdEventArgs> expected = EventListener.HandleStaticRaisedId;

            var actual = Wrapper.Wrap<RaisedIdEventArgs, EventListener>(expected);

            Assert.AreEqual(expected, actual);
        }
#endif

#if false
        [TestMethod]
        [Description("Wrapped handler SHOULD be invoked if listener instance is not disposed.")]
        public void Wrap_TArgs_TListener_T003()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs, EventListener>(
                test.Listener.HandleRaisedId);

            test.Raiser.RaiseId(1);

            Assert.IsTrue(test.Result.RaisedIdCalled);
        }
#endif

#if false
        [TestMethod]
        [Description("Wrapped handler SHOULD NOT be invoked if listener instance is disposed.")]
        public void Wrap_TArgs_TListener_T004()
        {
            var test = new EventTestContext();

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs, EventListener>(
                test.Listener.HandleRaisedId);

            test.Listener = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            test.Raiser.RaiseId(1);

            Assert.IsFalse(test.Result.RaisedIdCalled);
        }
#endif

#if false
        [TestMethod]
        [Description("An exception SHOULD be thrown on type mismatch between compile time listener type and runtime type of handler target.")]
        public void Wrap_TArgs_TListener_T005()
        {
            var test = new EventTestContext();

            Assert.ThrowsException<ArgumentException>(
                () => Wrapper.Wrap<RaisedIdEventArgs, EventRaiser>(
                    test.Listener.HandleRaisedId));
        }
#endif

#if false
        [TestMethod]
        [Description("SHOULD NOT affect event data passed to the handler.")]
        public void Wrap_TArgs_TListener_T006()
        {
            var test = new EventTestContext();

            const int Id = 1;

            EventHandler<RaisedIdEventArgs> handler = (sender, e) => Assert.AreEqual(Id, e.Id);

            test.Raiser.RaisedId += Wrapper.Wrap<RaisedIdEventArgs, EventListener>(handler);

            test.Raiser.RaiseId(Id);
        }
#endif
    }

    internal class EventTestContext
    {
        public EventTestContext()
        {
            this.Raiser = new EventRaiser();
            this.Result = new EventListenerResult();
            this.Listener = new EventListener(this.Result);
        }

        public EventRaiser Raiser { get; set; }

        public EventListener Listener { get; set; }

        public EventListenerResult Result { get; set; }
    }

    internal class EventListenerResult
    {
        public bool RaisedCalled { get; set; }

        public bool RaisedIdCalled { get; set; }
    }

    internal class EventListener
    {
        public EventListener(EventListenerResult result)
        {
            this.Result = result;
        }

        public EventListenerResult Result { get; set; }

        public void HandleRaised(object sender, EventArgs e)
        {
            this.Result.RaisedCalled = true;
        }

        public void HandleRaisedId(object sender, RaisedIdEventArgs e)
        {
            this.Result.RaisedIdCalled = true;
        }

        public static void HandleStaticRaised(object sender, EventArgs e) { }

        public static void HandleStaticRaisedId(object sender, RaisedIdEventArgs e) { }
    }

    internal class RaisedIdEventArgs : EventArgs
    {
        public int Id { get; set; }
    }

    internal class EventRaiser
    {
        public event EventHandler Raised;

        public event EventHandler<RaisedIdEventArgs> RaisedId;

        public void Raise() { this.OnRaised(EventArgs.Empty); }

        public void RaiseId(int id) { this.OnRaisedId(new RaisedIdEventArgs { Id = id }); }

        protected virtual void OnRaised(EventArgs e)
        {
            var handler = this.Raised;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnRaisedId(RaisedIdEventArgs e)
        {
            var handler = this.RaisedId;

            if (handler != null)
                handler(this, e);
        }
    }
}
