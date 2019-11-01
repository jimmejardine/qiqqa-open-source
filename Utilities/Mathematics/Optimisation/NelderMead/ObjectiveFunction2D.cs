namespace Utilities.Mathematics.Optimisation.NelderMead
{
    public interface ObjectiveFunction2D
    {
        double evaluate(Point2D p);
        void constrainSearch(ref Point2D p);
    }
}
