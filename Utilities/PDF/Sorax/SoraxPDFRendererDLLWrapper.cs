using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using Utilities.GUI;
using Utilities.PDF.MuPDF;

namespace Utilities.PDF.Sorax
{
    public class SoraxPDFRendererDLLWrapper
    {
        static SoraxPDFRendererDLLWrapper()
        {
            Logging.Debug特("+Initialising SoraxPDFRendererDLLWrapper");
            string config_filename = Path.GetFullPath(Path.Combine(UnitTestDetector.StartupDirectoryForQiqqa, @"SPdf.ini"));
            Logging.Debug特("-Initialising SoraxPDFRendererDLLWrapper");
        }

        // ------------------------------------------------------------------------------------------------------------------------

        public static int GetPageCount(string filename, string pdf_user_password, string pdf_owner_password)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            throw new ApplicationException("Not supported yet");
        }

        public static byte[] GetPageByHeightAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, int height, int width)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            return GetPageByDPIAsImage_LOCK(filename, pdf_user_password, pdf_owner_password, page, dpi: 0, height, width);
        }


        public static byte[] GetPageByDPIAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, int dpi)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            return GetPageByDPIAsImage_LOCK(filename, pdf_user_password, pdf_owner_password, page, dpi, 0, 0);
        }

        private static byte[] GetPageByDPIAsImage_LOCK(string filename, string pdf_user_password, string pdf_owner_password, int page, int dpi, int height, int width)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                // sample command (PNG written to stdout for page #2, width and height are limiting/reducing, dpi-resolution is driving):
                //
                //      mudraw -q -o - -F png -r 600 -w 1920 -h 1280 G:\Qiqqa\evil\Guest\documents\1\1A9760F3917A107AC46E6E292B9C839364F09E73.pdf  2
                var img = MuPDFRenderer.RenderPDFPageAsByteArray(filename, page, dpi, height, width, pdf_owner_password, ProcessPriorityClass.BelowNormal);

                return img;
            }
            catch (Exception ex)
            {
                throw new GenericException(ex, $"PDF Render: Error while rasterising page {page} at {dpi}dpi / {height}x{width} pixels of '{filename}'");
            }
        }
    }
}
