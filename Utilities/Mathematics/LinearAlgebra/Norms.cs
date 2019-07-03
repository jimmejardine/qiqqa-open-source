using System;

namespace Utilities.Mathematics.LinearAlgebra
{
	public class Norms
	{
		public static double Norm1(Matrix vector)
		{
			// Is it a row vector?
			if (1 == vector.rows)
			{
				double total = 0.0;
				for (int i = 0; i < vector.cols; ++i)
				{
					total += vector.data[0, i];
				}
				return total;
			}

			// Is it a column vector?
			if (1 == vector.cols)
			{
				double total = 0.0;
				for (int i = 0; i < vector.rows; ++i)
				{
					total += vector.data[i, 0];
				}
				return total;
			}

			// If we get this far, it is neither a row nor column vector
			throw new GenericException("Can't find the norm of a matrix");
		}

		public static double Norm2(Matrix vector)
		{
			// Is it a row vector?
			if (1 == vector.rows)
			{
				double total = 0.0;
				for (int i = 0; i < vector.cols; ++i)
				{
					total += vector.data[0, i]*vector.data[0, i];
				}
				return Math.Sqrt(total);
			}

			// Is it a column vector?
			if (1 == vector.cols)
			{
				double total = 0.0;
				for (int i = 0; i < vector.rows; ++i)
				{
					total += vector.data[i, 0]*vector.data[i, 0];
				}
				return Math.Sqrt(total);
			}

			// If we get this far, it is neither a row nor column vector
			throw new GenericException("Can't find the norm of a matrix");
		}
	}
}
