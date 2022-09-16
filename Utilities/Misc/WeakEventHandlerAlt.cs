// In a few editions of my book, I showed how to implement weak event handlers which allow an object to be GCd
// if no other reference is keeping the object alive. If the object is alive, it recieves the event notification
// and if it isn’t alive, it doesn’t receive the event notification.
// It has come to my attention that the code I show in my book is incorrect and does not work as advertised.
// I have corrected that code and the working version is shown below:

// https://www.wintellect.com/weak-event-handlers/
// https://stackoverflow.com/questions/12879650/what-is-the-bug-in-this-weakeventhandler

using System;
using System.Reflection;

namespace Utilities.Misc
{
    public static class WeakEventHandler2
    {
        private class WeakEventHandlerImpl
        {
            protected readonly WeakReference m_wrTarget; // WeakReference to original delegate’s target object
            protected Delegate m_openEventHandler;       // "Open" delegate to invoke original target’s delegate method

            public WeakEventHandlerImpl(Delegate d)
            {
                m_wrTarget = new WeakReference(d.Target);
            }

            // Match is used to compare a WeakEventHandlerImpl object with an actual delegate.
            // Typically used to remove a WeakEventHandlerImpl from an event collection.
            public Boolean Match(Delegate strongEventHandler)
            {
                // Returns true if original target & method match the WeakEventHandlerImpl’s Target & method
                return (m_wrTarget.Target == strongEventHandler.Target) && (m_openEventHandler.Method == strongEventHandler.Method);
            }
        }

        // "Open" delegate definition to quickly invoke original delegate’s callback
        private delegate void OpenEventHandler<TTarget, TEventArgs>(TTarget target, Object sender, TEventArgs eventArgs)
           where TTarget : class
           where TEventArgs : EventArgs;

        // A proxy object that knows how to invoke a callback on an object if it hasn’t been GC’d
        private sealed class WeakEventHandlerImpl<TEventHandler> : WeakEventHandlerImpl where TEventHandler : class
        {
            // Refers to a method that removes a delegate to this proxy object once we know the original target has been GC’d
            private readonly Action<TEventHandler> m_cleanup;

            // This is the delegate passed to m_cleanup that needs to be removed from an event
            private readonly TEventHandler m_proxyHandler;

            public static TEventHandler Create(TEventHandler eh, Action<TEventHandler> cleanup = null)
            {
                //Contract.Requires(eh != null && cleanup != null);
                if (eh == null)
                {
                    throw new ArgumentNullException("exception handler parameter must not be null");
                }

                // We don’t create weak events for static methods since types don’t get GC’d
                Delegate d = (Delegate)(Object)eh;  // We know that all event handlers are derived from Delegate
                if (d.Target == null) return eh;

                var weh = new WeakEventHandlerImpl<TEventHandler>(d, cleanup);
                return weh.m_proxyHandler; // Return the delegate to add to the event
            }

            private WeakEventHandlerImpl(Delegate d, Action<TEventHandler> cleanup = null) : base(d)
            {
                m_cleanup = cleanup;

                Type targetType = d.Target.GetType();
                Type eventHandlerType = typeof(TEventHandler);
                Type eventArgsType = eventHandlerType.IsGenericType
                   ? eventHandlerType.GetGenericArguments()[0]
                   : eventHandlerType.GetMethod("Invoke").GetParameters()[1].ParameterType;

                // Create a delegate to the ProxyInvoke method; this delegate is registered with the event
                var miProxy = typeof(WeakEventHandlerImpl<TEventHandler>)
                   .GetMethod("ProxyInvoke", BindingFlags.Instance | BindingFlags.NonPublic)
                   .MakeGenericMethod(targetType, eventArgsType);
                m_proxyHandler = (TEventHandler)(Object)Delegate.CreateDelegate(eventHandlerType, this, miProxy);

                // Create an "open" delegate to the original delegate’s method; ProxyInvoke calls this
                Type openEventHandlerType = typeof(OpenEventHandler<,>).MakeGenericType(d.Target.GetType(), eventArgsType);
                m_openEventHandler = Delegate.CreateDelegate(openEventHandlerType, null, d.Method);
            }

            private void ProxyInvoke<TTarget, TEventArgs>(Object sender, TEventArgs e)
               where TTarget : class
               where TEventArgs : EventArgs
            {
                // If the original target object still exists, call it; else call m_cleanup to unregister our delegate with the event
                TTarget target = (TTarget)m_wrTarget.Target;
                if (target != null)
                {
                    ((OpenEventHandler<TTarget, TEventArgs>)m_openEventHandler)(target, sender, e);
                }
                else if (m_cleanup != null)
                {
                    m_cleanup(m_proxyHandler);
                }
                else
                {
                    //m_cleanup(m_proxyHandler);
                }
            }
        }

        // We offer this overload because it is so common
        public static EventHandler Wrap(EventHandler eh, Action<EventHandler> cleanup = null)
        {
            return WeakEventHandlerImpl<EventHandler>.Create(eh, cleanup);
        }
        public static TEventHandler Wrap<TEventHandler>(TEventHandler eh, Action<TEventHandler> cleanup = null) where TEventHandler : class
        {
            return WeakEventHandlerImpl<TEventHandler>.Create(eh, cleanup);
        }
        public static EventHandler<TEventArgs> Wrap<TEventArgs>(EventHandler<TEventArgs> eh, Action<EventHandler<TEventArgs>> cleanup = null) where TEventArgs : EventArgs
        {
            return WeakEventHandlerImpl<EventHandler<TEventArgs>>.Create(eh, cleanup);
        }
        public static Boolean Match(Delegate WeakEventHandler2, Delegate strongEventHandler)
        {
            return ((WeakEventHandlerImpl)WeakEventHandler2.Target).Match(strongEventHandler);
        }
    }
}

// To use it, instead of registering an event callback like this:
//      someButton.Click += o.ClickHandler;
// Do this:
//      someButton.Click += WeakEventHandler2.Wrap(o.ClickHandler, eh => someButton.Click -= eh);

