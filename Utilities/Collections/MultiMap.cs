using System;
using System.Collections.Generic;

namespace Utilities.Collections
{
    /// <summary>
    /// Provides a multiple map, where a given key points to several items.  Note that the items will not be unique if specified at the constructor.
    /// </summary>
    /// <typeparam name="KEY"></typeparam>
    /// <typeparam name="VALUE"></typeparam>
    [Serializable]
    public class MultiMap<KEY, VALUE>
    {
        private Dictionary<KEY, List<VALUE>> data = new Dictionary<KEY, List<VALUE>>();
        private bool unique_values;

        public MultiMap(bool unique_values)
        {
            this.unique_values = unique_values;
        }

        public void Add(KEY key, VALUE value)
        {
            List<VALUE> list;
            if (!data.TryGetValue(key, out list))
            {
                list = new List<VALUE>();
                data[key] = list;
            }

            // Add it if we are allowed to and it doesnt exist
            if (!unique_values || !list.Contains(value))
            {
                list.Add(value);
            }
        }

        public List<VALUE> this[KEY key] => data[key];

        public Dictionary<KEY, List<VALUE>>.Enumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public List<VALUE> Get(KEY key)
        {
            List<VALUE> list;
            data.TryGetValue(key, out list);
            return list;
        }

        public Dictionary<KEY, List<VALUE>>.KeyCollection Keys => data.Keys;

        public bool TryGetValue(KEY key, out List<VALUE> values)
        {
            return data.TryGetValue(key, out values);
        }

        public bool Remove(KEY key)
        {
            return data.Remove(key);
        }

        public void Clear()
        {
            data.Clear();
        }

        public MultiMap<KEY, VALUE> Clone()
        {
            MultiMap<KEY, VALUE> result = new MultiMap<KEY, VALUE>(unique_values);

            foreach (var pair in data)
            {
                result.data[pair.Key] = new List<VALUE>(pair.Value);
            }

            return result;
        }
    }
}
