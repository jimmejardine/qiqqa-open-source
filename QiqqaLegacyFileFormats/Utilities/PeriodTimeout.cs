using System;

namespace QiqqaLegacyFileFormats          // namespace Utilities
{
    [Serializable]
    public class PeriodTimeout
    {
        private int seconds;
        private DateTime lastChecked = DateTime.MinValue;

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
