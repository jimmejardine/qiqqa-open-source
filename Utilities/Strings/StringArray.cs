using System;
using System.Collections;

namespace Utilities.Strings
{
    public class StringArray
    {
        private ArrayList _array;

        public StringArray()
        {
            _array = new ArrayList();
        }

        public void Add(string item)
        {
            _array.Add(item);
        }

        public int Count => _array.Count;

        public String this[int index]
        {
            get => (String)_array[index];

            set => _array[index] = value;
        }

        public IEnumerator GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}
