using System;
using System.Drawing;
using Utilities.GUI.Charting;
using Utilities.Mathematics.LinearAlgebra;
using Utilities.Random;

namespace Utilities.Mathematics.Statistics.Distributions
{
	public class GenzMultivariate
	{
		RandomAugmented random;

		Matrix correlation;
		Matrix correlation_root;

		public GenzMultivariate(Matrix acorrelation)
		{
			random = RandomAugmented.getSeededRandomAugmented();
			setCorrelation(acorrelation);			
		}

		public void setCorrelation(Matrix acorrelation)
		{
			// Store the correlation matrix
			correlation = acorrelation;
			correlation_root = Cholesky.factorise(correlation);
		}

		public static double CNorm(Vector x, Matrix correlation)
		{
			Vector minus_infinity = new Vector(x.cols);
			for (int i = 0; i < x.cols; ++i)
			{
				minus_infinity[i] = -1E10;
			}

			GenzMultivariate gm = new GenzMultivariate(correlation);
			return gm.getProb(minus_infinity, x, 2.5, 0.001, 10000);
		}

		public static double CNorm_2D(double x, double y, double rho)
		{
			Vector d = new Vector(2);
			d[0] = x;
			d[1] = y;
			Matrix rho_matrix = new Matrix(2, 2);
			rho_matrix[0,0] = 1;
			rho_matrix[0,1] = rho_matrix[1,0] = rho;
			rho_matrix[1,1] = 1;
			return CNorm(d, rho_matrix);
		}


		
		public double getProb(Vector a, Vector b, double alpha, double epsilon, int N_max)
		{
			// A neat reference to match the document
			int m = correlation.rows;
			Matrix c = correlation_root;

			// Check that we are not in univariate land....
			if (1 == m)
			{
				return Distributions.CDistStandardNormal(b[0]) - Distributions.CDistStandardNormal(a[0]);
			}

			// Our variables
			double[] d = new double[m];
			double[] e = new double[m];
			double[] f = new double[m];

			double[] y = new double[m];
			double[] w = new double[m];

			// Initialisation code
			double intsum = 0.0;
			int N = 0;
			double varsum = 0.0;

			d[0] = Distributions.CDistStandardNormal(a[0]/c[0,0]);
			e[0] = Distributions.CDistStandardNormal(b[0]/c[0,0]);
			f[0] = e[0] - d[0];

			double error = 0.0;

			do
			{
				for (int i = 0; i < m-1; ++i)
				{
					w[i] = random.NextDouble();
				}

				for (int i = 1; i < m; ++i)
				{
					y[i-1] = Distributions.InvDist_SN(d[i-1] + w[i-1] * (e[i-1] - d[i-1]));
					
					double sum_cij_yj = 0.0;
					for (int j = 0; j <= i-1; ++j)
					{
						sum_cij_yj += c[i,j]*y[j];
					}
					d[i] = Distributions.CDistStandardNormal((a[i]-sum_cij_yj) / c[i,i]);
					e[i] = Distributions.CDistStandardNormal((b[i]-sum_cij_yj) / c[i,i]);
					f[i] = (e[i] - d[i]) * f[i-1];

					++N;
					double delta = (f[m-1] - intsum) / N;
					intsum += delta;
					varsum = (N-2)*varsum/N + delta*delta;
					error = alpha * Math.Sqrt(varsum);
				}
			} while (error > epsilon && N < N_max);

			if (N >= N_max)
			{
				//System.Console.WriteLine("Exceeded");
				//throw new GenericException("Reached maximum number of iterations");
			}

			return intsum;
		}

		public static void TestHarness()
		{
			TestHarness2D();
			TestHarness3D();
			TestHarness_Irregularity();
			TestHarness_ComparisonWithDrezner();
			
		}

		public static void TestHarness_Irregularity()
		{
			Series series1 = new Series("Genz", ChartType.Line);
			Series series2 = new Series("Genz", ChartType.Line);
			Series series3 = new Series("Genz", ChartType.Line);
			Series series4 = new Series("Genz", ChartType.Line);

			for (int i = 0; i <= 300; ++i)
			{
				series1.addPoint(i,CNorm_2D(0, 0, 0.0));
				series2.addPoint(i,CNorm_2D(0, 0, 0.5));
				series3.addPoint(i,CNorm_2D(2, 1, 0.2));
				series4.addPoint(i,CNorm_2D(-2, 0, -0.7));
			}


			GenericChartForm charts = new GenericChartForm();
			charts.BackColor = Color.White;
			charts.Text = "Plots of Genz instabilities";
			charts.setChartCounts(2,2);

			charts[0,0].legend_height = 0;
			charts[0,0].title = "x=0, y=0, rho=0";
			charts[0,0].addSeries(series1);
			
			charts[1,0].legend_height = 0;
			charts[1,0].title = "x=0, y=0, rho=0.5";
			charts[1,0].addSeries(series2);

			charts[0,1].legend_height = 0;
			charts[0,1].title = "x=2, y=1, rho=0.2";
			charts[0,1].addSeries(series3);

			charts[1,1].legend_height = 0;
			charts[1,1].title = "x=-2, y=0, rho=-0.7";
			charts[1,1].addSeries(series4);

			charts.ShowDialog();


		}

		public static void TestHarness_ComparisonWithDrezner_Subtest(double y, double correlation, MultiChart2D chart)
		{
			Series series_genz = new Series("Genz", ChartType.Line);
			Series series_drezner = new Series("Drezner", ChartType.Line);

			for (double i = -10.0; i <= 10.0; i += 0.2)
			{
				series_drezner.addPoint(i,CNorm_2D(i, y, correlation));
				series_genz.addPoint(i, CNorm_2D(i, y, correlation));
			}

			chart.title = String.Format("y={0} correlation={1}", y, correlation);
			chart.addSeries(series_drezner);
			chart.addSeries(series_genz);
		}
		
		public static void TestHarness_ComparisonWithDrezner()
		{
			GenericChartForm charts = new GenericChartForm();
			charts.BackColor = Color.White;
			charts.Text = "Plots of Genz vs. Drezner";
			charts.setChartCounts(5,3);

			for (int i = 0; i < 15; ++i)
			{
				charts[i].axis_height = 20;
				charts[i].legend_height = 30;
			}

			TestHarness_ComparisonWithDrezner_Subtest(-2,    0, charts[0,0]);
			TestHarness_ComparisonWithDrezner_Subtest(-1,    0, charts[1,0]);
			TestHarness_ComparisonWithDrezner_Subtest( 0,    0, charts[2,0]);
			TestHarness_ComparisonWithDrezner_Subtest(+1,    0, charts[3,0]);
			TestHarness_ComparisonWithDrezner_Subtest(+2,    0, charts[4,0]);

			TestHarness_ComparisonWithDrezner_Subtest(-2, +0.5, charts[0,1]);
			TestHarness_ComparisonWithDrezner_Subtest(-1, +0.5, charts[1,1]);
			TestHarness_ComparisonWithDrezner_Subtest( 0, +0.5, charts[2,1]);
			TestHarness_ComparisonWithDrezner_Subtest(+1, +0.5, charts[3,1]);
			TestHarness_ComparisonWithDrezner_Subtest(+2, +0.5, charts[4,1]);

			TestHarness_ComparisonWithDrezner_Subtest(-2, -0.5, charts[0,2]);
			TestHarness_ComparisonWithDrezner_Subtest(-1, -0.5, charts[1,2]);
			TestHarness_ComparisonWithDrezner_Subtest( 0, -0.5, charts[2,2]);
			TestHarness_ComparisonWithDrezner_Subtest(+1, -0.5, charts[3,2]);
			TestHarness_ComparisonWithDrezner_Subtest(+2, -0.5, charts[4,2]);

			charts.ShowDialog();
		}

		public static void TestHarness2D()
		{
			Matrix correlation = new Matrix(2,2);
			correlation.setIdentity();
			correlation[0,1] = correlation[1,0] = 0.2; 
			Vector a = new Vector(2);
			a[0] = -1E10;
			a[1] = -1E10;
			Vector b = new Vector(2);
			b[0] = 0.1;
			b[1] = 0.1;
			GenzMultivariate genzmultivariate = new GenzMultivariate(correlation);
			Console.WriteLine("Prob is {0} and should be 0.323170477981", genzmultivariate.getProb(a, b, 2.5, 1E-5, 100000));
		}

		public static void TestHarness3D()
		{
		
			Matrix correlation = new Matrix(3,3);
			correlation.setIdentity();
			correlation[0,1] = correlation[1,0] = 3.0/5.0; 
			correlation[0,2] = correlation[2,0] = 1.0/3.0; 
			correlation[1,2] = correlation[2,1] = 11.0/15.0; 
			Vector a = new Vector(3);
			a[0] = -1E10;
			a[1] = -1E10;
			a[2] = -1E10;
			Vector b = new Vector(3);
			b[0] = 1.0;
			b[1] = 4.0;
			b[2] = 2.0;
			GenzMultivariate genzmultivariate = new GenzMultivariate(correlation);
			Console.WriteLine("Prob is {0} and should be 0.82798", genzmultivariate.getProb(a, b, 2.5, 1E-5, 100000));
		}
	}
}
