using Utilities.Mathematics.LinearAlgebra;

namespace Utilities.Random
{
    public interface IUniformRandomSource
    {
        void reset();
        double nextRandomDouble();
        void nextRandomVector(Vector vector);
        void nextRandomVectorMultiple(int N, Vector vector);
        void nextRandomVectorDiminishing(int N, Vector vector);

    }
}
