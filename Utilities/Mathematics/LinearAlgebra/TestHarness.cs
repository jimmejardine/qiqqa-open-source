namespace Utilities.Mathematics.LinearAlgebra
{
    public class TestHarness
    {
        public static void runTest()
        {
            Matrix A = new Matrix(3, 3);
            A.setIdentity();
            A.data[0, 1] = A.data[1, 0] = 0.2;
            A.data[0, 2] = A.data[2, 0] = 0.3;
            A.data[1, 2] = A.data[2, 1] = 0.1;
            A.dump();

            Matrix B = new Matrix(3, 3);
            B.setIdentity();
            B.dump();

            Matrix C = A.multiply(B);
            C.dump();

            //			LUDecomposition lu = new LUDecomposition(C);
            //			System.Console.WriteLine("LU decomposition");
            //			lu.dump();
            //
            //			Matrix b = new Matrix(3, 1);
            //			b.data[0,0] = 1;
            //			b.data[1,0] = 2;
            //			b.data[2,0] = 3;
            //			System.Console.WriteLine("b");
            //			b.dump();
            //
            //			Matrix x = lu.solve(b);
            //			System.Console.WriteLine("x");
            //			x.dump();
            //
            //			Matrix bprime = C.multiply(x);
            //			System.Console.WriteLine("bprime");
            //			bprime.dump();
            //
            //
            //			System.Console.WriteLine("LU squared");
            //			Matrix D = lu.decomposition.multiply(lu.decomposition.transpose());
            //			D.dump();
        }
    }
}
