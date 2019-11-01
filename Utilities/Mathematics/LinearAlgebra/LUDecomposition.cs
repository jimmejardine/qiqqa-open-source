using System;

namespace Utilities.Mathematics.LinearAlgebra
{
    public class LUDecomposition
    {
        private Matrix LU;
        private int[] indx;
        private double d;

        /*
		 * As implemented in Numerical Recipes in C, 2nd edition, pg 46
		 */
        public LUDecomposition(Matrix matrix)
        {
            if (!matrix.isSquare())
            {
                throw new GenericException("Cannot LU decompose a non-square matrix");
            }

            // Initialise data structure
            int n = matrix.rows;
            LU = new Matrix(n, n);
            LU.assign(matrix);
            indx = new int[matrix.rows];
            d = 1.0;

            int imax = 0;
            double[] vv = new double[n];

            // Loop over rows to get the implicit scaling information
            for (int i = 0; i < n; ++i)
            {
                double big = 0.0;
                for (int j = 0; j < n; ++j)
                {
                    if (Math.Abs(LU.data[i, j]) > big)
                    {
                        big = Math.Abs(LU.data[i, j]);
                    }
                }

                if (0.0 == big)
                {
                    throw new GenericException("Can not decompose singular matrix.");
                }

                vv[i] = 1.0 / big;
            }

            // Loop over columns of Crout's method
            for (int j = 0; j < n; ++j)
            {
                for (int i = 0; i < j; ++i)
                {
                    double sum = LU.data[i, j];
                    for (int k = 0; k < i; ++k)
                    {
                        sum -= LU.data[i, k] * LU.data[k, j];
                    }
                    LU.data[i, j] = sum;
                }

                // Search for the largest pivot method
                double big = 0.0;
                for (int i = j; i < n; ++i)
                {
                    double sum = LU.data[i, j];
                    for (int k = 0; k < j; ++k)
                    {
                        sum -= LU.data[i, k] * LU.data[k, j];
                    }
                    LU.data[i, j] = sum;

                    if (vv[i] * Math.Abs(sum) > big)
                    {
                        big = vv[i] * Math.Abs(sum);
                        imax = i;
                    }
                }

                // Do we need to interchange rows?
                if (j != imax)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        Swap.swap(ref LU.data[imax, k], ref LU.data[j, k]);
                    }
                    d = -d;
                    vv[imax] = vv[j];
                }

                indx[j] = imax;
                if (LU.data[j, j] == 0.0)
                {
                    throw new GenericException("Matrix is singular to machine precision.");
                }

                if (j != n)
                {
                    double dum = 1.0 / LU.data[j, j];
                    for (int i = j + 1; i < n; ++i)
                    {
                        LU.data[i, j] *= dum;
                    }
                }
            }
        }


        /**
		 * Solves for x in equations of the form Ax=b, where A is the matrix against which this LUDecomposition instance was created.
		 */
        public Matrix solve(Matrix b)
        {
            return solve_matrix(b);
        }

        /**
		 * Solves for x in equations of the form Ax=b, where A is the matrix against which this LUDecomposition instance was created.
		 * Can solve an entire matrix
		 */
        public Matrix solve_matrix(Matrix b)
        {
            if (b.rows != LU.rows)
            {
                throw new GenericException("Vector b has wrong number of rows");
            }


            int n = b.rows;
            Matrix result = new Matrix(b.rows, b.cols);
            result.assign(b);

            for (int col = 0; col < b.cols; ++col)
            {
                int ii = -1;

                // Do the forward substitution with permutation unscrambling
                for (int i = 0; i < n; ++i)
                {
                    int ip = indx[i];
                    double sum = result.data[ip, col];
                    result.data[ip, col] = result.data[i, col];
                    if (ii != -1)
                    {
                        for (int j = ii; j <= i - 1; ++j)
                        {
                            sum -= LU.data[i, j] * result.data[j, col];
                        }
                    }
                    else if (sum != 0)
                    {
                        ii = i;
                    }
                    result.data[i, col] = sum;
                }

                // Now for some back substitution
                for (int i = n - 1; i >= 0; --i)
                {
                    double sum = result.data[i, col];
                    for (int j = i + 1; j < n; ++j)
                    {
                        sum -= LU.data[i, j] * result.data[j, col];
                    }
                    result.data[i, col] = sum / LU.data[i, i];
                }
            }

            return result;
        }

        /**
		 * Solves for x in equations of the form Ax=b, where A is the matrix against which this LUDecomposition instance was created.
		 * Can solve only a single vector
		 */
        public Matrix solve_vector(Matrix b)
        {
            if (b.cols != 1)
            {
                throw new GenericException("Vector b must have one column");
            }
            if (b.rows != LU.rows)
            {
                throw new GenericException("Vector b has wrong number of rows");
            }

            int ii = -1;

            int n = b.rows;
            Matrix result = new Matrix(b.rows, b.cols);
            result.assign(b);

            // Do the forward substitution with permutation unscrambling
            for (int i = 0; i < n; ++i)
            {
                int ip = indx[i];
                double sum = result.data[ip, 0];
                result.data[ip, 0] = result.data[i, 0];
                if (ii != -1)
                {
                    for (int j = ii; j <= i - 1; ++j)
                    {
                        sum -= LU.data[i, j] * result.data[j, 0];
                    }
                }
                else if (sum != 0)
                {
                    ii = i;
                }
                result.data[i, 0] = sum;
            }

            // Now for some back substitution
            for (int i = n - 1; i >= 0; --i)
            {
                double sum = result.data[i, 0];
                for (int j = i + 1; j < n; ++j)
                {
                    sum -= LU.data[i, j] * result.data[j, 0];
                }
                result.data[i, 0] = sum / LU.data[i, i];
            }

            return result;
        }


        public String dump()
        {
            return LU.dump();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
//			Matrix A = new Matrix(3, 3);
//			A.setIdentity();
//			A.data[0,1] = A.data[1,0] = 0.2;
//			A.data[0,2] = A.data[2,0] = 0.3;
//			A.data[1,2] = A.data[2,1] = 0.1;
//			System.Console.WriteLine("A is\n{0}", A.dump());
//
//			LUDecomposition A_lu = new LUDecomposition(A);
//			System.Console.WriteLine("LU is\n{0}", A_lu.dump());
			
			Matrix B = new Matrix(3, 3);
			B.setIdentity();
			B.data[0,1] = B.data[1,0] = 0.2;
			B.data[0,2] = B.data[2,0] = 0.3;
			B.data[1,2] = 0.1;
			B.data[2,0] = 0.4;
			Console.WriteLine("B is\n{0}", B.dump());

			LUDecomposition B_lu = new LUDecomposition(B);
			Console.WriteLine("LU is\n{0}", B_lu.dump());

			Matrix b = new Matrix(3, 1);
			b.data[0,0] = 1.0;
			b.data[1,0] = 2.0;
			b.data[2,0] = 3.0;
			Console.WriteLine("b is\n{0}", b.dump());

			Matrix x = B_lu.solve(b);
			Console.WriteLine("x is\n{0}", x.dump());

			Matrix test_b = B.multiply(x);
			Console.WriteLine("test_B is\n{0}", test_b.dump());
		}
#endif

        #endregion
    }
}
