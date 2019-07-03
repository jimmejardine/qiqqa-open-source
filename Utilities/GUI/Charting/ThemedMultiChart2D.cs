using System.Collections.Generic;
using System.Drawing;

namespace Utilities.GUI.Charting
{
    public class ThemedMultiChart2D : MultiChart2D
    {
        class Theme
        {
            public ChartType chartType = ChartType.LineAndPoint;
            public LinkedList<Point2D> points = new LinkedList<Point2D>();
        }

        int MAX_AGE = 600;
        int current_age = 0;
        Dictionary<string, Theme> themes = new Dictionary<string,Theme>();
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ThemedMultiChart2D
            // 
            this.Name = "ThemedMultiChart2D";
            this.Size = new Size(412, 247);
            this.ResumeLayout(false);
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

        void staleOldPoints()        
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

        void rebuildSeries()
        {
            this.SuspendRefresh();

            this.clearSeries();
            foreach (string key in themes.Keys)            
            {
                Theme theme = themes[key];
                this.addSeries(new Series(key, theme.chartType, theme.points));
            }

            this.ResumeRefresh();
        }
    }
}
