using System;
using System.Globalization;
using System.Windows.Data;

namespace Utilities.GUI
{
    /// <summary>
    /// This is used to have WPF controls re-read their values after setting them.  
    /// Especially important after binding to properties that have underlying validation where the value retained is not exactly the value set.
    /// In WPF4 this is no longer necessary...
    /// </summary>
    public class ReadPropertyAfterSetConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
