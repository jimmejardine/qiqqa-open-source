using System;
using Utilities.GUI.Charting;

namespace Utilities.Mathematics.Statistics.Distributions
{
	public class DreznerBivariate
	{
		static double[] weights = 
			{
				5.54433663102343E-02,
				1.24027738987730E-01,
				1.75290943892075E-01,
				1.91488340747342E-01,
				1.63473797144070E-01,
				1.059376372784920-01,
				5.002702115345350-02,
				1.64429690052673E-02,
				3.57320421428311E-03,
				4.82896509305201E-04,
				3.74908650266318E-05,
				1.49368411589636E-06,
				2.55270496934465E-08,
				1.34217679136316E-10,
				9.56227446736465E-14
			};

		static double[] abscissae = 
			{
				2.16869474675590E-02,
				1.12684220347775E-01,
				2.70492671421899E-01,
				4.86902370381935E-01,
				7.53043683072978E-01,
				1.06093100362236E+00,
				1.40425495820363E+00,
				1.77864637941183E+00,
				2.18170813144494E+00,
				2.61306084533352E+00,
				3.07461811380851E+00,
				3.57140815113714E+00,
				4.11373608977209E+00,
				4.72351306243148E+00,
				5.46048893578335E+00
			};

		public static double CBivariateStandardNormal(double h, double k, double rho)
		{
			// Check the limiting cases of perfect correlation and negative correlation
			if (rho > 0.99999)
			{
				return Distributions.CDistStandardNormal(Math.Min(h, k));
			}
			else if (rho < -0.99999)
			{
				if (k <= -h)
				{
					return 0.0;
				}
				else
				{
					return Distributions.CDistStandardNormal(h) + Distributions.CDistStandardNormal(k) - 1;
				}
			}

			if (h <= 0 && k <= 0 && rho <= 0)
			{
				return CBivariateStandardNormal_UseForNegatives(h, k, rho);
			}
			else if (h <= 0 && k >= 0 && rho >= 0)
			{
				return Distributions.CDistStandardNormal(h) - CBivariateStandardNormal_UseForNegatives(h, -k, -rho);
			}
			else if (h >= 0 && k <= 0 && rho >= 0)
			{
				return Distributions.CDistStandardNormal(k) - CBivariateStandardNormal_UseForNegatives(-h, k, -rho);
			}
			else if (h >= 0 && k >= 0 && rho <= 0)
			{
				return Distributions.CDistStandardNormal(h) + Distributions.CDistStandardNormal(k) - 1 + CBivariateStandardNormal_UseForNegatives(-h, -k, rho);
			}
			else
			{
				double denom = Math.Sqrt(h*h - 2 * rho * h * k + k*k);
				double rho_hk = (rho * h - k) * Math.Sign(h) / denom;
				double rho_kh = (rho * k - h) * Math.Sign(k) / denom;
				double delta = (1.0 - Math.Sign(h) * Math.Sign(k)) / 4.0;
				return CBivariateStandardNormal(h, 0, rho_hk) + CBivariateStandardNormal(k, 0, rho_kh) - delta;
			}
		}
		
		
		static double CBivariateStandardNormal_UseForNegatives(double h, double k, double rho)
		{
			// Work out some efficiencies
			double root_one_minus_rho_squared = Math.Sqrt(1 - rho*rho);
			double denom = Constants.ROOT_2 * root_one_minus_rho_squared;
			double h1 = h / denom;
			double k1 = k / denom;

			// Determine the gauss quadrature approximation
			double sum = 0.0;
			for (int i = 0; i < 15; ++i)
			{
				for (int j = 0; j < 15; ++j)
				{
					double x = abscissae[i];
					double y = abscissae[j];
					
					double f_of_xy = Math.Exp(h1*(2*x-h1) + k1*(2*y-k1) + 2*rho*(x-h1)*(y-k1));
					sum += weights[i]*weights[j]*f_of_xy;
				}
			}

			// Build the result
			return root_one_minus_rho_squared / Math.PI * sum;
		}

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			Console.WriteLine("Zero is {0}", CBivariateStandardNormal(-1000, -1000, 0));

			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(0), CBivariateStandardNormal(+1000, 0, 0));
			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(0.5), CBivariateStandardNormal(+1000, 0.5, 0));
			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(-0.5), CBivariateStandardNormal(+1000, -0.5, 0));

			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(-0.5), CBivariateStandardNormal(-0.5, -0.5, 1.0));
			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(-0.5), CBivariateStandardNormal(-0.5, -0.2, 1.0));
			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(-0.2), CBivariateStandardNormal(-0.1, -0.2, 1.0));

			Console.WriteLine("{0} is {1}", 0, CBivariateStandardNormal(-0.5, -0.5, -1.0));
			Console.WriteLine("{0} is {1}", 0, CBivariateStandardNormal(-0.5, -0.2, -1.0));
			Console.WriteLine("{0} is {1}", 0, CBivariateStandardNormal(-0.1, -0.2, -1.0));
			Console.WriteLine("{0} is {1}", Distributions.CDistStandardNormal(0.1)+Distributions.CDistStandardNormal(0.5)-1, CBivariateStandardNormal(0.1, 0.5, -1.0));

			Console.WriteLine("Pain is {0}", CBivariateStandardNormal(3.8922701863527047, -0.28284271247461906, -0.70710678118654757));

			TestHarness_GUI();
		}

		public static void TestHarness_GUI()
		{
			int GRIDSIZE = 100;
			double[,] values = new double[GRIDSIZE,GRIDSIZE];
			double BOUND = 1.0;
			
			double rho = 0.0;

			for (int i = 0; i < GRIDSIZE; ++i)
			{
				for (int j = 0; j < GRIDSIZE; ++j)
				{
					double x = (i - GRIDSIZE / 2.0) * BOUND;
					double y = (j - GRIDSIZE / 2.0) * BOUND;
					
					double ans = CBivariateStandardNormal(x, y, rho);
					values[i,j] = ans;
				}
			}

			TopographicalChart chart = new TopographicalChart();
			chart.setDataset(values, "x", -BOUND, BOUND, "y", -BOUND, BOUND);
			chart.showFormModal();
		}
#endif

        #endregion
    }
}
