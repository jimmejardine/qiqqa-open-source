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

using System;
using System.Collections.Generic;

namespace Utilities.Misc
{
	/// <summary>
	/// A class demonstrating a CommandManager.InvalidateRequery-like event.
	/// </summary>
	public class WeakReferenceToDelegate
	{
		List<WeakReference> handlers = new List<WeakReference>();
		
		public event EventHandler Event {
			add {
				// event is easily made thread-safe is desired
				lock (handlers) {
					// optionally clean up when adding an event handler to prevent leak
					// when event is never fired:
					handlers.RemoveAll(wr => !wr.IsAlive);
					
					handlers.Add(new WeakReference(value));
				}
			}
			remove {
				lock (handlers) {
					handlers.RemoveAll(wr => {
					                   	EventHandler target = (EventHandler)wr.Target;
					                   	return target == null || target == value;
					                   });
				}
			}
		}
		
		void FireEvent(EventArgs e)
		{
			EventHandler callHandlers = null;
			lock (handlers) {
				for (int i = handlers.Count - 1; i >= 0; i--) {
					EventHandler target = (EventHandler)handlers[i].Target;
					if (target == null)
						handlers.RemoveAt(i);
					else
						callHandlers += target;
				}
			}
			// Call event handlers using a separate event handler list after removing the dead entries
			// from the old list to ensure that registering+deregistering events from within an event
			// handler works as expected.
			if (callHandlers != null)
				callHandlers(this, e);
		}
	}
}
