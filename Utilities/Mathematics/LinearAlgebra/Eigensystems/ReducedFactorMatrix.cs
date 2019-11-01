using System;

namespace Utilities.Mathematics.LinearAlgebra.Eigensystems
{
    public class ReducedFactorMatrix
    {
        public static Matrix reduceFactors(Matrix correlations, int num_factors, bool normalise)
        {
            // Check that our input matrix is square
            if (!correlations.isSquare())
            {
                throw new GenericException("Can only factor-reduct square matrices");
            }

            int N = correlations.rows;

            // There is nothing to do if we are allowed enough factors
            if (num_factors >= N)
            {
                return correlations;
            }

            // Calculate the eigensystem for the correlation matrix and rank in order of decreasing eignevalue
            Matrix eigenvectors = new Matrix(correlations.rows, correlations.cols);
            Vector eigenvalues = new Vector(correlations.rows);
            Eigenvectors.calculateEigensystem(correlations, eigenvectors, eigenvalues);
            Eigenvectors.sortEigensystem(eigenvectors, eigenvalues);

            // Multiply in the eigenvalue to build the pseudo-square-root and blot out the columns beyond the number of factors
            for (int j = 0; j < num_factors && j < N; ++j)
            {
                eigenvectors.multiplyColumn(j, Math.Sqrt(eigenvalues[j]));
            }
            for (int j = num_factors; j < N; ++j)
            {
                eigenvectors.multiplyColumn(j, 0);
            }

            // Build the new factor-reduced correlations matrix
            Matrix correlations_reduced = eigenvectors.multiply(eigenvectors.transpose());

            // Reconstruct the correlations matrix by normalising the reduced one
            if (!normalise)
            {
                return correlations_reduced;
            }
            else
            {
                Matrix correlations_normalised = new Matrix(N, N);

                for (int i = 0; i < N; ++i)
                {
                    for (int j = 0; j < N; ++j)
                    {
                        correlations_normalised[i, j] = correlations_reduced[i, j] / Math.Sqrt(correlations_reduced[i, i] * correlations_reduced[j, j]);
                    }
                }

                return correlations_normalised;
            }
        }
    }
}
