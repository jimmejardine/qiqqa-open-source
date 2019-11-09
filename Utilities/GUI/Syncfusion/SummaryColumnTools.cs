#if !HAS_NO_GUI

using System;
using System.Linq.Expressions;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Data;
using Utilities.Reflection;

namespace Utilities.GUI.Syncfusion
{
    public class SummaryColumnTools
    {
        public static GridDataSummaryColumn GetSummaryColumn<U>(Expression<Func<U>> property)
        {
            return GetSummaryColumn<U>(property, SummaryType.DoubleAggregate, "{Sum}", null, null);
        }

        public static GridDataSummaryColumn GetSummaryColumn<U>(Expression<Func<U>> property, SummaryType summary_type, string format, string summary_name)
        {
            return GetSummaryColumn<U>(property, summary_type, format, summary_name, null);
        }

        public static GridDataSummaryColumn GetSummaryColumn<U>(Expression<Func<U>> property, SummaryType summary_type, string format, ISummaryAggregate custom_aggregate)
        {
            return GetSummaryColumn<U>(property, summary_type, format, null, custom_aggregate);
        }

        public static GridDataSummaryColumn GetSummaryColumn<U>(Expression<Func<U>> property, SummaryType summary_type, string format, string summary_name, ISummaryAggregate custom_aggregate)
        {
            string property_name = PropertyNames.Get<U>(property);

            if (null == summary_name)
            {
                summary_name = property_name + "_Summary";
            }

            var summary_column = new GridDataSummaryColumn();
            summary_column.Name = summary_name;
            summary_column.MappingName = property_name;
            summary_column.SummaryType = summary_type;
            summary_column.CustomAggregate = custom_aggregate;
            summary_column.Format = format;
            return summary_column;
        }
    }
}

#endif
