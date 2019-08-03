using System;
using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Mathematics.Statistics.Distributions
{
	public class Distributions
	{
		public static double DStandardNormal(double x)
		{
			return (1.0 / Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-0.5 * (x*x));
		}

		public static double CDistStandardNormal(double x)
		{
			return CDistStandardNormal_Hart(x);
		}			

		public static double CDistStandardNormal_Hart(double x)
		{
			// This is also known as Genz 1D
			// Accurate to double precision
		
			double cn;
	
			double xabs = Math.Abs(x);
			if (xabs > 37.0) 
			{
				cn = 0;
			}
			else
			{
				double exponential = Math.Exp(-Math.Pow(xabs,2) / 2);
				if (xabs < 7.07106781186547)
				{
					double build = 3.52624965998911E-02 * xabs + 0.700383064443688;
					build = build * xabs + 6.37396220353165;
					build = build * xabs + 33.912866078383;
					build = build * xabs + 112.079291497871;
					build = build * xabs + 221.213596169931;
					build = build * xabs + 220.206867912376;
					cn = exponential * build;
					build = 8.83883476483184E-02 * xabs + 1.75566716318264;
					build = build * xabs + 16.064177579207;
					build = build * xabs + 86.7807322029461;
					build = build * xabs + 296.564248779674;
					build = build * xabs + 637.333633378831;
					build = build * xabs + 793.826512519948;
					build = build * xabs + 440.413735824752;
					cn = cn / build;
				}
				else
				{
					double build = xabs + 0.65;
					build = xabs + 4.0 / build;
					build = xabs + 3.0 / build;
					build = xabs + 2.0 / build;
					build = xabs + 1.0 / build;
					cn = exponential / build / 2.506628274631;
				}
			}

			if (x > 0) 
			{
				cn = 1 - cn;
			}
			return cn;
		}


		public static double CDistStandardNormal_Hull(double x)
		{
			// Accurate to only 6dp

			double[] a = new double[5];
			a[0] = 0.31938153;
			a[1] = -0.356563782;
			a[2] = 1.781477937;
			a[3] = -1.821255978;
			a[4] = 1.330274429;
    
			double tau = 0.2316419;
			double k = 1.0 / (1.0 + tau * Math.Abs(x));
    
			double result = 1.0 - DStandardNormal(x) * (k * (a[0] + k * (a[1] + k * (a[2] + k * (a[3] + k * (a[4]))))));
			if (x <= 0)
			{
				result = 1.0 - result;
			}
			
			return result;
		}

		public static double InvDist_SN(double u)
		{
			return InvDist_SN_Moro(u);
		}

		public static void InvDist_SN(Vector v)
		{
			for (int i = 0; i < v.cols; ++i)
			{
				v[i] = InvDist_SN(v[i]);
			}
		}

		public static double InvDist_SN_Moro(double y)
		{
			//Moro method

			double zz;
			double z = y - 0.5;
			if (Math.Abs(z) < 0.42)
			{
				zz = Math.Pow(z,2);
				zz = z * (((-25.44106049637 * zz + 41.39119773534) * zz + -18.61500062529) * zz + 2.50662823884) / 
					((((3.13082909833 * zz + -21.06224101826) * zz + 23.08336743743) * zz + -8.4735109309) * zz + 1);
			}
			else
			{
				if (z > 0)
				{
					zz = Math.Log(-Math.Log(1 - y));
				}
				else
				{
					zz = Math.Log(-Math.Log(y));
				}
				double build = 2.888167364E-07 + zz * 3.960315187E-07;
				build = 3.21767881768E-05 + zz * build;
				build = 3.951896511919E-04 + zz * build;
				build = 3.8405729373609E-03 + zz * build;
				build = 2.76438810333863E-02 + zz * build;
				build = 0.160797971491821 + zz * build;
				build = 0.976169019091719 + zz * build;
				zz = 0.337475482272615 + zz * build;
				if (z <= 0)
				{
					zz = -zz;
				}
			}
			return zz;
		}
		
		public static double InvDist_SN_Poor(double u)
		{
			// Only 5 coefficient poly approx...don't use

			double[] p = new double[5];
            p[0] = -0.322232431088;
			p[1] = -1;
			p[2] = -0.342242088547;
			p[3] = -0.0204231210245;
			p[4] = -4.53642210148E-05;
    
			double[] q = new double[5];
			q[0] = 0.099348462606;
			q[1] = 0.588581570495;
			q[2] = 0.531103462366;
			q[3] = 0.10353775285;
			q[4] = 0.0038560700634;
    
			if (u <= 0 || u >= 1)
			{
				throw new GenericException("u must lie inside [0,1]");
			}

			if (Math.Abs(u - 0.5) < 0.0000001) 
			{
				return 0;
			}

			if (u < 0.5)
			{
				return -InvDist_SN(1 - u);
			}

			double Y = Math.Sqrt(-Math.Log((1.0 - u) * (1.0 - u)));
			double num = p[4];
			double den = q[4];

			for (int i = 3; i >= 0; --i)
			{
				num = num * Y + p[i];
				den = den * Y + q[i];
			}

			return Y + num / den;
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Console.WriteLine("i\tN(i)");
			for (double i = -2; i <=2; i += 0.01)
			{
				Console.WriteLine("{0}\t{1}", i,CDistStandardNormal(i));
			}

			Console.WriteLine("Moro inv of 0.8 is {0} and should be 0.841621234897994", InvDist_SN(0.8));
		}
#endif

        #endregion
    }
}
