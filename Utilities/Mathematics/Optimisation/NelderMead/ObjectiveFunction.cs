namespace Utilities.Mathematics.Optimisation.NelderMead
{
	public interface ObjectiveFunction
	{
		double evaluate(double[] p);
		void constrainSearch(ref double[] p);
	}
}
