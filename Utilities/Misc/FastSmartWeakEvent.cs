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
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SmartWeakEvent
{
	/// <summary>
	/// A class for managing a weak event.
	/// </summary>
	public sealed class FastSmartWeakEvent<T> where T : class
	{
		struct EventEntry
		{
			public readonly FastSmartWeakEventForwarderProvider.ForwarderDelegate Forwarder;
			public readonly MethodInfo TargetMethod;
			public readonly WeakReference TargetReference;
			
			public EventEntry(FastSmartWeakEventForwarderProvider.ForwarderDelegate forwarder, MethodInfo targetMethod, WeakReference targetReference)
			{
				this.Forwarder = forwarder;
				this.TargetMethod = targetMethod;
				this.TargetReference = targetReference;
			}
		}
		
		readonly List<EventEntry> eventEntries = new List<EventEntry>();
		
		static FastSmartWeakEvent()
		{
			if (!typeof(T).IsSubclassOf(typeof(Delegate)))
				throw new ArgumentException("T must be a delegate type");
			MethodInfo invoke = typeof(T).GetMethod("Invoke");
			if (invoke == null || invoke.GetParameters().Length != 2)
				throw new ArgumentException("T must be a delegate type taking 2 parameters");
			ParameterInfo senderParameter = invoke.GetParameters()[0];
			if (senderParameter.ParameterType != typeof(object))
				throw new ArgumentException("The first delegate parameter must be of type 'object'");
			ParameterInfo argsParameter = invoke.GetParameters()[1];
			if (!(typeof(EventArgs).IsAssignableFrom(argsParameter.ParameterType)))
				throw new ArgumentException("The second delegate parameter must be derived from type 'EventArgs'");
			if (invoke.ReturnType != typeof(void))
				throw new ArgumentException("The delegate return type must be void.");
		}
		
		public void Add(T eh)
		{
			if (eh != null) {
				Delegate d = (Delegate)(object)eh;
				if (eventEntries.Count == eventEntries.Capacity)
					RemoveDeadEntries();
				MethodInfo targetMethod = d.Method;
				object targetInstance = d.Target;
				WeakReference target = targetInstance != null ? new WeakReference(targetInstance) : null;
				eventEntries.Add(new EventEntry(FastSmartWeakEventForwarderProvider.GetForwarder(targetMethod), targetMethod, target));
			}
		}
		
		void RemoveDeadEntries()
		{
			eventEntries.RemoveAll(ee => ee.TargetReference != null && !ee.TargetReference.IsAlive);
		}
		
		public void Remove(T eh)
		{
			if (eh != null) {
				Delegate d = (Delegate)(object)eh;
				object targetInstance = d.Target;
				MethodInfo targetMethod = d.Method;
				for (int i = eventEntries.Count - 1; i >= 0; i--) {
					EventEntry entry = eventEntries[i];
					if (entry.TargetReference != null) {
						object target = entry.TargetReference.Target;
						if (target == null) {
							eventEntries.RemoveAt(i);
						} else if (target == targetInstance && entry.TargetMethod == targetMethod) {
							eventEntries.RemoveAt(i);
							break;
						}
					} else {
						if (targetInstance == null && entry.TargetMethod == targetMethod) {
							eventEntries.RemoveAt(i);
							break;
						}
					}
				}
			}
		}
		
		public void Raise(object sender, EventArgs e)
		{
			bool needsCleanup = false;
			foreach (EventEntry ee in eventEntries.ToArray()) {
				needsCleanup |= ee.Forwarder(ee.TargetReference, sender, e);
			}
			if (needsCleanup)
				RemoveDeadEntries();
		}
	}
	
	// The forwarder-generating code is in a separate class because it does not depend on type T.
	static class FastSmartWeakEventForwarderProvider
	{
		static readonly MethodInfo getTarget = typeof(WeakReference).GetMethod("get_Target");
		static readonly Type[] forwarderParameters = { typeof(WeakReference), typeof(object), typeof(EventArgs) };
		internal delegate bool ForwarderDelegate(WeakReference wr, object sender, EventArgs e);
		
		static readonly Dictionary<MethodInfo, ForwarderDelegate> forwarders = new Dictionary<MethodInfo, ForwarderDelegate>();
		
		internal static ForwarderDelegate GetForwarder(MethodInfo method)
		{
			lock (forwarders) {
				ForwarderDelegate d;
				if (forwarders.TryGetValue(method, out d))
					return d;
			}
			
			if (method.DeclaringType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
				throw new ArgumentException("Cannot create weak event to anonymous method with closure.");
			var parameters = method.GetParameters();
			
			Debug.Assert(getTarget != null);
			
			DynamicMethod dm = new DynamicMethod(
				"FastSmartWeakEvent", typeof(bool), forwarderParameters, method.DeclaringType);
			
			ILGenerator il = dm.GetILGenerator();
			
			if (!method.IsStatic) {
				il.Emit(OpCodes.Ldarg_0);
				il.EmitCall(OpCodes.Callvirt, getTarget, null);
				il.Emit(OpCodes.Dup);
				Label label = il.DefineLabel();
				il.Emit(OpCodes.Brtrue, label);
				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Ret);
				il.MarkLabel(label);
				// The castclass here is required for the generated code to be verifiable.
				// We can leave it out because we know this cast will always succeed
				// (the instance/method pair was taken from a delegate).
				// Unverifiable code is fine because private reflection is only allowed under FullTrust
				// anyways.
				//il.Emit(OpCodes.Castclass, method.DeclaringType);
			}
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldarg_2);
			// This castclass here is required to prevent creating a hole in the .NET type system.
			// See Program.TypeSafetyProblem in the 'SmartWeakEventBenchmark' to see the effect when
			// this cast is not used.
			// You can remove this cast if you trust add FastSmartWeakEvent.Raise callers to do
			// the right thing, but the small performance increase (about 5%) usually isn't worth the risk.
			il.Emit(OpCodes.Castclass, parameters[1].ParameterType);
			
			il.EmitCall(OpCodes.Call, method, null);
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Ret);
			
			ForwarderDelegate fd = (ForwarderDelegate)dm.CreateDelegate(typeof(ForwarderDelegate));
			lock (forwarders) {
				forwarders[method] = fd;
			}
			return fd;
		}
	}
}
