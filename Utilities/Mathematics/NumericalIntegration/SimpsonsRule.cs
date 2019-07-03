using System;
using Utilities.Mathematics.ObjectiveFunction.Sample;

namespace Utilities.Mathematics.NumericalIntegration
{
	public class SimpsonsRule
	{
		public static double integrate(double from, double to, ObjectiveFunction.ObjectiveFunction f, int NUM_STEPS)
		{
			double total = 0.0;
			
			total += 0.5 * f.evaluate(from);
			for (int i = 1; i < NUM_STEPS; ++i)
			{
				total += f.evaluate(i * (to - from) / NUM_STEPS);
			}
			total += 0.5 * f.evaluate(from);

			total *= (to - from);
			total /= NUM_STEPS;

			return total;
		}

		public static void TestHarness()
		{
			ObjectiveFunction.ObjectiveFunction c = new Constant();
			ObjectiveFunction.ObjectiveFunction sin = new Sin();
			Console.WriteLine("Constant is {0}", integrate(0, 1, c, 100));
			Console.WriteLine("Sin is {0}", integrate(0, 1, sin, 100));
		}
	}
}
