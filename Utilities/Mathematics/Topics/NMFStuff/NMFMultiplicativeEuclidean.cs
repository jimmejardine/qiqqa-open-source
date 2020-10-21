using System;
using System.Threading;
using Utilities.Misc;

namespace Utilities.Mathematics.Topics.NMFStuff
{
    /*
     * NMF implementation of the multiplicative update algorithm for the Euclidean measure as stated in:
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

    public class NMFMultiplicativeEuclidean
    {
        private static readonly int LOG_PERIOD = 500;
        private static readonly int NUM_THREADS = 1;

        public static void Factorise(int K, int M, int N, float[,] V, float[,] W, float[,] H, int ITERATIONS)
        {
            double[] rmses = new double[NUM_THREADS];
            float[,] WH = new float[M, N];

            // Random initialisation of W and H
            Tools.InitialiseRandomMatrices(K, M, N, W, H);

            // And now we iterate
            int iteration = 0;
            while (true)
            {
                Logging.Info("Iteration {0}", iteration);

                // Calculate the new WH (for efficiency reasons so we dont have to calc twice)
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
                }


                // Check exit condition
                ++iteration;
                if (iteration >= ITERATIONS)
                {
                    break;
                }

                // Calculate the new P
                {
                    int num_threads_running = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        int thread_id_local = thread_id;
                        ThreadTools.StartThread(o => PARALLEL_H(K, M, N, V, W, H, WH, NUM_THREADS, thread_id_local, ref num_threads_running));
                    }
                    Tools.WaitForThreads(ref num_threads_running);
                }

                // Calculate the new A
                {
                    int num_threads_running = 0;
                    for (int thread_id = 0; thread_id < NUM_THREADS; ++thread_id)
                    {
                        int thread_id_local = thread_id;
                        ThreadTools.StartThread(o => PARALLEL_W(K, M, N, V, W, H, WH, NUM_THREADS, thread_id_local, ref num_threads_running));
                    }
                    Tools.WaitForThreads(ref num_threads_running);
                }
            }
        }

        /// <summary>
        /// This method calculates the interim product of E and M, but also scales E to have unitary columns (and hence inversely scales the rows of M).
        /// It also calculates the rmse for reporting progress...
        /// </summary>
        private static void PARALLEL_WH(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, int NUM_THREADS, int thread_id, ref int num_threads_running, double[] rmses)
        {
            Interlocked.Increment(ref num_threads_running);

            // First normalise the A matrix
            Tools.NormaliseColumns(K, M, N, W, H, NUM_THREADS, thread_id);

            // Then calculate WH and the rmses
            rmses[thread_id] = 0;
            for (int m = 0 + thread_id; m < M; m += NUM_THREADS)
            {
                if (m % LOG_PERIOD == 0) Logging.Info("m={0}/{1}", m, M);

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

        private static void PARALLEL_H(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, int NUM_THREADS, int thread_id, ref int num_threads_running)
        {
            Interlocked.Increment(ref num_threads_running);

            //            float[] V_N = new float[M];
            //            float[] WH_N = new float[M];

            float numerator = 0;
            float denominator = 0;

            for (int n = 0 + thread_id; n < N; n += NUM_THREADS)
            {
                if (n % LOG_PERIOD == 0) Logging.Info("n={0}/{1}", n, N);

                // Cache the N column of V and WH
                //for (int m = 0; m < M; ++m)
                //{
                //    V_N[m] = V[m, n];
                //    WH_N[m] = WH[m, n];
                //}

                for (int k = 0; k < K; ++k)
                {
                    numerator = 0;
                    denominator = 0;
                    for (int m = 0; m < M; ++m)
                    {
                        numerator += W[m, k] * V[m, n];
                        denominator += W[m, k] * WH[m, n];

                        //float W_mk = W[m, k];
                        //numerator += W_mk * V_N[m];
                        //denominator += W_mk * WH_N[m];
                    }

                    if (0 != denominator)
                    {
                        H[n, k] = H[n, k] * numerator / denominator;
                    }
                    else
                    {
                    }
                }
            }

            Interlocked.Decrement(ref num_threads_running);
        }

        private static void PARALLEL_W(int K, int M, int N, float[,] V, float[,] W, float[,] H, float[,] WH, int NUM_THREADS, int thread_id, ref int num_threads_running)
        {
            Interlocked.Increment(ref num_threads_running);

            float numerator = 0;
            float denominator = 0;

            for (int m = 0 + thread_id; m < M; m += NUM_THREADS)
            {
                if (m % LOG_PERIOD == 0) Logging.Info("m={0}/{1}", m, M);

                for (int k = 0; k < K; ++k)
                {
                    numerator = 0;
                    denominator = 0;
                    for (int n = 0; n < N; ++n)
                    {
                        numerator += V[m, n] * H[n, k];
                        denominator += WH[m, n] * H[n, k];
                    }

                    if (0 != denominator)
                    {
                        W[m, k] = W[m, k] * numerator / denominator;
                    }
                }
            }

            Interlocked.Decrement(ref num_threads_running);
        }
    }
}
