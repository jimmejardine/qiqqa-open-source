using System;

namespace Utilities.Strings
{
    public class LongestCommonSubstring
    {
        public class Result
        {
            public string substring = "";
            public int length = 0;
            public int S_pos = 0;
            public int T_pos = 0;

            public override string ToString()
            {
                return String.Format("{0}:{1}", length, substring);
            }
        }

        public static Result Find(string S, string T)
        {
            Result result = new Result();

            int[,] L = new int[S.Length, T.Length];
            for (int i = 0; i < S.Length; ++i)
            {
                for (int j = 0; j < T.Length; ++j)
                {
                    if (S[i] == T[j])
                    {
                        if (i == 0 || j == 0)
                        {
                            L[i, j] = 1;
                        }
                        else
                        {
                            L[i, j] = L[i - 1, j - 1] + 1;
                        }

                        if (result.length < L[i, j])
                        {
                            result.length = L[i, j];
                            result.S_pos = i;
                            result.T_pos = j;
                        }
                    }
                    else
                    {
                        L[i, j] = 0;
                    }
                }
            }

            // Get the actual substring
            if (0 < result.length)
            {
                result.substring = S.Substring(result.S_pos - result.length + 1, result.length);
            }

            return result;
        }
    }
}
