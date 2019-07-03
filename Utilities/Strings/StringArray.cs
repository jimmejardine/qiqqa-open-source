using System;
using System.Collections;

namespace Utilities.Strings
{
	public class StringArray
	{
		ArrayList _array;

		public StringArray()
		{
			_array = new ArrayList();
		}

		public void Add(string item)
		{
			_array.Add(item);
		}

		public int Count
		{
			get
			{
				return _array.Count;
			}
		}

		public String this[int index]
		{
			get
			{
				return (String) _array[index];
			}

			set
			{
				_array[index] = value;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _array.GetEnumerator();
		}
	}
}
