using System.Collections;
using System.Collections.Generic;

namespace Utilities.Collections
{
	public class Tools
	{
        public static IEnumerable<T> CopySorted<T>(IEnumerable<T> keys)
        {
            List<T> points_keys = new List<T>(keys);
            points_keys.Sort();
            return points_keys;
        }

        public static ArrayList removeDuplicates(ArrayList input)
		{
			input.Sort();
			ArrayList result = new ArrayList();
			object last_item = null;
			foreach (object current in input)
			{
				if (!current.Equals(last_item))
				{
					result.Add(current);
					last_item = current;
				}
			}

			return result;
		}
		
		public static object[] enumeratorToObjectArray(IEnumerator i)
		{
			ArrayList list = new ArrayList();
			while (i.MoveNext())
			{
				list.Add(i.Current);
			}

			return list.ToArray();			
		}

        public static HashSet<T> Intersect<T>(HashSet<T> a, HashSet<T> b)
        {
            HashSet<T> result = new HashSet<T>(a);
            result.IntersectWith(b);
            return result;
        }
	}
}
