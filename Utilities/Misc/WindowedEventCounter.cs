using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Misc
{
    public class WindowedEventCounter
    {
        long window_ms;
        
        List<long> memory = new List<long>();
        int total_count = 0;

        public WindowedEventCounter(long window_ms)
        {
            this.window_ms = window_ms;
        }

        public void Tally(int delta)
        {
            Tally(DateTime.UtcNow, delta);
        }

        public void Tally(DateTime time, int delta)
        {
            Purge(time);

            long millis = time.Ticks / TimeSpan.TicksPerMillisecond;
            for (int i = 0; i < delta; ++i)
            {
                memory.Add(millis);
            }

            total_count += delta;
        }

        public int TotalCount()
        {
            return total_count;
        }

        public int Count()
        {
            return Count(DateTime.UtcNow);
        }
        
        public int Count(DateTime time)
        {
            Purge(time);

            return memory.Count;
        }

        private void Purge(DateTime time)
        {
            long millis = time.Ticks / TimeSpan.TicksPerMillisecond;
            while (0 < memory.Count && window_ms < (millis - memory.First()))
            {
                memory.RemoveAt(0);
            }
        }
    }
}
