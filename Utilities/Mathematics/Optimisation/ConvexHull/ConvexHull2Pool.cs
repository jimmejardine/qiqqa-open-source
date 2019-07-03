using System.Collections;
using Utilities.GUI.Charting;
using Utilities.Random;

namespace Utilities.Mathematics.Optimisation.ConvexHull
{
	public class ConvexHull2Pool
	{
		public static void findHull(ArrayList pool1, ArrayList pool2, out double output_m, out double output_c)
		{
			double HULL_TOLERANCE = 0.00001;

			if (0 == pool1.Count || 0 == pool2.Count)
			{
				throw new GenericException("Must have at least two points to generate the hull");
			}

			// Now look for the best pair of points that define the hull
			
			for (int i = 0; i < pool1.Count; ++i)
			{
				Point2D p1 = (Point2D) pool1[i];
				for (int j = 0; j < pool2.Count; ++j)
				{
					Point2D p2 = (Point2D) pool2[j];

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
					for (int k1 = 0; k1 < pool1.Count; ++k1)
					{
						Point2D p3 = (Point2D) pool1[k1];
						double y_hull = c + m * p3.x;
						if (p3.y > y_hull+HULL_TOLERANCE)
						{
							is_hull = false;
							break;
						}
					}
					for (int k2 = 0; k2 < pool2.Count; ++k2)
					{
						Point2D p3 = (Point2D) pool2[k2];
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
			// So lets resort to our single pool version
			ArrayList combined_points = new ArrayList();
			combined_points.AddRange(pool1);
			combined_points.AddRange(pool2);
			ConvexHull.findHull(combined_points, out output_m, out output_c);
		}

		public static void TestHarness()
		{
			for (int i = 0; i < 5; ++i)
			{
				TestHarness_LOOP();
			}
		}

		public static void TestHarness_LOOP()
		{
			RandomAugmented random = RandomAugmented.getSeededRandomAugmented();

			ArrayList pool1 = new ArrayList();
			ArrayList pool2 = new ArrayList();
			for (int i = 0; i < 10; ++i)
			{
				pool1.Add(new Point2D(random.NextDouble(), random.NextDouble()));
				pool2.Add(new Point2D(random.NextDouble(), random.NextDouble()));
			}
			double m, c;
			findHull(pool1, pool2, out m, out c);
			ArrayList hull = new ArrayList();
			hull.Add(new Point2D(0,c)); hull.Add(new Point2D(1,m+c));

			GenericChartForm charts = new GenericChartForm();
			charts.setChartCounts(1,1);
			charts[0].addSeries(new Series("Pool1", ChartType.Point, pool1));
			charts[0].addSeries(new Series("Pool2", ChartType.Point, pool2));
			charts[0].addSeries(new Series("Hull", ChartType.Line, hull));
			charts[0].Refresh();
			charts.ShowDialog();
		}
	}
}
