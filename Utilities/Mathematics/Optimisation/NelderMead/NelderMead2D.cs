using System;

namespace Utilities.Mathematics.Optimisation.NelderMead
{
	public class NelderMead2D
	{
		const double GROW_FACTOR = 2.0;
		const double REFLECT_FACTOR = -1.0;
		const double CONTRACT_FACTOR = 0.5;
		const double SHRINK_FACTOR = 0.5;

		public double MAX_ITERATIONS = 10000;
		public double TOLERANCE = 1E-12;

		ObjectiveFunction2D objective_function;
		Point2D[] points = new Point2D[3];
		double[] scores = new double[3];

		public void initialiseSearch(ObjectiveFunction2D aobjective_function, Point2D p1, Point2D p2, Point2D p3)
		{
			objective_function = aobjective_function;

			// Store our initial conditions
			points[0] = p1;
			points[1] = p2;
			points[2] = p3;

			// Evaluate our initial conditions
			for (int i = 0; i < 3; ++i)
			{
				scores[i] = objective_function.evaluate(points[i]);
			}
			sortPoints();
		}

		public Point2D search(out string error_message, out double optimum_score)
		{
			// Check that we have been initialised
			if (null == objective_function)
			{
				throw new GenericException("Objective function not set.  Can not search.");
			}

			// Initialise the error message
			error_message = "";

			// Keeps track of how many iterations we have done
			int num_iterations = 0;

			while (true)
			{
//				for (int i = 0; i < 3; ++i)
//				{
//					System.Console.WriteLine("{0} : Rho is {1}, nu is {2}, score is {3}", i, points[i].x, points[i].y, scores[i]);
//				}

				// Check our tolerance.  If we are within it, return
				double tolerance = Math.Abs(points[0].x - points[2].x) + Math.Abs(points[0].y - points[2].y);
				tolerance /= 2;
				if (tolerance < TOLERANCE)
				{
					optimum_score = scores[0];
					return points[0];
				}

				// Check that we have not exceeded the number of iterations
				++num_iterations;
				//System.Console.WriteLine("Iteration {0}: {1}, {2}; {3}, {4}; {5}, {6}", num_iterations, points[0].x, points[0].y, points[1].x, points[1].y, points[2].x, points[2].y);
				if (num_iterations > MAX_ITERATIONS)
				{
					error_message = "Exceeded number of iterations in the Nelder-Mead search.  Results may be unstable!";
					optimum_score = scores[0];
					return points[0];
				}

				// Find the mid point of the two best points
				Point2D mid_point;
				mid_point.x = 0.5 * (points[0].x + points[1].x);
				mid_point.y = 0.5 * (points[0].y + points[1].y);

				// Do a reflection test
				if (testOnePointMove(REFLECT_FACTOR, ref mid_point))
				{
					// If we managed a reflection, test a little further out
					testOnePointMove(GROW_FACTOR, ref mid_point);
				}

				// If the reflection failed, then try a contraction
				else if (testOnePointMove(CONTRACT_FACTOR, ref mid_point))
				{
				}

				// If reflection and contraction failed, then shrink towards our best point
				else
				{
					doTwoPointMove();
				}
			}
		}

		void doTwoPointMove()
		{
			points[1].x = SHRINK_FACTOR * (points[1].x + points[0].x);
			points[1].y = SHRINK_FACTOR * (points[1].y + points[0].y);
			objective_function.constrainSearch(ref points[1]);
			scores[1] = objective_function.evaluate(points[1]);

			points[2].x = SHRINK_FACTOR * (points[2].x + points[0].x);
			points[2].y = SHRINK_FACTOR * (points[2].y + points[0].y);
			objective_function.constrainSearch(ref points[2]);
			scores[2] = objective_function.evaluate(points[2]);

			sortPoints();		
		}

		bool testOnePointMove(double scale, ref Point2D mid_point)
		{
			Point2D new_point;
			new_point.x = mid_point.x + scale * (points[2].x - mid_point.x);
			new_point.y = mid_point.y + scale * (points[2].y - mid_point.y);

			// Make sure our new point obeys the constraints
			objective_function.constrainSearch(ref new_point);
			
			// See if our new point is better
			double new_score = objective_function.evaluate(new_point);

			// If it is, replace our old point and return true
			if (new_score < scores[2])
			{
				points[2].x = new_point.x;
				points[2].y = new_point.y;
				scores[2] = new_score;

				sortPoints();
				
				return true;
			}

			// If it is not smaller, return false
			else
			{
				return false;
			}
		}

		void sortPoints()
		{
			if (scores[0] > scores[1])
			{
				swapPoints(0, 1);
			}
			if (scores[0] > scores[2])
			{
				swapPoints(0, 2);
			}
			if (scores[1] > scores[2])
			{
				swapPoints(1, 2);
			}
		}

		void swapPoints(int i, int j)
		{
			Swap.swap(ref scores[i], ref scores[j]);
			Swap.swap(ref points[i].x, ref points[j].x);
			Swap.swap(ref points[i].y, ref points[j].y);
		}
	}
}
