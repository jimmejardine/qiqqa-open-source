using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Utilities.Collections
{
    [Serializable]
    public class SummingDictionary<KEY> : Dictionary<KEY, double>
    {
        public SummingDictionary()
        {
        }

        protected SummingDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { 
        }

        public double GetSum(KEY key)
        {
            double count;
            if (!TryGetValue(key, out count))
            {
                count = 0;
            }

            return count;
        }

        public double TotalSum()
        {
            double sum = 0;
            foreach (var val in this.Values) sum += val;
            return sum;
        }

        public void AddOne(KEY key)
        {
            Add(key, 1);
        }

        public new void Add(KEY key, double delta)
        {
            this[key] = GetSum(key) + delta;
        }

        public List<KEY> OrderedKeys()
        {
            List<KEY> l = new List<KEY>(this.Keys);
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
            List<KEY> l = new List<KEY>(this.Keys);
            l.Sort(delegate(KEY a, KEY b) { return -Sorting.Compare(this[a], this[b]); });
            return l;
        }

        public override string ToString()
        {
            return ToString_OrderedValue(0);
        }

        public string ToString_OrderedXXX(List<KEY> keys, double threshold, string TAB = "\t", string NEWLINE = "\n", bool include_total = false)
        {
            double total = 0;

            StringBuilder sb = new StringBuilder();

            foreach (KEY key in keys)
            {
                double value = this[key];
                if (threshold < value)
                {
                    total += value;
                    sb.AppendFormat("{0}{1}{2:n}{3}", key, TAB, value, NEWLINE);
                }
            }

            if (include_total)
            {
                sb.AppendFormat("{0}{1}{2:n}{3}", "TOTAL", TAB, total, NEWLINE);
            }

            return sb.ToString();
        }

        public string ToString_OrderedKey(double threshold, string TAB = "\t", string NEWLINE = "\n", bool include_total = false)
        {
            List<KEY> keys = OrderedKeys();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE, include_total);
        }


        public string ToString_OrderedValue(double threshold, string TAB = "\t", string NEWLINE = "\n", bool include_total = false)
        {
            List<KEY> keys = OrderedValues();
            return ToString_OrderedXXX(keys, threshold, TAB, NEWLINE, include_total);
        }

    }
}
