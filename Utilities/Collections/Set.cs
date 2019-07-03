using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Collections
{
    [Obsolete("Use HashSet instead.")]
    public class Set<T> : IEnumerable, IEnumerable<T>
    {
        static object DUMMY = new object();
        Dictionary<T, object> set = new Dictionary<T, object>();

        public void Add(T t)
        {
            set[t] = DUMMY;
        }

        public bool Remove(T t)
        {
            return set.Remove(t);
        }

        public int Count
        {
            get { return set.Count; }
        }

        public bool Contains(T t)
        {
            return set.ContainsKey(t);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return set.Keys.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return set.Keys.GetEnumerator();
        }
    }

    [Obsolete("Use HashSet instead.")]
    public class Set : IEnumerable
	{
        Object _dummy = new Object();

		Hashtable _set;
		
		public Set()
		{
			_set = new Hashtable();
		}

        public void Add(object item)
		{
            if (!_set.Contains(item))
            {
                _set.Add(item, _dummy);
            }
        }

		public void Remove(object item)
		{
			_set.Remove(item);
		}

		public int Count
		{
			get { return _set.Count; }
		}

        public object Pop()
        {
            IEnumerator i = _set.Keys.GetEnumerator();
            if (i.MoveNext())
            {
                object obj = i.Current;
                _set.Remove(obj);
                return obj;
            }
            else
            {
                return null;
            }
        }

        public bool Contains(object item)
        {
            return _set.ContainsKey(item);
        }

		public IEnumerator GetEnumerator()
		{
			return _set.Keys.GetEnumerator();
		}
	}
}
