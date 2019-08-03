using System;

namespace Utilities.Mathematics.LinearAlgebra
{
	public class Cholesky
	{
		public Matrix decomposition;

		public Cholesky(Matrix matrix)
		{
			if (!matrix.isSquare())
			{
				throw new GenericException("Cannot LU decompose a non-square matrix");
			}

			if (!matrix.isSymmetric())
			{
				throw new GenericException("Cannot LU decompose a non-symmetric matrix");
			}

			decomposition = new Matrix(matrix.rows, matrix.cols);
			decomposition.data[0, 0] = matrix.data[0, 0];
			for (int i = 0; i < matrix.rows; ++i)
			{
				for (int j = i; j < matrix.cols; ++j)
				{
					double sum = matrix.data[i, j];
					for (int k = 0; k < i; ++k)
					{
						sum -= matrix.data[i, k] * matrix.data[j, k];
					}

					if (i == j)
					{
						if (sum <= 0)
						{
							throw new GenericException("Cannot LU decompose a non positive-definite matrix");
						}
						decomposition.data[i, i] = Math.Sqrt(sum);
					}

					decomposition.data[j, i] = sum / decomposition.data[i, i];
				}
			}
		}

		
		/**
		 * Solves for x in equations of the form Ax=b, where A is the matrix against which this Cholesky instance was created.
		 */
		public Matrix solve(Matrix b)
		{
			if (b.cols != 1)
			{
				throw new GenericException("Vector b must have one column");
			}
			if (b.rows != decomposition.rows)
			{
				throw new GenericException("Vector b has wrong number of rows");
			}

			Matrix result = new Matrix(b.rows, b.cols);

			// Solve Ly = b
			for (int i = 0; i < b.rows; ++i)
			{
				double sum = b.data[i, 0];
				for (int k = i-1; k >= 0; --k)
				{
					sum -= decomposition.data[i, k] * result.data[k, 0];
				}
				result.data[i, 0] = sum / decomposition.data[i, i];
			}

			// Solve L'x = y
			for (int i = b.rows-1; i >= 0; --i)
			{
				double sum = result.data[i, 0];
				for (int k = i+1; k < b.rows; ++k)
				{
					sum -= decomposition.data[k, i] * result.data[k, 0];
				}
				result.data[i, 0] = sum / decomposition.data[i, i];
			}

			return result;
		}

		public String dump()
		{
			return decomposition.dump();
		}


		public static Matrix factorise(Matrix matrix)
		{
			return factorise(matrix, true);
		}

		public static Matrix factorise(Matrix matrix, bool reduced_factor_allowed)
		{
			// Check that the matrix is square
			if (!matrix.isSquare())
			{
				throw new GenericException("Can only Cholesky factorise square matrices.");
			}
			if (!matrix.isSymmetric())
			{
				throw new GenericException("Can only Cholesky factorise symmetric matrices.");
			}

			// Create our result matrix
			Matrix result = new Matrix(matrix.rows, matrix.cols);

			if (0 == matrix.rows)
			{
				return result;
			}

			result.data[0,0] = 1.0;

			for (int i = 0; i < matrix.rows; ++i)
			{
				for (int j = i; j < matrix.cols; ++j)
				{
					// Calculate the sum of the entries up to the diagonal
					double sum = matrix.data[i,j];
					for (int k = 0; k <= i-1; ++k)
					{
						sum = sum - result.data[i,k] * result.data[j,k];
					}

					// If we are at the diagonal
					if (i == j)
					{
						if (sum <= 0)
						{
							if (!reduced_factor_allowed)
							{
								throw new GenericException("Can only Cholesky factorise positive definite matrices.");
							}

							result.data[i,i] = 0;
						}
						else
						{
							result.data[i,i] = Math.Sqrt(sum);
						}
					}

					// Store the off-diagonal element
					if (0 != result.data[i,i])
					{
						result.data[j,i] = sum / result.data[i,i];
					}
					else
					{
						result.data[j,i] = 0;
					}
				}
			}

			return result;
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Matrix A = new Matrix(3, 3);
			A.setIdentity();
			A.data[0,1] = A.data[1,0] = 0.2;
			A.data[0,2] = A.data[2,0] = 0.3;
			A.data[1,2] = A.data[2,1] = 0.1;
			Console.WriteLine(A.dump());

			Matrix factor = factorise(A);
			Console.WriteLine(factor.dump());

			Matrix test = factor.multiply(factor.transpose());
			Console.WriteLine(test.dump());
		}
#endif

        #endregion
    }
}
