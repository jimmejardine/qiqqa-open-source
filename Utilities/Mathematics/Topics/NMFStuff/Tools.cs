using System.Threading;
using Utilities.Random;

namespace Utilities.Mathematics.Topics.NMFStuff
{
    class Tools
    {
        internal static void InitialiseRandomMatrices(int K, int M, int N, float[,] W, float[,] H)
        {
            double SPARSENESS = 0.3;

            for (int k = 0; k < K; ++k)
            {
                for (int m = 0; m < M; ++m)
                {
                    if (RandomAugmented.Instance.NextDouble() > SPARSENESS)
                    {
                        W[m, k] = (float)RandomAugmented.Instance.NextDouble();
                    }
                }
                for (int n = 0; n < N; ++n)
                {
                    if (RandomAugmented.Instance.NextDouble() > SPARSENESS)
                    {
                        H[n, k] = (float)RandomAugmented.Instance.NextDouble();
                    }
                }
            }
        }

        internal static void NormaliseColumns(int K, int M, int N, float[,] W, float[,] H, int NUM_THREADS, int thread_id)
        {
            for (int k = 0 + thread_id; k < K; k += NUM_THREADS)
            {
                float total = 0;
                for (int m = 0; m < M; ++m)
                {
                    total += W[m, k];
                }

                // Let's rescale
                if (0 < total)
                {
                    for (int m = 0; m < M; ++m)
                    {
                        W[m, k] /= total;
                    }

                    for (int n = 0; n < N; ++n)
                    {
                        H[n, k] *= total;
                    }
                }
            }
        }

        internal static void NormaliseColumns(int M, int N, float[,] V)
        {
            for (int n = 0; n < N; ++n)
            {
                float total = 0;
                for (int m = 0; m < M; ++m)
                {
                    total += V[m, n];
                }

                if (total > 0)
                {
                    for (int m = 0; m < M; ++m)
                    {
                        V[m, n] /= total;
                    }
                }
            }
        }


        internal static void WaitForThreads(ref int num_threads_running)
        {
            while (true)
            {
                Thread.Sleep(100);
                if (0 == num_threads_running)
                {
                    return;
                }
            }
        }
    }
}
