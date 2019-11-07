using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Qiqqa.InCite
{
    public class FilenameToCSLNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filename = (string)value;
            return Path.GetFileNameWithoutExtension(filename);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
