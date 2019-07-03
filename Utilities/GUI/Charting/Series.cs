using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

// For now use this Point2D.  We will move it when we have time.

namespace Utilities.GUI.Charting
{
	public class Series
	{
		public string name;
		public ChartType charttype;
		public ChartAxis chartaxis = ChartAxis.Primary;
		public Color color = Color.Black;
		ArrayList points = new ArrayList();
		Point2D min = new Point2D();
		Point2D max = new Point2D();
		bool need_minmax_recalc = true;

		public Series(string aname, ChartType acharttype) : this(aname, acharttype, new Point2D[0])
		{
		}

		public Series(string aname, ChartType acharttype, ArrayList apoints)
		{
			name = aname;
			charttype = acharttype;
			setPoints(apoints);
		}

		public Series(string aname, ChartType acharttype, Point2D[] apoints)
		{
            name = aname;
            charttype = acharttype;
            setPoints(apoints);
        }

        public Series(string aname, ChartType acharttype, LinkedList<Point2D> apoints)
        {
            name = aname;
            charttype = acharttype;
            setPoints(apoints);
        }

		public void addPoint(double x, double y)
		{
			addPoint(new Point2D(x, y));
		}

		public void addPoint(Point2D point)
		{
			if (Double.IsNaN(point.x) || Double.IsNaN(point.y))
			{
				return;
			}

			need_minmax_recalc = true;
			points.Add(point);
		}

		public void setPoints(ArrayList apoints)
		{
			points.Clear();
			foreach (Point2D point in apoints)
			{
				addPoint(point);
			}
		}
			
		public void setPoints(Point2D[] apoints)
		{
            points.Clear();
            foreach (Point2D point in apoints)
            {
                addPoint(point);
            }
        }

        public void setPoints(LinkedList<Point2D> apoints)
        {
            points.Clear();
            foreach (Point2D point in apoints)
            {
                addPoint(point);
            }
        }


		public IEnumerator GetEnumerator()
		{
			return points.GetEnumerator();
		}

		public void getMinMax(out Point2D amin, out Point2D amax)
		{
			if (need_minmax_recalc)
			{
				recalculateMinMax();
			}

			amin = min.clone();
			amax = max.clone();
		}

		void recalculateMinMax()
		{
			// Check that we have a series
			if (points.Count == 0)
			{
				min.assign(0, 0);
				max.assign(0, 0);
				return;
			}

			// Otherwise find the bounds
			min.assign((Point2D) points[0]);
			max.assign((Point2D) points[0]);
			IEnumerator i = points.GetEnumerator();
			while (i.MoveNext())
			{
				Point2D point = (Point2D) i.Current;
				if (point.x < min.x) min.x = point.x;
				if (point.y < min.y) min.y = point.y;
				if (point.x > max.x) max.x = point.x;
				if (point.y > max.y) max.y = point.y;
			}

			need_minmax_recalc = false;
		}

		public int Count
		{
			get
			{
				return points.Count;
			}
		}

		public Point2D this[int index]
		{
			get
			{
				return (Point2D) points[index];
			}

			set
			{
				points[index] = value;
				
			}
		}	
	}
}
