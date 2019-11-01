using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Random
{
    public class AntitheticRandomSource : IUniformRandomSource
    {
        private IUniformRandomSource source;
        private double last_double;
        private bool reuse_last_double = false;
        private Vector last_vector = new Vector(0);
        private bool reuse_last_vector = false;

        public AntitheticRandomSource(IUniformRandomSource _source)
        {
            source = _source;
        }

        public void reset()
        {
            source.reset();
        }

        public double nextRandomDouble()
        {
            if (reuse_last_double)
            {
                reuse_last_double = false;
                return 1.0 - last_double;
            }
            else
            {
                reuse_last_double = true;
                last_double = source.nextRandomDouble();
                return last_double;
            }
        }

        // Our random numbers are independent so a multiple series is nothing special
        public void nextRandomVectorMultiple(int N, Vector vector)
        {
            nextRandomVector(vector);
        }

        // Our random numbers are independent so a diminishing series is nothing special
        public void nextRandomVectorDiminishing(int N, Vector vector)
        {
            nextRandomVector(vector);
        }

        public void nextRandomVector(Vector vector)
        {
            // Check that our stored data is compatible
            if (vector.cols != last_vector.cols)
            {
                last_vector = new Vector(vector.cols);
                reuse_last_vector = false;
            }

            if (reuse_last_vector)
            {
                reuse_last_vector = false;
                for (int i = 0; i < vector.cols; ++i)
                {
                    vector[i] = 1.0 - last_vector[i];
                }
            }
            else
            {
                reuse_last_vector = true;
                source.nextRandomVector(last_vector);
                for (int i = 0; i < vector.cols; ++i)
                {
                    vector[i] = last_vector[i];
                }
            }
        }
    }
}
