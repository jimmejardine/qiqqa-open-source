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

            _propertySource = new WeakReference(propertySource);

            Binding binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        ~WeakDependencyPropertyChangeNotifier()
        {
            Logging.Debug("~WeakDependencyPropertyChangeNotifier()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing WeakDependencyPropertyChangeNotifier");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        internal /*virtual*/ void Dispose(bool disposing)    // sealed class doesn't allow 'virtual' + warning CS0628: 'WeakDependencyPropertyChangeNotifier.Dispose(bool)': new protected member declared in sealed class
        {
            Logging.Debug("WeakDependencyPropertyChangeNotifier::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (dispose_count == 0)
                {
                    WPFDoEvents.InvokeInUIThread(() =>
                    {
                        BindingOperations.ClearBinding(this, ValueProperty);
                    });
                }

                _propertySource = null;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;
        }

        #endregion --- Lifetime management -----------------------------------------------------------------------

        #region --- Properties -----------------------------------------------------------------------

        private WeakReference _propertySource;
        public DependencyObject PropertySource
        {
            get
            {
                DependencyObject target = _propertySource.Target as DependencyObject;
                return target;
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(WeakDependencyPropertyChangeNotifier), new FrameworkPropertyMetadata(null, OnValuePropertyChanged));
        public object Value
        {
            get => (object)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeakDependencyPropertyChangeNotifier notifier = (WeakDependencyPropertyChangeNotifier)d;

            notifier.ValueChanged?.Invoke(notifier.PropertySource, EventArgs.Empty);
        }

        #endregion --- Properties -----------------------------------------------------------------------
    }
}
