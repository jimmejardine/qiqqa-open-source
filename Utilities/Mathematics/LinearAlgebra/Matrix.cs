using System;
using System.Text;

namespace Utilities.Mathematics.LinearAlgebra
{
	public class Matrix
	{
		public int rows;
		public int cols;
		public double[,] data;

		public Matrix(int arows, int acols)
		{
			resize(arows, acols);
		}

		public Matrix(double[,] adata)
		{
            rows = adata.GetLength(0);
            cols = adata.GetLength(1);
			data = adata;
		}
		
		public Matrix(Matrix other)
		{
			resize(other.rows, other.cols);
			
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					data[i, j] = other.data[i, j];
				}
			}
		}

        public Matrix(float[,] adata)
        {
            rows = adata.GetLength(0);
            cols = adata.GetLength(1);
            data = new double[rows, cols];

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    data[i, j] = adata[i, j];
                }
            }
        }

        public float[,] ToFloatMatrix()
        {
            float[,] adata = new float[rows, cols];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    adata[i, j] = (float)data[i, j];
                }
            }
            return adata;
        }

        public void resize(int arows, int acols)
		{
			rows = arows;
			cols = acols;
			data = new double[rows, cols];
		}

		public double this[int i, int j]
		{
			get
			{
				return data[i,j];
			}

			set
			{
				data[i,j] = value;
			}
		}

		public void assign(Matrix b)
		{
			if (!isDimensionCompatible(b))
			{
				throw new GenericException("Can not assign from an incompatible matrix.");
			}

			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					this[i,j] = b[i,j];
				}
			}
		}

		public bool isDimensionCompatible(Matrix b)
		{
			return (this.rows == b.rows && this.cols == b.cols);
		}

		public double asScalar()
		{
			if (1 != rows && 1 != cols)
			{
				throw new GenericException("Can convert only a 1x1 matrix to a scalar.");
			}

			return data[0, 0];
		}

		public void multiplyColumn(int j, double factor)
		{
			for (int i = 0; i < rows; ++i)
			{
				data[i,j] *= factor;
			}
		}

		public Matrix transpose()
		{
			Matrix result = new Matrix(cols, rows);
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[j, i] = data[i, j];
				}
			}
			return result;
		}

		public bool isSquare()
		{
			return rows == cols;
		}

		public bool isSymmetric()
		{
			for (int i = 0; i < rows; ++i)
			{
				for (int j = i+1; j < cols; ++j)
				{
					if (data[i, j] != data[j, i])
					{
						return false;
					}
				}
			}

			return true;
		}

		public Matrix add(Matrix B)
		{
			// A symbolic name for our own matrix
			Matrix A = this;

			// Check that the matrices are compatible
			if (A.cols != B.cols || A.rows != B.rows)
			{
				throw new GenericException("Cannot add matrices with different dimensions");
			}

			Matrix result = new Matrix(A.rows, A.cols);
			for (int i = 0; i < A.rows; ++i)
			{
				for (int j = 0; j < A.cols; ++j)
				{
					result[i,j] = A[i, j] + B[i, j];
				}
			}

			return result;
		}
		
		public Matrix subtract(Matrix B)
		{
			// A symbolic name for our own matrix
			Matrix A = this;

			// Check that the matrices are compatible
			if (A.cols != B.cols || A.rows != B.rows)
			{
				throw new GenericException("Cannot add matrices with different dimensions");
			}

			Matrix result = new Matrix(A.rows, A.cols);
			for (int i = 0; i < A.rows; ++i)
			{
				for (int j = 0; j < A.cols; ++j)
				{
					result[i,j] = A[i, j] - B[i, j];
				}
			}

			return result;
		}

		public Matrix multiply(double b)
		{
			Matrix result = new Matrix(rows, cols);
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i,j] = b * data[i, j];
				}
			}

			return result;
		}

		public Matrix multiply(Matrix B)
		{
			// A symbolic name for our own matrix
			Matrix A = this;

			// Check that the matrices are compatible
			if (A.cols != B.rows)
			{
				throw new GenericException("Cannot multiply two dimension incompatible matrices");
			}

			// Create the new matrix and perform the element calculation
			Matrix result = new Matrix(A.rows, B.cols);
			for (int i = 0; i < A.rows; ++i)
			{
				for (int j = 0; j < B.cols; ++j)
				{
					double total = 0.0;
					for (int k = 0; k < A.cols; ++k)
					{
						total += A[i, k] * B[k, j];
					}
					result[i, j] = total;
				}
			}

			return result;
		}

		public Matrix multiply(Vector b)
		{
			// Gonna be slow.  Can improve this sometime...
			Matrix B = b.toMatrix();
			return this.multiply(B);
		}

		public void setIdentity()
		{
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					data[i, j] = i == j ? 1 : 0;
				}
			}
		}		
		
		void setValue(double a)
		{
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					data[i, j] = a;
				}
			}
		}
		
		public void setZeros()
		{
			setValue(0);
		}
		
		public void setOnes()
		{
			setValue(1);
		}

		public Matrix inverse()
		{
			return inverse_lu();
		}

		/**
		 * Uses LU decomposition to determine inverse
		 */
		public Matrix inverse_lu()
		{
			LUDecomposition lu = new LUDecomposition(this);
			Matrix identity = new Matrix(this.cols, this.rows);
			identity.setIdentity();
			Matrix inverse = lu.solve(identity);
			return inverse;
		}
		
		/**
		 * Very crude implementation.  Beware!
		 * You may rather want to use LU decomposition...
		 */		
		public Matrix inverse_crude()
		{
			if (!isSquare())
			{
				throw new GenericException("Can not invert a non-square matrix.");
			}

			// Create our result matrix
			Matrix result_matrix = new Matrix(rows, cols);
			result_matrix.setIdentity();
			
			// Set up some references for our algorithm
			double[,] work = cloneData();
			double[,] result = result_matrix.data;			

			// Perform the reduction on the bottom triangle
			for (int j = 0; j < cols; ++j)
			{
				for (int i = j+1; i < rows; ++i)
				{
					double reduction_factor = work[i, j] / work[j, j];
					reduceRowByFactoredRow(work, i, j, reduction_factor, cols);
					reduceRowByFactoredRow(result, i, j, reduction_factor, cols);
				}
			}

			// Perform the reduction on the top triangle
			for (int j = cols-1; j >= 0; --j)
			{
				for (int i = j-1; i >= 0; --i)
				{
					double reduction_factor = work[i, j] / work[j, j];
					reduceRowByFactoredRow(work, i, j, reduction_factor, cols);
					reduceRowByFactoredRow(result, i, j, reduction_factor, cols);
				}
			}

			// Slay down the diagonals
			for (int i = 0; i < rows; ++i)
			{
				reduceRowByFactor(work, i, work[i, i], cols);
				reduceRowByFactor(result, i, work[i, i], cols);
			}

			return result_matrix;
		}

		private static void reduceRowByFactoredRow(double[,] matrix, int target_row, int pivot_row, double reduction_factor, int cols)
		{
			for (int i = 0 ; i < cols; ++i)
			{
				matrix[target_row, i] = matrix[target_row, i] - matrix[pivot_row, i] * reduction_factor;
			}
		}

		private static void reduceRowByFactor(double[,] matrix, int target_row, double reduction_factor, int cols)
		{
			for (int i = 0 ; i < cols; ++i)
			{
				matrix[target_row, i] = matrix[target_row, i] / reduction_factor;
			}
		}

		double[,] cloneData()
		{
			double[,] result = new double[rows, cols];
			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					result[i,j] = data[i,j];
				}
			}

			return result;
		}

		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					sb.AppendFormat("{0,8}\t", data[i, j]);
				}
				sb.AppendFormat("\n");
			}

			return sb.ToString();
		}

		public string dump()
		{	
			return ToString();
		}

		public void exchangeRows(int m, int n)
		{
			for (int i = 0; i < this.cols; ++i)
			{
				double temp = data[m,i];
				data[m,i] = data[n,i];
				data[n,i] = temp;
			}
		}

		public void exchangeCols(int m, int n)
		{
			for (int i = 0; i < this.rows; ++i)
			{
				double temp = data[i,m];
				data[i,m] = data[i,n];
				data[i,n] = temp;
			}
		}

		public void getColumn(int j, Vector data)
		{
			for (int i = 0; i < this.rows; ++i)
			{
				data[i] = this[i,j];
			}
		}

		public void getRow(int i, Vector data)
		{
			for (int j = 0; j < this.cols; ++j)
			{
				data[j] = this[i,j];
			}
		}
		
		public void zeroRow(int row)
		{
			for (int j = 0; j < cols; ++j)
			{
				this[row,j] = 0.0;
			}
		}

		public void zeroColumn(int col)
		{
			for (int i = 0; i < rows; ++i)
			{
				this[i, col] = 0.0;
			}
		}
		
		public Matrix removeRowAndColumn(int row, int col)
		{
			Matrix result = new Matrix(this.rows-1, this.cols-1);
			for (int i = 0; i < this.rows; ++i)
			{
				if (i == row)
				{
					continue;
				}
				int i_target = i < row ? i : i-1;

				for (int j = 0; j < this.cols; ++j)
				{
					if (j == col)
					{
						continue;
					}					
					int j_target = j < col ? j : j-1;

					result[i_target, j_target] = this.data[i,j];
				}
			}

			return result;
		}

		public Matrix clone()
		{
			return new Matrix(this);
		}

		public Matrix convertCovarianceToCorrelation()
		{
			Matrix result = new Matrix(this.rows, this.cols);
			for (int i = 0; i < this.rows; ++i)
			{
				for (int j = 0; j < this.cols; ++j)
				{
					result[i,j] = this[i,j] / Math.Sqrt(this[i,i] * this[j,j]);
				}
			}
		
			return result;
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Console.WriteLine("Matrix A");
			Matrix A = new Matrix(3, 3);
			A.setIdentity();
			A[0,1] = A[1,0] = 0.2;
			A[0,2] = A[2,0] = 0.3;
			A[1,2] = A[2,1] = 0.1;
			Console.WriteLine(A.dump());

			Console.WriteLine("Matrix B");
			Matrix B = new Matrix(3, 3);
			B.setIdentity();
			Console.WriteLine(B.dump());
			
			Console.WriteLine("Multiplying AB");
			Matrix C = A.multiply(B);
			Console.WriteLine(C.dump());

			Console.WriteLine("Interchanging rows 1,2");
			C.exchangeRows(1,2);
			Console.WriteLine(C.dump());
		
			Console.WriteLine("Interchanging cols 0,1");
			C.exchangeCols(0,1);
			Console.WriteLine(C.dump());

			Console.WriteLine("Inverting A");
			Matrix A_inverse = A.inverse();
			Console.WriteLine(A_inverse.dump());
			Console.WriteLine(A.multiply(A_inverse).dump());
		}
#endif

        #endregion
    }
}
