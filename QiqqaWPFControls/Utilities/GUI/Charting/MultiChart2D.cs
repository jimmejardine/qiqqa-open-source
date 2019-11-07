using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Utilities.Mathematics;
// For now use this Point2D.  We will move it when we have time.


namespace Utilities.GUI.Charting
{
    public class MultiChart2D : UserControl
    {
        private static Color color_almost_white = Color.FromArgb(248, 248, 248);

        public float title_height = 60;
        public float legend_height = 60;
        public float axis_height = 60;
        private double axis_tick_gap_min = 50;
        private double legend_gap_width = 16;
        private double legend_indicator_size = 10;
        private double legend_indicator_gap_width = 5;
        private SolidBrush brush_background = new SolidBrush(Color.White);
        private SolidBrush brush_background_axisguide = new SolidBrush(color_almost_white);
        private SolidBrush brush_series = new SolidBrush(Color.Black);
        private Pen pen_border = new Pen(Color.Black);
        private Pen pen_series = new Pen(Color.Black, 1.0f);
        private Pen pen_axis = new Pen(Color.Black);
        private Pen pen_axis_guide = new Pen(color_almost_white, 2.0f);
        private float point_size = 6.0f;
        private float axis_notch_height = 5.0f;
        private Font font_title = new Font(FontFamily.GenericSansSerif, 16);
        private Font font_axis = new Font(FontFamily.GenericSansSerif, 13);
        private Font font_legend = new Font(FontFamily.GenericSansSerif, 14);
        private SolidBrush brush_axis = new SolidBrush(Color.Black);
        private SolidBrush brush_legend = new SolidBrush(Color.Black);
        private Pen pen_legend = new Pen(Color.Black);
        private SolidBrush brush_title = new SolidBrush(Color.Black);

        public string title;
        public string x_axis_title;
        public string y1_axis_title;
        public string y2_axis_title;

        public double x_axis_max = double.NaN;
        public double x_axis_min = double.NaN;
        public double y1_axis_min = double.NaN;
        public double y1_axis_max = double.NaN;
        public double y2_axis_min = double.NaN;
        public double y2_axis_max = double.NaN;
        private ArrayList series_list;
        private bool suspendingRefresh = false;

        public MultiChart2D()
        {
            series_list = new ArrayList();
            //			doSampleChart();

            // Double buffering setup
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            BackColor = Color.White;
            UpdateStyles();
        }

        protected override void OnResize(EventArgs e)
        {
            if (!suspendingRefresh) Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (series_list.Count == 0)
            {
                ChartTools.renderNoDatasetMessage(e.Graphics);
                return;
            }

            ChartRegion region_fullscreen = new ChartRegion(0, 0, Width, Height);

            ChartRegion region_title; ChartRegion region_title_remainder;
            region_fullscreen.splitHorizontal(title_height, out region_title, out region_title_remainder);

            ChartRegion region_legend; ChartRegion region_legend_remainder;
            region_title_remainder.splitHorizontal(region_title_remainder.Height - legend_height, out region_legend_remainder, out region_legend);
            region_legend.Top += 10;

            ChartRegion region_chart; ChartRegion region_x_axis; ChartRegion region_y1_axis; ChartRegion region_y2_axis;
            region_legend_remainder.splitAxes(axis_height, out region_chart, out region_x_axis, out region_y1_axis, out region_y2_axis);

            Point2D min_pri; Point2D max_pri; Point2D min_sec; Point2D max_sec;
            getSeriesExtents(out min_pri, out max_pri, out min_sec, out max_sec);

            // Set the axis overrides
            if (!double.IsNaN(x_axis_max)) max_pri.x = x_axis_max;
            if (!double.IsNaN(x_axis_max)) max_sec.x = x_axis_max;
            if (!double.IsNaN(x_axis_min)) min_pri.x = x_axis_min;
            if (!double.IsNaN(x_axis_min)) min_sec.x = x_axis_min;
            if (!double.IsNaN(y1_axis_max)) max_pri.y = y1_axis_max;
            if (!double.IsNaN(y1_axis_min)) min_pri.y = y1_axis_min;
            if (!double.IsNaN(y2_axis_max)) max_sec.y = y2_axis_max;
            if (!double.IsNaN(y2_axis_min)) min_sec.y = y2_axis_min;

            // Check that our series are beautifully coloured
            for (int i_series = 0; i_series < series_list.Count; ++i_series)
            {
                Series series = (Series)series_list[i_series];
                if (series.color == Color.Black)
                {
                    series.color = ChartColours.getOrderedColour(i_series);
                }
            }

            // Draw the various bits of the chart
            try
            {
                performPaintBackground(e.Graphics, region_chart);
                performPaintTitle(e.Graphics, region_title);
                performPaintLegend(e.Graphics, region_legend);

                if (region_chart.Height > 0 && region_chart.Width > 0)
                {
                    performPaintXAxis(e.Graphics, region_chart, region_x_axis, min_pri, max_pri);
                    performPaintY1Axis(e.Graphics, region_chart, region_y1_axis, min_pri, max_pri);
                    performPaintY2Axis(e.Graphics, region_chart, region_y2_axis, min_sec, max_sec);
                    performPaintSeries(e.Graphics, region_chart, min_pri, max_pri, min_sec, max_sec);
                }

                performPaintBorders(e.Graphics, region_chart);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }

        private void getSeriesExtents_EnsureNoZeroExtent(ref double min, ref double max)
        {
            if (min == max)
            {
                if (0 == min)
                {
                    min = -1;
                    max = +1;
                }
                else
                {
                    min *= 0.9;
                    max *= 1.1;
                }
            }

            if (Double.IsNaN(min))
            {
                min = -1;
                max = +1;
            }
        }

        private void getSeriesExtents(out Point2D min_pri, out Point2D max_pri, out Point2D min_sec, out Point2D max_sec)
        {
            min_pri = new Point2D(Double.NaN, Double.NaN);
            max_pri = new Point2D(Double.NaN, Double.NaN);
            min_sec = new Point2D(Double.NaN, Double.NaN);
            max_sec = new Point2D(Double.NaN, Double.NaN);

            IEnumerator i = series_list.GetEnumerator();
            while (i.MoveNext())
            {
                Series series = (Series)i.Current;
                Point2D current_min; Point2D current_max;
                series.getMinMax(out current_min, out current_max);

                // Do the x-axis
                if (Double.IsNaN(min_pri.x) || current_min.x < min_pri.x) min_pri.x = current_min.x;
                if (Double.IsNaN(max_pri.x) || current_max.x > max_pri.x) max_pri.x = current_max.x;
                if (Double.IsNaN(min_sec.x) || current_min.x < min_sec.x) min_sec.x = current_min.x;
                if (Double.IsNaN(max_sec.x) || current_max.x > max_sec.x) max_sec.x = current_max.x;

                // Do the first y-axis
                if (Double.IsNaN(min_pri.y) || series.chartaxis == ChartAxis.Primary && current_min.y < min_pri.y) min_pri.y = current_min.y;
                if (Double.IsNaN(max_pri.y) || series.chartaxis == ChartAxis.Primary && current_max.y > max_pri.y) max_pri.y = current_max.y;
                if (Double.IsNaN(min_sec.y) || series.chartaxis == ChartAxis.Secondary && current_min.y < min_sec.y) min_sec.y = current_min.y;
                if (Double.IsNaN(max_sec.y) || series.chartaxis == ChartAxis.Secondary && current_max.y > max_sec.y) max_sec.y = current_max.y;
            }

            // Check that we have at least a small range to work with
            getSeriesExtents_EnsureNoZeroExtent(ref min_pri.x, ref max_pri.x);
            getSeriesExtents_EnsureNoZeroExtent(ref min_pri.y, ref max_pri.y);
            getSeriesExtents_EnsureNoZeroExtent(ref min_sec.x, ref max_sec.x);
            getSeriesExtents_EnsureNoZeroExtent(ref min_sec.y, ref max_sec.y);
        }

        private void performPaintBackground(Graphics g, ChartRegion region_chart)
        {
            g.FillRectangle(brush_background, region_chart.Left, region_chart.Top, region_chart.Width, region_chart.Height);
        }

        private void performPaintTitle(Graphics g, ChartRegion region_title)
        {
            SizeF title_text_size = g.MeasureString(title, font_title);
            g.DrawString(title, font_title, brush_title, (float)(region_title.Left + (region_title.Width - title_text_size.Width) / 2.0), (float)(region_title.Top + (region_title.Height - title_text_size.Height) / 2.0));
        }

        private void performPaintLegend(Graphics g, ChartRegion region_legend)
        {
            if (0 == region_legend.Height)
            {
                return;
            }

            double cumulative_legend_text_width = 0.0;

            // Resize the font of the legend as more series are added
            int legendFontSize = 14 - series_list.Count / 2;
            legendFontSize = Math.Max(legendFontSize, 4);
            if (font_legend.Size != legendFontSize)
            {
                font_legend = new Font(FontFamily.GenericSansSerif, legendFontSize);
            }

            // Count the space for the text labels
            SizeF[] measurements = new SizeF[series_list.Count];
            for (int i = 0; i < series_list.Count; ++i)
            {
                Series series = (Series)series_list[i];
                measurements[i] = g.MeasureString(series.name, font_legend);
                cumulative_legend_text_width += measurements[i].Width;
            }

            // Add in the space for the gaps and the indicators
            cumulative_legend_text_width += legend_gap_width * (series_list.Count - 1);
            cumulative_legend_text_width += legend_indicator_size * series_list.Count;
            cumulative_legend_text_width += legend_indicator_gap_width * series_list.Count;

            double left_offset = region_legend.Left + (region_legend.Width - cumulative_legend_text_width) / 2;

            // Now write out the labels
            double current_offset = left_offset;
            for (int i = 0; i < series_list.Count; ++i)
            {
                Series series = (Series)series_list[i];

                brush_legend.Color = series.color;
                pen_legend.Color = series.color;

                double indicator_top = region_legend.Top + (region_legend.Height - legend_indicator_size) / 2.0;
                double text_top = region_legend.Top + (region_legend.Height - measurements[i].Height) / 2.0;

                renderSeriesLegend(i, series, g, pen_legend, brush_legend, (float)(current_offset + legend_indicator_size / 2.0), (float)(indicator_top + legend_indicator_size / 2.0), (float)legend_indicator_size);
                current_offset += legend_indicator_size;
                current_offset += legend_indicator_gap_width;
                g.DrawString(series.name, font_legend, brush_legend, (float)current_offset, (float)text_top);
                current_offset += measurements[i].Width;
                current_offset += legend_gap_width;
            }
        }

        private void performPaintBorders(Graphics g, ChartRegion region_chart)
        {
            g.DrawRectangle(pen_border, region_chart.Left, region_chart.Top, region_chart.Width, region_chart.Height);
        }

        private bool testAndDivide(int divisor, ref int tick_divisor)
        {
            if (tick_divisor % divisor == 0)
            {
                tick_divisor /= divisor;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void performPaintXAxis(Graphics g, ChartRegion region_chart, ChartRegion region_x_axis, Point2D min, Point2D max)
        {
            if (0 == series_list.Count)
            {
                return;
            }

            double x_extent = region_x_axis.Width;
            int tick_divisor = determineTickDivisor(x_extent, axis_tick_gap_min);

            // Calculate the space between ticks
            double notch_spacing = (x_extent / tick_divisor);

            // Draw in the ticks
            for (double x_pos = region_x_axis.Left; x_pos <= region_x_axis.Right && notch_spacing > 0; x_pos += notch_spacing)
            {
                g.DrawLine(pen_axis_guide, (float)x_pos, region_chart.Top, (float)x_pos, region_chart.Bottom);
                g.DrawLine(pen_axis, (float)x_pos, region_x_axis.Top, (float)x_pos, region_x_axis.Top + axis_notch_height);
            }

            // Draw in the values
            int precision = Precision.estimateMeaningfulChartRoundingPrecision(max.x - min.x);
            for (double x_pos = region_x_axis.Left; x_pos <= region_x_axis.Right && notch_spacing > 0; x_pos += notch_spacing)
            {
                double x_value = min.x + (max.x - min.x) * (x_pos - region_x_axis.Left) / region_x_axis.Width;
                x_value = Math.Round(x_value, precision);
                string x_text = "" + x_value;
                SizeF x_text_size = g.MeasureString(x_text, font_axis);
                g.DrawString(x_text, font_axis, brush_axis, (float)(x_pos - x_text_size.Width / 2.0), region_x_axis.Top + 5.0f);
            }

            // Draw in the axis title
            SizeF x_axis_title_text_size = g.MeasureString(x_axis_title, font_axis);
            g.DrawString(x_axis_title, font_axis, brush_axis, (float)(region_x_axis.Left + (region_x_axis.Width - x_axis_title_text_size.Width) / 2.0), (float)(region_x_axis.Bottom - x_axis_title_text_size.Height));
        }

        private bool haveSeriesForChartAxis(ChartAxis chartaxis)
        {
            for (int i = 0; i < series_list.Count; ++i)
            {
                Series series = (Series)series_list[i];
                if (series.chartaxis == chartaxis) return true;
            }
            return false;
        }

        private int determineTickDivisor(double y_extent, double axis_tick_gap_min)
        {
            if (y_extent / 10 > axis_tick_gap_min) return 10;
            if (y_extent / 5 > axis_tick_gap_min) return 5;
            if (y_extent / 4 > axis_tick_gap_min) return 4;
            if (y_extent / 3 > axis_tick_gap_min) return 3;
            if (y_extent / 2 > axis_tick_gap_min) return 2;

            return 1;
        }

        private void performPaintY1Axis(Graphics g, ChartRegion region_chart, ChartRegion region_y1_axis, Point2D min, Point2D max)
        {
            // Check that we have at least one series of our kind
            if (!haveSeriesForChartAxis(ChartAxis.Primary))
            {
                return;
            }

            // Draw in the ticks
            double y_extent = region_y1_axis.Height;
            int tick_divisor = determineTickDivisor(y_extent, axis_tick_gap_min);

            // Calculate the space between ticks
            double notch_spacing = (y_extent / tick_divisor);

            // Draw in the ticks
            int parity = -1;
            for (double y_pos = region_y1_axis.Top; y_pos <= region_y1_axis.Bottom && notch_spacing > 0; y_pos += notch_spacing)
            {
                parity = -parity;
                if (parity > 0 && y_pos < region_y1_axis.Bottom)
                {
                    g.FillRectangle(brush_background_axisguide, region_chart.Left, (float)y_pos, region_chart.Width, (float)notch_spacing);
                }

                g.DrawLine(pen_axis_guide, region_chart.Left, (float)y_pos, region_chart.Right, (float)y_pos);
                g.DrawLine(pen_axis, region_y1_axis.Right - axis_notch_height, (float)y_pos, region_y1_axis.Right, (float)y_pos);
            }

            // Used to make text vertical
            using (StringFormat string_format = new StringFormat(StringFormatFlags.DirectionVertical))
            {
                // Draw in the values
                int precision = Precision.estimateMeaningfulChartRoundingPrecision(max.y - min.y);
                for (double y_pos = region_y1_axis.Top; y_pos <= region_y1_axis.Bottom && notch_spacing > 0; y_pos += notch_spacing)
                {
                    double y_value = max.y - (max.y - min.y) * (y_pos - region_y1_axis.Top) / region_y1_axis.Height;
                    y_value = Math.Round(y_value, precision);
                    string y_text = "" + y_value;
                    SizeF y_text_size = g.MeasureString(y_text, font_axis, (int)region_y1_axis.Width, string_format);
                    g.DrawString(y_text, font_axis, brush_axis, region_y1_axis.Right - 5.0f - y_text_size.Width, (float)(y_pos - y_text_size.Height / 2.0), string_format);
                }

                // Draw in the title
                SizeF y1_axis_title_text_size = g.MeasureString(y1_axis_title, font_axis, (int)region_y1_axis.Width, string_format);
                g.DrawString(y1_axis_title, font_axis, brush_axis, 0, (float)(region_y1_axis.Top + (region_y1_axis.Height - y1_axis_title_text_size.Height) / 2.0), string_format);
            }
        }

        private void performPaintY2Axis(Graphics g, ChartRegion region_chart, ChartRegion region_y2_axis, Point2D min, Point2D max)
        {
            // Check that we have at least one series of our kind
            if (!haveSeriesForChartAxis(ChartAxis.Secondary))
            {
                return;
            }

            // Draw in the ticks
            double y_extent = region_y2_axis.Height;
            int tick_divisor = determineTickDivisor(y_extent, axis_tick_gap_min);

            // Calculate the space between ticks
            double notch_spacing = (y_extent / tick_divisor);

            // Draw in the ticks
            for (double y_pos = region_y2_axis.Top; y_pos <= region_y2_axis.Bottom && notch_spacing > 0; y_pos += notch_spacing)
            {
                g.DrawLine(pen_axis_guide, region_chart.Left, (float)y_pos, region_chart.Right, (float)y_pos);
                g.DrawLine(pen_axis, region_y2_axis.Left, (float)y_pos, region_y2_axis.Left + axis_notch_height, (float)y_pos);
            }

            // Used to make text vertical
            using (StringFormat string_format = new StringFormat(StringFormatFlags.DirectionVertical))
            {
                // Draw in the values
                int precision = Precision.estimateMeaningfulChartRoundingPrecision(max.y - min.y);
                for (double y_pos = region_y2_axis.Top; y_pos <= region_y2_axis.Bottom && notch_spacing > 0; y_pos += notch_spacing)
                {
                    double y_value = max.y - (max.y - min.y) * (y_pos - region_y2_axis.Top) / region_y2_axis.Height;
                    y_value = Math.Round(y_value, precision);
                    string y_text = "" + y_value;
                    SizeF y_text_size = g.MeasureString(y_text, font_axis, (int)region_y2_axis.Width, string_format);
                    g.DrawString(y_text, font_axis, brush_axis, region_y2_axis.Left + 5.0f, (float)(y_pos - y_text_size.Height / 2.0), string_format);
                }

                // Draw in the title
                SizeF y2_axis_title_text_size = g.MeasureString(y2_axis_title, font_axis, (int)region_y2_axis.Width, string_format);
                g.DrawString(y2_axis_title, font_axis, brush_axis, region_y2_axis.Right - y2_axis_title_text_size.Width, (float)(region_y2_axis.Top + (region_y2_axis.Height - y2_axis_title_text_size.Height) / 2.0), string_format);
            }
        }

        private void performPaintSeries(Graphics g, ChartRegion region_chart, Point2D min_pri, Point2D max_pri, Point2D min_sec, Point2D max_sec)
        {
            if (0 == series_list.Count)
            {
                return;
            }

            for (int i_series = 0; i_series < series_list.Count; ++i_series)
            {
                Series series = (Series)series_list[i_series];

                Point2D min = series.chartaxis == ChartAxis.Primary ? min_pri : min_sec;
                Point2D max = series.chartaxis == ChartAxis.Primary ? max_pri : max_sec;

                // Skip any series with bad values
                if (Double.IsNaN(min.x)) continue;
                if (Double.IsNaN(min.y)) continue;
                if (Double.IsNaN(max.x)) continue;
                if (Double.IsNaN(max.y)) continue;

                pen_series.Color = series.color;
                brush_series.Color = series.color;

                pen_series.DashStyle = pickSeriesDashStyle(i_series, series.charttype);

                if (series.charttype == ChartType.Point)
                {
                    for (int i_point = 0; i_point < series.Count; ++i_point)
                    {
                        Point2D point = (Point2D)series[i_point];

                        float x_pos = (float)(region_chart.Left + region_chart.Width * (point.x - min.x) / (max.x - min.x));
                        float y_pos = (float)(region_chart.Bottom - region_chart.Height * (point.y - min.y) / (max.y - min.y));
                        renderSeriesPoint(i_series, g, pen_series, brush_series, x_pos, y_pos, point_size);
                    }
                }

                else if (series.charttype == ChartType.Line || series.charttype == ChartType.SmoothLine || series.charttype == ChartType.LineAndPoint || series.charttype == ChartType.SmoothLineAndPoint)
                {
                    Point[] points = new Point[series.Count];
                    for (int i = 0; i < series.Count; ++i)
                    {
                        Point2D point = (Point2D)series[i];

                        float x_pos = (float)(region_chart.Left + region_chart.Width * (point.x - min.x) / (max.x - min.x));
                        float y_pos = (float)(region_chart.Bottom - region_chart.Height * (point.y - min.y) / (max.y - min.y));

                        if (series.charttype == ChartType.LineAndPoint || series.charttype == ChartType.SmoothLineAndPoint)
                        {
                            renderSeriesPoint(i_series, g, pen_series, brush_series, x_pos, y_pos, point_size);
                        }

                        points[i] = new Point((int)x_pos, (int)y_pos);
                    }

                    if (series.charttype == ChartType.Line || series.charttype == ChartType.LineAndPoint)
                    {
                        if (points.Length > 1)
                        {
                            g.DrawLines(pen_series, points);
                        }
                    }
                    else if (series.charttype == ChartType.SmoothLine || series.charttype == ChartType.SmoothLineAndPoint)
                    {
                        if (points.Length > 1)
                        {
                            g.DrawCurve(pen_series, points);
                        }
                    }
                }

                else if (series.charttype == ChartType.Bar)
                {
                    for (int i = 0; i < series.Count; ++i)
                    {
                        Point2D point = (Point2D)series[i];

                        float x_pos = (float)(region_chart.Left + region_chart.Width * (point.x - min.x) / (max.x - min.x));
                        float y_pos = (float)(region_chart.Bottom - region_chart.Height * (point.y - min.y) / (max.y - min.y));

                        g.DrawLine(pen_series, x_pos, y_pos, x_pos, region_chart.Bottom);
                    }
                }

                else
                {
                    throw new GenericException("Unknown chart type " + series.charttype);
                }

            }
        }

        private DashStyle pickSeriesDashStyle(int i, ChartType charttype)
        {
            if (charttype == ChartType.Point || charttype == ChartType.LineAndPoint || charttype == ChartType.SmoothLineAndPoint || charttype == ChartType.Bar)
            {
                return DashStyle.Solid;
            }

            switch (i % 5)
            {
                case 0:
                    return DashStyle.Solid;
                case 1:
                    return DashStyle.Dash;
                case 2:
                    return DashStyle.Dot;
                case 3:
                    return DashStyle.DashDot;
                case 4:
                    return DashStyle.DashDotDot;
                default:
                    return DashStyle.Solid;
            }
        }

        private void renderSeriesLegend(int i, Series series, Graphics g, Pen pen_series, Brush brush_series, float x_pos, float y_pos, float point_size)
        {
            // If it requires a line, do it
            if (series.charttype == ChartType.Line || series.charttype == ChartType.SmoothLine || series.charttype == ChartType.LineAndPoint || series.charttype == ChartType.SmoothLineAndPoint)
            {
                pen_series.DashStyle = pickSeriesDashStyle(i, series.charttype);
                g.DrawLine(pen_series, x_pos - point_size / 2 - 5, y_pos, x_pos + point_size / 2 + 5, y_pos);
            }

            // If it requires a point, draw the point
            if (series.charttype == ChartType.Point || series.charttype == ChartType.LineAndPoint || series.charttype == ChartType.SmoothLineAndPoint)
            {
                renderSeriesPoint(i, g, pen_series, brush_series, x_pos, y_pos, point_size);
            }

            if (series.charttype == ChartType.Bar)
            {
                pen_series.DashStyle = pickSeriesDashStyle(i, series.charttype);
                g.DrawLine(pen_series, x_pos, y_pos - point_size / 2 - 5, x_pos, y_pos + point_size / 2 + 5);
            }
        }

        private void renderSeriesPoint(int i, Graphics g, Pen pen_series, Brush brush_series, float x_pos, float y_pos, float point_size)
        {
            switch (i % 5)
            {
                case 0:
                    // The circle
                    g.DrawEllipse(pen_series, x_pos - point_size / 2, y_pos - point_size / 2, point_size, point_size);
                    break;
                case 1:
                    // The square
                    g.DrawRectangle(pen_series, x_pos - point_size / 2, y_pos - point_size / 2, point_size, point_size);
                    break;
                case 2:
                    // The x
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos - point_size / 2, x_pos + point_size / 2, y_pos + point_size / 2);
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos + point_size / 2, x_pos + point_size / 2, y_pos - point_size / 2);
                    break;
                case 3:
                    // The +
                    g.DrawLine(pen_series, x_pos, y_pos - point_size / 2, x_pos, y_pos + point_size / 2);
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos, x_pos + point_size / 2, y_pos);
                    break;
                case 4:
                    // The *
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos - point_size / 2, x_pos + point_size / 2, y_pos + point_size / 2);
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos + point_size / 2, x_pos + point_size / 2, y_pos - point_size / 2);
                    g.DrawLine(pen_series, x_pos, y_pos - point_size / 2, x_pos, y_pos + point_size / 2);
                    g.DrawLine(pen_series, x_pos - point_size / 2, y_pos, x_pos + point_size / 2, y_pos);
                    break;
                default:
                    throw new GenericException("The renderer selector does not match the number of available point renderers.");
            }
        }

        public void clearSeries()
        {
            series_list.Clear();
            if (!suspendingRefresh) Refresh();
        }

        public void addSeries(Series series)
        {
            series_list.Add(series);
            if (!suspendingRefresh) Refresh();
        }

        public void SuspendRefresh()
        {
            base.SuspendLayout();
            suspendingRefresh = true;
        }

        public void ResumeRefresh()
        {
            base.ResumeLayout();
            suspendingRefresh = false;
            Refresh();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
		Series doSampleChartSeries(int series_number)
		{
			RandomAugmented random = RandomAugmented.getSeededRandomAugmented();

			Point2D[] points = new Point2D[25];
			for (int i = 0; i < points.Length; ++i)
			{
				//				points[i] = new Point2D((i+1)/1.0, random.NextDouble());
				points[i] = new Point2D(Math.PI * i / (points.Length-1), series_number * Math.Sin(series_number * Math.PI * i / (points.Length-1)));
			}
			
			Series series = new Series("Series #" + series_number, ChartType.Line, points);
			switch (series_number % 5)
			{
				case 0:
					series.charttype = ChartType.Point;
					break;
				case 1:
					series.charttype = ChartType.Line;
					break;
				case 2:
					series.charttype = ChartType.SmoothLine;
					break;
				case 3:
					series.charttype = ChartType.LineAndPoint;
					break;
				case 4:
					series.chartaxis = ChartAxis.Secondary;
					series.charttype = ChartType.SmoothLineAndPoint;
					break;
				default:
					break;
			}

			return series;
		}
		
		void doSampleChart()
		{
			title = "Sample chart!";
			x_axis_title = "Position in portfolio";
			y1_axis_title = "Primary y axis title";
			y2_axis_title = "Secondary y axis title";
			for (int i = 0; i < 5; ++i)
			{
				addSeries(doSampleChartSeries(i));
			}
		}

		public static void TestHarness()
		{
			MultiChart2D chart = new MultiChart2D();
			chart.doSampleChart();
			SingleControlForm form = new SingleControlForm();
			form.setControl(chart);
			form.ShowDialog();
		}
#endif

        #endregion

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MultiChart2D
            // 
            Name = "MultiChart2D";
            Size = new Size(450, 368);
            ResumeLayout(false);

        }
    }
}
