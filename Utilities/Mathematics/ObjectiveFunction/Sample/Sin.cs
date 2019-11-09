#if false

using System;

namespace Utilities.Mathematics.ObjectiveFunction.Sample
{
    public class Sin : ObjectiveFunction
    {
        public Sin()
        {
        }

        public double evaluate(double x)
        {
            return Math.Sin(x);
        }
    }
}

#endif
