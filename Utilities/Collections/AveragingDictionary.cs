using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Utilities.Mathematics;

namespace Utilities.Collections
{
    [Serializable]
    public class AveragingDictionary<KEY> : Dictionary<KEY, Average>
    {
        public AveragingDictionary()
        {
        }

        protected AveragingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Average GetSum(KEY key)
        {
            Average count;
            if (!TryGetValue(key, out count))
            {
                count = new Average();
                this[key] = count;
            }

            return count;
        }

        public void AddOne(KEY key)
        {
            Add(key, 1);
        }

        public void Add(KEY key, double delta)
        {
            GetSum(key).Add(delta);
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
                if (pair.Value.Current >= count_threshold)
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
            l.Sort(delegate (KEY a, KEY b) { return -Sorting.Compare(this[a].Current, this[b].Current); });
            return l;
        }

        public override string ToString()
        {
            return ToString_OrderedValue(0);
        }

        public string ToString_OrderedXXX(List<KEY> keys, double threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            StringBuilder sb = new StringBuilder();

            foreach (KEY key in keys)
            {
                Average value = this[key];
                if (threshold < value.Current)
                {
                    sb.AppendFormat("{0}{1}{2:n}{3}", key, TAB, value, NEWLINE);
                }
            }

            return sb.ToString();
        }

        public string ToString_OrderedKey(double threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            List<KEY> keys = OrderedKeys();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE);
        }


        public string ToString_OrderedValue(double threshold, string TAB = "\t", string NEWLINE = "\n")
        {
            List<KEY> keys = OrderedValues();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE);
        }

    }
}
