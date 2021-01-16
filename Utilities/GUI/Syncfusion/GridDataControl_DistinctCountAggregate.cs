#if SYNCFUSION_ANTIQUE

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.Windows.Data;
using Utilities.Collections;

namespace Utilities.GUI.Syncfusion
{
    public class GridDataControl_DistinctCountAggregate : ISummaryAggregate
    {
        public static readonly string PROPERTY_NAME = "{Sum}";

        public GridDataControl_DistinctCountAggregate() { }

        public string Sum { get; set; }

        private CountingDictionary<string> counts = new CountingDictionary<string>();

        public Action<IEnumerable, string, PropertyDescriptor> CalculateAggregateFunc()
        {
            return DoSum;
        }

        private void DoSum(IEnumerable items, string property, PropertyDescriptor pd)
        {
            counts.Clear();

            PropertyInfo property_info = null;
            foreach (var item in items)
            {
                if (null == property_info)
                {
                    Type type = item.GetType();
                    property_info = type.GetProperty(property);


                }

                object key = property_info.GetValue(item, null);
                if (null == key)
                {
                    key = "''";
                }
                counts.TallyOne(key.ToString());
            }

            Sum = counts.ToString_OrderedValue(0, ":", " ");
        }
    }
}

#endif
