using System;

namespace Utilities.Mathematics
{
    public class Average
    {
        private double numerator = 0;
        private double denominator = 0;

        public Average()
        {
            Reset();
        }

        public void Add(double n)
        {
            numerator += n;
            denominator += 1;
        }

        public double Current => numerator / denominator;

        public double Count => denominator;

        public void Reset()
        {
            numerator = 0;
            denominator = 0;
        }

        public override string ToString()
        {
            return Convert.ToString(Current);
        }
    }
}
