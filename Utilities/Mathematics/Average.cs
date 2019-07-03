using System;

namespace Utilities.Mathematics
{
    public class Average
    {
        double numerator = 0;
        double denominator = 0;

        public Average()
        {
            Reset();
        }

        public void Add(double n)
        {
            numerator += n;
            denominator += 1;
        }

        public double Current
        {
            get
            {
                return numerator / denominator;
            }
        }

        public double Count
        {
            get
            {
                return denominator;
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
    }
}
