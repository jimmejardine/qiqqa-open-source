using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;
using Qiqqa.Documents.PDF.PDFControls.Page;
using Utilities;
using Utilities.BibTex.Parsing;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls
{
    public class PDFRendererControlStats
    {
        internal PDFRendererControl pdf_renderer_control;
        internal PDFDocument pdf_document;

        public double zoom_factor = 1.0;
        public bool are_colours_inverted;

        public double largest_page_image_width = PDFRendererPageControl.BASIC_PAGE_WIDTH;
        public double largest_page_image_height = PDFRendererPageControl.BASIC_PAGE_HEIGHT;

        public readonly double DPI;

        private int start_page_offset = -1;
        public int StartPageOffset
        {
            get
            {
                EnsureStartPageOffset();
                return start_page_offset;
            }
        }

        public PDFRendererControlStats(PDFRendererControl pdf_renderer_control, PDFDocument pdf_document)
        {
            this.pdf_renderer_control = pdf_renderer_control;
            this.pdf_document = pdf_document;

            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                DPI = graphics.DpiY / 96.0;
            }
        }

        #region --- Background rendering of resized page images ------------------------------------------------------------------------------------------------------------------------

        public delegate void ResizedPageImageItemCallbackDelegate(BitmapSource image_page, int requested_height, int requested_width);

        private class ResizedPageImageItemRequest
        {
            internal int page;
            internal PDFRendererPageControl page_control;
            internal int height;
            internal int width;
            internal ResizedPageImageItemCallbackDelegate callback;
        }

        private Dictionary<int, ResizedPageImageItemRequest> resized_page_image_item_requests = new Dictionary<int, ResizedPageImageItemRequest>();
        private List<int> resized_page_image_item_request_orders = new List<int>();
        private int num_resized_page_image_item_thread_running = 0;

        public void GetResizedPageImage(PDFRendererPageControl page_control, int page, int height, int width, ResizedPageImageItemCallbackDelegate callback)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (resized_page_image_item_requests)
            {
                // l1_clk.LockPerfTimerStop();
                Logging.Debug("Queueing page redraw for {0}", page);
                resized_page_image_item_requests[page] = new ResizedPageImageItemRequest
                {
                    page = page,
                    page_control = page_control,
                    height = height,
                    width = width,
                    callback = callback
                };

                resized_page_image_item_request_orders.Add(page);

                if (num_resized_page_image_item_thread_running < 1)
                {
                    Interlocked.Increment(ref num_resized_page_image_item_thread_running);
                    ResizedPageImageItemThreadEntry();
                }
            }
        }

        private void ResizedPageImageItemThreadEntry()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            while (true)
            {
                ResizedPageImageItemRequest resized_page_image_item_request = null;

                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (resized_page_image_item_requests)
                {
                    // l1_clk.LockPerfTimerStop();
                    // If there is nothing more to do...
                    if (0 == resized_page_image_item_request_orders.Count)
                    {
                        Interlocked.Decrement(ref num_resized_page_image_item_thread_running);
                        break;
                    }

                    // Get a piece of work
                    int page = resized_page_image_item_request_orders[resized_page_image_item_request_orders.Count - 1];
                    resized_page_image_item_request_orders.RemoveAt(resized_page_image_item_request_orders.Count - 1);

                    if (resized_page_image_item_requests.TryGetValue(page, out resized_page_image_item_request))
                    {
                        resized_page_image_item_requests.Remove(page);
                    }
                    else
                    {
                        continue;
                    }
                }

                Logging.Debug("Performing page redraw for {0}", resized_page_image_item_request.page);

                // Check that the page is still visible
                if (!resized_page_image_item_request.page_control.PageIsInView)
                {
                    continue;
                }

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

                    try
                    {
                        //PngBitmapDecoder decoder = new PngBitmapDecoder(new MemoryStream(pdf_document.PDFRenderer.GetPageByHeightAsImage(resized_page_image_item_request.page, resized_page_image_item_request.height)), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        //BitmapSource bitmap = decoder.Frames[0];
                        //bitmap.Freeze();

                        BitmapImage bitmap = new BitmapImage();
                        using (MemoryStream ms = new MemoryStream(pdf_document.PDFRenderer.GetPageByHeightAsImage(resized_page_image_item_request.page, resized_page_image_item_request.height, resized_page_image_item_request.width)))
                        {
                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();
                        }

                        if (null != bitmap)
                        {
                            resized_page_image_item_request.callback(bitmap, resized_page_image_item_request.height, resized_page_image_item_request.width);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an error while resizing a PDF page image");
                    }
                });
            }
        }

        #endregion

        private void EnsureStartPageOffset()
        {
            try
            {
                // -1 means we are not initialised
                if (-1 == start_page_offset)
                {
                    // 0 means that there is no known offset
                    start_page_offset = 0;

                    BibTexItem bibtex_item = pdf_document.BibTexItem;
                    if (null != bibtex_item)
                    {
                        string start_page_offset_text = null;
                        if (String.IsNullOrEmpty(start_page_offset_text) && bibtex_item.ContainsField("page")) start_page_offset_text = bibtex_item["page"];
                        if (String.IsNullOrEmpty(start_page_offset_text) && bibtex_item.ContainsField("pages")) start_page_offset_text = bibtex_item["pages"];

                        if (!String.IsNullOrEmpty(start_page_offset_text))
                        {
                            MatchCollection matches = Regex.Matches(start_page_offset_text, @"(\d+).*");
                            if (0 < matches.Count && 1 < matches[0].Groups.Count)
                            {
                                string start_page_offset_string = matches[0].Groups[1].Value;
                                start_page_offset = Convert.ToInt32(start_page_offset_string);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem calculating start_page_offset");
            }
        }
    }
}
