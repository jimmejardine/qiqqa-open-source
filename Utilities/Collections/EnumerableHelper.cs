using System;
using System.Collections.Generic;

namespace Utilities.Collections
{
    public static class EnumerableHelper
    {
        public static void ForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action", "Parameter cannot be null.");
            }

            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
