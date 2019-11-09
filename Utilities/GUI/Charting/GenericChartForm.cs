#if false

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Utilities.GUI.Charting
{
    /// <summary>
    /// Summary description for GenericChartForm.
    /// </summary>
    public class GenericChartForm : Form
    {
        private int num_charts_x;
        private int num_charts_y;
        private int num_charts;
        private MultiChart2D[] charts;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        public GenericChartForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        private int getChartOffset(int i, int j)
        {
            return j * num_charts_x + i;
        }

        public void setChartCounts(int anum_charts_x, int anum_charts_y)
        {
            num_charts_x = anum_charts_x;
            num_charts_y = anum_charts_y;
            num_charts = num_charts_x * num_charts_y;

            SuspendLayout();
            Controls.Clear();

            charts = new MultiChart2D[num_charts];
            for (int i = 0; i < num_charts; ++i)
            {
                charts[i] = new MultiChart2D();
                charts[i].Location = new Point(0, 0);
                charts[i].Name = "Chart" + i;
                charts[i].Size = new Size(0, 0);
                charts[i].TabIndex = i;

                Controls.Add(charts[i]);
            }

            recalcChartSizes();

            ResumeLayout(false);
        }

        private void recalcChartSizes()
        {
            int chart_width = Width / num_charts_x;
            int chart_height = (Height - 50) / num_charts_y;

            for (int i = 0; i < num_charts_x; ++i)
            {
                for (int j = 0; j < num_charts_y; ++j)
                {
                    int chart_offset = getChartOffset(i, j);
                    charts[chart_offset].Width = chart_width;
                    charts[chart_offset].Height = chart_height;
                    charts[chart_offset].Left = i * chart_width;
                    charts[chart_offset].Top = j * chart_height;
                }
            }

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Nothing we can do if we have no charts...
            if (0 == num_charts_x || 0 == num_charts_y)
            {
                return;
            }

            SuspendLayout();
            recalcChartSizes();
            ResumeLayout(false);
        }

        public MultiChart2D this[int i] => charts[i];

        public MultiChart2D this[int i, int j]
        {
            get
            {
                int chart_offset = getChartOffset(i, j);
                return charts[chart_offset];
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		public static void TestHarness()
		{
			GenericChartForm a = new GenericChartForm();
			a.setChartCounts(3,5);
			a.Show();

			Series s = new Series("Jimme", ChartType.Line);
			s.addPoint(0, 0);
			s.addPoint(1, 1);
			a[0,0].addSeries(s);
			a[0,1].addSeries(s);
			a[2,0].addSeries(s);
		}
#endif

        #endregion

        private int dispose_count = 0;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            Logging.Debug("GenericChartForm::Dispose({0}) @{1}", disposing, dispose_count);

            if (dispose_count == 0)
            {
                components?.Dispose();
            }

            components = null;

            base.Dispose(disposing);

            ++dispose_count;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GenericChartForm));
            // 
            // GenericChartForm
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            ClientSize = new System.Drawing.Size(736, 350);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "GenericChartForm";
            Text = "GenericChartForm";

        }
        #endregion
    }
}

#endif
