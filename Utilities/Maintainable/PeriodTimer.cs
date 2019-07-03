using System;

namespace Utilities.Maintainable
{
    public class PeriodTimer
    {
        TimeSpan period;
        DateTime last_signalled;

        public PeriodTimer(TimeSpan period)
        {
            this.period = period;
        }

        public bool Expired
        {
            get
            {
                return (DateTime.UtcNow.Subtract(last_signalled).CompareTo(period) > 0);
            }
        }

        public void Signal()
        {
            last_signalled = DateTime.UtcNow;
        }
    }
}
