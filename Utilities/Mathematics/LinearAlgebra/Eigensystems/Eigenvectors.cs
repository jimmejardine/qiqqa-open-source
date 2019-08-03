using System;
using Utilities.Random;

namespace Utilities.Mathematics.LinearAlgebra.Eigensystems
{
	public class Eigenvectors
	{

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			RandomAugmented ra = RandomAugmented.getSeededRandomAugmented();

			int N = 4;

			// Create a matrix
			Matrix m = new Matrix(N,N);
			for (int i = 0; i < N; ++i)
			{
				for (int j = i; j < N; ++j)
				{
					m[i,j] = ra.NextDouble(10);
					m[j,i] = m[i,j];
				}
			}

			// Space for results
			Matrix eigenvectors = new Matrix(N,N);
			Vector eigenvalues = new Vector(N);

			// Get values
			calculateEigensystem(m, eigenvectors, eigenvalues);
			sortEigensystem(eigenvectors, eigenvalues);

			Console.WriteLine("Eigenvalues");
			Console.WriteLine(eigenvalues);
			Console.WriteLine("Eigenvectors");
			Console.WriteLine(eigenvectors);

			// Test each eigenpair
			Vector eigenvector = new Vector(N);
			for (int i = 0; i < N; ++i)
			{
				eigenvectors.getColumn(i, eigenvector);
				
				Console.WriteLine("Eigenpair {0}", i);
				Console.WriteLine(m.multiply(eigenvector));
				Console.WriteLine(eigenvector.multiply(eigenvalues[i]));
			}
		}
#endif

        #endregion
        public static void sortEigensystem(Matrix eigenvectors, Vector eigenvalues)
		{
			int N = eigenvectors.rows;

			for (int i = 0; i < N; ++i)
			{
				for (int j = i+1; j < N; ++j)
				{
					if (Math.Abs(eigenvalues[i]) < Math.Abs(eigenvalues[j]))
					{
						eigenvalues.exchangeCols(i, j);
						eigenvectors.exchangeCols(i, j);
					}
				}
			}
		}

		public static void calculateEigensystem(Matrix input, Matrix eigenvectors, Vector eigenvalues)
		{
			// Check that our matrix is symmetric.  We only implement that code...
			if (!input.isSymmetric())
			{
				throw new GenericException("Can only find the eigensystem of a symmetric matrix");
			}


			// Check that we have space in which to work
			if (!input.isSquare() || !eigenvectors.isSquare() || input.rows != eigenvectors.rows || input.rows != eigenvalues.cols)
			{
				throw new GenericException("Invalid parameters: there is a dimension mismatch");
			}

			// Define our vars
			int N = input.rows;
			double[,] a = new double[N,N];
			double[] d = new double[N];
			double[] e = new double[N];

			// Fill the input matrix
			for (int i = 0; i < N; ++i)
			{
				for (int j = 0; j < N; ++j)
				{
					a[i,j] = input[i,j];
				}
			}

			// First reduce to householder form and then calculate the eigens
			tred2(a, N, d, e);
			tqli(d, e, N, a);

			// Copy the rash numerical recipes data into our nice data structures
			for (int i = 0; i < N; ++i)
			{
				eigenvalues[i] = d[i];
			}

			for (int i = 0; i < N; ++i)
			{
				for (int j = 0; j < N; ++j)
				{
					eigenvectors[i,j] = a[i,j];
				}
			}
		}


		/**
		 * Computes (a2 + b2)^{1/2} without destructive underflow or overflow.
		 */

		static double pythag(double a, double b)
		{
			double absa = Math.Abs(a);
			double absb = Math.Abs(b);
			if (absa > absb) return absa*Math.Sqrt(1.0+(absb/absa)*(absb/absa));
			else return (absb == 0.0 ? 0.0 : absb*Math.Sqrt(1.0+(absa/absb)*(absa/absb)));
		} 

		/**
		 *  Householder reduction of a real, symmetric matrix a[0..n-1,0..n-1]. On output, a is replaced
		 *  by the orthogonal matrix Q effecting the transformation. d[0..n-1] returns the diagonal elements
		 *  of the tridiagonal matrix, and e[0..n-1] the off-diagonal elements, with e[0]=0. Several
		 *  statements, as noted in comments, can be omitted if only eigenvalues are to be found, in which
		 *  case a contains no useful information on output. Otherwise they are to be included.
		 */

		static void tred2(double[,] a, int n, double[] d, double[] e)
		{
			int l,k,j,i;
			double scale,hh,h,g,f;

			for (i=n-1;i>=1;i--) 
			{
				l=i-1;
				h=scale=0.0;
				if (l > 0) 
				{
					for (k=0;k<=l;k++)
						scale += Math.Abs(a[i,k]);
					if (scale == 0.0)
						e[i]=a[i,l];
					else 
					{
						for (k=0;k<=l;k++) 
						{
							a[i,k] /= scale;
							h += a[i,k]*a[i,k];
						}
						f=a[i,l];
						g=(f >= 0.0 ? -Math.Sqrt(h) : Math.Sqrt(h));
						e[i]=scale*g;
						h -= f*g;
						a[i,l]=f-g;
						f=0.0;
						for (j=0;j<=l;j++) 
						{
							a[j,i]=a[i,j]/h;
							g=0.0;
							for (k=0;k<=j;k++)
								g += a[j,k]*a[i,k];
							for (k=j+1;k<=l;k++)
								g += a[k,j]*a[i,k];
							e[j]=g/h;
							f += e[j]*a[i,j];
						}
						hh=f/(h+h);
						for (j=0;j<=l;j++) 
						{
							f=a[i,j];
							e[j]=g=e[j]-hh*f;
							for (k=0;k<=j;k++)
								a[j,k] -= (f*e[k]+g*a[i,k]);
						}
					}
				} 
				else
					e[i]=a[i,l];
				d[i]=h;
			}
			d[0]=0.0;
			e[0]=0.0;
			for (i=0;i<n;i++) 
			{
				l=i-1;
				if (d[i]!=0.0) 
				{
					for (j=0;j<=l;j++) 
					{
						g=0.0;
						for (k=0;k<=l;k++)
							g += a[i,k]*a[k,j];
						for (k=0;k<=l;k++)
							a[k,j] -= g*a[k,i];
					}
				}
				d[i]=a[i,i];
				a[i,i]=1.0;
				for (j=0;j<=l;j++) a[j,i]=a[i,j]=0.0;
			}
		}  

		/**
		 * 	
		 *  QL algorithm with implicit shifts, to determine the eigenvalues and eigenvectors of a real, 
		 *  symmetric, tridiagonal matrix, or of a real, symmetric matrix previously reduced by tred2 ¡×11.2.
		 *  On input, d[0..n-1] contains the diagonal elements of the tridiagonal matrix. On output, it returns
		 *  the eigenvalues. The vector e[0..n-1] inputs the subdiagonal elements of the tridiagonal matrix,
		 *  with e[0] arbitrary. On output e is destroyed. When finding only the eigenvalues, several lines
		 *  may be omitted, as noted in the comments. If the eigenvectors of a tridiagonal matrix are desired,
		 *  the matrix z[0..n-1,0..n-1] is input as the identity matrix. If the eigenvectors of a matrix
		 *  that has been reduced by tred2 are required, then z is input as the matrix output by tred2.
		 *  In either case, the kth column of z returns the normalized eigenvector corresponding to d[k].
		 */

		static void tqli(double[] d, double[] e, int n, double[,] z)
		{
			int m,l,iter,i,k;
			double s,r,p,g,f,dd,c,b;

			for (i=1;i<n;i++) e[i-1]=e[i];
			e[n-1]=0.0;
			for (l=0;l<n;l++) 
			{
				iter=0;
				do 
				{
					for (m=l;m<n-1;m++) 
					{
						dd=Math.Abs(d[m])+Math.Abs(d[m+1]);
						if ((double)(Math.Abs(e[m])+dd) == dd) break;
					}
					if (m != l) 
					{
						if (iter++ == 30)
						{
							throw new GenericException("Too many iterations in tqli");
						}

						g=(d[l+1]-d[l])/(2.0*e[l]);
						r=pythag(g,1.0);
						g=d[m]-d[l]+e[l]/(g+Math.Sign(g)*r);
						s=c=1.0;
						p=0.0;
						for (i=m-1;i>=l;i--) 
						{
							f=s*e[i];
							b=c*e[i];
							e[i+1]=(r=pythag(f,g));
							if (r == 0.0) 
							{
								d[i+1] -= p;
								e[m]=0.0;
								break;
							}
							s=f/r;
							c=g/r;
							g=d[i+1]-p;
							r=(d[i]-g)*s+2.0*c*b;
							d[i+1]=g+(p=s*r);
							g=c*r-b;
							for (k=0;k<n;k++) 
							{
								f=z[k,i+1];
								z[k,i+1]=s*z[k,i]+c*f;
								z[k,i]=c*z[k,i]-s*f;
							}
						}
						if (r == 0.0 && i >= l) continue;
						d[l] -= p;
						e[l]=g;
						e[m]=0.0;
					}
				} while (m != l);
			}
		}
	}
}
