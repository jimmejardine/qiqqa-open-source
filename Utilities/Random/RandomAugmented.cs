using System;
using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Random
{
	/// <summary>
	/// Summary description for RandomAugmented.
	/// </summary>
	public class RandomAugmented : IUniformRandomSource
	{
        private System.Random rand;

        public RandomAugmented()
		{
            rand = new System.Random();
		}

		public RandomAugmented(int ticks)
		{
            rand = new System.Random(Math.Max(1, ticks));
        }

        public void reset()
        {
            reset(1);
        }
        public void reset(int seed)
		{
            // We can't reset this number source :(
            //
            // But we CAN initialize a fresh random generator:
            rand = new System.Random(Math.Max(1, seed));
        }

        public int NextIntExclusive(int max)
		{
			return (int) NextDouble(max);
		}

		public int NextInt(int max)
		{
			return (int) NextDouble(1.0 + max);
		}

		public int NextIntBalanced(int max)
		{
			return (int) NextDoubleBalanced(1.0 + max);
		}

		public double NextDouble(double max = 1.0)
		{
			return max * nextRandomDouble();
		}

		public double NextDoubleBalanced(double max = 1.0)
		{
			return 2.0 * NextDouble(max) - 1.0;
		}

		// ---------------------------------------------------------------

        public static RandomAugmented Instance = new RandomAugmented((int)DateTime.Now.Ticks);
        
		public double nextRandomDouble()
		{
			return rand.NextDouble();
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

        //
        // Summary:
        //     Fills the elements of a specified array of bytes with random numbers.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes to contain random numbers.
        //
        // Exceptions:
        //   System.ArgumentNullException: buffer is null.
        public void NextBytes(byte[] buffer)
        {
            rand.NextBytes(buffer);
        }
    }
}
