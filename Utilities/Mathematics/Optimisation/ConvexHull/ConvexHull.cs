using System.Collections;
using Utilities.GUI.Charting;
using Utilities.Random;

namespace Utilities.Mathematics.Optimisation.ConvexHull
{
	public class ConvexHull
	{
		public static void findHull(ArrayList points, out double output_m, out double output_c)
		{
			double HULL_TOLERANCE = 0.00001;

			if (2 > points.Count)
			{
				throw new GenericException("Must have at least two points to generate the hull");
			}

			// Now look for the best paair of points that define the hull
			int num_points = points.Count;
			for (int i = 0; i < num_points; ++i)
			{
				Point2D p1 = (Point2D) points[i];
				for (int j = i+1; j < num_points; ++j)
				{
					Point2D p2 = (Point2D) points[j];

					// Ignore this pair if we can't calculate the gradient
					if (p1.x == p2.x)
					{
						continue;
					}

					// Calculate the gradient
					double m = (p1.y - p2.y) / (p1.x - p2.x);
					double c = p1.y - m * p1.x;

					// Check that every other point is below the line
					bool is_hull = true;
					for (int k = 0; k < num_points; ++k)
					{
						Point2D p3 = (Point2D) points[k];
						double y_hull = c + m * p3.x;
						if (p3.y > y_hull+HULL_TOLERANCE)
						{
							is_hull = false;
							break;
						}
					}

					// If we have found a hull, return it
					if (is_hull)
					{
						output_m = m;
						output_c = c;
						return;
					}
				}
			}

			// If we get this far there is something dodgy doing on (all co-linear, vertical, etc)
			output_m = 0.0;
			output_c = ((Point2D) points[0]).y;
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			RandomAugmented random = RandomAugmented.getSeededRandomAugmented();

			ArrayList points = new ArrayList();
			for (int i = 0; i < 10; ++i)
			{
				points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
			}
			double m, c;
			findHull(points, out m, out c);
			ArrayList hull = new ArrayList();
			hull.Add(new Point2D(0,c)); hull.Add(new Point2D(1,m+c));

			GenericChartForm charts = new GenericChartForm();
			charts.setChartCounts(1,1);
			charts[0].addSeries(new Series("Points", ChartType.Point, points));
			charts[0].addSeries(new Series("Hull", ChartType.Line, hull));
			charts[0].Refresh();
			charts.ShowDialog();
		}
#endif

        #endregion
    }
}
