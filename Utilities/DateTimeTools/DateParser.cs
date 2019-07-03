using System;

namespace Utilities.DateTimeTools
{
	public class DateParser
	{
        [Obsolete]
		public static DateTime parseYYYYMMDD(string datetime)
		{
			int year = Int32.Parse(datetime.Substring(0, 4));
			int month = Int32.Parse(datetime.Substring(4, 2));
			int day = Int32.Parse(datetime.Substring(6, 2));
			return new DateTime(year, month, day);
		}
	}
}
