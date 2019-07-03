using System;

namespace Utilities.Mathematics
{
	public class Polynomials
	{
		public static void findQuadraticRoot(double a, double b, double c, out Complex o1, out Complex o2)
		{
			o1 = new Complex();
			o2 = new Complex();

			double descriminant = b*b - 4*a*c;
			if (descriminant >= 0)
			{
				double descriminant_root = Math.Sqrt(descriminant);
				o1 = new Complex((-b + descriminant_root) / (2*a), 0.0);
				o2 = new Complex((-b - descriminant_root) / (2*a), 0.0);
			}
			else
			{
				double descriminant_root = Math.Sqrt(-descriminant);
				o1 = new Complex(-b / (2*a), +descriminant_root / (2*a));
				o2 = new Complex(-b / (2*a), -descriminant_root / (2*a));
			}
		}

		public static Complex evalQuadratic(double a2, double a1, double a0, Complex x)
		{
			Complex result = new Complex(a0);
			
			Complex factor = new Complex(x);
			result = Complex.add(result, Complex.multiply(a1, factor));
			factor = Complex.multiply(factor, x);
			result = Complex.add(result, Complex.multiply(a2, factor));

			return result;
		}

		public static Complex evalCubic(double a3, double a2, double a1, double a0, Complex x)
		{
			Complex result = new Complex(a0);
			
			Complex factor = new Complex(x);
			result = Complex.add(result, Complex.multiply(a1, factor));
			factor = Complex.multiply(factor, x);
			result = Complex.add(result, Complex.multiply(a2, factor));
			factor = Complex.multiply(factor, x);
			result = Complex.add(result, Complex.multiply(a3, factor));

			return result;
		}
		
		/**
		 * The findCubicRoot() method uses the method of Tartaglia - as shown in the Mathematica online help.
		 * The first result is the real root.  The other two are generally complex...
		 */
		
		public static void findCubicRoot(double a3, double a2, double a1, double a0, out Complex o1, out Complex o2, out Complex o3)
		{
			// Put into canonical form (coefficient of x^3 is 1)
			if (1 != a3)
			{
				a2 /= a3;
				a1 /= a3;
				a0 /= a3;
				a3 = 1.0;
			}

			double Q = (3*a1 - a2*a2) / 9.0;
			double R = (9*a1*a2 - 27*a0 - 2*a2*a2*a2) / 54.0;

			double R_squared = R*R;
			double Q_cubed = Q*Q*Q;

			double D = Q_cubed + R_squared;
			if (D > 0)
			{
				double S = General.cubeRoot(R + Math.Sqrt(D));
				double T = General.cubeRoot(R - Math.Sqrt(D));

				double B = (S+T) / 2.0;
				double A = (S-T) / 2.0;

				o1 = new Complex(-a2/3.0 + 2*B, 0);
				o2 = new Complex(-a2/3.0 - B, +Math.Sqrt(3)*A);
				o3 = new Complex(-a2/3.0 - B, -Math.Sqrt(3)*A);
			}

			else
			{
				double theta = Math.Acos(R / Math.Sqrt(-Q_cubed));
				
				double sqrt_minus_Q = Math.Sqrt(-Q);
				double third_a2 = a2 / 3.0;
				o1 = new Complex(2 * sqrt_minus_Q * Math.Cos(theta/3.0) - third_a2);
				o2 = new Complex(2 * sqrt_minus_Q * Math.Cos((theta+2.0*Math.PI)/3.0) - third_a2);
				o3 = new Complex(2 * sqrt_minus_Q * Math.Cos((theta+4.0*Math.PI)/3.0) - third_a2);
			}
		}

		public static void TestHarness()
		{
			Console.WriteLine("Testing quadratics");
			testQuadraticFactor(1,0,9);
			testQuadraticFactor(1,0,-9);
			testQuadraticFactor(2,0,-9);
			testQuadraticFactor(2,2,-9);
			testQuadraticFactor(2,2,9);

			Console.WriteLine("Testing cubics");
			testCubicFactor(1,0,0,-27);
			testCubicFactor(1,0,0,27);
			testCubicFactor(1,2,2,9);

			const double STEP_SIZE = 0.2;
			for (double w = -1.1; w <= 1.1; w += STEP_SIZE)
			{
				for (double x = -1.1; x <= 1.1; x += STEP_SIZE)
				{
					for (double y = -1.1; y <= 1.1; y += STEP_SIZE)
					{
						for (double z = -1.1; z <= 1.1; z += STEP_SIZE)
						{
							testCubicFactor(w,x,y,z);
						}
					}
				}
			}


		}

		static void testQuadraticFactor(double x2, double x1, double x0)
		{
			Complex a, b;
			Console.WriteLine("Factorizing {0}x^2 + {1}x^1 + {2}", x2, x1, x0);
			findQuadraticRoot(x2, x1, x0, out a, out b);
			Console.WriteLine("a; b = {0}; {1}", a, b);
			Console.WriteLine("a    = {0}", evalQuadratic(x2,x1,x0, a));
			Console.WriteLine("b    = {0}", evalQuadratic(x2,x1,x0, b));
		}

		static void testCubicFactor(double x3, double x2, double x1, double x0)
		{
			Complex a, b, c;
			findCubicRoot(x3,x2,x1,x0, out a, out b, out c);
			double a_cubed = evalCubic(x3,x2,x1,x0, a).norm();
			double b_cubed = evalCubic(x3,x2,x1,x0, b).norm();
			double c_cubed = evalCubic(x3,x2,x1,x0, c).norm();
			double TOLERANCE = 1E-5;
			if (Math.Abs(a_cubed) > TOLERANCE || Math.Abs(b_cubed) > TOLERANCE || Math.Abs(c_cubed) > TOLERANCE)
			{
				Console.WriteLine("TOLERANCE FAILED!!!  While factorizing {0}x^3 + {1}x^2 + {2}x + {3}", x3, x2, x1, x0);
				Console.WriteLine("a; b; c = {0}; {1}; {2}", a, b,c);
			}
		}

	}
}
