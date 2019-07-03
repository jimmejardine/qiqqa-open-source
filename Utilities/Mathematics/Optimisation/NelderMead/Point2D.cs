namespace Utilities.Mathematics.Optimisation.NelderMead
{
	public struct Point2D
	{
		public double x;
		public double y;
		
		public Point2D(double ax, double ay)
		{
			x = ax;
			y = ay;
		}

		public void assign(Point2D other)
		{
			x = other.x;
			y = other.y;
		}

		public void assign(double ax, double ay)
		{
			x = ax;
			y = ay;
		}

		public override string ToString()
		{
			return x + "," + y;
		}
	}
}
