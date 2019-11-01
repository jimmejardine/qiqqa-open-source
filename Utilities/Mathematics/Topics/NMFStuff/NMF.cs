using System.Text;

namespace Utilities.Mathematics.Topics.NMFStuff
{
    public class NMF
    {
        #region --- Test ------------------------------------------------------------------------

#if TEST

        private static readonly float SMALL_NUMBER = (float)1E-9;

        public static void Test_Simple()
        {
            int ITERATIONS = 50;            
            int M = 10;
            int N = 50;
            int K = 50;
            double SPARSITY = 0.5;

            float[,] tW = new float[M, K];
            float[,] tH = new float[K, N];

            for (int m = 0; m < M; ++m)
            {
                for (int k = 0; k < K; ++k)
                {
                    tW[m, k] = (float)RandomAugmented.Instance.NextDouble();
                }
            }

            for (int n = 0; n < N; ++n)
            {
                for (int k = 0; k < K; ++k)
                {
                    tH[k,n] = (float)RandomAugmented.Instance.NextDouble();
                }
            }

            // Then create their product
            Matrix mW = new Matrix(tW);
            Matrix mH = new Matrix(tH);
            Matrix mV = mW.multiply(mH);

            for (int m = 0; m < M; ++m)
            {
                for (int n = 0; n < N; ++n)
                {
                    if (RandomAugmented.Instance.NextDouble() < SPARSITY)
                    {
                        mV[m, n] = SMALL_NUMBER;
                    }
                }
            }

            float[,] V = mV.ToFloatMatrix();

            // Now factorise
            float[,] W = new float[M, K];
            float[,] H = new float[K, N];
            //NMFMultiplicativeEuclidean.Factorise(K, M, N, V, W, H, ITERATIONS);
            NMFMultiplicativeDivergence.Factorise(K, M, N, V, W, H, ITERATIONS);

            // Then test the resulting product
            Matrix mWW = new Matrix(W);
            Matrix mHH = new Matrix(H);
            Matrix mVV = mWW.multiply(mHH);

            // Now compare...            
            Logging.Info("Before:\n" + PrintMatrix(mV.ToFloatMatrix()));
            Logging.Info("After:\n" + PrintMatrix(mVV.ToFloatMatrix()));
        }
#endif

        #endregion

        private static string PrintMatrix(float[,] matrix)
        {
            int M = matrix.GetLength(0);
            int N = matrix.GetLength(1);

            StringBuilder sb = new StringBuilder();
            for (int m = 0; m < M; ++m)
            {
                for (int n = 0; n < N; ++n)
                {
                    sb.AppendFormat("{0:N2}\t", matrix[m, n]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            //Test_Simple();
            Test_ACL();
        }

        public static void Test_ACL()
        {
            ACLPaperAttributeWordCounts word_counts = ACLPaperAttributeWordCountsGenerator.WordCounts;
            int M = word_counts.AttributeCount;
            int N = word_counts.PaperCount;

            // Create the initial matrix of word counts
            float[,] V = new float[M, N];
            for (int n = 0; n < N; ++n)
            {
                for (int i = 0; i < word_counts.attributes_in_paper[n].Length; ++i)
                {
                    int m = word_counts.attributes_in_paper[n][i];
                    ++V[m, n];
                }
            }

            // Check for zero rows or columns
            Logging.Info("+Checking for empty rows and columns");
            int[] m_count = new int[M];
            int[] n_count = new int[N];

            for (int m = 0; m < M; ++m)
            {
                for (int n = 0; n < N; ++n)
                {
                    if (V[m, n] > 0)
                    {
                        ++m_count[m];
                        ++n_count[n];
                    }
                }
            }

            for (int m = 0; m < M; ++m)
            {
                if (0 == m_count[m])
                {
                    Logging.Info("Row {0} is all zero, corresponding to term {1}", m, word_counts.attributes[m].text);
                    V[m % M, m % N] = 1;
                }
            }

            for (int n = 0; n < N; ++n)
            {
                if (0 == n_count[n])
                {
                    Logging.Info("Column {0} is all zero, corresponding to doc {1}", n, word_counts.acl_ids[n]);
                    V[n % M, n % N] = 1;
                }
            }

            Logging.Info("-Checking for empty rows and columns");

            // Normalise the columns (i.e. each document gets a "unit" vector of words)
            Tools.NormaliseColumns(M, N, V);

            // Now factorize the dude
            int ITERATIONS = 100;
            int K = 200;
            float[,] W = new float[M, K];
            float[,] H = new float[N, K];
            //Factorise(K, M, N, D, A, P);
            //NMFMultiplicativeEuclidean.Factorise(K, M, N, D, A, P, ITERATIONS);
            NMFMultiplicativeDivergence.Factorise(K, M, N, V, W, H, ITERATIONS);

            // Let's save these matrices so that we can do some analysis with them...
            Utilities.Files.SerializeFile.Save(String.Format(@"C:\temp\nmf_{0}_W.dat", K), W);
            Utilities.Files.SerializeFile.Save(String.Format(@"C:\temp\nmf_{0}_H.dat", K), H);
        }
#endif

        #endregion
    }
}
