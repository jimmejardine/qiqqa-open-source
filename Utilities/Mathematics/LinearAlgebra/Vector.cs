using System;
using System.Text;

namespace Utilities.Mathematics.LinearAlgebra
{
    public class Vector
    {
        public bool is_column_vector;
        public int cols;
        public double[] data;

        public Vector(int acols)
        {
            is_column_vector = true;
            cols = acols;
            data = new double[cols];
        }

        public Vector(int acols, double[] adata)
        {
            is_column_vector = true;
            cols = acols;
            data = adata;
        }

        public double this[int i]
        {
            get => data[i];

            set => data[i] = value;
        }

        public double asScalar()
        {
            if (1 != cols)
            {
                throw new GenericException("Can convert only a 1 vector to a scalar.");
            }

            return data[0];
        }

        public Vector transpose()
        {
            Vector result = new Vector(cols);
            result.is_column_vector = !is_column_vector;
            for (int j = 0; j < cols; ++j)
            {
                result[j] = data[j];
            }
            return result;
        }

        public Vector add(Vector B)
        {
            // A symbolic name for our own matrix
            Vector A = this;

            // Check that the matrices are compatible
            if (A.cols != B.cols || A.is_column_vector != B.is_column_vector)
            {
                throw new GenericException("Cannot add vectors with different dimensions");
            }

            Vector result = new Vector(A.cols);
            result.is_column_vector = A.is_column_vector;
            for (int j = 0; j < A.cols; ++j)
            {
                result[j] = A[j] + B[j];
            }

            return result;
        }

        public Vector multiply(double b)
        {
            Vector result = new Vector(cols);
            result.is_column_vector = is_column_vector;
            for (int j = 0; j < cols; ++j)
            {
                result[j] = b * data[j];
            }

            return result;
        }

        private void setValue(double a)
        {
            for (int j = 0; j < cols; ++j)
            {
                data[j] = a;
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

        public void exchangeCols(int i, int j)
        {
            Swap.swap(ref data[i], ref data[j]);
        }


        public Matrix toMatrix()
        {
            Matrix B;

            if (is_column_vector)
            {
                B = new Matrix(cols, 1);
                for (int i = 0; i < cols; ++i)
                {
                    B[i, 0] = this[i];
                }
            }
            else
            {
                B = new Matrix(1, cols);
                for (int i = 0; i < cols; ++i)
                {
                    B[0, i] = this[i];
                }
            }

            return B;
        }

        private double[] cloneData()
        {
            double[] result = new double[cols];
            for (int j = 0; j < cols; ++j)
            {
                result[j] = data[j];
            }

            return result;
        }

        public String dump()
        {
            return ToString();
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int j = 0; j < cols; ++j)
            {
                sb.AppendFormat("{0,8}", data[j]);
                if (is_column_vector)
                {
                    sb.Append("\n");
                }
                else
                {
                    sb.Append("\t");
                }
            }

            if (!is_column_vector)
            {
                sb.AppendFormat("\n");
            }

            return sb.ToString();
        }

        public static double norm(double x, double y, double z)
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Vector A = new Vector(3);
			A.setOnes();
			A.dump();

			Vector B = new Vector(3);
			B.setOnes();
			B[1] = 4;
			B.dump();
			
			Vector C = A.add(B);
			C.dump();
		}
#endif

        #endregion
    }
}
