using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Mathematics
{
    public static class Perunage
    {
        // Produce a decent Perunage value, even when `total_count` is ZERO.
        //
        // Note: a special case is where the `current_count` is larger than `total_count` when `total_count` is ZERO:
        // this is assumed to represent 100%, which is consistent with the result when `total_count` is *not* ZERO.
        //
        // Output perunage is clipped to the range [0, 1], i.e. the inclusive range: 0% to 100%.
        public static double Calc(long current_count, long total_count)
        {
            if (total_count == 0)
            {
                return current_count > 0 ? 1.0 /* 100% */ : 0.0 /* 0% */;
            }
            return Math.Min(1.0, Math.Max(0.0, current_count * 1.0 / total_count));
        }
    }
}
