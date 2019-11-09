#if !HAS_NO_PROTOBUF

using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Utilities.Collections
{
    /// <summary>
    /// Provides a multiple map, where a given key points to several items.  Note that the items will be unique.
    /// </summary>
    /// <typeparam name="KEY"></typeparam>
    /// <typeparam name="VALUE"></typeparam>
    [Serializable]
    [ProtoContract]
    public class MultiMapSet<KEY, VALUE>
    {
        private static HashSet<VALUE> EMPTY_VALUE_SET = new HashSet<VALUE>();

        [ProtoMember(1)]
        private Dictionary<KEY, HashSet<VALUE>> data = new Dictionary<KEY, HashSet<VALUE>>();

        public MultiMapSet()
        {
        }

        public void Add(KEY key, VALUE value)
        {
            HashSet<VALUE> list;
            if (!data.TryGetValue(key, out list))
            {
                list = new HashSet<VALUE>();
                data[key] = list;
            }

            // Add it if we are allowed to and it doesnt exist
            list.Add(value);
        }

        public HashSet<VALUE> this[KEY key] => data[key];

        public Dictionary<KEY, HashSet<VALUE>>.Enumerator GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public HashSet<VALUE> Get(KEY key)
        {
            HashSet<VALUE> list;
            if (data.TryGetValue(key, out list))
            {
                return list;
            }
            else
            {
                return EMPTY_VALUE_SET;
            }
        }

        public Dictionary<KEY, HashSet<VALUE>>.KeyCollection Keys => data.Keys;

        public bool TryGetValue(KEY key, out HashSet<VALUE> values)
        {
            return data.TryGetValue(key, out values);
        }

        public bool Remove(KEY key)
        {
            return data.Remove(key);
        }

        public MultiMapSet<KEY, VALUE> Clone()
        {
            MultiMapSet<KEY, VALUE> result = new MultiMapSet<KEY, VALUE>();

            foreach (var pair in data)
            {
                result.data[pair.Key] = new HashSet<VALUE>(pair.Value);
            }

            return result;
        }

        public void Clear()
        {
            data.Clear();
        }

        public int Count => data.Count;

        public List<KeyValuePair<KEY, HashSet<VALUE>>> GetTopN(int N)
        {
            // Put the hash values into a list
            List<KeyValuePair<KEY, HashSet<VALUE>>> results_to_sort = new List<KeyValuePair<KEY, HashSet<VALUE>>>();
            foreach (var pair in data)
            {
                results_to_sort.Add(pair);
            }

            // Now sort the list in descending order
            results_to_sort.Sort(
                delegate (KeyValuePair<KEY, HashSet<VALUE>> a, KeyValuePair<KEY, HashSet<VALUE>> b)
                {
                    return -Sorting.Compare(a.Value.Count, b.Value.Count);
                }
            );

            // Read off the top N items
            List<KeyValuePair<KEY, HashSet<VALUE>>> results = new List<KeyValuePair<KEY, HashSet<VALUE>>>();
            for (int i = 0; i < N && i < results_to_sort.Count; ++i)
            {
                results.Add(results_to_sort[i]);
            }

            // Return
            return results;
        }
    }
}

#endif
