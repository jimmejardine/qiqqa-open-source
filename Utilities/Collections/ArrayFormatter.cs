using System;
using System.Collections;
using System.Text;

namespace Utilities.Collections
{
	public class ArrayFormatter
	{
		public static string listElements(Array array)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Object i in array)
			{
				sb.Append(i);
				sb.Append(", ");
			}
			return sb.ToString();
		}

        public static string listElements(ICollection array)
        {
            return ListElements(array, ", ");
        }

        public static string ListElements(ICollection array, string separator, string opener = null, string closer = null)
        {
            bool is_first = true;

            StringBuilder sb = new StringBuilder();
            foreach (Object i in array)
            {
                if (is_first)
                {
                    is_first = false;
                }
                else
                {
                    sb.Append(separator);
                }

                if (null != opener) sb.Append(opener);
                sb.Append(i);
                if (null != closer) sb.Append(closer);
            }
            return sb.ToString();
        }

        public static string listElements(IEnumerable array)
        {
            return listElements(array, ", ");
        }

        public static string listElements(IEnumerable array, string separator)
        {
            StringBuilder sb = new StringBuilder();            
            foreach (Object i in array)
            {
                sb.Append(i);
                sb.Append(separator);
            }
            return sb.ToString();
        }


		public static string listHashtableElements(Hashtable counters)
		{
			StringBuilder sb = new StringBuilder();
			foreach (object key in counters.Keys)
			{
				object counter = counters[key];

				sb.AppendFormat("{0}\t{1}\r\n", key, counter);
			}

			return sb.ToString();
		}
	}
}
