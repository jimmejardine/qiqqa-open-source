using System;

namespace Utilities
{
    [Serializable]
    public class PeriodTimeout
    {
        int seconds;
        DateTime lastChecked = DateTime.MinValue;

        public PeriodTimeout(int seconds)
        {
            this.seconds = seconds;
        }

        public bool hasElapsed()
        {
            return seconds < DateTime.Now.Subtract(lastChecked).TotalSeconds;
        }

        public void reset()
        {
            lastChecked = DateTime.Now;
        }
    }
}
