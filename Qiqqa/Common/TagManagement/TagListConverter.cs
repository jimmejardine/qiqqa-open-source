using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Qiqqa.Common.TagManagement
{
    public class TagsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> items = value as List<string>;
            if (null == items)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items.Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append("; ");
                }

                sb.Append(items[i]);
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
