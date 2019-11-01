using System;

namespace Utilities.Mathematics
{
    public class Complex
    {
        public double real;
        public double complex;

        public Complex()
        {
            real = 0;
            complex = 0;
        }

        public Complex(double areal)
        {
            real = areal;
            complex = 0;
        }

        public Complex(double areal, double acomplex)
        {
            real = areal;
            complex = acomplex;
        }

        public Complex(Complex other)
        {
            real = other.real;
            complex = other.complex;
        }

        public static Complex add(Complex a, Complex b)
        {
            return new Complex(a.real + b.real, a.complex + b.complex);
        }

        public static Complex multiply(double c, Complex b)
        {
            return new Complex(c * b.real, c * b.complex);
        }

        public static Complex multiply(Complex a, Complex b)
        {
            // return new Complex(a.real*b.real - a.complex*b.complex, a.real*b.complex + b.real*a.complex);	// Slow
            double ac = a.real * b.real;
            double bd = a.complex * b.complex;
            return new Complex(ac - bd, (a.real + a.complex) * (b.real + b.complex) - ac - bd);
        }

        public static Complex divide(Complex a, Complex b)
        {
            double mod_c = b.real;
            double mod_d = b.complex;

            if (mod_c >= mod_d)
            {
                double d_over_c = b.complex / b.real;
                double denom = b.real + b.complex * d_over_c;
                return new Complex((a.real + a.complex * d_over_c) / denom, (a.complex - a.real * d_over_c) / denom);
            }

            else
            {
                double c_over_d = b.real / b.complex;
                double denom = b.real * c_over_d + b.complex;
                return new Complex((a.real * c_over_d + a.complex) / denom, (a.complex * c_over_d - a.real) / denom);
            }
        }

        public static Complex sqrt(Complex a)
        {
            Console.WriteLine("Not implemented: see numerical recipes pg 177");
            return null;
        }

        public static Complex conjugate(Complex a)
        {
            return new Complex(a.real, -a.complex);
        }


        public override String ToString()
        {
            return real + " + " + complex + "i";
        }

        public double norm()
        {
            // return Math.Sqrt(real * real + complex * complex);	// Can overflow...

            double mod_a = Math.Abs(real);
            double mod_b = Math.Abs(complex);

            if (mod_a > mod_b)
            {
                double complex_over_real = complex / real;
                return mod_a * Math.Sqrt(1 + (complex_over_real) * (complex_over_real));
            }

            else
            {
                double real_over_complex = real / complex;
                return mod_b * Math.Sqrt(1 + (real_over_complex) * (real_over_complex));
            }
        }

        public bool isComplex()
        {
            return !(Precision.closeToZero(complex));
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Complex a = new Complex(2);
			Complex b = new Complex(3,3);
			Console.WriteLine("a    = {0}", a);
			Console.WriteLine("b    = {0}", b);
			Console.WriteLine("a+a  = {0}", add(a,a));
			Console.WriteLine("a*a  = {0}", multiply(a,a));
			Console.WriteLine("b+b  = {0}", add(b,b));
			Console.WriteLine("b*b  = {0}", multiply(b,b));
			Console.WriteLine("a+b  = {0}", add(a,b));
			Console.WriteLine("a*b  = {0}", multiply(a,b));
		}
#endif

        #endregion
    }
}
