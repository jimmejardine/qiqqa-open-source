using System.Collections.Generic;

namespace Utilities.Collections
{
    public class Batching
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> src, int size)
        {
            List<T> buffer = new List<T>();
            foreach (var t in src)
            {
                buffer.Add(t);
                if (size <= buffer.Count)
                {
                    yield return buffer;
                    buffer = new List<T>();
                }
            }

            if (0 < buffer.Count)
            {
                yield return buffer;
            }
        }
    }
}
