namespace Utilities.GUI.Render3D
{
    public struct Point3D
    {
        public double x;
        public double y;
        public double z;

        public Point3D(double ax, double ay, double az)
        {
            x = ax;
            y = ay;
            z = az;
        }

        public void assign(Point3D other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }

        public void assign(double ax, double ay, double az)
        {
            x = ax;
            y = ay;
            z = az;
        }

        public Point3D clone()
        {
            return new Point3D(x, y, z);
        }

        public override string ToString()
        {
            return x + "," + y + "," + z;
        }
    }
}
