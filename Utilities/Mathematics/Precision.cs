using System;
using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Mathematics
{
    public class Precision
    {
        private const double CLOSE_TO_TOLERANCE = 1E-8;

        public static void massageIntoCorrelation(Matrix source)
        {
            // Check its square
            if (source.rows != source.cols)
            {
                throw new GenericException("Cannot massage a non-square matrix");
            }

            int N = source.rows;

            for (int i = 0; i < N; ++i)
            {
                for (int j = i + 1; j < N; ++j)
                {
                    source[i, j] = 0.5 * (source[i, j] + source[j, i]);
                    source[j, i] = source[i, j];
                }
            }
        }

        public static bool closeToZero(double a)
        {
            return closeTo(a, 0.0);
        }

        public static bool closeTo(double a, double b)
        {
            return (Math.Abs(a - b) < CLOSE_TO_TOLERANCE);
        }

        public static int estimateMeaningfulChartRoundingPrecision(double range)
        {
            int digits = (int)(-Math.Floor(Math.Log10(range / 100.0)));
            if (digits < 0) digits = 0;
            if (digits > 10) digits = 0;
            return digits;
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Console.WriteLine("Rounding precision of 1000 is {0}", estimateMeaningfulChartRoundingPrecision(1000));
			Console.WriteLine("Rounding precision of 100 is {0}", estimateMeaningfulChartRoundingPrecision(100));
			Console.WriteLine("Rounding precision of 10 is {0}", estimateMeaningfulChartRoundingPrecision(10));
			Console.WriteLine("Rounding precision of 1 is {0}", estimateMeaningfulChartRoundingPrecision(1));
			Console.WriteLine("Rounding precision of 0.1 is {0}", estimateMeaningfulChartRoundingPrecision(0.1));
			Console.WriteLine("Rounding precision of 0.01 is {0}", estimateMeaningfulChartRoundingPrecision(0.01));
			Console.WriteLine("Rounding precision of 0.02 is {0}", estimateMeaningfulChartRoundingPrecision(0.02));
			Console.WriteLine("Rounding precision of 0.06 is {0}", estimateMeaningfulChartRoundingPrecision(0.06));
			Console.WriteLine("Rounding precision of 0.09 is {0}", estimateMeaningfulChartRoundingPrecision(0.09));
			Console.WriteLine("Rounding precision of 0 is {0}", estimateMeaningfulChartRoundingPrecision(0));
		}
#endif

        #endregion
    }
}
