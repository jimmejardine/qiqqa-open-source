using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Mathematics.Statistics
{
	public class Diffusions
	{
		/**
		 * Note: the offset is provided to allow you to apply a correlation matrix to the bottom values of the Z vector only.
		 */
		public static void convertUncorrelatedToCorrelated(Vector Z_in, Vector Z_out, Matrix correlation_root, int offset)
		{
			for (int i = offset; i < Z_in.cols; ++i)
			{
				Z_out[i] = 0.0;
				for (int j = offset; j <= i; ++j)
				{
					Z_out[i] += Z_in[j] * correlation_root[i-offset, j-offset];
				}
			}
		}
	}
}
