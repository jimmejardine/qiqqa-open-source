// Copyright (c) 2008 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

// reference: https://www.codeproject.com/Articles/29922/Weak-Events-in-C

using System;
using System.Diagnostics.CodeAnalysis;

namespace Utilities.Misc
{
	/// <summary>
	/// Helper class to add weak handlers to events of type System.EventHandler.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public sealed class WeakEventHandler
	{
		/// <summary>
		/// Registers an event handler that works with a weak reference to the target object.
		/// Access to the event and to the real event handler is done through lambda expressions.
		/// The code holds strong references to these expressions, so they must not capture any
		/// variables!
		/// </summary>
		/// <example>
		/// <code>
		/// WeakEventHandler.Register(
		/// 	textDocument,
		/// 	(d, eh) => d.Changed += eh,
		/// 	(d, eh) => d.Changed -= eh,
		/// 	this,
		/// 	(me, sender, args) => me.OnDocumentChanged(sender, args)
		/// );
		/// </code>
		/// </example>
		public static WeakEventHandler Register<TEventSource, TEventListener>(
			TEventSource senderObject,
			Action<TEventSource, EventHandler> registerEvent,
			Action<TEventSource, EventHandler> deregisterEvent,
			TEventListener listeningObject,
			Action<TEventListener, object, EventArgs> forwarderAction
		)
			where TEventSource : class where TEventListener : class
		{
			if (senderObject == null)
				throw new ArgumentNullException("senderObject");
			if (listeningObject == null)
				throw new ArgumentNullException("listeningObject");
			VerifyDelegate(registerEvent, "registerEvent");
			VerifyDelegate(deregisterEvent, "deregisterEvent");
			VerifyDelegate(forwarderAction, "forwarderAction");
			
			WeakEventHandler weh = new WeakEventHandler(listeningObject);
			EventHandler eh = weh.MakeDeregisterCodeAndWeakEventHandler(senderObject, deregisterEvent, forwarderAction);
			registerEvent(senderObject, eh);
			return weh;
		}
		
		internal static void VerifyDelegate(Delegate d, string parameterName)
		{
			if (d == null)
				throw new ArgumentNullException(parameterName);
			if (!d.Method.IsStatic)
				throw new ArgumentException("Delegates used for WeakEventHandler must not capture any variables (must point to static methods)", parameterName);
		}
		
		internal readonly WeakReference listeningReference;
		internal Action deregisterCode;
		
		internal WeakEventHandler(object listeningObject)
		{
			this.listeningReference = new WeakReference(listeningObject);
		}
		
		EventHandler MakeDeregisterCodeAndWeakEventHandler
			<TEventSource, TEventListener>
			(
				TEventSource senderObject,
				Action<TEventSource, EventHandler> deregisterEvent,
				Action<TEventListener, object, EventArgs> forwarderAction
			)
			where TEventSource : class where TEventListener : class
		{
			EventHandler eventHandler = (sender, args) => {
				TEventListener listeningObject = (TEventListener)listeningReference.Target;
				if (listeningObject != null) {
					forwarderAction(listeningObject, sender, args);
				} else {
					Deregister();
				}
			};
			
			deregisterCode = delegate {
				deregisterEvent(senderObject, eventHandler);
			};
			
			return eventHandler;
		}
		
		/// <summary>
		/// Deregisters the event handler.
		/// </summary>
		public void Deregister()
		{
			if (deregisterCode != null) {
				deregisterCode();
				deregisterCode = null;
			}
		}
	}
	
	/// <summary>
	/// Helper class to add weak handlers to events of type System.EventHandler{A}.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public static class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
	{
		/// <summary>
		/// Registers an event handler that works with a weak reference to the target object.
		/// Access to the event and to the real event handler is done through lambda expressions.
		/// The code holds strong references to these expressions, so they must not capture any
		/// variables!
		/// </summary>
		/// <example>
		/// <code>
		/// WeakEventHandler&lt;DocumentChangeEventArgs&gt;.Register(
		/// 	textDocument,
		/// 	(d, eh) => d.Changed += eh,
		/// 	(d, eh) => d.Changed -= eh,
		/// 	this,
		/// 	(me, sender, args) => me.OnDocumentChanged(sender, args)
		/// );
		/// </code>
		/// </example>
		public static WeakEventHandler Register<TEventSource, TEventListener>(
			TEventSource senderObject,
			Action<TEventSource, EventHandler<TEventArgs>> registerEvent,
			Action<TEventSource, EventHandler<TEventArgs>> deregisterEvent,
			TEventListener listeningObject,
			Action<TEventListener, object, TEventArgs> forwarderAction
		)
			where TEventSource : class where TEventListener : class
		{
			if (senderObject == null)
				throw new ArgumentNullException("senderObject");
			if (listeningObject == null)
				throw new ArgumentNullException("listeningObject");
			WeakEventHandler.VerifyDelegate(registerEvent, "registerEvent");
			WeakEventHandler.VerifyDelegate(deregisterEvent, "deregisterEvent");
			WeakEventHandler.VerifyDelegate(forwarderAction, "forwarderAction");
			
			WeakEventHandler weh = new WeakEventHandler(listeningObject);
			EventHandler<TEventArgs> eh = MakeDeregisterCodeAndWeakEventHandler(weh, senderObject, deregisterEvent, forwarderAction);
			registerEvent(senderObject, eh);
			return weh;
		}
		
		static EventHandler<TEventArgs> MakeDeregisterCodeAndWeakEventHandler
			<TEventSource, TEventListener>
			(
				WeakEventHandler weh,
				TEventSource senderObject,
				Action<TEventSource, EventHandler<TEventArgs>> deregisterEvent,
				Action<TEventListener, object, TEventArgs> forwarderAction
			)
			where TEventSource : class where TEventListener : class
		{
			EventHandler<TEventArgs> eventHandler = (sender, args) => {
				TEventListener listeningObject = (TEventListener)weh.listeningReference.Target;
				if (listeningObject != null) {
					forwarderAction(listeningObject, sender, args);
				} else {
					weh.Deregister();
				}
			};
			
			weh.deregisterCode = delegate {
				deregisterEvent(senderObject, eventHandler);
			};
			
			return eventHandler;
		}
	}
}
