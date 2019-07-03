using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Mathematics.Statistics.Distributions
{
	public class Genz4VariateClosed
	{
		public Genz4VariateClosed()
		{
		}

		public static double cumnorm(Vector a, Matrix M)
		{
			// Copy our original matrix
			Matrix K = new Matrix(M);

			// Remove the bottom-right correlation coefficient
			double removed_coefficient = K[2,3];
			K[2,3] = K[3,2] = 0.0;

			
			
			return 0.0;
		}
		
		public static void TestHarness()
		{
		}
	}
}
