using System;
using System.Windows.Media;

namespace Utilities.GUI
{
    public class WPFFrameRate
    {
        public static readonly WPFFrameRate Instance = new WPFFrameRate();

        DateTime last_time = DateTime.UtcNow;
        int update_count = 0;
        public int total_update_count = 0;
        public double fps = 0;
        
        private WPFFrameRate()
        {
            last_time = DateTime.UtcNow;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        static readonly int MAX_UPDATE_COUNT = 3;
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            ++update_count;
            ++total_update_count;

            if (MAX_UPDATE_COUNT == update_count)
            {
                update_count = 0;

                DateTime now_time = DateTime.UtcNow;
                double elapsed_time_ms = now_time.Subtract(last_time).TotalMilliseconds;
                last_time = now_time;

                double now_fps = MAX_UPDATE_COUNT * 1000.0 / elapsed_time_ms;

                if (0 < elapsed_time_ms)
                {
                    fps = 0.75 * fps + 0.25 * now_fps;
                }


            }
        }
    }
}
