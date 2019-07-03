using System;

namespace Utilities.Mathematics.Optimisation.BisectionSearch
{
	public class BisectionSearch
	{
		public static double locateMonotonic(ObjectiveFunction.ObjectiveFunction objectivefunction, double min, double max, double value_target, double tolerance, int max_iterations)
		{
			double value_min = objectivefunction.evaluate(min);
			double value_max = objectivefunction.evaluate(max);

			for (int i = 0; i < max_iterations; ++i)
			{
				double mid = 0.5 * (min + max);
				double value_mid = objectivefunction.evaluate(mid);

				if ((value_min <= value_target && value_mid >= value_target) || (value_min >= value_target && value_mid <= value_target))
				{
					max = mid;
					value_max = value_mid;
				}
				else
				{
					min = mid;
					value_min = value_mid;
				}

				if (Math.Abs(max - min) < tolerance)
				{
					return mid;
				}
			}

			throw new GenericException("Reached the maximum number of iterations before converging.");
		}
	}
}
