using System;

namespace Utilities
{
    public class MaxMin
    {
        public static DateTime Max(DateTime a, DateTime b)
        {
            return (a > b) ? a : b;
        }

        public static DateTime Min(DateTime a, DateTime b)
        {
            return (a < b) ? a : b;
        }
    }
}
