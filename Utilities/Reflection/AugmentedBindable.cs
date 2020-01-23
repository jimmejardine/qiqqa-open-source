using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Utilities.GUI;

namespace Utilities.Reflection
{
    public class AugmentedBindable<T> : ICustomTypeDescriptor, INotifyPropertyChanged
    {
        private T underlying;
        private Type underlying_type;
        private PropertyDependencies property_dependencies;

        public AugmentedBindable(T underlying, PropertyDependencies property_dependencies)
        {
            this.underlying = underlying;
            this.property_dependencies = property_dependencies;

            underlying_type = underlying.GetType();
        }

        public AugmentedBindable(T underlying) : this(underlying, null)
        {
        }

        public T Underlying => underlying;

        #region --- INotifyPropertyChanged ------------------------------------------------------------------

        private class CallbackWrapper
        {
            internal WeakReference object_to_callback;
            public MethodInfo method_to_call;

        }

        private List<CallbackWrapper> callback_wrappers = new List<CallbackWrapper>();
        private object callback_wrappers_lock = new object();

        /// <summary>
        /// This is a "special" event that registers binding requests as usual, but creates weak references back to the registered party, so they will not be rooted by this binding.
        /// Do NOT rely on this callback keeping your objects alive from garbage collection - it is SPECIFICALLY intended not to...
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                CallbackWrapper callback_wrapper = new CallbackWrapper();
                callback_wrapper.object_to_callback = new WeakReference(value.Target);
                callback_wrapper.method_to_call = value.Method;

                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (callback_wrappers_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    callback_wrappers.Add(callback_wrapper);
                }
            }

            remove
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (callback_wrappers_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    for (int i = callback_wrappers.Count - 1; i >= 0; --i)
                    {
                        if (value.Target == callback_wrappers[i].object_to_callback.Target && value.Method == callback_wrappers[i].method_to_call)
                        {
                            callback_wrappers.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private static PropertyChangedEventArgs NULL_PROPERTY_CHANGED_EVENT_ARGS = new PropertyChangedEventArgs(String.Empty);

        private void FirePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (callback_wrappers_lock)
            {
                //l1_clk.LockPerfTimerStop();
                if (0 < callback_wrappers.Count)
                {
                    object[] parameters = new object[] { sender, e };
                    for (int i = callback_wrappers.Count - 1; i >= 0; --i)
                    {
                        object target = callback_wrappers[i].object_to_callback.Target;
                        if (null != target)
                        {
                            callback_wrappers[i].method_to_call.Invoke(target, parameters);
                        }
                        else
                        {
                            Logging.Debug特("Removing garbage collected callback");
                            callback_wrappers.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void NotifyPropertyChanged()
        {
            FirePropertyChanged(this, null);
        }

        /// <summary>
        /// Always call this using the form xyz.NotifyPropertyChanged(() => yyy.ZZZ), where ZZZ is the property that just got updated.
        /// That way the compiler will catch any property name changes.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="property"></param>
        private void NotifyPropertyChanged<U>(Expression<Func<U>> property)
        {
            string property_name = PropertyNames.Get<U>(property);
            NotifyPropertyChanged(property_name);
        }

        /// <summary>
        /// Always call this using the form xyz.NotifyPropertyChanged(nameof(yyy.ZZZ)), where ZZZ is the property that just got updated.
        /// That way the compiler will catch any property name changes.
        /// </summary>
        /// <param name="property_name"></param>
        public void NotifyPropertyChanged(string property_name)
        {
            // if (Application.Current == null || Application.Current.Dispatcher.Thread == Thread.CurrentThread)
            // as per: https://stackoverflow.com/questions/5143599/detecting-whether-on-ui-thread-in-wpf-and-winforms#answer-14280425
            // and: https://stackoverflow.com/questions/2982498/wpf-dispatcher-the-calling-thread-cannot-access-this-object-because-a-differen/13726324#13726324
            WPFDoEvents.InvokeInUIThread(() =>
            {
                NotifyPropertyChanged_THREADSAFE(property_name);
            });
        }

        private void NotifyPropertyChanged_THREADSAFE(string property_name)
        {
            FirePropertyChanged(this, property_name);
        }

        private void FirePropertyChanged(object sender, string property_name)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            if (null == property_name)
            {
                FirePropertyChangedEventHandler(sender, NULL_PROPERTY_CHANGED_EVENT_ARGS);
            }
            else
            {
                //Logging.Debug("Firing '{0}'", property_name);
                FirePropertyChangedEventHandler(sender, new PropertyChangedEventArgs(property_name));

                if (null != property_dependencies)
                {
                    foreach (string dependent_property_name in property_dependencies.GetDependencies(property_name))
                    {
                        //Logging.Debug("Firing dependent '{0}'", dependent_property_name);
                        FirePropertyChangedEventHandler(sender, new PropertyChangedEventArgs(dependent_property_name));
                    }
                }
            }
        }

        #endregion

        #region --- ICustomTypeDescriptor - interesting ---------------------------------------------------------

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(underlying, attributes, true);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection pdc_augmented = new PropertyDescriptorCollection(new PropertyDescriptor[] { });

            PropertyDescriptorCollection pdc_underlying = TypeDescriptor.GetProperties(underlying_type);
            foreach (PropertyDescriptor pd_underlying in pdc_underlying)
            {
                AugmentedPropertyDescriptorForProperties pd_augmented = new AugmentedPropertyDescriptorForProperties(pd_underlying);
                pdc_augmented.Add(pd_augmented);
            }

            return pdc_augmented;
        }

        private class AugmentedPropertyDescriptorForProperties : PropertyDescriptor
        {
            private PropertyDescriptor pd;

            public AugmentedPropertyDescriptorForProperties(PropertyDescriptor pd)
                : base(pd)
            {
                this.pd = pd;
            }

            public override bool CanResetValue(object component)
            {
                return pd.CanResetValue(((AugmentedBindable<T>)component).Underlying);
            }

            public override Type ComponentType => pd.ComponentType;

            public override object GetValue(object component)
            {
                try
                {
                    return pd.GetValue(((AugmentedBindable<T>)component).underlying);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public override bool IsReadOnly => pd.IsReadOnly;

            public override Type PropertyType => pd.PropertyType;

            public override void ResetValue(object component)
            {
                AugmentedBindable<T> c = ((AugmentedBindable<T>)component);

                pd.ResetValue(c.underlying);
                c.NotifyPropertyChanged(pd.Name);
            }

            public override void SetValue(object component, object value)
            {
                // This is a horrible hack because I can't work out why the syncfusion grid CancelEdits a row if you type something over a null value...
                if (null == value)
                {
                    Logging.Warn("Ignoring null SetValue - remember to check with Syncfusion");
                    return;
                }

                AugmentedBindable<T> c = ((AugmentedBindable<T>)component);

                pd.SetValue(c.underlying, value);
                c.NotifyPropertyChanged(pd.Name);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return pd.ShouldSerializeValue(((AugmentedBindable<T>)component).underlying);
            }
        }

        #endregion

        #region --- ICustomTypeDescriptor - boring ---------------------------------------------------------

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(underlying, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(underlying, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(underlying, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(underlying, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(underlying, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(underlying, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(underlying, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(underlying, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(underlying, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return underlying_type;
        }

        #endregion

        // ---------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return String.Format("AugmentedBindable[{0}]", underlying.ToString());
        }
    }
}
