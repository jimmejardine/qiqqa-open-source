using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
