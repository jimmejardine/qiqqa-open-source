using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Collections
{
    [Obsolete("Use HashSet instead.", true)]
    public class Set<T> : IEnumerable, IEnumerable<T>
    {
        private static object DUMMY = new object();
        private Dictionary<T, object> set = new Dictionary<T, object>();

        public void Add(T t)
        {
            set[t] = DUMMY;
        }

        public bool Remove(T t)
        {
            return set.Remove(t);
        }

        public int Count => set.Count;

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

    [Obsolete("Use HashSet instead.", true)]
    public class Set : IEnumerable
    {
        private Object _dummy = new Object();
        private Hashtable _set;

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

        public int Count => _set.Count;

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
