using System;
using System.Globalization;
using System.Windows.Data;

namespace Qiqqa.Common.GUI
{
    public class BoolToYesNoStringConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool boolValue = (bool)value;
                string strParameter = parameter as string;
                if (strParameter != null)
                {
                    string[] strValues = strParameter.Split(",".ToCharArray());
                    if (strValues.Length == 2)
                    {
                        return (boolValue == true) ? strValues[0] : strValues[1];
                    }
                }
                
                return boolValue ? "Yes" : "No";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (strValue != null)
            {
                string strParameter = parameter as string;
                if (strParameter != null)
                {
                    string[] strValues = strParameter.Split(",".ToCharArray());
                    if (strValues.Length == 2)
                    {
                        return (strValue == strValues[0]) ? true : false;
                    }
                }
                return false;
            }
            return false;
        }
        #endregion
    }
}
