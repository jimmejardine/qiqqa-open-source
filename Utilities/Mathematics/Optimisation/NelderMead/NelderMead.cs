using System;
using Utilities.Random;

namespace Utilities.Mathematics.Optimisation.NelderMead
{
	public class NelderMead
	{
		const double GROW_FACTOR = 2.0;
		const double REFLECT_FACTOR = -1.0;
		const double CONTRACT_FACTOR = 0.5;
		const double SHRINK_FACTOR = 0.5;

		public double MAX_ITERATIONS = 10000;
		public double TOLERANCE = 1E-12;

		int dimensions;
		ObjectiveFunction objective_function;
		double[][] points;
		double[] scores;

		public NelderMead(int adimensions)
		{
			dimensions = adimensions;
			points = new double[dimensions+1][];
			for (int i = 0; i < dimensions+1; ++i)
			{
				points[i] = new double[dimensions];
			}
			scores = new double[dimensions+1];
		}

		public double[][] generateEmptyStartupParameters(double[] scales)
		{
			RandomAugmented ra = RandomAugmented.Instance;

			double[][] initial_points = new double[dimensions+1][];
			for (int i = 0; i < dimensions+1; ++i)
			{
				initial_points[i] = new double[dimensions];
				for (int j = 0; j < dimensions; ++j)
				{
					initial_points[i][j] = ra.NextDoubleBalanced(scales[j]);
				}
			}

			return initial_points;
		}
		
		public void initialiseSearch(ObjectiveFunction aobjective_function, double[][] starting_points)
		{
			objective_function = aobjective_function;

			// Store our initial conditions
			for (int i = 0; i < dimensions+1; ++i)
			{
				for (int j = 0; j < dimensions; ++j)
				{
					points[i][j] = starting_points[i][j];
				}
			}

			// Evaluate our initial conditions
			for (int i = 0; i < dimensions+1; ++i)
			{
				scores[i] = objective_function.evaluate(points[i]);
			}
			sortPoints();
		}

		public double[] search(out string error_message, out double optimum_score)
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

			// The latest midpoint and newpoint we have for reflections, etc.  Defined here for memory allocation efficiency
			double[] mid_point = new double[dimensions];
			double[] new_point = new double[dimensions];

			while (true)
			{
//				for (int i = 0; i < dimensions+1; ++i)
//				{
//					System.Console.WriteLine("{0} : Rho is {1}, nu is {2}, score is {3}", i, points[i][0], points[i][1], scores[i]);
//				}

				// Check our tolerance.  If we are within it, return
				double tolerance = 0.0;
				for (int j = 0; j < dimensions; ++j)
				{
					tolerance += Math.Abs(points[0][j] - points[dimensions][j]);
				}
				tolerance /= dimensions;
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

				// Find the mid point of the best points (note n points, not n+1, since we leave out the worst)
				for (int j = 0; j < dimensions; ++j)
				{
					mid_point[j] = 0.0;
				}
				for (int i = 0; i < dimensions; ++i)
				{
					for (int j = 0; j < dimensions; ++j)
					{
						mid_point[j] += points[i][j];
					}
				}
				for (int j = 0; j < dimensions; ++j)
				{
					mid_point[j] /= dimensions;
				}

				// Do a reflection test
				if (testOnePointMove(REFLECT_FACTOR, ref mid_point, ref new_point))
				{
					// If we managed a reflection, test a little further out
					testOnePointMove(GROW_FACTOR, ref mid_point, ref new_point);
				}

					// If the reflection failed, then try a contraction
				else if (testOnePointMove(CONTRACT_FACTOR, ref mid_point, ref new_point))
				{
				}

					// If reflection and contraction failed, then shrink towards our best point
				else
				{
					doManyPointMove();
				}
			}
		}

		void doManyPointMove()
		{
			for (int i = 1; i < dimensions+1; ++i)
			{
				for (int j = 1; j < dimensions; ++j)
				{
					points[i][j] = SHRINK_FACTOR * (points[i][j] + points[0][j]);
				}
				objective_function.constrainSearch(ref points[i]);
				scores[i] = objective_function.evaluate(points[i]);
			}

			sortPoints();		
		}

		bool testOnePointMove(double scale, ref double[] mid_point, ref double[] new_point)
		{
			// Calculate our new point dimensions
			for (int j = 0; j < dimensions; ++j)
			{
				new_point[j] = mid_point[j] + scale * (points[dimensions][j] - mid_point[j]);
			}

			// Make sure our new point obeys the constraints
			objective_function.constrainSearch(ref new_point);
			
			// See if our new point is better
			double new_score = objective_function.evaluate(new_point);

			// If it is, replace our old point and return true
			if (new_score < scores[dimensions])
			{
				for (int j = 0; j < dimensions; ++j)
				{
					points[dimensions][j] = new_point[j];
				}
				scores[dimensions] = new_score;

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
			// Lets do a simple bubble sort
			// This can be improved since the only unsorted one is usually at the end of the array
			// But its probably not worth it cos there are always so few dimensions
			for (int i = 0; i < dimensions; ++i)
			{
				for (int j=i+1; j < dimensions+1; ++j)
				{
					if (scores[i] > scores[j])
					{
						swapPoints(i, j);
					}
				}
			}
		}

		void swapPoints(int a, int b)
		{
			Swap.swap(ref scores[a], ref scores[b]);
			for (int j = 0; j < dimensions; ++j)
			{
				Swap.swap(ref points[a][j], ref points[b][j]);
			}
		}
	}
}
