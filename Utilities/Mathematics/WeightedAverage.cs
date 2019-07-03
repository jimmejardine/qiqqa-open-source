using System;

namespace Utilities.Mathematics
{
    public class WeightedAverage
    {
        double numerator = 0;
        double denominator = 0;

        public WeightedAverage()
        {
            Reset();
        }

        public void Add(double n, double weight)
        {
            numerator += n * weight;
            denominator += weight;
        }

        public double Current
        {
            get
            {
                return numerator / denominator;
            }
        }

        public void Reset()
        {
            numerator = 0;
            denominator = 0;
        }

        public override string ToString()
        {
            return Convert.ToString(Current);
        }

        public void TallyWith(WeightedAverage yield)
        {
            numerator += yield.numerator;
            denominator += yield.denominator;
        }
    }
}
