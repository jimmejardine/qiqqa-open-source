using System;
using System.Windows;

namespace Utilities.Mathematics.Geometry
{
    public class BoxDistance
    {
        public static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        public static double CalculateDistanceBetweenTwoBoxes(Rect a, Rect b)
        {
            /*    
             * 
             * Method of this madness: assume rectangle A is in position 5 below.
             * Then consider the position of rectangle B:
             *   B is in position 1 3 7 9 - the minimum distance is the two corners
             *   B is in position 2 4 6 8 - the minimum distance is the edge distance
             *   The rectangles either intersect (position 5) - distance 0
             *
             *       7 8 9
             *       4 5 6
             *       1 2 3
             *
             */

            // Init
            bool b_is_left = (b.Right < a.Left);
            bool b_is_right = (b.Left > a.Right);
            bool b_is_above = (b.Bottom < a.Top);
            bool b_is_below = (b.Top > a.Bottom);

            if (false) { }

            /*1*/
            else if (b_is_left && b_is_below) return LOGGIT(1) + CalculateDistance(b.Right, b.Top, a.Left, a.Bottom);
            /*3*/
            else if (b_is_right && b_is_below) return LOGGIT(3) + CalculateDistance(b.Left, b.Top, a.Right, a.Bottom);
            /*7*/
            else if (b_is_left && b_is_above) return LOGGIT(7) + CalculateDistance(b.Right, b.Bottom, a.Left, a.Top);
            /*9*/
            else if (b_is_right && b_is_above) return LOGGIT(9) + CalculateDistance(b.Left, b.Bottom, a.Right, a.Top);

            /*2*/
            else if (b_is_below) return LOGGIT(2) + (b.Top - a.Bottom);
            /*8*/
            else if (b_is_above) return LOGGIT(8) + (a.Top - b.Bottom);
            /*4*/
            else if (b_is_left) return LOGGIT(4) + (a.Left - b.Right);
            /*6*/
            else if (b_is_right) return LOGGIT(5) + (b.Left - a.Right);

            /*5*/
            else return LOGGIT(5) + 0;
        }

        private static double LOGGIT(int i)
        {
            //Logging.Info("{0}", i);
            return 0;
        }
    }

}
