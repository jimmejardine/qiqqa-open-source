using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utilities.Collections
{
    [Serializable]
    public class CountingDictionary<KEY> : Dictionary<KEY, int>
    {
        public CountingDictionary()
        {
        }

        protected CountingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public int GetCount(KEY key)
        {
            int count;
            if (!TryGetValue(key, out count))
            {
                count = 0;
            }

            return count;
        }

        public void TallyOne(KEY key)
        {
            TallyN(key, 1);
        }

        public void TallyN(KEY key, int n)
        {
            this[key] = GetCount(key) + n;
        }

        public void ResetTally(KEY key)
        {
            this[key] = 0;
        }

        public List<KEY> OrderedKeys()
        {
            List<KEY> l = new List<KEY>(Keys);
            l.Sort();
            return l;
        }

        public List<KEY> OrderedKeysWithThreshold(int count_threshold)
        {
            List<KEY> l = new List<KEY>();
            foreach (var pair in this)
            {
                if (pair.Value >= count_threshold)
                {
                    l.Add(pair.Key);
                }
            }

            l.Sort();
            return l;
        }

        /// <summary>
        /// Returns the KEYS ordered by most frequent first
        /// </summary>
        /// <returns></returns>
        public List<KEY> OrderedValues()
        {
            List<KEY> l = new List<KEY>(Keys);
            l.Sort(delegate (KEY a, KEY b) { return -Sorting.Compare(this[a], this[b]); });
            return l;
        }


        public void AddRange(CountingDictionary<KEY> other)
        {
            foreach (var pair in other)
            {
                TallyN(pair.Key, pair.Value);
            }
        }

        public override string ToString()
        {
            return ToString_OrderedValue(0);
        }

        public string ToString_OrderedXXX(List<KEY> keys, int threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            StringBuilder sb = new StringBuilder();

            foreach (KEY key in keys)
            {
                int value = this[key];
                if (threshold < value)
                {
                    sb.AppendFormat("{0}{2}{1}{3}", key, value, TAB, NEWLINE);
                }
            }

            return sb.ToString();
        }

        public string ToString_OrderedKey(int threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            List<KEY> keys = OrderedKeys();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE);
        }


        public string ToString_OrderedValue(int threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            List<KEY> keys = OrderedValues();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE);
        }
    }
}
