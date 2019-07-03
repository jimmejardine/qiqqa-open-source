using System;
using System.Globalization;

namespace Utilities.GUI
{
    public class TrialPeriod
    {
        string product_name;
        DateTime expiry_date;

        public TrialPeriod(string product_name, string expiry_date)
        {
            this.product_name = product_name;
            
            IFormatProvider culture = new CultureInfo("en-US", false);
            this.expiry_date = DateTime.Parse(expiry_date, culture);
        }

        public bool HasExpired()
        {
            TimeSpan time_span = expiry_date.Subtract(DateTime.UtcNow);

            if (time_span.TotalDays < 0)
            {
                MessageBoxes.Error("This version of {0} expired {1} day(s) ago on {2}", product_name, -(int)time_span.TotalDays, expiry_date.ToLongDateString());
                return true;
            }

            if (time_span.TotalDays < 28)
            {
                MessageBoxes.Warn("This version of {0} expires in {1} day(s) on {2}", product_name, (int)time_span.TotalDays, expiry_date.ToLongDateString());
                return false;
            }

            {
                //MessageBoxes.Info("This version of {0} expires in {1} day(s) on {2}", product_name, time_span.TotalDays, expiry_date.ToLongDateString());
                return false;
            }
        }
    }
}
