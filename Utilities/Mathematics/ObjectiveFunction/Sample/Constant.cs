namespace Utilities.Mathematics.ObjectiveFunction.Sample
{
    public class Constant : ObjectiveFunction
    {
        public Constant()
        {
        }

        public double evaluate(double x)
        {
            return 3.0;
        }
    }
}
