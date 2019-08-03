using Utilities.GUI.Charting;
using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Random
{
	/**
	 * This class is an implementation of the paper "Mersenne Twister: A 623-Dimensionally Equidistributed Uniform Pseudo-Random Number Generator" by Matsumoto
	 */
	public class MersenneTwister : IUniformRandomSource
	{
		const int N = 624;
		const int M = 397;
		const uint MATRIX_A = 0x9908b0df;
		const uint UPPER_MASK = 0x80000000;
		const uint LOWER_MASK = 0x7fffffff;
		const uint TEMPERING_MASK_B = 0x9d2c5680;
		const uint TEMPERING_MASK_C = 0xefc60000;
		ulong[] mt = new ulong[N];
		int mti = N+1;
		ulong[] mag01 = {0, MATRIX_A};

		const ulong DEFAULT_SEED = 4357;
		ulong seed_used;
	
		public MersenneTwister()
		{
			SeedMT(DEFAULT_SEED);
		}

		public MersenneTwister(ulong seed)
		{
			if (0 == seed)
			{
				throw new GenericException("Seed can not be zero.");
			}

			SeedMT(seed);
		}

		public void reset()
		{
			SeedMT(seed_used);
		}

		private void SeedMT(ulong seed)
		{
			seed_used = seed;

			mt[0] = seed & 0xffffffff;
			for (mti = 1; mti < N; ++mti)
			{
				mt[mti] = (69069 * mt[mti-1]) & 0xffffffff;
			}
		}

		public ulong RandomInt()
		{
			ulong y;
			if (mti >= N)
			{
				int kk;

				for (kk = 0; kk < N-M; ++kk)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk+1] & LOWER_MASK);
					mt[kk] = mt[kk+M] ^ (y >> 1) ^ mag01[y&0x1];
				}

				for (;kk < N-1; ++kk)
				{
					y = (mt[kk] & UPPER_MASK) | (mt[kk+1] & LOWER_MASK);
					mt[kk] = mt[kk+(M-N)] ^ (y >> 1) ^ mag01[y&0x1];
				}

				y = (mt[N-1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
				mt[N-1] = mt[M-1] ^ (y >> 1) ^ mag01[y&0x1];

				mti = 0;
			}

			y = mt[mti++];
			y ^= (y >> 11);
			y ^= (y << 7) & TEMPERING_MASK_B;
			y ^= (y << 15) & TEMPERING_MASK_C;
			y ^= (y >> 18);

			return y;
		}

		public double RandomDouble()
		{
			return ((double) RandomInt() / (double) 0xffffffff);
		}

		public void RandomUniformVector(Vector vector)
		{
			for (int i = 0; i < vector.cols; ++i)
			{
				vector[i] = RandomDouble();
			}
		}

		// --- Implements the IUniformRandomSource interface
        public int NextIntExclusive(int max)
        {
            return (int)((max-1) * this.nextRandomDouble());
        }

        public double NextDouble()
        {
            return RandomDouble();
        }


        public double nextRandomDouble()
		{
            return RandomDouble();
		}

		public void nextRandomVector(Vector vector)
		{
			RandomUniformVector(vector);
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

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			MersenneTwister mt = new MersenneTwister();
			Series series = new Series("Random", ChartType.Point);
			for (int i = 0; i < 1000; ++i)
			{
				series.addPoint(mt.RandomDouble(), mt.RandomDouble());
			}
			MultiChart2D chart = new MultiChart2D();
			chart.addSeries(series);
			SingleControlForm form = new SingleControlForm();
			form.setControl(chart);
			form.Show();
		}
#endif

        #endregion
    }
}
