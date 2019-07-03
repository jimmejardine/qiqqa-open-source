using System;
using System.Collections.Generic;

namespace Utilities.Mathematics
{
    public class RunningAverage
    {
        private int WINDOW_SIZE;
        private List<double> vals = new List<double>();

        public RunningAverage(int WINDOW_SIZE)
        {
            this.WINDOW_SIZE = WINDOW_SIZE;
            Reset();
        }

        public void Add(double val)
        {
            vals.Add(val);
            if (vals.Count > WINDOW_SIZE) vals.RemoveAt(0);
        }

        public double Current
        {
            get
            {
                double total = 0;
                foreach (var val in vals) total += val;
                return total / vals.Count;
            }
        }

        public void Reset()
        {
            vals.Clear();
        }

        public override string ToString()
        {
            return Convert.ToString(Current);
        }
    }
}
