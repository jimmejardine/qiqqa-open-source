using System;
using System.Diagnostics;
using System.Windows.Media;

namespace Utilities.GUI
{
    public class WPFFrameRate
    {
        public static readonly WPFFrameRate Instance = new WPFFrameRate();
        private Stopwatch clk;
        private long last_clk_tick;
        private int update_count = 0;
        public int total_update_count = 0;
        public double fps = 0;

        private WPFFrameRate()
        {
            clk = Stopwatch.StartNew();
            last_clk_tick = clk.ElapsedTicks;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private static readonly int MAX_UPDATE_COUNT = 3;

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                ++update_count;
                ++total_update_count;

                if (MAX_UPDATE_COUNT <= update_count)
                {
                    long now_tick = clk.ElapsedTicks;
                    long elapsed_time = now_tick - this.last_clk_tick;
                    this.last_clk_tick = now_tick;

                    if (elapsed_time > 0)
                    {
                        double now_fps = update_count * (double)Stopwatch.Frequency / elapsed_time;

                        fps = 0.75 * fps + 0.25 * now_fps;

                        update_count = 0;
                    }
                }
            });
        }
    }
}
