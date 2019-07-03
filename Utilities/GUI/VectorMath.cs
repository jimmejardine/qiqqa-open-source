using System;
using System.Drawing;

namespace Utilities.Mathematics.GUI
{
	public class VectorMath
	{
		public static PointF negate(PointF p)
		{
			return new PointF(-p.X, -p.Y);
		}

		public static void negate(ref PointF p)
		{
			p.X = -p.X;
			p.Y = -p.Y;
		}

		public static void sum(PointF a, PointF b, ref PointF d)
		{
			d.X = a.X + b.X;
			d.Y = a.Y + b.Y;
		}

		public static void difference(PointF a, PointF b, ref PointF d)
		{
			d.X = a.X - b.X;
			d.Y = a.Y - b.Y;
		}

		public static void multiply(double factor, ref PointF p)
		{
			p.X *= (float) factor;
			p.Y *= (float) factor;
		}

		public static double dot(PointF a, PointF b)
		{
			return a.X*b.X + a.Y*b.Y;
		}
		
		public static double length_squared(PointF p)
		{
			return Math.Pow(p.X, 2) + Math.Pow(p.Y, 2);
		}

		public static double length(PointF p)
		{
			return Math.Pow(length_squared(p), 0.5);
		}

		public static void normalise(ref PointF p)
		{
			double norm = length(p);
			if (0 < norm)
			{
				p.X = (float) (p.X / norm);
				p.Y = (float) (p.Y / norm);
			}
			else
			{
				p.X = p.Y = (float) Math.Sqrt(0.5);
			}
		}
	}
}
