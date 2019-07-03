using System;

namespace Utilities.Mathematics.Optimisation.NelderMead.TestHarness
{
	public class QuadraticSearchFunction2D : ObjectiveFunction2D
	{
		public double evaluate(Point2D p)
		{
			return Math.Pow(p.x - 3.0, 2) + Math.Pow(p.y - 4.0, 2);
		}

		public void constrainSearch(ref Point2D p)
		{
			if (p.x > 2.0)
			{
				p.x = 2.0;
			}
		}
	}
}
