using System;
using System.Collections;

namespace Utilities.Mathematics
{
	public class General
	{
        public static bool HasPercentageJustTicked(double numerator, double denominator)
        {
            if (0 == numerator) return true;

            double A = Math.Floor(100 * (numerator - 1) / denominator);
            double B = Math.Floor(100 * (numerator) / denominator);
            return (B - A > 0);
        }


        public static bool IsPowerOfTwo(double source)
        {
            double log_double = Math.Log(source, 2);
            int log_int = (int)log_double;
            return (log_double == log_int);
        }

		public static double parsePercentage(string source)
		{
			if (source.Length == 0)
			{
				return 0.0;
			}

			if (source.EndsWith("%"))
			{
				return Double.Parse(source.TrimEnd('%')) / 100.0;
			}
			else
			{
				return Double.Parse(source);
			}
		}

		public static double cubeRoot(double source)
		{
			if (source >= 0)
			{
				return Math.Pow(source, 1.0/3.0);
			}
			else
			{
				return -Math.Pow(-source, 1.0/3.0);
			}
		}

		public static ArrayList factorize(int source)
		{
			ArrayList result = new ArrayList();

			int limit = source / 2;
			for (int i = 1; i <= limit; ++i)
			{
				if (0 == source % i)
				{
					result.Add(i);
				}
			}

			result.Add(source);

			return result;
		}

		public static double nearestGreaterMultiple(double source, double factor)
		{
			source /= factor;
			source = Math.Ceiling(source);
			source *= factor;
			return source;
		}

		public static double nearestSmallerMultiple(double source, double factor)
		{
			source /= factor;
			source = Math.Floor(source);
			source *= factor;
			return source;
		}

		public static double interpolate_linear(double x, double x1, double x2, double y1, double y2)
		{
			// If we do not have an interpolation interval then average the y values (they should be identical!)
			if (x1 == x2)
			{
				return (y1 + y2) / 2.0;
			}

			// Otherwise return the linear interpolation of the two points
			return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
		}
	}
}
