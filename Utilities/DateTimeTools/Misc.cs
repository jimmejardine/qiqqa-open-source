using System;

namespace Utilities.DateTimeTools
{
    public class Misc
    {
        public static DateTime Max(DateTime a, DateTime b)
        {
            return (a < b) ? b : a;
        }
    }
}
