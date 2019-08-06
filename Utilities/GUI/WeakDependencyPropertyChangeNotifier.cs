using System;
using System.Windows;
using System.Windows.Data;

namespace Utilities.GUI
{
    public sealed class WeakDependencyPropertyChangeNotifier : DependencyObject, IDisposable
    {
        // We need this - even though it will not be used
        public event EventHandler ValueChanged;

        #region --- Lifetime management -----------------------------------------------------------------------

        public WeakDependencyPropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property) : this(propertySource, new PropertyPath(property))
        {
        }

        public WeakDependencyPropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
            {
                throw new ArgumentNullException("propertySource");
            }

            if (null == property)
            {
                throw new ArgumentNullException("property");
            }

            this._propertySource = new WeakReference(propertySource);

            Binding binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        ~WeakDependencyPropertyChangeNotifier()
        {
            Logging.Info("~WeakDependencyPropertyChangeNotifier()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Info("Disposing WeakDependencyPropertyChangeNotifier");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            Logging.Debug("WeakDependencyPropertyChangeNotifier::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
            if (disposing)
            {
                BindingOperations.ClearBinding(this, ValueProperty);
            }

            // Get rid of unmanaged resources 
        }

        #endregion --- Lifetime management -----------------------------------------------------------------------

        #region --- Properties -----------------------------------------------------------------------

        private WeakReference _propertySource;
        public DependencyObject PropertySource
        {
            get
            {
                DependencyObject target = this._propertySource.Target as DependencyObject;
                return target;
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(WeakDependencyPropertyChangeNotifier), new FrameworkPropertyMetadata(null, OnValuePropertyChanged));
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeakDependencyPropertyChangeNotifier notifier = (WeakDependencyPropertyChangeNotifier)d;

            if (null != notifier.ValueChanged)
            {
                notifier.ValueChanged(notifier.PropertySource, EventArgs.Empty);
            }
        }

        #endregion --- Properties -----------------------------------------------------------------------
    }
}