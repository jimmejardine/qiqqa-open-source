using System;

namespace Utilities.Mathematics.Optimisation.NelderMead.TestHarness
{
    public class QuadraticSearchFunction : ObjectiveFunction
    {
        public double evaluate(double[] p)
        {
            return Math.Pow(p[0] - 3.0, 2) + Math.Pow(p[1] - 4.0, 2);
        }

        public void constrainSearch(ref double[] p)
        {
            if (p[0] > 2.0)
            {
                p[0] = 2.0;
            }
        }
    }
}
