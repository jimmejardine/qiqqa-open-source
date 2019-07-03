using System;
using System.Collections.Generic;
using System.Drawing;
using Utilities;

namespace QiqqaOCR
{
    public class PDFRegionLocator
    {
        public enum SegmentState
        {
            TOP,
            BLANKS,
            PIXELS,
            BOTTOM
        }

        public class Region
        {
            public int y;
            public SegmentState state;

            public override string ToString()
            {
                return String.Format("{0}:{1}", y, state);
            }
        }


        public Bitmap bitmap;
        public List<Region> regions;
        public int width_x;

        public PDFRegionLocator(Bitmap bitmap)
        {
            this.bitmap = bitmap;
            GetRegions(this.bitmap, out regions, out width_x);
        }

        private static void GetRegions_FULLPAGE(Bitmap bitmap, out List<Region> regions, out int width_x)
        {
            Logging.Info("Height is {0}", bitmap.Height);
            regions = new List<Region>();
            regions.Add(new Region { y = 0, state = SegmentState.TOP });
            regions.Add(new Region { y = 0, state = SegmentState.PIXELS });
            regions.Add(new Region { y = bitmap.Height, state = SegmentState.BOTTOM });
            width_x = 0;
        }

        static int ScanTillCompletelyBlankRow(Bitmap bitmap, int direction, int start_y)
        {
            int bitmap_height = bitmap.Height;
            int bitmap_width = bitmap.Width;

            int last_good_y = start_y;

            int white_in_a_row_count = 0;

            int gutter_width = (int)(bitmap_width * 0.05);
            
            while (true)
            {
                int test_y = last_good_y + direction;
                
                // Test that we havent gone off the page
                if (0 > test_y) break;
                if (bitmap_height <= test_y) break;

                // Check the entire row
                bool have_dark_pixel = false;
                for (int x = gutter_width; x < bitmap_width - gutter_width; ++x)
                {
                    Color color = bitmap.GetPixel(x, test_y);
                    if (IsBelowWhitenessThreshold(color))
                    {
                        have_dark_pixel = true;
                        white_in_a_row_count = 0;
                        break;
                    }
                }

                // If we have an entirely white row, this is where we exit
                if (!have_dark_pixel)
                {
                    ++white_in_a_row_count;
                    if (white_in_a_row_count > 3)
                    {
                        break;
                    }
                } 

                last_good_y = test_y;
            }

            return last_good_y;
        }

        static bool IsBelowWhitenessThreshold(Color color)
        {
            int THRESHOLD = 250;
            return (color.R <= THRESHOLD || color.G <= THRESHOLD || color.B <= THRESHOLD);
        }
        
        private static void GetRegions(Bitmap bitmap, out List<Region> regions, out int width_x)
        {
            Logging.Info("Getting regions");

            width_x = bitmap.Width * 10 / 4 / 210;

            int mid_x = bitmap.Width / 2;
            int[] y_counts = new int[bitmap.Height];

            // Tally up the number of off-white pixels down the centre of the page
            // In fact, at the moment, just scan till one non-white is found - so count will be 0 or 1.
            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = mid_x - width_x / 2; x < mid_x + width_x / 2; ++x)
                {
                    Color color = bitmap.GetPixel(x, y);
                    
                    if (IsBelowWhitenessThreshold(color))
                    {
                        ++y_counts[y];
                        continue;
                    }

                }
            }

            regions = new List<Region>();            
            regions.Add(new Region { y = 0, state = SegmentState.TOP });

            // Now break them into segments
            SegmentState state = SegmentState.TOP;
            for (int y = 0; y < bitmap.Height; ++y)
            {
                if (0 == y_counts[y] && SegmentState.BLANKS != state)
                {
                    state = SegmentState.BLANKS;
                    regions.Add(new Region { y = y, state = state });
                    continue;
                }
                if (0 != y_counts[y] && SegmentState.PIXELS != state)
                {
                    state = SegmentState.PIXELS;
                    regions.Add(new Region { y = y, state = state });
                    continue;
                }
            }

            // Add the bottom region
            regions.Add(new Region { y = bitmap.Height, state = SegmentState.BOTTOM });





            // Get rid of any blank segments that are too small
            if (true)
            {
                const int TOO_SMALL_BLANKS_ROW_GAP = 50;
                for (int i = 0; i < regions.Count - 1; ++i)
                {
                    if (regions[i].state == SegmentState.BLANKS && (regions[i + 1].y - regions[i].y) < TOO_SMALL_BLANKS_ROW_GAP)
                    {
                        regions[i].state = SegmentState.PIXELS;
                    }
                }
                for (int i = 0; i < regions.Count - 1; ++i)
                {
                    while (i + 1 < regions.Count && regions[i].state == regions[i + 1].state)
                    {
                        regions.RemoveAt(i + 1);
                    }
                }
            }

            // Now pad out the text ones to give them some space to work
            if (true)
            {
                for (int i = 2; i < regions.Count; ++i)
                {
                    if (regions[i].state == SegmentState.PIXELS)
                    {
                        regions[i].y = ScanTillCompletelyBlankRow(bitmap, -1, regions[i].y);
                    }
                    if (regions[i].state == SegmentState.BLANKS)
                    {
                        regions[i].y = ScanTillCompletelyBlankRow(bitmap, +1, regions[i].y);
                    }
                }
            }

            // If there are too many regions, treat the whole page as a single entity by deleting all detected regions
            if (regions.Count > 30)
            {
                regions.Clear();
                regions.Add(new Region { y = 0, state = SegmentState.TOP });
                regions.Add(new Region { y = 0, state = SegmentState.PIXELS });
                regions.Add(new Region { y = bitmap.Height, state = SegmentState.BOTTOM });
            }

            Logging.Info("Got {0} regions", regions.Count);
        }
    }
}
