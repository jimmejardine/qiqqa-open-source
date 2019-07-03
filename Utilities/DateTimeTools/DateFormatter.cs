using System;
using System.Globalization;

namespace Utilities.DateTimeTools
{
	public class DateFormatter
	{
		public static string asMMYY(DateTime date)
		{
			string year_string = date.Year.ToString();
			return date.Month + "/" + year_string.Substring(2);
		}

        public static string asDDMMMYYYY(DateTime date)
        {
            return date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
        }

        public static string asYYYYMMDDHHMMSS(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public static DateTime FromYYYYMMDDHHMMSS(string date_string)
        {
            return DateTime.ParseExact(date_string, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public static string ToYYYYMMDDHHMMSSMMM(DateTime? date)
        {
            if (null == date || !date.HasValue)
            {
                return null;
            }
            else
            {
                return ToYYYYMMDDHHMMSSMMM(date.Value);
            }
        }

        public static string ToYYYYMMDDHHMMSSMMM(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }

        public static DateTime? FromYYYYMMDDHHMMSSMMM(string date_string)
        {
            if (null == date_string)
            {
                return new DateTime?();
            }
            else
            {
                return DateTime.ParseExact(date_string, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
            }
        }
    }
}
