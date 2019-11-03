using System;
using System.Threading;
using Utilities.Collections;
using Utilities.Misc;

namespace Utilities.Mathematics.Topics.NMFStuff
{
    /*
     * NMF implementation of the multiplicative update algorithm for the Kullback-Leibler divergence as stated in:
     * 
            @article{lee2001algorithms
            ,	author	= {Lee, D.D. and Seung, H.S.}
            ,	title	= {Algorithms for non-negative matrix factorization}
            ,	journal	= {Advances in neural information processing systems}
            ,	year	= {2001}
            ,	volume	= {13}
            ,	publisher	= {Citeseer}
            }
     */

    public class NMFMultiplicativeDivergence
    {
        private static readonly int LOG_PERIOD = 500;
        private static readonly int NUM_THREADS = 9;

        public static void Factorise(int K, int M, int N, float[,] V, float[,] W, float[,] H, int ITERATIONS)
        {
            double[] rmses = new double[NUM_THREADS];
            float[,] WH = new float[M, N];


            // Computational enhancement with sparse matrices
            MultiMap<int, int> M_sparse;
            MultiMap<int, int> N_sparse;
            FindSparseLocations(M, N, V, out M_sparse, out N_sparse);

            // Random initialisation of W and H
            Tools.InitialiseRandomMatrices(K, M, N, W, H);

            // And now we iterate            
            int iteration = 0;
            while (true)
            {
                Logging.Info("Iteration {0}", iteration);

                // Calculate the new WH every now and then to check our progress...
                ++iteration;
                if (0 == iteration % 3)
                {
                    int num_threads_running = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        int thread_id_local = thread_id;
                        ThreadTools.StartThread(o => PARALLEL_WH(K, M, N, V, W, H, WH, NUM_THREADS, thread_id_local, ref num_threads_running, rmses));
                    }
                    Tools.WaitForThreads(ref num_threads_running);

                    // Tally the RMSE
                    double rmse = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        rmse += rmses[thread_id];
                    }
                    rmse = Math.Sqrt(rmse);
                    Logging.Info("Iteration {0} has an RMSE of {1}", iteration, rmse);

                    // Check exit condition
                    if (iteration >= ITERATIONS)
                    {
                        break;
                    }
                }

                // Calculate the new H
                {
                    int num_threads_running = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        int thread_id_local = thread_id;
                        ThreadTools.StartThread(o => PARALLEL_H(K, M, N, V, W, H, WH, M_sparse, N_sparse, NUM_THREADS, thread_id_local, ref num_threads_running));
                    }
                    Tools.WaitForThreads(ref num_threads_running);
                }

                // Calculate the new W
                {
                    int num_threads_running = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        int thread_id_local = thread_id;
                        ThreadTools.StartThread(o => PARALLEL_W(K, M, N, V, W, H, WH, M_sparse, N_sparse, NUM_THREADS, thread_id_local, ref num_threads_running));
                    }
                    Tools.WaitForThreads(ref num_threads_running);
                }
            }
        }

        private static void FindSparseLocations(int M, int N, float[,] V, out MultiMap<int, int> M_sparse, out MultiMap<int, int> N_sparse)
        {
            Logging.Info("+Finding sparse locations");

            M_sparse = new MultiMap<int, int>(false);
            N_sparse = new MultiMap<int, int>(false);

            int count = 0;
            for (int m = 0; m < M; ++m)
            {
                for (int n = 0; n < N; ++n)
                {
                    if (V[m, n] > 0)
                    {
                        ++count;
                        M_sparse.Add(m, n);
                        N_sparse.Add(n, m);
                    }
                }
            }

            Logging.Info("The matrix has sparseness of {0}", count / (double)(M * N));
            Logging.Info("-Finding sparse locations");
        }

        /// <summary>
        /// This method calculates the interim product of E and M, but also scales E to have unitary columns (and hence inversely scales the rows of M).
        /// It also calculates the rmse for reporting progress...
        /// </summary>
        private static void PARALLEL_WH(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, int NUM_THREADS, int thread_id, ref int num_threads_running, double[] rmses)
        {
            Interlocked.Increment(ref num_threads_running);

            // First normalise the W matrix
            Tools.NormaliseColumns(K, M, N, W, H, NUM_THREADS, thread_id);

            // Then calculate WH and the rmses
            rmses[thread_id] = 0;
            for (int m = 0 + thread_id; m < M; m += NUM_THREADS)
            {
                if (m % LOG_PERIOD == 0) Logging.Info("WH.m={0}/{1}", m, M);

                for (int n = 0; n < N; ++n)
                {
                    float total = 0;

                    for (int k = 0; k < K; ++k)
                    {
                        total += W[m, k] * H[n, k];
                    }

                    WH[m, n] = total;

                    rmses[thread_id] += Math.Pow(V[m, n] - total, 2);
                }
            }

            Interlocked.Decrement(ref num_threads_running);
        }

        private static void PARALLEL_H(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, MultiMap<int, int> M_sparse, MultiMap<int, int> N_sparse, int NUM_THREADS, int thread_id, ref int num_threads_running)
        {
            Interlocked.Increment(ref num_threads_running);

            // First precalc the denominators for each k
            float[] denominators = new float[K];
            for (int k = 0; k < K; ++k)
            {
                for (int m = 0; m < M; ++m)
                {
                    denominators[k] += W[m, k];
                }
            }

            // Then do the update
            for (int n = 0 + thread_id; n < N; n += NUM_THREADS)
            {
                if (n % LOG_PERIOD == 0) Logging.Info("H.n={0}/{1}", n, N);

                for (int k = 0; k < K; ++k)
                {
                    float numerator = 0;
                    foreach (int m in N_sparse[n])
                    {
                        float wh = CalcWH(K, M, N, W, H, m, n);
                        numerator += W[m, k] * V[m, n] / wh;
                    }

                    if (n == 0 && k == 0)
                    {
                        Logging.Info("pre.H[n, k] = {0}", H[n, k]);
                        Logging.Info("num         = {0}", numerator);
                        Logging.Info("den         = {0}", denominators[k]);
                    }

                    H[n, k] = H[n, k] * numerator / denominators[k];

                    if (n == 0 && k == 0)
                    {
                        Logging.Info("H[n, k] = {0}", H[n, k]);
                    }
                }
            }

            Interlocked.Decrement(ref num_threads_running);
        }

        private static void PARALLEL_W(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, MultiMap<int, int> M_sparse, MultiMap<int, int> N_sparse, int NUM_THREADS, int thread_id, ref int num_threads_running)
        {
            Interlocked.Increment(ref num_threads_running);

            // First precalc the denominators for each k
            float[] denominators = new float[K];
            for (int n = 0; n < N; ++n)
            {
                for (int k = 0; k < K; ++k)
                {
                    denominators[k] += H[n, k];
                }
            }

            // Then do the update
            for (int m = 0 + thread_id; m < M; m += NUM_THREADS)
            {
                if (m % LOG_PERIOD == 0) Logging.Info("W.m={0}/{1}", m, M);

                for (int k = 0; k < K; ++k)
                {
                    float numerator = 0;
                    foreach (int n in M_sparse[m])
                    {
                        float wh = CalcWH(K, M, N, W, H, m, n);
                        numerator += H[n, k] * V[m, n] / wh;
                    }

                    W[m, k] = W[m, k] * numerator / denominators[k];
                    if (m == 0 && k == 0)
                    {
                        Logging.Info("W[m, k] = {0}", W[m, k]);
                    }
                }
            }

            Interlocked.Decrement(ref num_threads_running);
        }

        private static float CalcWH(int K, int M, int N, float[,] W, float[,] H, int m, int n)
        {
            float total = 0;

            for (int k = 0; k < K; ++k)
            {
                total += W[m, k] * H[n, k];
            }

            return total;
        }
    }
}
