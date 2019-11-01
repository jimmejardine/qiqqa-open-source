using System.Collections.Generic;
using System.Drawing;

namespace Utilities.GUI.Charting
{
    public class ThemedMultiChart2D : MultiChart2D
    {
        private class Theme
        {
            public ChartType chartType = ChartType.LineAndPoint;
            public LinkedList<Point2D> points = new LinkedList<Point2D>();
        }

        private int MAX_AGE = 600;
        private int current_age = 0;
        private Dictionary<string, Theme> themes = new Dictionary<string, Theme>();

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // ThemedMultiChart2D
            // 
            Name = "ThemedMultiChart2D";
            Size = new Size(412, 247);
            ResumeLayout(false);
        }


        public void addThemedPoint(string theme_name, double value, bool advanceTime, ChartType chartType)
        {
            if (advanceTime)
            {
                ++current_age;
            }

            if (!themes.ContainsKey(theme_name))
            {
                themes[theme_name] = new Theme();
            }

            Theme theme = themes[theme_name];
            theme.points.AddLast(new Point2D(current_age, value));
            theme.chartType = chartType;
            staleOldPoints();
            rebuildSeries();
        }

        public void addThemedPoint(string theme_name, double value)
        {
            addThemedPoint(theme_name, value, true, ChartType.LineAndPoint);
        }

        private void staleOldPoints()
        {
            List<string> keysToKill = new List<string>();

            // Purge the old points
            foreach (string key in themes.Keys)
            {
                Theme theme = themes[key];
                if (theme.points.First.Value.x < current_age - MAX_AGE)
                {
                    theme.points.RemoveFirst();
                    if (0 == theme.points.Count)
                    {
                        keysToKill.Add(key);
                    }
                }
            }

            // Kill the empty themes
            foreach (string key in keysToKill)
            {
                themes.Remove(key);
            }
        }

        private void rebuildSeries()
        {
            SuspendRefresh();

            clearSeries();
            foreach (string key in themes.Keys)
            {
                Theme theme = themes[key];
                addSeries(new Series(key, theme.chartType, theme.points));
            }

            ResumeRefresh();
        }
    }
}
