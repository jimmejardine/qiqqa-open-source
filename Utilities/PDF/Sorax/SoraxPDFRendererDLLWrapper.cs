using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Threading;
using Utilities.GUI;

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
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            throw new ApplicationException("Not supported yet");
        }

        public static byte[] GetPageByHeightAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, double height, double width)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                return GetPageByDPIAsImage_LOCK(filename, pdf_user_password, pdf_owner_password, page, dpi:0, height, width);
        }


        public static byte[] GetPageByDPIAsImage(string filename, string pdf_user_password, string pdf_owner_password, int page, float dpi)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

                return GetPageByDPIAsImage_LOCK(filename, pdf_user_password, pdf_owner_password, page, dpi);
        }

        private static byte[] GetPageByDPIAsImage_LOCK(string filename, string pdf_user_password, string pdf_owner_password, int page, float dpi, double height = 0, double width = 0)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            try
            {
                // sample command (PNG written to stdout for page #2, width and height are limiting/reducing, dpi-resolution is driving):
                //
                //      mudraw -q -o - -F png -r 600 -w 1920 -h 1280 G:\Qiqqa\evil\Guest\documents\1\1A9760F3917A107AC46E6E292B9C839364F09E73.pdf  2

                using (FileStream fs = new FileStream(@"C:\temp\aax.png", FileMode.Open, FileAccess.Read))
                {
                    MemoryStream ms = new MemoryStream();
                    fs.CopyTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new GenericException(ex, $"Error while rasterising page {page} at {dpi}dpi / {height}x{width} pixels of '{filename}'");
            }
        }
    }
}
