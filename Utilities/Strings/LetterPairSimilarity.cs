using System;

namespace Utilities.Strings
{
    public class LetterPairSimilarity
    {
        /// <summary>
        /// Compares the two strings based on letter pair matches
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns>The percentage match from 0.0 to 1.0 where 1.0 is 100%</returns>
        public static double CompareStrings(string A, string B)
        {
            bool[] A_used = new bool[A.Length];
            bool[] B_used = new bool[B.Length];

            int countA = 0;
            {
                for (int a = A.Length - 2; a >= 0; --a)
                {
                    // If the first one is a space, then we can jump back two spaces, cos it won't match as the second time either
                    if (Char.IsWhiteSpace(A[a]))
                    {
                        --a;
                        continue;
                    }

                    // If the second one is a space, move back
                    if (Char.IsWhiteSpace(A[a + 1]))
                    {
                        continue;
                    }

                    ++countA;
                }
            }

            int countB = 0;
            {
                for (int b = B.Length - 2; b >= 0; --b)
                {
                    // If the first one is a space, then we can jump back two spaces, cos it won't match as the second time either
                    if (Char.IsWhiteSpace(B[b]))
                    {
                        --b;
                        continue;
                    }

                    // If the second one is a space, move back
                    if (Char.IsWhiteSpace(B[b + 1]))
                    {
                        continue;
                    }

                    ++countB;
                }
            }

            int countBOTH = 0;
            {
                for (int a = A.Length - 2; a >= 0; --a)
                {
                    if (A_used[a])
                    {
                        continue;
                    }

                    // If the first one is a space, then we can jump back two spaces, cos it won't match as the second time either
                    if (Char.IsWhiteSpace(A[a]))
                    {
                        --a;
                        continue;
                    }

                    // If the second one is a space, move back
                    if (Char.IsWhiteSpace(A[a + 1]))
                    {
                        continue;
                    }

                    for (int b = B.Length - 2; b >= 0; --b)
                    {
                        if (B_used[b])
                        {
                            continue;
                        }

                        // If the first one is a space, then we can jump back two spaces, cos it won't match as the second time either
                        if (Char.IsWhiteSpace(B[b]))
                        {
                            --b;
                            continue;
                        }

                        // If the second one is a space, move back
                        if (Char.IsWhiteSpace(B[b + 1]))
                        {
                            continue;
                        }

                        if (A[a] == B[b] && A[a + 1] == B[b + 1])
                        {
                            ++countBOTH;

                            A_used[a] = true;
                            B_used[b] = true;
                        }
                    }
                }
            }

            double countEITHER = (countA + countB);
            return countEITHER > 0 ? (2.0 * countBOTH) / countEITHER : 1.0;
        }
    }
}
