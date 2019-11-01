using System;
using Utilities.Collections;

namespace Utilities.Mathematics.Optimisation.NelderMead.TestHarness
{
    public class TestHarness
    {
        public static void doTestHarness()
        {
            try
            {
                if (1 == 1)
                {
                    QuadraticSearchFunction2D qsf2d = new QuadraticSearchFunction2D();
                    NelderMead2D nm2d = new NelderMead2D();
                    nm2d.MAX_ITERATIONS = 500;
                    nm2d.initialiseSearch(qsf2d, new Point2D(0, 0), new Point2D(100, 100), new Point2D(-100, -50));
                    string error_message;
                    double optimal_score;
                    Point2D result = nm2d.search(out error_message, out optimal_score);
                    Console.WriteLine("Lowest at " + result + " with score " + optimal_score + " with error " + error_message);
                }

                if (1 == 1)
                {
                    QuadraticSearchFunction qsf = new QuadraticSearchFunction();
                    NelderMead nm = new NelderMead(3);
                    nm.MAX_ITERATIONS = 500;
                    double[][] initial_points = new double[4][];
                    for (int i = 0; i < 4; ++i)
                    {
                        initial_points[i] = new double[3];
                    }
                    initial_points[0][0] = 0; initial_points[0][1] = 0;
                    initial_points[1][0] = 100; initial_points[1][1] = 100;
                    initial_points[2][0] = -100; initial_points[2][1] = -50;
                    initial_points[3][0] = -200; initial_points[3][1] = -150;
                    nm.initialiseSearch(qsf, initial_points);
                    string error_message;
                    double optimal_score;
                    double[] result = nm.search(out error_message, out optimal_score);
                    Console.WriteLine("Lowest at " + ArrayFormatter.listElements(result) + " with score " + optimal_score + " with error " + error_message);
                }
            }

            catch (GenericException e)
            {
                Console.WriteLine("There was an error: {0}", e.Message);
            }
        }
    }
}
