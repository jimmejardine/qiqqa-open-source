namespace Utilities.GUI.Charting
{
    public class ChartType
    {
        public static ChartType Point = new ChartType();
        public static ChartType Line = new ChartType();
        public static ChartType LineAndPoint = new ChartType();
        public static ChartType SmoothLine = new ChartType();
        public static ChartType SmoothLineAndPoint = new ChartType();
        public static ChartType Bar = new ChartType();

        private ChartType()
        {
        }
    }
}
