using Utilities.GUI;

#if !HAS_MUPDF_PAGE_RENDERER
namespace Utilities.PDF.Sorax
{
    public class SoraxPDFRenderer
    {
        static private SoraxPDFRendererCache cache = new SoraxPDFRendererCache();

        // ------------------------------------------------------------------------------------------------------------------------

        static public byte[] GetPageByHeightAsImage(string filename, string pdf_user_password, int page, double height)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (cache)
            {
                // l1_clk.LockPerfTimerStop();
                byte[] bitmap = cache.Get(filename, page, height);
                if (null == bitmap)
                {
                    bitmap = SoraxPDFRendererDLLWrapper.GetPageByHeightAsImage(filename, pdf_user_password, pdf_user_password, page, height);
                    cache.Put(filename, page, height, bitmap);
                }
                return bitmap;
            }
        }

#if false
        static public byte[] GetPageByDPIAsImage(string filename, string pdf_user_password, int page, float dpi)
        {
            return SoraxPDFRendererDLLWrapper.GetPageByDPIAsImage(filename, pdf_user_password, pdf_user_password, page, dpi);
        }
#endif

        static public void Flush()
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (cache)
            {
                // l1_clk.LockPerfTimerStop();
                cache.Flush();
            }
        }
    }
}
#endif
