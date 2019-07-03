using System;
using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Random
{
	/// <summary>
	/// Summary description for RandomAugmented.
	/// </summary>
	public class RandomAugmented : System.Random, IUniformRandomSource
	{
		public RandomAugmented() :
			base()
		{
		}

		public RandomAugmented(int ticks) :
			base(ticks)
		{
		}

		public void reset()
		{
			// We can't reset this number source :(
		}

        public int NextIntExclusive(int max)
		{
			return (int) (max * this.NextDouble());
		}

		public int NextInt(int max)
		{
			return (int) ((1.0 + max) * this.NextDouble());
		}

		public int NextIntBalanced(int max)
		{
			return (int) ((1.0 + max) * this.NextDoubleBalanced());
		}

		public double NextDouble(double max)
		{
			return max * this.NextDouble();
		}

		public double NextDoubleBalanced()
		{
			return 2.0 * NextDouble() - 1.0;
		}

		public double NextDoubleBalanced(double max)
		{
			return max * this.NextDoubleBalanced();
		}

		// ---------------------------------------------------------------

        public static RandomAugmented Instance = new RandomAugmented((int)DateTime.Now.Ticks);
        
		public static RandomAugmented getSeededRandomAugmented()
		{
            return Instance;
		}


		public double nextRandomDouble()
		{
			return NextDouble();
		}

		public void nextRandomVector(Vector vector)
		{
			for (int i = 0; i < vector.cols; ++i)
			{
				vector[i] = nextRandomDouble();
			}
		}

		// Our random numbers are independent so a multiple series is nothing special
		public void nextRandomVectorMultiple(int N, Vector vector)
		{
			nextRandomVector(vector);
		}

		// Our random numbers are independent so a diminishing series is nothing special
		public void nextRandomVectorDiminishing(int N, Vector vector)
		{
			nextRandomVector(vector);
		}
	}
}
