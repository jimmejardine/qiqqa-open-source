using System;

namespace Utilities.DateTimeTools
{
    public class UnixTimestamps
    {

        private static DateTime epoch_base_time = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long MillisSinceEpoch(DateTime time)
        {
            return (time - epoch_base_time).Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static DateTime DateTimeFromMillisSinceEpoch(long millis_since_epoch)
        {            
            return epoch_base_time.AddSeconds(millis_since_epoch);
        }

    }
}
