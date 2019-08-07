using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Utilities.GUI.Charting
{
	public class TopographicalChart : UserControl
	{
		double[,] dataset;
		string x_name;
		double x_min;
		double x_max;
		string y_name;
		double y_min;
		double y_max;

		double datasetmin;
		double datasetmax;

		int last_mouse_x;
		int last_mouse_y;

		public TopographicalChart()
		{
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			last_mouse_x = e.X;
			last_mouse_y = e.Y;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			// If we have a dataset to render
			if (null != dataset)
			{
				int datasetwidth = dataset.GetUpperBound(1)+1;
				int datasetheight = dataset.GetUpperBound(0)+1;
				int screenwidth = this.Width;
				int screenheight = this.Height;

				float rendersquarewidth = screenwidth / (float) datasetwidth;
				float rendersquareheight = screenheight / (float) datasetheight;
				
				// Check that the mouse is still in range
				if (last_mouse_x > screenwidth) last_mouse_x = screenwidth-1;
				if (last_mouse_y > screenheight) last_mouse_y = screenheight-1;

                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    for (int i = 0; i < datasetheight; ++i)
                    {
                        for (int j = 0; j < datasetwidth; ++j)
                        {
                            if (Double.IsNaN(dataset[i, j]))
                            {
                                brush.Color = Color.Orange;
                            }
                            else if (datasetmin == dataset[i, j])
                            {
                                brush.Color = Color.Blue;
                            }
                            else if (datasetmax == dataset[i, j])
                            {
                                brush.Color = Color.Red;
                            }
                            else
                            {
                                int colorvalue = 255 - (int)(255.0 * (dataset[i, j] - datasetmin) / (datasetmax - datasetmin));
                                brush.Color = Color.FromArgb(colorvalue, colorvalue, colorvalue);
                            }

                            e.Graphics.FillRectangle(brush, j * rendersquarewidth, i * rendersquareheight, rendersquarewidth, rendersquareheight);
                        }
                    }

                    // Print out some information
                    double current_y = y_min + (y_max - y_min) * last_mouse_y / (double)(this.Height - rendersquareheight);
                    double current_x = x_min + (x_max - x_min) * last_mouse_x / (double)(this.Width - rendersquarewidth);

                    int dataset_i = (int)(datasetheight * last_mouse_y / (double)this.Height);
                    int dataset_j = (int)(datasetwidth * last_mouse_x / (double)this.Width);
                    if (dataset_i > dataset.GetUpperBound(0)) dataset_i = dataset.GetUpperBound(0);
                    if (dataset_j > dataset.GetUpperBound(1)) dataset_j = dataset.GetUpperBound(1);
                    if (dataset_i < 0) dataset_i = 0;
                    if (dataset_j < 0) dataset_j = 0;

                    using (Font font = new Font(FontFamily.GenericSansSerif, 8.0f))
                    {
                        brush.Color = Color.Green;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat("Mouse at {0}={1} {2}={3} value={4} i,j=({5},{6})", x_name, current_x, y_name, current_y, dataset[dataset_i, dataset_j], dataset_i, dataset_j);
                        e.Graphics.DrawString(sb.ToString(), font, brush, 10, 10);
                    }
                }
			}

			// Otherwise we have no dataset to render
			else
			{
				ChartTools.renderNoDatasetMessage(e.Graphics);
			}
		}

		public void setDataset(double[,] adataset, string ax_name, double ax_min, double ax_max, string ay_name, double ay_min, double ay_max)
		{
			this.dataset = adataset;
			x_name = ax_name;
			x_min = ax_min;
			x_max = ax_max;
			y_name = ay_name;
			y_min = ay_min;
			y_max = ay_max;

			datasetmin = dataset[0,0];
			datasetmax = dataset[0,0];

			int datasetwidth = dataset.GetUpperBound(1)+1;
			int datasetheight = dataset.GetUpperBound(0)+1;
			for (int i = 0; i < datasetheight; ++i)
			{
				for (int j = 0; j < datasetwidth; ++j)
				{
					if (dataset[i,j] < datasetmin)
					{
						datasetmin = dataset[i,j];
					}
					else if (dataset[i,j] > datasetmax)
					{
						datasetmax = dataset[i,j];
					}
				}
			}

			this.Refresh();
		}

		public void setTestDataset()
		{
			double[,] adataset = new double[4,3];
			adataset[0,0] = 5.0;
			adataset[2,1] = 6.0;
			adataset[1,2] = 15.0;
			setDataset(adataset, "x", 0, 1, "y", 0, 1);
		}

		public void showFormModal()
		{
            using (SingleControlForm form = new SingleControlForm())
            {
                form.setControl(this);
                form.ShowDialog();
            }
		}

		public void showForm()
		{
            using (SingleControlForm form = new SingleControlForm())
            {
                form.setControl(this);
                form.Show();
            }
		}
	}
}


