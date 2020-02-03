#if false

using System;

namespace Utilities.Maintainable
{
    public class PeriodTimer
    {
        private TimeSpan period;
        private DateTime last_signalled;

        public PeriodTimer(TimeSpan period)
        {
            this.period = period;
        }

        public bool Expired => (DateTime.UtcNow.Subtract(last_signalled).CompareTo(period) > 0);

        public void Signal()
        {
            last_signalled = DateTime.UtcNow;
        }
    }
}

#endif
